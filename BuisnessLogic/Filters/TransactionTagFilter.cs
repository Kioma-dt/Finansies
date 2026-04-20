using DataAccess.Entities;
using System.Linq;

namespace DataAccess.Filters
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
            return transaction
                .TransactionTags
                .Select(t => t.Id)
                .Contains(_transactionTagId);
        }
    }
}
