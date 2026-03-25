using BuisnessLogic.Enums;

namespace BuisnessLogic.Entities
{
    public class Debt
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;

        public decimal TotalAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public double InterestRate { get; set; }
        public DebtType Type { get; set; }

        public DateTime StartDate {  get; set; }
        public DateTime EndDate { get; set; }

        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
