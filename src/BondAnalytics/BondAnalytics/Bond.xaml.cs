using MySql.Data.MySqlClient;
using System;
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
        Validation _v= new Validation();
        private int _noOfErrorsOnScreen = 0;

        public Bond()
        {
            InitializeComponent();
            _db = DataBase.Connection;
            

        }

        public Bond(String User)
        {
            InitializeComponent();
            this._user = User;
            _db = DataBase.Connection;
           

        }

        public Bond(BondItems B, String User)
        {
            InitializeComponent();
            this._user = User;
            this._b = B;
            _db = DataBase.Connection;
            Name.Text = B.Name;
            InterestRate.Text = B.InterestRate.ToString();
            Ccy.Text = B.Ccy;
            Principal.Text = B.Principal.ToString();
            StartPick.Text = B.StartDate.ToString();
            EndPick.Text = B.EndDate.ToString();
            DayCountingConvention.Text = B.DayCountingConvention.ToString();
           
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
            var auditId = InsertAudit();
            InsertBond(auditId);
            InsertBondHist(auditId);

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
            using (cmd = new MySqlCommand("INSERT INTO bond_new.audit(username, TS, details, machine_name, ip) VALUES (@username,@TS,@details,@machine,@ip)", _db))
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
                cmd = new MySqlCommand($"Select version from bond_new.bond where Name='{Name.Text}'", _db);
                Int32? version = (Int32?)cmd.ExecuteScalar();
                if (version == null)
                {
                    version = 1;

                }
                else
                {
                    version++;

                }
                cmd = new MySqlCommand($"DELETE FROM bond_new.bond where Name = '{Name.Text}'", _db);
                cmd.ExecuteNonQuery();

                using (cmd = new MySqlCommand("INSERT INTO bond_new.bond(username,name, audit_id, interest_rate, ccy, principal,start_date,end_date,day_counting_convention,version) VALUES (@username,@name,@audit_id,@interest_rate,@ccy,@principal,@start_date,@end_date,@day_counting_convention,@version)", _db))
                {
                    cmd.Parameters.AddWithValue("@username", _user);
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

                    MessageBox.Show("The operation was successful");
                }

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
            //using (var conn = new MySqlConnection(connection))
            try
            {
                //conn.Open();
                String query = $"Insert into bond_new.bond_hist(username,name,audit_id,version,interest_rate,ccy,principal,day_counting_convention,start_date,end_date) select username,name,audit_id,version,interest_rate,ccy,principal,day_counting_convention,start_date,end_date from  bond b1 where b1.audit_id='{AuditId}' ";
                cmd = new MySqlCommand(query, _db);
                var xxx = cmd.ExecuteNonQuery();
                // conn.Close();
                cmd.Dispose();
                MessageBox.Show("Inserted in bond hist");

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "at InsertBondHist");
            }
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

       

    }
}
