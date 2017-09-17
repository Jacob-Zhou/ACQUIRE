using ACQUIRE.model;
using ACQUIRE.presenter;
using System;
using System.Collections.Generic;
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
	/// exchange.xaml 的交互逻辑
	/// </summary>
	/// 
	[ValueConversion(typeof(int), typeof(int))]
	public partial class DoubleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int number = int.Parse(value.ToString());
			return 2 * number;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			int number = int.Parse(value.ToString());
			return number / 2;
		}
	}

	public partial class Exchange : Window
	{
		private Button[] big_btn = new Button[3];
		private Button[] small_btn = new Button[3];
		private Slider[] slider = new Slider[3];
		private int companyCount = 0;
		private CompanyType biggestCompany;
		private int biggestRemainShare;
		private CompanyType[] companys = new CompanyType[3];
		private Dictionary<CompanyType, int> result = new Dictionary<CompanyType, int>();
		private Dictionary<CompanyType, int> available = new Dictionary<CompanyType, int>();
		private bool protectCount;


		public Exchange()
		{
			InitializeComponent();
			WindowStyle = WindowStyle.None;
			ResizeMode = ResizeMode.NoResize;
			WindowStartupLocation = WindowStartupLocation.CenterOwner;
			big_btn[0] = big_btn_0;
			big_btn[1] = big_btn_1;
			big_btn[2] = big_btn_2;

			small_btn[0] = small_btn_0;
			small_btn[1] = small_btn_1;
			small_btn[2] = small_btn_2;

			slider[0] = slider_0;
			slider[1] = slider_1;
			slider[2] = slider_2;
		}

		public Dictionary<CompanyType, int> exchange(CompanyType biggest, int biggestRemainShare, Dictionary<CompanyType, int> available)
		{
			var background = CompanyColor.Color[biggest];
			companyCount = available.Count;
			if(companyCount > 3)
			{
				throw new NotSupportedException("more than 3 small company");
			}
			biggestCompany = biggest;
			this.biggestRemainShare = biggestRemainShare;
			this.available = available;
			int i = 0;
			for(int j = 0; j < 3; j++)
			{
				big_btn[j].Background = CompanyColor.Color[biggest];
				big_btn[j].IsEnabled = false;
				small_btn[j].IsEnabled = false;
				slider[j].IsEnabled = false;
				protectCount = true;
				slider[j].Value = 0;
				protectCount = false;
			}
			
			foreach(var c in available)
			{
				if (c.Key != biggest)
				{
					companys[i] = c.Key;
					small_btn[i].Background = CompanyColor.Color[c.Key];
					big_btn[i].IsEnabled = true;
					small_btn[i].IsEnabled = true;
					slider[i].IsEnabled = true;
					slider[i].Maximum = c.Value / 2;
					result[c.Key] = 0;
					i++;
				}
			}

			ShowDialog();

			return result;
		}

		private void finish_Click(object sender, RoutedEventArgs e)
		{
			Hide();
		}

		private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{

			if (!protectCount)
			{
				int delta = (int)(e.NewValue - e.OldValue);
				CompanyType smallCompany = companys[int.Parse(((UIElement)sender).Uid)];
				if (biggestRemainShare - delta < 0 || available[smallCompany] - 2 * delta < 0)
				{
					protectCount = true;
					((Slider)sender).Value = e.OldValue;
					protectCount = false;
				}
				else
				{
					if(result.ContainsKey(smallCompany))
					{
						biggestRemainShare -= delta;
						result[smallCompany] += 2 * delta;
						available[smallCompany] -= 2 * delta;
					}
				}
			}
		}
	}
}
