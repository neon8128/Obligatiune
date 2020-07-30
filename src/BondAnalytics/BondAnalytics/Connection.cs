using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondAnalytics
{
   public class Connection
    {
        public String _name;
        public String _dataSource;
        public String _username;
        public String _password;
        public String _server;



         public Connection()
        {
           
        }




        /// <summary>
        /// Constructor with the needed settings
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="DataSource"></param>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <param name="Server"></param>
        public Connection(String Name,String DataSource, String Username, String Password, String Server)
        {
            _name = Name;
            _dataSource = DataSource;
            _username = Username;
            _password = Password;
            _server = Server;
        }



       
    }
}
