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
    public partial class Asset_Import : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }
        /*
         * Resizing form
         * 
        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;

        protected override void OnPaint(PaintEventArgs e) {
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
            rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
            //e.Graphics.FillRectangle(Brushes.DarkBlue, rc);
        }

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
        string Ref_Category = "";

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Asset_Import(Receipt _parent, string Cate, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            Ref_Category = Cate;
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {

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

            foreach (string g in parent.category_box.Items)
            {
                this.category_box.Items.Add(g);
            }

            label5.Text += " " + Ref_Category;

            category_box.SelectedIndex = 0;

            #region Add checkboxes
            DataGridViewCheckBoxColumn cbc = new DataGridViewCheckBoxColumn();
            {
                cbc.HeaderText = "";
                cbc.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;
                cbc.Width = 23;
                cbc.FlatStyle = FlatStyle.Flat;
                cbc.CellTemplate.Style.BackColor = Color.FromArgb(64, 64, 64);
                cbc.CellTemplate.Style.ForeColor = Color.White;
                cbc.DisplayIndex = 0;
            }
            dataGridView1.Columns.Insert(0, cbc);
            #endregion

            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
            dataGridView1.GridColor = Color.White;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(98, 110, 110);

            PopulateDGV();

            #region Get Max Category value for added focus
            
            // Select most appropriate current category based on maximum counts
            Dictionary<string, int> Max_Category_Count_Per_Profile = new Dictionary<string,int>();
            if (category_box.Items.Count > 0)
            {
                // Set IUO item categories for all according assets
                List<Asset_Item> AI_List_Temp = parent.Asset_List.Where(x => x.Asset_Category == Ref_Category).ToList();

                foreach (Asset_Item AI in AI_List_Temp)
                {
                    if (AI.OrderID.Length > 0)
                    {
                        AI.Item_category_IUO = parent.Master_Item_List.FirstOrDefault(x => x.Name == AI.Name && x.OrderID == AI.OrderID).Category;
                    }
                }

                foreach (string category in parent.category_box.Items)
                {
                    List<Item> Categorical_Items = parent.Master_Item_List.Where(x => x.Category == category).ToList();
                    Max_Category_Count_Per_Profile.Add(category, AI_List_Temp.Where(x => x.Item_category_IUO == category).ToList().Count);
                }
                var max = Max_Category_Count_Per_Profile.Aggregate((l, r) => l.Value > r.Value ? l : r).Key; // aggregate stores last time a value has swapped, giving location of kvp.key

                if (category_box.Items.Contains(max.ToString()))
                    category_box.Text = max.ToString();
            }
            #endregion
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridView1.ClearSelection();
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewCheckBoxColumn && e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                if (e.ColumnIndex == 0)
                {
                    if (Convert.ToBoolean(row.Cells[0].Value))
                    {
                        row.SetValues(false);

                        checkAll.CheckedChanged -= checkAll_CheckedChanged;
                        checkAll.Checked = false;
                        checkAll.CheckedChanged += checkAll_CheckedChanged;
                    }
                    else
                    {
                        row.SetValues(true);
                    }
                }
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            this.dataGridView1.ClearSelection();
        }

        List<Item> Post_Filter_Item_List;

        private void PopulateDGV()
        {
            Post_Filter_Item_List = new List<Item>();

            // We want to filter the list for existing items already denoted as an asset
            foreach (Item item in parent.Master_Item_List.Where(x => x.Category == category_box.Text).ToList())
            {
                // Copy for each current quantity
                for (int i = 0; i < item.Get_Current_Quantity(); i++)
                {
                    Post_Filter_Item_List.Add(item.Copy_Item()); // Add multiple copies of new clones
                }
            }

            // Alphabetical order
            Post_Filter_Item_List = Post_Filter_Item_List.OrderBy(x => x.Name).ToList();

            foreach (Asset_Item AI in parent.Asset_List)
            {
                for (int i = Post_Filter_Item_List.Count - 1; i >= 0; i--)
                {
                    Item Ref_Item =  Post_Filter_Item_List[i];
                    double Tax_Rate = parent.Tax_Rules_Dictionary.ContainsKey(Ref_Item.Category) ? Convert.ToDouble(parent.Tax_Rules_Dictionary[Ref_Item.Category]) : parent.Tax_Rate;
                    double cost = Math.Round((Ref_Item.Price * (1 + Tax_Rate)) - (Ref_Item.Discount_Amt > 0 ? (Ref_Item.Discount_Amt / parent.Master_Item_List.FirstOrDefault(x => x.Name == Ref_Item.Name && x.OrderID == Ref_Item.OrderID).Quantity) : 0), 2);

                    // If exact same item
                    if (AI.OrderID == Ref_Item.OrderID && AI.Name == Ref_Item.Name && 
                        Math.Round(AI.Cost, 2) == cost)
                    {
                        Post_Filter_Item_List.RemoveAt(i);
                        break; // stop from removing more than one item at a time
                    }
                }
            }

            dataGridView1.Rows.Clear();

            foreach (Item item in Post_Filter_Item_List)
            {
                double Tax_Rate = parent.Tax_Rules_Dictionary.ContainsKey(item.Category) ? Convert.ToDouble(parent.Tax_Rules_Dictionary[item.Category]) : parent.Tax_Rate;
                dataGridView1.Rows.Add(false, item.Name, "$" + String.Format("{0:0.00}", (item.Price * (1 + Tax_Rate)) - (item.Discount_Amt > 0 ? (item.Discount_Amt / parent.Master_Item_List.FirstOrDefault(x => x.Name == item.Name && x.OrderID == item.OrderID).Quantity) : 0)), item.Location);
            }
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
            bool hasChecked = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(row.Cells[0].Value))
                {
                    hasChecked = true;
                    break;
                }
            }

            if (hasChecked)
            {
                Grey_Out();
                using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish close? You have items selected", "Warning", "No", "Yes", 0, this.Location, this.Size))
                {
                    var result21 = form1.ShowDialog();
                    if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                    {
                        this.Dispose();
                        this.Close();
                    }
                    else if (result21 == DialogResult.OK && form1.ReturnValue1 == "0")
                    {
                    }
                }
                Grey_In();
            }
            else
            {
                this.Dispose();
                this.Close();
            }
        }

        public void Set_Form_Color(Color randomColor)
        {
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        private void category_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateDGV();
        }

        private void checkAll_CheckedChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1[0, i].Value = checkAll.Checked;
            }
        }

        private void import_Click(object sender, EventArgs e)
        {
            Grey_Out();
            bool hasChecked = false;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (Convert.ToBoolean(row.Cells[0].Value))
                {
                    hasChecked = true;
                    break;
                }
            }

            if (hasChecked)
            {
                using (var form = new Yes_No_Dialog(parent, "Are you sure you to import selected items?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK && form.ReturnValue1 == "1")
                    {
                        for (int i = 0; i < dataGridView1.Rows.Count; i++)
                        {
                            DataGridViewRow row = dataGridView1.Rows[i];

                            // If checked
                            if (Convert.ToBoolean(row.Cells[0].Value))
                            {
                                // add asset
                                Item item = Post_Filter_Item_List[i];
                                double Tax_Rate = parent.Tax_Rules_Dictionary.ContainsKey(item.Category) ? Convert.ToDouble(parent.Tax_Rules_Dictionary[item.Category]) : parent.Tax_Rate;

                                Asset_Item AI = new Asset_Item()
                                {
                                    Name = item.Name,
                                    Purchase_Date = item.Date,
                                    Cost = (item.Price * (1 + Tax_Rate)) - (item.Discount_Amt > 0 ? (item.Discount_Amt / parent.Master_Item_List.FirstOrDefault(x => x.Name == item.Name && x.OrderID == item.OrderID).Quantity) : 0),
                                    Serial_Identification = "",
                                    Note = "From Purchases",
                                    Asset_Category = Ref_Category,
                                    Purchase_Location = item.Location,
                                    OrderID = item.OrderID
                                };
                                parent.Asset_List.Add(AI);
                            }
                        }
                        Close();
                    }
                }
            }
            else
            {
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: You have nothing selected", true, -26, this.Location, this.Size);
                FMB.ShowDialog();
            }
            Grey_In();
        }
    }
}
