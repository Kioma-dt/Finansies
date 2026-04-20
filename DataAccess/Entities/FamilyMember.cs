namespace DataAccess.Entities
{
    public class FamilyMember
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;

        public List<Account> Accounts { get; } = new();
        public List<Transaction> Transactions { get;} = new();

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
