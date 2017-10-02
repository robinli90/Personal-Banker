using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;

namespace Financial_Journal
{
    public partial class Calendar_Settings : Form
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
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;
        Calendar parent_Calendar;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Calendar_Settings(Receipt _parent, Calendar par, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            parent_Calendar = par;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }


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

            TFLP.Opacity = 80;

            Apply_Toggle_Style();

            show_payment.Checked = parent.Settings_Dictionary["CALENDAR_TOG_2"] == "1";
            show_AR.Checked = parent.Settings_Dictionary["CALENDAR_TOG_3"] == "1";
            spending_warning.Checked = parent.Settings_Dictionary["CALENDAR_TOG_4"] == "1";
            payment_actions.Checked = parent.Settings_Dictionary["CALENDAR_TOG_5"] == "1";
            toggleSwitch1.Checked = parent.Settings_Dictionary["CALENDAR_TOG_6"] == "1";
            toggleSwitch2.Checked = parent.Settings_Dictionary["CALENDAR_TOG_7"] == "1";
            toggleSwitch3.Checked = parent.Settings_Dictionary["CALENDAR_TOG_8"] == "1";
            toggleSwitch4.Checked = parent.Settings_Dictionary["CALENDAR_TOG_9"] == "1";
            toggleSwitch5.Checked = parent.Settings_Dictionary["SHOW_CALENDAR_ON_LOAD"] == "1";
            toggleSwitch6.Checked = parent.Settings_Dictionary["CALENDAR_TOG_10"] == "1";
        }

        private void Apply_Toggle_Style()
        {

            spending_warning.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            spending_warning.Size = new Size(68, 25);
            spending_warning.OnText = "On";
            spending_warning.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            spending_warning.OnForeColor = Color.White;
            spending_warning.OffText = "Off";
            spending_warning.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            spending_warning.OffForeColor = Color.White;

            show_AR.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            show_AR.Size = new Size(68, 25);
            show_AR.OnText = "On";
            show_AR.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            show_AR.OnForeColor = Color.White;
            show_AR.OffText = "Off";
            show_AR.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            show_AR.OffForeColor = Color.White;

            show_payment.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            show_payment.Size = new Size(68, 25);
            show_payment.OnText = "On";
            show_payment.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            show_payment.OnForeColor = Color.White;
            show_payment.OffText = "Off";
            show_payment.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            show_payment.OffForeColor = Color.White;

            payment_actions.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            payment_actions.Size = new Size(68, 25);
            payment_actions.OnText = "On";
            payment_actions.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            payment_actions.OnForeColor = Color.White;
            payment_actions.OffText = "Off";
            payment_actions.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            payment_actions.OffForeColor = Color.White;

            toggleSwitch1.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            toggleSwitch1.Size = new Size(68, 25);
            toggleSwitch1.OnText = "On";
            toggleSwitch1.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch1.OnForeColor = Color.White;
            toggleSwitch1.OffText = "Off";
            toggleSwitch1.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch1.OffForeColor = Color.White;

            toggleSwitch2.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            toggleSwitch2.Size = new Size(68, 25);
            toggleSwitch2.OnText = "On";
            toggleSwitch2.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch2.OnForeColor = Color.White;
            toggleSwitch2.OffText = "Off";
            toggleSwitch2.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch2.OffForeColor = Color.White;

            toggleSwitch3.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            toggleSwitch3.Size = new Size(68, 25);
            toggleSwitch3.OnText = "On";
            toggleSwitch3.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch3.OnForeColor = Color.White;
            toggleSwitch3.OffText = "Off";
            toggleSwitch3.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch3.OffForeColor = Color.White;

            toggleSwitch4.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            toggleSwitch4.Size = new Size(68, 25);
            toggleSwitch4.OnText = "On";
            toggleSwitch4.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch4.OnForeColor = Color.White;
            toggleSwitch4.OffText = "Off";
            toggleSwitch4.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch4.OffForeColor = Color.White;

            toggleSwitch5.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            toggleSwitch5.Size = new Size(68, 25);
            toggleSwitch5.OnText = "On";
            toggleSwitch5.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch5.OnForeColor = Color.White;
            toggleSwitch5.OffText = "Off";
            toggleSwitch5.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch5.OffForeColor = Color.White;

            toggleSwitch6.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            toggleSwitch6.Size = new Size(68, 25);
            toggleSwitch6.OnText = "On";
            toggleSwitch6.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch6.OnForeColor = Color.White;
            toggleSwitch6.OffText = "Off";
            toggleSwitch6.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch6.OffForeColor = Color.White;

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
            //minimize_button.ForeColor = randomColor;
            //close_button.ForeColor = randomColor;
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }


        private void show_payment_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["CALENDAR_TOG_2"] = show_payment.Checked ? "1" : "0";
            parent_Calendar.Invalidate();
        }

        private void show_AR_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["CALENDAR_TOG_3"] = show_AR.Checked ? "1" : "0";
            parent_Calendar.Invalidate();
        }

        private void spending_warning_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["CALENDAR_TOG_4"] = spending_warning.Checked ? "1" : "0";
            parent_Calendar.Invalidate();
        }

        private void payment_actions_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["CALENDAR_TOG_5"] = payment_actions.Checked ? "1" : "0";
            parent_Calendar.Invalidate();
        }

        private void toggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["CALENDAR_TOG_6"] = toggleSwitch1.Checked ? "1" : "0";
            parent_Calendar.Invalidate();
        }

        private void toggleSwitch2_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["CALENDAR_TOG_7"] = toggleSwitch2.Checked ? "1" : "0";
            parent_Calendar.Invalidate();
        }

        private void toggleSwitch3_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["CALENDAR_TOG_8"] = toggleSwitch3.Checked ? "1" : "0";
            parent_Calendar.Invalidate();
        }

        private void toggleSwitch4_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["CALENDAR_TOG_9"] = toggleSwitch4.Checked ? "1" : "0";
            parent_Calendar.Invalidate();
        }

        private void percentage_box_TextChanged(object sender, EventArgs e)
        {
            if (percentage_box.Text.All(char.IsDigit) && percentage_box.Text.Length > 0)
            {
                if (!spending_warning.Checked) spending_warning.Checked = true;
                parent.Settings_Dictionary["CALENDAR_TOG_PERC"] = percentage_box.Text;
                parent_Calendar.Invalidate();
            }
            else if (percentage_box.Text.Length == 0)
            {
                spending_warning.Checked = false;
            }
            else
            {
                // If letter in SO_number box, do not output and move CARET to end
                try
                {
                    percentage_box.Text = percentage_box.Text.Substring(0, percentage_box.Text.Length - 1);
                    percentage_box.SelectionStart = percentage_box.Text.Length;
                    percentage_box.SelectionLength = 0;
                }
                catch
                { }
            }
        }

        #region handler thread

        private IntPtr secondThreadFormHandle;

        void SecondFormHandleCreated(object sender, EventArgs e)
        {
            Control second = sender as Control;
            secondThreadFormHandle = second.Handle;
            second.HandleCreated -= SecondFormHandleCreated;
        }

        void SecondFormHandleDestroyed(object sender, EventArgs e)
        {
            Control second = sender as Control;
            secondThreadFormHandle = IntPtr.Zero;
            second.HandleDestroyed -= SecondFormHandleDestroyed;
        }

        const int WM_CLOSE = 0x0010;
        [DllImport("User32.dll")]
        extern static IntPtr PostMessage(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam);
        #endregion

        private void toggleSwitch5_CheckedChanged(object sender, EventArgs e)
        {
            parent.Show_Calendar_On_Load = toggleSwitch5.Checked;
            parent.Settings_Dictionary["SHOW_CALENDAR_ON_LOAD"] = toggleSwitch5.Checked ? "1" : "0";
        }

        private void toggleSwitch6_CheckedChanged(object sender, EventArgs e)
        {
            parent.Settings_Dictionary["CALENDAR_TOG_10"] = toggleSwitch6.Checked ? "1" : "0";
            parent_Calendar.Invalidate();
        }

    }
}
