using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace Financial_Journal
{
    public partial class Cash_Flow_Spreadsheet : Form
    {

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public Cash_Flow_Spreadsheet(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            //this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {

            for (int i = 1; i < 13; i++)
            {
                from_month.Items.Add(mfi.GetMonthName(i));
                to_month.Items.Add(mfi.GetMonthName(i));
            }

            // Add years to box (only get the years where purchases have been made)
            List<string> Years = new List<string>();
            foreach (Order order in parent.Order_List)
            {
                if (!Years.Contains(order.Date.Year.ToString()))
                {
                    Years.Add(order.Date.Year.ToString());
                }
            }

            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            ToolTip1.SetToolTip(group_all, "Select All");
            ToolTip1.SetToolTip(reset, "Deselect All");

            Years = Years.OrderBy(x => Convert.ToInt32(x)).ToList();
            Years.ForEach(x => from_year.Items.Add(x));
            Years.ForEach(x => to_year.Items.Add(x));

            from_month.Text = mfi.GetMonthName(DateTime.Now.Month);
            to_year.Text = DateTime.Now.Year.ToString();
            from_year.Text = from_year.Items.Contains((DateTime.Now.Year - 1).ToString()) ? (DateTime.Now.Year - 1).ToString() : (DateTime.Now.Year).ToString();
            to_month.Text = mfi.GetMonthName(DateTime.Now.Month);

            Calculate_Months();

            // Add payment options
            foreach (Payment payment in parent.Payment_List)
            {
                cmbManual.Items.Add(payment.ToString());
            }

            // Select all if nothing previously chosen
            if (parent.Settings_Dictionary["CASHFLOW_PAYMENT_ACCS"].Length < 2)
            {
                for (int i = 0; i < cmbManual.Items.Count; i++)
                {
                    cmbManual.CheckBoxItems[i].Checked = true;
                }
            }
            else
            {
                for (int i = 0; i < cmbManual.Items.Count; i++)
                {
                    cmbManual.CheckBoxItems[i].Checked = parent.Settings_Dictionary["CASHFLOW_PAYMENT_ACCS"].Contains(cmbManual.CheckBoxItems[i].Text);
                }
            }

            //excel_button.PerformClick();
            //Environment.Exit(0);
            combine_GC_.Checked = parent.Settings_Dictionary["CASHFLOW_GROUP_GC"] == "1";
            show_percent.Checked = parent.Settings_Dictionary["CASHFLOW_SHOW_PERCENT"] == "1";
            show_value.Checked = parent.Settings_Dictionary["CASHFLOW_SHOW_VALUE"] == "1";
            checkBox1.Checked = parent.Settings_Dictionary["CASHFLOW_SHOW_PERCENT_VS_TOTAL"] == "1";
            checkBox2.Checked = parent.Settings_Dictionary["CASHFLOW_IGNORE_ZERO_VALUE"] == "1";

            Ignore_Zero_Value = parent.Settings_Dictionary["CASHFLOW_IGNORE_ZERO_VALUE"] == "1"; ;
            Show_Comparison_Values = parent.Settings_Dictionary["CASHFLOW_SHOW_VALUE"] == "1"; ;
            Show_Comparison_Percentages = parent.Settings_Dictionary["CASHFLOW_SHOW_PERCENT"] == "1"; ;
            Show_Percentages_vs_Total = parent.Settings_Dictionary["CASHFLOW_SHOW_PERCENT_VS_TOTAL"] == "1"; ;
            Combine_GC = parent.Settings_Dictionary["CASHFLOW_GROUP_GC"] == "1"; ;

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

            TFLP.Opacity = 90;

            Loaded = true;
        }

        bool Loaded = false;

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

        private void Grey_Out()
        {
            TFLP.Location = new Point(1, 1);
        }

        private void Grey_In()
        {
            TFLP.Location = new Point(1000, 1000);
        }

        private void WriteHeader(ref Excel.Worksheet sheet, ref int row)
        {
            // header
            int col = 3;

            for (int i = 1; i < Month_Count + 1; i++)
            {
                sheet.Cells[row, col++] = Get_Date(i);

                // If not first month (nothing to compare to)
                if (Show_Percentages_vs_Total)
                {
                    sheet.Cells[row, col++] = "% of Total";// prev. mth";
                }
                if (Show_Comparison_Values && i > 1)
                    sheet.Cells[row, col++] = "Δ$";// prev. mth";
                if (Show_Comparison_Percentages && i > 1)
                    sheet.Cells[row, col++] = "Δ%";// prev. mth";
            }
            sheet.Cells[row, col++] = "Range Total";
            if (Show_Percentages_vs_Total) sheet.Cells[row, col++] = "% of Total";// prev. mth";

            Excel.Range range = sheet.Cells.get_Range("A" + row.ToString(), Get_Column_Letter(col) + row.ToString());
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.Font.ColorIndex = 48;
            range.Cells.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            row++;
        }

        private void WriteSpreadsheet(ref Excel.Worksheet sheet, ref int row)
        {
            int col = 1;
            double prev_month_value = 0;
            double row_total = 0;

            int Total_Column = (Month_Count - 1) * (1 + (Show_Percentages_vs_Total ? 1 : 0) + (Show_Comparison_Percentages ? 1 : 0) + (Show_Comparison_Values ? 1 : 0)) + 5 + (Show_Percentages_vs_Total ? 2 : 0);

            int Asset_Total_Line = 0;
            int Liability_Total_Line = 0;

            sheet.Cells[row, col] = "ASSETS";
            sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
            sheet.Cells.get_Range("A" + row.ToString()).Interior.ColorIndex = 42;
            row++;

            #region Personal Income
            sheet.Cells[row, col++] = "Personal Income";
            sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
            col++;      
            for (int i = 1; i < Month_Count + 1; i++)
            {
                // Get Dynamic Income
                using (Savings_Helper SH = new Savings_Helper(parent))
                {
                    double current_month_value = SH.Get_Monthly_Salary(Get_Date_DateTime(i).Month, Get_Date_DateTime(i).Year);
                    sheet.Cells[row, col++] = Format_Currency(current_month_value);

                    // If not first month (nothing to compare to)
                    if (Show_Percentages_vs_Total)
                        col++;
                    if (Show_Comparison_Values && i > 1)
                        sheet.Cells[row, col++] = Format_Currency(current_month_value - prev_month_value);
                    if (Show_Comparison_Percentages && i > 1)
                        sheet.Cells[row, col++] = Format_Percentage((current_month_value - prev_month_value) / Math.Abs(prev_month_value));

                    prev_month_value = current_month_value;
                    row_total += current_month_value;
                }
            }
            sheet.Cells[row, col++] = Format_Currency(row_total);

            row++; col = 1; prev_month_value = 0; row_total = 0;
            #endregion

            #region Investments
            if (parent.Investment_List.Count > 0)
            {
                sheet.Cells[row, col++] = "Investments Interest Earnings:";
                sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                row++;
                foreach (Investment IV in parent.Investment_List.OrderByDescending(x => x.Get_Amt_Since_Period_Start(DateTime.Now)).ToList())
                {
                    sheet.Cells[row, col++] = IV.Name;
                    sheet.Cells.get_Range("B" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    for (int i = 1; i < Month_Count + 1; i++)
                    {

                        DateTime Ref_Date = Get_Date_DateTime(i, true);

                        // We want interest difference from last month
                        double current_month_value = IV.Get_Amt_Since_Period_Start(Ref_Date) - IV.Get_Amt_Since_Period_Start(Ref_Date.AddMonths(-1));
                        sheet.Cells[row, col++] = Format_Currency(current_month_value, current_month_value == 0);

                        // If not first month (nothing to compare to)
                        if (Show_Percentages_vs_Total)
                            col++;
                        if (Show_Comparison_Values && i > 1)
                            sheet.Cells[row, col++] = Format_Currency(current_month_value - prev_month_value, current_month_value == 0);
                        if (Show_Comparison_Percentages && i > 1)
                            sheet.Cells[row, col++] = Format_Percentage((current_month_value - prev_month_value) / Math.Abs(prev_month_value), current_month_value == 0);

                        prev_month_value = current_month_value;
                        row_total += current_month_value;

                    }
                    sheet.Cells[row, col++] = Format_Currency(row_total);
                    row++; col = 2; prev_month_value = 0; row_total = 0;
                }
                col = 1; prev_month_value = 0; row_total = 0;
            }

            #endregion

            #region GCs
            if (Combine_GC)
            {
                sheet.Cells[row, col++] = "Aggregate Gift Cards";
                sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                col++;

                prev_month_value = 0;

                for (int i = 1; i < Month_Count + 1; i++)
                {
                    DateTime Ref_Date = Get_Date_DateTime(i);

                    // Keep track of decuctions
                    double deductions = 0;
                    double current_month_value = 0;
                    double orig_sum = 0;

                    foreach (GC GCard in parent.GC_List.Where(x => x.Date_Added.AddMonths(-1) < Ref_Date).ToList())
                    {
                        // Get GC orig sum
                        orig_sum += GCard.Amount;
                        for (int j = 1; j < GCard.Associated_Orders.Count; j += 2) orig_sum += Convert.ToDouble(GCard.Associated_Orders[j]);

                        // Get GC Deductions based on orders with same order date as current ref date
                        for (int j = 0; j < GCard.Associated_Orders.Count; j += 2)
                        {
                            Order Ref_Order = parent.Order_List.FirstOrDefault(x => x.OrderID == GCard.Associated_Orders[j] && x.Date <= Ref_Date);
                            deductions += Ref_Order == null ? 0 : Convert.ToDouble(GCard.Associated_Orders[j + 1]);
                        }

                    }
                    current_month_value = orig_sum - deductions;
                    sheet.Cells[row, col++] = Format_Currency(current_month_value);

                    // If not first month (nothing to compare to)
                    if (Show_Percentages_vs_Total)
                        col++;
                    if (Show_Comparison_Values && i > 1)
                        sheet.Cells[row, col++] = Format_Currency(current_month_value - prev_month_value);
                    if (Show_Comparison_Percentages && i > 1)
                        sheet.Cells[row, col++] = Format_Percentage((current_month_value - prev_month_value) / Math.Abs(prev_month_value));

                    prev_month_value = current_month_value;
                }
                sheet.Cells[row, col++] = Format_Currency(prev_month_value);
                row++; col = 2; prev_month_value = 0; row_total = 0;

            }
            else
            {
                sheet.Cells[row, col++] = "Gift Cards:";
                sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                row++;

                foreach (GC GCard in parent.GC_List)
                {
                    double ongoingGCAmount = 0; // to check if all zero values in spreadsheet; if so, ignore GC 

                    prev_month_value = 0; row_total = 0;

                    // Get GC orig sum
                    double orig_sum = GCard.Amount;
                    for (int i = 1; i < GCard.Associated_Orders.Count; i += 2) orig_sum += Convert.ToDouble(GCard.Associated_Orders[i]);

                    sheet.Cells[row, col++] = GCard.Location + " ($" + String.Format("{0:0.00}", orig_sum) + ")";
                    sheet.Cells.get_Range("B" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    // Keep track of decuctions
                    double deductions = 0;

                    for (int i = 1; i < Month_Count + 1; i++)
                    {
                        DateTime Ref_Date = Get_Date_DateTime(i);

                        // Get GC Deductions based on orders with same order date as current ref date
                        for (int j = 0; j < GCard.Associated_Orders.Count; j += 2)
                        {
                            Order Ref_Order = parent.Order_List.FirstOrDefault(x => x.OrderID == GCard.Associated_Orders[j] && x.Date.Month == Ref_Date.Month && x.Date.Year == Ref_Date.Year);
                            deductions += Ref_Order == null ? 0 : Convert.ToDouble(GCard.Associated_Orders[j + 1]);
                        }

                        double current_month_value = Ref_Date < GCard.Date_Added ? 0 : (orig_sum - deductions);
                        ongoingGCAmount += Ref_Date < GCard.Date_Added ? 0 : (orig_sum - deductions);
                        //double current_month_value = (orig_sum - deductions);

                        bool Dash_Format = Ref_Date < GCard.Date_Added;
                        sheet.Cells[row, col++] = Format_Currency(current_month_value, Dash_Format);

                        // If not first month (nothing to compare to)
                        if (Show_Percentages_vs_Total)
                            col++;
                        if (Show_Comparison_Values && i > 1)
                            sheet.Cells[row, col++] = Format_Currency(current_month_value - prev_month_value, Dash_Format);
                        if (Show_Comparison_Percentages && i > 1)
                            sheet.Cells[row, col++] = Format_Percentage((current_month_value - prev_month_value) / Math.Abs(prev_month_value), Dash_Format);

                        prev_month_value = current_month_value;

                    }
                    //sheet.Cells[row, col++] = Format_Currency(GCard.Amount);
                    sheet.Cells[row, col++] = Format_Currency(prev_month_value);

                    if (ongoingGCAmount == 0) row--;

                    row++; col = 2; prev_month_value = 0; row_total = 0;
                }
            }

            col = 1;
            #endregion

            #region Payment Accounts

            // Get payments according to selected index
            string[] Payments_Selected = cmbManual.Text.Split(new string[] { ", " }, StringSplitOptions.None);
            List<Payment> Filtered_Payment_List = parent.Payment_List.Where(x => Payments_Selected.Contains(x.ToString())).ToList();

            // If any payment is selected, then add to spreadsheet
            if (Filtered_Payment_List.Count > 0)
            {

                sheet.Cells[row, col++] = "Payment Accounts:";
                sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                row++;

                foreach (Payment payment in Filtered_Payment_List)
                {

                    sheet.Cells[row, col++] = payment.ToString();
                    sheet.Cells.get_Range("B" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    for (int i = 1; i < Month_Count + 1; i++)
                    {

                        DateTime Ref_Date = Get_Date_DateTime(i);

                        double current_month_value = payment.Get_Final_Balance_Month(Ref_Date, parent.Payment_Options_List);

                        sheet.Cells[row, col++] = Format_Currency(current_month_value);

                        // If not first month (nothing to compare to)
                        if (Show_Percentages_vs_Total)
                            col++;
                        if (Show_Comparison_Values && i > 1)
                            sheet.Cells[row, col++] = Format_Currency(current_month_value - prev_month_value);
                        if (Show_Comparison_Percentages && i > 1)
                            sheet.Cells[row, col++] = Format_Percentage((current_month_value - prev_month_value) / Math.Abs(prev_month_value));

                        prev_month_value = current_month_value;
                        row_total += current_month_value;
                    }
                    sheet.Cells[row, col++] = Format_Currency(prev_month_value);

                    row++; col = 2; prev_month_value = 0;
                }

                col = 1; row_total = 0;
            }

            #endregion

            /*
            #region Accounts Receivable
            sheet.Cells[row, col++] = "Accounts Receivable";
            sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
            col++;
            for (int i = 1; i < Month_Count + 1; i++)
            {
                DateTime Ref_Date = Get_Date_DateTime(i);

                double current_month_value = parent.Account_List.Where(x => x.Type == "Receivable" &&
                                                                       x.Start_Date <= Ref_Date &&
                                                                       x.Inactive_Date > Ref_Date.AddMonths(-1)).Sum(x => Convert.ToDouble(x.Amount.Substring(1)));

                sheet.Cells[row, col++] = Format_Currency(current_month_value);

                // If not first month (nothing to compare to)
                if (Show_Percentages_vs_Total)
                    col++;
                if (Show_Comparison_Values && i > 1)
                    sheet.Cells[row, col++] = Format_Currency(current_month_value - prev_month_value);
                if (Show_Comparison_Percentages && i > 1)
                    sheet.Cells[row, col++] = Format_Percentage((current_month_value - prev_month_value) / Math.Abs(prev_month_value));

                prev_month_value = current_month_value;
            }
            sheet.Cells[row, col++] = Format_Currency(prev_month_value);

            row++; col = 1; prev_month_value = 0; row_total = 0;
            #endregion
    */

            #region Total Assets Line

            row++;
            sheet.Cells[row, col] = "TOTAL ASSETS";
            Asset_Total_Line = row;
            sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            sheet.Cells.get_Range("A" + (row).ToString(), Get_Column_Letter(Total_Column) + (row).ToString()).Font.Bold = true;

            //int g = (Month_Count - 1) * (1 + (Show_Comparison_Percentages ? 1 : 0) + (Show_Comparison_Values ? 1 : 0)) + 3;
            //Diagnostics.WriteLine(g);

            //for (int i = 1; i < Month_Count * (1 + (Show_Comparison_Percentages ? 1 : 0) + (Show_Comparison_Values ? 1 : 0)) + 2; i++)
            for (int i = 1; i < Total_Column; i++)
            {
                if (i > 2)
                {
                    // sum the lines
                    var cellValue = (sheet.Cells[3, i] as Excel.Range).Value;
                    // If percentage
                    if (cellValue.GetType() == typeof(string) && (string)(sheet.Cells[3, i] as Excel.Range).Value == "Δ%")
                    {
                        // Get correct rows
                        int col1 = i - (Show_Comparison_Values ? 5 : 3);
                        int col2 = i - (Show_Comparison_Values ? 2 : 1);
                        if (col1 < 3) col1 = 3; // No comparison first month so compensate for that
                        sheet.Cells[row, i] = "=(" + Get_Column_Letter(col2) + row + "-" + Get_Column_Letter(col1) + row + ")/" + Get_Column_Letter(col1) + row;
                        sheet.Range[Get_Column_Letter(i) + row].NumberFormat = "[<>0]#,##0.00%;[=0]0%";
                    }
                    else // Not percentage
                    {
                        sheet.Cells[row, i] = "=SUM(" + Get_Column_Letter(i) + "4:" + Get_Column_Letter(i) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(i) + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)";
                    }
                }
                sheet.Cells.get_Range(Get_Column_Letter(i) + row.ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                sheet.Cells.get_Range(Get_Column_Letter(i) + (row + 2).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            }
            row++;
            #endregion

            #region Calculate percentage vs total assets
            if (Show_Percentages_vs_Total)
            {
                for (int i = 3; i < Total_Column; i++)
                {
                    var cellValue = (sheet.Cells[3, i] as Excel.Range).Value;
                    if (cellValue.GetType() == typeof(string) && (string)(sheet.Cells[3, i] as Excel.Range).Value == "% of Total")
                    {
                        for (int j = 4; j < Asset_Total_Line + 1; j++)
                        {
                            var intCellValue = (sheet.Cells[j, i - 1] as Excel.Range).Value;
                            if (intCellValue != null && intCellValue.GetType() == typeof(decimal))
                            {
                                sheet.Cells[j, i] = "=" + Get_Column_Letter(i - 1) + j + "/" + Get_Column_Letter(i - 1) + Asset_Total_Line;
                                sheet.Range[Get_Column_Letter(i) + j].NumberFormat = "[<>0]#,##0.00%;[=0]0%";
                            }
                            else if (intCellValue != null && intCellValue.GetType() == typeof(string) && (string)(sheet.Cells[j, i - 1] as Excel.Range).Value == "-")
                            {
                                sheet.Cells[j, i] = "-";
                            }
                        }
                    }
                }
            }
            #endregion

            int Liabilities_Line = row++;

            sheet.Cells[row, col] = "LIABILITIES";
            sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
            sheet.Cells.get_Range("A" + row.ToString()).Interior.ColorIndex = 42;
            row++;

            #region Recurring Expenses
            sheet.Cells[row, col++] = "Recurring Expenses";
            sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
            row++;

            foreach (Expenses Expense in parent.Expenses_List)
            {
                sheet.Cells[row, col++] = Expense.Expense_Name;
                sheet.Cells.get_Range("B" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                for (int i = 1; i < Month_Count + 1; i++)
                {
                    DateTime Ref_Date = Get_Date_DateTime(i).AddDays(1);

                    double current_month_value = Expense.Get_Total_Paid(Ref_Date.AddMonths(-1), Ref_Date);
                    //sheet.Cells[row, col++] = Format_Currency(current_month_value, !Expense.Check_Expense_Active_Date(Ref_Date));
                    sheet.Cells[row, col++] = Format_Currency(current_month_value, current_month_value <= 1);

                    // If not first month (nothing to compare to)
                    if (Show_Percentages_vs_Total)
                        col++;
                    if (Show_Comparison_Values && i > 1)
                        sheet.Cells[row, col++] = Format_Currency(current_month_value - prev_month_value, current_month_value <= 1);
                    if (Show_Comparison_Percentages && i > 1)
                        sheet.Cells[row, col++] = Format_Percentage((current_month_value - prev_month_value) / Math.Abs(prev_month_value), current_month_value <= 1);

                    prev_month_value = current_month_value;
                    row_total += current_month_value;
                }
                sheet.Cells[row, col++] = Format_Currency(row_total);
                row++; col = 2; prev_month_value = 0; row_total = 0;
            }

            col = 1;
            #endregion

            #region Categorical Expenditure
            sheet.Cells[row, col++] = "Categorical Expenditure";
            sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
            row++;

            foreach (string Category in parent.category_box.Items)
            {
                sheet.Cells[row, col++] = Category;
                sheet.Cells.get_Range("B" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                for (int i = 1; i < Month_Count + 1; i++)
                {
                    DateTime Ref_Date = Get_Date_DateTime(i);

                    double current_month_value = parent.Master_Item_List.Where(x => x.Category == Category && x.Date.Month == Ref_Date.Month && x.Date.Year == Ref_Date.Year).ToList().Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));

                    row_total += current_month_value;

                    //sheet.Cells[row, col++] = Format_Currency(current_month_value, !Expense.Check_Expense_Active_Date(Ref_Date));
                    sheet.Cells[row, col++] = Format_Currency(current_month_value, row_total <= 1);

                    // If not first month (nothing to compare to)
                    if (Show_Percentages_vs_Total)
                        col++;
                    if (Show_Comparison_Values && i > 1)
                        sheet.Cells[row, col++] = Format_Currency(current_month_value - prev_month_value, row_total <= 1);
                    if (Show_Comparison_Percentages && i > 1)
                        sheet.Cells[row, col++] = Format_Percentage((current_month_value - prev_month_value) / Math.Abs(prev_month_value), row_total <= 1);

                    prev_month_value = current_month_value;
                }
                sheet.Cells[row, col++] = Format_Currency(row_total);
                if (Ignore_Zero_Value && row_total == 0) row--;
                row++; col = 2; prev_month_value = 0; row_total = 0;
            }

            col = 1;
            #endregion

            #region Accounts Payable
            sheet.Cells[row, col++] = "Accounts Payable";
            sheet.Cells[row, col++] = "";
            sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;

            for (int i = 1; i < Month_Count + 1; i++)
            {
                DateTime Ref_Date = Get_Date_DateTime(i);

                double current_month_value = parent.Account_List.Where(x => x.Type == "Payable" &&
                                                                       x.Start_Date <= Ref_Date &&
                                                                       x.Inactive_Date > Ref_Date.AddMonths(-1)).Sum(x => Convert.ToDouble(x.Amount.Substring(1)));

                sheet.Cells[row, col++] = Format_Currency(current_month_value);

                // If not first month (nothing to compare to)
                if (Show_Percentages_vs_Total)
                    col++;
                if (Show_Comparison_Values && i > 1)
                    sheet.Cells[row, col++] = Format_Currency(current_month_value - prev_month_value);
                if (Show_Comparison_Percentages && i > 1)
                    sheet.Cells[row, col++] = Format_Percentage((current_month_value - prev_month_value) / Math.Abs(prev_month_value));

                prev_month_value = current_month_value;
            }
            sheet.Cells[row, col++] = Format_Currency(prev_month_value);

            row++; col = 1; prev_month_value = 0; row_total = 0;
            #endregion

            #region Total Liabilities Line

            row++;
            sheet.Cells[row, col] = "TOTAL LIABILITIES";
            Liability_Total_Line = row;
            sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            sheet.Cells.get_Range("A" + (row).ToString(), Get_Column_Letter(Total_Column) + (row).ToString()).Font.Bold = true;

            //int g = (Month_Count - 1) * (1 + (Show_Comparison_Percentages ? 1 : 0) + (Show_Comparison_Values ? 1 : 0)) + 3;
            //Diagnostics.WriteLine(g);

            //for (int i = 1; i < Month_Count * (1 + (Show_Comparison_Percentages ? 1 : 0) + (Show_Comparison_Values ? 1 : 0)) + 2; i++)
            for (int i = 1; i < Total_Column; i++)
            {
                if (i > 2)
                {
                    // sum the lines
                    var cellValue = (sheet.Cells[3, i] as Excel.Range).Value;
                    // If percentage
                    if (cellValue.GetType() == typeof(string) && (string)(sheet.Cells[3, i] as Excel.Range).Value == "Δ%")
                    {
                        // Get correct rows
                        int col1 = i - (Show_Comparison_Values ? 5 : 3);
                        int col2 = i - (Show_Comparison_Values ? 2 : 1);
                        if (col1 < 3) col1 = 3; // No comparison first month so compensate for that
                        sheet.Cells[row, i] = "=(" + Get_Column_Letter(col2) + row + "-" + Get_Column_Letter(col1) + row + ")/" + Get_Column_Letter(col1) + row;
                        sheet.Range[Get_Column_Letter(i) + row].NumberFormat = "[<>0]#,##0.00%;[=0]0%";
                    }
                    else // Not percentage
                    {
                        sheet.Cells[row, i] = "=SUM(" + Get_Column_Letter(i) + Liabilities_Line.ToString() + ":" + Get_Column_Letter(i) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(i) + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)";
                    }
                }
                sheet.Cells.get_Range(Get_Column_Letter(i) + row.ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                sheet.Cells.get_Range(Get_Column_Letter(i) + (row + 2).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;

            }
            row++;
            #endregion

            #region Calculate percentage vs total liabilities
            if (Show_Percentages_vs_Total)
            {
                for (int i = 3; i < Total_Column; i++)
                {
                    var cellValue = (sheet.Cells[3, i] as Excel.Range).Value;
                    if (cellValue.GetType() == typeof(string) && (string)(sheet.Cells[3, i] as Excel.Range).Value == "% of Total")
                    {
                        for (int j = Asset_Total_Line + 1; j < Liability_Total_Line + 1; j++)
                        {
                            var intCellValue = (sheet.Cells[j, i - 1] as Excel.Range).Value;
                            if (intCellValue != null && intCellValue.GetType() == typeof(decimal))
                            {
                                sheet.Cells[j, i] = "=" + Get_Column_Letter(i - 1) + j + "/" + Get_Column_Letter(i - 1) + Liability_Total_Line;
                                sheet.Range[Get_Column_Letter(i) + j].NumberFormat = "[<>0]#,##0.00%;[=0]0%";
                            }
                            else if (intCellValue != null && intCellValue.GetType() == typeof(string) && (string)(sheet.Cells[j, i - 1] as Excel.Range).Value == "-")
                            {
                                sheet.Cells[j, i] = "-";
                            }
                        }
                    }
                }
            }
            #endregion

            #region Total Net Worth

            row++;
            sheet.Cells[row, col] = "RESIDUAL CASH IN/OUT";
            sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            sheet.Cells.get_Range("A" + (row).ToString(), Get_Column_Letter(Total_Column) + (row).ToString()).Font.Bold = true;

            //int g = (Month_Count - 1) * (1 + (Show_Comparison_Percentages ? 1 : 0) + (Show_Comparison_Values ? 1 : 0)) + 3;
            //Diagnostics.WriteLine(g);

            //for (int i = 1; i < Month_Count * (1 + (Show_Comparison_Percentages ? 1 : 0) + (Show_Comparison_Values ? 1 : 0)) + 2; i++)
            for (int i = 1; i < Total_Column; i++)
            {
                if (i > 2)
                {
                    // sum the lines
                    var cellValue = (sheet.Cells[3, i] as Excel.Range).Value;
                    // If percentage
                    if (cellValue.GetType() == typeof(string) && (string)(sheet.Cells[3, i] as Excel.Range).Value == "Δ%")
                    {
                        // Get correct rows
                        int col1 = i - (Show_Comparison_Values ? 5 : 3);
                        int col2 = i - (Show_Comparison_Values ? 2 : 1);
                        if (col1 < 3) col1 = 3; // No comparison first month so compensate for that
                        sheet.Cells[row, i] = "=(" + Get_Column_Letter(col2) + row + "-" + Get_Column_Letter(col1) + row + ")/" + Get_Column_Letter(col1) + row;
                        sheet.Range[Get_Column_Letter(i) + row].NumberFormat = "[<>0]#,##0.00%;[=0]0%";
                    }
                    else if (Show_Percentages_vs_Total && cellValue.GetType() == typeof(string) && (string)(sheet.Cells[3, i] as Excel.Range).Value == "% of Total")
                    {
                        sheet.Cells[row, i] = "=" + Get_Column_Letter(i - 1) + row + "/" + Get_Column_Letter(i - 1) + Asset_Total_Line;
                        sheet.Range[Get_Column_Letter(i) + row].NumberFormat = "[<>0]#,##0.00%;[=0]0%";
                    }
                    else // Not percentage
                    {
                        sheet.Cells[row, i] = "=(" + Get_Column_Letter(i) + Asset_Total_Line.ToString() + "-" + Get_Column_Letter(i) + Liability_Total_Line.ToString() + ")";
                        sheet.Range[Get_Column_Letter(i) + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)";
                    }
                }
                sheet.Cells.get_Range(Get_Column_Letter(i) + row.ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                sheet.Cells.get_Range(Get_Column_Letter(i) + (row + 1).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlDouble;

            }
            row++;
            #endregion

            // Vertical column line
            for (int i = 4; i < row; i++)
            {
                sheet.Cells.get_Range("B" + i.ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                sheet.Cells.get_Range(Get_Column_Letter(Total_Column - 2 - (Show_Percentages_vs_Total ? 1 : 0)) + i.ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
            }
        }

        private string Format_Currency(double amt, bool Special_Format = false)
        {
            if (Special_Format) return "-";
            //return amt == 0 ? "-" : "$" + String.Format("{0:0.00}", amt);
            return "$" + String.Format("{0:0.00}", amt);
        }

        private string Format_Percentage(double percent, bool Special_Format = false)
        {
            if (Special_Format) return "-";
            else if (Double.IsNaN(percent)) return "0.00%";
            else if (Double.IsInfinity(percent)) return "-%"; // Infinite increase x/0
            else return percent.ToString("P2");
        }

        private bool Ignore_Zero_Value = true;
        private bool Show_Comparison_Values = true;
        private bool Show_Comparison_Percentages = true;
        private bool Show_Percentages_vs_Total = false;
        private bool Combine_GC = false;


        bool GeneratingError = false;

        private void Write_Excel()
        {
            Calculate_Months();

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
            sheet.Name = "Cash Flow Spreadsheet";

            int row = 1;

            // insert title
            sheet.Cells[1, 1] = "Cash Flow Spreadsheet";
            sheet.Cells.get_Range("A1").Font.Bold = true;
            sheet.Cells.get_Range("A1").Font.Size = 20;
            sheet.Cells.get_Range("A1").Font.ColorIndex = 56;
            sheet.Cells.get_Range("A1", "B1").Merge();
            row += 2;

            // write header
            WriteHeader(ref sheet, ref row);

            // adjust alignment first before changing specific cells
            sheet.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            // write information
            WriteSpreadsheet(ref sheet, ref row);

            // adjust style
            sheet.Cells.Columns.AutoFit();

            // Fix first row
            sheet.Application.ActiveWindow.SplitRow = 3;
            sheet.Application.ActiveWindow.SplitColumn = 2;
            sheet.Application.ActiveWindow.FreezePanes = true;

            // Now apply autofilter
            Excel.Range firstRow = (Excel.Range)sheet.Rows[4];


            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Cash Flow Report (gen. on " + DateTime.Now.ToString("MM-dd-yyyy HH-mm") + ").xlsx");

            parent.GeneratedFilePaths_List.Add(path);

            try
            {
                try
                {
                    if (File.Exists(path)) File.Delete(path);
                    book.SaveAs(path, Excel.XlFileFormat.xlOpenXMLWorkbook);
                }
                catch
                {
                }
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

        FadeControl TFLP;

        // Converting month number to name
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        public string Get_Date(int Relative_Month_Count)
        {
            DateTime From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, 1);
            // Get last day of the month
            From_Date = From_Date.AddMonths(1).AddDays(-1);
            From_Date = From_Date.AddMonths(Relative_Month_Count - 1);
            return mfi.GetMonthName(From_Date.Month) + " " + From_Date.Year;
        }

        // Return datetime given month
        public DateTime Get_Date_DateTime(int Relative_Month_Count, bool Start_Of_Month = false)
        {
            DateTime From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, 1);
            // Get last day of the month

            From_Date = From_Date.AddMonths(1).AddDays(-1);
            From_Date = From_Date.AddMonths(Relative_Month_Count - 1);

            if (Start_Of_Month) return From_Date.AddDays(1).AddMonths(-1);

            return From_Date;
        }

        private void excel_button_Click(object sender, EventArgs e)
        {
            Grey_Out();

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

            Grey_In();

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

            parent.Settings_Dictionary["CASHFLOW_PAYMENT_ACCS"] = cmbManual.Text;
        }

        /// <summary>
        /// GET EXCEL COLUMN NAME
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        private string Get_Column_Letter(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        // Master month count
        int Month_Count = 0;

        private void Calculate_Months()
        {
            // Only check after loaded
            if (Loaded)
            {
                Month_Count = 0;

                DateTime From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, 1);
                DateTime To_Date = new DateTime(Convert.ToInt32(to_year.Text), to_month.SelectedIndex + 1, 1);

                // If invalid date selection, set dates to be the same
                if (From_Date > To_Date)
                {
                    from_month.Text = to_month.Text = mfi.GetMonthName(DateTime.Now.Month);
                    from_year.Text = to_year.Text = (DateTime.Now.Year).ToString();

                }
                else
                {
                    Month_Count = ((To_Date.Year - From_Date.Year) * 12) + To_Date.Month - From_Date.Month + 1;
                }
            }
        }


        private void group_all_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cmbManual.Items.Count; i++)
            {
                cmbManual.CheckBoxItems[i].Checked = true;
            }
        }

        private void reset_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < cmbManual.Items.Count; i++)
            {
                cmbManual.CheckBoxItems[i].Checked = false;
            }
        }

        private void combine_GC__CheckedChanged(object sender, EventArgs e)
        {
            if (Loaded)
            {
                Combine_GC = combine_GC_.Checked;
                parent.Settings_Dictionary["CASHFLOW_GROUP_GC"] = Combine_GC ? "1" : "0";
            }
        }

        private void show_value_CheckedChanged(object sender, EventArgs e)
        {
            if (Loaded)
            {
                checkBox1.CheckedChanged -= checkBox1_CheckedChanged;
                Show_Comparison_Values = show_value.Checked;
                parent.Settings_Dictionary["CASHFLOW_SHOW_VALUE"] = Show_Comparison_Values ? "1" : "0";
                checkBox1.Checked = false;
                Show_Percentages_vs_Total = checkBox1.Checked;
                parent.Settings_Dictionary["CASHFLOW_SHOW_PERCENT_VS_TOTAL"] = Show_Percentages_vs_Total ? "1" : "0";
                checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            }
        }

        private void show_percent_CheckedChanged(object sender, EventArgs e)
        {
            if (Loaded)
            {
                checkBox1.CheckedChanged -= checkBox1_CheckedChanged;
                Show_Comparison_Percentages = show_percent.Checked;
                parent.Settings_Dictionary["CASHFLOW_SHOW_PERCENT"] = Show_Comparison_Percentages ? "1" : "0";
                checkBox1.Checked = false;
                Show_Percentages_vs_Total = checkBox1.Checked;
                parent.Settings_Dictionary["CASHFLOW_SHOW_PERCENT_VS_TOTAL"] = Show_Percentages_vs_Total ? "1" : "0";
                checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (Loaded)
            {
                show_value.CheckedChanged -= show_value_CheckedChanged;
                show_percent.CheckedChanged -= show_percent_CheckedChanged;
                Show_Percentages_vs_Total = checkBox1.Checked;
                parent.Settings_Dictionary["CASHFLOW_SHOW_PERCENT_VS_TOTAL"] = Show_Percentages_vs_Total ? "1" : "0";
                show_value.Checked = false;
                show_percent.Checked = false;
                Show_Comparison_Percentages = show_percent.Checked;
                Show_Comparison_Values = show_value.Checked;
                parent.Settings_Dictionary["CASHFLOW_SHOW_PERCENT"] = Show_Comparison_Percentages ? "1" : "0";
                parent.Settings_Dictionary["CASHFLOW_SHOW_VALUE"] = Show_Comparison_Values ? "1" : "0";
                show_value.CheckedChanged += show_value_CheckedChanged;
                show_percent.CheckedChanged += show_percent_CheckedChanged;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (Loaded)
            {
                Ignore_Zero_Value = checkBox2.Checked;
                parent.Settings_Dictionary["CASHFLOW_IGNORE_ZERO_VALUE"] = Ignore_Zero_Value ? "1" : "0";
            }
        }

        private void cmbManual_SelectedIndexChanged(object sender, EventArgs e)
        {
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
    }


    internal static class FormExtensions
    {
        private static void ApplicationRunProc(object state)
        {
            Application.Run(state as Form);
        }

        public static void RunInNewThread(this Form form, bool isBackground)
        {
            if (form == null)
                throw new ArgumentNullException("form");
            if (form.IsHandleCreated)
                throw new InvalidOperationException("Form is already running.");
            Thread thread = new Thread(ApplicationRunProc);
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = isBackground;
            thread.Start(form);
        }
    }
}
