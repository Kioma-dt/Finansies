using BuisnessLogic.Enums;

namespace BuisnessLogic.Entities
{
    public class Debt
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = String.Empty;

        public decimal TotalAmount { get; private set; }
        public decimal RemainingAmount { get; private set; }
        public double InterestRate { get; private set; } = 0;
        public DebtType Type { get; private set; }

        public DateTime StartDate { get; private set; }
        public DateTime LastPaidDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public Guid? CategoryId { get; private set; } = null;
        public Category? Category { get; private set; } = null;

        public Guid UserId { get; private set; }
        public User? User { get; private set; }
    }
}
