using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altomatic.UI.ViewModels;

namespace Altomatic.UI.Game
{
  public interface IGameStrategy
  {
    /// <summary>
    /// Execute this strategy
    /// </summary>
    /// <param name="app"></param>
    /// <returns>True if execution should continue, false otherwise.</returns>
    Task<bool> ExecuteAsync(AppViewModel app);
  }
}
