namespace BuisnessLogic.Entities
{
    public class TransactionTag
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;

        public List<Transaction> Transactions { get; set; } = new();
        public List<PlannedTransaction> PlannedTransactions { get; set; } = new();

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
