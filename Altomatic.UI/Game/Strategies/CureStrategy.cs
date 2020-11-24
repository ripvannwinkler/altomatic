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
			var threshold = app.Options.Config.CureThreshold;
			var candidates = new List<PartyMember>();
			var potencies = new CurePotency(app);

			foreach (var player in app.ActivePlayers)
			{
				if (player.CurrentHp < 1) continue;
				if (player.CurrentHpp > threshold) continue;
				if (player.HasAnyBuff(Buffs.Charm, Buffs.Charm2)) continue;
				candidates.Add(player.Member);
			}

			// sort HPP (low to high)
			candidates.Sort((a, b) =>
			{
				return a.CurrentHPP.CompareTo(b.CurrentHPP);
			});

			if (app.Options.Config.PrioritizeTanks)
			{
				// bump tanks to top of the list
				foreach (var c in candidates.ToArray())
				{
					var job = app.Jobs.GetMainJob(c);
					if (job == "PLD" || job == "RUN")
					{
						candidates.Remove(c);
						candidates.Insert(0, c);
						break;
					}
				}
			}

			foreach (var target in candidates)
			{
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
