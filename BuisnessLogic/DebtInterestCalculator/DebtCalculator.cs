using BuisnessLogic.Enums;

namespace BuisnessLogic.DebtInterestCalculator
{
    public interface IDebtInterestCalculator
    {
        InterestType InterestType { get; }
        decimal Calculate(decimal amount, 
            decimal interestRate,
            decimal capitalisatonsPerYear, 
            decimal fixedAddition, 
            DateTime startDate, 
            DateTime currentDate);
    }

    public class ComplexInterestCalcilator : IDebtInterestCalculator
    {
        public InterestType InterestType => InterestType.Complex;

        public decimal Calculate(decimal amount, 
            decimal interestRate, 
            decimal capitalisatonsPerYear, 
            decimal fixedAddition,
            DateTime startDate, 
            DateTime currentDate)
        {
            var days = (currentDate - startDate).Days;

            if (days < 0)
            {
                throw new Exception("Wrong Dates");
            }

            var years = Math.Floor((decimal)days / 365);

            return amount * (decimal)Math.Pow((double)(1 + interestRate / capitalisatonsPerYear), (double)(years * capitalisatonsPerYear));
        }
    }
    public class FixedInterestCalcilator : IDebtInterestCalculator
    {
        public InterestType InterestType => InterestType.Fixed;
        public decimal Calculate(decimal amount, 
            decimal interestRate, 
            decimal capitalisatonsPerYear, 
            decimal fixedAddition, 
            DateTime startDate, 
            DateTime currentDate)
        {
            var days = (currentDate - startDate).Days;

            if (days < 0)
            {
                throw new Exception("Wrong Dates");
            }

            var years = Math.Floor((decimal)days / 365);

            return amount + fixedAddition * (decimal)Math.Floor((double)(years * capitalisatonsPerYear));
        }
    }

    public class NoneInterestCalculator : IDebtInterestCalculator
    {
        public InterestType InterestType => InterestType.None;
        public decimal Calculate(decimal amount,
            decimal interestRate,
            decimal capitalisatonsPerYear,
            decimal fixedAddition,
            DateTime startDate,
            DateTime currentDate)
        {
            return amount;
        }
    }
    public class SimpleInterestCalculator : IDebtInterestCalculator
    {
        public InterestType InterestType => InterestType.Simple;
        public decimal Calculate(decimal amount, 
            decimal interestRate, decimal capitalisatonsPerYear, 
            decimal fixedAddition, 
            DateTime startDate, 
            DateTime currentDate)
        {
            var days = (currentDate - startDate).Days;

            if (days < 0)
            {
                throw new Exception("Wrong Dates");
            }

            var years = Math.Floor((decimal)days / 365);

            return amount * (1 + interestRate * years);
        }
    }
}
