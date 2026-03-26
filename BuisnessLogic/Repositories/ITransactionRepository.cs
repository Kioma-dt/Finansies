using BuisnessLogic.Entities;
namespace BuisnessLogic.Repositories
{
    public interface ITransactionRepository
    {
        Task Add(Transaction transaction);
        Task Update(Transaction transaction);
    }

}
