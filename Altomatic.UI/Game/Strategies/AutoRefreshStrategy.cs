using System;
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
			var spells = new[] { "Refresh III", "Refresh II", "Refresh" };
			var spell = spells.FirstOrDefault(s => app.Spells.HasAccessTo(s));
			if (string.IsNullOrWhiteSpace(spell)) return false;
			if (!app.Spells.CanCast(spell)) return false;

			var healerEntity = app.Healer.Entity.GetLocalPlayer();
			var members = app.Monitored.Party.GetPartyMembers();
			var candidates = new List<PartyMember>();

			// Skip if party members have low HP.
			if (members.Min(m => m.CurrentHPP) < Constants.LowHpThreshold)
			{
				return false;
			}

			// Try casting on self if needed.
			if (app.Options.Config.SelfRefresh)
			{
				var hasRefresh = app.Buffs.HasAny(
					healerEntity.Name,
					Buffs.Refresh,
					Buffs.Refresh2,
					Buffs.SublimationActivated,
					Buffs.SublimationComplete);

				if (!hasRefresh)
				{
					if (await app.Actions.CastSpell(spell))
					{
						return true;
					}
				}
			}

			// Find other candidates.
			for (var i = 0; i < 6; i++)
			{
				var member = members[i];
				if (member.Active < 1) continue;
				if (member.Name == app.Healer.Player.Name) continue;

				var player = app.Players.SingleOrDefault(x => x.Name == member.Name);
				if (player == null) continue;

				if (player.DistanceFromHealer < Constants.DefaultCastRange)
				{
					if (player == null) continue;

					var enabled = player.IsEnabled;
					var autoRefresh = player.AutoBuffs.Refresh;
					var ageSeconds = app.Buffs.GetBuffAgeInSeconds(player.Name, Buffs.Refresh);
					var needsRecast = ageSeconds > app.Options.Config.AutoHasteSeconds;
					if (enabled && autoRefresh && needsRecast) candidates.Add(member);
				}
			}

			// Try casting on others.
			// Return true on first success.
			foreach (var target in candidates)
			{
				if (await app.Actions.CastSpell(spell, target.Name))
				{
					return true;
				}
			}

			return false;
		}
	}
}
