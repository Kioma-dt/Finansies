using BuisnessLogic.Enums;

namespace BuisnessLogic.DTO
{
    public class DebtCreateDTO(string Name,
    decimal Amount,
    DebtType Type,
    DateTime StartDate,
    DateTime EndDate,
    Guid? CategoryId,
    Guid? FamilyMemberId,
    decimal CapitalisatonsPerYear,
    InterestType InterestType,
    decimal InterestRate,
    decimal FixedAddition,
    bool IsAutoPlanned,
    uint PaymentsPerYear
    )
    {
        public string Name { get; } = Name;
        public decimal Amount { get; } = Amount;
        public DebtType Type { get; } = Type;
        public DateTime StartDate { get; } = StartDate;
        public DateTime EndDate { get; } = EndDate;
        public Guid? CategoryId { get; } = CategoryId;
        public Guid? FamilyMemberId { get; } = FamilyMemberId;
        public decimal CapitalisatonsPerYear { get; } = CapitalisatonsPerYear;
        public InterestType InterestType { get; } = InterestType;
        public decimal InterestRate { get; } = InterestRate;
        public decimal FixedAddition { get; } = FixedAddition;
        public bool IsAutoPlanned { get; } = IsAutoPlanned;
        public uint PaymentsPerYear { get; } = PaymentsPerYear;
    }
}
