using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;

namespace Financial_Journal
{
    public partial class Print_Calendar_Range : Form
    {

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;
        Calendar parent_Calendar;

        public Print_Calendar_Range(Receipt _parent, Calendar c, Point g = new Point(), Size s = new Size())
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            parent_Calendar = c;
            Start_Size = this.Size;
            this.Location = new Point(g.X + s.Width / 2 - this.Width / 2, g.Y + s.Height / 2 - this.Height / 2);
            Set_Form_Color(parent.Frame_Color);
        }

        // Converting month number to name
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            for (int i = 1; i < 32; i++)
            {
                from_day.Items.Add(i.ToString("D2"));
                to_day.Items.Add(i.ToString("D2"));
            }

            for (int i = 1; i < 13; i++)
            {
                from_month.Items.Add(mfi.GetMonthName(i));
                to_month.Items.Add(mfi.GetMonthName(i));
            }

            for (int i = DateTime.Now.AddYears(-5).Year; i <= DateTime.Now.Year; i++)
            {
                from_year.Items.Add(i);
                to_year.Items.Add(i);
            }

            DateTime Temp_Date = new DateTime(parent_Calendar.current_year, parent_Calendar.current_month, 1).AddMonths(1).AddDays(-1);

            from_day.SelectedIndex = 0;
            to_day.SelectedIndex = Temp_Date.Day - 1;

            from_month.Text = mfi.GetMonthName(parent_Calendar.current_month);
            to_month.Text = mfi.GetMonthName(parent_Calendar.current_month);

            from_year.Text = parent_Calendar.current_year.ToString();
            to_year.Text = parent_Calendar.current_year.ToString();

            Loaded = true;

            // Fade Box
            TFLP = new FadeControl();
            TFLP.Size = new Size(this.Width - 2, this.Height - 2);
            TFLP.Location = new Point(999, 999);
            TFLP.Visible = true;
            TFLP.BackColor = this.BackColor;
            TFLP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            TFLP.AllowDrop = true;
            TFLP.BringToFront();
            this.Controls.Add(TFLP);
            TFLP.BringToFront();

            TFLP.Opacity = 80;

            Apply_Toggle_Style();

