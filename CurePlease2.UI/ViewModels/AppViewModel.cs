using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CurePlease2.UI.Game;
using EliteMMO.API;

namespace CurePlease2.UI.ViewModels
{
	public class AppViewModel : INotifyPropertyChanged
	{
		EliteAPI healer;
		public EliteAPI Healer
		{
			get { return healer; }
			set
			{
				healer = value;
				OnPropertyChanged();
			}
		}

		EliteAPI monitored;
		public EliteAPI Monitored
		{
			get { return monitored; }
			set
			{
				monitored = value;
				OnPropertyChanged();
			}
		}

		List<PlayerViewModel> players = new List<PlayerViewModel>();
		public List<PlayerViewModel> Players
		{
			get { return players; }
			set
			{
				players = value;
				OnPropertyChanged();
			}
		}

		bool isPaused = true;
		public bool IsPaused
		{
			get { return isPaused; }
			set
			{
				isPaused = value;
				OnPropertyChanged();
			}
		}

		string profile;
		public string Profile
		{
			get { return profile; }
			set
			{
				profile = value;
				OnPropertyChanged();
			}
		}

		string statusMessage;
		public string StatusMessage
		{
			get { return statusMessage; }
			set
			{
				statusMessage = value;
				OnPropertyChanged();
			}
		}

		public string MainWindowTitle
		{
			get { return string.Join(" - ", Constants.MainWindowTitle, Profile ?? "No Profile Loaded"); }
		}


		public ActionManager ActionManager { get; }
		public List<IGameStrategy> Strategies { get; } = new List<IGameStrategy>();

		public AppViewModel()
		{
			ActionManager = new ActionManager(this);
			StatusMessage = "Ready";

			for (var i = 0; i < 18; i++)
			{
				Players.Add(new PlayerViewModel(this));
			}
		}

		public async Task ExecuteAsync()
		{
			if (!isPaused)
			{
				foreach (var strategy in Strategies)
				{
					StatusMessage = "Executing strategies...";
					if (await strategy.ExecuteAsync(this))
					{
						return;
					}
				}
			}

			StatusMessage = "Ready";
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
