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

			if (app.Options.Config.EnableComposure &&
					app.Healer.HasAnyBuff(Buffs.Composure) == false &&
					await app.Actions.UseAbility("Composure"))
			{
				return true;
			}

			if (app.Options.Config.EnableDivineCaress &&
					app.Healer.HasAnyBuff(Buffs.DivineCaress, Buffs.DivineCaress2) == false &&
					await app.Actions.UseAbility("Divine Caress"))
			{
				return true;
			}

			if (app.Options.Config.EnableDivineSeal &&
					app.Healer.Player.MPP < 15 &&
					app.Healer.HasAnyBuff(Buffs.DivineSeal) == false &&
					await app.Actions.UseAbility("Divine Seal"))
			{
				return true;
			}

			if (app.Options.Config.EnableLightArts &&
					app.Healer.HasAnyBuff(Buffs.LightArts) == false &&
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

			if (app.ActivePlayers.AreHealthy())
			{
				if (app.Options.Config.EnableConvert &&
						(app.Healer.Player.MP < 200 || app.Healer.Player.MPP < 15))
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
			}

			return false;
		}

		private string GetDevotionTargetAsync(AppViewModel app)
		{
			var party = app.Healer.Party.GetPartyMembers().Where(x => x.MemberNumber < 6);
			var players = app.ActivePlayers.Where(x => party.Any(y => y.Name == x.Name));

			foreach (var player in players.SortByJob())
			{
				if (player.Member.CurrentMPP < 50)
				{
					var index = (int)player.Member.TargetIndex;
					var entity = app.Healer.Entity.GetEntity(index);
					var job = app.Jobs.GetMainJob(entity);

					if (new[] { "PLD", "RUN", "WHM", "SCH" }.Contains(job))
					{
						return player.Name;
					}
				}
			}

			return null;
		}
	}
}
