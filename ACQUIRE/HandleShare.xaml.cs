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
	/// HandleShare.xaml 的交互逻辑
	/// </summary>
	/// 
	
	public partial class HandleShare : Window
	{
		private Exchange exchangeWindow = new Exchange();
		private SaleAndBuy saleWindow = new SaleAndBuy();
		private Dictionary<CompanyType, int> exchangeResult = new Dictionary<CompanyType, int>();
		private Dictionary<CompanyType, int> saleResult = new Dictionary<CompanyType, int>();
		private CompanyType biggestCompany;
		private int biggestCompanyRemain;
		private Dictionary<CompanyType, int> smallCompanyPrices;
		private Dictionary<CompanyType, int> smallCompanyAvailable;

		public HandleShare()
		{
			InitializeComponent();
			WindowStyle = WindowStyle.None;
			ResizeMode = ResizeMode.NoResize;
			WindowStartupLocation = WindowStartupLocation.CenterOwner;
		}

		public HandleResult Handle(CompanyType biggestCompany, int biggestCompanyRemain, Dictionary<CompanyType, int> smallCompanyPrices, Dictionary<CompanyType, int> smallCompanyAvailable)
		{
			this.biggestCompany = biggestCompany;
			this.biggestCompanyRemain = biggestCompanyRemain;
			this.smallCompanyPrices = smallCompanyPrices;
			this.smallCompanyAvailable = smallCompanyAvailable;
			exchangeResult = new Dictionary<CompanyType, int>();
			saleResult = new Dictionary<CompanyType, int>();
			ShowDialog();
			var result = new HandleResult();
			result.exchange = exchangeResult;
			result.sale = saleResult;
			return result;
		}

		private void exchange_Click(object sender, RoutedEventArgs e)
		{
			foreach (var s in exchangeResult)
			{
				smallCompanyAvailable[s.Key] += s.Value;
			}
			exchangeResult = exchangeWindow.exchange(biggestCompany, biggestCompanyRemain, smallCompanyAvailable);
			foreach (var s in exchangeResult)
			{
				smallCompanyAvailable[s.Key] -= s.Value;
			}
		}

		private void sale_Click(object sender, RoutedEventArgs e)
		{
			foreach(var s in saleResult)
			{
				smallCompanyAvailable[s.Key] += s.Value;
			}
			saleResult = saleWindow.Sale(smallCompanyAvailable);
			foreach (var s in saleResult)
			{
				smallCompanyAvailable[s.Key] -= s.Value;
			}
		}

		private void over_Click(object sender, RoutedEventArgs e)
		{
			Hide();
		}
	}
}
