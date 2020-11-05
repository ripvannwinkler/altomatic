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
		private static readonly string[] silenaJobs = new[] { "WHM", "RDM", "SCH", "PLD", "NIN", "RUN" };

		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (await RemoveSilenceFromHealer(app) ||
					await RemoveDoomFromPlayers(app) ||
					await RemoveSleepgaFromPlayers(app) ||
					await RemoveParalyzeFromHealer(app) ||
					await RemoveSilenceFromPlayers(app) ||
					await RemovePetrifyFromPlayers(app))
			{
				return true;
			}

			return false;
		}

		private async Task<bool> RemoveDoomFromPlayers(AppViewModel app)
		{
			if (app.Healer.HasAnyBuff(Buffs.Doom, Buffs.Curse))
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
				if (player.HasAnyBuff(Buffs.Doom, Buffs.Curse))
				{
					if (await app.Actions.CastSpell("Cursna"))
					{
						return true;
					}
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

		private async Task<bool> RemoveSilenceFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob(JobSort.HealersFirst))
			{
				if (app.Buffs.HasAny(player.Name, Buffs.Silence) &&
						silenaJobs.Contains(app.Jobs.GetMainJob(player.Entity)) &&
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
