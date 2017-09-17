using ACQUIRE.presenter;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using System.Windows.Shapes;

namespace ACQUIRE
{
	/// <summary>
	/// SelectServerWindow.xaml 的交互逻辑
	/// </summary>
	/// 

	[ValueConversion(typeof(Vector), typeof(string))]
	public partial class CountConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return ((int)((Vector)value).X).ToString() + " / " + ((int)((Vector)value).Y).ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

	[ValueConversion(typeof(bool), typeof(string))]
	public partial class PasswordBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if((bool)value)
			{
				return "密码保护";
			}
			else
			{
				return "";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

	public class RoomItem
	{

		public RoomItem()
		{
		}

		public RoomItem(int uid, string name, Vector count, bool needPassword, string password)
		{
			Uid = uid;
			Name = name;
			Count = count;
			NeedPassword = needPassword;
			Password = password;
		}

		public int Uid { get; set; }
		public string Name { get; set; }
		public Vector Count { get; set; }
		public bool NeedPassword { get; set; }
		public string Password { get; set; }
	}

	public partial class SelectServerWindow : Window
	{
		private ObservableCollection<RoomItem> rooms = new ObservableCollection<RoomItem>();
		private RoomItem selectedRoom;
		private PasswordWindow passwordWindow = new PasswordWindow();

		public SelectServerWindow()
		{
			InitializeComponent();
			dataGrid.ItemsSource = rooms;
		}

		public int SelectRoom(RoomInfomation[] roomInfos)
		{
			foreach(var r in roomInfos)
			{
				rooms.Add(new RoomItem(r.port, r.name, new Vector(r.playerCount, r.maxPlayerCount), r.needPassword, r.password));
			}
			ShowDialog();
			return selectedRoom.Uid;
		}

		private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var item = ((sender as DataGrid).SelectedItem as RoomItem);
			if(item != null)
			{
				selectedRoom = item;
			}
		}

		private void select_btn_Click(object sender, RoutedEventArgs e)
		{
			if(selectedRoom != null)
			{
				if(selectedRoom.Count.X >= selectedRoom.Count.Y)
				{
					PopupWindow.ShowPopup("人数已满");
					return;
				}
				if(selectedRoom.NeedPassword)
				{
					string password = passwordWindow.getPassword();
					if(password != null)
					{
						if(password == selectedRoom.Password)
						{
							passwordWindow.Close();
							Close();
						}
						else
						{
							PopupWindow.ShowPopup("密码错误");
						}
					}
				}
				else
				{
					passwordWindow.Close();
					Close();
				}
			}
		}
	}
}
