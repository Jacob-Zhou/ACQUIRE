using ACQUIRE.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ACQUIRE.presenter
{
	class GamePresenter
	{
		private static GamePresenter _singleton = new GamePresenter();
		private BoardPresenter bPresenter;
		private MainPlayerPresenter mainPlayerPresenter;
		private Game game;
		private int roundCount;
		private int[] roundIndex = new int[0];
		private MainWindow mainWindow;
		private SelectCompany selectWindow;
		private SaleAndBuy buyWindow;
		private HandleShare handleShareWindow;
		private Vector nowTile;

		public GamePresenter(int pCount = 8)
		{
			bPresenter = BoardPresenter.getInstance();
			mainPlayerPresenter = MainPlayerPresenter.getInstance();
			selectWindow = new SelectCompany();
			handleShareWindow = new HandleShare();
			buyWindow = new SaleAndBuy();
			game = Game.getInstance();
			roundIndex = new int[pCount];
			for(int i = 0; i < pCount; i++)
			{
				roundIndex[i] = i;
			}
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

		static public GamePresenter getInstance()
		{
			return _singleton;
		}

		public string getInfomation(string Uid)
		{
			return getInfomation(UidToVector(Uid));
		}

		public string getInfomation(Vector tilePosition)
		{
			Tile tile = game.GameBoard.getTile(tilePosition);
			if(tile.Company == CompanyType.NULL || tile.Company == CompanyType.SINGLE)
			{
				return "";
			}
			else
			{
				return tile.Company.ToString() + "\n" + game.Companys[tile.Company].TileCount.ToString() + "\n" + game.Companys[tile.Company].getPrice().ToString();
			}
	
		}

		public Vector UidToVector(string Uid)
		{
			int id = int.Parse(Uid);
			return new Vector((int)(id / 10), (int)(id % 10));
		}

		public bool tryPutTile(string Uid)
		{
			return tryPutTile(UidToVector(Uid));
		}

		public bool tryPutTile(Vector tilePosition)
		{
			return game.GameBoard.tryPutTile(tilePosition);
		}

		public bool putTile(string Uid)
		{
			return putTile(UidToVector(Uid));
		}

		public bool putTile(Vector tile)
		{
			nowTile = tile;
			switch(game.GameBoard.putTile(tile))
			{
				case tileResult.INDEPENDENT:
					break;
				case tileResult.CREATE:
					handleCreate(selectWindow.Select(game.getUnEstablishCompany()));
					//mainWindow.setCompanyClickable(game.getUnEstablishCompany());
					break;
				case tileResult.ATTACH:
					game.GameBoard.attch(tile);
					break;
				case tileResult.MERGER:
				case tileResult.TRIPLE:
				case tileResult.FOUR:
					switch (game.GameBoard.merger(tile))
					{
						case mergerResult.CANNOTMERGER:
							return false;
						case mergerResult.SELECT:
							//mainWindow.setCompanyClickable(game.GameBoard.getBiggestCompany(tile));
							game.GameBoard.changeCompany(nowTile, selectWindow.Select(game.GameBoard.getBiggestCompany(tile)));
							game.GameBoard.merger(nowTile);
							//handleShare
							break;
						case mergerResult.SUCCESS:
							break;
					}
					CompanyType biggestCompany = game.GameBoard.getBiggestCompany(tile).FirstOrDefault();
					//handleShareWindow.Handle(biggestCompany, game.Companys[biggestCompany].RemainShare, game.GameBoard.SmallCompanyPrices);
					break;
				case tileResult.ERROR:
					return false;
				default:
					break;
			}
			putTileOver();
			return true;
		}

		public void Start()
		{
			Start(8);
		}

		public void Start(int playerCount)
		{
			Game.getInstance().reset(playerCount);
			mixIndex();
			roundCount = 0;
			MainPlayerPresenter.getInstance().initTiles();
			mainWindow.RoundStart();
		}

		public void RoundStart()
		{
			roundCount++;
			mainWindow.RoundStart();
		}

		public bool isMainPlayer()
		{
			if(roundIndex[roundCount] == Game.getInstance().MainPlayer)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private void handleCreate(CompanyType com)
		{
			game.GameBoard.changeCompany(nowTile, com);
			mainPlayerPresenter.giveShare(com);
		}

		public void tileCallback(CompanyType com)
		{

		}

		public void mixIndex()
		{
			Random ram = new Random();
			int k = 0;
			int value = 0;
			int max = roundIndex.Length;
			for (int i = 0; i < max; i++)
			{
				k = ram.Next(0, max);
				if(k != i)
				{
					value = roundIndex[i];
					roundIndex[i] = roundIndex[k];
					roundIndex[k] = value;
				}
			}
		}

		public void GameOn()
		{

		}

		private void putTileOver()
		{
			Dictionary<CompanyType, int> companys = new Dictionary<CompanyType, int>();
			foreach(var c in game.getBuyableCompany())
			{
				companys[c] = game.Companys[c].RemainShare;
			}
			//var result = buyWindow.Buy(companys);

			//foreach(var c in result)
			//{
			//	mainPlayerPresenter.buyShare(c.Key, c.Value);
			//}

			bPresenter.drawTiles();
			//mainWindow.showMoney();
			//mainWindow.putTileOver(game.getBuyableCompany());
		}

		public void nextRound()
		{
			mixIndex();
			roundCount = 0;
			if(game.isOver())
			{
				Over();
			}
			else
			{
				mainWindow.RoundStart();
			}
		}

		public string getCompanysInfomation()
		{
			string info = "";
			foreach(var c in game.Companys)
			{
				info += c.Value.Type.ToString();
				info += ": ";
				info += c.Value.TileCount;
				info += "\n";
			}
			return info;
		}

		public void RoundOver()
		{
			nextRound();
		}

		public void Over()
		{
			mainWindow.gameOver();
		}
	}
}
