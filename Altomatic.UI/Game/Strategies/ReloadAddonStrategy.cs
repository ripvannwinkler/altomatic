using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altomatic.UI.ViewModels;

namespace Altomatic.UI.Game.Strategies
{
	public class ReloadAddonStrategy : IGameStrategy
	{
		private DateTime lastEvent = DateTime.Now;
		private bool subscribed = false;

		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (!subscribed)
			{
				subscribed = true;
				app.Addon.Events.Subscribe(e =>
				{
					lastEvent = DateTime.Now;
				});
			}

			if (!string.IsNullOrWhiteSpace(app.Healer?.Player?.Name))
			{
				if (DateTime.Now.Subtract(lastEvent).TotalSeconds > 30)
				{
					await app.ReloadAddon();
				}
			}

			return false;
		}
	}
}
