namespace BuisnessLogic.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public decimal Balance { get; set; }

        public Guid? ParentId { get; set; } = null;
        public Account? Parent { get; set; } = null;

        public List<Account> Children { get; set; } = new();

        public List<Transaction> Transactions { get; set; } = new();
        public List<PlannedTransaction> PlannedTransactions { get; set; } = new();

        public List<Transfer> TransfersFrom { get; set; } = new();
        public List<Transfer> TransfersTo { get; set; } = new();

        public Guid? FamilyMemberId { get; set; } = null;
        public FamilyMember? FamilyMember { get; set; } = null;

        public Guid UserId {  get; set; }
        public User? User { get; set; }

        public decimal TotalBalance
        {
            get
            {
                var sum = Balance;

                foreach(var child in Children)
                {
                    sum += child.TotalBalance;
                }

                return sum;
            }
        }

        public void AddToBalance(decimal amount)
        {
            if(amount < 0)
            {
                throw new Exception("Amount is Negative!");
            }
            Balance += amount;
        }
        public void RemoveFromBalance(decimal amount) 
        {
            if (amount < 0)
            {
                throw new Exception("Amount is Negative!");
            }

            if (Balance < amount)
            {
                throw new Exception("Not Enough Money!");
            }

            Balance -= amount;
        }
    }
}
