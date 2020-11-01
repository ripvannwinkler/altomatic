using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altomatic.UI.Game
{
	public class AddonInterface
	{
		public IPEndPoint Endpoint { get; }
		public UdpClient Listener { get; }
		public Subject<AddonEvent> Events { get; } = new Subject<AddonEvent>();

		public AddonInterface()
		{
			Endpoint = GetRandomEndpoint();
			Listener = new UdpClient(Endpoint);
			StartListening();
		}

		private IPEndPoint GetRandomEndpoint()
		{
			var tcp = new TcpListener(IPAddress.Loopback, 0);
			tcp.Start(); // this triggers the port assginment

			var endpoint = tcp.LocalEndpoint as IPEndPoint;
			tcp.Stop(); // free up the port

			return endpoint ?? new IPEndPoint(IPAddress.Loopback, 9901);
		}

		private void StartListening()
		{
			var listener = new Thread(ListenForData);
			listener.IsBackground = true;
			listener.Start();
		}

		private void ListenForData()
		{
			while (true)
			{
				try
				{
					var ep = Endpoint;
					var bytes = Listener.Receive(ref ep);
					var data = Encoding.UTF8.GetString(bytes);

					var prefix = "alto:";
					if (data.StartsWith(prefix))
					{
						ParseAddonData(data.Remove(0, prefix.Length));
						continue;
					}

					throw new Exception($"Unrecognized data string: {data}");
				}
				catch (Exception ex)
				{
					Debug.WriteLine("Error parsing addon data.");
					Debug.WriteLine(ex.ToString());
				}
			}
		}

		private void ParseAddonData(string data)
		{
			if (data == "addon_loaded")
			{
				Events.OnNext(new AddonEvent(AddonEventType.Loaded, data));
			}
			else if (data == "casting_started")
			{
				Events.OnNext(new AddonEvent(AddonEventType.CastingStarted, data));
			}
			else if (data == "casting_completed")
			{
				Events.OnNext(new AddonEvent(AddonEventType.CastingCompleted, data));
			}
			else if (data == "casting_interrupted")
			{
				Events.OnNext(new AddonEvent(AddonEventType.CastingInteruppted, data));
			}
			else if (data.StartsWith("buffs_"))
			{
				Events.OnNext(new AddonEvent(AddonEventType.BuffsUpdated, data));
			}
			else
			{
				Debug.WriteLine("Unexpected data from addon:");
				Debug.WriteLine(data);
			}
		}
	}

	public enum AddonEventType
	{
		None,
		Loaded,
		CastingStarted,
		CastingCompleted,
		CastingInteruppted,
		BuffsUpdated,
	}

	public class AddonEvent
	{
		public string Data { get; set; }
		public DateTimeOffset Timestamp { get; set; }
		public AddonEventType Type { get; set; }

		public AddonEvent(AddonEventType type, string data)
		{
			Data = data ?? throw new ArgumentNullException(nameof(data));
			Timestamp = DateTimeOffset.UtcNow;
			Type = type;
		}
	}
}
