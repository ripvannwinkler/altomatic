using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Altomatic.UI.Game.Data
{
	public class BuffStatus
	{
		public short Id { get; }
		public string Player { get; }
		public DateTime Applied { get; set; }
		private static readonly XElement buffData;

		static BuffStatus()
		{
			buffData = XElement.Load("./Buffs.xml");
		}

		/// <summary>
		/// Gets a buff's name from its ID
		/// </summary>
		public static string GetName(short buff)
		{
			var element = buffData.Elements("o")
				.FirstOrDefault(e => e.Attribute("id")?.Value == buff.ToString());

			var value = element?.Attribute("en").Value ?? "";
			var textInfo = new CultureInfo("en-US").TextInfo;
			return textInfo.ToTitleCase(value);
		}

		public BuffStatus(string player, short id)
		{
			Id = id;
			Player = player;
			Applied = DateTime.Now;
		}

		public int AgeInSeconds
		{
			get { return (int)Math.Ceiling(DateTime.Now.Subtract(Applied).TotalSeconds); }
		}

		public string Name
    {
			get => GetName(Id);
    }
	}
}
