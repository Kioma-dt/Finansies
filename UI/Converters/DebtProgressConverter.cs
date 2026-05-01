using BuisnessLogic.Entities;
using System.Globalization;

namespace UI.Converters
{
    public class DebtProgressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Debt debt)
            {
                if (debt.TotalAmount == 0)
                    return 0;

                return (double)(debt.PaidAmount / debt.TotalAmount);
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
