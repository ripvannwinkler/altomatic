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
	public class RaiseTheDeadStrategy : IGameStrategy
	{
		private readonly List<string> raisedPlayers = new List<string>();

		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			foreach (var player in app.ActivePlayers.SortByJob(JobSort.TanksFirst))
			{
				if (player.CurrentHp > 0)
				{
					raisedPlayers.Remove(player.Name);
				}
				else if (await RaisePlayer(player, app))
				{
					if (!raisedPlayers.Contains(player.Name))
					{
						raisedPlayers.Add(player.Name);
						return true;
					}
				}
			}

			return false;
		}

		private async Task<bool> RaisePlayer(PlayerViewModel player, AppViewModel app)
		{
			if (await app.Actions.CastSpell("Arise", player.Name) ||
					await app.Actions.CastSpell("Raise III", player.Name) ||
					await app.Actions.CastSpell("Raise II", player.Name) ||
					await app.Actions.CastSpell("Raise", player.Name))
			{
				return true;
			}

			return false;
		}
	}
}
