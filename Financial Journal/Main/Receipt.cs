using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Threading;
using Objects;


namespace Financial_Journal
{
    public partial class Receipt : Form
    {
        public Save_Functionalities SaveHelper;
        public Load_Functionalities LoadHelper;
        public MobileSync MobileSync;

        public bool FTP_Logging = true;
        public bool isTesting = false; // true
        //public bool isTesting = true;

        private Button editFocusButton;

        // Simplified testing function
        private void Run_Test_Form()
        {
            if (!isTesting) return;
        }

        public string localSavePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Personal Banker";
        
        bool Force_Close = false;

        public List<int> Tier_Format = new List<int>();

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            this.WindowState = FormWindowState.Normal;

            bool Quit = false;
            if (Item_List.Count > 0)
            {
                Grey_Out();
                using (var form = new Yes_No_Dialog(this, "You have un-submitted items. Do you wish to terminate?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        if (form.ReturnValue1 == "1")
                        {
                            Quit = true;
                        }
                    }
                }
                Grey_In();
            }
            else
            {
                Quit = true;
            }
            if (Quit)
            {

                #region Check if and if so, delete generated files
                if (Settings_Dictionary.ContainsKey("AUTO_DELETE") && Settings_Dictionary["AUTO_DELETE"] == "1")
                {
                    foreach (string path in GeneratedFilePaths_List)
                    {
                        try
                        {
                            #region Shred file (aka dump nothing)
                            using (StreamWriter sw = File.CreateText(path)) 
                            {
                                sw.Write("");
                                sw.Close();
                            }
                            #endregion
                            File.Delete(path);
                        }
                        catch (Exception es)
                        {
                            Diagnostics.WriteLine(es.ToString());
                        }
                    }
                }
                #endregion

                if (Settings_Dictionary.ContainsKey("AUTO_SAVE") && Settings_Dictionary["AUTO_SAVE"] == "1")
                {
                    // Hide before saving
                    //this.Visible = false;

                    // Set last login to now
                    Settings_Dictionary["LAST_LOGIN"] = DateTime.Now.ToString();

                    SaveHelper.Regular_Save();
                }
                else if (!SaveHelper.Check_Difference() && !Force_Close)
                {
                    #region Compare current version with the original loaded version and if changes, prompt a save
                    Grey_Out();
                    using (var form = new Yes_No_Dialog(this, "You have made changes. Do you wish to save changes?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                    {
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            if (form.ReturnValue1 == "1")
                            {
                                // Set last login to now
                                Settings_Dictionary["LAST_LOGIN"] = DateTime.Now.ToString();

                                // Hide before saving
                                //this.Visible = false;

                                SaveHelper.Regular_Save();
                            }
                        }
                    }
                    Grey_In();
                    #endregion
                }
                try
                {
                    SaveHelper.Delete_Temps(localSavePath);
                }
                catch
                {
                }

                #region Save to cloud if necessary
                if (Settings_Dictionary.ContainsKey("CLOUD_SYNC_ON_CLOSE") && Settings_Dictionary["CLOUD_SYNC_ON_CLOSE"] == "1")
                {
                    Grey_Out();
                    Application.DoEvents();
                    Cursor.Current = Cursors.WaitCursor;

                    if (secondThreadFormHandle == IntPtr.Zero)
                    {

                        Loading_Form form = new Loading_Form(this, new Point(this.Location.X, this.Location.Y), this.Size, "SYNCING DATA", "TO CLOUD")
                        {
                        };
                        form.HandleCreated += SecondFormHandleCreated;
                        form.HandleDestroyed += SecondFormHandleDestroyed;
                        form.RunInNewThread(false);
                    }

                    Cloud_Save(false);

                    if (secondThreadFormHandle != IntPtr.Zero)
                        PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

                    Grey_In();
                }
                #endregion

                #region Clear Systray Icon
                try
                {
                    if (notifyIcon1 != null)
                    {
                        notifyIcon1.Visible = false;
                        notifyIcon1.Icon = null;
                        notifyIcon1.Dispose();
                        notifyIcon1 = null;

                        System.Windows.Forms.Application.DoEvents();
                    }
                }
                catch { }
                #endregion

                this.Dispose();
                this.Close();
                base.OnFormClosing(e);
                Application.Exit();
            }
            e.Cancel = true;
        }

