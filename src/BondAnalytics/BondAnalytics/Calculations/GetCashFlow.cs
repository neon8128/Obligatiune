﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondAnalytics.Calculations
{
    class GetCashFlow
    {

        /// <summary>
        /// Get cashflow using cashflow formula
        /// </summary>
        /// <param name="Start"></param>
        /// <param name="Finish"></param>
        /// <param name="Principal"></param>
        /// <param name="InterestRate"></param>
        /// <returns></returns>
        public double GetCash(DateTime Start, DateTime Finish, Double Principal, Double InterestRate)
        {
            var years = (Double)((Finish - Start).Days) / 365; //number of years between start and finish
            return Principal * (1 + InterestRate/100 * years); // applying cashflow formula
        }


    }
}