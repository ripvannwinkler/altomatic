using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EliteMMO.API;
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
		public string Name
		{
			get { return name; }
			set { name = value; OnPropertyChanged(); }
		}

		private bool isEnabled;
		public bool IsEnabled
		{
			get { return isEnabled; }
			set { isEnabled = value; OnPropertyChanged(); }
		}

		private double distanceFromHealer;
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

		private uint currentHp;
		public uint CurrentHp
		{
			get { return currentHp; }
			set { currentHp = value; OnPropertyChanged(); }
		}

		private double currentHpp;
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

		public XiEntity Entity
		{
			get => AppData.Healer.Entity.GetEntity((int)member.TargetIndex);
		}

		public EntityStatus EntityStatus
		{
			get => (EntityStatus)Entity.Status;
		}

		public XiEntity TargetEntity
		{
			get { return AppData.Healer.Entity.GetEntity((int)Entity.TargetingIndex); }
		}

		public bool IsInHealerParty
		{
			get => AppData.Healer.Party
				.GetPartyMembers().Where(m => m.MemberNumber < 6)
				.Select(m => m.Name).Contains(Name);
		}

		private AppViewModel appData;
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

		private AutoBuffsViewModel autoBuffs;
		public AutoBuffsViewModel AutoBuffs
		{
			get { return autoBuffs; }
			set { autoBuffs = value; OnPropertyChanged(); }
		}

		private PartyMember member;
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

		private bool isGeoTarget;
		public bool IsGeoTarget
		{
			get => isGeoTarget;
			set
			{
				if (value)
				{
					foreach (var player in AppData.Players)
					{
						player.IsGeoTarget = false;
					}
				}

				isGeoTarget = value;
				OnPropertyChanged();
			}
		}

		private bool isEntrustTarget;
		public bool IsEntrustTarget
		{
			get => isEntrustTarget;
			set
			{
				if (value)
				{
					foreach (var player in AppData.Players)
					{
						player.IsEntrustTarget = false;
					}
				}

				isEntrustTarget = value;
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

		public bool HasAnyBuff(params short[] buffs)
    {
			return AppData.Buffs.HasAny(Name, buffs);
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
