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
    public partial class Item_Timeline : Form
    {
        private void Rearrange_Controls()
        {
            // Reorganize controls (Anchoring not working)
            textBox1.Location = new Point(this.Size.Width - 1, 0);
            textBox4.Location = new Point(0, this.Size.Height - 1);
            shoppingButton.Left = this.Width - shoppingButton.Width - 20;
            label12.Left = this.Width - label12.Width - 6;
            label11.Left = this.Width - label11.Width - 21;
            expirationButton.Left = this.Width - expirationButton.Width - 87;
            label14.Left = this.Width - label14.Width - 84;
            label13.Left = this.Width - label13.Width - 82;
            close_button.Left = this.Width - 19;
            bufferedPanel2.Top = this.Height - bufferedPanel2.Height + 2;
            showWeekend.Top = this.Height - bufferedPanel2.Height + 1;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        int tickCount = 0;
        public DateTime startDate;

        // Converting month number to name
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        private void Set_Tick_Count(DateTime startDate, DateTime endDate)
        {
            tickCount = Math.Abs((int)(endDate - startDate).TotalDays);

            if (tickCount == 0) tickCount = 1;
        }

        private DateTime getDateTime(int tickCount)
        {
            return startDate.AddDays(tickCount);
        }

        public string getDateString(int tickCount)
        {
            DateTime temp = startDate.AddDays(tickCount);
            return mfi.GetMonthName(temp.Month);// +" " + temp.Day + ", " + temp.Year;
        }

        bool paint = true;
        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;


        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x84)
            {  // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;  // HTCAPTION
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }
            }
            else if (m.Msg == 0x0112) // WM_SYSCOMMAND (ON MAXIMIZE)
            {
                // Check your window state here
                if (m.WParam == new IntPtr(0xF030)) // Maximize event - SC_MAXIMIZE from Winuser.h
                {
                }
            }

            base.WndProc(ref m);
        }

        private List<Item> Grocery_List = new List<Item>();

        private void Populate_Grocery_List()
        {
            Grocery_List = new List<Item>();
            // grab groceries in categories and current to date
            Grocery_List = parent.Master_Item_List.Where(x => cmbManual.Text.Contains(x.Category) && x.Date >= startDate && x.Get_Current_Quantity() > 0).ToList();
        }

        int sizeValue = 0;

        List<Label> Label_List = new List<Label>();
        List<ToolTip> ToolTip_List = new List<ToolTip>();

        private bool isLabelColliding(Rectangle bounds, string debugStr = "")
        {
            foreach (Label l in Label_List)
            {
                if (Rectangle.Intersect(l.Bounds, bounds) != Rectangle.Empty) //intersect = area of overlap
                {
                    return true; //has intersect
                }
            }
            return false;
        }

        int expirationCount = 0;

        protected override void OnPaint(PaintEventArgs e)
        {
            #region Grip
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
            rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
            #endregion

            Label_List.ForEach(x => this.Controls.Remove(x));
            Label_List.ForEach(x => x.Dispose());
            Label_List = new List<Label>();
            ToolTip_List.ForEach(x => x.Dispose());
            ToolTip_List = new List<ToolTip>();

            //Calculate_Months();
            ///Get_Line_Plots();
            Set_Tick_Count(DateTime.Now, startDate);

            Color DrawForeColor = Color.White;
            Color HighlightColor = Color.FromArgb(76, 76, 76);

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(122, 122, 122));
            SolidBrush BlueBrush = new SolidBrush(Color.LightBlue);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            SolidBrush PurpleBrush = new SolidBrush(Color.MediumPurple);
            SolidBrush RedBrush = new SolidBrush(Color.LightPink);
            SolidBrush OrangeBrush = new SolidBrush(Color.Orange);
            SolidBrush LightOrangeBrush = new SolidBrush(Color.FromArgb(255, 200, 0));

            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 1);
            Pen Blue_Pen = new Pen(BlueBrush, 1);
            Pen Green_Pen = new Pen(GreenBrush, 1);
            Pen Red_Pen = new Pen(RedBrush, 1);
            Pen Orange_Pen = new Pen(OrangeBrush, 1);
            Pen Purple_Pen = new Pen(PurpleBrush, 1);

            Font graph_font = new Font("MS Reference Sans Serif", 8, FontStyle.Regular);
            Font axis_font = new Font("MS Reference Sans Serif", sizeValue, FontStyle.Regular);
            Font axis_font_strikeout = new Font("MS Reference Sans Serif", sizeValue, FontStyle.Strikeout);
            Font axis_font_bold = new Font("MS Reference Sans Serif", sizeValue, FontStyle.Bold);

            int start_margin = 40;
            int end_margin = this.Width - 2 * start_margin;
            int timeLineAxisHeight = this.Height / 9 * 5;

            int itemDataBaseHeight = 45;
            int itemDataHeight = 11 + (sizeValue == 8 ? 2 : sizeValue == 7 ? 1 : 0); 

            int daysWithGroceries = 0;

            int lineLength = end_margin - start_margin;
            int tickLength = lineLength / tickCount;

            int tickHeight = 5;

            expirationCount = 0;

            // If has order
            if (true)
            {
                List<DateTime> upperDateItems = new List<DateTime>();
                List<DateTime> lowerDateItems = new List<DateTime>();

                for (int i = 0; i < tickCount + 1; i++)
                {
                    DateTime refDate = getDateTime(i);
                    int currentXValue = start_margin + (i * tickLength);
                    int direction = 1 * ((daysWithGroceries % 2 == 0) ? 1 : -1 );

                    // force base height based on proximity to other registered item dates

                    /*
                    if (viewPeriod.Text != "Current Month")
                    {
                        if (daysWithGroceries % 2 == 0 && lowerDateItems.Count > 0)
                        {
                            itemDataBaseHeight = Check_Date_Proximity(lowerDateItems, refDate);
                        }
                        else if (daysWithGroceries % 2 == 1 && upperDateItems.Count > 0)
                        {
                            itemDataBaseHeight = Check_Date_Proximity(upperDateItems, refDate);
                        }
                    }\
                    */

                    //get current groceries for this date
                    List<Item> tempGroceryList = Grocery_List.Where(x => x.Date.Date == refDate.Date).ToList().OrderBy(x => x.Name).ToList();

                    itemDataBaseHeight = 45;

                    if (tempGroceryList.Count > 0)
                    {

                        // +-2 is the invisible margin to check overlap
                        Rectangle invisibleBound = new Rectangle(currentXValue - 3, timeLineAxisHeight + direction * (itemDataBaseHeight) - itemDataHeight, 20, itemDataHeight + 2);
                        
                        // Keep iterating while collision exists
                        while (isLabelColliding(invisibleBound, tempGroceryList[tempGroceryList.Count - 1].Name))
                        {
                            itemDataBaseHeight += itemDataHeight * 2 + 20;
                            invisibleBound = new Rectangle(currentXValue, timeLineAxisHeight + direction * (itemDataBaseHeight) - 2, 1, itemDataHeight + 2);
                        }

                        e.Graphics.DrawLine(Grey_Pen, currentXValue, timeLineAxisHeight, currentXValue, timeLineAxisHeight + direction * (itemDataBaseHeight + (itemDataHeight * tempGroceryList.Count))); //vertical guider
                        e.Graphics.DrawLine(Grey_Pen, currentXValue, timeLineAxisHeight + direction * (itemDataBaseHeight + (itemDataHeight * tempGroceryList.Count)), currentXValue + 15, timeLineAxisHeight + direction * (itemDataBaseHeight + (itemDataHeight * tempGroceryList.Count))); //horizontal guider
                        e.Graphics.DrawString(refDate.ToShortDateString().Substring(0, refDate.ToShortDateString().Length - 5), axis_font, GreyBrush, currentXValue + 18, timeLineAxisHeight + direction * (itemDataBaseHeight + (itemDataHeight * tempGroceryList.Count)) - 5);

                        for (int j = 0; j < tempGroceryList.Count; j++)
                        {

                            string expirationStr = "";

                            // region check for expiration and append
                            if (checkExpiration)
                            {
                                int DaysTillExpiry = Check_For_Expiration(tempGroceryList[j], DateTime.Now);
                                if (DaysTillExpiry > -1000)
                                {
                                    if (DaysTillExpiry > 0)
                                        expirationStr = " - Expiring in " + DaysTillExpiry + " day(s)";
                                    else if (DaysTillExpiry < 0)
                                        expirationStr = " - Expired for " + DaysTillExpiry * -1 + " day(s)";
                                    else
                                        expirationStr = " - Expiring TODAY";
                                    expirationCount++;
                                }
                            }

                            Label label = new Label()
                            {
                                BorderStyle = expirationStr.Length > 0 ? BorderStyle.Fixed3D : BorderStyle.None,
                                //BackColor = expirationStr.Length > 0 ? Color.LightYellow : Color.FromArgb(64, 64, 64),
                                Name = i.ToString() + "_" + j.ToString(),
                                Text = tempGroceryList[j].Name + ((tempGroceryList[j].Get_Current_Quantity() > 1) ? "(" + tempGroceryList[j].Get_Current_Quantity() + ")" : "") + expirationStr,
                                Location = new Point(currentXValue + 2, (direction < 0 ? -12 : 0) + timeLineAxisHeight + direction * (itemDataBaseHeight - 12 + (itemDataHeight * j))),
                                Font = tempGroceryList[j].consumedStatus == 0 ? axis_font_strikeout : axis_font,
                                ForeColor = tempGroceryList[j].consumedStatus == 0 ? label3.ForeColor : tempGroceryList[j].consumedStatus == 1 ?label4.ForeColor : p.Color,
                                Size = new Size(1, itemDataHeight - 3),
                                Margin = new Padding(0, 0, 0, 0),
                                Padding = new Padding(0, 0, 0, 0),
                                AutoSize = true
                            };
                            
                            #region Set Tooltip for label
                            ToolTip ToolTip1 = new ToolTip();
                            ToolTip1.InitialDelay = 1;
                            ToolTip1.ReshowDelay = 1;
                            ToolTip1.SetToolTip(label, tempGroceryList[j].Location + " ($" + (String.Format("{0:0.00}", tempGroceryList[j].Price)) + ")");
                            ToolTip_List.Add(ToolTip1);
                            #endregion


                            label.MouseClick += label_MouseClick;
                            label.MouseEnter += OnMouseEnter;
                            label.MouseLeave += OnMouseLeave;
                            label.SendToBack();
                            Label_List.Add(label);
                            this.Controls.Add(label);
                        }

                        /*
                        if (daysWithGroceries % 2 == 0)
                        {
                            lowerDateItems.Add(refDate);
                        }
                        else
                        {
                            upperDateItems.Add(refDate);
                        }
                        */

                        daysWithGroceries++; //increment days with grocery (to alternate north and south addressing of labels)
                    }

                    // draw ticks
                    tickHeight = (refDate.Day == 1 || (showWeekend.Checked && (refDate.DayOfWeek == DayOfWeek.Saturday || refDate.DayOfWeek == DayOfWeek.Sunday))) ? 8 : tempGroceryList.Count > 0 ? 5 : 2; // longer ticks for new month
                    e.Graphics.DrawLine((refDate.Day == 1 ? Green_Pen : (showWeekend.Checked && (refDate.DayOfWeek == DayOfWeek.Saturday || refDate.DayOfWeek == DayOfWeek.Sunday)) ? Orange_Pen : p), currentXValue, timeLineAxisHeight - tickHeight, currentXValue, timeLineAxisHeight + tickHeight);

                    // Mark month for new month only
                    if (refDate.Day == 1)
                    {
                        // Draw axis label at end (to ensure top-most behaviour)
                        SizeF size = TextRenderer.MeasureText(getDateString(i), graph_font);

                        DrawDiagonalString(e.Graphics,
                            getDateString(i),
                            axis_font, WritingBrush,
                            new Point((int)(currentXValue) - 6, timeLineAxisHeight + 8),
                            40
                            );
                    }
                }

                // Draw X-axis
                e.Graphics.DrawLine(p, start_margin, timeLineAxisHeight, start_margin + ((tickCount)* tickLength), timeLineAxisHeight);
                e.Graphics.DrawString("Today", axis_font, WritingBrush, start_margin + ((tickCount) * tickLength) + 3, timeLineAxisHeight - 5);


                paint = false;
            }

            // Dispose all objects
            p.Dispose();
            Grey_Pen.Dispose();
            GreyBrush.Dispose();
            BlueBrush.Dispose();
            RedBrush.Dispose();
            GreenBrush.Dispose();
            PurpleBrush.Dispose();
            OrangeBrush.Dispose();
            LightOrangeBrush.Dispose();
            Blue_Pen.Dispose();
            Green_Pen.Dispose();
            Red_Pen.Dispose();
            Purple_Pen.Dispose();
            Orange_Pen.Dispose();
            WritingBrush.Dispose();
            graph_font.Dispose();
            axis_font.Dispose();
            axis_font_strikeout.Dispose();
            axis_font_bold.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        /// <summary>
        /// Compare a given date to a dateList and return whether or not date is within proximityMargin
        /// </summary>
        /// <param name="refDateList"></param>
        /// <param name="refDate"></param>
        /// <param name="proximityMargin"></param>
        /// <returns></returns>
        /// 
        /*
        private int Check_Date_Proximity(List<DateTime> refDateList, DateTime refDate, int proximityMargin = 3)
        {
            List<DateTime> magicList = refDateList;//.Where(x => x.Date >= refDate.AddDays(-proximityMargin * 2)).ToList();

            // Check for 2 consecutive dates
            if (refDateList.Count >= 2 && refDateList.Where(x => x.Date >= refDate.AddDays(-proximityMargin * 2)).ToList().Count >= 2)
            {
                magicList.ForEach(x => Console.Write(x.ToShortDateString() + ", "));
                return 100;
            }

            // Get closest day available aka LAST day on the refDateList
            if (Math.Round((decimal)Math.Abs((refDateList[refDateList.Count - 1] - refDate).TotalDays)) <= proximityMargin)
            {
                return 150;
            }

            return 45;
        }*/

        private void label_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Label label1 = (Label)sender;
                string[] labelDefinition = label1.Name.Split(new string[] { "_" }, StringSplitOptions.None);
                DateTime refDate = getDateTime(Convert.ToInt32(labelDefinition[0]));//get current groceries for this date
                List<Item> tempGroceryList = Grocery_List.Where(x => x.Date.Date == getDateTime(Convert.ToInt32(labelDefinition[0])).Date).ToList().OrderBy(x => x.Name).ToList();
                Item refItem = tempGroceryList[Convert.ToInt32(labelDefinition[1])];
                if (refItem.consumedStatus != 0)
                {
                    refItem.consumedStatus = 0;
                }
                else
                {
                    refItem.consumedStatus = 2;
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                Label label1 = (Label)sender;
                string[] labelDefinition = label1.Name.Split(new string[] { "_" }, StringSplitOptions.None);
                DateTime refDate = getDateTime(Convert.ToInt32(labelDefinition[0]));//get current groceries for this date
                List<Item> tempGroceryList = Grocery_List.Where(x => x.Date.Date == getDateTime(Convert.ToInt32(labelDefinition[0])).Date).ToList().OrderBy(x => x.Name).ToList();
                Item refItem = tempGroceryList[Convert.ToInt32(labelDefinition[1])];
                if (refItem.consumedStatus != 1)
                {
                    refItem.consumedStatus = 1;
                }
                else
                {
                    refItem.consumedStatus = 2;
                }
            }
            Invalidate();
            Update();
        }

        bool Resizing = false;

        private void Calendar_Resize(object sender, EventArgs e)
        {
            Resizing = true;
            SuspendLayout();
            textBox1.Visible = textBox4.Visible = false;
            Grey_Out();
        }

        private void Calendar_Resize_End(object sender, EventArgs e)
        {
            if (Resizing)
            {
                Grey_In();
                TFLP.Size = new Size(this.Width - 2, this.Height - 2);
                textBox1.Visible = textBox4.Visible = true;
                Resizing = false;
                ResumeLayout();
                Rearrange_Controls();
            }
        }

        private void OnMouseEnter(object sender, EventArgs e)
        {
            Label label1 = (Label)sender;
            label1.BackColor = Color.FromArgb(78, 78, 78);
            //label1.Font = new Font(label1.Font.Name, label1.Font.SizeInPoints, FontStyle.Underline);
        }

        private void OnMouseLeave(object sender, EventArgs e)
        {
            Label label1 = (Label)sender;
            label1.BackColor = Color.FromArgb(64, 64, 64);
            //label1.Font = new Font(label1.Font.Name, label1.Font.SizeInPoints, FontStyle.Regular);
        }

        Receipt parent;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Item_Timeline(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            ToolTip1.SetToolTip(group_all, "Select All");
            ToolTip1.SetToolTip(reset, "Deselect All");

            startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            // resize listeners
            this.Resize += Calendar_Resize;
            this.ResizeEnd += Calendar_Resize_End;

            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            #region Fade Box
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
            #endregion

            #region Setup viewPeriod box
            viewPeriod.Items.Add("Current Month");
            viewPeriod.Items.Add("Last 2 Months");
            viewPeriod.Items.Add("Last 3 Months");
            viewPeriod.Items.Add("Last 4 Months");
            viewPeriod.Items.Add("Last 5 Months");
            viewPeriod.Items.Add("Last 6 Months");

            viewPeriod.Text = "Current Month";

            DateTime currentDate = DateTime.Now;
            startDate = new DateTime(
                viewPeriod.Text == "Current Month" ? currentDate.Year :
                viewPeriod.Text == "Last 2 Months" ? currentDate.AddMonths(-1).Year :
                viewPeriod.Text == "Last 3 Months" ? currentDate.AddMonths(-2).Year :       //year
                viewPeriod.Text == "Last 4 Months" ? currentDate.AddMonths(-3).Year : 
                viewPeriod.Text == "Last 5 Months" ? currentDate.AddMonths(-4).Year :       //year
                viewPeriod.Text == "Last 6 Months" ? currentDate.AddMonths(-5).Year : 2017
                ,
                viewPeriod.Text == "Current Month" ? currentDate.Month :
                viewPeriod.Text == "Last 2 Months" ? currentDate.AddMonths(-1).Month :
                viewPeriod.Text == "Last 3 Months" ? currentDate.AddMonths(-2).Month :      //month
                viewPeriod.Text == "Last 4 Months" ? currentDate.AddMonths(-3).Month : 
                viewPeriod.Text == "Last 5 Months" ? currentDate.AddMonths(-4).Month :      //month
                viewPeriod.Text == "Last 6 Months" ? currentDate.AddMonths(-5).Month : 2017
                ,
                1); //day

            #endregion

            #region Populate cmbManual list
            cmbManual.SelectedIndexChanged -= cmbManual_SelectedIndexChanged;

            // Add category options
            foreach (string category in parent.category_box.Items)
            {
                cmbManual.Items.Add(category.ToString());
            }

            // Check previously selected items
            for (int i = 0; i < cmbManual.Items.Count; i++)
            {
                cmbManual.CheckBoxItems[i].Checked = parent.Settings_Dictionary["GROCERY_CATEGORIES"].Contains(cmbManual.CheckBoxItems[i].Text);
            }

            cmbManual.SelectedIndexChanged += cmbManual_SelectedIndexChanged;
            #endregion

            Populate_Grocery_List();

            sizeValue = trackBar3.Value;
        }

        FadeControl TFLP;

        private void Grey_Out()
        {
            TFLP.Size = new Size(this.Width - 2, this.Height - 2);
            TFLP.Location = new Point(1, 1);
        }

        private void Grey_In()
        {
            TFLP.Location = new Point(10000, 10000);
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
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        private void cmbManual_SelectedIndexChanged(object sender, EventArgs e)
        {
            paint = true;
            parent.Settings_Dictionary["GROCERY_CATEGORIES"] = cmbManual.Text;

            Populate_Grocery_List();
            Invalidate();
        }

        private void viewPeriod_SelectedIndexChanged(object sender, EventArgs e)
        {
            paint = true;

            DateTime currentDate = DateTime.Now;
            startDate = new DateTime(
                viewPeriod.Text == "Current Month" ? currentDate.Year :
                viewPeriod.Text == "Last 2 Months" ? currentDate.AddMonths(-1).Year :
                viewPeriod.Text == "Last 3 Months" ? currentDate.AddMonths(-2).Year :       //year
                viewPeriod.Text == "Last 4 Months" ? currentDate.AddMonths(-3).Year :
                viewPeriod.Text == "Last 5 Months" ? currentDate.AddMonths(-4).Year :       //year
                viewPeriod.Text == "Last 6 Months" ? currentDate.AddMonths(-5).Year : 2017
                ,
                viewPeriod.Text == "Current Month" ? currentDate.Month :
                viewPeriod.Text == "Last 2 Months" ? currentDate.AddMonths(-1).Month :
                viewPeriod.Text == "Last 3 Months" ? currentDate.AddMonths(-2).Month :      //month
                viewPeriod.Text == "Last 4 Months" ? currentDate.AddMonths(-3).Month :
                viewPeriod.Text == "Last 5 Months" ? currentDate.AddMonths(-4).Month :      //month
                viewPeriod.Text == "Last 6 Months" ? currentDate.AddMonths(-5).Month : 2017
                ,
                1); //day


            Populate_Grocery_List();
            Invalidate();
        }

        public void DrawDiagonalString(Graphics G, string S, Font F, Brush B, PointF P, int Angle)
        {

            SizeF MySize = TextRenderer.MeasureText(S, F);
            float Extra_Height = 0;
            Extra_Height += (S.Length >= 10 ? (S.Length >= 18 ? MySize.Width / 6 : MySize.Width / 7) : 8);
            //SizeF MySize = G.MeasureString(S, F);
            G.TranslateTransform(P.X + MySize.Width / 2, P.Y + MySize.Height / 2 + Extra_Height);
            G.RotateTransform(Angle);
            G.DrawString(S, F, B, new PointF(-MySize.Width / 2, -MySize.Height / 2));
            G.RotateTransform(-Angle);
            G.TranslateTransform(-P.X - MySize.Width / 2, -P.Y - MySize.Height / 2 - Extra_Height);
        }

        private void reset_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cmbManual.Items.Count; i++)
            {
                cmbManual.CheckBoxItems[i].Checked = false;
            }

            Populate_Grocery_List();
            Invalidate();
            Update();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            sizeValue = trackBar3.Value;
            Invalidate();
            Update();
        }

        private void excel_button_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Timeline_Shopping_List TSL = new Timeline_Shopping_List(parent, this.Location, this.Size);
            TSL.ShowDialog();
            Grey_In();
        }

        private void group_all_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cmbManual.Items.Count; i++)
            {
                cmbManual.CheckBoxItems[i].Checked = true;
            }
        }

        private void showWeekend_CheckedChanged(object sender, EventArgs e)
        {
            showWeekend.ForeColor = (showWeekend.Checked ? Color.Orange : SystemColors.ButtonHighlight);
            Invalidate();
            Update();
        }

        private int Check_For_Expiration(Item item, DateTime refDate)
        {
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

            foreach (Expiration_Entry EE in parent.Expiration_List)
            {
                // Only compare same item
                if (item.Name.Trim().ToLower().Contains(EE.Item_Name.Trim().ToLower()) && item.Name.Trim().ToLower().Length == EE.Item_Name.Trim().ToLower().Length && (EE.Location == "General_Expiration" || (EE.Location != "General_Expiration" && EE.Location == item.Location)))
                {
                    int Days_Till_Expiry = (int)Math.Round((item.Date.AddDays(EE.Exp_Date_Count).Date - refDate.Date).TotalDays);
                    return Days_Till_Expiry;
                }
            }

            return -9999999;
            #endregion
        }

        bool checkExpiration = false;

        private void expirationButton_Click(object sender, EventArgs e)
        {
            checkExpiration = !checkExpiration;
            expirationButton.Image = checkExpiration ? Financial_Journal.Properties.Resources.hourglassgreen : Financial_Journal.Properties.Resources.hourglass;
            Invalidate();

            if (checkExpiration)
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, expirationCount > 0 ? "Items with expiry have been highlighted" : "No items with expiration found", true, -15, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
        }
    }
}
