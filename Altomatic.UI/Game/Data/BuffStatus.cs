using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altomatic.UI.Game.Data
{
	public class BuffStatus
	{
		public short Id { get; set; }
		public DateTime Applied { get; set; }

    public BuffStatus(short id)
    {
      Id = id;
    }

    public int AgeInSeconds
		{
			get { return (int)Math.Ceiling(DateTime.Now.Subtract(Applied).TotalSeconds); }
		}
	}
}
