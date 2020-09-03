using MySql.Data.MySqlClient;
using System;
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
    /// <summary>
    /// Interaction logic for SavedInterestRate.xaml
    /// </summary>
    public partial class SavedInterestRate : Window
    {
        private string _user;
        MySqlConnection _db;
        public DataTable _dt = new DataTable();

        public SavedInterestRate()
        {
            InitializeComponent();
            _db = StaticDataManager.GetStaticDataManager().DBConnection;
        }

        public SavedInterestRate(string User):this()
        {
            this._user = User;
            LoadList();
            _data.ItemsSource = null;
            _data.ItemsSource = _dt.DefaultView;
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
            _data.ItemsSource = null;
            var query = $"select name,as_of_date,ccy,version,term,date from interest_rate group by as_of_date  ";

            cmd = new MySqlCommand(query, _db);

            var dataAdapter = new MySqlDataAdapter(cmd);

            dataAdapter.Fill(_dt);
            dataAdapter.Dispose();            
            cmd.Dispose();

            _data.ItemsSource = _dt.DefaultView;

        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }



        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

            DataRowView row = (DataRowView)_data.SelectedItem;
            if(row == null)
            {
                MessageBox.Show("Please select a row");
                return;
            }
            var r = new Interest_rate(row, _user);
            r.Show();
            this.Hide();
           
        }

        /// <summary>
        /// Delete row from menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Delete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                DataRowView row = (DataRowView)_data.SelectedItem;
                var asofStr = $"STR_TO_DATE('{row["as_of_date"]}', '%d/%m/%Y')";
                if (row != null)
                {
                    // _dt.Rows.Remove(row.Row);
                  
                    var cmd = new MySqlCommand($"DELETE FROM interest_rate where Name='{row["name"]}' and as_of_date = {asofStr} and ccy = '{row["ccy"]}' ", _db);
                    var x = cmd.ExecuteNonQuery();
                    _dt.Rows.Remove((DataRow)row.Row);
                    _dt.AcceptChanges();
                }
                else
                {
                    MessageBox.Show("Please select a row");
                }


            }
        }

     
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
  
}
