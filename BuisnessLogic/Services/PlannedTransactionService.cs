using BuisnessLogic.Repositories;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisnessLogic.Shared;
using BuisnessLogic.DebtInterestCalculator;

namespace BuisnessLogic.Services
{
    public interface IPlannedTransactionService
    {
        Task<List<PlannedTransaction>> GetAll(Guid userId);
        Task PlanTransaction(Guid userId,
            decimal amount,
            string description,
            TransactionType type,
            DateTime plannedDate,
            Guid? accountId,
            Guid? categoryId,
            Guid? familyMemberId,
            Guid? debtId);

        public Task PlanMultipleTransactions(Guid userId,
            decimal amount,
            string description,
            TransactionType type,
            DateTime startDate,
            TransactionPeriodicity periodicy,
            uint numberOfTransactions,
            Guid? accountId,
            Guid? categoryId,
            Guid? familyMemberId,
            Guid? debtId
            );

        Task PlanDebtPayments(Guid userId, Guid debtId, TransactionPeriodicity periodicity);

        Task ConfirmTransaction(Guid userId,
            Guid plannedTransactionId,
            DateTime? date = null);

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
        readonly ITransactionService _transactionService;
        readonly IAccountRepository _accountRepository;
        readonly ICategoryRepository _categoryRepository;
        readonly IFamilyMemberRepository _familyMemberRepository;
        readonly IPlannedTransactionRepository _plannedTransactionRepository;
        readonly IDebtRepository _debtRepository;
        readonly ITransactionTagRepository _transactionTagRepository;

        readonly IDebtInterestCalculatorProvider _debtInterestCalculatorProvider;

        public PlannedTransactionService(ITransactionService trasnsactionService, 
            IPlannedTransactionRepository plannedTransactionRepository,
            IAccountRepository accountRepository,
            ICategoryRepository categoryRepository, 
            IFamilyMemberRepository familyMemberRepository,
            IDebtRepository debtRepository,
            ITransactionTagRepository transactionTagRepository,
            IDebtInterestCalculatorProvider debtInterestCalculatorProvider)
        {
            _transactionService = trasnsactionService;
            _plannedTransactionRepository = plannedTransactionRepository;
            _accountRepository = accountRepository;
            _categoryRepository = categoryRepository;
            _familyMemberRepository = familyMemberRepository;
            _debtRepository = debtRepository;
            _transactionTagRepository = transactionTagRepository;
            _debtInterestCalculatorProvider = debtInterestCalculatorProvider;
        }

        public async Task<List<PlannedTransaction>> GetAll(Guid userId) => await _plannedTransactionRepository.GetAll(userId);

        public async Task AddAccount(Guid userId, Guid plannedTransactionId, Guid accountId)
        {
            var plannedTransaction = await _plannedTransactionRepository.GetById(userId, plannedTransactionId);

            if (plannedTransaction is null)
            {
                throw new ArgumentException($"No Planned Transaction with Id: {plannedTransactionId}");
            }

            var account = await _accountRepository.GetById(userId, accountId);

            if (account is null)
            {
                throw new ArgumentException($"No Account with Id: {accountId}");
            }


            plannedTransaction.Account = account;
            plannedTransaction.AccountId = accountId;

            account.PlannedTransactions.Add(plannedTransaction);

            await _plannedTransactionRepository.Update(plannedTransaction);
            //await _familyMemberRepository.Update(familyMember);
        }

        public async Task AddCategory(Guid userId, Guid plannedTransactionId, Guid categoryId)
        {
            var plannedTransaction = await _plannedTransactionRepository.GetById(userId, plannedTransactionId);

            if (plannedTransaction is null)
            {
                throw new ArgumentException($"No Planned Transaction with Id: {plannedTransactionId}");
            }


            var category = await _categoryRepository.GetById(userId, categoryId);

            if (category is null)
            {
                throw new ArgumentException($"No Category with Id: {categoryId}");
            }

            plannedTransaction.Category = category;
            plannedTransaction.CategoryId = categoryId;

            category.PlannedTransactions.Add(plannedTransaction);

            await _plannedTransactionRepository.Update(plannedTransaction);
            //await _familyMemberRepository.Update(familyMember);
        }

        public async Task AddFamilyMember(Guid userId, Guid plannedTransactionId, Guid familyMemberId)
        {
            var plannedTransaction = await _plannedTransactionRepository.GetById(userId, plannedTransactionId);

            if (plannedTransaction is null)
            {
                throw new ArgumentException($"No Planned Transaction with Id: {plannedTransactionId}");
            }


            var familyMember = await _familyMemberRepository.GetById(userId, familyMemberId);

            if (familyMember is null)
            {
                throw new ArgumentException($"No Family Member with Id: {familyMemberId}");
            }

            plannedTransaction.FamilyMember = familyMember;
            plannedTransaction.FamilyMemberId = familyMemberId;

            familyMember.PlannedTransactions.Add(plannedTransaction);

            await _plannedTransactionRepository.Update(plannedTransaction);
            //await _familyMemberRepository.Update(familyMember);
        }

