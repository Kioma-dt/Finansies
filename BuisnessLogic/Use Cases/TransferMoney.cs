using BuisnessLogic.Repositories;
using BuisnessLogic.Use_Cases.DTO;
using BuisnessLogic.Entities;
using System.Runtime.InteropServices;

namespace BuisnessLogic.Use_Cases
{
    public class TransferMoney
    {
        readonly IAccountRepository _accountRepository;
        readonly ITransferRepository _transferRepository;

        public TransferMoney(IAccountRepository accountRepository, ITransferRepository transferRepository)
        {
            _accountRepository = accountRepository;
            _transferRepository = transferRepository;
        }

        public async Task Execute(TransferMoneyDTO dto)
        {
            var fromAccount = await _accountRepository.GetById(dto.FromAccountId);
            var toAccount = await _accountRepository.GetById(dto.ToAccountId);

            fromAccount.RemoveFromBalance(dto.Amount);
            toAccount.AddToBalance(dto.Amount);

            await _transferRepository.Add(new Transfer
            {
                Amount = dto.Amount,
                Description = dto.Description,
                Date = dto.Date,
                FromAccountId = dto.FromAccountId,
                ToAccountId = dto.ToAccountId,
                UserId = dto.UserId
            });

            await _accountRepository.Update(fromAccount);
            await _accountRepository.Update(toAccount);
        }
    }
}
