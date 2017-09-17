using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ACQUIRE.presenter;

namespace ACQUIRE.model
{
	public enum DataType : byte
	{
		GAMESTART = 0,
		GAMEOVER = 1,
		START = 2,
		OVER = 3,
		PUTTILE = 4,
		SELECT = 5,
		SELECTRESULT = 6,
		HANDLE = 7,
		HANDLERESULT = 8,
		BUY = 9,
		BUYRESULT = 10,
		UPDATE = 11,
		UPDATETILES = 12,
		EMPTY = 19,
		INIT = 20,
		ERROR = 40
	}

	public delegate void ResultCallBack(string message);

	class GameClient
	{
		private static GameClient _singleton = new GameClient();
		private Socket client;
		private IPAddress address = IPAddress.Parse("182.254.158.39");
		//private IPAddress address = IPAddress.Parse("127.0.0.1");
		private IPEndPoint endPoint;
		private bool isInitialized = false;
		private bool isReady = false;
		private Thread receiveThread;
		private Dictionary<byte, ResultCallBack> callbacks = new Dictionary<byte, ResultCallBack>();
		private byte playerId = 0;
		private static string empty = "{}";

		public void close()
		{
			if (isInitialized)
			{
				isReady = false;
				Send(empty, (byte)DataType.ERROR);
				client.Disconnect(false);
				client.Dispose();
				if(receiveThread != null)
				{
					receiveThread.Abort();
				}
			}
		}

		public bool IsInitialized
		{
			get
			{
				return isInitialized;
			}
		}

		private GameClient() { }

		public static GameClient getInstance()
		{
			return _singleton;
		}

		private void InitCallBack(string data)
		{
			playerId = byte.Parse(data);
		}

		public void setCallBack(byte resultType, ResultCallBack callback)
		{
			callbacks[resultType] = callback;
		}

		public void Initialize(Int32 port)
		{
			callbacks[(byte)DataType.INIT] = InitCallBack;
			endPoint = new IPEndPoint(address, port);
			client = new Socket(SocketType.Stream, ProtocolType.Tcp);
			new Thread(delegate ()
			{
				for (int timeout = 1200; timeout > 0; timeout--)
				{
					try
					{
						client.Connect(endPoint);
						break;
					}
					catch (SocketException)
					{
						Thread.Sleep(50);
					}
				}
				if (client.Connected)
				{
					Connected();
				}
				else
				{
					ClientPresenter.getInstance().HandleError();
				}
			}).Start();
			isInitialized = true;
		}

		private void Connected()
		{

			isReady = true;

			receiveThread = new Thread(delegate ()
			{
				byte[] typeBytes = new Byte[2];
				byte[] lengthBytes = new Byte[8];
				byte[] bytes = new Byte[4096];
				string tempStr;
				int tempInt;
				while (isReady)
				{
					try
					{
						client.Receive(typeBytes, 2, SocketFlags.Partial);
						client.Receive(lengthBytes, 8, SocketFlags.Partial);
						tempStr = Encoding.Unicode.GetString(lengthBytes, 0, 8);
						tempInt = int.Parse(tempStr);
						tempInt *= 2;
						client.Receive(bytes, tempInt, SocketFlags.Partial);
						Console.Write(typeBytes[0].ToString());
						if (callbacks[typeBytes[0]] != null)
						{
							string text;
							text = Encoding.Unicode.GetString(bytes, 0, tempInt);
							Console.WriteLine(text);
							callbacks[typeBytes[0]](text);
						}
						else
						{
							Console.WriteLine("null");
						}
					}
					catch (Exception)
					{
						isReady = false;
						return;
					};
				}
			});

			receiveThread.Start();
			Thread.Sleep(10);
		}

		public void Send(string text, byte textType)
		{
			var message = Encoding.Unicode.GetBytes("0" + string.Format("{0:0000}", text.Length) + text);
			message[0] = textType;
			message[1] = playerId;
			try
			{
				client.Send(message);
			}
			catch (Exception)
			{
				isReady = false;
				ClientPresenter.getInstance().HandleError();
			}
		}

		public void Send(string text)
		{
			Send(text, 19);
		}

		public string Empty
		{
			get
			{
				return empty;
			}
		}

		public byte PlayerId
		{
			get
			{
				return playerId;
			}
		}
	}
}
