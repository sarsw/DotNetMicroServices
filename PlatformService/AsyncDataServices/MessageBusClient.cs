using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _cfg;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration cfg)
        {
            _cfg = cfg;
            var factory = new ConnectionFactory() 
                { HostName = _cfg["RabbitMQHost"],
                    Port = int.Parse(_cfg["RabbitMQPort"]) };


            try {
                // RabbitMQ needs connection, channel and exchange
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdownHandler;
    
                Console.WriteLine("RabbitMQ Connected!");
            } catch (Exception e) {
                Console.WriteLine($"RabbitMQ connection error:{e.Message}");
            }
        }

        private void RabbitMQ_ConnectionShutdownHandler(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("RabbitMQ Shutdown");
        }

        public void PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
        {
            var msg = JsonSerializer.Serialize(platformPublishedDto);
            if (_connection.IsOpen)
            {
                Console.WriteLine("RabbitMQ IsOpen, Sending -->");
                SendMessage(msg);
            }
            else
            {
                Console.WriteLine("RabbitMQ NOT Connected!!");
            }
        }

        private void SendMessage(string msg)
        {
            var body = Encoding.UTF8.GetBytes(msg);
            _channel.BasicPublish(exchange:"trigger", routingKey: "", basicProperties: null, body: body);
            Console.WriteLine($"RabbitMQ Sent [{msg}]");
        }

        public void Displose()
        {
            Console.WriteLine("MessageBus disposed");
            if (_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }
    }
}