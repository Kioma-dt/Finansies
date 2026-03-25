using BuisnessLogic.Entities;

namespace BuisnessLogic.Filters
{
    public class AccounFilter : IFilter
    {
        Guid _accountId;

        public AccounFilter(Guid accountId)
        {
            _accountId = accountId;
        }
        public bool Apply(Transaction transaction)
        {
            return transaction.AccountId == _accountId;
        }
    }
}
