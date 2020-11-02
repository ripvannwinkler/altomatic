﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altomatic.UI.Game.Data;
using Altomatic.UI.Utilities;
using Altomatic.UI.ViewModels;
using static EliteMMO.API.EliteAPI;

namespace Altomatic.UI.Game.Strategies
{
	public class AutoRefreshStrategy : IGameStrategy
	{
		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (!app.Spells.CanCast("Refresh")) return false;
			var healerEntity = app.Healer.Entity.GetLocalPlayer();
			var members = app.Monitored.Party.GetPartyMembers();
			var candidates = new List<PartyMember>();

			for (var i = 0; i < 6; i++)
			{
				var member = members[i];
				if (member.Active < 1) continue;

				var memberIndex = (int)member.TargetIndex;
				var memberEntity = app.Healer.Entity.GetEntity(memberIndex);
				var distance = PlayerUtilities.GetDistance(healerEntity, memberEntity);

				if (distance < 21)
				{
					var player = app.Players.SingleOrDefault(x => x.Name == member.Name);
					if (player == null) continue;

					var enabled = player.IsEnabled;
					var autoRefresh = player.AutoBuffs.Refresh;
					var ageSeconds = app.Buffs.GetBuffAgeInSeconds(player.Name, Buffs.Refresh);
					var needsRecast = ageSeconds > app.Options.Config.AutoHasteSeconds;

					if (member.Name == app.Healer.Player.Name)
					{
						if (app.Options.Config.SelfRefresh)
						{
							enabled = true;
							autoRefresh = true;
						}
					}

					if (enabled && autoRefresh && needsRecast)
					{
						candidates.Add(member);
					}
				}
			}

			if (candidates.Any() && candidates.Min(c => c.CurrentHPP) > 75)
			{
				var spell = new[] { "Refresh III", "Refresh II", "Refresh" }
					.Where(s => app.Spells.HasAccessTo(s))
					.FirstOrDefault();

				if (!string.IsNullOrWhiteSpace(spell))
				{
					foreach (var target in candidates)
					{
						if (await app.Actions.CastSpell(spell, target.Name))
						{
							return true;
						}
					}
				}
			}

			return false;
		}
	}
}