using GalaSoft.MvvmLight.Messaging;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
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
        public List<Schedule> _schedule = new List<Schedule>();

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
            Application.Current.Shutdown();

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
            _dt.Rows[0]["no_days"] = 12;
            return;

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
            catch(Exception f)
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
            String DayItem = DayCountingConvention.SelectionBoxItem.ToString();
            String name = Name.Text;

            try
            {
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
            catch (Exception e)
            {
                MessageBox.Show(e.ToString() + " at insert bond");
            }


        }

        /// <summary>
        /// Insert in the bond_hist table records of bond
        /// </summary>
        /// <param name="AuditId"></param>
        public void InsertBondHist(UInt64 AuditId)
        {
            MySqlCommand cmd = null;
            try
            {
                String query = $"Insert into bond_hist(name,audit_id,version,interest_rate,ccy,principal,day_counting_convention,start_date,end_date) select name,audit_id,version,interest_rate,ccy,principal,day_counting_convention,start_date,end_date from  bond b1 where b1.audit_id='{AuditId}' ";
                cmd = new MySqlCommand(query, _db);
                var xxx = cmd.ExecuteNonQuery();
                cmd.Dispose();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "at InsertBondHist");
            }
        }

        /// <summary>
        /// Insert in the schedule table
        /// </summary>
        public void InsertSchedule()
        {
            Schedule item = Data.SelectedItem as Schedule;

            var changesDT = _dt.GetChanges();

            /*
            if (changesDT == null)
            {
                return;
            }
            else
            {
            */
                // todo: check values for null when computing noDays

                var noDays = (Int32)(EndPick.SelectedDate.Value - StartPick.SelectedDate.Value).Days;
                MySqlCommand cmd = null;

                // todo: you need to save all the records, not only one

                var q = $"Select version from bond where name = '{(String)changesDT.Rows[0]["bond_name"]}'";
                cmd = new MySqlCommand(q, _db);
                Int32 version = (Int32)cmd.ExecuteScalar();
                cmd = new MySqlCommand("Insert into schedule (bond_name,ref_day,date_coupon,bond_version,no_days,principal) values (@bond_name,@ref_day,@date_coupon,@bond_version,@no_days,@principal)", _db);
                cmd.Parameters.AddWithValue("@bond_name", item.Name);
                cmd.Parameters.AddWithValue("@ref_day", item.RefDay);
                cmd.Parameters.AddWithValue("@date_coupon", item.DateCoupon);
                cmd.Parameters.AddWithValue("@bond_version", version);
                cmd.Parameters.AddWithValue("@no_days", noDays);
                cmd.Parameters.AddWithValue("@principal", Principal.Text);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            /*}*/
        }

        public void InsertScheduleHist()
        {
            MySqlCommand cmd = null;
            
            cmd = new MySqlCommand("Insert into schedule_hist (bond_name,ref_day,date_coupon,bond_version,no_days,principal) select bond_name,ref_day,date_coupon,bond_version,no_days,principal from schedule", _db);
            var c= cmd.ExecuteNonQuery();
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
            /*
            MySqlCommand cmd = null;
            var query = $"Select bond_name,ref_day,date_coupon,bond_version,no_days from schedule where bond_name='{Name.Text}'";
            cmd = new MySqlCommand(query, _db);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                try
                {
                    _schedule.Add(new Schedule
                    {
                        Name = reader.GetString(0),
                        RefDay=reader.GetDateTime(1),
                        DateCoupon=reader.GetDateTime(2),
                        BondVersion=reader.GetInt32(3),
                        NoDays=reader.GetInt32(4)
                    });
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }

            }
            reader.Dispose();
            cmd.Dispose();
            */


            var query = $"Select bond_name,ref_day,date_coupon,bond_version,no_days from schedule where bond_name='{Name.Text}'";
            var cmd = new MySqlCommand(query, _db);

            var dbAdapter = new MySqlDataAdapter(cmd);


            dbAdapter.Fill(_dt);

            dbAdapter.Dispose();
            cmd.Dispose();

            //Data.ItemsSource = _schedule;
            Data.ItemsSource = _dt.DefaultView;
        }

        private void Name_TextChanged(object sender, TextChangedEventArgs e)
        {
            int count = _schedule.Count;
            if (count == 0)
            {
                _schedule.Add(new Schedule { Name = Name.Text });
                Data.ItemsSource = _schedule;
            }
            else
            {
                return;
            }
            
        }
    }

    public class Schedule
    {
        public String Name { get; set; }

        public DateTime RefDay { get; set; }

        public DateTime DateCoupon { get; set; }

        public Int32 BondVersion { get; set; }

        public Int32 NoDays { get; set; }
       
    }
}
