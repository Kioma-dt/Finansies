namespace BuisnessLogic.Entities
{
    public class Account
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = String.Empty;
        public decimal Balance { get; private set; }
        public Account? Parent { get; private set; } = null;

        public List<Transaction> Transactions { get; } = new();

        public Guid? FamilyMemberId { get; private set; } = null;
        public FamilyMember? FamilyMember { get; private set; } = null;

        public Guid UserId {  get; private set; }
        public User? User { get; private set; }

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
