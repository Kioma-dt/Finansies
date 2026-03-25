using BuisnessLogic.Entities;

namespace BuisnessLogic.Filters
{
    public interface IFilter
    {
        bool Apply(Transaction transaction);
    }
}
