using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrinterInventory
{
    public partial class ViewCart : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.RefreshMain();
            parent.Grey_In();
            base.OnFormClosing(e);
            
        }

        Main parent;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public ViewCart(Main _parent, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
            parent.Grey_Out();
        }

        private void PopulateCart()
        {
            dataGridView1.Rows.Clear();

            foreach (Cartridge c in parent.CartridgeList.Where(x => x.CartQuantity > 0).ToList())
            {
                dataGridView1.Rows.Add(c.Brand, c.Model, "");

                // Datagrid Button Style Setting Dynamically
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Font = new Font(dataGridView1.Font.FontFamily, dataGridView1.Font.Size, FontStyle.Bold);
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.ApplyStyle(style);
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].ToolTipText = "Delete Entry";
            }
            //this.Height += dataGridView1.Rows.Count * 22;
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
            dataGridView1.GridColor = Color.White;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(98, 110, 110);

            // Disable sorting
            foreach (DataGridViewColumn column in dataGridView1.Columns) { column.SortMode = DataGridViewColumnSortMode.NotSortable; }

            // Add buttons
            DataGridViewButtonColumn buttons = new DataGridViewButtonColumn();
            {
                buttons.HeaderText = "";
                buttons.Text = "X";
                buttons.UseColumnTextForButtonValue = true;
                buttons.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;
                buttons.Width = 23;
                buttons.FlatStyle = FlatStyle.Flat;
                buttons.CellTemplate.Style.BackColor = Color.FromArgb(64, 64, 64);
                buttons.CellTemplate.Style.ForeColor = Color.White;
                buttons.DisplayIndex = 2;
            }

            //dataGridView1.Columns.Add(buttons);
            dataGridView1.Columns.Insert(2, buttons);

            PopulateCart();

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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridView1.ClearSelection();

            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0 && e.ColumnIndex == 2)
            {
                Cartridge refCartridge =
                    parent.CartridgeList.First(x => x.Model == dataGridView1[1, e.RowIndex].Value.ToString() &&
                                                    x.Brand == dataGridView1[0, e.RowIndex].Value.ToString());

                RemoveFromCart(refCartridge);
                PopulateCart();
            }
        }

        private void RemoveFromCart(Cartridge c)
        {
            List<Cartridge> tempCartridges =
                parent.EntireCartridgeList.Where(x => x.Model.ToLower() == c.Model.ToLower() &&
                                                      x.Brand.ToLower() == c.Brand.ToLower() &&
                                                      x.Memo.ToLower() == c.Memo.ToLower())
                    .ToList();

            // Update internal lists
            tempCartridges.ForEach(x => x.CartQuantity = 0);

            // Update SQL database
            parent.UpdateDatabase(tempCartridges, "cartquantity", "0");

            parent.repaintButtons = true;
            parent.AggregateCartridgeList();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            this.dataGridView1.ClearSelection();
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
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (YesNoDialog IB = new YesNoDialog(parent, "Are you sure you want to clear your shopping cart?",
                Location, Size))
            {
                IB.ShowDialog();
                if (IB.DialogResult == DialogResult.OK && IB.returnValue == 1)
                {
                    foreach (Cartridge cartridge in parent.CartridgeList)
                    {
                        RemoveFromCart(cartridge);
                    }
                    PopulateCart();
                }
            }
            Grey_In();
        }
    }
}
