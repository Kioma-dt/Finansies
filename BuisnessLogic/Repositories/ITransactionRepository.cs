using BuisnessLogic.Entities;
using System.Linq.Expressions;
namespace BuisnessLogic.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAll(Guid userId);
        Task<Transaction?> GetById(Guid userId, Guid id);
        Task<List<Transaction>> GetWithSpecification(Guid userId, Expression<Func<Transaction, bool>> specification);
        Task Add(Transaction transaction);
        Task Update(Transaction transaction);
    }

}
