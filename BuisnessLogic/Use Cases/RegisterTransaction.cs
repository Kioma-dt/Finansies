using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using BuisnessLogic.Use_Cases.DTO;

namespace BuisnessLogic.Use_Cases
{
    public interface IRegisterTransaction
    {
        Task<Transaction> Execute(RegisterTransactionDTO dto);
    }
    public class RegisterTransaction : IRegisterTransaction
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

        public async Task<Transaction> Execute(RegisterTransactionDTO dto)
        {
            var account = await _accountRepository.GetById(dto.AccountId);

            if (dto.Type == Enums.TransactionType.Expense)
            {
                account.RemoveFromBalance(dto.Amount);
            }
            else
            {
                account.AddToBalance(dto.Amount);
            }

            Category? category = null;
            FamilyMember? familyMember = null;

            if (dto.CategoryId is not null) 
            {
                category = category = await _categoryRepository.GetById(dto.CategoryId.Value);
            }
            if (dto.FamilyMemberId is not null)
            {
                familyMember = await _familyMemberRepository.GetById(dto.FamilyMemberId.Value);
            }

            var transaction = new Transaction
            {
                Amount = dto.Amount,
                Description = dto.Description,
                Date = dto.Date,
                Type = dto.Type,

                AccountId = account.Id,
                Account = account,

                CategoryId = dto.CategoryId,
                Category = category,

                FamilyMemberId = dto.FamilyMemberId,
                FamilyMember = familyMember
            };

            await _transactionRepository.Add(transaction);

            await _accountRepository.Update(account);

            return transaction;
        }
    }
}
