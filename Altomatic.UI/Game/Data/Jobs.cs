using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EliteMMO.API.EliteAPI;

namespace Altomatic.Game.Data
{
	public class Jobs : Dictionary<ushort, string>
	{
		public Jobs()
		{
			Add(1, "WAR");
			Add(2, "MNK");
			Add(3, "WHM");
			Add(4, "BLM");
			Add(5, "RDM");
			Add(6, "THF");
			Add(7, "PLD");
			Add(8, "DRK");
			Add(9, "BST");
			Add(10, "BRD");
			Add(11, "RNG");
			Add(12, "SAM");
			Add(13, "NIN");
			Add(14, "DRG");
			Add(15, "SMN");
			Add(16, "BLU");
			Add(17, "COR");
			Add(18, "PUP");
			Add(19, "DNC");
			Add(20, "SCH");
			Add(21, "GEO");
			Add(22, "RUN");
		}
	}
}
