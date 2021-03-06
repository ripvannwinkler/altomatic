﻿using System.Linq;
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
			if (await BuffSelf(app)) return true;
			if (await BuffOthers(app)) return true;
			return false;
		}

		private async Task<bool> BuffSelf(AppViewModel app)
		{
			var hasteSpell = app.Spells.FirstAvailable("Haste II", "Haste");
			var temperSpell = app.Spells.FirstAvailable("Temper II", "Temper");
			var refreshSpell = app.Spells.FirstAvailable("Refresh III", "Refresh II", "Refresh");
			var regenSpell = app.Spells.FirstAvailable("Regen V", "Regen IV", "Regen III", "Regen II", "Regen");
			var protectSpell = app.Spells.FirstAvailable("Protectra V", "Protectra IV", "Protectra III", "Protectra II", "Protectra", "Protect V", "Protect IV", "Protect III", "Protect II", "Protect");
			var shellSpell = app.Spells.FirstAvailable("Shellra V", "Shellra IV", "Shellra III", "Shellra II", "Shellra", "Shell V", "Shell IV", "Shell III", "Shell II", "Shell");
			var boostSpell = app.Spells.FirstAvailable($"Boost-{app.Options.Config.BoostSpellName}", $"Gain-{app.Options.Config.BoostSpellName}");
			var stormSpell = app.Spells.FirstAvailable(app.Options.Config.SelfStormSpellName + " II", app.Options.Config.SelfStormSpellName);
			var spikesSpell = app.Spells.FirstAvailable(app.Options.Config.SelfSpikesSpellName);
			var enspell = app.Spells.FirstAvailable(app.Options.Config.SelfEnspellName);

			if (app.Options.Config.SelfReraise &&
					!app.Healer.HasAnyBuff(Buffs.Reraise) &&
					(await app.Actions.CastSpell("Reraise IV") ||
					 await app.Actions.CastSpell("Reraise III") ||
					 await app.Actions.CastSpell("Reraise II") ||
					 await app.Actions.CastSpell("Reraise")))
			{
				return true;
			}

			if (app.Options.Config.SelfHaste &&
					!app.Healer.HasAnyBuff(Buffs.Haste, Buffs.Haste2) &&
					await app.Actions.CastSpell(hasteSpell)) return true;

			if (app.Options.Config.SelfRefresh &&
					!app.Healer.HasAnyBuff(Buffs.Refresh, Buffs.Refresh2) &&
					await app.Actions.CastSpell(refreshSpell)) return true;

			if (app.Options.Config.SelfProtect &&
					!app.Healer.HasAnyBuff(Buffs.Protect) &&
					await app.Actions.CastSpell(protectSpell)) return true;

			if (app.Options.Config.SelfShell &&
					!app.Healer.HasAnyBuff(Buffs.Shell) &&
					await app.Actions.CastSpell(shellSpell)) return true;

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
					(await app.Actions.CastSpell("Utsusemi: San") ||
					 await app.Actions.CastSpell("Utsusemi: Ni") ||
					 await app.Actions.CastSpell("Utsusemi: Ichi"))) return true;

			if (!string.IsNullOrWhiteSpace(app.Options.Config.SelfBarElementSpellName) &&
					await CastBarElementSpell(app)) return true;

			if (!string.IsNullOrWhiteSpace(app.Options.Config.SelfBarStatusSpellName) &&
					await CastBarStatusSpell(app)) return true;

			if (await CastStormSpell(app)) return true;
			if (await CastSpikesSpell(app)) return true;
			if (await CastEnspell(app)) return true;
			if (await CastBoostSpell(app, boostSpell)) return true;

			return false;
		}

		private async Task<bool> CastEnspell(AppViewModel app)
		{
			var spellName = app.Options.Config.SelfEnspellName;
			if (string.IsNullOrWhiteSpace(spellName)) return false;

			short[] buffs = spellName switch
			{
				string s when s.StartsWith("Enfire") => new[] { Buffs.EnfireII, Buffs.Enfire },
				string s when s.StartsWith("Enstone") => new[] { Buffs.EnstoneII, Buffs.Enstone },
				string s when s.StartsWith("Enwater") => new[] { Buffs.EnwaterII, Buffs.Enwater },
				string s when s.StartsWith("Enaero") => new[] { Buffs.EnaeroII, Buffs.Enaero },
				string s when s.StartsWith("Enthunder") => new[] { Buffs.EnthunderII, Buffs.Enthunder },
				string s when s.StartsWith("Enblizzard") => new[] { Buffs.EnblizzardII, Buffs.EnblizzardII },
				_ => new short[0]
			};

			return
				!app.Buffs.HasAny(app.Healer.Player.Name, buffs) &&
				await app.Actions.CastSpell(spellName);
		}

		private async Task<bool> CastStormSpell(AppViewModel app)
		{
			var configSpell = app.Options.Config.SelfStormSpellName;
			var spellName = app.Spells.FirstAvailable($"{configSpell} II", configSpell) ?? "";
			if (string.IsNullOrWhiteSpace(spellName)) return false;

			short[] buffs = spellName switch
			{
				string s when s.StartsWith("Firestorm") => new[] { Buffs.Firestorm, Buffs.Firestorm2 },
				string s when s.StartsWith("Sandstorm") => new[] { Buffs.Sandstorm, Buffs.Sandstorm2 },
				string s when s.StartsWith("Rainstorm") => new[] { Buffs.Rainstorm, Buffs.Rainstorm2 },
				string s when s.StartsWith("Windstorm") => new[] { Buffs.Windstorm, Buffs.Windstorm2 },
				string s when s.StartsWith("Thunderstorm") => new[] { Buffs.Thunderstorm, Buffs.Thunderstorm2 },
				string s when s.StartsWith("Hailstorm") => new[] { Buffs.Hailstorm, Buffs.Hailstorm2 },
				string s when s.StartsWith("Aurorastorm") => new[] { Buffs.Aurorastorm, Buffs.Aurorastorm2 },
				string s when s.StartsWith("Voidstorm") => new[] { Buffs.Voidstorm, Buffs.Voidstorm2 },
				_ => new short[0]
			};

			return
				!app.Buffs.HasAny(app.Healer.Player.Name, buffs) &&
				await app.Actions.CastSpell(spellName);
		}

		private async Task<bool> CastSpikesSpell(AppViewModel app)
		{
			var spellName = app.Options.Config.SelfSpikesSpellName;
			if (string.IsNullOrWhiteSpace(spellName)) return false;

			short buff = spellName switch
			{
				"Ice Spikes" => Buffs.IceSpikes,
				"Shock Spikes" => Buffs.ShockSpikes,
				"Blaze Spikes" => Buffs.BlazeSpikes,
				_ => -1
			};

			return
				!app.Buffs.HasAny(app.Healer.Player.Name, buff) &&
				await app.Actions.CastSpell(spellName);
		}

		private async Task<bool> CastBarElementSpell(AppViewModel app)
		{
			if (app.Options.Config.SelfBarElementSpellName == "Fire" &&
					!app.Healer.HasAnyBuff(Buffs.Barfire) &&
					(await app.Actions.CastSpell("Barfira") ||
					 await app.Actions.CastSpell("Barfire"))) return true;

			if (app.Options.Config.SelfBarElementSpellName == "Earth" &&
					!app.Healer.HasAnyBuff(Buffs.Barstone) &&
					(await app.Actions.CastSpell("Barstonra") ||
					 await app.Actions.CastSpell("Barstone"))) return true;

			if (app.Options.Config.SelfBarElementSpellName == "Water" &&
					!app.Healer.HasAnyBuff(Buffs.Barwater) &&
					(await app.Actions.CastSpell("Barwatera") ||
					 await app.Actions.CastSpell("Barwater"))) return true;

			if (app.Options.Config.SelfBarElementSpellName == "Wind" &&
					!app.Healer.HasAnyBuff(Buffs.Baraero) &&
					(await app.Actions.CastSpell("Baraera") ||
					 await app.Actions.CastSpell("Baraero"))) return true;

			if (app.Options.Config.SelfBarElementSpellName == "Thunder" &&
					!app.Healer.HasAnyBuff(Buffs.Barthunder) &&
					(await app.Actions.CastSpell("Barthundra") ||
					 await app.Actions.CastSpell("Barthunder"))) return true;

			if (app.Options.Config.SelfBarElementSpellName == "Ice" &&
					!app.Healer.HasAnyBuff(Buffs.Barblizzard) &&
					(await app.Actions.CastSpell("Barblizzara") ||
					 await app.Actions.CastSpell("Barblizzard"))) return true;

			return false;
		}

		private async Task<bool> CastBarStatusSpell(AppViewModel app)
		{
			if (app.Options.Config.SelfBarStatusSpellName == "Silence" &&
					!app.Healer.HasAnyBuff(Buffs.Barsilence) &&
					(await app.Actions.CastSpell("Barsilencera") ||
					 await app.Actions.CastSpell("Barsilence"))) return true;

			if (app.Options.Config.SelfBarStatusSpellName == "Amnesia" &&
					!app.Healer.HasAnyBuff(Buffs.Baramnesia) &&
					(await app.Actions.CastSpell("Baramnesra") ||
					 await app.Actions.CastSpell("Baramnesia"))) return true;

			if (app.Options.Config.SelfBarStatusSpellName == "Virus" &&
					!app.Healer.HasAnyBuff(Buffs.Barvirus) &&
					(await app.Actions.CastSpell("Barvira") ||
					 await app.Actions.CastSpell("Barvirus"))) return true;

			if (app.Options.Config.SelfBarStatusSpellName == "Paralyze" &&
					!app.Healer.HasAnyBuff(Buffs.Barparalyze) &&
					(await app.Actions.CastSpell("Barparalyzra") ||
					 await app.Actions.CastSpell("Barparalyze"))) return true;

			if (app.Options.Config.SelfBarStatusSpellName == "Petrify" &&
					!app.Healer.HasAnyBuff(Buffs.Barpetrify) &&
					(await app.Actions.CastSpell("Barpetra") ||
					 await app.Actions.CastSpell("Barpetrify"))) return true;

			if (app.Options.Config.SelfBarStatusSpellName == "Poison" &&
					!app.Healer.HasAnyBuff(Buffs.Barpoison) &&
					(await app.Actions.CastSpell("Barpoisonra") ||
					 await app.Actions.CastSpell("Barpoison"))) return true;

			if (app.Options.Config.SelfBarStatusSpellName == "Blind" &&
					!app.Healer.HasAnyBuff(Buffs.Barblind) &&
					(await app.Actions.CastSpell("Barblindra") ||
					 await app.Actions.CastSpell("Barblind"))) return true;

			if (app.Options.Config.SelfBarStatusSpellName == "Sleep" &&
					!app.Healer.HasAnyBuff(Buffs.Barsleep) &&
					(await app.Actions.CastSpell("Barsleepra") ||
					 await app.Actions.CastSpell("Barsleep"))) return true;

			return false;
		}

		private async Task<bool> CastBoostSpell(AppViewModel app, string boostSpell)
		{
			if (string.IsNullOrWhiteSpace(boostSpell)) return false;
			var suffix = boostSpell.Substring(boostSpell.Length - 3);

			short[] buffs = suffix switch
			{
				"AGI" => new[] { Buffs.AGIBoost, Buffs.AGIBoost2, Buffs.AGIBoost3 },
				"CHR" => new[] { Buffs.CHRBoost, Buffs.CHRBoost2, Buffs.CHRBoost3 },
				"DEX" => new[] { Buffs.DEXBoost, Buffs.DEXBoost2, Buffs.DEXBoost3 },
				"INT" => new[] { Buffs.INTBoost, Buffs.INTBoost2, Buffs.INTBoost3 },
				"MND" => new[] { Buffs.MNDBoost, Buffs.MNDBoost2, Buffs.MNDBoost3 },
				"STR" => new[] { Buffs.STRBoost, Buffs.STRBoost2, Buffs.STRBoost3 },
				"VIT" => new[] { Buffs.VITBoost, Buffs.VITBoost2, Buffs.VITBoost3 },
				_ => new short[0],
			};

			if (buffs.Length > 0 && !app.Healer.HasAnyBuff(buffs) &&
					await app.Actions.CastSpell(boostSpell)) return true;

			return false;
		}

		private async Task<bool> BuffOthers(AppViewModel app)
		{
			var hasteSpell = app.Spells.FirstAvailable("Haste II", "Haste");
			var refreshSpell = app.Spells.FirstAvailable("Refresh III", "Refresh II", "Refresh");
			var regenSpell = app.Spells.FirstAvailable("Regen V", "Regen IV", "Regen III", "Regen II", "Regen");
			var protectSpell = app.Spells.FirstAvailable("Protect V", "Protect IV", "Protect III", "Protect II", "Protect");
			var shellSpell = app.Spells.FirstAvailable("Shell V", "Shell IV", "Shell III", "Shell II", "Shell");

			foreach (var player in app.ActivePlayers.SortByJob())
			{
				// skip primary player on this run
				if (app.Healer.Player.Name == player.Name) continue;

				if (player.IsEnabled && player.Name != app.Healer.Player.Name)
				{
					if (player.AutoBuffs.Haste &&
							player.GetBuffAge(Buffs.Haste) > app.Options.Config.AutoHasteSeconds &&
							await app.Actions.CastSpell(hasteSpell, player.Name))
          {
						player.ResetBuffTimer(Buffs.Haste);
						return true;
					}

					if (player.AutoBuffs.Refresh && player.IsInHealerParty &&
							player.GetBuffAge(Buffs.Refresh) > app.Options.Config.AutoRefreshSeconds &&
							await app.Actions.CastSpell(refreshSpell, player.Name)) return true;

					if (player.AutoBuffs.Protect &&
							player.GetBuffAge(Buffs.Protect) > app.Options.Config.AutoProtectSeconds &&
							await app.Actions.CastSpell(protectSpell, player.Name))
          {
						player.ResetBuffTimer(Buffs.Protect);
						return true;
					}

					if (player.AutoBuffs.Shell &&
							player.GetBuffAge(Buffs.Shell) > app.Options.Config.AutoShellSeconds &&
							await app.Actions.CastSpell(shellSpell, player.Name))
          {
						player.ResetBuffTimer(Buffs.Shell);
						return true;
					}

					if (player.AutoBuffs.Regen &&
							player.GetBuffAge(Buffs.Regen) > app.Options.Config.AutoRegenSeconds &&
							await app.Actions.CastSpell(regenSpell, player.Name))
          {
						player.ResetBuffTimer(Buffs.Regen);
						return true;
					}

					if (player.AutoBuffs.Phalanx && player.IsInHealerParty &&
							player.GetBuffAge(Buffs.Phalanx) > app.Options.Config.AutoPhalanxSeconds &&
							await app.Actions.CastSpell("Phalanx II", player.Name)) return true;

					if (player.AutoBuffs.Firestorm && player.IsInHealerParty &&
							player.GetBuffAge(Buffs.Firestorm, Buffs.Firestorm2) > app.Options.Config.AutoStormSeconds &&
							await app.Actions.CastSpell(app.Spells.FirstAvailable("Firestorm II", "Firestorm"), player.Name)) return true;

					if (player.AutoBuffs.Sandstorm && player.IsInHealerParty &&
							player.GetBuffAge(Buffs.Sandstorm, Buffs.Sandstorm2) > app.Options.Config.AutoStormSeconds &&
							await app.Actions.CastSpell(app.Spells.FirstAvailable("Sandstorm II", "Sandstorm"), player.Name)) return true;

					if (player.AutoBuffs.Rainstorm && player.IsInHealerParty &&
							player.GetBuffAge(Buffs.Rainstorm, Buffs.Rainstorm2) > app.Options.Config.AutoStormSeconds &&
							await app.Actions.CastSpell(app.Spells.FirstAvailable("Rainstorm II", "Rainstorm"), player.Name)) return true;

					if (player.AutoBuffs.Windstorm && player.IsInHealerParty &&
							player.GetBuffAge(Buffs.Windstorm, Buffs.Windstorm2) > app.Options.Config.AutoStormSeconds &&
							await app.Actions.CastSpell(app.Spells.FirstAvailable("Windstorm II", "Windstorm"), player.Name)) return true;

					if (player.AutoBuffs.Thunderstorm && player.IsInHealerParty &&
							player.GetBuffAge(Buffs.Thunderstorm, Buffs.Thunderstorm2) > app.Options.Config.AutoStormSeconds &&
							await app.Actions.CastSpell(app.Spells.FirstAvailable("Thunderstorm II", "Thunderstorm"), player.Name)) return true;

					if (player.AutoBuffs.Hailstorm && player.IsInHealerParty &&
							player.GetBuffAge(Buffs.Hailstorm, Buffs.Hailstorm2) > app.Options.Config.AutoStormSeconds &&
							await app.Actions.CastSpell(app.Spells.FirstAvailable("Hailstorm II", "Hailstorm"), player.Name)) return true;

					if (player.AutoBuffs.Aurorastorm && player.IsInHealerParty &&
							player.GetBuffAge(Buffs.Aurorastorm, Buffs.Aurorastorm2) > app.Options.Config.AutoStormSeconds &&
							await app.Actions.CastSpell(app.Spells.FirstAvailable("Aurorastorm II", "Aurorastorm"), player.Name)) return true;

					if (player.AutoBuffs.Voidstorm && player.IsInHealerParty &&
							player.GetBuffAge(Buffs.Voidstorm, Buffs.Voidstorm2) > app.Options.Config.AutoStormSeconds &&
							await app.Actions.CastSpell(app.Spells.FirstAvailable("Voidstorm II", "Voidstorm"), player.Name)) return true;
				}
			}

			return false;
		}
	}
}
