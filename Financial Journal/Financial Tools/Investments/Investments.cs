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
    public partial class Investments : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        private List<Button> View_Payment_History_Button = new List<Button>();
        private List<Button> Delete_Item_Buttons = new List<Button>();
        private List<Button> Edit_Item_Button = new List<Button>();
        private List<Button> Interactive_Button_List = new List<Button>();

        protected override void OnPaint(PaintEventArgs e)
        {
            // Update payment values
            parent.Payment_List.ForEach(x => x.Get_Total(parent.Master_Item_List, parent.Tax_Rules_Dictionary, parent.Tax_Rate, parent.Order_List));

            int data_height = 30;
            int start_height = bufferedPanel1.Visible ? 159 : 77;
            int start_margin = 15;              // Item
            int height_offset = 9;

            int margin1 = start_margin + 130;   //Start Date
            int margin2 = margin1 + 100;        //Principal
            int margin3 = margin2 + 100;        //Rate
            int margin4 = margin3 + 60;        //Compound Freq
            int margin5 = margin4 + 140;        //Current Value

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
            Font f_italic = new Font("MS Reference Sans Serif", 8, FontStyle.Italic);

            // Draw gray header line
            e.Graphics.DrawLine(Grey_Pen, start_margin, start_height, this.Width - 15, start_height);

            Delete_Item_Buttons.ForEach(button => button.Image.Dispose());
            Delete_Item_Buttons.ForEach(button => button.Dispose());
            Delete_Item_Buttons.ForEach(button => this.Controls.Remove(button));
            Delete_Item_Buttons = new List<Button>();
            Edit_Item_Button.ForEach(button => button.Image.Dispose());
            Edit_Item_Button.ForEach(button => button.Dispose());
            Edit_Item_Button.ForEach(button => this.Controls.Remove(button));
            Edit_Item_Button = new List<Button>();
            View_Payment_History_Button.ForEach(button => button.Image.Dispose());
            View_Payment_History_Button.ForEach(button => button.Dispose());
            View_Payment_History_Button.ForEach(button => this.Controls.Remove(button));
            View_Payment_History_Button = new List<Button>();
            Interactive_Button_List.ForEach(button => button.Image.Dispose());
            Interactive_Button_List.ForEach(button => button.Dispose());
            Interactive_Button_List.ForEach(button => this.Controls.Remove(button));
            Interactive_Button_List = new List<Button>();


            if (parent.Investment_List.Count > 0)
            {
                int item_index = 0;

                height_offset += 1;
                // Header2   
                e.Graphics.DrawString("Investment", f_header, WritingBrush, start_margin, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("Start Date", f_header, WritingBrush, margin1, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("Principal", f_header, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("Rate", f_header, WritingBrush, margin3 - 7, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("Compound Freq.", f_header, WritingBrush, margin4, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("Current Value", f_header, WritingBrush, margin5, start_height + height_offset + (row_count * data_height));

                row_count += 1;
                height_offset += 5;

                foreach (Investment Invest in parent.Investment_List.Where(x => x.Active == ViewingActive).ToList())
                {

                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button delete_button = new Button();
                    delete_button.BackColor = this.BackColor;
                    delete_button.ForeColor = this.BackColor;
                    delete_button.FlatStyle = FlatStyle.Flat;
                    delete_button.Image = global::Financial_Journal.Properties.Resources.delete;
                    delete_button.Enabled = true;
                    delete_button.Size = new Size(29, 29);
                    delete_button.Location = new Point(this.Width - 40, start_height + height_offset + (row_count * data_height) - 6);
                    delete_button.Name = "d" + item_index.ToString();
                    delete_button.Text = "";
                    delete_button.Click += new EventHandler(this.dynamic_button_click);
                    Delete_Item_Buttons.Add(delete_button);
                    ToolTip1.SetToolTip(delete_button, "Delete " + Invest.Name);
                    this.Controls.Add(delete_button);

                    if (Invest.Active)
                    {
                        Button Stop_Button = new Button();
                        Stop_Button.BackColor = this.BackColor;
                        Stop_Button.ForeColor = this.BackColor;
                        Stop_Button.FlatStyle = FlatStyle.Flat;
                        Stop_Button.Image = global::Financial_Journal.Properties.Resources.stop;
                        Stop_Button.Enabled = true;
                        Stop_Button.Size = new Size(29, 29);
                        Stop_Button.Location = new Point(this.Width - 70, start_height + height_offset + (row_count * data_height) - 6);
                        Stop_Button.Name = "s" + item_index.ToString();
                        Stop_Button.Text = "";
                        Stop_Button.Click += new EventHandler(this.dynamic_button_click);
                        Interactive_Button_List.Add(Stop_Button);
                        ToolTip1.SetToolTip(Stop_Button, "Stop " + Invest.Name);
                        this.Controls.Add(Stop_Button);
                    }

                    Button Manage_Button = new Button();
                    Manage_Button.BackColor = this.BackColor;
                    Manage_Button.ForeColor = this.BackColor;
                    Manage_Button.FlatStyle = FlatStyle.Flat;
                    Manage_Button.Image = global::Financial_Journal.Properties.Resources.wallet;
                    Manage_Button.Enabled = true;
                    Manage_Button.Size = new Size(29, 29);
                    Manage_Button.Location = new Point(this.Width - 100 + (Invest.Active ? 0 : 30), start_height + height_offset + (row_count * data_height) - 6);
                    Manage_Button.Name = "m" + item_index.ToString();
                    Manage_Button.Text = "";
                    Manage_Button.Click += new EventHandler(this.dynamic_button_click);
                    //if (payment.Total > 0) Manage_Button.Enabled = false;
                    Edit_Item_Button.Add(Manage_Button);
                    ToolTip1.SetToolTip(Manage_Button, "Manage " + Invest.Name);
                    this.Controls.Add(Manage_Button);

                    Button view_button = new Button();
                    view_button.BackColor = this.BackColor;
                    view_button.ForeColor = this.BackColor;
                    view_button.FlatStyle = FlatStyle.Flat;
                    view_button.Image = global::Financial_Journal.Properties.Resources.eye;
                    view_button.Enabled = true;
                    view_button.Size = new Size(29, 29);
                    view_button.Location = new Point(this.Width - 130 + (Invest.Active ? 0 : 30), start_height + height_offset + (row_count * data_height) - 6);
                    view_button.Name = "v" + item_index.ToString();
                    view_button.Text = "";
                    view_button.Click += new EventHandler(this.dynamic_button_click);
                    View_Payment_History_Button.Add(view_button);
                    ToolTip1.SetToolTip(view_button, "View " + Invest.Name);
                    this.Controls.Add(view_button);

                    e.Graphics.DrawString(Invest.Name, f, WritingBrush, start_margin, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(Invest.Start_Date.ToShortDateString(), f, WritingBrush, margin1, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", Invest.Principal), f, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString((Invest.IRate).ToString("P2"), f, WritingBrush, margin3 - 7, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(Invest.Frequency, f, WritingBrush, margin4 + 12, start_height + height_offset + (row_count * data_height));
                    //e.Graphics.DrawString("$" + String.Format("{0:0.00}", Invest.Get_Balance_Since(DateTime.Now)), f, WritingBrush, margin5 + 10, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", Invest.Get_Amt_Since_Period_Start(DateTime.Now)), f, WritingBrush, margin5 + 10, start_height + height_offset + (row_count * data_height));
                    
                    row_count++;
                    item_index++;

                    
                }

                height_offset += 10;
                item_index++;
            }

            height_offset += 5;
            this.Height = start_height + height_offset + row_count * data_height;

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

        bool ViewingActive = true;

        private void dynamic_button_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            int index = Convert.ToInt32(b.Name.Substring(1));
            Investment Ref_I = parent.Investment_List.Where(x => x.Active == ViewingActive).ToList()[index];
            Grey_Out();

            // If delete
            if (b.Name.StartsWith("d"))
            {
                using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to delete " + Ref_I.Name + "?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                {
                    var result21 = form1.ShowDialog();
                    if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                    {
                        parent.Investment_List.Remove(Ref_I);
                    }
                }
            }
            // If stop
            else if (b.Name.StartsWith("s"))
            {
                using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to stop " + Ref_I.Name + "?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                {
                    var result21 = form1.ShowDialog();
                    if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                    {
                        parent.Investment_List.FirstOrDefault(x => x == Ref_I).Active = false;
                    }
                }
            }
            // If manage
            else if (b.Name.StartsWith("m"))
            {
                With_Dep_Box WDB = new With_Dep_Box(parent, Ref_I, this.Location, this.Size);
                WDB.ShowDialog();
            }
            // if view
            else if (b.Name.StartsWith("v"))
            {
                Grey_In();
                Investment_View IV = new Investment_View(parent, Ref_I.Clone(), this.Location, this.Size);
                IV.ShowDialog();
            }
            Grey_In();
            Invalidate();
        }

        Receipt parent;
        Size Start_Size = new Size();


        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Investments(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
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

            dateTimePicker1.Value = DateTime.Now;

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

            frequency_box.Items.Add("Monthly");
            frequency_box.Items.Add("Bi-monthly");
            frequency_box.Items.Add("Semi-annually");
            frequency_box.Items.Add("Annually");

            frequency_box.SelectedIndex = 0;
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

        private void button1_Click(object sender, EventArgs e)
        {
            bufferedPanel1.Visible = true;
            label10.Visible = false;
            button1.Visible = false;
            Invalidate();
        }


        private void search_desc_button_Click_1(object sender, EventArgs e)
        {
            bufferedPanel1.Visible = false;
            label10.Visible = true;
            button1.Visible = true;
            Invalidate();
        }

        private void bufferedPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void rate_box_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void principal_box_TextChanged(object sender, EventArgs e)
        {
            if (!(principal_box.Text.StartsWith("$")))
            {
                if (parent.Get_Char_Count(principal_box.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = principal_box.Text;
                    principal_box.Text = temp.Substring(1) + temp[0];
                    principal_box.SelectionStart = principal_box.Text.Length;
                    principal_box.SelectionLength = 0;
                }
                else
                {
                    principal_box.Text = "$" + principal_box.Text;
                }
            }
            else if ((principal_box.Text.Length > 1) && ((parent.Get_Char_Count(principal_box.Text, Convert.ToChar(".")) > 1) || (principal_box.Text[1].ToString() == ".") || (parent.Get_Char_Count(principal_box.Text, Convert.ToChar("$")) > 1) || (!((principal_box.Text.Substring(principal_box.Text.Length - 1).All(char.IsDigit))) && !(principal_box.Text[principal_box.Text.Length - 1].ToString() == "."))))
            {
                principal_box.TextChanged -= new System.EventHandler(principal_box_TextChanged);
                principal_box.Text = principal_box.Text.Substring(0, principal_box.Text.Length - 1);
                principal_box.SelectionStart = principal_box.Text.Length;
                principal_box.SelectionLength = 0;
                principal_box.TextChanged += new System.EventHandler(principal_box_TextChanged);
            }
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (name_box.Text.Length > 1 && principal_box.Text.Length > 1 && rate_box.Text.Length > 0 && !parent.Investment_List.Any(x => x.Name == name_box.Text))
            {
                try
                {
                    double Rate = Convert.ToDouble(rate_box.Text) / 100;
                    Investment Investment = new Investment()
                                            {
                                                Name = name_box.Text,
                                                Active = true,
                                                Principal = Convert.ToDouble(principal_box.Text.Substring(1)),
                                                IRate = Rate,
                                                Frequency = frequency_box.Text,
                                                Start_Date = dateTimePicker1.Value,
                                                Balance_Sequence = new List<Investment_Transaction>()
                                            };

                    Investment.Populate_Matrix();
                    parent.Investment_List.Add(Investment);
                    name_box.Text = "";
                    rate_box.Text = "";
                    principal_box.Text = "$";
                    Invalidate();
                }
                catch
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Error: Invalid Rate", true, -20, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Enabled = !dateTimePicker1.Enabled;
        }
    }
}
