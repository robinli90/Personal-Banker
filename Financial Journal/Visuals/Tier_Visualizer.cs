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
    public partial class Tier_View : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            type_box.Items.Add("Category");
            type_box.Items.Add("Location");
            advancedComboBox1.Items.Add("Total Spent");
            advancedComboBox1.Items.Add("Frequency");
            advancedComboBox1.Text = "Total Spent";
            type_box.Text = "Category";
            View_Type = "Spent_Total";
            View_By = "Category";


            a_date_1.Items.Add("Current Month");
            a_date_1.Items.Add("Last Month");
            a_date_1.Items.Add("Specific Month");
            a_date_1.Items.Add("Current Year");
            a_date_1.Items.Add("Specific Year");

            b_date_1.Items.Add("Current Month");
            b_date_1.Items.Add("Last Month");
            b_date_1.Items.Add("Specific Month");
            b_date_1.Items.Add("Current Year");
            b_date_1.Items.Add("Specific Year");

            a_date_1.Text = "Current Month";
            b_date_1.Text = "Last Month";

            // Add months and years to box (only get the months and years where purchases have been made)
            List<string> Years = new List<string>();
            List<string> Months = new List<string>();
            foreach (Item item in parent.Master_Item_List)
            {
                if (!Years.Contains(item.Date.Year.ToString()))
                {
                    Years.Add(item.Date.Year.ToString());
                }
                if (!Months.Contains(item.Date.Month.ToString("D2")))
                {
                    Months.Add(item.Date.Month.ToString("D2"));
                }
            }

            Years = Years.OrderBy(x => Convert.ToInt32(x)).ToList();
            Months = Months.OrderBy(x => Convert.ToInt32(x)).ToList();

            Years.ForEach(x => a_date_3.Items.Add(x));
            Years.ForEach(x => b_date_3.Items.Add(x));
            Months.ForEach(x => a_date_2.Items.Add(x));
            Months.ForEach(x => b_date_2.Items.Add(x));

            // Set Default Value
            a_date_2.Text = DateTime.Now.Month.ToString("D2");
            b_date_2.Text = DateTime.Now.Month.ToString("D2");
            a_date_3.Text = Convert.ToString(DateTime.Now.Year);
            b_date_3.Text = Convert.ToString(DateTime.Now.Year);

            // Preset date

            Time_Frame_Month = DateTime.Now.Month.ToString();
            Time_Frame_Year = DateTime.Now.Year.ToString();
            Compare_Time_Frame_Month = (DateTime.Now.AddMonths(-1).Month).ToString();
            Compare_Time_Frame_Year = DateTime.Now.AddYears(Compare_Time_Frame_Month == "12" ? -1 : 0).Year.ToString();

            Invalidate();
        }

        List<Button> Delete_Item_Buttons = new List<Button>();
        ToolTip ToolTip1 = new ToolTip();

        protected override void OnPaint(PaintEventArgs e)
        {
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;
            
            //Get_Tier_Information(ref Main_Tier_List, (DateTime.Now.Month - 1).ToString(), DateTime.Now.Year.ToString());
            //Get_Tier_Information(ref Compare_Tier_List, (DateTime.Now.Month - 2).ToString(), DateTime.Now.Year.ToString());
            Get_Tier_Information(ref Main_Tier_List, Time_Frame_Month, Time_Frame_Year);
            Get_Tier_Information(ref Compare_Tier_List, Compare_Time_Frame_Month, Compare_Time_Frame_Year);

            // Get Tier Percentages
            Main_Tier_List.ForEach(x => x.Set_Tier_Percentages());
            Compare_Tier_List.ForEach(x => x.Set_Tier_Percentages());

            // Set matching colors for matching categories
            foreach (Tier T in Main_Tier_List)
            {
                try
                {
                    Tier g = Compare_Tier_List[T.Level-1];
                    T.Check_Tier_Overlap(ref g);
                }
                catch
                { 
                }
            }

            int start_margin = 180;
            int start_height = 80;
            int Original_Start_Height = start_height;

            Color DrawForeColor = Color.White;

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(180, 180, 180));
            SolidBrush LightGreyBrush = new SolidBrush(Color.FromArgb(210, 210, 210));
            SolidBrush BlueBrush = new SolidBrush(Color.LightBlue);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            SolidBrush PurpleBrush = new SolidBrush(Color.MediumPurple);
            SolidBrush RedBrush = new SolidBrush(Color.LightPink);
            SolidBrush OrangeBrush = new SolidBrush(Color.Orange);
            SolidBrush LightOrangeBrush = new SolidBrush(Color.FromArgb(255, 200, 0));

            Pen p = new Pen(WritingBrush, 1);
            Pen p_dash = new Pen(GreyBrush, 1);
            p_dash.DashPattern = new float[] { 2, 2, 2, 2, 2, 2, 2 };
            Pen Grey_Pen = new Pen(GreyBrush, 1);
            Pen Blue_Pen = new Pen(BlueBrush, 1);
            Pen Green_Pen = new Pen(GreenBrush, 1);
            Pen Red_Pen = new Pen(RedBrush, 1);
            Pen Orange_Pen = new Pen(OrangeBrush, 1);
            Pen Purple_Pen = new Pen(PurpleBrush, 1);
            
            Font f_9_bold = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_10_bold = new Font("MS Reference Sans Serif", 10, FontStyle.Bold);
            Font f_10 = new Font("MS Reference Sans Serif", 10, FontStyle.Regular);
            Font f_9 = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
            Font f_14_bold = new Font("MS Reference Sans Serif", 14, FontStyle.Bold);
            Font f_12_bold = new Font("MS Reference Sans Serif", 11, FontStyle.Bold);

            int Tier_Entry_Height = 50;
            int Tier_Width = 190;
            int Spacing_Between_Tiers = 25;
            int Tier_Horizontal_Padding = 15;
            int Tier_Vertical_Padding = 15;
            int Data_Height = 20;

            int Max_Tier_Height = 0;
            int Right_End_Point = 0;


            // Draw Main Tier
            foreach (Tier T in Main_Tier_List)
            {
                int Item_Index = 0;

                // Tier Title Line
                e.Graphics.DrawString("TIER " + (T.Level), f_14_bold, WritingBrush,
                    start_margin + (T.Level - 1) * Tier_Width + (T.Level - 1) * Spacing_Between_Tiers + Tier_Horizontal_Padding / 2 + Tier_Width / 3 - 10,
                    start_height - 40);

                foreach (Tier_Item TI in T.Entries)
                {
                    if (Item_Index == 0)
                    {
                        e.Graphics.DrawString("Select Period", f_10_bold, WritingBrush, 22, start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding - 15);
                        a_date_1.Top = start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + 9;
                        a_date_2.Top = start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + 39;
                        a_date_3.Top = start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + 39;
                    }
                    // Entry Name
                    e.Graphics.DrawString(TI.Name, f_9_bold, TI.Tier_Brush,
                        start_margin + (T.Level - 1) * Tier_Width + (T.Level - 1) * Spacing_Between_Tiers + Tier_Horizontal_Padding,
                        start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding);

                    // Insert Entry Amount
                    e.Graphics.DrawString(View_Type == "Spent_Total" ? ("$" + String.Format("{0:0.00}", (TI.Total_Amount))) : TI.Total_Amount.ToString(), f_9, TI.Tier_Brush,
                        start_margin + (T.Level - 1) * Tier_Width + (T.Level - 1) * Spacing_Between_Tiers + Tier_Horizontal_Padding,
                        start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + Data_Height);

                    // Entry Percentage
                    e.Graphics.DrawString(TI.Item_Percentage, f_12_bold, TI.Tier_Brush,
                        start_margin + (T.Level - 1) * Tier_Width + (T.Level - 1) * Spacing_Between_Tiers + Tier_Horizontal_Padding + (Tier_Width - 85),
                        start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + Data_Height);

                    Item_Index++;
                }

                Rectangle r = new Rectangle(start_margin + (T.Level - 1) * Tier_Width + (T.Level - 1) * Spacing_Between_Tiers, // x location START
                                             start_height,                                                                     // y location START
                                             Tier_Width,
                                             T.Entries.Count * Tier_Entry_Height + Tier_Vertical_Padding * 2);

                e.Graphics.DrawRectangle(p, r);

                // Tier Total Line
                e.Graphics.DrawString("TOTAL: " + (View_Type == "Spent_Total" ? ("$" + String.Format("{0:0.00}", (T.Entries.Sum(x => x.Total_Amount)))) : (T.Entries.Sum(x => x.Total_Amount)).ToString()), f_10_bold, GreyBrush,
                    start_margin + (T.Level - 1) * Tier_Width + (T.Level - 1) * Spacing_Between_Tiers,
                    start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + Data_Height + 5);

                // Tier Percentage Line
                e.Graphics.DrawString(T.Tier_Percentage, f_10_bold, OrangeBrush,
                    start_margin + (T.Level) * Tier_Width + (T.Level - 1) * Spacing_Between_Tiers - 45 + (T.Tier_Percentage.Length == 2 ? 15 : T.Tier_Percentage.Length == 3 ? 10 : T.Tier_Percentage.Length == 4 ? 5 : 0),
                    start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + Data_Height + 5);

                // Set Max Tier height
                if (T.Entries.Count * Tier_Entry_Height + Tier_Vertical_Padding * 2 > Max_Tier_Height) Max_Tier_Height = T.Entries.Count * Tier_Entry_Height + Tier_Vertical_Padding * 2;
            }


            int Gap_Between_Tiers = 10;
            int Form_Height = 0;

            start_height += Max_Tier_Height + Gap_Between_Tiers + 30;

            /*
            // Sum of Tier Period
            string Tier_Sum_List = ("$" + String.Format("{0:0.00}", Main_Tier_List.Sum(yy => yy.Entries.Sum(y => y.Total_Amount))));
            e.Graphics.DrawString("Period Total", f_10_bold, WritingBrush, 15, start_height - 30 - Data_Height);
            e.Graphics.DrawString(Tier_Sum_List, f_10, WritingBrush, 15, start_height - 30);
            */

            int Center_Line_Height = start_height;

            start_height += Gap_Between_Tiers;

            // Draw Comparable Tier
            foreach (Tier T in Compare_Tier_List)
            {
                int Item_Index = 0;
                foreach (Tier_Item TI in T.Entries)
                {
                    if (Item_Index == 0)
                    {
                        e.Graphics.DrawString("Select Period", f_10_bold, WritingBrush, 22, start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding - 15);
                        b_date_1.Top = start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + 9;
                        b_date_2.Top = start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + 39;
                        b_date_3.Top = start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + 39;
                    }
                    // Entry Name
                    e.Graphics.DrawString(TI.Name, f_9_bold, TI.Tier_Brush,
                        start_margin + (T.Level - 1) * Tier_Width + (T.Level - 1) * Spacing_Between_Tiers + Tier_Horizontal_Padding,
                        start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding);

                    // Insert Entry Amount
                    e.Graphics.DrawString(View_Type == "Spent_Total" ? ("$" + String.Format("{0:0.00}", (TI.Total_Amount))) : TI.Total_Amount.ToString(), f_9, TI.Tier_Brush,
                        start_margin + (T.Level - 1) * Tier_Width + (T.Level - 1) * Spacing_Between_Tiers + Tier_Horizontal_Padding,
                        start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + Data_Height);

                    // Entry Percentage
                    e.Graphics.DrawString(TI.Item_Percentage, f_12_bold, TI.Tier_Brush,
                        start_margin + (T.Level - 1) * Tier_Width + (T.Level - 1) * Spacing_Between_Tiers + Tier_Horizontal_Padding + (Tier_Width - 85),
                        start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + Data_Height);

                    Item_Index++;
                }

                Rectangle r = new Rectangle(start_margin + (T.Level - 1) * Tier_Width + (T.Level - 1) * Spacing_Between_Tiers, // x location START
                                             start_height,                                                                     // y location START
                                             Tier_Width,
                                             T.Entries.Count * Tier_Entry_Height + Tier_Vertical_Padding * 2);

                e.Graphics.DrawRectangle(p, r);

                // Tier Total Line
                e.Graphics.DrawString("TOTAL: " + (View_Type == "Spent_Total" ? ("$" + String.Format("{0:0.00}", (T.Entries.Sum(x => x.Total_Amount)))) : (T.Entries.Sum(x => x.Total_Amount)).ToString()), f_10_bold, GreyBrush,
                    start_margin + (T.Level - 1) * Tier_Width + (T.Level - 1) * Spacing_Between_Tiers,
                    start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + Data_Height + 5);

                // Tier Percentage Line
                e.Graphics.DrawString(T.Tier_Percentage, f_10_bold, OrangeBrush,
                    start_margin + (T.Level) * Tier_Width + (T.Level - 1) * Spacing_Between_Tiers - 45 + (T.Tier_Percentage.Length == 2 ? 15 : T.Tier_Percentage.Length == 3 ? 10 : T.Tier_Percentage.Length == 4 ? 5 : 0),
                    start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + Data_Height + 5);

                if (Form_Height < start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + Data_Height + 15) Form_Height = start_height + Item_Index * Tier_Entry_Height + Tier_Vertical_Padding + Data_Height + 15;

            }

            /*
            // Sum of tier list
            Tier_Sum_List = ("$" + String.Format("{0:0.00}", Compare_Tier_List.Sum(yy => yy.Entries.Sum(y => y.Total_Amount))));
            e.Graphics.DrawString("Period Total", f_10_bold, WritingBrush, 15, Form_Height - 10 - Data_Height);
            e.Graphics.DrawString(Tier_Sum_List, f_10, WritingBrush, 15, Form_Height - 10);
            */

            // Draw vertical dashed lines

            for (int i = 1; i <= Main_Tier_List.Count + 1; i++)
            {
                int space_location = start_margin + (i - 1) * Tier_Width + (i - 1) * Spacing_Between_Tiers - Spacing_Between_Tiers / 2;
                e.Graphics.DrawLine(p_dash, space_location, Original_Start_Height - 35, space_location, Form_Height + 10);
                Right_End_Point = space_location;
            }

            // Draw horiziontal gray lines
            e.Graphics.DrawLine(p_dash, 15, Center_Line_Height, Right_End_Point, Center_Line_Height);
            e.Graphics.DrawLine(p_dash, 15, Original_Start_Height - 10, Right_End_Point, Original_Start_Height - 10);


            // Draw dynamic Tier Generation
            // Draw dynamic Tier + -
            Delete_Item_Buttons.ForEach(button => button.Image.Dispose());
            Delete_Item_Buttons.ForEach(button => button.Dispose());
            Delete_Item_Buttons.ForEach(button => this.Controls.Remove(button));
            Delete_Item_Buttons = new List<Button>();

            Original_Start_Height += 30;
            Right_End_Point -= 15;

            for (int i = 0; i < Tier_Capacities.Count; i++)
            {
                e.Graphics.DrawString("Tier " + (i + 1).ToString(), f_10_bold, WritingBrush, Right_End_Point + 40, Original_Start_Height + i * 29 + 4);

                Button minus_tier_button = new Button();
                minus_tier_button.BackColor = this.BackColor;
                minus_tier_button.ForeColor = this.BackColor;
                minus_tier_button.FlatStyle = FlatStyle.Flat;
                minus_tier_button.Image = global::Financial_Journal.Properties.Resources.minus;
                minus_tier_button.Enabled = true;
                minus_tier_button.Size = new Size(23, 23);
                minus_tier_button.Location = new Point(Right_End_Point + 100, Original_Start_Height + i*29);
                minus_tier_button.Name = "m" + i.ToString();
                minus_tier_button.Text = "";
                minus_tier_button.Click += new EventHandler(this.dynamic_button_click);
                Delete_Item_Buttons.Add(minus_tier_button);
                ToolTip1.SetToolTip(minus_tier_button, "Minus 1 Entry");
                this.Controls.Add(minus_tier_button);

                e.Graphics.DrawString(Tier_Capacities[i].ToString(), f_9_bold, WritingBrush, Right_End_Point + 130 + (Tier_Capacities[i] < 10 ? 5 : 0), Original_Start_Height + i * 29 + 4);

                Button plus_tier_button = new Button();
                plus_tier_button.BackColor = this.BackColor;
                plus_tier_button.ForeColor = this.BackColor;
                plus_tier_button.FlatStyle = FlatStyle.Flat;
                plus_tier_button.Image = global::Financial_Journal.Properties.Resources.plus;
                plus_tier_button.Enabled = true;
                plus_tier_button.Size = new Size(23, 23);
                plus_tier_button.Location = new Point(Right_End_Point + 160, Original_Start_Height + i * 29);
                plus_tier_button.Name = "a" + i.ToString();
                plus_tier_button.Text = "";
                plus_tier_button.Click += new EventHandler(this.dynamic_button_click);
                Delete_Item_Buttons.Add(plus_tier_button);
                ToolTip1.SetToolTip(plus_tier_button, "Plus 1 Entry");
                this.Controls.Add(plus_tier_button);
            }

            Button add_tier_button = new Button();
            add_tier_button.BackColor = this.BackColor;
            add_tier_button.ForeColor = this.BackColor;
            add_tier_button.FlatStyle = FlatStyle.Flat;
            add_tier_button.Image = global::Financial_Journal.Properties.Resources.upload;
            add_tier_button.Enabled = true;
            add_tier_button.Size = new Size(29, 29);
            add_tier_button.Location = new Point(Right_End_Point + 120, Original_Start_Height + (Tier_Capacities.Count) * 29 + 10);
            add_tier_button.Name = "add" + Tier_Capacities.Count.ToString();
            add_tier_button.Text = "";
            add_tier_button.Click += new EventHandler(this.add_tier_click);
            Delete_Item_Buttons.Add(add_tier_button);
            ToolTip1.SetToolTip(add_tier_button, "Add 1 Tier");
            this.Controls.Add(add_tier_button);

            Button subtract_tier_button = new Button();
            subtract_tier_button.BackColor = this.BackColor;
            subtract_tier_button.ForeColor = this.BackColor;
            subtract_tier_button.FlatStyle = FlatStyle.Flat;
            subtract_tier_button.Image = global::Financial_Journal.Properties.Resources.download;
            subtract_tier_button.Enabled = true;
            subtract_tier_button.Size = new Size(29, 29);
            subtract_tier_button.Location = new Point(Right_End_Point + 80, Original_Start_Height + (Tier_Capacities.Count) * 29 + 10);
            subtract_tier_button.Name = "sub" + Tier_Capacities.Count.ToString();
            subtract_tier_button.Text = "";
            subtract_tier_button.Click += new EventHandler(this.add_tier_click);
            Delete_Item_Buttons.Add(subtract_tier_button);
            ToolTip1.SetToolTip(subtract_tier_button, "Minus 1 Tier");
            this.Controls.Add(subtract_tier_button);

            this.Size = new Size(Right_End_Point + 200, Form_Height + 35);

            // Dispose all objects
            p.Dispose();
            Grey_Pen.Dispose();
            LightGreyBrush.Dispose();
            GreyBrush.Dispose();
            BlueBrush.Dispose();
            RedBrush.Dispose();
            GreenBrush.Dispose();
            PurpleBrush.Dispose();
            OrangeBrush.Dispose();
            f_14_bold.Dispose();
            LightOrangeBrush.Dispose();
            Blue_Pen.Dispose();
            Green_Pen.Dispose();
            Red_Pen.Dispose();
            f_12_bold.Dispose();
            f_10.Dispose();
            f_9_bold.Dispose();
            f_10_bold.Dispose();
            f_9.Dispose();
            f_14_bold.Dispose();
            Purple_Pen.Dispose();
            Orange_Pen.Dispose();
            WritingBrush.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        private void add_tier_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (b.Name.StartsWith("add")) // delete
            {
                if (Tier_Capacities.Count < 7)
                {
                    Tier_Capacities.Add(1);
                }
            }
            else if (b.Name.StartsWith("sub"))
            {
                if (Tier_Capacities.Count > 1)
                { 
                    Tier_Capacities.RemoveAt(Tier_Capacities.Count - 1);
                }
            }
            Invalidate();
        }

        private void dynamic_button_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            if (b.Name.StartsWith("m")) // delete
            {
                if (Tier_Capacities[Convert.ToInt32(b.Name.Substring(1))] > 1)
                {
                    Tier_Capacities[Convert.ToInt32(b.Name.Substring(1))]--;
                }
            }
            if (b.Name.StartsWith("a")) // delete
            {
                if (Tier_Capacities[Convert.ToInt32(b.Name.Substring(1))] < 10)
                {
                    Tier_Capacities[Convert.ToInt32(b.Name.Substring(1))]++;
                }
            }
            Invalidate();
        }

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public Tier_View(Receipt _parent)
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
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

        List<int> Tier_Capacities = new List<int>();

        public void Set_Form_Color(Color randomColor)
        {
            //minimize_button.ForeColor = randomColor;
            //close_button.ForeColor = randomColor;
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        public string View_By = ""; // Category, Location
        public string View_Type = ""; // Spent_Total, Frequency
        public string Time_Frame_Month = ""; // month#, year#, or 'ALL'
        public string Time_Frame_Year = ""; // month#, year#, or 'ALL'
        public string Compare_Time_Frame_Month = ""; // month#, year#, or 'ALL'
        public string Compare_Time_Frame_Year = ""; // month#, year#, or 'ALL'

        private List<Tier> Tier_Structure_Format = new List<Tier>();
        private List<Tier> Main_Tier_List = new List<Tier>();
        private List<Tier> Compare_Tier_List = new List<Tier>();
        private List<string> Category_List = new List<string>();
        private List<string> Location_List = new List<string>();

        /// <summary>
        /// Generate dynamic Tier List based on Reference Tier
        /// </summary>
        /// <param name="Ref_Tier_List"></param>
        public void Generate_Tier_List(ref List<Tier> Ref_Tier_List)
        {
            Tier_Capacities = parent.Tier_Format.GetRange(0, (parent.Tier_Format.Count > 7 ? 7 : parent.Tier_Format.Count));

            // Transfer format to parent (to save for next time)
            parent.Tier_Format = Tier_Capacities.Count == 0 ? new List<int>() {1} : Tier_Capacities;


            for (int i = 1; i <= Tier_Capacities.Count; i++)
            {
                Ref_Tier_List.Add(new Tier(i, Tier_Capacities[i - 1]));
            }
        }

        /// <summary>
        /// If Month < 0 return specific year
        /// If Month > 0 and Year > 0 return specific month of specific year
        /// If Month < 0 and Year < 0 return All
        /// </summary>
        /// <param name="Ref_Tier_List"></param>
        /// <param name="Month"></param>
        /// <param name="Year"></param>
        public void Get_Tier_Information(ref List<Tier> Ref_Tier_List, string Month = "-1", string Year = "-1")
        {
            Ref_Tier_List = new List<Tier>();
            Generate_Tier_List(ref Ref_Tier_List);

            List<Tier_Item> Temp_Pre_Allocation_List = new List<Tier_Item>();
            
            if (View_By == "Category")
            {
                // Get all categories
                Category_List = parent.Category_List;

                // If by dollar amount
                if (View_Type == "Spent_Total")
                {
                    foreach (string Category in Category_List)
                    {
                        Tier_Item TV = new Tier_Item();
                        TV.Name = Category;
                        TV.Total_Amount = Get_Category_Total(Category, Convert.ToInt32(Month), Convert.ToInt32(Year));
                        Temp_Pre_Allocation_List.Add(TV);
                    }
                }
                else if (View_Type == "Frequency")
                {
                    foreach (string Category in Category_List)
                    {
                        Tier_Item TV = new Tier_Item();
                        TV.Name = Category;
                        TV.Total_Amount = parent.Master_Item_List.Count(x => x.Category == Category && 
                            ((Convert.ToInt32(Month) < 0 ? true : x.Date.Month == Convert.ToInt32(Month)) && (Convert.ToInt32(Year) < 0 ? true : x.Date.Year == Convert.ToInt32(Year)))
                            );
                        Temp_Pre_Allocation_List.Add(TV);
                    }
                }
            }
            else if (View_By == "Location")
            {
                // Get all locations
                Location_List = new List<string>();
                foreach (string g in parent.location_box.Items) Location_List.Add(g);


                // If by dollar amount
                if (View_Type == "Spent_Total")
                {
                    foreach (string Location in Location_List)
                    {
                        Tier_Item TV = new Tier_Item();
                        TV.Name = Location;
                        TV.Total_Amount = Get_Location_Total(Location, Convert.ToInt32(Month), Convert.ToInt32(Year));
                        Temp_Pre_Allocation_List.Add(TV);
                    }
                }
                else if (View_Type == "Frequency")
                {
                    foreach (string Location in Location_List)
                    {
                        Tier_Item TV = new Tier_Item();
                        TV.Name = Location;
                        TV.Total_Amount = parent.Master_Item_List.Count(x => x.Location == Location &&
                            ((Convert.ToInt32(Month) < 0 ? true : x.Date.Month == Convert.ToInt32(Month)) && (Convert.ToInt32(Year) < 0 ? true : x.Date.Year == Convert.ToInt32(Year)))
                            );
                        Temp_Pre_Allocation_List.Add(TV);
                    }
                }
            }


            // Sort from greatest to least
            Temp_Pre_Allocation_List = Temp_Pre_Allocation_List.OrderByDescending(x => x.Total_Amount).ToList();

            int Tier_Count = 0;

            if (Ref_Tier_List.Count == 0)
            {

            }
            else
            {
                Tier Current_Tier = Ref_Tier_List[Tier_Count++];

                try
                {
                    // Only populate exactly enough to fill all the tiers
                    for (int i = 0; i < Ref_Tier_List.Sum(x => x.Capacity); i++)
                    {
                        // If reached capacity, move to next tier
                        if (Current_Tier.Capacity == Current_Tier.Entries.Count)
                        {
                            Current_Tier = Ref_Tier_List[Tier_Count++];
                        }
                        // Add entry to current tier
                        Current_Tier.Entries.Add(Temp_Pre_Allocation_List[i]);
                    }
                }
                catch
                {
                }
            }

            // Remove Tiers that do not have values
            Ref_Tier_List = Ref_Tier_List.Where(x => x.Entries.Count > 0).ToList();

            // Set internal percentages
            double Tier_List_Total = Ref_Tier_List.Sum(yy => yy.Entries.Sum(y => y.Total_Amount));
            Ref_Tier_List.ForEach(x => x.Set_Tier_Percentages(Tier_List_Total));

        }


        /// <summary>
        /// Get Categorical total (if month > 0, get specific month; if year provided, get specific year) 
        /// </summary>
        /// <param name="Category"></param>
        /// <param name="Month"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public double Get_Category_Total(string Category, int Month = -1, int Year = -1)
        {
            return parent.Master_Item_List.Where(x => x.Category == Category && (Month < 0 ? true : (x.Date.Month == Month)) && (Year < 0 ? true : x.Date.Year == Year)).ToList().Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));

        }

        /// <summary>
        /// Get Locational total (if month > 0, get specific month; if year provided, get specific year) 
        /// </summary>
        /// <param name="Category"></param>
        /// <param name="Month"></param>
        /// <param name="Year"></param>
        /// <returns></returns>
        public double Get_Location_Total(string Location, int Month = -1, int Year = -1)
        {
            return parent.Master_Item_List.Where(x => x.Location == Location && (Month < 0 ? true : (x.Date.Month == Month)) && (Year < 0 ? true : x.Date.Year == Year)).ToList().Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));
        }

        private void type_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            View_By = type_box.Text;
            Invalidate();
        }

        private void advancedComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            View_Type = advancedComboBox1.Text == "Total Spent" ? "Spent_Total" : "Frequency";
            Invalidate();
        }

        private void a_date_1_SelectedIndexChanged(object sender, EventArgs e)
        {
            a_date_2.Visible = a_date_1.Text.Contains("Specific Month");
            a_date_3.Visible = a_date_1.Text.Contains("Specific");

            /*
             * 
        public string Time_Frame_Month = ""; // month#, year#, or 'ALL'
        public string Time_Frame_Year = ""; // month#, year#, or 'ALL'
        public string Compare_Time_Frame_Month = ""; // month#, year#, or 'ALL'
        public string Compare_Time_Frame_Year = ""; // month#, year#, or 'ALL'
             */
            if (a_date_1.Text == "Current Month")
            {
                Time_Frame_Month = DateTime.Now.Month.ToString();
                Time_Frame_Year = DateTime.Now.Year.ToString();
            }
            if (a_date_1.Text == "Last Month")
            {
                Time_Frame_Month = (DateTime.Now.AddMonths(-1).Month).ToString();
                Time_Frame_Year = DateTime.Now.AddYears(Time_Frame_Month == "12" ? -1 : 0).Year.ToString();
            }
            if (a_date_1.Text == "Current Year")
            {
                Time_Frame_Month = "-1";
                Time_Frame_Year = DateTime.Now.Year.ToString();
            }
            Invalidate();
        }


        private void b_date_1_SelectedIndexChanged(object sender, EventArgs e)
        {
            b_date_2.Visible = b_date_1.Text.Contains("Specific Month");
            b_date_3.Visible = b_date_1.Text.Contains("Specific");

            if (b_date_1.Text == "Current Month")
            {
                Compare_Time_Frame_Month = DateTime.Now.Month.ToString();
                Compare_Time_Frame_Year = DateTime.Now.Year.ToString();
            }
            if (b_date_1.Text == "Last Month")
            {
                Compare_Time_Frame_Month = (DateTime.Now.AddMonths(-1).Month).ToString();
                Compare_Time_Frame_Year = DateTime.Now.AddYears(Compare_Time_Frame_Month == "12" ? -1 : 0).Year.ToString();
            }
            if (b_date_1.Text == "Current Year")
            {
                Compare_Time_Frame_Month = "-1";
                Compare_Time_Frame_Year = DateTime.Now.Year.ToString();
            }
            Invalidate();
        }

        private void a_date_2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Time_Frame_Month = a_date_2.Text;
            Invalidate();
        }

        private void b_date_2_SelectedIndexChanged(object sender, EventArgs e)
        {
            Compare_Time_Frame_Month = b_date_2.Text;
            Invalidate();
        }

        private void a_date_3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Time_Frame_Year = a_date_3.Text;
            Invalidate();
        }

        private void b_date_3_SelectedIndexChanged(object sender, EventArgs e)
        {
            Compare_Time_Frame_Year = b_date_3.Text;
            Invalidate();
        }
    }

    public class Tier
    {
        public List<Tier_Item> Entries { get; set; }
        public int Level { get; set; }
        public int Capacity { get; set; }
        public Color Tier_Match_Color = Color.LightPink;
        public string Tier_Percentage { get; set; }

        public Tier(int Tier_Level, int Tier_Capacity)
        {
            this.Entries = new List<Tier_Item>();
            this.Level = Tier_Level;
            this.Capacity = Tier_Capacity;
        }

        /// <summary>
        /// Check if two tiers have any overlapping names 
        /// </summary>
        /// <param name="Reference_Tier"></param>
        /// <returns></returns>
        public string Check_Tier_Overlap(ref Tier Reference_Tier)
        {
            // Only compare if same tier
            if (this.Level == Reference_Tier.Level)
            {
                foreach (Tier_Item T in Entries)
                {
                    foreach (Tier_Item RefT in Reference_Tier.Entries)
                    {
                        if (T.Name == RefT.Name)
                        {
                            T.Tier_Color = Tier_Match_Color;
                            RefT.Tier_Color = Tier_Match_Color;
                            T.Set_Brush_Color();
                            RefT.Set_Brush_Color();
                        }
                    }
                }
            }

            return "";
        }

        public void Set_Tier_Percentages(double Tier_List_Total)
        {
            Tier_Percentage = Math.Round((Entries.Sum(x => x.Total_Amount)) / Tier_List_Total * 100, 1).ToString() + "%";
        }

        /// <summary>
        /// Set the tier item percentages for all
        /// </summary>
        public void Set_Tier_Percentages()
        {
            double Tier_Sum = Entries.Sum(x => x.Total_Amount);

            foreach (Tier_Item TI in Entries)
            {
                if (Tier_Sum == 0)
                {
                    TI.Item_Percentage = "N/A";
                }
                else
                {
                    TI.Item_Percentage = Math.Round(TI.Total_Amount / Tier_Sum * 100, 1).ToString() + "%";
                }
            }

        }

    }

    public class Tier_Item
    {
        public string Name { get; set; }
        public double Total_Amount { get; set; }
        public Color Tier_Color { get; set; }
        public SolidBrush Tier_Brush { get; set; }
        public string Item_Percentage { get; set; }

        public Tier_Item()
        {
            // Preset default color
            Tier_Color = Color.White;
            this.Set_Brush_Color();
        }

        public void Set_Brush_Color()
        {
            Tier_Brush = new SolidBrush(Tier_Color);
        }
    }
}
