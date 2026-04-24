using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using System.Linq.Expressions;

namespace BuisnessLogic.BudgetService
{
    public interface IBudgetSpecificationsCreator
    {
        BudgetFilterType Type { get; }
        Expression Create(ParameterExpression parameter, BudgetFilter filter);
    }

    public class BudgetAccountSpecificationsCreator : IBudgetSpecificationsCreator
    {
        public BudgetFilterType Type => BudgetFilterType.Account;

        public Expression Create(ParameterExpression parameter, BudgetFilter filter)
        {
            return Expression.Equal(
                Expression.Property(parameter, nameof(Transaction.AccountId)),
                Expression.Constant(Guid.Parse(filter.Value))
            );
        }
    }

    public class BudgetCategorySpecificationsCreator : IBudgetSpecificationsCreator
    {
        public BudgetFilterType Type => BudgetFilterType.Category;

        public Expression Create(ParameterExpression parameter, BudgetFilter filter)
        {
            return Expression.Equal(
                Expression.Property(parameter, nameof(Transaction.CategoryId)),
                Expression.Constant(Guid.Parse(filter.Value))
                );
        }
    }

    public class BudgetFamilyMemberSpecificationsCreator : IBudgetSpecificationsCreator
    {
        public BudgetFilterType Type => BudgetFilterType.Category;

        public Expression Create(ParameterExpression parameter, BudgetFilter filter)
        {
            return Expression.Equal(
                Expression.Property(parameter, nameof(Transaction.FamilyMemberId)),
                Expression.Constant(Guid.Parse(filter.Value))
                );
        }
    }
    public class BudgetTransactionTagSpecificationsCreator : IBudgetSpecificationsCreator
    {
        public BudgetFilterType Type => BudgetFilterType.Category;

        public Expression Create(ParameterExpression parameter, BudgetFilter filter)
        {
            var tagId = Guid.Parse(filter.Value);

            var transactionTags = Expression.Property(
                parameter,
                nameof(Transaction.TransactionTags));

            var ttParam = Expression.Parameter(typeof(TransactionTag), "tt");

            var condition = Expression.Equal(
                Expression.Property(ttParam, nameof(TransactionTag.Id)),
                Expression.Constant(tagId)
            );

            var lambda = Expression.Lambda(condition, ttParam);

            var anyMethod = typeof(Enumerable)
                .GetMethods()
                .First(m => m.Name == "Any" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TransactionTag));

            return Expression.Call(
                anyMethod,
                transactionTags,
                lambda
            );
        }
    }

    public class BudgetTransactionTypeSpecificationsCreator : IBudgetSpecificationsCreator
    {
        public BudgetFilterType Type => BudgetFilterType.Category;

        public Expression Create(ParameterExpression parameter, BudgetFilter filter)
        {
            return Expression.Equal(
                Expression.Property(parameter, nameof(Transaction.Type)),
                Expression.Constant(Enum.Parse<TransactionType>(filter.Value))
                );
        }
    }
}
