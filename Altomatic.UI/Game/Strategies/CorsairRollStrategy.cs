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
	public class CorsairRollStrategy : IGameStrategy
	{
		private CorsairRoll lastRollUsed = new CorsairRoll("", 0);

		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (await UseRandomDeal(app) ||
					await FoldIfBusted(app) ||
					await UseSnakeEye(app) ||
					await DoubleUpRoll(app) ||
					await DoRoll(app, app.Options.Config.Roll1) ||
					await DoRoll(app, app.Options.Config.Roll2))
			{
				return true;
			}

			return false;
		}

		private async Task<bool> UseRandomDeal(AppViewModel app)
		{
			var useAbility = false;
			if (app.Healer.HasAnyBuff(Buffs.Bust) && !app.Jobs.CanUseAbility("Fold")) useAbility = true;
			if (app.LastKnownRoll == lastRollUsed.Unlucky && !app.Jobs.CanUseAbility("Snake Eye")) useAbility = true;
			if (useAbility && await app.Actions.UseAbility("Random Deal")) return true;
			return false;
		}

		private async Task<bool> FoldIfBusted(AppViewModel app)
		{
			if (app.Healer.HasAnyBuff(Buffs.Bust))
			{
				if (await app.Actions.UseAbility("Fold"))
				{
					return true;
				}
			}

			return false;
		}

		private async Task<bool> UseSnakeEye(AppViewModel app)
		{
			if (!app.Healer.HasAnyBuff(Buffs.DoubleUpChance)) return false;

			var useAbility = false;
			if (app.LastKnownRoll == 10) useAbility = true;
			if (app.LastKnownRoll == lastRollUsed.Unlucky) useAbility = true;
			if (app.LastKnownRoll + 1 == lastRollUsed.Lucky) useAbility = true;
			if (useAbility && await app.Actions.UseAbility("Snake Eye")) return true;
			return false;
		}

		private async Task<bool> DoubleUpRoll(AppViewModel app)
		{
			if (app.Healer.HasAnyBuff(Buffs.DoubleUpChance))
			{
				if (app.LastKnownRoll == lastRollUsed.Lucky) return false;

				var goingFor11 = app.LastKnownRoll == 357;
				var isUnluckyRoll = app.LastKnownRoll == lastRollUsed.Unlucky;
				var isLowRoll = app.LastKnownRoll < 7 || app.LastKnownRoll > 11;

				if (goingFor11 || isLowRoll || isUnluckyRoll)
				{
					if (await app.Actions.UseAbility("Double-Up"))
					{
						await Task.Delay(3000);
						return true;
					}
				}
			}

			return false;
		}

		private async Task<bool> DoRoll(AppViewModel app, CorsairRoll roll)
		{
			if (!string.IsNullOrWhiteSpace(roll?.Name) &&
					!app.Healer.HasAnyBuff(roll.BuffId) &&
					await app.Actions.UseAbility(roll?.Name))
			{
				lastRollUsed = roll;
				app.LastKnownRoll = 0;
				await Task.Delay(3000);
				return true;
			}

			return false;
		}
	}
}
