using CredentialManagement;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;

namespace BondAnalytics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int _i = 0;
        bool _ok = false;
        
        public MainWindow()
        {
            InitializeComponent();
            SizeToContent = System.Windows.SizeToContent.Manual;

            PopulateDBConnNames();
        }

        private void PopulateDBConnNames()
        {
            foreach( ConnectionStringSettings connString in ConfigurationManager.ConnectionStrings)
            {
                if ( connString.ProviderName.Equals("MySql.Data.MySqlClient", StringComparison.OrdinalIgnoreCase) )
                {
                    _dataBaseCombo.Items.Add(connString.Name);
                }
            }
            _dataBaseCombo.Text = _dataBaseCombo.Items[0].ToString();
        }

        /// <summary>
        ///     This way the Window is draggable
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        /// <summary>
        ///     Verifying if it is possible to connect with given username and password; otherwise throw exception
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public bool Connect(string User, string Pass)
        {
            var connName = _dataBaseCombo.SelectionBoxItem.ToString();
            var connString = GetConfig( connName, User, Pass);
           
            try
            {
                var con = new MySqlConnection(connString);
                con.Open();

                var staticDataMgr = StaticDataManager.GetStaticDataManager();
                staticDataMgr.DBConnection = con;

                return true;
                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        /// <summary>
        /// Trying to connect to the db
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            String DB = _dataBaseCombo.SelectionBoxItem.ToString();            
            String acquired_pass = GetCredential(DB, user.Text); //Get user credentials           
            //  Debug.Assert(GetCredential("Bond_calculator") == null);
            if (acquired_pass != null) // if a password is saved in CredentialManager
            {
                MessageBox.Show("Your password was saved in WCM, let me grab it");
                
                Connect(user.Text, acquired_pass);
                
                MessageBox.Show("HI " + user.Text + "!" + Environment.NewLine + "Now introduce your password for the app");
                this.Hide();
                var afterLogin = new After_login(user.Text); //Go to the Login App page
                afterLogin.Show();
                _ok = true;
            }
            else if (Connect(user.Text,pass.Password)) // if the password stored in the user table matches the pass from passwordbox go; otherwise retry 
            {
               
               MessageBox.Show("HI " + user.Text + "!" + Environment.NewLine + "Now introduce your password for the app");
               this.Hide();
               var after_Login = new After_login(user.Text);
               after_Login.Show();
               SetCredentials(DB, user.Text, pass.Password, PersistanceType.LocalComputer);
               _ok = true;

            }               
            if (_ok == false)   // if login failed try again for 3 times max.
            {
                _i++;
                MessageBox.Show("Try again!" + "\n" + "You have tried " + _i + " out of 3");
                //this.Close();
            }          
            if (_i >= 3)
            {
                MessageBox.Show("Bye");
                Application.Current.Shutdown();
            }                       
        }


        /// <summary>
        ///     username and password will be saved in the CredentialManager 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="persistenceType"></param>
        /// <returns></returns>
        public static bool SetCredentials(string target, string username, string password, PersistanceType persistenceType)
        {

            return new Credential
            {
                Target = target,
                Username = username,
                Password = password,
                PersistanceType = persistenceType
            }.Save();
        }
     
        /// <summary>
       /// Retrieve credentials from CredentialManager
       /// </summary>
       /// <param name="target"></param>
       /// <param name="user"></param>
       /// <returns></returns>
        public static string GetCredential(string target, string user)
        {
            var cm = new Credential
            {
                Target = target,
                Username = user
            };
            if (!cm.Load())
            {
                return null;
            }

            if (user == cm.Username)
                return cm.Password;
            else return null;
        }


        /// <summary>
        /// Shutting down the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }



        /// <summary>
        /// Get the configuration string used to connect to the database
        /// </summary>
        /// <param name="ConnName"></param>
        /// <param name="User"></param>
        /// <param name="Pass"></param>
        /// <returns></returns>
        public String GetConfig(String ConnName, String User, String Pass)
        {
            var connString = "";
            try
            {
                connString = ConfigurationManager.ConnectionStrings[ConnName].ConnectionString;
                connString += $"user='{User}';password='{Pass}'";
            }
            catch(Exception e)
            {
                MessageBox.Show("Select a database to connect to");
            }

            return connString;
        }

            
    }

   
}
