using System.Globalization;

namespace UI.Converters;

public class MoneyColorConverter : IValueConverter
{
    public Color PositiveColor { get; set; } = Colors.Green;

    public Color NegativeColor { get; set; } = Colors.Red;

    public object Convert(object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        decimal amount = 0;

        switch (value)
        {
            case decimal d:
                amount = d;
                break;

            case string s when decimal.TryParse(s, out var parsed):
                amount = parsed;
                break;

            default:
                return PositiveColor;
        }

        return amount < 0
            ? NegativeColor
            : PositiveColor;
    }

    public object ConvertBack(object value,
        Type targetType,
        object parameter,
        CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}