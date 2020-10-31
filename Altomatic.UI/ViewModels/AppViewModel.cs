using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Altomatic.UI.Game;
using Altomatic.UI.Utilities;
using EliteMMO.API;

namespace Altomatic.UI.ViewModels
{
	public class AppViewModel : INotifyPropertyChanged
	{
		IEnumerable<Process> processes;
		public IEnumerable<Process> Processes
		{
			get { return processes; }
			set
			{
				isPaused = true;
				processes = value;
				Monitored = null;
				Healer = null;

				OnPropertyChanged();
				OnPropertyChanged(nameof(IsPaused));
				OnPropertyChanged(nameof(IsReadyToRun));
			}
		}

		EliteAPI healer;
		public EliteAPI Healer
		{
			get { return healer; }
			set
			{
				healer = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsReadyToRun));
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
				OnPropertyChanged(nameof(IsReadyToRun));
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
				OnPropertyChanged(nameof(MainWindowTitle));
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
			set { /* ignore */ }
		}

		public bool IsReadyToRun
		{
			get { return healer != null && monitored != null; }
			set { /* ignore */ }
		}

		public ActionManager ActionManager { get; }
		public List<IGameStrategy> Strategies { get; } = new List<IGameStrategy>();

		public AppViewModel()
    {
      RefreshProcessList();
			InitializePlayers(true);
      ActionManager = new ActionManager(this);
    }

    public void RefreshProcessList()
		{
			Processes = ProcessUtilities.GetProcesses();
			ResetPlayerVitals();
		}

		public void SetHealer(Process process)
		{
			Healer = new EliteAPI(process.Id);
		}

		public void SetMonitored(Process process)
		{
			Monitored = new EliteAPI(process.Id);
		}

		private void InitializePlayers(bool firstRun = false)
		{
				Players.Clear();
				for (var i = 0; i < 18; i++)
				{
					Players.Add(new PlayerViewModel(this));
				}
		}

		private void ResetPlayerVitals()
    {
			for (var i = 0; i < players.Count; i++)
			{
				players[i].ResetVitals();
			}
		}

		public bool CanPerformActions()
    {
			return
				healer.Player.LoginStatus != (int)LoginStatus.LoginScreen &&
				healer.Player.LoginStatus != (int)LoginStatus.Loading &&
				monitored.Player.LoginStatus != (int)LoginStatus.LoginScreen &&
				monitored.Player.LoginStatus != (int)LoginStatus.Loading &&
				(
					healer.Player.Status == (int)EntityStatus.Idle ||
					healer.Player.Status == (int)EntityStatus.Engaged
				);
    }

		public async Task ExecuteAsync()
		{
			if (isPaused) return;

			if (IsReadyToRun && CanPerformActions())
			{
				foreach (var strategy in Strategies)
				{
					StatusMessage = "Executing strategies...";
					if (!await strategy.ExecuteAsync(this))
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
