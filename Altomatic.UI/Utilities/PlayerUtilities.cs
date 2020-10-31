using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EliteMMO.API.EliteAPI;

namespace Altomatic.UI.Utilities
{
	public static class PlayerUtilities
	{
		public static double GetDistance(XiEntity entity1, XiEntity entity2)
		{
			return Math.Sqrt(
				Math.Pow(entity1.X - entity2.X, 2) +
				Math.Pow(entity1.Y - entity2.Y, 2) +
				Math.Pow(entity1.Z - entity2.Z, 2));
		}
	}
}
