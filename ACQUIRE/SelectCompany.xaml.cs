using ACQUIRE.model;
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
	/// SelectCompany.xaml 的交互逻辑
	/// </summary>
	public partial class SelectCompany : Window
	{
		CompanyType result;
		private Dictionary<CompanyType, Button> companyButton;

		public SelectCompany()
		{
			InitializeComponent();
			WindowStyle = WindowStyle.None;
			ResizeMode = ResizeMode.NoResize;
			WindowStartupLocation = WindowStartupLocation.CenterOwner;

			companyButton = new Dictionary<CompanyType, Button> {
			{ CompanyType.WORLDWIDE, WORLDWIDE_btn },
			{ CompanyType.SACKSON, SACKSON_btn },
			{ CompanyType.FESTIVAL, FESTIVAL_btn },
			{ CompanyType.IMPERIAL, IMPERIAL_btn },
			{ CompanyType.AMERICAN, AMERICAN_btn },
			{ CompanyType.CONTINENTAL, CONTINENTAL_btn },
			{ CompanyType.TOWER, TOWER_btn }};
		}

		public CompanyType Select(HashSet<CompanyType> options)
		{
			Dispatcher.Invoke(() => {
				foreach (var b in companyButton)
				{
					b.Value.IsEnabled = false;
				}
				foreach (var o in options)
				{
					companyButton[o].IsEnabled = true;
				}
				ShowDialog();
			});
			return result;
		}

		private void returnResult(CompanyType company)
		{
			result = company;
			Hide();
		}

		private void WORLDWIDE_btn_Click(object sender, RoutedEventArgs e)
		{
			returnResult(CompanyType.WORLDWIDE);
		}

		private void SACKSON_btn_Click(object sender, RoutedEventArgs e)
		{
			returnResult(CompanyType.SACKSON);
		}

		private void FESTIVAL_btn_Click(object sender, RoutedEventArgs e)
		{
			returnResult(CompanyType.FESTIVAL);
		}

		private void IMPERIAL_btn_Click(object sender, RoutedEventArgs e)
		{
			returnResult(CompanyType.IMPERIAL);
		}

		private void AMERICAN_btn_Click(object sender, RoutedEventArgs e)
		{
			returnResult(CompanyType.AMERICAN);
		}

		private void CONTINENTAL_btn_Click(object sender, RoutedEventArgs e)
		{
			returnResult(CompanyType.CONTINENTAL);
		}

		private void TOWER_btn_Click(object sender, RoutedEventArgs e)
		{
			returnResult(CompanyType.TOWER);
		}
	}
}
