using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
    /// Interaction logic for InterestRate.xaml
    /// </summary>
    public partial class Interest_rate : Window
    {
        private string _user;
        MySqlConnection _db;
        private DataRowView row;
        public DataTable _dt = new DataTable();

        public Interest_rate()
        {
            InitializeComponent();
            _db = StaticDataManager.GetStaticDataManager().DBConnection;
        }

        public Interest_rate(string User):this()
        {
            this._user = User;
 
        }

        public Interest_rate(DataRowView row, string User):this(User)
        {
            this.row = row;
            Name.Text = row["name"].ToString();
            Version.Text = row["version"].ToString();
            AsOfDate.Text = row["as_of_date"].ToString();
            Rate.Text = row["rate"].ToString();
            Term.Text = row["term"].ToString();
            Ccy.Text = row["ccy"].ToString();
            Date.Text = row["date"].ToString();
            LoadList();
        }

        /// <summary>
        /// CLosing the app
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Exiting", System.Windows.MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Logging out of the app
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
        /// Changing pages 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int index = int.Parse(((Button)e.Source).Uid);

            //GridCursor.Margin = new Thickness(5 + (150 * index), 0, 0, 0);

            switch (index)
            {
                case 0:
                    Bond b = new Bond(_user);
                    this.Hide();
                    b.Show();
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
        /// Home button
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
                cmd.Parameters.AddWithValue("@details", "InterstRate");
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
        /// Insert in the InterestRate table
        /// </summary>
        /// <param name="AuditId"></param>
        public void InsertInterestRate(UInt64 AuditId)
        {
            MySqlCommand cmd = null;
            bool ok = false;
            String name = Name.Text;
            String asof = Convert.ToDateTime(AsOfDate.SelectedDate).ToString("yyyy/MM/dd");
            String date = Convert.ToDateTime(Date.SelectedDate).ToString("yyyy/MM/dd");
            String term = Term.Text;

            var getChanges = _dt.GetChanges();
            cmd = new MySqlCommand($"Select version from interest_rate where Name='{name}' and as_of_date = '{asof}' and date ='{date}' and term='{term}'", _db);
            Int32? version = (Int32?)cmd.ExecuteScalar();
            if (version == null)
            {
                version = 1;

            }
            else
            {
                version++;

            }
            cmd = new MySqlCommand($"DELETE FROM interest_rate where Name='{name}' and as_of_date = '{asof}' and date ='{date}' and term='{term}' ", _db);
            var x = cmd.ExecuteNonQuery();

            if (getChanges != null && !ok)
            {
                foreach(DataRow dr in getChanges.Rows)
                {
                    cmd = new MySqlCommand($"DELETE FROM interest_rate where Name='{name}' and as_of_date = '{dr["as_of_date"]}' and date ='{dr["date"]}' and term='{dr["term"]}' ", _db);
                    var xx = cmd.ExecuteNonQuery();
                    using (cmd = new MySqlCommand("INSERT INTO interest_rate(name, audit_id,rate,ccy,date,as_of_date,version,term) VALUES (@name,@audit_id,@rate,@ccy,@date,@as_of_date,@version,@term)", _db))
                    {
                        cmd.Parameters.AddWithValue("@name", Name.Text);
                        cmd.Parameters.AddWithValue("@audit_id", AuditId);
                        cmd.Parameters.AddWithValue("@rate", Double.Parse( dr["rate"].ToString() ));
                        cmd.Parameters.AddWithValue("@ccy", dr["ccy"].ToString() );
                        cmd.Parameters.AddWithValue("@as_of_date", dr["as_of_date"].ToString() );
                        cmd.Parameters.AddWithValue("@date", dr["date"].ToString());
                        cmd.Parameters.AddWithValue("@version", version);
                        cmd.Parameters.AddWithValue("@term", dr["term"].ToString());
                        cmd.ExecuteNonQuery();
                        
                    }
                   
                }
                ok = true;
            } // inca nu merge chiar cum trebuie

           else if (!ok)
            {
                using (cmd = new MySqlCommand("INSERT INTO interest_rate(name, audit_id,rate,ccy,date,as_of_date,version,term) VALUES (@name,@audit_id,@rate,@ccy,@date,@as_of_date,@version,@term)", _db))
                {
                    cmd.Parameters.AddWithValue("@name", Name.Text);
                    cmd.Parameters.AddWithValue("@audit_id", AuditId);
                    cmd.Parameters.AddWithValue("@rate", Double.Parse(Rate.Text));
                    cmd.Parameters.AddWithValue("@ccy", Ccy.Text);
                    cmd.Parameters.AddWithValue("@as_of_date", AsOfDate.SelectedDate);
                    cmd.Parameters.AddWithValue("@date", Date.SelectedDate);
                    cmd.Parameters.AddWithValue("@version", version);
                    cmd.Parameters.AddWithValue("@term", Term.Text);
                    cmd.ExecuteNonQuery();
                }
                ok = true;
            }

            Version.Text = version.ToString();

        }

        /// <summary>
        /// Insert in the interest_hist table records of InterestRate
        /// </summary>
        /// <param name="AuditId"></param>
        public void InsertInterestHist(UInt64 AuditId)
        {
            MySqlCommand cmd = null;
            String query = $"Insert into interest_rate_hist(name, audit_id, rate, ccy,date,as_of_date,version,term) select name, audit_id, rate, ccy,date,as_of_date,version,term from  interest_rate  where interest_rate.audit_id='{AuditId}' ";
            cmd = new MySqlCommand(query, _db);
            var xxx = cmd.ExecuteNonQuery();
            cmd.Dispose();

        }


        public void LoadList()
        {
            String asof = Convert.ToDateTime(AsOfDate.SelectedDate).ToString("yyyy/MM/dd");
            String date = Convert.ToDateTime(Date.SelectedDate).ToString("yyyy/MM/dd");
            MySqlCommand cmd = null;
            var query = $"Select rate,ccy,date,as_of_date,version,term from interest_rate  where Name='{Name.Text}' and as_of_date = '{asof}' and date ='{date}' and term='{Term.Text}' ";
            cmd = new MySqlCommand(query, _db);
            var dataAdapter = new MySqlDataAdapter(cmd);
            dataAdapter.Fill(_dt);
            dataAdapter.Dispose();
            cmd.Dispose();
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
                try
                {
                    DataRowView row = (DataRowView)_data.SelectedItem;
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
            DataRow dr = _dt.NewRow();
            var date = DateTime.Now.Date;
            dr["as_of_date"] = date;
            dr["date"] = date;
            dr["rate"] = Double.Parse(Rate.Text);
            dr["ccy"] = Ccy.Text;
            dr["term"] = Term.Text;
            _dt.Rows.Add(dr);

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
        ///     if  pressed it will insert in the audit and bond table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveClick(object sender, RoutedEventArgs e)
        {
           
            var dbTransaction = _db.BeginTransaction();

            try
            {
                var auditId = InsertAudit();
                InsertInterestRate(auditId);
                InsertInterestHist(auditId);
                dbTransaction.Commit();

                MessageBox.Show("The operation was succesful!");
            }
            catch (Exception f)
            {

                dbTransaction.Rollback();
                MessageBox.Show(f.ToString());
            }

            dbTransaction.Dispose();

        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var s = new SavedInterestRate(_user);
            s.Show();
            this.Hide();

        }
    }
}
