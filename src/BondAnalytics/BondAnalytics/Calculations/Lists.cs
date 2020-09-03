using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace BondAnalytics.Calculations
{
   public class Lists
    {
         public  List<Tuple<String, Int32, double>> _interestList = new List<Tuple<String, Int32, double>>();
        
        public List<Tuple<DateTime, DateTime>> _scheduleList = new List<Tuple<DateTime, DateTime>>();
        
        MySqlConnection _db= StaticDataManager.GetStaticDataManager().DBConnection;




        /// <summary>
        ///  Fill the list with a string as a key (O/N,1W,1M, etc..) and a pair <Int32,double> 
        /// where Int32 = no days and double  = interestRate
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Ccy"></param>
        /// <param name="AsOf"></param>
        /// <returns></returns>
        public List<Tuple<String, Int32, double>> GetInterestList(String Name, String Ccy, DateTime AsOf)
        {
            var asofStr = $"STR_TO_DATE('{AsOf.ToString("dd/MM/yyyy")}', '%d/%m/%Y')";
            
          
              var query = $"Select term, rate from interest_rate where name='{Name}'and ccy='{Ccy}' and as_of_date={asofStr}";
              var cmd = new MySqlCommand(query, _db);
              var reader = cmd.ExecuteReader();
              Int32 i = 0;
              while (reader.Read())
              {
                  String String = reader[0].ToString();
                  switch (String)
                  {
                      case "O/N":

                          _interestList.Add(new Tuple<String, Int32, double>("O/N", 1, reader.GetDouble(1)));
                          break;

                      case "T/N":

                          _interestList.Add(new Tuple<String, Int32, double>("T/N", 2, reader.GetDouble(1)));
                          break;

                      case "1W":

                          _interestList.Add(new Tuple<String, Int32, double>("1W", 7, reader.GetDouble(1)));
                          break;

                      case "2W":

                          _interestList.Add(new Tuple<String, Int32, double>("2W", 14, reader.GetDouble(1)));
                          break;

                      case "1M":

                          _interestList.Add(new Tuple<String, Int32, double>("1M", 30, reader.GetDouble(1)));
                          break;

                      case "3M":

                          _interestList.Add(new Tuple<String, Int32, double>("3M", 90, reader.GetDouble(1)));
                          break;

                      case "1Y":

                          _interestList.Add(new Tuple<String, Int32, double>("1Y", 360, reader.GetDouble(1)));
                          break;

                      case "2Y":

                          _interestList.Add(new Tuple<String, Int32, double>("2Y", 720, reader.GetDouble(1)));
                          break;

                      default:

                          break;
                  }
              }
              reader.Dispose();
              cmd.Dispose();
              _interestList.Sort((a, b) => { return a.Item2 - b.Item2; }); // sorting the list
            
          
            return _interestList;
        }


        /// <summary>
        /// Get schedule list from the schedule table
        /// </summary>
        /// <param name="BondName"></param>
        /// <returns></returns>
        public List<Tuple<DateTime, DateTime>> GetScheduleList(String BondName)
        {
            try
            {
                var query = $"Select ref_day,date_coupon from schedule where bond_name ='{BondName}'";
                var cmd = new MySqlCommand(query, _db);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    _scheduleList.Add(new Tuple<DateTime, DateTime>(reader.GetDateTime(0), reader.GetDateTime(1)));

                }
                reader.Dispose();
                _scheduleList.Sort((a, b) => { return (a.Item1 - b.Item1).Days; });
                
            }
            catch(Exception e)
            {
                // do nothing for now
            }
            return _scheduleList;
        }



        
    }
}
