using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Threading;
using System.Linq;
using System.Collections;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.PieChart;

namespace Financial_Journal
{
    public partial class Expenditures : Form
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            TFLP.Size = new Size(this.Size.Width - 2, this.Size.Height - 2);
            base.OnPaint(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }


        List<string> HTML_Color_List = new List<string>() { "F5A9A9" , "F3E2A9" , "A9F5A9" , "A9D0F5" , "A9F5E1" , "F5A9F2" , "F5A9BC" , 
                                                            "F3F781" , "81F781" , "F79F81" , "81F781" , "81DAF5" , "F5DA81" , "F781D8" ,
                                                            "8181F7" , "D8D8D8" , "CEF6D8" , "FAAC58" , "9E6DF9" , "87A6C1" , "87C1AA" ,
                                                            "A4C187" , "C19E87" , "EF6FD1" , "6FEFBA" , "87BEC1" , "88EF6F" , "6FA2EF" };
        int HTML_Color_Index = 0;

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;
        int collapsed_width = 740;
        int expanded_with = 995;

        

        List<PieSlice> Pie_Slices_List = new List<PieSlice>();

        private void Add_Pie_Slice(string Text, decimal Value, string ToolTipText, float Displacements = 0.05F, string return_value = "")
        {
            Pie_Slices_List.Add(new PieSlice() { Texts = Text, Values = Value, Displacement = Displacements, Color = Get_Color(), Tooltip = ToolTipText, Return_Texts = return_value });
        }

        private Color Get_Color()
        {
            if (HTML_Color_Index >= HTML_Color_List.Count) HTML_Color_Index = 0;
            HTML_Color_Index++;
            return System.Drawing.ColorTranslator.FromHtml("#" + HTML_Color_List[HTML_Color_Index - 1]);
        }

        public Expenditures(Receipt _parent)
        {
            // Recreate pieControlPanel for reference later
            /*
            #region
            m_panelDrawing = new System.Drawing.PieChart.PieChartControl();
            this.m_panelDrawing = new System.Drawing.PieChart.PieChartControl();
            this.m_panelDrawing.InitialAngle = 0F;
            this.m_panelDrawing.Location = new System.Drawing.Point(7, 8);
            this.m_panelDrawing.Name = "m_panelDrawing";
            this.m_panelDrawing.ShowToolTip = true;
            this.m_panelDrawing.Size = new System.Drawing.Size(700, 280);
            this.m_panelDrawing.TabIndex = 0;
            this.m_panelDrawing.ToolTips = null;
            #endregion
            */

            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);

            InitializeComponent();


            InitializeChart();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            expanded_with = Start_Size.Width;
            this.Width = collapsed_width;
            Set_Form_Color(parent.Frame_Color);

            comboBoxShadowStyle.Items.AddRange(new string[] { "NoShadow", "UniformShadow", "GradualShadow" });
            comboBoxShadowStyle.SelectedIndex = (int)ShadowStyle.GradualShadow;

            m_panelDrawing.SliceRelativeHeight = 0.25F;

            string[] types = Enum.GetNames(typeof(EdgeColorType));
            comboBoxEdgeType.Items.AddRange(types);
            comboBoxEdgeType.SelectedIndex = (int)EdgeColorType.DarkerThanSurface;

            m_panelDrawing.EdgeColorType = (EdgeColorType)comboBoxEdgeType.SelectedIndex;


            transparency_text.Text = (Math.Round((double)(trackBar1.Value / 2.55))).ToString() + "%";
            height_text.Text = (trackBar3.Value * 2).ToString();

