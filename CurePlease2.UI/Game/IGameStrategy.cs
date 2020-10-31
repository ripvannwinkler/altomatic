using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurePlease2.UI.ViewModels;

namespace CurePlease2.UI.Game
{
  public interface IGameStrategy
  {
    void Execute(AppViewModel appData);
  }
}
