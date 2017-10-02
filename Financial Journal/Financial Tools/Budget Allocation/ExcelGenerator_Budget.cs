using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace Financial_Journal
{
    public enum FigureType
    {
        Budget,
        Actual,
        Balance
    }

    public class ExcelGenerator_Budget
    {


        private BudgetAllocation parent;

        // Date culture
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        public ExcelGenerator_Budget(BudgetAllocation parent_)
        {
            parent = parent_;
        }

        private void WriteHeader(ref Excel.Worksheet sheet, ref int row)
        {
            // header
            int col = 2;

            sheet.Cells[row, col++] = "Prev. Actual";
            sheet.Cells[row, col++] = "Budget";
            sheet.Cells[row, col++] = "Actual";
            sheet.Cells[row, col++] = "Balance";

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
            int index = 0;

            while (index < parent.dataGridView1.Rows.Count)
            {
                string row1 = parent.dataGridView1[0, index].Value.ToString();
                string row2 = parent.dataGridView1[1, index].Value.ToString();
                string row3 = parent.dataGridView1[2, index].Value.ToString();
                string row4 = parent.dataGridView1[3, index].Value.ToString();
                string row5 = parent.dataGridView1[4, index].Value.ToString();
                string row6 = parent.dataGridView1[5, index].Value.ToString();

                if (row6.Contains("HeaderRowHere"))
                {
                    row++;
                    sheet.Cells[row, 1] = row1;
                    sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                    sheet.Cells[row, 2] = row2;
                    sheet.Cells[row, 3] = row3;
                    sheet.Cells[row, 4] = row4;
                    sheet.Cells[row, 5] = row5;

                    sheet.Cells.get_Range("A" + row, Get_Column_Letter(5) + row).Font.Bold = true;
                    sheet.Cells.get_Range("A" + row, Get_Column_Letter(5) + row).Interior.ColorIndex = 49;
                    sheet.Cells.get_Range("A" + row, Get_Column_Letter(5) + row).Font.Color = Color.White;
                    // extra lines
                }
                else
                {
                    sheet.Cells[row, 1] = row1;
                    sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                    sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                    sheet.Cells[row, 2] = row2;
                    sheet.Cells[row, 3] = row3;
                    sheet.Cells[row, 4] = row4;
                    sheet.Cells[row, 5] = row5;
                }

                row++;
                index++;
            }

        }

        public void Write_Excel_Current()
        {
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
            sheet.Name = "Budget Allocation Report";

            int row = 1;

            // insert title
            sheet.Cells[1, 1] = String.Format("Budget Allocation Report ({0})", parent.budgetMonth.Text);
            sheet.Cells.get_Range("A1").Font.Bold = true;
            sheet.Cells.get_Range("A1").Font.Size = 16;
            sheet.Cells.get_Range("A1").Font.ColorIndex = 56;
            sheet.Cells.get_Range("A1", "F1").Merge();
            row += 2;
            sheet.Cells.get_Range("A1", "F1").HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;

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
            //sheet.Application.ActiveWindow.SplitColumn = 1;
            sheet.Application.ActiveWindow.FreezePanes = true;


            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), String.Format("Budget Allocation Report ({0}).xlsx", parent.budgetMonth.Text));

            parent.parent.GeneratedFilePaths_List.Add(path);

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

            try
            {
                //book.Close();
            }
            catch
            {
                Diagnostics.WriteLine("Error disposing EXCEL files");
            }

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


        private List<FigureType> ftList = new List<FigureType>();

        private void WriteCategories(ref Excel.Worksheet sheet, int row, List<BudgetEntry> BEList, FigureType ft,
            int maxCol)
        {
            if (ftList.Contains(ft)) return;

            ftList.Add(ft);

            List<string> ExtraneousCategories = BEList
                .SelectMany(x => x.GetCategoryList()).Where(y => y.GetBCType() == BCType.Extraneous).Select(z => z.GetName()).Distinct().ToList();

            if (parent.profileBox.Text == "None")
            {
                // Categorical names
                foreach (string category in parent.parent.category_box.Items) // categories
                {
                    sheet.Cells[++row, 1] = category;
                    sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment =
                        Excel.XlHAlign.xlHAlignLeft;
                    sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                }
                foreach (string category in parent.parent.Expenses_List.Select(x => x.Expense_Name)) // recurring
                {
                    sheet.Cells[++row, 1] = category;
                    sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment =
                        Excel.XlHAlign.xlHAlignLeft;
                    sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                }
                foreach (string category in ExtraneousCategories) // extraneous
                {
                    sheet.Cells[++row, 1] = category;
                    sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment =
                        Excel.XlHAlign.xlHAlignLeft;
                    sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                }
            }
            else
            {
                string indent = "       ";
                foreach (GroupedCategory GC in parent.parent.GroupedCategoryList.Where(x => x._ProfileName == parent.profileBox.Text)
                    .OrderBy(x => x._GroupName))
                {
                    sheet.Cells[++row, 1] = GC._GroupName;
                    sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment =
                        Excel.XlHAlign.xlHAlignLeft;
                    sheet.Cells.get_Range("A" + row, Get_Column_Letter(maxCol) + (row)).Font.Bold = true;
                    sheet.Cells.get_Range("A" + row, Get_Column_Letter(maxCol) + (row)).Interior.ColorIndex = 49;
                    sheet.Cells.get_Range("A" + row, Get_Column_Letter(maxCol) + (row)).Font.Color = Color.White;

                    foreach (string category in GC.SubCategoryList.OrderBy(x => x))
                    {
                        sheet.Cells[++row, 1] = indent + category;
                        sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment =
                            Excel.XlHAlign.xlHAlignLeft;
                        sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                    }
                    foreach (string category in GC.SubExpenseList.Where(y => y.Length > 0).OrderBy(x => x))
                    {
                        sheet.Cells[++row, 1] = indent + category;
                        sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment =
                            Excel.XlHAlign.xlHAlignLeft;
                        sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                    }
                }

                if (ExtraneousCategories.Count > 0)
                {
                    sheet.Cells[++row, 1] = "Extraneous Categories";
                    sheet.Cells.get_Range("A" + row, Get_Column_Letter(maxCol) + (row)).Font.Bold = true;
                    sheet.Cells.get_Range("A" + row, Get_Column_Letter(maxCol) + (row)).Interior.ColorIndex = 49;
                    sheet.Cells.get_Range("A" + row, Get_Column_Letter(maxCol) + (row)).Font.Color = Color.White;
                    foreach (string category in ExtraneousCategories) // extraneous
                    {
                        sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment =
                            Excel.XlHAlign.xlHAlignLeft;
                        sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;

                        sheet.Cells[++row, 1] = indent + category;
                        sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment =
                            Excel.XlHAlign.xlHAlignLeft;
                        sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                    }
                }

                sheet.Cells.get_Range("A" + row).Cells.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous; //budget
            }
        }

        private void WriteHeaderAll(ref Excel.Worksheet sheet, ref int row)
        {
            // header
            int col = 0;

            for (int i = 0; i < parent.budgetMonth.Items.Count; i++)
            {
                sheet.Cells[row, col += 2] = parent.budgetMonth.Items[i];
                sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Merge();
            }

            Excel.Range range = sheet.Cells.get_Range("A" + row.ToString(), Get_Column_Letter(col) + row.ToString());
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.Font.ColorIndex = 48;
            range.Cells.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            row++;
        }

        private int budgetEndRow = 0;
        private int actualEndRow = 0;
        private int balanceEndRow = 0;

        private void WriteSpreadsheetAll(ref Excel.Worksheet sheet, ref int row, List<BudgetEntry> BEList)
        {
            CalculateFigures(BEList);

            // Get all extraneous and distinct categories
            List<string> ExtraneousCategories = BEList
                .SelectMany(x => x.GetCategoryList()).Where(y => y.GetBCType() == BCType.Extraneous).Select(z => z.GetName()).Distinct().ToList();

            int col = 1;
            int index = 0;
            int maxCol = parent.budgetMonth.Items.Count * 2 + 1;
            int rowStartIndex = 0;

            // for budget month
            for (int i = 0; i < parent.budgetMonth.Items.Count; i++)
            {
                if (parent.profileBox.Text == "None")
                {
                    WriteCategories(ref sheet, row, BEList, FigureType.Budget, maxCol);

                    row = 4;
                    col = 2 + (i * 2);
                    BudgetEntry refBE = BEList[i];

                    #region Budget

                    sheet.Cells[row, col] = "$ of Budget";
                    sheet.Cells[row, col + 1] = "% of Budget";
                    sheet.Cells.get_Range(Get_Column_Letter(1) + row, Get_Column_Letter(maxCol) + row).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Font.Bold =
                        true;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Interior
                        .ColorIndex = 9;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Font.Color =
                        Color.White;
                    row++;

                    rowStartIndex = row;
                    // categories data
                    foreach (string category in parent.parent.category_box.Items)
                    {

                        double amount = refBE.GetBudgetAmount(BCType.Categorical, category);
                        sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                        sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                        row++;
                    }

                    // recurring data
                    foreach (string category in parent.parent.Expenses_List.Select(x => x.Expense_Name)) // recurring
                    {

                        double amount = refBE.GetBudgetAmount(BCType.Recurring, category);
                        sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                        sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                        row++;
                    }

                    // extraneous data
                    foreach (string category in ExtraneousCategories) // extraneous
                    {

                        double amount = refBE.GetBudgetAmount(BCType.Extraneous, category);
                        sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                        sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                        row++;
                    }

                    // Write totals
                    // $ Total
                    sheet.Cells[row, col] = "=SUM(" + Get_Column_Letter(col) + rowStartIndex + ":" +
                                            Get_Column_Letter(col) + (row - 1).ToString() + ")";
                    sheet.Range[Get_Column_Letter(col) + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)";
                    // % Total
                    sheet.Cells[row, col + 1] = "=SUM(" + Get_Column_Letter(col + 1) + rowStartIndex + ":" +
                                                Get_Column_Letter(col + 1) + (row - 1).ToString() + ")";
                    sheet.Range[Get_Column_Letter(col + 1) + row].NumberFormat = "[<>0]#,##0.00%;[=0]0%";
                    // Line
                    sheet.Cells.get_Range(Get_Column_Letter(1) + row.ToString()).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row.ToString()).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    sheet.Cells.get_Range(Get_Column_Letter(col + 1) + row.ToString()).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    row++;

                    #endregion

                    budgetEndRow = row;
                    row++;

                    #region Actual

                    WriteCategories(ref sheet, row, BEList, FigureType.Actual, maxCol);

                    col = 2 + (i * 2);
                    sheet.Cells[row, col] = "$ of Actual";
                    sheet.Cells[row, col + 1] = "% of Actual";
                    sheet.Cells.get_Range(Get_Column_Letter(1) + row, Get_Column_Letter(maxCol) + row).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Font.Bold =
                        true;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Interior
                        .ColorIndex = 9;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Font.Color =
                        Color.White;
                    row++;

                    rowStartIndex = row;
                    // categories data
                    foreach (string category in parent.parent.category_box.Items)
                    {

                        double amount = refBE.GetActualAmount(BCType.Categorical, category);
                        sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                        sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                        row++;
                    }

                    // recurring data
                    foreach (string category in parent.parent.Expenses_List.Select(x => x.Expense_Name)) // recurring
                    {

                        double amount = refBE.GetActualAmount(BCType.Recurring, category);
                        sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                        sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                        row++;
                    }

                    // extraneous data
                    foreach (string category in ExtraneousCategories) // extraneous
                    {

                        double amount = refBE.GetActualAmount(BCType.Extraneous, category);
                        sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                        sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                        row++;
                    }
                    // Write totals
                    // $ Total
                    sheet.Cells[row, col] = "=SUM(" + Get_Column_Letter(col) + rowStartIndex + ":" +
                                            Get_Column_Letter(col) + (row - 1).ToString() + ")";
                    sheet.Range[Get_Column_Letter(col) + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)";
                    // % Total
                    sheet.Cells[row, col + 1] = "=SUM(" + Get_Column_Letter(col + 1) + rowStartIndex + ":" +
                                                Get_Column_Letter(col + 1) + (row - 1).ToString() + ")";
                    sheet.Range[Get_Column_Letter(col + 1) + row].NumberFormat = "[<>0]#,##0.00%;[=0]0%";
                    // Line
                    sheet.Cells.get_Range(Get_Column_Letter(1) + row.ToString()).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row.ToString()).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    sheet.Cells.get_Range(Get_Column_Letter(col + 1) + row.ToString()).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    row++;

                    #endregion

                    actualEndRow = row;
                    row++;

                    #region Balance

                    WriteCategories(ref sheet, row, BEList, FigureType.Balance, maxCol);

                    col = 2 + (i * 2);
                    sheet.Cells[row, col] = "$ of Balance";
                    sheet.Cells[row, col + 1] = "% of Balance";
                    sheet.Cells.get_Range(Get_Column_Letter(1) + row, Get_Column_Letter(maxCol) + row).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Font.Bold =
                        true;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Interior
                        .ColorIndex = 9;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Font.Color =
                        Color.White;
                    row++;

                    rowStartIndex = row;
                    // categories data
                    foreach (string category in parent.parent.category_box.Items)
                    {

                        double amount = refBE.GetBalanceAmount(BCType.Categorical, category);
                        sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                        sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                        row++;
                    }

                    // recurring data
                    foreach (string category in parent.parent.Expenses_List.Select(x => x.Expense_Name)) // recurring
                    {

                        double amount = refBE.GetBalanceAmount(BCType.Recurring, category);
                        sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                        sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                        row++;
                    }

                    // extraneous data
                    foreach (string category in ExtraneousCategories) // extraneous
                    {

                        double amount = refBE.GetBalanceAmount(BCType.Extraneous, category);
                        sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                        sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                        row++;
                    }
                    // Write totals
                    // $ Total
                    sheet.Cells[row, col] = "=SUM(" + Get_Column_Letter(col) + rowStartIndex + ":" +
                                            Get_Column_Letter(col) + (row - 1).ToString() + ")";
                    sheet.Range[Get_Column_Letter(col) + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)";
                    // % Total
                    sheet.Cells[row, col + 1] = "=SUM(" + Get_Column_Letter(col + 1) + rowStartIndex + ":" +
                                                Get_Column_Letter(col + 1) + (row - 1).ToString() + ")";
                    sheet.Range[Get_Column_Letter(col + 1) + row].NumberFormat = "[<>0]#,##0.00%;[=0]0%";
                    // Line
                    sheet.Cells.get_Range(Get_Column_Letter(1) + row.ToString()).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row.ToString()).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    sheet.Cells.get_Range(Get_Column_Letter(col + 1) + row.ToString()).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    row++;

                    #endregion

                    balanceEndRow = row;
                }
                else
                {

                    row = 4;
                    BudgetEntry refBE = BEList[i];

                    #region Budget
                    WriteCategories(ref sheet, row, BEList, FigureType.Budget, maxCol);
                    col = 2 + (i * 2);
                    sheet.Cells[row, col] = "$ of Budget";
                    sheet.Cells[row, col + 1] = "% of Budget";
                    sheet.Cells.get_Range(Get_Column_Letter(1) + row, Get_Column_Letter(maxCol) + row).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Font.Bold =
                        true;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Interior
                        .ColorIndex = 9;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Font.Color =
                        Color.White;
                    row++;

                    foreach (GroupedCategory GC in parent.parent.GroupedCategoryList
                        .Where(x => x._ProfileName == parent.profileBox.Text)
                        .OrderBy(x => x._GroupName))
                    {
                        rowStartIndex = row;
                        row++; // row for the GC._GroupName
                        #region Sub categories

                        foreach (string category in GC.SubCategoryList.OrderBy(x => x))
                        {
                            double amount = refBE.GetBudgetAmount(BCType.Categorical, category);
                            sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                            sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                            row++;
                        }

                        #endregion

                        #region Sub expenses

                        foreach (string category in GC.SubExpenseList.Where(y => y.Length > 0).OrderBy(x => x))
                        {
                            double amount = refBE.GetBudgetAmount(BCType.Recurring, category);
                            sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                            sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                            row++;
                        }
                        #endregion

                        // Write totals for GC group (above all written lines)
                        // $ Total
                        sheet.Cells[rowStartIndex, col] = "=SUM(" + Get_Column_Letter(col) + (rowStartIndex + 1) + ":" +
                                                Get_Column_Letter(col) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(col) + rowStartIndex].NumberFormat = "$#,##0.00;[Red]($#,##0.00)";
                        // % Total
                        sheet.Cells[rowStartIndex, col + 1] = "=SUM(" + Get_Column_Letter(col + 1) + (rowStartIndex + 1) + ":" +
                                                    Get_Column_Letter(col + 1) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(col + 1) + rowStartIndex].NumberFormat = "[<>0]#,##0.00%;[=0]0%";

                    }

                    if (ExtraneousCategories.Count > 0)
                    {
                        rowStartIndex = row;
                        row++; // row for the "Extraneous Categories" line
                        // extraneous data
                        foreach (string category in ExtraneousCategories) // extraneous
                        {

                            double amount = refBE.GetBudgetAmount(BCType.Extraneous, category);
                            sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                            sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                            row++;
                        }


                        // Write totals for extraneous (above all written lines)
                        // $ Total
                        sheet.Cells[rowStartIndex, col] = "=SUM(" + Get_Column_Letter(col) + (rowStartIndex + 1) + ":" +
                                                          Get_Column_Letter(col) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(col) + rowStartIndex].NumberFormat = "$#,##0.00;[Red]($#,##0.00)";
                        // % Total
                        sheet.Cells[rowStartIndex, col + 1] =
                            "=SUM(" + Get_Column_Letter(col + 1) + (rowStartIndex + 1) + ":" +
                            Get_Column_Letter(col + 1) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(col + 1) + rowStartIndex].NumberFormat = "[<>0]#,##0.00%;[=0]0%";
                    }
                    #endregion

                    budgetEndRow = row;
                    row++;

                    #region Actual
                    WriteCategories(ref sheet, row, BEList, FigureType.Actual, maxCol);
                    col = 2 + (i * 2);
                    sheet.Cells[row, col] = "$ of Actual";
                    sheet.Cells[row, col + 1] = "% of Actual";
                    sheet.Cells.get_Range(Get_Column_Letter(1) + row, Get_Column_Letter(maxCol) + row).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Font.Bold =
                        true;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Interior
                        .ColorIndex = 9;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Font.Color =
                        Color.White;
                    row++;

                    foreach (GroupedCategory GC in parent.parent.GroupedCategoryList
                        .Where(x => x._ProfileName == parent.profileBox.Text)
                        .OrderBy(x => x._GroupName))
                    {
                        rowStartIndex = row;
                        row++; // row for the GC._GroupName
                        #region Sub categories

                        foreach (string category in GC.SubCategoryList.OrderBy(x => x))
                        {
                            double amount = refBE.GetActualAmount(BCType.Categorical, category);
                            sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                            sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                            row++;
                        }

                        #endregion

                        #region Sub expenses

                        foreach (string category in GC.SubExpenseList.Where(y => y.Length > 0).OrderBy(x => x))
                        {
                            double amount = refBE.GetActualAmount(BCType.Recurring, category);
                            sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                            sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                            row++;
                        }
                        #endregion

                        // Write totals for GC group (above all written lines)
                        // $ Total
                        sheet.Cells[rowStartIndex, col] = "=SUM(" + Get_Column_Letter(col) + (rowStartIndex + 1) + ":" +
                                                          Get_Column_Letter(col) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(col) + rowStartIndex].NumberFormat = "$#,##0.00;[Red]($#,##0.00)";
                        // % Total
                        sheet.Cells[rowStartIndex, col + 1] = "=SUM(" + Get_Column_Letter(col + 1) + (rowStartIndex + 1) + ":" +
                                                              Get_Column_Letter(col + 1) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(col + 1) + rowStartIndex].NumberFormat = "[<>0]#,##0.00%;[=0]0%";

                    }

                    if (ExtraneousCategories.Count > 0)
                    {
                        rowStartIndex = row;
                        row++; // row for the "Extraneous Categories" line
                        // extraneous data
                        foreach (string category in ExtraneousCategories) // extraneous
                        {

                            double amount = refBE.GetActualAmount(BCType.Extraneous, category);
                            sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                            sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                            row++;
                        }


                        // Write totals for extraneous (above all written lines)
                        // $ Total
                        sheet.Cells[rowStartIndex, col] = "=SUM(" + Get_Column_Letter(col) + (rowStartIndex + 1) + ":" +
                                                          Get_Column_Letter(col) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(col) + rowStartIndex].NumberFormat = "$#,##0.00;[Red]($#,##0.00)";
                        // % Total
                        sheet.Cells[rowStartIndex, col + 1] =
                            "=SUM(" + Get_Column_Letter(col + 1) + (rowStartIndex + 1) + ":" +
                            Get_Column_Letter(col + 1) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(col + 1) + rowStartIndex].NumberFormat = "[<>0]#,##0.00%;[=0]0%";
                    }
                    #endregion

                    actualEndRow = row;
                    row++;

                    #region Balance
                    WriteCategories(ref sheet, row, BEList, FigureType.Balance, maxCol);
                    col = 2 + (i * 2);
                    sheet.Cells[row, col] = "$ of Balance";
                    sheet.Cells[row, col + 1] = "% of Balance";
                    sheet.Cells.get_Range(Get_Column_Letter(1) + row, Get_Column_Letter(maxCol) + row).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Font.Bold =
                        true;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Interior
                        .ColorIndex = 9;
                    sheet.Cells.get_Range(Get_Column_Letter(col) + row, Get_Column_Letter(col + 1) + row).Font.Color =
                        Color.White;
                    row++;

                    foreach (GroupedCategory GC in parent.parent.GroupedCategoryList
                        .Where(x => x._ProfileName == parent.profileBox.Text)
                        .OrderBy(x => x._GroupName))
                    {
                        rowStartIndex = row;
                        row++; // row for the GC._GroupName
                        #region Sub categories

                        foreach (string category in GC.SubCategoryList.OrderBy(x => x))
                        {
                            double amount = refBE.GetBalanceAmount(BCType.Categorical, category);
                            sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                            sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                            row++;
                        }

                        #endregion

                        #region Sub expenses

                        foreach (string category in GC.SubExpenseList.Where(y => y.Length > 0).OrderBy(x => x))
                        {
                            double amount = refBE.GetBalanceAmount(BCType.Recurring, category);
                            sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                            sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                            row++;
                        }
                        #endregion

                        // Write totals for GC group (above all written lines)
                        // $ Total
                        sheet.Cells[rowStartIndex, col] = "=SUM(" + Get_Column_Letter(col) + (rowStartIndex + 1) + ":" +
                                                          Get_Column_Letter(col) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(col) + rowStartIndex].NumberFormat = "$#,##0.00;[Red]($#,##0.00)";
                        // % Total
                        sheet.Cells[rowStartIndex, col + 1] = "=SUM(" + Get_Column_Letter(col + 1) + (rowStartIndex + 1) + ":" +
                                                              Get_Column_Letter(col + 1) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(col + 1) + rowStartIndex].NumberFormat = "[<>0]#,##0.00%;[=0]0%";

                    }

                    if (ExtraneousCategories.Count > 0)
                    {
                        rowStartIndex = row;
                        row++; // row for the "Extraneous Categories" line
                        // extraneous data
                        foreach (string category in ExtraneousCategories) // extraneous
                        {

                            double amount = refBE.GetBalanceAmount(BCType.Extraneous, category);
                            sheet.Cells[row, col] = parent.GetDollarFormat(amount);
                            sheet.Cells[row, col + 1] = String.Format("{0}%", (amount / refBE.BudgetSum) * 100);

                            row++;
                        }


                        // Write totals for extraneous (above all written lines)
                        // $ Total
                        sheet.Cells[rowStartIndex, col] = "=SUM(" + Get_Column_Letter(col) + (rowStartIndex + 1) + ":" +
                                                          Get_Column_Letter(col) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(col) + rowStartIndex].NumberFormat = "$#,##0.00;[Red]($#,##0.00)";
                        // % Total
                        sheet.Cells[rowStartIndex, col + 1] =
                            "=SUM(" + Get_Column_Letter(col + 1) + (rowStartIndex + 1) + ":" +
                            Get_Column_Letter(col + 1) + (row - 1).ToString() + ")";
                        sheet.Range[Get_Column_Letter(col + 1) + rowStartIndex].NumberFormat = "[<>0]#,##0.00%;[=0]0%";
                    }
                    #endregion

                    balanceEndRow = row;
                }
            }
        }

        private void CalculateFigures(List<BudgetEntry> BEList)
        {
            foreach (BudgetEntry BE in BEList)
            {
                BE.BudgetSum = BE.GetCategoryList().Sum(y => y.TargetAmount);
            }

            #region Calculate actual figures
            foreach (BudgetEntry BE in BEList)
            {
                foreach (BudgetCategory BC in BE.GetCategoryList())
                {
                    switch (BC.GetBCType())
                    {
                        case BCType.Categorical:
                        {
                            BC.ActualAmount = parent.parent.Master_Item_List.Where(x => x.Category == BC.GetName() && x.Date.Month == BE.Month && x.Date.Year == BE.Year).ToList().Sum(x => x.Get_Current_Amount(parent.parent.Get_Tax_Amount(x)));
                            break;
                        }
                        case BCType.Recurring:
                        {
                            DateTime refDate = new DateTime(BE.Year, BE.Month, 1);
                            BC.ActualAmount = parent.parent.Expenses_List.First(x => x.Expense_Name == BC.GetName())
                                .Get_Total_Paid(refDate.AddMonths(0), refDate.AddMonths(1));
                            break;
                        }
                        case BCType.Extraneous:
                        {
                            BC.ActualAmount = parent.parent.Master_Item_List.Where(x => x.Category == BC.GetName() && x.Date.Month == BE.Month && x.Date.Year == BE.Year).ToList().Sum(x => x.Get_Current_Amount(parent.parent.Get_Tax_Amount(x)));
                            break;
                        }
                    }
                }

                BE.ActualSum = BE.GetCategoryList().Sum(y => y.ActualAmount);
            }
            #endregion

        }

        public void Write_Excel_All(List<BudgetEntry> BEList)
        {
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
            sheet.Name = "Budget Allocation Report";

            int row = 1;

            //white out entire sheet
            sheet.Cells.get_Range("A1", "AZ2000").Interior.ColorIndex = 2;

            // insert title
            sheet.Cells[1, 1] = String.Format("Budget Allocation Report ({0})", parent.budgetMonth.Text);
            sheet.Cells.get_Range("A1").Font.Bold = true;
            sheet.Cells.get_Range("A1").Font.Size = 16;
            sheet.Cells.get_Range("A1").Font.ColorIndex = 56;
            sheet.Cells.get_Range("A1", "E1").Merge();
            sheet.Cells.get_Range("A1").HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
            row += 2;

            // write header
            WriteHeaderAll(ref sheet, ref row);

            // adjust alignment first before changing specific cells
            sheet.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            // write information
            WriteSpreadsheetAll(ref sheet, ref row, BEList);

            budgetEndRow--;
            actualEndRow--;
            balanceEndRow--;

            // header
            int maxCol = parent.budgetMonth.Items.Count * 2 + 1;
            for (int i = 2; i < 3 + parent.budgetMonth.Items.Count * 2; i += 2)
            {
                // Vertical thin line
                if (i < maxCol)
                {
                    sheet.Cells.get_Range(Get_Column_Letter(i + 1) + 5, Get_Column_Letter(i + 1) + (budgetEndRow)).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous; //budget
                    sheet.Cells.get_Range(Get_Column_Letter(i + 1) + (budgetEndRow + 3), Get_Column_Letter(i + 1) + (actualEndRow)).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous; //actual
                    sheet.Cells.get_Range(Get_Column_Letter(i + 1) + (actualEndRow + 3), Get_Column_Letter(i + 1) + (balanceEndRow)).Cells
                        .Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous; //actual
                }

                // Vertical thick line
                sheet.Cells.get_Range(Get_Column_Letter(i) + 3, Get_Column_Letter(i) + (budgetEndRow)).Cells
                    .Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlMedium;
                sheet.Cells.get_Range(Get_Column_Letter(i) + (budgetEndRow + 2), Get_Column_Letter(i) + (actualEndRow)).Cells
                    .Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlMedium;
                sheet.Cells.get_Range(Get_Column_Letter(i) + (actualEndRow + 2), Get_Column_Letter(i) + (balanceEndRow)).Cells
                    .Borders[Excel.XlBordersIndex.xlEdgeLeft].Weight = Excel.XlBorderWeight.xlMedium;
            }

            // Horizontal Thick Header Line (above months text)
            sheet.Cells.get_Range(Get_Column_Letter(2) + 3, Get_Column_Letter(maxCol) + 3).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlMedium;
           
            // Horizontal Thick Line
            sheet.Cells.get_Range(Get_Column_Letter(1) + 3, Get_Column_Letter(maxCol) + 3).Cells.Borders[Excel.XlBordersIndex.xlEdgeBottom].Weight = Excel.XlBorderWeight.xlMedium;

            // Horizontal Thick Footer Line
            sheet.Cells.get_Range(Get_Column_Letter(2) + (budgetEndRow + 1), Get_Column_Letter(maxCol) + (budgetEndRow + 1)).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlMedium;
            sheet.Cells.get_Range(Get_Column_Letter(2) + (actualEndRow + 1), Get_Column_Letter(maxCol) + (actualEndRow + 1)).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlMedium;
            sheet.Cells.get_Range(Get_Column_Letter(2) + (balanceEndRow + 1), Get_Column_Letter(maxCol) + (balanceEndRow + 1)).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlMedium;
           
            // Horizontal Thick Header Line
            sheet.Cells.get_Range(Get_Column_Letter(2) + 3, Get_Column_Letter(maxCol) + 3).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlMedium;
            sheet.Cells.get_Range(Get_Column_Letter(2) + (budgetEndRow + 2), Get_Column_Letter(maxCol) + (budgetEndRow + 2)).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlMedium;
            sheet.Cells.get_Range(Get_Column_Letter(2) + (actualEndRow + 2), Get_Column_Letter(maxCol) +( actualEndRow + 2)).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].Weight = Excel.XlBorderWeight.xlMedium;
           
            // adjust style
            sheet.Cells.Columns.AutoFit();

            // Fix first row
            sheet.Application.ActiveWindow.SplitRow = 3;
            sheet.Application.ActiveWindow.SplitColumn = 1;
            sheet.Application.ActiveWindow.FreezePanes = true;


            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), String.Format("Budget Allocation Report ({0}).xlsx", parent.budgetMonth.Text));

            parent.parent.GeneratedFilePaths_List.Add(path);

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

            try
            {
                //book.Close();
            }
            catch
            {
                Diagnostics.WriteLine("Error disposing EXCEL files");
            }

        }
    }
}

