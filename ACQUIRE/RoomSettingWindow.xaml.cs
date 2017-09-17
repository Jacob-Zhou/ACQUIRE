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
using System.Windows.Shapes;

namespace ACQUIRE
{
	/// <summary>
	/// RoomSettingWindow.xaml 的交互逻辑
	/// </summary>
	public partial class RoomSettingWindow : Window
	{
		private Dictionary<string, string> setting = new Dictionary<string, string>();

		public RoomSettingWindow()
		{
			InitializeComponent();
		}

		public Dictionary<string, string> getSetting()
		{
			setting.Clear();
			ShowDialog();
			return setting;
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			setting["name"] = name_textbox.Text;
			setting["needPassword"] = checkBox.IsChecked.Value.ToString();
			setting["password"] = passwordBox.Password;
			setting["maxPlayerCount"] = slider.Value.ToString();
			Close();
		}
	}
}
