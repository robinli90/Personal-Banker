using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Objects;

namespace Financial_Journal
{
    public partial class Receipt_Report : Form
    {
        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            #region Set status spending info
            parent.StatusSetSpending();
            #endregion

            if (activate_report) parent.Activate();
            base.OnFormClosing(e);
        }


        /*
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
                    return;
                }
            }
            base.WndProc(ref m);
        }
        */

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 4;
        Order Ref_Order = new Order();
        List<Order> Current_Order_List = new List<Order>();
        List<Item> Current_Order_Items = new List<Item>();
        List<Button> Refund_Buttons = new List<Button>();
        bool predefined_Order_View = false;
        bool activate_report = false;
        Purchases r = new Purchases(new Receipt());

        public Receipt_Report(Receipt _parent, Order order = null, object spawn_point = null, Purchases _parent_report = null, bool editdel = true, Point g = new Point(), Size s = new Size(), bool Allow_Drag = false)
        {
            //this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset);
            InitializeComponent();
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);

            activate_report = _parent_report != null;
            button1.Visible = editdel;
            delete_order_button.Visible = editdel;

            SetStyle(ControlStyles.ResizeRedraw, true);

            if (order != null)
            {
                Ref_Order = order;
                predefined_Order_View = true;
                r = _parent_report;
            }

