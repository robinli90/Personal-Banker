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
    public partial class Investment_History : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;
        Size Start_Size = new Size();

        Investment Ref_IV;
        DateTime Ref_Date;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Investment_History(Receipt _parent, Investment Ref_IV_, DateTime Ref_Date_, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Ref_IV = Ref_IV_;
            Ref_Date = Ref_Date_;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 3) - (this.Height / 2));
        }


        private void Receipt_Load(object sender, EventArgs e)
        {

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

            dataGridView1.GridColor = Color.White;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(98, 110, 110);
            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);

            double On_Going_Total = 0;

            List<Investment_Transaction> IT = Ref_IV.Get_Transactions_From_Period_Unfiltered(Ref_Date).OrderBy(x => x.Entry_No).ToList();
            double Diff_Amt = IT[0].Principal_Carry_Over - Ref_IV.Get_Matrix_Entry(Ref_Date.AddDays(-1)).Total_Principal_Since;
            On_Going_Total += Diff_Amt;
            dataGridView1.Rows.Add(IT[0].Date.ToShortDateString(), Get_Action(Diff_Amt), "$" + String.Format("{0:0.00}", Math.Abs((decimal)Diff_Amt)));

            for (int i = 1; i < IT.Count; i++)
            {
                Diff_Amt = IT[i].Principal_Carry_Over - IT[i - 1].Principal_Carry_Over;
                On_Going_Total += Diff_Amt;
                dataGridView1.Rows.Add(IT[i].Date.ToShortDateString(), Get_Action(Diff_Amt), "$" + String.Format("{0:0.00}", Math.Abs((decimal)Diff_Amt)));
            }
            dataGridView1.Rows.Add("", "Current Total:", (On_Going_Total < 0 ? "-" : "") + "$" + String.Format("{0:0.00}", Math.Abs((decimal)On_Going_Total)));

            
            // Style colors
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (dataGridView1.Rows.Count > row.Index + 1)
                {
                    if (row.Cells[1].Value.ToString() == "Contribution")
                    {
                        DataGridViewCellStyle style = new DataGridViewCellStyle(); // highlight increase
                        style.BackColor = Color.DarkGreen;
                        row.Cells[2].Style = style;
                    }
                    else
                    {
                        DataGridViewCellStyle style = new DataGridViewCellStyle(); // highlight decrease
                        style.BackColor = Color.Maroon;
                        row.Cells[2].Style = style;
                    }
                }
                if (dataGridView1.Rows.Count == row.Index + 1)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(86, 86, 86); // Current period
                }
            }

            this.Height += dataGridView1.Rows.Count * 22;
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            this.dataGridView1.ClearSelection();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridView1.ClearSelection();
        }

        public string Get_Action(double Amt)
        {
            if (Amt == 0)
            {
                return "No change";
            }
            else if (Amt > 0)
            {
                return "Contribution";
            }
            else
            {
                return "Withdrawal";
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
    }
}