            // Default start with current month
        }

        bool mouse_down = false;
        bool Form_Loaded = false;
        Point mouse_down_location = new Point();

        private List<Item> Current_Item_List = new List<Item>();
        private List<Item> Current_Expense_List = new List<Item>();

        private void pie_MouseDown(object sender, MouseEventArgs e)
        {
            m_panelDrawing.ShowToolTip = false;
            mouse_down = true;
            mouse_down_location = new Point(e.X, e.Y);
        }

        private void pie_MouseUp(object sender, MouseEventArgs e)
        {
            m_panelDrawing.ShowToolTip = true;
            mouse_down = false;
            mouse_down_location = Control.MousePosition;
            start_angle = m_panelDrawing.InitialAngle;
        }

        private void pie_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouse_down)
            {
                m_panelDrawing.InitialAngle = start_angle + (float)(mouse_down_location.X - e.X)/2;
            }
        }

        float start_angle;

        private void Receipt_Load(object sender, EventArgs e)
        {
            this.Click += new EventHandler(this.form_Click);
            if (DesignMode)
                return;
            else
            {
                m_panelDrawing.MouseDown += new MouseEventHandler(this.pie_MouseDown);
                m_panelDrawing.MouseUp += new MouseEventHandler(this.pie_MouseUp);
                m_panelDrawing.MouseMove += new MouseEventHandler(this.pie_MouseMove);
                // Mousedown anywhere to drag
                this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

                view_mode.Items.Add("Current Month");
                view_mode.Items.Add("Last Month");
                view_mode.Items.Add("Specific Month");
                view_mode.Items.Add("Last 3 Months");
                view_mode.Items.Add("Current Year");
                view_mode.Items.Add("Specific Year");
                view_mode.Items.Add("All");

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

                Years.ForEach(x => year_box.Items.Add(x));
                Months.ForEach(x => month_box.Items.Add(x));

                // Set Default Value
                month_box.Text = DateTime.Now.Month.ToString("D2");
                year_box.Text = Convert.ToString(DateTime.Now.Year);
                view_mode.Text = "Current Month";

                Get_Current_Month_Items();

                Form_Loaded = true;


                // Memo Tooltip (Hover)
                ToolTip ToolTip1 = new ToolTip();
                ToolTip1.InitialDelay = 1;
                ToolTip1.ReshowDelay = 1;

                ToolTip1.SetToolTip(close_view_button, "Hide Appearance Settings");
                ToolTip1.SetToolTip(open_appearance, "Change Pie Appearance");
                ToolTip1.SetToolTip(reset, "Reset Grouping");
                ToolTip1.SetToolTip(group_all, "Group All Categories");
                ToolTip1.SetToolTip(button3, "Hint");

                // Set title
                label7.Text = "General Expenditure";

                income_box.Enabled = parent.Monthly_Income > 0;
                
                this.SuspendLayout();
                this.Invalidate();
                Update();
                this.ResumeLayout();
            }

            foreach (string item in parent.category_box.Items)
            {
                cmbManual.Items.Add(item);
            }
            //cmbManual.CheckBoxItems[index].Checked = true;

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

        public string Parse_Dictionary_To_String(List<Item> KVP)
        {
            string line = "[APP_COLOR]=" + System.Drawing.ColorTranslator.ToHtml(parent.Frame_Color);
            foreach (Item item in KVP)
            {
                if (item.Quantity - Convert.ToInt32(item.Status) != 0)
                {
                    line += "||[ITEM_DESC]=" + item.Name +
                            "|[ITEM_LOCATION]=" + item.Location +
                            "|[ITEM_STATUS]=" + item.Status +
                            "|[ITEM_CATEGORY]=" + item.Category +
                            "|[ITEM_QUANTITY]=" + item.Quantity +
                            "|[ITEM_PRICE]=" + item.Price +
                            "|[ITEM_DISCOUNT_AMT]=" + item.Discount_Amt +
                            "|[ITEM_DATE]=" + item.Date.ToString();
                }
            }
            line += "||[TAX_RULE]=DEFAULT_RATE123,[TAX_RATE]=" + parent.Tax_Rate + "|";
            foreach (KeyValuePair<string, string> Tax_Entry in parent.Tax_Rules_Dictionary)
            {
                line += "[TAX_RULE]=" + Tax_Entry.Key + ",[TAX_RATE]=" + Tax_Entry.Value + "|";
            }
            return line.TrimEnd(Convert.ToChar("|"));
        }

        private void InitializeChart()
        {
            SetValues();
            SetPieDisplacements();
            SetColors();
            SetTexts();
            SetToolTips();
            SetReturnValues();
        }

        private void SetValues()
        {
            m_panelDrawing.Values = Values;
        }

        private void SetPieDisplacements()
        {
            m_panelDrawing.SliceRelativeDisplacements = Displacements;
        }

        private void SetColors()
        {
            m_panelDrawing.Colors = Colors;
        }

        private void SetTexts()
        {
            m_panelDrawing.Texts = Texts;
        }

        private void SetReturnValues()
        {
            m_panelDrawing.Return_Value = Return_Texts;
        }

        private void SetToolTips()
        {
            m_panelDrawing.ToolTips = ToolTips;
        }

        // Get/Set Slice Values from Pie_Slice_List
        #region Slice Setup
        private Color[] Colors
        {
            get
            {
                ArrayList colors = new ArrayList();
                int f = (int)trackBar1.Value;
                foreach (PieSlice PS in Pie_Slices_List)
                {
                    colors.Add(Color.FromArgb((int)f, PS.Color));
                }
                return (Color[])colors.ToArray(typeof(Color));
            }
        }

        private decimal[] Values
        {
            get
            {
                ArrayList values = new ArrayList();
                foreach (PieSlice PS in Pie_Slices_List)
                {
                    values.Add(PS.Values);
                }
                return (decimal[])values.ToArray(typeof(decimal));
            }
        }

        private float[] Displacements
        {
            get
            {
                ArrayList displacements = new ArrayList();
                foreach (PieSlice PS in Pie_Slices_List)
                {
                    displacements.Add(PS.Displacement);
                }
                return (float[])displacements.ToArray(typeof(float));
            }
        }

        private string[] Texts
        {
            get
            {
                ArrayList texts = new ArrayList();
                foreach (PieSlice PS in Pie_Slices_List)
                {
                    texts.Add(PS.Texts);
                }
                return (string[])texts.ToArray(typeof(string));
            }
        }

        private string[] Return_Texts
        {
            get
            {
                ArrayList return_texts = new ArrayList();
                foreach (PieSlice PS in Pie_Slices_List)
                {
                    return_texts.Add(PS.Return_Texts);
                }
                return (string[])return_texts.ToArray(typeof(string));
            }
        }

        private string[] ToolTips
        {
            get
            {
                ArrayList toolTips = new ArrayList();
                foreach (PieSlice PS in Pie_Slices_List)
                {
                    toolTips.Add(PS.Tooltip);
                }
                return (string[])toolTips.ToArray(typeof(string));
            }
        }
        #endregion

        private void Populate_Slices()
        {
            HTML_Color_Index = 0; // reset color pool
            Pie_Slices_List = new List<PieSlice>();
            double Spend_Total = 0;


            string[] Categories_Selected = cmbManual.Text.Split(new string[] { ", " }, StringSplitOptions.None);

            Dictionary<string, double> Category_Values = new Dictionary<string, double>();
            Dictionary<string, List<Item>> Category_To_Items = new Dictionary<string, List<Item>>();
            foreach (Item item in Current_Item_List)
            {
                double price = (item.Price * (1 + parent.Get_Tax_Amount(item)) - item.Discount_Amt / item.Quantity) * item.Get_Current_Quantity();// (item.Quantity - Convert.ToInt32(item.Status));

                if (Category_Values.ContainsKey(item.Category))
                {
                    Category_Values[item.Category] += price;

                }
                else
                {
                    Category_Values.Add(item.Category, price);
                }
                if (Category_To_Items.ContainsKey(item.Category))
                {
                    Category_To_Items[item.Category].Add(item);
                }
                else
                {
                    Category_To_Items.Add(item.Category, new List<Item>() { item });
                }



                if (show_general_box.Checked) Spend_Total += price;
            }

            List<Expenses> Expense_List = new List<Expenses>();

            // Get all the expense sum first and append to spent_Total since we need to use this number for each interation of slice insertion of expenses
            if (show_expenses_box.Checked)
            {
                Expense_List = parent.Expenses_List;//.Where(x => x.Expense_Status == "1").ToList();
                DateTime First_Year_Date = Convert.ToDateTime("01/01/" + DateTime.Now.Year.ToString());

                foreach (Expenses expense in Expense_List)
                {
                    /* only based on current period expenses
                    if (view_mode.Text == "Current Month") Spend_Total += expense.Get_Amount_From_Weeks(4.3452380952380940116);  // One month
                    else if (view_mode.Text == "Last Month") Spend_Total += expense.Get_Amount_From_Weeks(4.3452380952380940116);  // One month
                    else if (view_mode.Text == "Specific Month") Spend_Total += expense.Get_Amount_From_Weeks(4.3452380952380940116);  // One month
                    else if (view_mode.Text == "Last 3 Months") Spend_Total += expense.Get_Amount_From_Weeks(4.3452380952380940116 * 3);  // One month
                    else if (view_mode.Text == "Current Year") Spend_Total += expense.Get_Amount_From_Weeks((DateTime.Now - First_Year_Date).TotalDays / 7);  // One month
                    else if (view_mode.Text == "Specific Year") Spend_Total += expense.Get_Amount_From_Weeks(DateTime.Now.Year.ToString() == year_box.Text ? (DateTime.Now - First_Year_Date).TotalDays / 7: 365.25);  // One month
                    else if (view_mode.Text == "All") Spend_Total += expense.Get_Amount_From_Weeks(365.25 * year_box.Items.Count);  // One month
                    */

                    // based on according period expenses

                    DateTime Start_Date = DateTime.Now;
                    DateTime End_Date = DateTime.Now;

                    if (view_mode.Text == "Current Month") {
                        Start_Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                        End_Date = DateTime.Now; 
                    }
                    else if (view_mode.Text == "Last Month")
                    {
                        Start_Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(-1);
                        End_Date = (new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1)).AddDays(-1);
                    }
                    else if (view_mode.Text == "Specific Month")
                    {
                        Start_Date = new DateTime(Convert.ToInt32(year_box.Text), Convert.ToInt32(month_box.Text), 1);
                        End_Date = (Start_Date.AddMonths(1));
                    }
                    else if (view_mode.Text == "Last 3 Months")
                    {
                        Start_Date = DateTime.Now.AddMonths(-3);
                        End_Date = DateTime.Now;
                    }
                    else if (view_mode.Text == "Current Year")
                    {
                        Start_Date = new DateTime(DateTime.Now.Year, 1, 1);
                        End_Date = DateTime.Now;
                    }
                    else if (view_mode.Text == "Specific Year")
                    {
                        Start_Date = new DateTime(Convert.ToInt32(year_box.Text), 1, 1);
                        End_Date = (Start_Date.AddYears(1));
                    }
                    else if (view_mode.Text == "All")
                    {
                        Start_Date = parent.Order_List.OrderBy(x => x.Date).ToList()[0].Date; // get earliest order date
                        End_Date = DateTime.Now;
                    }

                    double spent = expense.Get_Total_Paid(Start_Date, End_Date);
                    Spend_Total += spent;
                    expense.Temp_Exp = spent;
                }
            }

            double Pre_Income_Total = Spend_Total;

            // Include income in total (before all other income)
            if (income_box.Checked)
            {
                Spend_Total = 0;

                /* Depreciated method
                if (view_mode.Text == "Current Month") Spend_Total = parent.Monthly_Income;  // One month
                else if (view_mode.Text == "Last Month") Spend_Total = parent.Monthly_Income; // One month
                else if (view_mode.Text == "Specific Month") Spend_Total = parent.Monthly_Income;  // One month
                else if (view_mode.Text == "Last 3 Months") Spend_Total = parent.Monthly_Income * 3;  // One month
                else if (view_mode.Text == "Current Year") Spend_Total = parent.Monthly_Income * DateTime.Now.Month;  // One month
                else if (view_mode.Text == "Specific Year") Spend_Total = parent.Monthly_Income * 12;  // One month
                else if (view_mode.Text == "All") Spend_Total = parent.Monthly_Income * year_box.Items.Count;  // One month
                */
                
                // Get Dynamic Income
                using (Savings_Helper SH = new Savings_Helper(parent))
                {
                    if (view_mode.Text == "Current Month") Spend_Total = SH.Get_Monthly_Salary(DateTime.Now.Month, DateTime.Now.Year);  // One month
                    else if (view_mode.Text == "Last Month") Spend_Total = SH.Get_Monthly_Salary(DateTime.Now.AddMonths(-1).Month, DateTime.Now.AddMonths(-1).Year); // One month
                    else if (view_mode.Text == "Specific Month") Spend_Total = SH.Get_Monthly_Salary(Convert.ToInt32(month_box.Text), Convert.ToInt32(year_box.Text));  // One month
                    else if (view_mode.Text == "Last 3 Months")
                    {
                        DateTime Ref_Date = DateTime.Now;
                        for (int i = 0; i < 3; i++)
                        {
                            Spend_Total += SH.Get_Monthly_Salary(Ref_Date.Month, Ref_Date.Year);
                            Ref_Date.AddMonths(-1);
                        }
                    }
                    else if (view_mode.Text == "Current Year")
                    {
                        DateTime Ref_Date = DateTime.Now;
                        for (int i = 0; i < 12; i++)
                        {
                            Spend_Total += SH.Get_Monthly_Salary(Ref_Date.Month, Ref_Date.Year);
                            Ref_Date.AddMonths(-1);
                        }
                    }
                    else if (view_mode.Text == "Specific Year")
                    {
                        DateTime Ref_Date = new DateTime(Convert.ToInt32(year_box.Text), 1, 1);
                        for (int i = 0; i < 12; i++)
                        {
                            Spend_Total += SH.Get_Monthly_Salary(Ref_Date.Month, Ref_Date.Year);
                            Ref_Date.AddMonths(1);
                        }
                    }
                    else if (view_mode.Text == "All") 
                    {
                        DateTime Ref_Date = new DateTime(Convert.ToInt32(year_box.Items[0]), 1, 1);
                        for (int j = 0; j < year_box.Items.Count; j++)
                        {
                            for (int i = 0; i < 12; i++)
                            {
                                Spend_Total += SH.Get_Monthly_Salary(Ref_Date.Month, Ref_Date.Year);
                                Ref_Date.AddMonths(1);
                            }
                        }
                    }
                }
            }

            // Put expenses with the slices
            if (show_expenses_box.Checked)
            {
                foreach (Expenses expense in Expense_List.Where(x => x.Expense_Status == "1").ToList())
                {
                    Add_Pie_Slice(expense.Expense_Name + Environment.NewLine + " (" + Math.Round((expense.Temp_Exp / Spend_Total) * 100, 2) + "%)", Convert.ToDecimal(expense.Temp_Exp), "Pay to: " + expense.Expense_Payee + " ($" + String.Format("{0:0.00}", expense.Temp_Exp) + ")");
                    
                    /*
                    if (view_mode.Text == "Current Month")
                        Add_Pie_Slice(expense.Expense_Name + Environment.NewLine + " (" + Math.Round((expense.Get_Amount_From_Weeks(4.3452380952380940116) / Spend_Total) * 100, 2) + "%)", Convert.ToDecimal(expense.Get_Amount_From_Weeks(4.3452380952380940116)), "Pay to: " + expense.Expense_Payee + " ($" + String.Format("{0:0.00}", expense.Get_Amount_From_Weeks(4.3452380952380940116)) + ")");
                    else if (view_mode.Text == "Last Month")
                        Add_Pie_Slice(expense.Expense_Name + Environment.NewLine + " (" + Math.Round((expense.Get_Amount_From_Weeks(4.3452380952380940116) / Spend_Total) * 100, 2) + "%)", Convert.ToDecimal(expense.Get_Amount_From_Weeks(4.3452380952380940116)), "Pay to: " + expense.Expense_Payee + " ($" + String.Format("{0:0.00}", expense.Get_Amount_From_Weeks(4.3452380952380940116)) + ")");
                    else if (view_mode.Text == "Specific Month")
                        Add_Pie_Slice(expense.Expense_Name + Environment.NewLine + " (" + Math.Round((expense.Get_Amount_From_Weeks(4.3452380952380940116) / Spend_Total) * 100, 2) + "%)", Convert.ToDecimal(expense.Get_Amount_From_Weeks(4.3452380952380940116)), "Pay to: " + expense.Expense_Payee + " ($" + String.Format("{0:0.00}", expense.Get_Amount_From_Weeks(4.3452380952380940116)) + ")");
                    else if (view_mode.Text == "Last 3 Months")
                        Add_Pie_Slice(expense.Expense_Name + Environment.NewLine + " (" + Math.Round((expense.Get_Amount_From_Weeks(4.3452380952380940116 * 3) / Spend_Total) * 100, 2) + "%)", Convert.ToDecimal(expense.Get_Amount_From_Weeks(4.3452380952380940116 * 3)), "Pay to: " + expense.Expense_Payee + " ($" + String.Format("{0:0.00}", expense.Get_Amount_From_Weeks(4.3452380952380940116 * 3)) + ")");
                    else if (view_mode.Text == "Current Year")
                        Add_Pie_Slice(expense.Expense_Name + Environment.NewLine + " (" + Math.Round((expense.Get_Amount_From_Weeks(4.3452380952380940116 * DateTime.Now.Month) / Spend_Total) * 100, 2) + "%)", Convert.ToDecimal(expense.Get_Amount_From_Weeks(4.3452380952380940116 * DateTime.Now.Month)), "Pay to: " + expense.Expense_Payee + " ($" + String.Format("{0:0.00}", expense.Get_Amount_From_Weeks(4.3452380952380940116 * DateTime.Now.Month)) + ")");
                    else if (view_mode.Text == "Specific Year")
                        Add_Pie_Slice(expense.Expense_Name + Environment.NewLine + " (" + Math.Round((expense.Get_Amount_From_Weeks(4.3452380952380940116 * 12) / Spend_Total) * 100, 2) + "%)", Convert.ToDecimal(expense.Get_Amount_From_Weeks(4.3452380952380940116 * 12)), "Pay to: " + expense.Expense_Payee + " ($" + String.Format("{0:0.00}", expense.Get_Amount_From_Weeks(4.3452380952380940116 * 12)) + ")");
                    else if (view_mode.Text == "All")
                        Add_Pie_Slice(expense.Expense_Name + Environment.NewLine + " (" + Math.Round((expense.Get_Amount_From_Weeks(4.3452380952380940116 * 12 * year_box.Items.Count) / Spend_Total) * 100, 2) + "%)", Convert.ToDecimal(expense.Get_Amount_From_Weeks(4.3452380952380940116 * 12 * year_box.Items.Count)), "Pay to: " + expense.Expense_Payee + " ($" + String.Format("{0:0.00}", expense.Get_Amount_From_Weeks(4.3452380952380940116 * 12 * year_box.Items.Count)) + ")");
                    */
                }
                
            }

            if (show_general_box.Checked)
            {
                // Add grouped slices first
                if (Categories_Selected.Count() > 1)
                {
                    double Grouped_Total = 0;
                    foreach (KeyValuePair<string, double> entry in Category_Values)
                    {
                        if (Categories_Selected.Contains(entry.Key))
                        {
                            Grouped_Total += entry.Value;
                        }
                    }
                    var itemsToRemove = Category_Values.Where(f => Categories_Selected.Contains(f.Key)).ToArray();
                    foreach (var item in itemsToRemove)
                        Category_Values.Remove(item.Key);
                    Category_Values.Add((itemsToRemove.Count() == cmbManual.Items.Count - 1? "All General Expenditure" + Environment.NewLine + "(All grouped)" : "Grouped"), Grouped_Total);
                }

                // Add each category
                foreach (KeyValuePair<string, double> entry in Category_Values)
                {
                    if (entry.Value > 0)
                    {
                        Add_Pie_Slice(entry.Key + Environment.NewLine + "(" + Math.Round((entry.Value / Spend_Total) * 100, 2) + "%)", Convert.ToDecimal(entry.Value), entry.Key + " ($" + String.Format("{0:0.00}", entry.Value) + ")", 0.05F, entry.Key == "Grouped" || entry.Key.Contains("All General Expenditure") ? "" : Parse_Dictionary_To_String(Category_To_Items[entry.Key]));
                    }
                    else
                    {
                    }
                }
            }

            if (income_box.Checked)
            {
                Add_Pie_Slice("Left-over Income" + Environment.NewLine + "(" + Math.Round(((Spend_Total - Pre_Income_Total) / Spend_Total) * 100, 2) + "%)", Convert.ToDecimal((Spend_Total - Pre_Income_Total)), "Left-over Income ($" + String.Format("{0:0.00}", (Spend_Total - Pre_Income_Total)) + ")", 0.16F);
            }

            if (sort_check.Checked)
            {
                List<PieSlice> unSorted = Pie_Slices_List;
                Pie_Slices_List = unSorted.OrderBy(x => x.Values).ToList();
            }
            else if (spread_data_box.Checked)
            {
                decimal Price_Margin = Convert.ToDecimal(Pre_Income_Total / 30);
                for (int i = 0; i < Pie_Slices_List.Count - 2; i++)
                {
                    if ((Pie_Slices_List[i].Values + Price_Margin <= Pie_Slices_List[i + 1].Values) || (Pie_Slices_List[i].Values - Price_Margin >= Pie_Slices_List[i + 1].Values))
                    {
                        // Swap algorithm
                        PieSlice temp = Pie_Slices_List[i];
                        Pie_Slices_List[i] = Pie_Slices_List[i + 1];
                        Pie_Slices_List[i + 1] = Pie_Slices_List[i + 2];
                        Pie_Slices_List[i + 2] = temp;
                    }
                }
            }

            total_text.Text = "Total Expenditure: $" + String.Format("{0:0.00}", Pre_Income_Total) + " for " + (view_mode.Text).ToLower();
            InitializeChart();
        }


        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void form_Click(object sender, EventArgs e)
        {
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

        private void comboBoxEdgeType_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            m_panelDrawing.EdgeColorType = (EdgeColorType)comboBoxEdgeType.SelectedIndex;
        }


        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            SetColors();
            transparency_text.Text = (Math.Round((double)(trackBar1.Value / 2.55))).ToString() + "%";
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            m_panelDrawing.SliceRelativeHeight = (float)(Convert.ToDouble(trackBar3.Value) / 100);
            height_text.Text = (trackBar3.Value * 2).ToString();
        }

        private void comboBoxShadowStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_panelDrawing.ShadowStyle = (ShadowStyle)comboBoxShadowStyle.SelectedIndex;
        }


        private void close_view_button_Click(object sender, EventArgs e)
        {
            open_appearance.Visible = true;
            this.Width = collapsed_width;
        }

        private void open_appearance_Click(object sender, EventArgs e)
        {
            open_appearance.Visible = false;
            this.Width = expanded_with;
        }

        private void view_mode_SelectedIndexChanged(object sender, EventArgs e)
        {
            income_box.Enabled = true;
            if (Form_Loaded)
            {
                if (view_mode.Text.Contains("Specific"))
                {
                    year_box.Visible = true;
                    open_appearance.Visible = false;
                    this.Width = expanded_with;
                    if (view_mode.Text == "Specific Month")
                    {
                        month_box.Visible = true;
                        Get_Specific_Month_Items(Convert.ToInt32(month_box.Text), Convert.ToInt32(year_box.Text));
                    }
                    else if (view_mode.Text == "Specific Year")
                    {
                        month_box.Visible = false;
                        Get_Specific_Year_Items(Convert.ToInt32(year_box.Text));
                    }
                }
                else
                {
                    year_box.Visible = false;
                    month_box.Visible = false;
                    if (view_mode.Text == "Current Month") Get_Current_Month_Items();
                    else if (view_mode.Text == "Last Month") Get_Specific_Month_Items(DateTime.Now.AddMonths(-1).Month, DateTime.Now.AddMonths(-1).Year);
                    else if (view_mode.Text == "Last 3 Months") Get_Specific_Month_Items(DateTime.Now.AddMonths(-2), DateTime.Now);
                    else if (view_mode.Text == "Current Year") Get_Current_Year_Items();
                    else if (view_mode.Text == "All")
                    {
                        Get_All();
                        income_box.Enabled = false;
                        income_box.Checked = false;
                    }
                }
            }
        }

        // Specific Year
        private void year_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Form_Loaded)
            {
                if (month_box.Visible)
                {
                    Get_Specific_Month_Items(Convert.ToInt32(month_box.Text), Convert.ToInt32(year_box.Text));
                }
                else
                {
                    Get_Specific_Year_Items(Convert.ToInt32(year_box.Text));
                }
            }
        }

        // Specific Month
        private void month_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Form_Loaded)
            {
                Get_Specific_Month_Items(Convert.ToInt32(month_box.Text), Convert.ToInt32(year_box.Text));
            }
        }

        private void Get_Current_Month_Items()
        {
            Current_Item_List = new List<Item>();
            Current_Item_List = parent.Master_Item_List.Where(p => p.Date.Month == DateTime.Now.Month && p.Date.Year == DateTime.Now.Year).ToList();
            Populate_Slices();
        }

        private void Get_Current_Year_Items()
        {
            Current_Item_List = new List<Item>();
            Current_Item_List = parent.Master_Item_List.Where(p => p.Date.Year == DateTime.Now.Year).ToList();
            Populate_Slices();
        }

        private void Get_Specific_Year_Items(int specific_year)
        {
            Current_Item_List = new List<Item>();
            Current_Item_List = parent.Master_Item_List.Where(p => p.Date.Year == specific_year).ToList();
            Populate_Slices();
        }

        private void Get_Specific_Month_Items(DateTime start_month, DateTime end_month)
        {
            Current_Item_List = new List<Item>();
            Current_Item_List = parent.Master_Item_List.Where(p => p.Date >= start_month
                                                                && p.Date <= end_month
                                                                ).ToList();
            Populate_Slices();
        }

        private void Get_Specific_Month_Items(int month, int year)
        {
            Current_Item_List = new List<Item>();
            Current_Item_List = parent.Master_Item_List.Where(p => p.Date.Month == month
                                                                && p.Date.Year == year
                                                                ).ToList();
            Populate_Slices();
        }

        private void Get_All()
        {
            Current_Item_List = parent.Master_Item_List;
            Populate_Slices();
        }

        private void sort_check_CheckedChanged(object sender, EventArgs e)
        {
            Populate_Slices();
        }

        private void income_box_CheckedChanged(object sender, EventArgs e)
        {
            Populate_Slices();
        }

        private void spread_data_box_CheckedChanged(object sender, EventArgs e)
        {
            Populate_Slices();
        }

        private void show_expenses_box_CheckedChanged(object sender, EventArgs e)
        {
            if (!show_expenses_box.Checked && !show_general_box.Checked)
            {
                show_expenses_box.CheckedChanged -= show_expenses_box_CheckedChanged;
                show_expenses_box.Checked = true;
                show_expenses_box.CheckedChanged += show_expenses_box_CheckedChanged;
            }
            else
            {
                if (show_expenses_box.Checked) label7.Text = "General Expenditure and Recurring Expenses";
                else label7.Text = "General Expenditure";
                Populate_Slices();
            }
        }

        private void show_general_box_CheckedChanged_1(object sender, EventArgs e)
        {
            if (!show_general_box.Checked && !show_expenses_box.Checked)
            {
                show_general_box.CheckedChanged -= show_general_box_CheckedChanged_1;
                show_general_box.Checked = true;
                show_general_box.CheckedChanged += show_general_box_CheckedChanged_1;
            }
            else
            {
                if (show_general_box.Checked) label7.Text = "General Expenditure and Recurring Expenses";
                else label7.Text = "Recurring Expenses";
                Populate_Slices();
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void cmbManual_SelectedIndexChanged(object sender, EventArgs e)
        {
            Populate_Slices();
        }

        private void height_text_Click(object sender, EventArgs e)
        {

        }

        private void search_desc_button_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cmbManual.Items.Count; i++)
            {
                cmbManual.CheckBoxItems[i].Checked = false;
            }
        }

        private void group_all_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cmbManual.Items.Count; i++)
            {
                cmbManual.CheckBoxItems[i].Checked = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Form_Message_Box FMB = new Form_Message_Box(parent, "Triple-click category to see detailed items. Hold and drag pie to rotate!", true, 0, this.Location, this.Size);
            FMB.ShowDialog();
            Grey_In();
        }

        private void transparency_text_Click(object sender, EventArgs e)
        {

        }

        private void rotation_text_Click(object sender, EventArgs e)
        {

        }



    }

    public class PieSlice
    {
        public string Texts { get; set; }
        public string Return_Texts { get; set; }
        public decimal Values { get; set; }
        public float Displacement { get; set; }
        public Color Color { get; set; }
        public string Tooltip { get; set; }

        public PieSlice()
        {

        }
    }
}
