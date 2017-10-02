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
    public partial class Customized_Settings : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;
        int Start_Location_Offset = 30;

        public Customized_Settings(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // load current status speed

            trackBar3.Value = (int)((Convert.ToDouble(parent.statusResetSeconds - 4) / 4) * 100);
            trackBar1.Value = (int)((Convert.ToDouble(parent.weatherResetSeconds - 8) / 10) * 100);
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

        private void Add_button_Click(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            parent.statusResetSeconds = (int)Math.Round(4 + (4 * (float)(Convert.ToDouble(trackBar3.Value) / 100)));
            parent.Settings_Dictionary["STATUSBAR_SETTINGS_INFO"] = parent.statusResetSeconds.ToString();
            parent.seconds = 0;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            parent.weatherResetSeconds = (int)Math.Round(8 + (10 * (float)(Convert.ToDouble(trackBar1.Value) / 100)));
            parent.Settings_Dictionary["STATUSBAR_SETTINGS_WEATHER"] = parent.weatherResetSeconds.ToString();
            parent.seconds = 0;
        }

        private void excel_button_Click(object sender, EventArgs e)
        {
            DialogResult result = colorDialog1.ShowDialog();
            // See if user pressed ok.
            if (result == DialogResult.OK)
            {
                Color Picked_Color = colorDialog1.Color;
                // Set form background to the selected color.
                parent.Set_Form_Color(Picked_Color);
                if (parent.Settings_Dictionary.ContainsKey("APP_SETTING_COLOR"))
                {
                    parent.Settings_Dictionary["APP_SETTING_COLOR"] = System.Drawing.ColorTranslator.ToHtml(Picked_Color);
                }
                else
                {
                    parent.Settings_Dictionary.Add("APP_SETTING_COLOR", System.Drawing.ColorTranslator.ToHtml(Picked_Color));
                }
                Set_Form_Color(Picked_Color);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Color Picked_Color = SystemColors.HotTrack;
            // Set form background to the selected color.
            parent.Set_Form_Color(Picked_Color);
            if (parent.Settings_Dictionary.ContainsKey("APP_SETTING_COLOR"))
            {
                parent.Settings_Dictionary["APP_SETTING_COLOR"] = System.Drawing.ColorTranslator.ToHtml(Picked_Color);
            }
            else
            {
                parent.Settings_Dictionary.Add("APP_SETTING_COLOR", System.Drawing.ColorTranslator.ToHtml(Picked_Color));
            }
            Set_Form_Color(Picked_Color);
        }
    }
}
