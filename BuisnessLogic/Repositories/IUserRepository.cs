using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetById(Guid id);
        Task Add(User user);
        Task Update(User user);
    }
}
