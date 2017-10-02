using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Financial_Journal
{
    public partial class Deposit_Input_Box : Form
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
        Manage_Prev_Period parent2;
        Size Start_Size = new Size();
        PayPeriod Ref_PP;

        public Deposit_Input_Box(Receipt _parent, Manage_Prev_Period _parent2, PayPeriod ref_Period, Point g = new Point(), Size s = new Size())
        {
            parent2 = _parent2;
            Ref_PP = ref_Period;
            //this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            this.Location = new Point(g.X + s.Width / 2 - this.Width / 2, g.Y + s.Height / 2 - this.Height / 2);
            Set_Form_Color(parent.Frame_Color);
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            amt_box.Text = "$" + Ref_PP.Amount;
            label3.Text = "Date: " + Ref_PP.Pay_Date.ToShortDateString();

            amt_box.KeyPress += new KeyPressEventHandler(this.comboBox_KeyPress);

            ModernStyleToggleSwitch.Checked = parent2.Advanced_To_Next;

            // Style the switch
            ModernStyleToggleSwitch.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            ModernStyleToggleSwitch.Size = new Size(68, 25);
            ModernStyleToggleSwitch.OnText = "On";
            ModernStyleToggleSwitch.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            ModernStyleToggleSwitch.OnForeColor = Color.White;
            ModernStyleToggleSwitch.OffText = "Off";
            ModernStyleToggleSwitch.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            ModernStyleToggleSwitch.OffForeColor = Color.White;
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
            textBox4.BackColor = randomColor;
        }

        
        // If press enter on length box, activate add (nmemonics)
        private void comboBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox g = (TextBox)sender;
            if (e.KeyChar == (char)Keys.Enter && g.Text.Length > 0)
            {
                Add_button.PerformClick();
            }
        }

        private void amt_box_TextChanged(object sender, EventArgs e)
        {
            if (!(amt_box.Text.StartsWith("$")))
            {
                if (parent.Get_Char_Count(amt_box.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = amt_box.Text;
                    amt_box.Text = temp.Substring(1) + temp[0];
                    amt_box.SelectionStart = amt_box.Text.Length;
                    amt_box.SelectionLength = 0;
                }
                else
                {
                    amt_box.Text = "$" + amt_box.Text;
                }
            }
            else if ((amt_box.Text.Length > 1) && ((parent.Get_Char_Count(amt_box.Text, Convert.ToChar(".")) > 1) || (amt_box.Text[1].ToString() == ".") || (parent.Get_Char_Count(amt_box.Text, Convert.ToChar("$")) > 1) || (!((amt_box.Text.Substring(amt_box.Text.Length - 1).All(char.IsDigit))) && !(amt_box.Text[amt_box.Text.Length - 1].ToString() == "."))))
            {
                amt_box.TextChanged -= new System.EventHandler(amt_box_TextChanged);
                amt_box.Text = amt_box.Text.Substring(0, amt_box.Text.Length - 1);
                amt_box.SelectionStart = amt_box.Text.Length;
                amt_box.SelectionLength = 0;
                amt_box.TextChanged += new System.EventHandler(amt_box_TextChanged);
            }
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (amt_box.Text.Length > 1)
            { 
                //parent.Income_Company_List.FirstOrDefault(x => x.Default).Intervals[Convert.ToInt32(Ref_PP.Pay_Period) - 1].Amount = Convert.ToDouble(amt_box.Text.Substring(1));
                Ref_PP.Amount = Convert.ToDouble(amt_box.Text.Substring(1));

                CustomIncome Ref_CI = parent.Income_Company_List.FirstOrDefault(x => x.Default);

                //if auto deposit
                if (Ref_CI.Deposit_Account.Length > 0)
                {
                    Payment Ref_Payment = parent.Payment_List.FirstOrDefault(x => x.Get_Long_String() == Ref_CI.Deposit_Account);

                    // remove existing IF exists from payment options
                    foreach (Payment_Options PO in parent.Payment_Options_List.Where(x => x.Payment_Company == Ref_Payment.Company && x.Payment_Last_Four == Ref_Payment.Last_Four && Ref_Payment.Bank == Ref_Payment.Bank))
                    {
                        // Set year to minimum value (new DateTime)
                        if (PO.Date.Date == Ref_PP.Pay_Date && PO.Note.Contains(Ref_CI.Company))
                        {
                            PO.Date = new DateTime();
                            break; // Break because there's only one valid
                        }
                    }

                    Payment_Options Temp_PO = parent.Payment_Options_List.FirstOrDefault(x => x.Date.Year < 1801);

                    // Remove min-value dates thus purging the single payment account
                    parent.Payment_Options_List = parent.Payment_Options_List.Where(x => x.Date.Year > 1801).ToList();

                    // Create new deposit and adjust balance and create options entry
                    Create_Payment_Options("Deposit", Ref_PP.Amount, Ref_PP.Pay_Date, Ref_CI.Company + " Pay Period " + Ref_PP.Pay_Period, Ref_Payment);

                    // Add balance
                    if (Temp_PO != null)
                    {
                        // Remove old balance
                        Ref_Payment.Balance -= Temp_PO.Amount;
                    }
                    Ref_Payment.Balance += Ref_PP.Amount;
                }

                this.Close();
            }
        }

        public void Create_Payment_Options(string Type_, double Amount_, DateTime Date_, string Note_, Payment Ref_Payment)
        {
            parent.Payment_Options_List.Add(new Payment_Options()
            {
                Type = Type_,
                Amount = Amount_,
                Date = Date_,
                Note = Note_,
                Hidden_Note = "",
                Ending_Balance = Ref_Payment.Balance + Amount_ * (Type_ == "Deposit" ? 1 : -1), // Adjust balance
                Payment_Bank = Ref_Payment.Bank,
                Payment_Company = Ref_Payment.Company,
                Payment_Last_Four = Ref_Payment.Last_Four
            });
        }

        private void ModernStyleToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            parent2.Advanced_To_Next = ModernStyleToggleSwitch.Checked;
        }

        private void close_button_Click_1(object sender, EventArgs e)
        {
            parent2.Advanced_To_Next = false;
            this.Dispose();
            this.Close();
        }
    }
}
