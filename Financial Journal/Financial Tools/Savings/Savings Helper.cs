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
    public partial class Savings_Helper : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }

        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        private List<Button> Direction_Arrow_Buttons = new List<Button>();
        int orig_margin3 = 0;
        bool first_load = true;

        protected override void OnPaint(PaintEventArgs e)
        {
            Current_Salary_Index = 0;

            int data_height = 25;
            int start_height = Start_Size.Height + 10;
            int start_margin = 15;
            int height_offset = 9;
            //Information
            int margin1 = start_margin + 15;       //Amount
            int margin2 = margin1 + 100;             //Frequency 
            int margin22 = margin2 + 170;             //Frequency 
            int margin3 = margin22 + 150;             //% of income

            // Adjust width
            if (checkBox2.Checked)
            {
                if (!first_load)
                {
                    margin3 = orig_margin3;
                    this.Width = Start_Size.Width + 87;
                }
                else
                {
                    first_load = false;
                    orig_margin3 = margin3;
                    margin3 = margin3 - 170;
                }
            }
            else
            {
                margin3 = margin3 - 170;
                this.Width = Start_Size.Width - 87;
            }

            int margin4 = margin3 + 120;            //% of income
            int margin5 = margin4 + 120;            //% of income
            int margin6 = margin5 + 120;            //% of income


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
            Font f_italic = new Font("MS Reference Sans Serif", 8, FontStyle.Italic);
            Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);
            Font f_title = new Font("MS Reference Sans Serif", 11, FontStyle.Bold);


            // Draw gray header line
            e.Graphics.DrawLine(Grey_Pen, start_margin, start_height, this.Width - start_margin, start_height);
            e.Graphics.DrawString("12-month Savings Summary", f_title, WritingBrush, start_margin, start_height + (row_count * data_height) + 10);
            row_count++;
            row_count++;

            // Headers
            e.Graphics.DrawString("Month", f_header, WritingBrush, margin1, start_height + (row_count * data_height));
            e.Graphics.DrawString("Expenditure" + ((checkBox2.Checked) ? " w/ Exp." : ""), f_header, WritingBrush, margin2 + ((checkBox2.Checked) ? 0 : 20), start_height + (row_count * data_height));
            if (checkBox2.Checked) e.Graphics.DrawString("Total Expenses", f_header, WritingBrush, margin22, start_height + (row_count * data_height));
            e.Graphics.DrawString("Earnings", f_header, WritingBrush, margin3, start_height + (row_count * data_height));
            e.Graphics.DrawString("Net Savings", f_header, WritingBrush, margin4, start_height + (row_count * data_height));
            e.Graphics.DrawString("Savings Ratio", f_header, WritingBrush, margin5, start_height + (row_count * data_height));
            e.Graphics.DrawString("vs. Goal", f_header, WritingBrush, margin6 + 22, start_height + (row_count * data_height));
            row_count++;

            Direction_Arrow_Buttons.ForEach(button => button.Image.Dispose());
            Direction_Arrow_Buttons.ForEach(button => button.Dispose());
            Direction_Arrow_Buttons.ForEach(button => this.Controls.Remove(button));
            Direction_Arrow_Buttons = new List<Button>();

            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;

            List<Help_Viewer> YTD_Info = new List<Help_Viewer>();

            // Populate information 
            for (int i = 0; i < 12; i++)
            {
                // Get current expenditure
                Help_Viewer Temp = new Help_Viewer();
                Temp.Current_Month_Expenditure = 0;
                Temp.Current_Month_Expenses = 0;

                Temp.Current_Month_Expenditure = (parent.Order_List.Where(x => x.Date.Month == month && x.Date.Year == year).ToList()).Sum(x => x.Order_Total_Pre_Tax + x.Order_Taxes);


                if (checkBox2.Checked)
                {
                    Temp.Current_Month_Expenses += (parent.Expenses_List).Sum(x => x.Get_Total_Paid(
                        //          From date          To date (first date of next month minus a day)
                            new DateTime(year, month, 1), (new DateTime(year, month, 1).AddMonths(1))
                        ));
                    Temp.Current_Month_Expenditure += Temp.Current_Month_Expenses;
                }

                Temp.Current_Month_Earnings = Get_Monthly_Salary(month, year);
                Temp.Month = month;
                Temp.Year = year;
                Temp.Savings_Diff = 0;

                YTD_Info.Add(Temp);

                //e.Graphics.DrawString("% of Income", f_header, WritingBrush, margin3, start_height + (row_count * data_height));

                // Reduce by years.
                month--;
                if (month < 1)
                {
                    year--;
                    month = 12;
                }
            }

            margin2 += 20;
            margin4 += 8;

            // Display information
            for (int i = 0; i < 12; i++)
            {
                Help_Viewer h = YTD_Info[i];

                // Get savings amount
                double Expected_Savings_Amt = (parent.Savings.Structure == "Percentage") ? h.Current_Month_Earnings * (parent.Savings.Ref_Value / 100) : parent.Savings.Ref_Value;

                // Get expected difference
                double Savings_Diff = (h.Current_Month_Earnings - h.Current_Month_Expenditure) - Expected_Savings_Amt;
                h.Savings_Diff = Savings_Diff;

                // If no expenditure, gray out row
                WritingBrush = (h.Current_Month_Expenditure > 0) ? new SolidBrush(DrawForeColor) : new SolidBrush(Color.FromArgb(150, 150, 150));

                e.Graphics.DrawString(mfi.GetAbbreviatedMonthName(h.Month) + ", " + h.Year, f, WritingBrush, margin1, start_height + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", h.Current_Month_Expenditure), f, WritingBrush, margin2, start_height + (row_count * data_height));
                if (checkBox2.Checked) e.Graphics.DrawString("$" + String.Format("{0:0.00}", h.Current_Month_Expenses), f, WritingBrush, margin22 + 16, start_height + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", h.Current_Month_Earnings), f, WritingBrush, margin3, start_height + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", h.Current_Month_Earnings - h.Current_Month_Expenditure), f, WritingBrush, margin4 + 16, start_height + (row_count * data_height));
                e.Graphics.DrawString(Math.Round((h.Current_Month_Earnings - h.Current_Month_Expenditure) / h.Current_Month_Earnings * 100, 2).ToString() + "%", f, WritingBrush, margin5 + 22, start_height + (row_count * data_height));
                e.Graphics.DrawString((Savings_Diff >= 0 ? "+" : "-") + ("$" + String.Format("{0:0.00}", Math.Abs(Savings_Diff))), f, WritingBrush, margin6 + 30, start_height + (row_count * data_height));

                ToolTip ToolTip1 = new ToolTip();
                ToolTip1.InitialDelay = 1;
                ToolTip1.ReshowDelay = 1;

                // Net savings buttons
                if (i < 11 && h.Current_Month_Expenditure > 0)
                {
                    Button Arrow_Button = new Button();
                    Arrow_Button.BackColor = this.BackColor;
                    Arrow_Button.ForeColor = this.BackColor;
                    Arrow_Button.FlatStyle = FlatStyle.Flat;
                    Arrow_Button.Image = ((YTD_Info[i + 1].Current_Month_Earnings - YTD_Info[i + 1].Current_Month_Expenditure <= h.Current_Month_Earnings - h.Current_Month_Expenditure)) ? global::Financial_Journal.Properties.Resources.up : global::Financial_Journal.Properties.Resources.down;
                    Arrow_Button.Size = new Size(25, 25);
                    Arrow_Button.Location = new Point(margin4 - 14, start_height + (row_count * data_height) - 3);
                    Arrow_Button.Name = "a" + i.ToString(); // ad = active delete, id = inactive delete
                    Arrow_Button.Text = "";
                    //Arrow_Button.Click += new EventHandler(this.delete_expense_click);
                    Direction_Arrow_Buttons.Add(Arrow_Button);
                    Savings_Diff = (h.Current_Month_Earnings - h.Current_Month_Expenditure) - (YTD_Info[i + 1].Current_Month_Earnings - YTD_Info[i + 1].Current_Month_Expenditure);
                    string TTStr = (((i < 12) && YTD_Info[i + 1].Current_Month_Expenditure >= h.Current_Month_Expenditure) ? " Increase" : " Decrease") + " compared to last month";
                    ToolTip1.SetToolTip(Arrow_Button, "$" + String.Format("{0:0.00}", Math.Abs(Savings_Diff)) + TTStr);
                    this.Controls.Add(Arrow_Button);
                }


                // Savings x Goal
                if (i < 11 && h.Current_Month_Expenditure > 0)
                {

                    Button Arrow_Button = new Button();
                    Arrow_Button.BackColor = this.BackColor;
                    Arrow_Button.ForeColor = this.BackColor;
                    Arrow_Button.FlatStyle = FlatStyle.Flat;
                    Arrow_Button.Image = (h.Savings_Diff >= 0) ? global::Financial_Journal.Properties.Resources.up : global::Financial_Journal.Properties.Resources.down;
                    Arrow_Button.Size = new Size(25, 25);
                    Arrow_Button.Location = new Point(margin6, start_height + (row_count * data_height) - 3);
                    Arrow_Button.Name = "a" + i + 12.ToString(); // ad = active delete, id = inactive delete
                    Arrow_Button.Text = "";
                    //Arrow_Button.Click += new EventHandler(this.delete_expense_click);
                    Direction_Arrow_Buttons.Add(Arrow_Button);
                    string TTStr = (h.Savings_Diff >= 0) ? " above goal" : "  below goal";
                    //ToolTip1.SetToolTip(Arrow_Button, "$" + String.Format("{0:0.00}", Math.Abs(Savings_Diff)) + TTStr);
                    this.Controls.Add(Arrow_Button);

                }

                row_count++;
            }

            // Reset color
            WritingBrush = new SolidBrush(DrawForeColor);

            height_offset -= 9;
            e.Graphics.DrawLine(p, start_margin, start_height + height_offset + (row_count * data_height), this.Width - 19, start_height + height_offset + (row_count * data_height));
            height_offset += 9;

            // Filter YTD 
            // - Do not include where expenditure = 0
            for (int i = 11; i >= 0; i--)
            {
                if (!(YTD_Info[i].Current_Month_Expenditure > 0))
                {
                    YTD_Info.RemoveAt(i);
                }
            }

            margin1 -= (YTD_Info.Count != 12 ? 5 : 0);

            // average line
            e.Graphics.DrawString((YTD_Info.Count != 12 ? "*" : "") + "Monthly Avg.", f, WritingBrush, margin1, start_height + height_offset + (row_count * data_height));
            e.Graphics.DrawString("$" + String.Format("{0:0.00}", YTD_Info.Sum(x => x.Current_Month_Expenditure) / YTD_Info.Count), f, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
            if (checkBox2.Checked) e.Graphics.DrawString("$" + String.Format("{0:0.00}", YTD_Info.Sum(x => x.Current_Month_Expenses) / YTD_Info.Count), f, WritingBrush, margin22 + 10, start_height + height_offset + (row_count * data_height));
            e.Graphics.DrawString("$" + String.Format("{0:0.00}", YTD_Info.Sum(x => x.Current_Month_Earnings) / YTD_Info.Count), f, WritingBrush, margin3, start_height + height_offset + (row_count * data_height));
            e.Graphics.DrawString("$" + String.Format("{0:0.00}", YTD_Info.Sum(x => x.Current_Month_Earnings - x.Current_Month_Expenditure) / YTD_Info.Count), f, WritingBrush, margin4 + 15, start_height + height_offset + (row_count * data_height));
            e.Graphics.DrawString(Math.Round(YTD_Info.Sum(x => ((x.Current_Month_Earnings - x.Current_Month_Expenditure) / x.Current_Month_Earnings * 100) / YTD_Info.Count), 2).ToString() + "%", f, WritingBrush, margin5 + 23, start_height + height_offset + (row_count * data_height));
            e.Graphics.DrawString("$" + String.Format("{0:0.00}", YTD_Info.Sum(x => x.Savings_Diff) / YTD_Info.Count), f, WritingBrush, margin6 + 43, start_height + height_offset + (row_count * data_height));
            row_count++;
            
            // total line
            e.Graphics.DrawString((YTD_Info.Count != 12 ? "*" : "") + "Total", f_total, WritingBrush, margin1, start_height + height_offset + (row_count * data_height));
            e.Graphics.DrawString("$" + String.Format("{0:0.00}", YTD_Info.Sum(x => x.Current_Month_Expenditure)), f_total, WritingBrush, margin2 - 5, start_height + height_offset + (row_count * data_height));
            if (checkBox2.Checked) e.Graphics.DrawString("$" + String.Format("{0:0.00}", YTD_Info.Sum(x => x.Current_Month_Expenses)), f_total, WritingBrush, margin22 + 5, start_height + height_offset + (row_count * data_height));
            e.Graphics.DrawString("$" + String.Format("{0:0.00}", YTD_Info.Sum(x => x.Current_Month_Earnings)), f_total, WritingBrush, margin3 - 5, start_height + height_offset + (row_count * data_height));
            e.Graphics.DrawString("$" + String.Format("{0:0.00}", YTD_Info.Sum(x => x.Current_Month_Earnings - x.Current_Month_Expenditure)), f_total, WritingBrush, margin4 + 10, start_height + height_offset + (row_count * data_height));
            e.Graphics.DrawString("-", f_total, WritingBrush, margin5 + 43, start_height + height_offset + (row_count * data_height));
            e.Graphics.DrawString("$" + String.Format("{0:0.00}", YTD_Info.Sum(x => x.Savings_Diff)), f_total, WritingBrush, margin6 + 37, start_height + height_offset + (row_count * data_height));
            row_count++;

            // Excluded 0 expenditure months
            if (YTD_Info.Count != 12)
            {
                // total line
                e.Graphics.DrawString("*Please note that these values do not include months with no expenditure", f_italic, RedBrush, margin4 - 70, start_height + height_offset + (row_count * data_height));
                row_count++;
            }

            this.Height = start_height + height_offset + row_count * data_height;

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
            f_italic.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public Savings_Helper(Receipt _parent)
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            amt_box.TextChanged -= new System.EventHandler(amt_box_TextChanged);
            percentage_box.TextChanged -= new System.EventHandler(percentage_box_TextChanged);

            if (parent.Savings.Structure == "Percentage")
            {
                checkBox1.Checked = true;
                percentage_box.Text = (parent.Savings.Ref_Value).ToString();
            }
            else if (parent.Savings.Structure == "Amount")
            {
                spread_data_box.Checked = true;
                amt_box.Text = "$" + parent.Savings.Ref_Value;
            }

            amt_box.TextChanged += new System.EventHandler(amt_box_TextChanged);
            percentage_box.TextChanged += new System.EventHandler(percentage_box_TextChanged);


            alertA.TextChanged -= new System.EventHandler(alertA_CheckedChanged);
            alertB.TextChanged -= new System.EventHandler(alertB_CheckedChanged);
            alertC.TextChanged -= new System.EventHandler(alertC_CheckedChanged);

            alertA.Checked = parent.Savings.Alert_1;
            alertB.Checked = parent.Savings.Alert_2;
            alertC.Checked = parent.Savings.Alert_3;

            if (parent.Savings.Structure == "")
            {
                alertA.Enabled = false;
                alertB.Enabled = false;
                alertC.Enabled = false;
            }

            alertA.TextChanged += new System.EventHandler(alertA_CheckedChanged);
            alertB.TextChanged += new System.EventHandler(alertB_CheckedChanged);
            alertC.TextChanged += new System.EventHandler(alertC_CheckedChanged);


        }

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            parent.Background_Save();
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

        private void spread_data_box_CheckedChanged(object sender, EventArgs e)
        {
            amt_box.TextChanged -= new System.EventHandler(amt_box_TextChanged);
            percentage_box.TextChanged -= new System.EventHandler(percentage_box_TextChanged);
            if (spread_data_box.Checked)
            {
                checkBox1.CheckedChanged -= new EventHandler(checkBox1_CheckedChanged);
                checkBox1.Checked = false;
                checkBox1.CheckedChanged += new EventHandler(checkBox1_CheckedChanged);
                label3.Visible = false;
                label11.Visible = false;
                percentage_box.Visible = false;
                percentage_box.Text = "";
                amt_box.Visible = true;
            }
            amt_box.TextChanged += new System.EventHandler(amt_box_TextChanged);
            percentage_box.TextChanged += new System.EventHandler(percentage_box_TextChanged);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            amt_box.TextChanged -= new System.EventHandler(amt_box_TextChanged);
            percentage_box.TextChanged -= new System.EventHandler(percentage_box_TextChanged);
            if (checkBox1.Checked)
            {
                spread_data_box.CheckedChanged -= new EventHandler(spread_data_box_CheckedChanged);
                spread_data_box.Checked = false;
                spread_data_box.CheckedChanged += new EventHandler(spread_data_box_CheckedChanged);
                label3.Visible = true;
                percentage_box.Visible = true;
                amt_box.Visible = false;
                amt_box.Text = "$";
            }
            amt_box.TextChanged += new System.EventHandler(amt_box_TextChanged);
            percentage_box.TextChanged += new System.EventHandler(percentage_box_TextChanged);
        }

        private void amt_box_TextChanged(object sender, EventArgs e)
        {
            if (!(amt_box.Text.StartsWith("$")))
            {
                if (parent.Get_Char_Count(amt_box.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = amt_box.Text;
                    amt_box.Text = temp.Substring(1) + temp[0];
                    amt_box.SelectionStart = amt_box.Text.Length;
                    amt_box.SelectionLength = 0;
                }
                else
                {
                    amt_box.Text = "$" + amt_box.Text;
                }
            }
            else if ((amt_box.Text.Length > 1) && ((parent.Get_Char_Count(amt_box.Text, Convert.ToChar(".")) > 1) || (amt_box.Text[1].ToString() == ".") || (parent.Get_Char_Count(amt_box.Text, Convert.ToChar("$")) > 1) || (!((amt_box.Text.Substring(amt_box.Text.Length - 1).All(char.IsDigit))) && !(amt_box.Text[amt_box.Text.Length - 1].ToString() == "."))))
            {
                amt_box.TextChanged -= new System.EventHandler(amt_box_TextChanged);
                amt_box.Text = amt_box.Text.Substring(0, amt_box.Text.Length - 1);
                amt_box.SelectionStart = amt_box.Text.Length;
                amt_box.SelectionLength = 0;
                amt_box.TextChanged += new System.EventHandler(amt_box_TextChanged);
            }
            else if (amt_box.Text[amt_box.Text.Length - 1] != '.' && amt_box.Text.Length > 1 || (amt_box.Text.Length == amt_box.MaxLength && amt_box.Text[amt_box.Text.Length - 1] == '.'))
            {
                parent.Savings.Ref_Value = Convert.ToDouble(amt_box.Text.Substring(1));
                parent.Savings.Structure = "Amount";
                Invalidate();
                label12.Text = "Approx. " + (Convert.ToDouble(amt_box.Text.Substring(1)) / parent.Monthly_Income * 100).ToString() + "%";
                if (amt_box.Text.Contains("."))
                {
                    amt_box.MaxLength = 8;
                }
                else
                {
                    amt_box.MaxLength = 6;
                }
            }
        }

        private void percentage_box_TextChanged(object sender, EventArgs e)
        {
            label11.Visible = percentage_box.Text.Length > 0;
            if (percentage_box.Text.All(char.IsDigit) && percentage_box.Text.Length > 0)
            {
                parent.Savings.Ref_Value = Convert.ToDouble(percentage_box.Text);
                parent.Savings.Structure = "Percentage";
                label11.Text = "Approx. $" + (parent.Monthly_Income * (Convert.ToDouble(percentage_box.Text) / 100)).ToString();
                Invalidate();
            }
            else
            {
                // If letter in SO_number box, do not output and move CARET to end
                try
                {
                    percentage_box.Text = percentage_box.Text.Substring(0, percentage_box.Text.Length - 1);
                    percentage_box.SelectionStart = percentage_box.Text.Length;
                    percentage_box.SelectionLength = 0;
                }
                catch
                { }
            }
        }

        private void alertA_CheckedChanged(object sender, EventArgs e)
        {
            parent.Savings.Alert_1 = alertA.Checked;
        }

        private void alertB_CheckedChanged(object sender, EventArgs e)
        {
            parent.Savings.Alert_2 = alertB.Checked;
        }

        private void alertC_CheckedChanged(object sender, EventArgs e)
        {
            parent.Savings.Alert_3 = alertC.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        int Current_Salary_Index = 0;

        public double Get_Monthly_Salary(int month, int year)
        {
            // Check if manual income first
            if (parent.Settings_Dictionary.ContainsKey("INCOME_MANUAL") && parent.Settings_Dictionary["INCOME_MANUAL"] == "1")
            {
                return parent.Income_Company_List.Sum(x => x.Get_Monthly_Amount(month, year));
            }
            else
            {
                // Sequential dictionary
                List<KeyValuePair<DateTime, double>> Salary_To_Month = new List<KeyValuePair<DateTime, double>>();

                if (parent.Settings_Dictionary.ContainsKey("INCOME_CHANGE_LOG"))
                {
                    string[] d = parent.Settings_Dictionary["INCOME_CHANGE_LOG"].Split(new string[] { "," }, StringSplitOptions.None);


                    if (d[0] == "")
                    {
                        return 0;
                    }

                    for (int i = 0; i < d.Count(); i += 2)
                    {
                        Salary_To_Month.Add(new KeyValuePair<DateTime, double>(Convert.ToDateTime(d[i + 1]), Convert.ToDouble(d[i])));
                    }

                    // Reverse to get current salary to oldest salary
                    Salary_To_Month.Reverse();


                    DateTime Month_Start = new DateTime(year, month, 1);
                    DateTime Month_End = (new DateTime(year, month, 1).AddMonths(1));

                    DateTime Ref_Salary_Month = new DateTime();
                    double Ref_Salary = 0;

                    // Get the right salary index
                    for (int i = 0; i < Salary_To_Month.Count; i++)
                    {
                        if (Month_End < Salary_To_Month[Current_Salary_Index].Key) Current_Salary_Index++;
                    }

                    try
                    {
                        Ref_Salary_Month = Salary_To_Month[Current_Salary_Index].Key;
                        Ref_Salary = Salary_To_Month[Current_Salary_Index].Value / (Expenses.Weeks_In_Monthly * 7);
                    }
                    catch
                    {
                        Ref_Salary_Month = new DateTime(1900, 1, 1);
                        Ref_Salary = 0;
                    }

                    // Check if salary change existed in between current period
                    if (Month_Start <= Ref_Salary_Month && Month_End > Ref_Salary_Month)
                    {
                        double Previous_Salary = 0;
                        try
                        {
                            Previous_Salary = Salary_To_Month[Current_Salary_Index + 1].Value / (Expenses.Weeks_In_Monthly * 7);
                        }
                        catch
                        {
                            // Previous salary does not exist so we set previous salary to 0
                        }

                        // Change the salary index to previous salary (since we reversed list, this becomes previous)
                        Current_Salary_Index++;

                        //                            Newer salary                                               Older salary
                        return ((Month_End - Ref_Salary_Month).TotalDays * Ref_Salary) + ((Ref_Salary_Month - Month_Start).TotalDays * Previous_Salary);
                    }
                    // If no salary change in this period, return salary for period
                    else
                    {
                        return (Month_End - Month_Start).TotalDays * Ref_Salary;
                    }
                }
                return 0;
            }
        }
    }

    public class Help_Viewer
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public double Current_Month_Expenditure { get; set; }
        public double Current_Month_Earnings { get; set; }
        public double Current_Month_Expenses { get; set; }
        public double Savings_Diff { get; set; }
    }
}
                                        