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
    public partial class Accounts_RP : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        private List<Button> Delete_Button = new List<Button>();
        private List<Button> Complete_Button = new List<Button>();
        private List<Button> History_Button = new List<Button>();
        List<Account> Payable_List = new List<Account>();
        List<Account> Receivable_List = new List<Account>();
        List<Account> History_List = new List<Account>();

        Receipt parent;
        bool paint = true;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        protected override void OnPaint(PaintEventArgs e)
        {
            int data_height = 19;
            int start_height = Start_Size.Height;
            int start_margin = 15;
            int height_offset = 0;
                                                    //Information
            int margin1 = start_margin + 300;       //Date
            int margin2 = margin1 + 80;             //Amount 
            int margin3 = margin2 + 8;             //Amount 
            
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
            Font f_italic = new Font("MS Reference Sans Serif", 9, FontStyle.Italic);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);
            Font f_title = new Font("MS Reference Sans Serif", 11, FontStyle.Bold);

            Payable_List = parent.Account_List.Where(x => x.Type == "Payable" && x.Status > 0).ToList();
            Receivable_List = parent.Account_List.Where(x => x.Type == "Receivable" && x.Status > 0).ToList();
            History_List = parent.Account_List.Where(x => x.Status == 0).ToList();
            

            Delete_Button.ForEach(button => button.Image.Dispose());
            Complete_Button.ForEach(button => button.Image.Dispose());
            History_Button.ForEach(button => button.Image.Dispose());

            // Remove existing buttons
            Delete_Button.ForEach(button => button.Dispose());
            Complete_Button.ForEach(button => button.Dispose());
            History_Button.ForEach(button => button.Dispose());

            Delete_Button.ForEach(button => this.Controls.Remove(button));
            Delete_Button = new List<Button>();
            Complete_Button.ForEach(button => this.Controls.Remove(button));
            Complete_Button = new List<Button>();
            History_Button.ForEach(button => this.Controls.Remove(button));
            History_Button = new List<Button>();
            int item_index = 0;

            // If has order
            if (paint)
            {
                //forloop
                e.Graphics.DrawLine(Grey_Pen, start_margin, start_height, this.Width - start_margin, start_height);
                height_offset += 10;

                // Do payables first
                if (Payable_List.Count > 0)
                {

                    item_index = 0;

                    e.Graphics.DrawString("Payables:", f_title, WritingBrush, start_margin, start_height + height_offset + (row_count * data_height));
                    row_count++;
                    height_offset += 4;
                    e.Graphics.DrawString("Payee Info", f_header, WritingBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("Date", f_header, WritingBrush, margin1, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("Amount", f_header, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
                    height_offset += data_height;
                    foreach (Account a in Payable_List)
                    {
                        // Insert Buttons
                        ToolTip ToolTip1 = new ToolTip();
                        ToolTip1.InitialDelay = 1;
                        ToolTip1.ReshowDelay = 1;

                        if (a.Remark.Length < 30)
                        {
                            e.Graphics.DrawString(a.Payer, f, WritingBrush, start_margin + 20, start_height + height_offset + (row_count * data_height));
                            height_offset += 8;
                            e.Graphics.DrawString(a.Start_Date.ToShortDateString(), f, WritingBrush, margin1 - 15, start_height + height_offset + (row_count * data_height));
                            e.Graphics.DrawString(a.Amount, f, WritingBrush, margin2 + 10, start_height + height_offset + (row_count * data_height));


                            Button delete_button = new Button();
                            delete_button.BackColor = this.BackColor;
                            delete_button.ForeColor = this.BackColor;
                            delete_button.FlatStyle = FlatStyle.Flat;
                            delete_button.Image = global::Financial_Journal.Properties.Resources.delete;
                            delete_button.Size = new Size(29, 29);
                            delete_button.Location = new Point(this.Width - 40, start_height + height_offset + (row_count * data_height) - 8);
                            delete_button.Name = "dp" + item_index.ToString(); // dr = delete receivable
                            delete_button.Text = "";
                            delete_button.Click += new EventHandler(this.delete_account_click);
                            Delete_Button.Add(delete_button);
                            ToolTip1.SetToolTip(delete_button, "Delete payable");
                            this.Controls.Add(delete_button);

                            Button complete_button = new Button();
                            complete_button.BackColor = this.BackColor;
                            complete_button.ForeColor = this.BackColor;
                            complete_button.FlatStyle = FlatStyle.Flat;
                            complete_button.Image = global::Financial_Journal.Properties.Resources.accept;
                            complete_button.Size = new Size(29, 29);
                            complete_button.Location = new Point(this.Width - 72, start_height + height_offset + (row_count * data_height) - 8);
                            complete_button.Name = "cp" + item_index.ToString(); // dr = delete receivable
                            complete_button.Text = "";
                            complete_button.Click += new EventHandler(this.delete_account_click);
                            Complete_Button.Add(complete_button);
                            ToolTip1.SetToolTip(complete_button, "Completed payable");
                            this.Controls.Add(complete_button);

                            height_offset -= 8;
                            height_offset += data_height - 5;
                            e.Graphics.DrawString(a.Remark, f_italic, WritingBrush, start_margin + 37, start_height + height_offset + (row_count * data_height));
                            row_count++;
                        }
                        else
                        {
                            e.Graphics.DrawString(a.Payer, f, WritingBrush, start_margin + 20, start_height + height_offset + (row_count * data_height));
                            height_offset += data_height - 5;
                            e.Graphics.DrawString(a.Start_Date.ToShortDateString(), f, WritingBrush, margin1 - 15, start_height + height_offset + (row_count * data_height));
                            e.Graphics.DrawString(a.Amount, f, WritingBrush, margin2 + 10, start_height + height_offset + (row_count * data_height));

                            // Insert Buttons
                            Button delete_button = new Button();
                            delete_button.BackColor = this.BackColor;
                            delete_button.ForeColor = this.BackColor;
                            delete_button.FlatStyle = FlatStyle.Flat;
                            delete_button.Image = global::Financial_Journal.Properties.Resources.delete;
                            delete_button.Size = new Size(29, 29);
                            delete_button.Location = new Point(this.Width - 40, start_height + height_offset + (row_count * data_height) - 8);
                            delete_button.Name = "dp" + item_index.ToString(); // dr = delete receivable
                            delete_button.Text = "";
                            delete_button.Click += new EventHandler(this.delete_account_click);
                            Delete_Button.Add(delete_button);
                            ToolTip1.SetToolTip(delete_button, "Delete payable");
                            this.Controls.Add(delete_button);

                            Button complete_button = new Button();
                            complete_button.BackColor = this.BackColor;
                            complete_button.ForeColor = this.BackColor;
                            complete_button.FlatStyle = FlatStyle.Flat;
                            complete_button.Image = global::Financial_Journal.Properties.Resources.accept;
                            complete_button.Size = new Size(29, 29);
                            complete_button.Location = new Point(this.Width - 72, start_height + height_offset + (row_count * data_height) - 8);
                            complete_button.Name = "cp" + item_index.ToString(); // dr = delete receivable
                            complete_button.Text = "";
                            complete_button.Click += new EventHandler(this.delete_account_click);
                            Complete_Button.Add(complete_button);
                            ToolTip1.SetToolTip(complete_button, "Completed payable");
                            this.Controls.Add(complete_button);

                            height_offset -= data_height - 5;
                            height_offset += data_height - 5;
                            e.Graphics.DrawString(a.Remark.Substring(0, 30) + "-", f_italic, WritingBrush, start_margin + 37, start_height + height_offset + (row_count * data_height));
                            height_offset += data_height - 5;
                            e.Graphics.DrawString(a.Remark.Substring(30), f_italic, WritingBrush, start_margin + 37, start_height + height_offset + (row_count * data_height));
                            row_count++;
                        }
                        item_index++;
                    }
                    if (Receivable_List.Count > 0)
                    {
                        height_offset += 9;
                        e.Graphics.DrawLine(Grey_Pen, start_margin, start_height + height_offset + (row_count * data_height), this.Width - start_margin, start_height + height_offset + (row_count * data_height));
                        height_offset -= 9;
                        row_count++;
                    }
                }

                if (Receivable_List.Count > 0)
                {
                    item_index = 0;

                    e.Graphics.DrawString("Receivables:", f_title, WritingBrush, start_margin, start_height + height_offset + (row_count * data_height));
                    row_count++;
                    height_offset += 4;
                    e.Graphics.DrawString("Payer Info", f_header, WritingBrush, start_margin + 10, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("Date", f_header, WritingBrush, margin1, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("Amount", f_header, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
                    height_offset += data_height;
                    foreach (Account a in Receivable_List)
                    {
                        // Insert Buttons
                        ToolTip ToolTip1 = new ToolTip();
                        ToolTip1.InitialDelay = 1;
                        ToolTip1.ReshowDelay = 1;

                        if (a.Remark.Length < 30)
                        {
                            e.Graphics.DrawString(a.Payer, f, WritingBrush, start_margin + 20, start_height + height_offset + (row_count * data_height));
                            height_offset += 8;
                            e.Graphics.DrawString(a.Start_Date.ToShortDateString(), f, WritingBrush, margin1 - 15, start_height + height_offset + (row_count * data_height));
                            e.Graphics.DrawString(a.Amount, f, WritingBrush, margin2 + 10, start_height + height_offset + (row_count * data_height));


                            Button delete_button = new Button();
                            delete_button.BackColor = this.BackColor;
                            delete_button.ForeColor = this.BackColor;
                            delete_button.FlatStyle = FlatStyle.Flat;
                            delete_button.Image = global::Financial_Journal.Properties.Resources.delete;
                            delete_button.Size = new Size(29, 29);
                            delete_button.Location = new Point(this.Width - 40, start_height + height_offset + (row_count * data_height) - 8);
                            delete_button.Name = "dr" + item_index.ToString(); // dr = delete receivable
                            delete_button.Text = "";
                            delete_button.Click += new EventHandler(this.delete_account_click);
                            Delete_Button.Add(delete_button);
                            ToolTip1.SetToolTip(delete_button, "Delete receivable");
                            this.Controls.Add(delete_button);

                            Button complete_button = new Button();
                            complete_button.BackColor = this.BackColor;
                            complete_button.ForeColor = this.BackColor;
                            complete_button.FlatStyle = FlatStyle.Flat;
                            complete_button.Image = global::Financial_Journal.Properties.Resources.accept;
                            complete_button.Size = new Size(29, 29);
                            complete_button.Location = new Point(this.Width - 72, start_height + height_offset + (row_count * data_height) - 8);
                            complete_button.Name = "cr" + item_index.ToString(); // dr = delete receivable
                            complete_button.Text = "";
                            complete_button.Click += new EventHandler(this.delete_account_click);
                            Complete_Button.Add(complete_button);
                            ToolTip1.SetToolTip(complete_button, "Completed receivable");
                            this.Controls.Add(complete_button);

                            height_offset -= 8;
                            height_offset += data_height - 5;
                            e.Graphics.DrawString(a.Remark, f_italic, WritingBrush, start_margin + 37, start_height + height_offset + (row_count * data_height));
                            row_count++;
                        }
                        else
                        {
                            e.Graphics.DrawString(a.Payer, f, WritingBrush, start_margin + 20, start_height + height_offset + (row_count * data_height));
                            height_offset += data_height - 5;
                            e.Graphics.DrawString(a.Start_Date.ToShortDateString(), f, WritingBrush, margin1 - 15, start_height + height_offset + (row_count * data_height));
                            e.Graphics.DrawString(a.Amount, f, WritingBrush, margin2 + 10, start_height + height_offset + (row_count * data_height));

                            // Insert Buttons
                            Button delete_button = new Button();
                            delete_button.BackColor = this.BackColor;
                            delete_button.ForeColor = this.BackColor;
                            delete_button.FlatStyle = FlatStyle.Flat;
                            delete_button.Image = global::Financial_Journal.Properties.Resources.delete;
                            delete_button.Size = new Size(29, 29);
                            delete_button.Location = new Point(this.Width - 40, start_height + height_offset + (row_count * data_height) - 8);
                            delete_button.Name = "dr" + item_index.ToString(); // dr = delete receivable
                            delete_button.Text = "";
                            delete_button.Click += new EventHandler(this.delete_account_click);
                            Delete_Button.Add(delete_button);
                            ToolTip1.SetToolTip(delete_button, "Delete receivable");
                            this.Controls.Add(delete_button);

                            Button complete_button = new Button();
                            complete_button.BackColor = this.BackColor;
                            complete_button.ForeColor = this.BackColor;
                            complete_button.FlatStyle = FlatStyle.Flat;
                            complete_button.Image = global::Financial_Journal.Properties.Resources.accept;
                            complete_button.Size = new Size(29, 29);
                            complete_button.Location = new Point(this.Width - 72, start_height + height_offset + (row_count * data_height) - 8);
                            complete_button.Name = "cr" + item_index.ToString(); // dr = delete receivable
                            complete_button.Text = "";
                            complete_button.Click += new EventHandler(this.delete_account_click);
                            Complete_Button.Add(complete_button);
                            ToolTip1.SetToolTip(complete_button, "Completed receivable");
                            this.Controls.Add(complete_button);

                            height_offset -= data_height - 5;
                            height_offset += data_height - 5;
                            e.Graphics.DrawString(a.Remark.Substring(0, 30) + "-", f_italic, WritingBrush, start_margin + 37, start_height + height_offset + (row_count * data_height));
                            height_offset += data_height - 5;
                            e.Graphics.DrawString(a.Remark.Substring(30), f_italic, WritingBrush, start_margin + 37, start_height + height_offset + (row_count * data_height));
                            row_count++;
                        }
                        item_index++;
                    }
                }
                if (History_List.Count > 0)
                {
                    if (Receivable_List.Count > 0 || Payable_List.Count > 0)
                    {
                        height_offset += 9;
                        e.Graphics.DrawLine(Grey_Pen, start_margin, start_height + height_offset + (row_count * data_height), this.Width - start_margin, start_height + height_offset + (row_count * data_height));
                        row_count++;
                    }
                    e.Graphics.DrawString("View Account History", f_title, WritingBrush, start_margin + 35, start_height + height_offset + (row_count * data_height));

                    // Insert Buttons
                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button history_button = new Button();
                    history_button.BackColor = this.BackColor;
                    history_button.ForeColor = this.BackColor;
                    history_button.FlatStyle = FlatStyle.Flat;
                    history_button.Image = global::Financial_Journal.Properties.Resources.book;
                    history_button.Size = new Size(29, 29);
                    history_button.Location = new Point(start_margin, start_height + height_offset + (row_count * data_height) - 8);
                    history_button.Name = "history_button"; // dr = delete receivable
                    history_button.Text = "";
                    history_button.Click += new EventHandler(this.delete_account_click);
                    History_Button.Add(history_button);
                    ToolTip1.SetToolTip(history_button, "View Account History");
                    this.Controls.Add(history_button);
                }

                row_count++;
                row_count++;
                this.Height = start_height + height_offset + row_count * data_height;
            }
            else
            {
                this.Height = Start_Size.Height;
            }

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
            f_italic.Dispose();
            f_total.Dispose();
            f_header.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        private void delete_account_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            if (b.Name == "history_button")
            {
                Start_Stop_Dialog SSD = new Start_Stop_Dialog(parent, new Expenses(), "Account_History");
                SSD.ShowDialog();
            }
            else if (b.Name.Contains("dr")) // delete receivable
            {
                parent.Account_List.Remove(Receivable_List[Convert.ToInt32(b.Name.Substring(2))]);
            }
            else if (b.Name.Contains("dp")) // delete receivable
            {
                parent.Account_List.Remove(Payable_List[Convert.ToInt32(b.Name.Substring(2))]);
            }
            else if (b.Name.Contains("cr")) // complete receivable
            {
                parent.Account_List.First(x => x == Receivable_List[Convert.ToInt32(b.Name.Substring(2))]).Status = 0;
                parent.Account_List.First(x => x == Receivable_List[Convert.ToInt32(b.Name.Substring(2))]).Inactive_Date = DateTime.Now;
            }
            else if (b.Name.Contains("cp")) // complete receivable
            {
                parent.Account_List.First(x => x == Payable_List[Convert.ToInt32(b.Name.Substring(2))]).Status = 0;
                parent.Account_List.First(x => x == Payable_List[Convert.ToInt32(b.Name.Substring(2))]).Inactive_Date = DateTime.Now;
            }

            this.Invalidate();
            this.Update();
        }


        public Accounts_RP(Receipt _parent)
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            type_box.Items.Add("Payable");
            type_box.Items.Add("Receivable");
            type_box.Items.Add("Personal Deposit");
            dateTimePicker1.Value = DateTime.Now;
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

        private void close_entry_button_Click(object sender, EventArgs e)
        {
            bufferedPanel1.Visible = false;
            add_entry_button.Visible = true;
            this.Height -= bufferedPanel1.Height - 15;
            Start_Size.Height -= bufferedPanel1.Height - 15;
            this.Invalidate();
        }

        private void add_entry_button_Click(object sender, EventArgs e)
        {
            bufferedPanel1.Visible = true ;
            add_entry_button.Visible = false;
            this.Height += bufferedPanel1.Height - 15;
            Start_Size.Height += bufferedPanel1.Height - 15;
            this.Invalidate();
        }

        private void type_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            paylabel2.Visible = true;
            payable_Box.Visible = true;
            if (type_box.Text == "Payable")
            {
                paylabel2.Text = "Payee";
                //paylabel2.Visible = false;
                //payable_Box.Visible = false;
            }
            else if (type_box.Text == "Receivable")
            {
                paylabel2.Text = "Payer";
            }
            else if (type_box.Text == "Personal Deposit")
            {
                paylabel2.Visible = false;
                payable_Box.Visible = false;
            }
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (type_box.Text.Length > 0 && limit_box.Text.Length > 1 && (payable_Box.Text.Length > 0 || type_box.Text == "Personal Deposit") && remark_box.Text.Length > 0)
            {
                Account ACC = new Account();
                ACC.Type = type_box.Text;
                ACC.Alert_Active = "1";
                ACC.Payer = type_box.Text == "Personal Deposit" ? "Me" : payable_Box.Text;
                ACC.Remark = String.Join(" ", remark_box.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None));
                ACC.Amount = limit_box.Text;
                ACC.Status = type_box.Text == "Personal Deposit" ? 0 : 1; // 1 = active, 0 = inactive
                ACC.Inactive_Date = dateTimePicker1.Value;
                ACC.Start_Date = dateTimePicker1.Value;
                parent.Account_List.Add(ACC);

                // reset boxes
                type_box.Text = "";
                limit_box.Text = "";
                payable_Box.Text = "";
                remark_box.Text = "";
                close_entry_button.PerformClick();
            }
        }

        private void limit_box_TextChanged(object sender, EventArgs e)
        {
            if (!(limit_box.Text.StartsWith("$")))
            {
                if (Get_Char_Count(limit_box.Text, Convert.ToChar("$")) == 1)
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
            else if ((limit_box.Text.Length > 1) && ((Get_Char_Count(limit_box.Text, Convert.ToChar(".")) > 1) || (limit_box.Text[1].ToString() == ".") || (Get_Char_Count(limit_box.Text, Convert.ToChar("$")) > 1) || (!((limit_box.Text.Substring(limit_box.Text.Length - 1).All(char.IsDigit))) && !(limit_box.Text[limit_box.Text.Length - 1].ToString() == "."))))
            {
                limit_box.TextChanged -= new System.EventHandler(limit_box_TextChanged);
                limit_box.Text = limit_box.Text.Substring(0, limit_box.Text.Length - 1);
                limit_box.SelectionStart = limit_box.Text.Length;
                limit_box.SelectionLength = 0;
                limit_box.TextChanged += new System.EventHandler(limit_box_TextChanged);
            }
        }

        // Return the token count within string given token
        public int Get_Char_Count(string comparison_text, char reference_char)
        {
            int count = 0;
            foreach (char c in comparison_text)
            {
                if (c == reference_char)
                {
                    count++;
                }
            }
            return count;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Enabled = !dateTimePicker1.Enabled;
        }
    }

    public class Account
    {
        public string Type { get; set; }
        public string Payer { get; set; }
        public string Remark { get; set; }
        public string Amount { get; set; }
        public int Status { get; set; }
        public DateTime Inactive_Date { get; set; }
        public DateTime Start_Date { get; set; }
        public string Alert_Active { get; set; }
    }
}
