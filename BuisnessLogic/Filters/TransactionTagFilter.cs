using BuisnessLogic.Entities;

namespace BuisnessLogic.Filters
{
    public class TransactionTagFilter : IFilter
    {
        Guid _transactionTagId;

        public TransactionTagFilter(Guid transactionTagId)
        {
            _transactionTagId = transactionTagId;
        }
        public bool Apply(Transaction transaction)
        {
            return transaction.TransactionTagId == _transactionTagId;
        }
    }
}
