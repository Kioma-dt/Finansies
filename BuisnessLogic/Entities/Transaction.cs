using BuisnessLogic.Enums;

namespace BuisnessLogic.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; } = 0;
        public string Description { get; set; } = String.Empty;
        public DateTime Date { get; set; }
        public TransactionType Type { get; set; }

        public Guid AccountId { get; set; }
        public Account? Account { get; set; } = null;

        public Guid? CategoryId { get; set; } = null;
        public Category? Category { get; set; } = null;

        public List<TransactionTag> TransactionTags { get; } = new();

        public Guid? FamilyMemberId { get; set; } = null;
        public FamilyMember? FamilyMember { get; set; } = null;
    }
}
