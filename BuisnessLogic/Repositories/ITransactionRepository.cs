using System.Transactions;

namespace BuisnessLogic.Repositories
{
    public interface ITransactionRepository
    {
        Task Add(BuisnessLogic.Entities.Transaction transaction);
        Task Update(BuisnessLogic.Entities.Transaction transaction);
    }

}
