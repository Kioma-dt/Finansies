using DataAccess.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IPlannedTransactionRepository
    {
        Task<PlannedTransaction> GetById(Guid userId, Guid id);
        Task Add(PlannedTransaction plannedTransaction);
        Task Update(PlannedTransaction plannedTransaction);
    }
}
