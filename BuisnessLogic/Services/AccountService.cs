using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisnessLogic.Entities;
using BuisnessLogic.Repositories;

namespace BuisnessLogic.Services
{
    public interface IAccountService
    {
        Task<List<Account>> GetAll(Guid userId);
        Task Add(Account account);
    }
    public class AccountService : IAccountService
    {
        readonly IAccountRepository _accountRepository;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<List<Account>> GetAll(Guid userId) => (await _accountRepository.GetAll(userId)) ?? new();

        public async Task Add(Account account) => await _accountRepository.Add(account);
    }
}
