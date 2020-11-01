using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace Altomatic.UI.Utilities
{
  public static class ProcessUtilities
	{
		public static void EnsureDlls()
		{
			if (!File.Exists("EliteMMO.Api.dll"))
			{
				new WebClient().DownloadFile("http://ext.elitemmonetwork.com/downloads/elitemmo_api/EliteMMO.API.dll", "EliteMMO.API.dll");
			}

			if (!File.Exists("EliteApi.dll"))
			{
				new WebClient().DownloadFile("http://ext.elitemmonetwork.com/downloads/eliteapi/EliteAPI.dll", "EliteAPI.dll");
			}
		}

		public static Process[] GetProcesses()
		{
			var pol = Process.GetProcessesByName("pol");
			var xiloader = Process.GetProcessesByName("xiloader");
			var edenxi = Process.GetProcessesByName("edenxi");
			return pol.Union(xiloader).Union(edenxi).ToArray();
		}
	}
}
