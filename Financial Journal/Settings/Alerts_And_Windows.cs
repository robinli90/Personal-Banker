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
    public partial class Alerts_And_Windows : Form
    {

        protected override void OnPaint(PaintEventArgs e)
        {
            TFLP.Size = new Size(this.Width - 2, this.Height - 2);
            base.OnPaint(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public Alerts_And_Windows(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            //this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            SetPropertiesForStylesTabSwitches();
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            SetSwitchStates();
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



        public void SetPropertiesForStylesTabSwitches()
        {
            ModernStyleToggleSwitch.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            ModernStyleToggleSwitch.Size = new Size(68, 25);
            ModernStyleToggleSwitch.OnText = "On";
            ModernStyleToggleSwitch.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            ModernStyleToggleSwitch.OnForeColor = Color.White;
            ModernStyleToggleSwitch.OffText = "Off";
            ModernStyleToggleSwitch.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            ModernStyleToggleSwitch.OffForeColor = Color.White;

            toggleSwitch1.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            toggleSwitch1.Size = new Size(68, 25);
            toggleSwitch1.OnText = "On";
            toggleSwitch1.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch1.OnForeColor = Color.White;
            toggleSwitch1.OffText = "Off";
            toggleSwitch1.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch1.OffForeColor = Color.White;

            email_sync.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            email_sync.Size = new Size(68, 25);
            email_sync.OnText = "On";
            email_sync.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            email_sync.OnForeColor = Color.White;
            email_sync.OffText = "Off";
            email_sync.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            email_sync.OffForeColor = Color.White;

            arp_alerts.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            arp_alerts.Size = new Size(68, 25);
            arp_alerts.OnText = "On";
            arp_alerts.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            arp_alerts.OnForeColor = Color.White;
            arp_alerts.OffText = "Off";
            arp_alerts.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            arp_alerts.OffForeColor = Color.White;

            toggleSwitch2.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            toggleSwitch2.Size = new Size(68, 25);
            toggleSwitch2.OnText = "On";
            toggleSwitch2.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch2.OnForeColor = Color.White;
            toggleSwitch2.OffText = "Off";
            toggleSwitch2.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch2.OffForeColor = Color.White;

            sneak_peek.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            sneak_peek.Size = new Size(68, 25);
            sneak_peek.OnText = "On";
            sneak_peek.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            sneak_peek.OnForeColor = Color.White;
            sneak_peek.OffText = "Off";
            sneak_peek.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            sneak_peek.OffForeColor = Color.White;

            toggleSwitch3.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            toggleSwitch3.Size = new Size(68, 25);
            toggleSwitch3.OnText = "On";
            toggleSwitch3.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch3.OnForeColor = Color.White;
            toggleSwitch3.OffText = "Off";
            toggleSwitch3.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch3.OffForeColor = Color.White;
        }

        public void SetSwitchStates()
        {
            ModernStyleToggleSwitch.Checked = parent.Show_Calendar_On_Load;
            toggleSwitch1.Checked = parent.Alerts_On;
            email_sync.Checked = parent.Settings_Dictionary["CALENDAR_EMAIL_SYNC"] == "1";
            arp_alerts.Checked = parent.Settings_Dictionary["ARP_ALERTS"] == "1";
            toggleSwitch2.Checked = parent.Settings_Dictionary["EXPENSE_ALERT"] == "1";
            sneak_peek.Checked = parent.Settings_Dictionary["SNEAK_PEAK"] == "1";
            toggleSwitch3.Checked = parent.Settings_Dictionary["START_MINIMIZED"] == "1";
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

        private void ModernStyleToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            parent.Show_Calendar_On_Load = ModernStyleToggleSwitch.Checked;
            parent.Settings_Dictionary["SHOW_CALENDAR_ON_LOAD"] = ModernStyleToggleSwitch.Checked ? "1" : "0";
        }

        private void toggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            parent.Alerts_On = toggleSwitch1.Checked;
            parent.Settings_Dictionary["ALERTS_ACTIVE"] =  toggleSwitch1.Checked ? "1" : "0";
        }

        private void email_sync_CheckedChanged(object sender, EventArgs e)
        {
            if (parent.Settings_Dictionary["PERSONAL_EMAIL"].Length < 5 && email_sync.Checked)
            {
                email_sync.Checked = false;
                this.Close();
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: No emails have been set. Please setup an email", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
                Personal_Information PI = new Personal_Information(parent);
                PI.ShowDialog();
            }
            parent.Settings_Dictionary["CALENDAR_EMAIL_SYNC"] = email_sync.Checked ? "1" : "0";
        }

        private void arp_alerts_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["ARP_ALERTS"] = arp_alerts.Checked ? "1" : "0";
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void toggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["EXPENSE_ALERT"] = toggleSwitch2.Checked ? "1" : "0";
        }

        private void sneak_peak_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["SNEAK_PEAK"] = sneak_peek.Checked ? "1" : "0";
        }

        private void toggleSwitch3_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["START_MINIMIZED"] = toggleSwitch3.Checked ? "1" : "0";
        }
    }
}
