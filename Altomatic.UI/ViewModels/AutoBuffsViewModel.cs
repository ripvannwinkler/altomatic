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
		private bool flurry;
		private bool phalanx;
		private bool refresh;

		private bool firestorm;
		private bool sandstorm;
		private bool rainstorm;
		private bool windstorm;
		private bool thunderstorm;
		private bool hailstorm;
		private bool aurorastorm;
		private bool voidstorm;

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

		public bool Phalanx
		{
			get { return phalanx; }
			set
			{
				phalanx = value;
				OnPropertyChanged();
			}
		}

		public bool Refresh
		{
			get { return refresh; }
			set
			{
				refresh = value;
				OnPropertyChanged();
			}
		}

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
