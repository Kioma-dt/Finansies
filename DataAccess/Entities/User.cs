namespace DataAccess.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;

        public List<Account> Accounts { get; } = new();
        public List<Budget> Budgets { get; } = new();
        public List<Category> Categories { get; } = new();
        public List<Debt> Debts { get; } = new();
        public List<FamilyMember> FamilyMembers { get; } = new();
        public List<TransactionTag> TransactionTags { get; } = new();
        public List<Transfer> Transfers { get; } = new();
    }
}
