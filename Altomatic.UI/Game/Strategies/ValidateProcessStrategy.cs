using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altomatic.UI.ViewModels;

namespace Altomatic.UI.Game.Strategies
{
  public class ValidateProcessStrategy : IGameStrategy
  {
    public Task<bool> ExecuteAsync(AppViewModel app)
    {
      foreach (var process in app.Processes)
      {
        if (process.HasExited)
        {
          app.RefreshProcessList();
          return Task.FromResult(true);
        }
      }

      return Task.FromResult(false);
    }
  }
}
