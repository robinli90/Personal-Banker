using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Financial_Journal
{
    public partial class Add_SMS : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
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
        SMSAlert Ref_SMSAlert;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Add_SMS(Receipt _parent, SMSAlert smsAlert = null, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
            label3.Text = DateTime.Now.ToString("hh:mm tt");

            Ref_SMSAlert = smsAlert;

        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            toggleSwitch3.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            toggleSwitch3.Size = new Size(68, 25);
            toggleSwitch3.OnText = "On";
            toggleSwitch3.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch3.OnForeColor = Color.White;
            toggleSwitch3.OffText = "Off";
            toggleSwitch3.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch3.OffForeColor = Color.White;

            // Clock setup
            timePickerPanel1.timePicker.ClockMenu.ClockButtonOK.Click += TimeOK;
            timePickerPanel1.timePicker.ClockMenu.ClockButtonCancel.Click += TimeCancel;
            timePickerPanel1.timePicker.ClockMenu.Closed += TimeClose;

            if (Ref_SMSAlert != null)
            {
                smsMessage.Text = Ref_SMSAlert.Name;
                label3.Text = Ref_SMSAlert.Time.ToString("hh:mm tt");
                toggleSwitch3.Checked = Ref_SMSAlert.Repeat;
                label28.Text = "SAVE ALERT";
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

            Time_Chooser = DateTime.Now;

            if (Ref_SMSAlert != null)
            {
                // SET TIME
                Time_Chooser = Time_Chooser.Date + Ref_SMSAlert.Time.TimeOfDay;
            }

        }


        private void button6_Click(object sender, EventArgs e)
        {
            if (Ref_SMSAlert != null)
            {
                timePickerPanel1.timePicker.Value = Ref_SMSAlert.Time;
            }
            else
            {
                timePickerPanel1.timePicker.Value = Time_Chooser;
            }

            Grey_Out();

            timePickerPanel1.timePicker.ClockMenu.Set_To_Hour();

            timePickerPanel1.timePicker.ClockMenu.Show(this,
                    (this.Size.Width / 2 - 98),// - timePickerPanel1.timePicker.ClockMenu.Width / 2),
                    (this.Size.Height / 2 - 145) //- timePickerPanel1.timePicker.ClockMenu.Height / 2)
                );
            timePickerPanel1.timePicker.ClockMenu.Visible = true;
            SetWindowPos(timePickerPanel1.timePicker.ClockMenu.Handle, (IntPtr)(-1), 0, 0, 0, 0,
                                                            SWP_NOACTIVATE | SWP_NOMOVE | SWP_NOSIZE);
        }


        private void TimeOK(object sender, EventArgs e)
        {
            Time_Chooser = timePickerPanel1.timePicker.Value;
            label3.Text = Time_Chooser.ToString("hh:mm tt");
            Grey_In();
        }

        private void TimeCancel(object sender, EventArgs e)
        {
            Grey_In();
        }

        private void TimeClose(object sender, ToolStripDropDownClosedEventArgs e)
        {
            Grey_In();
        }

        private DateTime Time_Chooser;

        private void TimePicker_Choosed(object sender, EventArgs e)
        {
            Time_Chooser = timePickerPanel1.timePicker.Value;
            Grey_In();
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


        #region Extension .dll function

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOACTIVATE = 0x0010;
        private const int SWP_NOMOVE = 0x0002;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,
                                                 int X, int Y, int cx, int cy, uint uFlags);

        enum idCursor
        {
            HAND = 32649,
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr LoadCursor(IntPtr hInstance, idCursor cursor);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern bool PlaySound(string pszSound, UIntPtr hmod, uint fdwSound);
        [Flags]
        public enum sndFlags
        {
            SND_SYNC = 0x0000,
            SND_ASYNC = 0x0001,
            SND_NODEFAULT = 0x0002,
            SND_LOOP = 0x0008,
            SND_NOSTOP = 0x0010,
            SND_NOWAIT = 0x00002000,
            SND_FILENAME = 0x00020000,
            SND_RESOURCE = 0x00040004
        }

        #endregion

        private void button3_Click(object sender, EventArgs e)
        {
            if (smsMessage.Text.Length > 0)
            {
                if (Ref_SMSAlert != null)
                {
                    parent.SMSAlert_List.Remove(Ref_SMSAlert);
                }

                parent.SMSAlert_List.Add(new SMSAlert()
                                            {
                                                Name = smsMessage.Text,
                                                Time = Time_Chooser,
                                                Repeat = toggleSwitch3.Checked,
                                                IUO_Flag = true
                                            });

                this.Close();
            }
        }

    }
}
