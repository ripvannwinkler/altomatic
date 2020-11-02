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

			if (app.Buffs.HasAny(app.Healer.Player.Name, Buffs.Silence))
			{
				if (await app.Actions.UseItem("Echo Drops")) return true;
				if (await app.Actions.UseItem("Remedy")) return true;
			}

			if (members.Min(m => m.CurrentHPP) < Constants.LowHpThreshold)
			{
				return false;
			}

			for (var i = 0; i < 18; i++)
			{
				var member = members[i];
				if (member.Active < 1) continue;
				if (app.Buffs.HasAny(member.Name, Buffs.Silence))
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

			candidates.SortByJob(app, JobSort.HealersFirst);
			foreach (var target in candidates)
			{
				if (await app.Actions.CastSpell("Silena", target.Name))
				{
					return true;
				}
			}

			return false;
		}
	}
}
