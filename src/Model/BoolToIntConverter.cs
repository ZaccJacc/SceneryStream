using Avalonia.Data;
using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SceneryStream.src.Model
{
    internal class BoolToIntConverter : IValueConverter
    {
        internal static readonly BoolToIntConverter Instance = new BoolToIntConverter();

        public object? Convert(object? value, Type targetType, object? parameter,
                          CultureInfo culture)
        {
            if (value is bool inputBool)
            {
                return (int)(inputBool ? 1 : 0);
            }
            return new BindingNotification(new InvalidCastException(),
                                                    BindingErrorType.Error);

        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
