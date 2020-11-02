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

			if (app.Buffs.HasAny(app.Healer.Player.Name, Buffs.Doom, Buffs.Curse))
			{
				if (await app.Actions.UseItem("Hallowed Water")) return true;
				if (await app.Actions.UseItem("Holy Water")) return true;
			}

			if (members.Min(m => m.CurrentHPP) < Constants.LowHpThreshold)
			{
				return false;
			}

			for (var i = 0; i < 18; i++)
			{
				var member = members[i];
				if (member.Active < 1) continue;
				if (app.Buffs.HasAny(member.Name, Buffs.Doom, Buffs.Curse))
				{
					var player = app.Players.SingleOrDefault(x => x.Name == member.Name);
					if (player != null && player.IsActive && player.IsEnabled)
					{
						if (player.DistanceFromHealer < Constants.DefaultCastRange)
						{
							candidates.Add(member);
						}
					}
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
