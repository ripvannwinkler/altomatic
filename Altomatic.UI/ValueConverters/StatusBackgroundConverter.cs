using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Altomatic.UI.ValueConverters
{
	[ValueConversion(typeof(bool), typeof(Brush))]
	public class StatusBackgroundConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var isPaused = (bool)value;
			return isPaused ? Brushes.MistyRose : Brushes.PaleGreen;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
