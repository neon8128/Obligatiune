using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondAnalytics
{

    class Validate : IDataErrorInfo
    {
       

        public string Error => throw new NotImplementedException(); //auto-generated
    

        public String _name { get; set; }

        public String _ccy { get; set; }

        public String _interestRate { get; set; }

        public Int32 _principal { get; set; }

        public String _startDate { get; set; }

        public String _endDate { get; set; }

        

        public string this[string columnName] 
        {
            get
            {
                string result = null;
                if (columnName == "_name") // if colomnName == xaml Type binding ;check the Name textbox
                {
                    if (string.IsNullOrEmpty(_name) || _name.Length < 3)
                        result = "Please enter a Name";
                }
                if (columnName == "_ccy")
                {
                    if (string.IsNullOrEmpty(_ccy) || _ccy.Length > 3) 
                        result = "Please enter a valid currency";
                }
                if (columnName == "_interestRate")
                { 
                    if (!double.TryParse(_interestRate, out double  d)) // check if Interest Rate is double
                        result = "Please enter a valid interest rate amount";
                }
                if(columnName == "_principal") 
                {

                    if (_principal<1)
                    {
                        result = "Principal must  be >1";
                    }
                }

                if(columnName == "_startDate")
                {
                   
                    if (!DateTime.TryParse(_startDate,out DateTime dt) )
                    {
                        result = "Please enter a valid format for date";
                    }
                }
                if (columnName == "_endDate")
                {

                    if (!DateTime.TryParse(_endDate, out DateTime dt))
                    {
                        result = "Please enter a valid format for date";
                    }
                }

                return result;
            }
        }
    }
}
