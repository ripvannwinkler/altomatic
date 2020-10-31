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
		public bool Enabled
		{
			get { return enabled; }
			set
			{
				enabled = value;
				OnPropertyChanged();
			}
		}

		double distance;
		public double Distance
		{
			get { return distance; }
			set
			{
				distance = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(Active));
			}
		}

		public bool Active
		{
			get { return distance < 21 && appData.IsReadyToRun; }
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
			Distance = uint.MaxValue;
			Enabled = true;
		}

    public void ResetVitals()
    {
			Name = "";
			CurrentHp = 0;
			CurrentHpp = 0;
			Distance = uint.MaxValue;
    }
  }
}
