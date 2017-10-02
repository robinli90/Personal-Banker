using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Financial_Journal
{
    public partial class BudgetAllocation : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        public Receipt parent;
        public BudgetEntry RefBudgetEntry;
        public string refBudgetText = "";

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public BudgetAllocation(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            Location = new Point(g.X + (s.Width / 2) - (Width / 2), g.Y + (s.Height / 2) - (Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
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

            Shown += BudgetAllocation_Shown;

            PopulateProfiles();

            string refProfile = parent.Settings_Dictionary["BUDDEFPROF"];
            if (refProfile != "" && profileBox.Items.Contains(refProfile))
            {
                profileBox.Text = refProfile;
            }

            PopulateMonths();

            dataGridView1.CellClick += dataGridView1_CellContentClick;
            dataGridView1.SelectionChanged += dataGridView1_SelectionChanged;
            dataGridView1.EditingControlShowing += dataGridView1_EditingControlShowing;
            dataGridView1.GridColor = Color.White;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(98, 110, 110);

            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[3].ReadOnly = true;
            dataGridView1.Columns[4].ReadOnly = true;

            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            ToolTip1.SetToolTip(setDefault, "Set profile as default");
            ToolTip1.SetToolTip(viewGroupManager, "Manage your profiles");
            ToolTip1.SetToolTip(button4, "Remove current month budget");
            ToolTip1.SetToolTip(addBudgetMonth, "Add a new budget month");
        }

        void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridView1.CurrentCell.ColumnIndex == 2)
            {
                TextBox tb = (TextBox)e.Control;
                tb.TextChanged += new EventHandler(dataGridView1_TextChanged);
            }
        }

        private void dataGridView1_TextChanged(object sender, EventArgs e)
        {
            TextBox limit_box = (sender as TextBox); 

            if (!(limit_box.Text.StartsWith("$")))
            {
                if (parent.Get_Char_Count(limit_box.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = limit_box.Text;
                    limit_box.Text = temp.Substring(1) + temp[0];
                    limit_box.SelectionStart = limit_box.Text.Length;
                    limit_box.SelectionLength = 0;
                }
                else
                {
                    limit_box.Text = "$" + limit_box.Text;
                }
            }
            else if ((limit_box.Text.Length > 1) && ((parent.Get_Char_Count(limit_box.Text, Convert.ToChar(".")) > 1) || (limit_box.Text[1].ToString() == ".") || (parent.Get_Char_Count(limit_box.Text, Convert.ToChar("$")) > 1) || (!((limit_box.Text.Substring(limit_box.Text.Length - 1).All(char.IsDigit))) && !(limit_box.Text[limit_box.Text.Length - 1].ToString() == "."))))
            {
                //limit_box.TextChanged -= new System.EventHandler(limit_box_TextChanged);
                limit_box.Text = limit_box.Text.Substring(0, limit_box.Text.Length - 1);
                limit_box.SelectionStart = limit_box.Text.Length;
                limit_box.SelectionLength = 0;
                //limit_box.TextChanged += new System.EventHandler(limit_box_TextChanged);
            }
        }

        private void BudgetAllocation_Shown(object sender, EventArgs e)
        {
            PopulateMonths();
        }

        double budgetTotal = 0;
        double actualTotal = 0;
        double prevActualTotal = 0;
        private int highlightIndex = 0;

        private void PopulateDGV()
        {

            if (budgetMonth.SelectedIndex < 0) return;

            dataGridView1.Rows.Clear();

            DateTime Ref_Date = new DateTime(RefBudgetEntry.Year, RefBudgetEntry.Month, 1); //.AddMonths(1);
            DateTime Prev_Date = Ref_Date.AddMonths(-1); //.AddMonths(1);

            // If no profile, non-indented categories with no subcategories
            if (profileBox.Text == "None")
            {

                // categorical
                foreach (string category in parent.category_box.Items)
                {
                    double budgetValue = RefBudgetEntry.GetBudgetAmount(BCType.Categorical, category);
                    double prevActualTotal = parent.Master_Item_List
                        .Where(x => x.Category == category && x.Date.Month == Prev_Date.Month &&
                                    x.Date.Year == Prev_Date.Year).ToList()
                        .Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));
                    double actualValue = parent.Master_Item_List
                        .Where(x => x.Category == category && x.Date.Month == Ref_Date.Month &&
                                    x.Date.Year == Ref_Date.Year).ToList()
                        .Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));

                    if (DGVAddRowValue(category, prevActualTotal, budgetValue, actualValue, budgetValue - actualValue, "Categorical")) highlightIndex++;
                        
                    
                }

                // recurring
                foreach (string category in parent.Expenses_List.Select(x => x.Expense_Name))
                {

                    double prevActualTotal = parent.Expenses_List.First(x => x.Expense_Name == category)
                        .Get_Total_Paid(Prev_Date.AddMonths(0), Prev_Date.AddMonths(1));
                    double actualValue = parent.Expenses_List.First(x => x.Expense_Name == category)
                        .Get_Total_Paid(Ref_Date.AddMonths(0), Ref_Date.AddMonths(1));
                    double budgetValue = RefBudgetEntry.GetBudgetAmount(BCType.Recurring, category);

                    if (DGVAddRowValue(category, prevActualTotal, budgetValue, actualValue, budgetValue - actualValue, "Recurring")) highlightIndex++;
                }

                // extraneous
                foreach (BudgetCategory BC in RefBudgetEntry.GetCategoryList()
                    .Where(x => x.GetBCType() == BCType.Extraneous)) // Manually populate extraneous
                {
                    double budgetValue = RefBudgetEntry.GetBudgetAmount(BCType.Extraneous, BC.GetName());
                    double prevActualTotal = parent.Master_Item_List
                        .Where(x => x.Category == BC.GetName() && x.Date.Month == Prev_Date.Month &&
                                    x.Date.Year == Prev_Date.Year).ToList()
                        .Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));
                    double actualValue = parent.Master_Item_List
                        .Where(x => x.Category == BC.GetName() && x.Date.Month == Ref_Date.Month &&
                                    x.Date.Year == Ref_Date.Year).ToList()
                        .Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));

                    if (DGVAddRowValue(BC.GetName(), prevActualTotal, budgetValue, actualValue, budgetValue - actualValue, "Extraneous")) highlightIndex++;
                }

            }
            else // By profile
            {

                foreach (GroupedCategory GC in parent.GroupedCategoryList.Where(x => x._ProfileName == profileBox.Text)
                    .OrderBy(x => x._GroupName))
                {
                    DGVAddRowHeader(GC._GroupName);

                    #region Sub categories
                    foreach (string category in GC.SubCategoryList.OrderBy(x => x))
                    {
                        double budgetValue = RefBudgetEntry.GetBudgetAmount(BCType.Categorical, category);
                        double prevActualTotal = parent.Master_Item_List
                            .Where(x => x.Category == category && x.Date.Month == Prev_Date.Month &&
                                        x.Date.Year == Prev_Date.Year).ToList()
                            .Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));
                        double actualValue = parent.Master_Item_List
                            .Where(x => x.Category == category && x.Date.Month == Ref_Date.Month &&
                                        x.Date.Year == Ref_Date.Year).ToList()
                            .Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));

                        DGVAddRowValue(category, prevActualTotal, budgetValue, actualValue, budgetValue - actualValue, "Categorical", true);
                    }
                    #endregion

                    #region Sub expenses
                    foreach (string category in GC.SubExpenseList.Where(y => y.Length > 0).OrderBy(x => x))
                    {
                        double prevActualTotal = parent.Expenses_List.First(x => x.Expense_Name == category)
                            .Get_Total_Paid(Prev_Date.AddMonths(0), Prev_Date.AddMonths(1));

                        double actualValue = parent.Expenses_List.First(x => x.Expense_Name == category)
                            .Get_Total_Paid(Ref_Date.AddMonths(0), Ref_Date.AddMonths(1));

                        double budgetValue = RefBudgetEntry.GetBudgetAmount(BCType.Recurring, category);
                        DGVAddRowValue(category, prevActualTotal, budgetValue, actualValue, budgetValue - actualValue, "Recurring", true);
                    }
                    #endregion
                }

                if (RefBudgetEntry.GetCategoryList().Any(x => x.GetBCType() == BCType.Extraneous))
                {
                    DGVAddRowHeader("Extraneous Categories");

                    foreach (BudgetCategory BC in RefBudgetEntry.GetCategoryList()
                        .Where(x => x.GetBCType() == BCType.Extraneous)) // Manually populate extraneous
                    {
                        double budgetValue = RefBudgetEntry.GetBudgetAmount(BCType.Extraneous, BC.GetName());
                        double prevActualTotal = parent.Master_Item_List
                            .Where(x => x.Category == BC.GetName() && x.Date.Month == Prev_Date.Month &&
                                        x.Date.Year == Prev_Date.Year).ToList()
                            .Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));
                        double actualValue = parent.Master_Item_List
                            .Where(x => x.Category == BC.GetName() && x.Date.Month == Ref_Date.Month &&
                                        x.Date.Year == Ref_Date.Year).ToList()
                            .Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));

                        if (DGVAddRowValue(BC.GetName(), prevActualTotal, budgetValue, actualValue, budgetValue - actualValue, "Extraneous")) highlightIndex++;
                    }
                }
            }

            // Update lines
            double budgetTotal = 0;
            double actualTotal = 0;

            // Calculate totals
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (dataGridView1.Rows.IndexOf(row) < dataGridView1.Rows.Count)
                {
                    try
                    {
                        budgetTotal += Math.Round(Convert.ToDouble(row.Cells[2].Value.ToString().Substring(1)), 2);
                        actualTotal += Math.Round(Convert.ToDouble(row.Cells[3].Value.ToString().Substring(1)), 2);
                    }
                    catch
                    {
                    }
                }
            }

            line1.Text = GetDollarFormat(RefBudgetEntry.TargetBudget - budgetTotal);
            line2.Text = GetDollarFormat(budgetTotal);
            line3.Text = GetDollarFormat(budgetTotal - actualTotal);

            line3.ForeColor = (budgetTotal - actualTotal >= 0) ? Color.LightGreen : Color.LightCoral;

            SumHeaderValues();
        }

        private void SaveCurrentBudgetEntry()
        {
            if (RefBudgetEntry == null) return;

            //RefBudgetEntry.ResetCategoryList();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (dataGridView1.Rows.IndexOf(row) < dataGridView1.Rows.Count && row.Cells[5].Value.ToString() != "HeaderRowHere")
                {
                    try
                    {
                        if (Convert.ToDouble(row.Cells[2].Value.ToString().Substring(1)) >= 0)
                        {
                            RefBudgetEntry.AddBudgetCategory(GetBCType(row.Cells[5].Value.ToString()),
                                row.Cells[0].Value.ToString().Trim(),
                                Convert.ToDouble(row.Cells[2].Value.ToString().Substring(1)));
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void SumHeaderValues()
        {
            bool isHeader = false;
            double budgetSum = 0;
            double actualSum = 0;
            double prevActualTotal = 0;

            for (int i = dataGridView1.Rows.Count - 1; i >= 0; i--)
            {
                if (dataGridView1[5, i].Value.ToString() == "HeaderRowHere")
                {
                    dataGridView1[1, i].Value = GetDollarFormat(prevActualTotal);
                    dataGridView1[2, i].Value = GetDollarFormat(budgetSum);
                    dataGridView1[3, i].Value = GetDollarFormat(actualSum);
                    dataGridView1[4, i].Value = GetDollarFormat(budgetSum - actualSum);

                    // Set bold
                    dataGridView1.Rows[i].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold);

                    // Set balance color green/red
                    DataGridViewCellStyle style = new DataGridViewCellStyle(); // highlight decrease
                    style.ForeColor = budgetSum - actualSum < 0 ? Color.LightCoral : Color.LightGreen;
                    dataGridView1[4, i].Style = style;


                    // Reset parameters
                    budgetSum = 0;
                    prevActualTotal = 0;
                    actualSum = 0;
                    isHeader = true;
                }
                else
                {
                    isHeader = false;
                }

                if (!isHeader)
                {
                    try
                    {
                        prevActualTotal += Convert.ToDouble(dataGridView1[1, i].Value.ToString().Substring(1));
                        budgetSum += Convert.ToDouble(dataGridView1[2, i].Value.ToString().Substring(1));
                        actualSum += Convert.ToDouble(dataGridView1[3, i].Value.ToString().Substring(1));
                    }
                    catch
                    {
                        // Sum error
                    }
                }
            }
        }

        private bool DGVAddRowHeader(string headerTitle)
        {
            dataGridView1.Rows.Add(headerTitle, "", "", "", "", "HeaderRowHere");

            // Set highlight
            dataGridView1.Rows[dataGridView1.RowCount - 1].DefaultCellStyle.BackColor = Color.FromArgb(0, 91, 133);
            dataGridView1[2, dataGridView1.RowCount - 1].ReadOnly = true;
            return true;
        }

        private bool DGVAddRowValue(string col1, double col2, double col3, double col4, double col5, string col6, bool indent = false)
        {
            col2 = Math.Round(col2, 2);
            col3 = Math.Round(col3, 2);
            col4 = Math.Round(col4, 2);
            col5 = Math.Round(col5, 2);

            if (col4 <= 0 && GetBCType(col6) == BCType.Recurring) return false; // Ignore rows that have 0 actual value
            prevActualTotal += col2;
            budgetTotal += col3;
            actualTotal += col4;
            dataGridView1.Rows.Add((indent ? "      " : "") + col1, GetDollarFormat(col2), GetDollarFormat(col3), GetDollarFormat(col4), GetDollarFormat(col5), col6);
            
            // Set edit color
            DataGridViewCellStyle style = new DataGridViewCellStyle(); // highlight decrease
            style.BackColor = editingBudget ? Color.DarkSlateGray : style.BackColor;
            dataGridView1[2, dataGridView1.RowCount - 1].Style = style;

            // Set balance color green/red
            style = new DataGridViewCellStyle(); // highlight decrease
            style.ForeColor = col5 < 0 ? Color.LightCoral : Color.LightGreen;
            dataGridView1[4, dataGridView1.RowCount - 1].Style = style;

            // Set highlight
            if (highlightIndex % 2 == 1)
                dataGridView1.Rows[dataGridView1.RowCount - 1].DefaultCellStyle.BackColor = Color.FromArgb(78, 78, 78);

            return true;
        }

        private void PopulateProfiles()
        {
            profileBox.Items.Clear();

            profileBox.Items.Add("None");

            foreach (string profileName in parent.GroupedCategoryList.Select(x => x._ProfileName).Distinct().OrderBy(x => x))
            {
                profileBox.Items.Add(profileName);
            }

            if (profileBox.Items.Count > 0) profileBox.SelectedIndex = 0;
        }

        // Converting month number to name
        public System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        public int setMonth = 0;
        public int setYear = 0;

        private void PopulateMonths()
        {
            Grey_Out();
            while (parent.BudgetEntryList.Count == 0)
            {
                AddNewMonth ANM = new AddNewMonth(parent, this, Location, Size);
                ANM.ShowDialog();
            }
            Grey_In();

            budgetMonth.Items.Clear();

            parent.BudgetEntryList = parent.BudgetEntryList.OrderBy(x => x.Year).ThenBy(x => x.Month).ToList();

            foreach (BudgetEntry BE in parent.BudgetEntryList)
            {
                budgetMonth.Items.Add(String.Format("{0}, {1}", mfi.GetMonthName(BE.Month), BE.Year));
            }

            int budgetMonthIndex = 0;

            if (setMonth > 0 && setYear > 0)
            {
                budgetMonthIndex =
                    parent.BudgetEntryList.IndexOf(
                        parent.BudgetEntryList.FirstOrDefault(
                            x => x.Month == setMonth && x.Year == setYear));
            }
            else
            {

                budgetMonthIndex =
                    parent.BudgetEntryList.IndexOf(
                        parent.BudgetEntryList.FirstOrDefault(
                            x => x.Month == DateTime.Now.Month && x.Year == DateTime.Now.Year));
            }

            budgetMonth.SelectedIndex = budgetMonthIndex;
            if (budgetMonthIndex < 0 && budgetMonth.Items.Count > 0)
                budgetMonth.SelectedIndex = 0;

            setMonth = 0;
            setYear = 0;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView) sender;
            if (e.RowIndex < 0)
            {
                dataGridView1.ClearSelection();
                return;
            }

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewColumn && e.ColumnIndex == 0)
            {
                string category = dataGridView1[0, e.RowIndex].Value.ToString().Trim();

                if (parent.category_box.Items.Contains(category))
                {
                    List<Item> tempItemList = parent.Master_Item_List
                        .Where(x => x.Category == category && x.Date.Month == RefBudgetEntry.Month &&
                                    x.Date.Year == RefBudgetEntry.Year).ToList();
                    if (tempItemList.Count > 0)
                    {
                        Grey_Out();
                        Expenditures g = new Expenditures(parent);
                        string Info_String = g.Parse_Dictionary_To_String(tempItemList);
                        Financial_Journal.Category_Summary FJCS =
                            new Financial_Journal.Category_Summary(category,
                                new Point(Cursor.Position.X - 5, Cursor.Position.Y - 5), Info_String, true);
                        FJCS.ShowDialog();
                        Grey_In();
                    }
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
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
            SaveCurrentBudgetEntry();
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

        private void viewGroupManager_Click(object sender, EventArgs e)
        {
            Grey_Out();
            CategoryGrouper CG = new CategoryGrouper(parent, Location, Size);
            CG.ShowDialog();
            Grey_In();
            PopulateProfiles();
        }

        private void addBudgetMonth_Click(object sender, EventArgs e)
        {
            Grey_Out();
            AddNewMonth ANM = new AddNewMonth(parent, this, Location, Size);
            ANM.ShowDialog();
            Grey_In();
            PopulateMonths();
        }

        private void budgetMonth_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveCurrentBudgetEntry();
            RefBudgetEntry = parent.BudgetEntryList[budgetMonth.SelectedIndex];
            PopulateDGV();
        }

        public string GetDollarFormat(double amt)
        {
            amt = Math.Round(amt, 2);
            if (amt == 0) return "$0"; // Ignore extra 0.00 trailing.
            if (amt < 0) return "-$" + string.Format("{0:0.00}", Math.Abs(amt));
            else return "$" + String.Format("{0:0.00}", amt);
        }

        private void profileBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            setDefault.Visible = parent.Settings_Dictionary["BUDDEFPROF"] != profileBox.Text;
            PopulateDGV();
        }

        private BCType GetBCType(string bcTypeStr)
        {
            switch (bcTypeStr)
            {
                case "Categorical":
                {
                    return BCType.Categorical;
                }
                case "Recurring":
                {
                    return BCType.Recurring;
                }
                case "Extraneous":
                {
                    return BCType.Extraneous;
                }
            }
            return BCType.Categorical;
        }

        private void recalculate_Click(object sender, EventArgs e)
        {
            SaveCurrentBudgetEntry();
            PopulateDGV();
        }

        private void PredictBudget(int usingXMonths, bool excludeZeroValues = true)
        {
            RefBudgetEntry.ResetCategoryList();

            // predict category values
            foreach (string category in parent.category_box.Items)
            {
                RefBudgetEntry.SetBudgetForBc(BCType.Categorical, category, 
                    GetCategoryAvg(BCType.Categorical, category, usingXMonths, new DateTime(RefBudgetEntry.Year, RefBudgetEntry.Month, 1), excludeZeroValues));
            }

            // predict recurring expense values
            foreach (string category in parent.Expenses_List.Select(x => x.Expense_Name))
            {
                DateTime Ref_Date = new DateTime(RefBudgetEntry.Year, RefBudgetEntry.Month, 1);
                double actualValue = parent.Expenses_List.First(x => x.Expense_Name == category)
                    .Get_Total_Paid(Ref_Date.AddMonths(0), Ref_Date.AddMonths(1));
                RefBudgetEntry.SetBudgetForBc(BCType.Recurring, category, actualValue);

                //GetCategoryAvg(BCType.Recurring, category, usingXMonths, DateTime.Now)); // Depreciated (using exact value)
            }

            PopulateDGV();
            SaveCurrentBudgetEntry();
        }

        private double GetCategoryAvg(BCType bcType, string category, int months, DateTime refDate, bool excludeZeroes = true)
        {
            int zeroCount = 0;
            double CategoryTotal = 0;
            refDate = refDate.AddMonths(-months);
            switch (bcType)
            {
                case BCType.Categorical:
                {
                    for (int i = 0; i < months; i++)
                    {
                        DateTime monthRefDate = refDate.AddMonths(i);
                        double currentMonth = parent.Master_Item_List.Where(x => x.Category == category && x.Date.Month == monthRefDate.Month && x.Date.Year == monthRefDate.Year).ToList().Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));

                        if (currentMonth <= 0 && excludeZeroes) zeroCount++; // To remove the zero values from avg count

                        CategoryTotal += currentMonth;
                    }
                    break;
                }
                case BCType.Recurring:
                {
                    
                    for (int i = 0; i < months; i++)
                    {
                        DateTime monthRefDate = refDate.AddMonths(i);
                        double currentMonth = parent.Expenses_List.First(x => x.Expense_Name == category)
                            .Get_Total_Paid(monthRefDate.AddMonths(-1), monthRefDate);


                        if (currentMonth <= 0 && excludeZeroes) zeroCount++; // To remove the zero values from avg count

                        CategoryTotal += currentMonth;
                    }
                    break;
                }
            }

            if (months == zeroCount) return 0;

            return CategoryTotal / (months - zeroCount);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            refBudgetText = "";
            Grey_Out();
            using (var form = new PredictModeDialog(parent, this, Location, Size))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (refBudgetText == "")
                        PredictBudget(form.monthPredictionCount, !form.includeZeroValues.Checked);
                    else
                    {
                        BudgetEntry tempBudgetEntry = parent.BudgetEntryList[budgetMonth.Items.IndexOf(refBudgetText)];

                        // set all to 0 value first before setting to current 
                        RefBudgetEntry.GetCategoryList().ForEach(x => x.TargetAmount = 0);

                        foreach (BudgetCategory BC in tempBudgetEntry.GetCategoryList())
                        {
                            if (BC.GetBCType() != BCType.Recurring)
                            {
                                RefBudgetEntry.SetBudgetForBc(BC.GetBCType(), BC.GetName(),
                                    tempBudgetEntry.GetBudgetAmount(BC.GetBCType(), BC.GetName()));
                            }
                            else // use exact recurring amount
                            {
                                DateTime Ref_Date = new DateTime(RefBudgetEntry.Year, RefBudgetEntry.Month, 1);

                                double actualValue = parent.Expenses_List.First(x => x.Expense_Name == BC.GetName())
                                    .Get_Total_Paid(Ref_Date.AddMonths(0), Ref_Date.AddMonths(1));

                                RefBudgetEntry.SetBudgetForBc(BCType.Recurring, BC.GetName(), actualValue);

                                //RefBudgetEntry.SetBudgetForBc(BCType.Recurring, BC.GetName(),
                                //    tempBudgetEntry.GetBudgetAmount(BC.GetBCType(), BC.GetName()));
                            }
                        }

                        PopulateDGV();
                        SaveCurrentBudgetEntry();
                    }
                }
            }
            Grey_In();
        }

        bool editingBudget = false;

        private void button3_Click(object sender, EventArgs e)
        {
            editingBudget = !editingBudget;
            dataGridView1.Columns[2].ReadOnly = !editingBudget;

            if (editingBudget)
            {

                label4.Text = "DISABLE";
                label4.Left -= 2;
                button3.Image = Financial_Journal.Properties.Resources.edit_on;
            }
            else
            {
                label4.Text = "ENABLE";
                label4.Left += 2;
                button3.Image = Financial_Journal.Properties.Resources.edit_off;
            }

            // Save on disable
            if (!editingBudget) SaveCurrentBudgetEntry();

            // Refresh view
            PopulateDGV();

        }

        private void addExtraneous_Click(object sender, EventArgs e)
        {

            Grey_Out();
            using (var form = new Input_Box_Small(parent, "Please provide a name for your extra category", "",
                "Add", null, this.Location, this.Size, 25, true))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string ItemName = form.Pass_String;

                    if (!parent.category_box.Items.Contains(ItemName) &&
                        !parent.Expenses_List.Any(x => x.Expense_Name.ToLower().Contains(ItemName.ToLower())))
                    {
                        RefBudgetEntry.AddBudgetCategory(BCType.Extraneous, ItemName, 0);
                        PopulateDGV();
                        SaveCurrentBudgetEntry();
                        Form_Message_Box FMB = new Form_Message_Box(parent, String.Format("Successfully added '{0}'. Please remember to allocate a value to '{0}' or else it will be removed after dialog closes", ItemName), true, 10, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    else
                    {
                        Form_Message_Box FMB = new Form_Message_Box(parent, "Error: An existing category/expense with that name already exists", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                }
            }
            Grey_In();
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new MonthlyAmountDialog(parent, this, Location, Size))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    RefBudgetEntry.TargetBudget = form.returnAmount;
                }
            }
            Grey_In();
            PopulateDGV();
        }

        private void excel_button_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new excelAll(parent, Location, Size))
            {
                form.ShowDialog();
                if (form.DialogResult == DialogResult.OK)
                {
                    ExportTable(form.returnExportType);
                }
            }
            Grey_In();
        }

        private void ExportTable(ExportType exportType)
        {
            Diagnostics.WriteLine(String.Format("Exporting via: {0}", exportType));
            switch (exportType)
            {
                case ExportType.Print:
                {
                    index = 0;
                    printDocument1.Print();
                    break;
                }
                case ExportType.Preview:
                {
                    index = 0;
                    printPreviewDialog1.TopMost = true;
                    printPreviewDialog1.ShowDialog();
                    break;
                }
                case ExportType.PDFCurrent:
                {
                    PDFGenerator_Budget PDFB = new PDFGenerator_Budget(this);
                    break;
                }
                case ExportType.PDFAll:
                {
                    break;
                }
                case ExportType.ExcelCurrent:
                {
                    Grey_Out();

                    if (secondThreadFormHandle == IntPtr.Zero)
                    {
                        Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size)
                        {
                        };
                        form.HandleCreated += SecondFormHandleCreated;
                        form.HandleDestroyed += SecondFormHandleDestroyed;
                        form.RunInNewThread(false);
                    }

                    ExcelGenerator_Budget EXLB = new ExcelGenerator_Budget(this);
                    EXLB.Write_Excel_Current();
                    break;
                }
                case ExportType.ExcelAll:
                {
                    Grey_Out();

                    if (secondThreadFormHandle == IntPtr.Zero)
                    {
                        Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size)
                        {
                        };
                        form.HandleCreated += SecondFormHandleCreated;
                        form.HandleDestroyed += SecondFormHandleDestroyed;
                        form.RunInNewThread(false);
                    }

                    ExcelGenerator_Budget EXLB = new ExcelGenerator_Budget(this);
                    EXLB.Write_Excel_All(parent.BudgetEntryList);
                    break;
                }
            }

            if (secondThreadFormHandle != IntPtr.Zero)
                PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            Grey_In();
        }

        private int index = 0;

        private void printDocument1_PrintPage(System.Object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            int column1 = 35;            // Category/SubHeader
            int column2 = column1 + 280; // Prev Month Actual
            int column3 = column2 + 145; // Budget
            int column4 = column3 + 125; // Actual
            int column5 = column4 + 125; // Balance
            int starty = 10;
            int dataheight = 15;
            int height = starty + starty;

            StringFormat format1 = new StringFormat();
            format1.Alignment = StringAlignment.Center;

            Pen p = new Pen(Brushes.Black, 2.5f);
            Font f2 = new Font("MS Reference Sans Serif", 9f);
            Font f3 = new Font("MS Reference Sans Serif", 10f);
            Font f4 = new Font("MS Reference Sans Serif", 10f, FontStyle.Bold);
            Font f5 = new Font("MS Reference Sans Serif", 9f, FontStyle.Italic);
            Font f1 = new Font("MS Reference Sans Serif", 10F, FontStyle.Bold);

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
                e.Graphics.DrawString(String.Format("BUDGET ALLOCATION REPORT - {0}", budgetMonth.Text.ToUpper()), f1, Brushes.Black, new Rectangle(10, height, 650, dataheight * 2));
                height += dataheight;
                height += 5;
                e.Graphics.DrawLine(Grey_Pen, 10, height, 850 - column1, height);
                e.Graphics.DrawString(String.Format("Total amount available for budgetting: {0}", line1.Text), f3, Brushes.Black, new Rectangle(12, height, 650, dataheight + 10));
                height += dataheight;
                e.Graphics.DrawString(String.Format("Total amount budgetted: {0}", line2.Text), f3, Brushes.Black, new Rectangle(12, height, 650, dataheight + 10));
                height += dataheight;
                e.Graphics.DrawString(String.Format("Current budget vs. total amount: {0}", line3.Text), f3, Brushes.Black, new Rectangle(12, height, 650, dataheight + 10));
                height += dataheight;
                height += 3;
                e.Graphics.DrawLine(Grey_Pen, 10, height, 850 - column1, height);

                height += dataheight;
            }

            e.Graphics.DrawString("Prev. Actual", f1, Brushes.Black, new Rectangle(column2, height, 650, dataheight + 10));
            e.Graphics.DrawString("Budget", f1, Brushes.Black, new Rectangle(column3, height, 650, dataheight + 10));
            e.Graphics.DrawString("Actual", f1, Brushes.Black, new Rectangle(column4, height, 650, dataheight + 10));
            e.Graphics.DrawString("Balance", f1, Brushes.Black, new Rectangle(column5, height, 650, dataheight + 10));
            height += dataheight;
            height += 3;
            e.Graphics.DrawLine(Grey_Pen, column2, height, 850 - column1, height);
            height += 3;
            height += dataheight;

            while (index < dataGridView1.Rows.Count)
            {
                string row1 = dataGridView1[0, index].Value.ToString();
                string row2 = dataGridView1[1, index].Value.ToString();
                string row3 = dataGridView1[2, index].Value.ToString();
                string row4 = dataGridView1[3, index].Value.ToString();
                string row5 = dataGridView1[4, index].Value.ToString();
                string row6 = dataGridView1[5, index].Value.ToString();

                if (height > e.MarginBounds.Height + 50) // + 20)
                {
                    height = starty;
                    e.HasMorePages = true;
                    // header
                    return;
                }

                if (row6.Contains("HeaderRowHere"))
                {
                    height += (index > 0 ? dataheight : 0);
                    e.Graphics.DrawString(row1, f1, Brushes.Black, new Rectangle(column1, height, 650, dataheight + 10));
                    e.Graphics.DrawString(row2, f1, Brushes.Black, new Rectangle(column2, height, 650, dataheight + 10));
                    e.Graphics.DrawString(row3, f1, Brushes.Black, new Rectangle(column3, height, 650, dataheight + 10));
                    e.Graphics.DrawString(row4, f1, Brushes.Black, new Rectangle(column4, height, 650, dataheight + 10));
                    e.Graphics.DrawString(row5, f1, Brushes.Black, new Rectangle(column5, height, 650, dataheight + 10));
                    height += 4;
                    height += dataheight;
                    height += 3;
                    e.Graphics.DrawLine(Grey_Pen, column1, height, 850 - column1, height);
                    height += 3;
                    index++;
                }
                else
                {
                    e.Graphics.DrawString(row1, f2, Brushes.Black, new Rectangle(column1, height, 650, dataheight));
                    e.Graphics.DrawString(row2, f2, Brushes.Black, new Rectangle(column2, height, 650, dataheight));
                    e.Graphics.DrawString(row3, f2, Brushes.Black, new Rectangle(column3, height, 650, dataheight));
                    e.Graphics.DrawString(row4, f2, Brushes.Black, new Rectangle(column4, height, 650, dataheight));
                    e.Graphics.DrawString(row5, f2, Brushes.Black, new Rectangle(column5, height, 650, dataheight));

                    height += dataheight;

                    height += 3;
                    e.Graphics.DrawLine(Grey_Pen, column1 + (profileBox.Text != "None" ? 25 : 0), height, 850 - column1, height);
                    height += 3;

                    index++;
                }


            }
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

        private void button4_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Yes_No_Dialog(parent,
                String.Format("Are you sure you wish to delete the budget for {0}?", budgetMonth.Text), "Warning", "No", "Yes", 0,
                Location, Size))
            {
                var result2 = form.ShowDialog();
                if (result2 == DialogResult.OK && form.ReturnValue1 == "1")
                {
                    parent.BudgetEntryList.Remove(RefBudgetEntry);
                    PopulateDGV();
                    SaveCurrentBudgetEntry();
                    PopulateMonths();
                }
            }
            Grey_In();
        }

        private void setDefault_Click(object sender, EventArgs e)
        {
            setDefault.Visible = false;
            parent.Settings_Dictionary["BUDDEFPROF"] = profileBox.Text;
        }
    }
}
