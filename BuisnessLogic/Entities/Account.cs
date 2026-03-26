namespace BuisnessLogic.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public decimal Balance { get; set; }
        public Account? Parent { get; set; } = null;

        public List<Transaction> Transactions { get; } = new();

        public Guid? FamilyMemberId { get; set; } = null;
        public FamilyMember? FamilyMember { get; set; } = null;

        public Guid UserId {  get; set; }
        public User? User { get; set; }

        public void AddToBalance(decimal amount)
        {
            Balance += amount;
        }
        public void RemoveFromBalance(decimal amount) 
        {
            if (Balance < amount)
            {
                throw new Exception("Not Enough Money!");
            }

            Balance -= amount;
        }
    }
}
