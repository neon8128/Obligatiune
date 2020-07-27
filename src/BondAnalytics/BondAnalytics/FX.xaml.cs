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
    /// Interaction logic for FX.xaml
    /// </summary>
    /// </summary>
    public partial class FX : Window
    {
        public FX()
        {
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
                    Bond b = new Bond();
                    this.Hide();
                    b.Show();
                    break;


                case 1:
                    FX fx = new FX();
                    this.Hide();
                    fx.Show();
                    break;

                case 2:
                    Interest_rate f = new Interest_rate();
                    this.Hide();
                    f.Show();
                    break;

                case 3:
                    Exchange e1 = new Exchange();
                    this.Hide();
                    e1.Show();
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
