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
    public partial class Refund_Settings : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Save_Refund_Days();
            parent.Background_Save();
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

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Refund_Settings(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }


        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            // Check if the KeyCode value has the Keys.Enter flag set
            //if ((e.KeyCode & Keys.Enter) == Keys.Enter)
            Save_Refund_Days();

            // Prevent the key event from being passed on to the control
            e.Handled = true;
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            dataGridView1.KeyUp += dataGridView1_KeyUp;


            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
            dataGridView1.GridColor = Color.White;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(98, 110, 110);

            dataGridView1.Columns[0].ReadOnly = true;

            // Disable sorting
            foreach (DataGridViewColumn column in dataGridView1.Columns) { column.SortMode = DataGridViewColumnSortMode.NotSortable; }

            Populate_Refund_List();

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

        private void Save_Refund_Days()
        {
            // Check if the KeyCode value has the Keys.Enter flag set
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row != null && dataGridView1.Rows.IndexOf(row) < dataGridView1.Rows.Count)
                {
                    int refundDays = 0;
                    if (row.Cells[1].Value.ToString().Length > 0 && row.Cells[1].Value.ToString().All(char.IsDigit))
                    {
                        refundDays = Convert.ToInt32(row.Cells[1].Value);
                    }
                    else
                    {
                        refundDays = 0;
                    }

                    parent.Location_List.First(x => x.Name == row.Cells[0].Value.ToString()).Refund_Days = refundDays;
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //this.dataGridView1.ClearSelection();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            this.dataGridView1.ClearSelection();
        }

        private void Populate_Refund_List()
        {
            dataGridView1.Rows.Clear();

            foreach (Location loc in parent.Location_List)
            {
                dataGridView1.Rows.Add(loc.Name, (loc.Refund_Days == 0 ? "" : loc.Refund_Days.ToString()), "");

            }
            //this.Height += dataGridView1.Rows.Count * 22;
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
    }
}
