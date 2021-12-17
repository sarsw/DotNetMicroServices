namespace CommandsService.EventProcessing
{
    public interface IEventProcessor
    {
        void ProcessEvent(string message); // event string from  eventlistener service & decide what to do with it!
    }
}
 