using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface ICategoryRepository
    {
        Task Add(Category category);
        Task<Category?> GetById(Guid userId, Guid id);

        Task<List<Category>?> GetAll(Guid userId);

        Task<List<Category>?> GetAllScalar(Guid userId);
    }

}
