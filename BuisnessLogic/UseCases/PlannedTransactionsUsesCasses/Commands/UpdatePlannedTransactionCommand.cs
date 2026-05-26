using BuisnessLogic.Enums;
using MediatR;
using BuisnessLogic.Repositories;

namespace BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands
{
    public sealed record UpdatePlannedTransactionCommand(Guid UserId,
            Guid Id,
            decimal Amount,
            string Description,
            TransactionType Type,
            DateTime PlannedDate,
            Guid? AccountId,
            Guid? CategoryId,
            Guid? FamilyMemberId,
            Guid? DebtId)
        : IRequest;

    public class UpdatePlannedTransactionCommandHandler(IPlannedTransactionRepository plannedTransactionRepository)
        : IRequestHandler<UpdatePlannedTransactionCommand>
    {
        public async Task Handle(UpdatePlannedTransactionCommand request, CancellationToken cancellationToken)
        {
            var plannedTransaction = await plannedTransactionRepository.GetById(request.UserId, request.Id);
            if (plannedTransaction is null)
            {
                throw new ArgumentException($"No Planned Transaction with Id: {request.Id}");
            }
            plannedTransaction.Amount = request.Amount;
            plannedTransaction.Description = request.Description;
            plannedTransaction.Type = request.Type;
            plannedTransaction.PlannedDate = request.PlannedDate;
            plannedTransaction.AccountId = request.AccountId;
            plannedTransaction.CategoryId = request.CategoryId;
            plannedTransaction.FamilyMemberId = request.FamilyMemberId;
            plannedTransaction.DebtId = request.DebtId;
            await plannedTransactionRepository.Update(plannedTransaction);
        }
    }
}
