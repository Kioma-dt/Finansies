using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BuisnessLogic.UseCases.FamilyMembersUseCasses.Commands
{
    public sealed record CreateFamilyMemberCommand(Guid UserId,
        string Name)
        : IRequest;

    public class CreateFamilyMemberCommandHandler(IFamilyMemberRepository familyMemberRepository)
        : IRequestHandler<CreateFamilyMemberCommand>
    {
        public async Task Handle(CreateFamilyMemberCommand request, CancellationToken cancellationToken)
        {
            await familyMemberRepository.Add(new FamilyMember()
            {
                Name = request.Name,
                UserId = request.UserId
            });
        }
    }


}
