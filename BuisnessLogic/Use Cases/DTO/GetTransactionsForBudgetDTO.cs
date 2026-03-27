namespace BuisnessLogic.Use_Cases.DTO
{
    public class GetTransactionsForBudgetDTO
    {
        public Guid TransactionId { get; set; }
        public Guid BudgetId { get; set; }
    }
}
