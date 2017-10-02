using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Financial_Journal
{
    public partial class Manage_Companies : Form
    {
        
        private List<Button> Icon_Button = new List<Button>();

        protected override void OnPaint(PaintEventArgs e)
        {
            // Remove existing buttons
            Icon_Button.ForEach(button => this.Controls.Remove(button));
            Icon_Button.ForEach(button => button.Image.Dispose());
            Icon_Button = new List<Button>();

            int data_height = 18;
            int start_height = Start_Size.Height + (bufferedPanel1.Visible ? 55 : 0);
            int start_margin = 15;
            int height_offset = 9;
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
            Font f_italic = new Font("MS Reference Sans Serif", 8, FontStyle.Italic);
            Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);
            Font f_title = new Font("MS Reference Sans Serif", 11, FontStyle.Bold);

            int item_index = 0;

            // If has order
            if (parent.Income_Company_List.Count > 0)
            {
                foreach (CustomIncome CI in parent.Income_Company_List)
                {

                    // Draw gray header line
                    e.Graphics.DrawLine(Grey_Pen, start_margin, 75 + (bufferedPanel1.Visible ? 55 : 0), Start_Size.Width - 15, 75 + (bufferedPanel1.Visible ? 55 : 0));
                    height_offset += 8;

                    e.Graphics.DrawString(CI.Company + ((CI.Default) ? " (Default)" : ""), f_total, WritingBrush, start_margin + 7, start_height + height_offset + (row_count * data_height));
                    row_count++;
                    e.Graphics.DrawString("Since " + CI.First_Period_Date.ToShortDateString(), f, WritingBrush, start_margin + 15, start_height + height_offset + (row_count * data_height));
                    

                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button refund_button = new Button();
                    refund_button.BackColor = this.BackColor;
                    refund_button.ForeColor = this.BackColor;
                    refund_button.FlatStyle = FlatStyle.Flat;
                    refund_button.Image = global::Financial_Journal.Properties.Resources.delete;
                    refund_button.Size = new Size(32, 32);
                    refund_button.Location = new Point(this.Width - 37, start_height + height_offset + (row_count * data_height) - 8);
                    refund_button.Name = "d" + item_index;
                    refund_button.Text = "";
                    refund_button.Click += new EventHandler(this.view_order_Click);
                    Icon_Button.Add(refund_button);
                    ToolTip1.SetToolTip(refund_button, "Delete " + CI.Company);
                    this.Controls.Add(refund_button);
                    
                    // Autodeposit money 'p'
                    refund_button = new Button();
                    refund_button.BackColor = this.BackColor;
                    refund_button.ForeColor = this.BackColor;
                    refund_button.FlatStyle = FlatStyle.Flat;
                    refund_button.Image = CI.Deposit_Account.Length > 0 ? global::Financial_Journal.Properties.Resources.greendebit : global::Financial_Journal.Properties.Resources.debit;
                    refund_button.Size = new Size(32, 32);
                    refund_button.Location = new Point(this.Width - 70, start_height + height_offset + (row_count * data_height) - 8);
                    refund_button.Name = "p" + item_index;
                    refund_button.Text = "";
                    refund_button.Click += new EventHandler(this.view_order_Click);
                    Icon_Button.Add(refund_button);
                    ToolTip1.SetToolTip(refund_button, "Manage " + CI.Company + " deposit account");
                    this.Controls.Add(refund_button);

                    if (CI.Stop_Date < new DateTime(1801, 1, 1))
                    {
                        refund_button = new Button();
                        refund_button.BackColor = this.BackColor;
                        refund_button.ForeColor = this.BackColor;
                        refund_button.FlatStyle = FlatStyle.Flat;
                        refund_button.Image = global::Financial_Journal.Properties.Resources.stop;
                        refund_button.Size = new Size(32, 32);
                        refund_button.Location = new Point(this.Width - 103, start_height + height_offset + (row_count * data_height) - 8);
                        refund_button.Name = "s" + item_index;
                        refund_button.Text = "";
                        refund_button.Click += new EventHandler(this.view_order_Click);
                        Icon_Button.Add(refund_button);
                        ToolTip1.SetToolTip(refund_button, "Stop " + CI.Company);
                        this.Controls.Add(refund_button);
                    }

                    if (CI.Default == false && CI.Stop_Date < new DateTime(1801, 1, 1))
                    {
                        refund_button = new Button();
                        refund_button.BackColor = this.BackColor;
                        refund_button.ForeColor = this.BackColor;
                        refund_button.FlatStyle = FlatStyle.Flat;
                        refund_button.Image = global::Financial_Journal.Properties.Resources.checked_box;
                        refund_button.Size = new Size(32, 32);
                        refund_button.Location = new Point(this.Width - (CI.Stop_Date > new DateTime(1801, 1, 1) ? 103 : 136), start_height + height_offset + (row_count * data_height) - 8);
                        refund_button.Name = "a" + item_index;
                        refund_button.Text = "";
                        refund_button.Click += new EventHandler(this.view_order_Click);
                        Icon_Button.Add(refund_button);
                        ToolTip1.SetToolTip(refund_button, "Set " + CI.Company + " as default company");
                        this.Controls.Add(refund_button);
                    }

                    row_count++;
                    e.Graphics.DrawString("Frequency: " + CI.Frequency, f, WritingBrush, start_margin + 15, start_height + height_offset + (row_count * data_height));

                    if (CI.Stop_Date > new DateTime(1801, 1, 1))
                    {
                        row_count++;
                        e.Graphics.DrawString("Stopped: " + CI.Stop_Date.ToShortDateString(), f, WritingBrush, start_margin + 15, start_height + height_offset + (row_count * data_height));
                    }
                    
                    item_index++;
                    row_count++;
                }
            }

            this.Height = start_height + height_offset + row_count * data_height + (parent.Income_Company_List.Count > 0 ? 24 : 0);

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
            f_italic.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);

        }


        private void view_order_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            Grey_Out();
            // delete
            if (b.Name.StartsWith("d"))
            {
                using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to delete " + parent.Income_Company_List[Convert.ToInt32(b.Name.Substring(1))].Company + "?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                {
                    var result21 = form1.ShowDialog();
                    if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                    {
                        parent.Income_Company_List.RemoveAt(Convert.ToInt32(b.Name.Substring(1)));
                    }
                }
            }
            // stop
            else if (b.Name.StartsWith("s"))
            {
                using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to stop " + parent.Income_Company_List[Convert.ToInt32(b.Name.Substring(1))].Company + "? You cannot restart once stopped", "Warning", "No", "Yes", 15, this.Location, this.Size))
                {
                    var result21 = form1.ShowDialog();
                    if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                    {
                        parent.Income_Company_List[Convert.ToInt32(b.Name.Substring(1))].Stop_Date = DateTime.Now;
                    }
                }
            }
            // set as default
            else if (b.Name.StartsWith("a"))
            {
                parent.Income_Company_List.ForEach(x => x.Default = false);
                parent.Income_Company_List[Convert.ToInt32(b.Name.Substring(1))].Default = true;
            }
            // if deposit manager
            else if (b.Name.StartsWith("p"))
            {
                Grey_Out();
                Manage_AutoDeposit MAD = new Manage_AutoDeposit(parent, parent.Income_Company_List[Convert.ToInt32(b.Name.Substring(1))], this.Location, this.Size);
                MAD.ShowDialog();
                Grey_In();
            }
            Grey_In();
            Invalidate();
        }

        Receipt parent;
        Size Start_Size = new Size();

        public Manage_Companies(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            //this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            this.Location = new Point(g.X + s.Width / 2 - this.Width / 2, g.Y + s.Height / 4);// - this.Height / 2);
            Set_Form_Color(parent.Frame_Color);
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            frequency_box.Items.Add("Weekly");
            frequency_box.Items.Add("Bi-weekly");
            frequency_box.Items.Add("Monthly");
            frequency_box.SelectedIndex = 1;
            initial_pay_date.Value = DateTime.Now;

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

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }


        private void close_button_Click(object sender, EventArgs e)
        {
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

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            bufferedPanel1.Visible = true;
            Invalidate();
        }

        private void search_desc_button_Click(object sender, EventArgs e)
        {
            bufferedPanel1.Visible = false;
            button1.Visible = true;
            Invalidate();
        }

        private void close_button_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (company_box.Text.Length > 0 && parent.Income_Company_List.Where(x => x.Company == company_box.Text).ToList().Count == 0)
            {
                CustomIncome CI = new CustomIncome() 
                                  { 
                                      Company = company_box.Text, 
                                      First_Period_Date = initial_pay_date.Value,
                                      Frequency = frequency_box.Text,
                                      Stop_Date = new DateTime(1800, 1, 1),
                                      Default = parent.Income_Company_List.Count == 0
                                  };
                CI.Populate_Intervals();
                parent.Income_Company_List.Add(CI);
                Invalidate();
                company_box.Text = "";
            }
            else if (parent.Income_Company_List.Where(x => x.Company == company_box.Text).ToList().Count >= 0)
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: A company with the same name already exists. Please use a different name", true, 0, this.Location, this.Size);
                FMB.Height += 18;
                FMB.ShowDialog();
                Grey_In();
            }
        }
    }

}
