using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
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
        Int32 _attempts = 0;
        private String _user;
        private String _acquiredPass;
        MySqlConnection _db;

        public After_login()
        {
            InitializeComponent();
            _db = StaticDataManager.GetStaticDataManager().DBConnection;
        }

        public After_login(String User) : this()
        {
            this._user = User;                     
        }

        /// <summary>
        ///     Encrypting a string in a hash256 format
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        static string ComputeSha256Hash(String Data)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(Data));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        /// <summary>
        ///    if the password stored in the table users matches Pass return true; otherwise return false
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Pass"></param>
        /// <returns></returns>
        public bool Verify(string User, string Pass)
        {
                try
                {   
                   var sqlQuery = $"SELECT password FROM user where username='{User}'";
                   String testPass = null;
                   using (var cmd = new MySqlCommand(sqlQuery, _db))
                    {
                     testPass = cmd.ExecuteScalar() as String;
                    }
                    // if the password stored in database is equal to the one the user entered return true; false otherwise
                   if (testPass == Pass)
                    {
                        return true;
                    }
                   else
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message+"at afterlogin Verify");
                    return false;
                }
            //}
        }


        /// <summary>
        ///     if the conditions are met the MainWindow will appear; otherwise there will be 2 more chances to connect 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {          
            String newPass = ComputeSha256Hash(password.Password); //convert the pass to hash256
            if (Verify(_user, newPass))
            {
                MessageBox.Show("You have successfully logged in as " + _user);
                InsertAudit();
                this.Close();
                var r1 = new Real_main_window(_user);
                r1.Show();
            }
            else
            {
                _attempts++;
                MessageBox.Show("Try again!" + "\n" + "You have tried " + _attempts + " out of 3");
            }
            if (_attempts == 3)
            {
                MessageBox.Show("Bye");
                Application.Current.Shutdown();                
            }           
        }


        /// <summary>
        ///     A row will be inserted in the audit table in order to track user activity
        /// </summary>
        public void InsertAudit()
        {
            String ip = GetLocalIPAddress();
            String pc = System.Environment.MachineName;
            var time = DateTime.Now;
           // String connection = ConfigurationManager.ConnectionStrings["Bond"].ConnectionString;                      
               using (var cmd = new MySqlCommand("INSERT INTO audit(username, TS, details, machine_name, ip) VALUES (@username,@TS,@details,@machine,@ip)", _db))
                {
                    cmd.Parameters.AddWithValue("@username", _user);
                    cmd.Parameters.AddWithValue("@TS", time);
                    cmd.Parameters.AddWithValue("@details", "user logged in");
                    cmd.Parameters.AddWithValue("@machine", pc);
                    cmd.Parameters.AddWithValue("@ip", ip);
                    cmd.ExecuteNonQuery();                   
               }            
        }

        /// <summary>
        ///     Get ip address
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
        /// Closing the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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

        private void ForgotPass_Click(object sender, RoutedEventArgs e)
        {
            var f = new ForgotPass(_user);
            f.Show();
            this.Hide();

        }
    }
}
