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
	public class RemoveDebuffStrategy : IGameStrategy
	{
		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (app.ActivePlayers.AreHealthy())
			{
				return await RemoveDebuffs(app);
			}

			return false;
		}

		private async Task<bool> RemoveDebuffs(AppViewModel app)
		{
			if (await RemoveParalysisFromPlayers(app) ||
					await RemoveMaxHpMpDownFromPlayers(app) ||
					await RemoveDefenseDownFromPlayers(app) ||
					await RemoveMagicDefenseDownFromPlayers(app) ||
					await RemoveSlowFromPlayers(app) ||
					await RemovePlagueFromPlayers(app) ||
					await RemoveParalysisFromPlayers(app) ||
					await RemoveGravityBindFromPlayers(app) ||
					await RemoveBlindPoisonFromPlayers(app) ||
					await RemoveHelixBioDiaFromPlayers(app) || 
					await RemoveAmnesiaFromPlayers(app))
			{
				return true;
			}

			return false;
		}

    private async Task<bool> RemoveMaxHpMpDownFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (player.Member.MemberNumber < 6 &&
						app.Buffs.HasAny(player.Name, Buffs.MaxHPDown) &&
						await app.Actions.CastSpell("Erase", player.Name))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemoveDefenseDownFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (player.Member.MemberNumber < 6 &&
						app.Buffs.HasAny(player.Name, Buffs.DefenseDown) &&
						await app.Actions.CastSpell("Erase", player.Name))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemoveMagicDefenseDownFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (player.Member.MemberNumber < 6 &&
						app.Buffs.HasAny(player.Name, Buffs.MagicDefDown) &&
						await app.Actions.CastSpell("Erase", player.Name))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemoveSlowFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (player.Member.MemberNumber < 6 &&
						app.Buffs.HasAny(player.Name, Buffs.Slow, Buffs.Elegy) &&
						await app.Actions.CastSpell("Erase", player.Name))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemovePlagueFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (app.Buffs.HasAny(player.Name, Buffs.Plague, Buffs.Disease) &&
						await app.Actions.CastSpell("Viruna", player.Name))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemoveParalysisFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (app.Buffs.HasAny(player.Name, Buffs.Paralysis) &&
						await app.Actions.CastSpell("Paralyna", player.Name))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemoveGravityBindFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (player.Member.MemberNumber < 6 &&
						app.Buffs.HasAny(player.Name, Buffs.Weight, Buffs.Bind) &&
						await app.Actions.CastSpell("Erase", player.Name))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemoveBlindPoisonFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (app.Buffs.HasAny(player.Name, Buffs.Blindness) &&
						await app.Actions.CastSpell("Blindna", player.Name))
				{
					return true;
				}

				if (app.Buffs.HasAny(player.Name, Buffs.Poison) &&
						await app.Actions.CastSpell("Poisona", player.Name))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemoveHelixBioDiaFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (player.Member.MemberNumber < 6 &&
						app.Buffs.HasAny(player.Name, Buffs.Helix, Buffs.Bio, Buffs.Dia) &&
						await app.Actions.CastSpell("Erase", player.Name))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemoveAmnesiaFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (player.Member.MemberNumber < 6 &&
						app.Healer.HasAnyBuff(Buffs.AfflatusMisery) &&
						app.Buffs.HasAny(player.Name, Buffs.Amnesia) &&
						await app.Actions.CastSpell("Esuna", player.Name))
				{
					return true;
				}
			}

			return false;
		}
	}
}
