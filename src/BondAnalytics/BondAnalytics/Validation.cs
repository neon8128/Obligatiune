using GalaSoft.MvvmLight;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BondAnalytics
{

    class Validation : ViewModelBase, IDataErrorInfo
    {
       

        public string Error => throw new NotImplementedException(); //auto-generated
    

        public String Name { get; set; }

        public String Ccy { get; set; }

        public String InterestRate { get; set; }

        public Int32 Principal { get; set; }

        public String StartDate { get; set; }

        public String EndDate { get; set; }

        public String DayCountingConvention { get; set; }
            

        public string this[String columnName] 
        {
            get
            {
                string result = null;

                if (columnName == "Name")
                {
                    if (string.IsNullOrEmpty(Name))
                    {
                        result = "Please enter a Name";
                    }
                }
                if (columnName == "Ccy")
                {
                    if (string.IsNullOrEmpty(Ccy) || Ccy.Length != 3)
                    {
                        result = "Please enter a valid currency";
                    }
                }
                if (columnName == "InterestRate")
                {
                    if (!double.TryParse(InterestRate, out double d))
                    {
                        result = "Please enter a valid interest rate amount";
                    }
                }
                if (columnName == "Principal")
                {

                    if (Principal < 1)
                    {
                        result = "Principal must  be >1";
                    }

                }

                if (columnName == "StartDate")
                {

                    if (!DateTime.TryParse(StartDate, out DateTime dt))
                    {
                        result = "Please enter a valid format for date";
                    }

                }
                if (columnName == "EndDate")
                {

                    if (!DateTime.TryParse(EndDate, out DateTime dt))
                    {
                        result = "Please enter a valid format for date";
                    }
                   else if(DateTime.Parse(EndDate) < DateTime.Parse(StartDate))
                    {
                        result = "EndDate should be greater than StartDate";
                    }  

                }

                if (columnName == "DayCountingConvention")
                {
                    if (string.IsNullOrEmpty(DayCountingConvention))
                    {
                        result = "Please choose a format";
                    }

                }
                return result;
            }
            
        }       
    }
}
