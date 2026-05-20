namespace BuisnessLogic.Entities
{
    public class Transfer : Entity
    {
        public decimal Amount { get; set; } = 0;
        public string Description { get; set; } = String.Empty;
        public DateTime Date { get; set; }

        public Guid FromAccountId { get; set; }
        public Account? FromAccount { get; set; }

        public Guid ToAccountId { get; set; }
        public Account? ToAccount { get; set; }

        public Guid UserId { get; set;}
        public User? User { get; set; }
    }
}
