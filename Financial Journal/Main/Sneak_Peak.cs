using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Financial_Journal
{
    public partial class Sneak_Peak : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }

        // Date culture
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        private List<Button> Icon_Button = new List<Button>();

        protected override void OnPaint(PaintEventArgs e)
        {
            // Remove existing buttons
            Icon_Button.ForEach(button => this.Controls.Remove(button));
            Icon_Button.ForEach(button => button.Image.Dispose());
            Icon_Button.ForEach(button => button.Dispose());
            Icon_Button = new List<Button>();

            int data_height = 16;
            int start_height = 35;
            int start_margin = 15;
            int height_offset = 9;
            int row_count = 0;

            Color DrawForeColor = Color.White;

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(88, 88, 88));
            SolidBrush RedBrush = new SolidBrush(Color.LightPink);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            SolidBrush BlueBrush = new SolidBrush(Color.LightBlue);
            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 2);
            Pen Thin_Grey_Pen = new Pen(GreyBrush, 1);

            Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
            Font f = new Font("MS Reference Sans Serif", 8, FontStyle.Regular);
            Font f_italic = new Font("MS Reference Sans Serif", 8, FontStyle.Italic);
            Font f_strike = new Font("MS Reference Sans Serif", 8, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
            Font f_title = new Font("MS Reference Sans Serif", 13, FontStyle.Bold);

            #region Title 
            string Title_Str = "";
            // if Yesterday
            if (DateTime.Now.AddDays(-1).Date == Ref_Date.Date)
            {
                Title_Str = "Yesterday";
            }
            else if (DateTime.Now.Date == Ref_Date.Date)// if Today
            {
                Title_Str = "Today";
            }
            else if (DateTime.Now.AddDays(1).Date == Ref_Date.Date)// if Tomorrow
            {
                Title_Str = "Tomorrow";
            }
            else
            {
                Title_Str = Ref_Date.ToString("ddd") + ", " + mfi.GetMonthName(Ref_Date.Month) + " " + Ref_Date.Day + ", " + Ref_Date.Year;
            }
            
            int Title_Center = this.Width/2 - (TextRenderer.MeasureText(Title_Str, f_title).Width / 2);
            e.Graphics.DrawString(Title_Str, f_title, WritingBrush, Title_Center, start_height - 3);

            row_count += 2;
            #endregion

            if (true)
            {
                #region Generate weather information

                string API_Str = JSON.Get_Weather_String(Ref_Date);
                double WeatherLow = JSON.FarToCel(Convert.ToDouble(JSON.ParseWeatherParameter(API_Str, "temperatureMin")));
                double WeatherHigh = JSON.FarToCel(Convert.ToDouble(JSON.ParseWeatherParameter(API_Str, "temperatureMax")));

                Button Show_Spending = new Button();
                Show_Spending.BackColor = this.BackColor;
                Show_Spending.ForeColor = this.BackColor;
                Show_Spending.FlatStyle = FlatStyle.Flat;
                Show_Spending.Image = JSON.Get_Weather_Icon(JSON.ParseWeatherParameter(API_Str, "icon"));
                Show_Spending.Size = new Size(34, 34);
                Show_Spending.Location = new Point(435, 27);
                Show_Spending.Name = "weather";
                Show_Spending.Text = "";
                Icon_Button.Add(Show_Spending);
                this.Controls.Add(Show_Spending);

                e.Graphics.DrawString("Low: " + WeatherLow + "°C", f_asterisk, BlueBrush, 470, 37);
                e.Graphics.DrawString("High: " + WeatherHigh + "°C", f_asterisk, RedBrush, 470, 47);

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
                List<Expenses> Expense_List = new List<Expenses>();
                List<PayPeriod> Income_Pay_Period = new List<PayPeriod>();

                // Get Calendar Events
                CE_List = parent.Calendar_Events_List.Where(x => x.Date.Date == Ref_Date.Date).ToList();
                // Get Agenda Items
                AI_List = parent.Agenda_Item_List.Where(x => x.Calendar_Date.Date == Ref_Date.Date).ToList();
                // Get Shopping Items
                parent.Agenda_Item_List.ForEach(x => SI_List.AddRange(x.Shopping_List.Where(y => y.Calendar_Date.Date == Ref_Date.Date).ToList()));
                // Get Payment Options list
                PO_List = parent.Payment_Options_List.Where(x => x.Date.Date == Ref_Date.Date).ToList();
                // Get Shipments
                TRK_List = parent.Tracking_List.Where(x => x.Status == 1 && x.Expected_Date.Date == Ref_Date.Date).ToList();
                // Get Payment Due Dates 
                Payment_List = parent.Payment_List.Where(x => (Convert.ToInt32(x.Billing_Start)) == Ref_Date.Day).ToList();
                // Get Accounts R/P List where is still outstanding and start date is less than current ref date
                ARP_List = parent.Account_List.Where(x => x.Status == 1 && x.Start_Date.Date < Ref_Date.Date).ToList();
                // Get Expense list where not autodebit
                Expense_List = parent.Expenses_List.Where(x => x.Check_Expenses(parent, 0, Ref_Date)).ToList();
                // Get Pay period that are not complete (and income must be manual)
                if (parent.Settings_Dictionary["INCOME_MANUAL"] == "1")
                {
                    foreach (CustomIncome CI in parent.Income_Company_List)
                    {
                        PayPeriod Ref_PP = CI.Intervals[CI.Intervals.Count - 1];
                        if (Ref_PP.Amount <= 0 && Ref_PP.Pay_Date.Date == Ref_Date.Date)
                        {
                            Ref_PP.Name_IUO = CI.Company;
                            Income_Pay_Period.Add(Ref_PP);
                        }
                    }
                }
                #endregion

                #region Draw calendar events
                foreach (Calendar_Events CE in CE_List)
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
                    e.Graphics.DrawString("- " + (CE.Time_Set ? ("[" + CE.Date.ToString("hh:mm tt") + "] : ") : "") + CE.Title + " (" + imp_string + ")", f, WritingBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                    row_count++;
                    string[] desc_lines = CE.Description.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    foreach (string g in desc_lines)
                    {
                        // Sub sub item
                        if (g.Length > 1)
                        {
                            e.Graphics.DrawString(g, f_header, WritingBrush, start_margin + 26, start_height + height_offset + (row_count * data_height));
                            height_offset -= 4;
                            row_count++;

                        }
                    }
                }
                #endregion
                #region Draw agenda items
                foreach (Agenda_Item AI in AI_List)
                {
                    // Sub item
                    e.Graphics.DrawString("- " + ((AI.Time_Set ? ("[" + AI.Calendar_Date.ToString("hh:mm tt") + "] : ") : "") + AI.Name + " (From Agenda)"), AI.Check_State ? f_strike : f, WritingBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                    row_count++;
                }
                #endregion
                #region Draw shopping items
                foreach (Shopping_Item AI in SI_List)
                {
                    // Sub item
                    e.Graphics.DrawString("- " + ((AI.Time_Set ? ("[" + AI.Calendar_Date.ToString("hh:mm tt") + "] : ") : "") + AI.Name + " ('" + parent.Agenda_Item_List.First(x => x.ID == AI.ID).Name + "' in Agenda)"), AI.Check_State ? f_strike : f, WritingBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                    row_count++;
                }
                #endregion

                #region Check if Line break required here
                int Grey_Line_Height = 0;
                int Grey_Line_Row_Count = row_count;

                // Grey separator line
                if (row_count > 2)
                {
                    height_offset += 5;
                    Grey_Line_Height = start_height + height_offset + (row_count * data_height);
                    height_offset += 5;
                }
                #endregion

                #region Check Expiration Dates
                List<Expiration_Entry> EE_List = new List<Expiration_Entry>();

                // Remove General items from parent list (this will allow program to check exact location first before checking the general expiration dates of items)
                for (int i = parent.Expiration_List.Count - 1; i >= 0; i--)
                {
                    if (parent.Expiration_List[i].Location == "General_Expiration")
                    {
                        EE_List.Add(parent.Expiration_List[i]);
                        parent.Expiration_List.RemoveAt(i);
                    }
                }

                // Append left-over to list
                parent.Expiration_List.AddRange(EE_List);
                
                foreach (Item item in parent.Master_Item_List.Where(x => x.Date.Date <= Ref_Date.Date).ToList())
                {
                    foreach (Expiration_Entry EE in parent.Expiration_List)
                    {
                        // Only compare same item
                        if (item.Name.Trim().ToLower().Contains(EE.Item_Name.Trim().ToLower()) && item.Name.Trim().ToLower().Length == EE.Item_Name.Trim().ToLower().Length && (EE.Location == "General_Expiration" || (EE.Location != "General_Expiration" && EE.Location == item.Location)))
                        {
                            // Check purchase dates
                            if (item.Date.AddDays(EE.Exp_Date_Count).Date >= Ref_Date.Date && item.Date.Date.AddDays(EE.Exp_Date_Count - EE.Warning_Date_Count) <= Ref_Date.Date)
                            {
                                int Days_Till_Expiry = (int)Math.Round((item.Date.AddDays(EE.Exp_Date_Count).Date - Ref_Date.Date).TotalDays);
                                // warn
                                e.Graphics.DrawString(item.Name + " purchased from " + item.Location + " is expiring " + (Days_Till_Expiry == 0 ? "today" : " in " + Days_Till_Expiry + " day(s)"), f, (Days_Till_Expiry == 0 ? RedBrush : WritingBrush), start_margin + 10, start_height + height_offset + (row_count * data_height));
                                row_count++;
                            }
                            break;
                        }
                    }
                }
                #endregion

                #region Recurring Expenses due not auto debit
                foreach (Expenses exp in Expense_List)
                {
                    // Sub item
                    e.Graphics.DrawString("- " + "Recurring expense " + exp.Expense_Name + " is due for amount " + "$" + (String.Format("{0:0.00}", exp.Expense_Amount) + (exp.Expense_Payee.Length > 1 ? " to " + exp.Expense_Payee : "")), f, WritingBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                    row_count++;
                }
                #endregion
                #region Show Payment Due Dates
                foreach (Payment payment in Payment_List)
                {
                    // Sub item
                    //e.Graphics.DrawString("- " + , f, WritingBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("- Payment: " + payment.Company + "(xx-" + payment.Last_Four + ") is due", f, WritingBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                    row_count++;
                }
                #endregion
                #region Show if manual deposit required
                foreach (PayPeriod PP in Income_Pay_Period)
                {
                    e.Graphics.DrawString("- Expecting Pay Cheque from " + PP.Name_IUO, f, WritingBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                    row_count++;
                }
                #endregion
                #region Get outstanding delivery expected to arrive on date
                foreach (Shipment_Tracking ST in TRK_List)
                {
                    e.Graphics.DrawString("- Expecting Delivery from " + parent.Order_List.FirstOrDefault(x => x.OrderID == ST.Ref_Order_Number).Location + (ST.Tracking_Number.Length > 1 ? " with tracking #: " + ST.Tracking_Number : "") , f, WritingBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                    row_count++;
                }
                #endregion
                #region Show oustanding Account R/P
                foreach (Account Acc in ARP_List)
                {
                    string temp = "";
                    switch (Acc.Type)
                    {
                        case "Payable":
                            temp = "You borrowed ";
                            break;
                        case "Receivable":
                            temp = "You lent ";
                            break;
                        case "Personal Deposit":
                            temp = "";
                            break;
                    }
                    // Ignore deposits
                    if (temp.Length > 0)
                    {
                        // Sub item
                        //e.Graphics.DrawString("- " + , f, WritingBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawString("- " + temp + Acc.Amount + " from " + Acc.Payer + " and is overdue by " + (int)(Ref_Date - Acc.Start_Date).TotalDays + " day(s) and is outstanding", f, WritingBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                        row_count++;
                        if (Acc.Remark.Length > 1)
                        {
                            e.Graphics.DrawString("Remark: " + Acc.Remark, f, WritingBrush, start_margin + 26, start_height + height_offset + (row_count * data_height));
                            row_count++;
                        }
                    }
                }
                #endregion
                #region Show Payment Transactions
                if (show_percent.Checked)
                {
                    foreach (Payment_Options PO in PO_List)
                    {
                        string str = ("$" + String.Format("{0:0.00}", PO.Amount) + "-" + (PO.Hidden_Note.Length > 0 ? PO.Hidden_Note : PO.Note) + " → " + (PO.Type == "Deposit" ? "to " : "from ") + (PO.Payment_Company + " (xx-" + PO.Payment_Last_Four + ")"));
                        e.Graphics.DrawString("- " + str, f, WritingBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                        row_count++;
                    }
                }
                #endregion

                #region Check if Line break still required (if nothing below line, don't draw it)
                // If has something below grey line, then draw grey line
                if (Grey_Line_Row_Count < row_count)
                {
                    e.Graphics.DrawLine(Thin_Grey_Pen, 30, Grey_Line_Height, this.Width - 30, Grey_Line_Height);
                }
                #endregion

            }

            if (row_count > 2)
            {
                this.Height = start_height + height_offset + row_count * data_height + 40;
            }
            else
            {
                string Empty_Day_Str = "Nothing here!";
                int Empty_Center = this.Width / 2 - (TextRenderer.MeasureText(Empty_Day_Str, f).Width / 2);
                e.Graphics.DrawString(Empty_Day_Str, f, WritingBrush, Empty_Center, start_height + height_offset + (row_count * data_height));
                this.Height = 143;
            }

            TFLP.Size = new Size(this.Width - 2, this.Height - 2);

            parent.DrawRoundedRectangle(e.Graphics, Grey_Pen, new Rectangle(start_margin, start_height + 30, this.Width - (start_margin * 2), this.Height - (start_height * 2) - 30), 6);

            this.Location = new Point(parentLocation.X + (parentSize.Width / 2) - (this.Width / 2), parentLocation.Y + (parentSize.Height / 2) - (this.Height / 2));

            // Dispose all objects
            p.Dispose();
            Grey_Pen.Dispose();
            Thin_Grey_Pen.Dispose();
            GreenBrush.Dispose();
            RedBrush.Dispose();
            GreyBrush.Dispose();
            WritingBrush.Dispose();
            f_asterisk.Dispose();
            BlueBrush.Dispose();
            f.Dispose();
            f_strike.Dispose();
            f_total.Dispose();
            f_header.Dispose();
            f_italic.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);

        }

        Receipt parent;
        Size Start_Size = new Size();
        Size parentSize = new Size();
        Point parentLocation = new Point();

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Sneak_Peak(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            parentSize = s;
            parentLocation = g;
        }

        private DateTime Ref_Date;

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

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

            Ref_Date = DateTime.Now;

            TFLP.Opacity = 80;
        }

        FadeControl TFLP;

        private void Grey_Out()
        {
            TFLP.Location = new Point(1, 1);
        }

        // Form mnemonics
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    {
                        back_page_button.PerformClick();
                        return true;
                    }
                case Keys.Right:
                    {
                        next_page_button.PerformClick();
                        return true;
                    }
            }
            return base.ProcessCmdKey(ref msg, keyData);
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

        private void next_page_button_Click(object sender, EventArgs e)
        {
            Ref_Date = Ref_Date.AddDays(1);
            Invalidate();
        }

        private void back_page_button_Click(object sender, EventArgs e)
        {
            Ref_Date = Ref_Date.AddDays(-1);
            Invalidate();
        }

        private void show_percent_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
