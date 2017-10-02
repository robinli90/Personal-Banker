using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Objects;

namespace Financial_Journal
{
    public class Load_Functionalities
    {

        Receipt parent;
        public Savings_Structure Savings = new Savings_Structure();

        public Load_Functionalities(Receipt _parent)
        {
            parent = _parent;
            Savings = parent.Savings;
        }

        public bool Load_Information(string root_config_file_path, bool loadFromCloud = false, bool multiThreadLoad = true)
        {
            DateTime startTime = DateTime.Now;

            parent.Load_Error = false;
            parent.Reset_Parameters();

            // enable multithreaded load times (synchronize 7 directory load simultaneously 
            // and wait till they all finish together before returning)
            if (multiThreadLoad && !loadFromCloud)
            {
                List<Thread> threads = new List<Thread>();
                List<bool> boolList = new List<bool>();

                Enumerable.Range(1, parent.SaveHelper.Save_Entries).Select(i =>
                {
                    Thread t = new Thread(() =>
                    {
                        boolList.Add(Load_Group(root_config_file_path, i, loadFromCloud));
                    });
                    t.Start();
                    return t;
                }).ToList().ForEach(x => x.Join());

                TimeSpan span = DateTime.Now - startTime;
                int ms = (int)span.TotalMilliseconds;
                Diagnostics.WriteLine(String.Format("Load time: {0}ms", ms));

                return !boolList.Contains(false);
            }

            // depreciated below
            for (int i = 1; i <= parent.SaveHelper.Save_Entries; i++)
            {
                if (!Load_Group(root_config_file_path, i, loadFromCloud))
                    return false;
            }

            TimeSpan span2 = DateTime.Now - startTime;
            int ms2 = (int)span2.TotalMilliseconds;
            Diagnostics.WriteLine(String.Format("Load time: {0}ms", ms2));
            return true;
        }


        /// <summary>
        /// Load information based off of root path
        /// </summary>
        /// <param name="root_path">The path containing the main config file (includes filename)</param>
        /// <param name="group_ID"></param>
        private bool Load_Group(string root_config_file_path, int group_ID, bool isCloudSync = false)
        {
            string Config_Serial = "";
            bool Requires_Settings = false;

            var text = "";

            // Get serial value for the config path
            if (!isCloudSync)
            {
                text = File.ReadAllText(root_config_file_path).Trim();
            }
            else
            {
                text = Cloud_Services.FTP_Read_Cloud(root_config_file_path);
                if (text == "") return false;
            }

            if (text.Length > 0)
            {
                string[] temp = AESGCM.SimpleDecryptWithPassword(text, "PASSWORDisHERE").Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                Config_Serial = temp[0];
                Requires_Settings = temp.Count() > 1 && temp[1] == "PASSWORD_REQUIRED";
                parent.SaveHelper.Current_Serial_Hash_Value = Config_Serial;
            }

            string save_path = Path.GetDirectoryName(root_config_file_path) + "\\";

            string[] lines = new string[] {};

            // Get load path
            switch (group_ID)
            {
                case 1:
                    save_path += "1\\" + Config_Serial;
                    break;
                case 2:
                    save_path += "2\\" + Config_Serial;
                    break;
                case 3:
                    save_path += "3\\" + Config_Serial;
                    break;
                case 4:
                    save_path += "4\\" + Config_Serial;
                    break;
                case 5:
                    save_path += "5\\" + Config_Serial;
                    break;
                case 6:
                    save_path += "6\\" + Config_Serial;
                    break;
                case 7:
                    save_path += "7\\" + Config_Serial;
                    break;
            }

            // Raise error if trying to settings required but settings file missing (to prevent bypass by deleting settings file
            if (!isCloudSync && !File.Exists(save_path) && group_ID == 1 && Requires_Settings) parent.Load_Error = true;

            if (isCloudSync || File.Exists(save_path))
            {
                // Decrypt text
                if (!isCloudSync)
                {
                    text = File.ReadAllText(save_path);
                }
                else
                {
                    string ftpPath = @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/Cloud_Sync/" + group_ID + "/" + Config_Serial;
                    text = Cloud_Services.FTP_Read_Cloud(ftpPath);

                    if (text == "") return false;
                }

                if (text.Trim().Length > 0)
                {
                    lines = AESGCM.SimpleDecryptWithPassword(text, "PASSWORDisHERE").Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                }

                //if (!Validate_File_Package(Config_Serial, lines)) Diagnostics.WriteLine("Load error");// parent.Load_Error = true;

                // Validate the integrity of the file package. If has been tampered with (aka beginning and end hash is not right)
                //if (!Validate_File_Package(Config_Serial, lines)) parent.Load_Error = true;

                switch (group_ID)
                {
                    case 1:
                        Load_Settings(lines);
                        break;
                    case 2:
                        Load_Links_Tax(lines);
                        break;
                    case 3:
                        Load_Items(lines);
                        break;
                    case 4:
                        Load_Hobby(lines);
                        break;
                    case 5:
                        Load_Agenda(lines);
                        break;
                    case 6:
                        Load_Calendar(lines);
                        break;
                    case 7:
                        Load_Expense_RP(lines);
                        break;
                }
            }

            return true;
        }

        private bool Validate_File_Package(string Hash_Key, string[] lines, int requiredHashCount = 2)
        {
            return (lines.Count(x => x == Hash_Key) == requiredHashCount);
        }

