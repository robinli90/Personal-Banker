using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financial_Journal
{
    public class MobileSync
    {
        public static readonly string AESGCMKey = "MobilePASSWORD";

        public enum InfoType
        {
            Location,
            Payment,
            Category
        }

        public bool currentlyChecking = false;
        private Receipt parent;
        public List<Item> SyncedItems = new List<Item>();
        public List<Order> SyncedOrders = new List<Order>();
            
        public MobileSync(Receipt _parent)
        {
            parent = _parent; // set parent
        }

        public bool CheckForSyncFiles()
        {
            if (currentlyChecking) return false;

            currentlyChecking = true;
            // Reset lists
            SyncedOrders = new List<Order>();
            SyncedItems = new List<Item>();

            string ftpPath = @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/Cloud_Sync/Sync/" + parent.Settings_Dictionary["LOGIN_EMAIL"] + "_sync.pbf";

            if (Cloud_Services.FTP_Check_File_Exists(ftpPath))
            {
                Diagnostics.WriteLine(@"File exists!");

                string text = Cloud_Services.FTP_Read_Cloud(ftpPath);
                string[] lines = { };

                if (text == "") return false;

                if (text.Trim().Length > 0)
                {
                    lines = AESGCM.SimpleDecryptWithPassword(text, AESGCMKey).Split(new string[] { "\n" }, StringSplitOptions.None);
                    //lines = text.Split(new string[] { "\n"}, StringSplitOptions.None);
                }



                // Sync the syncfile lines
                foreach (string line in lines)
                {
                    if (line.Contains("[IT_NA_]="))
                    {

                        Item New_Item = new Item();
                        New_Item.Name = parent.LoadHelper.Parse_Line_Information(line, "IT_NA_");
                        New_Item.Status = "0";
                        New_Item.RefundAlert = false;
                        New_Item.consumedStatus = 2;
                        New_Item.Category = parent.LoadHelper.Parse_Line_Information(line, "IT_CA_");
                        New_Item.Discount_Amt = 0;
                        New_Item.Price = Convert.ToDouble(parent.LoadHelper.Parse_Line_Information(line, "IT_PR_"));
                        New_Item.Quantity = Convert.ToInt32(parent.LoadHelper.Parse_Line_Information(line, "IT_QU_"));
                        New_Item.Refund_Date = DateTime.Now;
                        New_Item.Memo = "";
                        New_Item.OrderID = parent.LoadHelper.Parse_Line_Information(line, "IT_ID_");
                        SyncedItems.Add(New_Item);
                    }
                    else if (line.Contains("[OR_LO_]=") && !line.Contains("[OR_SY_]=1"))
                    {
                        Order New_Order = new Order();
                        New_Order.Location = parent.LoadHelper.Parse_Line_Information(line, "OR_LO_");
                        New_Order.OrderMemo = "";
                        New_Order.Payment_Type = parent.LoadHelper.Parse_Line_Information(line, "OR_PA_");
                        New_Order.Tax_Overridden = false;
                        New_Order.GC_Amount = 0;
                        New_Order.Order_Discount_Amt = 0;
                        New_Order.Date = Convert.ToDateTime(parent.LoadHelper.Parse_Line_Information(line, "OR_DA_"));
                        New_Order.OrderID = parent.LoadHelper.Parse_Line_Information(line, "OR_ID_");
                        SyncedOrders.Add(New_Order);
                    }

                    #region Setup missing details for orders and items

                    // Amend order information
                    foreach (Order order in SyncedOrders)
                    {
                        UpdateOrderInformation(order);
                    }
                }

                // Amend item information
                foreach (Item item in SyncedItems)
                {
                    // Only add if exists
                    if (SyncedOrders.Any(x => x.OrderID == item.OrderID))
                    {
                        Order refOrder = SyncedOrders.First(x => x.OrderID == item.OrderID);
                        item.Date = refOrder.Date;
                        item.Payment_Type = refOrder.Payment_Type;
                        item.Location = refOrder.Location;
                    }
                }

                #endregion

                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Update order information for order
        /// </summary>
        /// <param name="order"></param>
        public void UpdateOrderInformation(Order order)
        {

            // Amend order information
            List<Item> orderItems = SyncedItems.Where(x => x.OrderID == order.OrderID).ToList();

            // Monetary variables
            double Order_Total_Pre_Tax = 0;
            double Order_Taxes = 0;

            foreach (Item item in orderItems)
            {
                Order_Total_Pre_Tax += item.Price * item.Quantity;
                Order_Taxes += item.Price * item.Quantity * parent.Get_Tax_Amount(item);
            }

            order.Order_Total_Pre_Tax = Order_Total_Pre_Tax;
            order.Order_Taxes = Order_Taxes;
            order.Order_Quantity = orderItems.Count;
        }

        /// <summary>
        /// returns the infotype
        /// </summary>
        /// <param name="infoTypeStr"></param>
        /// <returns></returns>
        public InfoType GetInfoType(string infoTypeStr)
        {
            switch (infoTypeStr)
            {
                case "Payment":
                    return InfoType.Payment;
                case "Location":
                    return InfoType.Location;
                case "Category":
                    return InfoType.Category;
            }
            return InfoType.Payment;
        }
    }

    public class Association
    {
        public MobileSync.InfoType InfoType { get; set; }
        public string LinkSource { get; set; }
        public string LinkDestination { get; set; }

        public Association(){}

    }
}
