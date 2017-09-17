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
	/// SaleAndBuy.xaml 的交互逻辑
	/// </summary>
	public partial class SaleAndBuy : Window
	{
		private Dictionary<CompanyType, Button> companyLables;
		private Dictionary<CompanyType, Slider> companySliders;
		private Dictionary<string, int> available = new Dictionary<string, int>();
		private Dictionary<string, int> prices = new Dictionary<string, int>();
		private int remainMoney;
		private Dictionary<CompanyType, int> result;
		private int buyCount = 0;
		private bool isBuy = false;
		private bool protectCount = false;

		public SaleAndBuy()
		{
			InitializeComponent();
			WindowStyle = WindowStyle.None;
			ResizeMode = ResizeMode.NoResize;
			WindowStartupLocation = WindowStartupLocation.CenterOwner;
			companyLables = new Dictionary<CompanyType, Button>
			{
				{CompanyType.WORLDWIDE, WORLDWIDE_btn },
				{CompanyType.SACKSON, SACKSON_btn },
				{CompanyType.FESTIVAL, FESTIVAL_btn },
				{CompanyType.AMERICAN, AMERICAN_btn },
				{CompanyType.IMPERIAL, IMPERIAL_btn },
				{CompanyType.CONTINENTAL, CONTINENTAL_btn },
				{CompanyType.TOWER, TOWER_btn }
			};

			companySliders = new Dictionary<CompanyType, Slider>
			{
				{CompanyType.WORLDWIDE, WORLDWIDE_slider },
				{CompanyType.SACKSON, SACKSON_slider },
				{CompanyType.FESTIVAL, FESTIVAL_slider },
				{CompanyType.AMERICAN, AMERICAN_slider },
				{CompanyType.IMPERIAL, IMPERIAL_slider },
				{CompanyType.CONTINENTAL, CONTINENTAL_slider },
				{CompanyType.TOWER, TOWER_slider }
			};

			result = new Dictionary<CompanyType, int>();

			//result = new Dictionary<CompanyType, int>
			//{
			//	{CompanyType.WORLDWIDE, 0 },
			//	{CompanyType.SACKSON, 0 },
			//	{CompanyType.FESTIVAL, 0 },
			//	{CompanyType.AMERICAN, 0 },
			//	{CompanyType.IMPERIAL, 0 },
			//	{CompanyType.CONTINENTAL, 0 },
			//	{CompanyType.TOWER, 0 }
			//};

			for (int i = 0; i < 7; i++)
			{
				available[((CompanyType)i).ToString()] = 0;
			}
		}

		public Dictionary<CompanyType, int> Buy(Dictionary<CompanyType, int> companys, Dictionary<CompanyType, int> companyPrices, int playerMoney)
		{
			isBuy = true;
			if (companys.Count > 0)
			{
				foreach (var p in companyPrices)
				{
					prices[p.Key.ToString()] = p.Value;
				}
				remainMoney = playerMoney;
				Dispatcher.Invoke(() =>
				{
					Init(companys);
					ShowDialog();
				});
				return result;
			}
			else
			{
				return new Dictionary<CompanyType, int>();
			}
		}

		public Dictionary<CompanyType, int> Sale(Dictionary<CompanyType, int> companys)
		{
			isBuy = false;
			Init(companys);
			ShowDialog();
			return result;
		}

		private void Init(Dictionary<CompanyType, int> companys)
		{
			result.Clear();
			for (int i = 0; i < 7; i++)
			{
				companyLables[(CompanyType)i].IsEnabled = false;
				companySliders[(CompanyType)i].IsEnabled = false;
				protectCount = true;
				companySliders[(CompanyType)i].Value = 0;
				protectCount = false;
			}
			foreach (var c in companys)
			{
				available[c.Key.ToString()] = c.Value;
				if (c.Value != 0)
				{
					companyLables[c.Key].IsEnabled = true;
					companySliders[c.Key].IsEnabled = true;
					if(!isBuy)
					{
						companySliders[c.Key].Maximum = c.Value;
					}
					else
					{
						companySliders[c.Key].Maximum = 3;
					}
				}
			}
		}

		private CompanyType toCompanyType(string name)
		{
			switch(name)
			{
				case "WORLDWIDE":
					return CompanyType.WORLDWIDE;
				case "SACKSON":
					return CompanyType.SACKSON;
				case "FESTIVAL":
					return CompanyType.FESTIVAL;
				case "IMPERIAL":
					return CompanyType.IMPERIAL;
				case "AMERICAN":
					return CompanyType.AMERICAN;
				case "CONTINENTAL":
					return CompanyType.CONTINENTAL;
				case "TOWER":
					return CompanyType.TOWER;
			}
			return CompanyType.NULL;
		}

		private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

			if (!protectCount)
			{
				string Uid = ((UIElement)sender).Uid;
				if (isBuy)
				{
					if (e.NewValue - e.OldValue + buyCount > 3 || e.NewValue > available[Uid] || (e.NewValue - e.OldValue) * prices[Uid] > remainMoney)
					{
						protectCount = true;
						((Slider)sender).Value = e.OldValue;
						protectCount = false;
					}
					else
					{
						buyCount -= (int)e.OldValue;
						buyCount += (int)e.NewValue;
						remainMoney += (int)e.OldValue * prices[Uid];
						remainMoney -= (int)e.NewValue * prices[Uid];
						result[toCompanyType(Uid)] = (int)e.NewValue;
					}
				}
				else
				{
					if (e.NewValue > available[Uid])
					{
						protectCount = true;
						((Slider)sender).Value = e.OldValue;
						protectCount = false;
					}
					else
					{
						result[toCompanyType(Uid)] = (int)e.NewValue;
					}
				}
			}
		}

		private void finish_Click(object sender, RoutedEventArgs e)
		{
			Hide();
		}
	}
}
