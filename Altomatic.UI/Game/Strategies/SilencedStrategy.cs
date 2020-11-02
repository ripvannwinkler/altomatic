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

			candidates.Sort((a, b) =>
			{
				return MapJob(a, app).CompareTo(MapJob(b, app));
			});

			if (candidates.Any() && candidates.Min(c => c.CurrentHPP) > 75)
			{
				var target = candidates.First();
				if (await app.Actions.CastSpell("Silena", target.Name))
				{
					return true;
				}
			}

			return false;
		}

		private int MapJob(PartyMember member, AppViewModel app)
		{
			var e = app.Monitored.Entity.GetEntity((int)member.TargetIndex);
			var main = app.Jobs.GetMainJob(e);
			var sub = app.Jobs.GetMainJob(e);

			return
				new[] { "PLD", "RUN", "WHM" }.Contains(main) ? 1 :
				new[] { "SCH", "RDM", "BRD" }.Contains(main) ? 2 :
				new[] { "NIN" }.Contains(sub) ? 3 : 4;
		}
	}
}
