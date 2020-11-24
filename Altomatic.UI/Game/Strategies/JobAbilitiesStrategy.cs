using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altomatic.UI.Game.Data;
using Altomatic.UI.Utilities;
using Altomatic.UI.ViewModels;
using static EliteMMO.API.EliteAPI;

namespace Altomatic.UI.Game.Strategies
{
	public class JobAbilitiesStrategy : IGameStrategy
	{
		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (app.Options.Config.EnableSublimation &&
					app.Healer.HasAnyBuff(Buffs.SublimationActivated, Buffs.SublimationComplete) == false &&
					await app.Actions.UseAbility("Sublimation"))
			{
				return true;
			}

			if (app.Options.Config.EnableAfflatusMisery &&
					app.Healer.HasAnyBuff(Buffs.AfflatusMisery) == false &&
					await app.Actions.UseAbility("Afflatus Misery"))
			{
				return true;
			}

			if (app.Options.Config.EnableAfflatusSolace &&
					app.Healer.HasAnyBuff(Buffs.AfflatusSolace) == false &&
					await app.Actions.UseAbility("Afflatus Solace"))
			{
				return true;
			}

			if (app.Options.Config.EnableSublimation &&
					!app.Healer.HasAnyBuff(Buffs.SublimationActivated, Buffs.SublimationComplete) &&
					await app.Actions.UseAbility("Sublimation"))
			{
				return true;
			}

			if (app.Options.Config.EnableSublimation &&
					app.Healer.HasAnyBuff(Buffs.SublimationComplete) &&
					await app.Actions.UseAbility("Sublimation"))
			{
				return true;
			}

			if (app.Options.Config.EnableSublimation && app.Healer.Player.MP < 200 &&
						app.Healer.HasAnyBuff(Buffs.SublimationActivated, Buffs.SublimationComplete) &&
						await app.Actions.UseAbility("Sublimation"))
			{
				return true;
			}

			if (app.Options.Config.EnableLightArts &&
					app.Healer.HasAnyBuff(Buffs.LightArts, Buffs.AddendumWhite) == false &&
					await app.Actions.UseAbility("Light Arts"))
			{
				return true;
			}

			if (app.Options.Config.EnableAddendumWhite &&
					app.Healer.HasAnyBuff(Buffs.AddendumWhite) == false &&
					await app.Actions.UseAbility("Addendum: White"))
			{
				return true;
			}

			if (app.Options.Config.EnableConvert &&
					!app.Healer.HasAnyBuff(Buffs.Weakness) &&
					(app.Healer.Player.MP < 100 || app.Healer.Player.MPP < 15) &&
					await app.Actions.UseAbility("Convert"))
			{
				return true;
			}

			if (app.Options.Config.EnableDevotion)
			{
				var devotionTarget = GetDevotionTargetAsync(app);
				if (devotionTarget != null && await app.Actions.UseAbility("Devotion", devotionTarget))
				{
					return true;
				}
			}

			return false;
		}

		private string GetDevotionTargetAsync(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob())
			{
				if (player.Name == app.Healer.Player.Name) continue;

				if (player.IsInHealerParty)
				{
					if (player.Member.CurrentMPP < 50)
					{
						var index = (int)player.Member.TargetIndex;
						var job = app.Jobs.GetMainJob(player.Member);

						if (new[] { "PLD", "RUN", "WHM", "SCH" }.Contains(job))
						{
							return player.Name;
						}
					}
				}
			}

			return null;
		}
	}
}
