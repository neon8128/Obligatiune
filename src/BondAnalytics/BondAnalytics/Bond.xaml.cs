﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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

namespace BondAnalytics
{
    /// <summary>
    /// Interaction logic for Bond.xaml
    /// </summary>
    public partial class Bond : Window
    {
        private String _user;
        private String _acquired_pass;


        public Bond()
        {
            InitializeComponent();

        }

        public Bond(String User, String Acquired_pass)
        {
            InitializeComponent();
            this._user = User;
            this._acquired_pass = Acquired_pass;
        }


        /// <summary>
        ///     Closing the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        /// <summary>
        ///     LogOut button
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
        ///     Selecting which page to access
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
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
        private void Home_Click(object sender, RoutedEventArgs e)
        {
            Real_main_window r1 = new Real_main_window();
            this.Close();
            r1.Show();
        }


        /// <summary>
        ///  Displaying selected  StartDate in textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartDate(object sender, SelectionChangedEventArgs e)
        {
            
            Start.Text = StartPick.Text;
        }

        /// <summary>
        ///     Displaying selected EndDate in textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EndDate(object sender, SelectionChangedEventArgs e)
        {
          
            End.Text = EndPick.Text;
            

        }


        /// <summary>
        ///     if  pressed it will insert in the audit and bond table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Insert_audit();
            Insert_bond();

        }


        /// <summary>
        ///     Inserting in the audit table in order to track user activity
        /// </summary>
        public void Insert_audit()
        {
            string ip = GetLocalIPAddress();
            string pc = System.Environment.MachineName;
            DateTime time = DateTime.Now;
           


            string connection = "server=localhost;port=3306;uid=" + _user + ";pwd=" + _acquired_pass + ";database=bond;charset=utf8;SslMode=none";

            using (var conn = new MySqlConnection(connection))
            {
                //conn.Open();

                using (var cmd = new MySqlCommand("INSERT INTO `audit`(`username`, `TS`, `details`, `machine_name`, `ip`) VALUES (@username,@TS,@details,@machine,@ip)", conn))
                {
                    cmd.Parameters.AddWithValue("@username", _user);
                    cmd.Parameters.AddWithValue("@TS", time);
                    cmd.Parameters.AddWithValue("@details", "bond");
                    cmd.Parameters.AddWithValue("@machine", pc);
                    cmd.Parameters.AddWithValue("@ip", ip);
                    cmd.ExecuteNonQuery();

                    conn.Close();

                }

            }
        }

        /// <summary>
        ///     Inserting in the bond table 
        /// </summary>
        public void Insert_bond()
        {
            int audit_id = 0;
            bool ok = false;
            string connection = "server=localhost;port=3306;uid=" + _user + ";pwd=" + _acquired_pass + ";database=bond;charset=utf8;SslMode=none";

            using (var conn = new MySqlConnection(connection))
            {
                conn.Open();

                using (var cmd = new MySqlCommand("INSERT INTO `bond`(`name`, `audit_id`, `interest_rate`, `ccy`, `principal`,`start_date`,`end_date`) VALUES (@name,@audit_id,@interest_rate,@ccy,@principal,@start_date,@end_date)", conn))
                {
                    cmd.Parameters.AddWithValue("@name", Name.Text);
                    cmd.Parameters.AddWithValue("@audit_id", audit_id);
                    cmd.Parameters.AddWithValue("@interest_rate", InterestRate.Text);
                    cmd.Parameters.AddWithValue("@ccy", Ccy.Text);
                    cmd.Parameters.AddWithValue("@principal", Principal.Text);
                    //cmd.Parameters.AddWithValue("@day_counting_convention", Day_Counting_Convention.Text);
                    cmd.Parameters.AddWithValue("@start_date", Start.Text);
                    cmd.Parameters.AddWithValue("@end_date", End.Text);
                    cmd.ExecuteNonQuery();

                    conn.Close();

                }

            }
            ok = true;
            if (ok) MessageBox.Show("ok");
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
