namespace BuisnessLogic.Entities
{
    public class TransactionTag 
        : UsersEntity
    {
        public string Name { get; set; } = String.Empty;

        public List<Transaction> Transactions { get; set; } = new();
        public List<PlannedTransaction> PlannedTransactions { get; set; } = new();
    }
}
