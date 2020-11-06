using System;
using System.Threading.Tasks;
using Altomatic.UI.Utilities;
using Altomatic.UI.ViewModels;

namespace Altomatic.UI.Game.Strategies
{
  public class CureWarningStrategy : IGameStrategy
	{
		private DateTime lastWarning;

		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			await Task.Yield();
			foreach (var player in app.Players)
			{
				if (!player.IsEnabled) continue;
				if (string.IsNullOrWhiteSpace(player.Name)) continue;
				if (player.CurrentHpp <= app.Options.Config.CureThreshold)
				{
					if (player.DistanceFromHealer >= 21)
					{
						if (DateTime.Now.Subtract(lastWarning).TotalSeconds > 10)
						{
							lastWarning = DateTime.Now;
							await app.Healer.SendCommand($"/echo {player.Name} is out of range and cannot be cured.", 200);
						}
					}
				}
			}

			return false;
		}
	}
}
