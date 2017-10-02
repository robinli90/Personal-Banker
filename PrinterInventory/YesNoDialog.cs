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
    public partial class YesNoDialog : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.RefreshMain();
            //parent.Grey_In();
            base.OnFormClosing(e);
            
        }

        Main parent;
        public int returnValue = -1;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public YesNoDialog(Main _parent, string labelText, Point g = new Point(), Size s = new Size(), string yesButtonText="Yes", string noButtonText="No")
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
            parent.Grey_Out();

            label1.Text = labelText;
            yesbutton.Text = yesButtonText;
            nobutton.Text = noButtonText;

            
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

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

        private void yesbutton_Click(object sender, EventArgs e)
        {
            this.returnValue = 1;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void nobutton_Click(object sender, EventArgs e)
        {
            this.returnValue = 0;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
