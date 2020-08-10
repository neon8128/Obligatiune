using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
    public partial class SavedBonds : Window
    {
        MySqlConnection _db;
        private string _user;
        public ArrayList _bondItems = new ArrayList();
       

        public SavedBonds()
        {
            InitializeComponent();
            _db = DataBase.Connection;
            LoadList();
        }

        public SavedBonds(string user)
        {
            InitializeComponent();
            _db = DataBase.Connection;
            this._user = user;
            LoadList();
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

        /// <summary>
        /// if pressed the MainPage will appear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Real_main_window r1 = new Real_main_window(_user);
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

        /// <summary>
        /// Load all the bonds saved in the database
        /// </summary>
        public void LoadList()
        {
            MySqlCommand cmd = null;
            var query = $"Select username,name,ccy,interest_rate,principal,start_date,end_date,version,day_counting_convention from bond where username='{_user}'";
            cmd = new MySqlCommand(query, _db);
            var reader = cmd.ExecuteReader();
            while(reader.Read())
            {
                try
                {
                    _bondItems.Add(new BondItems
                    {
                        Username = reader.GetString(0),
                        Name = reader.GetString(1),
                        Ccy = reader.GetString(2),
                        InterestRate = reader.GetDouble(3),
                        Principal = reader.GetInt32(4),
                        StartDate = reader.GetString(5),
                        EndDate = reader.GetString(6),
                        Version = reader.GetInt32(7),
                        DayCountingConvention = reader.GetString(8)
                    });
                   
                   DataGrid.ItemsSource = _bondItems;
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message + " at LoadList");
                }
                
            }
            reader.Dispose();
            cmd.Dispose();
            
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            
        }

       

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            BondItems b = DataGrid.SelectedItem as BondItems;
            Bond b1 = new Bond(b,_user);
            this.Hide();
            b1.Show();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }

    public class BondItems
    {   
        public String Username { get; set; }

        public String Name { get; set; }

        public String Ccy { get; set; }

        public Double InterestRate { get; set; }

        public Int32 Principal { get; set; }

        public String StartDate { get; set; }

        public String EndDate { get; set; }

        public Int32 Version { get; set; }

        public String DayCountingConvention { get; set; }
    }
}
