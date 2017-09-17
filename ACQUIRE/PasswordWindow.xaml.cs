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
	/// PasswordWindow.xaml 的交互逻辑
	/// </summary>
	public partial class PasswordWindow : Window
	{
		private string password;

		public PasswordWindow()
		{
			InitializeComponent();
		}

		public string getPassword()
		{
			passwordBox.Password = null;
			password = null;
			ShowDialog();
			return password;
		}

		private void button_Click(object sender, RoutedEventArgs e)
		{
			password = passwordBox.Password;
			Hide();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
		}
	}
}
