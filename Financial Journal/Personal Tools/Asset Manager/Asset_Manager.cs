using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace Financial_Journal
{
    public partial class Asset_Manager : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }
        /*
         * Resizing form
         * 
        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;

        protected override void OnPaint(PaintEventArgs e) {
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
            rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
            //e.Graphics.FillRectangle(Brushes.DarkBlue, rc);
        }

        protected override void WndProc(ref Message m) 
        {
            if (m.Msg == 0x84) {  // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption) {
                    m.Result = (IntPtr)2;  // HTCAPTION
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip) {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
         * 
                    return;
                }
            }
            base.WndProc(ref m);
        }
        */

        Receipt parent;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Asset_Manager(Receipt _parent, Point g = new Point(), Size s = new Size())
        {

            InitializeComponent();

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

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Load all existing asset categories and cast uniqueness 
            List<string> Categories = parent.Asset_List.Select(x => x.Asset_Category).Distinct().ToList();
            Categories = Categories.OrderBy(x => x).ToList();
            Categories.ForEach(x => category_box.Items.Add(x));

            category_box.SelectedIndex = 0;


            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
            dataGridView1.GridColor = Color.White;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(98, 110, 110);

            #region Add view buttons
            DataGridViewButtonColumn buttons = new DataGridViewButtonColumn();
            {
                buttons.HeaderText = "";
                buttons.Text = "\uD83D\uDD0D";// "🔍";
                buttons.UseColumnTextForButtonValue = true;
                buttons.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;
                buttons.Width = 23;
                buttons.FlatStyle = FlatStyle.Flat;
                buttons.CellTemplate.Style.BackColor = Color.FromArgb(64, 64, 64);
                buttons.CellTemplate.Style.ForeColor = Color.White;
                buttons.DisplayIndex = 3;
            }
            dataGridView1.Columns.Insert(3, buttons);
            #endregion

            #region Add delete buttons
            DataGridViewButtonColumn buttons2 = new DataGridViewButtonColumn();
            {
                buttons2.HeaderText = "";
                buttons2.Text = "X";// "🔍";
                buttons2.UseColumnTextForButtonValue = true;
                buttons2.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;
                buttons2.Width = 23;
                buttons2.FlatStyle = FlatStyle.Flat;
                buttons2.CellTemplate.Style.BackColor = Color.FromArgb(64, 64, 64);
                buttons2.CellTemplate.Style.ForeColor = Color.White;
                buttons2.DisplayIndex = 4;
            }
            dataGridView1.Columns.Insert(4, buttons2);
            #endregion

            #region Add remove buttons
            DataGridViewButtonColumn buttons3 = new DataGridViewButtonColumn();
            {
                buttons3.HeaderText = "";
                buttons3.Text = "$";// "🔍";
                buttons3.UseColumnTextForButtonValue = true;
                buttons3.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;
                buttons3.Width = 23;
                buttons3.FlatStyle = FlatStyle.Flat;
                buttons3.CellTemplate.Style.BackColor = Color.FromArgb(64, 64, 64);
                buttons3.CellTemplate.Style.ForeColor = Color.White;
                buttons3.DisplayIndex = 5;
            }
            dataGridView1.Columns.Insert(5, buttons3);
            #endregion

            loaded = true;
            PopulateDGV();

            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;
            ToolTip1.SetToolTip(button2, "Add new Asset Category");
            ToolTip1.SetToolTip(button6, "Rename Asset Category");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridView1.ClearSelection();
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0 && e.RowIndex <= dataGridView1.Rows.Count - 2)
            {
                Grey_Out();
                Asset_Item Ref_AI = Current_Items[e.RowIndex];
                if (e.ColumnIndex == 3)
                {
                    Add_Asset AA = new Add_Asset(parent, category_box.Text, Ref_AI, this.Location, this.Size);
                    AA.ShowDialog();
                }
                else if (e.ColumnIndex == 4)
                {
                    using (var form = new Yes_No_Dialog(parent, "Are you sure you to delete " + Ref_AI.Name + "?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                    {
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK && form.ReturnValue1 == "1")
                        {
                            parent.Asset_List.Remove(Ref_AI);
                        }
                    }
                }
                else if (e.ColumnIndex == 5 && Ref_AI.Remove_Date.Year < 1801)
                {
                    Asset_Selling AS = new Asset_Selling(parent, Ref_AI, this.Location, this.Size);
                    AS.ShowDialog();
                }
                PopulateDGV();
                Grey_In();
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            this.dataGridView1.ClearSelection();
        }

        bool loaded = false;
        FadeControl TFLP;

        List<Asset_Item> Current_Items = new List<Asset_Item>();

        private void PopulateDGV()
        {
            dataGridView1.Rows.Clear();
            Current_Items = new List<Asset_Item>();
            
            // Sort asset list
            parent.Asset_List = parent.Asset_List.OrderBy(x => x.Name).ToList();

            Current_Items = parent.Asset_List.Where(x => x.Asset_Category == category_box.Text).ToList();

            if (category_box.Text.Length > 0)
            {
                // populate
                foreach (Asset_Item AI in Current_Items)
                {
                    dataGridView1.Rows.Add((AI.Remove_Date.Year > 1801 ? (AI.Selling_Amount > 0 ? "[SOLD] " : "[DISPOSED] ") : "") + AI.Name, "$" + String.Format("{0:0.00}", (AI.Remove_Date.Year > 1801 ? (AI.Selling_Amount) : AI.Cost)), AI.Purchase_Location, "", "", "");
                }

                dataGridView1.Rows.Add(category_box.Text + " Total:", "$" + String.Format("{0:0.00}", Current_Items.Where(x => x.Remove_Date.Year < 1801).ToList().Sum(x => x.Cost)), Current_Items.Count + " asset(s)", "", "", "");
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                try
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    style.Font = new Font("Segoe UI Symbol", dataGridView1.Font.Size, FontStyle.Regular);
                    row.Cells[3].Style.ApplyStyle(style);
                    row.Cells[3].ToolTipText = "Manage Entry";
                    row.Cells[4].ToolTipText = "Delete Entry";
                    row.Cells[5].ToolTipText = "Sold Entry";
                }
                catch
                {
                }
            }

            // Apply removed changes
            for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
            {
                DataGridViewRow row = dataGridView1.Rows[i];
                Asset_Item Ref_AI = Current_Items[row.Index];
                if (Ref_AI.Remove_Date.Year > 1809) //If removed date changed
                {
                    //
                    if (Ref_AI.Selling_Amount >= Ref_AI.Cost)
                    {
                        row.DefaultCellStyle.BackColor = Color.DarkGreen; // less
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.Maroon; // less
                    }

                    DataGridViewCellStyle style2 = new DataGridViewCellStyle();
                    style2.Padding = new Padding(0, 0, 1000, 0);
                    style2.BackColor = Color.FromArgb(76, 76, 76);
                    row.Cells[5].Style = style2;

                }
            }

            #region Apply style to last row
            DataGridViewRow lastRow = dataGridView1.Rows[dataGridView1.Rows.Count - 1];
            DataGridViewCellStyle lastRowStyle = new DataGridViewCellStyle(); // highlight increase
            lastRowStyle.BackColor = Color.FromArgb(0, 91, 133);
            //lastRowStyle.Font = new Font(dataGridView1.Font.FontFamily, dataGridView1.Font.Size, FontStyle.Bold);
            lastRow.Cells[1].Style = lastRowStyle;
            lastRow.Cells[0].Style = lastRowStyle;

            DataGridViewCellStyle style1 = new DataGridViewCellStyle();
            style1.Padding = new Padding(0, 0, 1000, 0);
            style1.BackColor = Color.FromArgb(76, 76, 76);
            lastRow.Cells[3].Style = style1;
            lastRow.Cells[4].Style = style1;
            lastRow.Cells[5].Style = style1;
            #endregion
        }

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
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Input_Box_Small(parent, "Add New Asset Category using name:", "Asset Category", "Add", null, this.Location, this.Size, 25, true))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string Asset_Category_Name = form.Pass_String;

                    if (category_box.Items.Contains(Asset_Category_Name))
                    {
                        Form_Message_Box FMB = new Form_Message_Box(parent, "Asset Category with name exists", true, -15, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    else
                    {
                        category_box.Items.Add(Asset_Category_Name);
                        category_box.Text = Asset_Category_Name;
                    }
                }
            }
            Grey_In();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Grey_Out();
            if (category_box.Text.Length > 0)
            {
                Add_Asset AA = new Add_Asset(parent, category_box.Text, null, this.Location, this.Size);
                AA.ShowDialog();
                PopulateDGV();
            }
            else
            {
                Form_Message_Box FMB = new Form_Message_Box(parent, "No asset category selected", true, -15, this.Location, this.Size);
                FMB.ShowDialog();
            }
            Grey_In();
        }

        private void label28_Click(object sender, EventArgs e)
        {
        }

        private void category_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loaded) PopulateDGV();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Asset_Import AI = new Asset_Import(parent, category_box.Text, this.Location, this.Size);
            AI.ShowDialog();
            PopulateDGV();
            Grey_In();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Yes_No_Dialog(parent, "Are you sure you to remove all assets?", "Warning", "No", "Yes", 0, this.Location, this.Size))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK && form.ReturnValue1 == "1")
                {
                    parent.Asset_List = parent.Asset_List.Where(x => x.Asset_Category != category_box.Text).ToList();
                    PopulateDGV();
                }
            }
            Grey_In();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Yes_No_Dialog(parent, "Are you sure you to remove all imports?", "Warning", "No", "Yes", 0, this.Location, this.Size))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK && form.ReturnValue1 == "1")
                {
                    parent.Asset_List = parent.Asset_List.Where(x => x.Asset_Category != category_box.Text || (x.OrderID.Length == 0 && x.Asset_Category == category_box.Text)).ToList();
                    PopulateDGV();
                }
            }
            Grey_In();
        }

        private void excel_button_Click(object sender, EventArgs e)
        {
            Grey_Out();

            // Force form to redraw
            Application.DoEvents();

            using (var form2 = new Yes_No_Dialog(parent, "Which asset category do you wish to export?", "Warning", "Current", "All", 0, this.Location, this.Size))
            {
                var result = form2.ShowDialog();

                if (result == DialogResult.OK)
                {
                    if (secondThreadFormHandle == IntPtr.Zero)
                    {
                        Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size)
                        {
                        };
                        form.HandleCreated += SecondFormHandleCreated;
                        form.HandleDestroyed += SecondFormHandleDestroyed;
                        form.RunInNewThread(false);
                    }
                }

                if (result == DialogResult.OK && form2.ReturnValue1 == "1")
                {
                    // all categories
                    Write_Excel(true);
                }
                else if (result == DialogResult.OK)
                {
                    // current category
                    Write_Excel();
                }


                if (secondThreadFormHandle != IntPtr.Zero)
                    PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);       
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

        bool GeneratingError = false;

        private void WriteHeader(ref Excel.Worksheet sheet, ref int row, bool isAllCategories = false)
        {
            // header
            int col = 1;

            sheet.Cells[row, col++] = "Item";
            sheet.Cells[row, col++] = "Price";
            sheet.Cells[row, col++] = "Location";
            sheet.Cells[row, col++] = "Purchase Date";
            sheet.Cells[row, col++] = "Serial";
            sheet.Cells[row, col++] = "Notes";
            sheet.Cells[row, col++] = "Removed Date";
            sheet.Cells[row, col++] = "Selling Cost";

            Excel.Range range = sheet.Cells.get_Range("A" + row.ToString(), Get_Column_Letter(col) + row.ToString());
            range.Font.Bold = true;
            range.Font.Size = 13;
            range.Font.ColorIndex = 48;
            range.Cells.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            row++;
        }

        private void WriteSpreadsheelAll(ref Excel.Worksheet sheet, ref int row)
        {
            int col = 1;


            foreach (string category in category_box.Items)
            {
                col = 1;
                sheet.Cells[row, col] = category;
                sheet.Cells.get_Range("A" + (row).ToString()).Font.Bold = true;
                sheet.Cells.get_Range("A" + row.ToString()).Interior.ColorIndex = 42;
                row++;

                int category_start_row = row;

                foreach (Asset_Item AI in parent.Asset_List.Where(x => x.Asset_Category == category).ToList().OrderBy(x => x.Name).ToList())
                {
                    sheet.Cells[row, col++] = AI.Name;
                    sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", AI.Remove_Date.Year > 1801 ? AI.Selling_Amount : AI.Cost);
                    sheet.Cells[row, col++] = AI.Purchase_Location;
                    sheet.Cells[row, col++] = AI.Purchase_Date.ToShortDateString();
                    sheet.Cells[row, col++] = AI.Serial_Identification.Length > 0 ? AI.Serial_Identification : "-";
                    sheet.Cells[row, col++] = AI.Note.Length > 0 ? AI.Note : "-";
                    sheet.Cells[row, col++] = AI.Remove_Date.Year < 1801 ? "N/A" : AI.Remove_Date.ToShortDateString();
                    sheet.Cells[row, col++] = AI.Remove_Date.Year < 1801 ? "N/A" : "$" + String.Format("{0:0.00}", AI.Selling_Amount);
                    
                    if (AI.Remove_Date.Year > 1801)
                        sheet.Cells[row, col++] = "Original price: " + "$" + String.Format("{0:0.00}", AI.Cost);

                    // Color lines where item is sold
                    if (AI.Remove_Date.Year > 1801 && AI.Cost < AI.Selling_Amount)
                        sheet.Cells.get_Range("A" + row.ToString(), Get_Column_Letter(col - 1) + row.ToString()).Interior.Color = Color.LightGreen;
                    if (AI.Remove_Date.Year > 1801 && AI.Cost > AI.Selling_Amount)
                        sheet.Cells.get_Range("A" + row.ToString(), Get_Column_Letter(col - 1) + row.ToString()).Interior.Color = Color.LightPink;

                    // Right side alignment
                    sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    row++; col = 1;
                }
                #region Final Total
                sheet.Cells[row, 1] = category + " Total:";
                sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
                sheet.Cells.get_Range("A" + (row).ToString(), "H" + (row).ToString()).Font.Bold = true;
                sheet.Cells.get_Range("A" + (row).ToString(), "H" + (row).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;

                sheet.Cells[row, 2] = "=SUM(" + "B" + category_start_row + ":" + "B" + (row - 1) + ")";

                // Last row (sum only if there is a value)
                if (parent.Asset_List.Any(x => x.Selling_Amount > 0 && x.Asset_Category == category))
                {
                    sheet.Cells[row, 8] = "=SUM(" + "H" + category_start_row + ":" + "H" + (row - 1) + ")";
                }

                sheet.Range["B" + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)"; // style currency
                sheet.Range["H" + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)"; // style currency
                #endregion

                row+=2;

            }

            #region Final Total
            sheet.Cells[row, 1] = "Grand Total:";
            sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            sheet.Cells.get_Range("A" + (row).ToString(), "H" + (row).ToString()).Font.Bold = true;
            sheet.Cells.get_Range("A" + (row).ToString(), "H" + (row).ToString()).Font.Size = 13;
            sheet.Cells.get_Range("A" + (row).ToString(), "H" + (row).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            sheet.Cells.get_Range("B" + (row + 1).ToString(), "B" + (row + 1).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlDouble;

            sheet.Cells[row, 2] = "=SUM(" + "B4:" + "B" + (row - 1) + ") / 2";

            // Last row (sum only if there is a value)
            if (parent.Asset_List.Any(x => x.Selling_Amount > 0))
            {
                sheet.Cells.get_Range("H" + (row + 1).ToString(), "H" + (row + 1).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlDouble;
                sheet.Cells[row, 8] = "=SUM(" + "H4:" + "H" + (row - 1) + ") / 2";
            }

            sheet.Range["B" + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)"; // style currency
            sheet.Range["H" + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)"; // style currency
            #endregion
        }

        private void WriteSpreadsheetCurrent(ref Excel.Worksheet sheet, ref int row)
        {
            int col = 1;

            foreach (Asset_Item AI in Current_Items)
            {
                sheet.Cells[row, col++] = AI.Name;
                sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", AI.Cost);
                sheet.Cells[row, col++] = AI.Purchase_Location;
                sheet.Cells[row, col++] = AI.Purchase_Date.ToShortDateString();
                sheet.Cells[row, col++] = AI.Serial_Identification.Length > 0 ? AI.Serial_Identification : "-";
                sheet.Cells[row, col++] = AI.Note.Length > 0 ? AI.Note : "-";
                sheet.Cells[row, col++] = AI.Remove_Date.Year < 1801 ? "N/A" : AI.Remove_Date.ToShortDateString();
                sheet.Cells[row, col++] = AI.Remove_Date.Year < 1801 ? "N/A" : "$" + String.Format("{0:0.00}", AI.Selling_Amount);

                // Color lines where item is sold
                if (AI.Remove_Date.Year > 1801 && AI.Cost < AI.Selling_Amount)
                    sheet.Cells.get_Range("A" + row.ToString(), Get_Column_Letter(col - 1) + row.ToString()).Interior.Color = Color.LightGreen;
                if (AI.Remove_Date.Year > 1801 && AI.Cost > AI.Selling_Amount)
                    sheet.Cells.get_Range("A" + row.ToString(), Get_Column_Letter(col - 1) + row.ToString()).Interior.Color = Color.LightPink;

                row++; col = 1; 
            }

            #region Final Total
            sheet.Cells[row, 1] = "Grand Total:";
            sheet.Cells.get_Range("A" + (row).ToString()).HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
            sheet.Cells.get_Range("A" + (row).ToString(), "H" + (row).ToString()).Font.Bold = true;
            sheet.Cells.get_Range("A" + (row).ToString(), "H" + (row).ToString()).Font.Size = 13;
            sheet.Cells.get_Range("A" + (row).ToString(), "H" + (row).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            sheet.Cells.get_Range("B" + (row + 1).ToString(), "B" + (row + 1).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlDouble;

            sheet.Cells[row, 2] = "=SUM(" + "B4:" + "B" + (row - 1) + ")";

            // Last row (sum only if there is a value)
            if (Current_Items.Any(x => x.Selling_Amount > 0))
            {
                sheet.Cells.get_Range("H" + (row + 1).ToString(), "H" + (row + 1).ToString()).Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlDouble;
                sheet.Cells[row, 8] = "=SUM(" + "H4:" + "H" + (row - 1) + ")";
            }

            sheet.Range["B" + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)"; // style currency
            sheet.Range["H" + row].NumberFormat = "$#,##0.00;[Red]($#,##0.00)"; // style currency
            #endregion
        }

        private void Write_Excel(bool isAllCategories = false)
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
            sheet.Cells[1, 1] = "Asset Report";
            sheet.Cells.get_Range("A1").Font.Bold = true;
            sheet.Cells.get_Range("A1").Font.Size = 20;
            sheet.Cells.get_Range("A1").Font.ColorIndex = 56;
            sheet.Cells.get_Range("A1", (isAllCategories ? "I" : "H") + "1").Merge();
            row++;
            sheet.Cells[2, 1] = "All assets for " + (isAllCategories ? "all categories" : category_box.Text);
            sheet.Cells.get_Range("A2").Font.Italic = true;
            sheet.Cells.get_Range("A2").Font.Size = 10;
            sheet.Cells.get_Range("A2").Font.ColorIndex = 56;
            sheet.Cells.get_Range("A2", (isAllCategories ? "I" : "H") + "2").Merge();
            row++;
            //sheet.Cells[3, 1] = "Initial contribution amount: " + Ref_IV.Principal + ", Annual Interest rate of " + Ref_IV.IRate + " compounded " + Ref_IV.Frequency.ToLower();
            //sheet.Cells.get_Range("A3").Font.Italic = true;
            //sheet.Cells.get_Range("A3").Font.Size = 10;
            //sheet.Cells.get_Range("A3").Font.ColorIndex = 56;
            //sheet.Cells.get_Range("A3", "F3").Merge();
            row++;

            // write header
            WriteHeader(ref sheet, ref row, isAllCategories);

            // adjust alignment first before changing specific cells
            sheet.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            // write information
            if (isAllCategories) WriteSpreadsheelAll(ref sheet, ref row);
            else WriteSpreadsheetCurrent(ref sheet, ref row);

            // adjust style
            sheet.Cells.Columns.AutoFit();

            // Fix first row
            sheet.Application.ActiveWindow.SplitRow = 4;
            //sheet.Application.ActiveWindow.SplitColumn = 2;
            sheet.Application.ActiveWindow.FreezePanes = true;

            // Now apply autofilter
            Excel.Range firstRow = (Excel.Range)sheet.Rows[4];


            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Asset Report for " + (isAllCategories ? "All Categories" : category_box.Text) + " (gen. on " + DateTime.Now.ToString("MM-dd-yyyy HH-mm") + ").xlsx");

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

        private void button6_Click(object sender, EventArgs e)
        {
            Grey_Out();

            using (var form = new Input_Box_Small(parent, "Rename Asset Category using name:", category_box.Text, "Save", null, this.Location, this.Size, 25, true))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string Asset_Category_Name = form.Pass_String;

                    if (category_box.Items.Contains(Asset_Category_Name))
                    {
                        Form_Message_Box FMB = new Form_Message_Box(parent, "Asset Category with name exists", true, -15, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                    else
                    {
                        using (var form11 = new Yes_No_Dialog(parent, "Are you sure you want to rename " + category_box.Text + " to " + Asset_Category_Name + "?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                        {
                            var result21 = form11.ShowDialog();
                            if (result21 == DialogResult.OK && form11.ReturnValue1 == "1")
                            {
                            }
                        }

                        // Rename all old asset names to current asset names
                        parent.Asset_List.Where(x => x.Asset_Category == category_box.Text).ToList().ForEach(x => x.Asset_Category = Asset_Category_Name);

                        // Clear item list
                        category_box.Items.Clear();

                        // Load all existing asset categories and cast uniqueness 
                        List<string> Categories = parent.Asset_List.Select(x => x.Asset_Category).Distinct().ToList();
                        Categories = Categories.OrderBy(x => x).ToList();
                        Categories.ForEach(x => category_box.Items.Add(x));

                        category_box.Text = Asset_Category_Name;
                    }
                }
            }

            Grey_In();
        }
    }
}
