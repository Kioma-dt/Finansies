using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;

namespace BuisnessLogic.UseCases.TransactionsUseCasses.Commands
{
    public sealed record AddFamilyMemberToTransactionCommand(Guid UserId,
        Guid TransactionId,
        Guid FamilyMemberId)
        : IRequest;

    public class AddFamilyMemberToTransactionCommandHandler(ITransactionRepository transactionRepository,
        IFamilyMemberRepository familyMemberRepository)
        : IRequestHandler<AddFamilyMemberToTransactionCommand>
    {
        public async Task Handle(AddFamilyMemberToTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await transactionRepository.GetById(request.UserId, request.TransactionId);

            if (transaction is null)
            {
                throw new ArgumentException($"No Transaction with Id: {request.TransactionId}");
            }

            var familyMember = await familyMemberRepository.GetById(request.UserId, request.FamilyMemberId);

            if (familyMember is null)
            {
                throw new ArgumentException($"No Family Member with Id: {request.FamilyMemberId}");
            }

            transaction.FamilyMemberId = request.FamilyMemberId;

            //familyMember.Transactions.Add(transaction);

            await transactionRepository.Update(transaction);
        }
    }
}
