namespace BuisnessLogic.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public Category? Parent { get; set; } = null;

        public List<Transaction> Transactions { get; } = new();
        public List<Debt> Debts { get; } = new();

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
