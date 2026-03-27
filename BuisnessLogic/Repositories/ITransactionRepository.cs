using BuisnessLogic.Entities;
using BuisnessLogic.Filters;
namespace BuisnessLogic.Repositories
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<Transaction>> GetAll();
        Task<IEnumerable<Transaction>> GetWithFilters(IEnumerable<IFilter> filters);
        Task<IEnumerable<Transaction>> GetWithFilters(IFilter filter);
        Task Add(Transaction transaction);
        Task Update(Transaction transaction);
    }

}
