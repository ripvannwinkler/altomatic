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
			set
			{
				isEnabled = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsActive));
			}
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
					IsEnabled && Member?.Active > 0 &&
					DistanceFromHealer < Constants.DefaultCastRange;
			}
		}

		public bool IsInHealerParty
		{
			get => AppData.Healer.Party
				.GetPartyMembers().Where(m => m.MemberNumber < 6)
				.Select(m => m.Name).Contains(Name);
		}

		public string MainJob
		{
			get => AppData.Jobs.GetMainJob(Member);
		}

		public string SubJob
		{
			get => AppData.Jobs.GetSubJob(Member);
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

		private bool isRequiredForRolls;
		public bool IsRequiredForRolls
		{
			get => isRequiredForRolls;
			set { isRequiredForRolls = value; OnPropertyChanged(); }
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
				OnPropertyChanged(nameof(MainJob));
				OnPropertyChanged(nameof(SubJob));
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
			IsGeoTarget = false;
			IsEntrustTarget = false;
			IsRequiredForRolls = false;

			AutoBuffs.Haste = false;
			AutoBuffs.Flurry = false;
			AutoBuffs.Refresh = false;
			AutoBuffs.Phalanx = false;
			AutoBuffs.Regen = false;
			AutoBuffs.Firestorm = false;
			AutoBuffs.Sandstorm = false;
			AutoBuffs.Rainstorm = false;
			AutoBuffs.Windstorm = false;
			AutoBuffs.Thunderstorm = false;
			AutoBuffs.Hailstorm = false;
			AutoBuffs.Aurorastorm = false;
			AutoBuffs.Voidstorm = false;
		}

		/// <summary>
    /// Gets the lowest age of any of the specified buffs (in seconds)
    /// </summary>
		public int GetBuffAge(params short[] buffs)
		{
			var age = int.MaxValue;
			foreach (var buff in buffs)
			{
				var thisAge = AppData.Buffs.GetBuffAgeSeconds(Name, buff);
				age = Math.Min(age, thisAge);
			}

			return age;
		}

		/// <summary>
    /// Does the player have any of the specified buffs?
    /// </summary>
    /// <param name="buffs"></param>
    /// <returns></returns>
		public bool HasAnyBuff(params short[] buffs)
		{
			return AppData.Buffs.HasAny(Name, buffs);
		}

		/// <inheritdoc/>
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

		/// <inheritdoc/>
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
