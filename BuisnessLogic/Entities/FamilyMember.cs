namespace BuisnessLogic.Entities
{
    public class FamilyMember
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = String.Empty;

        public List<Account> Accounts { get; } = new();
        public List<Transaction> Transactions { get;} = new();

        public Guid UserId { get; private set; }
        public User? User { get; private set; }
    }
}
