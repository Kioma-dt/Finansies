using MediatR;
using BuisnessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisnessLogic.Repositories;

namespace BuisnessLogic.UseCases.DebtsUseCasses.Queries
{
    public sealed record GetAllDebtsQuery(Guid UserId)
        : IRequest<IEnumerable<Debt>>;

    public class GetAllDebtsQueryHandler(IDebtRepository debtRepository)
        : IRequestHandler<GetAllDebtsQuery, IEnumerable<Debt>>
    {
        public async Task<IEnumerable<Debt>> Handle(GetAllDebtsQuery request, CancellationToken cancellationToken)
        {
            return await debtRepository.GetAll(request.UserId,
            x => x.FamilyMember,
            x => x.Category,
            x => x.PlannedTransactions,
            x => x.Transactions);
        }
    }
}
