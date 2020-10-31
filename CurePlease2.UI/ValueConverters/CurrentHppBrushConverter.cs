using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace CurePlease2.UI.ValueConverters
{
	[ValueConversion(typeof(double), typeof(Brush))]
	public class CurrentHppBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var hpp = (double)value;
			return hpp switch
			{
				var x when x > 75 => Brushes.Green,
				var x when x > 50 => Brushes.Yellow,
				var x when x > 25 => Brushes.Orange,
				_ => Brushes.Red
			};
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
