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
    public partial class Quick_Links : Form
    {

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        int Open_Height = 0;

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
        private Size Closed_Size = new Size(305, 111);

        public Quick_Links(Receipt _parent, Point g = new Point(), Size s = new Size())
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

            #region checkbox check state set
            quick_en.Checked = parent.Settings_Dictionary["QL_ENABLED"] == "1";
            agenda.Checked = parent.Settings_Dictionary["QL_AGENDA"] == "1";
            calendar.Checked = parent.Settings_Dictionary["QL_CALENDAR"] == "1";
            purchase.Checked = parent.Settings_Dictionary["QL_VIEW_PURCHASES"] == "1";
            deposit.Checked = parent.Settings_Dictionary["QL_DEPOSIT_PAY"] == "1";
            payments.Checked = parent.Settings_Dictionary["QL_MANAGE_PAYMENT"] == "1";
            onlinepurch.Checked = parent.Settings_Dictionary["QL_VIEW_ONLINE"] == "1";
            hobby.Checked = parent.Settings_Dictionary["QL_MANAGE_HOBBY"] == "1";
            lookup.Checked = parent.Settings_Dictionary["QL_LOOKUP"] == "1";
            sneak_peek.Checked = parent.Settings_Dictionary["QL_SNEAK_PEEK"] == "1";
            contacts.Checked = parent.Settings_Dictionary["QL_CONTACTS"] == "1";
            smsalert.Checked = parent.Settings_Dictionary["QL_SMS_ALERT"] == "1";
            shoppinglist.Checked = parent.Settings_Dictionary["QL_SHOPPING_LIST"] == "1";
            mobileSync.Checked = parent.Settings_Dictionary["QL_MOBILE_SYNC"] == "1";
            budgetSwitch.Checked = parent.Settings_Dictionary["QL_BUDGET"] == "1";
            #endregion

            #region style checkboxes
            quick_en.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            quick_en.Size = new Size(68, 25);
            quick_en.OnText = "On";
            quick_en.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            quick_en.OnForeColor = Color.White;
            quick_en.OffText = "Off";
            quick_en.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            quick_en.OffForeColor = Color.White;

            agenda.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            agenda.Size = new Size(68, 25);
            agenda.OnText = "On";
            agenda.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            agenda.OnForeColor = Color.White;
            agenda.OffText = "Off";
            agenda.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            agenda.OffForeColor = Color.White;


            calendar.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            calendar.Size = new Size(68, 25);
            calendar.OnText = "On";
            calendar.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            calendar.OnForeColor = Color.White;
            calendar.OffText = "Off";
            calendar.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            calendar.OffForeColor = Color.White;

            sneak_peek.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            sneak_peek.Size = new Size(68, 25);
            sneak_peek.OnText = "On";
            sneak_peek.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            sneak_peek.OnForeColor = Color.White;
            sneak_peek.OffText = "Off";
            sneak_peek.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            sneak_peek.OffForeColor = Color.White;

            deposit.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            deposit.Size = new Size(68, 25);
            deposit.OnText = "On";
            deposit.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            deposit.OnForeColor = Color.White;
            deposit.OffText = "Off";
            deposit.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            deposit.OffForeColor = Color.White;

            purchase.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            purchase.Size = new Size(68, 25);
            purchase.OnText = "On";
            purchase.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            purchase.OnForeColor = Color.White;
            purchase.OffText = "Off";
            purchase.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            purchase.OffForeColor = Color.White;

            payments.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            payments.Size = new Size(68, 25);
            payments.OnText = "On";
            payments.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            payments.OnForeColor = Color.White;
            payments.OffText = "Off";
            payments.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            payments.OffForeColor = Color.White;

            onlinepurch.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            onlinepurch.Size = new Size(68, 25);
            onlinepurch.OnText = "On";
            onlinepurch.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            onlinepurch.OnForeColor = Color.White;
            onlinepurch.OffText = "Off";
            onlinepurch.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            onlinepurch.OffForeColor = Color.White;

            hobby.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            hobby.Size = new Size(68, 25);
            hobby.OnText = "On";
            hobby.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            hobby.OnForeColor = Color.White;
            hobby.OffText = "Off";
            hobby.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            hobby.OffForeColor = Color.White;

            lookup.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            lookup.Size = new Size(68, 25);
            lookup.OnText = "On";
            lookup.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            lookup.OnForeColor = Color.White;
            lookup.OffText = "Off";
            lookup.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            lookup.OffForeColor = Color.White;

            contacts.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            contacts.Size = new Size(68, 25);
            contacts.OnText = "On";
            contacts.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            contacts.OnForeColor = Color.White;
            contacts.OffText = "Off";
            contacts.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            contacts.OffForeColor = Color.White;

            smsalert.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            smsalert.Size = new Size(68, 25);
            smsalert.OnText = "On";
            smsalert.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            smsalert.OnForeColor = Color.White;
            smsalert.OffText = "Off";
            smsalert.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            smsalert.OffForeColor = Color.White;

            shoppinglist.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            shoppinglist.Size = new Size(68, 25);
            shoppinglist.OnText = "On";
            shoppinglist.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            shoppinglist.OnForeColor = Color.White;
            shoppinglist.OffText = "Off";
            shoppinglist.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            shoppinglist.OffForeColor = Color.White;

            mobileSync.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            mobileSync.Size = new Size(68, 25);
            mobileSync.OnText = "On";
            mobileSync.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            mobileSync.OnForeColor = Color.White;
            mobileSync.OffText = "Off";
            mobileSync.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            mobileSync.OffForeColor = Color.White;

            budgetSwitch.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            budgetSwitch.Size = new Size(68, 25);
            budgetSwitch.OnText = "On";
            budgetSwitch.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            budgetSwitch.OnForeColor = Color.White;
            budgetSwitch.OffText = "Off";
            budgetSwitch.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            budgetSwitch.OffForeColor = Color.White;

            #endregion

            if (!quick_en.Checked) this.Size = Closed_Size;
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

        private void email_sync_CheckedChanged(object sender, EventArgs e)
        {
            if (quick_en.Checked) 
            {
                this.Size = Start_Size;
            }   
            else 
            {
                this.Size = Closed_Size;
            }

            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_ENABLED"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void hobby_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_MANAGE_HOBBY"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void onlinepurch_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_VIEW_ONLINE"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void payments_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_MANAGE_PAYMENT"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void purchase_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_VIEW_PURCHASES"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void deposit_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_DEPOSIT_PAY"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void calendar_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_CALENDAR"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void agenda_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_AGENDA"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void lookup_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_LOOKUP"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void sneak_peek_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_SNEAK_PEEK"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void contacts_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_CONTACTS"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void smsalert_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_SMS_ALERT"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void shoppinglist_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_SHOPPING_LIST"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void mobileSync_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_MOBILE_SYNC"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void budgetSwitch_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["QL_BUDGET"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }
    }
}
