using System.Threading.Tasks;
using Altomatic.UI.Game.Data;
using Altomatic.UI.Utilities;
using Altomatic.UI.ViewModels;

namespace Altomatic.UI.Game.Strategies
{
  public class AutoBuffsStrategy : IGameStrategy
	{
		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (app.ActivePlayers.AreHealthy())
			{
				if (await BuffSelf(app)) return true;
				if (await BuffOthers(app)) return true;
			}

			return false;
		}

		private async Task<bool> BuffSelf(AppViewModel app)
		{
			var hasteSpell = app.Spells.FirstAvailable("Haste II", "Haste");
			var refreshSpell = app.Spells.FirstAvailable("Refresh III", "Refresh II", "Refresh");
			var regenSpell = app.Spells.FirstAvailable("Regen V", "Regen IV", "Regen III", "Regen II", "Regen");
			var protectSpell = app.Spells.FirstAvailable("Protectra V", "Protectra IV", "Protectra III", "Protectra II", "Protectra", "Protect V", "Protect IV", "Protect III", "Protect II", "Protect");
			var shellSpell = app.Spells.FirstAvailable("Shellra V", "Shellra IV", "Shellra III", "Shellra II", "Shellra", "Shell V", "Shell IV", "Shell III", "Shell II", "Shell");
			var stormSpell = app.Spells.FirstAvailable(app.Options.Config.SelfStormSpellName + " II", app.Options.Config.SelfStormSpellName);
			var enspell = app.Spells.FirstAvailable(app.Options.Config.SelfEnspellName + " II", app.Options.Config.SelfEnspellName);
			var spikesSpell = app.Spells.FirstAvailable(app.Options.Config.SelfSpikesSpellName);

			if (app.Options.Config.SelfProtect &&
					!app.Healer.HasAnyBuff(Buffs.Protect) &&
					await app.Actions.CastSpell(protectSpell)) return true;

			if (app.Options.Config.SelfShell &&
					!app.Healer.HasAnyBuff(Buffs.Shell) &&
					await app.Actions.CastSpell(shellSpell)) return true;

			if (app.Options.Config.SelfRefresh &&
					!app.Healer.HasAnyBuff(Buffs.Refresh) &&
					await app.Actions.CastSpell(refreshSpell)) return true;

			if (app.Options.Config.SelfHaste &&
					!app.Healer.HasAnyBuff(Buffs.Haste) &&
					await app.Actions.CastSpell(hasteSpell)) return true;

			if (app.Options.Config.SelfRegen &&
					!app.Healer.HasAnyBuff(Buffs.Regen) &&
					await app.Actions.CastSpell(regenSpell)) return true;

			if (app.Options.Config.SelfPhalanx &&
					!app.Healer.HasAnyBuff(Buffs.Phalanx) &&
					await app.Actions.CastSpell("Phalanx")) return true;

			if (app.Options.Config.SelfStormSpell &&
					!app.Healer.HasAnyBuff(
						Buffs.Firestorm2, Buffs.Firestorm, Buffs.Sandstorm2, Buffs.Sandstorm,
						Buffs.Rainstorm2, Buffs.Rainstorm, Buffs.Windstorm2, Buffs.Windstorm,
						Buffs.Thunderstorm2, Buffs.Thunderstorm, Buffs.Hailstorm2, Buffs.Hailstorm,
						Buffs.Aurorastorm2, Buffs.Aurorastorm, Buffs.Voidstorm2, Buffs.Voidstorm) &&
					await app.Actions.CastSpell(stormSpell)) return true;

			if (app.Options.Config.SelfEnspell &&
					!app.Healer.HasAnyBuff(Buffs.Enfire, Buffs.Enstone, Buffs.Enwater, Buffs.Enaero, Buffs.Enthunder, Buffs.Enblizzard) &&
					await app.Actions.CastSpell(enspell)) return true;

			if (app.Options.Config.SelfSpikesSpell &&
					!app.Healer.HasAnyBuff(Buffs.BlazeSpikes, Buffs.IceSpikes, Buffs.ShockSpikes) &&
					await app.Actions.CastSpell(spikesSpell)) return true;

			return false;
		}


		private async Task<bool> BuffOthers(AppViewModel app)
		{
			var hasteSpell = app.Spells.FirstAvailable("Haste II", "Haste");
			var refreshSpell = app.Spells.FirstAvailable("Refresh III", "Refresh II", "Refresh");
			var regenSpell = app.Spells.FirstAvailable("Regen V", "Regen IV", "Regen III", "Regen II", "Regen");
			var protectSpell = app.Spells.FirstAvailable("Protect V", "Protect IV", "Protect III", "Protect II", "Protect");
			var shellSpell = app.Spells.FirstAvailable("Shell V", "Shell IV", "Shell III", "Shell II", "Shell");
			var stormSpell = app.Spells.FirstAvailable(app.Options.Config.SelfStormSpellName + " II", app.Options.Config.SelfStormSpellName);

			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (player.IsEnabled)
				{
					if (player.AutoBuffs.Protect &&
							player.GetBuffAgeSeconds(Buffs.Protect) > app.Options.Config.AutoProtectSeconds &&
							await app.Actions.CastSpell(protectSpell)) return true;

					if (player.AutoBuffs.Shell &&
							player.GetBuffAgeSeconds(Buffs.Shell) > app.Options.Config.AutoShellSeconds &&
							await app.Actions.CastSpell(shellSpell)) return true;

					if (player.AutoBuffs.Refresh &&
							player.Member.MemberNumber < 6 &&
							player.GetBuffAgeSeconds(Buffs.Refresh) > app.Options.Config.AutoRefreshSeconds &&
							await app.Actions.CastSpell(refreshSpell)) return true;

					if (player.AutoBuffs.Haste &&
							player.GetBuffAgeSeconds(Buffs.Haste) > app.Options.Config.AutoHasteSeconds &&
							await app.Actions.CastSpell(hasteSpell)) return true;

					if (player.AutoBuffs.Regen &&
							player.GetBuffAgeSeconds(Buffs.Regen) > app.Options.Config.AutoRegenSeconds &&
							await app.Actions.CastSpell(regenSpell)) return true;

					if (player.AutoBuffs.Phalanx &&
							player.Member.MemberNumber < 6 &&
							player.GetBuffAgeSeconds(Buffs.Phalanx) > app.Options.Config.AutoPhalanxSeconds &&
							await app.Actions.CastSpell("Phalanx II")) return true;

					if (player.AutoBuffs.Firestorm &&
							player.Member.MemberNumber < 6 &&
							player.GetBuffAgeSeconds(Buffs.Firestorm, Buffs.Firestorm2) > app.Options.Config.AutoStormSeconds &&
							await app.Actions.CastSpell(app.Spells.FirstAvailable("Firestorm II", "Firestorm"))) return true;
				}
			}

			return false;
		}
	}
}
