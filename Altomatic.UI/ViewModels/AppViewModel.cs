using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using Altomatic.UI.Game;
using Altomatic.UI.Game.Data;
using Altomatic.UI.Game.Strategies;
using Altomatic.UI.Utilities;
using EliteMMO.API;

namespace Altomatic.UI.ViewModels
{
	public class AppViewModel : INotifyPropertyChanged
	{
		private readonly AsyncLock guard = new AsyncLock();
		private readonly ObservableCollection<BuffStatus> activeBuffs = new ObservableCollection<BuffStatus>();

		private EliteAPI healer;
		private EliteAPI monitored;
		private OptionsViewModel options;
		private ObservableCollection<Process> processes;
		private ObservableCollection<PlayerViewModel> players = new ObservableCollection<PlayerViewModel>();
		private RefreshPlayerInfoStrategy refreshPlayerInfo = new RefreshPlayerInfoStrategy();
		private Process healerProcess;
		private string statusMessage;
		private short lastKnownRoll;
		private bool isBusy = false;
		private bool isPaused = true;
		private bool isAddonLoaded = false;
		private bool isPlayerMoving = false;
		private int lastZone = -1;
		private ulong loopCount = 0;
		private Point3D lastPosition;

		public Buffs Buffs { get; }
		public Spells Spells { get; }
		public Jobs Jobs { get; }
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
				OnPropertyChanged(nameof(AppTitle));
				OnPropertyChanged(nameof(IsGameReady));
				OnPropertyChanged(nameof(IsHealerSet));
				OnPropertyChanged(nameof(HealerMainJob));
				OnPropertyChanged(nameof(HealerSubJob));
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

		/// <summary>
		/// List of players who are active (alive, in range, etc.)
		/// </summary>
		public IEnumerable<PlayerViewModel> ActivePlayers
		{
			get { return Players.Where(p => p.IsActive); }
			set { /* ignore */ }
		}

		/// <summary>
		/// List of active player buffs
		/// </summary>
		public ObservableCollection<BuffStatus> ActiveBuffs
		{
			get => activeBuffs;
			set { /* ignore */ }
		}

		/// <summary>
		/// How many times as the action loop executed?
		/// </summary>
		/// <remarks>
		/// This is useful for determining if the bot is hung up or just idle.
		/// </remarks>
		public ulong LoopCount
		{
			get => loopCount;
			set { loopCount = value; OnPropertyChanged(); }
		}

		/// <summary>
		/// The healer's main job
		/// </summary>
		public string HealerMainJob
		{
			get => Jobs.GetMainJob(Healer?.Player?.MainJob ?? 99);
		}

		/// <summary>
		/// The healer's sub job
		/// </summary>
		public string HealerSubJob
		{
			get => Jobs.GetMainJob(Healer?.Player?.SubJob ?? 99);
		}

		/// <summary>
		/// The application window title
		/// </summary>
		public string AppTitle
		{
			get
			{
				var assembly = Assembly.GetExecutingAssembly();
				var version = FileVersionInfo.GetVersionInfo(assembly.Location);
				var buildDate = new FileInfo(assembly.Location).LastWriteTime;
				var healer = Healer?.Player?.Name;

				return string.IsNullOrWhiteSpace(healer)
					? $"Altomatic {version.ProductVersion} - {buildDate}"
					: $"[{healer}] - Altomatic {version.ProductVersion} - {buildDate}";
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
		/// The last known corsair roll value
		/// </summary>
		public short LastKnownRoll
		{
			get => lastKnownRoll;
			set { lastKnownRoll = value; OnPropertyChanged(); }
		}

		/// <summary>
		/// Is the application busy performing a user task?
		/// </summary>
		public bool IsBusy
		{
			get => isBusy;
			set
			{
				isBusy = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsNotBusy));
			}
		}

		/// <summary>
		/// Is the application ready to perform a user task?
		/// </summary>
		public bool IsNotBusy
		{
			get => !IsBusy;
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
				OnPropertyChanged(nameof(IsRunning));
			}
		}

