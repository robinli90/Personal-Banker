using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Objects;

namespace Financial_Journal
{
    public partial class MobileSyncDialog : Form
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
            if (syncCount > 0)
            {
                if (secondThreadFormHandle != IntPtr.Zero)
                    PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent,
                    "Please remember to refresh your orders on mobile device! This will prevent redundancy", true, 0,
                    this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
            parent.MobileSync.currentlyChecking = false; // toggle back so it can check again
            parent.Background_Save();
            parent.Activate();
            Task.Run(() => UploadAllOrders());
            base.OnFormClosing(e);
        }

        Receipt parent;

        public List<Order> _tempSyncOrderList;
        public List<Order> _startingOrderList;
        public List<Item> _tempSyncItemList;
        public List<Item> _startingItemList;

        private Order refOrder;

        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);

            int start_margin = bufferedPanel1.Left + 5; // Item
            int start_height = bufferedPanel1.Bottom + 7;
            int price_margin = priceLabel.Left + 7;

            Color DrawForeColor = Color.White;
            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(88, 88, 88));
            Pen Grey_Pen = new Pen(GreyBrush, 2);
            Pen p = new Pen(WritingBrush, 1);
            Font f = new Font("MS Reference Sans Serif", 8F, FontStyle.Regular);

            // Draw gray header line
            e.Graphics.DrawLine(Grey_Pen, 8, paymentBox.Bottom + 10, bufferedPanel2.Width - 8, paymentBox.Bottom + 10);

            // Draw white footer line
            e.Graphics.DrawLine(p, 8, start_height, bufferedPanel2.Width - 8, start_height);
            e.Graphics.DrawString("Subtotal", f, WritingBrush, price_margin - 80, start_height + 7);
            e.Graphics.DrawString("$" + String.Format("{0:0.00}", refOrder.Order_Total_Pre_Tax), f, WritingBrush,
                price_margin, start_height + 7);
            e.Graphics.DrawString(refOrder.Order_Quantity.ToString(), f, WritingBrush, quantityLabel.Left + 22,
                start_height + 7);

            e.Graphics.DrawString("Taxes", f, WritingBrush, price_margin - 65, start_height + 26);
            e.Graphics.DrawString("$" + String.Format("{0:0.00}", refOrder.Order_Taxes), f, WritingBrush, price_margin,
                start_height + 26);

            e.Graphics.DrawLine(p, 8, start_height + 44, bufferedPanel2.Width - 8, start_height + 44);
            e.Graphics.DrawString("Total", f, WritingBrush, price_margin - 65, start_height + 47);
            e.Graphics.DrawString("$" + String.Format("{0:0.00}", refOrder.Order_Total_Pre_Tax + refOrder.Order_Taxes),
                f, WritingBrush, price_margin, start_height + 47);


            // Dispose all objects
            p.Dispose();
            Grey_Pen.Dispose();
            GreyBrush.Dispose();
            WritingBrush.Dispose();
            f.Dispose();
            base.OnPaint(e);
        }


        // Panel paint
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);

            // allow scroll transformation
            e.Graphics.TranslateTransform(bufferedPanel3.AutoScrollPosition.X, bufferedPanel3.AutoScrollPosition.Y);

            int data_height = 20;
            int row_count = 0;
            int height_offset = 1;
            int start_height = 0;
            int start_margin = 5; // Item
            int price_margin = priceLabel.Left + 7;
            int qty_margin = quantityLabel.Left + 18;
            int category_margin = categoryLabel.Left - 4; // - 20;

            Color DrawForeColor = Color.White;

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);

            Pen p = new Pen(WritingBrush, 1);
            Font f = new Font("MS Reference Sans Serif", 8F, FontStyle.Regular);

            foreach (Item item in _tempSyncItemList.Where(x => x.OrderID == _selectedOrderID))
            {
                e.Graphics.DrawString(item.Name, f, WritingBrush, start_margin,
                    start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", item.Price), f, WritingBrush, price_margin,
                    start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString(item.Quantity.ToString(), f, WritingBrush, qty_margin,
                    start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString(item.Category, f, WritingBrush, category_margin,
                    start_height + height_offset + (row_count * data_height));
                row_count++;
            }

            // extra space at bottom
            row_count++;

            // Resize panel
            bufferedPanel1.AutoScrollMinSize = new Size(bufferedPanel3.Width,
                start_height + height_offset + row_count * data_height);

            // Force resize only if too big
            if (start_height + height_offset + row_count * data_height > bufferedPanel1.Height)
                bufferedPanel3.Height = new Size(bufferedPanel1.Width,
                    start_height + height_offset + row_count * data_height).Height;

            // Dispose all objects
            p.Dispose();
            WritingBrush.Dispose();
            f.Dispose();

            Invalidate();
        }


        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public MobileSyncDialog(Receipt _parent, List<Order> tempSyncOrderList, List<Item> tempSyncItemList,
            Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));

            _tempSyncOrderList = tempSyncOrderList;
            _tempSyncItemList = tempSyncItemList;

            _startingOrderList = new List<Order>();
            _startingItemList = new List<Item>();
            tempSyncOrderList.ForEach(x => _startingOrderList.Add(x.Copy_Item()));
            tempSyncItemList.ForEach(x => _startingItemList.Add(x.Copy_Item()));

            bufferedPanel3.Paint += new PaintEventHandler(panel1_Paint);
            bufferedPanel2.Paint += new PaintEventHandler(panel2_Paint);
            bufferedPanel3.Invalidate();
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            orderFlowPanel.HorizontalScroll.Maximum = 0;
            orderFlowPanel.AutoScroll = false;
            orderFlowPanel.VerticalScroll.Visible = false;
            orderFlowPanel.AutoScroll = true;

            // Double buffering layout panels
            SetDoubleBuffered(orderFlowPanel);

            Application.DoEvents();

            // Populate order buttons
            PopulateOrderButtons();

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

            refOrder = _tempSyncOrderList.First(x => x.OrderID == _selectedOrderID);
            ValidateInfoTypes(refOrder);

            orderButtons[0].PerformClick();
        }

        private List<Button> orderButtons = new List<Button>();

        private string _selectedOrderID = "";


        private void DBButtonClick(object sender, EventArgs e)
        {
            // Deselect all buttons
            orderButtons.ForEach(x => x.ForeColor = Color.White);


            Button DB = (Button) sender;
            DB.ForeColor = Color.Yellow;
            _selectedOrderID = DB.Name;

            refOrder = _tempSyncOrderList.First(x => x.OrderID == _selectedOrderID);
            ValidateInfoTypes(refOrder);

            Invalidate();
            bufferedPanel3.Paint += new PaintEventHandler(panel1_Paint);
            bufferedPanel2.Paint += new PaintEventHandler(panel2_Paint);

            PopulateBoxes();

            dateTimePicker1.Value = refOrder.Date;
            locationBox.Text = refOrder.Location;
            paymentBox.Text = refOrder.Payment_Type;

            bufferedPanel2.Invalidate();
            bufferedPanel3.Invalidate();
            bufferedPanel1.Invalidate();

        }

        private void PopulateOrderButtons()
        {
            orderFlowPanel.Controls.Clear();
            SuspendDrawing(orderFlowPanel);

            orderButtons.ForEach(button => button.Visible = false);
            orderButtons.ForEach(button => orderFlowPanel.Controls.Remove(button));
            orderButtons.ForEach(button => button.Dispose());
            orderButtons = new List<Button>();

            foreach (Order order in _tempSyncOrderList)
            {
                Size s = new Size(orderFlowPanel.Width - 2, 26);
                Button DB = new Button();
                DB.Size = s;
                DB.Text = order.Location + " ($" + String.Format("{0:0.00}", order.Order_Total_Pre_Tax) + ") - " +
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

            PopulateBoxes();

            orderFlowPanel.Controls.AddRange(orderButtons.ToArray());

            _selectedOrderID = _tempSyncOrderList[0].OrderID;
            orderButtons[0].ForeColor = Color.Yellow; // select first item

            refOrder = _tempSyncOrderList.First(x => x.OrderID == _selectedOrderID);

            bufferedPanel2.Invalidate();
            bufferedPanel3.Invalidate();
            ResumeDrawing(orderFlowPanel);

            this.Focus();
        }

        private void PopulateBoxes()
        {
            #region Populate locations and payments

            locationBox.Items.Clear();
            paymentBox.Items.Clear();

            foreach (string location in parent.location_box.Items)
            {
                locationBox.Items.Add(location);
            }
            parent.Payment_List.ForEach(x => paymentBox.Items.Add(x.ToString()));
            paymentBox.Items.Add("Cash");
            paymentBox.Items.Add("Other");

            #endregion
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

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            refOrder.Date = dateTimePicker1.Value;
        }

        private void deletebutton_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Yes_No_Dialog(parent, "Are you sure you wish to delete this order?", "Warning", "No",
                "Yes", 0, this.Location, this.Size))
            {
                var result2 = form.ShowDialog();
                if (result2 == DialogResult.OK && form.ReturnValue1 == "1")
                {
                    _tempSyncItemList = _tempSyncItemList.Where(x => x.OrderID != refOrder.OrderID).ToList();
                    _tempSyncOrderList.Remove(refOrder);

                    _startingOrderList.First(x => x.OrderID == refOrder.OrderID).IUO_IsSynced = true;

                    #region Save changes to FTP

                    // Create local file
                    string saveText = "";

                    // Create temp sync file
                    string tempMobileSyncFile = parent.localSavePath + "\\" +
                                                parent.Settings_Dictionary["LOGIN_EMAIL"] + "_sync.pbf";

                    foreach (Order order in _startingOrderList)
                    {
                        #region Save corresponding items with orderID

                        saveText += "[OR_LO_]=" + order.Location +
                                    //"||[PRETAX_PRICE]=" + order.OrderTotalPreTax +
                                    "||[OR_DA_]=" + order.Date.ToString() +
                                    "||[OR_PA_]=" + order.Payment_Type +
                                    "||[OR_ID_]=" + order.OrderID +
                                    (order.IUO_IsSynced ? "||[OR_SY_]=1" : "") +
                                    Environment.NewLine;

                        #endregion

                        #region Save order with corresponding orderID

                        foreach (Item item in _startingItemList.Where(x => x.OrderID == order.OrderID).ToList())
                        {
                            saveText += "[IT_NA_]=" + item.Name +
                                        "||[IT_CA_]=" + item.Category +
                                        "||[IT_QU_]=" + item.Quantity +
                                        "||[IT_PR_]=" + item.Price +
                                        "||[IT_ID_]=" + item.OrderID + Environment.NewLine;
                        }

                        #endregion
                    }

                    using (StreamWriter sw = File.CreateText(tempMobileSyncFile)) //
                    {
                        sw.Write(AESGCM.SimpleEncryptWithPassword(saveText, MobileSync.AESGCMKey));
                        sw.Close();
                    }

                    // Create mapping file on FTP
                    string ftpPath =
                        @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/Cloud_Sync/Sync/" +
                        parent.Settings_Dictionary["LOGIN_EMAIL"] + "_sync.pbf";
                    Cloud_Services.FTP_Upload_Synced(ftpPath, tempMobileSyncFile);

                    try
                    {
                        File.Delete(tempMobileSyncFile);
                    }
                    catch (Exception ex)
                    {
                        Diagnostics.WriteLine("Cannot delete temporary sync file");
                    }

                    parent.MobileSync.currentlyChecking = false;
                    //Close();
                    syncCount++;

                    #endregion

                    if (_tempSyncOrderList.Count > 0)
                    {
                        PopulateOrderButtons();
                        orderButtons[0].PerformClick();
                    }
                    else
                    {
                        Close();
                    }
                }
            }
            Grey_In();
        }

        /// <summary>
        /// Validate refOrder's location, payment type, and ALL item categories
        /// </summary>
        private void ValidateInfoTypes(Order order)
        {
            #region Validate Location

            if (getInfoType(order.Location, MobileSync.InfoType.Location) == "") // if doesn't have, prompt
            {
                Grey_Out();
                SyncSynonym SS = new SyncSynonym(parent, order.Location, MobileSync.InfoType.Location, this.Location,
                    this.Size);
                SS.ShowDialog();
                Grey_In();
            }
            order.Location = getInfoType(order.Location, MobileSync.InfoType.Location);

            #endregion

            #region Validate Payment

            if (getInfoType(order.Payment_Type, MobileSync.InfoType.Payment) == "") // if doesn't have, prompt
            {
                Grey_Out();
                SyncSynonym SS = new SyncSynonym(parent, order.Payment_Type, MobileSync.InfoType.Payment, this.Location,
                    this.Size);
                SS.ShowDialog();
                Grey_In();
            }
            order.Payment_Type = getInfoType(order.Payment_Type, MobileSync.InfoType.Payment);

            #endregion

            #region Validate ALL Item Categories for refOrder

            foreach (Item item in _tempSyncItemList.Where(x => x.OrderID == order.OrderID).ToList())
            {
                if (getInfoType(item.Category, MobileSync.InfoType.Category) == "") // if doesn't have, prompt
                {
                    Grey_Out();
                    SyncSynonym SS = new SyncSynonym(parent, item.Category, MobileSync.InfoType.Category, this.Location,
                        this.Size);
                    SS.ShowDialog();
                    Grey_In();
                }

                item.Category = getInfoType(item.Category, MobileSync.InfoType.Category);

            }

            #endregion

            parent.MobileSync.UpdateOrderInformation(order);
        }

        private string getInfoType(string comparisonName, MobileSync.InfoType _refType)
        {
            // Check associations
            foreach (Association association in parent.AssociationList.Where(x => x.InfoType == _refType))
            {
                if (association.LinkSource == comparisonName)
                {
                    return association.LinkDestination;
                }
            }

            switch (_refType)
            {
                case MobileSync.InfoType.Payment:
                {
                    if (comparisonName == "Cash" || comparisonName == "Other") return comparisonName;
                    if (parent.Payment_List.Any(x => x.ToString() == comparisonName))
                        return comparisonName;
                    break;
                }
                case MobileSync.InfoType.Category:
                {

                    foreach (string category in parent.category_box.Items)
                    {
                        if (category.ToLower() == comparisonName.ToLower())
                        {
                            return comparisonName;
                        }
                    }
                    break;
                }
                case MobileSync.InfoType.Location:
                {
                    if (parent.Location_List
                        .Any(x => x.Name.ToLower() == comparisonName.ToLower())) // has existing
                        return comparisonName;
                    break;
                }
            }

            return "";
        }

        private void Sync_Order(Order o, bool saveChangesToFtp = false)
        {
            // Validate order information and associations
            ValidateInfoTypes(o);

            // Validate orderID hash collision
            // Get Random Order ID
            Random OrderID_Gen = new Random();
            string randomID = OrderID_Gen.Next(100000000, 999999999).ToString();

            // Ensure no clashing of IDs
            List<Item> same_ID = parent.Master_Item_List.Where(x => x.OrderID == randomID).ToList();

            while (same_ID.Count > 0)
            {
                randomID = OrderID_Gen.Next(100000000, 999999999).ToString();
                same_ID = parent.Master_Item_List.Where(x => x.OrderID == randomID).ToList();
            }

            string originalOrderID = o.OrderID;

            // Set synced on reflist
            _startingOrderList.First(x => x.OrderID == o.OrderID).IUO_IsSynced = true;

            //o.Payment_Type = paymentBox.Text;
            //o.Location = locationBox.Text;

            // Update order with new orderID
            o.OrderID = randomID;
            parent.Order_List.Add(o);

            // Update items with new orderID
            _tempSyncItemList.Where(x => x.OrderID == originalOrderID).ToList().ForEach(y =>
            {
                y.OrderID = randomID;
            });

            parent.Master_Item_List.AddRange(_tempSyncItemList.Where(x => x.OrderID == randomID));

            // To ensure that old orders with cash do not get appended to current history if viewed and saved 
            if (o.Payment_Type == "Cash")
            {
                Cash.AddCashHistory(o.Date,
                    String.Format("Purchase(s) at {0}", o.Location), -(o.Order_Total_Pre_Tax + o.Order_Taxes), // No discounts for mobile sync
                    "O" + o.OrderID);
            }

            // Remove items from order AND item lists
            _tempSyncOrderList.Remove(o);
            _tempSyncItemList = _tempSyncItemList.Where(x => x.OrderID != randomID).ToList();

            if (_tempSyncOrderList.Count > 0)
            {
                PopulateOrderButtons();
                orderButtons[0].PerformClick();
            }

            if (saveChangesToFtp)
            {
                // Create local file
                string saveText = "";

                // Create temp sync file
                string tempMobileSyncFile = parent.localSavePath + "\\" +
                                            parent.Settings_Dictionary["LOGIN_EMAIL"] + "_sync.pbf";

                foreach (Order order in _startingOrderList)
                {
                    #region Save corresponding items with orderID

                    saveText += "[OR_LO_]=" + order.Location +
                                //"||[PRETAX_PRICE]=" + order.OrderTotalPreTax +
                                "||[OR_DA_]=" + order.Date.ToString() +
                                "||[OR_PA_]=" + order.Payment_Type +
                                "||[OR_ID_]=" + order.OrderID +
                                (order.IUO_IsSynced ? "||[OR_SY_]=1" : "") +
                                Environment.NewLine;

                    #endregion

                    #region Save order with corresponding orderID

                    foreach (Item item in _startingItemList.Where(x => x.OrderID == order.OrderID).ToList())
                    {
                        saveText += "[IT_NA_]=" + item.Name +
                                    "||[IT_CA_]=" + item.Category +
                                    "||[IT_QU_]=" + item.Quantity +
                                    "||[IT_PR_]=" + item.Price +
                                    "||[IT_ID_]=" + item.OrderID + Environment.NewLine;
                    }

                    #endregion
                }

                using (StreamWriter sw = File.CreateText(tempMobileSyncFile)) //
                {
                    sw.Write(AESGCM.SimpleEncryptWithPassword(saveText, MobileSync.AESGCMKey));
                    sw.Close();
                }

                // Create mapping file on FTP
                string ftpPath =
                    @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/Cloud_Sync/Sync/" +
                    parent.Settings_Dictionary["LOGIN_EMAIL"] + "_sync.pbf";
                Cloud_Services.FTP_Upload_Synced(ftpPath, tempMobileSyncFile);

                try
                {
                    File.Delete(tempMobileSyncFile);
                }
                catch (Exception e)
                {
                    Diagnostics.WriteLine("Cannot delete temporary sync file");
                }

                if (_tempSyncOrderList.Count == 0)
                {
                    parent.MobileSync.currentlyChecking = false;
                    Close();

                }
            }
        }

        private int syncCount = 0;

        // sync current
        private void button1_Click(object sender, EventArgs e)
        {
            parent.MobileSync.currentlyChecking = false;
            Sync_Order(refOrder, true);
            syncCount++;
        }

        // sync all
        private void button2_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Application.DoEvents();
            using (var form2 = new Yes_No_Dialog(parent, "Are you sure you wish to sync all orders?", "Warning", "No",
                "Yes", 0, this.Location, this.Size))
            {
                var result2 = form2.ShowDialog();
                if (result2 == DialogResult.OK && form2.ReturnValue1 == "1")
                {
                    foreach (Order order in _tempSyncOrderList)
                    {
                        ValidateInfoTypes(order);
                    }

                    Cursor.Current = Cursors.WaitCursor;

                    if (secondThreadFormHandle == IntPtr.Zero)
                    {

                        Loading_Form form =
                            new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size,
                                "SYNCHRONIZING", "YOUR ORDERS", 11)
                            {
                            };
                        form.HandleCreated += SecondFormHandleCreated;
                        form.HandleDestroyed += SecondFormHandleDestroyed;
                        form.RunInNewThread(false);
                    }

                    for (int i = _tempSyncOrderList.Count - 1; i >= 0; i--)
                    {
                        Sync_Order(_tempSyncOrderList[i], i == 0);
                        syncCount++;
                    }

                    if (secondThreadFormHandle != IntPtr.Zero)
                        PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
            }
            Grey_In();
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
            ManageAssociations MA = new ManageAssociations(parent, Location, Size);
            MA.ShowDialog();
            Grey_In();
        }

        private void locationBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            refOrder.Location = locationBox.Text;
        }

        private void paymentBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            refOrder.Payment_Type = paymentBox.Text;
        }

        /// <summary>
        /// Upload all items and order list to FTP server (for mobile services)
        /// </summary>
        private void UploadAllOrders()
        {
            parent.Saving_In_Process = true;
            parent.UpdateStatus("Syncing mobile information");

            // Create local file
            string saveText = "";

            // Create temp sync file
            string tempMobileSyncFile = parent.localSavePath + "\\" +
                                        parent.Settings_Dictionary["LOGIN_EMAIL"] + "_sync.pbf";

            // Save items
            saveText += parent.SaveHelper.Save_Item_Information();

            // Save orders
            saveText += parent.SaveHelper.Save_Orders();

            using (StreamWriter sw = File.CreateText(tempMobileSyncFile)) //
            {
                sw.Write(AESGCM.SimpleEncryptWithPassword(saveText, MobileSync.AESGCMKey));
                sw.Close();
            }

            // Create mapping file on FTP
            string ftpPath =
                @"ftp://robinli.asuscomm.com/Seagate_Backup_Plus_Drive/Personal%20Banker/Cloud_Sync/Sync/" +
                parent.Settings_Dictionary["LOGIN_EMAIL"] + "_fsync.pbf";
            Cloud_Services.FTP_Upload_Synced(ftpPath, tempMobileSyncFile);

            try
            {
                File.Delete(tempMobileSyncFile);
            }
            catch (Exception e)
            {
                Diagnostics.WriteLine("Cannot delete temporary sync file");
            }

            parent.UpdateStatus("Sync complete!");
            Thread.Sleep(250);
            parent.Saving_In_Process = false;
            parent.UpdateStatus("Ready");
        }
    }
}
