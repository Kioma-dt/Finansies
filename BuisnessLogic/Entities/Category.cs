namespace BuisnessLogic.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;

        public Guid? ParentId { get; set; } = null;
        public Category? Parent { get; set; } = null;

        public List<Category> Children { get; set; } = new();

        public List<Transaction> Transactions { get; set; } = new();
        public List<PlannedTransaction> PlannedTransactions { get; set; } = new();
        public List<Debt> Debts { get; set; } = new();

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public decimal TransactionsSum
        {
            get
            {
                var sum = 0m;

                foreach (var transaction in Transactions)
                {
                    sum += transaction.SignedAmount;
                }

                foreach (var child in Children)
                {
                    sum += child.TransactionsSum;
                }

                return sum;
            }
        }
    }
}
