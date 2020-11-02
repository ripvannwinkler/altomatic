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
	public class AutoPhalanxStrategy : IGameStrategy
	{
		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			var healerEntity = app.Healer.Entity.GetLocalPlayer();
			var members = app.Monitored.Party.GetPartyMembers();
			var candidates = new List<PartyMember>();

			// Skip if party members have low HP.
			if (members.Min(m => m.CurrentHPP) < Constants.LowHpThreshold)
			{
				return false;
			}

			// Try casting on self if needed.
			if (app.Options.Config.SelfPhalanx)
			{
				if (!app.Healer.Player.Buffs.Contains(Buffs.Phalanx))
				{
					if (await app.Actions.CastSpell("Phalanx"))
					{
						return true;
					}
				}
			}

			// Skip if no Phalanx II.
			if (app.Spells.CanCast("Phalanx II"))
			{
				for (var i = 0; i < 6; i++)
				{
					var member = members[i];
					if (member.Active < 1) continue;
					if (member.Name == app.Healer.Player.Name) continue;

					var player = app.Players.SingleOrDefault(x => x.Name == member.Name);
					if (player == null) continue;

					if (player.DistanceFromHealer < Constants.DefaultCastRange)
					{
						var enabled = player.IsEnabled;
						var autoHaste = player.AutoBuffs.Haste;
						var ageSeconds = app.Buffs.GetBuffAgeInSeconds(player.Name, Buffs.Phalanx);
						var needsRecast = ageSeconds > app.Options.Config.AutoHasteSeconds;
						if (enabled && autoHaste && needsRecast) candidates.Add(member);
					}
				}

				// Try cast on each target.
				// Return true on first success.
				foreach (var target in candidates)
				{
					if (await app.Actions.CastSpell("Phalanx II", target.Name))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
