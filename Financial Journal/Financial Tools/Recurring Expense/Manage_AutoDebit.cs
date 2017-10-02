using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;

namespace Financial_Journal
{
    public partial class Manage_AutoDebit : Form
    {
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
        Expenses Ref_Expense = new Expenses();

        public Manage_AutoDebit(Receipt _parent, ref Expenses expense, Point g = new Point(), Size s = new Size())
        {
            Ref_Expense = expense;
            //this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            this.Location = new Point(g.X + s.Width / 2 - this.Width / 2, g.Y + s.Height / 2 - this.Height / 2);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);

            ModernStyleToggleSwitch.Checked = Ref_Expense.AutoDebit == "1";
            ModernStyleToggleSwitch.MouseEnter += new EventHandler(mouseenter);

        }

        bool Toggle_Switch = false;

        List<Payment> List_Of_Payments = new List<Payment>();

        private void mouseenter(object sender, EventArgs e)
        {
            Toggle_Switch = true;
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            fromDate.Value = DateTime.Now;

            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            label5.Text = "Manage Autodebit for " + Ref_Expense.Expense_Name;

            // Style the switch
            ModernStyleToggleSwitch.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            ModernStyleToggleSwitch.Size = new Size(68, 25);
            ModernStyleToggleSwitch.OnText = "On";
            ModernStyleToggleSwitch.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            ModernStyleToggleSwitch.OnForeColor = Color.White;
            ModernStyleToggleSwitch.OffText = "Off";
            ModernStyleToggleSwitch.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            ModernStyleToggleSwitch.OffForeColor = Color.White;

            //ModernStyleToggleSwitch.Enabled = Ref_Expense.AutoDebit != "1";

            payFrom.Items.Add("None");
            parent.Payment_List.ForEach(x => List_Of_Payments.Add(x));
            List_Of_Payments.ForEach(x => payFrom.Items.Add(x.Company + " (xx-" + x.Last_Four + ")"));

            if (Ref_Expense.AutoDebit == "1")
            {
                payFrom.Text = Ref_Expense.Payment_Company + " (xx-" + Ref_Expense.Payment_Last_Four + ")";
                fromDate.Value = Ref_Expense.Last_Pay_Date;
                payFrom.Enabled = false;
                datePay.Enabled = false;
            }

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

        private void datePay_Click(object sender, EventArgs e)
        {
            fromDate.Enabled = !fromDate.Enabled;
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

        private void ModernStyleToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            if (Toggle_Switch)
            {
                if (payFrom.Text.Length > 4 && ModernStyleToggleSwitch.Checked)
                {
                    Grey_Out();
                    using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to set this Auto Debit?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                    {
                        var result21 = form1.ShowDialog();
                        if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                        {
                            Payment ref_payment = List_Of_Payments[payFrom.Items.IndexOf(payFrom.Text) - 1];
                            Ref_Expense.AutoDebit = "1";
                            Ref_Expense.Last_Pay_Date = (fromDate.Value < Ref_Expense.Expense_Start_Date ? (fromDate.Value > DateTime.Now ? DateTime.Now : Ref_Expense.Expense_Start_Date) : fromDate.Value); // cannot be before expense start date
                            Ref_Expense.Payment_Company = ref_payment.Company;
                            Ref_Expense.Payment_Last_Four = ref_payment.Last_Four;
                            ref_payment = parent.Payment_List.FirstOrDefault(x => x == ref_payment);
                            Ref_Expense.Process_Payments(parent, ref ref_payment);
                            datePay.PerformClick();

                        }
                        else
                        {
                            Toggle_Switch = false;
                            ModernStyleToggleSwitch.Checked = false;
                        }
                    }
                    Grey_In();
                }
                else if (payFrom.Text.Length <= 4 && ModernStyleToggleSwitch.Checked)
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Missing payment location", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                    ModernStyleToggleSwitch.Checked = false;
                }
                else if (!ModernStyleToggleSwitch.Checked && Ref_Expense.AutoDebit == "1")
                {
                    Grey_Out();
                    using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to turn off Auto Debit?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                    {
                        var result21 = form1.ShowDialog();
                        if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                        {

                            Ref_Expense.AutoDebit = "0";
                            payFrom.Text = "None";
                            payFrom.Enabled = true;
                            datePay.Enabled = true;
                            ModernStyleToggleSwitch.Checked = false;
                            Toggle_Switch = false;
                        }
                        else
                        {
                            ModernStyleToggleSwitch.Checked = true;
                            Toggle_Switch = false;
                        }
                    }
                    Grey_In();
                }
            }
        }

        private void fromDate_ValueChanged(object sender, EventArgs e)
        {
        }

        private void payFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}
