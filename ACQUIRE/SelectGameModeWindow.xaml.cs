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
using System.Windows.Shapes;

namespace ACQUIRE
{
	/// <summary>
	/// SelectGameModeWindow.xaml 的交互逻辑
	/// </summary>
	public partial class SelectGameModeWindow : Window
	{
		private ClientPresenter clientPresenter = ClientPresenter.getInstance();
		private bool isInit = false;

		public SelectGameModeWindow()
		{
			InitializeComponent();
			single_btn.IsEnabled = false;
		}

		private void new_room_btn_Click(object sender, RoutedEventArgs e)
		{
			clientPresenter.Init(true);
			isInit = true;
			Close();
		}

		private void search_room_btn_Click(object sender, RoutedEventArgs e)
		{
			clientPresenter.Init(false);
			isInit = true;
			Close();
		}

		private void single_btn_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if(!isInit)
			{
				clientPresenter.Close();
			}
		}
	}
}
