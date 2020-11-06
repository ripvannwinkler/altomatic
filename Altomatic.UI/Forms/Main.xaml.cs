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
			new Thread(async () =>
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
								await Model.ExecuteActionsAsync();
							});
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.ToString());
					}
				}
			})
			{
				IsBackground = true
			}.Start();
		}

		private void PauseButton_Click(object sender, RoutedEventArgs e)
		{
			Model.TogglePaused();
		}

		private async void RefreshProcessesButton_Click(object sender, RoutedEventArgs e)
		{
			await Model.RefreshProcessList();
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

		private async void ReloadAddonButton_Click(object sender, RoutedEventArgs e)
		{
			await Model.ReloadAddon();
		}

		private async void HealerInstance_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
			{
				ProcessUtilities.EnsureDlls();
				await Model.SetHealer(e.AddedItems[0] as Process);
			}
		}

		private void MonitoredInstance_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0)
			{
				ProcessUtilities.EnsureDlls();
				Model.SetMonitored(e.AddedItems[0] as Process);
			}
		}

    private async void RefreshPlayersButton_Click(object sender, RoutedEventArgs e)
    {
			await new RefreshPlayerInfoStrategy().ExecuteAsync(Model);
    }
  }
}
