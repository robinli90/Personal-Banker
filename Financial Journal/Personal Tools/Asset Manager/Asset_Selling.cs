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
    public partial class Asset_Selling : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;
        Asset_Item Ref_AI;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Asset_Selling(Receipt _parent, Asset_Item AI, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            Ref_AI = AI;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            label2.Text = (AI.Name.Length > 18 ? AI.Name.Substring(0, 18) + "..." : AI.Name) + " is being removed";
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = DateTime.Now;

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

        private void sold_CheckedChanged(object sender, EventArgs e)
        {
            if (sold.Checked)
            {
                this.Height = 198;
                disposed.Checked = !sold.Checked;
            }
            else
            {
                this.Height = 169;
            }
        }

        private void disposed_CheckedChanged(object sender, EventArgs e)
        {
            if (disposed.Checked)
            {
                this.Height = 169;
                sold.Checked = !disposed.Checked;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Enabled = !dateTimePicker1.Enabled;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (disposed.Checked || sold.Checked)
            {
                if (disposed.Checked)
                {
                    Ref_AI.Selling_Amount = 0;
                    Ref_AI.Remove_Date = dateTimePicker1.Value;
                    this.Close();
                }
                else if (sold.Checked)
                {
                    if (item_price.Text.Length > 1)
                    {
                        Ref_AI.Selling_Amount = Convert.ToDouble(item_price.Text.Substring(1));
                        Ref_AI.Remove_Date = dateTimePicker1.Value;
                        this.Close();
                    }
                }
            }
            else
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "No reason chosen", true, -26, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
        }

        private void item_price_TextChanged(object sender, EventArgs e)
        {
            parent.textBox6_TextChanged(sender, e);
        }
    }
}
