using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IDebtRepository
    {
        Task<List<Debt>> GetAll(Guid userId);
        Task<List<Debt>> GetAllScalar(Guid userId);
        Task Add(Debt debt);
        Task<Debt?> GetById(Guid userId, Guid id);
        Task Update(Debt debt);
    }

}
