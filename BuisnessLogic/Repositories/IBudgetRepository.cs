using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IBudgetRepository
        : IRepository<Budget>
    {
        Task AddBudgetFilter(Guid userId, Guid id, BudgetFilter filter);
        //Task AddBudgetFilter(BudgetFilter filter);
    }
}
