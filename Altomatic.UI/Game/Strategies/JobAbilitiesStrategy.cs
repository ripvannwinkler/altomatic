using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altomatic.UI.ViewModels;

namespace Altomatic.UI.Game.Strategies
{
  public class JobAbilitiesStrategy : IGameStrategy
  {
    public async Task<bool> ExecuteAsync(AppViewModel app)
    {
      await Task.Yield();
      return false;
    }
  }
}
