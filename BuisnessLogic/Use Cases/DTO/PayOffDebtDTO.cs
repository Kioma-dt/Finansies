namespace BuisnessLogic.Use_Cases.DTO
{
    public class PayOffDebtDTO
    {
        public Guid DebtId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date {  get; set; }
    }
}
