namespace BuisnessLogic.Use_Cases.DTO
{
    public class AddTagToTransactionDTO
    {
        public Guid TransactionId { get; set; }
        public Guid TransactionTagId { get; set; }

    }
}
