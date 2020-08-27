using GalaSoft.MvvmLight.Messaging;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Globalization;
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
       Dictionary<Int32, Tuple<String,Int32, Double>> _period = new Dictionary<Int32, Tuple<String,Int32, Double>>();


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

            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Exiting", System.Windows.MessageBoxButton.YesNo);

            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
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
            // _dt.Rows[0]["no_days"] = 12;
            // return;

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
            catch (Exception f)
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

        /// <summary>
        /// Insert in the bond_hist table records of bond
        /// </summary>
        /// <param name="AuditId"></param>
        public void InsertBondHist(UInt64 AuditId)
        {
            MySqlCommand cmd = null;
            String query = $"Insert into bond_hist(name,audit_id,version,interest_rate,ccy,principal,day_counting_convention,start_date,end_date) select name,audit_id,version,interest_rate,ccy,principal,day_counting_convention,start_date,end_date from  bond b1 where b1.audit_id='{AuditId}' ";
            cmd = new MySqlCommand(query, _db);
            var xxx = cmd.ExecuteNonQuery();
            cmd.Dispose();

        }

        /// <summary>
        /// Insert in the schedule table
        /// </summary>
        public void InsertSchedule()
        {
            // Schedule item = Data.SelectedItem as Schedule;

          //  var changesDT = _dt.GetChanges();


            // if (changesDT == null)
            // {
            //     return;
            // }
            // else
            // {
            //   foreach(DataRow dr in changesDT.Rows)
            //   {
            //      var noDays = (Int32)( DateTime.Parse(dr["ref_day"].ToString()) - StartPick.SelectedDate.Value).Days;
            //   }
            // }
            // todo: check values for null when computing noDays

            MySqlCommand cmd = null;

            // todo: you need to save all the records, not only one

            // var q = $"Select version from bond where name = '{(String)changesDT.Rows[0]["bond_name"]}'";
            // cmd = new MySqlCommand(q, _db);
            // Int32 version = (Int32)cmd.ExecuteScalar();

            var q = $"Delete  from schedule where bond_name ='{Name.Text}'";
            cmd = new MySqlCommand(q, _db);
            var c = cmd.ExecuteNonQuery();

            foreach (DataRow dr in _dt.Rows)
            {
                if (dr["no_days"] == null)
                {
                    return;
                }

                cmd = new MySqlCommand("Insert into schedule (bond_name,ref_day,date_coupon,bond_version,no_days,principal) values (@bond_name,@ref_day,@date_coupon,@bond_version,@no_days,@principal)", _db);
                cmd.Parameters.AddWithValue("@bond_name", Name.Text);
                cmd.Parameters.AddWithValue("@ref_day", dr["ref_day"]);
                cmd.Parameters.AddWithValue("@date_coupon", dr["date_coupon"]);
                cmd.Parameters.AddWithValue("@bond_version", Int32.Parse(Version.Text));
                cmd.Parameters.AddWithValue("@no_days", dr["no_days"]);
                cmd.Parameters.AddWithValue("@principal", dr["principal"]);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
            /*  */

            /*}*/
        }

        public void InsertScheduleHist()
        {
            MySqlCommand cmd = null;

            cmd = new MySqlCommand("Insert into schedule_hist (bond_name,ref_day,date_coupon,bond_version,no_days,principal) select bond_name,ref_day,date_coupon,bond_version,no_days,principal from schedule", _db);
            var c = cmd.ExecuteNonQuery();
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
            var query = $"Select bond_name, ref_day,date_coupon,no_days,bond_version, principal from schedule where bond_name='{Name.Text}'";
            var cmd = new MySqlCommand(query, _db);

            var dbAdapter = new MySqlDataAdapter(cmd);

            dbAdapter.Fill(_dt);

            dbAdapter.Dispose();
            cmd.Dispose();

            _dt.Columns.Remove("bond_name");
            _dt.Columns.Remove("bond_version");

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
                DataRowView row = (DataRowView)_data.SelectedItem;
                try
                {
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
            var ref_day = DateTime.Now.Date;
            var nodays = (ref_day - StartPick.SelectedDate.Value).Days;
            dr["ref_day"] = ref_day;
            dr["date_coupon"] = ref_day;
            dr["no_days"] = nodays;
            dr["principal"] = Int32.Parse(Principal.Text);
            _dt.Rows.Add(dr);

        }
       
       
        /// <summary>
        /// Add row by pressing + or delete selected row by pressing del
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _data_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Delete Confirmation", System.Windows.MessageBoxButton.YesNo);

                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    DataRowView row = (DataRowView)_data.SelectedItem;
                    e.Handled = true;
                    try
                    {
                        _dt.Rows.Remove(row.Row);
                    }
                    catch
                    {
                        MessageBox.Show("Please select a row ");
                    }
                }

            }
            else if (e.Key == Key.OemPlus || e.Key == Key.Add)
            {
                DataRow dr = _dt.NewRow();
                var ref_day = DateTime.Now.Date;
                var nodays = (ref_day - StartPick.SelectedDate.Value).Days;
                dr["ref_day"] = ref_day;
                dr["date_coupon"] = ref_day;
                dr["no_days"] = nodays;
                dr["principal"] = Int32.Parse(Principal.Text);
                _dt.Rows.Add(dr);
            }
        }

        private void _data_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {

                e.Column = new DataGridDateTimeColumn((DataGridBoundColumn)e.Column);
            }
        }

        internal class DataGridDateTimeColumn : DataGridBoundColumn
        {
            public DataGridDateTimeColumn(DataGridBoundColumn column)
            {
                Header = column.Header;
                Binding = (Binding)column.Binding;
            }

            protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
            {
                var control = new TextBlock();
                BindingOperations.SetBinding(control, TextBlock.TextProperty, Binding);
                return control;
            }

            protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
            {
                var control = new DatePicker();
               // control.PreviewKeyDown += Control_PreviewKeyDown;
                BindingOperations.SetBinding(control, DatePicker.SelectedDateProperty, Binding);
                BindingOperations.SetBinding(control, DatePicker.DisplayDateProperty, Binding);
                return control;
            }

           // private void Control_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
           // {
           //     if (e.Key == System.Windows.Input.Key.Return)
           //     {
           //         DataGridOwner.CommitEdit();
           //     }
           // }
        }

        /// <summary>
        /// Implementing linear interpolation with 2 given points with the respective coordinates 
        /// and the abscissa of a third point
        /// </summary>
        /// <param name="X1"></param>
        /// <param name="X2"></param>
        /// <param name="X3"></param>
        /// <param name="Y1"></param>
        /// <param name="Y3"></param>
        /// <returns></returns>
        public double LinearInterpolation(DateTime X1, DateTime X2,DateTime X3, Double Y1,Double Y3)
        {
            double y2 = 0;

            y2 = (((X2 - X1).Days) * (Y3 - Y1)) / ((X3 - X1).Days) + Y1;


            return y2;
        }

        public int MyListComparison(Tuple<String, Int32, double> x, Tuple<String, Int32, double> y)
        {
            return x.Item2 - y.Item2;
        }

        /// <summary>
        /// Fill the dictionary with a string as a key (O/N,1W,1M, etc..) and a pair <Int32,double> 
        /// where Int32 = no days and double  = interestRate
        /// </summary>
        public void FillDictionary()
        {

            var list = new List<Tuple<String, Int32, double>>();
            list.Add( new Tuple<String, Int32, double>("O/N", 1, 0.5));
            list.Add(new Tuple<String, Int32, double>("10Y", 10*365, 10));
            list.Add(new Tuple<String, Int32, double>("1W", 7, 0.7));
            list.Add(new Tuple<String, Int32, double>("2Y", 2 * 365, 2));

            //var compFunc = new Comparison<Tuple<String, Int32, double>>(MyListComparison);

            //list.Sort(compFunc);

            list.Sort( (a,b) => { return a.Item2 - a.Item2; } );

            try
            {
                var query = $"Select term, rate from interest_rate where name='{NameInterest.Text}'";
                var cmd = new MySqlCommand(query, _db);
                var reader = cmd.ExecuteReader();
                Int32 i = 0;
                while (reader.Read())
                {
                    String String = reader[0].ToString();                   
                    switch (String)
                    {
                        case "O/N":
                            
                            _period.Add(i++, new Tuple<String,Int32, double>("O/N", 1, Double.Parse(reader[1].ToString())));
                            break;

                        case "T/N":
                            
                            _period.Add(i++, new Tuple<String, Int32, double>("T/N", 2, Double.Parse(reader[1].ToString())));
                            break;

                        case "1W":
                            
                            _period.Add(i++, new Tuple<String, Int32, double>("1W", 7, Double.Parse(reader[1].ToString())));
                            break;

                        case "2W":
                            
                            _period.Add(i++, new Tuple<String, Int32, double>("2W", 14, Double.Parse(reader[1].ToString())));
                            break;

                        case "1M":
                           
                            _period.Add(i++, new Tuple<String, Int32, double>("1M", 30, Double.Parse(reader[1].ToString())));
                            break;

                        case "3M":
                            
                            _period.Add(i++, new Tuple<String, Int32, double>("3M", 90, Double.Parse(reader[1].ToString())));
                            break;

                        case "1Y":
                            
                            _period.Add(i++, new Tuple<String, Int32, double>("1Y", 360, Double.Parse(reader[1].ToString())));
                            break;

                        case "2Y":
                           
                            _period.Add(i++, new Tuple<String, Int32, double>("2Y", 720, Double.Parse(reader[1].ToString())));
                            break;

                        default:
                           
                            break;
                    }
                }
                reader.Dispose();
                cmd.Dispose();
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }

        public Double GetInterestRate()
        {
            if(_period.Count == 0)
            {
                FillDictionary(); // fill the _period dictionary
            }
            
            var asofdate = (DateTime)AsOfDate.SelectedDate;
            var start = (DateTime)StartPick.SelectedDate;
            Double rate = 0;

            //number of days between startDate and a given date
            var nodays = (Int32)(asofdate - start).Days;

            IEnumerator enumerator = _period.Keys.GetEnumerator();
            enumerator.MoveNext();
            if (_period.Count > 0) // if the dictionary has values

            {
                for (Int32 i=0; i<=_period.Count;i++) // iterate through dictionary
                {
                    Int32 j = i + 1;
                    if (nodays == _period[i].Item2) // if nodays is equal to period from interestTable 
                    {
                        rate = _period[i].Item3;
                        Rate.Text = _period[i].Item3.ToString();// write it in Rate Textbox
                    }
                    else if(_period.Count == 1)
                    {
                        MessageBox.Show("We need more values!");
                        break;
                    }
                    else
                    {                        
                       
                        if (asofdate > start.AddDays(_period[i].Item2) && asofdate < start.AddDays(_period[j].Item2))
                        {
                            rate = LinearInterpolation(start.AddDays(_period[i].Item2), asofdate, start.AddDays(_period[j].Item2), _period[i].Item3, _period[j].Item3);
                            Rate.Text = rate.ToString();
                            return rate;
                        }

                        else if (asofdate < start.AddDays(_period[i].Item2) && asofdate > start.AddDays(_period[j].Item2))
                        {
                            rate = LinearInterpolation(start.AddDays(_period[j].Item2), asofdate, start.AddDays(_period[i].Item2), _period[j].Item3, _period[i].Item3);
                            Rate.Text = rate.ToString();
                            return rate;
                            
                        }                        
                    }
                 }
            }
            return rate;
        }
        private void GetPrice_Click(object sender, RoutedEventArgs e)
        {
            //DateTime X1 = DateTime.ParseExact("14/08/2020", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //DateTime X2 = DateTime.ParseExact("07/09/2020", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            //DateTime X3 = DateTime.ParseExact("14/10/2020", "dd/MM/yyyy", CultureInfo.InvariantCulture);

            // Double y = LinearInterpolation(X1, X2, X3,0.1, 0.2);
            //Price.Text = y.ToString();

            double y = GetInterestRate() / 100;
            Double principal = Double.Parse(Principal.Text);
            Double price = principal + (1 + y * principal);
            Price.Text = price.ToString();
            

        }
    }
}
