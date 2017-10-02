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
    public partial class Salary_Manual : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
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
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public Salary_Manual(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            //this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        FadeControl TFLP;

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

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

            TFLP.Opacity = 65;

        }

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

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Salary_Calculator SC = new Salary_Calculator(parent, false, this.Location, this.Size);
            SC.ShowDialog();
            Grey_In();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Grey_Out();
            this.Visible = false;
            parent.Settings_Dictionary["INCOME_MANUAL"] = "0";
            Salary_Calculator SC = new Salary_Calculator(parent, false, this.Location, this.Size);
            SC.ShowDialog();
            Grey_In();
            SC.Activate();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Manage_Companies MC = new Manage_Companies(parent, new Point(this.Location.X, this.Location.Y), this.Size);
            MC.ShowDialog();
            Grey_In();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Grey_Out();
            if (parent.Income_Company_List.Where(x => x.Default).ToList().Count > 0)
            {
                Deposit_Paycheck DP = new Deposit_Paycheck(parent, new Point(this.Location.X, this.Location.Y), this.Size);
                DP.ShowDialog();
            }
            else
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: No default company has been set", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
                Manage_Companies MC = new Manage_Companies(parent, new Point(this.Location.X, this.Location.Y), this.Size);
                MC.ShowDialog();
            }
            Grey_In();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Grey_Out();
            if (parent.Income_Company_List.Where(x => x.Default).ToList().Count > 0)
            {
                Manage_Prev_Period_Range_Selector MPPRS = new Manage_Prev_Period_Range_Selector(parent, new Point(this.Location.X, this.Location.Y), this.Size);
                MPPRS.ShowDialog();
            }
            else
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: No default company has been set", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
                Manage_Companies MC = new Manage_Companies(parent, new Point(this.Location.X, this.Location.Y), this.Size);
                MC.ShowDialog();
            }
            Grey_In();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }
    }
}
