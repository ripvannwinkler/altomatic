using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Altomatic.UI.Game.Data;
using Altomatic.UI.Utilities;
using Altomatic.UI.ViewModels;

namespace Altomatic.UI.Game
{
	public class ActionManager
	{
		public AppViewModel App { get; }

		public ActionManager(AppViewModel App)
		{
			this.App = App;
		}

		public async Task<bool> UseItem(string itemName, string targetName = "<me>")
		{
			return await Task.Run<bool>(async () =>
			{
				if (App.IsPlayerMoving) return false;
				if (!App.Healer.HasItem(itemName)) return false;
				if (targetName == "<me>" && App.Healer.Player.Buffs.Contains(Buffs.Medicine))
				{
					if (Items.Medicines.Contains(itemName)) return false;
				}

				App.SetStatus($"Using item {itemName} on {targetName}");
				await App.Healer.SendCommand($"/ja \"{itemName}\" {targetName}", 3000);
				return true;
			});
		}

		public async Task<bool> UseAbility(string abilityName, string targetName = "<me>")
		{
			return await Task.Run<bool>(async () =>
			{
				if (App.IsPlayerMoving) return false;
				if (!App.Jobs.CanUseAbility(abilityName)) return false;
				App.SetStatus($"Using ability {abilityName} on {targetName}");
				await App.Healer.SendCommand($"/ja \"{abilityName}\" {targetName}", 2000);
				return true;
			});
		}

		public async Task<bool> CastSpell(string spellName, string targetName = "<me>")
		{
			return await Task.Run<bool>(async () =>
			{
				if (App.IsPlayerMoving) return false;
				if (!App.Spells.CanCast(spellName)) return false;
				App.SetStatus($"Casting {spellName} on {targetName}");

				var casting = false;
				var completed = false;
				var interrupted = false;

				using var sub = App.Addon.Events.Subscribe(@event =>
				{
					switch (@event.Type)
					{
						case AddonEventType.CastingStarted:
							casting = true;
							break;

						case AddonEventType.CastingInteruppted:
							interrupted = true;
							break;

						case AddonEventType.CastingCompleted:
							completed = true;
							break;
					}
				});

				var timer = Stopwatch.StartNew();
				var spellInfo = App.Healer.Resources.GetSpell(spellName, 0);
				await App.Healer.SendCommand($"/ma \"{spellName}\" {targetName}", 750);

				while (casting)
				{
					var castPercent = App.Healer.CastBar.Percent;
					if (castPercent == 1) break;

					if (interrupted)
					{
						await Task.Delay(3000);
						break;
					}

					if (completed)
					{
						await Task.Delay(3000);
						break;
					}

					await Task.Delay(200);
					continue;
				}

				return true;
			});
		}
	}
}
