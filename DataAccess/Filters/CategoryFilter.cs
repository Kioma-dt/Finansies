using DataAccess.Entities;

namespace DataAccess.Filters
{
    public class CategoryFilter : IFilter
    {
        Guid _categoryId;

        public CategoryFilter(Guid categoryId)
        {
            _categoryId = categoryId;
        }
        public bool Apply(Transaction transaction)
        {
            return transaction.CategoryId == _categoryId;
        }
    }
}
