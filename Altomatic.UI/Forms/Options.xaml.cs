using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Altomatic.UI.ViewModels;

namespace Altomatic.UI.Forms
{
	/// <summary>
	/// Interaction logic for Options.xaml
	/// </summary>
	public partial class Options : Window
	{
		public static readonly DependencyProperty optionsProperty = DependencyProperty.Register(nameof(Model), typeof(OptionsViewModel), typeof(Options));

		public OptionsViewModel Model
		{
			get { return (OptionsViewModel)GetValue(optionsProperty); }
			set { SetValue(optionsProperty, value); }
		}

		public Options(OptionsViewModel model)
		{
			InitializeComponent();
			WindowStartupLocation = WindowStartupLocation.CenterOwner;
			Model = model;
		}

		private void Save1_Click(object sender, RoutedEventArgs e)
		{
			Model.Save();
			DialogResult = true;
		}

    private void Save2_Click(object sender, RoutedEventArgs e)
    {
			Model.SaveAs();
			DialogResult = true;
		}

		private void Load1_Click(object sender, RoutedEventArgs e)
    {
			Model.LoadFrom();
    }

  }
}
