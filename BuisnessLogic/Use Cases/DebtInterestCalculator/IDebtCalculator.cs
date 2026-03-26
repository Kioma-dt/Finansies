namespace BuisnessLogic.Use_Cases.DebtInterestCalculator
{
    public interface IDebtInterestCalculator
    {
        decimal Calculate(decimal amount, decimal interestRate, decimal capitalisatonsPerYear, decimal fixedAddition, DateTime startDate, DateTime currentDate);
    }
}
