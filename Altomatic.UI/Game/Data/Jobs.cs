﻿using System;
using System.Collections.Generic;
using System.Linq;
using Altomatic.UI.Utilities;
using Altomatic.UI.ViewModels;
using static EliteMMO.API.EliteAPI;

namespace Altomatic.UI.Game.Data
{
	public class Jobs : Dictionary<ushort, string>
	{
		public AppViewModel App { get; }

		public Jobs(AppViewModel app)
		{
			Add(1, "WAR");
			Add(2, "MNK");
			Add(3, "WHM");
			Add(4, "BLM");
			Add(5, "RDM");
			Add(6, "THF");
			Add(7, "PLD");
			Add(8, "DRK");
			Add(9, "BST");
			Add(10, "BRD");
			Add(11, "RNG");
			Add(12, "SAM");
			Add(13, "NIN");
			Add(14, "DRG");
			Add(15, "SMN");
			Add(16, "BLU");
			Add(17, "COR");
			Add(18, "PUP");
			Add(19, "DNC");
			Add(20, "SCH");
			Add(21, "GEO");
			Add(22, "RUN");

			App = app ?? throw new ArgumentNullException(nameof(app));
		}

		public bool CanUseAbility(string abilityName)
		{
			if (App.Healer.HasAnyBuff(
				Buffs.Amnesia, Buffs.Impairment, Buffs.Terror,
				Buffs.Stun, Buffs.Sleep, Buffs.Petrification))
			{
				return false;
			}

			var ability = App.Healer.Resources.GetAbility(abilityName, 0);
			var hasAbility = App.Healer.Player.HasAbility(ability.ID);

			var timers = App.Healer.Recast.GetAbilityIds();
			var timerIndex = timers.IndexOf(ability.TimerID);
			var recast = App.Healer.Recast.GetAbilityRecast(timerIndex);
			return hasAbility && recast == 0;
		}

		public int MapPriority(PartyMember member, JobSort sortStrategy = JobSort.TanksFirst)
		{
			var entity = App.Monitored.Entity.GetEntity((int)member.TargetIndex);
			var main = App.Jobs.GetMainJob(entity);
			var sub = App.Jobs.GetMainJob(entity);

			if (sortStrategy == JobSort.TanksFirst)
			{
				return
					new[] { "PLD", "RUN", "WHM" }.Contains(main) ? 1 :
					new[] { "SCH", "RDM", "BRD", "BLM" }.Contains(main) ? 2 :
					new[] { "NIN" }.Contains(sub) ? 3 : 4;
			}
			else if (sortStrategy == JobSort.HealersFirst)
			{
				return
					new[] { "WHM", "SCH", "RDM" }.Contains(main) ? 1 :
					new[] { "PLD", "RUN" }.Contains(main) ? 2 :
					new[] { "BRD", "BLM" }.Contains(main) ? 3 :
					new[] { "NIN" }.Contains(sub) ? 4 : 5;
			}
			else
			{
				return 1;
			}
		}

		public bool IsMainJob(PlayerTools player, string jobName)
		{
			if (TryGetValue(player.MainJob, out var name))
			{
				return name.ToUpper() == jobName.ToUpper();
			}

			return false;
		}

		public bool IsSubJob(PlayerTools player, string jobName)
		{
			if (TryGetValue(player.SubJob, out var name))
			{
				return name.ToUpper() == jobName.ToUpper();
			}

			return false;
		}

		public string GetMainJob(XiEntity entity)
		{
			if (TryGetValue(entity.Main, out var name))
			{
				return name;
			}

			return null;
		}

		public string GetSubJob(XiEntity entity)
		{
			if (TryGetValue(entity.Sub, out var name))
			{
				return name;
			}

			return null;
		}
	}
}
