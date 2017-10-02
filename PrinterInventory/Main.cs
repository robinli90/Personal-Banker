using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrinterInventory
{
    public partial class Main : Form
    {
        // Only visible
        public List<Cartridge> CartridgeList = new List<Cartridge>();

        // Entire list
        public List<Cartridge> EntireCartridgeList = new List<Cartridge>();

        // Button list
        List<Button> ButtonList = new List<Button>();

        public bool repaintButtons;

        protected override void OnPaint(PaintEventArgs e)
        {

            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(88, 88, 88));
            Pen Grey_Pen = new Pen(GreyBrush, 2);

            // Draw gray header line
            e.Graphics.DrawLine(Grey_Pen, 8, addNewCartridge.Top - 8, this.Width - 8, addNewCartridge.Top - 8);

            // Draw gray separator
            e.Graphics.DrawLine(Grey_Pen, addNewCartridge.Left - 18, addNewCartridge.Top - 8, addNewCartridge.Left - 18, this.Height- 8);

            GreyBrush.Dispose();
            base.OnPaint(e);
        }

        public Button currentClickedButton;

        // Panel paint
        private void panel2_Paint(object sender, PaintEventArgs e)
        {
            if (repaintButtons)
            {
                ButtonList.ForEach(button => button.Image.Dispose());
                ButtonList.ForEach(button => button.Dispose());
                ButtonList.ForEach(button => this.Controls.Remove(button));
                ButtonList.ForEach(button => panel2.Controls.Remove(button));
                ButtonList = new List<Button>();
            }

            e.Graphics.Clear(BackColor);

            // allow scroll transformation
            e.Graphics.TranslateTransform(panel2.AutoScrollPosition.X, panel2.AutoScrollPosition.Y);

            int data_height = 25; //20
            int buttonSpacing = 20;
            int row_count = 0;
            int height_offset = 1;
            int start_height = 5;
            int start_margin = 5; // Item

            int brand_margin = brandLabel.Left - 10;
            int model_margin = modelLabel.Left - 15;
            int qty_margin = quantityLabel.Left + 4;
            int memo_margin = memoLabel.Left - 15; // - 20;
            int button_margin = panel1.Width - 90;

            Color DrawForeColor = Color.White;
            SolidBrush RedBrush = new SolidBrush(Color.FromArgb(255,50,50));
            SolidBrush LightOrangeBrush = new SolidBrush(Color.FromArgb(255, 200, 0));
            SolidBrush GrayBackground = new SolidBrush(Color.FromArgb(73, 73, 73));
            SolidBrush LighterGrayBackground = new SolidBrush(Color.DarkSlateGray);

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);

            Pen p = new Pen(WritingBrush, 1);
            Font f = new Font("MS Reference Sans Serif", 8F, FontStyle.Regular);
            Font fBold = new Font("MS Reference Sans Serif", 8F, FontStyle.Bold);

            int entryIndex = 0; // used to keep track of which button is pressed even when hidden items are there

            int modelBrandCount = 0;
            Cartridge prevCartridge = new Cartridge();
            
            foreach (Cartridge ctg in CartridgeList)
            {
                if ((ctg.Quantity > 0 || showZeroQuantities.Checked)
                    && (searchBox.Text.Length == 0 || cartridgeContainsText(ctg, searchBox.Text)))
                {
                    #region Gray background

                    if (groupCartridges.Checked)
                    {
                        if (prevCartridge.Model != ctg.Model || prevCartridge.Brand != ctg.Brand)
                        {
                            modelBrandCount++;
                        }

                        if (modelBrandCount % 2 == 1)
                        {
                            e.Graphics.FillRectangle(GrayBackground, 0, row_count * data_height + 1, button_margin - 5,
                                data_height);
                        }
                        else
                        {
                            e.Graphics.FillRectangle(LighterGrayBackground, 0, row_count * data_height + 1,
                                button_margin - 5,
                                data_height);
                        }

                        prevCartridge = ctg;
                    }

                    #endregion

                    e.Graphics.DrawString(ctg.Brand, f, WritingBrush, brand_margin,
                        start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(ctg.Model, f, WritingBrush, model_margin,
                        start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(ctg.Quantity.ToString(), 
                        ctg.Quantity == 0 ? fBold : f, 
                        ctg.Quantity == 0 ? RedBrush : WritingBrush,
                        qty_margin, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(ctg.Memo, f, WritingBrush, memo_margin,
                        start_height + height_offset + (row_count * data_height));

                    if (repaintButtons)
                    {
                        #region Add Buttons
                        Button button = new Button();
                        button.BackColor = BackColor;
                        button.ForeColor = BackColor;
                        button.FlatStyle = FlatStyle.Flat;
                        button.Image = global::PrinterInventory.Properties.Resources.add;
                        button.Size = new Size(21, 21);
                        button.Location = new Point(button_margin,
                            start_height + height_offset + (row_count * data_height) - 3);
                        button.Name = "add" + entryIndex;
                        button.Text = "";
                        button.Click += new EventHandler(this.dynamic_button_click);
                        ButtonList.Add(button);
                        panel2.Controls.Add(button);
                        #endregion

                        if (ctg.Quantity > 0)
                        {
                            #region Subtract Buttons

                            button = new Button();
                            button.BackColor = BackColor;
                            button.ForeColor = BackColor;
                            button.FlatStyle = FlatStyle.Flat;
                            button.Image = global::PrinterInventory.Properties.Resources.minus;
                            button.Size = new Size(21, 21);
                            button.Location = new Point(button_margin + buttonSpacing * 1 + 1,
                                start_height + height_offset + (row_count * data_height) - 3);
                            button.Name = "sub" + entryIndex;
                            button.Text = "";
                            button.Click += new EventHandler(this.dynamic_button_click);
                            ButtonList.Add(button);
                            panel2.Controls.Add(button);

                            #endregion
                        }

                        else // only delete if 0 quantity
                        {
                            #region Delete Buttons
                            button = new Button();
                            button.BackColor = BackColor;
                            button.ForeColor = BackColor;
                            button.FlatStyle = FlatStyle.Flat;
                            button.Image = global::PrinterInventory.Properties.Resources.delete;
                            button.Size = new Size(21, 21);
                            button.Location = new Point(button_margin + buttonSpacing * 1 + 1,
                                start_height + height_offset + (row_count * data_height) - 3);
                            button.Name = "del" + entryIndex;
                            button.Text = "";
                            button.Click += new EventHandler(this.dynamic_button_click);
                            ButtonList.Add(button);
                            panel2.Controls.Add(button);
                            #endregion
                        }

                        #region Cart buttons
                        button = new Button();
                        button.BackColor = BackColor;
                        button.ForeColor = BackColor;
                        button.FlatStyle = FlatStyle.Flat;
                        button.Image = ctg.CartQuantity > 0 ? Properties.Resources.addcartgreen : Properties.Resources.addcart;
                        button.Size = new Size(21, 21);
                        button.Location = new Point(button_margin + buttonSpacing * 2,
                            start_height + height_offset + (row_count * data_height) - 3);
                        button.Name = "adc" + entryIndex;
                        button.Text = "";
                        button.Click += new EventHandler(this.dynamic_button_click);
                        ButtonList.Add(button);
                        panel2.Controls.Add(button);
                        #endregion
                    }
                    row_count++;
                }
                entryIndex++; 
            }

            // extra space at bottom
            //row_count++;

            // Resize panel
            panel1.AutoScrollMinSize = new Size(panel2.Width,
                start_height + height_offset + row_count * data_height);

            // Force resize only if too big
            if (start_height + height_offset + row_count * data_height > panel1.Height)
                panel2.Height = new Size(panel1.Width,
                    start_height + height_offset + row_count * data_height).Height;

            // Dispose all objects
            p.Dispose();
            WritingBrush.Dispose();
            RedBrush.Dispose();
            LighterGrayBackground.Dispose();
            GrayBackground.Dispose();
            f.Dispose();

            repaintButtons = false;

        }


        Database masterDB = Database.DECADE_MARKHAM;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Main()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            repaintButtons = true;

            // Mousedown anywhere to drag
            MouseDown += Form_MouseDown;

            // Load prexisting
            QueryAllCartridges();

            panel2.Paint += new PaintEventHandler(panel2_Paint);
            panel2.Invalidate();

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


        }


        private void dynamic_button_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            int cartridgeIndex = Convert.ToInt32(b.Name.Substring(3));
            Cartridge refCartridge = CartridgeList[cartridgeIndex];

            currentClickedButton = b;

            if (b.Name.StartsWith("add"))
            {
                #region add quantity

                using (InputBox QB = new InputBox(this, InputType.String,
                    "Please provide a requisitioner for " + refCartridge, true,
                    0, true, Location, Size))
                {
                    QB.ShowDialog();
                    if (QB.DialogResult == DialogResult.OK && QB.returnValue.Length > 0)
                    {


                        double refPrice = 0;

                        if (refCartridge.Quantity == 0) repaintButtons = true; // force repaint

                        if (!useCurrentPrices.Checked)
                        {
                            using (InputBox IB = new InputBox(this, InputType.Currency,
                                String.Format("Please provide a different price for this cartridge: {2}{0} ({1})",
                                    refCartridge.Model, refCartridge.Brand, Environment.NewLine), true,
                                10, true, Location, Size))
                            {
                                IB.ShowDialog();
                                if (IB.DialogResult == DialogResult.OK && IB.returnValue.Length > 1)
                                {
                                    refPrice = Convert.ToDouble(IB.returnValue.Substring(1));
                                }
                            }
                        }
                        else // Get most recent price
                        {
                            refPrice = EntireCartridgeList.OrderByDescending(x => x.ReceiveDate).First(
                                x => x.Model.ToLower() == refCartridge.Model.ToLower() &&
                                     x.Brand.ToLower() == refCartridge.Brand.ToLower() &&
                                     x.Memo.ToLower() == refCartridge.Memo.ToLower()).Price;
                        }

                        AddCartridge(
                            new Cartridge()
                            {
                                Brand = refCartridge.Brand,
                                Model = refCartridge.Model,
                                Memo = refCartridge.Memo,
                                Quantity = 1,
                                Price = refPrice,
                                ReceiveDate = DateTime.Now,
                                RemoveDate = new DateTime(),
                                RemoveMemo = "",
                                Requisitioner = QB.returnValue,
                                InternalNote = "",
                                CartQuantity = refCartridge.CartQuantity,
                                HashID = GetNewHashID()
                            }
                        );
                    }
                }
                #endregion
            }
            else if (b.Name.StartsWith("sub"))
            {
                #region subtract quantity
                using (InputBox IB = new InputBox(this, InputType.String, "Please provide a reason" + Environment.NewLine + "(ex. replacement for Bonnie)", true,
                    0, true, Location, Size))
                {
                    IB.ShowDialog();
                    if (IB.DialogResult == DialogResult.OK && IB.returnValue.Length > 0)
                    {

                        if (refCartridge.Quantity == 1) repaintButtons = true; // force repaint
                        // Remove the first instance of cartridge (FIFO)

                        Cartridge passCartridge =
                            EntireCartridgeList.First(x => x.Model.ToLower() == refCartridge.Model.ToLower() &&
                                                           x.Brand.ToLower() == refCartridge.Brand.ToLower() &&
                                                           x.Memo.ToLower() == refCartridge.Memo.ToLower() &&
                                                           x.Quantity > 0);

                        RemoveCartridge(passCartridge, "", IB.returnValue);
                    }
                }
                #endregion
            }
            else if (b.Name.StartsWith("del"))
            {
                #region delete cartridge
                if (refCartridge.Quantity > 0)
                {
                    Form_Message_Box FMB =
                        new Form_Message_Box(this, "You cannot remove a cartridge with existing inventory", true, -10, Location, Size);
                    FMB.ShowDialog();
                    return;
                }
                else
                {
                    using (YesNoDialog IB = new YesNoDialog(this, "Are you sure you want to remove this cartridge?",
                        Location, Size))
                    {
                        IB.ShowDialog();
                        if (IB.DialogResult == DialogResult.OK && IB.returnValue == 1)
                        {

                            List<Cartridge> tempCartridges =
                                EntireCartridgeList.Where(x => x.Model.ToLower() == refCartridge.Model.ToLower() &&
                                                               x.Brand.ToLower() == refCartridge.Brand.ToLower() &&
                                                               x.Memo.ToLower() == refCartridge.Memo.ToLower())
                                    .ToList();

                            tempCartridges.ForEach(x => x.InternalNote = "donotshow" + x.InternalNote);
                            tempCartridges.ForEach(x => RemoveCartridge(x, "", "", true));
                        }
                    }
                    Grey_In();
                    AggregateCartridgeList();
                }
                #endregion
            }
            else if (b.Name.StartsWith("adc"))
            {
                #region add/remove to/from cart
                List<Cartridge> tempCartridges =
                    EntireCartridgeList.Where(x => x.Model.ToLower() == refCartridge.Model.ToLower() &&
                                                   x.Brand.ToLower() == refCartridge.Brand.ToLower() &&
                                                   x.Memo.ToLower() == refCartridge.Memo.ToLower())
                        .ToList();

                // Update internal lists
                tempCartridges.ForEach(x => x.CartQuantity = (refCartridge.CartQuantity == 1 ? 0 : 1));

                // Update SQL database
                UpdateDatabase(tempCartridges, "cartquantity", (refCartridge.CartQuantity == 1 ? 0 : 1).ToString());

                repaintButtons = true;
                AggregateCartridgeList();
                #endregion
            }

            panel2.Invalidate();

            //Point scrollPoint = new Point(currentScrollLocation.X, currentScrollLocation.Y + panel2.Height - ButtonList[1].Height);
            panel2.AutoScrollPosition = new Point(0, 9999);//currentScrollLocation == null ? new Point() : scrollPoint;
            //Diagnostics.WriteLine(scrollPoint.X + ", " + scrollPoint.Y);
            //panel2.ScrollControlIntoView(currentClickedButton);
        }

        /// <summary>
        /// Add cartridge to 
        /// </summary>
        /// <param name="ctg"></param>
        public void AddCartridge(Cartridge c)
        {
            // Set new hashID and set to cartridge c
            string hashID = GetNewHashID();
            c.HashID = hashID;

            // Add cartridge to current entire list and aggregate
            EntireCartridgeList.Add(c);
            AggregateCartridgeList();

            // Update decade with an insert
            string query = String.Format("insert into [tiger].[dbo].printer_inventory " +
                "values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', " +
                "'{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}')",
                c.Brand, ParamaterizeQuery(c.Model), c.Quantity.ToString(), ParamaterizeQuery(c.Memo), c.Price.ToString(), c.ReceiveDate.ToString(),
                c.RemoveDate.ToString(), ParamaterizeQuery(c.RemoveMemo), ParamaterizeQuery(c.Requisitioner), c.InternalNote, hashID, c.CartQuantity, "", "", "", "");


            ExcoODBC instance = ExcoODBC.Instance;
            instance.Open(masterDB);
            OdbcDataReader reader = instance.RunQuery(query);
            reader.Close();
        }

        /// <summary>
        /// Remove Cartridge (overload removes exact hashID instead of cartridge object)
        /// *** ASSUMES THERE IS A QUANTITY > 0 (error is handled however)
        /// </summary>
        /// <param name="c"></param>
        /// <param name="hashID"></param>
        public void RemoveCartridge(Cartridge c=null, string hashID="", string removeNote="", bool permanentRemoval=false)
        {
            // Update entire list
            if (c == null && hashID == "")
                throw new NoCartridgeException(); 

            string refHashID = c != null ? c.HashID : hashID;

            Cartridge refCartridge = EntireCartridgeList.First(x => x.HashID == refHashID);
            try
            {
                refCartridge.Quantity--;
                refCartridge.RemoveDate = DateTime.Now;
                refCartridge.RemoveMemo = removeNote;
            }
            catch
            {
                //if (refCartridge.Quantity <= 0) 
                throw new NoCartridgeException();
            }

            //refCartridge.Quantity--;
            AggregateCartridgeList();

            // Update database (should only affect a single line)
            string query = String.Format("update [tiger].[dbo].printer_inventory set quantity = '{0}', removedate = '{2}', " +
                                         "removenote = '{3}', internalnote = '{4}' where hashID = '{1}'", "0",
                                         refHashID, refCartridge.RemoveDate, refCartridge.RemoveMemo, (permanentRemoval ? "donotshow" : ""));

            ExcoODBC instance = ExcoODBC.Instance;
            instance.Open(masterDB);
            OdbcDataReader reader = instance.RunQuery(query);
            reader.Close();
        }

        public void UpdateDatabase(List<Cartridge> cartridgeList, string columnName, string value)
        {
            // update only cartridges from list provided
            string cartridges = "";
            cartridgeList.ForEach(x => cartridges += String.Format("or hashID = '{0}' ", x.HashID));

            // Update database (should only affect a single line)
            string query = String.Format("update [tiger].[dbo].printer_inventory set {1} = '{0}' where {2}", value, columnName, cartridges.Substring(3));

            ExcoODBC instance = ExcoODBC.Instance;
            instance.Open(masterDB);
            OdbcDataReader reader = instance.RunQuery(query);
            reader.Close();
        }

        public string ParamaterizeQuery(string query)
        {
            string returnStr = "";
            // Escape '
            for (int i = 0; i < query.Length; i++)
            {
                returnStr += query[i];
                if (query[i] == '\'')
                {
                    returnStr += "'";
                }
            }
            return returnStr;
        }

        /// <summary>
        /// Generate a new hashID for cartridge
        /// </summary>
        /// <returns></returns>
        public string GetNewHashID()
        {
            Random rgen = new Random();
            string HashID = rgen.Next(100000000, 999999999).ToString();

            // Prevent collision
            while (EntireCartridgeList.Any(x => x.HashID == HashID))
            {
                HashID = rgen.Next(100000000, 999999999).ToString();
            }

            return HashID;
        }

        /// <summary>
        /// Query all cartridges from decade master list
        /// </summary>
        public void QueryAllCartridges()
        {
            ExcoODBC instance = ExcoODBC.Instance;

            // Reset cartridge lists (temp is before aggregation of quantities)
            EntireCartridgeList = new List<Cartridge>();
            CartridgeList = new List<Cartridge>();

            #region Get All Cartridges
            string query = "select * from [tiger].[dbo].printer_inventory where internalnote not like '%donotshow%'";
            instance.Open(masterDB);
            OdbcDataReader reader = instance.RunQuery(query);
            while (reader.Read())
            {
                EntireCartridgeList.Add(new Cartridge()
                {
                    Brand = reader[0].ToString(),
                    Model = reader[1].ToString(),
                    Memo = reader[3].ToString(),
                    Quantity = Convert.ToInt32(reader[2].ToString()),
                    Price = Convert.ToDouble(reader[4].ToString()),
                    ReceiveDate = Convert.ToDateTime(reader[5].ToString()),
                    RemoveDate = Convert.ToDateTime(reader[6].ToString()),
                    RemoveMemo = reader[7].ToString(),
                    Requisitioner = reader[8].ToString(),
                    InternalNote = reader[9].ToString(),
                    HashID = reader[10].ToString(),
                    CartQuantity = Convert.ToInt32(reader[11].ToString() == "" ? "0" : reader[11].ToString())
                });
            }
            reader.Close();
            #endregion

            // Order list by brand and then model
            EntireCartridgeList = EntireCartridgeList.OrderBy(x => x.Brand).ThenBy(x => x.Model).ThenBy(x => x.ReceiveDate).ToList();

            // Aggregate List
            AggregateCartridgeList();
        }

        /// <summary>
        /// Combine existing quantites together
        /// </summary>
        public void AggregateCartridgeList()
        {
            CartridgeList = new List<Cartridge>();

            // Aggregate quantities
            foreach (Cartridge ctg in EntireCartridgeList.Where(x => !x.InternalNote.Contains("donotshow")).ToList())
            {
                List<Cartridge> tempList = CartridgeList.Where(x => x.Model.ToLower() == ctg.Model.ToLower() &&
                                                                    x.Brand.ToLower() == ctg.Brand.ToLower() &&
                                                                    x.Memo.ToLower() == ctg.Memo.ToLower()).ToList();
                if (tempList.Count == 0)
                {
                    // Create in list
                    ctg.Quantity = ctg.Quantity > 0 ? 1 : 0;
                    CartridgeList.Add(ctg.HardCopy());
                }
                else
                {
                    Cartridge refCartridge = CartridgeList.First(x => x.Model.ToLower() == ctg.Model.ToLower() &&
                                                        x.Brand.ToLower() == ctg.Brand.ToLower() &&
                                                        x.Memo.ToLower() == ctg.Memo.ToLower());

                    // Get latest date
                    if (ctg.ReceiveDate > refCartridge.ReceiveDate)
                    {
                        refCartridge.ReceiveDate = ctg.ReceiveDate;
                    }

                    // Add one to quantity
                    refCartridge.Quantity += (ctg.Quantity > 0 ? 1 : 0);
                }
            }
        }

        /// <summary>
        /// Return a list of cartridge that have the same model brand and memo
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public List<Cartridge> GetMasterCartridgesList(Cartridge c)
        {
            return EntireCartridgeList.Where(x => x.Model.ToLower() == c.Model.ToLower() &&
                                                  x.Brand.ToLower() == c.Brand.ToLower() &&
                                                  x.Memo.ToLower() == c.Memo.ToLower()).ToList();
        }

        /// <summary>
        /// Refreshes main window
        /// </summary>
        public void RefreshMain()
        {
            repaintButtons = true;
            panel2.Invalidate();
        }

        FadeControl TFLP;

        public void Grey_Out()
        {
            TFLP.Location = new Point(1, 1);
        }

        public void Grey_In()
        {
            TFLP.Location = new Point(1000, 1000);
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        public void Set_Form_Color(Color randomColor)
        {
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        public List<string> printerBrandList = new List<string>()
        {
            "Canon",
            "HP",
            "Lexmark",
            "OKI",
            "Samsung",
            "Xerox",
        };

        private void addSolidButton_Click(object sender, EventArgs e)
        {
            AddCartridge AC = new AddCartridge(this, Location, Size);
            AC.ShowDialog();
        }

        private void show_general_box_CheckedChanged(object sender, EventArgs e)
        {
            repaintButtons = true;
            panel2.Invalidate();
        }

        private bool cartridgeContainsText(Cartridge c, string text)
        {
            text = text.ToLower();
            if (text.Length == 0) return true;
            if (
                c.Brand.ToLower().Contains(text) ||
                c.Model.ToLower().Contains(text) ||
                c.Memo.ToLower().Contains(text) ||
                //c.RemoveDate.ToString().ToLower().Contains(text) ||
                //c.ReceiveDate.ToString().ToLower().Contains(text) ||
                c.Requisitioner.ToLower().Contains(text)
                //c.RemoveMemo.ToLower().Contains(text)
                )
            {
                return true;
            }
            return false;
        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {
            repaintButtons = true;
            panel2.Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PDFGenerator PDFG = new PDFGenerator(this, ReportType.Usage);
        }

        private void settingsButton_Click(object sender, EventArgs e)
        {
            // Load prexisting
            repaintButtons = true;
            QueryAllCartridges();
            panel2.Invalidate();
        }

        private void viewCart_Click(object sender, EventArgs e)
        {
            ViewCart VC = new ViewCart(this, Location, Size);
            VC.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (YesNoDialog IB = new YesNoDialog(this, "Do you want to include cartridges with zero quantity?",
                Location, Size))
            {
                IB.ShowDialog();
                if (IB.DialogResult == DialogResult.OK)
                {
                    PDFGenerator PDFG = new PDFGenerator(this, ReportType.Inventory, IB.returnValue == 1);
                }
            }
            Grey_In();
        }

        private void groupCartridges_CheckedChanged(object sender, EventArgs e)
        {
            panel2.Invalidate();
        }

    }

    public class AdvancedComboBox : ComboBox
    {
        new public System.Windows.Forms.DrawMode DrawMode { get; set; }
        public Color HighlightColor { get; set; }
        public Color FrameColor { get; set; }

        public AdvancedComboBox()
        {
            //this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.FrameColor = SystemColors.HotTrack;
            base.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.HighlightColor = Color.Gray;
            this.DrawItem += new DrawItemEventHandler(AdvancedComboBox_DrawItem);
        }

        void AdvancedComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;


            ComboBox combo = sender as ComboBox;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e.Graphics.FillRectangle(new SolidBrush(HighlightColor),
                    e.Bounds);
            else
                e.Graphics.FillRectangle(new SolidBrush(combo.BackColor),
                    e.Bounds);

            e.Graphics.DrawString(combo.Items[e.Index].ToString(), e.Font,
                new SolidBrush(combo.ForeColor),
                new Point(e.Bounds.X, e.Bounds.Y));

            //e.Graphics.DrawString(combo.Items[e.Index].ToString(), e.Font, brush, e.Bounds, new StringFormat(StringFormatFlags.DirectionRightToLeft));

            //e.Graphics.DrawRectangle(new Pen(FrameColor, 2), 0, 0,
            //  this.Width - 2, this.Items.Count * 20);

            // Draw the rectangle around the drop-down list
            //if (combo.DroppedDown)
            if (false)
            {
                SolidBrush ArrowBrush = new SolidBrush(SystemColors.HighlightText);

                Rectangle dropDownBounds = new Rectangle(0, 0, combo.Width - 2, combo.Items.Count * combo.ItemHeight);
                //ControlPaint.DrawBorder(g, dropDownBounds, _borderColor, _borderStyle);
                ControlPaint.DrawBorder(e.Graphics, dropDownBounds,
                    FrameColor, 1, ButtonBorderStyle.Solid,
                    FrameColor, 1, ButtonBorderStyle.Solid,
                    FrameColor, 1, ButtonBorderStyle.Solid,
                    FrameColor, 1, ButtonBorderStyle.Solid);
            }
            e.DrawFocusRectangle();
        }

    }

    public class NoCartridgeException : Exception
    {
    }
}
