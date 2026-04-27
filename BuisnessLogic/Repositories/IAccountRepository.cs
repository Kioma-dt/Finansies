using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IAccountRepository
    {
        Task Add(Account account);
        Task<Account?> GetById(Guid userId, Guid id);
        Task<List<Account>?> GetAll(Guid userId);
        Task Update(Account account);
    }
}
