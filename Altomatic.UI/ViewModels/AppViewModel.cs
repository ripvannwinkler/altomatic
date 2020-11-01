using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Altomatic.UI.Game;
using Altomatic.UI.Game.Data;
using Altomatic.UI.Game.Strategies;
using Altomatic.UI.Utilities;
using EliteMMO.API;

namespace Altomatic.UI.ViewModels
{
	public class AppViewModel : INotifyPropertyChanged
	{
		EliteAPI healer;
		EliteAPI monitored;
		OptionsViewModel options;
		IEnumerable<Process> processes;
		List<PlayerViewModel> players = new List<PlayerViewModel>();

		string statusMessage;
		bool isPaused = true;

		public Jobs Jobs { get; } = new Jobs();
		public Spells Spells { get; } = new Spells();
		public ActionManager ActionManager { get; }
		public List<IGameStrategy> Strategies { get; } = new List<IGameStrategy>();
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Notify UI of property changes
		/// </summary>
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// <summary>
		/// FFXI process list
		/// </summary>
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
				OnPropertyChanged(nameof(IsGameReady));
			}
		}

		/// <summary>
		/// The healing player instance
		/// </summary>
		public EliteAPI Healer
		{
			get { return healer; }
			set
			{
				healer = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsGameReady));
			}
		}

		/// <summary>
		/// The monitored player instance
		/// </summary>
		public EliteAPI Monitored
		{
			get { return monitored; }
			set
			{
				monitored = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsGameReady));
			}
		}

		/// <summary>
		/// List of players to be cared after
		/// </summary>
		public List<PlayerViewModel> Players
		{
			get { return players; }
			set
			{
				players = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Is the bot paused?
		/// </summary>
		public bool IsPaused
		{
			get { return isPaused; }
			set
			{
				isPaused = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// The status message to display
		/// </summary>
		public string StatusMessage
		{
			get { return statusMessage; }
			set
			{
				statusMessage = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// Gets or sets the options to use
		/// </summary>
		public OptionsViewModel Options
		{
			get { return options; }
			set
			{
				options = value;
				OnPropertyChanged();
			}
		}

		/// <summary>
		/// The title to use for the main window
		/// </summary>
		public string MainWindowTitle
		{
			get
			{
				return string.Join(" - ",
					Constants.MainWindowTitle,
					Options.SettingsFile ?? "No Profile Loaded");
			}
			set { /* ignore */ }
		}

		/// <summary>
		/// Has the user selected both instances?
		/// </summary>
		public bool IsGameReady
		{
			get { return healer != null && monitored != null; }
			set { /* ignore */ }
		}

		/// <summary>
		/// Creates a new instance of <see cref="AppViewModel"/>
		/// </summary>
		public AppViewModel()
		{
			InitializePlayers();
			RefreshProcessList();
			Options = new OptionsViewModel(this);
			ActionManager = new ActionManager(this);
		}

		/// <summary>
		/// Get an updated list of FFXI processes
		/// </summary>
		public void RefreshProcessList()
		{
			StatusMessage = "Refreshing process list...";
			Processes = ProcessUtilities.GetProcesses();
			ResetAllPlayerVitals();
			StatusMessage = "Ready";
		}

		/// <summary>
		/// Set the healer instance
		/// </summary>
		public void SetHealer(Process process)
		{
			Healer = new EliteAPI(process.Id);

			var playerName = Healer?.Player?.Name ?? "";
			var jobNumber = (ushort)(Healer?.Player?.Main ?? -1);
			if (Jobs.TryGetValue(jobNumber, out var jobName))
			{
				Options.Autoload(playerName, jobName);
			}
		}

		/// <summary>
		/// Set the monitored instance
		/// </summary>
		public void SetMonitored(Process process)
		{
			Monitored = new EliteAPI(process.Id);
		}

		/// <summary>
		/// Pauses or unpauses the bot
		/// </summary>
		public void TogglePaused()
		{
			if (IsPaused)
			{
				// handle stuff
				IsPaused = false;
			}
			else
			{
				// handle stuff
				IsPaused = true;
			}
		}

		/// <summary>
		/// Is the healer instance ready to act?
		/// </summary>
		/// <returns></returns>
		public bool CanExecuteStrategies()
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

		/// <summary>
		/// Populate the initial player list
		/// </summary>
		private void InitializePlayers()
		{
			Players.Clear();
			for (var i = 0; i < 18; i++)
			{
				Players.Add(new PlayerViewModel(this));
			}
		}

		/// <summary>
		/// Reset all player vital info
		/// </summary>
		private void ResetAllPlayerVitals()
		{
			for (var i = 0; i < players.Count; i++)
			{
				players[i].ResetVitals();
			}
		}

		/// <summary>
		/// Execute the main action loop
		/// </summary>
		public async Task ExecuteAsync()
		{
			if (isPaused) return;

			try
			{
				if (IsGameReady && CanExecuteStrategies())
				{
					foreach (var strategy in Strategies)
					{
						StatusMessage = $"Executing {strategy.GetType().Name}";
						if (await strategy.ExecuteAsync(this)) return;
					}
				}
			}
			finally
			{
				StatusMessage = "Ready";
			}
		}

	}
}
