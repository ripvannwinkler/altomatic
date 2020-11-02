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
  /// Casts curaga on members of the primary party if the number of required targets meet the threshold.
  /// </summary>
  public class CuragaStrategy : IGameStrategy
	{
		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			var healerEntity = app.Healer.Entity.GetLocalPlayer();
			var threshold = app.Options.Config.CuragaThreshold;
			var required = app.Options.Config.CuragaRequiredTargets;
			var potencies = new CurePotency(app);
			var members = app.Monitored.Party.GetPartyMembers();
			var candidates = new List<PartyMember>();

			// sort by HPP (low to high)
			members.Sort((a, b) => a.CurrentHPP.CompareTo(b.CurrentHPP));

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
					candidates.Add(member);
				}
			}

			if (candidates.Count() >= required)
			{
				var target = candidates.FirstOrDefault();
				var loss = target.CurrentHP * 100 / target.CurrentHPP - target.CurrentHP;

				if (loss >= potencies.Curaga4)
				{
					if (await app.Actions.CastSpell("Curaga V", target.Name) ||
							await app.Actions.CastSpell("Curaga IV", target.Name))
					{
						return true;
					}
				}
				else if (loss >= potencies.Curaga3)
				{
					if (await app.Actions.CastSpell("Curaga IV", target.Name) ||
							await app.Actions.CastSpell("Curaga V", target.Name) ||
							await app.Actions.CastSpell("Curaga III", target.Name))
					{
						return true;
					}
				}
				else if (loss >= potencies.Curaga2)
				{
					if (await app.Actions.CastSpell("Curaga III", target.Name) ||
							await app.Actions.CastSpell("Curaga IV", target.Name) ||
							await app.Actions.CastSpell("Curaga II", target.Name))
					{
						return true;
					}
				}
				else if (loss >= potencies.Curaga)
				{
					if (await app.Actions.CastSpell("Curaga II", target.Name) ||
							await app.Actions.CastSpell("Curaga III", target.Name) ||
							await app.Actions.CastSpell("Curaga", target.Name))
					{
						return true;
					}
				}
				else
				{
					if (await app.Actions.CastSpell("Curaga", target.Name) ||
							await app.Actions.CastSpell("Curaga II", target.Name))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
