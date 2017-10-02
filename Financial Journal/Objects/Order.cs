using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financial_Journal
{
    public class Order
    {
        public string Location { get; set; }
        public string OrderID { get; set; }
        public DateTime Date { get; set; }

        public string OrderMemo { get; set; }
        public string OrderSerial { get; set; }
        public string Payment_Type { get; set; }
        public double GC_Amount { get; set; }
        public double Order_Total_Pre_Tax { get; set; }
        public double Order_Taxes { get; set; }
        public double Order_Discount_Amt { get; set; }
        public bool Tax_Overridden { get; set; }
        public int Order_Quantity { get; set; }
        public bool IUO_IsSynced { get; set; }

        public Order()
        {
            IUO_IsSynced = false;
        }

        public Order Copy_Item()
        {
            return System.MemberwiseClone.Copy(this);
        }
    }
}
