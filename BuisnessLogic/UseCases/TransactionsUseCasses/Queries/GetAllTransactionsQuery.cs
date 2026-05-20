using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.UseCases.TransactionsUseCasses.Queries
{
    public sealed record GetAllTransactionsQuery(Guid UserId)
        : IRequest<IEnumerable<Transaction>>;

    public class GetAllTransactionsQueryHandler(ITransactionRepository transactionRepository)
        : IRequestHandler<GetAllTransactionsQuery, IEnumerable<Transaction>>
    {
        public async Task<IEnumerable<Transaction>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
        {
            return await transactionRepository.GetAll(request.UserId,
                x => x.Account,
                x => x.Category,
                x => x.FamilyMember,
                x => x.TransactionTags,
                x => x.Debt,
                x => x.User);
        }
    }
}
