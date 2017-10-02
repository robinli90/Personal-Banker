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
    public partial class Refund_Report : Form
    {

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            int data_height = 23;
            int start_height = 77;
            int start_margin = 15;
            int height_offset = 9;

            int margin1 = start_margin + 240;   //Location
            int margin2 = margin1 + 150;        //Date
            int margin3 = margin2 + 140;        //Quantity
            int margin4 = margin3 + 80;         //Unit Price
            int margin5 = margin4 + 90;         //Total Amount
            int margin6 = margin5 + 92;         //Payment

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


            // If has order
            if (paint)
            {
                Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
                Font f = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
                Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
                Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
                Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);

                // Draw gray header line
                e.Graphics.DrawLine(Grey_Pen, start_margin, start_height - 10, margin6 + 140, start_height - 10);

                // Header
                e.Graphics.DrawString("Item", f_header, WritingBrush, start_margin, start_height + (row_count * data_height));
                e.Graphics.DrawString("Location", f_header, WritingBrush, margin1, start_height + (row_count * data_height));
                e.Graphics.DrawString("Refund Date", f_header, WritingBrush, margin2, start_height + (row_count * data_height));
                e.Graphics.DrawString("Quantity", f_header, WritingBrush, margin3, start_height + (row_count * data_height));
                e.Graphics.DrawString("Unit Price", f_header, WritingBrush, margin4, start_height + (row_count * data_height));
                e.Graphics.DrawString("Credit Amt", f_header, WritingBrush, margin5, start_height + (row_count * data_height));
                e.Graphics.DrawString("Payment Type", f_header, WritingBrush, margin6 + 37, start_height + (row_count * data_height));
                row_count += 1;

                int item_index = 0;
                bool has_exempt = false;
                bool has_overwritten = false;
                double Ongoing_Total = 0;

                // Remove existing buttons
                Search_Order_Button.ForEach(button => this.Controls.Remove(button));
                Search_Order_Button = new List<Button>();

                // For each refund item
                foreach (Item item in Current_Refund_Item_List)
                {
                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button refund_button = new Button();
                    refund_button.BackColor = this.BackColor;
                    refund_button.ForeColor = this.BackColor;
                    refund_button.FlatStyle = FlatStyle.Flat;
                    refund_button.Image = global::Financial_Journal.Properties.Resources.magnifier;
                    refund_button.Size = new Size(23, 23);
                    refund_button.Location = new Point(margin6 + 158, start_height + height_offset + (row_count * data_height) - 4);
                    refund_button.Name = "b" + item_index.ToString();
                    refund_button.Text = "";
                    refund_button.Click += new EventHandler(this.view_order_Click);
                    Search_Order_Button.Add(refund_button);
                    ToolTip1.SetToolTip(refund_button, "View Receipt");
                    this.Controls.Add(refund_button);
                    
                    // Find corresponding order based on orderID
                    Order Ref_Order = parent.Order_List.First(x => x.OrderID == item.OrderID);

                    // Flag unit price where tax is exempt
                    string temp = parent.Tax_Rules_Dictionary.ContainsKey(item.Category) ? ((parent.Tax_Rules_Dictionary[item.Category] == "0") ? "*" : "") : "";
                    if (temp.Length > 0) has_exempt = true;
                    if (Ref_Order.Tax_Overridden) has_overwritten = true;


                    double Total_Price = ((Convert.ToDouble(item.Status) * (item.Price) *
                        (Ref_Order.Tax_Overridden ? 1 : (1 + (parent.Tax_Rules_Dictionary.ContainsKey(item.Category) ? Convert.ToDouble(parent.Tax_Rules_Dictionary[item.Category]) : parent.Tax_Rate)))) - Convert.ToDouble(item.Status) * (item.Discount_Amt / item.Quantity)); // tax rate

                    e.Graphics.DrawString(item.Name, f, WritingBrush, start_margin + 6, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(item.Location, f, WritingBrush, margin1, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(item.Refund_Date.ToShortDateString(), f, WritingBrush, margin2 + 10, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(item.Status, f, WritingBrush, margin3 + 24, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", item.Price) + temp, f, WritingBrush, margin4 + 12, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(item.Payment_Type, f, WritingBrush, margin6 + 15, start_height + height_offset + (row_count * data_height));
                    // Show different total price for unit discount
                    if (item.Discount_Amt == 0)
                    {
                        e.Graphics.DrawString("$" + String.Format("{0:0.00}", Total_Price) + (Ref_Order.Tax_Overridden ? "**" : ""), f, WritingBrush, margin5 + 12, start_height + height_offset + (row_count * data_height));
                    }
                    else // if has discount
                    {
                        
                        e.Graphics.DrawString("$" + String.Format("{0:0.00}", Total_Price + Convert.ToDouble(item.Status) * (item.Discount_Amt / item.Quantity)) + (Ref_Order.Tax_Overridden ? "**" : ""), f_strike, WritingBrush, margin5 + 12, start_height + height_offset + (row_count * data_height));
                        row_count++;
                        e.Graphics.DrawString("-(Discount)", f, WritingBrush, margin3-5, start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawString("-($" + String.Format("{0:0.00}", (item.Discount_Amt / item.Quantity)) + ")", f, WritingBrush, margin4 + 7, start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawString("-($" + String.Format("{0:0.00}", Convert.ToDouble(item.Status) * (item.Discount_Amt / item.Quantity)) + ")", f, WritingBrush, margin5 + 7, start_height + height_offset + (row_count * data_height));
                        row_count++;
                        e.Graphics.DrawLine(p, margin5, start_height + height_offset + (row_count * data_height), margin6, start_height + height_offset + (row_count * data_height));
                        //e.Graphics.DrawLine(p, margin4, start_height + height_offset + (row_count * data_height), margin5 - 5, start_height + height_offset + (row_count * data_height));
                        height_offset += 1;
                        e.Graphics.DrawString("$" + String.Format("{0:0.00}", Total_Price), f, WritingBrush, margin5 + 12, start_height + height_offset + (row_count * data_height));

                        // draw double underline
                        height_offset += 17;
                        e.Graphics.DrawLine(p, margin5, start_height + height_offset + (row_count * data_height), margin6, start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawLine(p, margin5, start_height + height_offset + 2 + (row_count * data_height), margin6, start_height + height_offset + 2 + (row_count * data_height));
                        height_offset -= 15;

                    }
                    Ongoing_Total += Total_Price;
                    row_count++;


                    item_index++;
                }

                // Total line
                e.Graphics.DrawLine(p, margin4, start_height + height_offset + (row_count * data_height), margin6, start_height + height_offset + (row_count * data_height));
                height_offset += 4;
                e.Graphics.DrawString("Total Credit", f_total, WritingBrush, margin4, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Ongoing_Total), f_total, GreenBrush, margin5 + 12, start_height + height_offset + (row_count * data_height));

                // Draw accounting double lines
                height_offset += 19;
                e.Graphics.DrawLine(p, margin5, start_height + height_offset + (row_count * data_height), margin6, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawLine(p, margin5, start_height + height_offset + 2 + (row_count * data_height), margin6, start_height + height_offset + 2 + (row_count * data_height));
                height_offset -= 17;

                row_count++;
                if (has_exempt) e.Graphics.DrawString("*Tax Exempt" + (has_overwritten ? "           **Tax amount has been overrided" : ""), f_asterisk, RedBrush, margin4 + 4, start_height + height_offset + 4 + (row_count * data_height));
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
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        private void view_order_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            Order Ref_Order = parent.Order_List.First(x => x.OrderID == Current_Refund_Item_List[Convert.ToInt32(b.Name.Substring(1))].OrderID);
            Grey_Out();
            Receipt_Report RP = new Receipt_Report(parent, Ref_Order, new Point(this.Left + 300, this.Top + 40), null, false, this.Location, this.Size);
            RP.ShowDialog();
            Grey_In();
            /*
            parent.Master_Item_List[parent.Master_Item_List.IndexOf(Current_Order_Items[Convert.ToInt32(b.Name.Substring(1))])].Status = (Convert.ToInt32(parent.Master_Item_List[parent.Master_Item_List.IndexOf(Current_Order_Items[Convert.ToInt32(b.Name.Substring(1))])].Status) + 1).ToString();
            parent.Master_Item_List[parent.Master_Item_List.IndexOf(Current_Order_Items[Convert.ToInt32(b.Name.Substring(1))])].Refund_Date = DateTime.Now;

            foreach (Order order in parent.Order_List)
            {
                Item ref_Item = Current_Order_Items[Convert.ToInt32(b.Name.Substring(1))];
                if (order.OrderID == ref_Item.OrderID)
                {
                    order.Order_Total_Pre_Tax -= ref_Item.Price - (ref_Item.Discount_Amt / ref_Item.Quantity);
                    order.Order_Taxes -= ref_Item.Price * ((order.Tax_Overridden || order.Order_Taxes == 0) ? 0 : (parent.Tax_Rules_Dictionary.ContainsKey(ref_Item.Category) ? Convert.ToDouble(parent.Tax_Rules_Dictionary[ref_Item.Category]) : parent.Tax_Rate));
                }
            }

            order_list_box.Items.Clear();
            double Sum_Total = 0;
            List<Order> sortedList = Current_Order_List.OrderByDescending(x => x.Date).ToList();
            foreach (Order order in sortedList)
            {
                Sum_Total += order.Order_Total_Pre_Tax + order.Order_Taxes;
                order_list_box.Items.Add(order.Location + " (" + order.Date.ToString() + ") - $" + String.Format("{0:0.00}", order.Order_Total_Pre_Tax + order.Order_Taxes));
            }
            label11.Text = "$" + String.Format("{0:0.00}", Sum_Total);

            Load_Order_Information(Ref_Order);
            */

        }

        private List<Item> Current_Refund_Item_List = new List<Item>();
        private List<Button> Search_Order_Button = new List<Button>();

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;
        bool paint = true;

        public Refund_Report(Receipt _parent)
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
        }

        private void Get_Refunds_From_Current_Month()
        {
            Current_Refund_Item_List = new List<Item>();
            List<Item> unsortedList = parent.Master_Item_List.Where(p => p.Refund_Date.Month == CustomDTP1.Value.Month && p.Refund_Date.Year == CustomDTP1.Value.Year && (Convert.ToInt32(p.Status) > 0)).ToList();
            Current_Refund_Item_List = unsortedList.OrderByDescending(x => x.Refund_Date).ToList();
        }

        private void Get_Refunds_From_Current_Location(bool All_Refunds = false)
        {
            Current_Refund_Item_List = new List<Item>();
            List<Item> unsortedList = parent.Master_Item_List.Where(p => (All_Refunds || p.Location == location_box.Text) && (Convert.ToInt32(p.Status) > 0)).ToList();
            Current_Refund_Item_List = unsortedList.OrderByDescending(x => x.Refund_Date).ToList();
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

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            location_box.Items.Add("All");
            foreach (string location in parent.location_box.Items)
            {
                location_box.Items.Add(location);
            }

            Get_Refunds_From_Current_Month();
            CustomDTP1.Value = DateTime.Now;

            adjustWidth();

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

        private void CustomDTP1_ValueChanged(object sender, EventArgs e)
        {
            // Reset canvas
            paint = false;

            Get_Refunds_From_Current_Month();

            adjustWidth();

            paint = Current_Refund_Item_List.Count > 0;
            Invalidate();
            Update();
        }

        private void adjustWidth()
        {
            if (Current_Refund_Item_List.Count == 0)
            {
                this.Width = 505;
            }
            else
            {
                this.Width = Start_Size.Width;
            }
        }

        private void location_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            Get_Refunds_From_Current_Location(location_box.Text == "All");

            adjustWidth();

            // Reset canvas
            paint = false;

            paint = Current_Refund_Item_List.Count > 0;
            Invalidate();
            Update();
        }
    }
}
