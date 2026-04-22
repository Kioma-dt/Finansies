using DataAccess.Entities;
using DataAccess.Enums;
using System.Dynamic;
using System.Linq.Expressions;


namespace BuisnessLogic.BudgetService
{
    public interface IBudgetSpecificationsExtender
    {
        Expression<Func<Transaction, bool>> GetFullExpression(Budget budget);
    }
    public class BudgetSpecificationsExtender : IBudgetSpecificationsExtender
    {
        readonly IBudgetSpecificationsCreatorsProvider _creatorsProvider;

        public BudgetSpecificationsExtender(IBudgetSpecificationsCreatorsProvider creatorsProvider)
        {
            _creatorsProvider = creatorsProvider;
        }

        public Expression<Func<Transaction, bool>> GetFullExpression(Budget budget)
        {
            var parameter = Expression.Parameter(typeof(Transaction), "t");

            Expression baseFilter =
                Expression.AndAlso(
                    Expression.Equal(
                        Expression.Property(parameter, nameof(Transaction.UserId)),
                        Expression.Constant(budget.UserId)
                    ),

                    Expression.AndAlso(
                        Expression.GreaterThanOrEqual(
                            Expression.Property(parameter, nameof(Transaction.Date)),
                            Expression.Constant(budget.StartDate)
                        ),
                        Expression.LessThanOrEqual(
                            Expression.Property(parameter, nameof(Transaction.Date)),
                            Expression.Constant(budget.EndDate)
                        )
                    )
            );

            Expression? finalBody = null;
            var groups = budget.Filters.GroupBy(bf => bf.Type);

            foreach (var group in groups)
            {
                Expression? groupBody = null;

                foreach (var filter in group) 
                {
                    var creator = _creatorsProvider.Get(filter.Type);
                    var currentBody = creator.Create(parameter, filter);

                    groupBody = groupBody is null 
                        ? currentBody
                        : Expression.OrElse(groupBody, currentBody);
                }

                finalBody = finalBody is null
                    ? groupBody
                    : Expression.AndAlso(finalBody, groupBody!);
            }

            finalBody = finalBody == null
                ? baseFilter
                : Expression.AndAlso(baseFilter, finalBody);

            return Expression.Lambda<Func<Transaction, bool>>(finalBody!, parameter);
        }
    }
}
