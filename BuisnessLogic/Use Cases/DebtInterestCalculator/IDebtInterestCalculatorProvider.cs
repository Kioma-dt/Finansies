using BuisnessLogic.Enums;
namespace BuisnessLogic.Use_Cases.DebtInterestCalculator
{
    public interface IDebtInterestCalculatorProvider
    {
        IDebtInterestCalculator GetCalculator(InterestType interestType);
    }
}
