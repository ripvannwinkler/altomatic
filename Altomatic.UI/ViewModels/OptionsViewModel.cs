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
		Config config = new Config();
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
			fd.Filter = "Settings Files | *.xml";
			if (fd.ShowDialog() == DialogResult.OK)
			{
				SettingsFile = fd.FileName;
				Save();
			}
		}

		public void Load()
		{
			var fd = new OpenFileDialog();
			fd.Filter = "Settings Files | *.xml";
			if (fd.ShowDialog() == DialogResult.OK)
			{
				using var reader = File.OpenText(fd.FileName);
				var xml = new XmlSerializer(typeof(Config));
				Config = (Config)xml.Deserialize(reader);
				SettingsFile = fd.FileName;
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
	}
}
