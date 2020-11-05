using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Altomatic.UI.ViewModels
{
	public class ConfigViewModel : INotifyPropertyChanged
	{
		#region Healing Magic
		private int curePotency = 50;
		public int CurePotency
		{
			get { return curePotency; }
			set { curePotency = value; OnPropertyChanged(); }
		}

		private int cureThreshold = 80;
		public int CureThreshold
		{
			get { return cureThreshold; }
			set { cureThreshold = value; OnPropertyChanged(); }
		}

		private int curagaThreshold = 80;
		public int CuragaThreshold
		{
			get { return curagaThreshold; }
			set { curagaThreshold = value; OnPropertyChanged(); }
		}

		private int curagaRequiredTargets = 3;
		public int CuragaRequiredTargets
		{
			get { return curagaRequiredTargets; }
			set { curagaRequiredTargets = value; OnPropertyChanged(); }
		}
		#endregion

		#region Enhancing Magic
		private int autoHasteSeconds = 180;
		public int AutoHasteSeconds
		{
			get { return autoHasteSeconds; }
			set { autoHasteSeconds = value; OnPropertyChanged(); }
		}

		private int autoRefreshSeconds = 180;
		public int AutoRefreshSeconds
		{
			get { return autoRefreshSeconds; }
			set { autoRefreshSeconds = value; OnPropertyChanged(); }
		}

		private int autoRegenSeconds = 60;
		public int AutoRegenSeconds
		{
			get { return autoRegenSeconds; }
			set { autoRegenSeconds = value; OnPropertyChanged(); }
		}

		private int autoPhalanxSeconds = 180;
		public int AutoPhalanxSeconds
		{
			get { return autoPhalanxSeconds; }
			set { autoPhalanxSeconds = value; OnPropertyChanged(); }
		}

		private int autoProtectSeconds = 1800;
		public int AutoProtectSeconds
		{
			get { return autoProtectSeconds; }
			set { autoProtectSeconds = value; OnPropertyChanged(); }
		}

		private int autoShellSeconds = 1800;
		public int AutoShellSeconds
		{
			get { return autoShellSeconds; }
			set { autoShellSeconds = value; OnPropertyChanged(); }
		}

		private int autoStormSeconds = 180;
		public int AutoStormSeconds
		{
			get { return autoStormSeconds; }
			set { autoStormSeconds = value; OnPropertyChanged(); }
		}

		private int autoAdloquiumSeconds = 180;
		public int AutoAdloquiumSeconds
		{
			get { return autoAdloquiumSeconds; }
			set { autoAdloquiumSeconds = value; OnPropertyChanged(); }
		}

		private bool selfReraise = false;
		public bool SelfReraise
		{
			get { return selfReraise; }
			set { selfReraise = value; OnPropertyChanged(); }
		}

		private bool selfProtect = false;
		public bool SelfProtect
		{
			get { return selfProtect; }
			set { selfProtect = value; OnPropertyChanged(); }
		}

		private bool selfShell = false;
		public bool SelfShell
		{
			get { return selfShell; }
			set { selfShell = value; OnPropertyChanged(); }
		}

		private bool selfRegen = false;
		public bool SelfRegen
		{
			get { return selfRegen; }
			set { selfRegen = value; OnPropertyChanged(); }
		}

		private bool selfRefresh = false;
		public bool SelfRefresh
		{
			get { return selfRefresh; }
			set { selfRefresh = value; OnPropertyChanged(); }
		}

		private bool selfHaste = false;
		public bool SelfHaste
		{
			get { return selfHaste; }
			set { selfHaste = value; OnPropertyChanged(); }
		}

		private bool selfPhalanx = false;
		public bool SelfPhalanx
		{
			get { return selfPhalanx; }
			set { selfPhalanx = value; OnPropertyChanged(); }
		}

		private bool selfAquaveil = false;
		public bool SelfAquaveil
		{
			get { return selfAquaveil; }
			set { selfAquaveil = value; OnPropertyChanged(); }
		}

		private bool selfBlink = false;
		public bool SelfBlink
		{
			get { return selfBlink; }
			set { selfBlink = value; OnPropertyChanged(); }
		}

		private bool selfStoneskin = false;
		public bool SelfStoneskin
		{
			get { return selfStoneskin; }
			set { selfStoneskin = value; OnPropertyChanged(); }
		}

		private bool selfKlimaform = false;
		public bool SelfKlimaform
		{
			get { return selfKlimaform; }
			set { selfKlimaform = value; OnPropertyChanged(); }
		}

		private bool selfTemper = false;
		public bool SelfTemper
		{
			get { return selfTemper; }
			set { selfTemper = value; OnPropertyChanged(); }
		}

		private bool selfUtsusemi = false;
		public bool SelfUtsusemi
		{
			get { return selfUtsusemi; }
			set { selfUtsusemi = value; OnPropertyChanged(); }
		}

		public string[] Enspells { get; } =
		{
			"",
			"Enfire",
			"Enstone",
			"Enwater",
			"Enaero",
			"Enthunder",
			"Enblizzard",
		};

		private string selfEnspellName = "";
		public string SelfEnspellName
		{
			get { return selfEnspellName; }
			set { selfEnspellName = value; OnPropertyChanged(); }
		}

		public string[] StormSpells { get; } =
		{
			"",
			"Firestorm",
			"Sandstorm",
			"Rainstorm",
			"Windstorm",
			"Thunderstorm",
			"Hailstorm",
			"Aurorastorm",
			"Voidstorm",
		};

		private string selfStormSpellName = "";
		public string SelfStormSpellName
		{
			get { return selfStormSpellName; }
			set { selfStormSpellName = value; OnPropertyChanged(); }
		}

		public string[] SpikesSpells { get; } =
		{
			"",
			"Blaze Spikes",
			"Ice Spikes",
			"Shock Spikes",
		};

		private string selfSpikesSpellName = "";
		public string SelfSpikesSpellName
		{
			get => selfSpikesSpellName;
			set { selfSpikesSpellName = value; OnPropertyChanged(); }
		}

		public string[] BarElementSpells { get; } =
		{
			"",
			"Fire",
			"Earth",
			"Water",
			"Wind",
			"Thunder",
			"Ice",
		};

		private string selfBarElementSpellName = "";
		public string SelfBarElementSpellName
		{
			get => selfBarElementSpellName;
			set { selfBarElementSpellName = value; OnPropertyChanged(); }
		}

		public string[] BarStatusSpells { get; } =
		{
			"",
			"Amnesia",
			"Virus",
			"Paralyze",
			"Petrify",
			"Poison",
			"Blind",
			"Sleep",
		};

		private string selfBarStatusSpellName = "";
		public string SelfBarStatusSpellName
		{
			get => selfBarStatusSpellName;
			set { selfBarStatusSpellName = value; OnPropertyChanged(); }
		}

		#endregion

		#region Debuff Removal
		private bool preferItemOverCursna;
		public bool PreferItemOverCursna
		{
			get => preferItemOverCursna;
			set { preferItemOverCursna = value; OnPropertyChanged(); }
		}

		private bool preferItemOverParalyna;
		public bool PreferItemOverParalyna
		{
			get => preferItemOverParalyna;
			set { preferItemOverParalyna = value; OnPropertyChanged(); }
		}
		#endregion

		#region Job Abilities
		private bool enableAfflatusMisery;
		public bool EnableAfflatusMisery
		{
			get => enableAfflatusMisery;
			set { enableAfflatusMisery = value; OnPropertyChanged(); }
		}

		private bool enableAfflatusSolace;
		public bool EnableAfflatusSolace
		{
			get => enableAfflatusSolace;
			set { enableAfflatusSolace = value; OnPropertyChanged(); }
		}

		private bool enableDevotion;
		public bool EnableDevotion
		{
			get => enableDevotion;
			set { enableDevotion = value; OnPropertyChanged(); }
		}

		private bool enableDivineSeal;
		public bool EnableDivineSeal
		{
			get => enableDivineSeal;
			set { enableDivineSeal = value; OnPropertyChanged(); }
		}

		private bool enableDivineCaress;
		public bool EnableDivineCaress
		{
			get => enableDivineCaress;
			set { enableDivineCaress = value; OnPropertyChanged(); }
		}

		private bool enableComposure;
		public bool EnableComposure
		{
			get => enableComposure;
			set { enableComposure = value; OnPropertyChanged(); }
		}

		private bool enableConvert;
		public bool EnableConvert
		{
			get => enableConvert;
			set { enableConvert = value; OnPropertyChanged(); }
		}

		private bool enableEntrust;
		public bool EnableEntrust
		{
			get => enableEntrust;
			set { enableEntrust = value; OnPropertyChanged(); }
		}

		private bool enableFullCircle;
		public bool EnableFullCircle
		{
			get => enableFullCircle;
			set { enableFullCircle = value; OnPropertyChanged(); }
		}

		private bool enableDematerialize;
		public bool EnableDematerialize
		{
			get => enableDematerialize;
			set { enableDematerialize = value; OnPropertyChanged(); }
		}

		private bool enableBlazeOfGlory;
		public bool EnableBlazeOfGlory
		{
			get => enableBlazeOfGlory;
			set { enableBlazeOfGlory = value; OnPropertyChanged(); }
		}

		private bool enableRadialArcana;
		public bool EnableRadialArcana
		{
			get => enableRadialArcana;
			set { enableRadialArcana = value; OnPropertyChanged(); }
		}

		private bool enableEclipticAttrition;
		public bool EnableEclipticAttrition
		{
			get => enableEclipticAttrition;
			set { enableEclipticAttrition = value; OnPropertyChanged(); }
		}

		private bool enableLifeCycle;
		public bool EnableLifeCycle
		{
			get => enableLifeCycle;
			set { enableLifeCycle = value; OnPropertyChanged(); }
		}

		private bool enableSublimation;
		public bool EnableSublimation
		{
			get => enableSublimation;
			set { enableSublimation = value; OnPropertyChanged(); }
		}

		private bool enableLightArts;
		public bool EnableLightArts
		{
			get => enableLightArts;
			set { enableLightArts = value; OnPropertyChanged(); }
		}

		private bool enableAddendumWhite;
		public bool EnableAddendumWhite
		{
			get => enableAddendumWhite;
			set { enableAddendumWhite = value; OnPropertyChanged(); }
		}
		#endregion

		#region Geomancer
		private bool enableIndiSpells;
		public bool EnableIndiSpells
		{
			get => enableIndiSpells;
			set { enableIndiSpells = value; OnPropertyChanged(); }
		}

		private bool enableGeoSpells;
		public bool EnableGeoSpells
		{
			get => enableGeoSpells;
			set { enableGeoSpells = value; OnPropertyChanged(); }
		}

		private bool enableGeomancyWhenEngagedOnly;
		public bool EnableGeomancyWhenEngagedOnly
		{
			get => enableGeomancyWhenEngagedOnly;
			set { enableGeomancyWhenEngagedOnly = value; OnPropertyChanged(); }
		}

		private bool disableGeoDebuffs;
		public bool DisableGeoDebuffs
		{
			get => disableGeoDebuffs;
			set { disableGeoDebuffs = value; OnPropertyChanged(); }
		}

		public string[] GeoSpells
		{
			get => new string[]
			{
				"Acumen",
				"Attunement",
				"Barrier",
				"Fade",
				"Fend",
				"Focus",
				"Frailty",
				"Fury",
				"Gravity",
				"Haste",
				"Languor",
				"Malaise",
				"Paralysis",
				"Poison",
				"Precision",
				"Refresh",
				"Regen",
				"Slip",
				"Slow",
				"Torpor",
				"Vex",
				"Voidance",
				"Wilt",
				"AGI",
				"CHR",
				"DEX",
				"INT",
				"MND",
				"STR",
				"VIT",
			};
		}

		public string[] GeoDebuffs
		{
			get => new string[]
			{
				"Fade",
				"Frailty",
				"Gravity",
				"Languor",
				"Malaise",
				"Paralysis",
				"Poison",
				"Slip",
				"Slow",
				"Torpor",
				"Vex",
				"Wilt",
			};
		}

		private string indiSpellName;
		public string IndiSpellName
		{
			get => indiSpellName;
			set { indiSpellName = value; OnPropertyChanged(); }
		}

		private string entrustIndiSpellName;
		public string EntrustIndiSpellName
		{
			get => entrustIndiSpellName;
			set { entrustIndiSpellName = value; OnPropertyChanged(); }
		}

		private string geoSpellName;
		public string GeoSpellName
		{
			get => geoSpellName;
			set { geoSpellName = value; OnPropertyChanged(); }
		}
		#endregion

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		#endregion
	}
}
