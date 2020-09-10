using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BondAnalytics.ExchangeRateData
{
   public class ExchangeTable
    {
        public List<Tuple<String, Double>> _exchangeData = new List<Tuple<string, double>>();
        
        /// <summary>
        ///  Get exchange data from cursbnr.ro 
        ///   only from foreign ccy -> ron 
        /// </summary>
        public List<Tuple<String, Double>> GetExchangeRate()
        {

            var url = "https://www.cursbnr.ro/";
            var web = new HtmlAgilityPack.HtmlWeb();
            var doc = web.Load(url);
      
            for (int i = 1; i <=32; i++)
            {
                try
                {
                    var xccy = doc.DocumentNode.SelectNodes($"//*[@id=\"table-currencies\"]/tbody/tr[{i}]/td[1]"); // xpath for ccy
                    var xrate = doc.DocumentNode.SelectNodes($"//*[@id=\"table-currencies\"]/tbody/tr[{i}]/td[3]"); // xpath for rate
                    var ccy = xccy[0].InnerHtml; //converting to string
                    var rate = Double.Parse(xrate[0].InnerHtml);   // converting to double    
                    _exchangeData.Add(new Tuple<String, Double>(ccy, rate));                   
                }
                catch(Exception e)
                {
                    // do nothing
                }                
            }
            return _exchangeData;
        }
    }
}
