namespace BuisnessLogic.Entities
{
    public class Category 
        : UsersEntity
    {
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;

        public Guid? ParentId { get; set; } = null;
        public Category? Parent { get; set; } = null;

        public List<Category> Children { get; set; } = new();

        public List<Transaction> Transactions { get; set; } = new();
        public List<PlannedTransaction> PlannedTransactions { get; set; } = new();
        public List<Debt> Debts { get; set; } = new();

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

        public decimal PeriodTransactionsSum(DateTime startDate, DateTime endDate)
        {
            var sum = 0m;

            foreach (var transaction in Transactions.Where(x => x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date))
            {
                sum += transaction.SignedAmount;
            }

            foreach (var child in Children)
            {
                sum += child.PeriodTransactionsSum(startDate, endDate);
            }

            return sum;
        }

        public int PeriodTransactionsNumber(DateTime startDate, DateTime endDate)
        {
            var count = Transactions.Where(x => x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date).Count();

            foreach (var child in Children)
            {
                count += child.PeriodTransactionsNumber(startDate, endDate);
            }
            return count;
        }
    }
}
