using ACQUIRE.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ACQUIRE.presenter
{
	class CompanyColor
	{
		private static Dictionary<CompanyType, SolidColorBrush> color = new Dictionary<CompanyType, SolidColorBrush>
		{
			{CompanyType.WORLDWIDE, Brushes.Purple },
		    {CompanyType.SACKSON, Brushes.DarkOrange},
			{CompanyType.FESTIVAL, Brushes.ForestGreen},
			{CompanyType.IMPERIAL, Brushes.Gold},
			{CompanyType.AMERICAN, Brushes.Navy},
			{CompanyType.CONTINENTAL, Brushes.Red},
			{CompanyType.TOWER, Brushes.CadetBlue},
			{CompanyType.NULL, Brushes.WhiteSmoke},
			{CompanyType.SINGLE, Brushes.WhiteSmoke}
		};

		public static Dictionary<CompanyType, SolidColorBrush> Color
		{
			get
			{
				return color;
			}
		}
	}

	class BoardPresenter
	{
		private static BoardPresenter _singleton = new BoardPresenter();
		private static Dictionary<CompanyType, SolidColorBrush> companyColor = new Dictionary<CompanyType, SolidColorBrush>();
		private MainWindow mainWindow;

		public MainWindow MainWindow
		{
			get
			{
				return mainWindow;
			}

			set
			{
				mainWindow = value;
			}
		}

		public Dictionary<CompanyType, SolidColorBrush> CompanyColor
		{
			get
			{
				return companyColor;
			}
		}

		private BoardPresenter()
		{
			companyColor[CompanyType.WORLDWIDE] = Brushes.Purple;
			companyColor[CompanyType.SACKSON] = Brushes.DarkOrange;
			companyColor[CompanyType.FESTIVAL] = Brushes.ForestGreen;
			companyColor[CompanyType.IMPERIAL] = Brushes.Gold;
			companyColor[CompanyType.AMERICAN] = Brushes.Navy;
			companyColor[CompanyType.CONTINENTAL] = Brushes.Red;
			companyColor[CompanyType.TOWER] = Brushes.CadetBlue;
			companyColor[CompanyType.NULL] = Brushes.WhiteSmoke;
			companyColor[CompanyType.SINGLE] = Brushes.WhiteSmoke;
		}

		public static BoardPresenter getInstance()
		{
			return _singleton;
		}

		public void drawTiles()
		{
			Vector position = new Vector();
			int border;
			CompanyType com;
			string Uid;
			for (int i = 0; i < 12; i++)
			{
				for(int j = 0; j < 9; j++)
				{
					position.X = i;
					position.Y = j;
					border = Game.getInstance().GameBoard.getTileNeighbour(position);
					com = Game.getInstance().GameBoard.getCompany(position);
					Uid = ((int)position.X).ToString() + ((int)position.Y).ToString();
					if(border != -1)
					{
						mainWindow.drawTile(Uid, border, companyColor[com]);
					}
				}
			}
		}

		public void updateBoard()
		{

		}
	}
}
