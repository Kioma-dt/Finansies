namespace BuisnessLogic.Use_Cases.DebtInterestCalculator
{
    public class NoneInterestCalculator : IDebtInterestCalculator
    {
        public decimal Calculate(decimal amount, decimal interestRate, decimal capitalisatonsPerYear, decimal fixedAddition, DateTime startDate, DateTime currentDate)
        {
            return amount;
        }
    }
}
