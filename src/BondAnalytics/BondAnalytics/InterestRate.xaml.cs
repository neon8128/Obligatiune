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

namespace BondAnalytics
{
    /// <summary>
    /// Interaction logic for InterestRate.xaml
    /// </summary>
    public partial class Interest_rate : Window
    {
        private string _user;

        public Interest_rate()
        {
            InitializeComponent();
        }

        public Interest_rate(string User)
        {
            this._user = User;
            InitializeComponent();
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

            //GridCursor.Margin = new Thickness(5 + (150 * index), 0, 0, 0);

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
    }
}
