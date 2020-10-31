using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CurePlease2.UI.Game;
using EliteMMO.API;

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

		private bool isPaused = true;
		public bool IsPaused
    {
      get { return isPaused; }
			set
      {
				isPaused = value;
				OnPropertyChanged();
      }
    }

		public List<IGameStrategy> Strategies { get; set; } = new List<IGameStrategy>();

		public AppViewModel()
		{
			for (var i = 0; i < 18; i++)
			{
				Players.Add(new PlayerViewModel(this));
			}
		}

		public async Task ExecuteAsync()
		{
			await Task.Yield();
			foreach (var strategy in Strategies)
			{
				strategy.Execute(this);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}
}
