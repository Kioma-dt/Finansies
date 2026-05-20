using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IBudgetRepository
        : IRepository<Budget>
    {
        Task AddBudgetFilter(Guid userId, BudgetFilter filter);
        //Task AddBudgetFilter(BudgetFilter filter);
    }
}
