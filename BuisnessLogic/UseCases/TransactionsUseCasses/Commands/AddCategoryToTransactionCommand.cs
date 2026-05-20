using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BuisnessLogic.UseCases.TransactionsUseCasses.Commands
{
    public sealed record AddCategoryToTransactionCommand(Guid UserId,
        Guid TransactionId,
        Guid CategoryId)
        : IRequest;

    public class AddCategoryToTransactionCommandHandler(ITransactionRepository transactionRepository,
        ICategoryRepository categoryRepository)
        : IRequestHandler<AddCategoryToTransactionCommand>
    {
        public async Task Handle(AddCategoryToTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await transactionRepository.GetById(request.UserId, request.TransactionId);

            if (transaction is null)
            {
                throw new ArgumentException($"No Transaction with Id: {request.TransactionId}");
            }

            var category = await categoryRepository.GetById(request.UserId, request.CategoryId);

            if (category is null)
            {
                throw new ArgumentException($"No Category with Id: {request.CategoryId}");
             }

            transaction.CategoryId = request.CategoryId;

            //category.Transactions.Add(transaction);

            await transactionRepository.Update(transaction);
        }
    }
}
