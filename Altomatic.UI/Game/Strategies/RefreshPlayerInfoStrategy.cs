using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altomatic.UI.Utilities;
using Altomatic.UI.ViewModels;
using EliteMMO.API;

namespace Altomatic.UI.Game.Strategies
{
	public class RefreshPlayerInfoStrategy : IGameStrategy
	{
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

			return Task.FromResult(true);
		}

		/// <summary>
		/// Update player vitals
		/// </summary>
		private void UpdatePlayerInfo(AppViewModel app, EliteAPI.PartyMember member)
		{
			var player = app.Players[member.MemberNumber];
			var entity1 = app.Healer.Entity.GetLocalPlayer();
			var entity2 = app.Healer.Entity.GetEntity((int)member.TargetIndex);

			player.Name = member.Name;
			player.CurrentHp = member.CurrentHP;
			player.CurrentHpp = member.CurrentHPP;
			player.Distance = PlayerUtilities.GetDistance(entity1, entity2);
		}
	}
}
