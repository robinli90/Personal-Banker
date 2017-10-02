using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using Microsoft.Win32;
using Excel = Microsoft.Office.Interop.Excel;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Runtime.InteropServices;

namespace Financial_Journal
{
    public partial class Investment_View : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;

        /*
        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
            rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
            //e.Graphics.FillRectangle(Brushes.DarkBlue, rc);
        }

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
            base.WndProc(ref m);
        }*/


        Receipt parent;
        Size Start_Size = new Size();
        Investment Ref_IV;
        Investment Ref_IV_Undo;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Investment_View(Receipt _parent, Investment Ref_I, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            Ref_IV = Ref_I;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));

            Ref_IV_Undo = Ref_I.Clone();
        }
        
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            this.dataGridView1.ClearSelection();
        }


        private void Receipt_Load(object sender, EventArgs e)   
        {
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

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

            dateTimePicker1.Value = Ref_IV.Start_Date;
            textBox6.Text = "$" + String.Format("{0:0.00}", Ref_IV.Principal);
            rate_box.Text = (Ref_IV.IRate * 100).ToString();

            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
            dataGridView1.GridColor = Color.White;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(98, 110, 110);

            #region Add view buttons
            DataGridViewButtonColumn buttons = new DataGridViewButtonColumn();
            {
                buttons.HeaderText = "";
                buttons.Text = "Δ";// "🔍";
                buttons.UseColumnTextForButtonValue = true;
                buttons.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;
                buttons.Width = 23;
                buttons.FlatStyle = FlatStyle.Flat;
                buttons.CellTemplate.Style.BackColor = Color.FromArgb(64, 64, 64);
                buttons.CellTemplate.Style.ForeColor = Color.White;
                buttons.DisplayIndex = 6;
            }
            dataGridView1.Columns.Insert(6, buttons);
            #endregion

            if (Ref_IV.Balance_Sequence.Count > 0)
            {
                #region Add contribution buttons2
                DataGridViewButtonColumn buttons2 = new DataGridViewButtonColumn();
                {
                    buttons2.HeaderText = "";
                    buttons2.Text = "\uD83D\uDD0D";// "🔍";
                    buttons2.UseColumnTextForButtonValue = true;
                    buttons2.AutoSizeMode =
                        DataGridViewAutoSizeColumnMode.None;
                    buttons2.Width = 23;
                    buttons2.FlatStyle = FlatStyle.Flat;
                    buttons2.CellTemplate.Style.BackColor = Color.FromArgb(64, 64, 64);
                    buttons2.CellTemplate.Style.ForeColor = Color.White;
                    buttons2.DisplayIndex = 7;
                }
                dataGridView1.Columns.Insert(7, buttons2);
                #endregion
            }

            contribution_box.KeyPress += textboxEnterKey_KeyPress;
            withdraw_amt.KeyPress += textboxEnterKey_KeyPress;
            rate_box.KeyPress += textboxEnterKey_KeyPress;
            textBox6.KeyPress += textboxEnterKey_KeyPress;
            dateTimePicker1.KeyPress += dateTimePicker1_KeyPress;

            Populate_Matrix_Grid();

            for (int i = 1; i <= 30; i++)
            {
                yearsBox.Items.Add(i.ToString());
            }

            yearsBox.SelectedIndex = Ref_IV.Matrix_Extend_Years - 1;
        }

        private void dateTimePicker1_KeyPress(object sender, KeyPressEventArgs e)
        {
            DateTimePicker g = (DateTimePicker)sender;
            if (e.KeyChar == (char)Keys.Enter)
            {
                simulate_date.PerformClick();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.ClearSelection();

            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 6 && e.RowIndex >= 1)
                {
                    Grey_Out();
                    With_Dep_Box WDB = new With_Dep_Box(parent, Ref_IV, this.Location, this.Size, 0, Ref_IV.Matrix[e.RowIndex].Entry_Date);
                    WDB.ShowDialog();
                    Ref_IV.Populate_Matrix();
                    Populate_Matrix_Grid();
                    Grey_In();
                }
                else if (e.ColumnIndex == 7 && Ref_IV.Get_Transactions_From_Period(Ref_IV.Matrix[e.RowIndex].Entry_Date).Count > 0)
                {
                    Grey_Out();
                    if (Ref_IV.Get_Transactions_From_Period(Ref_IV.Matrix[e.RowIndex].Entry_Date).Count <= 0)
                    {
                        Form_Message_Box FMB = new Form_Message_Box(parent, "No history for this period", true, -26, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    else
                    {
                        Investment_History IH = new Investment_History(parent, Ref_IV, Ref_IV.Matrix[e.RowIndex].Entry_Date, this.Location, this.Size);
                        IH.ShowDialog();
                    }
                    Grey_In();
                }
            }
        }

        private void Populate_Matrix_Grid()
        {
            dataGridView1.Rows.Clear();


            foreach (Matrix_Entry ME in Ref_IV.Matrix.GetRange(0, Ref_IV.Matrix.Count - 1))
            {
                dataGridView1.Rows.Add(ME.Period + 1,
                                       ME.Entry_Date.ToShortDateString(),
                                       "$" + String.Format("{0:0.00}", ME.Interest_Amount),
                                       "$" + String.Format("{0:0.00}", ME.Total_Interest_Since),
                                       "$" + String.Format("{0:0.00}", ME.Total_Principal_Since),
                                       "$" + String.Format("{0:0.00}", ME.Total_Balance_Since));

                // Datagrid Button Style Setting Dynamically
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Font = new Font("Segoe UI Symbol", dataGridView1.Font.Size, FontStyle.Regular);
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[6].Style.ApplyStyle(style);
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[6].ToolTipText = "Manage Entry";


                if (Ref_IV.Balance_Sequence.Count > 0)
                {
                    if (dataGridView1.Columns.Count == 7)
                    {
                        // Add missing column
                        #region Add contribution buttons2
                        DataGridViewButtonColumn buttons2 = new DataGridViewButtonColumn();
                        {
                            buttons2.HeaderText = "";
                            buttons2.Text = "\uD83D\uDD0D";// "🔍";
                            buttons2.UseColumnTextForButtonValue = true;
                            buttons2.AutoSizeMode =
                                DataGridViewAutoSizeColumnMode.None;
                            buttons2.Width = 23;
                            buttons2.FlatStyle = FlatStyle.Flat;
                            buttons2.CellTemplate.Style.BackColor = Color.FromArgb(64, 64, 64);
                            buttons2.CellTemplate.Style.ForeColor = Color.White;
                            buttons2.DisplayIndex = 7;
                        }
                        dataGridView1.Columns.Insert(7, buttons2);
                        #endregion
                    }

                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[7].Style.ApplyStyle(style);
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[7].ToolTipText = "View Transactions";
                }
                else
                {
                    if (dataGridView1.Columns.Count == 8)
                    {
                        dataGridView1.Columns.RemoveAt(7);
                    }
                }
            }


            // Style colors
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Ref_IV.Matrix[row.Index].Period == 0)
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle(); // remove first contribution button
                    style.Padding = new Padding(0, 0, 1000, 0);
                    style.BackColor = Color.FromArgb(76, 76, 76);
                    row.Cells[6].Style = style;
                }
                if (dataGridView1.Columns.Count > 7 && Ref_IV.Get_Transactions_From_Period(Ref_IV.Matrix[row.Index].Entry_Date).Count == 0)
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle(); // remove no history transactions
                    style.Padding = new Padding(0, 0, 1000, 0);
                    style.BackColor = Color.FromArgb(76, 76, 76);
                    row.Cells[7].Style = style;
                }
                if (Ref_IV.Get_Matrix_Entry(DateTime.Now, true).Period > Ref_IV.Matrix[row.Index].Period)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(88, 88, 88); // Dates that have passed
                }
                if (Ref_IV.Get_Matrix_Entry(DateTime.Now) == Ref_IV.Matrix[row.Index])
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(0, 91, 133); // Current period
                }
                if (Ref_IV.Matrix[row.Index].Period > 0 && Ref_IV.Matrix[row.Index].Total_Principal_Since > Ref_IV.Matrix[row.Index - 1].Total_Principal_Since)
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle(); // highlight increase
                    style.BackColor = Color.DarkGreen;
                    row.Cells[4].Style = style;
                }
                else if (Ref_IV.Matrix[row.Index].Period > 0 && Ref_IV.Matrix[row.Index].Total_Principal_Since < Ref_IV.Matrix[row.Index - 1].Total_Principal_Since)
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle(); // highlight decrease
                    style.BackColor = Color.Maroon;
                    row.Cells[4].Style = style;
                }
            }
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

        private void item_price_TextChanged(object sender, EventArgs e)
        {
            parent.textBox6_TextChanged(sender, e);
        }

        private void simulate_contribution_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Cursor.Current = Cursors.WaitCursor;

            if (secondThreadFormHandle == IntPtr.Zero)
            {
                Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size, "APPLYING", "CHANGES")
                {
                };
                form.HandleCreated += SecondFormHandleCreated;
                form.HandleDestroyed += SecondFormHandleDestroyed;
                form.RunInNewThread(false);
            }

            // Force form to redraw
            Application.DoEvents();

            if (contribution_box.Text.Length > 1)
            {
                Ref_IV.Balance_Sequence = new List<Investment_Transaction>();
                for (int i = 1; i < Ref_IV.Matrix.Count - 1; i++)
                {
                    Ref_IV.Deposit(Convert.ToDouble(contribution_box.Text.Substring(1)), Ref_IV.Matrix[i].Entry_Date);
                }
                Ref_IV.Populate_Matrix();
                contribution_box.Text = "$";
                Populate_Matrix_Grid();
            }

            if (secondThreadFormHandle != IntPtr.Zero)
                PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            Grey_In();
            Cursor.Current = Cursors.Default;
        }

        // If press enter on length box, activate add (nmemonics)
        private void textboxEnterKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox g = (TextBox)sender;
            if (e.KeyChar == (char)Keys.Enter && g.Text.Length > 0)
            {
                if (g.Name.Contains("contribution"))
                {
                    simulate_contribution.PerformClick();
                }
                else if (g.Name.Contains("withdraw"))
                {
                    simulate_withdraw.PerformClick();
                }
                else if (g.Name.Contains("rate_") || g.Name.Contains("textBox6"))
                {
                    button1.PerformClick();
                }
            }
        }

        private void withdraw_amt_TextChanged(object sender, EventArgs e)
        {
            parent.textBox6_TextChanged(sender, e);
        }

        private void simulate_withdraw_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Cursor.Current = Cursors.WaitCursor;

            if (secondThreadFormHandle == IntPtr.Zero)
            {
                Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size, "APPLYING", "CHANGES")
                {
                };
                form.HandleCreated += SecondFormHandleCreated;
                form.HandleDestroyed += SecondFormHandleDestroyed;
                form.RunInNewThread(false);
            }

            // Force form to redraw
            Application.DoEvents();

            if (withdraw_amt.Text.Length > 1)
            {
                Ref_IV.Balance_Sequence = new List<Investment_Transaction>();
                for (int i = 1; i < Ref_IV.Matrix.Count - 1; i++)
                {
                    Ref_IV.Withdraw(Convert.ToDouble(withdraw_amt.Text.Substring(1)), Ref_IV.Matrix[i].Entry_Date);
                }
                Ref_IV.Populate_Matrix();
                withdraw_amt.Text = "$";
                Populate_Matrix_Grid();
            }

            if (secondThreadFormHandle != IntPtr.Zero)
                PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            Grey_In();
            Cursor.Current = Cursors.Default;
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            Ref_IV = Ref_IV_Undo.Clone();
            dateTimePicker1.Value = Ref_IV.Start_Date;
            textBox6.Text = "$" + String.Format("{0:0.00}", Ref_IV.Principal);
            rate_box.Text = (Ref_IV.IRate * 100).ToString();
            Ref_IV.Populate_Matrix();
            Populate_Matrix_Grid();
        }

        private void bufferedPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void simulate_date_Click(object sender, EventArgs e)
        {
            Ref_IV.Balance_Sequence = new List<Investment_Transaction>();
            Ref_IV.Start_Date = dateTimePicker1.Value;
            Ref_IV.Populate_Matrix();
            Populate_Matrix_Grid();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            parent.textBox6_TextChanged(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox6.Text.Length > 1)
            {
                try
                {
                    //Ref_IV.Balance_Sequence = new List<Investment_Transaction>();
                    Ref_IV.IRate = Convert.ToDouble(rate_box.Text) / 100;
                    Ref_IV.Principal = Convert.ToDouble(textBox6.Text.Substring(1));
                    Ref_IV.Populate_Matrix();
                    Populate_Matrix_Grid();
                }
                catch
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Error: Invalid Rate", true, -26, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Cursor.Current = Cursors.WaitCursor;

            if (secondThreadFormHandle == IntPtr.Zero)
            {
                Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size, "APPLYING", "CHANGES")
                {
                };
                form.HandleCreated += SecondFormHandleCreated;
                form.HandleDestroyed += SecondFormHandleDestroyed;
                form.RunInNewThread(false);
            }

            // Force form to redraw
            Application.DoEvents();

            Ref_IV.Matrix_Extend_Years = Convert.ToInt32(yearsBox.Text);
            Ref_IV.Populate_Matrix();
            Populate_Matrix_Grid();

            if (secondThreadFormHandle != IntPtr.Zero)
                PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            Grey_In();
            Cursor.Current = Cursors.Default;
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

        bool GeneratingError = false;

        private void WriteHeader(ref Excel.Worksheet sheet, ref int row)
        {
            // header
            int col = 1;

            sheet.Cells[row, col++] = "Period";
            sheet.Cells[row, col++] = "Date";
            sheet.Cells[row, col++] = "Interest";
            sheet.Cells[row, col++] = "Interest Total";
            sheet.Cells[row, col++] = "Net Contribution";
            sheet.Cells[row, col++] = "Period Value";

            Excel.Range range = sheet.Cells.get_Range("A" + row.ToString(), Get_Column_Letter(col) + row.ToString());
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.Font.ColorIndex = 48;
            range.Cells.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            row++;
        }

        private void Write_Excel()
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
            sheet.Name = "Investment Spreadsheet";

            int row = 1;

            // insert title
            sheet.Cells[1, 1] = "Investment Report for " + Ref_IV.Name;
            sheet.Cells.get_Range("A1").Font.Bold = true;
            sheet.Cells.get_Range("A1").Font.Size = 20;
            sheet.Cells.get_Range("A1").Font.ColorIndex = 56;
            sheet.Cells.get_Range("A1", "F1").Merge();
            row++;
            sheet.Cells[2, 1] = "Investment Period: " + Ref_IV.Start_Date.ToShortDateString() + " to " + Ref_IV.Matrix[Ref_IV.Matrix.Count - 1].Entry_Date.ToShortDateString();
            sheet.Cells.get_Range("A2").Font.Italic = true;
            sheet.Cells.get_Range("A2").Font.Size = 10;
            sheet.Cells.get_Range("A2").Font.ColorIndex = 56;
            sheet.Cells.get_Range("A2", "F2").Merge();
            row++;
            sheet.Cells[3, 1] = "Initial contribution amount: " + Ref_IV.Principal + ", Annual Interest rate of " + Ref_IV.IRate + " compounded " + Ref_IV.Frequency.ToLower();
            sheet.Cells.get_Range("A3").Font.Italic = true;
            sheet.Cells.get_Range("A3").Font.Size = 10;
            sheet.Cells.get_Range("A3").Font.ColorIndex = 56;
            sheet.Cells.get_Range("A3", "F3").Merge();
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


            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Investment Report for " + Ref_IV.Name + " (gen. on " + DateTime.Now.ToString("MM-dd-yyyy HH-mm") + ").xlsx");

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


        private void WriteSpreadsheet(ref Excel.Worksheet sheet, ref int row)
        {
            int col = 1;
            int ME_Index = 0;

            foreach (Matrix_Entry ME in Ref_IV.Matrix.GetRange(0, Ref_IV.Matrix.Count - 1))
            {
                sheet.Cells[row, col++] = ME.Period;
                sheet.Cells[row, col++] = ME.Entry_Date.ToShortDateString();
                sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", ME.Interest_Amount);
                sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", ME.Total_Interest_Since);
                sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", ME.Total_Principal_Since);
                sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", ME.Total_Balance_Since);

                // Color current period
                if (Ref_IV.Get_Matrix_Entry(DateTime.Now) == Ref_IV.Matrix[ME_Index])
                {
                    sheet.Cells.get_Range(Get_Column_Letter(1) + row.ToString(), Get_Column_Letter(col) + row.ToString()).Interior.Color = Color.LightSkyBlue; // Current period
                }

                // Color contribution changes after first row
                if (ME_Index > 0)
                {
                    if (ME.Total_Principal_Since > Ref_IV.Matrix[ME_Index - 1].Total_Principal_Since)
                        sheet.Cells.get_Range(Get_Column_Letter(col - 2) + row.ToString()).Interior.Color = Color.LightGreen;
                    if (ME.Total_Principal_Since < Ref_IV.Matrix[ME_Index - 1].Total_Principal_Since)
                        sheet.Cells.get_Range(Get_Column_Letter(col - 2) + row.ToString()).Interior.Color = Color.LightPink;
                }

                row++; col = 1; ME_Index++;
            }
        }

        // Export to excel
        private void button3_Click(object sender, EventArgs e)
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

        private void button3_Click_1(object sender, EventArgs e)
        {
            Investment_Visualizer IV = new Investment_Visualizer(parent, Ref_IV, this.Location, this.Size);
            IV.ShowDialog();
        }

        private void rate_box_TextChanged(object sender, EventArgs e)
        {

        }
    }

    // An abastract class that implements the functionality of an image button
    // except for a single abstract method to load the Normal, Hot and Disabled 
    // images that represent the icon that is displayed on the button. The loading
    // of these images is done in each derived concrete class.
    public abstract class DataGridViewImageButtonCell : DataGridViewButtonCell
    {
        private bool _enabled;                // Is the button enabled
        private PushButtonState _buttonState; // What is the button state
        protected Image _buttonImageNormal;   // The normal image
        private int _buttonImageOffset;       // The amount of offset or border around the image

        protected DataGridViewImageButtonCell()
        {
            // In my project, buttons are disabled by default
            _enabled = true;

            // Changing this value affects the appearance of the image on the button.
            _buttonImageOffset = 2;

            // Call the routine to load the images specific to a column.
            LoadImages();
        }

        // Button Enabled Property
        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
                _buttonState = value ? PushButtonState.Normal : PushButtonState.Disabled;
            }
        }

        // PushButton State Property
        public PushButtonState ButtonState
        {
            get { return _buttonState; }
            set { _buttonState = value; }
        }

        // Image Property
        // Returns the correct image based on the control's state.
        public Image ButtonImage
        {
            get
            {
                return _buttonImageNormal;
            }
        }

        protected override void Paint(Graphics graphics,
            Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
            DataGridViewElementStates elementState, object value,
            object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            //base.Paint(graphics, clipBounds, cellBounds, rowIndex, elementState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            /*
            // Draw the cell background, if specified.
            if ((paintParts & DataGridViewPaintParts.Background) ==
                DataGridViewPaintParts.Background)
            {
                SolidBrush cellBackground =
                    new SolidBrush(Color.FromArgb(64, 64, 64));
                graphics.FillRectangle(cellBackground, cellBounds);
                cellBackground.Dispose();
            }
            */

            // Draw the cell borders, if specified.
            if ((paintParts & DataGridViewPaintParts.Border) ==
                DataGridViewPaintParts.Border)
            {
                PaintBorder(graphics, clipBounds, cellBounds, cellStyle,
                    advancedBorderStyle);
            }

            // Calculate the area in which to draw the button.
            // Adjusting the following algorithm and values affects
            // how the image will appear on the button.
            Rectangle buttonArea = cellBounds;

            Rectangle buttonAdjustment =
                BorderWidths(advancedBorderStyle);

            buttonArea.X += buttonAdjustment.X;
            buttonArea.Y += buttonAdjustment.Y;
            buttonArea.Height -= buttonAdjustment.Height;
            buttonArea.Width -= buttonAdjustment.Width;

            Rectangle imageArea = new Rectangle(
                buttonArea.X + _buttonImageOffset,
                buttonArea.Y + _buttonImageOffset,
                16,
                16);

            ButtonRenderer.DrawButton(graphics, buttonArea, ButtonImage, imageArea, false, ButtonState);
        }

        // An abstract method that must be created in each derived class.
        // The images in the derived class will be loaded here.
        public abstract void LoadImages();
    }

    // Create a column class to display the Save buttons.
    public class DataGridViewImageButtonSaveColumn : DataGridViewButtonColumn
    {
        public DataGridViewImageButtonSaveColumn()
        {
            this.CellTemplate = new DataGridViewImageButtonSaveCell();
            this.Width = 22;
            this.Resizable = DataGridViewTriState.False;
        }
    }

    // Create a cell class to display the Save button cells. It is derived from the 
    // abstract class DataGridViewImageButtonCell. The only method that has to be 
    // implemented is LoadImages to load the Normal, Hot and Disabled Save images.
    public class DataGridViewImageButtonSaveCell : DataGridViewImageButtonCell
    {
        public override void LoadImages()
        {
            _buttonImageNormal = global::Financial_Journal.Properties.Resources.magnifier;
        }
    }


}
