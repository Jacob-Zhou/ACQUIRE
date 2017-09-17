using ACQUIRE.model;
using ACQUIRE.presenter;
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

namespace ACQUIRE
{
	/// <summary>
	/// MainWindow.xaml 的交互逻辑
	/// </summary>
	/// 

	public enum GameState
	{
		PAUSE,
		PUTTILE
	}

	public enum alphabate
	{
		A,
		B,
		C,
		D,
		E,
		F,
		G,
		H,
		I
	}

	public partial class MainWindow : Window
	{
		private ClientPresenter clientPresenter;

		private Dictionary<string, Button> tileButton;
		private Dictionary<string, bool> userTile;
		private Button[] userTileButton;
		private bool isSelect = false;
		private GameState gameState = GameState.PAUSE;
		private string HightlightUid;

		internal ClientPresenter ClientPresenter
		{
			get
			{
				return clientPresenter;
			}

			set
			{
				clientPresenter = value;
			}
		}

		public MainWindow()
		{
			InitializeComponent();
			ResizeMode = ResizeMode.NoResize;
			
			userTileButton = new Button[6]
			{
				user_tile_0,
				user_tile_1,
				user_tile_2,
				user_tile_3,
				user_tile_4,
				user_tile_5,
			};
			
			foreach (var b in userTileButton)
			{
				b.Click += Tile_Click;
				b.MouseEnter += UserTile_MouseEnter;
				b.MouseLeave += UserTile_MouseLeave;
			}

			tileButton = new Dictionary<string, Button>();
			for(int i = 0; i < 12; i++)
			{
				for(int j = 0; j < 9; j++)
				{
					Button btn = new Button();
					btn.Uid = i.ToString() + j.ToString();
					btn.Width = 52;
					btn.Height = 52;
					btn.Content = (i + 1).ToString() + ((alphabate)j).ToString();
					btn.SetValue(Grid.ColumnProperty, i);
					btn.SetValue(Grid.RowProperty, j);
					//btn.Click += Tile_Click;
					btn.MouseEnter += Tile_MouseEnter;
					btn.BorderThickness = new Thickness(1);
					btn.BorderBrush = Brushes.DimGray;
					btn.Background = Brushes.LightGray;
					board.Children.Add(btn);
					tileButton[btn.Uid] = btn;
				}
			}

			userTile = new Dictionary<string, bool>();
			
		}

		private void UserTile_MouseLeave(object sender, MouseEventArgs e)
		{
			UnhighlightTile(HightlightUid);
		}

		private void UserTile_MouseEnter(object sender, MouseEventArgs e)
		{
			HightlightUid = ((Button)sender).Uid;
			highlightTile(HightlightUid, Brushes.Red);
		}
		

		public string UidToContent(string Uid)
		{
			int id = int.Parse(Uid);
			return (id / 10 + 1).ToString() + ((alphabate)(id % 10)).ToString();
		}

		public void changeGameState(GameState gs)
		{
			gameState = gs;
		}

		public void updateUserTile(Dictionary<string, bool> Uids)
		{
			userTile.Clear();
			userTile = Uids;
			drawUserTile();
		}

		public void drawUserTile()
		{
			foreach(Button b in userTileButton)
			{
				b.IsEnabled = false;
				b.Content = "";
			}
			int i = 0;
			foreach(var s in userTile)
			{
				if(s.Value)
				{
					userTileButton[i].Content = UidToContent(s.Key);
					userTileButton[i].Uid = s.Key;
					userTileButton[i].IsEnabled = true;
				}
				i++;
			}
		}

		internal void gameStart()
		{
			state.Content = "游戏开始";
		}

		public void RoundStart()
		{
			state.Content = "你的回合";
			gameState = GameState.PUTTILE;
		}

		public void showInformation(string info1, string info)
		{
			label.Content = info1;
			money.Content = info;
		}

		public void RoundOver()
		{
			gameState = GameState.PAUSE;
			//setCompanyUnclickable();
			state.Content = "对手回合";
		}

		public void highlightTile(string Uid, SolidColorBrush border)
		{

			tileButton[Uid].BorderThickness = new Thickness(3);
			tileButton[Uid].BorderBrush = border;
		}

		public void UnhighlightTile(string Uid)
		{
			tileButton[Uid].BorderThickness = new Thickness(1);
			tileButton[Uid].BorderBrush = Brushes.DimGray;
		}

		public void drawTile(string Uid, int border, CompanyType company)
		{
			drawTile(Uid, border, CompanyColor.Color[company]);
		}

		public void drawTile(string Uid, int border, SolidColorBrush background)
		{
			int left = 1;
			int top = 1;
			int right = 1;
			int bottom = 1;

			if((border & 8) == 0)
			{
				left = 3;
			}
			else
			{
				left = 0;
			}

			if ((border & 1) == 0)
			{
				top = 3;
			}
			else
			{
				top = 0;
			}

			if ((border & 2) == 0)
			{
				right = 3;
			}
			else
			{
				right = 0;
			}

			if ((border & 4) == 0)
			{
				bottom = 3;
			}
			else
			{
				bottom = 0;
			}

			if(border == -1)
			{
				left = 1;
				right = 1;
				top = 1;
				bottom = 1;
			}

			tileButton[Uid].BorderThickness = new Thickness(left, top, right, bottom);
			tileButton[Uid].BorderBrush = Brushes.DimGray;
			tileButton[Uid].Background = background;
			tileButton[Uid].Foreground = background;
		}

		public void gameOver()
		{
			label.Content = "Game Over";
		}

		private void Tile_Click(object sender, RoutedEventArgs e)
		{
			if (!isSelect && gameState == GameState.PUTTILE)
			{
				string Uid = ((Button)sender).Uid;
				clientPresenter.putTile(Uid);
				userTile.Remove(Uid);
				drawUserTile();
				gameState = GameState.PAUSE;
			}
		}

		private void Tile_MouseEnter(object sender, RoutedEventArgs e)
		{
			//label.Content = gamePresenter.getInfomation(((Button)sender).Uid);
		}
		
		private void Window_Closed(object sender, EventArgs e)
		{
			if(clientPresenter != null)
			{
				clientPresenter.Close();
			}
		}
	}
}
