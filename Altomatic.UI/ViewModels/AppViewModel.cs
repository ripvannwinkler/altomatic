using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
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
		private EliteAPI healer;
		private EliteAPI monitored;
		private OptionsViewModel options;
		private ObservableCollection<Process> processes;
		private ObservableCollection<PlayerViewModel> players = new ObservableCollection<PlayerViewModel>();
		private Process healerProcess;
		private string statusMessage;
		private bool isPaused = true;
		private bool isAddonLoaded = false;

		public JobNames Jobs { get; }
		public ActionManager Actions { get; }
		public AddonInterface Addon { get; } = new AddonInterface();
		public List<IGameStrategy> Strategies { get; } = new List<IGameStrategy>();
		public event PropertyChangedEventHandler PropertyChanged;

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
		/// FFXI process list
		/// </summary>
		public ObservableCollection<Process> Processes
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
		/// List of players to be cared after
		/// </summary>
		public ObservableCollection<PlayerViewModel> Players
		{
			get { return players; }
			set
			{
				players = value;
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
		/// Is the game addon loaded?
		/// </summary>
		public bool IsAddonLoaded
		{
			get { return isAddonLoaded; }
			set
			{
				isAddonLoaded = value;
				OnPropertyChanged();
			}
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
			InitializePlayerData();
			RefreshProcessList();

			Jobs = new JobNames(this);
			Options = new OptionsViewModel(this);
			Actions = new ActionManager(this);

			Addon.Events.Subscribe((@event) =>
			{
				switch (@event.Type)
				{
					case AddonEventType.CastingStarted:
					case AddonEventType.CastingCompleted:
					case AddonEventType.CastingInteruppted:
						Actions.ProcessAddonEvent(@event);
						break;

					case AddonEventType.BuffsUpdated:
						break;

					case AddonEventType.Loaded:
						IsAddonLoaded = true;
						SetStatus("Addon loaded successfully.");
						break;
				}
			});
		}

		/// <summary>
		/// Get an updated list of FFXI processes
		/// </summary>
		public void RefreshProcessList()
		{
			SetStatus("Refreshing process list...");
			Processes = new ObservableCollection<Process>(ProcessUtilities.GetProcesses());
			ResetPlayerData();
			SetStatus();
		}

		/// <summary>
		/// Set the healer instance
		/// </summary>
		public void SetHealer(Process process)
		{
			healerProcess = process;
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
		/// Updates the application status
		/// </summary>
		/// <param name="message">The message to display (defaults to 'Ready')</param>
		public void SetStatus(string message = "Ready")
		{
			StatusMessage = message;
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

		public async Task ReloadAddon()
		{
			if (!IsGameReady) return;
			var mode = ProcessUtilities.GetHookMode(healerProcess);
			var ip = Addon.Endpoint.Address;
			var port = Addon.Endpoint.Port;

			if (mode == HookMode.Ashita)
			{
				await healer.SendCommand($"/addon unload altomatic", 300);
				await healer.SendCommand($"/addon load altomatic", 300);
				await healer.SendCommand($"/alto config {ip} {port}", 100);
			}
			else if (mode == HookMode.Windower)
			{
				await healer.SendCommand($"//lua unload altomatic", 300); Thread.Sleep(300);
				await healer.SendCommand($"//lua load altomatic", 300); Thread.Sleep(1500);
				await healer.SendCommand($"//alto config {ip} {port}", 100);
			}
		}

		/// <summary>
		/// Execute the main action loop
		/// </summary>
		public async Task ExecuteActionsAsync()
		{
			if (isPaused) return;

			try
			{
				if (IsGameReady && CanExecuteActions())
				{
					foreach (var strategy in Strategies)
					{
						SetStatus($"Executing {strategy.GetType().Name}");
						if (await strategy.ExecuteAsync(this)) return;
					}
				}
			}
			finally
			{
				SetStatus();
			}
		}


		/// <summary>
		/// Notify UI of property changes
		/// </summary>
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		/// <summary>
		/// Is the healer instance ready to act?
		/// </summary>
		/// <returns></returns>
		private bool CanExecuteActions()
		{
			return
				isAddonLoaded &&
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
		/// Populate initial player data
		/// </summary>
		private void InitializePlayerData()
		{
			Players.Clear();
			for (var i = 0; i < 18; i++)
			{
				Players.Add(new PlayerViewModel(this));
			}
		}

		/// <summary>
		/// Reset all players
		/// </summary>
		private void ResetPlayerData()
		{
			for (var i = 0; i < players.Count; i++)
			{
				players[i].ResetVitals();
			}
		}
	}
}
