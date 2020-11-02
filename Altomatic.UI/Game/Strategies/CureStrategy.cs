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
	/// <summary>
	/// Casts cure on the member of the alliance with the lowest HP that meets the threshold.
	/// </summary>
	public class CureStrategy : IGameStrategy
	{
		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			var healerEntity = app.Healer.Entity.GetLocalPlayer();
			var threshold = app.Options.Config.CureThreshold;
			var potencies = new CurePotency(app);
			var members = app.Monitored.Party.GetPartyMembers();
			var candidates = new List<PartyMember>();

			// sort lowest HPP first
			members.Sort((a, b) => a.CurrentHPP.CompareTo(b.CurrentHPP));

			// find candidates
			for (var i = 0; i < 6; i++)
			{
				var member = members[i];
				var memberIndex = (int)member.TargetIndex;
				var memberEntity = app.Healer.Entity.GetEntity(memberIndex);
				var distance = PlayerUtilities.GetDistance(healerEntity, memberEntity);

				if (distance < 21 &&
						member.Active > 0 &&
						member.CurrentHP > 0 &&
						member.CurrentHPP < threshold)
				{
					var player = app.Players.SingleOrDefault(x => x.Name == member.Name);
					if (player?.IsEnabled ?? false) candidates.Add(member);
				}
			}

			if (candidates.Any())
			{
				var target = candidates.FirstOrDefault();
				var loss = target.CurrentHP * 100 / target.CurrentHPP - target.CurrentHP;

				if (loss >= potencies.Cure5)
				{
					if (await app.Actions.CastSpell("Cure VI", target.Name) ||
							await app.Actions.CastSpell("Cure V", target.Name) ||
							await app.Actions.CastSpell("Cure IV", target.Name) ||
							await app.Actions.CastSpell("Cure III", target.Name))
					{
						return true;
					}
				}
				else if (loss >= potencies.Cure4)
				{
					if (await app.Actions.CastSpell("Cure V", target.Name) ||
							await app.Actions.CastSpell("Cure IV", target.Name) ||
							await app.Actions.CastSpell("Cure III", target.Name))
					{
						return true;
					}
				}
				else if (loss >= potencies.Cure3)
				{
					if (await app.Actions.CastSpell("Cure IV", target.Name) ||
							await app.Actions.CastSpell("Cure III", target.Name) ||
							await app.Actions.CastSpell("Cure II", target.Name))
					{
						return true;
					}
				}
				else if (loss >= potencies.Cure2)
				{
					if (await app.Actions.CastSpell("Cure III", target.Name) ||
							await app.Actions.CastSpell("Cure II", target.Name) ||
							await app.Actions.CastSpell("Cure", target.Name))
					{
						return true;
					}
				}
				else
				{
					if (await app.Actions.CastSpell("Cure II", target.Name) ||
							await app.Actions.CastSpell("Cure", target.Name))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
