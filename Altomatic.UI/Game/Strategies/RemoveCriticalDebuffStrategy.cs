using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altomatic.UI.Game.Data;
using Altomatic.UI.Utilities;
using Altomatic.UI.ViewModels;

namespace Altomatic.UI.Game.Strategies
{
	public class RemoveCriticalDebuffStrategy : IGameStrategy
	{
		private const int CRITICAL_REMOVAL_HPP_THRESHOLD = 60;
		private static readonly string[] silenaJobs = new[] { "WHM", "RDM", "SCH", "PLD", "NIN", "RUN" };

		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (app.Options.Config.EnableDivineCaress &&
					app.Healer.HasAnyBuff(Buffs.DivineCaress, Buffs.DivineCaress2) == false &&
					await app.Actions.UseAbility("Divine Caress"))
			{
				return true;
			}

			// always remove silence and doom regardless of player HPP
			if (await RemoveSilenceFromHealer(app) ||
					await RemoveDoomFromPlayers(app))
			{
				return true;
			}

			// remove other critical debuffs as long as all players above N HPP
			var minHpp = app.ActivePlayers.Select(p => p.CurrentHpp).Min();
			if (minHpp >= CRITICAL_REMOVAL_HPP_THRESHOLD)
			{
				if (await RemoveDoomFromPlayers(app, true) ||
						await RemoveParalyzeFromHealer(app) ||
						await RemoveSleepgaFromPlayers(app) ||
						await RemovePlagueFromPlayers(app) ||
						await RemoveSilenceFromPlayers(app) ||
						await RemovePetrifyFromPlayers(app))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemoveSilenceFromHealer(AppViewModel app)
		{
			if (app.Healer.HasAnyBuff(Buffs.Silence))
			{
				if (await app.Actions.UseItem("Echo Drops") ||
						await app.Actions.UseItem("Remedy"))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemoveDoomFromPlayers(AppViewModel app, bool curseToo = false)
		{
			var buffsToCheck = curseToo
				? new[] { Buffs.Doom, Buffs.Curse }
				: new[] { Buffs.Doom };

			if (app.Healer.HasAnyBuff(buffsToCheck))
			{
				if (app.Options.Config.PreferItemOverCursna)
				{
					if (await app.Actions.UseItem("Hallowed Water") ||
							await app.Actions.UseItem("Holy Water"))
					{
						return true;
					}
				}

				if (await app.Actions.CastSpell("Cursna"))
				{
					return true;
				}
			}

			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (player.HasAnyBuff(buffsToCheck))
				{
					if (await app.Actions.CastSpell("Cursna", player.Name))
					{
						return true;
					}
				}
			}

			return false;
		}

		private async Task<bool> RemoveParalyzeFromHealer(AppViewModel app)
		{
			if (app.Healer.HasAnyBuff(Buffs.Paralysis))
			{
				if (app.Options.Config.PreferItemOverParalyna &&
						await app.Actions.UseItem("Remedy"))
				{
					return true;
				}

				if (await app.Actions.CastSpell("Paralyna"))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemoveSleepgaFromPlayers(AppViewModel app)
		{
			var party = new List<PlayerViewModel>();
			var other = new List<PlayerViewModel>();
			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (app.Buffs.HasAny(player.Name, Buffs.Sleep, Buffs.Sleep2))
				{
					if (player.IsInHealerParty)
					{
						party.Add(player);
					}
					else
					{
						other.Add(player);
					}
				}
			}

			if (party.Any())
			{
				var target = party.First();
				if (await app.Actions.CastSpell("Curaga", target.Name))
				{
					return true;
				}
			}

			if (other.Any())
			{
				var target = other.First();
				if (await app.Actions.CastSpell("Cure", target.Name))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemovePlagueFromPlayers(AppViewModel app)
		{
			var buffsToCheck = new[] { Buffs.Disease, Buffs.Plague };

			foreach (var player in app.ActivePlayers.SortByJob(JobSort.MeleeFirst))
			{
				if (player.HasAnyBuff(buffsToCheck))
				{
					if (await app.Actions.CastSpell("Viruna", player.Name))
					{
						return true;
					}
				}
			}

			return false;
		}

		private async Task<bool> RemoveSilenceFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob(JobSort.HealersFirst))
			{
				if (app.Buffs.HasAny(player.Name, Buffs.Silence) &&
						silenaJobs.Contains(app.Jobs.GetMainJob(player.Member)) &&
						await app.Actions.CastSpell("Silena", player.Name))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemovePetrifyFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob(JobSort.HealersFirst))
			{
				if (app.Buffs.HasAny(player.Name, Buffs.Petrification) &&
						await app.Actions.CastSpell("Stona", player.Name))
				{
					return true;
				}
			}

			return false;
		}
	}
}
