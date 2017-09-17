using ACQUIRE.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace ACQUIRE.presenter
{
	public class ServerResponse
	{
		public int port { get; set; }
		public string password { get; set; }
	}

	public enum CompanyType
	{
		WORLDWIDE,
		SACKSON,
		FESTIVAL,
		IMPERIAL,
		AMERICAN,
		CONTINENTAL,
		TOWER,
		NULL,
		SINGLE
	}

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

	public class RoomInfomation
	{
		public RoomInfomation() { }

		public RoomInfomation(string uid, int port, string name, bool needPassword, string password, int maxPlayerCount, int playerCount)
		{
			Uid = uid;
			this.port = port;
			this.name = name;
			this.needPassword = needPassword;
			this.password = password;
			this.maxPlayerCount = maxPlayerCount;
			this.playerCount = playerCount;
		}

		public string Uid { get; set; }
		public Int32 port { get; set; }
		public string name { get; set; }
		public bool needPassword { get; set; }
		public string password { get; set; }
		public int maxPlayerCount { get; set; }
		public int playerCount { get; set; }
	}

	public class TileData
	{
		public TileData(string uid)
		{
			Uid = uid;
		}

		public TileData() { }

		public string Uid { get; set; }
		public int border { get; set; }
		public CompanyType company { get; set; }
	}

	public class HandleResult
	{
		public Dictionary<CompanyType, int> exchange { get; set; }
		public Dictionary<CompanyType, int> sale { get; set; }
	}

	public class PlayerData
	{
		public PlayerData(int index, int money, Dictionary<CompanyType, int> share)
		{
			this.index = index;
			this.money = money;
			this.share = share;
		}

		public PlayerData() { }

		public int index { get; set; }
		public int money { get; set; }
		public Dictionary<CompanyType, int> share { get; set; }
	}

	public class BuyData
	{
		public Dictionary<CompanyType, int> companyPrice { get; set; }
		public Dictionary<CompanyType, int> companyAvailable { get; set; }
		public int playerMoney { get; set; }
	}

	public class HandleData
	{
		public CompanyType bigCompany { get; set; }
		public int bigCompanyRemain { get; set; }
		public Dictionary<CompanyType, int> smallcomPrice { get; set; }
		public Dictionary<CompanyType, int> smallcomAvailable { get; set; }
	}

	public class UpdateData
	{
		public HashSet<TileData> tiles { get; set; }
		public HashSet<PlayerData> players { get; set; }
	}


	class ClientPresenter
	{
		private static ClientPresenter _singleton = new ClientPresenter();
		private MainWindow mainWindow = new MainWindow();
		private GameClient client = GameClient.getInstance();
		private string hostname = "http://182.254.158.39:56000/aquire?require=";

		private ClientPresenter() { }

		public static ClientPresenter getInstance()
		{
			return _singleton;
		}

		public void HandleError()
		{

		}

		public void Close()
		{
			client.close();
			mainWindow.Close();
		}

		public async void Init(bool isCreateRoom)
		{
			client.setCallBack((byte)DataType.GAMESTART, gameStartCallBack);
			client.setCallBack((byte)DataType.GAMEOVER, gameOverCallBack);
			client.setCallBack((byte)DataType.START, playerStartCallBack);
			client.setCallBack((byte)DataType.OVER, playerOverCallBack);
			client.setCallBack((byte)DataType.SELECT, selectCallBack);
			client.setCallBack((byte)DataType.HANDLE, HandleShareCallBack);
			client.setCallBack((byte)DataType.BUY, BuyCallBack);
			client.setCallBack((byte)DataType.UPDATE, UpdateInformationCallBack);
			client.setCallBack((byte)DataType.UPDATETILES, UpdateTilesCallBack);

			mainWindow.ClientPresenter = this;
			HttpClient httpClient = new HttpClient();
			if (isCreateRoom)
			{
				await mainWindow.Dispatcher.Invoke(async () =>
				  {
					  var content = new StringContent(JsonConvert.SerializeObject(new RoomSettingWindow().getSetting()));
					  var respond = await httpClient.PostAsync(hostname + "create", content);
					  if (respond.IsSuccessStatusCode)
					  {
						  var respondString = await respond.Content.ReadAsStringAsync();
						  ServerResponse res = JsonConvert.DeserializeObject<ServerResponse>(respondString);
						  client.Initialize(res.port);
					  }
					  else
					  {
						  HandleError();
					  }
				  });

			}
			else
			{
				var respondString = await httpClient.GetStringAsync(hostname + "search");
				RoomInfomation[] res = JsonConvert.DeserializeObject<RoomInfomation[]>(respondString);
				int port = new SelectServerWindow().SelectRoom(res);
				if (port > -1)
				{
					var content = new StringContent(port.ToString());
					var selectMessage = await httpClient.PostAsync(hostname + "select", content);
					var selectRes = bool.Parse(await selectMessage.Content.ReadAsStringAsync());
					if(selectRes)
					{
						client.Initialize(port);
					}
					else
					{
						HandleError();
					}
				}
				else
				{
					throw new NotSupportedException();
				}
			}
		}

		public void putTile(string Uid)
		{
			var data = JsonConvert.SerializeObject(new TileData(Uid));
			client.Send(data, (byte)DataType.PUTTILE);
		}

		public void gameStartCallBack(string data) //0
		{//TODO
			mainWindow.Dispatcher.Invoke(() => {
				mainWindow.Show();
				mainWindow.gameStart();
			});
		}

		public void gameOverCallBack(string data) //1
		{
			mainWindow.Dispatcher.Invoke(() => {
				//mainWindow.gameOver();
			});
		}

		public void playerStartCallBack(string data) //2
		{
			//TO PUT TILE
			mainWindow.Dispatcher.Invoke(() => {
				mainWindow.RoundStart();
			});
		}

		public void playerOverCallBack(string data) //3
		{
			mainWindow.Dispatcher.Invoke(() => {
				mainWindow.RoundOver();
			});
		}

		public void selectCallBack(string data) //5
		{
			var companys = JsonConvert.DeserializeObject<HashSet<CompanyType>>(data);
			mainWindow.Dispatcher.Invoke(() => {
				var result = new SelectCompany().Select(companys);
				client.Send(((int)result).ToString(), (byte)DataType.SELECTRESULT);
			});
		}

		public void HandleShareCallBack(string data) //7
		{
			var handleData = JsonConvert.DeserializeObject<HandleData>(data);
			mainWindow.Dispatcher.Invoke(() =>
			{
				var result = new HandleShare().Handle(handleData.bigCompany, handleData.bigCompanyRemain, handleData.smallcomPrice, handleData.smallcomAvailable);
				var resultData = JsonConvert.SerializeObject(result);
				client.Send(resultData, (byte)DataType.HANDLERESULT);
			});
		}

		public void BuyCallBack(string data) //9
		{
			var buyData = JsonConvert.DeserializeObject<BuyData>(data);
			mainWindow.Dispatcher.Invoke(() =>
			{
				var result = new SaleAndBuy().Buy(buyData.companyAvailable, buyData.companyPrice, buyData.playerMoney);
				var resultData = JsonConvert.SerializeObject(result);
				client.Send(resultData, (byte)DataType.BUYRESULT);
			});
		}

		public void UpdateInformationCallBack(string data) //11
		{
			mainWindow.Dispatcher.Invoke(() => {
				var updateData = JsonConvert.DeserializeObject<UpdateData>(data);
				foreach (var t in updateData.tiles)
				{
					mainWindow.drawTile(t.Uid, t.border, t.company);
				}
				string info1 = string.Empty;
				string info = string.Empty;
				foreach (var p in updateData.players)
				{
					info1 += p.index.ToString() + ": " + p.money.ToString() + "\n";
					if(p.index == client.PlayerId)
					{
						foreach (var c in p.share)
						{
							info += c.Key.ToString() + ": " + c.Value.ToString() + "\n";
						}
					}
				}
				mainWindow.showInformation(info1, info);
			});
		}

		public void UpdateTilesCallBack(string data) //12
		{
			try
			{
				var tileUids = JsonConvert.DeserializeObject<Dictionary<string, bool>>(data);
				mainWindow.Dispatcher.Invoke(() =>
				{
					mainWindow.updateUserTile(tileUids);
				});
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
