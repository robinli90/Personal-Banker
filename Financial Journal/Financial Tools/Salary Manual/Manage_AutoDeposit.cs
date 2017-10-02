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
    public partial class Manage_AutoDeposit : Form
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
        CustomIncome Ref_CI;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Manage_AutoDeposit(Receipt _parent, CustomIncome CI_, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Ref_CI = CI_;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

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

            depositTo.Items.Add("None");
            depositTo.Items.AddRange(parent.Payment_List.Select(x => x.Get_Long_String()).ToArray());

            // Style the switch
            ModernStyleToggleSwitch.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            ModernStyleToggleSwitch.Size = new Size(68, 25);
            ModernStyleToggleSwitch.OnText = "On";
            ModernStyleToggleSwitch.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            ModernStyleToggleSwitch.OnForeColor = Color.White;
            ModernStyleToggleSwitch.OffText = "Off";
            ModernStyleToggleSwitch.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            ModernStyleToggleSwitch.OffForeColor = Color.White;

            if (Ref_CI.Deposit_Account.Length == 0)
            {
                depositTo.Enabled = true;
                ModernStyleToggleSwitch.Checked = false;
                depositTo.SelectedIndex = 0;
            }
            else
            {
                depositTo.Text = Ref_CI.Deposit_Account;
                ModernStyleToggleSwitch.Checked = true;
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
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        private void ModernStyleToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            depositTo.Enabled = !ModernStyleToggleSwitch.Checked;

            if (depositTo.Text != "None" && ModernStyleToggleSwitch.Checked)
                Ref_CI.Deposit_Account = depositTo.Text;
            else
            {
                ModernStyleToggleSwitch.CheckedChanged -= ModernStyleToggleSwitch_CheckedChanged;
                ModernStyleToggleSwitch.Checked = false;
                Ref_CI.Deposit_Account = "";
                ModernStyleToggleSwitch.CheckedChanged += ModernStyleToggleSwitch_CheckedChanged;
            }
        }

        private void depositTo_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
