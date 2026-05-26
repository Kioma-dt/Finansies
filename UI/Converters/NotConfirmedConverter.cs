using BuisnessLogic.Enums;
using System.Globalization;

namespace UI.Converters
{
    public class NotConfirmedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is PlannedTransactionStatus status)
                return status != PlannedTransactionStatus.Confirmed;

            if (value is string statsStr)
                return statsStr != PlannedTransactionStatus.Confirmed.ToString();

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
