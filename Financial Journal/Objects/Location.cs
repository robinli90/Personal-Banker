using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financial_Journal
{
    public class Location
    {
        public string Name { get; set; }
        public int Refund_Days { get; set; }

        public Location()
        {

        }

        public override string ToString()
        {
            return Name;
        }
    }
}
