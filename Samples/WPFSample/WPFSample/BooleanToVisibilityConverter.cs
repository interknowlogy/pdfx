using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WPFSample
{
	public class CustomBooleanToVisibilityConverter : IValueConverter
	{
		public bool Inverted { get; set; }

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null || !(value is bool))
				return Visibility.Collapsed;

			if ((bool) value)
			{
				return Inverted ? Visibility.Collapsed : Visibility.Visible;
			}

			return Inverted ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
