using DataAccess.Entities;

namespace DataAccess.Filters
{
    public interface IFilter
    {
        bool Apply(Transaction transaction);
    }
}
