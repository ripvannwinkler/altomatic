using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
		// ================================================================
		// Member Variables
		// ================================================================

		private EliteAPI healer;
		private EliteAPI monitored;
		private OptionsViewModel options;
		private ObservableCollection<Process> processes;
		private ObservableCollection<PlayerViewModel> players = new ObservableCollection<PlayerViewModel>();
		private SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);
		private Process healerProcess;
		private string statusMessage;
		private bool isPaused = true;
		private bool isAddonLoaded = false;

		// ================================================================
		// Public Non-UI Properties
		// ================================================================

		public Buffs Buffs { get; }
		public Spells Spells { get; }
		public Jobs Jobs { get; }
		public ActionManager Actions { get; }
		public AddonInterface Addon { get; } = new AddonInterface();
		public List<IGameStrategy> Strategies { get; } = new List<IGameStrategy>();
		public event PropertyChangedEventHandler PropertyChanged;

		// ================================================================
		// Public UI Properties
		// ================================================================

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
				OnPropertyChanged(nameof(ActivePlayers));
			}
		}

		public IEnumerable<PlayerViewModel> ActivePlayers
		{
			get { return Players.Where(p => p.IsActive); }
			set { /* ignore */ }
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

		// ================================================================
		// Public Methods
		// ================================================================

		/// <summary>
		/// Creates a new instance of <see cref="AppViewModel"/>
		/// </summary>
		public AppViewModel()
		{
			InitializePlayerData();

			Strategies.Add(new ValidateProcessStrategy());
			Strategies.Add(new RefreshPlayerInfoStrategy());
			Strategies.Add(new RemoveCriticalDebuffStrategy());
			Strategies.Add(new RemoveDebuffStrategy());
			Strategies.Add(new AutoBuffsStrategy());
			Strategies.Add(new CuragaStrategy());
			Strategies.Add(new CureStrategy());

			Jobs = new Jobs(this);
			Buffs = new Buffs(this);
			Spells = new Spells(this);
			Options = new OptionsViewModel(this);
			Actions = new ActionManager(this);

			Addon.Events.Subscribe((@event) =>
			{
				switch (@event.Type)
				{
					case AddonEventType.BuffsUpdated:
						var data = @event.Data.Split('_');
						if (data.Length == 3)
						{
							var delims = new[] { ',' };
							var buffCodes = data[2].Split(delims, StringSplitOptions.RemoveEmptyEntries);
							var buffs = buffCodes.Select(b => short.Parse(b));
							Buffs.Update(data[1], buffs.ToArray());
						}
						break;

					case AddonEventType.Loaded:
						IsAddonLoaded = true;
						SetStatus("Addon loaded successfully.");
						break;
				}
			});

			Task.Run(async () =>
			{
				await RefreshProcessList();
			});
		}

		/// <summary>
		/// Get an updated list of FFXI processes
		/// </summary>
		public async Task RefreshProcessList()
		{
			SetStatus("Refreshing process list...");
			await UnloadAddon();
			Processes = new ObservableCollection<Process>(ProcessUtilities.GetProcesses());
			ResetPlayerData();
			SetStatus();
		}

		/// <summary>
		/// Set the healer instance
		/// </summary>
		public async Task SetHealer(Process process)
		{
			healerProcess = process;
			Healer = new EliteAPI(process.Id);
			await ReloadAddon();

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

		/// <summary>
		/// Unloads the game addon
		/// </summary>
		public async Task UnloadAddon()
		{
			if (!IsAddonLoaded) return;
			if (string.IsNullOrEmpty(Healer?.Player?.Name))
			{
				IsAddonLoaded = false;
				return;
			}

			var mode = ProcessUtilities.GetHookMode(healerProcess);

			if (mode == HookMode.Ashita)
			{
				await Healer.SendCommand($"/addon unload altomatic", 300);
				IsAddonLoaded = false;
			}
			else if (mode == HookMode.Windower)
			{
				await Healer.SendCommand($"//lua unload altomatic", 300);
				IsAddonLoaded = false;
			}
		}

		/// <summary>
		/// Reloads the game addon
		/// </summary>
		public async Task ReloadAddon()
		{
			if (string.IsNullOrEmpty(Healer.Player.Name)) return;
			var mode = ProcessUtilities.GetHookMode(healerProcess);
			var ip = Addon.Endpoint.Address;
			var port = Addon.Endpoint.Port;

			if (mode == HookMode.Ashita)
			{
				await UnloadAddon();
				Buffs.Clear();

				await Healer.SendCommand($"/addon load altomatic", 300);
				await Healer.SendCommand($"/alto config {ip} {port}", 100);
			}
			else if (mode == HookMode.Windower)
			{
				await UnloadAddon();
				Buffs.Clear();

				await Healer.SendCommand($"//lua load altomatic", 300);
				await Healer.SendCommand($"//alto config {ip} {port}", 100);
			}
		}

		/// <summary>
		/// Execute the main action loop
		/// </summary>
		public async Task ExecuteActionsAsync()
		{
			if (isPaused) return;
			if (await semaphore.WaitAsync(100))
			{
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
					semaphore.Release();
					SetStatus();
				}
			}
		}

		// ================================================================
		// Protected and Private Methods
		// ================================================================

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
