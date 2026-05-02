namespace UI.Messages
{
    public class DateRangeChangedMessage
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public DateRangeChangedMessage(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate;
            EndDate = endDate;
        }

    }
}
