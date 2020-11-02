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
	public class DoomedStrategy : IGameStrategy
	{
		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (!app.Spells.CanCast("Cursna")) return false;
			var healerEntity = app.Healer.Entity.GetLocalPlayer();
			var members = app.Monitored.Party.GetPartyMembers();
			var candidates = new List<PartyMember>();

			// use item if healer is doomed
			if (app.Buffs.HasAny(app.Healer.Player.Name, Buffs.Doom, Buffs.Curse))
			{
				if (await app.Actions.UseItem("Hallowed Water")) return true;
				if (await app.Actions.UseItem("Holy Water")) return true;
			}

			// cursna doomed members
			for (var i = 0; i < 18; i++)
			{
				var member = members[i];
				if (member.Active < 1) continue;

				var memberIndex = (int)member.TargetIndex;
				var memberEntity = app.Healer.Entity.GetEntity(memberIndex);
				var distance = PlayerUtilities.GetDistance(healerEntity, memberEntity);

				if (distance < 21 && app.Buffs.HasAny(member.Name, Buffs.Doom, Buffs.Curse))
				{
					var player = app.Players.SingleOrDefault(x => x.Name == member.Name);
					if (player?.IsEnabled ?? false) candidates.Add(member);
				}
			}

			candidates.SortByJob(app);
			foreach (var target in candidates)
			{
				if (await app.Actions.CastSpell("Cursna", target.Name))
				{
					return true;
				}
			}

			return false;
		}
	}
}
