﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurePlease2.UI.ViewModels;

namespace CurePlease2.UI.Game
{
  public class DummyGameStrategy : IGameStrategy
  {
    public Task<bool> ExecuteAsync(AppViewModel app)
    {
      Debug.WriteLine("Dummy strategy executing...");
      return Task.FromResult(false);
    }
  }
}
