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
    public partial class Online_Orders : Form
    {
        private int Entries_Per_Page = 16;
        int Pages_Required = 0;
        int Current_Page = 0;

        bool paint = false;
        private List<Button> Search_Order_Button = new List<Button>();
        
        protected override void OnPaint(PaintEventArgs e)
         
        {
            int data_height = 23;
            int start_height = 35;
            int start_margin = 20;              //Location
            int height_offset = 9;

            int margin1 = start_margin + 150;   //Purchase Date
            int margin2 = margin1 + 135;        //Amount
            int margin3 = margin2 + 145;        //Tracking#
            int margin4 = margin3 + 190;         //Exp. Date
            int margin5 = margin4 + 130;         //Status

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

            List<Shipment_Tracking> Ref_List = new List<Shipment_Tracking>();
            // Get ref list based on sort order

            // Sort by incomplete deliveries first
            Ref_List = parent.Tracking_List.Where(x => x.Ref_Order_Number != "999999999").ToList();

            // Order by status and then by date
            Ref_List = Ref_List.OrderByDescending(x => x.Status).ThenByDescending(x => x.Expected_Date).ToList();

            // Enable button
            next_page_button.Visible = Pages_Required > 1;

            Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
            Font f = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
            Font f_reg = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);

            // If has order
            if (paint)
            {

                // Draw gray header line
                //e.Graphics.DrawLine(Grey_Pen, start_margin, start_height - 10, margin6 + 140, start_height - 10);

                // Header
                e.Graphics.DrawString("Location", f_header, WritingBrush, start_margin, start_height + (row_count * data_height));
                e.Graphics.DrawString("Purchase Date", f_header, WritingBrush, margin1, start_height + (row_count * data_height));
                e.Graphics.DrawString("Amount", f_header, WritingBrush, margin2, start_height + (row_count * data_height));
                e.Graphics.DrawString("Tracking #", f_header, WritingBrush, margin3, start_height + (row_count * data_height));
                e.Graphics.DrawString("Exp. Date", f_header, WritingBrush, margin4, start_height + (row_count * data_height));
                e.Graphics.DrawString("Status", f_header, WritingBrush, margin5, start_height + (row_count * data_height));
                row_count += 1;


                // Remove existing buttons
                Search_Order_Button.ForEach(button => this.Controls.Remove(button));
                Search_Order_Button.ForEach(button => button.Image.Dispose());
                Search_Order_Button = new List<Button>();

                // For each refund item
                foreach (Shipment_Tracking ST in Ref_List.GetRange(Current_Page * Entries_Per_Page, (Ref_List.Count - Entries_Per_Page * Current_Page) >= Entries_Per_Page ? Entries_Per_Page : (Ref_List.Count % Entries_Per_Page)))
                {
                    f = new Font("MS Reference Sans Serif", 9, ST.Status == 0 ? FontStyle.Strikeout : (DateTime.Now > ST.Expected_Date ? FontStyle.Bold : FontStyle.Regular));
                    WritingBrush = new SolidBrush(ST.Status == 0 ? Color.LightGreen : (DateTime.Now > ST.Expected_Date ? Color.LightPink : DrawForeColor));

                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button refund_button = new Button();
                    refund_button.BackColor = this.BackColor;
                    refund_button.ForeColor = this.BackColor;
                    refund_button.FlatStyle = FlatStyle.Flat;
                    refund_button.Image = global::Financial_Journal.Properties.Resources.magnifier;
                    refund_button.Size = new Size(23, 23);
                    refund_button.Location = new Point(this.Width - 30, start_height + height_offset + (row_count * data_height) - 4);
                    refund_button.Name = ST.Ref_Order_Number;
                    refund_button.Text = "";
                    refund_button.Click += new EventHandler(this.view_order_Click);
                    Search_Order_Button.Add(refund_button);
                    ToolTip1.SetToolTip(refund_button, "View Receipt");
                    this.Controls.Add(refund_button);

                    Order Ref_Order = parent.Order_List.FirstOrDefault(x => x.OrderID == ST.Ref_Order_Number);

                    e.Graphics.DrawString(Ref_Order.Location, f, WritingBrush, start_margin - 5, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(Ref_Order.Date.ToShortDateString(), f, WritingBrush, margin1 + 12, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", Ref_Order.Order_Total_Pre_Tax + Ref_Order.Order_Taxes), f, WritingBrush, margin2, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(ST.Tracking_Number, f, WritingBrush, margin3 - 40, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(ST.Expected_Date.ToShortDateString(), f, WritingBrush, margin4 - 5, start_height + height_offset + (row_count * data_height));
                    int Days_Due = (int)Math.Abs((ST.Expected_Date.Date - DateTime.Now.Date).TotalDays);
                    e.Graphics.DrawString(ST.Status == 0 ? "Completed" : (Days_Due == 0 ? "Arriving today" : ((DateTime.Now > ST.Expected_Date ? "Overdue by " : "Due in ") + Days_Due + " day(s)")), f_reg, ST.Status == 0 ? WritingBrush : RedBrush, margin5 - 8 - (ST.Status == 1 ? 15 : 0) + (Days_Due == 0 ? 7 : 0), start_height + height_offset + (row_count * data_height));
                    
                    row_count++;
                }

                this.Height = start_height + height_offset + row_count * data_height + 10 + (Pages_Required > 1 ? 30 : 0);
                
            }
            else
            {
                this.Height = Start_Size.Height;
                paint = false;
            }


            TFLP.Size = new Size(this.Size.Width - 2, this.Size.Height - 2);

            p.Dispose();
            Grey_Pen.Dispose();
            GreyBrush.Dispose();
            RedBrush.Dispose();
            GreenBrush.Dispose();
            WritingBrush.Dispose();
            f_asterisk.Dispose();
            f_reg.Dispose();
            f_total.Dispose();
            f_header.Dispose();
            f.Dispose();
        }

        FadeControl TFLP;


        private void view_order_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            Grey_Out();
            Tracking T = new Tracking(parent, true, b.Name, this.Location, this.Size);
            T.ShowDialog();
            Invalidate();
            Grey_In();
        }

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public Online_Orders(Receipt _parent)
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
            paint = true;


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


            Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)parent.Tracking_List.Count() / (decimal)Entries_Per_Page));
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

        private void Grey_Out()
        {
            TFLP.Location = new Point(1, 1);
        }

        private void Grey_In()
        {
            TFLP.Location = new Point(1000, 1000);
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

        private void next_page_button_Click(object sender, EventArgs e)
        {
            if (Current_Page + 1 < Pages_Required)
            {
                Current_Page++;
                back_page_button.Visible = true;
                this.Invalidate();
                this.Update();
                paint = true;
                if (Pages_Required == Current_Page + 1) next_page_button.Visible = false;
            }
        }

        private void back_page_button_Click(object sender, EventArgs e)
        {
            if (Current_Page >= 1)
            {
                Current_Page--;
                next_page_button.Visible = true;
                this.Invalidate();
                this.Update();
                paint = true;
                if (0 == Current_Page) back_page_button.Visible = false;
            }
        }
    }
}
