using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altomatic.UI.Game.Data;
using Altomatic.UI.ViewModels;

namespace Altomatic.UI.Game.Strategies
{
  public class SleepCharmedPlayersStrategy : IGameStrategy
  {
    private static readonly string[] sleepSpells = new[]
    {
      "Repose", "Foe Lullaby II", "Foe Lullaby", "Sleep II", "Sleep", "Sleepga"
    };

    public async Task<bool> ExecuteAsync(AppViewModel app)
    {
      if (!app.Options.Config.SleepCharmedPlayers) return false;

      foreach (var player in app.ActivePlayers)
      {
        if (player.HasAnyBuff(Buffs.Charm, Buffs.Charm2))
        {
          foreach (var spell in sleepSpells)
          {
            if (await app.Actions.CastSpell(spell, player.Name))
            {
              return true;
            }
          }
        }
      }
      
      return false;
    }
  }
}
