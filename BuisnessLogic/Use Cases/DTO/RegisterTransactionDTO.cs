using BuisnessLogic.Entities;
using BuisnessLogic.Enums;

namespace BuisnessLogic.Use_Cases.DTO
{
    public class RegisterTransactionDTO
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; } = 0;
        public string Description { get; set; } = String.Empty;
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }
        public Guid AccountId { get; set; }
        public Guid? CategoryId { get;  set; }
        public Guid? FamilyMemberId { get; set; }
    }
}
