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
			var threshold = app.Options.Config.CuragaThreshold;
			var required = app.Options.Config.CuragaRequiredTargets;
			var candidates = new List<PartyMember>();
			var potencies = new CurePotency(app);

			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (player.IsInHealerParty && player.DistanceFromHealer < 21)
				{
					if (player.CurrentHp < 1) continue;
					if (player.CurrentHpp > threshold) continue;
					if (player.HasAnyBuff(Buffs.Charm, Buffs.Charm2)) continue;
					candidates.Add(player.Member);
				}
			}

			// sort HPP (low to high)
			candidates.Sort((a, b) => a.CurrentHPP.CompareTo(b.CurrentHPP));

			if (candidates.Count() >= required)
			{
				foreach (var target in candidates)
				{
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
			}

			return false;
		}
	}
}
