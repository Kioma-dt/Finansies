namespace BuisnessLogic.Entities
{
    public class FamilyMember 
        : UsersEntity
    {
        public string Name { get; set; } = String.Empty;

        public List<Account> Accounts { get; set; } = new();
        public List<Transaction> Transactions { get; set; } = new();
        public List<PlannedTransaction> PlannedTransactions { get; set; } = new();
        public List<Debt> Debts { get; set; } = new();

        public decimal PeriodTransactionsSum(DateTime startDate, DateTime endDate, Guid? accountId = null)
        {
            var sum = 0m;

            var transactions = Transactions;

            if (accountId is not null)
            {
                transactions = transactions.Where(x => x.AccountId == accountId).ToList();
            }

            foreach (var transaction in transactions.Where(x => x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date))
            {
                sum += transaction.SignedAmount;
            }

            return sum;
        }

        public int PeriodTransactionsNumber(DateTime startDate, DateTime endDate, Guid? accountId = null)
        {
            var transactions = Transactions;

            if (accountId is not null)
            {
                transactions = transactions.Where(x => x.AccountId == accountId).ToList();
            }

            return transactions.Where(x => x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date).Count();
        }
    }
}
