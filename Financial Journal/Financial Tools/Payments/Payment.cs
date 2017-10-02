using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financial_Journal
{
    public class Payment
    {
        public string Emergency_No { get; set; }
        public string Payment_Type { get; set; }
        public string Last_Four { get; set; }
        public string Company { get; set; }
        public string Bank { get; set; }
        public double Limit { get; set; }
        public string Billing_Start { get; set; }
        public double Total { get; set; }
        public DateTime Last_Reset_Date { get; set; }
        public List<Payment_Alert> Alerts { get; set; }
        public int Calendar_Toggle { get; set; }
        public double Balance { get; set; }

        public Payment()
        {
            Alerts = new List<Payment_Alert>() 
            {
                new Payment_Alert() { ID = 0, Active = false, Repeat = false, Desc = "Upcoming payment due alert"
                },
                new Payment_Alert() { ID = 1, Active = false, Repeat = false, Desc = "20% spending alert"
                },
                new Payment_Alert() { ID = 2, Active = false, Repeat = false, Desc = "50% spending alert"
                },
                new Payment_Alert() { ID = 3, Active = false, Repeat = false, Desc = "90% spending alert"
                }
            };
        }

        public string Check_Alerts()
        {
            if (Alerts[3].Active && Alerts[3].Repeat && (Limit * 0.9 < Total))
            {
                Alerts[3].Repeat = false;
                return "You have spent over 90% ($" + String.Format("{0:0.00}", Total) + ") of your limit ($" + String.Format("{0:0.00}", Limit) + ") on your " + Payment_Type + " ending in " + Last_Four;
            }
            else if (Alerts[2].Active && Alerts[2].Repeat && (Limit * 0.5 < Total))
            {
                Alerts[2].Repeat = false;
                return "You have spent over 50% ($" + String.Format("{0:0.00}", Total) + ") of your limit ($" + String.Format("{0:0.00}", Limit) + ") on your " + Payment_Type + " ending in " + Last_Four;
            }
            else if (Alerts[1].Active && Alerts[1].Repeat && (Limit * 0.2 < Total))
            {
                Alerts[1].Repeat = false;
                return "You have spent over 20% ($" + String.Format("{0:0.00}", Total) + ") of your limit ($" + String.Format("{0:0.00}", Limit) + ") on your " + Payment_Type + " ending in " + Last_Four;
            }
            return "";
        }

        public string Check_Payment_Due()
        {
            string[] Day_Name = new string[] {"", "1st", "2nd", "3rd", "4th", "5th", "6th", "7th", "8th", "9th", "10th", 
                                                        "11th", "12th", "13th", "14th", "15th", "16th", "17th", "18th", "19th", "20th", 
                                                        "21st", "22nd", "23rd", "24th", "25th", "26th", "27th", "28th", "29th", "30th", "31st"};

            if (Alerts[0].Active && Alerts[0].Repeat && Total > 0)
            {
                if (DateTime.Now.Day + 5 > Convert.ToInt32(Billing_Start) && DateTime.Now.Day < Convert.ToInt32(Billing_Start))
                {
                    Alerts[0].Repeat = false;
                    return "Your " + Company + " ending in " + Last_Four + " is due on the " + Day_Name[Convert.ToInt32(Billing_Start)] + " ($" + String.Format("{0:0.00}", Total) + ")";
                }
            }
            return "";
        }

        public void Get_Total(List<Item> Item_List, Dictionary<string, string> Tax_Rules, double base_tax, List<Order> Order_List)
        {
            // Check reset
            if (DateTime.Now > Last_Reset_Date.AddDays(29) || (DateTime.Now > Last_Reset_Date.AddDays(25) && Convert.ToInt32(Billing_Start) <= DateTime.Now.Day))
            {
                Alerts.ForEach(x => x.Repeat = true);
                Last_Reset_Date = DateTime.Now;
            }


            string Date_String = DateTime.Now.Year + "-" + DateTime.Now.Month.ToString("D2") + "-" + (Convert.ToInt32(DateTime.Now.Month == 2 && Convert.ToInt32(Billing_Start) - 1 > 28 ? "28" : (Billing_Start == "31" ? DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month).ToString() : (Convert.ToInt32(Billing_Start)).ToString()))).ToString("D2");
            DateTime End_Date = DateTime.ParseExact(Date_String, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture);
            if (DateTime.Now.Day > Convert.ToInt32(Billing_Start) - 1) End_Date = End_Date.AddMonths(1);

            DateTime Start_Date = End_Date.AddMonths(-1 + (End_Date.Date == DateTime.Now.Date ? 1 : 0));

            End_Date = Start_Date.AddMonths(1).AddDays(-1);

            List<Order> Filtered_Item_List = new List<Order>();

            for (int i = 0; i < Order_List.Count; i++)
            {
                Order x = Order_List[i];
                if (x.Payment_Type == (this.Company + " (xx-" + this.Last_Four + ")") && x.Date > Start_Date &&
                    x.Date < End_Date)
                {
                    Filtered_Item_List.Add(x);
                }
            }
            
            // Race condition on load - pending fix
           //  = Order_List.Where(x => x.Payment_Type == (this.Company + " (xx-" + this.Last_Four + ")") && x.Date > Start_Date && x.Date < End_Date).ToList();
            Total = Filtered_Item_List.Sum(x => x.Order_Total_Pre_Tax + x.Order_Taxes - x.GC_Amount);
         }

        /// <summary>
        /// Return the last transaction balance of the month. If none exists, use the previous period.... n-1
        /// </summary>
        /// <param name="Month"></param>
        /// <returns></returns> 
        public double Get_Final_Balance_Month(DateTime Ref_Month, List<Payment_Options> Payment_Options_List)
        {
            DateTime Comp_Month = new DateTime(Ref_Month.Year, Ref_Month.Month, 1);
            Comp_Month = Comp_Month.AddMonths(1).AddDays(-1); // Get the last day of the month

            // Filter List for this payment and transactions BEFORE ref_month
            Payment_Options_List = Payment_Options_List.Where(x => x.Payment_Last_Four == Last_Four &&
                                                                   x.Payment_Company == Company &&
                                                                   x.Payment_Bank == Bank &&
                                                                   x.Date <= Ref_Month).ToList();
            // Sort by newest first
            Payment_Options_List = Payment_Options_List.OrderByDescending(x => x.Date).ToList();

            if (Payment_Options_List.Count == 0) return 0;
            else return Payment_Options_List[0].Ending_Balance;
        }

        public string Get_Long_String()
        {
            return this.ToString() + " - " + Bank;
        }

        public override string ToString()
        {
            return Company + " (xx-" + Last_Four + ")";
        }
    }

    public class Payment_Alert
    {
        public string Desc { get; set; }
        public int ID { get; set; }
        public bool Active { get; set; }
        public bool Repeat { get; set; }
    }
}
