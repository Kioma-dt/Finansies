namespace BuisnessLogic.Use_Cases.DebtInterestCalculator
{
    public class FixedInterestCalcilator : IDebtInterestCalculator
    {
        public decimal Calculate(decimal amount, decimal interestRate, decimal capitalisatonsPerYear, decimal fixedAddition, DateTime startDate, DateTime currentDate)
        {
            var days = (currentDate - startDate).Days;

            if (days < 0)
            {
                throw new Exception("Wrong Dates");
            }

            var years = Math.Floor((decimal)days / 365);

            return amount + (fixedAddition * (decimal)Math.Floor((double)(years * capitalisatonsPerYear)));
        }
    }
}
