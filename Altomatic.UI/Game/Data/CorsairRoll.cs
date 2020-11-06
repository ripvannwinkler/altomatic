using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altomatic.UI.Game.Data
{
	public class CorsairRoll
	{
		public short BuffId { get; set; }
		public string Name { get; set; }
		public short Lucky { get; set; }
		public short Unlucky { get; set; }

		public CorsairRoll() { }

		public CorsairRoll(string name, short buffId, short lucky = -1, short unlucky = -1)
		{
			Name = name ?? throw new ArgumentNullException(nameof(name));
			BuffId = buffId;
			Lucky = lucky;
			Unlucky = unlucky;
		}

		public override bool Equals(object obj)
		{
			if (obj is CorsairRoll other)
			{
				return Name == other.Name && BuffId == BuffId;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return (Name, BuffId).GetHashCode();
		}
	}
}
