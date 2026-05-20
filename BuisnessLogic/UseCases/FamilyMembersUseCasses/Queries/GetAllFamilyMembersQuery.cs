using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.UseCases.FamilyMembersUseCasses.Queries
{
    public sealed record GetAllFamilyMembersQuery(Guid UserId)
        : IRequest<IEnumerable<FamilyMember>>;

    public class GetAllFamilyMembersQueryHandler(IFamilyMemberRepository familyMemberRepository)
        : IRequestHandler<GetAllFamilyMembersQuery, IEnumerable<FamilyMember>>
    {
        public async Task<IEnumerable<FamilyMember>> Handle(GetAllFamilyMembersQuery request, CancellationToken cancellationToken)
        {
            return await familyMemberRepository.GetAll(request.UserId,
                    x => x.Transactions,
                    x => x.PlannedTransactions,
                    x => x.Debts);
        }
    }
}
