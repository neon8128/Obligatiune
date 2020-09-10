using BondAnalytics.Calculations;
using GalaSoft.MvvmLight.Messaging;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
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
    /// Interaction logic for Bond.xaml
    /// </summary>
    public partial class Bond : Window
    {
        public String _user;
        private String _acquired_pass;
        MySqlConnection _db;
        private BondItems _b;
        List<Tuple<String, Int32, double>> _interestList = new List<Tuple<string, int, double>>();
        List<Tuple<DateTime, DateTime>> _scheduleList = new List<Tuple<DateTime, DateTime>>();


        public DataTable _dt = new DataTable();


        public Bond()
        {
            InitializeComponent();
            _db = StaticDataManager.GetStaticDataManager().DBConnection;
            

        }

        public Bond(String User) : this()
        {
            this._user = User;

        }

        public Bond(BondItems B, String User) : this(User)
        {
            this._b = B;
            Name.Text = B.Name;
            InterestRate.Text = B.InterestRate.ToString();
            Ccy.Text = B.Ccy;
            Principal.Text = B.Principal.ToString();
            StartPick.Text = B.StartDate.ToString();
            EndPick.Text = B.EndDate.ToString();
            DayCountingConvention.Text = B.DayCountingConvention.ToString();
            Version.Text = B.Version.ToString();
            LoadList();
        }


        /// <summary>
        ///     Closing the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseBtnClick(object sender, RoutedEventArgs e)
        {

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Exiting", System.Windows.MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }


        /// <summary>
        ///     LogOut button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogOutClick(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();
            this.Close();
            main.Show();
        }

        /// <summary>
        ///     Selecting which page to access
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid);

            //  GridCursor.Margin = new Thickness(5 + (150 * index), 0, 0, 0);

            switch (index)
            {
                case 0:
                    Bond B = new Bond(_user);
                    this.Hide();
                    B.Show();
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
        ///     Going back to MainPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomeClick(object sender, RoutedEventArgs e)
        {
            Real_main_window r1 = new Real_main_window(_user);
            this.Close();
            r1.Show();
        }

        /// <summary>
        ///     if  pressed it will insert in the audit and bond table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveClick(object sender, RoutedEventArgs e)
        {
            // _dt.Rows[0]["no_days"] = 12;
            // return;

            var dbTransaction = _db.BeginTransaction();

            try
            {
                var auditId = InsertAudit();
                InsertBond(auditId);
                InsertBondHist(auditId);
                InsertSchedule();
                InsertScheduleHist();
                dbTransaction.Commit();

                MessageBox.Show("The operation was succesful!");
            }
            catch (Exception f)
            {
                // rollback()
                // rollabck to P1

                dbTransaction.Rollback();
                MessageBox.Show(f.ToString());
            }

            dbTransaction.Dispose();

        }

        /// <summary>
        ///     Inserting in the audit table and return last inserted id in order to track user activity
        /// </summary>
        public UInt64 InsertAudit()
        {
            string ip = GetLocalIPAddress();
            string pc = System.Environment.MachineName;
            DateTime time = DateTime.Now;
            UInt64 auditId = 0;
            MySqlCommand cmd = null;
            using (cmd = new MySqlCommand("INSERT INTO audit(username, TS, details, machine_name, ip) VALUES (@username,@TS,@details,@machine,@ip)", _db))
            {
                cmd.Parameters.AddWithValue("@username", _user);
                cmd.Parameters.AddWithValue("@TS", time);
                cmd.Parameters.AddWithValue("@details", "bond");
                cmd.Parameters.AddWithValue("@machine", pc);
                cmd.Parameters.AddWithValue("@ip", ip);
                cmd.ExecuteNonQuery();
            }
            cmd = new MySqlCommand("select last_insert_id()", _db);
            auditId = (UInt64)cmd.ExecuteScalar();
            cmd.Dispose();
            return auditId;
        }

        /// <summary>
        ///     Inserting in the bond table 
        /// </summary>
        public void InsertBond(UInt64 AuditId)
        {
            MySqlCommand cmd = null;
            var DayItem = DayCountingConvention.SelectionBoxItem.ToString();
            var name = Name.Text;


            cmd = new MySqlCommand($"Select version from bond where Name='{Name.Text}'", _db);
            Int32? version = (Int32?)cmd.ExecuteScalar();
            if (version == null)
            {
                version = 1;

            }
            else
            {
                version++;

            }
            cmd = new MySqlCommand($"DELETE FROM bond where Name = '{Name.Text}'", _db);
            cmd.ExecuteNonQuery();

            using (cmd = new MySqlCommand("INSERT INTO bond(name, audit_id, interest_rate, ccy, principal,start_date,end_date,day_counting_convention,version) VALUES (@name,@audit_id,@interest_rate,@ccy,@principal,@start_date,@end_date,@day_counting_convention,@version)", _db))
            {
                cmd.Parameters.AddWithValue("@name", Name.Text);
                cmd.Parameters.AddWithValue("@audit_id", AuditId);
                cmd.Parameters.AddWithValue("@interest_rate", Double.Parse(InterestRate.Text));
                cmd.Parameters.AddWithValue("@ccy", Ccy.Text);
                cmd.Parameters.AddWithValue("@principal", Principal.Text);
                cmd.Parameters.AddWithValue("@day_counting_convention", DayItem);
                cmd.Parameters.AddWithValue("@start_date", StartPick.DisplayDate);
                cmd.Parameters.AddWithValue("@end_date", EndPick.DisplayDate);
                cmd.Parameters.AddWithValue("@version", version);
                cmd.ExecuteNonQuery();
            }
            Version.Text = version.ToString();

        }

        /// <summary>
        /// Insert in the bond_hist table records of bond
        /// </summary>
        /// <param name="AuditId"></param>
        public void InsertBondHist(UInt64 AuditId)
        {
            MySqlCommand cmd = null;
            String query = $"Insert into bond_hist(name,audit_id,version,interest_rate,ccy,principal,day_counting_convention,start_date,end_date) select name,audit_id,version,interest_rate,ccy,principal,day_counting_convention,start_date,end_date from  bond b1 where b1.audit_id='{AuditId}' ";
            cmd = new MySqlCommand(query, _db);
            var xxx = cmd.ExecuteNonQuery();
            cmd.Dispose();

        }

        /// <summary>
        /// Insert in the schedule table
        /// </summary>
        public void InsertSchedule()
        {

            MySqlCommand cmd = null;
            var q = $"Delete  from schedule where bond_name ='{Name.Text}'";
            cmd = new MySqlCommand(q, _db);
            var c = cmd.ExecuteNonQuery();

            foreach (DataRow dr in _dt.Rows)
            {
                if (dr["no_days"] == null)
                {
                    return;
                }

                cmd = new MySqlCommand("Insert into schedule (bond_name,ref_day,date_coupon,bond_version,no_days,principal) values (@bond_name,@ref_day,@date_coupon,@bond_version,@no_days,@principal)", _db);
                cmd.Parameters.AddWithValue("@bond_name", Name.Text);
                cmd.Parameters.AddWithValue("@ref_day", dr["ref_day"]);
                cmd.Parameters.AddWithValue("@date_coupon", dr["date_coupon"]);
                cmd.Parameters.AddWithValue("@bond_version", Int32.Parse(Version.Text));
                cmd.Parameters.AddWithValue("@no_days", dr["no_days"]);
                cmd.Parameters.AddWithValue("@principal", dr["principal"]);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        public void InsertScheduleHist()
        {
            MySqlCommand cmd = null;

            cmd = new MySqlCommand("Insert into schedule_hist (bond_name,ref_day,date_coupon,bond_version,no_days,principal) select bond_name,ref_day,date_coupon,bond_version,no_days,principal from schedule", _db);
            var c = cmd.ExecuteNonQuery();
            cmd.Dispose();
        }

        /// <summary>
        ///     Get local ip address
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
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
        /// Load SavedBonds window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Load_Click(object sender, RoutedEventArgs e)
        {
            SavedBonds s = new SavedBonds(_user);
            s.Show();
            this.Close();
        }

        /// <summary>
        /// Loading data from schedule table
        /// </summary>
        public void LoadList()
        {
            var query = $"Select bond_name, ref_day,date_coupon,no_days,bond_version, principal from schedule where bond_name='{Name.Text}'";
            var cmd = new MySqlCommand(query, _db);

            var dbAdapter = new MySqlDataAdapter(cmd);

            dbAdapter.Fill(_dt);

            dbAdapter.Dispose();
            cmd.Dispose();

            _dt.Columns.Remove("bond_name");
            _dt.Columns.Remove("bond_version");

            _data.ItemsSource = _dt.DefaultView;

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
                try
                {
                    _dt.Rows.Remove(row.Row);
                  
                }
                catch
                {
                    MessageBox.Show("Please select a row ");
                }
            }
        }

        /// <summary>
        /// Add row from menu item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Add(object sender, RoutedEventArgs e)
        {
            var ref_day = DateTime.Now;          
            
        
            if(_dt.Rows.Count > 0)
            {
                var start = (DateTime)StartPick.SelectedDate;
                var nodays = (ref_day - start).Days;
                var principal = Int32.Parse(Principal.Text);
                DataRow dr = _dt.NewRow();
                dr["date_coupon"] = ref_day.AddYears(1);
                dr["no_days"] = nodays;
                dr["principal"] = principal;
                _dt.Rows.Add(dr);
            }
            else
            {
                LoadList();
                DataRow dr = _dt.NewRow();
                _dt.Rows.Add(dr);

            }
          
            
            

        }
       
       
        /// <summary>
        /// Add row by pressing + or delete selected row by pressing del
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _data_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    DataRowView row = (DataRowView)_data.SelectedItem;
                    e.Handled = true;
                    try
                    {
                        _dt.Rows.Remove(row.Row);
                    }
                    catch
                    {
                        MessageBox.Show("Please select a row ");
                    }
                }

            }
            else if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                DataRow dr = _dt.NewRow();
                var ref_day = DateTime.Now.Date;
                var nodays = (ref_day - StartPick.SelectedDate.Value).Days;
                dr["ref_day"] = ref_day;
                dr["date_coupon"] = ref_day;
                dr["no_days"] = nodays;
                dr["principal"] = Int32.Parse(Principal.Text);
                _dt.Rows.Add(dr);
            }
        }

        private void _data_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            
        }

       

        /// <summary>
        /// Calculate price of the bond
        /// </summary>
        /// <returns></returns>
        public double GetCurrentPrice()
        {
            Lists lists = new Lists();
            CashFlow getCashFlow = new CashFlow();
            ZeroRate z = new ZeroRate();
            var asof = (DateTime)AsOfDate.SelectedDate;
            var principal = Double.Parse(Principal.Text);
            var interestRate = Double.Parse(InterestRate.Text);
            var startDate = (DateTime)StartPick.SelectedDate;
            var endDate = (DateTime)EndPick.SelectedDate;

            _interestList = lists.GetInterestList(NameInterest.Text, Ccy.Text, asof); // get list from interestTable
            _scheduleList = lists.GetScheduleList(Name.Text);// get list from schedule table

            Double FinalSum = 0;
            Double sum = getCashFlow.GetDiscoutendCashFlow(startDate, _scheduleList[0].Item2, principal, interestRate);

            if (_scheduleList.Count > 0)
            {
                for (Int32 i = 0; i < _scheduleList.Count; i++)
                {

                   var zeroRate = (Double)z.LinearInterpolation(asof, _scheduleList[i].Item2, _interestList); // get interest rate at payday
                    if(i == 0)
                    {
                        FinalSum = FinalSum + getCashFlow.GetCurrentCashFlow(asof, _scheduleList[0].Item2, sum, 1.6);
                    }
                                     
                    if (_scheduleList.Count > 1 && _scheduleList[i] != _scheduleList[_scheduleList.Count - 1]) 
                        // if there is only one item in schedule
                        // and we are not at the last pair
                    {
                        sum = sum + getCashFlow.GetDiscoutendCashFlow(_scheduleList[i].Item2, _scheduleList[i + 1].Item2, principal, interestRate);
                        FinalSum = FinalSum + getCashFlow.GetCurrentCashFlow(asof, _scheduleList[i].Item2, sum, zeroRate);
                    }
                   

                    if (_scheduleList[i] == _scheduleList[_scheduleList.Count - 1] || _scheduleList.Count == 1) // if we are at the last pair 
                    {
                         var zeroRateFinal = (Double)z.LinearInterpolation(asof, endDate, _interestList);
                       // if(zeroRateFinal == 0)
                       // {
                       //     MessageBox.Show("Not enough data in interestList");
                       // }
                        sum = sum +principal;
                        FinalSum = FinalSum + getCashFlow.GetCurrentCashFlow(asof, endDate, sum, 1.8);
                    }
                }
            }
        
            return (FinalSum/principal)*100;
        }
        
       

        private void GetPrice_Click(object sender, RoutedEventArgs e)
        {
            Double FinalSum = Math.Round(GetCurrentPrice(), 3);
            Price.Text = FinalSum.ToString();

        }
    }
}
