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
    public class StaticDataManager
    {   
        public MySqlConnection DBConnection { get; set; }

        public String DBSchema { get; set; }

        private static StaticDataManager _instance = null;

        private StaticDataManager()
        {
           
        }

        /// <summary>
        /// Returns the singleton instance - warning, this is not thread safe
        /// </summary>
        /// <returns></returns>
        public static StaticDataManager GetStaticDataManager()
        {
            if( _instance == null )
            {
                _instance = new StaticDataManager();
            }

            return _instance;
        }
    }     
    
}
