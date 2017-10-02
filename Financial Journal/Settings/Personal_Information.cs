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
    public partial class Personal_Information : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public Personal_Information(Receipt _parent, Point g = new Point(), Size s = new Size())
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

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            email.Text = parent.Settings_Dictionary["PERSONAL_EMAIL"];
            firstname.Text = parent.Settings_Dictionary["PERSONAL_FIRST_NAME"];
            lastname.Text = parent.Settings_Dictionary["PERSONAL_LAST_NAME"];
            //if (parent.Settings_Dictionary["PERSONAL_EMAIL"].Length > 5) email.Enabled = false;
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

        private void firstname_TextChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["PERSONAL_FIRST_NAME"] = firstname.Text;
        }

        private void lastname_TextChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["PERSONAL_LAST_NAME"] = lastname.Text;
        }

        private void email_TextChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["PERSONAL_EMAIL"] = email.Text;
        }
    }
}
