using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;

namespace BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands
{
    public sealed record AddFamilyMemberToPlannedTransactionCommand(Guid UserId,
        Guid PlannedTransactionId,
        Guid FamilyMemberId)
        : IRequest;

    public class AddFamilyMemberToPlannedTransactionCommandHandler(IPlannedTransactionRepository plannedTransactionRepository,
        IFamilyMemberRepository familyMemberRepository)
        : IRequestHandler<AddFamilyMemberToPlannedTransactionCommand>
    {
        public async Task Handle(AddFamilyMemberToPlannedTransactionCommand request, CancellationToken cancellationToken)
        {
            var plannedTransaction = await plannedTransactionRepository.GetById(request.UserId, request.PlannedTransactionId);

            if (plannedTransaction is null)
            {
                throw new ArgumentException($"No Planned Transaction with Id: {request.PlannedTransactionId}");
            }


            var familyMember = await familyMemberRepository.GetById(request.UserId, request.FamilyMemberId);

            if (familyMember is null)
            {
                throw new ArgumentException($"No Family Member with Id: {request.FamilyMemberId}");
            }

            plannedTransaction.FamilyMemberId = request.FamilyMemberId;

            familyMember.PlannedTransactions.Add(plannedTransaction);

            await plannedTransactionRepository.Update(plannedTransaction);
        }
    }
}
