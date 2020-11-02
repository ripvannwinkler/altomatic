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
	public class SilencedStrategy : IGameStrategy
	{
		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (!app.Spells.CanCast("Silena")) return false;
			var healerEntity = app.Healer.Entity.GetLocalPlayer();
			var members = app.Monitored.Party.GetPartyMembers();
			var candidates = new List<PartyMember>();

			// use item if healer is silenced
			if (app.Buffs.HasAny(app.Healer.Player.Name, Buffs.Silence))
			{
				if (await app.Actions.UseItem("Echo Drops")) return true;
				if (await app.Actions.UseItem("Remedy")) return true;
			}

			// silena party members
			for (var i = 0; i < 18; i++)
			{
				var member = members[i];
				if (member.Active < 1) continue;

				var memberIndex = (int)member.TargetIndex;
				var memberEntity = app.Healer.Entity.GetEntity(memberIndex);
				var distance = PlayerUtilities.GetDistance(healerEntity, memberEntity);

				if (distance < 21 && app.Buffs.HasAny(member.Name, Buffs.Silence))
				{
					var player = app.Players.SingleOrDefault(x => x.Name == member.Name);
					if (player?.IsEnabled ?? false) candidates.Add(member);
				}
			}

			candidates.SortByJob(app, JobSort.HealersFirst);
			if (candidates.Any() && candidates.Min(c => c.CurrentHPP) > 75)
			{
				foreach (var target in candidates)
				{
					if (await app.Actions.CastSpell("Silena", target.Name))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
