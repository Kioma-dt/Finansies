namespace UI.Messages
{
    public class CurrentTimeMessage
    {
        public DateTime CurrentTime { get; }

        public CurrentTimeMessage(DateTime currentTime)
        {
            CurrentTime = currentTime;
        }
        
    }
}
