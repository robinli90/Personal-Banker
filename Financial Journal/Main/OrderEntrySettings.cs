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
    public partial class OrderEntrySettings : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public OrderEntrySettings(Receipt _parent, Point g = new Point(), Size s = new Size())
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

            ModernStyleToggleSwitch.Checked = parent.Settings_Dictionary["OE_AUTO_POPULATE"] == "1";

            ModernStyleToggleSwitch.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            ModernStyleToggleSwitch.Size = new Size(68, 25);
            ModernStyleToggleSwitch.OnText = "On";
            ModernStyleToggleSwitch.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            ModernStyleToggleSwitch.OnForeColor = Color.White;
            ModernStyleToggleSwitch.OffText = "Off";
            ModernStyleToggleSwitch.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            ModernStyleToggleSwitch.OffForeColor = Color.White;

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
            parent.Show_Calendar_On_Load = ModernStyleToggleSwitch.Checked;
            parent.Settings_Dictionary["OE_AUTO_POPULATE"] = ModernStyleToggleSwitch.Checked ? "1" : "0";
        }
    }
}
