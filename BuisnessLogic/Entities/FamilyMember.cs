namespace BuisnessLogic.Entities
{
    public class FamilyMember
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;

        public List<Account> Accounts { get; set; } = new();
        public List<Transaction> Transactions { get; set; } = new();
        public List<PlannedTransaction> PlannedTransactions { get; set; } = new();
        public List<Debt> Debts { get; set; } = new();

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
