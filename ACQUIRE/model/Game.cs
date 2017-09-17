using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ACQUIRE.model
{



	class Game
	{
		static private Game _singleton = new Game();
		private Board gameBoard;
		private Dictionary<CompanyType, Company> companys;
		private Queue<Vector> tiles = new Queue<Vector>();
		private int playerCount;
		private Player[] players;
		private int mainPlayer;

		static public Game getInstance()
		{
			return _singleton;
		}
		

		public int PlayerCount
		{
			get
			{
				return playerCount;
			}

			set
			{
				playerCount = value;
			}
		}

		public Player getMainPlayer()
		{
			return players[mainPlayer];
		}

		public int MainPlayer
		{
			get
			{
				return mainPlayer;
			}

			set
			{
				mainPlayer = value;
			}
		}

		internal Dictionary<CompanyType, Company> Companys
		{
			get
			{
				return companys;
			}
		}

		public bool exchangeShare(CompanyType destination, int destinationShareCount, Dictionary<CompanyType, int> source)
		{
			if (companys[destination].RemainShare >= destinationShareCount)
			{
				companys[destination].decreaseShare(destinationShareCount);
				foreach(var s in source)
				{
					companys[s.Key].increaseShare(s.Value);
				}
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool giveShare(CompanyType com)
		{
			if(companys[com].RemainShare >= 1)
			{
				companys[com].decreaseShare();
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool buyShare(CompanyType com, int count, int playerRemainMoney)
		{
			if (count > 3)
			{
				return false;
			}
			else
			{
				if(companys[com].RemainShare >= count && playerRemainMoney >= (companys[com].getPrice() * count))
				{
					companys[com].decreaseShare(count);
					return true;
				}
				else
				{
					return false;
				}
			}
		}

		public void saleShare(CompanyType com, int count)
		{
			companys[com].increaseShare(count);
		}

		public bool isOver()
		{
			int count = 0;
			foreach (var c in Companys)
			{
				if (c.Value.getState() == CompanyState.SAFE)
				{
					count++;
				}
				else if(c.Value.getState() == CompanyState.FINNISH)
				{
					return true;
				}
			}
			if(count >= 7)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		internal Board GameBoard
		{
			get
			{
				return gameBoard;
			}

			set
			{
				gameBoard = value;
			}
		}

		public Game(int pCount = 8)
		{
			reset(pCount);
		}

		public HashSet<CompanyType> getUnEstablishCompany()
		{
			HashSet<CompanyType> com = new HashSet<CompanyType>();
			foreach(var c in Companys)
			{
				if(c.Value.getState() == CompanyState.UNESTABLISH)
				{
					com.Add(c.Value.Type);
				}
			}
			return com;
		}

		public int getCompanyCount()
		{
			int count = 0;
			foreach (var c in Companys)
			{
				if (c.Value.getState() != CompanyState.UNESTABLISH)
				{
					count++;
				}
			}
			return count;
		}

		public Vector getNewTile()
		{
			return tiles.Dequeue();
		}

		public HashSet<CompanyType> getBuyableCompany()
		{
			HashSet<CompanyType> com = new HashSet<CompanyType>();
			foreach (var c in Companys)
			{
				if (c.Value.getState() > CompanyState.UNESTABLISH && c.Value.RemainShare > 0)
				{
					com.Add(c.Value.Type);
				}
			}
			return com;
		}

		public void reset(int pCount = 8)
		{
			mainPlayer = 0;
			gameBoard = new Board();
			companys = new Dictionary<CompanyType, Company>();
			playerCount = pCount;
			for (int i = 0; i < 7; i++)
			{
				Companys[(CompanyType)i] = new Company((CompanyType)i);
			}
			players = new Player[8];
			for (int i = 0; i < 8; i++)
			{
				players[i] = new Player(i < pCount);
			}
			Vector[] tempTiles = new Vector[108];
			int index = 0;
			for (int i = 0; i < 12; i++)
			{
				for(int j = 0; j < 9; j++)
				{
					tempTiles[index] = new Vector(i, j);
					index++;
				}
			}
			Random ran = new Random();
			int k;
			Vector tempVector;
			for(int i = 0; i < 108; i++)
			{
				k = ran.Next(0, 108);
				tempVector = tempTiles[i];
				tempTiles[i] = tempTiles[k];
				tempTiles[k] = tempVector;
			}

			foreach (var v in tempTiles)
			{
				tiles.Enqueue(v);
			}

		}
	}
}
