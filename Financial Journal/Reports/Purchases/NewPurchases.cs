using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using Objects;
using Excel = Microsoft.Office.Interop.Excel;

namespace Financial_Journal
{
    public partial class NewPurchases : Form
    {
        #region Forcing Windows to suspend and resume drawing the flow layout panels

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            parent.Refresh();
        }

        /// <summary>
        /// Suspends painting for the target control. Do NOT forget to call EndControlUpdate!!!
        /// </summary>
        /// <param name="control">visual control</param>
        public static void BeginControlUpdate(Control control)
        {
            Message msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        /// <summary>
        /// Resumes painting for the target control. Intended to be called following a call to BeginControlUpdate()
        /// </summary>
        /// <param name="control">visual control</param>
        public static void EndControlUpdate(Control control)
        {
            // Create a C "true" boolean as an IntPtr
            IntPtr wparam = new IntPtr(1);
            Message msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
                IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);
            control.Invalidate();
            control.Refresh();
        }

        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.MobileSync.currentlyChecking = false; // toggle back so it can check again
            //parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;

        public List<Order> currentOrderList;

        public Dictionary<string, string> filterSettings = new Dictionary<string, string>();
        public string sortSetting = "";

        private Order refOrder;

        private int panel1StartHeight = 0;

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            purchaseTotalText.Text = "Purchase total: $" + String.Format("{0:0.00}", 0);

            if (refOrder != null)
            {
                e.Graphics.Clear(BackColor);

                int start_margin = bufferedPanel1.Left + 5; // Item
                int start_height = bufferedPanel1.Bottom + 14;
                int price_margin = priceLabel.Left + 7;

                Color DrawForeColor = Color.White;
                SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
                SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(88, 88, 88));
                Pen Grey_Pen = new Pen(GreyBrush, 2);
                Pen p = new Pen(WritingBrush, 1);
                Font f = new Font("MS Reference Sans Serif", 8F, FontStyle.Regular);
                Font f_italic = new Font("MS Reference Sans Serif", 8F, FontStyle.Italic);

                locationText.Text = refOrder.Location;
                paymentText.Text = refOrder.Payment_Type;
                dateText.Text = refOrder.Date.ToString();

                int data_height = 19;
                int row_count = 0;

                //bufferedPanel1.Height = refOrder.GC_Amount > 0
                //    ? panel1StartHeight - data_height
                //    : panel1StartHeight;

                // Draw gray header line
                e.Graphics.DrawLine(Grey_Pen, 8, dateText.Bottom + 10, bufferedPanel2.Width - 8, dateText.Bottom + 10);

                // Draw white footer line
                e.Graphics.DrawLine(p, 8, start_height - 5 + (row_count * data_height), bufferedPanel2.Width - 8,
                    start_height - 5 + (row_count * data_height));
                e.Graphics.DrawString("Subtotal", f, WritingBrush, price_margin - 80,
                    start_height + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", refOrder.Order_Total_Pre_Tax), f, WritingBrush,
                    price_margin, start_height + (row_count * data_height));
                e.Graphics.DrawString(parent.Master_Item_List.Where(x => x.OrderID == refOrder.OrderID).Sum(y => y.Get_Current_Quantity()).ToString(), f, WritingBrush, quantityLabel.Left + 22,
                    start_height + (row_count * data_height));

                row_count++;
                e.Graphics.DrawString("Taxes", f, WritingBrush, price_margin - 65,
                    start_height + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", refOrder.Order_Taxes), f, WritingBrush,
                    price_margin,
                    start_height + (row_count * data_height));

                string gcString = "";
                if (refOrder.GC_Amount > 0)
                {
                    gcString = "($" + String.Format("{0:0.00}", refOrder.GC_Amount) +
                               ") from Gift Card";
                }

                row_count++;
                e.Graphics.DrawLine(p, 8, start_height + +(row_count * data_height), bufferedPanel2.Width - 8, start_height + (row_count * data_height));
                e.Graphics.DrawString("Total", f, WritingBrush, price_margin - 65, start_height + 4 + (row_count * data_height));
                e.Graphics.DrawString(
                    "$" + String.Format("{0:0.00}", refOrder.Order_Total_Pre_Tax + refOrder.Order_Taxes),
                    f, WritingBrush, price_margin, start_height + 4 + (row_count * data_height));
                e.Graphics.DrawString(
                    gcString,
                    f_italic, WritingBrush, price_margin + 120, start_height + 4 + (row_count * data_height));


                double totalOrderValue = currentOrderList.Sum(x => x.Order_Total_Pre_Tax + x.Order_Taxes);
                purchaseTotalText.Text = "Purchase total: $" + String.Format("{0:0.00}", totalOrderValue);

