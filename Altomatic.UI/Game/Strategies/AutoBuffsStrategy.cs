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
      var temperSpell = app.Spells.FirstAvailable("Temper II", "Temper");
			var refreshSpell = app.Spells.FirstAvailable("Refresh III", "Refresh II", "Refresh");
			var reraiseSpell = app.Spells.FirstAvailable("Reraise IV", "Reraise III", "Reraise II", "Reraise");
			var regenSpell = app.Spells.FirstAvailable("Regen V", "Regen IV", "Regen III", "Regen II", "Regen");
			var protectSpell = app.Spells.FirstAvailable("Protectra V", "Protectra IV", "Protectra III", "Protectra II", "Protectra", "Protect V", "Protect IV", "Protect III", "Protect II", "Protect");
			var shellSpell = app.Spells.FirstAvailable("Shellra V", "Shellra IV", "Shellra III", "Shellra II", "Shellra", "Shell V", "Shell IV", "Shell III", "Shell II", "Shell");
			var stormSpell = app.Spells.FirstAvailable(app.Options.Config.SelfStormSpellName + " II", app.Options.Config.SelfStormSpellName);
			var spikesSpell = app.Spells.FirstAvailable(app.Options.Config.SelfSpikesSpellName);
			var enspell = app.Spells.FirstAvailable(app.Options.Config.SelfEnspellName);

			if (app.Options.Config.SelfReraise &&
					!app.Healer.HasAnyBuff(Buffs.Reraise) &&
					await app.Actions.CastSpell(reraiseSpell)) return true;

			if (app.Options.Config.SelfProtect &&
					!app.Healer.HasAnyBuff(Buffs.Protect) &&
					await app.Actions.CastSpell(protectSpell)) return true;

			if (app.Options.Config.SelfShell &&
					!app.Healer.HasAnyBuff(Buffs.Shell) &&
					await app.Actions.CastSpell(shellSpell)) return true;

			if (app.Options.Config.SelfRefresh &&
					!app.Healer.HasAnyBuff(Buffs.Refresh, Buffs.Refresh2) &&
					await app.Actions.CastSpell(refreshSpell)) return true;

			if (app.Options.Config.SelfHaste &&
					!app.Healer.HasAnyBuff(Buffs.Haste, Buffs.Haste2) &&
					await app.Actions.CastSpell(hasteSpell)) return true;

			if (app.Options.Config.SelfRegen &&
					!app.Healer.HasAnyBuff(Buffs.Regen) &&
					await app.Actions.CastSpell(regenSpell)) return true;

			if (app.Options.Config.SelfPhalanx &&
					!app.Healer.HasAnyBuff(Buffs.Phalanx) &&
					await app.Actions.CastSpell("Phalanx")) return true;

			if (app.Options.Config.SelfAquaveil &&
					!app.Healer.HasAnyBuff(Buffs.Aquaveil) &&
					await app.Actions.CastSpell("Aquaveil")) return true;

			if (app.Options.Config.SelfBlink &&
					!app.Healer.HasAnyBuff(Buffs.Blink) &&
					await app.Actions.CastSpell("Blink")) return true;

			if (app.Options.Config.SelfStoneskin &&
					!app.Healer.HasAnyBuff(Buffs.Stoneskin) &&
					await app.Actions.CastSpell("Stoneskin")) return true;

			if (app.Options.Config.SelfKlimaform &&
					!app.Healer.HasAnyBuff(Buffs.Klimaform) &&
					await app.Actions.CastSpell("Klimaform")) return true;

			if (app.Options.Config.SelfTemper &&
					!app.Healer.HasAnyBuff(Buffs.MultiStrikes) &&
					await app.Actions.CastSpell(temperSpell)) return true;

			if (app.Options.Config.SelfUtsusemi &&
					!app.Healer.HasAnyBuff(Buffs.CopyImage, Buffs.CopyImage2, Buffs.CopyImage3, Buffs.CopyImage4) &&
					await app.Actions.CastSpell("Utsusemi: Ni") ||
					await app.Actions.CastSpell("Utsusemi: Ichi")) return true;

			if (!app.Healer.HasAnyBuff(
						Buffs.Firestorm2, Buffs.Firestorm, Buffs.Sandstorm2, Buffs.Sandstorm,
						Buffs.Rainstorm2, Buffs.Rainstorm, Buffs.Windstorm2, Buffs.Windstorm,
						Buffs.Thunderstorm2, Buffs.Thunderstorm, Buffs.Hailstorm2, Buffs.Hailstorm,
						Buffs.Aurorastorm2, Buffs.Aurorastorm, Buffs.Voidstorm2, Buffs.Voidstorm) &&
					!string.IsNullOrWhiteSpace(app.Options.Config.SelfStormSpellName) &&
					await app.Actions.CastSpell(stormSpell)) return true;

			if (!app.Healer.HasAnyBuff(
					Buffs.Enfire, Buffs.Enstone, Buffs.Enwater, Buffs.Enaero, Buffs.Enthunder, Buffs.Enblizzard,
					Buffs.EnfireII, Buffs.EnstoneII, Buffs.EnwaterII, Buffs.EnaeroII, Buffs.EnthunderII, Buffs.EnblizzardII) &&
					!string.IsNullOrWhiteSpace(app.Options.Config.SelfEnspellName) &&
					await app.Actions.CastSpell(enspell)) return true;

			if (!app.Healer.HasAnyBuff(Buffs.BlazeSpikes, Buffs.IceSpikes, Buffs.ShockSpikes) &&
					!string.IsNullOrWhiteSpace(app.Options.Config.SelfSpikesSpellName) &&
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
				if (player.IsEnabled && player.Name != app.Healer.Player.Name)
				{
					if (player.AutoBuffs.Protect &&
							player.GetBuffAgeSeconds(Buffs.Protect) > app.Options.Config.AutoProtectSeconds &&
							await app.Actions.CastSpell(protectSpell, player.Name)) return true;

					if (player.AutoBuffs.Shell &&
							player.GetBuffAgeSeconds(Buffs.Shell) > app.Options.Config.AutoShellSeconds &&
							await app.Actions.CastSpell(shellSpell, player.Name)) return true;

					if (player.AutoBuffs.Refresh &&
							player.Member.MemberNumber < 6 &&
							player.GetBuffAgeSeconds(Buffs.Refresh) > app.Options.Config.AutoRefreshSeconds &&
							await app.Actions.CastSpell(refreshSpell, player.Name)) return true;

					if (player.AutoBuffs.Haste &&
							player.GetBuffAgeSeconds(Buffs.Haste) > app.Options.Config.AutoHasteSeconds &&
							await app.Actions.CastSpell(hasteSpell, player.Name)) return true;

					if (player.AutoBuffs.Regen &&
							player.GetBuffAgeSeconds(Buffs.Regen) > app.Options.Config.AutoRegenSeconds &&
							await app.Actions.CastSpell(regenSpell, player.Name)) return true;

					if (player.AutoBuffs.Phalanx &&
							player.Member.MemberNumber < 6 &&
							player.GetBuffAgeSeconds(Buffs.Phalanx) > app.Options.Config.AutoPhalanxSeconds &&
							await app.Actions.CastSpell("Phalanx II", player.Name)) return true;

					if (player.AutoBuffs.Firestorm &&
							player.Member.MemberNumber < 6 &&
							player.GetBuffAgeSeconds(Buffs.Firestorm, Buffs.Firestorm2) > app.Options.Config.AutoStormSeconds &&
							await app.Actions.CastSpell(app.Spells.FirstAvailable("Firestorm II", "Firestorm"), player.Name)) return true;
				}
			}

			return false;
		}
	}
}
