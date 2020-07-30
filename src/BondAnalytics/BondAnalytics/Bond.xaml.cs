using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using MessageBox = System.Windows.Forms.MessageBox;

namespace BondAnalytics
{
    /// <summary>
    /// Interaction logic for Bond.xaml
    /// </summary>
    public partial class Bond : Window
    {
        private String _user;
        private String _acquired_pass;
        private Validate _bond = new Validate();


        public Bond()
        {
            InitializeComponent();
            GridMain.DataContext = _bond;

        }

        public Bond(String User, String Acquired_pass)
        {
            InitializeComponent();
            this._user = User;
            this._acquired_pass = Acquired_pass;
            GridMain.DataContext = _bond;
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


        /// <summary>
        ///     Going back to MainPage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HomeClick(object sender, RoutedEventArgs e)
        {
            Real_main_window r1 = new Real_main_window();
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

        }


        /// <summary>
        ///     Inserting in the audit table in order to track user activity
        /// </summary>
        public UInt64 InsertAudit()
        {
            string ip = GetLocalIPAddress();
            string pc = System.Environment.MachineName;
            DateTime time = DateTime.Now;
            UInt64 auditId = 0;
            MySqlCommand cmd = null;
            String connection = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;

            using (var conn = new MySqlConnection(connection))
            {
                conn.Open();
                using (cmd = new MySqlCommand("INSERT INTO bond.audit(username, TS, details, machine_name, ip) VALUES (@username,@TS,@details,@machine,@ip)", conn))
                {
                    cmd.Parameters.AddWithValue("@username", _user);
                    cmd.Parameters.AddWithValue("@TS", time);
                    cmd.Parameters.AddWithValue("@details", "bond");
                    cmd.Parameters.AddWithValue("@machine", pc);
                    cmd.Parameters.AddWithValue("@ip", ip);
                    cmd.ExecuteNonQuery();
                }

                cmd = new MySqlCommand("select last_insert_id()", conn);
            
                auditId = (UInt64)cmd.ExecuteScalar();
                cmd.Dispose();

                conn.Close();
            }

            return auditId;
        }

        /// <summary>
        ///     Inserting in the bond table 
        /// </summary>
        public void InsertBond(UInt64 AuditId)
        {
            MySqlCommand cmd = null;
            String connection = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            using (var conn = new MySqlConnection(connection))
            {
                try
                {
                    conn.Open();

                    cmd = new MySqlCommand($"DELETE FROM bond.bond where Name = '{Name.Text}'", conn);
                    var xxx = cmd.ExecuteNonQuery();

                    using (cmd = new MySqlCommand("INSERT INTO bond.bond(name, audit_id, interest_rate, ccy, principal,start_date,end_date) VALUES (@name,@audit_id,@interest_rate,@ccy,@principal,@start_date,@end_date)", conn))
                    {
                        cmd.Parameters.AddWithValue("@name", Name.Text);
                        cmd.Parameters.AddWithValue("@audit_id", AuditId);
                        cmd.Parameters.AddWithValue("@interest_rate", Double.Parse(InterestRate.Text));
                        cmd.Parameters.AddWithValue("@ccy", Ccy.Text);
                        cmd.Parameters.AddWithValue("@principal", Principal.Text);
                        cmd.Parameters.AddWithValue("@day_counting_convention", DayCountingConvention.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@start_date", StartPick.DisplayDate);
                        cmd.Parameters.AddWithValue("@end_date", EndPick.DisplayDate);
                        cmd.ExecuteNonQuery();
                        conn.Close();
                        MessageBox.Show("The operation was successful");
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }               
            }              
        }


        /// <summary>
        ///     Get last inserted audit with description = bond
        /// </summary>
        /// <returns></returns>
        public Int32 GetLastAuditID()
        {
            Int32 id = 0;
            String desc = "bond";
            String connection = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
            var query = $"Select audit_id From bond.audit Where details='{desc}' ORDER BY audit_id DESC";
            try
            {
                using(var con= new MySqlConnection(connection))
                {
                    con.Open(); //open connection
                    using (var cmd = new MySqlCommand(query, con))
                    {
                       var id_test = cmd.ExecuteScalar();
                        id = (Int32)id_test;
                    }                   
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return id;
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


       
    }
}
