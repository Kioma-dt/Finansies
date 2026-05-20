using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;

namespace BuisnessLogic.UseCases.PlannedTransactionsUsesCasses.Commands
{
    public sealed record AddCategoryToPlannedTransactionCommand(Guid UserId,
        Guid PlannedTransactionId,
        Guid CategoryId)
        : IRequest;

    public class AddCategoryToPlannedTransactionCommandHandler(IPlannedTransactionRepository plannedTransactionRepository,
        ICategoryRepository categoryRepository)
        : IRequestHandler<AddCategoryToPlannedTransactionCommand>
    {
        public async Task Handle(AddCategoryToPlannedTransactionCommand request, CancellationToken cancellationToken)
        {
            var plannedTransaction = await plannedTransactionRepository.GetById(request.UserId, request.PlannedTransactionId);

            if (plannedTransaction is null)
            {
                throw new ArgumentException($"No Planned Transaction with Id: {request.PlannedTransactionId}");
            }


            var category = await categoryRepository.GetById(request.UserId, request.CategoryId);

            if (category is null)
            {
                throw new ArgumentException($"No Category with Id: {request.CategoryId}");
            }

            plannedTransaction.CategoryId = request.CategoryId;

            //category.PlannedTransactions.Add(plannedTransaction);

            await plannedTransactionRepository.Update(plannedTransaction);
        }
    }
}
