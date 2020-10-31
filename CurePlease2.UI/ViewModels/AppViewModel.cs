using EliteMMO.API;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CurePlease2.UI.ViewModels
{
	public class AppViewModel : INotifyPropertyChanged
	{
		private EliteAPI healer;
		public EliteAPI Healer
		{
			get { return healer; }
			set
			{
				healer = value;
				OnPropertyChanged();
			}
		}

		private EliteAPI monitored;
		public EliteAPI Monitored
		{
			get { return monitored; }
			set
			{
				monitored = value;
				OnPropertyChanged();
			}
		}

		private List<PlayerViewModel> players = new List<PlayerViewModel>();
		public List<PlayerViewModel> Players
		{
			get { return players; }
			set
			{
				players = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public AppViewModel()
		{
			for (var i = 0; i < 18; i++)
			{
				Players.Add(new PlayerViewModel(this));
			}
		}
	}
}
