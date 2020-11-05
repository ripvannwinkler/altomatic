using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altomatic.UI.Game.Data;
using Altomatic.UI.Utilities;
using Altomatic.UI.ViewModels;
using EliteMMO.API;
using static EliteMMO.API.EliteAPI;

namespace Altomatic.UI.Game.Strategies
{
	public class GeomancerStrategy : IGameStrategy
	{
		private uint lastGeoTargetId = 0;
		private string lastIndiSpell = null;
		private bool eclipticActive = false;

		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (app.ActivePlayers.AreHealthy())
			{
				if (await DoGeomancyStuff(app))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> DoGeomancyStuff(AppViewModel app)
		{
			if (await CastIndiSpell(app)) return true;
			if (await CastEntrustIndiSpell(app)) return true;
			if (await CastGeoSpell(app)) return true;
			return false;
		}

		private async Task<bool> CastIndiSpell(AppViewModel app)
		{
			if (!app.Options.Config.EnableIndiSpells) return false;
			var spellName = app.Options.Config.IndiSpellName;

			// don't recast same indi spell if colure active
			// but do try to recast if indi spell has changed
			if (app.Healer.HasAnyBuff(Buffs.ColureActive) &&
					lastIndiSpell == spellName) return false;

			if (await app.Actions.CastSpell($"Indi-{spellName}"))
			{
				lastIndiSpell = spellName;
				return true;
			}

			return false;
		}

		private async Task<bool> CastEntrustIndiSpell(AppViewModel app)
		{
			var target = GetGeoSpellTarget(app);
			if (!app.Options.Config.EnableIndiSpells) return false;
			if (!app.Options.Config.EnableEntrust) return false;
			if (target == null) return false;

			foreach (var player in app.ActivePlayers)
			{
				if (player.IsInHealerParty && player.IsEntrustTarget)
				{
					if (!app.Buffs.HasAny(player.Name, Buffs.ColureActive))
					{
						if (await app.Actions.UseAbility("Entrust"))
						{
							var spellName = app.Options.Config.EntrustIndiSpellName;
							if (await app.Actions.CastSpell($"Indi-{spellName}", player.Name))
							{
								return true;
							}
						}
					}

					break;
				}
			}

			return false;
		}

		private async Task<bool> CastGeoSpell(AppViewModel app)
		{
			var target = GetGeoSpellTarget(app);
			if (!app.Options.Config.EnableGeoSpells) return false;
			if (target == null) return false;

			if (app.Healer.Player.Pet?.HealthPercent < 1)
			{
				var spellName = app.Options.Config.GeoSpellName;
				var isDebuff = app.Options.Config.GeoDebuffs.Contains(spellName);
				var canUseGeoSpell = isDebuff ? !app.Options.Config.DisableGeoDebuffs : true;

				if (canUseGeoSpell)
				{
					if (app.Options.Config.EnableBlazeOfGlory &&
						await app.Actions.UseAbility("Blaze Of Glory", "<me>"))
					{
						return true;
					}

					app.Healer.Target.SetTarget((int)target.TargetID);
					if (await app.Actions.CastSpell($"Geo-{spellName}", $"<t>"))
					{
						eclipticActive = false;
						lastGeoTargetId = target.TargetID;
						return true;
					}
				}
			}
			else
			{
				if (eclipticActive == false &&
						app.Options.Config.EnableEclipticAttrition &&
						await app.Actions.UseAbility("Ecliptic Attrition"))
				{
					eclipticActive = true;
					return true;
				}

				if (app.Options.Config.EnableDematerialize &&

						await app.Actions.UseAbility("Dematerialize"))
				{
					return true;
				}

				if (app.Options.Config.EnableLifeCycle &&
						app.Healer.Player.Pet.HealthPercent < 50 &&
						await app.Actions.UseAbility("Life Cycle"))
				{
					return true;
				}

				if (app.Healer.Player.MP < 100 &&
						app.Options.Config.EnableRadialArcana &&
						app.Healer.Player.Pet.HealthPercent > 50 &&
						await app.Actions.UseAbility("Radial Arcana"))
				{
					return true;
				}

				if (await EstablishHate(app, target))
				{
					return true;
				}

				if (await UseFullCircle(app))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> UseFullCircle(AppViewModel app)
		{
			if (!app.Options.Config.EnableFullCircle) return false;
			var petEntity = app.Healer.Entity.GetEntity((int)app.Healer.Player.Pet.TargetID);
			var geoTargetPlayer = app.ActivePlayers.FirstOrDefault(p => p.IsGeoTarget);
			var geoEntity = geoTargetPlayer == null
				? app.Healer.Entity.GetEntity((int)app.Healer.Player.TargetID)
				: app.Healer.Entity.GetEntity((int)geoTargetPlayer.Member.TargetIndex);

			var distance = PlayerUtilities.GetDistance(petEntity, geoEntity);
			if (distance > 8 && await app.Actions.UseAbility("Full Circle"))
			{
				return true;
			}

			return false;
		}

		private async Task<bool> EstablishHate(AppViewModel app, XiEntity target)
		{
			if (target == null) return false;
			if (target.TargetID == lastGeoTargetId) return false;

			// use cure for hate if geo target set
			foreach (var player in app.ActivePlayers)
			{
				if (player.IsGeoTarget)
				{
					if (await app.Actions.CastSpell("Cure", player.Name))
					{
						lastGeoTargetId = target.TargetID;
						return true;
					}
				}
			}

			// otherwise use dia on mob for hate
			app.Healer.Target.SetTarget((int)target.TargetID);
			if (await app.Actions.CastSpell("Dia", "<t>"))
			{
				lastGeoTargetId = target.TargetID;
				return true;
			}

			return false;
		}

		private XiEntity GetGeoSpellTarget(AppViewModel app)
		{
			var geoTargetPlayer = app.Healer.Party.GetPartyMembers()
				.Join(app.ActivePlayers, m => m.Name, p => p.Name, (m, p) => p)
				.Where(p => p.IsInHealerParty).FirstOrDefault(p => p.IsGeoTarget)?
				.Name ?? app.Healer.Player.Name;

			for (var i = 0; i < 2048; i++)
			{
				var entity = app.Healer.Entity.GetEntity(i);
				if (entity.Name == geoTargetPlayer)
				{
					var target = entity.TargetingIndex;
					var engaged = entity.Status == (int)EntityStatus.Engaged;

					if (engaged && target > 0)
					{
						return app.Healer.Entity.GetEntity(target);
					}
				}
			}

			if (app.Healer.GetEntityStatus() == EntityStatus.Engaged)
			{
				return app.Healer.GetTargetEntity();
			}

			return null;
		}
	}
}
