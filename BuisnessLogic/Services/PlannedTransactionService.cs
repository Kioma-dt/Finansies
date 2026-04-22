using BuisnessLogic.Repositories;
using DataAccess.Entities;
using DataAccess.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.Services
{
    public interface IPlannedTransactionService
    {
        Task PlanTransaction(Guid userId,
            decimal amount,
            string description,
            DateTime plannedDate,
            TransactionType type);

        Task ConfirmTransaction(Guid userId,
            Guid plannedTransactionId,
            DateTime date);

        Task UpdatePlannedTransaction(Guid userId,
            Guid plannedTransactionId,
            DateTime date);

        Task AddAccount(Guid userId,
            Guid plannedTransactionId,
            Guid accountId);
        Task AddCategory(Guid userId,
            Guid plannedTransactionId,
            Guid categoryId);

        Task AddTag(Guid userId,
            Guid plannedTransactionId,
            Guid transactionTagId);

        Task AddFamilyMember(Guid userId,
            Guid plannedTransactionId,
            Guid familyMemberId);
    }
    public class PlannedTransactionService : IPlannedTransactionService
    {
        readonly ITrasnsactionService _transactionService;
        readonly IAccountRepository _accountRepository;
        readonly ICategoryRepository _categoryRepository;
        readonly IFamilyMemberRepository _familyMemberRepository;
        readonly IPlannedTransactionRepository _plannedTransactionRepository;
        readonly ITransactionTagRepository _transactionTagRepository;

        public PlannedTransactionService(ITrasnsactionService trasnsactionService, 
            IPlannedTransactionRepository plannedTransactionRepository,
            IAccountRepository accountRepository,
            ICategoryRepository categoryRepository, 
            IFamilyMemberRepository familyMemberRepository,
            ITransactionTagRepository transactionTagRepository)
        {
            _transactionService = trasnsactionService;
            _plannedTransactionRepository = plannedTransactionRepository;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository;
            _familyMemberRepository = familyMemberRepository;
            _transactionTagRepository = transactionTagRepository;
        }

        public async Task AddAccount(Guid userId, Guid plannedTransactionId, Guid accountId)
        {
            var plannedTransaction = await _plannedTransactionRepository.GetById(userId, plannedTransactionId);
            var account = await _accountRepository.GetById(userId, accountId);

            plannedTransaction.Account = account;
            plannedTransaction.AccountId = accountId;

            account.PlannedTransactions.Add(plannedTransaction);

            await _plannedTransactionRepository.Update(plannedTransaction);
            //await _familyMemberRepository.Update(familyMember);
        }

        public async Task AddCategory(Guid userId, Guid plannedTransactionId, Guid categoryId)
        {
            var plannedTransaction = await _plannedTransactionRepository.GetById(userId, plannedTransactionId);
            var category = await _categoryRepository.GetById(userId, categoryId);

            plannedTransaction.Category = category;
            plannedTransaction.CategoryId = categoryId;

            category.PlannedTransactions.Add(plannedTransaction);

            await _plannedTransactionRepository.Update(plannedTransaction);
            //await _familyMemberRepository.Update(familyMember);
        }

        public async Task AddFamilyMember(Guid userId, Guid plannedTransactionId, Guid familyMemberId)
        {
            var plannedTransaction = await _plannedTransactionRepository.GetById(userId, plannedTransactionId);
            var familyMember = await _familyMemberRepository.GetById(userId, familyMemberId);

            plannedTransaction.FamilyMember = familyMember;
            plannedTransaction.FamilyMemberId = familyMemberId;

            familyMember.PlannedTransactions.Add(plannedTransaction);

            await _plannedTransactionRepository.Update(plannedTransaction);
            //await _familyMemberRepository.Update(familyMember);
        }

        public async Task AddTag(Guid userId, Guid plannedTransactionId, Guid transactionTagId)
        {
            var plannedTransaction = await _plannedTransactionRepository.GetById(userId, plannedTransactionId);
            var transactionTag = await _transactionTagRepository.GetById(userId, transactionTagId);

            plannedTransaction.TransactionTags.Add(transactionTag);

            transactionTag.PlannedTransactions.Add(plannedTransaction);

            await _plannedTransactionRepository.Update(plannedTransaction);
            //await _familyMemberRepository.Update(familyMember);
        }

        public async Task ConfirmTransaction(Guid userId, Guid plannedTransactionId, DateTime date)
        {
            var plannedTransaction = await _plannedTransactionRepository.GetById(userId, plannedTransactionId);

            if(plannedTransaction.AccountId is null)
            {
                throw new Exception("Planned Transaction Does't Contain Account");
            }

            await _transactionService.RegsiterTransaction(
                plannedTransaction.UserId,
                plannedTransaction.Amount,
                plannedTransaction.Description,
                date,
                plannedTransaction.Type,
                plannedTransaction.AccountId.Value
            );

            plannedTransaction.Conirm();
            await _plannedTransactionRepository.Update(plannedTransaction);
        }

        public async Task PlanTransaction(Guid userId, decimal amount, string description, DateTime plannedDate, TransactionType type)
        {
            var planedTransaction = new PlannedTransaction()
            {
                Amount = amount,
                Description = description,
                Type = type,
                PlannedDate = plannedDate
            };

            await _plannedTransactionRepository.Add(planedTransaction);
        }

        public async Task UpdatePlannedTransaction(Guid userId, Guid plannedTransactionId, DateTime date)
        {
            var plannedTransaction = await _plannedTransactionRepository.GetById(userId, plannedTransactionId);

            plannedTransaction.Update(date);

            await _plannedTransactionRepository.Update(plannedTransaction);
        }
    }
}
