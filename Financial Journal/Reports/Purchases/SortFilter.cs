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
    public partial class SortFilter : Form
    {

        Receipt parent;
        private string presetSortMode = "";

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public SortFilter(Receipt _parent, string _presetSortMode="", Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
            presetSortMode = _presetSortMode;
        }

        public string sortMode = "";

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            sortBox.Items.Add("Date Ascending");
            sortBox.Items.Add("Date Descending");
            sortBox.Items.Add("Price Ascending");
            sortBox.Items.Add("Price Descending");
            sortBox.Items.Add("Location Ascending");
            sortBox.Items.Add("Location Descending");
            sortBox.Items.Add("Payment Ascending");
            sortBox.Items.Add("Payment Descending");
            //sortBox.Items.Add(" Ascending");
            //sortBox.Items.Add(" Descending");

            if (presetSortMode.Length > 0)
            {
                sortBox.Text = presetSortMode;
            }
            else
            {
                sortBox.SelectedIndex = 1;
            }

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

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            sortMode = sortBox.Text;
            this.Dispose();
            this.Close();
        }

        public void Set_Form_Color(Color randomColor)
        {
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; 
        }

        private void sortBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            sortMode = sortBox.Text;
        }
    }
}
