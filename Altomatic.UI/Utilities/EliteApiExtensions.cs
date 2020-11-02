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
  }
}
