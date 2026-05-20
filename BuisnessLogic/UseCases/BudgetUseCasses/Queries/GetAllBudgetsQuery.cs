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
    public sealed record GetAllBudgetsQuery(Guid UserId)
        : IRequest<IEnumerable<Budget>>;

    public class GetAllBudgetsQueryHandler(IBudgetRepository budgetRepository)
        : IRequestHandler<GetAllBudgetsQuery, IEnumerable<Budget>>
    {
        public async Task<IEnumerable<Budget>> Handle(GetAllBudgetsQuery request, CancellationToken cancellationToken)
        {
            return (await budgetRepository.GetAll(request.UserId,
                x => x.Filters)).ToList();
        }
    }
}