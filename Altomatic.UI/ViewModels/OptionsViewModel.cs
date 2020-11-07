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
		private static readonly string settingsDir = Path.Combine(Environment.CurrentDirectory, "settings");
		private static readonly string settingsFilter = "Settings Files | *.xml";

		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
		#endregion

		private ConfigViewModel config;
		public ConfigViewModel Config
		{
			get { return config; }
			set { config = value; OnPropertyChanged(); }
		}

		private AppViewModel appData;
		public AppViewModel AppData
		{
			get { return appData; }
			set { appData = value; OnPropertyChanged(); }
		}

		private string settingsFile;
		public string SettingsFile
		{
			get { return settingsFile; }
			set
			{
				settingsFile = value; OnPropertyChanged();
				OnPropertyChanged(nameof(SettingsFilenameOnly));
			}
		}

		public string SettingsFilenameOnly
		{
			get => string.IsNullOrWhiteSpace(SettingsFile)
				? "None" : Path.GetFileNameWithoutExtension(SettingsFile);
		}

		public OptionsViewModel(AppViewModel app)
		{
			Config = new ConfigViewModel();
			AppData = app ?? throw new ArgumentNullException(nameof(app));
			Directory.CreateDirectory(settingsDir);
		}

		/// <summary>
		/// Saves settings the current <see cref="SettingsFile"/>, if specified
		/// </summary>
		public void Save()
		{
			if (!string.IsNullOrEmpty(SettingsFile))
			{
				using var writer = File.CreateText(SettingsFile);
				var xml = new XmlSerializer(typeof(ConfigViewModel));
				xml.Serialize(writer, Config);
			}
		}

		/// <summary>
		/// Saves settings to a new <see cref="SettingsFile"/>
		/// </summary>
		public void SaveAs()
		{
			var fd = new SaveFileDialog();
			fd.InitialDirectory = settingsDir;
			fd.Filter = settingsFilter;

			if (fd.ShowDialog() == DialogResult.OK)
			{
				var temp = fd.FileName;
				if (!temp.ToLower().EndsWith(".xml"))
				{
					temp += ".xml";
				}

				SettingsFile = temp;
				Save();
			}
		}

		/// <summary>
		/// Loads settings from the current <see cref="SettingsFile"/>, if specified
		/// </summary>
		public void Load()
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(SettingsFile))
				{
					using var reader = File.OpenText(SettingsFile);
					var xml = new XmlSerializer(typeof(ConfigViewModel));
					Config = (ConfigViewModel)xml.Deserialize(reader);
				}
			}
			catch (Exception ex)
			{
				AppData.SetStatus($"Error loading profile '{SettingsFilenameOnly}'. {ex.Message}");

			}
		}

		/// <summary>
		/// Loads settings from a new <see cref="SettingsFile"/>
		/// </summary>
		public void LoadFrom()
		{
			var fd = new OpenFileDialog();
			fd.InitialDirectory = settingsDir;
			fd.Filter = settingsFilter;

			if (fd.ShowDialog() == DialogResult.OK)
			{
				SettingsFile = fd.FileName;
				Load();
			}
		}

		/// <summary>
		/// Loads settings based on current player and job, if possible
		/// </summary>
		public void Autoload(string playerName, string jobName)
		{
			SettingsFile = null;
			if (!string.IsNullOrWhiteSpace(playerName))
			{
				var filesToTry = new[]
				{
					$@"{playerName}_{jobName}.xml",
					$@"{jobName}.xml",
				};

				foreach (var file in Directory.GetFiles(settingsDir))
				{
					foreach (var myFile in filesToTry)
					{
						var name1 = Path.GetFileName(file).ToLower();
						var name2 = myFile.ToLower();

						if (name1 == name2)
						{
							SettingsFile = file;
							Load(); break;
						}
					}
				}
			}
		}
	}
}
