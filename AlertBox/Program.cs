using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlertBox
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                // Application
                if (args.Length == 4)
                    Application.Run(new Alert_Box(args[0], args[1], args[2], args[3]));
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.ToString());
            }



            
        }
    }
}