		/// <summary>
		/// Is the bot running
		/// </summary>
		public bool IsRunning
		{
			get => IsPaused;
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
			get { return Healer != null && Monitored != null && IsNotBusy; }
			set { /* ignore */ }
		}

		/// <summary>
		/// Is the player moving?
		/// </summary>
		public bool IsPlayerMoving
		{
			get => isPlayerMoving;
			set { isPlayerMoving = value; OnPropertyChanged(); }
		}

		public bool IsHealerSet
    {
			get => !string.IsNullOrWhiteSpace(Healer?.Player?.Name);
		}

		/// <summary>
		/// List of users targeted for rolls
		/// </summary>
		public string RollTargets
		{
			get
			{
				var players = Players.Where(x => x.IsRequiredForRolls);
				return players.Any() ? string.Join(", ", players.Select(x => x.Name)) : "Any";
			}
		}

		/// <summary>
		/// Is this user the entrust/indi target?
		/// </summary>
		public string EntrustTarget
		{
			get
			{
				var player = Players.FirstOrDefault(x => x.IsEntrustTarget);
				return player?.Name ?? "None";
			}
		}

		/// <summary>
		/// Is this player the geo bubble target?
		/// </summary>
		public string GeoTarget
		{
			get
			{
				var player = Players.FirstOrDefault(x => x.IsGeoTarget);
				return player?.Name ?? "None";
			}
		}


