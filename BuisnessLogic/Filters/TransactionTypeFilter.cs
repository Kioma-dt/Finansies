using DataAccess.Entities;
using DataAccess.Enums;

namespace DataAccess.Filters
{
    public class TransactionTypeFilter : IFilter
    {
        TransactionType _transactionType;

        public TransactionTypeFilter(TransactionType transactionType)
        {
            _transactionType = transactionType;
        }
        public bool Apply(Transaction transaction)
        {
            return transaction.Type == _transactionType;
        }
    }
}
