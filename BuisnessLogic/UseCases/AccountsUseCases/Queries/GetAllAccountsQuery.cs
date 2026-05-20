using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;

namespace BuisnessLogic.UseCases.AccountsUseCases.Queries
{
    public sealed record GetAllAccountsQuery(Guid userId)
        : IRequest<IEnumerable<Account>>;

    public class GetAllAccountsQueryHandler(IAccountRepository accountRepository)
        : IRequestHandler<GetAllAccountsQuery, IEnumerable<Account>>
    {
        public async Task<IEnumerable<Account>> Handle(GetAllAccountsQuery request, 
            CancellationToken cancellationToken)
        {
            return (await accountRepository.GetAll(request.userId,
                x => x.Parent,
                x => x.Children,
                x => x.Transactions,
                x => x.PlannedTransactions,
                x => x.TransfersFrom,
                x => x.TransfersTo,
                x => x.FamilyMember,
                x => x.User));
        }
    }
}
