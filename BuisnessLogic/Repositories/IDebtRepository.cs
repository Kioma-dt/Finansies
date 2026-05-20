using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IDebtRepository
        : IRepository<Debt>
    {
        //Task PayOffDebt(Guid userId, Guid debtId, decimal amount, DateTime date);
    }

}
