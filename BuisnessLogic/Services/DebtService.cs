using BuisnessLogic.DebtInterestCalculator;
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
        readonly IDebtInterestCalculatorProvider _debtInterestCalculatorProvider;

        public DebtService(IDebtRepository debtRepository, IDebtInterestCalculatorProvider debtInterestCalculatorProvider)
        {
            _debtRepository = debtRepository;
            _debtInterestCalculatorProvider = debtInterestCalculatorProvider;
        }

        public async Task PayOffDebt(Guid userId, Guid debtId, decimal amount, DateTime date)
        {
            var debt = await _debtRepository.GetById(userId, debtId);

            debt.MakeAPayment(amount, date);

            await _debtRepository.Update(debt);
        }

        public async Task UpdateDebt(Guid userId, Guid debtId, DateTime date)
        {
            var debt = await _debtRepository.GetById(userId, debtId);

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
