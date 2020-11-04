using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static EliteMMO.API.EliteAPI;

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
		private PartyMember member;

		public string Name
		{
			get { return name; }
			set { name = value; OnPropertyChanged(); }
		}

		public bool IsEnabled
		{
			get { return isEnabled; }
			set { isEnabled = value; OnPropertyChanged(); }
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
			set { currentHp = value; OnPropertyChanged(); }
		}

		public double CurrentHpp
		{
			get { return currentHpp; }
			set { currentHpp = value; OnPropertyChanged(); }
		}


		public bool IsActive
		{
			get
			{
				return
					AppData.IsGameReady && Member != null && member.Active > 0 &&
					DistanceFromHealer < Constants.DefaultCastRange && AppData.IsGameReady;
			}
		}

		public AppViewModel AppData
		{
			get { return appData; }
			set
			{
				appData = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsActive));
			}
		}

		public AutoBuffsViewModel AutoBuffs
		{
			get { return autoBuffs; }
			set { autoBuffs = value; OnPropertyChanged(); }
		}

		public PartyMember Member
		{
			get { return member; }
			set
			{
				member = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsActive));
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

		public int GetBuffAgeSeconds(params short[] buffs)
		{
			var age = int.MaxValue;
			foreach (var buff in buffs)
			{
				var thisAge = AppData.Buffs.GetBuffAgeSeconds(Name, buff);
				age = Math.Min(age, thisAge);
			}

			return age;
		}

    public override bool Equals(object obj)
    {
			if (obj is PlayerViewModel other)
      {
				return
					Name == other.Name &&
					CurrentHpp == other.CurrentHpp &&
					Member?.MainJob == other.Member?.MainJob &&
					Member?.SubJob == other.member?.SubJob;
      }

			return false;
    }

    public override int GetHashCode()
    {
			return (
				Name,
				CurrentHpp,
				Member?.MainJob,
				Member?.SubJob
			).GetHashCode();
    }
  }
}
