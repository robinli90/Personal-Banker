using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace AlertBox
{
    public partial class Alert_Box : Form
    {
        private void Form_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.Visible = false;
            this.Dispose();
            this.Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Alert_Box(string message, string r, string g, string b)
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);

            InitializeComponent();

            Set_Form_Color(Color.FromArgb(Convert.ToInt32(r), Convert.ToInt32(g), Convert.ToInt32(b)));
            //this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
            label1.Text = message;

            Taskbar tB = new Taskbar();

            current_y = current_y + (tB.AutoHide ? 35 : 0);

            this.Location = new System.Drawing.Point(Convert.ToInt32(current_x), Convert.ToInt32(current_y) );
            InitializeComponent();
            up_direction_tick.Interval = 5;
            up_direction_tick.Enabled = true;
            up_direction_tick.Tick += new EventHandler(traverse_alert);
            //this.SetStyle(
            //    ControlStyles.AllPaintingInWmPaint |
            //    ControlStyles.UserPaint |
            //    ControlStyles.DoubleBuffer, true);
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

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
            textBox4.BackColor = randomColor;
        }

        double current_x = get_x() - 237; 
        double current_y = get_y() - 69;
        double direction = -1;
        double traverse_factor = 0.01;
        int traverse_count = 0;
        bool alert_on = true;
        System.Windows.Forms.Timer up_direction_tick = new System.Windows.Forms.Timer();

        // Return screen x
        public static int get_x()
        {
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            return resolution.Width;
        }

        // Return screen y
        public static int get_y()
        {
            Rectangle resolution = Screen.PrimaryScreen.Bounds;
            return resolution.Height;
        }


        // Entire scrolling functionality
        private void traverse_alert(object sender, EventArgs e)
        {
            if (alert_on)
            {
                traverse_count++;
                current_y = current_y + (direction) * (2 * traverse_factor);
                this.Location = new System.Drawing.Point(Convert.ToInt32(current_x), Convert.ToInt32(current_y));
            }
            if (traverse_count < 10)
            {
                traverse_factor = 1.5;
            }
            else if (traverse_count < 17)
            {
                traverse_factor = 2.2;
            }
            InitializeComponent();

            if (traverse_count > 19)
            {
                alert_on = false;
                up_direction_tick.Enabled = false;
                System.Windows.Forms.Timer down_direction_tick = new System.Windows.Forms.Timer();
                up_direction_tick.Interval = 10000;
                up_direction_tick.Enabled = true;
                up_direction_tick.Tick += new EventHandler(Close);
            }
        }

        // Close
        private void Close(object sender, EventArgs e)
        {
            this.Close();
        }


        private void alert_mouse_down(object sender, EventArgs e)
        {
        }
    }
}

