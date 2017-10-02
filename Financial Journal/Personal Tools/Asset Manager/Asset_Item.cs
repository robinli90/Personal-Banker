using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financial_Journal
{
    public class Asset_Item
    {
        public string Name { get; set; }
        public DateTime Purchase_Date { get; set; }
        public DateTime Remove_Date { get; set; }
        public double Cost { get; set; }
        public string Serial_Identification { get; set; }
        public string Note { get; set; }
        public string Asset_Category { get; set; }
        public double Selling_Amount { get; set; }
        public string Purchase_Location { get; set; }
        public string OrderID { get; set; }

        public string Item_category_IUO { get; set; }

        public Asset_Item(DateTime Remove_Date_ = new DateTime())
        {
            this.Remove_Date = Remove_Date_;
            this.OrderID = "";
            this.Selling_Amount = 0;
        }

        public override string ToString()
        {
            return Name + ", " + Asset_Category + ", " + Purchase_Location;
        }
    }
}
