using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WPFSample.MetroBlendCompatibility
{
	public static class AttachedProperties
	{
		#region LayoutRounding (Attached Property)
		public static readonly DependencyProperty LayoutRoundingProperty =
		  DependencyProperty.RegisterAttached("LayoutRounding", typeof(string), typeof(AttachedProperties),
			new FrameworkPropertyMetadata(String.Empty));

		public static void SetLayoutRounding(DependencyObject d, string value)
		{
			d.SetValue(LayoutRoundingProperty, value);
		}

		public static string GetLayoutRounding(DependencyObject d)
		{
			return (string)d.GetValue(LayoutRoundingProperty);
		}
		#endregion 
	}
}
