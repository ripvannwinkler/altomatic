﻿using System;
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
		public AppViewModel AppData { get; }

		public ActionManager(AppViewModel app)
		{
			AppData = app;
		}

		public async Task<bool> UseItem(string itemName, string targetName = "<me>")
		{
			if (AppData.Healer.IsDead()) return false;
			if (AppData.IsPlayerMoving) return false;
			if (!AppData.Healer.HasItem(itemName)) return false;
			if (targetName == "<me>" && AppData.Healer.Player.Buffs.Contains(Buffs.Medicine))
			{
				if (Items.Medicines.Contains(itemName)) return false;
			}

			AppData.SetStatus($"Using item {itemName} on {targetName}");
			await AppData.Healer.SendCommand($"/ja \"{itemName}\" {targetName}", 3000);
			return true;
		}

		public async Task<bool> UseAbility(string abilityName, string targetName = "<me>")
		{
			if (AppData.Healer.IsDead()) return false;
			if (AppData.IsPlayerMoving) return false;
			if (!AppData.Jobs.CanUseAbility(abilityName)) return false;

			// skip ability attempt if target player is dead
			var targetPlayer = AppData.Players.FirstOrDefault(x => x.Name == targetName);
			if (targetPlayer != null)
			{
				if (targetPlayer.CurrentHp < 1) return false;
			}

			AppData.SetStatus($"Using ability {abilityName} on {targetName}");
			await AppData.Healer.SendCommand($"/ja \"{abilityName}\" {targetName}", 2000);
			return true;
		}

		public async Task<bool> CastSpell(string spellName, string targetName = "<me>")
		{
			if (!await AppData.Spells.CanCast(spellName))
			{
				Debug.WriteLine($"Player cannot cast {spellName} at this time.");
				return false;
			}

			if (AppData.IsPlayerMoving)
			{
				Debug.WriteLine($"Player is moving. Cannot cast {spellName} at this time.");
				return false;
			}

			if (AppData.Healer.IsDead())
			{
				Debug.WriteLine($"Player is dead. Cannot cast {spellName} at this time.");
				return false;
			}

			// skip spell cast attempt if target player is dead and not a raise spell
			var targetPlayer = AppData.Players.FirstOrDefault(x => x.Name == targetName);
			if (targetPlayer != null)
			{
				if (targetPlayer.CurrentHp < 1)
				{
					var raise = new[] { "Arise", "Raise III", "Raise II", "Raise" };
					if (!raise.Contains(spellName))
					{
						Debug.WriteLine($"{targetName} is dead and {spellName} is not a raise spell.");
						return false;
					}
				}
			}

			Debug.WriteLine($"Casting {spellName} on {targetName}...");
			AppData.SetStatus($"Casting {spellName} on {targetName}...");
			var cts = new CancellationTokenSource();

			var casting = false;
			var completed = false;
			var interrupted = false;

			using var sub = AppData.Addon.Events.Subscribe(@event =>
			{
				switch (@event.Type)
				{
					case AddonEventType.CastingStarted:
						Debug.WriteLine($"Casting {spellName} started...");
						casting = true;
						cts.Cancel();
						break;

					case AddonEventType.CastingInteruppted:
						Debug.WriteLine($"Casting {spellName} interrupted...");
						interrupted = true;
						cts.Cancel();
						break;

					case AddonEventType.CastingCompleted:
						Debug.WriteLine($"Casting {spellName} completed...");
						completed = true;
						cts.Cancel();
						break;
				}
			});

			// send cast command
			var timer = Stopwatch.StartNew();
			var spellInfo = AppData.Healer.Resources.GetSpell(spellName, 0);
			await AppData.Healer.SendCommand($"/ma \"{spellName}\" {targetName}", 500);

			// wait for casting to start
			while (timer.ElapsedMilliseconds < 2000)
			{
				if (cts.IsCancellationRequested) break;
				await Task.Delay(200);
			}

			// account for quick magic (instacast)
			if (AppData.Healer.CastBar.Percent == 1)
      {
				casting = false;
			}

			// if still casting, wait for interrupt / completion
			while (casting && timer.ElapsedMilliseconds < 20000)
			{
				if (interrupted)
				{
					// player moved, paralyzed, etc.
					Debug.WriteLine("Spell interrupted. Waiting and exiting loop...");
					await Task.Delay(AppData.Options.Config.GlobalCastDelay);
					break;
				}

				if (completed)
				{
					// spell finished - may occur earlier than 100% cast bar
					Debug.WriteLine("Spell completed. Waiting and exiting loop...");
					await Task.Delay(AppData.Options.Config.GlobalCastDelay);
					break;
				}

				await Task.Delay(250);
			}

			return true;
		}
	}
}
