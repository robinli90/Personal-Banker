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
    public partial class Item_Summary_Spreadsheet : Form
    {

        Receipt parent;
        Size Start_Size = new Size();

        public Item_Summary_Spreadsheet(Receipt _parent, Point g = new Point(), Size s = new Size())
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
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

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
            foreach (string Cat in parent.category_box.Items)
            {
                cmbManual.Items.Add(Cat);
            }

            // Select all 
            for (int i = 0; i < cmbManual.Items.Count; i++)
            {
                cmbManual.CheckBoxItems[i].Checked = true;
            }

            sortByBox.Items.Add("Date");
            sortByBox.Items.Add("Location");
            sortByBox.Items.Add("Price");
            sortByBox.SelectedIndex = 0;

            sortDirection.Items.Add("Ascending");
            sortDirection.Items.Add("Descending");
            sortDirection.SelectedIndex = 0;

            showAvg.Visible = group.Checked;

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
            int col = 2 - (group.Checked ? 0 : 1);

            sheet.Cells[row, col++] = "Location";
            sheet.Cells[row, col++] = "Item";
            if (!group.Checked)
                sheet.Cells[row, col++] = "Category";
            sheet.Cells[row, col++] = "Date";
            sheet.Cells[row, col++] = "Quantity";
            sheet.Cells[row, col++] = "Total Price";
            sheet.Cells[row, col++] = "% of Total ";

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

            string[] Categories_Selected = cmbManual.Text.Split(new string[] { ", " }, StringSplitOptions.None);

            // Pre-sorted items
            List<Item> Pre_Sort = parent.Master_Item_List.Where(x => x.Date.Date >= From_Date.Date && x.Date.Date <= To_Date.Date).ToList();

            double Grand_Total = Pre_Sort.Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));

            if (sortByBox.Text == "Price")
            {
                if (sortDirection.Text == "Descending") Pre_Sort = Pre_Sort.OrderByDescending(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x))).ToList();
                else if (sortDirection.Text == "Ascending") Pre_Sort = Pre_Sort.OrderBy(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x))).ToList();
            }
            else if (sortByBox.Text == "Location")
            {
                if (sortDirection.Text == "Descending") Pre_Sort = Pre_Sort.OrderByDescending(x => x.Location).ToList();
                else if (sortDirection.Text == "Ascending") Pre_Sort = Pre_Sort.OrderBy(x => x.Location).ToList();
            }
            else if (sortByBox.Text == "Date")
            {
                if (sortDirection.Text == "Descending") Pre_Sort = Pre_Sort.OrderByDescending(x => x.Date).ToList();
                else if (sortDirection.Text == "Ascending") Pre_Sort = Pre_Sort.OrderBy(x => x.Date).ToList();
            }

            #region Grouped Categories Output
            if (group.Checked)
            {
                int Category_Row_Start = 0;
                foreach (string cate in Categories_Selected)
                {
                    List<Item> Categorical_Item_List = Pre_Sort.Where(x => x.Category == cate).ToList();

                    if (Categorical_Item_List.Count > 0)
                    {

                        col = 1;
                        sheet.Cells[row, col++] = cate;
                        sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                        sheet.Cells.get_Range("A" + row.ToString()).Interior.ColorIndex = 42;
                        row++;
                        Category_Row_Start = row;
                        int categoryCount = 0;

                        foreach (Item item in Categorical_Item_List)
                        {
                            sheet.Cells[row, col++] = item.Location;
                            sheet.Cells[row, col++] = item.Name;
                            sheet.Cells[row, col++] = item.Date.ToShortDateString();
                            sheet.Cells[row, col++] = item.Quantity - Convert.ToInt32(item.Status);
                            sheet.Cells[row, col++] = Format_Currency(item.Get_Current_Amount(parent.Get_Tax_Amount(item)));
                            sheet.Cells[row, col++] = Format_Percentage(item.Get_Current_Amount(parent.Get_Tax_Amount(item)) / Grand_Total);
                            categoryCount++;
                            row++; col = 2;
                        }


                        #region Categorical Total
                        sheet.Cells[row, 4] = cate + "  Total:";
                        sheet.Cells.get_Range("D" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                        sheet.Cells.get_Range("D" + (row).ToString(), Get_Column_Letter(7) + (row).ToString()).Font.Bold = true;
                        sheet.Cells.get_Range("A" + (row).ToString(), Get_Column_Letter(7) + (row).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                        sheet.Cells.get_Range("E" + (row + 1).ToString(), Get_Column_Letter(7) + (row + 1).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlDouble;

                        sheet.Cells[row, 5] = "=SUM(" + Get_Column_Letter(5) + Category_Row_Start + ":" + Get_Column_Letter(5) + (row - 1) + ")";
                        sheet.Cells[row, 6] = "=SUM(" + Get_Column_Letter(6) + Category_Row_Start + ":" + Get_Column_Letter(6) + (row - 1) + ")";
                        sheet.Cells[row, 7] = "=" + Get_Column_Letter(6) + row + "/" + Grand_Total.ToString();
                        sheet.Range[Get_Column_Letter(7) + row].NumberFormat = "[<>0]#,##0.00%;[=0]0%";

                        if (showAvg.Checked && categoryCount > 1)// && Convert.ToInt32(Get_Column_Letter(5) + (row - 1)) > 1) // only show avg if greater than 1 item
                        {
                            row++;
                            sheet.Cells[row, 4] = "Average:";
                            sheet.Cells[row, 5] = "=" + Get_Column_Letter(6) + (row - 1) + "/" + Get_Column_Letter(5) + (row - 1);
                            sheet.Cells.get_Range("D" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                            sheet.Cells.get_Range("D" + (row).ToString(), Get_Column_Letter(7) + (row).ToString()).Font.Bold = true;
                            sheet.Range["E" + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)"; // style currency
                        }

                        #endregion

                        row += 2; col = 1;
                    }
                }

                #region Final Total
                sheet.Cells[row, 5] = "Grand Total:";
                sheet.Cells.get_Range("E" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                sheet.Cells.get_Range("E" + (row).ToString(), "F" + (row).ToString()).Font.Bold = true;
                sheet.Cells.get_Range("E" + (row).ToString(), "F" + (row).ToString()).Font.Size = 13;
                sheet.Cells.get_Range("F" + (row).ToString(), "F" + (row).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                sheet.Cells.get_Range("F" + (row + 1).ToString(), "F" + (row + 1).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlDouble;

                sheet.Cells[row, 6] = "=SUM(" + "F4:" + "F" + (row - 1) + ") / 2";
                sheet.Range["F" + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)"; // style currency
                #endregion

            }
            #endregion
            else if (!group.Checked)
            {
                // Get all valid items with categories selected
                Pre_Sort = Pre_Sort.Where(x => Categories_Selected.Contains(x.Category)).ToList();

                foreach (Item item in Pre_Sort)
                {
                    col = 1;
                    sheet.Cells[row, col++] = item.Location;
                    sheet.Cells[row, col++] = item.Name;
                    sheet.Cells[row, col++] = item.Category;
                    sheet.Cells[row, col++] = item.Date.ToShortDateString();
                    sheet.Cells[row, col++] = item.Quantity - Convert.ToInt32(item.Status);
                    sheet.Cells[row, col++] = Format_Currency(item.Get_Current_Amount(parent.Get_Tax_Amount(item)));
                    sheet.Cells[row, col++] = Format_Percentage(item.Get_Current_Amount(parent.Get_Tax_Amount(item)) / Grand_Total);
                    row++; col = 1;
                }


                #region Final Total
                sheet.Cells[row, 4] = "Grand Total:";
                sheet.Cells.get_Range("D" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                sheet.Cells.get_Range("D" + (row).ToString(), "F" + (row).ToString()).Font.Bold = true;
                sheet.Cells.get_Range("D" + (row).ToString(), "F" + (row).ToString()).Font.Size = 13;
                sheet.Cells.get_Range("A" + (row).ToString(), "G" + (row).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                sheet.Cells.get_Range("E" + (row + 1).ToString(), "F" + (row + 1).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlDouble;

                sheet.Cells[row, 5] = "=SUM(" + "E4:" + "E" + (row - 1) + ")";
                sheet.Cells[row, 6] = "=SUM(" + "F4:" + "F" + (row - 1) + ")";
                sheet.Range["F" + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)"; // style currency
                #endregion
            }
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
            sheet.Name = "Item Summary Spreadsheet";

            int row = 1;

            // insert title
            sheet.Cells[1, 1] = "Item Summary Spreadsheet";
            sheet.Cells.get_Range("A1").Font.Bold = true;
            sheet.Cells.get_Range("A1").Font.Size = 20;
            sheet.Cells.get_Range("A1").Font.ColorIndex = 56;
            sheet.Cells.get_Range("A1", "F1").Merge();
            row++;
            sheet.Cells[2, 1] = "Summary from " + From_Date.ToShortDateString() + " to " + To_Date.ToShortDateString() + " with sort applied to '" + sortByBox.Text + "' in " + sortDirection.Text.ToLower() + " order";
            sheet.Cells.get_Range("A2").Font.Italic = true;
            sheet.Cells.get_Range("A2").Font.Size = 10;
            sheet.Cells.get_Range("A2").Font.ColorIndex = 56;
            sheet.Cells.get_Range("A2", "F2").Merge();
            row++;
            row++;

            // write header
            WriteHeader(ref sheet, ref row);

            // adjust alignment first before changing specific cells
            sheet.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            // write information
            WriteSpreadsheet(ref sheet, ref row);

            // adjust style
            sheet.Cells.Columns.AutoFit();

            // Fix first row
            sheet.Application.ActiveWindow.SplitRow = 4;
            //sheet.Application.ActiveWindow.SplitColumn = 2;
            sheet.Application.ActiveWindow.FreezePanes = true;

            // Now apply autofilter
            Excel.Range firstRow = (Excel.Range)sheet.Rows[4];


            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Item Summary Report (gen. on " + DateTime.Now.ToString("MM-dd-yyyy HH-mm") + ").xlsx");

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
        DateTime From_Date;
        DateTime To_Date;

        private void Calculate_Months()
        {
            // Only check after loaded
            if (Loaded)
            {
                Month_Count = 0;

                From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, 1);
                To_Date = new DateTime(Convert.ToInt32(to_year.Text), to_month.SelectedIndex + 1, 1);

                // If invalid date selection, set dates to be the same
                if (From_Date > To_Date)
                {
                    from_month.Text = to_month.Text = mfi.GetMonthName(DateTime.Now.Month);
                    from_year.Text = to_year.Text = (DateTime.Now.Year).ToString();
                    From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, 1);
                    To_Date = new DateTime(Convert.ToInt32(to_year.Text), to_month.SelectedIndex + 1, 1);
                }
                else
                {
                    Month_Count = ((To_Date.Year - From_Date.Year) * 12) + To_Date.Month - From_Date.Month + 1;
                }
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

        private void show_percent_CheckedChanged(object sender, EventArgs e)
        {
            showAvg.Visible = group.Checked;
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
}