        public async Task AddTag(Guid userId, Guid plannedTransactionId, Guid transactionTagId)
        {
            var plannedTransaction = await _plannedTransactionRepository.GetById(userId, plannedTransactionId);

            if (plannedTransaction is null)
            {
                throw new ArgumentException($"No Planned Transaction with Id: {plannedTransactionId}");
            }


            var transactionTag = await _transactionTagRepository.GetById(userId, transactionTagId);

            if (transactionTag is null)
            {
                throw new ArgumentException($"No Transaction Tag with Id: {transactionTagId}");
            }

            plannedTransaction.TransactionTags.Add(transactionTag);

            transactionTag.PlannedTransactions.Add(plannedTransaction);

            await _plannedTransactionRepository.Update(plannedTransaction);
            //await _familyMemberRepository.Update(familyMember);
        }

        public async Task ConfirmTransaction(Guid userId, Guid plannedTransactionId, DateTime? date = null)
        {

            var plannedTransaction = await _plannedTransactionRepository.GetById(userId, plannedTransactionId);

            if (plannedTransaction is null)
            {
                throw new ArgumentException($"No Planned Transaction with Id: {plannedTransactionId}");
            }

            if (plannedTransaction.Status == PlannedTransactionStatus.Confirmed)
            {
                throw new ArgumentException($"No Planned Transaction is Already Confirmed: {plannedTransaction.Description}");
            }


            if (plannedTransaction.AccountId is null)
            {
                throw new ArgumentException("Planned Transaction Does't Contain Account");
            }

            if (date is null)
            {
                date = plannedTransaction.PlannedDate;
            }

            await _transactionService.RegsiterTransaction(
                plannedTransaction.UserId,
                plannedTransaction.Amount,
                plannedTransaction.Description,
                date.Value,
                plannedTransaction.Type,
                plannedTransaction.AccountId.Value,
                plannedTransaction.CategoryId,
                plannedTransaction.FamilyMemberId,
                plannedTransaction.DebtId
            );

            plannedTransaction.Conirm();
            await _plannedTransactionRepository.Update(plannedTransaction);
        }

        public async Task PlanTransaction(Guid userId, decimal amount, string description, TransactionType type, DateTime plannedDate,
            Guid? accountId,
            Guid? categoryId,
            Guid? familyMemberId,
            Guid? debtId)
        {
            var planedTransaction = new PlannedTransaction()
            {
                Amount = amount,
                Description = description,
                Type = type,
                PlannedDate = plannedDate,
                AccountId = accountId,
                CategoryId = categoryId,
                FamilyMemberId = familyMemberId,
                DebtId = debtId,
                UserId = userId
            };

            await _plannedTransactionRepository.Add(planedTransaction);
        }

        public async Task PlanMultipleTransactions(Guid userId,
            decimal amount,
            string description,
            TransactionType type,
            DateTime startDate,
            TransactionPeriodicity periodicy,
            uint numberOfTransactions,
            Guid? accountId,
            Guid? categoryId,
            Guid? familyMemberId, 
            Guid? debtId)
        {
            for (int i = 0; i < numberOfTransactions; i++)
            {
                await this.PlanTransaction(userId, amount, description, type, startDate, accountId, categoryId, familyMemberId, debtId);

                startDate = startDate.AddTransactionPeriodicity(periodicy);
            }
        }

        public async Task PlanDebtPayments(Guid userId, Guid debtId, TransactionPeriodicity periodicity)
        {
            var debt = await _debtRepository.GetById(userId, debtId);

            if (debt is null)
            {
                throw new ArgumentException($"No Such Debt: {debt}");
            }

            var period = (decimal)(debt.EndDate - debt.StartDate).TotalDays;

            var totalAmount = _debtInterestCalculatorProvider
                .GetCalculator(debt.InterestType)
                .Calculate
                (
                    debt.StartAmount,
                    debt.InterestRate,
                    debt.CapitalisationsPerYear,
                    debt.FixedAddition,
                    debt.StartDate,
                    debt.EndDate
                );

            var transactionAmount = periodicity switch
            {
                TransactionPeriodicity.Daily => totalAmount / period,
                TransactionPeriodicity.Monthly => totalAmount / (period / 30m),
                TransactionPeriodicity.Yearly => totalAmount / (period / 365m),
                _ => totalAmount
            };

            var type = debt.Type switch
            {
                DebtType.Debit => TransactionType.Income,

                _ => TransactionType.Expense
            };

            var paymentsNumber = (uint)Math.Ceiling(totalAmount / transactionAmount);

            await this.PlanMultipleTransactions(userId,
                transactionAmount,
                debt.Name,
                type,
                debt.StartDate,
                periodicity,
                paymentsNumber,
                null,
                null,
                null,
                debtId
                );
        }

        public async Task UpdatePlannedTransaction(Guid userId, Guid plannedTransactionId, DateTime date)
        {
            var plannedTransaction = await _plannedTransactionRepository.GetById(userId, plannedTransactionId);

            if (plannedTransaction is null)
            {
                throw new ArgumentException($"No Planned Transaction with Id: {plannedTransactionId}");
            }


            plannedTransaction.Update(date);

            await _plannedTransactionRepository.Update(plannedTransaction);
        }
    }
}
