using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IDebtRepository
    {
        Task Add(Debt debt);
        Task<Debt> GetById(Guid id);
        Task Update(Debt debt);
    }

}