        #region Minimize to tray
        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                notifyIcon1.Visible = true; 
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.BalloonTipText = "your text";
                notifyIcon1.BalloonTipTitle = "Welcome Message";
                //notifyIcon1.ShowBalloonTip(500);
                this.Hide();

            }

            else if (FormWindowState.Normal == this.WindowState && notifyIcon1 != null)
            {
                notifyIcon1.Visible = false;
            }
        }
        #endregion

        #region Time-out functionality
        // Detect user input in order to reset inactivity
        [DllImport("User32.dll")]
        private static extern bool
                GetLastInputInfo(ref LASTINPUTINFO plii);

        internal struct LASTINPUTINFO
        {
            public uint cbSize;

            public uint dwTime;
        }

        public static uint GetIdleTime()
        {
            LASTINPUTINFO lastInPut = new LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            GetLastInputInfo(ref lastInPut);

            return ((uint)Environment.TickCount - lastInPut.dwTime);
        }

        public static long GetTickCount()
        {
            return Environment.TickCount;
        }

        private System.Windows.Forms.Timer CheckIdleTimer;
        private System.Windows.Forms.Timer FadeTimer;
        private System.Windows.Forms.Timer FadeTimerWeather;
        private System.Windows.Forms.Timer MarqueeTimer;

        public int _TIMEOUT_REF = 600000;// x1000 = seconds

        public int statusCount = 8; //must add upon adding new iterator (number of statuses)

        public int currentStatus = 1;
        public int statusResetSeconds = 6;
        public int weatherResetSeconds = 14;
        public int seconds = 0;

        #region Status Bar Variables (to prevent recalcuations - immutable)
        int SB_shipmentCount = 0;
        int currentAlertCount = 0;
        int currentCalendarCount = 0;
        int currentRefundOrderCount = 0;
        int currentTrackingCount = 0;
        string currentStatusBarOrder = "";
        string currentTrackingOrder = "";
        int isPayDate = -1;

        private bool weatherInfoLoaded = false;
        #endregion

        private void CheckIdleTimer_Tick(object sender, System.EventArgs e)
        {
            if (seconds <= 0)
            {
                if (Settings_Dictionary.ContainsKey("START_MINIMIZED") && Settings_Dictionary["START_MINIMIZED"] == "1")
                {
                    minimize_button.PerformClick();
                }
            }

            seconds++; 

            if (this.WindowState == FormWindowState.Minimized && GetIdleTime() > 10000)
            {
                isAFK = true;
            }

            if (GetIdleTime() > (_TIMEOUT_REF - 30000) && GetIdleTime() < (_TIMEOUT_REF - 29000) && Settings_Dictionary["SESSION_EXPIRY"] == "1" && this.WindowState != FormWindowState.Minimized)
            {
                //SaveHelper.Regular_Save();
                //Environment.Exit(0);
                this.Close();
            }

            if (this.WindowState != FormWindowState.Minimized && !ftpWorkerWorking) // Only check timers if not minimized
            {
                #region Fade in and out weather
                if (seconds % weatherResetSeconds == 0 && weatherInfoLoaded)
                {
                    // Fade out
                    FadeTimerWeather.Start();
                    setFade2 = !setFade2;

                    if (weatherLabelString == weatherLabelStringTemperature)
                    {
                        weatherLabelString = weatherLabelStringCondition;
                    }
                    else
                    {
                        weatherLabelString = weatherLabelStringTemperature;
                    }
                }
                #endregion

                #region Fade in and out spending label
                if (seconds % statusResetSeconds == 0)
                {
                    // Fade out
                    FadeTimer.Start();
                    setFade = !setFade;

                    if (currentStatus == 0)
                    {
                        StatusSetSpending(false);
                    }

                    #region Check Budget Information
                    if (currentStatus == 1)
                    {
                        if (Monthly_Income >= 0 && Current_Month_Expenditure >= 0)
                        {
                            using (Savings_Helper SH = new Savings_Helper(this))
                            {
                                double current_savings = SH.Get_Monthly_Salary(DateTime.Now.Month, DateTime.Now.Year) - Current_Month_Expenditure;
                                spendingLabelString = "Your current monthly savings: " + (current_savings < 0 ? "-" : "") + "$" + (String.Format("{0:0.00}", Math.Abs(current_savings)));
                            }
                        }
                        else
                            currentStatus++;
                    }
                    #endregion
                    #region Check shipping
                    if (currentStatus == 2)
                    {
                        // Get Shipments
                        List<Shipment_Tracking> Temp = Tracking_List.Where(x => x.Status == 1 && x.Expected_Date.Date == DateTime.Now.Date && x.Ref_Order_Number != "999999999").ToList();

                        if (Temp.Count > 0)
                        {

                            if (currentTrackingCount >= Temp.Count) currentTrackingCount = 0; //cycle

                            Order refOrder = Order_List.First(x => x.OrderID == Temp[currentTrackingCount].Ref_Order_Number);

                            currentTrackingCount++;

                            currentTrackingOrder = refOrder.OrderID;
                            spendingLabelString = "Exp. delivery from " + refOrder.Location;

                            if (Temp.Count > currentTrackingCount)
                                currentStatus--; // Move to next shipment reminder
                            }
                        else
                            currentStatus++;
                    }
                    #endregion
                    #region Check Upcoming Alerts (within 10 minutes)
                    if (currentStatus == 3)
                    {
                        List<SMSAlert> Temp = SMSAlert_List.Where(x => x.Time.TimeOfDay > DateTime.Now.TimeOfDay && DateTime.Now.AddMinutes(10).TimeOfDay > x.Time.TimeOfDay).ToList();

                        if (Temp.Count > 0)
                        {
                            if (currentAlertCount >= Temp.Count) currentAlertCount = 0; //cycle
                            spendingLabelString = "Upcoming alert: " + Temp[currentAlertCount++].Name;

                            if (Temp.Count > currentAlertCount)
                                currentStatus--; // Move to next reminder
                        }
                        else
                            currentStatus++; //skip to next
                    }
                    #endregion
                    #region Check if pay date deposit
                    if (currentStatus == 4)
                    {
                        if (isPayDate < 0)
                        {

                            // Get Pay period that are not complete (and income must be manual)
                            if (Settings_Dictionary.ContainsKey("INCOME_MANUAL") && Settings_Dictionary["INCOME_MANUAL"] == "1")
                            {
                                foreach (CustomIncome CI in Income_Company_List)
                                {
                                    PayPeriod Ref_PP = CI.Intervals[CI.Intervals.Count - 1];
                                    if (Ref_PP.Amount <= 0 && Ref_PP.Pay_Date.Date == DateTime.Now.Date)
                                    {
                                        Ref_PP.Name_IUO = CI.Company;
                                        isPayDate = 1;
                                    }
                                }
                            }
                        }

                        if (isPayDate > 0)
                        {
                            spendingLabelString = "Expecting Pay Cheque today from work!";
                        }
                        else
                            currentStatus++;
                    }
                    #endregion
                    #region Check Calendar Events / Agenda Items
                    if (currentStatus == 5)
                    {
                        List<string> Temp = Calendar_Events_List.Where(x => x.Date.Date == DateTime.Now.Date).ToList().Select(x => x.Title).ToList();

                        // Check any multi-day events to append to list
                        foreach (Calendar_Events CE in Calendar_Events_List)
                        {
                            List<DateTime> tempDateList = new List<DateTime>();
                            if (CE.MultiDays > 0)
                            {
                                // Generate a temporary day list
                                for (int i = 1; i <= CE.MultiDays; i++)
                                {
                                    tempDateList.Add(CE.Date.AddDays(i));
                                }
                                if (tempDateList.Any(x => x.Date == DateTime.Now.Date))
                                {
                                    Temp.Add(CE.Title);
                                }
                            }
                        }

                        Temp = Temp.Distinct().ToList(); // Get unique values

                        Temp.AddRange(Agenda_Item_List.Where(x => x.Calendar_Date.Date == DateTime.Now.Date).ToList().Select(x => x.Name).ToList());

                        if (Temp.Count > 0)
                        {
                            if (currentCalendarCount >= Temp.Count) currentCalendarCount = 0; //cycle
                            spendingLabelString = "Event: " + Temp[currentCalendarCount++];

                            if (Temp.Count > currentCalendarCount)
                                currentStatus--; // Move to next reminder
                        }
                        else
                            currentStatus++; //skip to next
                    }
                    #endregion
                    #region Check items needed to be refunded
                    if (currentStatus == 6)
                    {
                        List<Order> tempOrders = Get_Orders_With_Upcoming_Refunds();

                        if (tempOrders.Count > 0)
                        {
                            if (currentRefundOrderCount >= tempOrders.Count) currentRefundOrderCount = 0; //cycle
                            currentStatusBarOrder = tempOrders[currentRefundOrderCount].OrderID; //reference for clicking later
                            spendingLabelString = "Refund Reminder: " + tempOrders[currentRefundOrderCount++].Location;

                            if (tempOrders.Count > currentRefundOrderCount)
                                currentStatus--; // Move to next reminder
                        }
                        else
                            currentStatus++; //skip to next
                    }
                    #endregion
                    #region Current wallet cash
                    if (currentStatus == 7)
                    {
                        spendingLabelString = "Your wallet has " + Cash.GetCurrentBalanceStr(false) + (Cash.GetCurrentBalance(false) <= 0 ? "   :-(" : "");
                    }
                    #endregion

                    currentStatus++; // natural iteration

                    // reset status
                    if (currentStatus == statusCount) currentStatus = 0;
                    else if (currentStatus > statusCount) // set default if blank currentStatus appears (wrap around)
                    {
                        StatusSetSpending(false);
                        currentStatus = 1;
                    }

                }
                #endregion


                if (seconds % 300 == 0 && Settings_Dictionary["AUTO_MOBILE_SYNC"] == "1") // every 60 seconds
                {
                    bool hasSyncFile = MobileSync.CheckForSyncFiles();

                    if (hasSyncFile && MobileSync.SyncedOrders.Count > 0)
                    {
                        Grey_Out();
                        MobileSyncDialog MSD = new MobileSyncDialog(this, MobileSync.SyncedOrders, MobileSync.SyncedItems,
                            this.Location, this.Size);
                        MSD.ShowDialog();
                        Grey_In();
                    }
                }
            }

            Check_SMS_Alerts();
        }

        bool setFade = true; //0 = out, 1 = in
        bool setFade2 = true; //0 = out, 1 = in

        private void FadeTimer_Tick(object sender, System.EventArgs e)
        {
            if (setFade && spendingLabel.ForeColor != Color.Silver)
            {
                FadeOutSpendingLabel(1000);
            }
            else if (!setFade && spendingLabel.ForeColor != spendingLabel.BackColor)
            {
                FadeOutSpendingLabel();
            }

        }

        private void FadeTimerWeather_Tick(object sender, System.EventArgs e)
        {
            if (setFade2 && weatherLabel.ForeColor != Color.Silver)
            {
                FadeOutWeatherLabel(1000);
            }
            else if (!setFade2 && weatherLabel.ForeColor != weatherLabel.BackColor)
            {
                FadeOutWeatherLabel();
            }

        }
        #endregion


        #region Marquee features
        private void MarqueeTimer_Tick(object sender, System.EventArgs e)
        {
            if (WindowState != FormWindowState.Minimized)
                ShiftMarquee();
        }

        private bool marqueeSwitch1 = true;
        private bool marqueeSwitch2 = true;

        private int repeatCount = 1;

        private int endBoundary = 0;
        private int startBoundary = 0;

        private void ShiftMarquee()
        {
            // Marquee wrap around

            if (marqueeLabel.Left < endBoundary && marqueeSwitch1)
            {
                if (repeatCount % 3 == 0)
                    RefreshMarqueeText(2);
                marqueeLabel2.Left = startBoundary;
                marqueeSwitch1 = false;
                repeatCount++;
            }
            else if (marqueeLabel2.Left < endBoundary && marqueeSwitch2)
            {
                if (repeatCount % 3 == 0)
                    RefreshMarqueeText(1);
                marqueeLabel.Left = startBoundary;
                marqueeSwitch2 = false;
                repeatCount++;
            }
            
            // Toggle switches
            if (marqueeLabel.Left < endBoundary)
            {
                marqueeSwitch2 = true;
            }
            else if (marqueeLabel2.Left < endBoundary)
            {
                marqueeSwitch1 = true;
            }

            marqueeLabel.Left -= 1;
            marqueeLabel2.Left -= 1;
        }
        #endregion

        string spendingLabelString;

        /// <summary>
        /// direction = 1 = fade in
        /// </summary>
        /// <param name="direction"></param>
        private void FadeOutSpendingLabel(int direction = -1)
        {

            HSLColor hsl = new HSLColor(spendingLabel.ForeColor);
            if (direction > 0)
            {
                hsl.Luminosity += 1.9;
            }
            else
            {
                hsl.Luminosity -= 1; // Brightness is here lightness
            }

            //if (direction < 0 && (Color)hsl == spendingLabel.BackColor) return;
            if (direction > 0 && hsl.Luminosity > 182.704885887146)
            {
                FadeTimer.Stop();
                return;
            }
            if (direction < 0 && hsl.Luminosity < 61.1754740943909)
            {
                // Fade in
                spendingLabel.Text = spendingLabelString;
                setFade = !setFade; 
                return;
            }

            spendingLabel.ForeColor = (Color)hsl;
        }

        /// <summary>
        /// direction = 1 = fade in
        /// </summary>
        /// <param name="direction"></param>
        private void FadeOutWeatherLabel(int direction = -1)
        {
            HSLColor hsl = new HSLColor(weatherLabel.ForeColor);
            if (direction > 0)
            {
                hsl.Luminosity += 1.9;
            }
            else
            {
                hsl.Luminosity -= 1; // Brightness is here lightness
            }

            //if (direction < 0 && (Color)hsl == spendingLabel.BackColor) return;
            if (direction > 0 && hsl.Luminosity > 182.704885887146)
            {
                FadeTimerWeather.Stop();
                return;
            }
            if (direction < 0 && hsl.Luminosity < 61.1754740943909)
            {
                // Fade in
                weatherLabel.Text = weatherLabelString;
                setFade2 = !setFade2; 
                return;
            }

            weatherLabel.ForeColor = (Color)hsl;
        }

        #region QL Variables
        int QL_Row = 1;
        int QL_Column = 1;
        int QL_Max_Col = 6;
        int QL_Column_Spacing = 90;
        int QL_Row_Spacing = 80;
        #endregion

        public void Calculate_Locations()
        {
            QL_Column++;
            if (QL_Column > QL_Max_Col)
            {
                QL_Column = 1;
                QL_Row++;
            }
        }

        private List<Button> Icon_Button = new List<Button>();

        private bool repaintButtons = true;
        int QL_Height_Factor = 0;

        protected override void OnPaint(PaintEventArgs e)
        {
            toolStripMenuItem52.Enabled = (Settings_Dictionary.ContainsKey("LOGIN_EMAIL") && Settings_Dictionary["LOGIN_EMAIL"].Length > 5);

            QL_Row = 1;
            QL_Column = 1;


            button5.Enabled = false;

            int data_height = 26;
            int start_height = Start_Size.Height - statusBarPanel.Height;
            int start_margin = 15;              // Item
            int height_offset = 9;

            int margin1 = start_margin + 170;   //Price
            int margin2 = margin1 + 85;        //Quantity
            int margin3 = margin2 + 125;        //Category
            int margin4 = margin3 + 162;        //Actions

            int row_count = 0;

            Color DrawForeColor = Color.White;
            Color BackColor = Color.FromArgb(64, 64, 64);
            Color HighlightColor = Color.FromArgb(76, 76, 76);

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(88, 88, 88));
            SolidBrush GreyBrushIcon = new SolidBrush(SystemColors.ButtonShadow);
            SolidBrush RedBrush = new SolidBrush(Color.LightPink);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 2);
            Pen Grey_Pen_Icon = new Pen(GreyBrush, 1);

            Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
            Font f_temp = new Font("MS Reference Sans Serif", 8, FontStyle.Regular);
            Font f = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
            Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);
            Font f_italic = new Font("MS Reference Sans Serif", 9, FontStyle.Italic);

            bool has_exempt = false;
            int item_index = 0;
            double Running_Total = 0;
            double Tax_Total = 0;
            int Running_Quantity = 0;
            Running_Total_Master = 0;

            double total_discount = Item_List.Where(item => item.Discount_Amt > 0).Sum(item => item.Discount_Amt);

            // Remove existing buttons
            if (repaintButtons || Item_List.Count > 0)
            {
                Icon_Button.ForEach(button => this.Controls.Remove(button));
                Icon_Button.ForEach(button => button.Image.Dispose());
                Icon_Button.ForEach(button => button.Dispose());
                Icon_Button = new List<Button>();
            }

            Delete_Item_Buttons.ForEach(button => button.Image.Dispose());
            Delete_Item_Buttons.ForEach(button => button.Dispose());
            Delete_Item_Buttons.ForEach(button => this.Controls.Remove(button));
            Delete_Item_Buttons = new List<Button>();
            Edit_Buttons.ForEach(button => button.Image.Dispose());
            Edit_Buttons.ForEach(button => button.Dispose());
            Edit_Buttons.ForEach(button => this.Controls.Remove(button));
            Edit_Buttons = new List<Button>();
            Discount_Buttons.ForEach(button => button.Image.Dispose());
            Discount_Buttons.ForEach(button => button.Dispose());
            Discount_Buttons.ForEach(button => this.Controls.Remove(button));
            Discount_Buttons = new List<Button>();


            // Setup Quick Link Icons
            if (Item_List.Count == 0 && Settings_Dictionary.ContainsKey("QL_ENABLED") &&
                Settings_Dictionary["QL_ENABLED"] == "1")
            {

                #region Agenda

                if (Settings_Dictionary["QL_AGENDA"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.agenda,
                            Name = "Agenda"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("VIEW", f, GreyBrushIcon,
                        new Point(start_margin + 3 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("AGENDA", f, GreyBrushIcon,
                        new Point(start_margin - 7 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region Budget

                if (Settings_Dictionary["QL_BUDGET"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.qlbudget,
                            Name = "Budget"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("VIEW", f, GreyBrushIcon,
                        new Point(start_margin + 3 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("BUDGET", f, GreyBrushIcon,
                        new Point(start_margin - 7 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region Calendar

                if (Settings_Dictionary["QL_CALENDAR"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.calendar,
                            Name = "Calendar"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("VIEW", f, GreyBrushIcon,
                        new Point(start_margin + 3 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("CALENDAR", f, GreyBrushIcon,
                        new Point(start_margin - 13 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region Contacts

                if (Settings_Dictionary["QL_CONTACTS"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.C_Family,
                            Name = "Contacts"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("PERSONAL", f, GreyBrushIcon,
                        new Point(start_margin - 10 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("CONTACTS", f, GreyBrushIcon,
                        new Point(start_margin - 11 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region Deposit

                // Only if manual income and default set
                if (Settings_Dictionary["QL_DEPOSIT_PAY"] == "1" && Settings_Dictionary["INCOME_MANUAL"] == "1" &&
                    Income_Company_List.FirstOrDefault(x => x.Default) != null)
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.deposit,
                            Name = "Deposit"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("DEPOSIT", f, GreyBrushIcon,
                        new Point(start_margin - 8 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("PAYCHECK", f, GreyBrushIcon,
                        new Point(start_margin - 12 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region Hobby

                if (Settings_Dictionary["QL_MANAGE_HOBBY"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.hobby,
                            Name = "Hobby"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("MANAGE", f, GreyBrushIcon,
                        new Point(start_margin - 6 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("HOBBY", f, GreyBrushIcon,
                        new Point(start_margin - 2 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region Payments

                if (Settings_Dictionary["QL_MANAGE_PAYMENT"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.payments,
                            Name = "Payments"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("MANAGE", f, GreyBrushIcon,
                        new Point(start_margin - 6 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("PAYMENTS", f, GreyBrushIcon,
                        new Point(start_margin - 12 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region Mobile Sync

                if (Settings_Dictionary["LOGIN_EMAIL"].Length > 5 && Settings_Dictionary["QL_MOBILE_SYNC"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.mobilesync,
                            Name = "QLMobileSync"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("MOBILE", f, GreyBrushIcon,
                        new Point(start_margin - 12 - 22 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("SYNC", f, GreyBrushIcon,
                        new Point(start_margin - 12 - 15 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region Purchases

                if (Settings_Dictionary["QL_VIEW_PURCHASES"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.purchases,
                            Name = "Purchases"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("VIEW", f, GreyBrushIcon,
                        new Point(start_margin + 3 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("PURCHASES", f, GreyBrushIcon,
                        new Point(start_margin - 16 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region Online

                if (Settings_Dictionary["QL_VIEW_ONLINE"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.online,
                            Name = "Online"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("ONLINE", f, GreyBrushIcon,
                        new Point(start_margin - 5 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("ORDERS", f, GreyBrushIcon,
                        new Point(start_margin - 6 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region ItemLookup

                if (Settings_Dictionary["QL_LOOKUP"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.lookup,
                            Name = "Lookup"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("QUICK ITEM", f, GreyBrushIcon,
                        new Point(start_margin - 17 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("LOOKUP", f, GreyBrushIcon,
                        new Point(start_margin - 5 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region Shopping List

                if (Settings_Dictionary["QL_SHOPPING_LIST"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.qlshoppingCart,
                            Name = "ShoppingList"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("SHOPPING", f, GreyBrushIcon,
                        new Point(start_margin - 10 - 28 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("LIST", f, GreyBrushIcon,
                        new Point(start_margin - 5 - 15 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region Sneak Peek

                if (Settings_Dictionary["QL_SNEAK_PEEK"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.sneakpeek,
                            Name = "sneakpeek"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("UPCOMING", f, GreyBrushIcon,
                        new Point(start_margin - 12 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("SCHEDULE", f, GreyBrushIcon,
                        new Point(start_margin - 12 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region SMS Alert

                if (Settings_Dictionary["QL_SMS_ALERT"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.smsalert,
                            Name = "QLSMSAlert"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("SMS", f, GreyBrushIcon,
                        new Point(start_margin - 12 - 12 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("ALERTS", f, GreyBrushIcon,
                        new Point(start_margin - 12 - 22 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region QLSettings

                if (Settings_Dictionary["QL_ENABLED"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.qlsettings,
                            Name = "QLSettings"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("QUICK LINKS", f, GreyBrushIcon,
                        new Point(start_margin - 19 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("SETTINGS", f, GreyBrushIcon,
                        new Point(start_margin - 10 - 27 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                #region QLWallet

                if (Settings_Dictionary["QL_ENABLED"] == "1")
                {
                    if (repaintButtons)
                    {
                        Button QL_Button = new Button()
                        {
                            // Unchanged
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Size = new Size(40, 40),
                            Location = new Point(start_margin - 27 + QL_Column * QL_Column_Spacing,
                                80 + (QL_Row - 1) * (QL_Row_Spacing)),

                            // Change these below
                            Image = global::Financial_Journal.Properties.Resources.deposit,
                            Name = "QLWallet"
                        };
                        QL_Button.Click += new EventHandler(this.QL_Button_Click);
                        Icon_Button.Add(QL_Button);
                        this.Controls.Add(QL_Button);
                    }

                    e.Graphics.DrawString("VIEW", f, GreyBrushIcon,
                        new Point(start_margin - 2 - 20 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) - 2));
                    e.Graphics.DrawString("WALLET", f, GreyBrushIcon,
                        new Point(start_margin - 10 - 20 + QL_Column * QL_Column_Spacing,
                            80 + (QL_Row - 1) * (QL_Row_Spacing) + (40 + 0) + 13 - 2));
                    Calculate_Locations();
                }

                #endregion

                // Adjust empty last row
                if (QL_Column == 1) QL_Row--;

                // Get Height Factor of all icons
                QL_Height_Factor = (QL_Row_Spacing * (QL_Row) + 40) * (Icon_Button.Count == 0 ? 0 : 1);

                int side_spacing = 40;

                // Only draw if there are icons
                if (Icon_Button.Count > 0)
                {
                    e.Graphics.DrawString("QUICK  LINKS", f_total, WritingBrush, (this.Width / 2) - 48, 57);
                    //e.Graphics.DrawRectangle(Grey_Pen_Icon, new Rectangle(side_spacing, 55, this.Width - (side_spacing) * 2, QL_Height_Factor - 15));
                    //e.Graphics.DrawRectangle(Grey_Pen, new Rectangle(side_spacing, 53, this.Width - (side_spacing) * 2, QL_Height_Factor - 15));
                    DrawRoundedRectangle(e.Graphics, Grey_Pen,
                        new Rectangle(side_spacing, 53, this.Width - (side_spacing) * 2, QL_Height_Factor - 15), 6);
                }

                repaintButtons = false;
            }
            else
            {
                QL_Height_Factor = 0;
            }

            start_height += QL_Height_Factor;


            // If has order
            if (paint && Item_List.Count > 0)
            {
                submit_button.Image = Editing_Receipt ? global::Financial_Journal.Properties.Resources.floppy : global::Financial_Journal.Properties.Resources.checkout;
                label12.Text = Editing_Receipt ? " SAVE" : "SUBMIT";

                bufferedPanel2.Visible = true;

                //location_box.Enabled = false;
                //payment_type.Enabled = false;
                dateTimePicker1.Enabled = false;
                //location_box.ForeColor = Color.LightBlue;
                //payment_type.ForeColor = Color.LightBlue;


                // Draw gray header line
                e.Graphics.DrawLine(Grey_Pen, start_margin, start_height, this.Width - 15, start_height);

                height_offset += 1;
                // Header2   
                e.Graphics.DrawString("Item", f_header, WritingBrush, start_margin, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("Unit Price", f_header, WritingBrush, margin1, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("Quantity", f_header, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("Category", f_header, WritingBrush, margin3 - 10, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("Actions", f_header, WritingBrush, margin4, start_height + height_offset + (row_count * data_height));
                row_count += 1;
                height_offset += 5;

                foreach (Item item in Item_List)
                {
                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button delete_button = new Button();
                    delete_button.BackColor = this.BackColor;
                    delete_button.ForeColor = this.BackColor;
                    delete_button.FlatStyle = FlatStyle.Flat;
                    delete_button.Image = item.Status != "0" ? global::Financial_Journal.Properties.Resources.na : global::Financial_Journal.Properties.Resources.delete;
                    delete_button.Enabled = item.Status == "0";
                    delete_button.Size = new Size(29, 29);
                    delete_button.Location = new Point(this.Width - 40, start_height + height_offset + (row_count * data_height) - 6);
                    delete_button.Name = "d" + item_index.ToString();
                    delete_button.Text = "";
                    delete_button.Click += new EventHandler(this.dynamic_button_click);
                    Delete_Item_Buttons.Add(delete_button);
                    ToolTip1.SetToolTip(delete_button, "Delete " + item.Name);
                    this.Controls.Add(delete_button);

                    Button refund_notice_button = new Button();
                    refund_notice_button.BackColor = this.BackColor;
                    refund_notice_button.ForeColor = this.BackColor;
                    refund_notice_button.FlatStyle = FlatStyle.Flat;
                    refund_notice_button.Size = new Size(29, 29);
                    refund_notice_button.Image = item.RefundAlert ? global::Financial_Journal.Properties.Resources.refundAlertGreen : global::Financial_Journal.Properties.Resources.refundAlert;
                    refund_notice_button.Location = new Point(this.Width - 70, start_height + height_offset + (row_count * data_height) - 6);
                    refund_notice_button.Name = "r" + item_index.ToString();
                    refund_notice_button.Text = "";
                    refund_notice_button.Click += new EventHandler(this.dynamic_button_click);
                    Discount_Buttons.Add(refund_notice_button);
                    ToolTip1.SetToolTip(refund_notice_button, (!item.RefundAlert ? "Set" : "Remove") + " refund notice for " + item.Name);
                    this.Controls.Add(refund_notice_button);

                    Button discount_button = new Button();
                    discount_button.BackColor = this.BackColor;
                    discount_button.ForeColor = this.BackColor;
                    discount_button.FlatStyle = FlatStyle.Flat;
                    discount_button.Size = new Size(29, 29);
                    discount_button.Enabled = item.Status == "0";
                    discount_button.Image = item.Status != "0" ? global::Financial_Journal.Properties.Resources.na : global::Financial_Journal.Properties.Resources.discount;
                    discount_button.Location = new Point(this.Width - 100, start_height + height_offset + (row_count * data_height) - 6);
                    discount_button.Name = "s" + item_index.ToString();
                    discount_button.Text = "";
                    discount_button.Click += new EventHandler(this.dynamic_button_click);
                    Discount_Buttons.Add(discount_button);
                    ToolTip1.SetToolTip(discount_button, "Apply Discount to " + item.Name);
                    this.Controls.Add(discount_button);

                    Button edit_button = new Button();
                    edit_button.BackColor = this.BackColor;
                    edit_button.ForeColor = this.BackColor;
                    edit_button.FlatStyle = FlatStyle.Flat;
                    edit_button.Size = new Size(29, 29);
                    edit_button.Enabled = item.Status == "0";
                    edit_button.Image = (edit_index == item_index ? global::Financial_Journal.Properties.Resources.accept : item.Status != "0" ? global::Financial_Journal.Properties.Resources.na : global::Financial_Journal.Properties.Resources.edit);
                    edit_button.Location = new Point(this.Width - 130, start_height + height_offset + (row_count * data_height) - 6);
                    edit_button.Name = "e" + item_index.ToString();
                    edit_button.Text = "";
                    edit_button.Click += new EventHandler(this.dynamic_button_click);
                    Edit_Buttons.Add(edit_button);
                    ToolTip1.SetToolTip(edit_button, (edit_index == item_index ? "Apply " : "Edit " + item.Name));
                    this.Controls.Add(edit_button);


                    string temp = Tax_Rules_Dictionary.ContainsKey(item.Category) ? ((Tax_Rules_Dictionary[item.Category] == "0") ? "*" : "") : "";
                    if (temp.Length > 0) has_exempt = true;

                    e.Graphics.DrawString(item.Name, f_temp, (item.Status == "0") ? WritingBrush : RedBrush, start_margin + 4, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", item.Price) + temp, f_temp, WritingBrush, margin1 + 12 - (item.Price >= 10 ? 7 : 0), start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString((item.Quantity - Convert.ToInt32(item.Status)).ToString(), f_temp, WritingBrush, margin2 + 22 - (item.Quantity >= 10 ? 5 : 0), start_height + height_offset + (row_count * data_height));
                    using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1)))
                    {
                        SizeF size = graphics.MeasureString(item.Category, new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point));
                        e.Graphics.DrawString(item.Category, f_temp, WritingBrush, margin3 + 18 - (size.Width) / 2, start_height + height_offset + (row_count * data_height));
                    }

                    if (item.Discount_Amt > 0)
                    {
                        row_count++;
                        height_offset -= 5;
                        e.Graphics.DrawString("Discount", f_italic, WritingBrush, margin1 - 78, start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawString("-($" + String.Format("{0:0.00}", item.Discount_Amt) + ")", f_temp, WritingBrush, margin1 + 4 - (item.Price >= 10 ? 7 : 0), start_height + height_offset + (row_count * data_height));
                        height_offset += 2;
                    }
                    if (item.Status != "0")
                    {
                        row_count++;
                        height_offset -= 3;
                        if (item.Discount_Amt > 0) row_count--;
                        e.Graphics.DrawString(((item.Status == "0") ? "" : " (Refunded " + item.Status + ")"), f_italic, (item.Status == "0") ? WritingBrush : RedBrush, start_margin - 2, start_height + height_offset + (row_count * data_height));
                        height_offset += 2;
                    }

                    // Adjust for refunded items w/ discount
                    total_discount -= item.Discount_Amt - item.Get_Current_Discount();
                    Running_Total -= item.Price * (Convert.ToInt32(item.Status));
                    Tax_Total -= item.Price * item.Quantity * Get_Tax_Amount(item) - item.Get_Current_Tax_Amount(Get_Tax_Amount(item));

                    Running_Total += item.Price * item.Quantity;
                    Running_Quantity += item.Status.StartsWith("r") ? (item.Quantity - Convert.ToInt32(item.Status.Substring(1))) : (item.Quantity - Convert.ToInt32(item.Status));
                    Tax_Total += (Tax_Rules_Dictionary.ContainsKey(item.Category) ? Convert.ToDouble(Tax_Rules_Dictionary[item.Category]) * item.Price * item.Quantity : Tax_Rate * item.Price * item.Quantity);
                    row_count++;
                    item_index++;
                    height_offset += 2;
                }

                Running_Total_Master = (Running_Total + (Tax_Exempt_Order ? 0 : (Tax_Override_Amt > 0 ? Tax_Override_Amt : Tax_Total)) - total_discount);

                // Apply Gift card amount
                GC_Amt = (Running_Total_Master <= GC_Available_Credit ? Running_Total_Master : GC_Available_Credit);

                // Total line
                e.Graphics.DrawLine(p, start_margin, start_height + height_offset + (row_count * data_height), this.Width - 15, start_height + height_offset + (row_count * data_height));
                height_offset += 4;
                e.Graphics.DrawString("Subtotal", f, WritingBrush, margin1 - 75, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Running_Total), f, WritingBrush, margin1 + 10 - (Running_Total >= 100 ? 7 : 0), start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString(Running_Quantity.ToString(), f, WritingBrush, margin2 + 19 - (Running_Quantity >= 10 ? 5 : 0), start_height + height_offset + (row_count * data_height));
                row_count++;

                string note = "";
                List<Button> Temp = Edit_Buttons.Where(x => x.Enabled == false).ToList();
                if (Temp.Count() > 0)
                {
                    // Add note
                    height_offset -= 20;
                    e.Graphics.DrawString("*Note: Certain actions are disabled because" + note, f, WritingBrush, margin3 - 24, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("            you cannot modify refunded items" + note, f, WritingBrush, margin3 - 20, start_height + height_offset + (row_count * data_height) + 18);
                    height_offset += 20;
                }

                height_offset -= 2;
                e.Graphics.DrawString("Taxes", f, WritingBrush, margin1 - 60, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString(Tax_Exempt_Order ? "$0.00" : "$" + (Tax_Override_Amt > 0 ? String.Format("{0:0.00}", Tax_Override_Amt) : String.Format("{0:0.00}", Tax_Total)), f, WritingBrush, margin1 + 10 - (Tax_Total >= 10 ? 7 : 0), start_height + height_offset + (row_count * data_height));
                row_count++;
                height_offset -= 2;

                if (total_discount > 0)
                {
                    e.Graphics.DrawString("Less Discounts", f, WritingBrush, margin1 - 110, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("-($" + String.Format("{0:0.00}", total_discount) + ")", f, WritingBrush, margin1 - (total_discount >= 10 ? 7 : 0), start_height + height_offset + (row_count * data_height));
                    row_count++;
                }
                if (Math.Round(GC_Amt, 3) > 0)
                {
                    e.Graphics.DrawString("Gift Credit Applied", f, WritingBrush, margin1 - 135, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("-($" + String.Format("{0:0.00}", GC_Amt) + ")", f, WritingBrush, margin1 - (total_discount >= 10 ? 7 : 0), start_height + height_offset + (row_count * data_height));

                    // Gift card remove button
                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button GC_Button = new Button();
                    GC_Button.BackColor = this.BackColor;
                    GC_Button.ForeColor = this.BackColor;
                    GC_Button.FlatStyle = FlatStyle.Flat;
                    GC_Button.Size = new Size(22, 22);
                    GC_Button.Enabled = true;
                    GC_Button.Image = global::Financial_Journal.Properties.Resources.error;
                    GC_Button.Location = new Point(start_margin + 13, start_height + height_offset + (row_count * data_height) - 3);
                    GC_Button.Name = "gc_button" + item_index.ToString();
                    GC_Button.Text = "";
                    GC_Button.Click += new EventHandler(this.remove_gc_click);
                    Edit_Buttons.Add(GC_Button);
                    ToolTip1.SetToolTip(GC_Button, "Remove Gift Card(s)");
                    this.Controls.Add(GC_Button);

                    row_count++;
                }
                else
                {
                    Pending_GC_Use = new List<GC>();
                    GC_Available_Credit = 0;
                    GC_Amt = 0;

                    foreach (GC GCard in GC_List)
                    {
                        GCard.Reverse_Transaction(Editing_Order.OrderID);
                    }
                }
                height_offset -= 6;
                e.Graphics.DrawLine(p, margin1 - 75, start_height + height_offset + (row_count * data_height), margin2, start_height + height_offset + (row_count * data_height));
                height_offset += 5;
                e.Graphics.DrawString("Total", f_total, WritingBrush, margin1 - 56, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", (Running_Total + (Tax_Exempt_Order ? 0 : (Tax_Override_Amt > 0 ? Tax_Override_Amt : Tax_Total)) - total_discount - GC_Amt)), f_total, WritingBrush, margin1 + 10 - (Tax_Total >= 10 ? (Tax_Total >= 100 ? 15 : 8) : 0), start_height + height_offset + (row_count * data_height));

                row_count++;
                if (has_exempt) e.Graphics.DrawString("*Tax Exempt", f_asterisk, RedBrush, start_margin - 12, start_height + 12 + height_offset + 4 + (row_count * data_height));
                row_count++;


                button5.Enabled = (Running_Total > 0 && (!Editing_Receipt || GC_Amt == 0));

                //row_count++;
                height_offset += 19;
                //height_offset -= 15;
                this.Height = start_height + height_offset + row_count * data_height + (Temp.Count > 0 ? 50 : 0);// +QL_Height_Factor;
            }
            else
            {
                bufferedPanel2.Visible = false;
                this.Height = Start_Size.Height + QL_Height_Factor;
                location_box.Enabled = true;
                payment_type.Enabled = true;
                location_box.ForeColor = Color.White;
                payment_type.ForeColor = Color.White;
            }

            bufferedPanel1.Top = 53 + QL_Height_Factor;// +100; // height of the quick links

            TFLP.Size = new Size(this.Width - 2, this.Height - 2);

            // Dispose all objectsasdfasd
            p.Dispose();
            Grey_Pen.Dispose();
            Grey_Pen_Icon.Dispose();
            GreenBrush.Dispose();
            RedBrush.Dispose();
            GreyBrush.Dispose();
            GreyBrushIcon.Dispose();
            WritingBrush.Dispose();
            f_asterisk.Dispose();
            f_temp.Dispose();
            f_strike.Dispose();
            f_total.Dispose();
            f_header.Dispose();
            f_italic.Dispose();

            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);

        }

        private void QL_Button_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            Grey_Out();
            Application.DoEvents();
            if (b.Name == "Agenda")
            {
                Agenda A = new Agenda(this, null, this.Location, this.Size);
                A.ShowDialog();
            }
            if (b.Name == "Budget")
            {
                BudgetAllocation CG = new BudgetAllocation(this, Location, Size);
                CG.ShowDialog();
            }
            if (b.Name == "Purchases")
            {
                Grey_Out();
                NewPurchases NP = new NewPurchases(this, "", Location, Size);
                NP.ShowDialog();
                Grey_In();
            }
            if (b.Name == "ShoppingList")
            {
                Timeline_Shopping_List TSL = new Timeline_Shopping_List(this, this.Location, this.Size);
                TSL.ShowDialog();
            }
            if (b.Name == "Deposit")
            {
                if (Income_Company_List.Where(x => x.Default).ToList().Count > 0)
                {
                    Deposit_Paycheck DP = new Deposit_Paycheck(this, new Point(this.Location.X, this.Location.Y), this.Size);
                    DP.ShowDialog();
                }
            }
            if (b.Name == "Calendar")
            {
                Calendar c = new Calendar(this);
                c.Show();
            }
            if (b.Name == "Contacts")
            {
                Contacts c = new Contacts(this, this.Location, this.Size);
                c.ShowDialog();
            }
            if (b.Name == "Payments")
            {
                foreach (Payment p in Payment_List)
                {
                    p.Get_Total(Master_Item_List, Tax_Rules_Dictionary, Tax_Rate, Order_List);
                }
                Payment_Information PI = new Payment_Information(this);
                PI.ShowDialog();
            }
            if (b.Name == "Online")
            {
                Online_Orders OO = new Online_Orders(this);
                OO.ShowDialog();
            }
            if (b.Name == "Hobby")
            {
                Hobby_Management HM = new Hobby_Management(this);
                HM.ShowDialog();
            }
            if (b.Name == "sneakpeek")
            {
                Sneak_Peak SP = new Sneak_Peak(this, this.Location, this.Size);
                SP.ShowDialog();
            }
            if (b.Name == "QLSettings")
            {
                Quick_Links QL = new Quick_Links(this, this.Location, this.Size);
                QL.ShowDialog();
                repaintButtons = true;
            }
            if (b.Name == "QLWallet")
            {
                CashView CG = new CashView(this, Location, Size);
                CG.ShowDialog();
                repaintButtons = true;
            }
            if (b.Name == "QLSMSAlert")
            {
                SMS_Alert SA = new SMS_Alert(this, this.Location, this.Size);
                SA.ShowDialog();
            }
            if (b.Name == "QLMobileSync")
            {
                ManualMobileSync();
            }
            if (b.Name == "Lookup")
            {
                Grey_Out();
                using (var form = new Input_Box_Small(this, "Search for item or dollar amount (must start with $):", "Item", "Find", null, this.Location, this.Size, 25))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        string Item_Name = form.Pass_String;

                        List<Order> Current_Order_List = new List<Order>();

                        bool Parse_Error = false; // If true, check item name instead

                        // Search by dollar figure (must start with dollar sign ($) and contain two digits
                        if (Item_Name.StartsWith("$") && Item_Name.Where(x => char.IsDigit(x)).ToList().Count >= 2)
                        {
                            try
                            {
                                foreach (Order order in Order_List)
                                {
                                    string orderTotalString = (order.Order_Total_Pre_Tax + order.Order_Taxes).ToString();
                                    if (orderTotalString.Contains(Item_Name.Substring(1)))
                                    {
                                        Current_Order_List.Add(order);
                                    }
                                }

                                if (Current_Order_List.Count == 0) Parse_Error = true;
                            }
                            catch
                            {
                                Parse_Error = true;
                            }
                        }
                        else if (Item_Name.StartsWith("$"))
                        {
                            Parse_Error = true;
                        }

                        // Search by item name
                        if (Parse_Error || !Item_Name.StartsWith("$"))
                        { 
                            foreach (Item item in Master_Item_List)
                            {
                                try
                                {
                                    if (Regex.IsMatch(item.Name, Item_Name, RegexOptions.IgnoreCase))
                                    {
                                        List<Order> temp_Order = Order_List.Where(x => x.OrderID == item.OrderID).ToList();
                                        Current_Order_List.AddRange(temp_Order);
                                    }
                                }
                                catch
                                { }
                            }
                        }

                        // Distinct orders only #safeguard (because multiple orders will show up due to overlapping items)
                        Current_Order_List = Current_Order_List.Distinct().ToList();

                        // Sort by date descending
                        Current_Order_List = Current_Order_List.OrderByDescending(x => x.Date).ToList();

                        if (Current_Order_List.Count == 0)
                        {
                            Form_Message_Box FMB = new Form_Message_Box(this, "No results found for item", true, -30, this.Location, this.Size);
                            FMB.ShowDialog();
                        }

                        for (int i = 0; i < Current_Order_List.Count; i++)
                        {
                            Receipt_Report RP = new Receipt_Report(this, Current_Order_List[i], null, null, true, this.Location, this.Size);
                            RP.ShowDialog();

                            // End loop if editing
                            if (Editing_Receipt) i += Current_Order_List.Count + 1;

                            if (i < Current_Order_List.Count - 1)
                            {
                                Grey_Out();
                                using (var form1 = new Yes_No_Dialog(this, "There are " + (Current_Order_List.Count - 1 - i).ToString() + " result(s) left. View the next?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                                {
                                    var result21 = form1.ShowDialog();
                                    if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                                    {
                                    }
                                    else
                                    {
                                        // End loop if not viewing next
                                        i += Current_Order_List.Count + 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            Grey_In();
        }

        private void remove_gc_click(object sender, EventArgs e)
        {
            Pending_GC_Use = new List<GC>();
            GC_Available_Credit = 0;
            GC_Amt = 0;

            foreach (GC GCard in GC_List)
            {
                GCard.Reverse_Transaction(Editing_Order.OrderID);
            }
            Invalidate();
        }

        int origQuantity = 0;
         
        private void dynamic_button_click(object sender, EventArgs e)
        {

            Button b = (Button)sender;
            Expenses Ref_Expense = new Expenses();

            // Remove existing comboboxes
            List_Combos.ForEach(x => this.Controls.Remove(x));
            List_Combos = new List<AdvancedComboBox>();

            if (b.Name.StartsWith("d")) // delete
            {
                if (Item_List.Count == 1 && Editing_Receipt)
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(this, "You are trying to delete the last item in order. Please delete entire order instead", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }
                else
                {
                    // Remove gift cards to retain accuracy for gift numbers
                    if (GC_Available_Credit > 0)
                    {
                        Grey_Out();
                        Form_Message_Box FMB = new Form_Message_Box(this, "Gift card credits have been removed. Please apply them back", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                        Grey_In();
                        Pending_GC_Use = new List<GC>();
                        GC_Available_Credit = 0;
                        GC_Amt = 0;

                        foreach (GC GCard in GC_List)
                        {
                            GCard.Reverse_Transaction(Editing_Order.OrderID);
                        }
                    }

                    if (Editing_Receipt)
                    {
                        // Remove hobby items
                        Item Ref_Item = Item_List[Convert.ToInt32(b.Name.Substring(1))];
                        for (int i = Master_Hobby_Item_List.Count - 1; i >= 0; i--)
                        {
                            if (Master_Hobby_Item_List[i].OrderID == Ref_Item.OrderID && Ref_Item.Name == Master_Hobby_Item_List[i].Name)
                                Master_Hobby_Item_List.RemoveAt(i);
                        }

                        // Remove asset items
                        for (int i = Asset_List.Count - 1; i >= 0; i--)
                        {
                            if (Asset_List[i].OrderID == Ref_Item.OrderID && Ref_Item.Name == Asset_List[i].Name)
                                Asset_List.RemoveAt(i);
                        }
                    }

                    Item_List.RemoveAt(Convert.ToInt32(b.Name.Substring(1)));
                    edit_index = -1;
                    Invalidate();
                    Update();
                    Add_button.Visible = true;
                    label14.Visible = true;
                }
            }
            else if (b.Name.StartsWith("s")) // discount
            {
                Grey_Out();
                Input_Box IB = new Input_Box(this, "Set total discount amount", (Item_List[Convert.ToInt32(b.Name.Substring(1))].Discount_Amt).ToString(), null, this.Location, this.Size);
                IB.ShowDialog();
                Grey_In();
                double Cost_w_tax = (Item_List[Convert.ToInt32(b.Name.Substring(1))].Quantity * Item_List[Convert.ToInt32(b.Name.Substring(1))].Price) * (1 + (Tax_Rules_Dictionary.ContainsKey(Item_List[Convert.ToInt32(b.Name.Substring(1))].Category) ? Convert.ToDouble(Tax_Rules_Dictionary[Item_List[Convert.ToInt32(b.Name.Substring(1))].Category]) : Tax_Rate));
                Item_List[Convert.ToInt32(b.Name.Substring(1))].Discount_Amt = (Discount_Transfer_Amount > Cost_w_tax ? (Cost_w_tax - Discount_Transfer_Amount < 0 ? Cost_w_tax : Cost_w_tax - Discount_Transfer_Amount) : Discount_Transfer_Amount); // If transfer amt is greater than unit price, set discount to unit price
                Discount_Transfer_Amount = 0;
                edit_index = -1;
                Invalidate();
                Update();
                Add_button.Visible = true;
                label14.Visible = true;
            }
            else if (b.Name.StartsWith("r")) // refund alert
            {
                Item_List[Convert.ToInt32(b.Name.Substring(1))].RefundAlert = !Item_List[Convert.ToInt32(b.Name.Substring(1))].RefundAlert;// If transfer amt is greater than unit price, set discount to unit price
                Invalidate();
                Update();
            }
            else if (b.Name.StartsWith("e")) // edit mode on
            {
                editFocusButton = b;

                // Remove gift cards to retain accuracy for gift numbers
                if (GC_Available_Credit > 0)
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(this, "Gift card credits have been removed. Please apply them back", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                    Pending_GC_Use = new List<GC>();
                    GC_Available_Credit = 0;
                    GC_Amt = 0;

                    foreach (GC GCard in GC_List)
                    {
                        GCard.Reverse_Transaction(Editing_Order.OrderID);
                    }
                }

                if (edit_index.ToString() != b.Name.Substring(1)) // Switch edit index to new edit index
                {
                    edit_index = Convert.ToInt32(b.Name.Substring(1));
                    Add_button.Visible = false;
                    label14.Visible = false;
                    Item Ref_Item = Item_List[edit_index];
                    category_box.Text = Ref_Item.Category;
                    item_desc.Text = Ref_Item.Name;
                    item_price.Text = "$" + String.Format("{0:0.00}", Ref_Item.Price);
                    quantity.Text = Ref_Item.Quantity.ToString();

                    origQuantity = Convert.ToInt32(quantity.Text);
                }
                else // Accept changes
                {
                    // Remove hobby & asset items in case quantity changed
                    if (Editing_Receipt && origQuantity != Convert.ToInt32(quantity.Text) && Asset_List.Any(x => x.OrderID == Item_List[Convert.ToInt32(b.Name.Substring(1))].OrderID && x.Name == Item_List[Convert.ToInt32(b.Name.Substring(1))].Name))
                    {
                        Grey_Out();
                        using (var form1 = new Yes_No_Dialog(this, "Item quantity has been changed. Do you wish to remove associated hobby and asset items with respect to this item?", "Warning", "No", "Yes", 25, this.Location, this.Size))
                        {
                            var result21 = form1.ShowDialog();
                            if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                            {
                                // Remove hobby items
                                Item Ref_Item = Item_List[Convert.ToInt32(b.Name.Substring(1))];
                                for (int i = Master_Hobby_Item_List.Count - 1; i >= 0; i--)
                                {
                                    if (Master_Hobby_Item_List[i].OrderID == Ref_Item.OrderID && Ref_Item.Name == Master_Hobby_Item_List[i].Name)
                                        Master_Hobby_Item_List.RemoveAt(i);
                                }

                                // Remove asset items
                                for (int i = Asset_List.Count - 1; i >= 0; i--)
                                {
                                    if (Asset_List[i].OrderID == Ref_Item.OrderID && Ref_Item.Name == Asset_List[i].Name)
                                        Asset_List.RemoveAt(i);
                                }
                            }
                        }
                        Grey_In();
                    }

                    if (item_price.Text.Length > 1 && item_desc.Text.Length > 0)
                    {
                        if (Editing_Receipt)
                        {
                            Item Ref_Item = Item_List[edit_index];
                            for (int i = Master_Hobby_Item_List.Count - 1; i >= 0; i--)
                            {
                                if (Master_Hobby_Item_List[i].OrderID == Ref_Item.OrderID && Ref_Item.Name == Master_Hobby_Item_List[i].Name)
                                {
                                    Master_Hobby_Item_List[i].Name = item_desc.Text;
                                    Master_Hobby_Item_List[i].Category = category_box.Text;
                                    Master_Hobby_Item_List[i].Price = (Convert.ToDouble(item_price.Text.Substring(1)) * (1 + (Tax_Rules_Dictionary.ContainsKey(Ref_Item.Category) ? Convert.ToDouble(Tax_Rules_Dictionary[Ref_Item.Category]) : Tax_Rate))) - (Ref_Item.Discount_Amt / Convert.ToInt32(quantity.Text));
                                }
                            }
                        }

                        Item_List[edit_index].Category = category_box.Text;
                        Item_List[edit_index].Name = item_desc.Text;
                        Item_List[edit_index].Price = Convert.ToDouble(item_price.Text.Substring(1));
                        Item_List[edit_index].Quantity = Convert.ToInt32(quantity.Text);

                        edit_index = -1;
                        Add_button.Visible = true;
                        label14.Visible = true;

                        item_desc.Text = "";
                        item_price.Text = "$";

                        // Change hobby_items with current name
                    }

                }
                Invalidate();
                Update();
            }
        }

        public List<AdvancedComboBox> List_Combos = new List<AdvancedComboBox>();
        
        // Add a memo
        private void memo_button_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Input_Box IB = new Input_Box(this, "Attach a memo to item:", Temp_Memo, null, this.Location, this.Size);
            IB.ShowDialog();
            Grey_In();
        }

        /// <summary>
        /// Check if user has gift card at current location
        /// </summary>
        /// <returns></returns>
        private bool Check_GC()
        {
            foreach (GC GCard in GC_List.Where(x => x.Amount > 0).ToList())
            {
                // if contains same location AND not pending
                if ((location_box.Text.Contains(GCard.Location) || GCard.Location.Contains(location_box.Text)) && GC_Available_Credit == 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void submit_button_Click(object sender, EventArgs e)
        {
            if (Item_List.Count > 0)
            {
                bool Add_GC = false;
                if (Check_GC() && !Editing_Receipt)
                {
                    Grey_Out();
                    using (var form = new Yes_No_Dialog(this, "We've found a gift card for this location. Do you wish to add a gift-card?", "Warning", "No", "Yes", 16, this.Location, this.Size))
                    {
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            if (form.ReturnValue1 == "1")
                            {
                                Grey_Out();
                                Add_GC = true;
                                GC_Manager GCM = new GC_Manager(this, true, this.Location, this.Size);
                                GCM.ShowDialog();
                                Grey_In();
                            }
                        }
                    }
                    Grey_In();
                }
                if (!Add_GC)
                {
                    bool hasCashHistory = false;

                    string Edit_ID = "";

                    // Set all items in item list to current location
                    Item_List.ForEach(x => x.Location = location_box.Text);

                    if (Editing_Receipt)
                    {

                        // delete existing items and order from Master_Item_List and Order_List and proceed with regular editing 

                        for (int i = Master_Item_List.Count - 1; i >= 0; i--)
                        {
                            if (Master_Item_List[i].OrderID == Editing_Order.OrderID)
                            {
                                Master_Item_List.RemoveAt(i);
                            }
                        }

                        // Delete cash history
                        hasCashHistory = Cash.DeleteCashHistryByID("O" + Editing_Order.OrderID);

                        Edit_ID = Editing_Order.OrderID;
                        Order_List.Remove(Editing_Order);

                    }

                    // Set order information to keep track of
                    int Order_Quantity = 0;
                    double Order_Total_Pre_Tax = 0;
                    double Order_Taxes = 0;

                    // Get Random Order ID
                    Random OrderID_Gen = new Random();
                    string randomID = OrderID_Gen.Next(100000000, 999999999).ToString();

                    // Ensure no clashing of IDs
                    List<Item> same_ID = Master_Item_List.Where(x => x.OrderID == randomID).ToList();
                    while (same_ID.Count > 0)
                    {
                        randomID = OrderID_Gen.Next(100000000, 999999999).ToString();
                        same_ID = Master_Item_List.Where(x => x.OrderID == randomID).ToList();
                    }

                    randomID = (Edit_ID.Length > 2) ? Edit_ID : randomID;

                    double total_discount = Item_List.Where(item => item.Discount_Amt > 0).Sum(item => item.Discount_Amt);

                    foreach (Item item in Item_List)
                    {

                        // Adjust for refunded items w/ discount
                        total_discount -= item.Discount_Amt - item.Get_Current_Discount();
                        Order_Total_Pre_Tax -= item.Price * (Convert.ToInt32(item.Status));
                        Order_Taxes -= item.Price * item.Quantity * Get_Tax_Amount(item) - item.Get_Current_Tax_Amount(Get_Tax_Amount(item));
                    }

                    if (Alerts_On && !Editing_Receipt)
                    {
                        // Check for categorical warnings
                        foreach (string category in Item_List.Select(x => x.Category).Distinct())
                        {
                            Check_Warnings(category);
                        }
                    }

                Repeat:
                    // Change temporary shipping to current
                    if (Tracking_Created)
                    {
                        if (Edit_ID.Length == 0)
                        {
                            Tracking_List.FirstOrDefault(x => x.Ref_Order_Number == "999999999").Ref_Order_Number = (Edit_ID.Length == 0 ? randomID : Edit_ID);
                        }
                        else
                        {
                            if (Tracking_List.FirstOrDefault(x => x.Ref_Order_Number == "999999999") != null)
                            {
                                Tracking_List.FirstOrDefault(x => x.Ref_Order_Number == "999999999").Ref_Order_Number = Edit_ID;
                            }
                            else
                            {
                                Tracking_List.FirstOrDefault(x => x.Ref_Order_Number == Edit_ID).Ref_Order_Number = Edit_ID;
                            }
                        }
                    }
                    else
                    {
                        if (Check_Current_Order_Online() && !Editing_Receipt)
                        {
                            Grey_Out();
                            using (var form = new Yes_No_Dialog(this, "We noticed that this order might be an online purchase. Do you wish to setup tracking information and/or expected arrival dates?", "Warning", "No", "Yes", 30, this.Location, this.Size))
                            {
                                var result = form.ShowDialog();
                                if (result == DialogResult.OK)
                                {
                                    if (form.ReturnValue1 == "1")
                                    {
                                        button4.PerformClick();
                                        goto Repeat;
                                    }
                                    else
                                    {
                                    }
                                }
                            }
                            Grey_In();
                        }
                    }

                    // Automatically set payment type to gift card if no excess payment needed
                    if (GC_Available_Credit >= Running_Total_Master) payment_type.Text = "Other";

                    // Take values off gift cards
                    int GC_Index = 0;
                    double GC_Paid = 0;
                    //string GC_Memo = "Gift Cards Used: ";

                    while (GC_Index < Pending_GC_Use.Count && GC_Amt > 0)
                    {
                        double GC_Transfer_Amount = (GC_Amt - GC_Paid > Pending_GC_Use[GC_Index].Amount ? Pending_GC_Use[GC_Index].Amount : GC_Amt - GC_Paid);
                        GC_Paid += GC_Transfer_Amount;
                        //GC_Memo += Pending_GC_Use[GC_Index].Location + " ($" + GC_Transfer_Amount + ");";
                        GC_List.First(x => x == Pending_GC_Use[GC_Index]).Amount -= GC_Transfer_Amount;
                        GC_List.First(x => x == Pending_GC_Use[GC_Index]).Add_Order(randomID, GC_Transfer_Amount.ToString());
                        GC_Index++;
                    }

                    for (int i = Item_List.Count - 1; i >= 0; i--)
                    {
                        // Store order information
                        Order_Quantity += Item_List[i].Quantity;
                        Order_Total_Pre_Tax += Item_List[i].Price * Item_List[i].Quantity;
                        Order_Taxes += (Tax_Rules_Dictionary.ContainsKey(Item_List[i].Category) ? Convert.ToDouble(Tax_Rules_Dictionary[Item_List[i].Category]) * Item_List[i].Price * Item_List[i].Quantity : Tax_Rate * Item_List[i].Price * Item_List[i].Quantity);

                        // Transfer from current to master item list
                        Item_List[i].OrderID = randomID;
                        Item_List[i].Date = dateTimePicker1.Value;
                        Item_List[i].Payment_Type = payment_type.Text;
                        Master_Item_List.Add(Item_List[i]);
                        Item_List.RemoveAt(i);
                    }

                    if (Tax_Exempt_Order) Order_Taxes = 0;
                    else if (Tax_Override_Amt > 0) Order_Taxes = Tax_Override_Amt;

                    Order Current_Order = new Order();
                    Current_Order.Location = location_box.Text;
                    Current_Order.OrderMemo = Order_Memo;// + " " + ((Pending_GC_Use.Count > 0) ? GC_Memo : "");
                    Current_Order.OrderID = randomID;
                    Current_Order.GC_Amount = GC_Amt;
                    
                    double totalAmt = Order_Total_Pre_Tax + (Tax_Exempt_Order ? 0 : (Tax_Override_Amt > 0 ? Tax_Override_Amt : Order_Taxes)) - total_discount - GC_Amt;

                    Current_Order.Tax_Overridden = (Tax_Override_Amt > 0 || Tax_Exempt_Order);
                    Current_Order.Payment_Type = payment_type.Text;
                    Current_Order.Order_Total_Pre_Tax = Order_Total_Pre_Tax - total_discount;
                    Current_Order.Order_Taxes = Order_Taxes;
                    Current_Order.Order_Discount_Amt = total_discount;
                    Current_Order.Order_Quantity = Order_Quantity;
                    Current_Order.Date = dateTimePicker1.Value;

                    // To ensure that old orders with cash do not get appended to current history if viewed and saved 
                    if (Current_Order.Payment_Type == "Cash" && ((Editing_Receipt && hasCashHistory) || !Editing_Receipt))
                    {
                        Cash.AddCashHistory(Current_Order.Date,
                            String.Format("Purchase(s) at {0}", Current_Order.Location), -totalAmt,
                            "O" + Current_Order.OrderID);
                    }

                    Order_List.Add(Current_Order);

                    // Update current expenditure
                    if (dateTimePicker1.Value.Month == DateTime.Now.Month && dateTimePicker1.Value.Year == DateTime.Now.Year) Current_Month_Expenditure += Order_Total_Pre_Tax + Order_Taxes;

                    // Only check budget if new order and alert is on
                    if (Alerts_On && !Editing_Receipt)
                    {
                        // Check over-budget
                        Check_Budget();
                    }

                    dateTimePicker1.Value = DateTime.Now;
                    Tax_Override_Amt = 0;
                    GC_Amt = 0;
                    GC_Available_Credit = 0;
                    Tax_Exempt_Order = false;
                    Order_Memo = "";


                    // Check over payment limit
                    if (Alerts_On)
                    {
                        foreach (Payment payment in Payment_List.Where(x => (x.Company + " (xx-" + x.Last_Four + ")") == payment_type.Text).ToList())
                        {
                            payment.Get_Total(Master_Item_List, Tax_Rules_Dictionary, Tax_Rate, Order_List);
                            string Alert_Message = payment.Check_Alerts();
                            if (Alert_Message.Length > 5)
                            {
                                Grey_Out();
                                Form_Message_Box FMB = new Form_Message_Box(this, Alert_Message, true, 0, this.Location, this.Size);
                                FMB.ShowDialog();
                                Grey_In();
                            }
                        }
                    }

                    repaintButtons = true;
                    Invalidate();
                    Update();

                    if (MOMG_open)
                    {

                        MOMG.Refresh_Window();
                    }

                    if (Settings_Dictionary.ContainsKey("AUTO_SAVE") && Settings_Dictionary["AUTO_SAVE"] == "1")
                    {
                        Background_Save();
                    }

                    quantity.Text = "1";
                    category_box.SelectedIndex = 0;
                    item_desc.Text = "";
                    item_price.Text = "$";
                    Shipping_ID = "";
                    reset_button.Enabled = true;
                    Tracking_Created = false;
                    Pending_GC_Use = new List<GC>();
                    Editing_Receipt = false;
                    Add_button.Visible = true;
                    label14.Visible = true;
                    

                    #region Set status spending info
                    StatusSetSpending(false, true);
                    #endregion
                }
            }
        }

        public void Check_SMS_Alerts()
        {
            foreach (SMSAlert SMSA in SMSAlert_List)
            {
                if (SMSA.IUO_Flag && SMSA.Time.Hour == DateTime.Now.Hour && SMSA.Time.Minute == DateTime.Now.Minute && SMSA.Time.Second == DateTime.Now.Second)
                {

                    Task.Run(() =>
                    {
                        UpdateStatus("Sending SMS...");
                        JSON.SendSMS(SMSA.Name);
                        System.Threading.Thread.Sleep(1500);
                        UpdateStatus("SMS sent!");
                        System.Threading.Thread.Sleep(2500);
                        UpdateStatus("Ready");
                    });

                    Diagnostics.WriteLine("Sending SMS: " + SMSA.Name);

                    // Show notification
                    new Thread(() => new Alert_Box(this, SMSA.Name).ShowDialog()).Start();

                    //Alert_Box AB = new Alert_Box(this, SMSA.Name);
                    //AB.Show();

                    Diagnostics.WriteLine("Showing Notification: " + SMSA.Name);

                    if (!SMSA.Repeat)
                    {
                        SMSA.IUO_Flag = false;
                    }
                }
            }
        }

        public void Edit_Receipt(Order order)
        {
            if (Item_List.Count > 0)
            {
                Grey_Out();
                using (var form = new Yes_No_Dialog(this, "You have un-submitted items. Continuing will reset your current receipt. Do you wish to continue?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        if (form.ReturnValue1 == "1")
                        {
                        }
                        else
                        {
                            goto Finish;
                        }
                    }
                }
                Grey_In();
            }

            // Continue
            Editing_Receipt = true;

            Shipping_ID = order.OrderID;
            Item_List = new List<Item>();
            Item_List = Master_Item_List.Where(x => x.OrderID == order.OrderID).ToList();
            Editing_Order = order;
            GC_Amt = order.GC_Amount;
            edit_index = -1;
            Add_button.Visible = true;
            label14.Visible = true;
            paint = true;
            Tax_Override_Amt = Editing_Order.Tax_Overridden ? Editing_Order.Order_Taxes : 0;
            GC_Amt = Editing_Order.GC_Amount > 0 ? Editing_Order.GC_Amount : 0;
            GC_Available_Credit = GC_Amt;
            dateTimePicker1.Value = Editing_Order.Date;
            location_box.Text = Editing_Order.Location;
            payment_type.Text = Editing_Order.Payment_Type;
            Invalidate();
            Update();

            reset_button.Enabled = false;

            Finish: ; // Do nothing
        }

        /// <summary>
        /// Remove a specific character from string
        /// </summary>
        /// <param name="input"></param>
        /// <param name="Remove_Char"></param>
        /// <returns></returns>
        public string Remove_Character(string input, char Remove_Char)
        {
            int index = 0;
            char[] result = new char[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] != Remove_Char)
                {
                    result[index++] = input[i];
                }
            }
            return new string(result, 0, index);
        }

        private void tax_override_button_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Input_Box IB = new Input_Box(this, "Override existing tax amount to:", Editing_Receipt ? Editing_Order.Tax_Overridden ? Editing_Order.Order_Taxes.ToString() : "0" : Tax_Override_Amt.ToString(), null, this.Location, this.Size);
            IB.ShowDialog();
            Grey_In();
        }

        private void order_memo_button_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Input_Box IB = new Input_Box(this, "Attach a memo to this order:", Editing_Receipt ? Editing_Order.OrderMemo : Temp_Memo, null, this.Location, this.Size);
            IB.ShowDialog();
            Grey_In();
        }

        public string Shipping_ID = "";

        private void reset_button_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form1 = new Yes_No_Dialog(this, "Are you sure you wish to reset receipt?", "Warning", "No", "Yes", 0, this.Location, this.Size))
            {
                var result21 = form1.ShowDialog();
                if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                {
                    Item_List = new List<Item>();
                    Tax_Exempt_Order = false;
                    Order_Memo = "";
                    Shipping_ID = "";
                    Tax_Override_Amt = 0;
                    GC_Amt = 0;
                    GC_Available_Credit = 0;
                    quantity.Text = "1";
                    Editing_Receipt = false;
                    repaintButtons = true;
                    Invalidate();
                    Update();
                    Add_button.Visible = true;
                    label14.Visible = true;
                    Tracking_Created = false;
                    Pending_GC_Use = new List<GC>();

                    // Remove temp
                    Tracking_List = Tracking_List.Where(x => x.Ref_Order_Number != "999999999").ToList();
                }
            }
            Grey_In();
        }

        private List<Button> Delete_Item_Buttons = new List<Button>();
        private List<Button> Edit_Buttons = new List<Button>();
        private List<Button> Discount_Buttons = new List<Button>();

        // Lists and Object Vars
        public Savings_Structure Savings = new Savings_Structure();
        List<Item> Item_List = new List<Item>();
        Month_Over_Month_Graph MOMG;
        FadeControl TFLP;

        public List<Item> Master_Item_List = new List<Item>();
        public List<Expiration_Entry> Expiration_List = new List<Expiration_Entry>();
        public List<Hobby_Item> Master_Hobby_Item_List = new List<Hobby_Item>();
        public Dictionary<string, List<Container>> Master_Container_Dict = new Dictionary<string, List<Container>>();
        public List<string> Hobby_Profile_List = new List<string>();
        public List<Company> Company_List = new List<Company>();
        public List<string> Category_List = new List<string>();
        public List<Order> Order_List = new List<Order>();
        public List<Shipment_Tracking> Tracking_List = new List<Shipment_Tracking>();
        public List<Expenses> Expenses_List = new List<Expenses>();
        public List<Payment> Payment_List = new List<Payment>();
        public List<Asset_Item> Asset_List = new List<Asset_Item>();
        public List<Account> Account_List = new List<Account>();
        public List<GC> GC_List = new List<GC>();
        public List<CustomIncome> Income_Company_List = new List<CustomIncome>();
        public List<Payment_Options> Payment_Options_List = new List<Payment_Options>();
        public List<Calendar_Events> Calendar_Events_List = new List<Calendar_Events>();
        public List<Agenda_Item> Agenda_Item_List = new List<Agenda_Item>();
        public List<GC> Pending_GC_Use = new List<GC>();
        public List<Investment> Investment_List = new List<Investment>();
        public List<Contact> Contact_List = new List<Contact>();
        public List<SMSAlert> SMSAlert_List = new List<SMSAlert>();
        public List<Location> Location_List = new List<Location>();
        public List<string> GeneratedFilePaths_List = new List<string>();
        public List<Association> AssociationList = new List<Association>();
        public List<GroupedCategory> GroupedCategoryList = new List<GroupedCategory>();
        public List<BudgetEntry> BudgetEntryList = new List<BudgetEntry>();

        // Settings Variables
        public Dictionary<string, string> Link_Location = new Dictionary<string, string>(); //link source -> link destination
        public Dictionary<string, string> Tax_Rules_Dictionary = new Dictionary<string, string>();     //category -> tax rate %
        public Dictionary<string, string> Settings_Dictionary = new Dictionary<string, string>();
        public Dictionary<string, Warning> Warnings_Dictionary = new Dictionary<string, Warning>();

        // Temporary / Feedback variables
        public string Temp_Memo = "";
        public Color Frame_Color = SystemColors.HotTrack;
        public string Pass_Through_String = "";
        private bool Editing_Receipt = false;
        private Order Editing_Order = new Order();

        // Reference variables
        private bool paint = false;
        public bool Show_Calendar_On_Load = false;
        public bool Alerts_On = true;
        public bool Savings_Instantiated = false;
        public bool MOMG_open = false;
        public bool Tax_Exempt_Order = false;
        public string Order_Memo = "";
        bool Tracking_Created = false;
        public bool Saving_In_Process = false;
        public double Discount_Transfer_Amount = 0;
        public int edit_index = -1;

        // Preset Variables 
        public double Tax_Rate = 0.13;
        public double Monthly_Income = 0;
        public double Tax_Override_Amt = 0;

        // Gift card values
        public double GC_Amt = 0;
        public double GC_Available_Credit = 0;
        public double Running_Total_Master = 0;

        // Form Sizing Parameters
        Size Start_Size = new Size();

        public Receipt()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            Start_Size = this.Size;

            // Force redraw on resizing (required for borderless form resize
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        string currencyTextBase = ""; // "          "  <--- tab length

        private void mouseClick(object sender, MouseEventArgs e)
        {
            item_price.Text = "$";
        }

        public void Grey_Out()
        {
            TFLP.Location = new Point(1, 1);
        }

        public void Grey_In()
        {
            TFLP.Location = new Point(1000, 1000);
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Create root app data folder
            if (!Directory.Exists(localSavePath))
            {
                Directory.CreateDirectory(localSavePath);
            }

            #region Fade Box
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

            TFLP.Opacity = 75;
            #endregion

            #region Variables and control initializers
            SaveHelper = new Save_Functionalities(this);
            LoadHelper = new Load_Functionalities(this);
            MobileSync = new MobileSync(this);

            for (int i = 1; i <= 20; i++) quantity.Items.Add(i.ToString());
            quantity.Text = "1";

            // Memo Tooltip (Hover)
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            ToolTip1.SetToolTip(memo_button, "Add a memo");
            ToolTip1.SetToolTip(Add_button, "Add item to receipt");
            ToolTip1.SetToolTip(button1, "Add a new location");
            ToolTip1.SetToolTip(button2, "Add a new category");
            ToolTip1.SetToolTip(button3, "Change Receipt Date");
            ToolTip1.SetToolTip(button5, "Use gift credit");

            ToolTip1.SetToolTip(reset_button, "Clear Receipt");
            ToolTip1.SetToolTip(order_memo_button, "Add a memo to order");
            ToolTip1.SetToolTip(tax_override_button, "Override existing tax amount");
            ToolTip1.SetToolTip(submit_button, "Submit Receipt");

            // Render menustrip
            menuStrip1.ForeColor = Color.LightGray;

            menuStrip1.Renderer = new ToolStripProfessionalRenderer(new TestColorTable() { Menu_Border_Color = Frame_Color });

            dateTimePicker1.Value = DateTime.Now;
            //menuStrip1.Renderer = new MyRenderer();

            close_button.FlatAppearance.BorderSize = 0;
            minimize_button.FlatAppearance.BorderSize = 0;

            this.TopMost = true;
            this.TopMost = false;

            #endregion

            // Load user information
            Reload_Program();

            // Get current expenditure
            Get_Current_Month_Expenditure();

            #region Clear Backups
            if (Settings_Dictionary.ContainsKey("BACKUP_DEL") && Settings_Dictionary["BACKUP_DEL"] == "1")
            {
                Clear_Backups(14);
            }
            #endregion

            #region Backup if last login is past 5 minutes
            if (Settings_Dictionary.ContainsKey("BACKUP_REQ") && Settings_Dictionary["BACKUP_REQ"] == "1")
            {
                // Current backup functionality (only if last save is 5 minutes ago)
                if (Settings_Dictionary.ContainsKey("LAST_LOGIN") && Convert.ToDateTime(Settings_Dictionary["LAST_LOGIN"]).AddMinutes(5) < DateTime.Now)
                {
                    Diagnostics.WriteLine("Backing up data");
                    Task.Run(() =>
                    {
                        SaveHelper.Save_Backup();
                    });
                }
            }
            #endregion

            #region Create savings if not started
            if (!Savings_Instantiated)
            {
                Savings.Structure = "";
                Savings.Ref_Value = 0;
                Savings.Alert_1 = false;
            }
            #endregion

            #region Get Currency Info & QOTD (Quote of the day)
            Task.Run(() =>
            {
                JSON.PopulateCurrencyDict();

                foreach (KeyValuePair<string, double> currencyValue in JSON.GetCurrencyValues())
                {
                    currencyTextBase += String.Format("{0}: {1}    ", currencyValue.Key,
                        String.Format("${0:0.00}", currencyValue.Value));
                }

                RefreshMarqueeText();
            });
            #endregion

            #region Set status bar temperature (load form and weather in background)
            DateTime Ref_Date = DateTime.Now;

            weatherLabel.Text = "Getting weather data...";
            Task.Run(() =>
            {
                try
                {
                    string API_Str = JSON.Get_Weather_String(Ref_Date);
                    double WeatherLow = JSON.FarToCel(Convert.ToDouble(JSON.ParseWeatherParameter(API_Str, "temperatureMin")));
                    double WeatherHigh = JSON.FarToCel(Convert.ToDouble(JSON.ParseWeatherParameter(API_Str, "temperatureMax")));
                    weatherLabelStringTemperature = "Low: " + WeatherLow + "°C" + "   High: " + WeatherHigh + "°C";
                    weatherLabelString = weatherLabelStringTemperature;
                    weatherLabelStringCondition = "Condition: " + JSON.Get_Weather_String(JSON.ParseWeatherParameter(API_Str, "icon"));
                    SetWeatherInfo(API_Str); //cross-thread function
                    weatherIcon.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch
                {
                    //weatherPanel.Controls.Remove(weatherIcon);
                    weatherLabelString = "Error Collecting Weather Info...";
                }
            });
            #endregion

            if (!isTesting)
            {
                #region Authentication
                try
                {
                    if (Settings_Dictionary.ContainsKey("LOGIN_EMAIL") && Settings_Dictionary["LOGIN_EMAIL"].Contains("lirobin9@gmail.com"))
                    {
                        toolStripMenuItem4.Visible = true;
                        toolStripMenuItem16.Visible = true;
                    }

                    // If authentication set already
                    if (Settings_Dictionary.ContainsKey("AUTHENTICATION_REQ") && Settings_Dictionary["AUTHENTICATION_REQ"] == "1")
                    {
                        if (Settings_Dictionary["LOGIN_BYPASS"] == "0" || Convert.ToDateTime(Settings_Dictionary["LAST_LOGIN"]).AddMinutes(5) < DateTime.Now)
                        {
                            Grey_Out();
                            using (var form = new Authentication_Form(this, "Please login using your personal email and password.", "Authentication", Settings_Dictionary["LOGIN_EMAIL"], Settings_Dictionary["LOGIN_PASSWORD"], true, true, this.Location, this.Size))
                            {
                                var result2 = form.ShowDialog();
                                if (result2 == DialogResult.OK)
                                {
                                    if (form.ReturnValue1 == "1")
                                    {
                                        this.Show();
                                        this.Activate();
                                        this.TopMost = true;
                                        this.TopMost = false;
                                        //this.TopMost = true;
                                    }
                                    else
                                    {
                                        Environment.Exit(0);
                                    }
                                }
                                else
                                {
                                    Environment.Exit(0);
                                }
                            }
                            Grey_In();
                        }
                    }
                }
                catch
                {
                    // Authentication error, probably empty file
                }
                #endregion
            }

            #region Initialize Shutdown timer
            CheckIdleTimer = new System.Windows.Forms.Timer();
            CheckIdleTimer.Interval = 1000;
            CheckIdleTimer.Tick += new EventHandler(CheckIdleTimer_Tick);
            CheckIdleTimer.Start();
            #endregion

            #region Cloud Load
            // check if cloud load
            if (Settings_Dictionary.ContainsKey("CLOUD_LOAD") && Settings_Dictionary["CLOUD_LOAD"] == "1")
            {
                //ftpWorkerWorking = true;
                if (!(Load_From_Cloud(false))) //if FTP fails at any point, load pre-saved
                {
                    Reload_Program();

                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(this, "Unable to retrieve profile from cloud. Local account has been loaded instead", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();

                }
                //ftpWorkerWorking = false;
            }
            #endregion

            #region FTP logging
            if (((Settings_Dictionary.ContainsKey("AUTHENTICATION_REQ") && (Settings_Dictionary["AUTHENTICATION_REQ"] == "1")) &&
                 (Settings_Dictionary["LOGIN_BYPASS"] == "1" && Convert.ToDateTime(Settings_Dictionary["LAST_LOGIN"]).AddMinutes(5) > DateTime.Now)) && FTP_Logging)
            {
                Task.Run(() => { FTP_Log("PRE-LOADED AUTHENTICATION"); });
            }
            else if (Settings_Dictionary.ContainsKey("AUTHENTICATION_REQ") && !(Settings_Dictionary["AUTHENTICATION_REQ"] == "1") && FTP_Logging)
            {
                Task.Run(() => { FTP_Log("NO-AUTHENTICATION REQUIRED"); });
            }

            #endregion

            #region Initialize Fade timer
            FadeTimer = new System.Windows.Forms.Timer();
            FadeTimer.Interval = 5;
            FadeTimer.Tick += new EventHandler(FadeTimer_Tick);

            FadeTimerWeather = new System.Windows.Forms.Timer();
            FadeTimerWeather.Interval = 5;
            FadeTimerWeather.Tick += new EventHandler(FadeTimerWeather_Tick);
            #endregion

            #region Initialize marquee timer
            MarqueeTimer = new System.Windows.Forms.Timer();
            MarqueeTimer.Interval = 5;
            MarqueeTimer.Tick += new EventHandler(MarqueeTimer_Tick);

            #endregion

            #region Check Alerts and Automate features

            if (Settings_Dictionary.ContainsKey("LAST_LOGIN") && Convert.ToDateTime(Settings_Dictionary["LAST_LOGIN"]).Date != DateTime.Now.Date)
            {
                // Check calendar alerts
                Check_Calendar_Alerts();

                // Check payment alerts
                Check_Payment_Alerts();

                // Check outstanding accounts;
                Check_Outstanding_ARP();
            }

            // Automatically pay expenses
            Pay_Expenses();

            // Run the current form in testing mode
            Run_Test_Form();

            // Check tracking information
            Check_Tracking_Alerts();
            #endregion

            #region Sneak Peak
            if (Settings_Dictionary.ContainsKey("SNEAK_PEAK") && Settings_Dictionary["SNEAK_PEAK"] == "1" && Convert.ToDateTime(Settings_Dictionary["LAST_LOGIN"]).Date != DateTime.Now.Date)
            {
                Grey_Out();
                Sneak_Peak SP = new Sneak_Peak(this, this.Location, this.Size);
                SP.ShowDialog();
                Grey_In();
            }
            #endregion

            #region Notification popup & email notification (assimilated)
            if (Alert_String.Length > 0)
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(this, Alert_String, true, Alert_String.Count(x => x.Equals('\n')) * 35, new Point(this.Location.X, this.Location.Y), this.Size);
                FMB.ShowDialog();
                Grey_In();

                // Send to email if active
                if (Settings_Dictionary["CALENDAR_EMAIL_SYNC"] == "1")
                {
                    // Check if email is set. If not, prompt setup
                    if (Settings_Dictionary["PERSONAL_EMAIL"].Length < 5)
                    {
                        Grey_Out();
                        FMB = new Form_Message_Box(this, "Error: No emails have been set. Please setup an email", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                        Personal_Information PI = new Personal_Information(this);
                        PI.ShowDialog();
                        Grey_In();
                    }
                    else
                    {
                        try
                        {
                            MailMessage mailmsg = new MailMessage();
                            MailAddress from = new MailAddress("automatedpersonalbanker@gmail.com");
                            mailmsg.From = from;
                            mailmsg.To.Add(Settings_Dictionary["PERSONAL_EMAIL"]);
                            mailmsg.Subject = "Personal Banker Calendar Notification";
                            mailmsg.Body = "This is an automated message. Please do not reply to this email. " + Environment.NewLine + Environment.NewLine +

                                           Alert_String;

                            mailmsg.Body += "" + Environment.NewLine;

                            // smtp client
                            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                            client.EnableSsl = true;
                            NetworkCredential credential = new NetworkCredential("automatedpersonalbanker@gmail.com", "R5o2b6i8R5o2b6i8");
                            client.Credentials = credential;
                            //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
                            Task.Run(() => { client.Send(mailmsg); });
                        }
                        catch
                        {
                            Grey_Out();
                            FMB = new Form_Message_Box(this, "Error: Email is invalid", true, 0, this.Location, this.Size);
                            FMB.ShowDialog();
                            Grey_In();
                        }
                    }
                }
            }
            #endregion

            #region Set last login to now

            // Only set time if auto save (since background save is for auto)
            if (Settings_Dictionary.ContainsKey("AUTO_SAVE") && Settings_Dictionary["AUTO_SAVE"] == "1")
            {
                // Set last login to now
                Settings_Dictionary["LAST_LOGIN"] = DateTime.Now.ToString();
            }

            #endregion

            #region Populate Autocomplete
            //SuggestStrings will have the logic to return array of strings either from cache/db
            string[] arr = SuggestStrings("");
            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
            collection.AddRange(arr);
            item_desc.AutoCompleteCustomSource = collection;
            item_desc.AutoCompleteSource = AutoCompleteSource.CustomSource;
            #endregion

            // Save after successful login
            // Background_Save(true, true, false);
            UpdateStatus("Ready");

            #region Set status bar date
            dateLabel.Text = Ref_Date.ToString("ddd") + ", " + mfi.GetMonthName(Ref_Date.Month) + " " + Ref_Date.Day + ", " + Ref_Date.Year;
            #endregion

            #region Set status spending info
            StatusSetSpending();
            #endregion

            #region Check syncable mobile orders
            if (Settings_Dictionary.ContainsKey("MOBILE_SYNC_ON_LOAD") && Settings_Dictionary["MOBILE_SYNC_ON_LOAD"] == "1")
            {
                bool hasSyncFile = MobileSync.CheckForSyncFiles();

                if (hasSyncFile && MobileSync.SyncedOrders.Count > 0)
                {
                    Grey_Out();
                    MobileSyncDialog MSD = new MobileSyncDialog(this, MobileSync.SyncedOrders, MobileSync.SyncedItems,
                        this.Location, this.Size);
                    MSD.ShowDialog();
                    Grey_In();
                }
            }
            #endregion

            if (Settings_Dictionary.ContainsKey("START_MINIMIZED") && Settings_Dictionary["START_MINIMIZED"] == "1")
            {
                //this.WindowState = FormWindowState.Minimized;

                if (FormWindowState.Minimized == WindowState)
                {
                    ShowInTaskbar = false;
                    //notifyIcon1.Visible = true;
                    //notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                    //notifyIcon1.BalloonTipText = "your text";
                    //notifyIcon1.BalloonTipTitle = "Welcome Message";
                    //notifyIcon1.ShowBalloonTip(500);
                    //this.Hide();

                }
            }

            #region Start calendar
            if (Show_Calendar_On_Load && Settings_Dictionary.ContainsKey("START_MINIMIZED") && Settings_Dictionary["START_MINIMIZED"] != "1")
            {
                Calendar c = new Calendar(this);
                c.ShowDialog();
            }
            #endregion

            #region Handlers
            this.KeyPress += Receipt_KeyPress;

            Resize += frmMain_Resize;

            notifyIcon1.BalloonTipClosed += notifyIcon1_BalloonTipClosed;

            // Enter to text tab
            quantity.KeyPress += comboBox_KeyPress;

            // Mousedown anywhere to drag
            MouseDown += Form_MouseDown;

            // Cleaer box on click
            item_price.MouseClick += mouseClick;


            // Add Enter-key auto add
            item_desc.KeyPress += textboxEnterKey_KeyPress;
            item_price.KeyPress += textboxEnterKey_KeyPress;
            item_desc.LostFocus += item_desc_LostFocus;
            #endregion

            // Get Payment Totals
            Payment_List.ForEach(x => x.Get_Total(Master_Item_List, Tax_Rules_Dictionary, Tax_Rate, Order_List));

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        /// <summary>
        /// Refresh text (target label sets target, 0 = both, 1 = 1, 2 = 2)
        /// </summary>
        /// <param name="targetLabel"></param>
        private void RefreshMarqueeText(int targetLabel = 0)
        {
            SetMarqueeLabel(currencyTextBase + "        " + JSON.GetQuotesAPI(), targetLabel);
        }

        void item_desc_LostFocus(object sender, EventArgs e)
        {
            SetAutoCategory();
        }

        private void SetAutoCategory()
        {
            if (item_desc.Text.Length > 0 && Settings_Dictionary.ContainsKey("OE_AUTO_POPULATE") && Settings_Dictionary["OE_AUTO_POPULATE"] == "1")
            {
                Item item = Master_Item_List.FirstOrDefault(x => x.Name.ToLower().Contains(item_desc.Text.ToLower()));
                if (item != null && category_box.Items.Contains(item.Category))
                {
                    category_box.Text = item.Category;
                }
            }
        }

        void Receipt_KeyPress(object sender, KeyPressEventArgs e)
        {
            return;
        }

        private void notifyIcon1_BalloonTipClosed(object sender, EventArgs e)
        {
            var thisIcon = (NotifyIcon)sender;
            thisIcon.Visible = false;
            thisIcon.Dispose();
        }

        double monthlyAmt = 0;
        double weeklyAmt = 0;

        public void StatusSetSpending(bool re_calculate = true, bool update = false)
        {
            // Implementation of calculation. Pulls saved records instead of recalculating
            if (re_calculate || update)
            {
                // update spending 
                Get_Current_Month_Expenditure();

                // Get monthly spending 
                monthlyAmt = 0;
                List<Order> Current_Order_List = Order_List.Where(p => p.Date.Month == DateTime.Now.Month && p.Date.Year == DateTime.Now.Year).ToList();

                foreach (Order order in Current_Order_List)
                {
                    monthlyAmt += order.Order_Taxes + order.Order_Total_Pre_Tax;
                }

                // Get weekly spending
                weeklyAmt = 0;
                DateTime startOfWeek = DateTime.Today.AddDays(-1 * (int)(DateTime.Today.DayOfWeek)); // get start of date
                Current_Order_List = Order_List.Where(x => x.Date >= startOfWeek).ToList();

                foreach (Order order in Current_Order_List)
                {
                    weeklyAmt += order.Order_Taxes + order.Order_Total_Pre_Tax;
                }
            }

            if (!re_calculate || update)
            {
                spendingLabelString = "Weekly: $" + String.Format("{0:0.00}", weeklyAmt) + "   Monthly: $" + String.Format("{0:0.00}", monthlyAmt);
            }
            else
            {
                spendingLabel.Text = "Weekly: $" + String.Format("{0:0.00}", weeklyAmt) + "   Monthly: $" + String.Format("{0:0.00}", monthlyAmt);
            }
            spendingLabel.TextAlign = ContentAlignment.MiddleCenter;
        }

        // Store weather variables to prevent API GET REQUESTS again
        string weatherLabelString;
        string weatherLabelStringTemperature;
        string weatherLabelStringCondition;

        bool ftpWorkerWorking = false;

        // This method is executed on the worker thread and makes  
        // a thread-safe call on the TextBox control.  
        public void SetWeatherInfo(string _1)
        {
            // Cross thread activation
            this.InvokeEx(f_temp => f_temp.weatherIcon.Image = JSON.Get_Weather_Icon(JSON.ParseWeatherParameter(_1, "icon")));
            this.InvokeEx(f_temp => f_temp.weatherLabel.Text = weatherLabelString);
            this.InvokeEx(f_temp => f_temp.weatherLabel.TextAlign = ContentAlignment.TopCenter);
            weatherInfoLoaded = true;
        }

        // This method is executed on the worker thread and makes  
        // a thread-safe call on the TextBox control.  
        public void SetMarqueeLabel(string _1, int labelTarget = 0)
        {
            // Cross thread activation
            if (labelTarget != 2) this.InvokeEx(f_temp => marqueeLabel.Text = _1 + "          ");
            if (labelTarget != 1) this.InvokeEx(f_temp => marqueeLabel2.Text = _1 + "          ");

            if (labelTarget != 2 && marqueeLabel.Width > Width)
                endBoundary = 0 - (marqueeLabel.Width - Width);
            else if (labelTarget != 1 && marqueeLabel2.Width > Width)
                endBoundary = 0 - (marqueeLabel2.Width - Width);
            else
                endBoundary = 0;

            startBoundary = Width;

            if (marqueeInitial.Visible)
            {
                this.InvokeEx(f_temp => marqueeInitial.Visible = false);
                this.InvokeEx(f_temp => MarqueeTimer.Start());
            }
        }

        // Date culture
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        private string[] SuggestStrings(string text)
        {
            return Master_Item_List.Select(x => x.Name).Distinct().ToArray();
        }

        public string Alert_String = "";

        public void FTP_Log(string Login_Style = "PASSWORD AUTHENTICATION")
        {
            Diagnostics.WriteLine("FTP Thread start at " + DateTime.Now.TimeOfDay);

            TimeSpan Start_Time = DateTime.Now.TimeOfDay;
            // Login log to FTP server
            try
            {
                string ftpUsername = "Guest";
                string ftpPassword = "robinisthebest";

                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(ftpUsername, ftpPassword);

                    string ftpPath = @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/login.log";
                        
                    bool Download_Fail = false;

                    // try to download log from client
                    try
                    {
                        File.Delete(Path.Combine(localSavePath, "login.log"));
                        client.DownloadFile(ftpPath, Path.Combine(localSavePath, "login.log"));
                    }
                    catch
                    {
                        /* INITIAL FILE CREATION SHOULD NEVER GO HERE
                        using (var myFile = File.Create(Path.Combine(localSavePath, "login.log")))
                        {
                            myFile.Close();
                        }
                        */
                        // Download file fail so we don't want to create a new file and overwrite FTP log
                        Download_Fail = true;
                    }

                    #region File size query
                    if (!Download_Fail)
                    {

                        string Backup_Path = localSavePath;
                        double Total_File_Size = 0;
                        bool Use_MB = false;

                        if (Directory.Exists(Backup_Path))
                        {
                            string[] File_in_dir;
                            File_in_dir = Directory.GetFiles(Backup_Path + "\\", "*", SearchOption.AllDirectories);
                            File_in_dir = File_in_dir.Where(x => !x.Contains("Backup")).ToArray();
                            foreach (string file in File_in_dir)
                            {
                                Total_File_Size += new FileInfo(file).Length;
                            }

                            // Convert to KB
                            Total_File_Size = Total_File_Size / 1000;


                            if (Total_File_Size > 1000)
                            {
                                Use_MB = true;
                                Total_File_Size = Total_File_Size / 1000;
                            }
                        }
                        else
                        {
                            Total_File_Size = 0;
                        }

                        #endregion

                        string Login_Credentials = Settings_Dictionary["LOGIN_EMAIL"];

                        if (Settings_Dictionary["AUTHENTICATION_REQ"] == "0") Login_Credentials = Environment.UserName + " (" + Environment.MachineName + ")";

                        string log_text = Login_Credentials +
                                            "__Login time: " + DateTime.Now.ToString() +
                                            "__File size: " + String.Format("{0:n}", Total_File_Size) + " " + (Use_MB ? "MB" : "KB") +
                                            "__FTP Login Time: = " + (DateTime.Now.TimeOfDay - Start_Time).TotalMilliseconds + "ms" +
                                            "__" + Login_Style +
                                            Environment.NewLine;


                        // write to log
                        File.AppendAllText(Path.Combine(localSavePath, "login.log"), log_text);

                        client.UploadFile(ftpPath, "STOR", Path.Combine(localSavePath, "login.log"));

                        Thread.Sleep(1000);

                        try
                        {
                            File.Delete(Path.Combine(localSavePath, "login.log"));
                        }
                        catch
                        {
                            Diagnostics.WriteLine("Error Deleting Log File");
                        }
                    }
                }
            }
            catch (Exception ez)
            {
                Diagnostics.WriteLine("FTP ERROR : " + ez.ToString());
                // FTP Error
            }

            Diagnostics.WriteLine("FTP Thread end at " + DateTime.Now.TimeOfDay);
        }

        public void Pay_Expenses()
        {
            foreach (Expenses e in Expenses_List)
            {
                if (e.AutoDebit == "1")
                {
                    Payment temp = Payment_List.First(x => x.Company == e.Payment_Company && x.Last_Four == e.Payment_Last_Four);
                    e.Process_Payments(this, ref temp);
                }
                else if (Settings_Dictionary.ContainsKey("EXPENSE_ALERT") && Settings_Dictionary["EXPENSE_ALERT"] == "1" && e.AutoDebit != "1")
                {
                    if (e.Check_Expenses(this))
                    {
                        Append_Alert_Str("Your recurring expense " + e.Expense_Name + " to " + e.Expense_Payee + " is due in " + Math.Round((e.Next_Pay_Date_IUO - DateTime.Now).TotalDays) + " day(s) on " + e.Expense_Start_Date.ToShortDateString() + " for amount $" + e.Expense_Amount);
                    }
                }
            }
        }

        public void Set_Payment_Box()
        {
            payment_type.Items.Clear();
            foreach (Payment PT in Payment_List)
            {
                payment_type.Items.Add(PT.Company + " (xx-" + PT.Last_Four + ")");
            }

            // Preset Payment Types
            payment_type.Items.Add("Cash");
            payment_type.Items.Add("Other");
            payment_type.Text = payment_type.Items[0].ToString();
        }

        public bool Load_Error = false;

        public bool Reload_Program(string load_path = "", bool newProfile = false, bool skipLoadFile = false)
        {
            if (!skipLoadFile)
            {
                Reset_Parameters();
            }

            // If not new, load from old
            if (!skipLoadFile && !newProfile)
            {
                if (!(Load_Information(load_path))) return false;
            }

            location_box.Items.Clear();
            category_box.Items.Clear();

            Set_Payment_Box();

            payment_type.Text = payment_type.Items[0].ToString();

            // Load Combobox texts
            foreach (Company p in Company_List)
            {
                location_box.Items.Add(p.Name);
            }
            location_box.Sorted = true;

            // Load Combobox texts
            foreach (string p in Category_List)
            {
                category_box.Items.Add(p);
            }
            category_box.Sorted = true;

            // Preset text for combobox
            if (category_box.Items.Count > 0) category_box.Text = category_box.Items[0].ToString();
            if (location_box.Items.Count > 0) location_box.Text = location_box.Items[0].ToString();

            Invalidate();
            Update();

            if (Order_List.Count > 0)
            {
                editTest1ToolStripMenuItem.Enabled = true;
                toolStripMenuItem36.Enabled = true;
                toolStripMenuItem12.Enabled = true;
                toolStripMenuItem19.Enabled = true;
                toolStripMenuItem35.Enabled = true;
                toolStripMenuItem18.Enabled = true;
                toolStripMenuItem28.Enabled = true;
                toolStripMenuItem33.Enabled = true;
                toolStripMenuItem32.Enabled = true;
                toolStripMenuItem11.Enabled = true;
                toolStripMenuItem16.Enabled = true;
            }

            Add_button.Visible = true;
            label14.Visible = true;
            return true;
        }

        public void Reset_Parameters()
        {
            Item_List = new List<Item>();
            Expiration_List = new List<Expiration_Entry>();
            Master_Item_List = new List<Item>();
            Order_List = new List<Order>();
            Company_List = new List<Company>();
            Expenses_List = new List<Expenses>();
            Location_List = new List<Location>();
            Category_List = new List<string>();
            Account_List = new List<Account>();
            GC_List = new List<GC>();
            Income_Company_List = new List<CustomIncome>();
            Pending_GC_Use = new List<GC>();
            Link_Location = new Dictionary<string, string>();
            Tax_Rules_Dictionary = new Dictionary<string, string>();
            Settings_Dictionary = new Dictionary<string, string>();
            Warnings_Dictionary = new Dictionary<string, Warning>();
            Asset_List = new List<Asset_Item>();
            Payment_List = new List<Payment>();
            Payment_Options_List = new List<Payment_Options>();
            Contact_List = new List<Contact>();
            Calendar_Events_List = new List<Calendar_Events>();
            Master_Container_Dict = new Dictionary<string, List<Container>>();
            Tracking_List = new List<Shipment_Tracking>();
            Master_Hobby_Item_List = new List<Hobby_Item>();
            Hobby_Profile_List = new List<string>();
            Agenda_Item_List = new List<Agenda_Item>();
            Investment_List = new List<Investment>();
            SMSAlert_List = new List<SMSAlert>();
            Tier_Format = new List<int>();
            AssociationList = new List<Association>();
            GroupedCategoryList = new List<GroupedCategory>();
            BudgetEntryList = new List<BudgetEntry>();
            Cash.ClearCashHistory();

            Temp_Memo = "";
            Tax_Override_Amt = 0;
            GC_Amt = 0;
            GC_Available_Credit = 0;

            // Order information
            Shipping_ID = "";
            reset_button.Enabled = true;
            Tracking_Created = false;
            Pending_GC_Use = new List<GC>();
            Editing_Receipt = false;
            Add_button.Visible = true;
            label14.Visible = true;
        }

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            if (Saving_In_Process && spendingLabel.Text.Length > 4) // text > 4  means if form is completed loading, allow close
            {
                Grey_Out();
                // Get monthly income and compare
                Form_Message_Box FMB = new Form_Message_Box(this, "Application not ready. Please wait a few seconds", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
            else
            {
                base.Close();
            }
        }
        
        double Current_Month_Expenditure = 0;

        private void Get_Current_Month_Expenditure()
        {
            DateTime Ref_Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            Current_Month_Expenditure = (Order_List.Where(x => x.Date.Month == DateTime.Now.Month && x.Date.Year == DateTime.Now.Year).ToList()).Sum(x => x.Order_Total_Pre_Tax + x.Order_Taxes);
            Current_Month_Expenditure += (Expenses_List.Where(x => x.Expense_Status != "0").ToList()).Sum(x => x.Get_Total_Paid(Ref_Date, Ref_Date.AddMonths(1).AddDays(0)));
        }

        // If press enter on length box, activate add (nmemonics)
        private void textboxEnterKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox g = (TextBox)sender;

            if (e.KeyChar == (char)Keys.Enter && g.Text.Length > 0 && edit_index >= 0)
            {
                Edit_Buttons[edit_index].PerformClick();
            }
            if (e.KeyChar == (char)Keys.Enter && g.Text.Length > 0)
            {
                if (g.Name.Contains("desc"))
                {
                    item_price.Focus();
                }
                else
                {
                    SetAutoCategory();
                    Add_button.PerformClick();
                }
            }
        }

        // If press enter on length box, activate add (nmemonics)
        private void comboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            AdvancedComboBox g = (AdvancedComboBox)sender;

            if (e.KeyChar == (char)Keys.Enter && g.Text.Length > 0 && edit_index >= 0)
            {
                Edit_Buttons[edit_index].PerformClick();
            }
            else if (e.KeyChar == (char)Keys.Enter && g.Text.Length > 0)
            {
                Add_button.PerformClick();
                MouseInput.ScrollWheel(-1);
            }
        }

        // Money text only
        public void textBox6_TextChanged(object sender, EventArgs e)
        {
            TextBox Ref_Box = (TextBox)sender;

            if (!(Ref_Box.Text.StartsWith("$")))
            {
                if (Get_Char_Count(Ref_Box.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = Ref_Box.Text;
                    Ref_Box.Text = temp.Substring(1) + temp[0];
                    Ref_Box.SelectionStart = Ref_Box.Text.Length;
                    Ref_Box.SelectionLength = 0;
                }
                else
                {
                    Ref_Box.Text = "$" + Ref_Box.Text;
                }
            }
            else if ((Ref_Box.Text.Length > 1) && ((Get_Char_Count(Ref_Box.Text, Convert.ToChar(".")) > 1) || (Ref_Box.Text[1].ToString() == ".") || (Get_Char_Count(Ref_Box.Text, Convert.ToChar("$")) > 1) || (!((Ref_Box.Text.Substring(Ref_Box.Text.Length - 1).All(char.IsDigit))) && !(Ref_Box.Text[Ref_Box.Text.Length - 1].ToString() == "."))))
            {
                Ref_Box.TextChanged -= new System.EventHandler(textBox6_TextChanged);
                Ref_Box.Text = Ref_Box.Text.Substring(0, Ref_Box.Text.Length - 1);
                Ref_Box.SelectionStart = Ref_Box.Text.Length;
                Ref_Box.SelectionLength = 0;
                Ref_Box.TextChanged += new System.EventHandler(textBox6_TextChanged);
            }
        }

        // Return the token count within string given token
        public int Get_Char_Count(string comparison_text, char reference_char)
        {
            int count = 0;
            foreach (char c in comparison_text)
            {
                if (c == reference_char)
                {
                    count++;
                }
            }
            return count;
        }

        // Add Item Button
        private void start_button_Click(object sender, EventArgs e)
        {
            if (category_box.Text.Length > 0 && item_desc.Text.Length > 0 && location_box.Text.Length > 0 && item_price.Text.Length > 1)
            {
                QL_Height_Factor = 0;

                // Check if an item with same name exists in current list; if so, add current quantity to it.
                Item temp_Item = Item_List.FirstOrDefault(x => x.Name == item_desc.Text && x.Price.ToString() == item_price.Text.Substring(1));

                if (temp_Item != null)
                {
                    temp_Item.Quantity += Convert.ToInt32(quantity.Text);
                }
                else
                {
                    Item New_Item = new Item();
                    New_Item.Category = category_box.Text;
                    New_Item.Name = item_desc.Text;
                    New_Item.Status = "0";
                    New_Item.Payment_Type = payment_type.Text;
                    New_Item.Location = location_box.Text;
                    New_Item.Price = Convert.ToDouble(item_price.Text.Substring(1));
                    New_Item.Quantity = Convert.ToInt32(quantity.Text);
                    New_Item.Date = DateTime.Now;
                    New_Item.Refund_Date = DateTime.Now;
                    New_Item.Memo = Temp_Memo;
                    Item_List.Add(New_Item);
                }

                item_desc.Text = "";
                item_price.Text = "";
                quantity.Text = "1";
                Temp_Memo = "";

                item_desc.Focus();

                paint = true;
                Invalidate();
                Update();

                edit_index = -1;

            }

            // Update expenditure when first inserting item (improves performance)
            if (Item_List.Count == 1) Get_Current_Month_Expenditure();

        }

        public void Set_Form_Color(Color chooseColor)
        {
            Frame_Color = chooseColor; label5.ForeColor = Color.Silver; 
            menuStrip1.Renderer = new ToolStripProfessionalRenderer(new TestColorTable() { Menu_Border_Color = chooseColor });
            //minimize_button.ForeColor = chooseColor;
            //close_button.ForeColor = chooseColor;
            textBox1.BackColor = chooseColor;
            textBox2.BackColor = chooseColor;
            textBox3.BackColor = chooseColor;
            textBox4.BackColor = chooseColor;
            location_box.FrameColor = chooseColor;
            category_box.FrameColor = chooseColor;
            payment_type.FrameColor = chooseColor;
        }

        // Add a new location
        private void button1_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Input_Box IB = new Input_Box(this, "Add new location:", "", null, this.Location, this.Size);
            IB.ShowDialog();
            Grey_In();
        }

        // Add a new category
        private void button2_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Input_Box IB = new Input_Box(this, "Add new category:", "", null, this.Location, this.Size);
            IB.ShowDialog();
            Grey_In();
        }

        public void Check_Warnings(string Category)
        {
            if (Warnings_Dictionary.ContainsKey(Category))
            {
                // Get total spent
                List<Item> temp = Master_Item_List.Where(x => x.Category == Category && x.Date.Month == DateTime.Now.Month && x.Date.Year == DateTime.Now.Year).ToList();
                temp.AddRange(Item_List.Where(x => x.Category == Category).ToList());
                double Category_Total = temp.Sum(x => x.Get_Current_Amount(Get_Tax_Amount(x)));

                if (Warnings_Dictionary[Category].Warning_Type == "Price")
                {
                    if (Category_Total > (Warnings_Dictionary[Category].Warning_Amt * (Warnings_Dictionary[Category].Final_Level / 100)))
                    {
                        Grey_Out();
                        // Get monthly income and compare
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have spent $" + Category_Total + " of your '" + Category + "' monthly limit of your $" + Warnings_Dictionary[Category].Warning_Amt + " limit", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    else if (Category_Total > (Warnings_Dictionary[Category].Warning_Amt * (Warnings_Dictionary[Category].Second_Level / 100)))
                    {
                        Grey_Out();
                        // Get monthly income and compare
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have spent $" + Category_Total + " of your '" + Category + "' monthly limit of your $" + Warnings_Dictionary[Category].Warning_Amt + " limit", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    else if (Category_Total > (Warnings_Dictionary[Category].Warning_Amt * (Warnings_Dictionary[Category].First_Level / 100)))
                    {
                        Grey_Out();
                        // Get monthly income and compare
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have spent $" + Category_Total + " of your '" + Category + "' monthly limit of your $" + Warnings_Dictionary[Category].Warning_Amt + " limit", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();

                    }
                    Grey_In();
                }
                else if (Warnings_Dictionary[Category].Warning_Type == "Percent")
                {
                    double Ref_Percent = (1 - (Monthly_Income - Category_Total) / Monthly_Income);

                    // Check final level first
                    if (Ref_Percent > (Warnings_Dictionary[Category].Final_Level / 100 * Warnings_Dictionary[Category].Warning_Amt / 100))
                    {
                        Grey_Out();
                        // Get monthly income and compare
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have spent " + Math.Round(Ref_Percent * 10000, 2) + "% of your '" + Category + "' monthly limit of " + Warnings_Dictionary[Category].Warning_Amt + "% of monthly income", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    else
                    if (Ref_Percent > (Warnings_Dictionary[Category].Second_Level / 100 * Warnings_Dictionary[Category].Warning_Amt / 100))
                    {
                        Grey_Out();
                        // Get monthly income and compare
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have spent " + Math.Round(Ref_Percent * 10000, 2) + "% of your '" + Category + "' monthly limit of " + Warnings_Dictionary[Category].Warning_Amt + "% of monthly income", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    else if (Ref_Percent > (Warnings_Dictionary[Category].First_Level / 100 * Warnings_Dictionary[Category].Warning_Amt / 100))
                    {
                        Grey_Out();
                        // Get monthly income and compare
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have spent " + Math.Round(Ref_Percent * 10000, 2) + "% of your '" + Category + "' monthly limit of " + Warnings_Dictionary[Category].Warning_Amt + "% of monthly income", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    Grey_In();
                }
            }
            else
            {
                // Do nothing
            }
        }

        private void Check_Budget()
        {
            if (Monthly_Income > 0 && Current_Month_Expenditure > 0 && (Savings.Alert_1 || Savings.Alert_2 || Savings.Alert_3))
            {
                double Reference_Value = Savings.Structure == "Amount" ? Savings.Ref_Value : Monthly_Income * (Savings.Ref_Value / 100);

                using (Savings_Helper SH = new Savings_Helper(this))
                {
                    double current_month_value = SH.Get_Monthly_Salary(DateTime.Now.Month, DateTime.Now.Year);
                    // Fixed amount budget checking
                    if (Savings.Alert_3 && (current_month_value - Current_Month_Expenditure == Reference_Value))
                    {
                        Grey_Out();
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have exactly no budget left", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    if (Savings.Alert_3 && (current_month_value - Current_Month_Expenditure < Reference_Value))
                    {
                        Grey_Out();
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have spent $" + String.Format("{0:0.00}", Math.Abs(current_month_value - Current_Month_Expenditure - Reference_Value)) + " over budget", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    else if (Savings.Alert_2 && ((current_month_value - Current_Month_Expenditure) * 0.9 < Reference_Value) && current_month_value - Current_Month_Expenditure > 0)
                    {
                        Grey_Out();
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have $" + String.Format("{0:0.00}", Math.Abs((current_month_value - Current_Month_Expenditure) - Reference_Value)) + " before going over-budget", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    else if (Savings.Alert_1 && ((current_month_value - Current_Month_Expenditure) * 0.8 < Reference_Value) && current_month_value - Current_Month_Expenditure > 0)
                    {
                        Grey_Out();
                        Form_Message_Box FMB = new Form_Message_Box(this, "You have $" + String.Format("{0:0.00}", Math.Abs((current_month_value - Current_Month_Expenditure) - Reference_Value)) + " before going over-budget", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    Grey_In();
                }
            }
        }

        string[] lines;

        public bool Enable_Encrypt = true;

        public void Clear_Backups(int days = 30)
        {
            string Backup_Path = localSavePath + @"\Backups";

            if (Directory.Exists(Backup_Path))
            {
                string[] File_in_dir;

                File_in_dir = Directory.GetFiles(Backup_Path, "*", SearchOption.AllDirectories);
                foreach (string file in File_in_dir)
                {
                    //Diagnostics.WriteLine((DateTime.Now - File.GetCreationTime(file)).TotalDays);
                    if ((DateTime.Now - File.GetLastWriteTime(file)).TotalDays > days)
                    {
                        File.Delete(file);
                    }
                }
            }
        }

        public void Check_Outstanding_ARP()
        {
            if (Settings_Dictionary.ContainsKey("ARP_ALERTS") && Settings_Dictionary["ARP_ALERTS"] == "1")
            {
                foreach (Account a in Account_List.Where(x => x.Status > 0))
                {
                    double over_due_days = Math.Abs((DateTime.Now - a.Start_Date).TotalDays);
                    if (over_due_days > 14)// && a.Alert_Active == "1")
                    {
                        /*
                        Grey_Out();
                        Form_Message_Box FMB = new Form_Message_Box(this, "The " + a.Type + " account by " + a.Payer + " is overdue by " + Math.Round(over_due_days) + " days for " + a.Amount, true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                        Grey_In();
                        */
                        Append_Alert_Str("The " + a.Type + " account by " + a.Payer + " is overdue by " + Math.Round(over_due_days) + " days for " + a.Amount);
                    }
                }
            }
        }

        public void Append_Alert_Str(string str)
        {
            if (Alert_String.Length > 1)
            {
                Alert_String += Environment.NewLine;
            }

            Alert_String += str;
        }

        public void Check_Calendar_Alerts()
        {
            foreach (Calendar_Events CE in Calendar_Events_List)
            {
                // If the event date is today
                if (CE.Date == DateTime.Now.Date && CE.Is_Active == "1")
                {
                    // get date difference
                    CE.Is_Active = "0";
                    double Date_Diff = (DateTime.Now.Date - CE.Date).TotalDays;

                    Append_Alert_Str(CE.Title + " is today");
                    /* 
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(this, CE.Title + " is today", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                    */
                }
                else
                {
                    for (int i = CE.Alert_Dates.Count - 1; i >= 0; i--)
                    {
                        DateTime temp = CE.Alert_Dates[i];

                        // check if the date coincides with any event dates. If so, prompt an alert
                        if (temp.Date == DateTime.Now.Date)
                        {
                            // get date difference
                            double Date_Diff = (DateTime.Now.Date - CE.Date).TotalDays;
                            /*
                            Grey_Out();
                            Form_Message_Box FMB = new Form_Message_Box(this, CE.Title + " is in " + Math.Round(Math.Abs(Date_Diff), 1) + " days (on " + CE.Date.ToShortDateString() + ")", true, 0, this.Location, this.Size);
                            FMB.ShowDialog();
                            Grey_In();*/
                            Append_Alert_Str(CE.Title + " is in " + Math.Round(Math.Abs(Date_Diff), 1) + " days (on " + CE.Date.ToShortDateString() + ")");

                            // Do not remind more than once. If alert goes off, don't remind anymore
                            CE.Alert_Dates.RemoveAt(i);
                        }
                        else
                        {
                        }
                    }
                }
                // Delete expired alerts (dates before today)
                for (int i = CE.Alert_Dates.Count - 1; i >= 0; i--)
                {
                    if (CE.Alert_Dates[i] < DateTime.Now)
                    {
                        CE.Alert_Dates.RemoveAt(i);
                    }
                }
            }
        }

        public void Check_Payment_Alerts()
        {
            foreach (Payment p in Payment_List)
            {
                string Alert_Message = p.Check_Payment_Due();
                if (Alert_Message.Length > 5)
                {
                    /*
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(this, Alert_Message, true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();*/
                    Append_Alert_Str(Alert_Message);
                }
            }
        }

        public List<Order> Get_Orders_With_Upcoming_Refunds(DateTime RefDate = new DateTime())
        {
            DateTime refDate = RefDate.Year > 1850 ? RefDate : DateTime.Now;
            List<Order> returnList = new List<Order>();

            foreach (Item item in Master_Item_List)
            {
                try
                {
                    Location refLoc = Location_List.First(x => x.Name == item.Location);
                    if (item.RefundAlert) Diagnostics.WriteLine("");
                    if (refLoc.Refund_Days > 0 && (item.Get_Current_Quantity() > 0) && item.RefundAlert &&
                        item.Date.AddDays(refLoc.Refund_Days).Date >= refDate.Date &&
                        returnList.Count(x => x.OrderID == item.OrderID) == 0
                    ) //if last date to return item is greater than refDate, add to list
                    {
                        returnList.Add(Order_List.First(x => x.OrderID == item.OrderID));
                    }
                }
                catch
                {
                    // Add location to list if it does not exist
                    if (!Location_List.Any(x => x.Name == item.Location))
                    {
                        Location_List.Add(new Location()
                        {
                            Name = item.Location,
                            Refund_Days = 0
                        });
                    }
                }
            }

            return returnList;

        }

        // Initialize legacy load information
        private void Legacy_Load_Information(string Path_Bypass = "")
        {
            string Info_Path = localSavePath + "\\journal_info.txt";

            for (int i = 0; i < 90; i++) Diagnostics.WriteLine("LEGACY LOAD DETECTED!!!!");
            Info_Path = Path_Bypass == "" ? localSavePath + "\\journal_info.txt" : Path_Bypass;
            if (File.Exists(Info_Path))
            {

                var text = File.ReadAllText(Info_Path);
                if (text.Length > 0)
                {
                    try
                    {
                        if (Enable_Encrypt && !text.Contains("[INFO_TYPE]=ITEM||[ITEM_DESC]") && !text.Contains("[PERSONAL_SETTINGS]"))  // force decrypt 
                        {
                            lines = AESGCM.SimpleDecryptWithPassword(text, "PASSWORDisHERE").Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        }
                        else if (!Enable_Encrypt || text.Contains("[INFO_TYPE]=ITEM||[ITEM_DESC]") || text.Contains("PERSONAL_SETTINGS"))
                        {
                            lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                        }
                    }
                    catch (Exception e)
                    {
                        //Form_Message_Box FMB = new Form_Message_Box(this, "Decryption failed! " + e.ToString());
                        //FMB.Show();
                    }
                }

                foreach (string line in lines)
                {
                    // If Item
                    if (line.Contains("[INFO_TYPE]=ITEM"))
                    {
                        Item New_Item = new Item();
                        New_Item.Name = Parse_Line_Information(line, "ITEM_DESC");
                        New_Item.Status = Parse_Line_Information(line, "ITEM_STATUS") == "" ? "0" : Parse_Line_Information(line, "ITEM_STATUS");
                        New_Item.Location = Parse_Line_Information(line, "ITEM_LOCATION");
                        New_Item.Payment_Type = Parse_Line_Information(line, "ITEM_PAYMENT");
                        New_Item.Category = Parse_Line_Information(line, "ITEM_CATEGORY");
                        New_Item.Discount_Amt = Parse_Line_Information(line, "ITEM_DISCOUNT_AMT") == "" ? 0 : Convert.ToDouble(Parse_Line_Information(line, "ITEM_DISCOUNT_AMT"));
                        New_Item.Price = Convert.ToDouble(Parse_Line_Information(line, "ITEM_PRICE"));
                        New_Item.Quantity = Convert.ToInt32(Parse_Line_Information(line, "ITEM_QUANTITY"));
                        New_Item.Date = Convert.ToDateTime(Parse_Line_Information(line, "ITEM_DATE"));
                        New_Item.Refund_Date = Parse_Line_Information(line, "ITEM_REFUND_DATE").Length > 0 ? Convert.ToDateTime(Parse_Line_Information(line, "ITEM_REFUND_DATE")) : DateTime.Now;
                        New_Item.Memo = Parse_Line_Information(line, "ITEM_MEMO");
                        New_Item.OrderID = Parse_Line_Information(line, "ITEM_ORDERID");
                        Master_Item_List.Add(New_Item);

                        // Add pre-existing information to comboboxes
                        if (!Category_List.Contains(New_Item.Category)) Category_List.Add(New_Item.Category);

                        bool Contains_Location = false;
                        foreach (Company g in Company_List)
                        {
                            if (g.Name == New_Item.Location)
                            {
                                Contains_Location = true;
                            }
                        }

                        if (!Contains_Location) Company_List.Add(new Company() { Name = New_Item.Location });
                    }
                    // If Link information, store link information
                    else if (line.Contains("[LINK_SOURCE]"))
                    {
                        Link_Location.Add(Parse_Line_Information(line, "LINK_SOURCE"), Parse_Line_Information(line, "LINK_DESTINATION"));
                    }

                    // If Tax information, store tax information
                    else if (line.Contains("[TAX_CATEGORY]"))
                    {
                        Tax_Rules_Dictionary.Add(Parse_Line_Information(line, "TAX_CATEGORY"), Parse_Line_Information(line, "TAX_RATE"));
                    }
                    else if (line.Contains("[PERSONAL_SETTINGS]"))
                    {
                        // App settings
                        Frame_Color = System.Drawing.ColorTranslator.FromHtml(Parse_Line_Information(line, "APP_SETTING_COLOR"));
                        Set_Form_Color(Frame_Color);
                        Settings_Dictionary.Add("APP_SETTING_COLOR", Parse_Line_Information(line, "APP_SETTING_COLOR"));
                        // Login Credentials and parameters
                        Settings_Dictionary.Add("LOGIN_PASSWORD", Parse_Line_Information(line, "LOGIN_PASSWORD") == "" ? "" : Parse_Line_Information(line, "LOGIN_PASSWORD"));
                        Settings_Dictionary.Add("AUTHENTICATION_REQ", Parse_Line_Information(line, "AUTHENTICATION_REQ") == "" ? "0" : Parse_Line_Information(line, "AUTHENTICATION_REQ"));
                        Settings_Dictionary.Add("REMEMBER_ME", Parse_Line_Information(line, "REMEMBER_ME") == "" ? "0" : Parse_Line_Information(line, "REMEMBER_ME"));
                        Settings_Dictionary.Add("LOGIN_EMAIL", Parse_Line_Information(line, "LOGIN_EMAIL") == "" ? "" : Parse_Line_Information(line, "LOGIN_EMAIL"));
                        // Personal Information
                        Settings_Dictionary.Add("PERSONAL_FIRST_NAME", Parse_Line_Information(line, "PERSONAL_FIRST_NAME") == "" ? "" : Parse_Line_Information(line, "PERSONAL_FIRST_NAME"));
                        Settings_Dictionary.Add("PERSONAL_LAST_NAME", Parse_Line_Information(line, "PERSONAL_LAST_NAME") == "" ? "" : Parse_Line_Information(line, "PERSONAL_LAST_NAME"));
                        Settings_Dictionary.Add("PERSONAL_EMAIL", Parse_Line_Information(line, "PERSONAL_EMAIL") == "" ? "" : Parse_Line_Information(line, "PERSONAL_EMAIL"));
                        // Alerts and Windows characteristics
                        Settings_Dictionary.Add("SHOW_CALENDAR_ON_LOAD", Parse_Line_Information(line, "SHOW_CALENDAR_ON_LOAD") == "" ? "0" : Parse_Line_Information(line, "SHOW_CALENDAR_ON_LOAD"));
                        Settings_Dictionary.Add("CALENDAR_EMAIL_SYNC", Parse_Line_Information(line, "CALENDAR_EMAIL_SYNC") == "" ? "0" : Parse_Line_Information(line, "CALENDAR_EMAIL_SYNC"));
                        Settings_Dictionary.Add("ALERTS_ACTIVE", Parse_Line_Information(line, "ALERTS_ACTIVE") == "" ? "0" : Parse_Line_Information(line, "ALERTS_ACTIVE"));
                        Settings_Dictionary.Add("ARP_ALERTS", Parse_Line_Information(line, "ARP_ALERTS") == "" ? "0" : Parse_Line_Information(line, "ARP_ALERTS"));
                        Show_Calendar_On_Load = Settings_Dictionary["SHOW_CALENDAR_ON_LOAD"] == "1";
                        Alerts_On = Settings_Dictionary["ALERTS_ACTIVE"] == "1";
                        // Hobby Management Settings
                        Settings_Dictionary.Add("HOBBY_MGMT_X", Parse_Line_Information(line, "HOBBY_MGMT_X") == "" ? "250" : Parse_Line_Information(line, "HOBBY_MGMT_X"));
                        Settings_Dictionary.Add("HOBBY_MGMT_Y", Parse_Line_Information(line, "HOBBY_MGMT_Y") == "" ? "305" : Parse_Line_Information(line, "HOBBY_MGMT_Y"));
                        // Backup settings
                        Settings_Dictionary.Add("AUTO_SAVE", Parse_Line_Information(line, "AUTO_SAVE") == "" ? "0" : Parse_Line_Information(line, "AUTO_SAVE"));
                        Settings_Dictionary.Add("BACKUP_REQ", Parse_Line_Information(line, "BACKUP_REQ") == "" ? "0" : Parse_Line_Information(line, "BACKUP_REQ"));
                        Settings_Dictionary.Add("BACKUP_DEL", Parse_Line_Information(line, "BACKUP_DEL") == "" ? "0" : Parse_Line_Information(line, "BACKUP_DEL"));
                        // Income information
                        Settings_Dictionary.Add("INCOME_MONTHLY", (Parse_Line_Information(line, "INCOME_MONTHLY") == "" ? "0" : Parse_Line_Information(line, "INCOME_MONTHLY")));
                        Settings_Dictionary.Add("INCOME_HOURLY", (Parse_Line_Information(line, "INCOME_HOURLY") == "" ? "0" : Parse_Line_Information(line, "INCOME_HOURLY")));
                        Settings_Dictionary.Add("INCOME_WEEKLY", (Parse_Line_Information(line, "INCOME_WEEKLY") == "" ? "0" : Parse_Line_Information(line, "INCOME_WEEKLY")));
                        Settings_Dictionary.Add("INCOME_DAILY", (Parse_Line_Information(line, "INCOME_DAILY") == "" ? "0" : Parse_Line_Information(line, "INCOME_DAILY")));
                        Settings_Dictionary.Add("INCOME_YEARLY", (Parse_Line_Information(line, "INCOME_YEARLY") == "" ? "0" : Parse_Line_Information(line, "INCOME_YEARLY")));
                        Settings_Dictionary.Add("WORK_HPD", (Parse_Line_Information(line, "WORK_HPD") == "" ? "0" : Parse_Line_Information(line, "WORK_HPD")));
                        Settings_Dictionary.Add("WORK_OHPD", (Parse_Line_Information(line, "WORK_OHPD") == "" ? "0" : Parse_Line_Information(line, "WORK_OHPD")));
                        Settings_Dictionary.Add("WORK_OMULTI", (Parse_Line_Information(line, "WORK_OMULTI") == "" ? "0" : Parse_Line_Information(line, "WORK_OMULTI")));
                        Settings_Dictionary.Add("INCOME_TAX_RATE", (Parse_Line_Information(line, "INCOME_TAX_RATE") == "" ? "0" : Parse_Line_Information(line, "INCOME_TAX_RATE")));
                        Settings_Dictionary.Add("GENERAL_TAX_RATE", (Parse_Line_Information(line, "GENERAL_TAX_RATE") == "" ? "0.13" : Parse_Line_Information(line, "GENERAL_TAX_RATE")));
                        Settings_Dictionary.Add("INCOME_CHANGE_LOG", (Parse_Line_Information(line, "INCOME_CHANGE_LOG") == "" ? "" : Parse_Line_Information(line, "INCOME_CHANGE_LOG")));

                        Monthly_Income = Convert.ToDouble(Settings_Dictionary["INCOME_MONTHLY"]);
                        Tax_Rate = Convert.ToDouble(Settings_Dictionary["GENERAL_TAX_RATE"]);
                    }
                    else if (line.Contains("[INFO_TYPE]=ORDER"))
                    {
                        Order New_Order = new Order();
                        New_Order.Location = Parse_Line_Information(line, "ORDER_LOCATION");
                        New_Order.OrderMemo = Parse_Line_Information(line, "ORDER_MEMO");
                        New_Order.Payment_Type = Parse_Line_Information(line, "ORDER_PAYMENT");
                        New_Order.Tax_Overridden = (Parse_Line_Information(line, "ORDER_TAX_OVERRIDEN") == "1");
                        New_Order.Order_Total_Pre_Tax = Convert.ToDouble(Parse_Line_Information(line, "ORDER_PRETAX_PRICE"));
                        New_Order.GC_Amount = Convert.ToDouble(Parse_Line_Information(line, "GC_AMOUNT", "||", "0"));
                        New_Order.Order_Taxes = Convert.ToDouble(Parse_Line_Information(line, "ORDER_TAX"));
                        New_Order.Order_Discount_Amt = Parse_Line_Information(line, "ORDER_DISCOUNT_AMT") == "" ? 0 : Convert.ToDouble(Parse_Line_Information(line, "ORDER_DISCOUNT_AMT"));
                        New_Order.Order_Quantity = Convert.ToInt32(Parse_Line_Information(line, "ORDER_QUANTITY"));
                        New_Order.Date = Convert.ToDateTime(Parse_Line_Information(line, "ORDER_DATE"));
                        New_Order.OrderID = Parse_Line_Information(line, "ORDER_ORDERID");
                        Order_List.Add(New_Order);
                        editTest1ToolStripMenuItem.Enabled = true;
                        toolStripMenuItem36.Enabled = true;
                        toolStripMenuItem12.Enabled = true;
                        toolStripMenuItem19.Enabled = true;
                        toolStripMenuItem35.Enabled = true;
                        toolStripMenuItem18.Enabled = true;
                        toolStripMenuItem32.Enabled = true;
                        toolStripMenuItem28.Enabled = true;
                        toolStripMenuItem33.Enabled = true;
                        toolStripMenuItem11.Enabled = true;
                        toolStripMenuItem16.Enabled = true;
                    }
                    else if (line.Contains("[EXPENSE_TYPE]="))
                    {
                        List<DateTime> Date_Sequence = new List<DateTime>();
                        if (Parse_Line_Information(line, "EXPENSE_DATE_SEQUENCE") != "")
                        {
                            string[] dates = Parse_Line_Information(line, "EXPENSE_DATE_SEQUENCE").Split(new string[] { "," }, StringSplitOptions.None);
                            foreach (string Date in dates)
                                Date_Sequence.Add(Convert.ToDateTime(Date));
                        }

                        Expenses New_Expense = new Expenses();
                        New_Expense.Expense_Type = Parse_Line_Information(line, "EXPENSE_TYPE");
                        New_Expense.Expense_Name = Parse_Line_Information(line, "EXPENSE_NAME");
                        New_Expense.Expense_Payee = Parse_Line_Information(line, "EXPENSE_PAYEE");
                        New_Expense.Expense_Frequency = Parse_Line_Information(line, "EXPENSE_FREQUENCY");
                        New_Expense.Date_Sequence = Date_Sequence;
                        New_Expense.Expense_Status = Parse_Line_Information(line, "EXPENSE_STATUS");
                        New_Expense.Expense_Amount = Convert.ToDouble(Parse_Line_Information(line, "EXPENSE_AMOUNT"));
                        New_Expense.Payment_Last_Four = Parse_Line_Information(line, "EXPENSE_PAYMENT_LAST_FOUR");
                        New_Expense.AutoDebit = Parse_Line_Information(line, "EXPENSE_AUTODEBIT", "||", "0");
                        New_Expense.Payment_Company = Parse_Line_Information(line, "EXPENSE_PAYMENT_COMPANY");
                        New_Expense.Expense_Start_Date = Convert.ToDateTime(Parse_Line_Information(line, "EXPENSE_START_DATE"));
                        New_Expense.Last_Pay_Date = Parse_Line_Information(line, "EXPENSE_LAST_PAY_DATE") == "" ? new DateTime(1990, 1, 1) : Convert.ToDateTime(Parse_Line_Information(line, "EXPENSE_LAST_PAY_DATE"));
                        Expenses_List.Add(New_Expense);


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
                        Warnings_Dictionary.Add(warn.Category, warn);
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
                        Payment_List.Add(Payment);

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
                        Account_List.Add(ACC);
                    }
                    else if (line.Contains("SAVINGS_SETTINGS") && line.Contains("STRUCTURE"))
                    {
                        Savings.Structure = Parse_Line_Information(line, "STRUCTURE");
                        Savings.Ref_Value = Convert.ToDouble(Parse_Line_Information(line, "AMOUNT"));
                        Savings.Alert_1 = Parse_Line_Information(line, "ALERT_1") == "1" ? true : false;
                        Savings.Alert_2 = Parse_Line_Information(line, "ALERT_2") == "1" ? true : false;
                        Savings.Alert_3 = Parse_Line_Information(line, "ALERT_3") == "1" ? true : false;
                        Savings_Instantiated = true;
                    }
                    else if (line.Contains("CALENDAR_EVENT") && line.Contains("CALENDAR_IMPORTANCE"))
                    {
                        Calendar_Events CE = new Calendar_Events();
                        CE.Title = Parse_Line_Information(line, "CALENDAR_TITLE");
                        CE.Is_Active = Parse_Line_Information(line, "CALENDAR_ACTIVE") == "" ? "0" : Parse_Line_Information(line, "CALENDAR_ACTIVE");
                        CE.Description = Parse_Line_Information(line, "CALENDAR_DESC").Replace("~~", Environment.NewLine);
                        CE.Importance = Convert.ToInt32(Parse_Line_Information(line, "CALENDAR_IMPORTANCE"));
                        CE.Date = Convert.ToDateTime(Parse_Line_Information(line, "CALENDAR_DATE"));
                        string[] date_Strings = (Parse_Line_Information(line, "CALENDAR_ALERT_SEQUENCE").Split(new string[] { "~" }, StringSplitOptions.None));
                        foreach (string dS in date_Strings)
                        {
                            if (dS.Length > 0)
                            {
                                CE.Alert_Dates.Add(Convert.ToDateTime(dS));
                            }
                        }
                        Calendar_Events_List.Add(CE);
                    }
                    else if (line.Contains("[TIER_STRUCTURE]"))
                    {
                        string[] temp = Parse_Line_Information(line, "TIER_STRUCTURE").Split(new string[] { "," }, StringSplitOptions.None);
                        foreach (string g in temp) Tier_Format.Add(Convert.ToInt32(g));
                        Tier_Format = Tier_Format.Count == 0 ? new List<int> { 1 } : Tier_Format;
                    }
                    else if (line.Contains("[PO_BANK_NAME]"))
                    {
                        Payment_Options PO = new Payment_Options();
                        PO.Type = Parse_Line_Information(line, "PO_TYPE");
                        PO.Payment_Last_Four = Parse_Line_Information(line, "PO_LAST_FOUR");
                        PO.Payment_Company = Parse_Line_Information(line, "PO_COMPANY");
                        PO.Payment_Bank = Parse_Line_Information(line, "PO_BANK_NAME");
                        PO.Ending_Balance = Convert.ToDouble(Parse_Line_Information(line, "PO_BALANCE"));
                        PO.Date = Convert.ToDateTime(Parse_Line_Information(line, "PO_DATE"));
                        PO.Note = Parse_Line_Information(line, "PO_NOTE");
                        PO.Hidden_Note = Parse_Line_Information(line, "PO_HIDDENNOTE");
                        PO.Amount = Convert.ToDouble(Parse_Line_Information(line, "PO_AMOUNT"));
                        Payment_Options_List.Add(PO);
                    }
                    else if (line.Contains("[HOBBY_CONTAINER_ID]"))
                    {
                        Hobby_Item HI = new Hobby_Item();
                        HI.Name = Parse_Line_Information(line, "HOBBY_NAME");
                        HI.Category = Parse_Line_Information(line, "HOBBY_CATEGORY");
                        HI.OrderID = Parse_Line_Information(line, "HOBBY_ORDER_ID");
                        HI.Price = Convert.ToDouble(Parse_Line_Information(line, "HOBBY_PRICE"));
                        HI.Container_ID = Convert.ToInt32(Parse_Line_Information(line, "HOBBY_CONTAINER_ID"));
                        HI.Profile_Number = Parse_Line_Information(line, "PROFILE_NUMBER");
                        HI.Unique_ID = Parse_Line_Information(line, "UNIQUE_ID");
                        Master_Hobby_Item_List.Add(HI);
                    }
                    else if (line.Contains("[CONTAINER_NAME"))
                    {
                        List<Container> temp = new List<Container>();
                        int Max_Container_ID = Master_Hobby_Item_List.Count == 0 ? 0 : Master_Hobby_Item_List.Max(x => x.Container_ID);

                        for (int i = 1; i <= Max_Container_ID; i++)
                        {
                            if (Parse_Line_Information(line, "CONTAINER_NAME" + i.ToString()) != "")
                            {
                                temp.Add(new Container() { Name = Parse_Line_Information(line, "CONTAINER_NAME" + i), ID = i.ToString() });
                            }
                        }

                        Master_Container_Dict.Add(Parse_Line_Information(line, "CONTAINER_PROFILE_NUMBER") == "" ? "1" : Parse_Line_Information(line, "CONTAINER_PROFILE_NUMBER"), temp);

                    }
                    else if (line.Contains("[HOBBY_PROFILE"))
                    {
                        int profile_number = 1;
                        while (Parse_Line_Information(line, "HOBBY_PROFILE" + profile_number) != "")
                        {
                            if (Master_Container_Dict.ContainsKey(profile_number.ToString()))
                                Hobby_Profile_List.Add(Parse_Line_Information(line, "HOBBY_PROFILE" + profile_number));
                            profile_number++;
                        }
                    }
                    else if (line.Contains("[AGENDA_ITEM]"))
                    {
                        Agenda_Item AI = new Agenda_Item();
                        AI.Name = Parse_Line_Information(line, "A_NAME");
                        AI.Date = Convert.ToDateTime(Parse_Line_Information(line, "A_DATE"));
                        AI.ID = Convert.ToInt32(Parse_Line_Information(line, "A_ID"));
                        AI.Check_State = Parse_Line_Information(line, "A_CHECK_STATE") == "1";
                        Agenda_Item_List.Add(AI);
                    }
                    else if (line.Contains("[SHOPPING_ITEM]"))
                    {
                        Shopping_Item AI = new Shopping_Item();
                        AI.Name = Parse_Line_Information(line, "S_NAME");
                        AI.ID = Convert.ToInt32(Parse_Line_Information(line, "S_ID"));
                        AI.Check_State = Parse_Line_Information(line, "S_CHECK_STATE") == "1";
                        Agenda_Item_List.FirstOrDefault(x => x.ID == AI.ID).Shopping_List.Add(AI);
                    }
                }

                if (Settings_Dictionary["BACKUP_DEL"] == "1")
                {
                    Clear_Backups(14);
                }
                if (Settings_Dictionary["BACKUP_REQ"] == "1")
                {
                    if (!Directory.Exists(localSavePath + @"\Backups"))
                    {
                        Directory.CreateDirectory(localSavePath + @"\Backups");
                    }
                    try
                    {
                        File.Copy(Info_Path, localSavePath + "\\Backups\\personal_banker_backup_" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + ".cfg", true);
                    }
                    catch
                    { }
                }
                else
                {
                    using (StreamWriter sw = File.CreateText(localSavePath + "\\journal_info.txt"))
                    {
                        sw.Write("[PERSONAL_SETTINGS]");
                        sw.Close();
                        Info_Path = localSavePath + "\\journal_info.txt";
                        //Reload_Program();
                    }
                }
                if (!Savings_Instantiated)
                {
                    Savings.Structure = "";
                    Savings.Ref_Value = 0;
                    Savings.Alert_1 = false;
                }
            }
        }

        // Initialize existing information5
        private bool Load_Information(string load_path = "", bool loadFromCloud = false)
        {
            long FileSize = 0;

            if (load_path.Length > 0 && !loadFromCloud) FileSize = new FileInfo(load_path).Length;
            if (loadFromCloud || (load_path.Length > 0 && load_path.Length > 0 && FileSize < 1000))
            {
                if (!(LoadHelper.Load_Information(load_path, loadFromCloud))) return false;
            }
            else if (load_path.Length > 0)
            {
                try
                {
                    Legacy_Load_Information(load_path);
                }
                catch
                {
                    
                }
            }
            else if (File.Exists(localSavePath + "\\Personal_Banker.cfg"))
            {
                LoadHelper.Load_Information(localSavePath + "\\Personal_Banker.cfg");
            }
            else if (File.Exists(localSavePath + "\\journal_info.txt"))
            {
                Legacy_Load_Information();
            }
            else
            {
                Reload_Program("", true);
            }

            // SETTINGS REMOVED, DO NOT LOAD
            if (Load_Error)
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(this, "Files have been corrupted. Please load from backup or import from another profile", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();

                Reload_Program("", true);
                SaveHelper.Regular_Save();
            }

            return true;
        }

        public void Check_Tracking_Alerts()
        {
            // Retain all outstanding orders only
            foreach (Shipment_Tracking ST in Tracking_List)
            {
                Form_Message_Box FMB;

                // get date difference
                double Date_Diff = (DateTime.Now.Date - ST.Expected_Date.Date).TotalDays;
                Order Ref_Order = Order_List.FirstOrDefault(x => x.OrderID == ST.Ref_Order_Number);

                if (ST.Alert_Active && ST.Status > 0)
                {
                    if (ST.Last_Alert_Date.Date != DateTime.Now.Date)
                    {
                        Append_Alert_Str("A package from " + Ref_Order.Location + " is expected to arrive " + (ST.Expected_Date.Date == DateTime.Now.Date ? "today" : ("in " + Math.Round(Math.Abs(Date_Diff), 1) + " days (on " + ST.Expected_Date.ToShortDateString() + ")") + (DateTime.Now > ST.Expected_Date ? " ago" : "")));
                        /*
                        Grey_Out();
                        FMB = new Form_Message_Box(this, "A package from " + Ref_Order.Location + " is expected to arrive " + (ST.Expected_Date.Date == DateTime.Now.Date ? "today" : ("in " + Math.Round(Math.Abs(Date_Diff), 1) + " days (on " + ST.Expected_Date.ToShortDateString() + ")") + (DateTime.Now > ST.Expected_Date ? " ago" : "")), true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                        Grey_In();*/
                    }
                }
                if (ST.Email_Active || ST.Alert_Active)
                {
                    // Set last alert date to today
                    ST.Last_Alert_Date = DateTime.Now.Date;
                }
            }
        }
        
        // Return the tax rate for item
        public double Get_Tax_Amount(Item ref_Item)
        {
            return (Tax_Rules_Dictionary.ContainsKey(ref_Item.Category) ? Convert.ToDouble(Tax_Rules_Dictionary[ref_Item.Category]) : Tax_Rate);
        }

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
            string[] Split_Layer_1 =  input.Split(new string[] { parse_token }, StringSplitOptions.None);

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

        private void location_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Link_Location.ContainsKey(location_box.Text))
            {
                category_box.Text = Link_Location[location_box.Text];
                item_desc.Focus();
            }
        }

        private void fILE2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Settings_Dictionary["LAST_LOGIN"] = DateTime.Now.ToString();
            //SaveHelper.Regular_Save();
            Background_Save(true);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Enable_Encrypt = false;
            try
            {
                // Try deleting temp file
                File.Delete(localSavePath + "\\temp.txt");
                SaveHelper.Save_All_Sections(localSavePath + "\\temp.txt");
                File.SetAttributes(localSavePath + "\\temp.txt", File.GetAttributes(localSavePath + "\\temp.txt") | FileAttributes.Hidden);


                var text = File.ReadAllText(localSavePath + "\\temp.txt").Trim();
                string[] temp = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                Diagnostics.WriteLine("Line count = " + temp.Length);

                Process.Start(localSavePath + "\\temp.txt");
            }
            catch
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(this, "Please close all opened confiuration files windows", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
            Enable_Encrypt = true;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Tax_Rules TR = new Tax_Rules(this, this.Location, this.Size);
            TR.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Link_Rules LR = new Link_Rules(this, this.Location, this.Size);
            LR.ShowDialog();
            Grey_In();
        }

        private void editTest1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Refund_Report RP = new Refund_Report(this);
            RP.ShowDialog();
            Grey_In();
        }

        #region Function dump
        private void cALCULATORToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
        }
        private void eDITToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Master_Item_List.Count < 1)
            {
                editTest1ToolStripMenuItem.Enabled = false;
                toolStripMenuItem36.Enabled = false;
                toolStripMenuItem12.Enabled = false;
                toolStripMenuItem19.Enabled = false;
                toolStripMenuItem35.Enabled = false;
                toolStripMenuItem11.Enabled = false;
                toolStripMenuItem16.Enabled = false;
                toolStripMenuItem18.Enabled = false;
                toolStripMenuItem28.Enabled = false;
                toolStripMenuItem33.Enabled = false;
                toolStripMenuItem32.Enabled = false;
            }
            else
            {
                editTest1ToolStripMenuItem.Enabled = true;
                toolStripMenuItem36.Enabled = true;
                toolStripMenuItem12.Enabled = true;
                toolStripMenuItem19.Enabled = true;
                toolStripMenuItem35.Enabled = true;
                toolStripMenuItem11.Enabled = true;
                toolStripMenuItem16.Enabled = true;
                toolStripMenuItem18.Enabled = true;
                toolStripMenuItem28.Enabled = true;
                toolStripMenuItem33.Enabled = true;
                toolStripMenuItem32.Enabled = true;
            }
        }


        private void fILEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripMenuItem34.Visible = Settings_Dictionary.ContainsKey("AUTO_SAVE") && Settings_Dictionary["AUTO_SAVE"] == "1";
        }
        #endregion

        public void UpdateStatus(string statusString)
        {
            this.InvokeEx(f_temp => f_temp.statusLabel.Text = statusString);
            if (statusLabel.Text == "Ready" || statusLabel.Text.Contains("complete!") || statusLabel.Text.Contains("sent!") || statusLabel.Text.Contains("created!"))
            {
                this.InvokeEx(f_temp => f_temp.statusLabel.ForeColor = Color.PaleGreen);
            }
            else
            {
                this.InvokeEx(f_temp => f_temp.statusLabel.ForeColor = Color.Pink);
            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Customized_Settings CS = new Customized_Settings(this, this.Location, this.Size);
            CS.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            Grey_Out();
            NewPurchases NP = new NewPurchases(this, "", Location, Size);
            NP.ShowDialog();
            Grey_In();

            #region Set status spending info
            StatusSetSpending(false, true);
            #endregion
        }

        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "Select the directory you wish to export information to:";

            DialogResult result = fbd.ShowDialog();

            if (result == DialogResult.OK)
            {
                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    SaveHelper.Export_Data(fbd.SelectedPath);
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(this, "Information export successfully to \"" + fbd.SelectedPath + "\"", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }
            }
        }

        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            string Info_Path = localSavePath + "\\journal_info.txt";

            SaveHelper.Regular_Save();
            OpenFileDialog file = new OpenFileDialog();
            file.Title = "Import information file";
            file.Multiselect = false;
            file.DefaultExt = ".cfg";
            file.Filter = "Config Files (*.cfg)|*.cfg";

            if (file.ShowDialog() == DialogResult.OK)
            {
                Grey_Out();
                using (var form = new Yes_No_Dialog(this, "Your current information will be lost. Do you wish to export it?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                {
                    var result2 = form.ShowDialog();
                    if (result2 == DialogResult.OK && form.ReturnValue1 == "1")
                    {
                        FolderBrowserDialog fbd = new FolderBrowserDialog();
                        fbd.Description = "Select the directory you wish to export information to:";

                        DialogResult result = fbd.ShowDialog();

                        if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                        {
                            SaveHelper.Export_Data(fbd.SelectedPath);
                            Grey_Out();
                            Form_Message_Box FMB = new Form_Message_Box(this, "Information export successfully to \"" + fbd.SelectedPath + "\"", true, 0, this.Location, this.Size);
                            FMB.ShowDialog();
                            Grey_In();
                        }
                    }
                }
                Grey_In();

                string file_path = file.FileName;
                if (file_path.Contains(".cfg"))
                {
                    Info_Path = file_path;
                    reset_button.PerformClick();
                    Reload_Program(Info_Path);


                    // If authentication set already
                    if (Settings_Dictionary.ContainsKey("AUTHENTICATION_REQ") && Settings_Dictionary["AUTHENTICATION_REQ"] == "1")
                    {
                        Grey_Out();
                        using (var form = new Authentication_Form(this, "Profile is locked. Please unlock using appropriate email and password.", "Authentication", Settings_Dictionary["LOGIN_EMAIL"], Settings_Dictionary["LOGIN_PASSWORD"], true, false, this.Location, this.Size))
                        {
                            var result2 = form.ShowDialog();
                            if (result2 == DialogResult.OK)
                            {
                                if (form.ReturnValue1 == "1" && !Load_Error)
                                {
                                    Grey_Out();
                                    Form_Message_Box FMB = new Form_Message_Box(this, "Profile successfully imported! Please restart program", true, 0, this.Location, this.Size);
                                    FMB.ShowDialog();
                                    Grey_In();
                                    this.Show();
                                    this.Activate();
                                    this.TopMost = true;
                                    this.TopMost = false;
                                    SaveHelper.Regular_Save();
                                    //this.TopMost = true;
                                    Force_Close = true;
                                    this.Close();
                                }
                                else
                                {
                                    reset_button.PerformClick();
                                    Reload_Program();
                                    Grey_Out();
                                    Form_Message_Box FMB3 = new Form_Message_Box(this, "Profile not successfully loaded. Please restart program", true, 0, this.Location, this.Size);
                                    FMB3.ShowDialog();
                                    Grey_In();
                                    Force_Close = true;
                                    this.Close();
                                }
                            }
                            else
                            {
                                reset_button.PerformClick();
                                Reload_Program();
                                Grey_Out();
                                Form_Message_Box FMB3 = new Form_Message_Box(this, "Profile not successfully loaded. Please restart program", true, 0, this.Location, this.Size);
                                FMB3.ShowDialog();
                                Grey_In();
                                Force_Close = true;
                                this.Close();
                            }
                        }
                        Grey_In();
                    }
                }

                if (!Load_Error)
                {
                    SaveHelper.Regular_Save();
                    Grey_Out();
                    Form_Message_Box FMB2 = new Form_Message_Box(this, "Profile successfully imported! Please restart program", true, 0, this.Location, this.Size);
                    FMB2.ShowDialog();
                    Grey_In();
                    Force_Close = true;
                    this.Close();
                }
                else
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(this, "Files have been corrupted. Please load from backup or import from another profile", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }
            }  
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void quantity_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Enabled = dateTimePicker1.Enabled ? false : true;
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            Expenditures E = new Expenditures(this);
            E.ShowDialog();
        }

        private void toolStripMenuItem18_Click(object sender, EventArgs e)
        {
            if (!MOMG_open)
            {
                MOMG_open = true;
                MOMG = new Month_Over_Month_Graph(this);
                MOMG.ShowDialog();
            }
        }

        private void toolStripMenuItem16_Click(object sender, EventArgs e)
        {
            Analysis_Report AR = new Analysis_Report(this);
            AR.Show();
        }


        private void toolStripMenuItem21_Click(object sender, EventArgs e)
        {
            string Info_Path = localSavePath + "\\journal_info.txt";

            Grey_Out();
            using (var form1 = new Yes_No_Dialog(this, "Are you sure you wish to start a new profile?", "Warning", "No", "Yes", 0, this.Location, this.Size))
            {
                var result21 = form1.ShowDialog();
                if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                {
                    Grey_Out();
                    using (var form = new Yes_No_Dialog(this, "Your current information will be lost. Do you wish to export it?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                    {
                        var result2 = form.ShowDialog();
                        if (result2 == DialogResult.OK)
                        {
                            if (form.ReturnValue1 == "1")
                            {
                                FolderBrowserDialog fbd = new FolderBrowserDialog();
                                fbd.Description = "Select the directory you wish to export information to:";

                                DialogResult result = fbd.ShowDialog();

                                if (!string.IsNullOrWhiteSpace(fbd.SelectedPath))
                                {
                                    SaveHelper.Export_Data(fbd.SelectedPath);
                                    Grey_Out();
                                    Form_Message_Box FMB = new Form_Message_Box(this, "Information export successfully to \"" + fbd.SelectedPath + "\"", true, 0, this.Location, this.Size);
                                    FMB.ShowDialog();
                                    Grey_In();
                                }
                            }
                        }
                    }

                    Info_Path = "";
                    reset_button.PerformClick();


                    Reload_Program("", true);


                    // Enable Quick Links by default
                    // Suspend layout with 
                    Settings_Dictionary["QL_ENABLED"] = "1";
                    Settings_Dictionary["QL_AGENDA"] = "1";
                    Settings_Dictionary["QL_LOOKUP"] = "1";
                    Settings_Dictionary["QL_CALENDAR"] = "1";
                    Settings_Dictionary["QL_MANAGE_HOBBY"] = "1";
                    Settings_Dictionary["QL_DEPOSIT_PAY"] = "1";
                    Settings_Dictionary["QL_VIEW_PURCHASES"] = "1";
                    Settings_Dictionary["QL_MANAGE_PAYMENT"] = "1";
                    Settings_Dictionary["QL_VIEW_ONLINE"] = "1";
                    Settings_Dictionary["QL_SNEAK_PEEK"] = "1";
                    Settings_Dictionary["QL_CONTACTS"] = "1";
                    Settings_Dictionary["QL_BUDGET"] = "1";

                    SaveHelper.Regular_Save();

                    // Force restart
                    Grey_Out();
                    Form_Message_Box FMB2 = new Form_Message_Box(this, "Please restart the application", true, -20, this.Location, this.Size);
                    FMB2.ShowDialog();
                    Grey_In();

                    Environment.Exit(0);

                    /*
                    editTest1ToolStripMenuItem.Enabled = false;
                    toolStripMenuItem6.Enabled = false;
                    toolStripMenuItem36.Enabled = false;
                    toolStripMenuItem12.Enabled = false;
                    toolStripMenuItem18.Enabled = false;
                    toolStripMenuItem28.Enabled = false;
                    toolStripMenuItem33.Enabled = false;
                    toolStripMenuItem11.Enabled = false;
                    toolStripMenuItem16.Enabled = false;
                    Tier_Format = new List<int>() { 1 };
                    */
                }
                Grey_In();
            }
            Grey_In();
        }

        private void toolStripMenuItem23_Click(object sender, EventArgs e)
        {
            Calendar c = new Calendar(this);
            c.Show();
        }

        private void toolStripMenuItem24_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Alerts_And_Windows AAW = new Alerts_And_Windows(this, this.Location, this.Size);
            AAW.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem25_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Security S = new Security(this, this.Location, this.Size);
            S.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem26_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Personal_Information PI = new Personal_Information(this, this.Location, this.Size);
            PI.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem27_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Backup_Settings BS = new Backup_Settings(this, this.Location, this.Size);
            BS.ShowDialog();
            Grey_In();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
        }

        private void toolStripMenuItem28_Click(object sender, EventArgs e)
        {
            Tier_View TV = new Tier_View(this);
            TV.Show();
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        //public bool Reload_Hobby_Window = false;
        public bool Hobby_Window_Maximized = false;

        bool isAFK = false;

        private void toolStripMenuItem29_Click(object sender, EventArgs e)
        {
            Hobby_Management HM = new Hobby_Management(this);
            HM.ShowDialog();
        }

        /// <summary>
        /// Try and guess if current order is an online order and prompt shipping information dialog
        /// </summary>
        /// <returns></returns>
        private bool Check_Current_Order_Online()
        {
            // Check location if online
            if (Compare_Words(location_box.Text)) return true;
            else
            {
                // Check if items are online items
                foreach (Item i in Item_List)
                {
                    if (Compare_Words(i.Name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Check if any word in list contains word
        private bool Compare_Words(string word)
        {
            word = word.ToLower();
            List<string> Keywords = new List<string>() { "amazon", "ali-express", "aliexpress", "ali express", 
                                                         ".ca", ".com", ".net", ".gov", "website", "online",
                                                         "on line", "on-line", "shipping", "tracking", "site",
                                                         "www", "http", "https", "ebay", "newegg" 
                                                       };

            if (Keywords.Any(x => word.Contains(x)))
            {
                return true;
            }

            return false;
        }

        private void toolStripMenuItem31_Click(object sender, EventArgs e)
        {
            Agenda A = new Agenda(this, null, this.Location, this.Size);
            A.ShowDialog();
        }

        // tracking button
        private void button4_Click(object sender, EventArgs e)
        {
            bool Continue = true;
            if (!Check_Current_Order_Online())
            {
                Grey_Out();
                using (var form = new Yes_No_Dialog(this, "This order does not seem to be an online order. Are you sure you wish to setup tracking information and/or expected arrival dates?", "Warning", "No", "Yes", 50, this.Location, this.Size))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        if (form.ReturnValue1 == "1")
                        {
                        }
                        else
                        {
                            Continue = false;
                        }
                    }
                }
                Grey_In();
            }

            if (Continue)
            {
                List<Shipment_Tracking> Temp_List;
                if (Shipping_ID.Length > 0)
                {
                    Temp_List = Tracking_List.Where(x => x.Ref_Order_Number == Shipping_ID).ToList();

                }
                else
                {
                    Temp_List = Tracking_List.Where(x => x.Ref_Order_Number == "999999999").ToList();
                }
                // Create temp tracking if non existant
                if (Temp_List.Count == 0)
                {
                    Shipment_Tracking ST = new Shipment_Tracking();
                    ST.Ref_Order_Number = "999999999";
                    Temp_List.Add(ST);
                    if (!Tracking_Created) Tracking_List.Add(ST);
                }
                Grey_Out();
                Tracking T = new Tracking(this, false, Temp_List[0].Ref_Order_Number, this.Location, this.Size);
                T.ShowDialog();
                Grey_In();

                if (Editing_Receipt)
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(this, "Please remember to submit order changes in order to save tracking changes", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }
                Tracking_Created = true;
            } 
        }

        // Online orders
        private void toolStripMenuItem32_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Online_Orders OO = new Online_Orders(this);
            OO.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem33_Click(object sender, EventArgs e)
        {
            Wealth_Visualizer WV = new Wealth_Visualizer(this);
            WV.ShowDialog();
        }

        private void toolStripMenuItem34_Click(object sender, EventArgs e)
        {
            Force_Close = true;
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Grey_Out();
            GC_Manager GCM = new GC_Manager(this, true, this.Location, this.Size);
            GCM.ShowDialog();
            Grey_In();
            Invalidate();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem37_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Cash_Flow_Spreadsheet CFS = new Cash_Flow_Spreadsheet(this, this.Location, this.Size);
            CFS.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem38_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Quick_Links QL = new Quick_Links(this, this.Location, this.Size);
            QL.ShowDialog();
            repaintButtons = true;
            Grey_In();
        }

        public void Background_Save(bool forceSave = false, bool isBackup = false, bool multiThreadSave = true)
        {
            // Save changes only if auto_Save active after every change (IN BACKGROUND THREAD)
            if (forceSave || (Settings_Dictionary.ContainsKey("AUTO_SAVE") && Settings_Dictionary["AUTO_SAVE"] == "1"))
            {
                // Only set time if auto save (since background save is for auto)
                if (Settings_Dictionary.ContainsKey("AUTO_SAVE") && Settings_Dictionary["AUTO_SAVE"] == "1")
                {
                    // Set last login to now
                    //Settings_Dictionary["LAST_LOGIN"] = DateTime.Now.ToString();
                }

                if (!Saving_In_Process)
                {
                    Task.Run(() =>
                    {
                        Saving_In_Process = true;
                        UpdateStatus(isBackup ? "Creating backup..." : "Save in progress...");
                        Diagnostics.WriteLine("Save in progress... ");
                        SaveHelper.Regular_Save(true, false, multiThreadSave);
                        UpdateStatus(isBackup ? "Backup created!" : "Saving complete!");
                        Diagnostics.WriteLine("Done Saving!");
                        System.Threading.Thread.Sleep(250);
                        Saving_In_Process = false;
                        UpdateStatus("Ready");
                    });
                }
            }

            if (GC_List.Count <= 0 && Settings_Dictionary.ContainsKey("PERSONAL_FIRST_NAME") && Settings_Dictionary["PERSONAL_FIRST_NAME"] == "Robin")
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(this, "GC List is empty. Debug error!", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
        }

        private void toolStripMenuItem40_Click(object sender, EventArgs e)
        {
            Contacts C = new Contacts(this, this.Location, this.Size);
            C.Show();
        }

        private void toolStripMenuItem41_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Item_Summary_Spreadsheet ISS = new Item_Summary_Spreadsheet(this, this.Location, this.Size);
            ISS.ShowDialog();
            Grey_In();
        }

        private void bufferedPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void item_desc_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem42_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Expiration_Settings ES = new Expiration_Settings(this, this.Location, this.Size);
            ES.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem17_Click(object sender, EventArgs e)
        {

            if (Master_Item_List.Count < 1)
            {
                toolStripMenuItem33.Enabled = false;
                toolStripMenuItem12.Enabled = false;
                toolStripMenuItem19.Enabled = false;
                toolStripMenuItem35.Enabled = false;
                toolStripMenuItem18.Enabled = false;
                toolStripMenuItem28.Enabled = false;
            }
            else
            {
                toolStripMenuItem33.Enabled = true;
                toolStripMenuItem12.Enabled = true;
                toolStripMenuItem19.Enabled = true;
                toolStripMenuItem35.Enabled = true;
                toolStripMenuItem18.Enabled = true;
                toolStripMenuItem28.Enabled = true;
            }
        }

        private void toolStripMenuItem44_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Accounts_RP ARP = new Accounts_RP(this);
            ARP.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem45_Click(object sender, EventArgs e)
        {
            if (Monthly_Income > 0)
            {
                Savings_Helper SH = new Savings_Helper(this);
                SH.Show();
            }
            else
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(this, "Please set a personal salary before setting up personal savings", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Salary_Calculator PF = new Salary_Calculator(this, true, this.Location, this.Size);
                PF.ShowDialog();
                Grey_In();
            }
        }

        private void toolStripMenuItem46_Click(object sender, EventArgs e)
        {
            Recurring_Expenses RE = new Recurring_Expenses(this);
            RE.ShowDialog();
        }

        private void toolStripMenuItem47_Click(object sender, EventArgs e)
        {
            if (Settings_Dictionary["INCOME_MANUAL"] == "1")
            {
                Grey_Out();
                Salary_Manual SM = new Salary_Manual(this, this.Location, this.Size);
                SM.ShowDialog();
                Grey_In();
            }
            else
            {
                Grey_Out();
                Salary_Calculator PF = new Salary_Calculator(this, true, this.Location, this.Size);
                PF.ShowDialog();
                Grey_In();
            }
            Invalidate();
        }

        private void toolStripMenuItem48_Click(object sender, EventArgs e)
        {
            foreach (Payment p in Payment_List)
            {
                p.Get_Total(Master_Item_List, Tax_Rules_Dictionary, Tax_Rate, Order_List);
            }
            Payment_Information PI = new Payment_Information(this);
            PI.Show();
        }

        private void toolStripMenuItem49_Click(object sender, EventArgs e)
        {
            Grey_Out();
            GC_Manager GCM = new GC_Manager(this, false, this.Location, this.Size);
            GCM.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem50_Click(object sender, EventArgs e)
        {
            Investments I = new Investments(this, this.Location, this.Size);
            I.ShowDialog();
        }

        private void toolStripMenuItem51_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Expenditure_Warnings EW = new Expenditure_Warnings(this);
            EW.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Asset_Manager AM = new Asset_Manager(this, this.Location, this.Size);
            AM.ShowDialog();
            Grey_In();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Invalidate();
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
            this.CenterToParent();
            //this.CenterToScreen();
            this.ShowInTaskbar = true;

            if (Settings_Dictionary.ContainsKey("AUTHENTICATION_REQ") && Settings_Dictionary["AUTHENTICATION_REQ"] == "1" && isAFK)
            {
                TFLP.Opacity = 90;
                Grey_Out();
                using (var form = new Input_Box_Small(this, "Session expired." + Environment.NewLine + "Please enter login password", "", "OK", null, this.Location, this.Size, 20, true))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        string password = form.Pass_String;
                        // If authentication set already

                        if (password == Settings_Dictionary["LOGIN_PASSWORD"])
                        {
                            isAFK = false;
                            this.Show();
                            this.WindowState = FormWindowState.Normal;
                            notifyIcon1.Visible = false;
                            //this.CenterToScreen();
                            this.ShowInTaskbar = true;
                        }
                        else
                        {
                            Form_Message_Box FMB = new Form_Message_Box(this, "Invalid Password Entered", true, -30, this.Location, this.Size);
                            FMB.ShowDialog();

                            this.WindowState = FormWindowState.Minimized;

                        }
                    }
                    else
                    {
                        this.WindowState = FormWindowState.Minimized;

                    }
                }
                Grey_In();
                TFLP.Opacity = 75;
            }
            else
            {
            }
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            Grey_Out();
            SMS_Alert SA = new SMS_Alert(this, this.Location, this.Size);
            SA.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem36_Click(object sender, EventArgs e)
        {

        }

        private void toolStripMenuItem15_Click_1(object sender, EventArgs e)
        {
            Grey_Out();
            Refund_Settings RS = new Refund_Settings(this, this.Location, this.Size);
            RS.ShowDialog();
            Grey_In();
        }

        private void spendingLabel_Click(object sender, EventArgs e)
        {
            if (spendingLabel.Text.ToLower().Contains("refund") && spendingLabel.Text.ToLower().Contains("reminder"))
            {
                Grey_Out();
                Receipt_Report RP = new Receipt_Report(this, Order_List.First(x => x.OrderID == currentStatusBarOrder), new Point(this.Left + 300, this.Top + 40), null, true, this.Location, this.Size);
                RP.ShowDialog();
                Grey_In();
            }
            else if (spendingLabel.Text.ToLower().Contains("exp. delivery"))
            {
                Grey_Out();
                Receipt_Report RP = new Receipt_Report(this, Order_List.First(x => x.OrderID == currentTrackingOrder), new Point(this.Left + 300, this.Top + 40), null, true, this.Location, this.Size);
                RP.ShowDialog();
                Grey_In();
            }
        }

        private void toolStripMenuItem19_Click(object sender, EventArgs e)
        {
            Categorical_Visualizer CV = new Categorical_Visualizer(this, this.Location, this.Size);
            CV.Show();
        }

        private void Cloud_Save(bool runAsync = true)
        {

            if (Settings_Dictionary["LOGIN_EMAIL"].Length > 3 && !Saving_In_Process && Settings_Dictionary["UNIQUE_IDENTIFIER"].Length == 9)
            {
                if (runAsync)
                {
                    Task.Run(() =>
                    {
                        Saving_In_Process = true;
                        UpdateStatus("Syncing to cloud...");
                        Settings_Dictionary["CLOUD_SYNC_TIME"] = DateTime.Now.ToString();
                        SaveHelper.Regular_Save(true, true);
                        UpdateStatus("Sync complete!"); ;
                        System.Threading.Thread.Sleep(1500);
                        Saving_In_Process = false;
                        UpdateStatus("Ready");
                    });
                }
                else
                {
                    Settings_Dictionary["CLOUD_SYNC_TIME"] = DateTime.Now.ToString();
                    SaveHelper.Regular_Save(true, true);
                }
            }
            else if (Settings_Dictionary["UNIQUE_IDENTIFIER"].Length != 9)
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(this, "Cloud Key invalid", true, -10, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
            else if (Settings_Dictionary["LOGIN_EMAIL"].Length < 3)
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(this, "No email found on record", true, -30, this.Location, this.Size);
                FMB.ShowDialog();

                // Show email record page
                Security S = new Security(this, this.Location, this.Size);
                S.ShowDialog();
                Grey_In();
            }
            else
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(this, "Application not ready. Please wait a few seconds", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
        }

        private bool Load_From_Cloud(bool showSuccessfulLoad = true)
        {
            if (Settings_Dictionary["LOGIN_EMAIL"].Length > 3 && Settings_Dictionary["UNIQUE_IDENTIFIER"].Length == 9)
            {
                try
                {
                    Grey_Out();
                    Application.DoEvents();

                    Cursor.Current = Cursors.WaitCursor;

                    if (secondThreadFormHandle == IntPtr.Zero)
                    {

                        Loading_Form form = new Loading_Form(this, new Point(this.Location.X, this.Location.Y), this.Size, "RETRIEVING", "FROM CLOUD")
                        {
                        };
                        form.HandleCreated += SecondFormHandleCreated;
                        form.HandleDestroyed += SecondFormHandleDestroyed;
                        form.RunInNewThread(false);
                    }
                    
                    string ftpPath = @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/Cloud_Sync/" + Settings_Dictionary["LOGIN_EMAIL"] + "_" + Settings_Dictionary["UNIQUE_IDENTIFIER"] + ".cfg";

                    if (Cloud_Services.FTP_Check_File_Exists(ftpPath))
                    {
                        if (!(Load_Information(ftpPath, true))) return false;

                        Reload_Program("", false, true);

                        if (secondThreadFormHandle != IntPtr.Zero)
                            PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

                        if (showSuccessfulLoad)
                        {
                            Form_Message_Box FMB2 = new Form_Message_Box(this, "Profile successfully loaded from the Cloud!", true, -10, this.Location, this.Size);
                            FMB2.ShowDialog();
                        }

                    }
                    else
                    {
                        if (secondThreadFormHandle != IntPtr.Zero)
                            PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

                        Form_Message_Box FMB = new Form_Message_Box(this, "No profile found for existing email/cloud key", true, -10, this.Location, this.Size);
                        FMB.ShowDialog();
                    }

                    Grey_In();
                }
                catch
                {
                }
            }
            else if (Settings_Dictionary["UNIQUE_IDENTIFIER"].Length != 9)
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(this, "Cloud Key invalid", true, -10, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
            else
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(this, "No email found on record", true, -30, this.Location, this.Size);
                FMB.ShowDialog();

                // Show email record page
                Security S = new Security(this, this.Location, this.Size);
                S.ShowDialog();

                Grey_In();
            }
            
            return true;
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

        #region Rounded Rectangle
        public void DrawRoundedRectangle(Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (pen == null)
                throw new ArgumentNullException("pen");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }


        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
        #endregion

        private void toolStripMenuItem22_Click_2(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Yes_No_Dialog(this, "Are you sure you wish to load cloud profile? This will overwrite your existing profile", "Warning", "No", "Yes", 15, this.Location, this.Size))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (form.ReturnValue1 == "1")
                    {
                        ftpWorkerWorking = true;
                        Load_From_Cloud();

                        #region Set status spending info
                        StatusSetSpending(false, true);
                        #endregion

                        ftpWorkerWorking = false;

                        if (secondThreadFormHandle != IntPtr.Zero)
                            PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    }
                }
            }
            Grey_In();


        }

        private void toolStripMenuItem20_Click_3(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Yes_No_Dialog(this, "Are you sure you wish to save profile to cloud? This will overwrite your existing profile", "Warning", "No", "Yes", 15, this.Location, this.Size))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (form.ReturnValue1 == "1")
                    {
                        Cloud_Save();
                    }
                }
            }
            Grey_In();
        }

        private void toolStripMenuItem39_Click(object sender, EventArgs e)
        {
            Grey_Out();
            if (Settings_Dictionary.ContainsKey("LOGIN_EMAIL") && Settings_Dictionary["LOGIN_EMAIL"].Length > 5)
            {
                Cloud_Settings CS = new Cloud_Settings(this, this.Location, this.Size);
                CS.ShowDialog();
            }
            else
            {
                Form_Message_Box FMB = new Form_Message_Box(this, "You must setup email authentication for cloud features", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Security S = new Security(this, this.Location, this.Size);
                S.ShowDialog();
            }
            Grey_In();
        }

        private void toolStripMenuItem35_Click_1(object sender, EventArgs e)
        {
            Grey_Out();
            Item_Timeline GV = new Item_Timeline(this, this.Location, this.Size); 
            GV.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem53_Click(object sender, EventArgs e)
        {
            ManualMobileSync();
        }

        private void ManualMobileSync()
        {
            if (Payment_List.Count > 0)
            {
                MobileSync.currentlyChecking = false;

                Grey_Out();
                Application.DoEvents();

                Cursor.Current = Cursors.WaitCursor;

                if (secondThreadFormHandle == IntPtr.Zero)
                {

                    Loading_Form form = new Loading_Form(this, new Point(this.Location.X, this.Location.Y), this.Size,
                        "SYNCHRONIZING", "WITH MOBILE", 11);
                    form.HandleCreated += SecondFormHandleCreated;
                    form.HandleDestroyed += SecondFormHandleDestroyed;
                    form.RunInNewThread(false);
                }

                //Thread.Sleep(2000);

                bool hasSyncFile = MobileSync.CheckForSyncFiles();

                if (secondThreadFormHandle != IntPtr.Zero)
                    PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

                if (hasSyncFile && MobileSync.SyncedOrders.Count > 0)
                {
                    Grey_Out();
                    MobileSyncDialog MSD = new MobileSyncDialog(this, MobileSync.SyncedOrders, MobileSync.SyncedItems,
                        this.Location, this.Size);
                    MSD.ShowDialog();
                    Grey_In();
                }
                else if (!hasSyncFile)
                {
                    Grey_Out();
                    Form_Message_Box FMB2 =
                        new Form_Message_Box(this, "Error connecting to server. Please try again later", true, -10, this.Location, this.Size);
                    FMB2.ShowDialog();
                }
                else if (MobileSync.SyncedOrders.Count == 0)
                {
                    Grey_Out();
                    Form_Message_Box FMB2 =
                        new Form_Message_Box(this, "You do not have any orders waiting to be synced", true, 0,
                            this.Location, this.Size);
                    FMB2.ShowDialog();
                }

                Grey_In();
            }
            else
            {
                Grey_Out();
                Form_Message_Box FMB2 =
                    new Form_Message_Box(this, "You do not have any payments set up. You must set one up before syncing", true, 0,
                        this.Location, this.Size);
                FMB2.ShowDialog();
                Payment_Information PI = new Payment_Information(this);
                PI.Show();
                Grey_In();
            }

            Activate();
        }

        private void toolStripMenuItem54_Click(object sender, EventArgs e)
        {
            Grey_Out();
            SyncSettings SS = new SyncSettings(this, Location, Size);
            SS.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem55_Click(object sender, EventArgs e)
        {
            Grey_Out();
            ManageAssociations MA = new ManageAssociations(this, Location, Size);
            MA.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem57_Click(object sender, EventArgs e)
        {
            Grey_Out();
            CategoryGrouper CG = new CategoryGrouper(this, Location, Size);
            CG.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem58_Click(object sender, EventArgs e)
        {
            Grey_Out();
            CustomCategorySpreadsheet CG = new CustomCategorySpreadsheet(this, Location, Size);
            CG.ShowDialog();
            Grey_In();
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            Grey_Out();
            BudgetAllocation CG = new BudgetAllocation(this, Location, Size);
            CG.ShowDialog();
            Grey_In();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Grey_Out();
            OrderEntrySettings OES = new OrderEntrySettings(this, Location, Size);
            OES.ShowDialog();
            Grey_In();
        }

    }

    public class AdvancedComboBox : ComboBox
    {
        new public System.Windows.Forms.DrawMode DrawMode { get; set; }
        public Color HighlightColor { get; set; }
        public Color FrameColor { get; set; }

        public AdvancedComboBox()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.FrameColor = SystemColors.HotTrack;
            base.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.HighlightColor = Color.Gray;
            this.DrawItem += new DrawItemEventHandler(AdvancedComboBox_DrawItem);
        }

        void AdvancedComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;


            ComboBox combo = sender as ComboBox;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e.Graphics.FillRectangle(new SolidBrush(HighlightColor),
                                         e.Bounds);
            else
                e.Graphics.FillRectangle(new SolidBrush(combo.BackColor),
                                         e.Bounds);

            e.Graphics.DrawString(combo.Items[e.Index].ToString(), e.Font,
                                  new SolidBrush(combo.ForeColor),
                                  new Point(e.Bounds.X, e.Bounds.Y));
            
            //e.Graphics.DrawString(combo.Items[e.Index].ToString(), e.Font, brush, e.Bounds, new StringFormat(StringFormatFlags.DirectionRightToLeft));

            //e.Graphics.DrawRectangle(new Pen(FrameColor, 2), 0, 0,
            //  this.Width - 2, this.Items.Count * 20);

            // Draw the rectangle around the drop-down list
            //if (combo.DroppedDown)
            if (false)
            {
                SolidBrush ArrowBrush = new SolidBrush(SystemColors.HighlightText);

                Rectangle dropDownBounds = new Rectangle(0, 0, combo.Width-2, combo.Items.Count*combo.ItemHeight);
                //ControlPaint.DrawBorder(g, dropDownBounds, _borderColor, _borderStyle);
                ControlPaint.DrawBorder(e.Graphics, dropDownBounds,
                    FrameColor, 1, ButtonBorderStyle.Solid,
                    FrameColor, 1, ButtonBorderStyle.Solid,
                    FrameColor, 1, ButtonBorderStyle.Solid,
                    FrameColor, 1, ButtonBorderStyle.Solid);
            }
            e.DrawFocusRectangle();
        }

    }

    // Custom Menustrip 
    public class TestColorTable : ProfessionalColorTable
    {
        //private Color Menu_Bar_Color = Color.FromArgb(76, 76, 76);
        public Color Menu_Bar_Color = Color.FromArgb(64, 64, 64);
        public Color Highlight_Menu_Color = Color.FromArgb(80, 80, 80);
        public Color Menu_Border_Color = SystemColors.HotTrack;
        //public Color Menu_Border_Color = SystemColors.Control;

        public void Set_Menu_Color(Color color)
        {
            Menu_Border_Color = color;
        }

        public override Color MenuItemSelected
        {
            get { return Highlight_Menu_Color; }
        }

        public override Color MenuItemBorder
        {
            get { return Menu_Bar_Color; }
        }

        public override Color MenuBorder  //added for changing the menu border
        {
            get { return Menu_Bar_Color; }
        }

        public override Color MenuStripGradientBegin
        {
            get { return Menu_Bar_Color; }
        }

        public override Color MenuStripGradientEnd
        {
            get { return Menu_Bar_Color; }
        }

        public override Color MenuItemSelectedGradientBegin
        {
            get { return Highlight_Menu_Color; }
        }

        public override Color MenuItemSelectedGradientEnd
        {
            get { return Highlight_Menu_Color; }
        }

        public override Color ToolStripDropDownBackground
        {
            get { return Menu_Border_Color; }
        }

        public override Color ImageMarginGradientBegin
        {
            get { return Menu_Border_Color; }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get { return Highlight_Menu_Color; }
        }

        public override Color MenuItemPressedGradientEnd
        {
            get { return Highlight_Menu_Color; }
        }

        public override Color CheckSelectedBackground
        {
            get { return Highlight_Menu_Color; }
        }

        public override Color ButtonSelectedHighlight
        {
            get { return Highlight_Menu_Color; }
        }

        public override Color ButtonSelectedHighlightBorder
        {
            get { return Highlight_Menu_Color; }
        }
    }

    public class NoFocusCueButton : Button
    {
        public NoFocusCueButton() : base()
        {
            //InitializeComponent();

            this.SetStyle(ControlStyles.Selectable, false);
        }

        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
    }

    public static class ISynchronizeInvokeExtensions
    {
        public static void InvokeEx<T>(this T @this, Action<T> action) where T : ISynchronizeInvoke
        {
            if (@this.InvokeRequired)
            {
                @this.Invoke(action, new object[] { @this });
            }
            else
            {
                action(@this);
            }
        }
    }

    
}
