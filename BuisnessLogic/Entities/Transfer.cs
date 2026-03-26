namespace BuisnessLogic.Entities
{
    public class Transfer
    {
        public Guid Id { get; private set; }
        public decimal Amount { get; private set; } = 0;
        public string Description { get; private set; } = String.Empty;
        public DateTime Date { get; private set; }

        public Guid FromAccountId { get; private set; }
        public Account? FromAccount { get; private set; }

        public Guid ToAccountId { get; private set; }
        public Account? ToAccount { get; private set; }

        public Guid UserId { get; private set;}
        public User? User { get; private set; }
    }
}
