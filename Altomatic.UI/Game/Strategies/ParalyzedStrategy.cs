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
	public class ParalyzedStrategy : IGameStrategy
	{
		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (!app.Spells.CanCast("Paralyna")) return false;
			var healerEntity = app.Healer.Entity.GetLocalPlayer();
			var members = app.Monitored.Party.GetPartyMembers();
			var candidates = new List<PartyMember>();

			for (var i = 0; i < 18; i++)
			{
				var member = members[i];
				if (member.Active < 1) continue;

				var memberIndex = (int)member.TargetIndex;
				var memberEntity = app.Healer.Entity.GetEntity(memberIndex);
				var distance = PlayerUtilities.GetDistance(healerEntity, memberEntity);

				if (distance < 21 && app.Buffs.HasAny(member.Name, Buffs.Paralysis))
				{
					var player = app.Players.SingleOrDefault(x => x.Name == member.Name);
					if (player?.IsEnabled ?? false) candidates.Add(member);
				}
			}

			if (candidates.Any() && candidates.Min(c => c.CurrentHPP) > 75)
			{
				var target = candidates.First();
				if (await app.Actions.CastSpell("Paralyna", target.Name))
				{
					return true;
				}
			}

			return false;
		}
	}
}
