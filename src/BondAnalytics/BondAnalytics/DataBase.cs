using MySql.Data.MySqlClient;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BondAnalytics
{
    public  class DataBase
    {
       // private static  DataBase _instance = new DataBase();
        static string _connectionString = ConfigurationManager.ConnectionStrings["MyDB"].ConnectionString;
        static MySqlConnection _db;
        private DataBase()
        {
           
        }
        public static MySqlConnection Connection
        {
            get
            {
                if (_db == null)
                {
                     LazyInitializer.EnsureInitialized(ref _db, GetConnection);
                    
                }
                else if(_db.State != ConnectionState.Open)
                {
                    _db.Open();
                }
                return _db;
            }
        }       
        public static MySqlConnection GetConnection()
        {
            MySqlConnection db= new MySqlConnection(_connectionString);
             db.Open();
            return db;
        }      

    }     
    
}
