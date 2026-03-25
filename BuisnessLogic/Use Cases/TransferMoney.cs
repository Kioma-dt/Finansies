using BuisnessLogic.Repositories;
using BuisnessLogic.Use_Cases.DTO;
using System.Runtime.InteropServices;

namespace BuisnessLogic.Use_Cases
{
    public class TransferMoney
    {
        readonly IAccountRepository _accountRepository;

        public TransferMoney(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task Execute(TransferMoneyDTO dto)
        {
            var fromAccount = await _accountRepository.GetById(dto.FromAccountId);
            var toAccount = await _accountRepository.GetById(dto.ToAccountId);

            fromAccount.RemoveFromBalance(dto.Amount);
            toAccount.AddToBalance(dto.Amount);

            await _accountRepository.Update(fromAccount);
            await _accountRepository.Update(toAccount);
        }
    }
}
