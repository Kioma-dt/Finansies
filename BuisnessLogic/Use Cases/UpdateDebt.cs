using BuisnessLogic.Repositories;
using BuisnessLogic.Entities;
using System.Data.Common;
using BuisnessLogic.Use_Cases.DebtInterestCalculator;

namespace BuisnessLogic.Use_Cases
{
    public class UpdateDebt
    {
        readonly IDebtRepository _debtRepository;
        readonly IDebtInterestCalculatorProvider _debtInterestCalculatorProvider;

        public UpdateDebt(IDebtRepository debtRepository, IDebtInterestCalculatorProvider debtInterestCalculatorProvider)
        {
            _debtRepository = debtRepository;
            _debtInterestCalculatorProvider = debtInterestCalculatorProvider;
        }

        public async Task Execute(DTO.UpdateDebtDTO dto)
        {
            var debt = await _debtRepository.GetById(dto.DebtId);

            if (dto.Date > debt.EndDate)
            {
                throw new Exception("The Debt Is Overdue!");
            }

            var calculator = _debtInterestCalculatorProvider.GetCalculator(debt.InterestType);

            var new_amount = calculator.Calculate(debt.StartAmount,
                                                  debt.InterestRate,
                                                  debt.CapitalisationsPerYear,
                                                  debt.FixedAddition,
                                                  debt.StartDate,
                                                  dto.Date);
            
            var interests = debt.TotalAmount - new_amount;

            debt.CalculateInterest(interests);

            await _debtRepository.Update(debt);
        }
    }
}
