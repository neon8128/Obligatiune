using MySql.Data.MySqlClient;
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
    /// Interaction logic for After_login.xaml
    /// </summary>
    public partial class After_login : Window
    {
        int attempts = 0;
        private string user;
        private string acquired_pass;





        public After_login(string user, string acquired_pass)
        {
            InitializeComponent();
            this.user = user;
            this.acquired_pass = acquired_pass;

        }


        /// <summary>
        ///    if the password stored in the table users matches Pass return true; otherwise return false
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Pass"></param>
        /// <returns></returns>
        public bool Verify(string User, string Pass)
        {
            string test_pass = null;
            string connection = "server=localhost;port=3306;uid=" + User + ";pwd=" + acquired_pass + ";database=bond;charset=utf8;SslMode=none";

            using (var conn = new MySqlConnection(connection))
            {
                conn.Open();    // open the connection
                                // question: what happens if Open() fails / throws exception???

                var sqlQuery = $"SELECT password FROM user where username='{user}'";

                String testPass = null;
                using (var cmd = new MySqlCommand(sqlQuery, conn))
                {
                    testPass = cmd.ExecuteScalar() as String;
                }

                conn.Close();   // close the connection????

                // if the password stored in database is equal to teh one the user entered return true; false otherwise
                if (testPass == Pass)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool Verify_with_credentials(string user)
        {

            string test_pass = null;
            string connection = "server=localhost;port=3306;uid=" + user + ";pwd=" + acquired_pass + ";database=bond;charset=utf8;SslMode=none";

            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                conn.Open();

                using (var cmd = new MySqlCommand("SELECT password FROM user where username=@user", conn))
                {
                    cmd.Parameters.AddWithValue("@user", user);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            test_pass = reader.GetString(0);
                        }
                    }
                    conn.Close();
                    if (test_pass == acquired_pass)
                        return true;
                }
                return false;
            }
        }

        private void c1_Checked(object sender, RoutedEventArgs e)
        {
            // if (c1.IsChecked ?? false)
            // {
            //     MessageBox.Show(password.Password);
            // }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (Verify(user, password.Password))
            {
                MessageBox.Show("You have successfully logged in as " + user);
                Insert_audit();
                this.Close();
                Real_main_window r1 = new Real_main_window(user, acquired_pass);
                r1.Show();

            }

            else
            {
                attempts++;
                MessageBox.Show("Try again!" + "\n" + "You have tried " + attempts + " out of 3");


            }

            if (attempts == 3)
            {
                MessageBox.Show("Bye");
                Application.Current.Shutdown();
            }
        }

        public void Insert_audit()
        {
            string ip = GetLocalIPAddress();
            string pc = System.Environment.MachineName;
            var time = DateTime.Now;

            string connection = "server=localhost;port=3306;uid=" + user + ";pwd=" + acquired_pass + ";database=bond;charset=utf8;SslMode=none";

            using (MySqlConnection conn = new MySqlConnection(connection))
            {
                conn.Open();

                using (var cmd = new MySqlCommand("INSERT INTO audit(username, TS, details, machine_name, ip) VALUES (@username,@TS,@details,@machine,@ip)", conn))
                {
                    cmd.Parameters.AddWithValue("@username", user);
                    cmd.Parameters.AddWithValue("@TS", time);
                    cmd.Parameters.AddWithValue("@details", "user logged in");
                    cmd.Parameters.AddWithValue("@machine", pc);
                    cmd.Parameters.AddWithValue("@ip", ip);
                    cmd.ExecuteNonQuery();

                    conn.Close();
                }
            }
        }

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

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


    }
}
