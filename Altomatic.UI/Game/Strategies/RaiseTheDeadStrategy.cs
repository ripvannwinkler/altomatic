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
		/// <summary>
    /// The blacklist holds a list of player names for whom raise should not be attempted
    /// either because they have already been raisd and just not yet accepted, or because
    /// our attempt to raise them failed. Names older than N seconds are pruned automatically.
    /// </summary>
		private readonly Dictionary<string, DateTime> blacklist = new Dictionary<string, DateTime>();

		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			PruneBlacklist();
			foreach (var player in app.ActivePlayers.SortByJob(JobSort.TanksFirst))
			{
				if (player.CurrentHp > 0)
				{
					blacklist.Remove(player.Name);
				}
				else if (!blacklist.ContainsKey(player.Name))
				{
					// Add player to the blacklist regardless of whether the spell succeeds
					// so that we don't try them again too soon in the event that someone
					// else cast raise on them and we get a "cannot be cast on" message.
					blacklist.Add(player.Name, DateTime.Now);

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
		private void PruneBlacklist()
		{
			var now = DateTime.Now;
			foreach (var entry in blacklist.ToArray())
			{
				if (now.Subtract(entry.Value).TotalSeconds > 30)
				{
					blacklist.Remove(entry.Key);
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
