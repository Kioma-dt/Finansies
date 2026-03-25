namespace BuisnessLogic.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public decimal Balance { get; set; }
        public Account? Parent { get; set; } = null;

        public List<Transaction> Transactions { get; set; } = new();

        public Guid FamilyMemberId { get; set; }
        public FamilyMember? FamilyMember { get; set; } = null;

        public Guid UserId {  get; set; }
        public User? User { get; set; }
    }
}
