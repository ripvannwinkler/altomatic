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
		private bool resumePause;
		private DateTime lastPosChange;
		private Point3D lastPosition;

		/// <inheritdoc/>
		public Task<bool> ExecuteAsync(AppViewModel app)
		{
			var members = app.Monitored.Party.GetPartyMembers();

			for (var i = 0; i < members.Count; i++)
			{
				if (members.Count >= i)
				{
					UpdatePlayerInfo(app, members[i]);
				}
			}

			HandleZoning(app);
			UpdateActiveBuffs(app);
			return Task.FromResult(false);
		}

    private void HandleZoning(AppViewModel app)
		{
			if (app.Healer.Player.LoginStatus == (int)LoginStatus.Loading ||
					app.Healer.Player.LoginStatus == (int)LoginStatus.Loading)
			{
				if (!app.IsPaused)
				{
					resumePause = true;
					app.IsPaused = true;
				}
			}
			else
			{
				if (resumePause)
				{
					resumePause = false;
					app.IsPaused = false;
				}
			}
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
