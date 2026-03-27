using BuisnessLogic.Enums;

namespace BuisnessLogic.Entities
{
    public class Debt
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = String.Empty;

        public decimal StartAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal InterestRate { get; set; } = 0;
        public decimal CapitalisationsPerYear { get; set; } = 12;
        public decimal FixedAddition { get; set; } = 0;
        public DebtType Type { get; set; }
        public InterestType InterestType { get; set; }


        public DateTime StartDate { get; set; }
        public DateTime LastPaidDate { get; set; }
        public DateTime EndDate { get; set; }

        public Guid? CategoryId { get; set; } = null;
        public Category? Category { get; set; } = null;

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public void ChangeTotalAmount(decimal newAmount)
        {
            TotalAmount = newAmount;
        }
        public void MakeAPayment(decimal amount, DateTime date)
        {
            if (amount < 0)
            {
                throw new Exception("Negative Amount");
            }

            if (PaidAmount + amount > TotalAmount)
            {
                throw new Exception("Debt is OverPaing");
            }

            PaidAmount += amount;
            LastPaidDate = date;
        }
    }
}
