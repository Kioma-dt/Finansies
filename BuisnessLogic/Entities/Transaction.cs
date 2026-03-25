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
        public Account? Account { get; set; }

        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        public List<TransactionTag> TransactionTags { get; set; } = new();

        public Guid FamilyMemberId { get; set; }
        public FamilyMember? FamilyMember { get; set; }
    }
}
