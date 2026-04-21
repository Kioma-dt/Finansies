namespace DataAccess.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;

        public List<Account> Accounts { get; set; } = new();
        public List<Budget> Budgets { get; set; } = new();
        public List<Category> Categories { get; set; } = new();
        public List<Debt> Debts { get; set; } = new();
        public List<FamilyMember> FamilyMembers { get; set; } = new();
        public List<Transaction> Transactions { get; set; } = new();
        public List<PlannedTransaction> PlannedTransactions { get; set; } = new();
        public List<TransactionTag> TransactionTags { get; set; } = new();
        public List<Transfer> Transfers { get; set; } = new();
    }
}
