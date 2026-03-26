using BuisnessLogic.Enums;

namespace BuisnessLogic.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public decimal Amount { get; private set; } = 0;
        public string Description { get; private set; } = String.Empty;
        public DateTime Date { get; private set; }
        public TransactionType Type { get; private set; }

        public Guid AccountId { get; private set; }
        public Account? Account { get; private set; } = null;

        public Guid? CategoryId { get; private set; } = null;
        public Category? Category { get; private set; } = null;

        public List<TransactionTag> TransactionTags { get; } = new();

        public Guid? FamilyMemberId { get; private set; } = null;
        public FamilyMember? FamilyMember { get; private set; } = null;
    }
}