            // Load print
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage_1);
            this.printPreviewDialog1.Document = this.printDocument1;
        }

        FadeControl TFLP;

        private void Grey_Out()
        {
            TFLP.Location = new Point(1, 1);
        }

        private void Grey_In()
        {
            TFLP.Location = new Point(1000, 1000);
        }

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        public void Set_Form_Color(Color randomColor)
        {
            //minimize_button.ForeColor = randomColor;
            //close_button.ForeColor = randomColor;
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        // Print button
        private void button2_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Curr_Print_Date = DateTime.MinValue; // reset print canvas
            
            using (var form2 = new Yes_No_Dialog(parent, "Are you sure you wish to print this calendar report?", "Warning", "Preview", "Print", 0, this.Location, this.Size))
            {
                var result = form2.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Calculate_Months();
                    parent_Calendar.Filter_Lists(From_Date, To_Date, true);
                    this.Visible = false;
                    parent_Calendar.Activate();
                    parent_Calendar.Grey_Out();
                    Cursor.Current = Cursors.WaitCursor;

                    if (form2.ReturnValue1 == "1") // Actually print
                    {
                        if (secondThreadFormHandle == IntPtr.Zero)
                        {
                            Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size, "PRINTING", "CALENDAR")
                            {
                            };
                            form.HandleCreated += SecondFormHandleCreated;
                            form.HandleDestroyed += SecondFormHandleDestroyed;
                            form.RunInNewThread(false);
                        }

                        printDocument1.Print();

                    }
                    else // Preview
                    {
                        //printPreviewDialog1.TopMost = true;
                        printPreviewDialog1.ShowDialog();

                    }

                    #region Restoration
                    parent_Calendar.Grey_In();
                    parent_Calendar.Activate();

                    if (secondThreadFormHandle != IntPtr.Zero)
                        PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

                    Cursor.Current = Cursors.Default;

                    this.Close();
                    #endregion
                }
                parent_Calendar.Activate();
            }
            /*testing only BELOW
            Calculate_Months();
            printPreviewDialog1.TopMost = true;
            printPreviewDialog1.ShowDialog();
            */

            Grey_In();
        }

        #region handler thread

        private IntPtr secondThreadFormHandle;

        void SecondFormHandleCreated(object sender, EventArgs e)
        {
            Control second = sender as Control;
            secondThreadFormHandle = second.Handle;
            second.HandleCreated -= SecondFormHandleCreated;
        }

        void SecondFormHandleDestroyed(object sender, EventArgs e)
        {
            Control second = sender as Control;
            secondThreadFormHandle = IntPtr.Zero;
            second.HandleDestroyed -= SecondFormHandleDestroyed;
        }

        const int WM_CLOSE = 0x0010;
        [DllImport("User32.dll")]
        extern static IntPtr PostMessage(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam);
        #endregion

        private void to_month_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        private void from_month_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        private void from_year_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        private void to_year_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        private void to_day_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        private void from_day_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        DateTime From_Date;
        DateTime To_Date;

        DateTime Curr_Print_Date;

        bool Ignore_Empty_Dates = true;
        bool Show_Spent_Figures = false;
        int Curr_Page = 0;

        // Main print function
        private void printDocument1_PrintPage_1(System.Object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // Reset toggles
            parent.Tracking_List.ForEach(x => x.Temp_Toggle = true);

            #region Parameters and Variables (fonts, pens, etc)
            int column1 = 35;           // Item name
            int col_subitem = column1 + 10; // sub item
            int col_subsubitem = col_subitem + 10; // sub sub item
            int starty = 10;
            int dataheight = 15;
            int height = starty + starty;

            StringFormat format1 = new StringFormat();
            format1.Alignment = StringAlignment.Center;

            Pen p = new Pen(Brushes.Black, 2.5f);
            Font f2 = new Font("MS Reference Sans Serif", 9f);
            Font f_subitem = new Font("MS Reference Sans Serif", 8f, FontStyle.Bold);
            Font f_subsubitem = new Font("MS Reference Sans Serif", 8f);
            Font f_subheader = new Font("MS Reference Sans Serif", 8.5f, FontStyle.Italic);
            Font f_subheader_reg = new Font("MS Reference Sans Serif", 8.5f);
            Font f4 = new Font("MS Reference Sans Serif", 10f, FontStyle.Bold);
            Font f5 = new Font("MS Reference Sans Serif", 9f, FontStyle.Italic);
            Font f3 = new Font("MS Reference Sans Serif", 12f, FontStyle.Bold);
            Font f1 = new Font("MS Reference Sans Serif", 14.0f, FontStyle.Bold);


            SolidBrush BlackBrush = new SolidBrush(Color.Black);
            SolidBrush RedBrush = new SolidBrush(Color.Red);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(122, 122, 122));
            SolidBrush LightGreyBrush = new SolidBrush(Color.FromArgb(200, 200, 200));
            Pen Grey_Pen = new Pen(GreyBrush, 1);
            Pen Light_Grey_Pen = new Pen(LightGreyBrush, 1);
            #endregion

            #region Header
            // First initialization
            if (Curr_Print_Date == DateTime.MinValue)
            {
                e.Graphics.DrawString("CALENDAR REPORT (" + From_Date.ToShortDateString() + " to " + To_Date.ToShortDateString() + ")", f1, Brushes.Black, new Rectangle(10, height, 650, dataheight * 2));
                height += dataheight;
                height += dataheight;
                height += dataheight;
                Curr_Print_Date = From_Date; // set start date
            }
            #endregion
            
            while (Curr_Print_Date <= To_Date)
            {
                #region Next page check
                if (height > e.MarginBounds.Height + 50)// + 20)
                {
                    height = starty;
                    e.HasMorePages = true;
                    Curr_Page++;
                    return;
                }

                #endregion

                #region Get Appropriate items for current print date
                // Instantiate lists
                List<Calendar_Events> CE_List = new List<Calendar_Events>();
                List<Agenda_Item> AI_List = new List<Agenda_Item>();
                List<Shopping_Item> SI_List = new List<Shopping_Item>();
                List<Payment_Options> PO_List = new List<Payment_Options>();
                List<Account> ARP_List = new List<Account>();
                List<Payment> Payment_List = new List<Payment>();
                List<Shipment_Tracking> TRK_List = new List<Shipment_Tracking>();

                // Get Calendar Events
                CE_List = parent_Calendar.CE_Filtered_List.Where(x => x.Date.Date == Curr_Print_Date.Date).ToList();
                // Get Agenda Items
                AI_List = parent_Calendar.AI_Filtered_List.Where(x => x.Calendar_Date.Date == Curr_Print_Date.Date).ToList();
                // Get Shopping Items
                parent_Calendar.AI_Filtered_List.ForEach(x => SI_List.AddRange(x.Shopping_List.Where(y => y.Calendar_Date.Date == Curr_Print_Date.Date).ToList()));
                // Get Payment Options list
                if (parent.Settings_Dictionary["CALENDAR_TOG_5"] == "1")
                    PO_List = parent.Payment_Options_List.Where(x => x.Date.Date == Curr_Print_Date.Date).ToList();
                // Get Shipments
                TRK_List = parent.Tracking_List.Where(x => ((x.Status == 1 && x.Expected_Date.Date == Curr_Print_Date.Date) || 
                                                            (x.Status == 0 && x.Received_Date.Date == Curr_Print_Date.Date)) 
                                                            && x.Temp_Toggle).ToList();
                // Get Payment Due Dates 
                if (parent.Settings_Dictionary["CALENDAR_TOG_2"] == "1")
                    Payment_List = parent.Payment_List.Where(x => (Convert.ToInt32(x.Billing_Start)) == Curr_Print_Date.Day).ToList();
                // Get Accounts R/P List
                if (parent.Settings_Dictionary["CALENDAR_TOG_3"] == "1")
                    ARP_List = parent.Account_List.Where(x => x.Start_Date.Date == Curr_Print_Date.Date).ToList();
                #endregion

                #region Checkbox toggle factors
                double Daily_Total = 0;

                // If showing spent figures
                if (Show_Spent_Figures)
                {
                    // Get Daily spent total
                    Daily_Total = parent.Order_List.Where(x => x.Date.Date == Curr_Print_Date.Date).ToList().Sum(x => x.Order_Total_Pre_Tax + x.Order_Taxes);
                }
                    
                // If ignoring empty values
                if (Daily_Total == 0 && CE_List.Count + AI_List.Count + SI_List.Count + PO_List.Count + TRK_List.Count + Payment_List.Count + ARP_List.Count == 0 && Ignore_Empty_Dates)
                {
                    goto End;
                }
                #endregion

                #region Draw Date & Line
                height += 4;
                height += 3;
                e.Graphics.DrawLine(Grey_Pen, column1 - 20, height, 840, height);
                height += 3;

                // Write date
                e.Graphics.DrawString(Curr_Print_Date.ToShortDateString(), f4, BlackBrush, new Rectangle(column1 - 20, height, 650, dataheight));
                #endregion
                #region Show Spent Total
                if (Daily_Total > 0)
                {
                    e.Graphics.DrawLine(Light_Grey_Pen, column1, height + dataheight, 840, height + dataheight);
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", Daily_Total) + " spent", f_subheader_reg, BlackBrush, new Rectangle(750, height, 650, dataheight));//, format1);
                }
                height += dataheight;
                #endregion
                #region If has Calendar/Agenda events

                if ((CE_List.Count + AI_List.Count + SI_List.Count) > 0)
                {
                    e.Graphics.DrawLine(Light_Grey_Pen, column1, height + dataheight, 840, height + dataheight);
                    e.Graphics.DrawString("Calendar & Agenda Events:", f_subheader, BlackBrush, new Rectangle(column1, height, 650, dataheight));//, format1);
                    height += dataheight;

                    // Order calendar events and agenda items by time
                    for (int i = 0; i < 24; i++) // Hours
                    {
                        for (int j = 0; j < 60; j++) // Minutes
                        {
                            #region Draw calendar events
                            foreach (Calendar_Events CE in CE_List.Where(x => x.Date.Hour == i && x.Date.Minute == j).ToList())
                            {
                                #region Get Importance String
                                string imp_string = "";
                                switch (CE.Importance)
                                {
                                    case 0:
                                        imp_string = "Not Important";
                                        break;
                                    case 1:
                                        imp_string = "Important";
                                        break;
                                    case 2:
                                        imp_string = "Very Important";
                                        break;
                                    default:
                                        MessageBox.Show("Invalid Importance level");
                                        break;
                                }
                                #endregion

                                // Sub item
                                e.Graphics.DrawString((CE.Time_Set ? ("[" + CE.Date.ToString("hh:mm tt") + "] : ") : "") + CE.Title + " (" + imp_string + ")", f_subitem, BlackBrush, new Rectangle(col_subitem, height, 650, dataheight));
                                height += dataheight - 2;

                                string[] desc_lines = CE.Description.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                                foreach (string g in desc_lines)
                                {
                                    // Sub sub item
                                    if (g.Length > 1)
                                    {
                                        e.Graphics.DrawString(g, f_subsubitem, BlackBrush, new Rectangle(col_subsubitem, height, 650, dataheight));
                                        height += dataheight - 2;
                                    }
                                }

                                // Append contact information
                                if (CE.Contact_Hash_Value.Length > 0 && parent_Calendar.CO_Filtered_List.Any(x => x.Hash_Value == CE.Contact_Hash_Value)) //has contact
                                {
                                    Contact Ref_Contact = parent.Contact_List.First(x => x.Hash_Value == CE.Contact_Hash_Value);
                                    List<string> Output = Ref_Contact.ToStringList(2, true);
                                    Output.ForEach(x => { e.Graphics.DrawString(x, f_subsubitem, BlackBrush, new Rectangle(col_subsubitem, height, 650, dataheight)); height += dataheight - 2; });
                                }
                            }
                            #endregion
                            #region Draw agenda items
                            foreach (Agenda_Item AI in AI_List.Where(x => x.Calendar_Date.Hour == i && x.Calendar_Date.Minute == j).ToList())
                            {
                                // Sub item
                                e.Graphics.DrawString(((AI.Time_Set ? ("[" + AI.Calendar_Date.ToString("hh:mm tt") + "] : ") : "") + AI.Name + " (From Agenda)"), f_subitem, BlackBrush, new Rectangle(col_subitem, height, 650, dataheight));
                                height += dataheight - 2;

                                // Append contact information
                                if (AI.Contact_Hash_Value.Length > 0 && parent_Calendar.CO_Filtered_List.Any(x => x.Hash_Value == AI.Contact_Hash_Value)) //has contact
                                {
                                    Contact Ref_Contact = parent.Contact_List.First(x => x.Hash_Value == AI.Contact_Hash_Value);
                                    List<string> Output = Ref_Contact.ToStringList(2, true);
                                    Output.ForEach(x => { e.Graphics.DrawString(x, f_subsubitem, BlackBrush, new Rectangle(col_subsubitem, height, 650, dataheight)); height += dataheight - 2; });
                                }
                            }
                            #endregion
                            #region Draw shopping items
                            foreach (Shopping_Item AI in SI_List.Where(x => x.Calendar_Date.Hour == i && x.Calendar_Date.Minute == j).ToList())
                            {
                                // Sub item
                                e.Graphics.DrawString(((AI.Time_Set ? ("[" + AI.Calendar_Date.ToString("hh:mm tt") + "] : ") : "") + AI.Name + " ('" + parent.Agenda_Item_List.First(x => x.ID == AI.ID).Name + "' in Agenda)"), f_subitem, BlackBrush, new Rectangle(col_subitem, height, 650, dataheight));
                                height += dataheight - 2;
                                // Append contact information

                                if (AI.Contact_Hash_Value.Length > 0 && parent_Calendar.CO_Filtered_List.Any(x => x.Hash_Value == AI.Contact_Hash_Value)) //has contact
                                {
                                    Contact Ref_Contact = parent.Contact_List.First(x => x.Hash_Value == AI.Contact_Hash_Value);
                                    List<string> Output = Ref_Contact.ToStringList(2, true);
                                    Output.ForEach(x => { e.Graphics.DrawString(x, f_subsubitem, BlackBrush, new Rectangle(col_subsubitem, height, 650, dataheight)); height += dataheight - 2; });
                                }
                            }
                            #endregion
                        }
                    }
                }
                #endregion
                #region If has Shipment for this day
                if (TRK_List.Count > 0)
                {
                    e.Graphics.DrawLine(Light_Grey_Pen, column1, height + dataheight, 840, height + dataheight);
                    e.Graphics.DrawString("Package Delivery Information:", f_subheader, BlackBrush, new Rectangle(column1, height, 650, dataheight));//, format1);
                    height += dataheight;

                    foreach (Shipment_Tracking ST in TRK_List)
                    {
                        // Get reference order
                        Order Ref_Order = parent.Order_List.First(x => x.OrderID == ST.Ref_Order_Number);

                        // If received
                        if (Curr_Print_Date.Date == ST.Received_Date && ST.Status == 0 && ST.Temp_Toggle)
                        {
                            parent.Tracking_List.FirstOrDefault(x => x == ST).Temp_Toggle = false;
                            e.Graphics.DrawString(Ref_Order.Location + " package arrived on this date", f_subitem, BlackBrush, new Rectangle(col_subitem, height, 650, dataheight));
                            height += dataheight - 2;
                        }
                        else if ((Curr_Print_Date.Date == ST.Received_Date || ST.Status == 1))// && ST.Temp_Toggle)// If expected on this date but still outstanding
                        {
                            parent.Tracking_List.FirstOrDefault(x => x == ST).Temp_Toggle = false;
                            e.Graphics.DrawString(Ref_Order.Location + " package expected to arrive on this date (and is still outstanding)", f_subitem, RedBrush, new Rectangle(col_subitem, height, 650, dataheight));
                            height += dataheight - 2;
                        }
                    }
                }
                #endregion
                #region If has Payment for this day
                if (Payment_List.Count > 0)
                {
                    e.Graphics.DrawLine(Light_Grey_Pen, column1, height + dataheight, 840, height + dataheight);
                    e.Graphics.DrawString("Payment" + (Payment_List.Count > 1 ? "s" : "") + " due:", f_subheader, BlackBrush, new Rectangle(column1, height, 650, dataheight));//, format1);
                    height += dataheight;
                    foreach (Payment pay in Payment_List)
                    {
                        e.Graphics.DrawString(pay.Company + "(xx-" + pay.Last_Four + ")", f_subitem, BlackBrush, new Rectangle(col_subitem, height, 650, dataheight));
                        height += dataheight - 2;
                    }
                }
                #endregion
                #region If has Payment Options 
                if (PO_List.Count > 0)
                {
                    e.Graphics.DrawLine(Light_Grey_Pen, column1, height + dataheight, 840, height + dataheight);
                    e.Graphics.DrawString("Payment Activities & Transactions:", f_subheader, BlackBrush, new Rectangle(column1, height, 650, dataheight));//, format1);
                    height += dataheight;
                    foreach (Payment_Options PO in PO_List)
                    {
                        string str = ("$" + String.Format("{0:0.00}", PO.Amount) + "-" + (PO.Hidden_Note.Length > 0 ? PO.Hidden_Note : PO.Note) + " → " + (PO.Type == "Deposit" ? "to " : "from ") + (PO.Payment_Company + " (xx-" + PO.Payment_Last_Four + ")"));
                        e.Graphics.DrawString(str, f_subitem, BlackBrush, new Rectangle(col_subitem, height, 650, dataheight));
                        height += dataheight - 2;
                    }
                }
                #endregion
                #region If has Account R/P Options
                if (ARP_List.Count > 0)
                {
                    e.Graphics.DrawLine(Light_Grey_Pen, column1, height + dataheight, 840, height + dataheight);
                    e.Graphics.DrawString("Accounts Payables & Receivables:", f_subheader, BlackBrush, new Rectangle(column1, height, 650, dataheight));//, format1);
                    height += dataheight;
                    foreach (Account Acc in ARP_List)
                    {
                        string temp = "";
                        switch (Acc.Type)
                        {
                            case "Payable":
                                temp = "Owe ";
                                break;
                            case "Receivable":
                                temp = "Lent ";
                                break;
                            case "Personal Deposit":
                                temp = "Deposited";
                                break;
                        }
                        string str = temp + ((temp != "Deposited") ? Acc.Payer : "") + " " + Acc.Amount + (Acc.Status == 0 ? (Acc.Type == "Payable" ? " (Paid)" : (Acc.Type == "Receivable" ? " (Received)" : "")) : "");
                        e.Graphics.DrawString(str, f_subitem, BlackBrush, new Rectangle(col_subitem, height, 650, dataheight));
                        height += dataheight - 2;
                    }
                }
                #endregion

                height += dataheight;

                End: 
                Curr_Print_Date = Curr_Print_Date.AddDays(1);
            }
            #region Disposal Area
            p.Dispose();
            f2.Dispose();
            f_subitem.Dispose();
            f_subheader_reg.Dispose();
            f_subsubitem.Dispose();
            f_subheader.Dispose();
            f4.Dispose();
            f5.Dispose();
            f3.Dispose();
            f1.Dispose();
            RedBrush.Dispose();
            BlackBrush.Dispose();
            GreyBrush.Dispose();
            LightGreyBrush.Dispose();
            Grey_Pen.Dispose();
            Light_Grey_Pen.Dispose();
            #endregion
        }

        bool Loaded = false;

        private void Calculate_Months()
        {
            // Only check after loaded
            if (Loaded)
            {
                try
                {
                    From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, from_day.SelectedIndex + 1);
                    To_Date = new DateTime(Convert.ToInt32(to_year.Text), to_month.SelectedIndex + 1, to_day.SelectedIndex + 1);
                }
                catch
                {
                    from_day.Text = "01";
                    to_day.Text = "01";

                    from_month.Text = from_month.Items[From_Date.Month - 1].ToString();
                    to_month.Text = to_month.Items[To_Date.Month - 1].ToString();

                    from_year.Text = From_Date.Year.ToString();
                    to_year.Text = To_Date.Year.ToString();

                    From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, 1);
                    To_Date = new DateTime(Convert.ToInt32(to_year.Text), to_month.SelectedIndex + 1, 1);
                }

                // If invalid date selection, set dates to be hte same
                if (From_Date > To_Date)
                {
                    from_month.Text = to_month.Text = mfi.GetMonthName(DateTime.Now.Month);
                    from_year.Text = to_year.Text = (DateTime.Now.Year).ToString();

                    From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, 1);
                    To_Date = new DateTime(Convert.ToInt32(to_year.Text), to_month.SelectedIndex + 1, 1);
                }
                else
                {
                }
            }
        }

        private void Apply_Toggle_Style()
        {

            hide_empty.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            hide_empty.Size = new Size(68, 25);
            hide_empty.OnText = "On";
            hide_empty.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            hide_empty.OnForeColor = Color.White;
            hide_empty.OffText = "Off";
            hide_empty.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            hide_empty.OffForeColor = Color.White;

            show_spent.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            show_spent.Size = new Size(68, 25);
            show_spent.OnText = "On";
            show_spent.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            show_spent.OnForeColor = Color.White;
            show_spent.OffText = "Off";
            show_spent.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            show_spent.OffForeColor = Color.White;
        }

        private void hide_empty_CheckedChanged(object sender, EventArgs e)
        {
            Ignore_Empty_Dates = hide_empty.Checked;
        }

        private void show_spent_CheckedChanged(object sender, EventArgs e)
        {
            Show_Spent_Figures = show_spent.Checked;
        }

    }
}
