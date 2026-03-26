using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IAccountRepository
    {
        Task Add(Account account);
        Task<Account> GetById (Guid id);
        Task Update(Account account);
    }

}
