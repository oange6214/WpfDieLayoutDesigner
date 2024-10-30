using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DieLayoutDesigner.Converters;

public class BoolToStrokeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? new SolidColorBrush(Colors.Blue) : null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}