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
    public partial class Tracking : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
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

        Shipment_Tracking Ref_ST = new Shipment_Tracking();
        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public Tracking(Receipt _parent, bool Enable_Actions = true, string ref_order = "", Point g = new Point(), Size s = new Size())
        {
            //this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Ref_ST = parent.Tracking_List.FirstOrDefault(x => x.Ref_Order_Number == (ref_order.Length > 0 ? ref_order : "999999999"));
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            if (!Enable_Actions)
            {
                this.Height -= 115;
                viewbutton.Enabled = false;
                receivebutton.Enabled = false;
                deletebutton.Enabled = false;
            }
            this.Location = new Point(g.X + s.Width / 2 - this.Width / 2, g.Y + s.Height / 2 - this.Height / 2);
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            dateTimePicker1.Value = Ref_ST.Expected_Date;
            item_desc.Text = Ref_ST.Tracking_Number;

            item_desc.Enabled = !(Ref_ST.Tracking_Number.Length > 0);
            dateTimePicker1.Enabled = !(Ref_ST.Tracking_Number.Length > 0);

            memo_button.Enabled = (Ref_ST.Tracking_Number.Length > 0);
            trackbutton.Enabled = (Ref_ST.Tracking_Number.Length > 0);
            button3.Enabled = (Ref_ST.Tracking_Number.Length > 0);
            button3.Visible = (Ref_ST.Tracking_Number.Length > 0);

            req_authentication.Checked = Ref_ST.Alert_Active;
            toggleSwitch1.Checked = Ref_ST.Email_Active;

            if (Ref_ST.Status == 0)
            {
                label15.Text = "RECEIVED";
                label15.ForeColor = Color.LightGreen;
                label15.Left -= 3;
            }

            receivebutton.Enabled = Ref_ST.Status == 1;
            deletebutton.Enabled = Ref_ST.Status == 1;

            Apply_Toggle_Style();

            Form_Loaded = true;


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

        private void deletebutton_Click(object sender, EventArgs e)
        {
            parent.Tracking_List.Remove(Ref_ST);
            this.Close();
            this.Dispose();
        }

        private void receivebutton_Click(object sender, EventArgs e)
        {
            Ref_ST.Status = 0;
            receivebutton.Enabled = false;
            label15.Text = "RECEIVED";
            label15.ForeColor = Color.LightGreen;
            label15.Left -= 3;
            Ref_ST.Received_Date = DateTime.Now;
        }

        private void req_authentication_CheckedChanged(object sender, EventArgs e)
        {
            Ref_ST.Alert_Active = req_authentication.Checked;
        }

        private void toggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            Ref_ST.Email_Active = toggleSwitch1.Checked;
        }


        private void Apply_Toggle_Style()
        {
            req_authentication.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            req_authentication.Size = new Size(68, 25);
            req_authentication.OnText = "On";
            req_authentication.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            req_authentication.OnForeColor = Color.White;
            req_authentication.OffText = "Off";
            req_authentication.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            req_authentication.OffForeColor = Color.White;

            toggleSwitch1.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            toggleSwitch1.Size = new Size(68, 25);
            toggleSwitch1.OnText = "On";
            toggleSwitch1.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch1.OnForeColor = Color.White;
            toggleSwitch1.OffText = "Off";
            toggleSwitch1.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            toggleSwitch1.OffForeColor = Color.White;
        }

        private void memo_button_Click(object sender, EventArgs e)
        {
            item_desc.Enabled = !item_desc.Enabled;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Enabled = !dateTimePicker1.Enabled;
        }

        private void item_desc_TextChanged(object sender, EventArgs e)
        {
            memo_button.Visible = item_desc.Text.Length > 0;
            memo_button.Enabled = item_desc.Text.Length > 0;
            viewbutton.Enabled = item_desc.Text.Length > 0;
            Ref_ST.Tracking_Number = item_desc.Text;
        }

        bool Form_Loaded = false;

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (Form_Loaded)
            {
                button3.Visible = true;
                button3.Enabled = true;
                Ref_ST.Expected_Date = dateTimePicker1.Value;
            }
        }

        //view order
        private void viewbutton_Click(object sender, EventArgs e)
        {
            Order Ref_Order = parent.Order_List.First(x => x.OrderID == Ref_ST.Ref_Order_Number);
            Grey_Out();
            Receipt_Report RP = new Receipt_Report(parent, Ref_Order, new Point(Cursor.Position.X - this.Width/2, Cursor.Position.Y - this.Height/2), null, false, this.Location, this.Size);
            RP.ShowDialog();
            Grey_In();
        }

        // track
        private void button1_Click(object sender, EventArgs e)
        {
            Ref_ST.Show_Tracking_Information();
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
    }
}
