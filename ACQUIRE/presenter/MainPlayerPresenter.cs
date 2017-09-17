using ACQUIRE.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ACQUIRE.presenter
{
	class MainPlayerPresenter
	{
		private static MainPlayerPresenter _singleton = new MainPlayerPresenter();
		private MainWindow mainWindow;
		private SaleAndBuy saleWindow;
		private Exchange exchangeWindow;
		private Player mainPlayer;

		private MainPlayerPresenter()
		{
			saleWindow = new SaleAndBuy();
			exchangeWindow = new Exchange();
			mainPlayer = Game.getInstance().getMainPlayer();
		}

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

		public static MainPlayerPresenter getInstance()
		{
			return _singleton;
		}

		public int getPlayerMoney()
		{
			return mainPlayer.Money;
		}

		public string getShareInfomations()
		{
			string str = "";
			foreach(var s in mainPlayer.Share)
			{
				str += s.Key.ToString() + ": " + s.Value.ToString() + "\n	Value: " + (s.Value * Game.getInstance().Companys[s.Key].getPrice()) + "\n";
			}
			return str;
		}

		public int getProperty()
		{
			return mainPlayer.getProperty();
		}

		public void giveShare(CompanyType com)
		{
			if(Game.getInstance().giveShare(com))
			{
				mainPlayer.giveShare(com);
			}
		}

		public bool buyShare(CompanyType com, int count)
		{
			if(Game.getInstance().buyShare(com, count, mainPlayer.Money))
			{
				mainPlayer.buyShare(com, count, Game.getInstance().Companys[com].getPrice());
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool saleShare(CompanyType com, int count)
		{
			return saleShare(com, count, Game.getInstance().Companys[com].getPrice());
		}

		public bool saleShare(CompanyType com, int count, int price)
		{
			if (mainPlayer.saleShare(com, count, price))
			{
				Game.getInstance().saleShare(com, count);
				return true;
			}
			return false;
		}

		public bool putTile(Vector tile)
		{
			if (GamePresenter.getInstance().putTile(tile))
			{
				mainPlayer.Tiles.Remove(tile);
				return true;
			}
			return false;
		}

		private Dictionary<CompanyType, int> getAvailable(Dictionary<CompanyType, int> companyPrices)
		{
			Dictionary<CompanyType, int> available = new Dictionary<CompanyType, int>();
			foreach (var r in companyPrices)
			{
				available[r.Key] = mainPlayer.Share[r.Key];
			}
			return available;
		}

		public void handleExchange(CompanyType biggest, int biggestRemainShare, Dictionary<CompanyType, int> companyPrices)
		{
			Dictionary<CompanyType, int> available = getAvailable(companyPrices);
			available[biggest] = mainPlayer.Share[biggest];
			var result = exchangeWindow.exchange(biggest, biggestRemainShare, available);

			Dictionary<CompanyType, int> source = new Dictionary<CompanyType, int>();
			int destinationShareCount = 0;
			foreach (var r in result)
			{
				if(r.Key == biggest)
				{
					destinationShareCount = r.Value - mainPlayer.Share[r.Key];
				}
				else
				{
					source[r.Key] = mainPlayer.Share[r.Key] - r.Value;
				}
			}
			if(Game.getInstance().exchangeShare(biggest, destinationShareCount, source))
			{
				foreach (var r in result)
				{
					mainPlayer.Share[r.Key] = r.Value;
				}
			}

		}

		public bool needHandle(Dictionary<CompanyType, int> companyPrices)
		{
			foreach(var c in companyPrices)
			{
				if (mainPlayer.Share[c.Key] > 0)
					return true;
			}
			return false;
		}

		public void handleSale(Dictionary<CompanyType, int> companyPrices)
		{
			Dictionary<CompanyType, int> available = getAvailable(companyPrices);
			var result = saleWindow.Sale(available);

			foreach(var r in result)
			{
				if(r.Value > 0)
				{
					saleShare(r.Key, r.Value, companyPrices[r.Key]);
				}
			}
		}

		public void initTiles()
		{
			for (int i = 0; i < 6; i++)
			{
				mainPlayer.Tiles.Add(Game.getInstance().getNewTile());
			}
			HashSet<string> Uids = new HashSet<string>();
			foreach (var t in mainPlayer.Tiles)
			{
				Uids.Add(((int)t.X).ToString() + ((int)t.Y).ToString());
			}
			//mainWindow.updateUserTile(Uids);
		}

		public void getNewTile()
		{
			mainPlayer.Tiles.Add(Game.getInstance().getNewTile());
			HashSet<string> Uids = new HashSet<string>();
			foreach(var t in mainPlayer.Tiles)
			{
				Uids.Add(((int)t.X).ToString() + ((int)t.Y).ToString());
			}
			//mainWindow.updateUserTile(Uids);
		}

		public void RoundOver()
		{

		}
	}
}
