using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altomatic.UI.Game.Data;
using Altomatic.UI.ViewModels;
using EliteMMO.API;
using static EliteMMO.API.EliteAPI;

namespace Altomatic.UI.Utilities
{
	public static class EliteApiExtensions
	{
		public static async Task SendCommand(this EliteAPI instance, string command, int delayMs = 1000)
		{
			instance.ThirdParty.SendString(command);
			await Task.Delay(delayMs);
		}

		public static bool HasItem(this EliteAPI instance, string itemName)
		{
			var count = 0U;
			var itemInfo = instance.Resources.GetItem(itemName, 0);
			if (itemInfo == null) return false;

			for (var x = 0; x < 80; x++)
			{
				var item = instance.Inventory.GetContainerItem(0, x);
				if (item.Id == itemInfo.ItemID) count += item.Count;
			}

			return count > 0;
		}

		public static bool HasAnyBuff(this EliteAPI instance, params short[] buffs)
    {
			return instance.Player.Buffs.Intersect(buffs).Any();
    }

		public static IOrderedEnumerable<PlayerViewModel> SortByJob(this IEnumerable<PlayerViewModel> players, JobSort sortStrategy = JobSort.TanksFirst)
    {
			return memoizedSortByJobInternal(players, sortStrategy);
    }

    private static IOrderedEnumerable<PlayerViewModel> SortByJobInternal(IEnumerable<PlayerViewModel> players, JobSort sortStrategy)
    {
      return players.OrderBy(p => p.AppData.Jobs.MapPriority(p.Member, sortStrategy));
    }

		private static Func<IEnumerable<PlayerViewModel>, JobSort, IOrderedEnumerable<PlayerViewModel>> memoizedSortByJobInternal =
			Memoizer.Memoize<IEnumerable<PlayerViewModel>, JobSort, IOrderedEnumerable<PlayerViewModel>>(SortByJobInternal);
	}
}