            if (spawn_point != null)
            {
                this.Location = (Point)spawn_point;
            }
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 4) - (this.Height / 2));

            Allow_Drag_ = Allow_Drag;
        }

        bool Allow_Drag_ = false;

        private void Receipt_Load(object sender, EventArgs e)
        {

            // Increase performance by buffering twice
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            // Mousedown anywhere to drag
            if (Allow_Drag_)
            {
                this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            }

            // Enter key search
            this.item_desc.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxEnterKey_KeyPress);

            // Full report view
            if (!predefined_Order_View)
            {
                CustomDTP1.Value = DateTime.Now;
                ToolTip ToolTip1 = new ToolTip();
                ToolTip1.InitialDelay = 1;
                ToolTip1.ReshowDelay = 1;
                ToolTip1.SetToolTip(search_desc_button, "Search");
                ToolTip1.SetToolTip(delete_order_button, "Delete this receipt");
                ToolTip1.SetToolTip(button1, "Edit this receipt");
                order_list_box.Items.Clear();
                /*
                this.SetStyle(
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.DoubleBuffer,
                    true);*/

                // Preset orders
                Get_Orders_From_Current_Month();

                foreach (string location in parent.location_box.Items)
                {
                    location_box.Items.Add(location);
                }

                double Sum_Total = 0;
                List<Order> sortedList = Current_Order_List.OrderByDescending(x => x.Date).ToList();
                foreach (Order order in sortedList)
                {
                    Sum_Total += order.Order_Total_Pre_Tax + order.Order_Taxes;
                    order_list_box.Items.Add(order.Location + " (" + order.Date.ToShortDateString() + ") - $" + String.Format("{0:0.00}", order.Order_Total_Pre_Tax + order.Order_Taxes));
                }
                label11.Text = "$" + String.Format("{0:0.00}", Sum_Total);

                if (order_list_box.Items.Count > 0) order_list_box.Text = order_list_box.Items[0].ToString();

                paint = true;
                Invalidate();
            }
            else
            {
                Order order = Ref_Order;
                Current_Order_Items = parent.Master_Item_List.Where(x => x.OrderID == order.OrderID).ToList().OrderBy(x => x.Date).ToList();

                // Show All
                label1.Visible = true;
                label2.Visible = true;
                label3.Visible = true;
                label9.Visible = order.OrderMemo.Length > 0;

                // Hide existing controls
                panel1.Visible = false;
                panel2.Visible = false;
                //delete_order_button.Visible = false;
                //button1.Visible = false;

                // Shift Labels up
                int Shift_Value = 100;
                delete_order_button.Top -= Shift_Value;
                button1.Top -= Shift_Value;
                label1.Top -= Shift_Value;
                label2.Top -= Shift_Value;
                label3.Top -= Shift_Value;
                label9.Top -= Shift_Value;
                location_label.Top -= Shift_Value;
                date_label.Top -= Shift_Value;
                payment_label.Top -= Shift_Value;
                memo_label.Top -= Shift_Value;

                location_label.Text = order.Location;
                date_label.Text = order.Date.ToString();
                payment_label.Text = order.Payment_Type;
                memo_label.Text = order.OrderMemo;
                paint = true;
                this.Invalidate();
                Update();

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

        FadeControl TFLP;

        private void Grey_Out()
        {
            TFLP.Location = new Point(1, 1);
        }

        private void Grey_In()
        {
            TFLP.Location = new Point(1000, 1000);
        }

        private void Load_Order_Information(Order order)
        {
            Ref_Order = order;
            Current_Order_Items = parent.Master_Item_List.Where(x => x.OrderID == order.OrderID).ToList().OrderBy(x => x.Date).ToList();

            // Show All
            label1.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            label9.Visible = order.OrderMemo.Length > 0;
            location_label.Text = order.Location;
            date_label.Text = order.Date.ToString();


            payment_label.Text = order.Payment_Type;
            memo_label.Text = order.OrderMemo;
            paint = (order_list_box.Items.Count > 0);
            this.Invalidate();
            Update();


        }

        // If press enter on length box, activate add (nmemonics)
        private void textboxEnterKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox g = (TextBox)sender;
            if (e.KeyChar == (char)Keys.Enter && g.Text.Length > 0)
            {
                search_desc_button.PerformClick();
            }
        }

        public bool paint = true;

        protected override void OnPaint(PaintEventArgs e)
        {
            int data_height = 20;
            int start_height = label9.Top + 30 - (Ref_Order.OrderMemo != null && Ref_Order.OrderMemo.Length == 0 ? 15 : 0);
            int start_margin = 15;
            int height_offset = 9;
            int columnmargin1 = start_margin + 0;
            int columnmargin2 = start_margin + 270;
            int columnmargin3 = columnmargin2 + 70;
            int row_count = 0;

            Color DrawForeColor = Color.White;
            Color BackColor = Color.FromArgb(64, 64, 64);
            Color HighlightColor = Color.FromArgb(76, 76, 76);

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(88, 88, 88));
            SolidBrush ErrorBrush = new SolidBrush(Color.LightPink);
            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 2);


            Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
            Font f = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);

            if (Ref_Order.OrderMemo != null && Ref_Order.OrderMemo.Length > 0) start_height += 10;

            // If has order
            if (paint && Current_Order_Items.Count > 0)
            {

                string GC_Str = "Gift card(s) used: ";
                // has GC
                if (Ref_Order.GC_Amount > 0)
                {
                    foreach (GC GCard in parent.GC_List)
                    {
                        if (GCard.Associated_Orders.Contains(Ref_Order.OrderID))
                        {
                            GC_Str += GCard.Location + " ($" + String.Format("{0:0.00}", GCard.Get_Order_Amount(Ref_Order.OrderID)) + "), ";
                        }
                    }

                    e.Graphics.DrawString(GC_Str.Trim().Trim(','), f_asterisk, WritingBrush, columnmargin1, start_height + (row_count * data_height));
                    row_count++;
                }


                e.Graphics.DrawLine(Grey_Pen, start_margin, label1.Top - 9, Start_Size.Width - 17, label1.Top - 9);

                // Header
                e.Graphics.DrawString("Item", f_header, WritingBrush, columnmargin1, start_height + (row_count * data_height));
                e.Graphics.DrawString("Quantity", f_header, WritingBrush, columnmargin2 - 21, start_height + (row_count * data_height));
                e.Graphics.DrawString("Unit Price", f_header, WritingBrush, columnmargin3 - 4, start_height + (row_count * data_height));
                row_count += 1;

                bool has_exempt = false;

                int item_index = 0;

                Refund_Buttons.ForEach(button => button.Image.Dispose());
                Refund_Buttons.ForEach(button => button.Dispose());
                Refund_Buttons.ForEach(button => this.Controls.Remove(button));
                Refund_Buttons = new List<Button>();

                foreach (Item item in Current_Order_Items)
                {
                    if (Convert.ToInt32(item.Status) < item.Quantity && true)//!predefined_Order_View)
                    {
                        ToolTip ToolTip1 = new ToolTip();
                        ToolTip1.InitialDelay = 1;
                        ToolTip1.ReshowDelay = 1;

                        Button refund_button = new Button();
                        refund_button.BackColor = this.BackColor;
                        refund_button.ForeColor = this.BackColor;
                        refund_button.FlatStyle = FlatStyle.Flat;
                        refund_button.Image = global::Financial_Journal.Properties.Resources.Refund_Icon2;
                        refund_button.Size = new Size(23, 23);
                        refund_button.Location = new Point(start_margin - 3, start_height + height_offset + (row_count * data_height) - 3 + (item.Memo.Length > 0 ? 5 : 0));
                        refund_button.Name = "b" + item_index.ToString();
                        refund_button.Text = "";
                        refund_button.Click += new EventHandler(this.refund_button_Click);
                        Refund_Buttons.Add(refund_button);
                        ToolTip1.SetToolTip(refund_button, "Refund " + item.Name);
                        this.Controls.Add(refund_button);
                    }

                    // Flag tax exempt items from rules
                    string temp = parent.Tax_Rules_Dictionary.ContainsKey(item.Category) ? ((parent.Tax_Rules_Dictionary[item.Category] == "0") ? "*" : "") : "";
                    if (temp.Length > 0) has_exempt = true;

                    if (item.Memo.Length == 0)
                    {
                        e.Graphics.DrawString(item.Name + (item.Status != "0" ? " (Refunded " + item.Status + ")" : ""), f, (item.Status != "0" ? ErrorBrush : WritingBrush), columnmargin1 + 20, start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawString(item.Quantity.ToString(), f, WritingBrush, columnmargin2, start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawString("$" + String.Format("{0:0.00}", item.Price) + temp, f, WritingBrush, columnmargin3, start_height + height_offset + (row_count * data_height));
                        row_count++;
                        if (item.Discount_Amt > 0)
                        {
                            e.Graphics.DrawString(" Discount               (-$" + String.Format("{0:0.00}", (item.Discount_Amt / item.Quantity) * (item.Quantity - Convert.ToInt32(item.Status))) + ")", f, WritingBrush, columnmargin3 - 100, start_height + height_offset + (row_count * data_height));
                            row_count++;
                        }
                    }
                    else
                    {
                        e.Graphics.DrawString(item.Name + (item.Status != "0" ? " (Refunded " + item.Status + ")" : ""), f, (item.Status != "0" ? ErrorBrush : WritingBrush), columnmargin1 + 20, start_height + height_offset + (row_count * data_height));

                        // Center with  memo
                        height_offset += 5; // Offset
                        e.Graphics.DrawString(item.Quantity.ToString(), f, WritingBrush, columnmargin2, start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawString("$" + String.Format("{0:0.00}", item.Price) + temp, f, WritingBrush, columnmargin3, start_height + height_offset + (row_count * data_height));
                        if (item.Discount_Amt > 0)
                        {
                            e.Graphics.DrawString(" Discount          (-$" + String.Format("{0:0.00}", (item.Discount_Amt / item.Quantity) * (item.Quantity - Convert.ToInt32(item.Status))) + ")", f, WritingBrush, columnmargin3 - 100, start_height + height_offset + (row_count * data_height) + data_height);
                        }
                        height_offset -= 5; // Reverse  offset


                        row_count++; height_offset -= 4;
                        e.Graphics.DrawString("(" + item.Memo + ")", f, (item.Status != "0" ? ErrorBrush : WritingBrush), columnmargin1 + 20, start_height + height_offset + (row_count * data_height));
                        row_count++; height_offset += 4;
                    }

                    item_index++;
                }

                // Draw Line
                height_offset += 5;
                e.Graphics.DrawLine(p, columnmargin2 - 25, start_height + height_offset + (row_count * data_height), columnmargin3 + 60, start_height + height_offset + (row_count * data_height));
                height_offset += 5;

                e.Graphics.DrawString("Sub Total", f, WritingBrush, columnmargin2 - 25, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Ref_Order.Order_Total_Pre_Tax), f, WritingBrush, columnmargin3, start_height + height_offset + (row_count * data_height));
                row_count++;
                e.Graphics.DrawString("Taxes", f, WritingBrush, columnmargin2 - 3, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Ref_Order.Order_Taxes), f, WritingBrush, columnmargin3, start_height + height_offset + (row_count * data_height));
                row_count++;

                // Draw Total Line
                e.Graphics.DrawLine(p, columnmargin2 - 25, start_height + height_offset + (row_count * data_height), columnmargin3 + 60, start_height + height_offset + (row_count * data_height));
                height_offset += 5;

                e.Graphics.DrawString("Total", f_total, WritingBrush, columnmargin2 - 3, start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Ref_Order.Order_Taxes + Ref_Order.Order_Total_Pre_Tax), f_total, WritingBrush, columnmargin3, start_height + height_offset + (row_count * data_height));

                row_count++;
                e.Graphics.DrawLine(p, columnmargin3 - 5, start_height + height_offset + (row_count * data_height), columnmargin3 + 60, start_height + height_offset + (row_count * data_height));
                height_offset += 2;
                e.Graphics.DrawLine(p, columnmargin3 - 5, start_height + height_offset + (row_count * data_height), columnmargin3 + 60, start_height + height_offset + (row_count * data_height));

                if (Ref_Order.Tax_Overridden) e.Graphics.DrawString("Tax on order has been overrided", f_asterisk, ErrorBrush, columnmargin2, start_height + height_offset + 4 + (row_count * data_height));
                if (has_exempt) e.Graphics.DrawString("*Tax Exempt", f_asterisk, WritingBrush, start_margin - 12, start_height + height_offset + 4 + (row_count * data_height));
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
            GreyBrush.Dispose();
            WritingBrush.Dispose();
            f_asterisk.Dispose();
            f.Dispose();
            f_total.Dispose();
            f_header.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
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

        private void refund_button_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form2 = new Yes_No_Dialog(parent, "Are you sure you wish to refund this item? (You cannot reverse this action)", "Warning", "No", "Yes", 10, this.Location, this.Size))
            {
                var result = form2.ShowDialog();
                if (result == DialogResult.OK && form2.ReturnValue1 == "1")
                {
                    Button b = (Button)sender;

                    parent.Master_Item_List[parent.Master_Item_List.IndexOf(Current_Order_Items[Convert.ToInt32(b.Name.Substring(1))])].Status = (Convert.ToInt32(parent.Master_Item_List[parent.Master_Item_List.IndexOf(Current_Order_Items[Convert.ToInt32(b.Name.Substring(1))])].Status) + 1).ToString();
                    parent.Master_Item_List[parent.Master_Item_List.IndexOf(Current_Order_Items[Convert.ToInt32(b.Name.Substring(1))])].Refund_Date = DateTime.Now;

                    // Remove hobby
                    Item Ref_Item = parent.Master_Item_List[parent.Master_Item_List.IndexOf(Current_Order_Items[Convert.ToInt32(b.Name.Substring(1))])];
                    for (int i = parent.Master_Hobby_Item_List.Count - 1; i >= 0; i--)
                    {
                        if (parent.Master_Hobby_Item_List[i].OrderID == Ref_Item.OrderID && Ref_Item.Name == parent.Master_Hobby_Item_List[i].Name)
                        {
                            parent.Master_Hobby_Item_List.RemoveAt(i);
                            i -= parent.Master_Hobby_Item_List.Count; // End loop, just remove one
                        }
                    }

                    // Remove asset items
                    for (int i = parent.Asset_List.Count - 1; i >= 0; i--)
                    {
                        if (parent.Asset_List[i].OrderID == Ref_Item.OrderID && Ref_Item.Name == parent.Asset_List[i].Name)
                            parent.Asset_List.RemoveAt(i);
                    }

                    Item ref_Item = Current_Order_Items[Convert.ToInt32(b.Name.Substring(1))];
                    Order order = parent.Order_List.First(x => x.OrderID == ref_Item.OrderID);
                    double itemPriceLessTax = ref_Item.Price - (ref_Item.Discount_Amt / ref_Item.Quantity);
                    double itemPriceTax = ref_Item.Price * ((order.Tax_Overridden || order.Order_Taxes == 0) ? 0 : (parent.Tax_Rules_Dictionary.ContainsKey(ref_Item.Category) ? Convert.ToDouble(parent.Tax_Rules_Dictionary[ref_Item.Category]) : parent.Tax_Rate));
                    
                    double itemTotal = itemPriceLessTax + itemPriceTax;
                    double orderTotal = order.Order_Taxes + order.Order_Total_Pre_Tax;

                    // Reverse any gift cards (remove from non-GC value first)
                    if (order.GC_Amount > 0)
                    {
                        if (itemTotal == order.GC_Amount && orderTotal == order.GC_Amount) // Remove entire sum from GC
                        {
                            //order.GC_Amount = 0;
                        }
                        else if (orderTotal - order.GC_Amount > 0) // Remove from payment first and then GC
                        {
                            if (itemTotal > orderTotal - order.GC_Amount) // If amount is greater than what cash can be removed; remove difference from GC
                            {
                                Grey_Out();
                                Form_Message_Box FMB = new Form_Message_Box(parent, "Gift cards removed. Please apply them back if neccessary", true, 0, this.Location, this.Size);
                                FMB.ShowDialog();
                                Grey_In();
                            }
                        }
                        // Reverse gift cards applied to this order
                        if (Ref_Order.GC_Amount > 0)
                        {
                            foreach (GC GCard in parent.GC_List)
                            {
                                GCard.Reverse_Transaction(Ref_Order.OrderID);
                            }
                        }
                        order.GC_Amount = 0;
                    }

                    order.Order_Total_Pre_Tax -= itemPriceLessTax;
                    order.Order_Taxes -= itemPriceTax;

                    // Add cash value if paid by cash
                    if (order.Payment_Type == "Cash")
                    {
                        Cash.UpdateCashHistoryByID("O" + order.OrderID, (itemPriceLessTax + itemPriceTax));
                    }

                    order_list_box.Items.Clear();
                    double Sum_Total = 0;
                    List<Order> sortedList = Current_Order_List.OrderByDescending(x => x.Date).ToList();
                    foreach (Order order2 in sortedList)
                    {
                        Sum_Total += order2.Order_Total_Pre_Tax + order2.Order_Taxes;
                        order_list_box.Items.Add(order2.Location + " (" + order2.Date.ToShortDateString() + ") - $" + String.Format("{0:0.00}", order2.Order_Total_Pre_Tax + order2.Order_Taxes));
                    }
                    label11.Text = "$" + String.Format("{0:0.00}", Sum_Total);

                    Load_Order_Information(Ref_Order);

                    // Remove from tracking
                    if (parent.Tracking_List.Where(x => x.Ref_Order_Number == Current_Order_Items[0].OrderID).ToList().Count > 0)
                    {
                        parent.Tracking_List.Remove(parent.Tracking_List.First(x => x.Ref_Order_Number == Current_Order_Items[0].OrderID));
                    }

                    if (Current_Order_Items.Count == 1 && Current_Order_Items[0].Quantity - Convert.ToInt32(Current_Order_Items[0].Status) == 0)
                    {
                        base.Close();
                    }

                    parent.Background_Save();

                }
            }
            Grey_In();

        }

        public void Set_Form_Color(Color randomColor)
        {
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        private void order_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (Order order in Current_Order_List)
            {
                if (order.Location + " (" + order.Date.ToShortDateString() + ") - $" + String.Format("{0:0.00}", order.Order_Total_Pre_Tax + order.Order_Taxes) == order_list_box.Text)
                {
                    Load_Order_Information(order);
                }
            }
        }

        private void Get_Orders_From_Current_Month()
        {
            Current_Order_List = new List<Order>();
            Current_Order_List = parent.Order_List.Where(p => p.Date.Month == CustomDTP1.Value.Month && p.Date.Year == CustomDTP1.Value.Year ).ToList();
        }

        private void Get_Orders_From_Current_Location()
        {
            Current_Order_List = new List<Order>();
            Current_Order_List = parent.Order_List.Where(p => p.Location == location_box.Text).ToList();
        }

        private void Get_Orders_From_Current_Search()
        {
            Current_Order_List = new List<Order>();
            foreach (Item item in parent.Master_Item_List)
            {
                try
                {
                    if (Regex.IsMatch(item.Name, item_desc.Text, RegexOptions.IgnoreCase))
                    {
                        List<Order> temp_Order = parent.Order_List.Where(x => x.OrderID == item.OrderID).ToList();
                        Current_Order_List.AddRange(temp_Order);
                    }
                }
                catch
                { }
            }
            Current_Order_List = Current_Order_List.Distinct().ToList();
            //Current_Order_List = parent.Order_List.Where(p => p.).ToList();
        }

        private void CustomDTP1_ValueChanged(object sender, EventArgs e)
        {
            Get_Orders_From_Current_Month();
            order_list_box.Items.Clear();

            // Hide all
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label9.Visible = false;
            location_label.Text = "";
            date_label.Text = "";
            payment_label.Text = "";
            memo_label.Text = "";
            paint = false;
            Invalidate();
            Update();

            double Sum_Total = 0;
            List<Order> sortedList = Current_Order_List.OrderByDescending(x => x.Date).ToList();
            foreach (Order order in sortedList)
            {
                Sum_Total += order.Order_Total_Pre_Tax + order.Order_Taxes;
                order_list_box.Items.Add(order.Location + " (" + order.Date.ToShortDateString() + ") - $" + String.Format("{0:0.00}", order.Order_Total_Pre_Tax + order.Order_Taxes));
            }
            label11.Text = "$" + String.Format("{0:0.00}", Sum_Total);

            if (order_list_box.Items.Count > 0)
            {
                order_list_box.Text = order_list_box.Items[0].ToString();
            }


        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            foreach (Item item in parent.Master_Item_List)
            {
                if (item == Current_Order_Items[0])
                {
                    Diagnostics.WriteLine(item.Name + ", " + item.OrderID);
                }
            }
        }

        private void location_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            Get_Orders_From_Current_Location();
            order_list_box.Items.Clear();

            // Hide all
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label9.Visible = false;
            location_label.Text = "";
            date_label.Text = "";
            payment_label.Text = "";
            memo_label.Text = "";
            paint = false;
            Invalidate();
            Update();

            double Sum_Total = 0;
            List<Order> sortedList = Current_Order_List.OrderByDescending(x => x.Date).ToList();
            foreach (Order order in sortedList)
            {
                Sum_Total += order.Order_Total_Pre_Tax + order.Order_Taxes;
                order_list_box.Items.Add(order.Location + " (" + order.Date.ToShortDateString() + ") - $" + String.Format("{0:0.00}", order.Order_Total_Pre_Tax + order.Order_Taxes));
            }
            label11.Text = "$" + String.Format("{0:0.00}", Sum_Total);

            if (order_list_box.Items.Count > 0)
            {
                order_list_box.Text = order_list_box.Items[0].ToString();
            }
        }

        private void item_desc_TextChanged(object sender, EventArgs e)
        {
        }

        private void memo_button_Click(object sender, EventArgs e)
        {

            Get_Orders_From_Current_Search();
            order_list_box.Items.Clear();

            // Hide all
            label1.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            label9.Visible = false;
            location_label.Text = "";
            date_label.Text = "";
            payment_label.Text = "";
            memo_label.Text = "";
            paint = false;
            Invalidate();
            Update();

            double Sum_Total = 0;
            List<Order> sortedList = Current_Order_List.OrderByDescending(x => x.Date).ToList();
            foreach (Order order in sortedList)
            {
                Sum_Total += order.Order_Total_Pre_Tax + order.Order_Taxes;
                order_list_box.Items.Add(order.Location + " (" + order.Date.ToShortDateString() + ") - $" + String.Format("{0:0.00}", order.Order_Total_Pre_Tax + order.Order_Taxes));
            }
            label11.Text = "$" + String.Format("{0:0.00}", Sum_Total);

            if (order_list_box.Items.Count > 0)
            {
                order_list_box.Text = order_list_box.Items[0].ToString();
            }
        }

        // Delete Order Button
        private void delete_order_button_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Yes_No_Dialog(parent, "Are you sure you wish to remove this order?", "Warning", "No", "Yes", 0, this.Location, this.Size))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (form.ReturnValue1 == "1")
                    {
                        // Reverse gift cards applied to this order
                        if (Ref_Order.GC_Amount > 0)
                        {
                            foreach (GC GCard in parent.GC_List)
                            {
                                GCard.Reverse_Transaction(Ref_Order.OrderID);
                            }
                        }

                        // Remove all cash relevant history
                        Cash.DeleteCashHistryByID("O" + Ref_Order.OrderID);

                        // Remove all asset items relevant to any items in order
                        parent.Asset_List = parent.Asset_List.Where(x => x.OrderID != Ref_Order.OrderID).ToList();

                        // Remove all hobby items relevant to any items in order
                        parent.Master_Hobby_Item_List = parent.Master_Hobby_Item_List.Where(x => x.OrderID != Ref_Order.OrderID).ToList();

                        parent.Master_Item_List = parent.Master_Item_List.Where(x => x.OrderID != Ref_Order.OrderID).ToList();
                        Current_Order_List.Remove(Ref_Order);
                        parent.Order_List.Remove(Ref_Order);

                        // Populate order_list_box
                        order_list_box.Items.Clear();

                        double Sum_Total = 0;
                        List<Order> sortedList = Current_Order_List.OrderByDescending(x => x.Date).ToList();
                        foreach (Order order in sortedList)
                        {
                            Sum_Total += order.Order_Total_Pre_Tax + order.Order_Taxes;
                            order_list_box.Items.Add(order.Location + " (" + order.Date.ToShortDateString() + ") - $" + String.Format("{0:0.00}", order.Order_Total_Pre_Tax + order.Order_Taxes));
                        }
                        label11.Text = "$" + String.Format("{0:0.00}", Sum_Total);

                        if (order_list_box.Items.Count > 0)
                        {
                            order_list_box.Text = order_list_box.Items[0].ToString();
                        }

                        try
                        {
                            r.update = true;
                        }
                        catch
                        {
                        }

                        // Remove associated tracking information with order
                        parent.Tracking_List = parent.Tracking_List.Where(x => x.Ref_Order_Number != Ref_Order.OrderID).ToList();
                    }
                    else
                    {
                    }
                }
                if (predefined_Order_View)
                {
                    this.close_button.PerformClick();
                }
            }
            Grey_In();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        // Edit receipt
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (r != null) r.close_this_form = true;
                this.Close();
                this.Dispose();
                parent.Edit_Receipt(Ref_Order);
            }
            catch
            {
            }
        }
    }

    public class CustomDTP : DateTimePicker
    {
        public Color BackColor_DTP { get; set; }

        /*
        const int WM_ERASEBKGND = 0x14;
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == WM_ERASEBKGND)
            {
                //Graphics g = Graphics.FromHdc(m.WParam);
                //g.FillRectangle(new SolidBrush(BackColor_DTP), ClientRectangle);
                //g.Dispose();
                //return;
            }

            base.WndProc(ref m);
        }*/

        public CustomDTP()
        {
            BackColor_DTP = Color.FromArgb(64, 64, 64);
            this.Format = DateTimePickerFormat.Custom;
            this.CustomFormat = "MM-yyyy";
            this.Text = DateTime.Now.ToString("MM-yyyy");
        }
    }
}
