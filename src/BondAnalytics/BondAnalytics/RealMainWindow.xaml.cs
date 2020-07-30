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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BondAnalytics
{
  
    public partial class Real_main_window : Window
    {
        private String _user;
        private String _acquired_pass;

        public Real_main_window()
        {
            InitializeComponent();

        }

        public Real_main_window(String User, String Acquired_pass)
        {
            InitializeComponent();
            this._user = User;
            this._acquired_pass =Acquired_pass;//parola baza de date
        }


        /// <summary>
        /// Closing the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        /// <summary>
        /// LogOut button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogOut_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Close();
            main.Show();
        }

        /// <summary>
        /// Changing pages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid); //Getting the index of the menulist 

            //GridCursor.Margin = new Thickness(5 + (150 * index), 0, 0, 0);

            switch (index) //changing pages based on index
            {
                case 0:
                    Bond b = new Bond(_user, _acquired_pass);
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

        /// <summary>
        /// if pressed the MainPage will appear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Real_main_window r1 = new Real_main_window();
            this.Close();
            r1.Show();
        }

        /// <summary>
        /// Making the window draggable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
