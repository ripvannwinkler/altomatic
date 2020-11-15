using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altomatic.UI.Utilities;
using Altomatic.UI.ViewModels;

namespace Altomatic.UI.Game.Data
{
	public class Spells
	{
		private DateTime lastWarning;

		public AppViewModel App { get; }

		public Spells(AppViewModel app)
		{
			App = app ?? throw new ArgumentNullException(nameof(app));
		}

		public async Task<bool> CanCast(string spellName)
		{
			if (IsCasterDisabled()) return false;
			if (App.Healer.Menu.IsMenuOpen) return false;
			if (string.IsNullOrWhiteSpace(spellName)) return false;
			var spellInfo = App.Healer.Resources.GetSpell(spellName, 0);
			if (spellInfo == null) return false;

			if (HasRequiredJob(spellName) && HasAccessTo(spellName) &&
					App.Healer.Recast.GetSpellRecast(spellInfo.Index) == 0)
			{
				if (spellInfo.MPCost <= App.Healer.Player.MP)
				{
					return true;
				}
				else
				{
					if (DateTime.Now.Subtract(lastWarning).TotalSeconds > 30)
					{
						var spell = spellName ?? "???";
						var player = App.Healer?.Player?.Name ?? "???";
						await App.Monitored.SendCommand($"/echo \x1e\x5Tried to cast {spell} but {player}'s MP is too low.\x1f\x5", 200);
						lastWarning = DateTime.Now;
					}

					return false;
				}
			}

			return false;
		}

		public bool HasAccessTo(string spellName)
		{
			var spell = App.Healer.Resources.GetSpell(spellName, 0);
			if (spell == null) return false;

			return
				HasRequiredJob(spellName) &&
				App.Healer.Player.HasSpell(spell.Index);
		}

		public string FirstAvailable(params string[] spellNames)
		{
			return spellNames
				.Where(s => !string.IsNullOrWhiteSpace(s))
				.FirstOrDefault(s => HasAccessTo(s));
		}

		private bool IsCasterDisabled()
		{
			return App.Healer.HasAnyBuff(
				Buffs.Sleep, Buffs.Petrification, Buffs.Stun,
				Buffs.Silence, Buffs.Terror);
		}

		private bool HasRequiredJob(string spellName)
		{
			var spell = App.Healer.Resources.GetSpell(spellName, 0);
			if (string.IsNullOrWhiteSpace(spell?.Name[0])) return false;
			if (spell.Name[0] == "Honor March") return true;

			var ucSpellName = spell.Name[0].ToUpper();
			var mainJobLevel = spell.LevelRequired[App.Healer.Player.MainJob];
			var subJobLevel = spell.LevelRequired[App.Healer.Player.SubJob];

			if ((mainJobLevel > 0 && App.Healer.Player.MainJobLevel >= mainJobLevel) ||
					(subJobLevel > 0 && App.Healer.Player.SubJobLevel >= subJobLevel))
			{
				return true;
			}

			if (mainJobLevel > 99 && App.Healer.Player.MainJobLevel == 99)
			{
				var jp = App.Healer.Player.GetJobPoints(App.Healer.Player.MainJob);

				if (App.Jobs.IsMainJob(App.Healer.Player, "RDM"))
				{
					if (jp.SpentJobPoints >= 1200)
					{
						if (ucSpellName == "REFRESH III" || ucSpellName == "TEMPER II") return true;
					}

					if (jp.SpentJobPoints >= 550)
					{
						if (ucSpellName == "DISTRACT III" || ucSpellName == "FRAZZLE III") return true;
					}
				}

				if (App.Jobs.IsMainJob(App.Healer.Player, "SCH"))
				{
					if (jp.SpentJobPoints >= 100)
					{
						if (ucSpellName.EndsWith("STORM II")) return true;
					}
				}

				if (App.Jobs.IsMainJob(App.Healer.Player, "WHM"))
				{
					if (jp.SpentJobPoints >= 1200)
					{
						if (ucSpellName == "FULL CURE") return true;
					}

					if (jp.SpentJobPoints >= 100)
					{
						if (ucSpellName == "RERAISE IV") return true;
					}
				}
			}

			return false;
		}
	}
}
