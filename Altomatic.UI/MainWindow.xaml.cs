using Altomatic.UI.Game;
using Altomatic.UI.ViewModels;
using EliteMMO.API;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Altomatic.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly static DependencyProperty AppDataProperty = DependencyProperty.Register(nameof(AppData), typeof(AppViewModel), typeof(MainWindow));

		public AppViewModel AppData
		{
			get { return (AppViewModel)GetValue(AppDataProperty); }
			set { SetValue(AppDataProperty, value); }
		}

		public MainWindow()
		{
			InitializeComponent();
			InitializeAppData();
			StartMainLoop();
		}

		private void InitializeAppData()
		{
			AppData = new AppViewModel();
			AppData.Strategies.Add(new RefreshPlayerInfoStrategy());
			DataContext = AppData;
		}

		private void StartMainLoop()
		{
			var t = new Thread(async () =>
			{
				while (true)
				{
					try
					{
						await Task.Delay(200);
						if (Application.Current?.Dispatcher != null)
						{
							await Application.Current.Dispatcher.InvokeAsync(async () =>
							{
								await AppData.ExecuteAsync();
							});
						}
					}
					catch (Exception ex)
					{
						AppData.StatusMessage = ex.Message;
					}
				}
			});

			t.IsBackground = true;
			t.Start();
		}

		private void EnsureDlls()
		{
			if (!File.Exists("EliteMMO.Api.dll"))
			{
				new WebClient().DownloadFile("http://ext.elitemmonetwork.com/downloads/elitemmo_api/EliteMMO.API.dll", "EliteMMO.API.dll");
			}

			if (!File.Exists("EliteApi.dll"))
			{
				new WebClient().DownloadFile("http://ext.elitemmonetwork.com/downloads/eliteapi/EliteAPI.dll", "EliteAPI.dll");
			}
		}

		private void PauseButton_Click(object sender, RoutedEventArgs e)
		{
			AppData.IsPaused = !AppData.IsPaused;
		}

		private void RefreshProcessesButton_Click(object sender, RoutedEventArgs e)
		{
			AppData.RefreshProcessList();
		}

		private void HealerInstance_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			EnsureDlls();

			if (e.AddedItems.Count > 0)
			{
				if (e.AddedItems[0] is Process process)
				{
					AppData.SetHealer(process);
				}
				else
				{
					MessageBox.Show("Invalid FFXI process.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		private void MonitoredInstance_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			EnsureDlls();

			if (e.AddedItems.Count > 0)
			{
				if (e.AddedItems[0] is Process process)
				{
					AppData.SetMonitored(process);
				}
				else
				{
					MessageBox.Show("Invalid FFXI process.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
	}
}
