using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Altomatic.UI.Game.Strategies;
using Altomatic.UI.Utilities;
using Altomatic.UI.ViewModels;

namespace Altomatic.UI.Forms
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly static DependencyProperty appDataProperty = DependencyProperty.Register(nameof(Model), typeof(AppViewModel), typeof(MainWindow));

		private bool started = false;

		public AppViewModel Model
		{
			get { return (AppViewModel)GetValue(appDataProperty); }
			set { SetValue(appDataProperty, value); }
		}

		public MainWindow()
		{
			Style = (Style)FindResource(typeof(Window));
			InitializeComponent();
			InitializeAppData();
			StartMainLoop();
		}

		private void InitializeAppData()
		{
			Model = new AppViewModel();
			DataContext = Model;

			Closing += async (sender, args) =>
			{
				await Dispatcher.InvokeAsync(async () =>
				{
					await Model.UnloadAddon();
				});
			};
		}

		private void StartMainLoop()
		{
			if (started)
			{
				throw new InvalidOperationException("Main loop already started.");
			}

			started = true;
			var semaphore = new SemaphoreSlim(1);
			new Thread(async () =>
			{
				while (true)
				{
					semaphore.Wait();

					try
					{
						if (Application.Current?.Dispatcher == null) continue;
						await Application.Current.Dispatcher.InvokeAsync(ExecuteActions).Result;
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.ToString());
					}
					finally
					{
						semaphore.Release();
					}
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		private async Task ExecuteActions()
		{
			Debug.WriteLine("Executing main action loop...");
			await Model.ExecuteActionsAsync();
			await Task.Delay(500);
		}

		private async void HealerInstance_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
			{
				ProcessUtilities.EnsureDlls();
				await Model.SetHealer(e.AddedItems[0] as Process);
			}
		}

		private async void MonitoredInstance_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
			{
				ProcessUtilities.EnsureDlls();
				await Model.SetMonitored(e.AddedItems[0] as Process);
			}
		}

		private void PauseButton_Click(object sender, RoutedEventArgs e)
		{
			if (Model.IsPaused)
			{
				Model.Unpause();
			}
			else
			{
				Model.Pause();
			}
		}

		private void OptionsButton_Click(object sender, RoutedEventArgs e)
		{
			var window = new Options(Model.Options)
			{
				Owner = this,
				WindowStartupLocation = WindowStartupLocation.CenterOwner
			};

			window.ShowDialog();
		}

		private async void RefreshProcessesButton_Click(object sender, RoutedEventArgs e)
		{
			if (!Model.IsBusy)
			{
				Model.SetStatus("Refreshing processes...");

				Model.IsBusy = true;
				await Model.RefreshProcessList();
				Model.IsBusy = false;
				Model.SetStatus();
			}
		}

		private async void ReloadAddonButton_Click(object sender, RoutedEventArgs e)
		{
			if (!Model.IsBusy)
			{
				Model.SetStatus("Reloading addon...");

				Model.IsBusy = true;
				await Model.ReloadAddon();
				Model.IsBusy = false;
				Model.SetStatus();
			}
		}

		private async void RefreshPlayersButton_Click(object sender, RoutedEventArgs e)
		{
			if (!Model.IsBusy)
			{
				Model.SetStatus("Refreshing players...");

				Model.IsBusy = true;
				await new RefreshPlayerInfoStrategy().ExecuteAsync(Model);
				Model.IsBusy = false;
				Model.SetStatus();
			}
		}
	}
}
