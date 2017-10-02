using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Financial_Journal
{
    public partial class Calendar : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        private void Rearrange_Controls()
        {
            // Reorganize controls (Anchoring not working)
            textBox1.Location = new Point(this.Size.Width - 1, 0);
            textBox4.Location = new Point(0, this.Size.Height - 1);
            label15.Location = new Point(this.Width - 267, this.Height - 13);
            close_button.Left = this.Width - 19;
            settings_button.Left = this.Width - 55;
            export.Left = this.Width - 55;
            import.Left = this.Width - 55;
            email_button.Left = this.Width - 55;
            undo_import.Left = this.Width - 55;
            weatherbutton.Left = this.Width - 55;
            button2.Left = this.Width - 55;
            screenshot_button.Left = this.Width - 55;
            edit_calendar.Left = this.Width - 55;
            next_page_button.Left = this.Width - 97;
        }

        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;

        protected override void WndProc(ref Message m) 
        {
            if (m.Msg == 0x84) {  // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption) {
                    m.Result = (IntPtr)2;  // HTCAPTION
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip) {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }
            }
            else if (m.Msg == 0x0112) // WM_SYSCOMMAND (ON MAXIMIZE)
            {
                // Check your window state here
                if (m.WParam == new IntPtr(0xF030)) // Maximize event - SC_MAXIMIZE from Winuser.h
                {
                    Rearrange_Controls();
                }
            }

            base.WndProc(ref m);
        }


        // Calendar events for current calendar (31 for max number of days in month)
        private List<Calendar_Events>[] Calendar_Events_Array = new List<Calendar_Events>[31];
        
        public int current_month = 0;
        public int current_year = 0;
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        // Calendar parameters preset
        int cleft, col0, col1, col2, col3, col4, col5, col6;
        int ctitle, ctop, cheader, c0, c1, c2, c3, c4, c5;
        int row_height, col_width, start_margin;

        /// <summary>
        /// Return top left index of the calendar cell
        /// </summary>
        /// <param name="Column_No"></param>
        /// <param name="Row_No"></param>
        /// <returns></returns>
        private Point Get_Calendar_Intersection(int Column_No, int Row_No, int X_Offset = 0, int Y_Offset = 0)
        {
            int X = 0, Y = 0;

            switch (Column_No)
            {
                case 1:
                    X = cleft;
                    break;
                case 2:
                    X = col0;
                    break;
                case 3:
                    X = col1;
                    break;
                case 4:
                    X = col2;
                    break;
                case 5:
                    X = col3;
                    break;
                case 6:
                    X = col4;
                    break;
                case 7:
                    X = col5;
                    break;
            }
            switch (Row_No)
            {
                case 1:
                    Y = cheader;
                    break;
                case 2:
                    Y = c0;
                    break;
                case 3:
                    Y = c1;
                    break;
                case 4:
                    Y = c2;
                    break;
                case 5:
                    Y = c3;
                    break;
                case 6:
                    Y = c4;
                    break;
            }
            return new Point(X + X_Offset, Y + Y_Offset);
        }

        private Dictionary<DateTime, double> TempLowDict;
        private Dictionary<DateTime, double> TempHighDict;
        private Dictionary<DateTime, string> WeatherIconDict;

        private void Populate_Weather_Dictionary()
        {
            DateTime CalendarDate = new DateTime(current_year, current_month, 1);

            if (DateTime.Now.Date.AddDays(7) > CalendarDate)
            {
                Grey_Out();
                Application.DoEvents();

                if (secondThreadFormHandle == IntPtr.Zero)
                {
                    Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size, "RETRIEVING", "WEATHER")
                    {
                    };
                    form.HandleCreated += SecondFormHandleCreated;
                    form.HandleDestroyed += SecondFormHandleDestroyed;
                    form.RunInNewThread(false);
                }

                TempLowDict = new Dictionary<DateTime, double>();
                TempHighDict = new Dictionary<DateTime, double>();
                WeatherIconDict = new Dictionary<DateTime, string>();


                while (CalendarDate.Month == current_month && CalendarDate.Date <= DateTime.Now.Date.AddDays(7)) //7 day forecast
                {
                    string API_Str = JSON.Get_Weather_String(CalendarDate);
                    TempLowDict.Add(CalendarDate, JSON.FarToCel(Convert.ToDouble(JSON.ParseWeatherParameter(API_Str, "temperatureMin"))));
                    TempHighDict.Add(CalendarDate, JSON.FarToCel(Convert.ToDouble(JSON.ParseWeatherParameter(API_Str, "temperatureMax"))));
                    WeatherIconDict.Add(CalendarDate, JSON.ParseWeatherParameter(API_Str, "icon"));
                    CalendarDate = CalendarDate.AddDays(1);
                }

                if (secondThreadFormHandle != IntPtr.Zero)
                    PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

                Grey_In();
            }
            else
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: Cannot forecast weather that far into the future!", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }

        }

        

        private void Initialize_Calendar_Paremeters(int PageWidth = 0, int PageHeight = 0)
        {
            //row_height = this.Height / 8;

            //start_margin = col_width / 8;
            start_margin = 20;
            col_width = (this.Width - start_margin - 58) / 7;

            ctitle = 42;
            ctop = ctitle + 50;
            cheader = ctop + 30;

            row_height = (this.Height - cheader - 25) / (Need_Last_Row ? 6 : 5);

            // Check if overriding existing dimensions
            if (PageWidth > 0 && PageHeight > 0)
            {
                col_width = (PageWidth - start_margin - 58) / 7;
                row_height = (PageHeight - cheader - 25) / (Need_Last_Row ? 6 : 5);
            }

            c0 = cheader + row_height;
            c1 = c0 + row_height;
            c2 = c1 + row_height;
            c3 = c2 + row_height;
            c4 = c3 + row_height;
            c5 = c4 + row_height;

            cleft = start_margin;
            col0 = cleft + col_width;
            col1 = col0 + col_width;
            col2 = col1 + col_width;
            col3 = col2 + col_width;
            col4 = col3 + col_width;
            col5 = col4 + col_width;
            col6 = col5 + col_width;
        }
        
        List<Button> Show_Spending_Button = new List<Button>();

        private void Populate_Calendar_Events()
        {
            for (int i = 0; i < Calendar_Events_Array.Count(); i++)
            {
                Calendar_Events_Array[i] = new List<Calendar_Events>();
            }

            foreach (Calendar_Events e in parent.Calendar_Events_List.Where(x => x.Date.Month == current_month && x.Date.Year == current_year).ToList())
            {
                Calendar_Events_Array[e.Date.Day - 1].Add(e);
            }
        }

        bool Need_Last_Row = false;
        bool showWeather = false;

        protected override void OnPaint(PaintEventArgs e)
        {
            #region Grip 
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
            rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
            #endregion

            #region Draw values first to evaluate if we need the last row
            // find start date of first date of current month
            DateTime First_Date_Cur_Month = new DateTime(current_year, current_month, 1);

            PopulateMultiDays(current_month, current_year);
            int multiLineSpacing = 20;

            int Start_Date_Number = 0;
            int Current_Month_Max_Day = new DateTime(current_year, current_month, 1).AddMonths(1).AddDays(-1).Day;

            switch (First_Date_Cur_Month.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    Start_Date_Number = 0;
                    break;
                case DayOfWeek.Monday:
                    Start_Date_Number = 1;
                    break;
                case DayOfWeek.Tuesday:
                    Start_Date_Number = 2;
                    break;
                case DayOfWeek.Wednesday:
                    Start_Date_Number = 3;
                    break;
                case DayOfWeek.Thursday:
                    Start_Date_Number = 4;
                    break;
                case DayOfWeek.Friday:
                    Start_Date_Number = 5;
                    break;
                case DayOfWeek.Saturday:
                    Start_Date_Number = 6;
                    break;
            }

            // Need_Last_Row (NLR)
            bool NLR = false;

            // Populate current month numbers
            int recurring_index = Start_Date_Number;

            Need_Last_Row = Current_Month_Max_Day - (((7 - Start_Date_Number) + 21)) > 7;

            int[] DayLabels = new int[42]; //42 6 rows, 7 columns 6x7
            for (int i = 1; i <= 42; i++)
            {
                if (i > Current_Month_Max_Day)
                {
                    break;
                }
                else
                {
                    DayLabels[recurring_index] = i;
                    recurring_index++;
                }
            }

            #endregion

            Initialize_Calendar_Paremeters();

            Populate_Calendar_Events();

            Show_Spending_Button.ForEach(button => button.Image.Dispose());
            Show_Spending_Button.ForEach(button => button.Dispose());
            Show_Spending_Button.ForEach(button => this.Controls.Remove(button));
            Show_Spending_Button = new List<Button>();

            // Reset calendar payment toggle
            parent.Payment_List.ForEach(x => x.Calendar_Toggle = 1);

            Color DrawForeColor = Color.White;
            Color HighlightColor = Color.FromArgb(76, 76, 76);

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(122, 122, 122));
            SolidBrush BlueBrush = new SolidBrush(Color.LightBlue);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            SolidBrush CyanBrush = new SolidBrush(Color.Cyan);
            SolidBrush PurpleBrush = new SolidBrush(Color.MediumPurple);
            SolidBrush RedBrush = new SolidBrush(Color.Red);
            SolidBrush YellowBrush = new SolidBrush(Color.Yellow);
            SolidBrush PinkBrush = new SolidBrush(Color.LightPink);
            SolidBrush OrangeBrush = new SolidBrush(Color.Orange);
            SolidBrush LightOrangeBrush = new SolidBrush(Color.FromArgb(255, 200, 0));

            Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
            Font f = new Font("MS Reference Sans Serif", 7.5F, FontStyle.Regular);
            Font f_minor_bold = new Font("MS Reference Sans Serif", 7.5F, FontStyle.Bold);
            Font f_minor = new Font("MS Reference Sans Serif", 6.5F, FontStyle.Regular);
            Font f_minor_strike = new Font("MS Reference Sans Serif", 6.5F, FontStyle.Strikeout);
            Font f_reg_bold = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 12, FontStyle.Bold);
            Font f_title = new Font("MS Reference Sans Serif", 24, FontStyle.Bold);

            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 1);
            Pen Blue_Pen = new Pen(BlueBrush, 1);
            Pen Green_Pen = new Pen(GreenBrush, 1);
            Pen Red_Pen = new Pen(RedBrush, 2);
            Pen Orange_Pen = new Pen(OrangeBrush, 1);
            Pen Box_Orange_Pen = new Pen(OrangeBrush, 2);
            Pen Box_Green_Pen = new Pen(CyanBrush, 2);
            Pen Purple_Pen = new Pen(PurpleBrush, 1);
            Pen Yellow_Pen = new Pen(YellowBrush, 1);

            Font graph_font = new Font("MS Reference Sans Serif", 8, FontStyle.Regular);
            Font axis_font = new Font("MS Reference Sans Serif", 6, FontStyle.Regular);

            // Predefine new Tooltip for rest of OnPaint function
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            // Populate previous and next month numbers
            for (int i = 0; i < 6; i++) // Row
            {
                for (int j = 1; j < 8; j++) // Col
                {
                    // If matrix has a date (Perform checks for that day)
                    if (DayLabels[(i * 7) + j - 1] > 0)
                    {
                        int Day_Number = DayLabels[(i * 7) + j - 1];

                        // Used to determine the max length of text from start to end of a cell
                        int Letter_Max = (int)Math.Round((col_width / 6.25));
                        
                        Point Temp_Pt = Get_Calendar_Intersection(j, i + 1, 4, 4);

                        // Weather overrides all existing drawings (only weather and day number is drawn on)
                        if (showWeather && TempLowDict != null)
                        {
                            DateTime refDate = new DateTime(current_year, current_month, Day_Number);
                            Temp_Pt = Get_Calendar_Intersection(j, i + 1, 4, 4);

                            if (TempLowDict.ContainsKey(refDate))
                            {
                                e.Graphics.DrawString("Low: " + TempLowDict[refDate] + "°C", f_reg_bold, BlueBrush, Temp_Pt.X + 39, Temp_Pt.Y + 2);
                            }
                            if (TempHighDict.ContainsKey(refDate))
                            {
                                e.Graphics.DrawString("High: " + TempHighDict[refDate] + "°C", f_reg_bold, PinkBrush, Temp_Pt.X + 39, Temp_Pt.Y + 15);
                            }
                            if (WeatherIconDict.ContainsKey(refDate))
                            {
                                Button Show_Spending = new Button();
                                Show_Spending.BackColor = this.BackColor;
                                Show_Spending.ForeColor = this.BackColor;
                                Show_Spending.FlatStyle = FlatStyle.Flat;
                                Show_Spending.Image = JSON.Get_Weather_Icon(WeatherIconDict[refDate]);
                                Show_Spending.Size = new Size(34, 34);
                                Show_Spending.Location = new Point(Temp_Pt.X + 2, Temp_Pt.Y);
                                Show_Spending.Name = "weather" + Day_Number.ToString();
                                Show_Spending.Text = "";
                                Show_Spending_Button.Add(Show_Spending);
                                ToolTip1.SetToolTip(Show_Spending, JSON.Get_Weather_String(WeatherIconDict[refDate].Substring(1, WeatherIconDict[refDate].Length - 2)));
                                this.Controls.Add(Show_Spending);

                            }

                            #region Shipping packages

                            int package_count = 1 - (parent.Settings_Dictionary["CALENDAR_TOG_1"] == "1" ? 0 : 1);

                            foreach (Shipment_Tracking ST in parent.Tracking_List.Where(x => x.Expected_Date.Day == Day_Number && x.Expected_Date.Month == current_month && x.Expected_Date.Year == current_year).ToList())
                            {
                                Button Add_Event = new Button();
                                Add_Event.BackColor = this.BackColor;
                                Add_Event.ForeColor = this.BackColor;
                                Add_Event.FlatStyle = FlatStyle.Flat;
                                Add_Event.Image = ST.Status == 0 ? global::Financial_Journal.Properties.Resources.gbox : global::Financial_Journal.Properties.Resources.box;
                                Add_Event.Size = new Size(20, 20);
                                Add_Event.Location = new Point(Temp_Pt.X + col_width - 25 - (package_count * 20), Temp_Pt.Y + row_height - 25);
                                Add_Event.Name = ST.Ref_Order_Number;
                                Add_Event.Text = "";
                                Add_Event.Click += new EventHandler(this.View_Package);
                                Show_Spending_Button.Add(Add_Event);
                                ToolTip1.SetToolTip(Add_Event, "Package " + (ST.Status == 0 ? "received" : "due") + " on " + (ST.Status == 0 ? ST.Received_Date.ToShortDateString() : "this date") + Environment.NewLine + "(TRK: " + ST.Tracking_Number + ")");
                                this.Controls.Add(Add_Event);

                                package_count++;
                            }

                            // Circle today's date number
                            //Rectangle r = new Rectangle(Temp_Pt.X + col_width - 25 - (package_count * 20), Temp_Pt.Y + row_height - 25);

                            #endregion
                        }
                        else
                        {

                            #region Get daily spending totals
                            Temp_Pt = Get_Calendar_Intersection(j, i + 1, 4, 4);
                            List<Order> Temp = parent.Order_List.Where(x => x.Date.Year == current_year && x.Date.Month == current_month && x.Date.Day == Day_Number).ToList();
                            double Daily_Total = Temp.Sum(x => x.Order_Total_Pre_Tax + x.Order_Taxes);
                            if (parent.Settings_Dictionary["CALENDAR_TOG_9"] == "0") Daily_Total = 0; // If not shown, zero it out

                            if (Daily_Total > 0)
                            {

                                Button Show_Spending = new Button();
                                Show_Spending.BackColor = this.BackColor;
                                Show_Spending.ForeColor = this.BackColor;
                                Show_Spending.FlatStyle = FlatStyle.Flat;
                                Show_Spending.Image = global::Financial_Journal.Properties.Resources.magnifier;
                                Show_Spending.Size = new Size(23, 23);
                                Show_Spending.Location = new Point(Temp_Pt.X - 2, Temp_Pt.Y - 2);
                                Show_Spending.Name = "p" + Day_Number.ToString();
                                Show_Spending.Text = "";
                                Show_Spending.Click += new EventHandler(this.view_order_Click);
                                Show_Spending_Button.Add(Show_Spending);
                                ToolTip1.SetToolTip(Show_Spending, "View Purchases on " + current_month + "/" + Day_Number + "/" + current_year);
                                this.Controls.Add(Show_Spending);

                                bool Needs_Warning = false;
                                double percentage_boundary = 0;

                                double mo_income = 1;

                                // Get Dynamic Income
                                using (Savings_Helper SH = new Savings_Helper(parent))
                                {
                                    mo_income = SH.Get_Monthly_Salary(current_month, current_year);
                                }

                                if (Convert.ToDouble(parent.Settings_Dictionary["CALENDAR_TOG_PERC"]) > 0)
                                {
                                    percentage_boundary = Convert.ToDouble(parent.Settings_Dictionary["CALENDAR_TOG_PERC"]) / 100;
                                }

                                //Needs_Warning = spending_warning.Checked && (Daily_Total / (parent.Monthly_Income > 0 ? parent.Monthly_Income : 1)) > percentage_boundary;
                                Needs_Warning = parent.Settings_Dictionary["CALENDAR_TOG_4"] == "1" && (Daily_Total / (mo_income > 0 ? mo_income : 1)) > percentage_boundary;

                                if (Needs_Warning)
                                {
                                    Button Warning_Button = new Button();
                                    Warning_Button.BackColor = this.BackColor;
                                    Warning_Button.ForeColor = this.BackColor;
                                    Warning_Button.FlatStyle = FlatStyle.Flat;
                                    Warning_Button.Image = global::Financial_Journal.Properties.Resources.exclamation;
                                    Warning_Button.Size = new Size(20, 20);
                                    Warning_Button.Location = new Point(Temp_Pt.X + 18, Temp_Pt.Y - 2);
                                    Warning_Button.Name = "w" + Day_Number.ToString();
                                    Warning_Button.Text = "";
                                    Warning_Button.Click += new EventHandler(this.view_order_Click);
                                    Show_Spending_Button.Add(Warning_Button);
                                    ToolTip1.SetToolTip(Warning_Button, "Spending beyond " + (percentage_boundary * 100) + "% of monthly income");
                                    this.Controls.Add(Warning_Button);
                                }

                                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Daily_Total), f, (Needs_Warning ? PinkBrush : BlueBrush), Temp_Pt.X + (Needs_Warning ? 37 : 18), Temp_Pt.Y + 2);

                            }

                            #endregion

                            #region Organize personal events buttons
                            if (parent.Settings_Dictionary["CALENDAR_TOG_1"] == "1" || Calendar_Events_Array[Day_Number - 1].Count > 0)
                            {
                                Button Add_Event = new Button();
                                Add_Event.BackColor = this.BackColor;
                                Add_Event.ForeColor = this.BackColor;
                                Add_Event.FlatStyle = FlatStyle.Flat;
                                //                                                                if edit, show edit.                                            Else, show add new calendar
                                Add_Event.Image = Calendar_Events_Array[Day_Number - 1].Count > 0 ? global::Financial_Journal.Properties.Resources.edit_small : global::Financial_Journal.Properties.Resources.add_calendar;
                                Add_Event.Size = new Size(20, 20);
                                Add_Event.Location = new Point(Temp_Pt.X + col_width - 25, Temp_Pt.Y + row_height - 25);
                                Add_Event.Name = "e" + Day_Number.ToString();
                                Add_Event.Text = "";
                                Add_Event.Click += new EventHandler(this.view_order_Click);
                                Show_Spending_Button.Add(Add_Event);
                                ToolTip1.SetToolTip(Add_Event, (Calendar_Events_Array[Day_Number - 1].Count > 0 ? "Edit" : "Add") + " event(s) on " + current_month + "/" + Day_Number + "/" + current_year);
                                this.Controls.Add(Add_Event);
                            }


                            #endregion

                            #region Shipping packages

                            int package_count = 1 - (parent.Settings_Dictionary["CALENDAR_TOG_1"] == "1" ? 0 : 1);

                            foreach (Shipment_Tracking ST in parent.Tracking_List.Where(x => x.Expected_Date.Day == Day_Number && x.Expected_Date.Month == current_month && x.Expected_Date.Year == current_year).ToList())
                            {
                                Button Add_Event = new Button();
                                Add_Event.BackColor = this.BackColor;
                                Add_Event.ForeColor = this.BackColor;
                                Add_Event.FlatStyle = FlatStyle.Flat;
                                Add_Event.Image = ST.Status == 0 ? global::Financial_Journal.Properties.Resources.gbox : global::Financial_Journal.Properties.Resources.box;
                                Add_Event.Size = new Size(20, 20);
                                Add_Event.Location = new Point(Temp_Pt.X + col_width - 25 - (package_count * 20), Temp_Pt.Y + row_height - 25);
                                Add_Event.Name = ST.Ref_Order_Number;
                                Add_Event.Text = "";
                                Add_Event.Click += new EventHandler(this.View_Package);
                                Show_Spending_Button.Add(Add_Event);
                                ToolTip1.SetToolTip(Add_Event, "Package " + (ST.Status == 0 ? "received" : "due") + " on " + (ST.Status == 0 ? ST.Received_Date.ToShortDateString() : "this date") + Environment.NewLine + "(TRK: " + ST.Tracking_Number + ")");
                                this.Controls.Add(Add_Event);

                                package_count++;
                            }

                            // Circle today's date number
                            //Rectangle r = new Rectangle(Temp_Pt.X + col_width - 25 - (package_count * 20), Temp_Pt.Y + row_height - 25);

                            #endregion

                            #region Draw MultiDay lines
                            DateTime refDay = new DateTime(current_year, current_month, Day_Number); 
                            if (Daily_Total > 0 || MultiDayList.Where(y => y.eventDays.Any(z => z == refDay)).Any(x => x.hasDailyTotalEntry)) Temp_Pt.Y += 19;


                            foreach (MultiDayEvents MDE in MultiDayList.Where(x => x.eventDays.Any(y => y.Date == refDay.Date)))
                            {
                                Pen refPen = new Pen(MDE.lineColor, 1);
                                Pen refPenDash = new Pen(MDE.lineColor, 1);
                                refPenDash.DashPattern = new float[] { 4.0F, 2.0F };

                                if (Daily_Total > 0) MDE.hasDailyTotalEntry = true;

                                int lineHeight = (MDE.lineIndex + 1) * multiLineSpacing - 8;
                                if (MDE.GetDateType(refDay) == DateType.Start)
                                {
                                    // DRAW START ARROW
                                    e.Graphics.DrawLine(refPenDash, Temp_Pt.X - 3, Temp_Pt.Y + 1 + lineHeight, Temp_Pt.X + col_width - 4,
                                        Temp_Pt.Y + 1 + lineHeight); // line


                                    e.Graphics.DrawLine(refPen, Temp_Pt.X - 3, Temp_Pt.Y + 1 + lineHeight, Temp_Pt.X + 6,
                                        Temp_Pt.Y - 3 + lineHeight); //arrow top
                                    e.Graphics.DrawLine(refPen, Temp_Pt.X - 3, Temp_Pt.Y + 1 + lineHeight, Temp_Pt.X + 6,
                                        Temp_Pt.Y + 5 + lineHeight); //arrow bottom
                                }
                                else if (MDE.GetDateType(refDay) == DateType.Middle)
                                {
                                    // DRAW CONTINUOUS LINE
                                    e.Graphics.DrawLine(refPenDash, Temp_Pt.X - 3, Temp_Pt.Y + 1 + lineHeight, Temp_Pt.X + col_width - 4,
                                        Temp_Pt.Y + 1 + lineHeight);
                                }
                                else
                                {
                                    // DRAW END ARROW
                                    e.Graphics.DrawLine(refPenDash, Temp_Pt.X - 3, Temp_Pt.Y + 1 + lineHeight, Temp_Pt.X + col_width - 5,
                                        Temp_Pt.Y + 1 + lineHeight);

                                    e.Graphics.DrawLine(refPen, Temp_Pt.X + col_width - 12, Temp_Pt.Y - 3 + lineHeight, Temp_Pt.X + col_width - 5,
                                        Temp_Pt.Y + 1 + lineHeight); //arrow top
                                    e.Graphics.DrawLine(refPen, Temp_Pt.X + col_width - 12, Temp_Pt.Y + 5 + lineHeight, Temp_Pt.X + col_width - 5,
                                        Temp_Pt.Y + 1 + lineHeight); //arrow bottom
                                }
                                refPen.Dispose();
                                refPenDash.Dispose();
                            }
                            #endregion

                            #region Get important+ calendar events
                            List<Calendar_Events> Important = Calendar_Events_Array[Day_Number - 1].Where(x => x.Importance >= 1).ToList();

                            #endregion

                            #region Show important+ calendar events
                            if (Important.Count > 0)
                            {
                                foreach (Calendar_Events CE in Important)
                                {
                                    if (MultiDayList.Any(x => x.calendarHash == CE.Hash_Value))
                                        Temp_Pt.Y +=
                                            (MultiDayList.First(x => x.calendarHash == CE.Hash_Value).lineIndex) * multiLineSpacing;
                                    string str = (parent.Settings_Dictionary["CALENDAR_TOG_8"] == "1" && CE.Time_Set ? "[" + CE.Date.ToString("hh:mm tt") + "]" : "") + CE.Title;
                                    e.Graphics.DrawString(str.Length < Letter_Max ? str : str.Substring(0, Letter_Max - 1) + "...", f_minor_bold, 
                                        MultiDayList.Any(x => x.calendarHash == CE.Hash_Value) ? new SolidBrush(MultiDayList.First(x => x.calendarHash == CE.Hash_Value).lineColor) :
                                        CE.Importance > 1 ? PinkBrush : YellowBrush
                                        , Temp_Pt.X, Temp_Pt.Y);
                                    Temp_Pt.Y += 11;
                                }
                            }

                            #endregion

                            #region Show Agenda items with calendarDate set on that date


                            foreach (Agenda_Item AI in parent.Agenda_Item_List.Where(x => x.Calendar_Date.Day == Day_Number && x.Calendar_Date.Month == current_month && x.Calendar_Date.Year == current_year).ToList())
                            {
                                if (parent.Settings_Dictionary["CALENDAR_TOG_7"] == "1" || !AI.Check_State)
                                {
                                    Temp_Pt.Y += 1;

                                    Button Show_Spending = new Button();
                                    Show_Spending.BackColor = this.BackColor;
                                    Show_Spending.ForeColor = this.BackColor;
                                    Show_Spending.FlatStyle = FlatStyle.Flat;
                                    Show_Spending.Image = global::Financial_Journal.Properties.Resources.magnifier;
                                    Show_Spending.Size = new Size(20, 20);
                                    Show_Spending.Location = new Point(Temp_Pt.X - 2, Temp_Pt.Y - 2);
                                    Show_Spending.Name = "q" + AI.ID;
                                    Show_Spending.Text = "";
                                    Show_Spending.Click += new EventHandler(this.view_order_Click);
                                    Show_Spending_Button.Add(Show_Spending);
                                    this.Controls.Add(Show_Spending);

                                    string str = ((parent.Settings_Dictionary["CALENDAR_TOG_8"] == "1" && AI.Time_Set ? "[" + AI.Calendar_Date.ToString("hh:mm tt") + "]" : "") + AI.Name);

                                    e.Graphics.DrawString(str.Length < Letter_Max ? str : str.Substring(0, Letter_Max - 1) + "...", AI.Check_State ? f_minor_strike : f_minor, PinkBrush, Temp_Pt.X + 18, Temp_Pt.Y + 2);
                                    Temp_Pt.Y += 16;
                                }
                            }
                            #endregion

                            #region Show Shopping items with calendarDate set on that date
                            foreach (Agenda_Item AI in parent.Agenda_Item_List)
                            {
                                foreach (Shopping_Item SI in AI.Shopping_List.Where(x => x.Calendar_Date.Day == Day_Number && x.Calendar_Date.Month == current_month && x.Calendar_Date.Year == current_year).ToList())
                                {
                                    if (parent.Settings_Dictionary["CALENDAR_TOG_7"] == "1" || !SI.Check_State)
                                    {
                                        Temp_Pt.Y += 1;

                                        Button Show_Spending = new Button();
                                        Show_Spending.BackColor = this.BackColor;
                                        Show_Spending.ForeColor = this.BackColor;
                                        Show_Spending.FlatStyle = FlatStyle.Flat;
                                        Show_Spending.Image = global::Financial_Journal.Properties.Resources.magnifier;
                                        Show_Spending.Size = new Size(20, 20);
                                        Show_Spending.Location = new Point(Temp_Pt.X - 2, Temp_Pt.Y - 2);
                                        Show_Spending.Name = "w_" + AI.ID + "_" + AI.Shopping_List.IndexOf(SI);
                                        Show_Spending.Text = "";
                                        Show_Spending.Click += new EventHandler(this.view_order_Click);
                                        Show_Spending_Button.Add(Show_Spending);
                                        this.Controls.Add(Show_Spending);

                                        string str = ((parent.Settings_Dictionary["CALENDAR_TOG_8"] == "1" && SI.Time_Set ? "[" + SI.Calendar_Date.ToString("hh:mm tt") + "]" : "") + SI.Name);

                                        e.Graphics.DrawString(str.Length < Letter_Max ? str : str.Substring(0, Letter_Max - 1) + "...", SI.Check_State ? f_minor_strike : f_minor, PinkBrush, Temp_Pt.X + 18, Temp_Pt.Y + 2);
                                        Temp_Pt.Y += 16;
                                    }
                                }
                            }

                            #endregion

                            #region Company pay information
                            if (parent.Settings_Dictionary["CALENDAR_TOG_10"] == "1")
                            {
                                foreach (CustomIncome CI in parent.Income_Company_List)
                                {
                                    List<PayPeriod> tempPP = CI.GetIntervalsPlusNext(52);
                                    if (tempPP.Any(x => x.Pay_Date.Date == refDay.Date))
                                    {
                                        e.Graphics.DrawString(String.Format("Pay day from {0}", CI.Company), f_minor, OrangeBrush, Temp_Pt.X, Temp_Pt.Y);
                                        Temp_Pt.Y += 10;
                                    }
                                }
                            }

                            #endregion

                            #region Label card/payment information
                            if (parent.Settings_Dictionary["CALENDAR_TOG_2"] == "1")
                            {
                                //Temp_Pt = Get_Calendar_Intersection(j, i + 1, 4, Daily_Total > 0 ? 23 : 4);
                                //List<Payment> Temp_Payment = parent.Payment_List.Where(x => (Convert.ToInt32(x.Billing_Start) - 1) == Day_Number || (Convert.ToInt32(x.Billing_Start)) == Day_Number && (x.Calendar_Toggle == 1)).ToList();
                                List<Payment> Temp_Payment = parent.Payment_List.Where(x => (Convert.ToInt32(x.Billing_Start)) == Day_Number && (x.Calendar_Toggle == 1)).ToList();
                                Temp_Payment = Temp_Payment.OrderBy(x => x.Company.Length).ToList();
                                for (int ij = 0; ij < Temp_Payment.Count; ij++)
                                {
                                    //Temp_Payment[ij].Calendar_Toggle = 0;
                                    e.Graphics.DrawString((Temp_Payment[ij].Company + "(xx-" + Temp_Payment[ij].Last_Four + ")"), f_minor, PinkBrush, Temp_Pt.X, Temp_Pt.Y);
                                    Temp_Pt.Y += 10;
                                }
                            }

                            #endregion

                            #region Get A/P information
                            if (parent.Settings_Dictionary["CALENDAR_TOG_3"] == "1")
                            {
                                List<Account> Temp_RP = parent.Account_List.Where(x => x.Start_Date.Day == Day_Number && x.Start_Date.Month == current_month && x.Start_Date.Year == current_year).ToList();
                                foreach (Account a in Temp_RP)
                                {
                                    string temp = "";
                                    switch (a.Type)
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
                                    e.Graphics.DrawString(temp + ((temp != "Deposited") ? a.Payer : "") + " " + a.Amount, a.Status == 0 ? f_minor_strike : f_minor, OrangeBrush, Temp_Pt.X, Temp_Pt.Y);
                                    Temp_Pt.Y += 10;
                                }
                                if (Temp_RP.Count > 0) Temp_Pt.Y += 2;
                            }

                            #endregion

                            #region Payment Options (w/ autodebit notices too)
                            if (parent.Settings_Dictionary["CALENDAR_TOG_5"] == "1")
                            {
                                //Temp_Pt = Get_Calendar_Intersection(j, i + 1, 4, Daily_Total > 0 ? 23 : 4);
                                List<Payment_Options> Temp_Payment = parent.Payment_Options_List.Where(x => x.Date.Day == Day_Number && x.Date.Month == current_month && x.Date.Year == current_year).ToList();
                                //Temp_Payment = Temp_Payment.OrderBy(x => x.Company.Length).ToList();
                                for (int ij = 0; ij < Temp_Payment.Count; ij++)
                                {
                                    e.Graphics.DrawString(("$" + String.Format("{0:0.00}", Temp_Payment[ij].Amount) + "-" + (Temp_Payment[ij].Hidden_Note.Length > 0 ? Temp_Payment[ij].Hidden_Note : Temp_Payment[ij].Note)), f_minor, WritingBrush, Temp_Pt.X, Temp_Pt.Y);
                                    Temp_Pt.Y += 10;
                                    e.Graphics.DrawString("→ " + (Temp_Payment[ij].Type == "Deposit" ? "to " : "from ") + (Temp_Payment[ij].Payment_Company + " (xx-" + Temp_Payment[ij].Payment_Last_Four + ")"), f_minor, WritingBrush, Temp_Pt.X, Temp_Pt.Y);
                                    Temp_Pt.Y += 10;
                                }
                            }

                            #endregion

                        }


                        #region Draw day numbers
                        Temp_Pt = Get_Calendar_Intersection(j, i + 1, col_width - (Day_Number >= 10 ? 23 : 14), 2);
                        e.Graphics.DrawString(Day_Number.ToString(), f_reg_bold, WritingBrush, Temp_Pt.X, Temp_Pt.Y);
                        #endregion

                        if (!NLR && i == 5) NLR = true;
                    }
                }
            }


            // Draw horizontal grid
            e.Graphics.DrawLine(p, cleft, ctitle, col6, ctitle);
            e.Graphics.DrawLine(p, cleft, ctop, col6, ctop);
            e.Graphics.DrawLine(p, cleft, cheader, col6, cheader);
            e.Graphics.DrawLine(p, cleft, c0, col6, c0);
            e.Graphics.DrawLine(p, cleft, c1, col6, c1);
            e.Graphics.DrawLine(p, cleft, c2, col6, c2);
            e.Graphics.DrawLine(p, cleft, c3, col6, c3);
            e.Graphics.DrawLine(p, cleft, c4, col6, c4);
            if (NLR) e.Graphics.DrawLine(p, cleft, c5, col6, c5);

            // Draw vertical grid
            e.Graphics.DrawLine(p, cleft, ctitle, cleft, NLR ? c5 : c4); // border goes to top
            e.Graphics.DrawLine(p, col0, ctop, col0, NLR ? c5 : c4);
            e.Graphics.DrawLine(p, col1, ctop, col1, NLR ? c5 : c4);
            e.Graphics.DrawLine(p, col2, ctop, col2, NLR ? c5 : c4);
            e.Graphics.DrawLine(p, col3, ctop, col3, NLR ? c5 : c4);
            e.Graphics.DrawLine(p, col4, ctop, col4, NLR ? c5 : c4);
            e.Graphics.DrawLine(p, col5, ctop, col5, NLR ? c5 : c4);
            e.Graphics.DrawLine(p, col6, ctitle, col6, NLR ? c5 : c4); // border goes to top

            // title
            int Title_Center = col1 + ((col4 - col1) / 2) - (TextRenderer.MeasureText(mfi.GetAbbreviatedMonthName(current_month), f_title).Width) - 7;
            e.Graphics.DrawString(mfi.GetAbbreviatedMonthName(current_month) + " " + current_year, f_title, WritingBrush, Title_Center, ctitle + 5);
            
            // Adjust based on length to center text
            int SUNDAY = (col_width / 2) - TextRenderer.MeasureText("SUNDAY", f_header).Width / 2;
            int MONDAY = (col_width / 2) - TextRenderer.MeasureText("MONDAY", f_header).Width / 2;
            int TUESDAY = (col_width / 2) - TextRenderer.MeasureText("TUESDAY", f_header).Width / 2;
            int WEDNESDAY = (col_width / 2) - TextRenderer.MeasureText("WEDNESDAY", f_header).Width / 2;
            int THURSDAY = (col_width / 2) - TextRenderer.MeasureText("THURSDAY", f_header).Width / 2;
            int FRIDAY = (col_width / 2) - TextRenderer.MeasureText("FRIDAY", f_header).Width / 2;
            int SATURDAY = (col_width / 2) - TextRenderer.MeasureText("SATURDAY", f_header).Width / 2;
            // week header
            e.Graphics.DrawString("SUNDAY", f_header, WritingBrush, cleft + SUNDAY, ctop + 5);
            e.Graphics.DrawString("MONDAY", f_header, WritingBrush, col0 + MONDAY, ctop + 5);
            e.Graphics.DrawString("TUESDAY", f_header, WritingBrush, col1 + TUESDAY, ctop + 5);
            e.Graphics.DrawString("WEDNESDAY", f_header, WritingBrush, col2 + WEDNESDAY, ctop + 5);
            e.Graphics.DrawString("THURSDAY", f_header, WritingBrush, col3 + THURSDAY, ctop + 5);
            e.Graphics.DrawString("FRIDAY", f_header, WritingBrush, col4 + FRIDAY, ctop + 5);
            e.Graphics.DrawString("SATURDAY", f_header, WritingBrush, col5 + SATURDAY, ctop + 5);


            for (int i = 0; i < 6; i++) // Row
            {
                for (int j = 1; j < 8; j++) // Col
                {
                    if ((DayLabels[(i * 7) + j - 1] > 0))
                    {
                        int Day_Number = DayLabels[(i * 7) + j - 1];
                        // Circle today's date
                        if (DateTime.Now.Day == Day_Number && DateTime.Now.Month == current_month && DateTime.Now.Year == current_year)
                        {
                            Point Temp_Pt = Get_Calendar_Intersection(j, i + 1);

                            // Circle today's date number
                            Temp_Pt = Get_Calendar_Intersection(j, i + 1, col_width - (Day_Number >= 10 ? 23 : 18), 2);
                            Rectangle r = new Rectangle(Temp_Pt.X - 4, Temp_Pt.Y - 6, 30, 30);
                            e.Graphics.DrawEllipse(Red_Pen, r);

                            // Rectangle around date rectangle
                            //Rectangle r = new Rectangle(Temp_Pt.X + 1 - circular_offset, Temp_Pt.Y + 1 - circular_offset / 2, col_width - 1 + circular_offset * 2, row_height + circular_offset);
                            //e.Graphics.DrawRectangle(Red_Pen, r);
                        }

                        // Check if has important date
                        List<Shipment_Tracking> Shipments = parent.Tracking_List.Where(x => x.Expected_Date.Day == Day_Number && x.Expected_Date.Month == current_month && x.Expected_Date.Year == current_year).ToList();
                        if (Shipments.Count > 0 && parent.Settings_Dictionary["CALENDAR_TOG_6"] == "1")
                        {
                            Point Temp_Pt = Get_Calendar_Intersection(j, i + 1);
                            Rectangle r = new Rectangle(Temp_Pt.X + 1, Temp_Pt.Y + 1, col_width - 1, row_height - 1);
                            e.Graphics.DrawRectangle(Box_Green_Pen, r);
                        }

                        // Circle overdue dates
                        int package_count = parent.Settings_Dictionary["CALENDAR_TOG_1"] == "1" ? 1 : 0;

                        foreach (Shipment_Tracking ST in parent.Tracking_List.Where(x => x.Expected_Date.Day == Day_Number && x.Expected_Date.Month == current_month && x.Expected_Date.Year == current_year).ToList())
                        {
                            // Circle overdue dates
                            if (ST.Expected_Date < DateTime.Now && ST.Status == 1)
                            {
                                Point Temp_Pt = Get_Calendar_Intersection(j, i + 1);
                                Rectangle r = new Rectangle(Temp_Pt.X + col_width - 28 - (package_count * 20), Temp_Pt.Y + row_height - 28, 35, 35);
                                e.Graphics.DrawEllipse(Red_Pen, r);
                            }

                            package_count++;
                        }

                        // Check if has important date
                        List<Calendar_Events> Very_Important = Calendar_Events_Array[Day_Number - 1].Where(x => x.Importance > 1).ToList();
                        if (Very_Important.Count > 0)
                        {
                            // Draw orange box around very important
                            if (Very_Important.Count > 0)
                            {
                                Point Temp_Pt = Get_Calendar_Intersection(j, i + 1);
                                Rectangle r = new Rectangle(Temp_Pt.X + 1, Temp_Pt.Y + 1, col_width - 1, row_height - 1);
                                e.Graphics.DrawRectangle(Box_Orange_Pen, r);
                            }
                        }
                    }
                }
            }

            // Remove listeners
            this.ResizeBegin -= Calendar_Resize_Begin;
            this.Resize -= Calendar_Resize;
            this.ResizeEnd -= Calendar_Resize_End;


            //if (NLR) this.Size = Resize_Size; else this.Height = Resize_Size.Height - 120;
            this.Height = Resize_Size.Height;// -(!NLR ? row_height : 0);

            // Add listeners
            this.ResizeBegin += Calendar_Resize_Begin;
            this.Resize += Calendar_Resize;
            this.ResizeEnd += Calendar_Resize_End;

            TFLP.Size = new Size(this.Width - 2, this.Height - 2);

            Rearrange_Controls();

            // Dispose all objects
            p.Dispose();
            Grey_Pen.Dispose();
            GreyBrush.Dispose();
            BlueBrush.Dispose();
            RedBrush.Dispose();
            PinkBrush.Dispose();
            GreenBrush.Dispose();
            PurpleBrush.Dispose();
            YellowBrush.Dispose();
            OrangeBrush.Dispose();
            LightOrangeBrush.Dispose();
            Blue_Pen.Dispose();
            Green_Pen.Dispose();
            Red_Pen.Dispose();
            Purple_Pen.Dispose();
            Yellow_Pen.Dispose();
            Orange_Pen.Dispose();
            Box_Orange_Pen.Dispose();
            CyanBrush.Dispose();
            Box_Green_Pen.Dispose();
            WritingBrush.Dispose();
            graph_font.Dispose();
            axis_font.Dispose();
            f_asterisk.Dispose();
            f_minor_strike.Dispose();
            f.Dispose();
            f_title.Dispose();
            f_minor.Dispose();
            f_minor_bold.Dispose();
            f_reg_bold.Dispose();
            f_header.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        Receipt parent;
        Size Start_Size = new Size();

        private void View_Package(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            Grey_Out();
            Tracking T = new Tracking(parent, true, b.Name, this.Location, this.Size);
            T.ShowDialog();
            Invalidate();
            Grey_In();
            parent.Background_Save();
        }

        private void view_order_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            if (b.Name.StartsWith("p"))
            {
                Grey_Out();
                Expenditures g = new Expenditures(parent);
                string Info_String = g.Parse_Dictionary_To_String(parent.Master_Item_List.Where(x => x.Date.Day == Convert.ToInt32(b.Name.Substring(1)) && x.Date.Month == current_month && x.Date.Year == current_year).ToList());
                Financial_Journal.Category_Summary FJCS = new Financial_Journal.Category_Summary("Purchases on " + current_month + "/" + b.Name.Substring(1) + "/" + current_year, new Point(), Info_String, false, this.Location, this.Size);
                FJCS.ShowDialog();
                Grey_In();
            }
            else if (b.Name.StartsWith("e"))
            {
                Grey_Out();
                Add_Edit_Calendar AEC = new Add_Edit_Calendar(parent, new DateTime(current_year, current_month, Convert.ToInt32(b.Name.Substring(1))), this,this.Location, this.Size);
                AEC.ShowDialog();
                Invalidate();
                Grey_In();
            }
            // View agenda item
            else if (b.Name.StartsWith("q"))
            {
                Grey_Out();
                Agenda A = new Agenda(parent, parent.Agenda_Item_List.First(x => x.ID.ToString() == b.Name.Substring(1)), this.Location, this.Size);
                A.ShowDialog();
                Invalidate();
                Grey_In();
            }
            // View agenda item
            else if (b.Name.StartsWith("w"))
            {
                string[] Temp = b.Name.Split(new string[] { "_" }, StringSplitOptions.None);
                Grey_Out();
                Agenda A = new Agenda(parent, parent.Agenda_Item_List.First(x => x.ID.ToString() == Temp[1]), this.Location, this.Size);
                A.ShowDialog();
                Invalidate();
                Grey_In();
            }
            parent.Background_Save();
        }

        public Calendar(Receipt _parent)
        {

            //this.Location = new Point(left + 50, top + 50);
            InitializeComponent();
            this.DoubleBuffered = true;
            //this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer,
                true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
        }

        public void Grey_Out()
        {
            TFLP.Location = new Point(1, 1);
        }

        public void Grey_In()
        {
            TFLP.Location = new Point(1000, 3000);
        }

        FadeControl TFLP;

        private void Receipt_Load(object sender, EventArgs e)
        {
            typeof(Panel).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, this, new object[] { true });

            // Fade Box
            TFLP = new FadeControl();
            TFLP.Size = new Size(this.Width - 2, this.Height - 2);
            TFLP.Location = new Point(999, 2999);
            TFLP.Visible = true;
            TFLP.BackColor = this.BackColor;
            TFLP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            TFLP.AllowDrop = true;
            TFLP.BringToFront();
            this.Controls.Add(TFLP);
            TFLP.BringToFront();

            TFLP.Opacity = 75;

            current_month = DateTime.Now.Month;
            current_year = DateTime.Now.Year;
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            this.ResizeBegin += Calendar_Resize_Begin;
            this.Resize += Calendar_Resize;
            this.ResizeEnd += Calendar_Resize_End;

            Resize_Size = this.Size;

            Reset_Tooltips();

            // Predefine new Tooltip for rest of OnPaint function
            edit_calendar.Image = parent.Settings_Dictionary["CALENDAR_TOG_1"] == "1" ? global::Financial_Journal.Properties.Resources.edit_on : global::Financial_Journal.Properties.Resources.edit_off;

            // Load print
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage_1);
            this.printPreviewDialog1.Document = this.printDocument1;
            printDocument1.DefaultPageSettings.Landscape = true;
        }

        ToolTip ToolTip1;

        private void Reset_Tooltips()
        {
            if (ToolTip1 != null) ToolTip1.Dispose();
            ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            ToolTip1.SetToolTip(button2, "Print Calendar Report");
            ToolTip1.SetToolTip(email_button, "Email Calendar Report");
            ToolTip1.SetToolTip(export, "Export Calendar");
            ToolTip1.SetToolTip(import, "Import Calendar");
            ToolTip1.SetToolTip(undo_import, "Undo Calendar Import");
            ToolTip1.SetToolTip(settings_button, "Calendar Settings");
            ToolTip1.SetToolTip(screenshot_button, "Print Current Calendar");
            ToolTip1.SetToolTip(edit_calendar, (parent.Settings_Dictionary["CALENDAR_TOG_1"] == "1" ? "Stop Editing" : "Edit") + " Calendar Events");
            ToolTip1.SetToolTip(weatherbutton, (showWeather ? "Hide" : "Show") + " Weather");
        }


        bool Resizing = false;

        private void Calendar_Resize(object sender, EventArgs e)
        {
            Resizing = true;
            textBox1.Visible = textBox4.Visible = false;
            this.SuspendLayout();
        }

        Size Resize_Size;

        private void Calendar_Resize_End(object sender, EventArgs e)
        {
            if (Resizing)
            {
                Resize_Size = this.Size;
                textBox1.Visible = textBox4.Visible = true;
                this.Invalidate();
                this.ResumeLayout();
                Resizing = false;
            }
        }

        private void Calendar_Resize_Begin(object sender, EventArgs e)
        {
            this.SuspendLayout();
        }

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            parent.BringToFront();
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

        private void back_page_button_Click(object sender, EventArgs e)
        {
            showWeather = false;
            weatherbutton.Image = global::Financial_Journal.Properties.Resources.rain;
            Reset_Tooltips();

            current_month--;
            if (current_month < 1)
            {
                current_month = 12;
                current_year--;
            }
            Invalidate();
        }

        private void next_page_button_Click(object sender, EventArgs e)
        {
            showWeather = false;
            weatherbutton.Image = global::Financial_Journal.Properties.Resources.rain;
            Reset_Tooltips();

            current_month++;
            if (current_month > 12)
            {
                current_month = 1;
                current_year++;
            }
            Invalidate();
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

        public string Import_From_FTP(string Passcode, string Hash_Value)
        {
            Diagnostics.WriteLine("Import start: " + DateTime.Now.TimeOfDay);
            TimeSpan Start_Time = DateTime.Now.TimeOfDay;

            string ftpPath = @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/Export/" + Hash_Value + ".txt";

            string ftpUsername = "Guest";
            string ftpPassword = "robinisthebest";

            string Local_Path = Path.Combine(parent.localSavePath, Hash_Value.Trim() + ".txt");

            using (WebClient client = new WebClient())
            {
                client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                // try to download import file from client
                try
                {
                    client.DownloadFile(ftpPath, Local_Path);
                }
                catch
                {
                    return "Error: Failed to connect to server or code is invalid";
                }
            }

            string[] lines;
            string text = File.ReadAllText(Local_Path);

            if (text.Trim().Length > 0)
            {
                try
                {
                    if (!AESGCM.SimpleDecryptWithPassword(text, Passcode + "PASSWORDisHERE").StartsWith(Passcode))
                    {
                        return "Error: Invalid Password";
                    }
                }
                catch
                {
                    // invmalid password
                    return "Error: Invalid Password";
                }

                lines = AESGCM.SimpleDecryptWithPassword(text, Passcode + "PASSWORDisHERE").Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                // import function

                Import_Count = 0;

                parent.Agenda_Item_List.ForEach(x => Before_Import_AI_List.Add(x));
                parent.Calendar_Events_List.ForEach(x => Before_Import_CE_List.Add(x));
                parent.Contact_List.ForEach(x => Before_Import_Contact_List.Add(x));

                this.Load_Calendar(lines);
                this.Load_Agenda(lines);
                this.Load_Contacts(lines);

                File.Delete(Local_Path);

                undo_import.Enabled = true;
                undo_import.Image = global::Financial_Journal.Properties.Resources.undo;
            }

            Diagnostics.WriteLine("FTP Thread end at " + DateTime.Now.TimeOfDay);

            Invalidate();
            return "";
        }

        private int Import_Count = 0;

        public void Load_Contacts(string[] lines)
        {
            List<Contact> Temp_Contact_List = new List<Contact>();
            foreach (string line in lines)
            {
                if (line.Contains("[CONTACT_FIRST_NAME]"))
                {
                    Contact C = new Contact()
                    {
                        First_Name = parent.Parse_Line_Information(line, "CONTACT_FIRST_NAME"),
                        Last_Name = parent.Parse_Line_Information(line, "CONTACT_LAST_NAME"),
                        Email = parent.Parse_Line_Information(line, "CONTACT_EMAIL"),
                        Email_Second = parent.Parse_Line_Information(line, "CONTACT_EMAIL_SECOND"),
                        Phone_No_Primary = parent.Parse_Line_Information(line, "CONTACT_PRIMARY"),
                        Phone_No_Second = parent.Parse_Line_Information(line, "CONTACT_SECONDARY"),
                        Association = parent.Parse_Line_Information(line, "CONTACT_ASSOCIATION"),
                        Hash_Value = parent.Parse_Line_Information(line, "CONTACT_HASHVALUE")
                    };
                    Temp_Contact_List.Add(C);
                }
            }

            foreach (Contact c in Temp_Contact_List)
            {
                if (!parent.Contact_List.Any(x => x.Hash_Value == c.Hash_Value))
                {
                    parent.Contact_List.Add(c);
                }
            }
        }

        public void Load_Agenda(string[] lines)
        {
            List<Agenda_Item> Temp_Agenda_Item_List = new List<Agenda_Item>();

            foreach (string line in lines)
            {
                if (line.Contains("[AGENDA_ITEM]"))
                {
                    Agenda_Item AI = new Agenda_Item();
                    AI.Name = parent.Parse_Line_Information(line, "A_NAME");
                    AI.Hash_Value = parent.Parse_Line_Information(line, "HASH_VALUE");
                    AI.Contact_Hash_Value = parent.Parse_Line_Information(line, "C_HASH_VALUE");
                    AI.Date = Convert.ToDateTime(parent.Parse_Line_Information(line, "A_DATE"));
                    AI.Calendar_Date = parent.Parse_Line_Information(line, "A_CALENDAR_DATE") == "" ? new DateTime(1800, 1, 1) : Convert.ToDateTime(parent.Parse_Line_Information(line, "A_CALENDAR_DATE"));
                    AI.ID = Convert.ToInt32(parent.Parse_Line_Information(line, "A_ID"));
                    AI.Time_Set = parent.Parse_Line_Information(line, "TIME_SET") == "1";
                    AI.Check_State = parent.Parse_Line_Information(line, "A_CHECK_STATE") == "1";

                    // Remove original duplicate (check hash and first 4 letters of name)
                    Agenda_Item Dup_AI_Tester = parent.Agenda_Item_List.FirstOrDefault(x => x.Hash_Value == AI.Hash_Value && (x.Name.Length > 2 && AI.Name.Length > 2 && x.Name.Substring(0, 3) == AI.Name.Substring(0, 3)));

                    if (Dup_AI_Tester != null)
                    {
                        // Transfer shopping list
                        AI.Shopping_List = Dup_AI_Tester.Shopping_List;
                        AI.Overwrite = true;
                        parent.Agenda_Item_List.Remove(Dup_AI_Tester);
                    }

                    Temp_Agenda_Item_List.Add(AI);
                }
                else if (line.Contains("[SHOPPING_ITEM]"))
                {
                    Shopping_Item AI = new Shopping_Item();
                    AI.Name = parent.Parse_Line_Information(line, "S_NAME");
                    AI.Hash_Value = parent.Parse_Line_Information(line, "HASH_VALUE");
                    AI.Contact_Hash_Value = parent.Parse_Line_Information(line, "C_HASH_VALUE");
                    AI.ID = Convert.ToInt32(parent.Parse_Line_Information(line, "S_ID"));
                    AI.Calendar_Date = parent.Parse_Line_Information(line, "S_DATE") == "" ? new DateTime(1800, 1, 1) : Convert.ToDateTime(parent.Parse_Line_Information(line, "S_DATE"));
                    AI.Check_State = parent.Parse_Line_Information(line, "S_CHECK_STATE") == "1";
                    AI.Time_Set = parent.Parse_Line_Information(line, "TIME_SET") == "1";

                    Agenda_Item Temp_Agenda_Item = Temp_Agenda_Item_List.FirstOrDefault(x => x.ID == AI.ID);


                    int count = 0;
                    foreach (Shopping_Item SI in Temp_Agenda_Item.Shopping_List)
                    {
                        // If same item (replace existing)
                        if (SI.Hash_Value == AI.Hash_Value)
                        {
                            Temp_Agenda_Item.Shopping_List[count] = AI; // Replace
                            break;
                        }
                        count++;
                    }

                    // If doesnt exist add as new
                    if (count >= Temp_Agenda_Item.Shopping_List.Count) Temp_Agenda_Item.Shopping_List.Add(AI);

                }
            }

            int currentID = parent.Agenda_Item_List.Count + 1;
            foreach (Agenda_Item AI in Temp_Agenda_Item_List)
            {
                if (!AI.Overwrite)
                { 
                    // New ID
                    AI.Shopping_List.ForEach(x => x.ID = currentID);
                    AI.ID = currentID;
                    parent.Agenda_Item_List.Add(AI);
                    currentID++;
                }
                else
                {
                    // Old ID
                    parent.Agenda_Item_List.Add(AI);
                }
            }

            // Sort by descending
            parent.Agenda_Item_List = parent.Agenda_Item_List.OrderByDescending(x => x.Date).ToList();
        }

        public List<Agenda_Item> Before_Import_AI_List = new List<Agenda_Item>();
        public List<Calendar_Events> Before_Import_CE_List = new List<Calendar_Events>();
        public List<Contact> Before_Import_Contact_List = new List<Contact>();

        public void Load_Calendar(string[] lines)
        {
            foreach (string line in lines)
            {
                if (line.Contains("CALENDAR_EVENT") && line.Contains("CALENDAR_IMPORTANCE"))
                {
                    Calendar_Events CE = new Calendar_Events();
                    CE.Title = parent.Parse_Line_Information(line, "CALENDAR_TITLE");
                    CE.Hash_Value = parent.Parse_Line_Information(line, "CALENDAR_HASHVALUE");
                    CE.Contact_Hash_Value = parent.Parse_Line_Information(line, "CALENDAR_CONTACT_HASH");
                    CE.Is_Active = parent.Parse_Line_Information(line, "CALENDAR_ACTIVE") == "" ? "0" : parent.Parse_Line_Information(line, "CALENDAR_ACTIVE");
                    CE.Description = parent.Parse_Line_Information(line, "CALENDAR_DESC").Replace("~~", Environment.NewLine);
                    CE.Importance = Convert.ToInt32(parent.Parse_Line_Information(line, "CALENDAR_IMPORTANCE"));
                    CE.Date = Convert.ToDateTime(parent.Parse_Line_Information(line, "CALENDAR_DATE"));
                    CE.Time_Set = parent.Parse_Line_Information(line, "CALENDAR_TIMESET") == "1";
                    string[] date_Strings = (parent.Parse_Line_Information(line, "CALENDAR_ALERT_SEQUENCE").Split(new string[] { "~" }, StringSplitOptions.None));
                    foreach (string dS in date_Strings)
                    {
                        if (dS.Length > 0)
                        {
                            CE.Alert_Dates.Add(Convert.ToDateTime(dS));
                        }
                    }

                    // Remove original duplicate
                    Calendar_Events Dup_CE_Tester = parent.Calendar_Events_List.FirstOrDefault(x => x.Hash_Value == CE.Hash_Value && (x.Title.Length > 2 && CE.Title.Length > 2 && x.Title.Substring(0, 3) == CE.Title.Substring(0, 3)));
                    if (Dup_CE_Tester != null)
                    {
                        parent.Calendar_Events_List.Remove(Dup_CE_Tester);
                            Import_Count--;
                    }

                    Import_Count++;
                    parent.Calendar_Events_List.Add(CE);
                }
            }
        }

        private void settings_button_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Calendar_Settings CS = new Calendar_Settings(parent, this, this.Location, this.Size);
            CS.ShowDialog();
            Grey_In();
        }

        // Print
        private void button2_Click_1(object sender, EventArgs e)
        {
            Grey_Out();
            Print_Calendar_Range PCR = new Print_Calendar_Range(parent, this, this.Location, this.Size);
            PCR.ShowDialog();
            Grey_In();
        }

        private void edit_calendar_Click_1(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["CALENDAR_TOG_1"] = (parent.Settings_Dictionary["CALENDAR_TOG_1"] == "1") ? "0" : "1";
            edit_calendar.Image = parent.Settings_Dictionary["CALENDAR_TOG_1"] == "1" ? global::Financial_Journal.Properties.Resources.edit_on : global::Financial_Journal.Properties.Resources.edit_off;
            Invalidate();
            Reset_Tooltips();
        }

        private void screenshot_button_Click(object sender, EventArgs e)
        {
            Grey_Out();
            SuspendLayout();
            DateTime Curr_Print_Date = new DateTime(current_year, current_month, 1);

            using (var form2 = new Yes_No_Dialog(parent, "Are you sure you wish to print the current calendar?", "Warning", "Preview", "Print", 0, this.Location, this.Size))
            {
                var result = form2.ShowDialog();
                if (result == DialogResult.OK)
                {
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
                        printPreviewDialog1.TopMost = true;
                        printPreviewDialog1.ShowDialog();

                    }

                    #region Restoration
                    if (secondThreadFormHandle != IntPtr.Zero)
                        PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

                    Cursor.Current = Cursors.Default;

                    #endregion
                }
            }
            /*testing only BELOW
            Calculate_Months();
            printPreviewDialog1.TopMost = true;
            printPreviewDialog1.ShowDialog();
            */
            this.Activate();
            Grey_In();
            ResumeLayout();
        }

        
        // Main print function
        private void printDocument1_PrintPage_1(System.Object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            #region Draw values first to evaluate if we need the last row
            // find start date of first date of current month
            DateTime First_Date_Cur_Month = new DateTime(current_year, current_month, 1);

            PopulateMultiDays(current_month, current_year);
            int multiLineSpacing = 20;

            int Start_Date_Number = 0;
            int Current_Month_Max_Day = new DateTime(current_year, current_month, 1).AddMonths(1).AddDays(-1).Day;

            switch (First_Date_Cur_Month.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    Start_Date_Number = 0;
                    break;
                case DayOfWeek.Monday:
                    Start_Date_Number = 1;
                    break;
                case DayOfWeek.Tuesday:
                    Start_Date_Number = 2;
                    break;
                case DayOfWeek.Wednesday:
                    Start_Date_Number = 3;
                    break;
                case DayOfWeek.Thursday:
                    Start_Date_Number = 4;
                    break;
                case DayOfWeek.Friday:
                    Start_Date_Number = 5;
                    break;
                case DayOfWeek.Saturday:
                    Start_Date_Number = 6;
                    break;
            }

            // Need_Last_Row (NLR)
            bool NLR = false;

            // Populate current month numbers
            int recurring_index = Start_Date_Number;

            Need_Last_Row = Current_Month_Max_Day - (((7 - Start_Date_Number) + 21)) > 7;

            int[] DayLabels = new int[42]; //42 6 rows, 7 columns 6x7
            for (int i = 1; i <= 42; i++)
            {
                if (i > Current_Month_Max_Day)
                {
                    break;
                }
                else
                {
                    DayLabels[recurring_index] = i;
                    recurring_index++;
                }
            }

            #endregion

            //Initialize_Calendar_Paremeters(1135, 856);
            Initialize_Calendar_Paremeters(1100, 820);

            Populate_Calendar_Events();

            Show_Spending_Button.ForEach(button => button.Image.Dispose());
            Show_Spending_Button.ForEach(button => button.Dispose());
            Show_Spending_Button.ForEach(button => this.Controls.Remove(button));
            Show_Spending_Button = new List<Button>();

            // Reset calendar payment toggle
            parent.Payment_List.ForEach(x => x.Calendar_Toggle = 1);

            Color DrawForeColor = Color.White;
            Color HighlightColor = Color.FromArgb(76, 76, 76);

            SolidBrush WritingBrush = new SolidBrush(Color.Black);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(76, 76, 76));
            SolidBrush BlueBrush = new SolidBrush(Color.Blue);
            SolidBrush GreenBrush = new SolidBrush(Color.Green);
            SolidBrush CyanBrush = new SolidBrush(Color.Cyan);
            SolidBrush PurpleBrush = new SolidBrush(Color.Purple);
            //SolidBrush RedBrush = new SolidBrush(Color.Red);
            SolidBrush RedBrush = new SolidBrush(Color.Black);
            SolidBrush YellowBrush = new SolidBrush(Color.Black);
            //SolidBrush PinkBrush = new SolidBrush(Color.Red);
            SolidBrush PinkBrush = new SolidBrush(Color.Black);
            SolidBrush OrangeBrush = new SolidBrush(Color.Orange);
            SolidBrush LightOrangeBrush = new SolidBrush(Color.FromArgb(255, 200, 0));

            Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
            Font f = new Font("MS Reference Sans Serif", 6F, FontStyle.Regular);
            Font f_minor_bold = new Font("MS Reference Sans Serif", 7.5F, FontStyle.Bold);
            Font f_minor = new Font("MS Reference Sans Serif", 6F, FontStyle.Regular);
            Font f_minor_strike = new Font("MS Reference Sans Serif", 6F, FontStyle.Strikeout);
            Font f_reg_bold = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 12, FontStyle.Bold);
            Font f_title = new Font("MS Reference Sans Serif", 24, FontStyle.Bold);

            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 1);
            Pen Blue_Pen = new Pen(BlueBrush, 1);
            Pen Green_Pen = new Pen(GreenBrush, 1);
            Pen Red_Pen = new Pen(RedBrush, 2);
            Pen Orange_Pen = new Pen(OrangeBrush, 1);
            Pen Box_Orange_Pen = new Pen(OrangeBrush, 2);
            Pen Box_Green_Pen = new Pen(CyanBrush, 2);
            Pen Purple_Pen = new Pen(PurpleBrush, 1);
            Pen Yellow_Pen = new Pen(YellowBrush, 1);

            Font graph_font = new Font("MS Reference Sans Serif", 8, FontStyle.Regular);
            Font axis_font = new Font("MS Reference Sans Serif", 6, FontStyle.Regular);

            // Predefine new Tooltip for rest of OnPaint function
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            // Populate previous and next month numbers
            for (int i = 0; i < 6; i++) // Row
            {
                for (int j = 1; j < 8; j++) // Col
                {
                    // If matrix has a date (Perform checks for that day)
                    if (DayLabels[(i * 7) + j - 1] > 0)
                    {
                        int Day_Number = DayLabels[(i * 7) + j - 1];

                        int Letter_Max = (int)Math.Round((col_width / 6.25));

                        #region Get daily spending totals
                        Point Temp_Pt = Get_Calendar_Intersection(j, i + 1, 1, 1);
                        List<Order> Temp = parent.Order_List.Where(x => x.Date.Year == current_year && x.Date.Month == current_month && x.Date.Day == Day_Number).ToList();
                        double Daily_Total = Temp.Sum(x => x.Order_Total_Pre_Tax + x.Order_Taxes);

                        if (parent.Settings_Dictionary["CALENDAR_TOG_9"] == "0") Daily_Total = 0;

                        if (Daily_Total > 0)
                        {

                            bool Needs_Warning = false;
                            double percentage_boundary = 0;

                            double mo_income = 1;

                            // Get Dynamic Income
                            using (Savings_Helper SH = new Savings_Helper(parent))
                            {
                                mo_income = SH.Get_Monthly_Salary(current_month, current_year);
                            }

                            if (Convert.ToDouble(parent.Settings_Dictionary["CALENDAR_TOG_PERC"]) > 0)
                            {
                                percentage_boundary = Convert.ToDouble(parent.Settings_Dictionary["CALENDAR_TOG_PERC"]) / 100;
                            }

                            //Needs_Warning = spending_warning.Checked && (Daily_Total / (parent.Monthly_Income > 0 ? parent.Monthly_Income : 1)) > percentage_boundary;
                            Needs_Warning = parent.Settings_Dictionary["CALENDAR_TOG_4"] == "1" && (Daily_Total / (mo_income > 0 ? mo_income : 1)) > percentage_boundary;

                            e.Graphics.DrawString("$" + String.Format("{0:0.00}", Daily_Total), f, (Needs_Warning ? PinkBrush : BlueBrush), Temp_Pt.X, Temp_Pt.Y + 2);

                        }

                        #endregion
                        
                        #region Draw MultiDay lines
                        DateTime refDay = new DateTime(current_year, current_month, Day_Number); 
                        if (Daily_Total > 0 || MultiDayList.Where(y => y.eventDays.Any(z => z == refDay)).Any(x => x.hasDailyTotalEntry)) Temp_Pt.Y += 19;

                        foreach (MultiDayEvents MDE in MultiDayList.Where(x => x.eventDays.Any(y => y.Date == refDay.Date)))
                        {
                            Pen refPen = new Pen(MDE.lineColor, 1);
                            Pen refPenDash = new Pen(MDE.lineColor, 1);
                            refPenDash.DashPattern = new float[] { 4.0F, 2.0F};

                            if (Daily_Total > 0) MDE.hasDailyTotalEntry = true;

                            int lineHeight = (MDE.lineIndex + 1) * multiLineSpacing - 8;
                            if (MDE.GetDateType(refDay) == DateType.Start)
                            {
                                // DRAW START ARROW
                                e.Graphics.DrawLine(refPenDash, Temp_Pt.X - 3, Temp_Pt.Y + 1 + lineHeight, Temp_Pt.X + col_width - 4,
                                    Temp_Pt.Y + 1 + lineHeight); // line


                                e.Graphics.DrawLine(refPen, Temp_Pt.X - 3, Temp_Pt.Y + 1 + lineHeight, Temp_Pt.X + 6,
                                    Temp_Pt.Y - 3 + lineHeight); //arrow top
                                e.Graphics.DrawLine(refPen, Temp_Pt.X - 3, Temp_Pt.Y + 1 + lineHeight, Temp_Pt.X + 6,
                                    Temp_Pt.Y + 5 + lineHeight); //arrow bottom
                            }
                            else if (MDE.GetDateType(refDay) == DateType.Middle)
                            {
                                // DRAW CONTINUOUS LINE
                                e.Graphics.DrawLine(refPenDash, Temp_Pt.X - 3, Temp_Pt.Y + 1 + lineHeight, Temp_Pt.X + col_width - 4,
                                    Temp_Pt.Y + 1 + lineHeight);
                            }
                            else
                            {
                                // DRAW END ARROW
                                e.Graphics.DrawLine(refPenDash, Temp_Pt.X - 3, Temp_Pt.Y + 1 + lineHeight, Temp_Pt.X + col_width - 5,
                                    Temp_Pt.Y + 1 + lineHeight);

                                e.Graphics.DrawLine(refPen, Temp_Pt.X + col_width - 12, Temp_Pt.Y - 3 + lineHeight, Temp_Pt.X + col_width - 5,
                                    Temp_Pt.Y + 1 + lineHeight); //arrow top
                                e.Graphics.DrawLine(refPen, Temp_Pt.X + col_width - 12, Temp_Pt.Y + 5 + lineHeight, Temp_Pt.X + col_width - 5,
                                    Temp_Pt.Y + 1 + lineHeight); //arrow bottom
                            }
                            refPen.Dispose();
                            refPenDash.Dispose();
                        }
                        #endregion

                        #region Get important+ calendar events
                        List<Calendar_Events> Important = Calendar_Events_Array[Day_Number - 1].Where(x => x.Importance >= 1).ToList();

                        #endregion

                        #region Show important+ calendar events
                        if (Important.Count > 0)
                        {
                            foreach (Calendar_Events CE in Important)
                            {
                                if (MultiDayList.Any(x => x.calendarHash == CE.Hash_Value))
                                    Temp_Pt.Y +=
                                        (MultiDayList.First(x => x.calendarHash == CE.Hash_Value).lineIndex) * multiLineSpacing;
                                string str = (parent.Settings_Dictionary["CALENDAR_TOG_8"] == "1" && CE.Time_Set ? "[" + CE.Date.ToString("hh:mm tt") + "]" : "") + CE.Title;
                                e.Graphics.DrawString(str.Length < Letter_Max ? str : str.Substring(0, Letter_Max - 1) + "...", f_minor_bold, 
                                    MultiDayList.Any(x => x.calendarHash == CE.Hash_Value) ? new SolidBrush(MultiDayList.First(x => x.calendarHash == CE.Hash_Value).lineColor) :
                                        CE.Importance > 1 ? PinkBrush : YellowBrush
                                    , Temp_Pt.X, Temp_Pt.Y);
                                Temp_Pt.Y += 11;
                            }
                        }

                        #endregion

                        // Circle today's date number
                        //Rectangle r = new Rectangle(Temp_Pt.X + col_width - 25 - (package_count * 20), Temp_Pt.Y + row_height - 25);
                        /*
                        if (Daily_Total > 0) Temp_Pt.Y += 9;

                        #region Get important+ calendar events
                        List<Calendar_Events> Important = Calendar_Events_Array[Day_Number - 1].Where(x => x.Importance >= 1).ToList();

                        #endregion

                        #region Show important+ calendar events
                        if (Important.Count > 0)
                        {
                            foreach (Calendar_Events CE in Important)
                            {

                                string str = (parent.Settings_Dictionary["CALENDAR_TOG_8"] == "1" && CE.Time_Set ? "[" + CE.Date.ToString("hh:mm tt") + "]" : "") + CE.Title;
                                e.Graphics.DrawString(str.Length < Letter_Max ? str : str.Substring(0, Letter_Max - 1) + "...", f_minor, CE.Importance > 1 ? PinkBrush : YellowBrush, Temp_Pt.X, Temp_Pt.Y);
                                Temp_Pt.Y += 9;
                            }
                        }

                        #endregion
                        */

                        #region Show Agenda items with calendarDate set on that date
                        foreach (Agenda_Item AI in parent.Agenda_Item_List.Where(x => x.Calendar_Date.Day == Day_Number && x.Calendar_Date.Month == current_month && x.Calendar_Date.Year == current_year).ToList())
                        {
                            if (parent.Settings_Dictionary["CALENDAR_TOG_7"] == "1" || !AI.Check_State)
                            {
                                string str = ((parent.Settings_Dictionary["CALENDAR_TOG_8"] == "1" && AI.Time_Set ? "[" + AI.Calendar_Date.ToString("hh:mm tt") + "]" : "") + AI.Name);

                                e.Graphics.DrawString(str.Length < Letter_Max ? str : str.Substring(0, Letter_Max - 1) + "...", AI.Check_State ? f_minor_strike : f_minor, PinkBrush, Temp_Pt.X, Temp_Pt.Y + 2);
                                Temp_Pt.Y += 9;
                            }
                        }
                        #endregion

                        #region Show Shopping items with calendarDate set on that date
                        foreach (Agenda_Item AI in parent.Agenda_Item_List)
                        {
                            foreach (Shopping_Item SI in AI.Shopping_List.Where(x => x.Calendar_Date.Day == Day_Number && x.Calendar_Date.Month == current_month && x.Calendar_Date.Year == current_year).ToList())
                            {
                                if (parent.Settings_Dictionary["CALENDAR_TOG_7"] == "1" || !SI.Check_State)
                                {
                                    string str = ((parent.Settings_Dictionary["CALENDAR_TOG_8"] == "1" && SI.Time_Set ? "[" + SI.Calendar_Date.ToString("hh:mm tt") + "]" : "") + SI.Name);

                                    e.Graphics.DrawString(str.Length < Letter_Max ? str : str.Substring(0, Letter_Max - 1) + "...", SI.Check_State ? f_minor_strike : f_minor, PinkBrush, Temp_Pt.X, Temp_Pt.Y + 2);
                                    Temp_Pt.Y += 9;
                                }
                            }
                        }

                        #endregion

                        #region Label card/payment information
                        if (parent.Settings_Dictionary["CALENDAR_TOG_2"] == "1")
                        {
                            //Temp_Pt = Get_Calendar_Intersection(j, i + 1, 4, Daily_Total > 0 ? 23 : 4);
                            //List<Payment> Temp_Payment = parent.Payment_List.Where(x => (Convert.ToInt32(x.Billing_Start) - 1) == Day_Number || (Convert.ToInt32(x.Billing_Start)) == Day_Number && (x.Calendar_Toggle == 1)).ToList();
                            List<Payment> Temp_Payment = parent.Payment_List.Where(x => (Convert.ToInt32(x.Billing_Start)) == Day_Number && (x.Calendar_Toggle == 1)).ToList();
                            Temp_Payment = Temp_Payment.OrderBy(x => x.Company.Length).ToList();
                            for (int ij = 0; ij < Temp_Payment.Count; ij++)
                            {
                                //Temp_Payment[ij].Calendar_Toggle = 0;
                                e.Graphics.DrawString((Temp_Payment[ij].Company + "(xx-" + Temp_Payment[ij].Last_Four + ")"), f_minor, PinkBrush, Temp_Pt.X, Temp_Pt.Y);
                                Temp_Pt.Y += 9;
                            }
                        }

                        #endregion

                        #region Get A/P information
                        if (parent.Settings_Dictionary["CALENDAR_TOG_3"] == "1")
                        {
                            List<Account> Temp_RP = parent.Account_List.Where(x => x.Start_Date.Day == Day_Number && x.Start_Date.Month == current_month && x.Start_Date.Year == current_year).ToList();
                            foreach (Account a in Temp_RP)
                            {
                                string temp = "";
                                switch (a.Type)
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
                                e.Graphics.DrawString(temp + ((temp != "Deposited") ? a.Payer : "") + " " + a.Amount, a.Status == 0 ? f_minor_strike : f_minor, OrangeBrush, Temp_Pt.X, Temp_Pt.Y);
                                Temp_Pt.Y += 9;
                            }
                            if (Temp_RP.Count > 0) Temp_Pt.Y += 2;
                        }

                        #endregion

                        #region Payment Options (w/ autodebit notices too)
                        if (parent.Settings_Dictionary["CALENDAR_TOG_5"] == "1")
                        {
                            //Temp_Pt = Get_Calendar_Intersection(j, i + 1, 4, Daily_Total > 0 ? 23 : 4);
                            List<Payment_Options> Temp_Payment = parent.Payment_Options_List.Where(x => x.Date.Day == Day_Number && x.Date.Month == current_month && x.Date.Year == current_year).ToList();
                            //Temp_Payment = Temp_Payment.OrderBy(x => x.Company.Length).ToList();
                            for (int ij = 0; ij < Temp_Payment.Count; ij++)
                            {
                                e.Graphics.DrawString(("$" + String.Format("{0:0.00}", Temp_Payment[ij].Amount) + "-" + (Temp_Payment[ij].Hidden_Note.Length > 0 ? Temp_Payment[ij].Hidden_Note : Temp_Payment[ij].Note)), f_minor, WritingBrush, Temp_Pt.X, Temp_Pt.Y);
                                Temp_Pt.Y += 9;
                                e.Graphics.DrawString("→ " + (Temp_Payment[ij].Type == "Deposit" ? "to " : "from ") + (Temp_Payment[ij].Payment_Company + " (xx-" + Temp_Payment[ij].Payment_Last_Four + ")"), f_minor, WritingBrush, Temp_Pt.X, Temp_Pt.Y);
                                Temp_Pt.Y += 9;
                            }
                        }

                        #endregion

                        #region Draw day numbers
                        Temp_Pt = Get_Calendar_Intersection(j, i + 1, col_width - (Day_Number >= 10 ? 23 : 14), 2);
                        e.Graphics.DrawString(Day_Number.ToString(), f_reg_bold, WritingBrush, Temp_Pt.X, Temp_Pt.Y);
                        #endregion

                        if (!NLR && i == 5) NLR = true;
                    }
                }
            }

            // Draw horizontal grid
            e.Graphics.DrawLine(p, cleft, ctitle, col6, ctitle);
            e.Graphics.DrawLine(p, cleft, ctop, col6, ctop);
            e.Graphics.DrawLine(p, cleft, cheader, col6, cheader);
            e.Graphics.DrawLine(p, cleft, c0, col6, c0);
            e.Graphics.DrawLine(p, cleft, c1, col6, c1);
            e.Graphics.DrawLine(p, cleft, c2, col6, c2);
            e.Graphics.DrawLine(p, cleft, c3, col6, c3);
            e.Graphics.DrawLine(p, cleft, c4, col6, c4);
            if (NLR) e.Graphics.DrawLine(p, cleft, c5, col6, c5);

            // Draw vertical grid
            e.Graphics.DrawLine(p, cleft, ctitle, cleft, NLR ? c5 : c4); // border goes to top
            e.Graphics.DrawLine(p, col0, ctop, col0, NLR ? c5 : c4);
            e.Graphics.DrawLine(p, col1, ctop, col1, NLR ? c5 : c4);
            e.Graphics.DrawLine(p, col2, ctop, col2, NLR ? c5 : c4);
            e.Graphics.DrawLine(p, col3, ctop, col3, NLR ? c5 : c4);
            e.Graphics.DrawLine(p, col4, ctop, col4, NLR ? c5 : c4);
            e.Graphics.DrawLine(p, col5, ctop, col5, NLR ? c5 : c4);
            e.Graphics.DrawLine(p, col6, ctitle, col6, NLR ? c5 : c4); // border goes to top

            // title
            int Title_Center = col1 + ((col4 - col1) / 2) - (TextRenderer.MeasureText(mfi.GetAbbreviatedMonthName(current_month), f_title).Width) - 7;
            e.Graphics.DrawString(mfi.GetAbbreviatedMonthName(current_month) + " " + current_year, f_title, WritingBrush, Title_Center, ctitle + 5);

            // Adjust based on length to center text
            int SUNDAY = (col_width / 2) - TextRenderer.MeasureText("SUNDAY", f_header).Width / 2;
            int MONDAY = (col_width / 2) - TextRenderer.MeasureText("MONDAY", f_header).Width / 2;
            int TUESDAY = (col_width / 2) - TextRenderer.MeasureText("TUESDAY", f_header).Width / 2;
            int WEDNESDAY = (col_width / 2) - TextRenderer.MeasureText("WEDNESDAY", f_header).Width / 2;
            int THURSDAY = (col_width / 2) - TextRenderer.MeasureText("THURSDAY", f_header).Width / 2;
            int FRIDAY = (col_width / 2) - TextRenderer.MeasureText("FRIDAY", f_header).Width / 2;
            int SATURDAY = (col_width / 2) - TextRenderer.MeasureText("SATURDAY", f_header).Width / 2;
            // week header
            e.Graphics.DrawString("SUNDAY", f_header, WritingBrush, cleft + SUNDAY, ctop + 5);
            e.Graphics.DrawString("MONDAY", f_header, WritingBrush, col0 + MONDAY, ctop + 5);
            e.Graphics.DrawString("TUESDAY", f_header, WritingBrush, col1 + TUESDAY, ctop + 5);
            e.Graphics.DrawString("WEDNESDAY", f_header, WritingBrush, col2 + WEDNESDAY, ctop + 5);
            e.Graphics.DrawString("THURSDAY", f_header, WritingBrush, col3 + THURSDAY, ctop + 5);
            e.Graphics.DrawString("FRIDAY", f_header, WritingBrush, col4 + FRIDAY, ctop + 5);
            e.Graphics.DrawString("SATURDAY", f_header, WritingBrush, col5 + SATURDAY, ctop + 5);


            for (int i = 0; i < 6; i++) // Row
            {
                for (int j = 1; j < 8; j++) // Col
                {
                    if ((DayLabels[(i * 7) + j - 1] > 0))
                    {
                        int Day_Number = DayLabels[(i * 7) + j - 1];
                        // Circle today's date
                        if (DateTime.Now.Day == Day_Number && DateTime.Now.Month == current_month && DateTime.Now.Year == current_year)
                        {
                            Point Temp_Pt = Get_Calendar_Intersection(j, i + 1);

                            // Circle today's date number
                            Temp_Pt = Get_Calendar_Intersection(j, i + 1, col_width - (Day_Number >= 10 ? 23 : 18), 2);
                            Rectangle r = new Rectangle(Temp_Pt.X - 4, Temp_Pt.Y - 6, 30, 30);
                            e.Graphics.DrawEllipse(Red_Pen, r);

                        }
                    }
                }
            }

            // Remove listeners
            this.ResizeBegin -= Calendar_Resize_Begin;
            this.Resize -= Calendar_Resize;
            this.ResizeEnd -= Calendar_Resize_End;


            //if (NLR) this.Size = Resize_Size; else this.Height = Resize_Size.Height - 120;
            this.Height = Resize_Size.Height;// -(!NLR ? row_height : 0);

            // Add listeners
            this.ResizeBegin += Calendar_Resize_Begin;
            this.Resize += Calendar_Resize;
            this.ResizeEnd += Calendar_Resize_End;

            TFLP.Size = new Size(this.Width - 2, this.Height - 2);

            Rearrange_Controls();

            // Dispose all objects
            p.Dispose();
            Grey_Pen.Dispose();
            GreyBrush.Dispose();
            BlueBrush.Dispose();
            RedBrush.Dispose();
            PinkBrush.Dispose();
            GreenBrush.Dispose();
            PurpleBrush.Dispose();
            YellowBrush.Dispose();
            OrangeBrush.Dispose();
            LightOrangeBrush.Dispose();
            Blue_Pen.Dispose();
            Green_Pen.Dispose();
            Red_Pen.Dispose();
            Purple_Pen.Dispose();
            Yellow_Pen.Dispose();
            Orange_Pen.Dispose();
            Box_Orange_Pen.Dispose();
            CyanBrush.Dispose();
            Box_Green_Pen.Dispose();
            WritingBrush.Dispose();
            graph_font.Dispose();
            axis_font.Dispose();
            f_asterisk.Dispose();
            f_minor_strike.Dispose();
            f.Dispose();
            f_title.Dispose();
            f_minor.Dispose();
            f_minor_bold.Dispose();
            f_reg_bold.Dispose();
            f_header.Dispose();

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        private void email_button_Click(object sender, EventArgs e)
        {
            if (parent.Settings_Dictionary["PERSONAL_FIRST_NAME"].Length > 0 && parent.Settings_Dictionary["PERSONAL_EMAIL"].Length > 0 && parent.Settings_Dictionary["PERSONAL_LAST_NAME"].Length > 0)
            {
                Grey_Out();
                Email_Calendar_Range PCR = new Email_Calendar_Range(parent, this, this.Location, this.Size);
                PCR.ShowDialog();
                Grey_In();
            }
            else
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: Missing personal information", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Personal_Information PI = new Personal_Information(parent, this.Location, this.Size);
                PI.ShowDialog();
                Grey_In();
            }
        }

        private void undo_import_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form2 = new Yes_No_Dialog(parent, "Are you sure you wish to undo the calendar import?", "Warning", "No", "Yes", 0, this.Location, this.Size))
            {
                var result = form2.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (form2.ReturnValue1 == "1") // Actually print
                    {
                        parent.Calendar_Events_List = Before_Import_CE_List;
                        parent.Agenda_Item_List = Before_Import_AI_List;
                        parent.Contact_List = Before_Import_Contact_List;
                        undo_import.Enabled = false;
                        undo_import.Image = global::Financial_Journal.Properties.Resources.undo_grey;
                        Invalidate();

                    }
                }
            }
            Grey_In();
        }

        private void import_Click(object sender, EventArgs e)
        {
            string Passcode = "";
            string Hash_Value = "";
            Grey_Out();

            using (var form = new Input_Box_Small(parent, "Please enter the file code for the import file", "Code", "Done", null, this.Location, this.Size, 25))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Hash_Value = form.Pass_String;
                }
                else
                {
                    goto End;
                }
            }

            using (var form = new Input_Box_Small(parent, "Please enter the password for the import file", "Passcode", "Done", null, this.Location, this.Size, 25))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Passcode = form.Pass_String;
                }
                else
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Invalid passcode entered", true, -30, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                    goto End;
                }
            }

            this.Activate();
            Activate();

            Cursor.Current = Cursors.WaitCursor;

            Grey_Out();
            if (secondThreadFormHandle == IntPtr.Zero)
            {
                Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size, "IMPORTING", "CALENDAR")
                {
                };
                form.HandleCreated += SecondFormHandleCreated;
                form.HandleDestroyed += SecondFormHandleDestroyed;
                form.RunInNewThread(false);
            }

            if (Passcode.Length > 0 && Hash_Value.Length > 0)
            {
                string error_str = Import_From_FTP(Passcode, Hash_Value);

                if (secondThreadFormHandle != IntPtr.Zero)
                    PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

                Cursor.Current = Cursors.Default;

                if (error_str.Contains("Error"))
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, error_str, true, -2, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }
                else
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Import Successful!", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                    parent.Background_Save();
                }

            }

            Invalidate();
        End: ;
        }

        private void export_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Export_Calendar_Range ECR = new Export_Calendar_Range(parent, this, this.Location, this.Size);
            ECR.ShowDialog();
            Invalidate();
            Grey_In();
        }



        public List<Calendar_Events> CE_Filtered_List = new List<Calendar_Events>();
        public List<Agenda_Item> AI_Filtered_List = new List<Agenda_Item>();
        public List<Contact> CO_Filtered_List = new List<Contact>();

        /// <summary>
        ///  Filtering the action lists
        /// </summary>
        /// <param name="From_Date"></param>
        /// <param name="To_Date"></param>
        /// <param name="Validate_Shopping_Filter_Dates"></param>
        public void Filter_Lists(DateTime From_Date, DateTime To_Date, bool Validate_Shopping_Filter_Dates)
        {
            // Reset instances
            CE_Filtered_List = new List<Calendar_Events>();
            AI_Filtered_List = new List<Agenda_Item>();
            CO_Filtered_List = new List<Contact>();

            // Create new instances
            parent.Calendar_Events_List.Where(x => x.Date <= To_Date && x.Date >= From_Date).ToList().ForEach(x => CE_Filtered_List.Add(x.Clone_CE()));
            parent.Agenda_Item_List.Where(x => (x.Calendar_Date <= To_Date && x.Calendar_Date >= From_Date) ||
                                x.Shopping_List.Any(y => y.Calendar_Date <= To_Date && y.Calendar_Date >= From_Date)).ToList().ForEach(x => AI_Filtered_List.Add(x.Clone_Agenda()));

            // Remove shopping items not in time range by validating dates
            if (Validate_Shopping_Filter_Dates)
            {
                AI_Filtered_List.ForEach(x => x.Shopping_List = x.Shopping_List.Where(y => y.Calendar_Date <= To_Date && y.Calendar_Date >= From_Date).ToList());
            }

            // Filter contacts
            List<string> Contact_Hash_List = new List<string>();

            // Iterate through the tree lists for the hash values
            CE_Filtered_List.Where(x => x.Contact_Hash_Value.Length > 0).ToList().ForEach(x => Contact_Hash_List.Add(x.Contact_Hash_Value));
            AI_Filtered_List.Where(x => x.Contact_Hash_Value.Length > 0).ToList().ForEach(x => Contact_Hash_List.Add(x.Contact_Hash_Value));
            AI_Filtered_List.ForEach(z => z.Shopping_List.Where(x => x.Contact_Hash_Value.Length > 0).ToList().ForEach(x => Contact_Hash_List.Add(x.Contact_Hash_Value)));

            Contact_Hash_List = Contact_Hash_List.Distinct().ToList();

            // Append distinct hash values to filtered list
            foreach (Contact C in parent.Contact_List.Where(x => Contact_Hash_List.Contains(x.Hash_Value)).ToList())
                CO_Filtered_List.Add(C.Clone_Contact());

            Grey_Out();
            Export_Filter EF = new Export_Filter(parent, this, this.Location, this.Size);
            EF.ShowDialog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!showWeather)
            {
                Populate_Weather_Dictionary();
            }

            showWeather = showWeather ? false : true;
            weatherbutton.Image = showWeather ? global::Financial_Journal.Properties.Resources.raingreen : global::Financial_Journal.Properties.Resources.rain;


            Invalidate();
            Reset_Tooltips();
        }

        List<MultiDayEvents> MultiDayList = new List<MultiDayEvents>();

        private int colorIndex = 0;
        List<Color> ColorList = new List<Color>() { Color.MediumSpringGreen, 
                                                    Color.FromArgb(237, 106, 219),
                                                    Color.LightSkyBlue,
                                                    Color.Gold
        };

        private Color GetNextColor()
        {
            if (colorIndex > ColorList.Count - 1) colorIndex = 0;
            return ColorList[colorIndex++];
        }

        private void PopulateMultiDays(int month, int year)
        {
            colorIndex = 0;
            MultiDayList = new List<MultiDayEvents>();

            foreach (Calendar_Events
                CE in parent.Calendar_Events_List)//.Where(x => x.Date.Month == month && x.Date.Year == year).OrderBy(x => x .Date))
            {
                if (CE.MultiDays > 0)
                {
                    MultiDayEvents MDE =
                        new MultiDayEvents(GetNextColor())
                        {
                            calendarHash = CE.Hash_Value,
                            eventDays = new List<DateTime>()
                        };

                    for (int i = 0; i < CE.MultiDays + 1; i++)
                    {
                        MDE.eventDays.Add(CE.Date.AddDays(i).Date);
                    }

                    Dictionary<DateTime, int> overlapIndices = new Dictionary<DateTime, int>();

                    // Check overlapping days and add appropriate index
                    foreach (DateTime eventDay in MDE.eventDays)
                    {
                        overlapIndices.Add(eventDay, 0);

                        foreach (MultiDayEvents MDE2 in MultiDayList)
                        {
                            if (MDE2.eventDays.Contains(eventDay.Date)) overlapIndices[eventDay]++;
                        }

                    }

                    if (overlapIndices.Where(x => x.Value > 0).ToList().Count > 0)
                        MDE.lineIndex = overlapIndices.Max(x => x.Value); // get max day

                    MultiDayList.Add(MDE);
                }
            }
        }
    }

    public enum DateType
    {
        Start,
        Middle,
        End
    }

    public class MultiDayEvents
    {
        public string calendarHash { get; set; } //who this event belogns to
        public List<DateTime> eventDays { get; set; }
        public Color lineColor { get; set; }
        public int lineIndex { get; set; } //line index (overlapping lines goes to next index instead)
        public bool hasDailyTotalEntry { get; set; }

        public MultiDayEvents(Color lineColor)
        {
            this.lineColor = lineColor;
            lineIndex = 0;
            hasDailyTotalEntry = false;
        }

        public DateType GetDateType(DateTime refDate)
        {
            // Assumes eventDays.Count >= 2
            if (refDate.Date == eventDays[0])
            {
                return DateType.Start;
            }
            else if (refDate.Date == eventDays[eventDays.Count - 1])
            {
                return DateType.End;
            }
            else return DateType.Middle;
        }
    }

    public class Calendar_Events
    {
        public string Is_Active { get; set; }
        public string Title { get; set; }
        public string Hash_Value { get; set; }
        public string Contact_Hash_Value { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public List<DateTime> Alert_Dates { get; set; }
        public bool Time_Set { get; set; }
        public int MultiDays { get; set; }

        public Calendar_Events Clone_CE()
        {
            return System.MemberwiseClone.Copy(this);
        }

        // Definition of importance;
        // 0 - No visible alert visible on calendar besides a small icon
        // 1 - Visible icon, preview of titie
        // 2 - Green box around date with all above
        //
        public int Importance { get; set; }

        public Calendar_Events()
        {
            MultiDays = 0; // 0 = none, 1 = next day
            Alert_Dates = new List<DateTime>();
        }

        public override string ToString()
        {
            return String.Format("{1} ({0})", Is_Active, Title);
        }
    }
}
