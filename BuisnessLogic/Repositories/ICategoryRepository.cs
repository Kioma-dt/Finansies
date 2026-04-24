using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface ICategoryRepository
    {
        Task Add(Category category);
        Task<Category?> GetById(Guid userId, Guid id);
    }

}
