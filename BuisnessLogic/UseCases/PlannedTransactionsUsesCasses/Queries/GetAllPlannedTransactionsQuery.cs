using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Queries
{
    public sealed record GetAllPlannedTransactionsQuery(Guid UserId)
        : IRequest<IEnumerable<PlannedTransaction>>;

    public class GetAllPlannedTransactionsQueryHandler(IPlannedTransactionRepository plannedTransactionRepository)
        : IRequestHandler<GetAllPlannedTransactionsQuery, IEnumerable<PlannedTransaction>>
    {
        public async Task<IEnumerable<PlannedTransaction>> Handle(GetAllPlannedTransactionsQuery request, CancellationToken cancellationToken)
        {
            return await plannedTransactionRepository.GetAll(request.UserId,
                x => x.Account,
                x => x.Category,
                x => x.FamilyMember,
                x => x.TransactionTags,
                x => x.Debt,
                x => x.User);
        }
    }
}
