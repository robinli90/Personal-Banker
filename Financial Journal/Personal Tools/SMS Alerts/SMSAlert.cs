using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financial_Journal
{
    public class SMSAlert
    {
        public string Name { get; set; }
        public DateTime Time { get; set; }
        public bool Repeat { get; set; }
        public bool IUO_Flag { get; set; }
    }
}