using BuisnessLogic.DebtInterestCalculator;
using BuisnessLogic.DTO;
using BuisnessLogic.Entities;
using BuisnessLogic.Enums;
using BuisnessLogic.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisnessLogic.Services
{
    

    public interface IDebtService
    {
        Task<List<Debt>> GetAll(Guid userId);
        Task CreateDebt(Guid userId, DebtCreateDTO dto);
        Task PayOffDebt(Guid userId,
            Guid debtId,
            decimal amount,
            DateTime date);

        Task UpdateDebt(Guid userId,
            Guid debtId,
            DateTime date);
    }
    public class DebtService : IDebtService
    {
        readonly IDebtRepository _debtRepository;
        readonly IPlannedTransactionService _plannedTransactionService;
        readonly IDebtInterestCalculatorProvider _debtInterestCalculatorProvider;

        public DebtService(IDebtRepository debtRepository, 
            IDebtInterestCalculatorProvider debtInterestCalculatorProvider,
            IPlannedTransactionService plannedTransactionService)
        {
            _debtRepository = debtRepository;
            _plannedTransactionService = plannedTransactionService;
            _debtInterestCalculatorProvider = debtInterestCalculatorProvider;
        }

        public async Task CreateDebt(Guid userId, DebtCreateDTO dto)
        {
            var id = Guid.NewGuid();
            var debt = new Debt()
            {
                Id = id,
                Name = dto.Name,
                StartAmount = dto.Amount,
                TotalAmount = dto.Amount,
                PaidAmount = 0,
                InterestRate = dto.InterestRate,
                CapitalisationsPerYear = dto.CapitalisatonsPerYear,
                FixedAddition = dto.FixedAddition,
                Type = dto.Type,
                InterestType = dto.InterestType,
                StartDate = dto.StartDate,
                LastPaidDate = dto.StartDate,
                EndDate = dto.EndDate,
                CategoryId = dto.CategoryId,
                FamilyMemberId = dto.FamilyMemberId,
                UserId = userId
            };

            await _debtRepository.Add(debt);

            if (dto.IsAutoPlanned)
            {
                await _plannedTransactionService.PlanDebtPayments(userId, id, dto.TransactionPeriodicity);
            }
        }

        public Task<List<Debt>> GetAll(Guid userId) => _debtRepository.GetAll(userId);

        public async Task PayOffDebt(Guid userId, Guid debtId, decimal amount, DateTime date) => await _debtRepository.PayOffDebt(userId, debtId, amount, date);

        public async Task UpdateDebt(Guid userId, Guid debtId, DateTime date)
        {
            var debt = await _debtRepository.GetById(userId, debtId);

            if (debt is null)
            {
                throw new ArgumentException($"No Debt with Id: {debtId}");
            }

            if (date > debt.EndDate)
            {
                throw new Exception("The Debt Is Overdue!");
            }

            var calculator = _debtInterestCalculatorProvider.GetCalculator(debt.InterestType);

            var new_amount = calculator.Calculate(debt.StartAmount,
                                                  debt.InterestRate,
                                                  debt.CapitalisationsPerYear,
                                                  debt.FixedAddition,
                                                  debt.StartDate,
                                                  date);

            var interests = debt.TotalAmount - new_amount;

            debt.ChargeInterest(interests);

            await _debtRepository.Update(debt);
        }
    }
}
