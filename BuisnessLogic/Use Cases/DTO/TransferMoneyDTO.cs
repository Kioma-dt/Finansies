using BuisnessLogic.Entities;

namespace BuisnessLogic.Use_Cases.DTO
{
    public class TransferMoneyDTO
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = String.Empty;
        public DateTime Date { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public Guid UserId { get; set; }
    }
}
