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
	public class RemoveCriticalDebuffStrategy : IGameStrategy
	{
		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (await RemoveDoomFromHealer(app) ||
					await RemoveSilenceFromHealer(app) ||
					await RemoveParalyzeFromHealer(app) ||
					await RemoveSilenceFromPlayers(app) ||
					await RemovePetrifyFromPlayers(app))
      {
				return true;
			}

			return false;
		}

		private async Task<bool> RemoveDoomFromHealer(AppViewModel app)
		{
			if (app.Healer.HasAnyBuff(Buffs.Doom, Buffs.Curse))
			{
				if (app.Options.Config.PreferItemOverCursna)
				{
					if (await app.Actions.UseItem("Hallowed Water") ||
							await app.Actions.UseItem("Holy Water"))
					{
						return true;
					}
				}

				if (await app.Actions.CastSpell("Cursna"))
				{
					return true;
				}
			}

			return false;
		}
		
		private async Task<bool> RemoveSilenceFromHealer(AppViewModel app)
    {
			if (app.Healer.HasAnyBuff(Buffs.Silence))
			{
				if (await app.Actions.UseItem("Echo Drops") ||
						await app.Actions.UseItem("Remedy"))
        {
					return true;
				}
			}

			return false;
		}
		
		private async Task<bool> RemoveParalyzeFromHealer(AppViewModel app)
		{
			if (app.Healer.HasAnyBuff(Buffs.Paralysis))
			{
				if (app.Options.Config.PreferItemOverParalyna &&
						await app.Actions.UseItem("Remedy"))
        {
					return true;
				}

				if (await app.Actions.CastSpell("Paralyna"))
        {
					return true;
				}
			}

			return false;
		}

		private async Task<bool> RemoveSilenceFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob(JobSort.HealersFirst))
			{
				if (app.Buffs.HasAny(player.Name, Buffs.Silence) &&
						await app.Actions.CastSpell("Silena", player.Name))
        {
					return true;
				}
			}

			return false;
		}

    private async Task<bool> RemovePetrifyFromPlayers(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob(JobSort.HealersFirst))
			{
				if (app.Buffs.HasAny(player.Name, Buffs.Petrification) &&
						await app.Actions.CastSpell("Stona", player.Name))
				{
					return true;
				}
			}

			return false;
		}
	}
}
