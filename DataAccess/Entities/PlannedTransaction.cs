using DataAccess.Enums;
using System.Diagnostics.Contracts;

namespace DataAccess.Entities
{
    public class PlannedTransaction
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; } = 0;
        public string Description { get; set; } = String.Empty;
        public TransactionType Type { get; set; }

        public DateTime PlannedDate { get; set; }
        public PlannedTransactionStatus Status { get; set; } = PlannedTransactionStatus.Planned;

        public Guid? AccountId { get; set; }
        public Account? Account { get; set; }

        public Guid? CategoryId { get; set; } = null;
        public Category? Category { get; set; } = null;

        public List<TransactionTag> TransactionTags { get; set; } = new();

        public Guid? FamilyMemberId { get; set; } = null;
        public FamilyMember? FamilyMember { get; set; } = null;

        public void Conirm()
        {
            Status = PlannedTransactionStatus.Confirmed;
        }

        public void Update(DateTime date)
        {
            if(Status != PlannedTransactionStatus.Confirmed && date >= PlannedDate)
            {
                Status = PlannedTransactionStatus.WaitingForConfirm;
            }
        }
    }
}
