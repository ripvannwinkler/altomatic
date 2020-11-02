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

		private string name;
		private bool isEnabled;
		private uint currentHp;
		private double currentHpp;
		private double distanceFromHealer;
		private AppViewModel appData;
		private AutoBuffsViewModel autoBuffs;

		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				OnPropertyChanged();
			}
		}

		public bool IsEnabled
		{
			get { return isEnabled; }
			set
			{
				isEnabled = value;
				OnPropertyChanged();
			}
		}

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

		public uint CurrentHp
		{
			get { return currentHp; }
			set
			{
				currentHp = value;
				OnPropertyChanged();
			}
		}

		public double CurrentHpp
		{
			get { return currentHpp; }
			set
			{
				currentHpp = value;
				OnPropertyChanged();
			}
		}


		public bool IsActive
		{
			get { return distanceFromHealer < 21 && appData.IsGameReady; }
			set { /* ignore */}
		}

		public AppViewModel AppData
		{
			get { return appData; }
			set
			{
				appData = value;
				OnPropertyChanged();
			}
		}

		public AutoBuffsViewModel AutoBuffs
    {
      get { return autoBuffs; }
      set
      {
				autoBuffs = value;
				OnPropertyChanged();
      }
    }

		public PlayerViewModel(AppViewModel appData)
		{
			AppData = appData;
			AutoBuffs = new AutoBuffsViewModel();
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
