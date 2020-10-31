using CurePlease2.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CurePlease2.UI.Controls
{
	/// <summary>
	/// Interaction logic for PlayerStatus.xaml
	/// </summary>
	public partial class PlayerStatus : UserControl
	{
		public static readonly DependencyProperty PlayerProperty = DependencyProperty.Register(nameof(Player), typeof(PlayerViewModel), typeof(PlayerStatus));

		public PlayerViewModel Player
		{
			get { return (PlayerViewModel)GetValue(PlayerProperty); }
			set { SetValue(PlayerProperty, value); }
		}

		public PlayerStatus()
		{
			InitializeComponent();
		}
  }
}
