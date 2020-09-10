using BondAnalytics.ExchangeRateData;
using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
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


namespace BondAnalytics
{
    /// <summary>
    /// Interaction logic for FX.xaml
    /// </summary>
    /// </summary>
    public partial class FX : Window
    {
        private string _user;
        public List<Tuple<String, Double>> _exchangeData = new List<Tuple<string, double>>();
        MySqlConnection _db;

        public FX()
        {
            InitializeComponent();
            _db = StaticDataManager.GetStaticDataManager().DBConnection;
            GetData();
            
        }

        public FX(string User):this()
        {
            
            this._user = User;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Close();
            main.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid);            

            switch (index)
            {
                case 0:
                    Bond b = new Bond(_user);
                    this.Hide();
                    b.Show();
                    break;
                case 1:
                    FX fx = new FX(_user);
                    this.Hide();
                    fx.Show();
                    break;

                case 2:
                    Interest_rate f = new Interest_rate(_user);
                    this.Hide();
                    f.Show();
                    break;
            }
        }
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Real_main_window r1 = new Real_main_window();
            this.Close();
            r1.Show();
        }

        /// <summary>
        ///     This way the Window is draggable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        public void GetData()
        {
            ExchangeTable exchange = new ExchangeTable();
            _exchangeData = exchange.GetExchangeRate(); //get list from exchange site
            if (_exchangeData.Count > 0)
            {
                for (Int32 i = 0; i < _exchangeData.Count; i++)
                {
                    _comboCcy1.Items.Add(_exchangeData[i].Item1);
                }
                _comboCcy1.Text = _comboCcy1.Items[0].ToString();
                _comboCcy2.Text = "RON";
            }
          

        }


        private void _comboExchange_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in _exchangeData)
            {
                if (item.Item1 == _comboCcy1.SelectedItem)
                {
                    _rate.Text = item.Item2.ToString();
                }
            }
        }

        private void Sum_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var sum = Double.Parse(_sum.Text);
                var rate = Double.Parse(_rate.Text);
                _result.Text = (sum * rate).ToString();
            }
            catch (Exception f)
            {
                // do nothing
            }


        }

        private void _rate_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var sum = Double.Parse(_sum.Text);
                var rate = Double.Parse(_rate.Text);
                _result.Text = (sum * rate).ToString();
            }
            catch (Exception f)
            {
                // do nothing
            }
        }
    }
}
