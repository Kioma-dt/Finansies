using DataAccess.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IBudgetRepository
    {
        Task Add(Budget budget);
        Task<Budget> GetById(Guid usserId, Guid id);
        Task Update(Budget budget);
    }
}
