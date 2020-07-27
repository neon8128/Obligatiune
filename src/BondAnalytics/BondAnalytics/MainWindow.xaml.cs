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

namespace BondAnalytics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int i = 0;
        bool ok = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        public bool Connect(string a, string b)
        {
            string connection = "server=localhost;port=3306;uid=" + a + ";pwd=" + b + ";database=bond;charset=utf8;SslMode=none";

            try
            {
                using (MySqlConnection con = new MySqlConnection(connection))
                {
                    con.Open();

                    return true;
                }
            }
            catch (Exception e)
            {

                return false;
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string acquired_pass = GetCredential("Bond_calculator", user.Text);

            //  Debug.Assert(GetCredential("Bond_calculator") == null);

            if (acquired_pass != null)
            {
                MessageBox.Show("Your password was saved in WCM, let me grab it");
                MessageBox.Show("HI " + user.Text + "!" + Environment.NewLine + "Now introduce your password for the app");
                this.Hide();
                After_login after_Login = new After_login(user.Text, acquired_pass);
                after_Login.Show();
                ok = true;

            }
            if (Connect(user.Text, pass.Password))
            {
                MessageBox.Show("HI " + user.Text + "!" + Environment.NewLine + "Now introduce your password for the app");
                this.Hide();
                var after_Login = new After_login(user.Text, pass.Password.ToString());
                after_Login.Show();
                SetCredentials("Bond_calculator", user.Text, pass.Password, PersistanceType.LocalComputer);
                ok = true;

            }
            else
            {

                if (ok == false)
                {
                    i++;
                    MessageBox.Show("Try again!" + "\n" + "You have tried " + i + " out of 3");
                    this.Close();
                }



            }

            if (i >= 3)
            {
                MessageBox.Show("Bye");
                Application.Current.Shutdown();
            }
        }

        private void SetCredentials(string v, string text, string password, object localComputer)
        {
            throw new NotImplementedException();
        }

        private void c1_Checked(object sender, RoutedEventArgs e)
        {

            // if (c1.IsChecked ?? false)
            // {
            //     MessageBox.Show(pass_text.Password);
            // }
        }
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

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
