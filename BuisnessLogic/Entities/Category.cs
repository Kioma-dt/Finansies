namespace BuisnessLogic.Entities
{
    public class Category
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = String.Empty;
        public string Description { get; private set; } = String.Empty;
        public Category? Parent { get; private set; } = null;

        public List<Transaction> Transactions { get; } = new();
        public List<Debt> Debts { get; } = new();

        public Guid UserId { get; private set; }
        public User? User { get; private set; }
    }
}