		/// <summary>
		/// Creates a new instance of <see cref="AppViewModel"/>
		/// </summary>
		public AppViewModel()
		{
			InitializePlayerData();

			Strategies.Add(new ReloadAddonStrategy());
			Strategies.Add(new AcceptRaiseStrategy());
			Strategies.Add(new RemoveCriticalDebuffStrategy());
			Strategies.Add(new CureWarningStrategy());
			Strategies.Add(new CuragaStrategy());
			Strategies.Add(new CureStrategy());

			Strategies.Add(new SleepCharmedPlayersStrategy());
			Strategies.Add(new RemoveDebuffStrategy());
			Strategies.Add(new RaiseTheDeadStrategy());
			Strategies.Add(new JobAbilitiesStrategy());
			Strategies.Add(new AutoBuffsStrategy());
			Strategies.Add(new CorsairRollStrategy());
			Strategies.Add(new GeomancerStrategy());

			Jobs = new Jobs(this);
			Buffs = new Buffs(this);
			Spells = new Spells(this);
			Options = new OptionsViewModel(this);
			Actions = new ActionManager(this);

			Addon.Events.Subscribe((@event) =>
			{
				string[] data;
				switch (@event.Type)
				{
					case AddonEventType.BuffsUpdated:
						data = @event.Data.Split('_');
						if (data.Length == 3)
						{
							var delims = new[] { ',' };
							var buffCodes = data[2].Split(delims, StringSplitOptions.RemoveEmptyEntries);
							var buffs = buffCodes.Select(b => short.Parse(b));
							Buffs.Update(data[1], buffs.ToArray());
						}
						break;

					case AddonEventType.CorsairRoll:
						data = @event.Data.Split('_');
						if (data.Length == 2)
						{
							if (short.TryParse(data[1], out var roll))
							{
								LastKnownRoll = roll;
							}
						}
						break;

					case AddonEventType.Loaded:
						IsAddonLoaded = true;
						SetStatus();
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
			await Application.Current.Dispatcher?.Invoke<Task>(async () =>
			{
				await UnloadAddon();

				Pause();
				LastKnownRoll = -1;
				ActiveBuffs.Clear();
				Options.SettingsFile = null;
				Processes = new ObservableCollection<Process>(ProcessUtilities.GetProcesses());
				ResetPlayerData();
			});
		}

		/// <summary>
		/// Set the healer instance
		/// </summary>
		public async Task SetHealer(Process process)
		{
			LoopCount = 0;
			LastKnownRoll = -1;
			ActiveBuffs.Clear();
			Options.SettingsFile = null;

			healerProcess = process;
			Healer = new EliteAPI(process.Id);
			await ReloadAddon();

			var playerName = Healer?.Player?.Name ?? "";
			var jobNumber = (ushort)(Healer?.Player?.MainJob ?? -1);
			if (Jobs.TryGetValue(jobNumber, out var jobName))
			{
				Options.Autoload(playerName, jobName);
			}
		}

		/// <summary>
		/// Set the monitored instance
		/// </summary>
		public async Task SetMonitored(Process process)
		{
			Monitored = new EliteAPI(process.Id);
			await new RefreshPlayerInfoStrategy().ExecuteAsync(this);
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
		/// Pauses the bot
		/// </summary>
		public void Pause()
		{
			IsPaused = true;
			ActiveBuffs.Clear();

			lastKnownRoll = -1;
			lastPosition = new Point3D();
		}

		/// <summary>
		/// Unpauses the bot
		/// </summary>
		public void Unpause()
		{
			IsPaused = false;
		}

		public async Task Heartbeat()
		{
			var mode = ProcessUtilities.GetHookMode(healerProcess);

			switch (mode)
			{
				case HookMode.Ashita: await Healer.SendCommand("/alto heartbeat", 200); break;
				case HookMode.Windower: await Healer.SendCommand("//alto heartbeat", 200); break;
			}
		}

		/// <summary>
		/// Unloads the game addon
		/// </summary>
		public async Task UnloadAddon()
		{
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
			if (healerProcess == null) return;
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
			if (IsGameReady)
			{
				DetectMovement();
				PauseIfZoning();
				await refreshPlayerInfo.ExecuteAsync(this);

				if (!IsPaused && CanExecuteActions())
				{
					LoopCount++;

					foreach (var strategy in Strategies)
					{
						if (IsPaused) break;
						if (await strategy.ExecuteAsync(this)) return;
					}

					SetStatus();
				}
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
				monitored.Player.LoginStatus != (int)LoginStatus.Loading;
		}

		/// <summary>
		/// Populate initial player data
		/// </summary>
		private void InitializePlayerData()
		{
			Players.Clear();
			for (var i = 0; i < 18; i++)
			{
				var player = new PlayerViewModel(this);
				player.PropertyChanged += (s, e) =>
				{
					if (e.PropertyName == nameof(player.Name))
					{
						OnPropertyChanged(nameof(player.IsRequiredForRolls));
						OnPropertyChanged(nameof(player.IsEntrustTarget));
						OnPropertyChanged(nameof(player.IsGeoTarget));
					}
					else if (e.PropertyName == nameof(player.IsRequiredForRolls))
					{
						OnPropertyChanged(nameof(RollTargets));
					}
					else if (e.PropertyName == nameof(player.IsEntrustTarget))
					{
						OnPropertyChanged(nameof(EntrustTarget));
					}
					else if (e.PropertyName == nameof(player.IsGeoTarget))
					{
						OnPropertyChanged(nameof(GeoTarget));
					}
				};

				Players.Add(player);
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

		/// <summary>
		/// Pauses the bot if player is zoning
		/// </summary>
		private void PauseIfZoning()
		{
			var zoneId = Healer?.Player?.ZoneId ?? -1;

			if (zoneId >= 0)
			{
				if (lastZone < 0)
				{
					lastZone = zoneId;
				}
				else if (zoneId != lastZone)
				{
					Pause();
					LastKnownRoll = -1;
					SetStatus("Paused due to zoning...");
					lastZone = zoneId;
				}
			}
		}

		/// <summary>
		/// Detect whether the healer is moving
		/// </summary>
		public void DetectMovement()
		{
			if (Healer == null) return;
			if (Healer.Player.LoginStatus == (int)LoginStatus.LoginScreen ||
					Healer.Player.LoginStatus == (int)LoginStatus.Loading)
			{
				return;
			}

			var currentPosition = new Point3D(
				Healer.Player.X,
				Healer.Player.Y,
				Healer.Player.Z);

			Debug.WriteLine(currentPosition);
			Debug.WriteLine(lastPosition);

			if (currentPosition.X != lastPosition.X ||
					currentPosition.Y != lastPosition.Y ||
					currentPosition.Z != lastPosition.Z)
			{
				lastPosition = currentPosition;
				IsPlayerMoving = true;
			}
			else
			{
				IsPlayerMoving = false;
			}
		}
	}
}
