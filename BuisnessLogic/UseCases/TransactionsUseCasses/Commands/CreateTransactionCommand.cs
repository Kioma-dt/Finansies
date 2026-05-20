using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BuisnessLogic.UseCases.TransactionsUseCasses.Commands
{
    public sealed record CreateTransactionCommand(Guid UserId,
            decimal Amount,
            string Description,
            DateTime Date,
            TransactionType Type,
            Guid AccountId,
            Guid? CategoryId,
            Guid? FamilyMemberId,
            Guid? DebtId)
        : IRequest;

    public class CreateTransactionCommandHandler(ITransactionRepository transactionRepository,
        IAccountRepository accountRepository,
        IDebtRepository debtRepository)
        : IRequestHandler<CreateTransactionCommand>
    {
        public async Task Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var account = await accountRepository.GetById(request.UserId, request.AccountId);

            if (account is null)
            {
                throw new ArgumentException($"No Account with Id: {request.AccountId}");
            }

            if (request.Type == TransactionType.Expense)
            {
                account.RemoveFromBalance(request.Amount);
            }
            else
            {
                account.AddToBalance(request.Amount);
            }

            if (request.DebtId is not null)
            {
                var debt = await debtRepository.GetById(request.UserId, request.DebtId.Value);

                if (debt is null)
                {
                    throw new ArgumentException($"No Debt with id {request.DebtId.Value}");
                }

                debt.MakeAPayment(request.Amount, request.Date);

                await debtRepository.Update(debt);
            }

            var transaction = new Transaction
            {
                Amount = request.Amount,
                Description = request.Description,
                Date = request.Date,
                Type = request.Type,

                AccountId = account.Id,
                CategoryId = request.CategoryId,
                FamilyMemberId = request.FamilyMemberId,
                DebtId = request.DebtId,
                UserId = request.UserId
            };

            await transactionRepository.Add(transaction);

            await accountRepository.Update(account);
        }
    }
}
