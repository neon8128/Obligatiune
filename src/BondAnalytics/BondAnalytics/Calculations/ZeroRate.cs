using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondAnalytics.Calculations
{
    class ZeroRate
    {

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
        public double LinearInterpolation(DateTime AsOfDate, DateTime X2, List<Tuple<String, Int32, double>> _interestList)
        {
            Double y2 = 0;

            for (int i = 0; i < _interestList.Count - 1; i++)
            {
                var BeforeDate = AsOfDate.AddDays(_interestList[i].Item2);
                var AfterDate = AsOfDate.AddDays(_interestList[i+1].Item2);


                if (X2 > BeforeDate && X2 < AfterDate)
                {
                    y2 = (((X2 - BeforeDate).Days) * (_interestList[i+1].Item3 - _interestList[i].Item3)) / ((AfterDate - BeforeDate).Days) + _interestList[i].Item3;

                    return y2;
                }

            }
            return y2;
           
        }
      
    }
}
