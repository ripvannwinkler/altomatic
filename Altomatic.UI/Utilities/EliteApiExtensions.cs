using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EliteMMO.API;
using static EliteMMO.API.EliteAPI;

namespace Altomatic.UI.Utilities
{
  public static class EliteApiExtensions
  {
    public static async Task SendCommand(this EliteAPI instance, string command, int delayMs)
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
  }
}
