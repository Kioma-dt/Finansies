using BuisnessLogic.Repositories;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using System;

namespace BuisnessLogic.Services
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetAll(Guid userId);
        Task RegsiterTransaction(Guid userId,
            decimal amount,
            string description,
            DateTime date,
            TransactionType type,
            Guid accountId, 
            Guid? categoryId,
            Guid? familyMemberId,
            Guid? debtId);

        Task AddCategory(Guid userId,
            Guid transactionId,
            Guid categoryId);

        Task AddTag(Guid userId, 
            Guid transactionId,
            Guid transactionTagId);

        Task AddFamilyMember(Guid userId,
            Guid transactionId,
            Guid familyMemberId);
    }
    public class TransactionService : ITransactionService
    {
        readonly ITransactionRepository _transactionRepository;
        readonly IAccountRepository _accountRepository;
        readonly ICategoryRepository _categoryRepository;
        readonly IFamilyMemberRepository _familyMemberRepository;
        readonly IDebtRepository _debtRepository;
        readonly ITransactionTagRepository _transactionTagRepository;

        public TransactionService(ITransactionRepository transactionRepository,
            IAccountRepository accountRepository, 
            ICategoryRepository categoryRepository, 
            IFamilyMemberRepository familyMemberRepository, 
            ITransactionTagRepository transactionTagRepository,
            IDebtRepository debtRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository;
            _familyMemberRepository = familyMemberRepository;
            _transactionTagRepository = transactionTagRepository;
            _debtRepository = debtRepository;
        }

        public async Task<List<Transaction>> GetAll(Guid userId) => (await _transactionRepository.GetAll(userId));
        public async Task AddCategory(Guid userId, Guid transactionId, Guid categoryId)
        {
            var transaction = await _transactionRepository.GetById(userId, transactionId);

            if (transaction is null)
            {
                throw new ArgumentException($"No Transaction with Id: {transactionId}");
            }

            var category = await _categoryRepository.GetById(userId, categoryId);

            if (category is null)
            {
                throw new ArgumentException($"No Category with Id: {categoryId}");
            }

            //transaction.Category = category;
            transaction.CategoryId = categoryId;


            category.Transactions.Add(transaction);



            await _transactionRepository.Update(transaction);
            //await _familyMemberRepository.Update(familyMember);
        }

        public async Task AddFamilyMember(Guid userId, Guid transactionId, Guid familyMemberId)
        {
            var transaction = await _transactionRepository.GetById(userId, transactionId);

            if (transaction is null)
            {
                throw new ArgumentException($"No Transaction with Id: {transactionId}");
            }

            var familyMember = await _familyMemberRepository.GetById(userId, familyMemberId);

            if (familyMember is null)
            {
                throw new ArgumentException($"No Family Member with Id: {familyMemberId}");
            }

            //transaction.FamilyMember = familyMember;
            transaction.FamilyMemberId = familyMemberId;

            familyMember.Transactions.Add(transaction);

            await _transactionRepository.Update(transaction);
            //await _familyMemberRepository.Update(familyMember);
        }

        public async Task AddTag(Guid userId, Guid transactionId, Guid transactionTagId)
        {
            var transaction = await _transactionRepository.GetById(userId, transactionId);

            if (transaction is null)
            {
                throw new ArgumentException($"No Transaction with Id: {transactionId}");
            }

            var transactionTag = await _transactionTagRepository.GetById(userId, transactionTagId);

            if (transactionTag is null)
            {
                throw new ArgumentException($"No Transaction Tag with Id: {transactionTagId}");
            }

            transaction.TransactionTags.Add(transactionTag);

            transactionTag.Transactions.Add(transaction);

            await _transactionRepository.Update(transaction);
            //await _familyMemberRepository.Update(familyMember);
        }

        public async Task RegsiterTransaction(Guid userId, 
            decimal amount,
            string description,
            DateTime date, 
            TransactionType type, 
            Guid accountId, 
            Guid? categoryId,
            Guid? familyMemberId,
            Guid? debtId)
        {
            var account = await _accountRepository.GetById(userId, accountId);

            if (account is null)
            {
                throw new ArgumentException($"No Account with Id: {accountId}");
            }

            if (type == TransactionType.Expense)
            {
                account.RemoveFromBalance(amount);
            }
            else
            {
                account.AddToBalance(amount);
            }       

            if (debtId is not null)
            {
                await _debtRepository.PayOffDebt(userId, debtId.Value, amount, date);
            }

            var transaction = new Transaction
            {
                Amount = amount,
                Description = description,
                Date = date,
                Type = type,

                AccountId = account.Id,
                CategoryId = categoryId,
                FamilyMemberId = familyMemberId,
                UserId = userId
            };

            await _transactionRepository.Add(transaction);

            await _accountRepository.Update(account);
        }
    }
}