                // Dispose all objects
                p.Dispose();
                Grey_Pen.Dispose();
                GreyBrush.Dispose();
                WritingBrush.Dispose();
                f.Dispose();
                f_italic.Dispose();
                base.OnPaint(e);
            }
        }

        List<Button> Refund_Buttons = new List<Button>();
        private bool repaintButtons = true;

        // Panel paint
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (repaintButtons)
            {
                Refund_Buttons.ForEach(button => button.Image.Dispose());
                Refund_Buttons.ForEach(button => button.Dispose());
                Refund_Buttons.ForEach(button => bufferedPanel3.Controls.Remove(button));
                Refund_Buttons = new List<Button>();
            }

            e.Graphics.Clear(BackColor);

            // allow scroll transformation
            e.Graphics.TranslateTransform(bufferedPanel3.AutoScrollPosition.X, bufferedPanel3.AutoScrollPosition.Y);

            int data_height = 20;
            int row_count = 0;
            int height_offset = 1;
            int start_height = 0;
            int start_margin = itemLabel.Left; // Item
            int price_margin = priceLabel.Left + 7;
            int qty_margin = quantityLabel.Left + 18;
            int category_margin = categoryLabel.Left - 4; // - 20;

            Color DrawForeColor = Color.White;

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);

            Pen p = new Pen(WritingBrush, 1);
            Font f = new Font("MS Reference Sans Serif", 8F, FontStyle.Regular);
            Font f_italic = new Font("MS Reference Sans Serif", 8F, FontStyle.Italic);

            if (refOrder != null)
            {
                foreach (Item item in parent.Master_Item_List.Where(x => x.OrderID == refOrder.OrderID))
                {
                    if (Convert.ToInt32(item.Status) < item.Quantity) //!predefined_Order_View)
                    {
                        if (repaintButtons)
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
                            refund_button.Location = new Point(start_margin - 30,
                                start_height + height_offset + (row_count * data_height) - 3);
                            refund_button.Name = item.Name;
                            refund_button.Text = "";
                            refund_button.Click += refundButtonClick;
                            Refund_Buttons.Add(refund_button);
                            ToolTip1.SetToolTip(refund_button, "Refund " + item.Name);
                            bufferedPanel3.Controls.Add(refund_button);
                        }
                    }

                    e.Graphics.DrawString(item.Name, f, WritingBrush, start_margin,
                        start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString("$" + String.Format("{0:0.00}", item.Price), f, WritingBrush, price_margin,
                        start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(item.Get_Current_Quantity().ToString(), f, WritingBrush, qty_margin,
                        start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(item.Category, f, WritingBrush, category_margin,
                        start_height + height_offset + (row_count * data_height));
                    if (item.Discount_Amt > 0)
                    {
                        row_count++;
                        height_offset -= 5;
                        e.Graphics.DrawString("Discount", f_italic, WritingBrush, start_margin + 130,
                            start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawString("-($" + String.Format("{0:0.00}", item.Discount_Amt) + ")", f,
                            WritingBrush, price_margin - 3 - (item.Price >= 10 ? 7 : 0),
                            start_height + height_offset + (row_count * data_height));
                        height_offset += 2;
                    }
                    row_count++;

                }


                // Resize panel
                bufferedPanel1.AutoScrollMinSize = new Size(bufferedPanel3.Width,
                    start_height + height_offset + row_count * data_height);

                // Force resize only if too big
                if (start_height + height_offset + row_count * data_height > bufferedPanel1.Height)
                    bufferedPanel3.Height = new Size(bufferedPanel1.Width,
                        start_height + height_offset + row_count * data_height).Height;
            }

            // Dispose all objects
            p.Dispose();
            WritingBrush.Dispose();
            f.Dispose();
            f_italic.Dispose();

            repaintButtons = false;
        }

        private void refundButtonClick(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form2 = new Yes_No_Dialog(parent, "Are you sure you wish to refund this item? (You cannot reverse this action)", "Warning", "No", "Yes", 10, this.Location, this.Size))
            {
                var result = form2.ShowDialog();
                if (result == DialogResult.OK && form2.ReturnValue1 == "1")
                {
                    Button b = (Button)sender;

                    parent.Master_Item_List.First(x => x.OrderID == refOrder.OrderID && x.Name == b.Name).Status = (Convert.ToInt32(parent.Master_Item_List.First(x => x.OrderID == refOrder.OrderID && x.Name == b.Name).Status) + 1).ToString();
                    parent.Master_Item_List.First(x => x.OrderID == refOrder.OrderID && x.Name == b.Name).Refund_Date = DateTime.Now;

                    // Remove hobby
                    Item refItem = parent.Master_Item_List.First(x => x.OrderID == refOrder.OrderID && x.Name == b.Name);

                    for (int i = parent.Master_Hobby_Item_List.Count - 1; i >= 0; i--)
                    {
                        if (parent.Master_Hobby_Item_List[i].OrderID == refItem.OrderID && refItem.Name == parent.Master_Hobby_Item_List[i].Name)
                        {
                            parent.Master_Hobby_Item_List.RemoveAt(i);
                            i -= parent.Master_Hobby_Item_List.Count; // End loop, just remove one
                        }
                    }

                    // Remove asset items
                    for (int i = parent.Asset_List.Count - 1; i >= 0; i--)
                    {
                        if (parent.Asset_List[i].OrderID == refItem.OrderID && refItem.Name == parent.Asset_List[i].Name)
                            parent.Asset_List.RemoveAt(i);
                    }

                    Order order = parent.Order_List.First(x => x.OrderID == refItem.OrderID);
                    double itemPriceLessTax = refItem.Price - (refItem.Discount_Amt / refItem.Quantity);
                    double itemPriceTax = refItem.Price * ((order.Tax_Overridden || order.Order_Taxes == 0) ? 0 : (parent.Tax_Rules_Dictionary.ContainsKey(refItem.Category) ? Convert.ToDouble(parent.Tax_Rules_Dictionary[refItem.Category]) : parent.Tax_Rate));

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
                        if (refOrder.GC_Amount > 0)
                        {
                            foreach (GC GCard in parent.GC_List)
                            {
                                GCard.Reverse_Transaction(refOrder.OrderID);
                            }
                        }
                        order.GC_Amount = 0;
                    }


                    // Remove from tracking
                    if (parent.Tracking_List.Where(x => x.Ref_Order_Number == refOrder.OrderID).ToList().Count > 0)
                    {
                        parent.Tracking_List.Remove(parent.Tracking_List.First(x => x.Ref_Order_Number == refOrder.OrderID));
                    }

                    order.Order_Total_Pre_Tax -= itemPriceLessTax;
                    order.Order_Taxes -= itemPriceTax;

                    // Add cash value if paid by cash
                    if (order.Payment_Type == "Cash")
                    {
                        Cash.UpdateCashHistoryByID("O" + order.OrderID, (itemPriceLessTax + itemPriceTax));
                    }

                    repaintButtons = true;

                    PopulateOrderButtons(); // new prices so reprocess
                    bufferedPanel3.Invalidate();

                    parent.Background_Save();

                }
            }
            Grey_In();

        }

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public NewPurchases(Receipt _parent, string preLoadOrderID = "",
            Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));

            bufferedPanel3.Paint += new PaintEventHandler(panel1_Paint);
            bufferedPanel2.Paint += new PaintEventHandler(panel2_Paint);
            bufferedPanel3.Invalidate();
        }

        private void CreateFilterDictionary()
        {
            filterSettings = new Dictionary<string, string>();

            // Default is current month
            filterSettings.Add("fromMonth", DateTime.Now.Month.ToString());
            filterSettings.Add("toMonth", DateTime.Now.Month.ToString());
            filterSettings.Add("fromYear", DateTime.Now.Year.ToString());
            filterSettings.Add("toYear", DateTime.Now.Year.ToString());
            filterSettings.Add("category", "All");
            filterSettings.Add("payment", "All");
            filterSettings.Add("location", "All");
            filterSettings.Add("itemName", "");
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            CreateFilterDictionary();

            orderFlowPanel.HorizontalScroll.Maximum = 0;
            orderFlowPanel.AutoScroll = false;
            orderFlowPanel.VerticalScroll.Visible = false;
            orderFlowPanel.AutoScroll = true;

            panel1StartHeight = bufferedPanel1.Height;

            // Double buffering layout panels
            SetDoubleBuffered(orderFlowPanel);

            Application.DoEvents();

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

            // Populate order buttons
            PopulateOrderButtons();

        }
        // Form mnemonics
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            orderFlowPanel.Focus();
            switch (keyData)
            {
                case Keys.Up:
                {
                    if (currentSelectedButtonIndex > 0)
                        orderButtons[currentSelectedButtonIndex - 1].PerformClick();
                    return true;
                }
                case Keys.Down:
                {
                    if (currentSelectedButtonIndex < currentOrderList.Count - 1)
                        orderButtons[currentSelectedButtonIndex + 1].PerformClick();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private List<Button> orderButtons = new List<Button>();

        private void DBButtonClick(object sender, EventArgs e)
        {
            // Deselect all buttons
            orderButtons.ForEach(x => x.ForeColor = Color.White);

            Button DB = (Button) sender;
            DB.ForeColor = Color.Yellow;

            currentSelectedButtonIndex = orderButtons.IndexOf(DB);

            refOrder = currentOrderList.First(x => x.OrderID == DB.Name);

            Invalidate();
            bufferedPanel3.Paint += new PaintEventHandler(panel1_Paint);
            bufferedPanel2.Paint += new PaintEventHandler(panel2_Paint);

            locationText.Text = refOrder.Location;
            paymentText.Text = refOrder.Payment_Type;
            dateText.Text = refOrder.Date.ToString();

            repaintButtons = true;

            bufferedPanel2.Invalidate();
            bufferedPanel3.Invalidate();
            bufferedPanel1.Invalidate();

            orderFlowPanel.ScrollControlIntoView(DB);

        }

        private DateTime getFromDate()
        {
            return new DateTime(Convert.ToInt32(filterSettings["fromYear"]), Convert.ToInt32(filterSettings["fromMonth"]), 1);
        }

        private DateTime getToDate()
        {
            return new DateTime(Convert.ToInt32(filterSettings["toYear"]), Convert.ToInt32(filterSettings["toMonth"]), 1).AddMonths(1).AddDays(-1); // last day of current month
        }

        private List<Order> GetFilteredOrderList()
        {
            List<Order> preFilterList = new List<Order>();

            // Filter dates
            //Diagnostics.WriteLine("From date: " + getFromDate());
            //Diagnostics.WriteLine("To date: " + getToDate());
            preFilterList = parent.Order_List.Where(x => x.Date.Date <= getToDate().Date && x.Date.Date >= getFromDate().Date).ToList();

            // Filter category
            if (filterSettings["category"] != "All")
            {
                for (int i = preFilterList.Count - 1; i >= 0; i--)
                {
                    if (!parent.Master_Item_List.Any(x => x.OrderID == preFilterList[i].OrderID &&   //must have same orderID and contain filtered category
                                                          x.Category == filterSettings["category"]))
                    {
                        preFilterList.RemoveAt(i);
                    }
                }
            }

            // Filter payment
            if (filterSettings["payment"] != "All")
            {
                preFilterList = preFilterList.Where(x => x.Payment_Type.ToString() == filterSettings["payment"])
                    .ToList();
            }

            // Filter location
            if (filterSettings["location"] != "All")
            {
                preFilterList = preFilterList.Where(x => x.Location == filterSettings["location"])
                    .ToList();
            }

            // Filter items
            if (filterSettings["itemName"].Length > 0)
            {
                for (int i = preFilterList.Count - 1; i >= 0; i--)
                {
                    if (!parent.Master_Item_List.Any(x => x.OrderID == preFilterList[i].OrderID &&   //must have same orderID and contain filtered category
                                                          x.Name.ToLower().Contains(filterSettings["itemName"].ToLower())))
                    {
                        preFilterList.RemoveAt(i);
                    }
                }
            }

            // Reverse list
            preFilterList.Reverse();
            
            return preFilterList;
        }

        private void SortOrderList(ref List<Order> refOrderList)
        {
            switch (sortSetting)
            {
                case "Date Ascending":
                {
                    refOrderList = refOrderList.OrderBy(x => x.Date).ToList();
                    break;
                }
                case "Date Descending":
                {
                    refOrderList = refOrderList.OrderByDescending(x => x.Date).ToList();
                    break;
                }
                case "Price Ascending":
                {
                    refOrderList = refOrderList.OrderBy(x => (x.Order_Total_Pre_Tax + x.Order_Taxes)).ToList();
                    break;
                }
                case "Price Descending":
                {
                    refOrderList = refOrderList.OrderByDescending(x => (x.Order_Total_Pre_Tax + x.Order_Taxes)).ToList();
                    break;
                }
                case "Location Ascending":
                {
                    refOrderList = refOrderList.OrderBy(x => x.Location).ToList();
                    break;
                }
                case "Location Descending":
                {
                    refOrderList = refOrderList.OrderByDescending(x => x.Location).ToList();
                    break;
                }
                case "Payment Ascending":
                {
                    refOrderList = refOrderList.OrderBy(x => x.Payment_Type.ToString()).ToList();
                    break;
                }
                case "Payment Descending":
                {
                    refOrderList = refOrderList.OrderByDescending(x => x.Payment_Type.ToString()).ToList();
                    break;
                }
                default:
                {
                    refOrderList = refOrderList.OrderByDescending(x => x.Date).ToList();
                    break;
                }
            }
        }

        private int currentSelectedButtonIndex = 0;

        private void PopulateOrderButtons()
        {
            // Filter
            currentOrderList = GetFilteredOrderList();

            // Sort
            SortOrderList(ref currentOrderList);

            orderFlowPanel.Controls.Clear();
            if (currentOrderList.Count > 0)
            {
                SuspendDrawing(orderFlowPanel);

                orderButtons.ForEach(button => button.Visible = false);
                orderButtons.ForEach(button => orderFlowPanel.Controls.Remove(button));
                orderButtons.ForEach(button => button.Dispose());
                orderButtons = new List<Button>();

                foreach (Order order in currentOrderList)
                {
                    Size s = new Size(orderFlowPanel.Width - 2, 26);
                    Button DB = new Button();
                    DB.Size = s;
                    DB.Text = order.Location + " ($" +
                              String.Format("{0:0.00}", order.Order_Total_Pre_Tax + order.Order_Taxes) + ") - " +
                              order.Date.ToShortDateString();
                    DB.Name = order.OrderID;
                    DB.FlatStyle = FlatStyle.Flat;
                    DB.Margin = new Padding(0);
                    DB.Padding = new Padding(0);
                    DB.ForeColor = Color.White;
                    DB.TextAlign = ContentAlignment.MiddleLeft;
                    DB.Click += new EventHandler(DBButtonClick);

                    orderButtons.Add(DB);
                }

                orderFlowPanel.Controls.AddRange(orderButtons.ToArray());

                refOrder = currentOrderList[0];
                orderButtons[0].ForeColor = Color.Yellow; // select first item

                repaintButtons = true;

                bufferedPanel2.Invalidate();
                bufferedPanel3.Invalidate();
                ResumeDrawing(orderFlowPanel);

                refOrder = currentOrderList.First(x => x.OrderID == refOrder.OrderID);
                orderButtons[0].PerformClick();
            }
            else
            {
                refOrder = null;
                repaintButtons = true;
                Invalidate();
                locationText.Text = paymentText.Text = dateText.Text = "N/A";
            }

            editButton.Enabled = deletebutton.Enabled = locationText.Text != "N/A";
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

        public static void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] {true});
        }

        public void Set_Form_Color(Color randomColor)
        {
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor;
            label5.ForeColor = Color.Silver;
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

        private void button1_Click_1(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new OrderFilter(parent, filterSettings, Location, Size))
            {
                var result2 = form.ShowDialog();
                filterSettings = form.filterSettings;
                PopulateOrderButtons();
            }
            Grey_In();
        }

        private void deletebutton_Click(object sender, EventArgs e)
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
                        if (refOrder.GC_Amount > 0)
                        {
                            foreach (GC GCard in parent.GC_List)
                            {
                                GCard.Reverse_Transaction(refOrder.OrderID);
                            }
                        }

                        // Remove all cash relevant history
                        Cash.DeleteCashHistryByID("O" + refOrder.OrderID);

                        // Remove all asset items relevant to any items in order
                        parent.Asset_List = parent.Asset_List.Where(x => x.OrderID != refOrder.OrderID).ToList();

                        // Remove all hobby items relevant to any items in order
                        parent.Master_Hobby_Item_List = parent.Master_Hobby_Item_List.Where(x => x.OrderID != refOrder.OrderID).ToList();

                        parent.Master_Item_List = parent.Master_Item_List.Where(x => x.OrderID != refOrder.OrderID).ToList();
                        parent.Order_List.Remove(refOrder);

                        // Remove associated tracking information with order
                        parent.Tracking_List = parent.Tracking_List.Where(x => x.Ref_Order_Number != refOrder.OrderID).ToList();

                        repaintButtons = true;
                        PopulateOrderButtons(); // new prices so reprocess
                        bufferedPanel3.Invalidate();
                        parent.Background_Save();
                    }
                }
            }
            Grey_In();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
                this.Dispose();
                parent.Edit_Receipt(refOrder);
            }
            catch
            {
            }
        }

        private void excel_button_Click(object sender, EventArgs e)
        {

            TFLP.Height = this.Height - 2;
            TFLP.Location = new Point(1, 1);

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

            TFLP.Location = new Point(1000, 1000);

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

        bool GeneratingError = false;

        private List<Item> Print_Item_List = new List<Item>();

        private void Write_Excel()
        {
            // define which items to print
            Print_Item_List = new List<Item>();
            foreach (Order o in currentOrderList)
            {
                foreach (Item i in parent.Master_Item_List)
                {
                    if (i.OrderID == o.OrderID)
                    {
                        Print_Item_List.Add(i);
                    }
                }
            }

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
            sheet.Name = "Purchase Report";

            int row = 1;

            // insert title
            sheet.Cells[1, 1] = "Purchase Report";
            sheet.Cells.get_Range("A1").Font.Bold = true;
            sheet.Cells.get_Range("A1").Font.Size = 20;
            sheet.Cells.get_Range("A1").Font.ColorIndex = 56;
            sheet.Cells.get_Range("A1", "H1").Merge();
            row += 2;

            // write header
            WriteHeader(ref sheet, ref row);

            // adjust alignment first before changing specific cells
            sheet.Cells.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            // write information
            WriteOrders(ref sheet, ref row);

            // adjust style
            sheet.Cells.Columns.AutoFit();

            // Fix first row
            sheet.Application.ActiveWindow.SplitRow = 3;
            sheet.Application.ActiveWindow.FreezePanes = true;
            // Now apply autofilter
            Excel.Range firstRow = (Excel.Range)sheet.Rows[4];


            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Purchase Report (gen. on " + DateTime.Now.ToString("MM-dd-yyyy HH-mm") + ").xlsx");

            parent.GeneratedFilePaths_List.Add(path);

            try
            {
                if (File.Exists(path)) File.Delete(path);
                book.SaveAs(path, Excel.XlFileFormat.xlOpenXMLWorkbook);
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


        private void WriteHeader(ref Excel.Worksheet sheet, ref int row)
        {
            // header
            int col = 1;
            sheet.Cells[row, col++] = "Location";
            sheet.Cells[row, col++] = "Item";
            sheet.Cells[row, col++] = "Category";
            sheet.Cells[row, col++] = "Date";
            sheet.Cells[row, col++] = "Quantity";
            sheet.Cells[row, col++] = "Refunded";
            sheet.Cells[row, col++] = "Total";
            sheet.Cells[row, col++] = "Order Total";
            Excel.Range range = sheet.Cells.get_Range("A" + row.ToString(), "H" + row.ToString());
            range.Font.Bold = true;
            range.Font.Size = 14;
            range.Font.ColorIndex = 48;
            range.Cells.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
            row++;
        }

        double order_total = 0;

        private void WriteOrders(ref Excel.Worksheet sheet, ref int row)
        {
            int col = 1;

            /*
             * TEMPLATE
            
                sheet.Cells[row, col++] = // Location
                sheet.Cells[row, col++] = // Item
                sheet.Cells[row, col++] = // Category
                sheet.Cells[row, col++] = // Date
                sheet.Cells[row, col++] = // Quantity
                sheet.Cells[row, col++] = // Refunded
                sheet.Cells[row, col++] = // Item Total
                sheet.Cells[row, col++] = // Order Total
            */

            Order ref_Order = new Order();
            string current_order_id = "";
            Item item = new Item();
            double search_total = 0;

            for (int i = 0; i <= Print_Item_List.Count; i++)
            {
                if (i < Print_Item_List.Count) item = Print_Item_List[i];

                // Insert total line at the end
                if ((item.OrderID != current_order_id && current_order_id != "") || i == Print_Item_List.Count)
                {

                    ref_Order = parent.Order_List.First(x => x.OrderID == current_order_id);


                    // tax row
                    if (ref_Order.Tax_Overridden || ref_Order.Order_Taxes > 0)
                    {

                        // subtotal
                        Excel.Range range3 = sheet.Cells.get_Range("F" + row.ToString(), "G" + row.ToString());
                        // Accounting line
                        range3.Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;

                        range3 = sheet.Cells.get_Range("F" + row.ToString(), "F" + row.ToString());
                        range3.Font.ColorIndex = 48;
                        range3.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "Subtotal";
                        sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", ref_Order.Order_Total_Pre_Tax);
                        sheet.Cells[row, col++] = "";

                        row++;
                        col = 1;

                        // tax total
                        range3 = sheet.Cells.get_Range("F" + row.ToString(), "F" + row.ToString());
                        range3.Font.ColorIndex = 48;
                        range3.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "Taxes";
                        sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", ref_Order.Order_Taxes);
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = ref_Order.Tax_Overridden ? "*Tax Overridden" : "";

                        row++;
                        col = 1;
                    }

                    // final total row
                    Excel.Range range2 = sheet.Cells.get_Range("H" + row.ToString(), "H" + row.ToString());
                    range2.Font.Bold = true;
                    range2.Font.Size = 12;
                    range2.Cells.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
                    range2.Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;

                    ref_Order = parent.Order_List.First(x => x.OrderID == current_order_id);
                    sheet.Cells[row, col++] = "";
                    sheet.Cells[row, col++] = "";
                    sheet.Cells[row, col++] = "";
                    sheet.Cells[row, col++] = "";
                    sheet.Cells[row, col++] = "";
                    sheet.Cells[row, col++] = "";
                    sheet.Cells[row, col++] = "";
                    //sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", order_total);
                    sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", ref_Order.Order_Taxes + ref_Order.Order_Total_Pre_Tax);
                    search_total += ref_Order.Order_Taxes + ref_Order.Order_Total_Pre_Tax;
                    sheet.Cells[row, col++] = "";

                    // has GC
                    if (ref_Order.GC_Amount > 0)
                    {
                        string GC_Str = "Gift card(s) used: ";
                        foreach (GC GCard in parent.GC_List)
                        {
                            if (GCard.Associated_Orders.Contains(ref_Order.OrderID))
                            {
                                GC_Str += GCard.Location + " ($" + String.Format("{0:0.00}", GCard.Get_Order_Amount(ref_Order.OrderID)) + "), ";
                            }
                        }
                        sheet.Cells[row, col++] = GC_Str.Trim().Trim(',');
                    }

                    row++;
                    col = 1;

                    order_total = 0;
                }
                if (i < Print_Item_List.Count)
                {
                    if (item.OrderID != current_order_id)
                    {
                        // Insert double accounting total line and total


                        Excel.Range range = sheet.Cells.get_Range("A" + row.ToString(), "G" + row.ToString());
                        range.Font.Bold = true;
                        range.Font.Size = 12;
                        range.Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;

                        current_order_id = item.OrderID;
                        ref_Order = parent.Order_List.First(x => x.OrderID == current_order_id);
                        if (ref_Order.Tax_Overridden)
                            tax_override = true;

                        sheet.Cells[row, col++] = ref_Order.Location;
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = ref_Order.Date.ToShortDateString();
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = "";
                        sheet.Cells[row, col++] = ref_Order.OrderMemo;

                        // Adjust memo format
                        range = sheet.Cells.get_Range("J" + row.ToString(), "J" + row.ToString());
                        range.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                        range.Font.Italic = true;

                        row++;
                        col = 1;
                    }

                    if (item.Quantity == Convert.ToInt32(item.Status))
                    {
                        Excel.Range range = sheet.Cells.get_Range("B" + row.ToString(), "G" + row.ToString());
                        range.Font.Strikethrough = true;
                        range.Font.Color = Color.Red;
                    }

                    sheet.Cells[row, col++] = "";// Location
                    sheet.Cells[row, col++] = item.Name; // Item
                    sheet.Cells[row, col++] = item.Category; // Category
                    sheet.Cells[row, col++] = "";// Date
                    sheet.Cells[row, col++] = item.Quantity;// Quantity
                    sheet.Cells[row, col++] = Convert.ToInt32(item.Status) > 0 ? item.Status : "-";// Refunded
                    sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", item.Get_Current_Amount(parent.Get_Tax_Amount(item), true) - item.Get_Current_Tax_Amount(parent.Get_Tax_Amount(item)));// Item Total
                    order_total += item.Get_Full_Amount(parent.Get_Tax_Amount(item));
                    sheet.Cells[row, col++] = "";// Order Total
                    sheet.Cells[row, col++] = "";// Order Total
                    sheet.Cells[row, col++] = item.Memo;// Order Total

                    // Adjust memo format
                    Excel.Range range5 = sheet.Cells.get_Range("J" + row.ToString(), "J" + row.ToString());
                    range5.HorizontalAlignment = Excel.XlHAlign.xlHAlignLeft;
                    range5.Font.Italic = true;

                    // Highlight blue
                    if (filterSettings["category"] != "All" && item.Category == filterSettings["category"])
                    {
                        Excel.Range range = sheet.Cells.get_Range("B" + row.ToString(), "G" + row.ToString());
                        range.Interior.Color = Color.LightBlue;
                    }

                    col = 1;
                    row++;

                    if (item.Get_Current_Discount() > 0)
                    {

                        sheet.Cells[row, col++] = "";// Location
                        sheet.Cells[row, col++] = "";// Item
                        sheet.Cells[row, col++] = "Discount"; // Category
                        sheet.Cells[row, col++] = "";// Date
                        sheet.Cells[row, col++] = "";// Quantity
                        sheet.Cells[row, col++] = "Less";
                        sheet.Cells[row, col++] = "-$" + String.Format("{0:0.00}", item.Get_Current_Discount());// Item Total
                        order_total -= item.Discount_Amt;// Item Total
                        sheet.Cells[row, col++] = "";// Order Total

                        col = 1;
                        row++;
                    }
                }
            }

            row++;
            row++;

            col = 7;
            sheet.Cells[row, col++] = "GRAND TOTAL:";// Location
            //sheet.Cells[row, col++] = "TOTAL:";// Location
            sheet.Cells[row, col++] = "$" + String.Format("{0:0.00}", search_total);

            Excel.Range range55 = sheet.Cells.get_Range("A" + row.ToString(), "H" + row.ToString());
            range55.Font.Bold = true;
            range55.Font.Size = 14;
            range55.Font.ColorIndex = 48;

            range55 = sheet.Cells.get_Range("F" + row.ToString(), "G" + row.ToString());
            range55.Cells.Merge();

            range55 = sheet.Cells.get_Range("H" + row.ToString(), "H" + row.ToString());
            range55.Font.Color = Color.Black;
            range55.Font.Size = 14;
            range55.Cells.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlDouble;
            range55.Cells.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
            //range55.Cells.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
            //range55.Cells.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;

            //range55 = sheet.Cells.get_Range("F" + row.ToString(), "F" + row.ToString());
            //range55.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;
        }

        int index = 0;
        double total = 0;
        string current_order_id = "";
        bool tax_override = false;

        private void Add_button_Click(object sender, EventArgs e)
        {
            tax_override = false;
            total = 0;
            index = 0;
            current_order_id = "";

            Print_Item_List = new List<Item>();
            foreach (Order o in currentOrderList)
            {
                foreach (Item i in parent.Master_Item_List)
                {
                    if (i.OrderID == o.OrderID)
                    {
                        Print_Item_List.Add(i);
                    }
                }
            }

            if (currentOrderList.Count > 0)
            {
                Grey_Out();
                using (var form = new Yes_No_Dialog(parent, "Are you sure you wish to print current purchase list?", "Warning", "Preview", "Print", 0, this.Location, this.Size))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {

                        if (form.ReturnValue1 == "1")
                        {
                            printDocument1.Print();
                        }
                        else
                        {
                            printPreviewDialog1.TopMost = true;
                            printPreviewDialog1.ShowDialog();
                        }
                    }
                }
                Grey_In();
            }
            else
            {
                // Get monthly income and compare
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: There is nothing to print", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
        }

        private void printDocument1_PrintPage(System.Object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

            int column1 = 35;           // Item name
            int column2 = column1 + 220; // Category
            int column3 = column2 + 180; // Quantity
            int column4 = column3 + 100; // Refunded status
            int column5 = column4 + 166; // Price
            int starty = 10;
            int dataheight = 15;
            int height = starty + starty;

            StringFormat format1 = new StringFormat();
            format1.Alignment = StringAlignment.Center;

            Pen p = new Pen(Brushes.Black, 2.5f);
            Font f2 = new Font("MS Reference Sans Serif", 9f);
            Font f4 = new Font("MS Reference Sans Serif", 10f, FontStyle.Bold);
            Font f5 = new Font("MS Reference Sans Serif", 9f, FontStyle.Italic);
            Font f3 = new Font("MS Reference Sans Serif", 12f, FontStyle.Bold);
            Font f1 = new Font("MS Reference Sans Serif", 14.0f, FontStyle.Bold);

            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(122, 122, 122));
            SolidBrush LightGreyBrush = new SolidBrush(Color.FromArgb(200, 200, 200));
            Pen Grey_Pen = new Pen(GreyBrush, 1);
            Pen Light_Grey_Pen = new Pen(LightGreyBrush, 1);

            /*
            if (index < 5)
            {
                e.Graphics.DrawString(tabControl1.TabPages[selected_tab].Text, f1, Brushes.Black, new Rectangle(column1, height, 650, dataheight * 2));//, format1);
                height += dataheight;
                height += dataheight;
                height += dataheight
                height += dataheight;
            }*/

            if (index == 0)
            {
                e.Graphics.DrawString("PURCHASE  REPORT", f1, Brushes.Black, new Rectangle(10, height, 650, dataheight * 2));
                height += dataheight;
                height += dataheight;
                height += dataheight;
            }

            while (index < Print_Item_List.Count)
            {
                Item ref_item = Print_Item_List[index];
                if (height > e.MarginBounds.Height + 50)// + 20)
                {
                    if (tax_override)
                    {
                        e.Graphics.DrawString("*Tax has been overridden", f2, Brushes.Black, new Rectangle(639, 1030, 650, dataheight + 10));
                    }
                    tax_override = false;
                    height = starty;
                    e.HasMorePages = true;
                    return;
                }


                //if (g.Contains("`S`"))
                //{
                if (ref_item.OrderID != current_order_id)
                {
                    column1 -= 20;
                    height += 4;
                    height += 3;
                    e.Graphics.DrawLine(Grey_Pen, column1, height, 840, height);
                    height += 3;
                    current_order_id = ref_item.OrderID;
                    Order ref_Order = parent.Order_List.First(x => x.OrderID == current_order_id);
                    if (ref_Order.Tax_Overridden)
                        tax_override = true;

                    e.Graphics.DrawString(ref_Order.Location + " (" + ref_Order.Payment_Type + ")", f4, Brushes.Black, new Rectangle(column1, height, 650, dataheight));
                    e.Graphics.DrawString("Date: " + ref_Order.Date.ToShortDateString(), f4, Brushes.Black, new Rectangle(column3 - 47, height, 650, dataheight));
                    //e.Graphics.DrawString("Quantity: " + ref_Order.Order_Quantity.ToString(), f4, Brushes.Black, new Rectangle(650, height, 650, dataheight));
                    e.Graphics.DrawString("Total: $" + String.Format("{0:0.00}", ref_Order.Order_Taxes + ref_Order.Order_Total_Pre_Tax) +
                                          (ref_Order.Tax_Overridden ? "*" : ""), f4, Brushes.Black, new Rectangle(700, height, 650, dataheight));
                    total += ref_Order.Order_Taxes + ref_Order.Order_Total_Pre_Tax;
                    height += dataheight;
                    if (ref_Order.OrderMemo.Length > 1)
                    {
                        e.Graphics.DrawString(" Memo: " + ref_Order.OrderMemo, f5, Brushes.Black, new Rectangle(column1, height, 650, dataheight));
                        height += dataheight;
                    }

                    // has GC
                    if (ref_Order.GC_Amount > 0)
                    {
                        string GC_Str = "Gift card(s) used: ";
                        foreach (GC GCard in parent.GC_List)
                        {
                            if (GCard.Associated_Orders.Contains(ref_Order.OrderID))
                            {
                                GC_Str += GCard.Location + " ($" + String.Format("{0:0.00}", GCard.Get_Order_Amount(ref_Order.OrderID)) + "), ";
                            }
                        }
                        e.Graphics.DrawString(GC_Str.Trim().Trim(','), f5, Brushes.Black, new Rectangle(column1, height, 650, dataheight));
                        height += dataheight;
                    }

                    column1 += 20;
                    height += 3;
                }
                e.Graphics.DrawLine(Light_Grey_Pen, column1, height + dataheight + 2, 800, height + dataheight + 2);
                e.Graphics.DrawString(ref_item.Name, f2, Brushes.Black, new Rectangle(column1, height, 650, dataheight));//, format1);
                e.Graphics.DrawString(ref_item.Category, f2, Brushes.Black, new Rectangle(column2, height, 650, dataheight));//, format1);
                e.Graphics.DrawString("Quantity: " + ref_item.Quantity.ToString(), f2, Brushes.Black, new Rectangle(column3, height, 650, dataheight));//, format1);
                if (Convert.ToInt32(ref_item.Status) > 0) e.Graphics.DrawString("Refunded: " + ref_item.Status, f2, Brushes.Black, new Rectangle(column4, height, 650, dataheight));//, format1);
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", ref_item.Get_Full_Amount(parent.Get_Tax_Amount(ref_item))), f2, Brushes.Black, new Rectangle(column5 + 30, height, 650, dataheight));//, format1);
                //}
                //else
                //{
                //    e.Graphics.DrawString(g, f2, Brushes.Black, new Rectangle(column1, height, 650, dataheight));//, format1);
                //}
                height += dataheight;
                index++;
            }
            height += dataheight;
            height += dataheight;
            e.Graphics.DrawString("Grand Total: $" + String.Format("{0:0.00}", total), f3, Brushes.Black, new Rectangle(600, height, 650, dataheight + 10));
            if (!e.HasMorePages && tax_override)
            {
                e.Graphics.DrawString("*Tax has been overridden", f2, Brushes.Black, new Rectangle(649, 1030, 650, dataheight + 10));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Visible = false;
            Purchases P = new Purchases(parent, false, null, this.Location, this.Size);
            P.ShowDialog();
        }

        private void sortOptions_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new SortFilter(parent, sortSetting, Location, Size))
            {
                var result2 = form.ShowDialog();
                sortSetting = form.sortMode;
                PopulateOrderButtons();
            }
            Grey_In();
        }

    }
}
