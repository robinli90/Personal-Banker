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
    public partial class CustomCategorySpreadsheet : Form
    {

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public CustomCategorySpreadsheet(Receipt _parent, Point g = new Point(), Size s = new Size())
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

            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

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

            PopulateProfiles();

            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            Years = Years.OrderBy(x => Convert.ToInt32(x)).ToList();
            Years.ForEach(x => from_year.Items.Add(x));
            Years.ForEach(x => to_year.Items.Add(x));

            from_month.Text = mfi.GetMonthName(DateTime.Now.Month);
            to_year.Text = DateTime.Now.Year.ToString();
            from_year.Text = from_year.Items.Contains((DateTime.Now.Year - 1).ToString()) ? (DateTime.Now.Year - 1).ToString() : (DateTime.Now.Year).ToString();
            to_month.Text = mfi.GetMonthName(DateTime.Now.Month);

            Calculate_Months();

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

        private void PopulateProfiles()
        {
            profileBox.Items.Clear();

            foreach (string profileName in parent.GroupedCategoryList.Select(x => x._ProfileName).Distinct().OrderBy(x => x))
            {
                profileBox.Items.Add(profileName);
            }

            if (profileBox.Items.Count > 0) profileBox.SelectedIndex = 0;
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

            }
            sheet.Cells[row, col++] = "Range Total";

            Excel.Range range = sheet.Cells.get_Range("A" + row.ToString(), Get_Column_Letter(col) + row.ToString());
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.Font.ColorIndex = 48;
            range.Cells.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            row++;
        }

        private void WriteSpreadsheet(ref Excel.Worksheet sheet, ref int row)
        {
            int col = 2;
            double prev_month_value = 0;
            double row_total = 0;

            int Total_Column = (Month_Count + 3);


            foreach (GroupedCategory GC in parent.GroupedCategoryList.Where(x => x._ProfileName == profileBox.Text).OrderBy(x => x._GroupName))
            {
                sheet.Cells[row, 1] = GC._GroupName;
                sheet.Cells.get_Range("A" + row, Get_Column_Letter(Total_Column) + row).Font.Bold = true;
                sheet.Cells.get_Range("A" + row, Get_Column_Letter(Total_Column) + row).Interior.ColorIndex = 49;
                sheet.Cells.get_Range("A" + row, Get_Column_Letter(Total_Column) + row).Font.Color = Color.White;
                row++;


                int rowstart = row;

                #region Sub categories
                foreach (string category in GC.SubCategoryList.OrderBy(x => x))
                {

                    sheet.Cells[row, 2] = category;
                    sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                    col++;
                    for (int i = 1; i < Month_Count + 1; i++)
                    {

                        DateTime Ref_Date = Get_Date_DateTime(i);

                        double current_month_value = parent.Master_Item_List.Where(x => x.Category == category && x.Date.Month == Ref_Date.Month && x.Date.Year == Ref_Date.Year).ToList().Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));

                        row_total += current_month_value;

                        //sheet.Cells[row, col++] = Format_Currency(current_month_value, !Expense.Check_Expense_Active_Date(Ref_Date));
                        sheet.Cells[row, col++] = Format_Currency(current_month_value, row_total <= 1);

                    }
                    sheet.Cells[row, col++] = Format_Currency(row_total);

                    row++;
                    col = 2;
                    row_total = 0;
                }
                #endregion

                #region Sub expenses

                if (includeExpenses.Checked)
                {
                    foreach (string category in GC.SubExpenseList.Where(y => y.Length > 0).OrderBy(x => x))
                    {

                        sheet.Cells[row, 2] = category;
                        sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment =
                            Excel.XlHAlign.xlHAlignRight;
                        sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                        col++;
                        for (int i = 1; i < Month_Count + 1; i++)
                        {

                            DateTime Ref_Date = Get_Date_DateTime(i).AddDays(1);

                            double current_month_value = parent.Expenses_List.First(x => x.Expense_Name == category)
                                .Get_Total_Paid(Ref_Date.AddMonths(-1), Ref_Date);

                            row_total += current_month_value;

                            //sheet.Cells[row, col++] = Format_Currency(current_month_value, !Expense.Check_Expense_Active_Date(Ref_Date));
                            sheet.Cells[row, col++] = Format_Currency(current_month_value, row_total <= 1);

                        }
                        sheet.Cells[row, col++] = Format_Currency(row_total);
                        sheet.Cells.get_Range("B" + row, Get_Column_Letter(Total_Column) + row).Interior.Color =
                            Color.LightGray;

                        row++;
                        col = 2;
                        row_total = 0;

                    }
                }

                #endregion

                #region Category Row Total

                    sheet.Cells[row, col] = "TOTAL (" + GC._GroupName + ")";
                    sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    sheet.Cells.get_Range("A" + (row).ToString(), Get_Column_Letter(Total_Column) + (row).ToString()).Font
                        .Bold = true;

                    for (int i = 3; i < Total_Column + 1; i++)
                    {
                        sheet.Cells[row, i] = "=SUM(" + Get_Column_Letter(i) + rowstart.ToString() + ":" +
                                              Get_Column_Letter(i) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(i) + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)";
                        sheet.Cells.get_Range(Get_Column_Letter(i) + (row + 1).ToString()).Cells
                            .Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlDouble;
                        sheet.Cells.get_Range(Get_Column_Letter(i-2) + (row).ToString()).Cells
                            .Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                        sheet.Cells.get_Range(Get_Column_Letter(i) + (row).ToString()).Cells
                            .Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;

                    }
                    row++;

                    #endregion



                row++;

            }
            // Vertical column line
            for (int i = 4; i < row - 1; i++)
            {
                sheet.Cells.get_Range("B" + i.ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                sheet.Cells.get_Range(Get_Column_Letter(Total_Column - 1) + i.ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
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
            sheet.Name = "Custom Categorical Spreadsheet";

            int row = 1;

            // insert title
            sheet.Cells[1, 1] = profileBox.Text;
            sheet.Cells.get_Range("A1").Font.Bold = true;
            sheet.Cells.get_Range("A1").Font.Size = 18;
            sheet.Cells.get_Range("A1").Font.ColorIndex = 56;
            sheet.Cells.get_Range("A1", "B1").Merge();
            row += 2;
            sheet.Cells.get_Range("A1", "B1").HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

            // write header
            WriteHeader(ref sheet, ref row);

            // adjust alignment first before changing specific cells
            sheet.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            row++;

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


            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Custom Categorical Spreadsheet (" + profileBox.Text + "_gen. on " + DateTime.Now.ToString("MM-dd-yyyy HH-mm") + ").xlsx");

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

            From_Date = From_Date.AddMonths(Relative_Month_Count - 1);
            From_Date = From_Date.AddMonths(1).AddDays(-1);

            if (Start_Of_Month) return From_Date.AddDays(1).AddMonths(-1);

            return From_Date;
        }

        private void excel_button_Click(object sender, EventArgs e)
        {
            Grey_Out();

            if (profileBox.Text.Length > 0)
            {

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
                    Form_Message_Box FMB =
                        new Form_Message_Box(parent,
                            "Error: Cannot overwrite existing Excel document. Please close existing 'Purchase Report' excel documents.",
                            true, 0, this.Location, this.Size);
                    FMB.Height += 18;
                    FMB.ShowDialog();
                    GeneratingError = false;
                }
            }
            else
            {
                Form_Message_Box FMB =
                    new Form_Message_Box(parent, "Error: No profile chosen",  true, -10, this.Location, this.Size);
                FMB.ShowDialog();
            }
            Grey_In();
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

        private void findID_Click(object sender, EventArgs e)
        {
            Grey_Out();
            CategoryGrouper CG = new CategoryGrouper(parent, Location, Size);
            CG.ShowDialog();
            Grey_In();
            PopulateProfiles();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void profileBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
