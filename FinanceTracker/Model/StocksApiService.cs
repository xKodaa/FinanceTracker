using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Model
{
    public class StocksApiService
    {
        private static readonly string API_KEY = "VY6WUPN7ZWBJRRTK";
        static readonly HttpClient client = new HttpClient();

        public StocksApiService() { }
    }
}
