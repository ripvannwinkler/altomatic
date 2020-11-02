﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altomatic.UI.ViewModels;

namespace Altomatic.UI.Game.Data
{
	public class Spells
	{
		public AppViewModel App { get; }

		public Spells(AppViewModel app)
		{
			App = app ?? throw new ArgumentNullException(nameof(app));
		}

		public bool CanCast(string spellName)
		{
			var spell = App.Healer.Resources.GetSpell(spellName, 0);
			if (spell == null) return false;

			return
				HasRequiredJob(spellName) &&
				App.Healer.Player.HasSpell(spell.ID) &&
				App.Healer.Recast.GetSpellRecast(spell.Index) == 0 &&
				spell.MPCost<= App.Healer.Player.MP;
		}

		public bool HasRequiredJob(string spellName)
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
					else if (jp.SpentJobPoints >= 550)
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
					else if (jp.SpentJobPoints >= 100)
					{
						if (ucSpellName == "RERAISE IV") return true;
					}
				}
			}

			return false;
		}
	}
}
