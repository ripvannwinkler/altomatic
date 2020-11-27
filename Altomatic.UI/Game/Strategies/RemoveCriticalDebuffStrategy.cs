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
		private DateTimeOffset lastWarned = DateTime.MinValue;

		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			var refreshSpell = app.Spells.FirstAvailable("Refresh III", "Refresh II", "Refresh");

			if (await RemoveSilenceFromHealer(app))
			{
				return true;
			}

			if (app.Options.Config.EnableComposure &&
					app.Healer.HasAnyBuff(Buffs.Composure) == false &&
					await app.Actions.UseAbility("Composure"))
			{
				return true;
			}

			if (app.Options.Config.SelfRefresh &&
					!app.Healer.HasAnyBuff(Buffs.Refresh, Buffs.Refresh2))
			{
				if (await app.Actions.CastSpell(refreshSpell)) return true;
			}

			if (await RemoveDoomFromPlayers(app) ||
					await RemoveSleepgaFromPlayers(app))
			{
				return true;
			}

			if (app.ActivePlayers.AreHealthy())
			{
				if (app.Options.Config.EnableDivineCaress &&
						app.Healer.HasAnyBuff(Buffs.DivineCaress, Buffs.DivineCaress2) == false)
				{
					if (await app.Actions.UseAbility("Divine Caress")) return true;
				}

				if (await RemoveDoomFromPlayers(app, true) ||
						await RemovePlagueFromPlayers(app) ||
						await RemoveSilenceFromPlayers(app) ||
						await RemovePetrifyFromPlayers(app) ||
						await RemoveParalyzeFromHealer(app) ||
						await RemoveSlowFromPlayers(app))
				{
					return true;
				}
			}

			return false;
		}

		private static async Task PutUpDivineSeal(AppViewModel app)
		{
			if (app.Options.Config.EnableDivineSeal &&
					!app.Healer.HasAnyBuff(Buffs.DivineSeal))
			{
				await app.Actions.UseAbility("Divine Seal");
			}
		}

		private async Task<bool> RemoveSilenceFromHealer(AppViewModel app)
		{
			if (app.Healer.HasAnyBuff(Buffs.Silence))
			{
				if (await app.Actions.UseItem("Echo Drops")) return true;
				if (await app.Actions.UseItem("Remedy")) return true;

				if (DateTimeOffset.UtcNow.Subtract(lastWarned).TotalSeconds > 15)
				{
					await app.Monitored.SendCommand($"/echo \x1e\x5{app.Healer.Player.Name} is silenced and has no removal tools.\x1f\x5", 100);
					lastWarned = DateTimeOffset.UtcNow;
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
					await PutUpDivineSeal(app);
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

				await PutUpDivineSeal(app);
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
					await PutUpDivineSeal(app);
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
						silenaJobs.Contains(app.Jobs.GetMainJob(player.Member)))
				{
					await PutUpDivineSeal(app);
					if (await app.Actions.CastSpell("Silena", player.Name))
					{
						return true;
					}
				}
			}

			return false;
		}

		private async Task<bool> RemovePetrifyFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob(JobSort.HealersFirst))
			{
				if (app.Buffs.HasAny(player.Name, Buffs.Petrification))
				{
					await PutUpDivineSeal(app);
					if (await app.Actions.CastSpell("Stona", player.Name))
					{
						return true;
					}
				}
			}

			return false;
		}

		private async Task<bool> RemoveSlowFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (player.HasAnyBuff(Buffs.Slow))
				{
					await PutUpDivineSeal(app);
					if (await app.Actions.CastSpell("Erase", player.Name))
					{
						return true;
					}
				}
			}

			return false;
		}
	}
}
