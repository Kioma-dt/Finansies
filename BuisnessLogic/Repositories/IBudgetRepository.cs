using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IBudgetRepository
    {
        Task Add(Budget budget);
        Task<Budget?> GetById(Guid usserId, Guid id);
        Task AddBudgetFilter(Guid userId, Guid id, BudgetFilter filter);
        Task Update(Budget budget);
    }
}
