using BuisnessLogic.Repositories;
using DataAccess.Entities;

namespace BuisnessLogic.Services
{
    public interface ITransferService
    {
        Task TransferMoney(Guid userId,
            decimal amount,
            string description,
            DateTime date,
            Guid fromAccountId,
            Guid toAccountId);
    }
    public class TransferService : ITransferService
    {
        readonly IAccountRepository _accountRepository;
        readonly ITransferRepository _transferRepository;

        public TransferService(IAccountRepository accountRepository, ITransferRepository transferRepository)
        {
            _accountRepository = accountRepository;
            _transferRepository = transferRepository;
        }

        public async Task TransferMoney(Guid userId, decimal amount, string description, DateTime date, Guid fromAccountId, Guid toAccountId)
        {
            var fromAccount = await _accountRepository.GetById(userId, fromAccountId);
            var toAccount = await _accountRepository.GetById(userId, toAccountId);

            fromAccount.RemoveFromBalance(amount);
            toAccount.AddToBalance(amount);

            await _transferRepository.Add(new Transfer
            {
                Amount = amount,
                Description = description,
                Date = date,
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId,
                UserId = userId
            });

            await _accountRepository.Update(fromAccount);
            await _accountRepository.Update(toAccount);
        }
    }
}
