using BuisnessLogic.Entities;

namespace BuisnessLogic.Repositories
{
    public interface ICategoryRepository
        : IRepository<Category>
    {
        Task SetParent(Guid userId, Guid id, Category parent);
    }

}
