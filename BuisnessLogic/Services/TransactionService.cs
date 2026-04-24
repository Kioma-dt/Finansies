using BuisnessLogic.Repositories;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using System;

namespace BuisnessLogic.Services
{
    public interface ITrasnsactionService
    {
        Task RegsiterTransaction(Guid userId,
            decimal amount,
            string description,
            DateTime date,
            TransactionType type,
            Guid accountId);

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
    public class TransactionService : ITrasnsactionService
    {
        readonly ITransactionRepository _transactionRepository;
        readonly IAccountRepository _accountRepository;
        readonly ICategoryRepository _categoryRepository;
        readonly IFamilyMemberRepository _familyMemberRepository;
        readonly ITransactionTagRepository _transactionTagRepository;

        public TransactionService(ITransactionRepository transactionRepository, IAccountRepository accountRepository, ICategoryRepository categoryRepository, IFamilyMemberRepository familyMemberRepository, ITransactionTagRepository transactionTagRepository)
        {
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository;
            _familyMemberRepository = familyMemberRepository;
            _transactionTagRepository = transactionTagRepository;
        }

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

            transaction.Category = category;
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

            transaction.FamilyMember = familyMember;
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

        public async Task RegsiterTransaction(Guid userId, decimal amount, string description, DateTime date, TransactionType type, Guid accountId)
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

            var transaction = new Transaction
            {
                Amount = amount,
                Description = description,
                Date = date,
                Type = type,

                AccountId = account.Id,
                Account = account
            };

            await _transactionRepository.Add(transaction);

            await _accountRepository.Update(account);
        }
    }
}
