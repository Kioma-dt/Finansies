using BuisnessLogic.Entities;
using BuisnessLogic.Enums;

namespace BuisnessLogic.Use_Cases.DTO
{
    public class PlanTransactionDTO
    {
        public decimal Amount { get; set; } = 0;
        public string Description { get; set; } = String.Empty;
        public TransactionType Type { get; set; }

        public DateTime PlannedDate { get; set; }

        public Guid AccountId { get; set; }

        public Guid? CategoryId { get; set; }

        public Guid? FamilyMemberId { get; set; }
    }
}
