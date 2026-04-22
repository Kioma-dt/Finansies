using DataAccess.Enums;
namespace BuisnessLogic.DebtInterestCalculator
{
    public interface IDebtInterestCalculatorProvider
    {
        IDebtInterestCalculator GetCalculator(InterestType interestType);
    }

    public class DebtInterestCalculatorProvider : IDebtInterestCalculatorProvider
    {
        readonly Dictionary<InterestType, IDebtInterestCalculator> _calculators;

        public DebtInterestCalculatorProvider(IEnumerable<IDebtInterestCalculator> calculators)
        {
            _calculators = calculators.ToDictionary(c => c.InterestType);
        }

        public IDebtInterestCalculator GetCalculator(InterestType interestType)
        {
            if (! _calculators.TryGetValue(interestType, out var calculator))
            {
                throw new ArgumentException($"No Calculator for Interest Type: {interestType}");
            }

            return calculator;
        }
    }
}
