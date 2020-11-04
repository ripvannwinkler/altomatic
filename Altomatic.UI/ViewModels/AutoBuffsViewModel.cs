using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Altomatic.UI.ViewModels
{
	public class AutoBuffsViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		private bool haste;
		public bool Haste
		{
			get { return haste; }
			set
			{
				haste = value;
				OnPropertyChanged();

				if (value && Flurry)
				{
					Flurry = false;
				}
			}
		}

		private bool flurry;
		public bool Flurry
		{
			get { return flurry; }
			set
			{
				flurry = value;
				OnPropertyChanged();

				if (value && Haste)
				{
					Haste = false;
				}
			}
		}

		private bool phalanx;
		public bool Phalanx
		{
			get { return phalanx; }
			set { phalanx = value; OnPropertyChanged(); }
		}

		private bool refresh;
		public bool Refresh
		{
			get { return refresh; }
			set { refresh = value; OnPropertyChanged(); }
		}

		private bool regen;
		public bool Regen
		{
			get { return regen; }
			set { regen = value; OnPropertyChanged(); }
		}

		private bool protect;
		public bool Protect
		{
			get { return protect; }
			set { protect = value; OnPropertyChanged(); }
		}

		private bool shell;
		public bool Shell
		{
			get { return shell; }
			set { shell = value; OnPropertyChanged(); }
		}

		private bool firestorm;		
		public bool Firestorm
		{
			get { return firestorm; }
			set
			{
				firestorm = value;
				OnPropertyChanged();

				if (value)
				{
					Sandstorm = false;
					Rainstorm = false;
					Windstorm = false;
					Thunderstorm = false;
					Hailstorm = false;
					Aurorastorm = false;
					Voidstorm = false;
				}
			}
		}

		private bool sandstorm;
		public bool Sandstorm
		{
			get { return sandstorm; }
			set
			{
				sandstorm = value;
				OnPropertyChanged();

				if (value)
				{
					Firestorm = false;
					Rainstorm = false;
					Windstorm = false;
					Thunderstorm = false;
					Hailstorm = false;
					Aurorastorm = false;
					Voidstorm = false;
				}
			}
		}

		private bool rainstorm;
		public bool Rainstorm
		{
			get { return rainstorm; }
			set
			{
				rainstorm = value;
				OnPropertyChanged();

				if (value)
				{
					Firestorm = false;
					Sandstorm = false;
					Windstorm = false;
					Thunderstorm = false;
					Hailstorm = false;
					Aurorastorm = false;
					Voidstorm = false;
				}
			}
		}

		private bool windstorm;
		public bool Windstorm
		{
			get { return windstorm; }
			set
			{
				windstorm = value;
				OnPropertyChanged();

				if (value)
				{
					Firestorm = false;
					Sandstorm = false;
					Rainstorm = false;
					Thunderstorm = false;
					Hailstorm = false;
					Aurorastorm = false;
					Voidstorm = false;
				}
			}
		}

		private bool thunderstorm;
		public bool Thunderstorm
		{
			get { return thunderstorm; }
			set
			{
				thunderstorm = value;
				OnPropertyChanged();

				if (value)
				{
					Firestorm = false;
					Sandstorm = false;
					Rainstorm = false;
					Windstorm = false;
					Hailstorm = false;
					Aurorastorm = false;
					Voidstorm = false;
				}
			}
		}

		private bool hailstorm;
		public bool Hailstorm
		{
			get { return hailstorm; }
			set
			{
				hailstorm = value;
				OnPropertyChanged();

				if (value)
				{
					Firestorm = false;
					Sandstorm = false;
					Rainstorm = false;
					Windstorm = false;
					Thunderstorm = false;
					Aurorastorm = false;
					Voidstorm = false;
				}
			}
		}

		private bool aurorastorm;
		public bool Aurorastorm
		{
			get { return aurorastorm; }
			set
			{
				aurorastorm = value;
				OnPropertyChanged();

				if (value)
				{
					Firestorm = false;
					Sandstorm = false;
					Rainstorm = false;
					Windstorm = false;
					Thunderstorm = false;
					Hailstorm = false;
					Voidstorm = false;
				}
			}
		}

		private bool voidstorm;
		public bool Voidstorm
		{
			get { return voidstorm; }
			set
			{
				voidstorm = value;
				OnPropertyChanged();

				if (value)
				{
					Firestorm = false;
					Sandstorm = false;
					Rainstorm = false;
					Windstorm = false;
					Thunderstorm = false;
					Hailstorm = false;
					Aurorastorm = false;
				}
			}
		}
	}
}
