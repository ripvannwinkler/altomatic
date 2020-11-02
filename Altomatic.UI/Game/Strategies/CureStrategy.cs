using System.Collections.Generic;
using System.Linq;
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
			if (!app.Spells.CanCast("Cure")) return false;
			var healerEntity = app.Healer.Entity.GetLocalPlayer();
			var members = app.Monitored.Party.GetPartyMembers();
			var candidates = new List<PartyMember>();

			var threshold = app.Options.Config.CureThreshold;
			var potencies = new CurePotency(app);

			for (var i = 0; i < 18; i++)
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

			// sort HPP (low to high)
			candidates.Sort((a, b) => a.CurrentHPP.CompareTo(b.CurrentHPP));

			if (candidates.Any())
			{
				var target = candidates.First();
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
