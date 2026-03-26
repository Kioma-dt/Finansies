using BuisnessLogic.Enums;
namespace BuisnessLogic.Use_Cases.DebtInterestCalculator
{
    public class DebtInterestCalculatorProvider : IDebtInterestCalculatorProvider
    {
        public IDebtInterestCalculator GetCalculator(InterestType interestType)
        {
            return interestType switch
            {
                InterestType.None => new NoneInterestCalculator(),
                InterestType.Simple => new SimpleInterestCalculator(),
                InterestType.Complex => new ComplexInterestCalcilator(),
                InterestType.Fixed => new FixedInterestCalcilator(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
