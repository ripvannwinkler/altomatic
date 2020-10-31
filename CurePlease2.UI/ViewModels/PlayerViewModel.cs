using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CurePlease2.UI.ViewModels
{
	public class PlayerViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		private static readonly Random rng = new Random();

		string name;
		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				OnPropertyChanged();
			}
		}

		int currentHp;
		public int CurrentHp
		{
			get { return currentHp; }
			set
			{
				currentHp = value;
				OnPropertyChanged();
			}
		}

		double currentHpp;
		public double CurrentHpp
		{
			get { return currentHpp; }
			set
			{
				currentHpp = value;
				OnPropertyChanged();
			}
		}

		private AppViewModel appData;
		public AppViewModel AppData
		{
			get { return appData; }
			set
			{
				appData = value;
				OnPropertyChanged();
			}
		}

		public PlayerViewModel(AppViewModel appData)
		{
			AppData = appData;
			Name = Guid.NewGuid().ToString("n");

			lock (rng)
			{
				currentHpp = (int)Math.Ceiling(rng.NextDouble() * 100);
			}
		}
	}
}
