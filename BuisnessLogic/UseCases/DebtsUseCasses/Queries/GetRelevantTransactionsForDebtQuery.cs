using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.UseCases.DebtsUseCasses.Queries
{
    public sealed record GetRelevantTransactionsForDebtQuery(Guid UserId,
        Guid DebtId)
        : IRequest<IEnumerable<Transaction>>;


public class GetRelevantTransactionsForDebtQueryHandler(IDebtRepository debtRepository)
        : IRequestHandler<GetRelevantTransactionsForDebtQuery, IEnumerable<Transaction>>
    {
        public async Task<IEnumerable<Transaction>> Handle(GetRelevantTransactionsForDebtQuery request, CancellationToken cancellationToken)
        {

            var debt = await debtRepository.GetById(request.UserId,
                request.DebtId,
                x => x.Transactions);

            if (debt is null)
            {
                throw new ArgumentException($"No Debt With Id: {request.DebtId}");
            }

            return debt.Transactions;
        }
    }
}
