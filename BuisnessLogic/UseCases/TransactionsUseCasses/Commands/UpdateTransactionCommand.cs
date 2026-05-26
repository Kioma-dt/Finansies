using MediatR;
using BuisnessLogic.Repositories;

namespace BuisnessLogic.UseCases.TransactionsUseCasses.Commands
{
    public sealed record UpdateTransactionCommand(Guid UserId,
            Guid Id,
            string Description,
            DateTime Date,
            Guid? CategoryId,
            Guid? FamilyMemberId)
        : IRequest;

    public class UpdateTransactionCommandHandler(ITransactionRepository transactionRepository)
        : IRequestHandler<UpdateTransactionCommand>
    {
        public async Task Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await transactionRepository.GetById(request.UserId, request.Id);
            if (transaction is null)
            {
                throw new ArgumentException($"No Transaction with Id: {request.Id}");
            }
            transaction.Description = request.Description;
            transaction.Date = request.Date;
            transaction.CategoryId = request.CategoryId;
            transaction.FamilyMemberId = request.FamilyMemberId;
            await transactionRepository.Update(transaction);
        }
    }
}
