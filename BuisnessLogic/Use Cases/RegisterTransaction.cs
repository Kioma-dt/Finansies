using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.Use_Cases.DTO;

namespace BuisnessLogic.Use_Cases
{
    public class RegisterTransaction
    {
        readonly ITransactionRepository _transactionRepository;
        readonly IAccountRepository _accountRepository;
        readonly ICategoryRepository _categoryRepository;
        readonly IFamilyMemberRepository _familyMemberRepository;

        public RegisterTransaction(ITransactionRepository transactionRepository, IAccountRepository accountRepository, ICategoryRepository categoryRepository, IFamilyMemberRepository familyMemberRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository;
            _familyMemberRepository = familyMemberRepository;
        }

        public async Task Execute(RegisterTransactionDTO dto)
        {
            var account = await _accountRepository.GetById(dto.AccountId);
            var category = await _categoryRepository.GetById(dto.CategoryId);
            var familyMemeber = await _familyMemberRepository.GetById(dto.FamilyMemberId);

            if (dto.Type == Enums.TransactionType.Expense)
            {
                account.RemoveFromBalance(dto.Amount);
            }
            else
            {
                account.AddToBalance(dto.Amount);
            }

            await _transactionRepository.Add(new Entities.Transaction
            {
                Amount = dto.Amount,
                Description = dto.Description,
                Date = dto.Date,
                Type = dto.Type,

                AccountId = account.Id,
                Account = account,

                CategoryId = category.Id,
                Category = category,

                FamilyMemberId = familyMemeber.Id,
                FamilyMember = familyMemeber
            });

            await _accountRepository.Update(account);
        }
    }
}
