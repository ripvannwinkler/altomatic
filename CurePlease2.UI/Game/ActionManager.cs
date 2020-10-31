using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CurePlease2.UI.ViewModels;

namespace CurePlease2.UI.Game
{
	public class ActionManager
	{
		static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

		public AppViewModel App { get; }

		public ActionManager(AppViewModel App)
		{
			this.App = App;
		}

		public async Task<bool> UseItem(string itemName, string targetName = "<me>")
		{
			return await DoAction(() =>
			{
				App.StatusMessage = $"Using item {itemName} on {targetName}";
				return true;
			});
		}

		public async Task<bool> UseAbility(string abilityName, string targetName = "<me>")
		{
			return await DoAction(() =>
			{
				App.StatusMessage = $"Using ability {abilityName} on {targetName}";
				return true;
			});
		}

		public async Task<bool> CastSpell(string spellName, string targetName = "<me>")
		{
			return await DoAction(() =>
			{
				App.StatusMessage = $"Casting {spellName} on {targetName}";
				return true;
			});
		}

		protected async Task<bool> DoAction(Func<bool> action)
		{
			if (await semaphore.WaitAsync(500))
			{
				try { return action.Invoke(); }
				finally { semaphore.Release(); }
			}

			return false;
		}
	}
}
