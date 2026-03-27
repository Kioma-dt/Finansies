using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface ITransactionTagRepository
    {
        Task<TransactionTag> GetById(Guid id);
        Task Add(TransactionTag transactionTag);
        Task Update(TransactionTag transactionTag);
    }
}
