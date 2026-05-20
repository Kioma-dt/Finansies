using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.UseCases.CategoryUseCasses.Queries
{
    public sealed record GetAllCategoriesQuery(Guid UserId)
        : IRequest<IEnumerable<Category>>;

    public class GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
        : IRequestHandler<GetAllCategoriesQuery, IEnumerable<Category>>
    {
        public async Task<IEnumerable<Category>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            return await categoryRepository.GetAll(request.UserId,
                    x => x.Parent,
                    x => x.Children,
                    x => x.Transactions,
                    x => x.PlannedTransactions,
                    x => x.Debts);
        }
    }
}
