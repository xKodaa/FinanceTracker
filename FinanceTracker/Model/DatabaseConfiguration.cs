using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Model
{
    // Třída pro uložení objektů z konfiguračního souboru projektu
    public class DatabaseConfiguration
    {
        public string ConnectionString { get; set; }
        private DatabaseConfiguration()
        {
            ConnectionString = string.Empty;
        }
    }
}
