namespace BuisnessLogic.Entities
{
    public class TransactionTag
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = String.Empty;

        public List<Transaction> Transactions { get;} = new();

        public Guid UserId { get; private set; }
        public User? User { get; private set; }
    }
}
