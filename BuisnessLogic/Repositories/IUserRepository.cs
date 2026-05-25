using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetById(Guid id);
        Task<User?> GetByName(string name); 
        Task Add(User user);
        Task Update(User user);
    }
}
