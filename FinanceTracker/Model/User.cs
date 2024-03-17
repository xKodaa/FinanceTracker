using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceTracker.Model
{
    public class User
    {
        public string Username { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }

        
        public User(string username)
        {
            Username = username;
        }

        public override string ToString()
        {
            if (Name != null && Surname != null)
            {
                return $"{Name} {Surname}";
            }   
            return Username;
        }
    }
}
