using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Altomatic.UI.Game.Data;
using Altomatic.UI.Utilities;
using Altomatic.UI.ViewModels;
using EliteMMO.API;

namespace Altomatic.UI.Game.Strategies
{
	public class RefreshPlayerInfoStrategy : IGameStrategy
	{
		/// <inheritdoc/>
		public async Task<bool> ExecuteAsync(AppViewModel app)
		{
			if (app.Monitored == null) return false;

			await Task.Yield();
			foreach (var member in app.Monitored.Party.GetPartyMembers())
      {
				UpdatePlayerInfo(app, member);
      }

			UpdateActiveBuffs(app);
			return false;
		}

		/// <summary>
		/// Update active buff list
		/// </summary>
		private void UpdateActiveBuffs(AppViewModel app)
		{
			app.ActiveBuffs.Clear();
			var myName = app.Healer.Player.Name;
			foreach (var buff in app.Healer.Player.Buffs)
			{
				if (buff > 0)
				{
					var status = new BuffStatus(myName, buff);
					app.ActiveBuffs.Add(status);
				}
			}

			foreach (var playerBuff in app.Buffs)
			{
				app.ActiveBuffs.Add(playerBuff);
			}
		}

		/// <summary>
		/// Update player vitals
		/// </summary>
		private void UpdatePlayerInfo(AppViewModel app, EliteAPI.PartyMember member)
		{
			var healer = app.Healer.Entity.GetLocalPlayer();
			var player = app.Players.ElementAt(member.MemberNumber);
			var playerEntity = app.Healer.Entity.GetEntity((int)member.TargetIndex);

			if (member.Active > 0)
			{
				player.Name = member.Name;
				player.CurrentHp = member.CurrentHP;
				player.CurrentHpp = member.CurrentHPP;
				player.DistanceFromHealer = PlayerUtilities.GetDistance(healer, playerEntity);
				player.Member = member;
			}
			else
			{
				player.Name = "";
				player.CurrentHp = 0;
				player.CurrentHpp = 0;
				player.DistanceFromHealer = double.MaxValue;
				player.Member = null;
			}
		}
	}
}
