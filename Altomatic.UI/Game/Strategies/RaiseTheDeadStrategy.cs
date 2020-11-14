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
		private readonly Dictionary<string, DateTime> raisedPlayers = new Dictionary<string, DateTime>();

		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			ClearAgedRaises();
			foreach (var player in app.ActivePlayers.SortByJob(JobSort.TanksFirst))
			{
				if (player.CurrentHp > 0)
				{
					raisedPlayers.Remove(player.Name);
				}
				else if (!raisedPlayers.ContainsKey(player.Name))
				{
					raisedPlayers.Add(player.Name, DateTime.Now);
					if (await RaisePlayer(player, app)) return true;
				}
			}

			return false;
		}

		/// <summary>
    /// Clear raise timers older than 30 seconds.
    /// </summary>
    /// <remarks>
    /// If raise was last attempted on a player more than 30 seconds ago,
    /// clear the raise flag so that it can be attempted again.
    /// </remarks>
		private void ClearAgedRaises()
		{
			var now = DateTime.Now;
			foreach (var entry in raisedPlayers.ToArray())
			{
				if (now.Subtract(entry.Value).TotalSeconds > 30)
				{
					raisedPlayers.Remove(entry.Key);
				}
			}
		}

		/// <summary>
    /// Casts the highest available raise spell on the player.
    /// </summary>
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
