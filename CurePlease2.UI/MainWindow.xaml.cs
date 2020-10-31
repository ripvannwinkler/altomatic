﻿using CurePlease2.UI.Game;
using CurePlease2.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace CurePlease2.UI
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
			AppData.Strategies.Add(new DummyGameStrategy());
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

		private void PauseButton_Click(object sender, RoutedEventArgs e)
		{
			AppData.IsPaused = !AppData.IsPaused;
		}
	}
}
