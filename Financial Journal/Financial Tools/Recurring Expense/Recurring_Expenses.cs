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
    public partial class Recurring_Expenses : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        private void Grey_Out()
        {
            TFLP.Location = new Point(1, 1);
        }

        private void Grey_In()
        {
            TFLP.Location = new Point(1000, 1000);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            List<Expenses> Ref_Expenses = (view_box.Text == "Active Expenses" ? Recurring_Expenses_List : Depreciated_Expenses_List);
            Expenses Ref_Expense = (view_box.Text == "Active Expenses" ?
                                    Recurring_Expenses_List.FirstOrDefault(x => x.Date_Sequence.Count > 0) :
                                    Depreciated_Expenses_List.FirstOrDefault(x => x.Date_Sequence.Count > 0));

            if (Ref_Expense != null)
            {
                this.Width = Start_Size.Width + 30;
            } 
            else
            {
                this.Width = Start_Size.Width;
            }

            int data_height = 19;
            int start_height = Start_Size.Height + 10 + 37;  
            int start_margin = 15;
            int height_offset = 9;
                                                    //Information
            int margin1 = start_margin + 200;       //Amount
            int margin2 = margin1 + 75;             //Frequency 
            int margin3 = margin2 + 90;             //% of income
            int margin4 = margin3 + 120;            //% of income


            int row_count = 0;

            Color DrawForeColor = Color.White;
            Color BackColor = Color.FromArgb(64, 64, 64);
            Color HighlightColor = Color.FromArgb(76, 76, 76);

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(88, 88, 88));
            SolidBrush RedBrush = new SolidBrush(Color.LightPink);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 2);

            Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
            Font f = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
            Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);
            Font f_title = new Font("MS Reference Sans Serif", 11, FontStyle.Bold);

            // If has order
            if (paint)
            {

                // Draw gray header line
                e.Graphics.DrawLine(Grey_Pen, start_margin, start_height - 48, this.Width - 17, start_height - 48);
                e.Graphics.DrawString((view_box.Text == "Active Expenses" ? "Current Expenses" : "Inactive Expenses"), f_title, WritingBrush, start_margin, start_height - 38);

                // Header
                e.Graphics.DrawString("Expense Item", f_header, WritingBrush, start_margin, start_height + (row_count * data_height));
                e.Graphics.DrawString("Amount", f_header, WritingBrush, margin1, start_height + (row_count * data_height));
                e.Graphics.DrawString("Frequency", f_header, WritingBrush, margin2, start_height + (row_count * data_height));
                e.Graphics.DrawString("% of Income", f_header, WritingBrush, margin3, start_height + (row_count * data_height));
                e.Graphics.DrawString("Amt to Date", f_header, WritingBrush, margin4 - 10, start_height + (row_count * data_height));
                row_count += 1;
                height_offset += 4;

                int item_index = 0;
                double Total_Expenses = 0;
                double Total_Paid = 0;

                // Dispose images
                Delete_Expense_Buttons.ForEach(button => button.Image.Dispose());
                History_Buttons.ForEach(button => button.Image.Dispose());
                Stop_Expense_Buttons.ForEach(button => button.Image.Dispose());
                Edit_Item_Button.ForEach(button => button.Image.Dispose());

                // Remove existing buttons
                Delete_Expense_Buttons.ForEach(button => button.Dispose());
                History_Buttons.ForEach(button => button.Dispose());
                Stop_Expense_Buttons.ForEach(button => button.Dispose());
                Edit_Item_Button.ForEach(button => button.Dispose());

                Delete_Expense_Buttons.ForEach(button => this.Controls.Remove(button));
                Delete_Expense_Buttons = new List<Button>();
                History_Buttons.ForEach(button => this.Controls.Remove(button));
                History_Buttons = new List<Button>();
                Edit_Item_Button.ForEach(button => this.Controls.Remove(button));
                Edit_Item_Button = new List<Button>();
                Stop_Expense_Buttons.ForEach(button => this.Controls.Remove(button));
                Stop_Expense_Buttons = new List<Button>();

                foreach (Expenses exp in Ref_Expenses)
                {
                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button refund_button = new Button();
                    refund_button.BackColor = this.BackColor;
                    refund_button.ForeColor = this.BackColor;
                    refund_button.FlatStyle = FlatStyle.Flat;
                    refund_button.Image = global::Financial_Journal.Properties.Resources.delete;
                    refund_button.Size = new Size(26, 29);
                    refund_button.Location = new Point(this.Width - 40, start_height + height_offset + (row_count * data_height) + 12);
                    refund_button.Name = ((exp.Expense_Status == "1") ? "ad" : "id") + item_index.ToString(); // ad = active delete, id = inactive delete
                    refund_button.Text = "";
                    refund_button.Click += new EventHandler(this.delete_expense_click);
                    Delete_Expense_Buttons.Add(refund_button);
                    ToolTip1.SetToolTip(refund_button, "Delete " + exp.Expense_Name);
                    this.Controls.Add(refund_button);

                    Button stop_button = new Button();
                    stop_button.BackColor = this.BackColor;
                    stop_button.ForeColor = this.BackColor;
                    stop_button.FlatStyle = FlatStyle.Flat;
                    stop_button.Size = new Size(29, 29);
                    if (exp.Expense_Status == "1") stop_button.Image = global::Financial_Journal.Properties.Resources.stop;
                    if (exp.Expense_Status == "0") stop_button.Image = global::Financial_Journal.Properties.Resources.start;
                    stop_button.Location = new Point(this.Width - 70, start_height + height_offset + (row_count * data_height) + 12);
                    stop_button.Name = ((exp.Expense_Status == "1") ? "a" : "i") + item_index.ToString(); // a = active, i = inactive
                    stop_button.Text = "";
                    stop_button.Click += new EventHandler(this.stop_expense_click);
                    Stop_Expense_Buttons.Add(stop_button);
                    ToolTip1.SetToolTip(stop_button, ((exp.Expense_Status == "1") ? "Stop " : "Start ") + exp.Expense_Name);
                    this.Controls.Add(stop_button);

                    if (exp.Expense_Status != "0")
                    {
                        Button Edit_Button = new Button();
                        Edit_Button.BackColor = this.BackColor;
                        Edit_Button.ForeColor = this.BackColor;
                        Edit_Button.FlatStyle = FlatStyle.Flat;
                        Edit_Button.Image = exp.AutoDebit == "1" ? global::Financial_Journal.Properties.Resources.greendebit : global::Financial_Journal.Properties.Resources.debit;
                        Edit_Button.Enabled = true;
                        Edit_Button.Size = new Size(29, 29);
                        Edit_Button.Location = new Point(this.Width - 100, start_height + height_offset + (row_count * data_height) + 12);
                        Edit_Button.Name = ((exp.Expense_Status == "1") ? "ae" : "ie") + item_index.ToString(); // ad = active edit, id = inactive edit
                        Edit_Button.Text = "";
                        Edit_Button.Click += new EventHandler(this.edit_button_click);
                        //if (payment.Total > 0) Edit_Button.Enabled = false;
                        Edit_Item_Button.Add(Edit_Button);
                        ToolTip1.SetToolTip(Edit_Button, "Manage Auto Debit for " + exp.Expense_Name);
                        this.Controls.Add(Edit_Button);
                    }

                    if (exp.Date_Sequence.Count > 0)
                    {
                        Button history_button = new Button();
                        history_button.BackColor = this.BackColor;
                        history_button.ForeColor = this.BackColor;
                        history_button.FlatStyle = FlatStyle.Flat;
                        history_button.Size = new Size(29, 29);
                        history_button.Image = global::Financial_Journal.Properties.Resources.book;
                        history_button.Location = new Point(this.Width - 130 + (exp.Expense_Status == "0" ? 30 : 0) , start_height + height_offset + (row_count * data_height) + 12);
                        history_button.Name = ((exp.Expense_Status == "1") ? "ah" : "ih") + item_index.ToString(); // ah = active history, ih = inactive history
                        history_button.Text = "";
                        history_button.Click += new EventHandler(this.view_history_click);
                        History_Buttons.Add(history_button);
                        ToolTip1.SetToolTip(history_button, "View change history for " + exp.Expense_Name);
                        this.Controls.Add(history_button);
                    }

                    e.Graphics.DrawString(exp.Expense_Name, f_total, WritingBrush, start_margin + 6, start_height + height_offset + (row_count * data_height));
                    row_count++;

                    e.Graphics.DrawString(exp.Expense_Payee, f, WritingBrush, start_margin + 20, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", exp.Expense_Amount), f, WritingBrush, margin1 - (exp.Expense_Amount >= 1000 ? 7 : 0), start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(exp.Expense_Frequency, f, WritingBrush, margin2 + 3, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString((parent.Monthly_Income != 0 ? Math.Round((exp.Get_Amount_From_Weeks(4.3452380952380940116) / parent.Monthly_Income * 100), 1) + "%" : "N/A"), f, WritingBrush, margin3 + 19, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", exp.Get_Total_Paid()), f, WritingBrush, margin4 + 3 - (exp.Get_Total_Paid() >= 1000 ? 7 : 0), start_height + height_offset + (row_count * data_height));
                    row_count++;

                    Total_Expenses += exp.Get_Amount_From_Weeks(4.3452380952380940116);
                    Total_Paid += exp.Get_Total_Paid();

                    e.Graphics.DrawString("Since " + exp.Expense_Start_Date.ToShortDateString(), f, WritingBrush, start_margin + 20, start_height + height_offset + (row_count * data_height));
                    row_count++;

                    height_offset += 4;
                    item_index++;
                }

                if (view_box.Text == "Active Expenses")
                {

                    // Total line
                    e.Graphics.DrawLine(p, start_margin, start_height + height_offset + (row_count * data_height), this.Width - 19, start_height + height_offset + (row_count * data_height));
                    height_offset += 9;
                    e.Graphics.DrawString("Adjusted Monthly Totals: ", f_total, WritingBrush, start_margin + 5, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", Total_Expenses), f_total, WritingBrush, margin1 - 4, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", Total_Paid), f_total, WritingBrush, margin4 + 3 - (Total_Paid >= 1000 ? 7 : 0), start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString((parent.Monthly_Income != 0 ? Math.Round((Total_Expenses / parent.Monthly_Income * 100), 1) + "%" : "N/A"), f_total, WritingBrush, margin3 + 11, start_height + height_offset + (row_count * data_height));
                    //row_count++;

                    // Draw accounting double lines
                    height_offset += 19;
                    // First double underline
                    e.Graphics.DrawLine(p, margin1 - 4, start_height + height_offset + (row_count * data_height), margin2 - 11, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawLine(p, margin1 - 4, start_height + height_offset + 2 + (row_count * data_height), margin2 - 11, start_height + height_offset + 2 + (row_count * data_height));
                    // Second double underline
                    e.Graphics.DrawLine(p, margin3 + 10, start_height + height_offset + (row_count * data_height), margin3 + 59, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawLine(p, margin3 + 10, start_height + height_offset + 2 + (row_count * data_height), margin3 + 59, start_height + height_offset + 2 + (row_count * data_height));
                    // Third double underline
                    e.Graphics.DrawLine(p, margin4 - 4, start_height + height_offset + (row_count * data_height), margin4 + 70 - (Total_Paid >= 1000 ? 7 : 0) + (Total_Paid >= 10000 ? 10 : 0), start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawLine(p, margin4 - 4, start_height + height_offset + 2 + (row_count * data_height), margin4 + 70 - (Total_Paid >= 1000 ? 7 : 0) + (Total_Paid >= 10000 ? 10 : 0), start_height + height_offset + 2 + (row_count * data_height));
                    height_offset -= 17;
                    row_count++;
                }

                row_count++;
                this.Height = start_height + height_offset + row_count * data_height;
            }
            else
            {
                this.Height = Start_Size.Height;
            }

            TFLP.Size = new Size(this.Width - 2, this.Height - 2);

            // Dispose all objects
            p.Dispose();
            Grey_Pen.Dispose();
            GreenBrush.Dispose();
            RedBrush.Dispose();
            GreyBrush.Dispose();
            WritingBrush.Dispose();
            f_asterisk.Dispose();
            f.Dispose();
            f_strike.Dispose();
            f_total.Dispose();
            f_header.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }


        private void delete_expense_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            List<Expenses> Ref_Expense_List = (b.Name.StartsWith("a")) ? Recurring_Expenses_List : Depreciated_Expenses_List;
            Grey_Out();
            using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to delete " + Ref_Expense_List[Convert.ToInt32(b.Name.Substring(2))].Expense_Name + "?", "Warning", "No", "Yes", 0, this.Location, this.Size))
            {
                var result21 = form1.ShowDialog();
                if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                {
                    parent.Expenses_List.Remove(Ref_Expense_List[Convert.ToInt32(b.Name.Substring(2))]);

                    if (b.Name.StartsWith("ad")) // active --> inactive
                    {
                        Recurring_Expenses_List.RemoveAt(Convert.ToInt32(b.Name.Substring(2)));
                    }
                    else
                    {
                        Depreciated_Expenses_List.RemoveAt(Convert.ToInt32(b.Name.Substring(2)));
                    }
                }
            }
            Grey_In();
            this.Invalidate();
            this.Update();
        }

        private void edit_button_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            List<Expenses> Ref_Expense_List = (b.Name.StartsWith("a")) ? Recurring_Expenses_List : Depreciated_Expenses_List;

            Expenses Pass_Exp = parent.Expenses_List[parent.Expenses_List.IndexOf(Ref_Expense_List[Convert.ToInt32(b.Name.Substring(2))])];

            Grey_Out();
            Manage_AutoDebit MAD = new Manage_AutoDebit(parent, ref Pass_Exp, this.Location, this.Size);
            MAD.ShowDialog();
            Grey_In();

            Invalidate();
            Update();
        }


        private void stop_expense_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            List<Expenses> Ref_Expense_List = (b.Name.StartsWith("a")) ? Recurring_Expenses_List : Depreciated_Expenses_List;

            Grey_Out();
            using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to " + (b.Name.StartsWith("a") ? "stop " : "start " ) + Ref_Expense_List[Convert.ToInt32(b.Name.Substring(1))].Expense_Name + "?", "Warning", "No", "Yes", 0, this.Location, this.Size))
            {
                var result21 = form1.ShowDialog();
                if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                {
                    //parent.Expenses_List.Remove(Recurring_Expenses_List[Convert.ToInt32(b.Name.Substring(1))]);


                    foreach (Expenses exp in parent.Expenses_List)
                    {
                        if (exp == Ref_Expense_List[Convert.ToInt32(b.Name.Substring(1))])
                        {
                            exp.Expense_Status = (b.Name.StartsWith("a")) ? "0" : "1"; // change expense status expense from master expenses list
                            exp.Date_Sequence.Add(DateTime.Now);
                        }
                    }

                    if (b.Name.StartsWith("a")) // active --> inactive
                    {
                        Depreciated_Expenses_List.Add(Recurring_Expenses_List[Convert.ToInt32(b.Name.Substring(1))]);
                        Recurring_Expenses_List.RemoveAt(Convert.ToInt32(b.Name.Substring(1))); // remove from current recurring list
                    }
                    else if (b.Name.StartsWith("i")) // inactive --> active
                    {
                        // Set last pay date to day;
                        Depreciated_Expenses_List[Convert.ToInt32(b.Name.Substring(1))].Last_Pay_Date = DateTime.Now;
                        Recurring_Expenses_List.Add(Depreciated_Expenses_List[Convert.ToInt32(b.Name.Substring(1))]);
                        Depreciated_Expenses_List.RemoveAt(Convert.ToInt32(b.Name.Substring(1))); // remove from current recurring list
                    }
                }
            }
            Grey_In();

            this.Invalidate();
            this.Update();
        }


        private void view_history_click(object sender, EventArgs e)
        { 
            Button b = (Button)sender;
            Expenses Ref_Expense = new Expenses();

            if (b.Name.StartsWith("a")) // active --> inactive
            {
                Ref_Expense = Recurring_Expenses_List[Convert.ToInt32(b.Name.Substring(2))];
            }
            else if (b.Name.StartsWith("i")) // inactive --> active
            {
                 Ref_Expense = Depreciated_Expenses_List[Convert.ToInt32(b.Name.Substring(2))];
            }
            Start_Stop_Dialog SSD = new Start_Stop_Dialog(parent, Ref_Expense);
            SSD.ShowDialog();
        }

        private List<Button> Delete_Expense_Buttons = new List<Button>();
        private List<Button> History_Buttons = new List<Button>();
        private List<Button> Stop_Expense_Buttons = new List<Button>();
        private List<Button> Edit_Item_Button = new List<Button>();
        Receipt parent;
        bool paint = true;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;
        List<Expenses> Recurring_Expenses_List = new List<Expenses>();
        List<Expenses> Depreciated_Expenses_List = new List<Expenses>();


        public Recurring_Expenses(Receipt _parent)
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);

            Recurring_Expenses_List = parent.Expenses_List.Where(x => x.Expense_Type == "Recurring" && x.Expense_Status == "1").ToList();
            Depreciated_Expenses_List = parent.Expenses_List.Where(x => x.Expense_Type == "Recurring" && x.Expense_Status == "0").ToList();

            if (Recurring_Expenses_List.Count == 0)
            {
                paint = false;
            }
        }

        private void Sort_Expenses(bool ascending = false)
        {
            List<Expenses> Return_List = Recurring_Expenses_List;
            // Highest amount first
            if (ascending)
            {
                Recurring_Expenses_List = Return_List.OrderByDescending(x => x.Get_Amount_From_Weeks(1)).ToList();
            }
            else
            {
                Recurring_Expenses_List = Return_List.OrderBy(x => x.Get_Amount_From_Weeks(1)).ToList();
            }
        }

        FadeControl TFLP;

        private void Receipt_Load(object sender, EventArgs e)
        {
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

            TFLP.Opacity = 75;

            // Mousedown anywhere to drag
            this.DoubleBuffered = true;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            freq_box.Items.Add("Weekly");
            freq_box.Items.Add("Bi-Weekly");
            freq_box.Items.Add("Monthly");
            freq_box.Items.Add("Bi-Monthly");
            freq_box.Items.Add("Quarterly");
            freq_box.Items.Add("Semi-Annually");
            freq_box.Items.Add("Annually");

            view_box.Items.Add("Active Expenses");
            view_box.Items.Add("Inactive Expenses");

            sort_Box.Items.Add("Ascending");
            sort_Box.Items.Add("Descending");
            sort_Box.Text = "Descending";

            view_box.Text = "Active Expenses";

            CustomDTP1.Value = DateTime.Now;
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

        private void textBox7_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (type_box.Text.Length > 0 && payee_box.Text.Length > 0 && freq_box.Text.Length > 0 &&
                amt_box.Text.Length > 0 && parent.Expenses_List.Any(x => x.Expense_Name == type_box.Text))
            {
                Expenses New_Expense = new Expenses();
                New_Expense.Expense_Type = "Recurring";
                New_Expense.Expense_Name = type_box.Text;
                New_Expense.Date_Sequence = new List<DateTime>();
                New_Expense.Expense_Payee = payee_box.Text;
                New_Expense.Expense_Frequency = freq_box.Text;
                New_Expense.Expense_Amount = Convert.ToDouble(amt_box.Text.Substring(1));
                New_Expense.Expense_Start_Date = CustomDTP1.Value;
                New_Expense.Expense_Status = "1"; // Status = 1 means its active
                parent.Expenses_List.Add(New_Expense);
                Recurring_Expenses_List.Add(New_Expense);
                // populate
                if (sort_Box.Text == "Ascending")
                {
                    Sort_Expenses();
                }
                else
                {
                    Sort_Expenses(true);
                }

                paint = true;
                Invalidate();
                Update();

                type_box.Text = "";
                payee_box.Text = "";
                freq_box.Text = "";
                amt_box.Text = "$";
            }
        }

        private void amt_box_TextChanged(object sender, EventArgs e)
        {
            if (!(amt_box.Text.StartsWith("$")))
            {
                if (parent.Get_Char_Count(amt_box.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = amt_box.Text;
                    amt_box.Text = temp.Substring(1) + temp[0];
                    amt_box.SelectionStart = amt_box.Text.Length;
                    amt_box.SelectionLength = 0;
                }
                else
                {
                        amt_box.Text = "$" + amt_box.Text;
                }
            }
            else if ((amt_box.Text.Length > 1) && ((parent.Get_Char_Count(amt_box.Text, Convert.ToChar(".")) > 1) || (amt_box.Text[1].ToString() == ".") || (parent.Get_Char_Count(amt_box.Text, Convert.ToChar("$")) > 1) || (!((amt_box.Text.Substring(amt_box.Text.Length - 1).All(char.IsDigit))) && !(amt_box.Text[amt_box.Text.Length - 1].ToString() == "."))))
            {
                amt_box.TextChanged -= new System.EventHandler(amt_box_TextChanged);
                amt_box.Text = amt_box.Text.Substring(0, amt_box.Text.Length - 1);
                amt_box.SelectionStart = amt_box.Text.Length;
                amt_box.SelectionLength = 0;
                amt_box.TextChanged += new System.EventHandler(amt_box_TextChanged);
            }
        }

        private void sort_Box_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sort_Box.Text == "Ascending")
            {
                Sort_Expenses();
            }
            else
            {
                Sort_Expenses(true);

            }
            this.Invalidate();
            this.Update();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void view_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            Invalidate();
            Update();
        }

        private void CustomDTP1_ValueChanged(object sender, EventArgs e)
        {

        }
    }

}
