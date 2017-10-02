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
    public partial class GC_Manager : Form
    {

        private List<Button> Icon_Button = new List<Button>();
        bool use_GCard = false;


        private bool Check_GC(GC GCard)
        {
            if ((parent.location_box.Text.Contains(GCard.Location) || GCard.Location.Contains(parent.location_box.Text)) && use_GCard)
            {
                return true;
            }
            return false;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Remove existing buttons
            Icon_Button.ForEach(button => this.Controls.Remove(button));
            Icon_Button.ForEach(button => button.Image.Dispose());
            Icon_Button.ForEach(button => button.Dispose());
            Icon_Button = new List<Button>();

            int data_height = 25;
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
            Font f = new Font("MS Reference Sans Serif", 8, FontStyle.Regular);
            Font f_italic = new Font("MS Reference Sans Serif", 8, FontStyle.Italic);
            Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);
            Font f_title = new Font("MS Reference Sans Serif", 11, FontStyle.Bold);

            int item_index = 0;

            // If has order
            if (parent.GC_List.Count > 0)
            {
                // Draw gray header line
                e.Graphics.DrawLine(Grey_Pen, start_margin, 70 + (bufferedPanel1.Visible ? 55 : 0), Start_Size.Width - 15, 70 + (bufferedPanel1.Visible ? 55 : 0));
                height_offset += 8;

                foreach (GC GCard in parent.GC_List.Where(x => x.Amount > 0).ToList())
                {
                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button refund_button = new Button();
                    refund_button.BackColor = this.BackColor;
                    refund_button.ForeColor = this.BackColor;
                    refund_button.FlatStyle = FlatStyle.Flat;
                    refund_button.Image = global::Financial_Journal.Properties.Resources.gc_icon;
                    refund_button.Size = new Size(40, 40);
                    refund_button.Location = new Point(start_margin + 10, start_height + height_offset + (row_count * data_height) - 4);
                    refund_button.Name = item_index.ToString();
                    refund_button.Text = "";
                    //refund_button.Click += new EventHandler(this.view_order_Click);
                    Icon_Button.Add(refund_button);
                    if (GCard.Last_Four.Length > 0) ToolTip1.SetToolTip(refund_button, "Ending in (xx-" + GCard.Last_Four + ")");
                    this.Controls.Add(refund_button);

                    refund_button = new Button();
                    refund_button.BackColor = this.BackColor;
                    refund_button.ForeColor = this.BackColor;
                    refund_button.FlatStyle = FlatStyle.Flat;
                    refund_button.Image = use_GCard && !parent.Pending_GC_Use.Contains(GCard) ? global::Financial_Journal.Properties.Resources.tick : global::Financial_Journal.Properties.Resources.delete;
                    refund_button.Size = new Size(32, 32);
                    refund_button.Location = new Point(this.Width - 48, start_height + height_offset + (row_count * data_height));
                    refund_button.Name = "d" + item_index.ToString();
                    refund_button.Text = "";
                    refund_button.Click += new EventHandler(this.view_order_Click);
                    Icon_Button.Add(refund_button);
                    ToolTip1.SetToolTip(refund_button, (use_GCard ? "Use " : "Delete ") + GCard.Location);
                    if ((parent.GC_Available_Credit > parent.Running_Total_Master & parent.Pending_GC_Use.Contains(GCard)) ||
                        (parent.GC_Available_Credit < parent.Running_Total_Master) || !use_GCard)
                    {
                        this.Controls.Add(refund_button);
                    }

                    height_offset += 2;
                    e.Graphics.DrawString(GCard.Location, f, Check_GC(GCard) ? RedBrush : WritingBrush, start_margin + 55, start_height + height_offset + (row_count * data_height));
                    row_count++;
                    height_offset -= 8;
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", GCard.Amount), f, Check_GC(GCard) ? RedBrush : WritingBrush, start_margin + 55, start_height + height_offset + (row_count * data_height));
                    row_count++;

                    item_index++;
                }
            }

            if (use_GCard)
            {
                e.Graphics.DrawString("*Order the cards used is the order in which the credit", f_asterisk, WritingBrush, 5, start_height + height_offset + (row_count * data_height));
                height_offset += 15;
                e.Graphics.DrawString("will be taken from", f_asterisk, WritingBrush, 5, start_height + height_offset + (row_count * data_height));
            }

            row_count -= 1;
            this.Height = start_height + height_offset + row_count * data_height + (parent.GC_List.Count > 0 ? 30 : 0) + (use_GCard ? 18 : 0) + (parent.GC_List.Count == 0 ? 15 : 0);

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

        FadeControl TFLP;

        private void view_order_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            GC Ref_GC = parent.GC_List.Where(x => x.Amount > 0).ToList()[Convert.ToInt32(b.Name.Substring(1))];

            if (!use_GCard)
            {
                Grey_Out();
                using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to delete " + Ref_GC.Location + " gift card?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                {
                    var result21 = form1.ShowDialog();
                    if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                    {
                        parent.GC_List.Remove(Ref_GC);
                    }
                }
                Grey_In();
            }
            else
            {
                if (parent.Pending_GC_Use.Contains(Ref_GC))
                {
                    parent.Pending_GC_Use.Remove(Ref_GC);
                    parent.GC_Available_Credit -= Ref_GC.Amount;
                }
                else
                {
                    if (!Check_GC(Ref_GC))
                    {
                        Grey_Out();
                        using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to use this " + Ref_GC.Location + " gift card? The location does not match the gift card!", "Warning", "No", "Yes", 15, this.Location, this.Size))
                        {
                            var result21 = form1.ShowDialog();
                            if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                            {
                                parent.Pending_GC_Use.Add(Ref_GC);
                                parent.GC_Available_Credit += Ref_GC.Amount;
                            }
                        }
                        Grey_In();
                    }
                    else
                    {
                        parent.Pending_GC_Use.Add(Ref_GC);
                        parent.GC_Available_Credit += Ref_GC.Amount;
                    }
                }
            }

            Invalidate();
            parent.Invalidate();
        }

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public GC_Manager(Receipt _parent, bool useGCard = false, Point g = new Point(), Size s = new Size())
        {
            use_GCard = useGCard;
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 4) - (this.Height / 2));
        }

        private void Grey_Out()
        {
            TFLP.Location = new Point(1, 1);
        }

        private void Grey_In()
        {
            TFLP.Location = new Point(1000, 1000);
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            if (!use_GCard)
            {
                //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            }

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

        // Dollar handler
        private void item_price_TextChanged(object sender, EventArgs e)
        {
            if (!(item_price.Text.StartsWith("$")))
            {
                if (parent.Get_Char_Count(item_price.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = item_price.Text;
                    item_price.Text = temp.Substring(1) + temp[0];
                    item_price.SelectionStart = item_price.Text.Length;
                    item_price.SelectionLength = 0;
                }
                else
                {
                    item_price.Text = "$" + item_price.Text;
                }
            }
            else if ((item_price.Text.Length > 1) && ((parent.Get_Char_Count(item_price.Text, Convert.ToChar(".")) > 1) || (item_price.Text[1].ToString() == ".") || (parent.Get_Char_Count(item_price.Text, Convert.ToChar("$")) > 1) || (!((item_price.Text.Substring(item_price.Text.Length - 1).All(char.IsDigit))) && !(item_price.Text[item_price.Text.Length - 1].ToString() == "."))))
            {
                item_price.TextChanged -= new System.EventHandler(item_price_TextChanged);
                item_price.Text = item_price.Text.Substring(0, item_price.Text.Length - 1);
                item_price.SelectionStart = item_price.Text.Length;
                item_price.SelectionLength = 0;
                item_price.TextChanged += new System.EventHandler(item_price_TextChanged);
            }
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (bank_box.Text.Length > 0 && item_price.Text.Length > 1)
            {
                parent.GC_List.Add(new GC() { Location = bank_box.Text, 
                                              Amount = Convert.ToDouble(item_price.Text.Substring(1)), 
                                              Last_Four = textBox6.Text,
                                              Date_Added = DateTime.Now });
                item_price.Text = "$";
                bank_box.Text = "";
                textBox6.Text = "";
                search_desc_button.PerformClick();
                Invalidate();
            }
        }

        private void bank_box_TextChanged(object sender, EventArgs e)
        {

        }
    }


    public class GC
    {
        public string Location { get; set; }
        public double Amount { get; set; }
        public string Last_Four { get; set; }
        public DateTime Date_Added { get; set; }
        public List<string> Associated_Orders { get; set; }

        public void Add_Order(string Order_No, string Amount)
        {
            if (Amount.StartsWith("$"))
            {
                Associated_Orders.Add(Order_No);
                Associated_Orders.Add(Amount.Substring(1));
            }
            else
            {
                Associated_Orders.Add(Order_No);
                Associated_Orders.Add(Amount);
            }
        }

        public GC()
        {
            Associated_Orders = new List<string>();
            Last_Four = "";
        }

        public double Get_Order_Amount(string orderNo)
        {
            for (int i = Associated_Orders.Count() - 2; i >= 0; i -= 2)
            {
                if (Associated_Orders[i] == orderNo)
                {
                    return Math.Round(Convert.ToDouble(Associated_Orders[i + 1]), 2);
                }
            }
            return 0;
        }

        // Reverse gift card transaction and place amount to original
        public bool Reverse_Transaction(string orderNo)
        {
            for (int i = Associated_Orders.Count() - 2; i >= 0 ; i -= 2)
            {
                if (Associated_Orders[i] == orderNo)
                {
                    this.Amount += Math.Round(Convert.ToDouble(Associated_Orders[i + 1]), 2);

                    // Remove order
                    Associated_Orders.RemoveAt(i + 1);
                    Associated_Orders.RemoveAt(i);
                }
            }
            return false;
        }
    }
}
