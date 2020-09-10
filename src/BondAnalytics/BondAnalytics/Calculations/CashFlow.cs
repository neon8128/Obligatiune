using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondAnalytics.Calculations
{
    class CashFlow
    {

        /// <summary>
        /// Get cashflow using cashflow formula
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="Finish"></param>
        /// <param name="Principal"></param>
        /// <param name="InterestRate"></param>
        /// <returns></returns>
        public double GetDiscoutendCashFlow(DateTime Start, DateTime Finish, Double Principal, Double InterestRate)
        {
            var years = (Double)((Finish - Start).Days) / 365; //number of years between start and finish
            var interest = Principal * years * (InterestRate / 100);
            return interest ; // applying cashflow formula
        }

        public Double GetCurrentCashFlow(DateTime Today, DateTime Finish, Double DiscountedCash, Double InterestRate) 
        {
            var years = (Double)((Finish - Today).Days) / 365; //number of years between start and finish
            var sum = (Double)DiscountedCash / (1 + (InterestRate / 100) * years);
            return sum;

        }

    }
}
