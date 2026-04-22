using DataAccess.Entities;
using DataAccess.Enums;
using System.Dynamic;
using System.Linq.Expressions;


namespace BuisnessLogic.BudgetService
{
    public interface IBudgetSpecificationsCreatorsProvider
    {
        IBudgetSpecificationsCreator Get(BudgetFilterType type);
    }
    public class BudgetSpecificationsCreatorsProvider : IBudgetSpecificationsCreatorsProvider
    {
        readonly Dictionary<BudgetFilterType, IBudgetSpecificationsCreator> _creators;

        public BudgetSpecificationsCreatorsProvider(IEnumerable<IBudgetSpecificationsCreator> creators)
        {
            _creators = creators.ToDictionary(c => c.Type);
        }

        public IBudgetSpecificationsCreator Get(BudgetFilterType type)
        {
            if (!_creators.TryGetValue(type, out var creator))
            {
                throw new ArgumentException($"No Creator for filter type: {type}");
            }
                
            return creator;
        }
    }
}
