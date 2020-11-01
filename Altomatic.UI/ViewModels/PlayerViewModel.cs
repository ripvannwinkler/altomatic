using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Altomatic.UI.ViewModels
{
	public class PlayerViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		string name;
		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				OnPropertyChanged();
			}
		}

		bool enabled;
		public bool IsEnabled
		{
			get { return enabled; }
			set
			{
				enabled = value;
				OnPropertyChanged();
			}
		}

		double distanceFromHealer;
		public double DistanceFromHealer
		{
			get { return distanceFromHealer; }
			set
			{
				distanceFromHealer = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsActive));
			}
		}

		public bool IsActive
		{
			get { return distanceFromHealer < 21 && appData.GameIsReady; }
			set { /* ignore */}
		}

		uint currentHp;
		public uint CurrentHp
		{
			get { return currentHp; }
			set
			{
				currentHp = value;
				OnPropertyChanged();
			}
		}

		double currentHpp;
		public double CurrentHpp
		{
			get { return currentHpp; }
			set
			{
				currentHpp = value;
				OnPropertyChanged();
			}
		}

		private AppViewModel appData;
		public AppViewModel AppData
		{
			get { return appData; }
			set
			{
				appData = value;
				OnPropertyChanged();
			}
		}

		public PlayerViewModel(AppViewModel appData)
		{
			AppData = appData;
			DistanceFromHealer = uint.MaxValue;
			IsEnabled = true;
		}

		public void ResetVitals()
		{
			Name = "";
			CurrentHp = 0;
			CurrentHpp = 0;
			DistanceFromHealer = uint.MaxValue;
		}
	}
}
