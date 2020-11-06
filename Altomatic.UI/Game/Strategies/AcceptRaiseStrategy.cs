using System.Threading.Tasks;
using Altomatic.UI.Utilities;
using Altomatic.UI.ViewModels;
using EliteMMO.API;

namespace Altomatic.UI.Game.Strategies
{
  public class AcceptRaiseStrategy : IGameStrategy
	{
		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (app.Options.Config.AcceptRaise == true &&
					(app.Healer.GetEntityStatus() == EntityStatus.Dead ||
					 app.Healer.GetEntityStatus() == EntityStatus.DeadEngaged))
			{
				if (app.Healer.Menu.IsMenuOpen &&
						app.Healer.Menu.HelpName == "Revival" &&
						app.Healer.Menu.MenuIndex == 1)
				{
					await Task.Delay(3000);
					app.Healer.ThirdParty.KeyPress(EliteMMO.API.Keys.NUMPADENTER);
					await Task.Delay(500);
					return true;
				}
			}

			return false;
		}
	}
}
