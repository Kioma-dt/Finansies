using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IPlannedTransactionRepository
    {
        Task<List<PlannedTransaction>> GetAll(Guid userId);
        Task<List<PlannedTransaction>> GetAllScalar(Guid userId);
        Task<PlannedTransaction?> GetById(Guid userId, Guid id);
        Task Add(PlannedTransaction plannedTransaction);
        Task Update(PlannedTransaction plannedTransaction);
    }
}
