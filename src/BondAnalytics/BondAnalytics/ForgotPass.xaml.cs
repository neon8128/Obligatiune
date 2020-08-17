using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for ForgotPass.xaml
    /// </summary>
    public partial class ForgotPass : Window
    {
        private string _user;
        MySqlConnection _db = StaticDataManager.GetStaticDataManager().DBConnection;
        Int16 _attempts = 0;

        public ForgotPass()
        {
            InitializeComponent();
        }

        public ForgotPass(string User):this()
        {           
            this._user = User;
            
        }

        /// <summary>
        /// Closing the application
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

        /// <summary>
        /// Encrypting a string in a hash256 format
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
        /// Change pass if button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var dbTransaction = _db.BeginTransaction();
            try
            {
                InsertUser();
                var x = 0;

                var t = 1 / x;
                InsertAudit();
                InsertUserHist();
                dbTransaction.Commit();
                MessageBox.Show("Password has been changed");
                var a = new After_login(_user);
                a.Show();
                this.Hide();
            }
            catch
            {
                dbTransaction.Rollback();
                MessageBox.Show("An error has occured");
            }
            dbTransaction.Dispose();
            

        }

        /// <summary>
        /// Insert in the user table 
        /// </summary>
        public void InsertUser()
        {
            MySqlCommand cmd = null;
            String passtxt = pass.Password; // storing in a string the password from the passbox
            String againtxt = again.Password;
            if (passtxt == againtxt)
            {
                try
                {
                    cmd = new MySqlCommand($"Select version from user where username='{_user}'", _db);
                    int version = (int)cmd.ExecuteScalar();                   
                    version++;          
                    
                    String encrypted = ComputeSha256Hash(passtxt);
                    String query = $"Update user set password='{encrypted}',version='{version}' where username='{_user}' ";
                    cmd = new MySqlCommand(query, _db);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.ToString());
                }               
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
        /// Insert in the audit table
        /// </summary>
        public void InsertAudit()
        {
            string ip = GetLocalIPAddress();
            string pc = System.Environment.MachineName;
            DateTime time = DateTime.Now;
            try
            {
                using ( var cmd = new MySqlCommand("INSERT INTO audit(username, TS, details, machine_name, ip) VALUES (@username,@TS,@details,@machine,@ip)", _db))
                {
                    cmd.Parameters.AddWithValue("@username", _user);
                    cmd.Parameters.AddWithValue("@TS", time);
                    cmd.Parameters.AddWithValue("@details", "User password has been changed");
                    cmd.Parameters.AddWithValue("@machine", pc);
                    cmd.Parameters.AddWithValue("@ip", ip);
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }                        
        }

        /// <summary>
        /// Insert data from user table to userhist 
        /// </summary>
        public void InsertUserHist()
        {
            MySqlCommand cmd = null;
            try
            {
                String query = $"Insert into user_hist(version,username,password,first_name,last_name,email)  select version,username,password,first_name,last_name,email from user where username='{_user}'";
                cmd = new MySqlCommand(query, _db);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
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


    }
}
