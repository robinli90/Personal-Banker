using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financial_Journal
{
    public class Item
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string OrderID { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public string Payment_Type { get; set; }
        public string Memo { get; set; }
        public double Discount_Amt { get; set; }
        public bool RefundAlert { get; set; }
        public int consumedStatus { get; set; } // 2=regular, 1=pendingPurchase, 0=complete (for shopping list)
        public DateTime Date { get; set; }
        public DateTime Refund_Date { get; set; }


        public override string ToString()
        {
            return Name + " ($" + String.Format("{0:0.00}", Price) + ")";
        }

        public Item()
        {
            consumedStatus = 2;
        }

        public Item Copy_Item()
        {
            return System.MemberwiseClone.Copy(this);
        }

        /// <summary>
        /// return amount less refunds less discounts
        /// </summary>
        /// <param name="tax_rate"></param>
        /// <returns></returns>
        public double Get_Current_Amount(double tax_rate, bool ignoreDiscounts = false)
        {
            return Price * Get_Current_Quantity() * (1 + tax_rate) // Base price with tax : adjust quantity to refunded
                - (ignoreDiscounts ? 0 : Get_Current_Discount());// Less total discount : adjust refunded
        }

        /// <summary>
        /// Return the total discounted amount currentyl
        /// </summary>
        /// <returns></returns>
        public double Get_Current_Discount()
        {
            return (Discount_Amt / Quantity) * Get_Current_Quantity();
        }

        /// <summary>
        /// Return current item count
        /// </summary>
        /// <returns></returns>
        public int Get_Current_Quantity()
        {
            return (Quantity - Convert.ToInt32(Status));
        }
         
        /// <summary>
        /// Full Amount of item (disregard refunds)
        /// </summary>
        /// <param name="tax_rate"></param>
        /// <returns></returns>
        public double Get_Full_Amount(double tax_rate)
        {
            return Price * Get_Current_Quantity() * (1 + tax_rate);// -Discount_Amt; 
        }

        public double Get_Current_Tax_Amount(double tax_rate)
        {
            return tax_rate * Get_Current_Quantity() * Price;
        }

    }
}
