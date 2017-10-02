using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financial_Journal
{
    public static class Diagnostics
    {
        public static void WriteLine(string str, string str2 = "")
        {
            if (str.Length > 0)
            Console.WriteLine(String.Format("[{0}:{1}:{2}:{3}] - {4}",
                DateTime.Now.Hour,
                DateTime.Now.Minute,
                DateTime.Now.Second,
                DateTime.Now.Millisecond,
                str));
        }
    }
}
