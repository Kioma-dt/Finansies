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

        public decimal PeriodTransactionsSum(DateTime startDate, DateTime endDate)
        {
            var sum = 0m;

            foreach (var transaction in Transactions.Where(x => x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date))
            {
                sum += transaction.SignedAmount;
            }

            return sum;
        }

        public int PeriodTransactionsNumber(DateTime startDate, DateTime endDate)
        {
            return Transactions.Where(x => x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date).Count();
        }
    }
}
