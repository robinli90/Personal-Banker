using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace Financial_Journal
{
    public partial class Purchases : Form
    {

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }

        private int Entries_Per_Page = 20;
        int Pages_Required = 0;
        int Current_Page = 0;
        List<Order> Current_Order_Paint_List = new List<Order>();

        protected override void OnPaint(PaintEventArgs e)
        {
            int data_height = 20;
            //int start_height = Start_Size.Height + 18;
            int start_height = External_View ? 66 + (ref_Payment != null ? 15 : 0) : (bufferedPanel1.Location.Y + 90);
            int height_offset = 9;

            int start_margin = 15;              //Location
            int margin1 = start_margin + 240;   //Date
            int margin2 = margin1 + 100;        //Quantity of Items
            int margin3 = margin2 + 100;        //Total Order Amount
            int margin4 = margin3 + 120;         //Payment

            int row_count = 0;

            Color DrawForeColor = Color.White;
            Color BackColor = Color.FromArgb(64, 64, 64);
            Color HighlightColor = Color.FromArgb(76, 76, 76);

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(88, 88, 88));
            SolidBrush RedBrush = new SolidBrush(Color.LightPink);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 2);

            Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
            Font f = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
            Font f_italic_memo = new Font("MS Reference Sans Serif", 9, FontStyle.Italic);
            Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);

            // If has order
            if (paint)
            {

                // Draw gray header line
                e.Graphics.DrawLine(Grey_Pen, start_margin, start_height - 10, Start_Size.Width - 15, start_height - 10);

                // Header
                e.Graphics.DrawString("Location", f_header, WritingBrush, start_margin, start_height + (row_count * data_height));
                e.Graphics.DrawString("Order Date", f_header, WritingBrush, margin1, start_height + (row_count * data_height));
                e.Graphics.DrawString("# of Items", f_header, WritingBrush, margin2, start_height + (row_count * data_height));
                e.Graphics.DrawString("Total Amount", f_header, WritingBrush, margin3, start_height + (row_count * data_height));
                e.Graphics.DrawString("Payment Type", f_header, WritingBrush, margin4 + 15, start_height + (row_count * data_height));
                row_count += 1;

                int item_index = 0;
                double ongoing_total = 0;

                Search_Order_Button.ForEach(button => button.Image.Dispose());
                Search_Order_Button.ForEach(button => button.Dispose());
                // Remove existing buttons
                Search_Order_Button.ForEach(button => this.Controls.Remove(button));
                Search_Order_Button = new List<Button>();

                // If just basic purchase view
                if (!External_View)
                {
                    Current_Order_Paint_List = Current_Order_List.Where(x => (payment_type.Text == "All" ? true : x.Payment_Type == payment_type.Text)).ToList();
                }
                // Initial load of special payment view
                else if (!period_change)
                {
                    string Date_String = DateTime.Now.Year + "-" + DateTime.Now.Month.ToString("D2") + "-" + 
                                        (Convert.ToInt32(DateTime.Now.Month == 2 && Convert.ToInt32(ref_Payment.Billing_Start) > 28 
                                            ? "28" : 
                                            (ref_Payment.Billing_Start == "31" ? 
                                                DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month).ToString() : 
                                                ref_Payment.Billing_Start))).ToString("D2");

                    DateTime End_Date = DateTime.ParseExact(Date_String, "yyyy-MM-dd",
                                               System.Globalization.CultureInfo.InvariantCulture);
                    if (DateTime.Now.Day > Convert.ToInt32(ref_Payment.Billing_Start)) End_Date = End_Date.AddMonths(1);

                    DateTime Start_Date = End_Date.AddMonths(-1 + (End_Date.Date == DateTime.Now.Date ? 1 : 0));
                    //Billing_End = Billing_End.AddDays(-1);
                    End_Date = Start_Date.AddMonths(1).AddDays(-1);

                    //Current_Order_Paint_List = parent.Order_List.Where(x => x.Payment_Type == (ref_Payment.Company + " (xx-" + ref_Payment.Last_Four + ")") && x.Date > Start_Date && x.Date < End_Date).ToList();
                    Current_Order_Paint_List = parent.Order_List.Where(x => x.Payment_Type == (ref_Payment.Company + " (xx-" + ref_Payment.Last_Four + ")") && x.Date.Date >= Start_Date.Date && x.Date.Date < End_Date.Date).ToList();

                    next_page_button.Visible = Pages_Required > 1;

                    if (Current_Order_Paint_List.Count == 0)
                    {
                        periodbox2.SelectedIndex = 0;
                        period_change = true;
                    }
                }
                // Changing payment period view
                if (External_View && period_change)
                {

                    //DateTime End_Date = Convert.ToDateTime(periodbox2.Text.Split(new string[] { " - " }, StringSplitOptions.None)[2].Trim());
                    //DateTime Start_Date = End_Date.AddMonths(-1);
                    //Start_Date = Start_Date.AddDays(1);
                    //End_Date = End_Date.AddDays(1);

                    DateTime Start_Date = Convert.ToDateTime(periodbox2.Text.Split(new string[] { " - " }, StringSplitOptions.None)[1].Trim());

                    DateTime End_Date = Start_Date.AddMonths(1);

                    Current_Order_Paint_List = parent.Order_List.Where(x => x.Payment_Type == (ref_Payment.Company + " (xx-" + ref_Payment.Last_Four + ")") && x.Date.Date >= Start_Date.Date && x.Date.Date < End_Date.Date).ToList();

                    next_page_button.Visible = Pages_Required > 1;
                }

                ongoing_total = Current_Order_Paint_List.Sum(x => x.Order_Total_Pre_Tax + x.Order_Taxes - (External_View ? x.GC_Amount : 0));

                Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)Current_Order_Paint_List.Count() / (decimal)Entries_Per_Page));

                // sort by date desc
                Current_Order_Paint_List = Current_Order_Paint_List.OrderByDescending(x => x.Date).ToList();

                // For each refund item
                //foreach (Order order in Current_Order_Paint_List)
                foreach (Order order in Current_Order_Paint_List.GetRange(Current_Page * Entries_Per_Page, (Current_Order_Paint_List.Count - Entries_Per_Page * Current_Page) >= Entries_Per_Page ? Entries_Per_Page : (Current_Order_Paint_List.Count % Entries_Per_Page)))
                {

                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button refund_button = new Button();
                    refund_button.BackColor = this.BackColor;
                    refund_button.ForeColor = this.BackColor;
                    refund_button.FlatStyle = FlatStyle.Flat;
                    refund_button.Image = global::Financial_Journal.Properties.Resources.magnifier;
                    refund_button.Size = new Size(23, 23);
                    refund_button.Location = new Point(Start_Size.Width - 42, start_height + height_offset + (row_count * data_height) - 4 + ((order.OrderMemo.Length > 1) ? 9 : 0));
                    refund_button.Name = "b" + (Current_Page * Entries_Per_Page + item_index).ToString();
                    refund_button.Text = "";
                    refund_button.Click += new EventHandler(this.view_order_Click);
                    Search_Order_Button.Add(refund_button);
                    ToolTip1.SetToolTip(refund_button, "View Receipt");
                    this.Controls.Add(refund_button);

                    int Order_Item_Count = parent.Master_Item_List.Where(x => x.OrderID == order.OrderID).ToList().Sum(x => x.Quantity);


                    e.Graphics.DrawString(order.Location, f, WritingBrush, start_margin + 6, start_height + height_offset + (row_count * data_height));
                    if (order.OrderMemo.Length > 1) height_offset += 9; 
                    e.Graphics.DrawString(order.Date.ToShortDateString(), f, WritingBrush, margin1, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(Order_Item_Count.ToString(), f, WritingBrush, margin2 + 35 - (Order_Item_Count > 9 ? 4 : 0), start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", order.Order_Total_Pre_Tax + order.Order_Taxes - (External_View ? order.GC_Amount : 0)), f, WritingBrush, margin3 + 16, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(order.Payment_Type, f, WritingBrush, margin4 - 5, start_height + height_offset + (row_count * data_height));
                    if (order.OrderMemo.Length > 1) height_offset -= 9;


                    //ongoing_total += order.Order_Total_Pre_Tax + order.Order_Taxes;

                    row_count++;

                    // has GC
                    if (order.GC_Amount > 0)
                    {
                        string GC_Str = "Gift card(s) used: ";
                        foreach (GC GCard in parent.GC_List)
                        {
                            if (GCard.Associated_Orders.Contains(order.OrderID))
                            {
                                GC_Str += GCard.Location + " ($" + String.Format("{0:0.00}", GCard.Get_Order_Amount(order.OrderID)) + "), ";
                            }
                        }
                        height_offset -= 5;
                        e.Graphics.DrawString(GC_Str.Trim().Trim(','), f_asterisk, WritingBrush, start_margin + 20, start_height + height_offset + (row_count * data_height));
                        height_offset -= 6;
                        row_count++;
                    }

                    if (order.OrderMemo.Length > 1)
                    {
                        height_offset -= 5;
                        e.Graphics.DrawString(order.OrderMemo, f_italic_memo, WritingBrush, start_margin + 20, start_height + height_offset + (row_count * data_height));
                        row_count++;
                    }

                    item_index++;
                }

                height_offset += 6;

                //Current_Order_Paint_List = Current_Order_List.Where(x => (payment_type.Text == "All" ? true : x.Payment_Type == payment_type.Text)).ToList();
                //ongoing_total = Current_Order_Paint_List.Sum(x => x.Order_Total_Pre_Tax + x.Order_Taxes);

                e.Graphics.DrawLine(p, start_margin, start_height + height_offset + (row_count * data_height), Start_Size.Width - 15, start_height + height_offset + (row_count * data_height));
                height_offset += 6;
                e.Graphics.DrawString("Total", f_total, WritingBrush, margin2 + 20, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", ongoing_total), f_total, WritingBrush, margin3 + 7 + ((ongoing_total < 1000) ? 7 : 0), start_height + height_offset + (row_count * data_height));

                // Draw accounting double lines
                height_offset += 19;
                e.Graphics.DrawLine(p, margin3 + 7, start_height + height_offset + (row_count * data_height), margin3 + 85, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawLine(p, margin3 + 7, start_height + height_offset + 2 + (row_count * data_height), margin3 + 85, start_height + height_offset + 2 + (row_count * data_height));
                height_offset -= 11;

                row_count++;
                height_offset += 10;
                this.Height = start_height + height_offset + row_count * data_height;
            }
            else
            {
                //this.Height = Start_Size.Height;
                this.Height = start_height + height_offset + row_count * data_height;
                paint = false;
            }

            TFLP.Size = new Size(this.Size.Width - 2, this.Size.Height - 2);

            // Dispose all objects
            p.Dispose();
            Grey_Pen.Dispose();
            GreenBrush.Dispose();
            RedBrush.Dispose();
            GreyBrush.Dispose();
            WritingBrush.Dispose();
            f_asterisk.Dispose();
            f.Dispose();
            f_strike.Dispose();
            f_total.Dispose();
            f_header.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        bool External_View = false;
        Payment ref_Payment = new Payment();

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;
        private List<Order> Current_Order_List = new List<Order>();
        bool paint = true;
        private List<Button> Search_Order_Button = new List<Button>();

        public Purchases(Receipt _parent, bool external_View = false, Payment payment = null, Point g = new Point(), Size s = new Size())
        {
            External_View = external_View;
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            if (External_View)
            {
                bufferedPanel1.Visible = false;
                ref_Payment = payment;

                if (payment != null)
                {
                    bufferedPanel2.Visible = true;
                    label2.Visible = true;
                    periodbox2.Visible = true;

                    // Populate period box
                    string Date_String = DateTime.Now.Year + "-" + DateTime.Now.Month.ToString("D2") + "-" + (Convert.ToInt32(DateTime.Now.Month == 2 && Convert.ToInt32(ref_Payment.Billing_Start) > 28 ? "28" : (ref_Payment.Billing_Start == "31" ? DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month).ToString() : ref_Payment.Billing_Start))).ToString("D2");                  
                    DateTime Ref_Date_Start = parent.Order_List.Min(x => x.Date);
                    DateTime Billing_End = DateTime.ParseExact(Date_String, "yyyy-MM-dd",
                                               System.Globalization.CultureInfo.InvariantCulture);
                    if (DateTime.Now.Day > Convert.ToInt32(ref_Payment.Billing_Start)) Billing_End = Billing_End.AddMonths(1);

                    DateTime Billing_Start = Billing_End.AddMonths(-1 + (Billing_End.Date == DateTime.Now.Date ? 1 : 0));
                    //Billing_End = Billing_End.AddDays(-1);
                    Billing_End = Billing_Start.AddMonths(1);

                    while (Billing_Start > Ref_Date_Start.AddMonths(-1))
                    {
                        List<Order> Temp_List = parent.Order_List.Where(x => x.Payment_Type == (ref_Payment.Company + " (xx-" + ref_Payment.Last_Four + ")") && x.Date.Date.Date >= Billing_Start.Date && x.Date.Date < Billing_End.Date).ToList();
                        if (Temp_List.Count > 0)
                        {
                            periodbox2.Items.Add("Statement - " + Billing_Start.ToShortDateString() + " - " + Billing_End.ToShortDateString() + " - $" + String.Format("{0:0.00}", Temp_List.Sum(x => x.Order_Total_Pre_Tax + x.Order_Taxes - x.GC_Amount)));
                        }
                        Billing_Start = Billing_Start.AddMonths(-1);
                        Billing_End = Billing_End.AddMonths(-1);
                    }
                    periodbox2.SelectedIndex = 0;
                }
                //List<Order> Current_Order_Paint_List = Current_Order_List.Where(x => (payment_type.Text == "All" ? true : x.Payment_Type == payment_type.Text)).ToList();
            }
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        string search_setup = "";
        public bool close_this_form = false;
        public bool update = false;

        private void view_order_Click(object sender, EventArgs e)
        {
            close_this_form = false;

            Button b = (Button)sender;

            Order Ref_Order = parent.Order_List.First(x => x == Current_Order_Paint_List[Convert.ToInt32(b.Name.Substring(1))]);

            Grey_Out();
            Receipt_Report RP = new Receipt_Report(parent, Ref_Order, new Point(this.Left + 300, this.Top + 40), this, true, this.Location, this.Size);
            RP.ShowDialog();
            Grey_In();

            if (update)
            {
                if (search_setup == "date") { view_type_date_select_SelectedIndexChanged(view_type_date_select, new EventArgs()); }
                if (search_setup == "location") { Get_Orders_From_Current_Location(); }
                if (search_setup == "category") { Get_Orders_By_Category(); }

                this.Invalidate();
            }
            if (close_this_form) close_button.PerformClick();
            update = false;
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

        private void Receipt_Load(object sender, EventArgs e)
        {

            this.DoubleBuffered = true;
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            // Add combobox items
            view_type.Items.Add("Date");
            view_type.Items.Add("Location");
            view_type.Items.Add("Category");
            view_type_date_select.Items.Add("All");
            view_type_date_select.Items.Add("Past 7 days");
            view_type_date_select.Items.Add("Current Month");
            view_type_date_select.Items.Add("Last Month");
            view_type_date_select.Items.Add("Specific Month");
            view_type_date_select.Items.Add("Current Year");
            view_type_date_select.Items.Add("Specific Year");

            foreach (string category in parent.category_box.Items) { view_by_category.Items.Add(category); }

            payment_type.Items.Add("All");
            foreach (string payment in parent.payment_type.Items) { payment_type.Items.Add(payment); }

            // Copy location combobox
            foreach (string location in parent.location_box.Items) { view_by_location.Items.Add(location); }

            // Add years to box (only get the years where purchases have been made)
            List<string> Years = new List<string>();
            foreach (Item item in parent.Master_Item_List)
            {
                if (!Years.Contains(item.Date.Year.ToString()))
                {
                    Years.Add(item.Date.Year.ToString());
                    year_box.Items.Add(item.Date.Year.ToString());
                }
            }

            CustomDTP1.Value = DateTime.Now;


            // Default
            view_type.Text = "Date";
            payment_type.Text = "All";
            view_type_date_select.Text = "Past 7 days";
            view_type_date_select.Visible = true;
            CustomDTP1.Visible = false;

            //paint = false;

            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;
            ToolTip1.SetToolTip(Add_button, "Print current list");
            ToolTip1.SetToolTip(excel_button, "Export current list to Excel");

            this.Invalidate();
            Update();

            excel_button.Visible = IsApplictionInstalled("Excel") || IsApplictionInstalled("Office");
            ready = true;

            // Fade Box
            TFLP = new FadeControl();
            TFLP.Size = new Size(this.Width - 2, this.Height - 2);
            TFLP.Location = new Point(1000, 1000);
            TFLP.Visible = true;
            TFLP.BackColor = this.BackColor;
            TFLP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            TFLP.AllowDrop = true;
            TFLP.BringToFront();
            this.Controls.Add(TFLP);
            TFLP.BringToFront();

            TFLP.Opacity = 80;

            if (parent.Order_List.Count == 0)
            {
                excel_button.Enabled = false;
                Add_button.Enabled = false;
            }
        }

        public static bool IsApplictionInstalled(string p_name)
        {
            string displayName;
            RegistryKey key;

            // search in: CurrentUser
            key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (String keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;
                if (displayName != null && displayName.ToLower().Contains(p_name.ToLower()))
                {
                    return true;
                }
            }

            // search in: LocalMachine_32
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (String keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;
                if (displayName != null && displayName.ToLower().Contains(p_name.ToLower()))
                {
                    return true;
                }
            }

            // search in: LocalMachine_64
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (String keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;
                if (displayName != null && displayName.ToLower().Contains(p_name.ToLower()))
                {
                    return true;
                }
            }

            // NOT FOUND
            return false;
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

        private void Get_Orders_From_Current_Location()
        {
            Current_Page = 0;
            Current_Order_List = new List<Order>();
            List<Order> unsortedList = parent.Order_List.Where(p => p.Location == view_by_location.Text).ToList();
            Current_Order_List = unsortedList.OrderByDescending(x => x.Date).ToList();
            paint = Current_Order_List.Count > 0;
            Invalidate();
            Update();
            List<Order> Current_Order_Paint_List = Current_Order_List.Where(x => (payment_type.Text == "All" ? true : x.Payment_Type == payment_type.Text)).ToList();
            Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)Current_Order_Paint_List.Count() / (decimal)Entries_Per_Page));
            next_page_button.Visible = Pages_Required > 1;
        }

        private void Get_Orders_By_Category()
        {
            Current_Page = 0;
            Current_Order_List = new List<Order>();
            List<Item> Item_List_by_Category = parent.Master_Item_List.Where(p => p.Category == view_by_category.Text).ToList();

            foreach (Item item in Item_List_by_Category)
            {
                Order ref_Order = parent.Order_List.FirstOrDefault(x => x.OrderID == item.OrderID);
                if (!Current_Order_List.Contains(ref_Order))
                    Current_Order_List.Add(ref_Order);
            }

            paint = Current_Order_List.Count > 0;
            Invalidate();
            Update();
            List<Order> Current_Order_Paint_List = Current_Order_List.Where(x => (payment_type.Text == "All" ? true : x.Payment_Type == payment_type.Text)).ToList();
            Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)Current_Order_Paint_List.Count() / (decimal)Entries_Per_Page));
            next_page_button.Visible = Pages_Required > 1;
        }

        // If no month is passed through, return current month; else get specific month
        private void Get_Orders_From_Month(object Month = null, object Year = null)
        {
            Current_Page = 0;
            Current_Order_List = new List<Order>();
            List<Order> unsortedList = parent.Order_List.Where(p => p.Date.Month == (Month != null ? Convert.ToDateTime(Month).Month : DateTime.Now.Month) && (Year != null ? p.Date.Year == Convert.ToDateTime(Month).Year : p.Date.Year == CustomDTP1.Value.Year)).ToList();
            Current_Order_List = unsortedList.OrderByDescending(x => x.Date).ToList();
            paint = Current_Order_List.Count > 0;
            Invalidate();
            Update();
            List<Order> Current_Order_Paint_List = Current_Order_List.Where(x => (payment_type.Text == "All" ? true : x.Payment_Type == payment_type.Text)).ToList();
            Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)Current_Order_Paint_List.Count() / (decimal)Entries_Per_Page));
            next_page_button.Visible = Pages_Required > 1;
        }

        // If no month is passed through, return current month; else get specific month
        private void Get_Orders_Past_7()
        {
            Current_Page = 0;
            Current_Order_List = new List<Order>();
            List<Order> unsortedList = parent.Order_List.Where(x => x.Date >= DateTime.Now.AddDays(-7).Date).ToList();
            Current_Order_List = unsortedList.OrderByDescending(x => x.Date).ToList();
            paint = Current_Order_List.Count > 0;
            Invalidate();
            Update();
            List<Order> Current_Order_Paint_List = Current_Order_List.Where(x => (payment_type.Text == "All" ? true : x.Payment_Type == payment_type.Text)).ToList();
            Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)Current_Order_Paint_List.Count() / (decimal)Entries_Per_Page));
            next_page_button.Visible = Pages_Required > 1;
        }

        // If no month is passed through, return current month; else get specific month
        private void Get_Orders_All()
        {
            Current_Page = 0;
            Current_Order_List = new List<Order>();
            Current_Order_List = parent.Order_List.OrderByDescending(x => x.Date).ToList();
            paint = Current_Order_List.Count > 0;
            Invalidate();
            Update();
            List<Order> Current_Order_Paint_List = parent.Order_List.Where(x => (payment_type.Text == "All" ? true : x.Payment_Type == payment_type.Text)).ToList();
            Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)Current_Order_Paint_List.Count() / (decimal)Entries_Per_Page));
            next_page_button.Visible = Pages_Required > 1;
        }

        private void Get_Orders_From_Year(int Year)
        {
            Current_Page = 0;
            Current_Order_List = new List<Order>();
            List<Order> unsortedList = parent.Order_List.Where(p => p.Date.Year == Year).ToList();
            Current_Order_List = unsortedList.OrderByDescending(x => x.Date).ToList();
            paint = Current_Order_List.Count > 0;
            Invalidate();
            Update();
            List<Order> Current_Order_Paint_List = Current_Order_List.Where(x => (payment_type.Text == "All" ? true : x.Payment_Type == payment_type.Text)).ToList();
            Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)Current_Order_Paint_List.Count() / (decimal)Entries_Per_Page));
            next_page_button.Visible = Pages_Required > 1;
        }

        private void view_by_location_SelectedIndexChanged(object sender, EventArgs e)
        {
            search_setup = "location";
            back_page_button.Visible = false;
            Pages_Required = 0;
            Current_Page = 0;
            CustomDTP1.Visible = false;
            Get_Orders_From_Current_Location();
        }

        private void view_type_date_select_SelectedIndexChanged(object sender, EventArgs e)
        {
            search_setup = "date";
            back_page_button.Visible = false;
            Pages_Required = 0;
            Current_Page = 0;
            if (view_type_date_select.Text.Contains("All"))
            {
                Get_Orders_All();
            }
            if (view_type_date_select.Text.Contains("Specific Month"))
            {
                CustomDTP1.Visible = true;
                year_box.Visible = false;
            }
            else if (view_type_date_select.Text.Contains("Specific Year"))
            {
                CustomDTP1.Visible = false;
                year_box.Visible = true;
                year_box.Text = DateTime.Now.Year.ToString();
            }
            else
            {
                if (view_type_date_select.Text == "Current Month")
                {
                    Get_Orders_From_Month();
                }
                if (view_type_date_select.Text == "Past 7 days")
                {
                    Get_Orders_Past_7();
                }
                else if (view_type_date_select.Text == "Last Month")
                {
                    object j = null;
                    if (DateTime.Now.Month == 1) j = 12;
                    Get_Orders_From_Month(DateTime.Now.AddMonths(-1), j);
                }
                else if (view_type_date_select.Text == "Current Year")
                {
                    Get_Orders_From_Year(DateTime.Now.Year);
                }
                CustomDTP1.Visible = false;
                year_box.Visible = false;
            }
        }

        private void view_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            back_page_button.Visible = false;
            Pages_Required = 0;
            Current_Page = 0;
            if (view_type.Text.Contains("Date"))
            {
                view_by_location.Visible = false;
                view_type_date_select.Visible = true;
                view_by_category.Visible = false;
                CustomDTP1.Visible = false;
                Get_Orders_From_Month();
                
            }
            else if (view_type.Text.Contains("Location"))
            {
                view_by_location.Visible = true;
                view_type_date_select.Visible = false;
                view_by_category.Visible = false;
                year_box.Visible = false;
                CustomDTP1.Visible = false;
                Get_Orders_From_Current_Location();
                if (view_by_location.Items.Count > 0)
                {
                    view_by_location.Text = view_by_location.Items[0].ToString();
                }
            }
            else
            {
                view_by_location.Visible = false;
                view_type_date_select.Visible = false;
                view_by_category.Visible = true;
                CustomDTP1.Visible = false;
                if (view_by_category.Items.Count > 0)
                {
                    view_by_category.Text = view_by_category.Items[0].ToString();
                }
            }
        }

        private void CustomDTP1_ValueChanged(object sender, EventArgs e)
        {
            back_page_button.Visible = false;
            Pages_Required = 0;
            Current_Page = 0;
            Get_Orders_From_Month(CustomDTP1.Value);
        }

        private void year_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            back_page_button.Visible = false;
            Pages_Required = 0;
            Current_Page = 0;
            Get_Orders_From_Year(Convert.ToInt32(year_box.Text));
        }

        private void payment_type_SelectedIndexChanged(object sender, EventArgs e)
        {
            back_page_button.Visible = false;
            Pages_Required = 0;
            Current_Page = 0;
            //List<Order> temp_order = Current_Order_List.Where(x => (payment_type.Text == "All" ? true : x.Payment_Type == payment_type.Text)).ToList();
            //Current_Order_List = temp_order;
            paint = Current_Order_List.Count > 0;
            Invalidate();
            Update();
            next_page_button.Visible = Pages_Required > 1;
        }

        private void bufferedPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void next_page_button_Click(object sender, EventArgs e)
        {
            if (Current_Page + 1 < Pages_Required)
            {
                Current_Page++;
                back_page_button.Visible = true;
                this.Invalidate();
                this.Update();
                if (Pages_Required == Current_Page + 1) next_page_button.Visible = false;
            }
        }

        private void back_page_button_Click(object sender, EventArgs e)
        {
            if (Current_Page >= 1)
            {
                Current_Page--;
                next_page_button.Visible = true;
                this.Invalidate();
                this.Update();
                if (0 == Current_Page) back_page_button.Visible = false;
            }
        }

        int index = 0;
        double total = 0;
        string current_order_id = "";
        bool tax_override = false;

        private void printDocument1_PrintPage_1(System.Object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            int column1 = 35;           // Item name
            int column2 = column1 + 220; // Category
            int column3 = column2 + 180; // Quantity
            int column4 = column3 + 100; // Refunded status
            int column5 = column4 + 166; // Price
            int starty = 10;
            int dataheight = 15;
            int height = starty + starty;

            StringFormat format1 = new StringFormat();
            format1.Alignment = StringAlignment.Center;

            Pen p = new Pen(Brushes.Black, 2.5f);
            Font f2 = new Font("MS Reference Sans Serif", 9f);
            Font f4 = new Font("MS Reference Sans Serif", 10f, FontStyle.Bold);
            Font f5 = new Font("MS Reference Sans Serif", 9f, FontStyle.Italic);
            Font f3 = new Font("MS Reference Sans Serif", 12f, FontStyle.Bold);
            Font f1 = new Font("MS Reference Sans Serif", 14.0f, FontStyle.Bold);

            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(122, 122, 122));
            SolidBrush LightGreyBrush = new SolidBrush(Color.FromArgb(200, 200, 200));
            Pen Grey_Pen = new Pen(GreyBrush, 1);
            Pen Light_Grey_Pen = new Pen(LightGreyBrush, 1);

            /*
            if (index < 5)
            {
                e.Graphics.DrawString(tabControl1.TabPages[selected_tab].Text, f1, Brushes.Black, new Rectangle(column1, height, 650, dataheight * 2));//, format1);
                height += dataheight;
                height += dataheight;
                height += dataheight
                height += dataheight;
            }*/

            if (index == 0)
            {
                e.Graphics.DrawString("PURCHASE  REPORT", f1, Brushes.Black, new Rectangle(10, height, 650, dataheight*2));
                height += dataheight;
                height += dataheight;
                height += dataheight;
            }

            while (index < Print_Item_List.Count)
            {
                Item ref_item = Print_Item_List[index];
                if (height > e.MarginBounds.Height + 50)// + 20)
                {
                    if (tax_override)
                    {
                        e.Graphics.DrawString("*Tax has been overridden", f2, Brushes.Black, new Rectangle(639, 1030, 650, dataheight + 10));
                    }
                    tax_override = false;
                    height = starty;
                    e.HasMorePages = true;
                    return;
                }


                //if (g.Contains("`S`"))
                //{
                if (ref_item.OrderID != current_order_id)
                {
                    column1 -= 20;
                    height += 4;
                    height += 3;
                    e.Graphics.DrawLine(Grey_Pen, column1, height, 840, height);
                    height += 3;
                    current_order_id = ref_item.OrderID;
                    Order ref_Order = parent.Order_List.First(x => x.OrderID == current_order_id);
                    if (ref_Order.Tax_Overridden)
                        tax_override = true;

                    e.Graphics.DrawString(ref_Order.Location + " (" + ref_Order.Payment_Type + ")", f4, Brushes.Black, new Rectangle(column1, height, 650, dataheight));
                    e.Graphics.DrawString("Date: " + ref_Order.Date.ToShortDateString(), f4, Brushes.Black, new Rectangle(column3 - 47, height, 650, dataheight));
                    //e.Graphics.DrawString("Quantity: " + ref_Order.Order_Quantity.ToString(), f4, Brushes.Black, new Rectangle(650, height, 650, dataheight));
                    e.Graphics.DrawString("Total: $" + String.Format("{0:0.00}", ref_Order.Order_Taxes + ref_Order.Order_Total_Pre_Tax) +
                        (ref_Order.Tax_Overridden ? "*" : ""), f4, Brushes.Black, new Rectangle(700, height, 650, dataheight));
                    total += ref_Order.Order_Taxes + ref_Order.Order_Total_Pre_Tax;
                    height += dataheight;
                    if (ref_Order.OrderMemo.Length > 1)
                    {
                        e.Graphics.DrawString(" Memo: " + ref_Order.OrderMemo, f5, Brushes.Black, new Rectangle(column1, height, 650, dataheight));
                        height += dataheight;
                    }

                    // has GC
                    if (ref_Order.GC_Amount > 0)
                    {
                        string GC_Str = "Gift card(s) used: ";
                        foreach (GC GCard in parent.GC_List)
                        {
                            if (GCard.Associated_Orders.Contains(ref_Order.OrderID))
                            {
                                GC_Str += GCard.Location + " ($" + String.Format("{0:0.00}", GCard.Get_Order_Amount(ref_Order.OrderID)) + "), ";
                            }
                        }
                        e.Graphics.DrawString(GC_Str.Trim().Trim(','), f5, Brushes.Black, new Rectangle(column1, height, 650, dataheight));
                        height += dataheight;
                    }

                    column1 += 20;
                    height += 3;
                }
                e.Graphics.DrawLine(Light_Grey_Pen, column1, height + dataheight + 2, 800, height + dataheight + 2);
                e.Graphics.DrawString(ref_item.Name, f2, Brushes.Black, new Rectangle(column1, height, 650, dataheight));//, format1);
                e.Graphics.DrawString(ref_item.Category, f2, Brushes.Black, new Rectangle(column2, height, 650, dataheight));//, format1);
                e.Graphics.DrawString("Quantity: " + ref_item.Quantity.ToString(), f2, Brushes.Black, new Rectangle(column3, height, 650, dataheight));//, format1);
                if (Convert.ToInt32(ref_item.Status) > 0) e.Graphics.DrawString("Refunded: " + ref_item.Status, f2, Brushes.Black, new Rectangle(column4, height, 650, dataheight));//, format1);
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", ref_item.Get_Full_Amount(parent.Get_Tax_Amount(ref_item))), f2, Brushes.Black, new Rectangle(column5 + 30, height, 650, dataheight));//, format1);
                //}
                //else
                //{
                //    e.Graphics.DrawString(g, f2, Brushes.Black, new Rectangle(column1, height, 650, dataheight));//, format1);
                //}
                height += dataheight;
                index++;
            }
            height += dataheight;
            height += dataheight;
            e.Graphics.DrawString("Grand Total: $" + String.Format("{0:0.00}", total), f3, Brushes.Black, new Rectangle(600, height, 650, dataheight + 10));
            if (!e.HasMorePages && tax_override)
            {
                e.Graphics.DrawString("*Tax has been overridden", f2, Brushes.Black, new Rectangle(649, 1030, 650, dataheight + 10));
            } 
        }

        private List<Item> Print_Item_List = new List<Item>();

        private void Add_button_Click(object sender, EventArgs e)
        {
            tax_override = false;
            total = 0;
            index = 0;
            current_order_id = "";

            Print_Item_List = new List<Item>();
            foreach (Order o in Current_Order_Paint_List)
            {
                foreach (Item i in parent.Master_Item_List)
                {
                    if (i.OrderID == o.OrderID)
                    {
                        Print_Item_List.Add(i);
                    }
                }
            }

            if (Current_Order_Paint_List.Count > 0)
            {
                Grey_Out();
                using (var form = new Yes_No_Dialog(parent, "Are you sure you wish to print current purchase list?", "Warning", "Preview", "Print", 0, this.Location, this.Size))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {

                        if (form.ReturnValue1 == "1")
                        {
                            printDocument1.Print();
                        }
                        else
                        {
                            printPreviewDialog1.TopMost = true;
                            printPreviewDialog1.ShowDialog();
                        }
                    }
                }
                Grey_In();
            }
            else
            {
                // Get monthly income and compare
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: There is nothing to print", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
        }

        private void view_by_category_SelectedIndexChanged(object sender, EventArgs e)
        {
            search_setup = "category";
            back_page_button.Visible = false;
            Pages_Required = 0;
            Current_Page = 0;
            Get_Orders_By_Category();
        }

        private void excel_button_Click(object sender, EventArgs e)
        {
            TFLP.Height = this.Height - 2;
            TFLP.Location = new Point(1, 1);

            Cursor.Current = Cursors.WaitCursor;
            excel_button.Enabled = false;

            if (secondThreadFormHandle == IntPtr.Zero)
            {
                Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size)
                {
                };
                form.HandleCreated += SecondFormHandleCreated;
                form.HandleDestroyed += SecondFormHandleDestroyed;
                form.RunInNewThread(false);
            }

            Write_Excel();

            if (secondThreadFormHandle != IntPtr.Zero)
                PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            TFLP.Location = new Point(1000, 1000);

            Cursor.Current = Cursors.Default;
            excel_button.Enabled = true;

            if (GeneratingError)
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: Cannot overwrite existing Excel document. Please close existing 'Purchase Report' excel documents.", true, 0, this.Location, this.Size);
                FMB.Height += 18;
                FMB.ShowDialog();
                Grey_In();
                GeneratingError = false;
            }
        }

        #region Loading thread

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
        bool GeneratingError = false;

        private void Write_Excel()
        {
            // define which items to print
            Print_Item_List = new List<Item>();
            foreach (Order o in Current_Order_Paint_List)
            {
                foreach (Item i in parent.Master_Item_List)
                {
                    if (i.OrderID == o.OrderID)
                    {
                        Print_Item_List.Add(i);
                    }
                }
            }

            // create excel object
            Excel.Application excel = new Excel.Application();
            object misValue = System.Reflection.Missing.Value;
            Excel.Workbook book = excel.Workbooks.Add(misValue);

            // Void pre-existing pages
            while (book.Worksheets.Count > 1)
            {
                ((Excel.Worksheet)(book.Worksheets[1])).Delete();
            }
            Excel.Worksheet sheet = book.Worksheets[1];
            sheet.Name = "Purchase Report";

            int row = 1;

            // insert title
            sheet.Cells[1, 1] = "Purchase Report";
            sheet.Cells.get_Range("A1").Font.Bold = true;
            sheet.Cells.get_Range("A1").Font.Size = 20;
            sheet.Cells.get_Range("A1").Font.ColorIndex = 56;
            sheet.Cells.get_Range("A1", "H1").Merge();
            row+=2;

            // write header
            WriteHeader(ref sheet, ref row);

            // adjust alignment first before changing specific cells
            sheet.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            // write information
            WriteOrders(ref sheet, ref row);

            // adjust style
            sheet.Cells.Columns.AutoFit();

            // Fix first row
            sheet.Application.ActiveWindow.SplitRow = 3;
            sheet.Application.ActiveWindow.FreezePanes = true;
            // Now apply autofilter
            Excel.Range firstRow = (Excel.Range)sheet.Rows[4];
            

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Purchase Report (gen. on " + DateTime.Now.ToString("MM-dd-yyyy HH-mm") + ").xlsx");

            parent.GeneratedFilePaths_List.Add(path);

            try
            {
                if (File.Exists(path)) File.Delete(path);
                book.SaveAs(path, Excel.XlFileFormat.xlOpenXMLWorkbook);
                excel.Quit();
                Marshal.ReleaseComObject(sheet);
                Marshal.ReleaseComObject(book);
                Marshal.ReleaseComObject(excel);
                sheet = null;
                book = null;
                excel = null;
                // Garbage collection
                System.GC.GetTotalMemory(false);
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                System.GC.Collect();
                System.GC.GetTotalMemory(true);
                System.Diagnostics.Process.Start(path);
            }
            catch
            {
                GeneratingError = true;
            }

            try
            {
                //book.Close();
            }
            catch
            {
                Diagnostics.WriteLine("Error disposing EXCEL files");
            }

        }

        private void WriteHeader(ref Excel.Worksheet sheet, ref int row)
        {
            // header
            int col = 1;
            sheet.Cells[row, col++] = "Location";
            sheet.Cells[row, col++] = "Item";
            sheet.Cells[row, col++] = "Category";
            sheet.Cells[row, col++] = "Date";
            sheet.Cells[row, col++] = "Quantity";
            sheet.Cells[row, col++] = "Refunded";
            sheet.Cells[row, col++] = "Total";
            sheet.Cells[row, col++] = "Order Total";
            Excel.Range range = sheet.Cells.get_Range("A" + row.ToString(), "H" + row.ToString());
            range.Font.Bold = true;
            range.Font.Size = 14;
            range.Font.ColorIndex = 48;
            range.Cells.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            row++;
        }

        double order_total = 0;

        private void WriteOrders(ref Excel.Worksheet sheet, ref int row)
        {
            int col = 1;

            /*
             * TEMPLATE
            
                sheet.Cells[row, col++] = // Location
                sheet.Cells[row, col++] = // Item
                sheet.Cells[row, col++] = // Category
                sheet.Cells[row, col++] = // Date
                sheet.Cells[row, col++] = // Quantity
                sheet.Cells[row, col++] = // Refunded
                sheet.Cells[row, col++] = // Item Total
                sheet.Cells[row, col++] = // Order Total
            */

            Order ref_Order = new Order();
            string current_order_id = "";
            Item item = new Item();
            double search_total = 0;

            for (int i = 0; i <= Print_Item_List.Count; i++)
            {
                if (i < Print_Item_List.Count) item = Print_Item_List[i];

                // Insert total line at the end
                if ((item.OrderID != current_order_id && current_order_id != "") || i == Print_Item_List.Count)
                {

                    ref_Order = parent.Order_List.First(x => x.OrderID == current_order_id);


                    // tax row
                    if (ref_Order.Tax_Overridden || ref_Order.Order_Taxes > 0)
                    {

                        // subtotal
                        Excel.Range range3 = sheet.Cells.get_Range("F" + row.ToString(), "G" + row.ToString());
                        // Accounting line
                        range3.Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;

                        range3 = sheet.Cells.get_Range("F" + row.ToString(), "F" + row.ToString());
                        range3.Font.ColorIndex = 48;
                        range3.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "Subtotal";
                        sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", ref_Order.Order_Total_Pre_Tax);
                        sheet.Cells[row, col++] = "";

                        row++;
                        col = 1;

                        // tax total
                        range3 = sheet.Cells.get_Range("F" + row.ToString(), "F" + row.ToString());
                        range3.Font.ColorIndex = 48;
                        range3.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "Taxes";
                        sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", ref_Order.Order_Taxes);
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = ref_Order.Tax_Overridden ? "*Tax Overridden" : "";

                        row++;
                        col = 1;
                    }

                    // final total row
                    Excel.Range range2 = sheet.Cells.get_Range("H" + row.ToString(), "H" + row.ToString());
                    range2.Font.Bold = true;
                    range2.Font.Size = 12;
                    range2.Cells.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
                    range2.Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;

                    ref_Order = parent.Order_List.First(x => x.OrderID == current_order_id);
                    sheet.Cells[row, col++] = "";
                    sheet.Cells[row, col++] = "";
                    sheet.Cells[row, col++] = "";
                    sheet.Cells[row, col++] = "";
                    sheet.Cells[row, col++] = "";
                    sheet.Cells[row, col++] = "";
                    sheet.Cells[row, col++] = "";
                    //sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", order_total);
                    sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", ref_Order.Order_Taxes + ref_Order.Order_Total_Pre_Tax);
                    search_total += ref_Order.Order_Taxes + ref_Order.Order_Total_Pre_Tax;
                    sheet.Cells[row, col++] = "";

                    // has GC
                    if (ref_Order.GC_Amount > 0)
                    {
                        string GC_Str = "Gift card(s) used: ";
                        foreach (GC GCard in parent.GC_List)
                        {
                            if (GCard.Associated_Orders.Contains(ref_Order.OrderID))
                            {
                                GC_Str += GCard.Location + " ($" + String.Format("{0:0.00}", GCard.Get_Order_Amount(ref_Order.OrderID)) + "), ";
                            }
                        }
                        sheet.Cells[row, col++] = GC_Str.Trim().Trim(',');
                    }

                    row++;
                    col = 1;

                    order_total = 0;
                }
                if (i < Print_Item_List.Count)
                {
                    if (item.OrderID != current_order_id)
                    {
                        // Insert double accounting total line and total


                        Excel.Range range = sheet.Cells.get_Range("A" + row.ToString(), "G" + row.ToString());
                        range.Font.Bold = true;
                        range.Font.Size = 12;
                        range.Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;

                        current_order_id = item.OrderID;
                        ref_Order = parent.Order_List.First(x => x.OrderID == current_order_id);
                        if (ref_Order.Tax_Overridden)
                            tax_override = true;

                        sheet.Cells[row, col++] = ref_Order.Location;
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = ref_Order.Date.ToShortDateString();
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = ref_Order.OrderMemo;

                        // Adjust memo format
                        range = sheet.Cells.get_Range("J" + row.ToString(), "J" + row.ToString());
                        range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                        range.Font.Italic = true;

                        row++;
                        col = 1;
                    }

                    if (item.Quantity == Convert.ToInt32(item.Status))
                    {
                        Excel.Range range = sheet.Cells.get_Range("B" + row.ToString(), "G" + row.ToString());
                        range.Font.Strikethrough = true;
                        range.Font.Color = Color.Red;
                    }

                    sheet.Cells[row, col++] = "";// Location
                    sheet.Cells[row, col++] = item.Name; // Item
                    sheet.Cells[row, col++] = item.Category; // Category
                    sheet.Cells[row, col++] = "";// Date
                    sheet.Cells[row, col++] = item.Quantity;// Quantity
                    sheet.Cells[row, col++] = Convert.ToInt32(item.Status) > 0 ? item.Status : "-";// Refunded
                    sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", item.Get_Current_Amount(parent.Get_Tax_Amount(item), true) - item.Get_Current_Tax_Amount(parent.Get_Tax_Amount(item)));// Item Total
                    order_total += item.Get_Full_Amount(parent.Get_Tax_Amount(item));
                    sheet.Cells[row, col++] = "";// Order Total
                    sheet.Cells[row, col++] = "";// Order Total
                    sheet.Cells[row, col++] = item.Memo;// Order Total

                    // Adjust memo format
                    Excel.Range range5 = sheet.Cells.get_Range("J" + row.ToString(), "J" + row.ToString());
                    range5.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    range5.Font.Italic = true;


                    if (search_setup == "category" && item.Category == view_by_category.Text)
                    {
                        Excel.Range range = sheet.Cells.get_Range("B" + row.ToString(), "G" + row.ToString());
                        range.Interior.Color = Color.LightBlue;
                    }

                    col = 1;
                    row++;

                    if (item.Get_Current_Discount() > 0)
                    {

                        sheet.Cells[row, col++] = "";// Location
                        sheet.Cells[row, col++] = "";// Item
                        sheet.Cells[row, col++] = "Discount"; // Category
                        sheet.Cells[row, col++] = "";// Date
                        sheet.Cells[row, col++] = "";// Quantity
                        sheet.Cells[row, col++] = "Less";
                        sheet.Cells[row, col++] = "-$" + String.Format("{0:0.00}", item.Get_Current_Discount());// Item Total
                        order_total-=item.Discount_Amt;// Item Total
                        sheet.Cells[row, col++] = "";// Order Total

                        col = 1;
                        row++;
                    }
                }
            }

            row++;
            row++;

            col = 7;
            sheet.Cells[row, col++] = "GRAND TOTAL:";// Location
            //sheet.Cells[row, col++] = "TOTAL:";// Location
            sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", search_total);

            Excel.Range range55 = sheet.Cells.get_Range("A" + row.ToString(), "H" + row.ToString());
            range55.Font.Bold = true;
            range55.Font.Size = 14;
            range55.Font.ColorIndex = 48;

            range55 = sheet.Cells.get_Range("F" + row.ToString(), "G" + row.ToString());
            range55.Cells.Merge();

            range55 = sheet.Cells.get_Range("H" + row.ToString(), "H" + row.ToString());
            range55.Font.Color = Color.Black;
            range55.Font.Size = 14;
            range55.Cells.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
            range55.Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //range55.Cells.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //range55.Cells.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;

            //range55 = sheet.Cells.get_Range("F" + row.ToString(), "F" + row.ToString());
            //range55.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
        }

        bool period_change = false;
        bool ready = false;

        private void periodbox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ready)
            {
                back_page_button.Visible = false;
                Pages_Required = 0;
                Current_Page = 0;
                //List<Order> temp_order = Current_Order_List.Where(x => (payment_type.Text == "All" ? true : x.Payment_Type == payment_type.Text)).ToList();
                //Current_Order_List = temp_order;
                paint = Current_Order_List.Count > 0;
                period_change = true;
                Invalidate();
                Update();
                next_page_button.Visible = Pages_Required > 1;
            }
        }

    }

    class BufferedPanel : Panel
    {
        public BufferedPanel()
        {
            this.DoubleBuffered = true;
            this.ResizeRedraw = true;
        }
    }
}