        private void Load_Links_Tax(string[] lines)
        {
            foreach (string line in lines)
            {
                if (line.Contains("[LINK_SOURCE]"))
                {
                    parent.Link_Location.Add(Parse_Line_Information(line, "LINK_SOURCE"), Parse_Line_Information(line, "LINK_DESTINATION"));

                    // Add location if it doesnt exist to location_list
                    if (!parent.Location_List.Any(x => x.Name == Parse_Line_Information(line, "LINK_SOURCE")))
                    {
                        parent.Location_List.Add(new Location()
                        {
                            Name = Parse_Line_Information(line, "LINK_SOURCE"),
                            Refund_Days = Convert.ToInt32(Parse_Line_Information(line, "REFUND_DAYS", "||", "0"))
                        });
                    }
                    parent.Location_List = parent.Location_List.OrderBy(x => x.Name).ToList();
                }

                // If Tax information, store tax information
                else if (line.Contains("[TAX_CATEGORY]"))
                {
                    parent.Tax_Rules_Dictionary.Add(Parse_Line_Information(line, "TAX_CATEGORY"), Parse_Line_Information(line, "TAX_RATE"));
                }
                else if (line.Contains("||[CO_LA_]="))
                {
                    Contact C = new Contact()
                    {
                        First_Name = Parse_Line_Information(line, "CO_FI_"),
                        Last_Name = Parse_Line_Information(line, "CO_LA_"),
                        Email = Parse_Line_Information(line, "CO_EF_"),
                        Email_Second = Parse_Line_Information(line, "CO_ES_"),
                        Phone_No_Primary = Parse_Line_Information(line, "CO_PR_"),
                        Phone_No_Second = Parse_Line_Information(line, "CO_SE_"),
                        Association = Parse_Line_Information(line, "CO_AS_"),
                        Hash_Value = Parse_Line_Information(line, "CO_HA_")
                    };
                    parent.Contact_List.Add(C);
                }
                else if (line.Contains("[EXPIRATION_NAME]"))
                {
                    Expiration_Entry EE = new Expiration_Entry()
                    {
                        Item_Name = Parse_Line_Information(line, "EXPIRATION_NAME"),
                        Exp_Date_Count = Convert.ToInt32(Parse_Line_Information(line, "EXPIRATION_DAY_COUNT")),
                        Warning_Date_Count = Convert.ToInt32(Parse_Line_Information(line, "EXPIRATION_WARNING_COUNT")),
                        Location = Parse_Line_Information(line, "EXPIRATION_LOCATION"),
                        Last_Warn_Date = Parse_Line_Information(line, "EXPIRATION_LAST_WARN_DATE").Length > 0 ? Convert.ToDateTime(Parse_Line_Information(line, "EXPIRATION_LAST_WARN_DATE")) : DateTime.Now
                    };
                    parent.Expiration_List.Add(EE);
                }
                else if (line.Contains("||[AS_PR_]="))
                {
                    Asset_Item AI = new Asset_Item()
                    {
                        Name = Parse_Line_Information(line, "AS_NA_"),
                        Cost = Convert.ToDouble(Parse_Line_Information(line, "AS_PR_")),
                        Selling_Amount = Convert.ToDouble(Parse_Line_Information(line, "AS_SA_")),
                        Serial_Identification = Parse_Line_Information(line, "AS_SE_"),
                        OrderID = Parse_Line_Information(line, "AS_ID_"),
                        Note = Parse_Line_Information(line, "AS_NO_"),
                        Asset_Category = Parse_Line_Information(line, "AS_CA_"),
                        Purchase_Location = Parse_Line_Information(line, "AS_LO_"),
                        Purchase_Date = Convert.ToDateTime(Parse_Line_Information(line, "AS_PU_")),
                        Remove_Date = Convert.ToDateTime(Parse_Line_Information(line, "AS_RE_")), 
                    };
                    parent.Asset_List.Add(AI);
                }
                else if (line.Contains("[CATEGORY_GROUP]"))
                {
                    GroupedCategory GC = new GroupedCategory(Parse_Line_Information(line, "PROF_NAME"), Parse_Line_Information(line, "GRP_NAME"));
                    GC.SubCategoryList = Parse_Line_Information(line, "SUB_CATEGORIES")
                        .Split(new string[] {"~"}, StringSplitOptions.None).ToList();
                    GC.SubExpenseList = Parse_Line_Information(line, "SUB_EXPENSES")
                        .Split(new string[] {"~"}, StringSplitOptions.None).ToList();

                    // Filter out null lines
                    GC.SubCategoryList = GC.SubCategoryList.Where(x => x.Length > 0).ToList();
                    GC.SubExpenseList = GC.SubExpenseList.Where(x => x.Length > 0).ToList();

                    parent.GroupedCategoryList.Add(GC);
                }
            }

        }
        private void Load_Items(string[] lines)
        {
            foreach (string line in lines)
            {
                if (line.Contains("||[IT_LO]="))
                {
                    Item New_Item = new Item();
                    New_Item.Name = Parse_Line_Information(line, "IT_DE_");
                    New_Item.Status = Parse_Line_Information(line, "IT_ST_") == "" ? "0" : Parse_Line_Information(line, "IT_ST_");
                    New_Item.RefundAlert = Parse_Line_Information(line, "IT_RE_") == "1" ? true : false;
                    New_Item.consumedStatus = Parse_Line_Information(line, "IT_CO_") == "" ? 2 : Convert.ToInt32(Parse_Line_Information(line, "IT_CO_"));
                    New_Item.Location = Parse_Line_Information(line, "IT_LO");
                    New_Item.Payment_Type = Parse_Line_Information(line, "IT_PA_");
                    New_Item.Category = Parse_Line_Information(line, "IT_CA_");
                    New_Item.Discount_Amt = Convert.ToDouble(Parse_Line_Information(line, "IT_DI_", "||", "0"));
                    New_Item.Price = Convert.ToDouble(Parse_Line_Information(line, "IT_PR_"));
                    New_Item.Quantity = Convert.ToInt32(Parse_Line_Information(line, "IT_QU_"));
                    New_Item.Date = Convert.ToDateTime(Parse_Line_Information(line, "IT_DA_"));
                    New_Item.Refund_Date = Parse_Line_Information(line, "IT_RD_").Length > 0 ? Convert.ToDateTime(Parse_Line_Information(line, "IT_RD_")) : DateTime.Now;
                    New_Item.Memo = Parse_Line_Information(line, "IT_ME_");
                    New_Item.OrderID = Parse_Line_Information(line, "IT_ID_");

                    parent.Master_Item_List.Add(New_Item);

                    // Add pre-existing information to comboboxes
                    if (!parent.Category_List.Contains(New_Item.Category)) parent.Category_List.Add(New_Item.Category);

                    bool Contains_Location = false;
                    foreach (Company g in parent.Company_List)
                    {
                        if (g.Name == New_Item.Location)
                        {
                            Contains_Location = true;
                        }
                    }

                    if (!Contains_Location) parent.Company_List.Add(new Company() { Name = New_Item.Location });
                }
                else if (line.Contains("||[OR_QU_]="))
                {
                    Order New_Order = new Order();
                    New_Order.Location = Parse_Line_Information(line, "OR_LO_");
                    New_Order.OrderMemo = Parse_Line_Information(line, "OR_ME_");
                    New_Order.Payment_Type = Parse_Line_Information(line, "OR_PA_");
                    New_Order.Tax_Overridden = (Parse_Line_Information(line, "OR_TO_") == "1");
                    New_Order.Order_Total_Pre_Tax = Convert.ToDouble(Parse_Line_Information(line, "OR_PP_"));
                    New_Order.GC_Amount = Convert.ToDouble(Parse_Line_Information(line, "OR_GC_", "||", "0"));
                    New_Order.Order_Taxes = Convert.ToDouble(Parse_Line_Information(line, "OR_TA_"));
                    New_Order.Order_Discount_Amt = Convert.ToDouble(Parse_Line_Information(line, "OR_DI_", "||", "0"));
                    New_Order.Order_Quantity = Convert.ToInt32(Parse_Line_Information(line, "OR_QU_"));
                    New_Order.Date = Convert.ToDateTime(Parse_Line_Information(line, "OR_DA_"));
                    New_Order.OrderID = Parse_Line_Information(line, "OR_ID_");
                    parent.Order_List.Add(New_Order);
                }
            }

            /// Temporary to manipulate location name
            if (false)
            {
                string locationName = "Japanese Restaurant";
                parent.Location_List.Add(new Location() {Name = locationName, Refund_Days = 0});

                int changeCount = 0;

                // Change Chinese restaurants to appropriate names
                foreach (Order order in parent.Order_List.Where(x => x.Location == "Chinese Restaurant"))
                {
                    foreach (Item item in parent.Master_Item_List.Where(x => x.OrderID == order.OrderID && (
                                                                                 x.Name.ToLower().Contains("donburi") || 
                                                                                 x.Name.ToLower().Contains("don buri") || 
                                                                                 //|| x.Name.ToLower().Contains("gal's")
                                                                                 //|| x.Name.ToLower().Contains("japanese")
                                                                                 x.Name.ToLower()
                                                                                     .Contains("hfgsdfgsdfg"))))
                    {
                        item.Location = locationName;
                        order.Location = locationName;
                        changeCount++;
                    }
                }

                Diagnostics.WriteLine("Number of changes/manipulated: " + changeCount);
            }
        }
        private void Load_Expense_RP(string[] lines)
        {
            foreach (string line in lines)
            {
                if (line.Contains("[EXPENSE_TYPE]="))
                {
                    List<DateTime> Date_Sequence = new List<DateTime>();
                    if (Parse_Line_Information(line, "EXPENSE_DATE_SEQUENCE") != "")
                    {
                        string[] dates = Parse_Line_Information(line, "EXPENSE_DATE_SEQUENCE").Split(new string[] { "," }, StringSplitOptions.None);
                        foreach (string Date in dates)
                            Date_Sequence.Add(Convert.ToDateTime(Date));
                    }

                    Expenses New_Expense = new Expenses();
                    New_Expense.Expense_Status = Parse_Line_Information(line, "EXPENSE_STATUS");
                    New_Expense.Expense_Type = Parse_Line_Information(line, "EXPENSE_TYPE");
                    New_Expense.Expense_Name = Parse_Line_Information(line, "EXPENSE_NAME");
                    New_Expense.Expense_Payee = Parse_Line_Information(line, "EXPENSE_PAYEE");
                    New_Expense.Expense_Frequency = Parse_Line_Information(line, "EXPENSE_FREQUENCY");
                    New_Expense.Date_Sequence = Date_Sequence;
                    New_Expense.Expense_Amount = Convert.ToDouble(Parse_Line_Information(line, "EXPENSE_AMOUNT"));
                    New_Expense.Payment_Last_Four = Parse_Line_Information(line, "EXPENSE_PAYMENT_LAST_FOUR");
                    New_Expense.AutoDebit = Parse_Line_Information(line, "EXPENSE_AUTODEBIT", "||", "0");
                    New_Expense.Payment_Company = Parse_Line_Information(line, "EXPENSE_PAYMENT_COMPANY");
                    New_Expense.Expense_Start_Date = Convert.ToDateTime(Parse_Line_Information(line, "EXPENSE_START_DATE"));
                    New_Expense.Last_Pay_Date = Parse_Line_Information(line, "EXPENSE_LAST_PAY_DATE") == "" ? new DateTime(1990, 1, 1) : Convert.ToDateTime(Parse_Line_Information(line, "EXPENSE_LAST_PAY_DATE"));
                    New_Expense.Alert_Off_Date = Parse_Line_Information(line, "EXPENSE_ALERT_DATE") == "" ? new DateTime(1990, 1, 1) : Convert.ToDateTime(Parse_Line_Information(line, "EXPENSE_ALERT_DATE"));
                    parent.Expenses_List.Add(New_Expense);


                }
                else if (line.Contains("GC_LOCATION") && line.Contains("GC_ASC_ORD"))
                {
                    GC GCard = new GC();
                    GCard.Amount = Convert.ToDouble(Parse_Line_Information(line, "GC_AMOUNT", "||", "0"));
                    GCard.Location = Parse_Line_Information(line, "GC_LOCATION");
                    GCard.Date_Added = Parse_Line_Information(line, "GC_DATE_ADDED") == "" ? DateTime.Now : Convert.ToDateTime(Parse_Line_Information(line, "GC_DATE_ADDED"));
                    GCard.Last_Four = Parse_Line_Information(line, "GC_LAST_FOUR");
                    string[] temp = Parse_Line_Information(line, "GC_ASC_ORD").Split(new string[] { "," }, StringSplitOptions.None);
                    if (temp.Count() >= 2)
                    {
                        for (int i = 0; i < temp.Count(); i += 2)
                        {
                            GCard.Add_Order(temp[i], temp[i + 1]);
                        }
                    }
                    parent.GC_List.Add(GCard);

                }
                else if (line.Contains("BILLING_START") && line.Contains("EMERGENCY_NO"))
                {
                    Payment Payment = new Payment();
                    Payment.Payment_Type = Parse_Line_Information(line, "PAYMENT_TYPE");
                    Payment.Last_Four = Parse_Line_Information(line, "LAST_FOUR");
                    Payment.Company = Parse_Line_Information(line, "COMPANY");
                    Payment.Bank = Parse_Line_Information(line, "BANK_NAME");
                    Payment.Limit = Convert.ToDouble(Parse_Line_Information(line, "CARD_LIMIT"));
                    Payment.Balance = Convert.ToDouble(Parse_Line_Information(line, "BALANCE") == "" ? "0" : Parse_Line_Information(line, "BALANCE"));
                    Payment.Billing_Start = Parse_Line_Information(line, "BILLING_START");
                    Payment.Emergency_No = Parse_Line_Information(line, "EMERGENCY_NO");
                    Payment.Last_Reset_Date = Parse_Line_Information(line, "LAST_UPDATE_DATE") == "" ? DateTime.Now.AddYears(-1) : Convert.ToDateTime(Parse_Line_Information(line, "LAST_UPDATE_DATE"));
                    Payment.Alerts[0].Active = Parse_Line_Information(line, "ALERT_A").Split(new string[] { ":" }, StringSplitOptions.None)[0] == "1";
                    Payment.Alerts[0].Repeat = Parse_Line_Information(line, "ALERT_A").Split(new string[] { ":" }, StringSplitOptions.None)[1] == "1";
                    Payment.Alerts[1].Active = Parse_Line_Information(line, "ALERT_B").Split(new string[] { ":" }, StringSplitOptions.None)[0] == "1";
                    Payment.Alerts[1].Repeat = Parse_Line_Information(line, "ALERT_B").Split(new string[] { ":" }, StringSplitOptions.None)[1] == "1";
                    Payment.Alerts[2].Active = Parse_Line_Information(line, "ALERT_C").Split(new string[] { ":" }, StringSplitOptions.None)[0] == "1";
                    Payment.Alerts[2].Repeat = Parse_Line_Information(line, "ALERT_C").Split(new string[] { ":" }, StringSplitOptions.None)[1] == "1";
                    Payment.Alerts[3].Active = Parse_Line_Information(line, "ALERT_D").Split(new string[] { ":" }, StringSplitOptions.None)[0] == "1";
                    Payment.Alerts[3].Repeat = Parse_Line_Information(line, "ALERT_D").Split(new string[] { ":" }, StringSplitOptions.None)[1] == "1";
                    Payment.Calendar_Toggle = 0;
                    parent.Payment_List.Add(Payment);
                    Payment.Get_Total(parent.Master_Item_List, parent.Tax_Rules_Dictionary, parent.Tax_Rate, parent.Order_List);

                }
                else if (line.Contains("ACCOUNT_TYPE") && line.Contains("ACCOUNT_PAYER"))
                {
                    Account ACC = new Account();
                    ACC.Type = Parse_Line_Information(line, "ACCOUNT_TYPE");
                    ACC.Payer = Parse_Line_Information(line, "ACCOUNT_PAYER");
                    ACC.Remark = Parse_Line_Information(line, "ACCOUNT_REMARK");
                    ACC.Alert_Active = Parse_Line_Information(line, "ACCOUNT_ALERT_ACTIVE") == "" ? "0" : Parse_Line_Information(line, "ACCOUNT_ALERT_ACTIVE");
                    ACC.Amount = Parse_Line_Information(line, "ACCOUNT_AMOUNT");
                    ACC.Status = Convert.ToInt32(Parse_Line_Information(line, "ACCOUNT_STATUS"));
                    ACC.Inactive_Date = Parse_Line_Information(line, "ACCOUNT_INACTIVE") == "" ? DateTime.Now : Convert.ToDateTime(Parse_Line_Information(line, "ACCOUNT_INACTIVE"));
                    ACC.Start_Date = Parse_Line_Information(line, "ACCOUNT_START") == "" ? DateTime.Now : Convert.ToDateTime(Parse_Line_Information(line, "ACCOUNT_START"));
                    parent.Account_List.Add(ACC);
                }
                else if (line.Contains("||[PO_LA_]="))
                {
                    Payment_Options PO = new Payment_Options();
                    PO.Type = Parse_Line_Information(line, "PO_TY_");
                    PO.Payment_Last_Four = Parse_Line_Information(line, "PO_LA_");
                    PO.Payment_Company = Parse_Line_Information(line, "PO_CO_");
                    PO.Payment_Bank = Parse_Line_Information(line, "PO_BA_");
                    PO.Ending_Balance = Convert.ToDouble(Parse_Line_Information(line, "PO_EN_"));
                    PO.Date = Convert.ToDateTime(Parse_Line_Information(line, "PO_DA_"));
                    PO.Note = Parse_Line_Information(line, "PO_NO_");
                    PO.Hidden_Note = Parse_Line_Information(line, "PO_HI_");
                    PO.Amount = Convert.ToDouble(Parse_Line_Information(line, "PO_AM_"));
                    parent.Payment_Options_List.Add(PO);
                }
                else if (line.Contains("[INVESTMENT_NAME]"))
                {
                    Investment Iv = new Investment()
                    {
                        Name = Parse_Line_Information(line, "INVESTMENT_NAME"),
                        Active = Parse_Line_Information(line, "ACTIVE") == "1",
                        Principal = Convert.ToDouble(Parse_Line_Information(line, "PRINCIPAL")),
                        IRate = Convert.ToDouble(Parse_Line_Information(line, "IRATE")),
                        Frequency = Parse_Line_Information(line, "FREQUENCY"),
                        Start_Date = Convert.ToDateTime(Parse_Line_Information(line, "START_DATE")),
                        End_Date = Parse_Line_Information(line, "END_DATE") == "" ? new DateTime() : Convert.ToDateTime(Parse_Line_Information(line, "END_DATE")),
                        Balance_Sequence = new List<Investment_Transaction>()
                    };
                    string[] temp = Parse_Line_Information(line, "SEQUENCE").Trim(',').Split(new string[] { "," }, StringSplitOptions.None);
                    if (temp.Count() >= 3)
                    {
                        for (int i = 0; i < temp.Count(); i += 3)
                        {
                            Iv.Balance_Sequence.Add(new Investment_Transaction() { Date = Convert.ToDateTime(temp[i]), Entry_No = Convert.ToInt32(temp[i + 1]), Principal_Carry_Over = Convert.ToDouble(temp[i + 2]) });
                        }
                    }
                    Iv.Populate_Matrix();
                    parent.Investment_List.Add(Iv);
                }
                else if (line.Contains("CH_DA_"))
                {
                    Cash.AddCashHistory(
                        Convert.ToDateTime(Parse_Line_Information(line, "CH_DA_")),
                        Parse_Line_Information(line, "CH_ME_"),
                        Convert.ToDouble(Parse_Line_Information(line, "CH_NE_")),
                        Parse_Line_Information(line, "CH_ID_"), false);
                }

            }

            // Finally calculate balance
            Cash.CalculateBalances();
        }
        private void Load_Hobby(string[] lines)
        {
            foreach (string line in lines)
            {
                if (line.Contains("[CONTAINER_NAME"))
                {
                    List<Container> temp = new List<Container>();
                    int Max_Container_ID = parent.Master_Hobby_Item_List.Count == 0 ? 0 : parent.Master_Hobby_Item_List.Max(x => x.Container_ID);

                    for (int i = 1; i <= Max_Container_ID; i++)
                    {
                        if (Parse_Line_Information(line, "CONTAINER_NAME" + i.ToString()) != "")
                        {
                            temp.Add(new Container() { Name = Parse_Line_Information(line, "CONTAINER_NAME" + i), ID = i.ToString() });
                        }
                    }

                    parent.Master_Container_Dict.Add(Parse_Line_Information(line, "CONTAINER_PROFILE_NUMBER") == "" ? "1" : Parse_Line_Information(line, "CONTAINER_PROFILE_NUMBER"), temp);

                }
                else if (line.Contains("[HOBBY_PROFILE"))
                {
                    int profile_number = 1;
                    while (Parse_Line_Information(line, "HOBBY_PROFILE" + profile_number) != "")
                    {
                        if (parent.Master_Container_Dict.ContainsKey(profile_number.ToString()))
                            parent.Hobby_Profile_List.Add(Parse_Line_Information(line, "HOBBY_PROFILE" + profile_number));
                        profile_number++;
                    }
                }
                else if (line.Contains("||[HO_CA_]="))
                {
                    Hobby_Item HI = new Hobby_Item();
                    HI.Name = Parse_Line_Information(line, "HO_NA_");
                    HI.Category = Parse_Line_Information(line, "HO_CA_");
                    HI.OrderID = Parse_Line_Information(line, "HO_ID_");
                    HI.Price = Convert.ToDouble(Parse_Line_Information(line, "HO_PR_"));
                    HI.Container_ID = Convert.ToInt32(Parse_Line_Information(line, "HO_CO_"));
                    HI.Profile_Number = Parse_Line_Information(line, "HO_PN_");
                    HI.Unique_ID = Parse_Line_Information(line, "HO_UN_");
                    parent.Master_Hobby_Item_List.Add(HI);
                }
            }
        }
        private void Load_Agenda(string[] lines)
        {
            foreach (string line in lines)
            {
                if (line.Contains("[AGENDA_ITEM]"))
                {
                    Agenda_Item AI = new Agenda_Item();
                    AI.Name = Parse_Line_Information(line, "A_NAME");
                    AI.Contact_Hash_Value = Parse_Line_Information(line, "C_HASH_VALUE");
                    AI.Date = Convert.ToDateTime(Parse_Line_Information(line, "A_DATE"));
                    AI.Hash_Value = parent.Parse_Line_Information(line, "HASH_VALUE") == "" ? OrderID_Gen.Next(100000000, 999999999).ToString() : Parse_Line_Information(line, "HASH_VALUE");
                    AI.Calendar_Date = Parse_Line_Information(line, "A_CALENDAR_DATE") == "" ? new DateTime(1800, 1, 1) : Convert.ToDateTime(Parse_Line_Information(line, "A_CALENDAR_DATE"));
                    AI.ID = Convert.ToInt32(Parse_Line_Information(line, "A_ID"));
                    AI.Time_Set = Parse_Line_Information(line, "A_CHECK_STATE") == "1";
                    AI.Check_State = Parse_Line_Information(line, "TIME_SET") == "1";
                    parent.Agenda_Item_List.Add(AI);
                }
                else if (line.Contains("[SHOPPING_ITEM]"))
                {
                    Shopping_Item AI = new Shopping_Item();
                    AI.Name = Parse_Line_Information(line, "S_NAME");
                    AI.Contact_Hash_Value = Parse_Line_Information(line, "C_HASH_VALUE");
                    AI.ID = Convert.ToInt32(Parse_Line_Information(line, "S_ID"));
                    AI.Hash_Value = parent.Parse_Line_Information(line, "HASH_VALUE") == "" ? OrderID_Gen.Next(100000000, 999999999).ToString() : Parse_Line_Information(line, "HASH_VALUE");
                    AI.Calendar_Date = Parse_Line_Information(line, "S_DATE") == "" ? new DateTime(1800, 1, 1) : Convert.ToDateTime(Parse_Line_Information(line, "S_DATE"));
                    AI.Check_State = Parse_Line_Information(line, "S_CHECK_STATE") == "1";
                    AI.Time_Set = Parse_Line_Information(line, "TIME_SET") == "1";
                    parent.Agenda_Item_List.FirstOrDefault(x => x.ID == AI.ID).Shopping_List.Add(AI);
                }
                else if (line.Contains("[TRK_REF_ORDER_NUMBER]"))
                {
                    Shipment_Tracking ST = new Shipment_Tracking()
                    {
                        Tracking_Number = Parse_Line_Information(line, "TRK_NUMBER"),
                        Ref_Order_Number = Parse_Line_Information(line, "TRK_REF_ORDER_NUMBER"),
                        Expected_Date = Convert.ToDateTime(Parse_Line_Information(line, "TRK_EXP_DATE")),
                        Received_Date = Parse_Line_Information(line, "TRK_REC_DATE") == "" ? DateTime.Now : Convert.ToDateTime(Parse_Line_Information(line, "TRK_REC_DATE")),
                        Last_Alert_Date = Convert.ToDateTime(Parse_Line_Information(line, "TRK_ALERT_DATE")),
                        Alert_Active = Parse_Line_Information(line, "TRK_ALERT_ACTIVE") == "1",
                        Email_Active = Parse_Line_Information(line, "TRK_EMAIL_ACTIVE") == "1",
                        Status = Convert.ToInt32(Parse_Line_Information(line, "TRK_STATUS"))
                    };
                    parent.Tracking_List.Add(ST);
                }
            }
        }
        private void Load_Calendar(string[] lines)
        {
            foreach (string line in lines)
            {
                if (line.Contains("||[CA_AC_]="))
                {
                    Calendar_Events CE = new Calendar_Events();
                    CE.Title = Parse_Line_Information(line, "CA_TI_");
                    CE.Hash_Value = Parse_Line_Information(line, "CA_HA_") == "" ? OrderID_Gen.Next(100000000, 999999999).ToString() : Parse_Line_Information(line, "CA_HA_");
                    CE.Contact_Hash_Value = Parse_Line_Information(line, "CA_CO_");
                    CE.Is_Active = Parse_Line_Information(line, "CA_AC_", "||", "0");
                    CE.Description = Parse_Line_Information(line, "CA_DE_").Replace("~~", Environment.NewLine);
                    CE.Importance = Convert.ToInt32(Parse_Line_Information(line, "CA_IM_"));
                    CE.Date = Convert.ToDateTime(Parse_Line_Information(line, "CA_DA_"));
                    CE.MultiDays = Convert.ToInt32(Parse_Line_Information(line, "CA_MU_", "||", "0"));
                    CE.Time_Set = Parse_Line_Information(line, "CA_TI_") == "1";
                    string[] date_Strings = (Parse_Line_Information(line, "CA_AL_").Split(new string[] { "~" }, StringSplitOptions.None));
                    foreach (string dS in date_Strings)
                    {
                        if (dS.Length > 0)
                        {
                            CE.Alert_Dates.Add(Convert.ToDateTime(dS));
                        }
                    }
                    parent.Calendar_Events_List.Add(CE);
                }
                else if (line.Contains("||[BU_IN_]="))
                {
                    string moYr = Parse_Line_Information(line, "BU_MY_", "||", "0101");
                    string income = Parse_Line_Information(line, "BU_IN_", "||", "0101");
                    string budgetCategory = Parse_Line_Information(line, "BU_IT_");

                    BudgetEntry BE = new BudgetEntry(Convert.ToInt32(moYr.Substring(0, 2)),
                        2000 + Convert.ToInt32(moYr.Substring(2, 2)),
                        (income.Substring(0, 1) == "A" ? IncomeMode.Automatic : IncomeMode.Manual),
                        Convert.ToDouble(income.Substring(1)));

                    if (budgetCategory.Length > 0)
                    {
                        string[] budgetCategoryList =
                            budgetCategory.Split(new string[] { "~~" }, StringSplitOptions.None);

                        for (int i = 0; i < budgetCategoryList.Length; i += 3)
                        {
                            BE.AddBudgetCategory(budgetCategoryList[i], budgetCategoryList[i + 1],
                                Convert.ToDouble(budgetCategoryList[i + 2]));
                        }
                    }
                    parent.BudgetEntryList.Add(BE);
                }
            }
        }
        private void Load_Settings(string[] lines)
        {
            foreach (string line in lines)
            {
                if (line.Contains("[PERSONAL_SETTINGS]"))
                {
                    // App settings
                    parent.Frame_Color = System.Drawing.ColorTranslator.FromHtml(Parse_Line_Information(line, "APP_SETTING_COLOR"));
                    parent.Set_Form_Color(parent.Frame_Color);
                    parent.Settings_Dictionary.Add("APP_SETTING_COLOR", Parse_Line_Information(line, "APP_SETTING_COLOR"));
                    parent.Settings_Dictionary.Add("STATUSBAR_SETTINGS_INFO", Parse_Line_Information(line, "STATUSBAR_SETTINGS_INFO") == "" ? "6" : Parse_Line_Information(line, "STATUSBAR_SETTINGS_INFO"));
                    parent.Settings_Dictionary.Add("STATUSBAR_SETTINGS_WEATHER", Parse_Line_Information(line, "STATUSBAR_SETTINGS_WEATHER") == "" ? "14" : Parse_Line_Information(line, "STATUSBAR_SETTINGS_WEATHER"));
                    parent.statusResetSeconds = Convert.ToInt32(parent.Settings_Dictionary["STATUSBAR_SETTINGS_INFO"]);
                    parent.weatherResetSeconds = Convert.ToInt32(parent.Settings_Dictionary["STATUSBAR_SETTINGS_WEATHER"]);
                    // Login Credentials and parameters
                    parent.Settings_Dictionary.Add("LOGIN_PASSWORD", Parse_Line_Information(line, "LOGIN_PASSWORD") == "" ? "" : Parse_Line_Information(line, "LOGIN_PASSWORD"));
                    parent.Settings_Dictionary.Add("UNIQUE_IDENTIFIER", Parse_Line_Information(line, "UNIQUE_IDENTIFIER") == "" ? GetRandomIdentifier() : Parse_Line_Information(line, "UNIQUE_IDENTIFIER"));
                    parent.Settings_Dictionary.Add("AUTHENTICATION_REQ", Parse_Line_Information(line, "AUTHENTICATION_REQ") == "" ? "0" : Parse_Line_Information(line, "AUTHENTICATION_REQ"));
                    parent.Settings_Dictionary.Add("CLOUD_LOAD", Parse_Line_Information(line, "CLOUD_LOAD") == "" ? "0" : Parse_Line_Information(line, "CLOUD_LOAD"));
                    parent.Settings_Dictionary.Add("CLOUD_SYNC_ON_CLOSE", Parse_Line_Information(line, "CLOUD_SYNC_ON_CLOSE") == "" ? "0" : Parse_Line_Information(line, "CLOUD_SYNC_ON_CLOSE"));
                    parent.Settings_Dictionary.Add("REMEMBER_ME", Parse_Line_Information(line, "REMEMBER_ME") == "" ? "0" : Parse_Line_Information(line, "REMEMBER_ME"));
                    parent.Settings_Dictionary.Add("LOGIN_EMAIL", Parse_Line_Information(line, "LOGIN_EMAIL") == "" ? "" : Parse_Line_Information(line, "LOGIN_EMAIL"));
                    parent.Settings_Dictionary.Add("CLOUD_SYNC_TIME", Parse_Line_Information(line, "CLOUD_SYNC_TIME") == "" ? new DateTime().ToString() : Parse_Line_Information(line, "CLOUD_SYNC_TIME"));
                    parent.Settings_Dictionary.Add("AUTO_MOBILE_SYNC", Parse_Line_Information(line, "AUTO_MOBILE_SYNC") == "" ? "0" : Parse_Line_Information(line, "AUTO_MOBILE_SYNC"));
                    parent.Settings_Dictionary.Add("MOBILE_SYNC_ON_LOAD", Parse_Line_Information(line, "MOBILE_SYNC_ON_LOAD") == "" ? "0" : Parse_Line_Information(line, "MOBILE_SYNC_ON_LOAD"));

                    // Personal Information
                    parent.Settings_Dictionary.Add("PERSONAL_FIRST_NAME", Parse_Line_Information(line, "PERSONAL_FIRST_NAME") == "" ? "" : Parse_Line_Information(line, "PERSONAL_FIRST_NAME"));
                    parent.Settings_Dictionary.Add("PERSONAL_LAST_NAME", Parse_Line_Information(line, "PERSONAL_LAST_NAME") == "" ? "" : Parse_Line_Information(line, "PERSONAL_LAST_NAME"));
                    parent.Settings_Dictionary.Add("PERSONAL_EMAIL", Parse_Line_Information(line, "PERSONAL_EMAIL") == "" ? "" : Parse_Line_Information(line, "PERSONAL_EMAIL"));
                    // Alerts and Windows characteristics
                    parent.Settings_Dictionary.Add("SHOW_CALENDAR_ON_LOAD", Parse_Line_Information(line, "SHOW_CALENDAR_ON_LOAD") == "" ? "0" : Parse_Line_Information(line, "SHOW_CALENDAR_ON_LOAD"));
                    parent.Settings_Dictionary.Add("CALENDAR_EMAIL_SYNC", Parse_Line_Information(line, "CALENDAR_EMAIL_SYNC") == "" ? "0" : Parse_Line_Information(line, "CALENDAR_EMAIL_SYNC"));
                    parent.Settings_Dictionary.Add("ALERTS_ACTIVE", Parse_Line_Information(line, "ALERTS_ACTIVE") == "" ? "0" : Parse_Line_Information(line, "ALERTS_ACTIVE"));
                    parent.Settings_Dictionary.Add("ARP_ALERTS", Parse_Line_Information(line, "ARP_ALERTS") == "" ? "0" : Parse_Line_Information(line, "ARP_ALERTS"));
                    parent.Settings_Dictionary.Add("EXPENSE_ALERT", Parse_Line_Information(line, "EXPENSE_ALERT") == "" ? "0" : Parse_Line_Information(line, "EXPENSE_ALERT"));
                    parent.Settings_Dictionary.Add("SNEAK_PEAK", Parse_Line_Information(line, "SNEAK_PEAK") == "" ? "0" : Parse_Line_Information(line, "SNEAK_PEAK"));
                    parent.Settings_Dictionary.Add("START_MINIMIZED", Parse_Line_Information(line, "START_MINIMIZED") == "" ? "0" : Parse_Line_Information(line, "START_MINIMIZED"));
                    parent.Settings_Dictionary.Add("ENABLE_EXPIRATION_WARNINGS", Parse_Line_Information(line, "ENABLE_EXPIRATION_WARNINGS") == "" ? "0" : Parse_Line_Information(line, "ENABLE_EXPIRATION_WARNINGS"));
                    parent.Show_Calendar_On_Load = parent.Settings_Dictionary["SHOW_CALENDAR_ON_LOAD"] == "1";
                    parent.Alerts_On = parent.Settings_Dictionary["ALERTS_ACTIVE"] == "1";
                    // Hobby Management Settings
                    parent.Settings_Dictionary.Add("HOBBY_MGMT_X", Parse_Line_Information(line, "HOBBY_MGMT_X") == "" ? "250" : Parse_Line_Information(line, "HOBBY_MGMT_X"));
                    parent.Settings_Dictionary.Add("HOBBY_MGMT_Y", Parse_Line_Information(line, "HOBBY_MGMT_Y") == "" ? "305" : Parse_Line_Information(line, "HOBBY_MGMT_Y"));
                    // Backup settings
                    parent.Settings_Dictionary.Add("AUTO_SAVE", Parse_Line_Information(line, "AUTO_SAVE") == "" ? "0" : Parse_Line_Information(line, "AUTO_SAVE"));
                    parent.Settings_Dictionary.Add("AUTO_DELETE", Parse_Line_Information(line, "AUTO_DELETE") == "" ? "0" : Parse_Line_Information(line, "AUTO_DELETE"));
                    parent.Settings_Dictionary.Add("BACKUP_REQ", Parse_Line_Information(line, "BACKUP_REQ") == "" ? "0" : Parse_Line_Information(line, "BACKUP_REQ"));
                    parent.Settings_Dictionary.Add("BACKUP_DEL", Parse_Line_Information(line, "BACKUP_DEL") == "" ? "0" : Parse_Line_Information(line, "BACKUP_DEL"));
                    // Login times
                    parent.Settings_Dictionary.Add("LOGIN_BYPASS", Parse_Line_Information(line, "LOGIN_BYPASS") == "" ? "0" : Parse_Line_Information(line, "LOGIN_BYPASS"));
                    parent.Settings_Dictionary.Add("LAST_LOGIN", Parse_Line_Information(line, "LAST_LOGIN") == "" ? DateTime.Now.ToString() : Parse_Line_Information(line, "LAST_LOGIN"));
                    parent.Settings_Dictionary.Add("SESSION_EXPIRY", Parse_Line_Information(line, "SESSION_EXPIRY") == "" ? DateTime.Now.ToString() : Parse_Line_Information(line, "SESSION_EXPIRY"));
                    // Spreadsheet : Cash flow ssettings
                    parent.Settings_Dictionary.Add("CASHFLOW_SHOW_PERCENT", Parse_Line_Information(line, "CASHFLOW_SHOW_PERCENT") == "" ? "0" : Parse_Line_Information(line, "CASHFLOW_SHOW_PERCENT"));
                    parent.Settings_Dictionary.Add("CASHFLOW_SHOW_VALUE", Parse_Line_Information(line, "CASHFLOW_SHOW_VALUE") == "" ? "0" : Parse_Line_Information(line, "CASHFLOW_SHOW_VALUE"));
                    parent.Settings_Dictionary.Add("CASHFLOW_GROUP_GC", Parse_Line_Information(line, "CASHFLOW_GROUP_GC") == "" ? "0" : Parse_Line_Information(line, "CASHFLOW_GROUP_GC"));
                    parent.Settings_Dictionary.Add("CASHFLOW_IGNORE_ZERO_VALUE", Parse_Line_Information(line, "CASHFLOW_IGNORE_ZERO_VALUE") == "" ? "0" : Parse_Line_Information(line, "CASHFLOW_IGNORE_ZERO_VALUE"));
                    parent.Settings_Dictionary.Add("CASHFLOW_SHOW_PERCENT_VS_TOTAL", Parse_Line_Information(line, "CASHFLOW_SHOW_PERCENT_VS_TOTAL") == "" ? "0" : Parse_Line_Information(line, "CASHFLOW_SHOW_PERCENT_VS_TOTAL"));
                    parent.Settings_Dictionary.Add("CASHFLOW_PAYMENT_ACCS", Parse_Line_Information(line, "CASHFLOW_PAYMENT_ACCS"));
                    // Income information
                    parent.Settings_Dictionary.Add("INCOME_MONTHLY", (Parse_Line_Information(line, "INCOME_MONTHLY") == "" ? "0" : Parse_Line_Information(line, "INCOME_MONTHLY")));
                    parent.Settings_Dictionary.Add("INCOME_HOURLY", (Parse_Line_Information(line, "INCOME_HOURLY") == "" ? "0" : Parse_Line_Information(line, "INCOME_HOURLY")));
                    parent.Settings_Dictionary.Add("INCOME_WEEKLY", (Parse_Line_Information(line, "INCOME_WEEKLY") == "" ? "0" : Parse_Line_Information(line, "INCOME_WEEKLY")));
                    parent.Settings_Dictionary.Add("INCOME_DAILY", (Parse_Line_Information(line, "INCOME_DAILY") == "" ? "0" : Parse_Line_Information(line, "INCOME_DAILY")));
                    parent.Settings_Dictionary.Add("INCOME_YEARLY", (Parse_Line_Information(line, "INCOME_YEARLY") == "" ? "0" : Parse_Line_Information(line, "INCOME_YEARLY")));
                    parent.Settings_Dictionary.Add("WORK_HPD", (Parse_Line_Information(line, "WORK_HPD") == "" ? "0" : Parse_Line_Information(line, "WORK_HPD")));
                    parent.Settings_Dictionary.Add("WORK_OHPD", (Parse_Line_Information(line, "WORK_OHPD") == "" ? "0" : Parse_Line_Information(line, "WORK_OHPD")));
                    parent.Settings_Dictionary.Add("WORK_OMULTI", (Parse_Line_Information(line, "WORK_OMULTI") == "" ? "0" : Parse_Line_Information(line, "WORK_OMULTI")));
                    parent.Settings_Dictionary.Add("INCOME_TAX_RATE", (Parse_Line_Information(line, "INCOME_TAX_RATE") == "" ? "0" : Parse_Line_Information(line, "INCOME_TAX_RATE")));
                    parent.Settings_Dictionary.Add("GENERAL_TAX_RATE", (Parse_Line_Information(line, "GENERAL_TAX_RATE") == "" ? "0.13" : Parse_Line_Information(line, "GENERAL_TAX_RATE")));
                    parent.Settings_Dictionary.Add("INCOME_CHANGE_LOG", (Parse_Line_Information(line, "INCOME_CHANGE_LOG") == "" ? "" : Parse_Line_Information(line, "INCOME_CHANGE_LOG")));
                    // Check manual income
                    parent.Settings_Dictionary.Add("INCOME_MANUAL", (Parse_Line_Information(line, "INCOME_MANUAL") == "" ? "0" : Parse_Line_Information(line, "INCOME_MANUAL")));
                    // Quick Links
                    parent.Settings_Dictionary.Add("QL_ENABLED", (Parse_Line_Information(line, "QL_ENABLED", "||", "0")));
                    parent.Settings_Dictionary.Add("QL_AGENDA", (Parse_Line_Information(line, "QL_AGENDA", "||", "0")));
                    parent.Settings_Dictionary.Add("QL_LOOKUP", (Parse_Line_Information(line, "QL_LOOKUP", "||", "0")));
                    parent.Settings_Dictionary.Add("QL_CALENDAR", (Parse_Line_Information(line, "QL_CALENDAR", "||", "0")));
                    parent.Settings_Dictionary.Add("QL_MANAGE_HOBBY", (Parse_Line_Information(line, "QL_MANAGE_HOBBY", "||", "0")));
                    parent.Settings_Dictionary.Add("QL_DEPOSIT_PAY", (Parse_Line_Information(line, "QL_DEPOSIT_PAY", "||", "0")));
                    parent.Settings_Dictionary.Add("QL_VIEW_PURCHASES", (Parse_Line_Information(line, "QL_VIEW_PURCHASES", "||", "0")));
                    parent.Settings_Dictionary.Add("QL_MANAGE_PAYMENT", (Parse_Line_Information(line, "QL_MANAGE_PAYMENT", "||", "0")));
                    parent.Settings_Dictionary.Add("QL_VIEW_ONLINE", (Parse_Line_Information(line, "QL_VIEW_ONLINE", "||", "0")));
                    parent.Settings_Dictionary.Add("QL_SNEAK_PEEK", (Parse_Line_Information(line, "QL_SNEAK_PEEK", "||", "0")));
                    parent.Settings_Dictionary.Add("QL_CONTACTS", (Parse_Line_Information(line, "QL_CONTACTS", "||", "0")));
                    parent.Settings_Dictionary.Add("QL_SMS_ALERT", (Parse_Line_Information(line, "QL_SMS_ALERT", "||", "0")));
                    parent.Settings_Dictionary.Add("QL_SHOPPING_LIST", (Parse_Line_Information(line, "QL_SHOPPING_LIST", "||", "0")));
                    parent.Settings_Dictionary.Add("QL_MOBILE_SYNC", (Parse_Line_Information(line, "QL_MOBILE_SYNC", "||", "0")));
                    parent.Settings_Dictionary.Add("QL_BUDGET", (Parse_Line_Information(line, "QL_BUDGET", "||", "0")));
                    // Calendar Settings/toggles
                    parent.Settings_Dictionary.Add("CALENDAR_TOG_1", Parse_Line_Information(line, "CALENDAR_TOG_1") == "" ? "0" : Parse_Line_Information(line, "CALENDAR_TOG_1"));
                    parent.Settings_Dictionary.Add("CALENDAR_TOG_2", Parse_Line_Information(line, "CALENDAR_TOG_2") == "" ? "0" : Parse_Line_Information(line, "CALENDAR_TOG_2"));
                    parent.Settings_Dictionary.Add("CALENDAR_TOG_3", Parse_Line_Information(line, "CALENDAR_TOG_3") == "" ? "0" : Parse_Line_Information(line, "CALENDAR_TOG_3"));
                    parent.Settings_Dictionary.Add("CALENDAR_TOG_4", Parse_Line_Information(line, "CALENDAR_TOG_4") == "" ? "0" : Parse_Line_Information(line, "CALENDAR_TOG_4"));
                    parent.Settings_Dictionary.Add("CALENDAR_TOG_5", Parse_Line_Information(line, "CALENDAR_TOG_5") == "" ? "0" : Parse_Line_Information(line, "CALENDAR_TOG_5"));
                    parent.Settings_Dictionary.Add("CALENDAR_TOG_6", Parse_Line_Information(line, "CALENDAR_TOG_6") == "" ? "0" : Parse_Line_Information(line, "CALENDAR_TOG_6"));
                    parent.Settings_Dictionary.Add("CALENDAR_TOG_7", Parse_Line_Information(line, "CALENDAR_TOG_7") == "" ? "0" : Parse_Line_Information(line, "CALENDAR_TOG_7"));
                    parent.Settings_Dictionary.Add("CALENDAR_TOG_8", Parse_Line_Information(line, "CALENDAR_TOG_8") == "" ? "0" : Parse_Line_Information(line, "CALENDAR_TOG_8"));
                    parent.Settings_Dictionary.Add("CALENDAR_TOG_9", Parse_Line_Information(line, "CALENDAR_TOG_9") == "" ? "0" : Parse_Line_Information(line, "CALENDAR_TOG_9"));
                    parent.Settings_Dictionary.Add("CALENDAR_TOG_10", Parse_Line_Information(line, "CALENDAR_TOG_10") == "" ? "0" : Parse_Line_Information(line, "CALENDAR_TOG_10"));
                    parent.Settings_Dictionary.Add("CALENDAR_TOG_PERC", Parse_Line_Information(line, "CALENDAR_TOG_PERC") == "" ? "5" : Parse_Line_Information(line, "CALENDAR_TOG_PERC"));
                    // Grocery manager information
                    parent.Settings_Dictionary.Add("GROCERY_CATEGORIES", Parse_Line_Information(line, "GROCERY_CATEGORIES"));
                    parent.Settings_Dictionary.Add("EXTRANEOUS_SHOPPING_ITEMS", Parse_Line_Information(line, "EXTRANEOUS_SHOPPING_ITEMS"));
                    // Order Entry settings
                    parent.Settings_Dictionary.Add("OE_AUTO_POPULATE", Parse_Line_Information(line, "OE_AUTO_POPULATE", "||", "0"));
                    // Budget Settings
                    parent.Settings_Dictionary.Add("BUDDEFPROF", Parse_Line_Information(line, "BUDDEFPROF"));

                    parent.Monthly_Income = Convert.ToDouble(parent.Settings_Dictionary["INCOME_MONTHLY"]);
                    parent.Tax_Rate = Convert.ToDouble(parent.Settings_Dictionary["GENERAL_TAX_RATE"]);
                }
                else if (line.Contains("WARNING_FINAL"))
                {
                    Warning warn = new Warning();
                    warn.Category = Parse_Line_Information(line, "WARNING_CATEGORY");
                    warn.First_Level = Convert.ToDouble(Parse_Line_Information(line, "WARNING_FIRST"));
                    warn.Second_Level = Convert.ToDouble(Parse_Line_Information(line, "WARNING_SECOND"));
                    warn.Final_Level = Convert.ToDouble(Parse_Line_Information(line, "WARNING_FINAL"));
                    warn.Warning_Type = Parse_Line_Information(line, "WARNING_TYPE");
                    warn.Warning_Amt = Convert.ToDouble(Parse_Line_Information(line, "WARNING_AMOUNT"));
                    parent.Warnings_Dictionary.Add(warn.Category, warn);
                }
                else if (line.Contains("SAVINGS_SETTINGS") && line.Contains("STRUCTURE"))
                {
                    Savings.Structure = Parse_Line_Information(line, "STRUCTURE");
                    Savings.Ref_Value = Convert.ToDouble(Parse_Line_Information(line, "AMOUNT"));
                    Savings.Alert_1 = Parse_Line_Information(line, "ALERT_1") == "1" ? true : false;
                    Savings.Alert_2 = Parse_Line_Information(line, "ALERT_2") == "1" ? true : false;
                    Savings.Alert_3 = Parse_Line_Information(line, "ALERT_3") == "1" ? true : false;
                    parent.Savings_Instantiated = true;
                }
                else if (line.Contains("[TIER_STRUCTURE]"))
                {
                    string[] temp = Parse_Line_Information(line, "TIER_STRUCTURE").Split(new string[] { "," }, StringSplitOptions.None);
                    foreach (string g in temp) parent.Tier_Format.Add(Convert.ToInt32(g));
                    parent.Tier_Format = parent.Tier_Format.Count == 0 ? new List<int> { 1 } : parent.Tier_Format;
                }
                else if (line.Contains("[MANUAL_INCOME_COMPANY]"))
                {
                    CustomIncome CI = new CustomIncome();
                    CI.Company = Parse_Line_Information(line, "MANUAL_INCOME_COMPANY");
                    CI.First_Period_Date = Convert.ToDateTime(Parse_Line_Information(line, "MANUAL_FIRST_DATE"));
                    CI.Stop_Date = Convert.ToDateTime(Parse_Line_Information(line, "MANUAL_STOP_DATE"));
                    CI.Frequency = Parse_Line_Information(line, "MANUAL_FREQUENCY");
                    CI.Deposit_Account = Parse_Line_Information(line, "MANUAL_DEPOSIT_ACC");
                    CI.Default = Parse_Line_Information(line, "MANUAL_DEFAULT") == "1";
                    CI.Populate_Intervals(Parse_Line_Information(line, "MANUAL_INTERVALS"));
                    parent.Income_Company_List.Add(CI);
                }
                else if (line.Contains("SMS_ALERT||"))
                {
                    SMSAlert SMSA = new SMSAlert();
                    SMSA.Name = Parse_Line_Information(line, "NAME");
                    SMSA.Repeat = Parse_Line_Information(line, "REPEAT") == "1";
                    SMSA.Time = Convert.ToDateTime(Parse_Line_Information(line, "TIME"));
                    SMSA.IUO_Flag = true;
                    parent.SMSAlert_List.Add(SMSA);
                }
                else if (line.Contains("SYNC_ASSOCIATION||[T"))
                {
                    Association ASSO = new Association();
                    ASSO.InfoType = parent.MobileSync.GetInfoType(Parse_Line_Information(line, "TYPE"));
                    ASSO.LinkSource = Parse_Line_Information(line, "SOURCE");
                    ASSO.LinkDestination = Parse_Line_Information(line, "DESTINATION");
                    parent.AssociationList.Add(ASSO);
                }
            }
        }

        // Get Random Order ID
        Random OrderID_Gen = new Random();


        /// <summary>
        /// Return the output line after [output].
        /// 
        /// For example, in line = [INFO_TYPE]=ITEM||[ITEM_NAME]=CLOTHING||[ITEM_PRICE]=49.22||....
        ///     Calling this program:
        /// 
        ///     
        ///     Parse_Line_Information(line, "ITEM_PRICE", parse_token = "||") returns "49.22"
        ///     
        /// </summary>
        public string Parse_Line_Information(string input, string output, string parse_token = "||", string default_string = "")
        {
            string[] Split_Layer_1 = input.Split(new string[] { parse_token }, StringSplitOptions.None);

            foreach (string Info_Pair in Split_Layer_1)
            {
                if (Info_Pair.Contains("[" + output + "]"))
                {
                    return Info_Pair.Split(new string[] { "=" }, StringSplitOptions.None)[1];
                }
            }
            //Diagnostics.WriteLine("Potential error with Parse Line info for output: " + output);
            return default_string;
        }

        private string GetRandomIdentifier()
        {
            // Get Random Order ID
            Random OrderID_Gen = new Random(); // new random seed
            return OrderID_Gen.Next(100000000, 999999999).ToString();
        }
    }
}
