using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Altomatic.UI.ViewModels
{
	public class OptionsViewModel : INotifyPropertyChanged
	{
		AppViewModel App;
		Config config = new Config();
		string settingsDir = Path.Combine(Environment.CurrentDirectory, "settings");
		string settingsFile;

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public Config Config
		{
			get { return config; }
			set
			{
				config = value;
				OnPropertyChanged();
			}
		}

		public string SettingsFile
		{
			get { return settingsFile; }
			set
			{
				settingsFile = value;
				OnPropertyChanged();
			}
		}

		public OptionsViewModel(AppViewModel app)
		{
			App = app ?? throw new ArgumentNullException(nameof(app));
			Directory.CreateDirectory(settingsDir);
		}

		public void Save()
		{
			if (!string.IsNullOrEmpty(SettingsFile))
			{
				using var writer = File.CreateText(SettingsFile);
				var xml = new XmlSerializer(typeof(Config));
				xml.Serialize(writer, Config);
			}
		}

		public void SaveAs()
		{
			var fd = new SaveFileDialog();
			fd.InitialDirectory = settingsDir;
			fd.Filter = "Settings Files | *.xml";
			if (fd.ShowDialog() == DialogResult.OK)
			{
				SettingsFile = fd.FileName;
				Save();
			}
		}

		public void Load()
		{
			if (!string.IsNullOrWhiteSpace(SettingsFile))
			{
				using var reader = File.OpenText(SettingsFile);
				var xml = new XmlSerializer(typeof(Config));
				Config = (Config)xml.Deserialize(reader);
			}
		}

		public void LoadFrom()
		{
			var fd = new OpenFileDialog();
			fd.InitialDirectory = settingsDir;
			fd.Filter = "Settings Files | *.xml";
			if (fd.ShowDialog() == DialogResult.OK)
			{
				SettingsFile = fd.FileName;
				Load();
			}
		}

		public void Autoload(string playerName, string jobName)
		{
			if (!string.IsNullOrWhiteSpace(playerName))
			{
				if (File.Exists($@"settings\{playerName}_{jobName}.xml"))
				{
					SettingsFile = $@"settings\{playerName}_{jobName}.xml";
					Load();
				}
				else if (File.Exists($@"settings\{jobName}.xml"))
				{
					SettingsFile = $@"settings\{jobName}.xml";
					Load();
				}
			}
		}
	}

	public class Config : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		int curePotency = 50;
		int cureThreshold = 80;
		int curagaThreshold = 80;
		int curagaRequiredTargets = 3;
		int autoHasteSeconds = 300;

		bool selfHaste = true;
		bool selfRefresh = true;
		bool selfPhalanx = true;

		public int CurePotency
		{
			get { return curePotency; }
			set
			{
				curePotency = value;
				OnPropertyChanged();
			}
		}

		public int CureThreshold
		{
			get { return cureThreshold; }
			set
			{
				cureThreshold = value;
				OnPropertyChanged();
			}
		}

		public int CuragaThreshold
		{
			get { return curagaThreshold; }
			set
			{
				curagaThreshold = value;
				OnPropertyChanged();
			}
		}

		public int CuragaRequiredTargets
		{
			get { return curagaRequiredTargets; }
			set
			{
				curagaRequiredTargets = value;
				OnPropertyChanged();
			}
		}

		public int AutoHasteSeconds
    {
      get { return autoHasteSeconds; }
      set
      {
				autoHasteSeconds = value;
				OnPropertyChanged();
      }
    }


		public bool SelfHaste
    {
      get { return selfHaste; }
      set
      {
				selfHaste = value;
				OnPropertyChanged();
      }
    }

		public bool SelfRefresh
    {
      get { return selfRefresh; }
      set
      {
				selfRefresh = value;
				OnPropertyChanged();
      }
    }

		public bool SelfPhalanx
		{
			get { return selfPhalanx; }
			set
			{
				selfPhalanx = value;
				OnPropertyChanged();
			}
		}
	}
}
