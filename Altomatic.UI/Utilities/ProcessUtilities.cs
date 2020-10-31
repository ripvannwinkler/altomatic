using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altomatic.UI.Utilities
{
	public static class ProcessUtilities
	{
		public static Process[] GetProcesses()
		{
			var pol = Process.GetProcessesByName("pol");
			var xiloader = Process.GetProcessesByName("xiloader");
			var edenxi = Process.GetProcessesByName("edenxi");
			return pol.Union(xiloader).Union(edenxi).ToArray();
		}
	}
}
