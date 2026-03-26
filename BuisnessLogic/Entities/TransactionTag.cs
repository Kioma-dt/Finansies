namespace BuisnessLogic.Entities
{
    public class TransactionTag
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;

        public List<Transaction> Transactions { get;} = new();

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
