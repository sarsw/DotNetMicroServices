using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommandsService.EventProcessing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    // run as as background service listening for message events. We do it like this to avoid lifetime issues if run as a server service
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration cfg, IEventProcessor evtProc)
        {
            _configuration = cfg;
            _eventProcessor = evtProc;

            InitialiseRabbitMQ();       // setup the comms
        }

        private void InitialiseRabbitMQ()
        {
            var factory = new ConnectionFactory() {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName = _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: "");
            Console.WriteLine("Listening on the message bus...");

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("Shutdown message bus connection");
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }

            base.Dispose();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();   // make sure we stop if asked to do so

            var consumer = new EventingBasicConsumer(_channel);     // we are listening, not producing messages
            consumer.Received += (ModuleHandle, ea) =>              // message handler
            {
                Console.WriteLine("Event received on RabbitMQ");

                var body = ea.Body;
                var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
                _eventProcessor.ProcessEvent(notificationMessage);  // determine event and process
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);    // ensure we continue to receive messages
        
            return Task.CompletedTask;
        }
    }
}