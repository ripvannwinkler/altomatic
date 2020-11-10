using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Altomatic.UI.Utilities;

namespace Altomatic.UI
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			DispatcherUnhandledException += HandleExceptions;
			ProcessUtilities.EnsureDlls();
		}

		private void HandleExceptions(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			MessageBox.Show(e.Exception.Message, "Unhandled Exception", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}
}
