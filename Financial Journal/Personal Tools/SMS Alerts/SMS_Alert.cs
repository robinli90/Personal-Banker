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
    public partial class SMS_Alert : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
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
        public SMS_Alert(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
            dataGridView1.GridColor = Color.White;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(98, 110, 110);

            #region Add edit buttons
            DataGridViewButtonColumn buttons = new DataGridViewButtonColumn();
            {
                buttons.HeaderText = "";
                buttons.Text = "\uD83D\uDD0D";// "🔍";
                buttons.UseColumnTextForButtonValue = true;
                buttons.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;
                buttons.Width = 23;
                buttons.FlatStyle = FlatStyle.Flat;
                buttons.CellTemplate.Style.BackColor = Color.FromArgb(64, 64, 64);
                buttons.CellTemplate.Style.ForeColor = Color.White;
                buttons.DisplayIndex = 3;
            }
            dataGridView1.Columns.Insert(3, buttons);
            #endregion

            #region Add delete buttons
            DataGridViewButtonColumn buttons2 = new DataGridViewButtonColumn();
            {
                buttons2.HeaderText = "";
                buttons2.Text = "X";// "🔍";
                buttons2.UseColumnTextForButtonValue = true;
                buttons2.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;
                buttons2.Width = 23;
                buttons2.FlatStyle = FlatStyle.Flat;
                buttons2.CellTemplate.Style.BackColor = Color.FromArgb(64, 64, 64);
                buttons2.CellTemplate.Style.ForeColor = Color.White;
                buttons2.DisplayIndex = 4;
            }
            dataGridView1.Columns.Insert(4, buttons2);
            #endregion

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

            PopulateDGV();

            
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridView1.ClearSelection();
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0 && e.RowIndex <= dataGridView1.Rows.Count - 1)
            {

                Grey_Out();
                SMSAlert Ref_SMSA = parent.SMSAlert_List[e.RowIndex];

                if (e.ColumnIndex == 3)
                {
                    Add_SMS SA = new Add_SMS(parent, Ref_SMSA, this.Location, this.Size);
                    SA.ShowDialog();
                }
                else if (e.ColumnIndex == 4)
                {
                    using (var form = new Yes_No_Dialog(parent, "Are you sure you to delete this alert?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                    {
                        var result = form.ShowDialog();
                        if (result == DialogResult.OK && form.ReturnValue1 == "1")
                        {
                            parent.SMSAlert_List.Remove(Ref_SMSA);
                        }
                    }
                }
                Grey_In();
                PopulateDGV();
            }
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

        private void button2_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Add_SMS SA = new Add_SMS(parent, null, this.Location, this.Size);
            SA.ShowDialog();
            Grey_In();

            PopulateDGV();
        }


        private void PopulateDGV()
        {
            dataGridView1.Rows.Clear();

            // Sort asset list
            parent.SMSAlert_List = parent.SMSAlert_List.OrderBy(x => x.Time.TimeOfDay).ToList();

            // populate
            foreach (SMSAlert SMSA in parent.SMSAlert_List)
            {
                dataGridView1.Rows.Add(SMSA.Name, SMSA.Time.ToString("hh:mm tt"), (SMSA.Repeat ? "Yes" : "No"), "", "", "");
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                try
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    style.Font = new Font("Segoe UI Symbol", dataGridView1.Font.Size, FontStyle.Regular);
                    row.Cells[3].Style.ApplyStyle(style);
                    row.Cells[3].ToolTipText = "Manage Entry";
                    row.Cells[4].ToolTipText = "Delete Entry";
                }
                catch
                {
                }
            }
        }
    }
}
