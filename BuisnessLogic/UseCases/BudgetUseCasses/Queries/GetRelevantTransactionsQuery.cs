using BuisnessLogic.BudgetService;
using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.UseCases.BudgetUseCasses.Queries
{
    public sealed record GetRelevantTransactionsQuery(Guid UserId, Guid BudgetId)
        : IRequest<IEnumerable<Transaction>>;

    public class GetRelevantTransactionsQueryHandler(IBudgetRepository budgetRepository,
        ITransactionRepository transactionRepository,
        IBudgetSpecificationsExtender budgetSpecificationsExtender)
        : IRequestHandler<GetRelevantTransactionsQuery, IEnumerable<Transaction>>
    {
        public async Task<IEnumerable<Transaction>> Handle(GetRelevantTransactionsQuery request, CancellationToken cancellationToken)
        {
            var budget = await budgetRepository.GetById(request.UserId, request.BudgetId, x => x.Filters);

            if (budget is null)
            {
                throw new ArgumentException($"No Budget with Id: {request.BudgetId}");
            }

            var specification = budgetSpecificationsExtender.GetFullExpression(budget);

            return await transactionRepository.GetWithSpecification(request.UserId, specification);
        }
    }
}
