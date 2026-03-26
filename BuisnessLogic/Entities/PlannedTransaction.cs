using BuisnessLogic.Enums;

namespace BuisnessLogic.Entities
{
    public class PlannedTransaction
    {
        public Guid Id { get; private set; }
        public decimal Amount { get; private set; } = 0;
        public string Description { get; private set; } = String.Empty;
        public TransactionType Type { get; private set; }

        public DateTime PlannedDate { get; private set; }
        public PlannedTransactionStatus Status { get; private set; } = PlannedTransactionStatus.Planned;

        public Guid AccountId { get; private set; }
        public Account? Account { get; private set; }

        public Guid? CategoryId { get; private set; } = null;
        public Category? Category { get; private set; } = null;

        public List<TransactionTag> TransactionTags { get; } = new();

        public Guid? FamilyMemberId { get; private set; } = null;
        public FamilyMember? FamilyMember { get; private set; } = null;
    }
}
