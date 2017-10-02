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
    public partial class Deposit_Paycheck : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            base.OnFormClosing(e);
        }

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public Deposit_Paycheck(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
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
            CustomIncome CI = parent.Income_Company_List.FirstOrDefault(x => x.Default);
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            label3.Text = "Pay Period: " + CI.Intervals.Count + " (" + CI.Intervals[CI.Intervals.Count - 1].Pay_Date.ToShortDateString() + ")";
            amt_box.KeyPress += new KeyPressEventHandler(this.comboBox_KeyPress);
            amt_box.Focus();

            amt_box.Text = "$" + parent.Income_Company_List.FirstOrDefault(x => x.Default).Intervals[parent.Income_Company_List.FirstOrDefault(x => x.Default).Intervals.Count - 1].Amount;


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
                bool Deposit = false;
                if (parent.Income_Company_List.FirstOrDefault(x => x.Default).Intervals[parent.Income_Company_List.FirstOrDefault(x => x.Default).Intervals.Count - 1].Amount > 0)
                {
                    Grey_Out();
                    using (var form1 = new Yes_No_Dialog(parent, "The latest period has already been deposited. Do you wish to overwrite?", "Warning", "No", "Yes", 15, this.Location, this.Size))
                    {
                        var result21 = form1.ShowDialog();
                        if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                        {
                            Deposit = true;
                        } 
                    }
                    Grey_In();
                }
                else
                {
                    Deposit = true;
                }

                if (Deposit)
                {
                    PayPeriod Ref_PP = parent.Income_Company_List.FirstOrDefault(x => x.Default).Intervals[parent.Income_Company_List.FirstOrDefault(x => x.Default).Intervals.Count - 1];

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
                            if (PO.Date.Date == Ref_PP.Pay_Date.Date && PO.Note.Contains(Ref_CI.Company))
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

    }
}
