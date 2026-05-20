using BuisnessLogic.Entities;
using System.Linq.Expressions;
namespace BuisnessLogic.Repositories
{
    public interface ITransactionRepository
        : IRepository<Transaction>
    {
        Task<List<Transaction>> GetWithSpecification(Guid userId, Expression<Func<Transaction, bool>> specification);
    }

}
