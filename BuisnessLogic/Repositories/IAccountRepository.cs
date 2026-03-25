using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IAccountRepository
    {
        Task Add(Account account);
        Task<Account> GetById (int id);
        Task Update(Account account);
    }

}
