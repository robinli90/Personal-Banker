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
    public partial class With_Dep_Box : Form
    {
        Receipt parent;
        int Start_Location_Offset = 25;
        object Pass_Object = null;

        public string Pass_String = "";
        Investment Ref_Investment;

        public With_Dep_Box(Receipt _parent, Investment Ref_Inv_ = null, Point g = new Point(), Size s = new Size(), int Grow_Height = 0, DateTime Preset_Date = new DateTime())
        {

            InitializeComponent();

            close_button.Visible = true;
            label5.Text = "Withdraw or Contribute";

            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            Ref_Investment = Ref_Inv_;

            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
            this.Height += Grow_Height;

            // If preset date
            if (Preset_Date.Year > 1800)
            {
                dateTimePicker1.Value = Preset_Date;
                button3.Enabled = false;
                this.Height = label7.Top - 1;
            }
            else
            {
                dateTimePicker1.Value = Ref_Investment.Start_Date > DateTime.Now ? Ref_Investment.Start_Date : DateTime.Now;
            }
        }

        List<Payment> List_Of_Payments = new List<Payment>();

        private void Receipt_Load(object sender, EventArgs e)
        {
            action_box.Items.Add("Contribute");
            action_box.Items.Add("Withdraw");
            action_box.SelectedIndex = 0;

            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.input.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxEnterKey_KeyPress);
            input.Focus();
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

            payFrom.Items.Add("None");
            parent.Payment_List.ForEach(x => List_Of_Payments.Add(x));
            List_Of_Payments.ForEach(x => payFrom.Items.Add(x.Company + " (xx-" + x.Last_Four + ")"));

            payFrom.SelectedIndex = 0;
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


        // If press enter on length box, activate add (nmemonics)
        private void textboxEnterKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox g = (TextBox)sender;
            if (e.KeyChar == (char)Keys.Enter)
            {
                Add_button.PerformClick();
            }
        }

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            parent.Focus();
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

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }   

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }

        private void input_TextChanged(object sender, EventArgs e)
        {
            parent.textBox6_TextChanged(sender, e);
        }

        private void Add_button_Click_1(object sender, EventArgs e)
        {
            if (input.Text.Length > 1)
            {
                if (!Ref_Investment.Check_Date_Validity(dateTimePicker1.Value))
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Invalid Date Selection. Valid dates between '" + Ref_Investment.Get_Next_Period(Ref_Investment.Start_Date).ToShortDateString() + "' and '" + DateTime.Now.AddYears(5).ToShortDateString() + "' only", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                    dateTimePicker1.Value = Ref_Investment.Start_Date > DateTime.Now ? Ref_Investment.Start_Date : DateTime.Now;
                }
                else
                {
                    if (action_box.Text == "Withdraw")
                    {
                        if (payFrom.Text != "None")
                        {
                            Grey_Out();
                            using (var form = new Yes_No_Dialog(parent, "Do you wish to transfer funds from '" + Ref_Investment.Name + "' to '" + payFrom.Text + "'?", "Warning", "No", "Yes", 15, this.Location, this.Size))
                            {
                                var result2 = form.ShowDialog();
                                if (result2 == DialogResult.OK)
                                {
                                    if (form.ReturnValue1 == "1")
                                    {
                                        if (!Ref_Investment.Withdraw(Convert.ToDouble(input.Text.Substring(1)), dateTimePicker1.Value))
                                        {
                                            Form_Message_Box FMB = new Form_Message_Box(parent, "Error: Insufficient funds", true, -20, this.Location, this.Size);
                                            FMB.ShowDialog();
                                        }
                                        else
                                        {
                                            Payment Reference_Pay = List_Of_Payments[payFrom.Items.IndexOf(payFrom.Text) - 1];

                                            Create_Payment_Options("Deposit", Convert.ToDouble(input.Text.Substring(1)), dateTimePicker1.Value, "Transferred from investment '" + Ref_Investment.Name + "'", Reference_Pay);
                                            Reference_Pay.Balance += Convert.ToDouble(input.Text.Substring(1));
                                        }
                                    }
                                }
                            }
                            Grey_In();
                        }
                        else
                        {
                            if (!Ref_Investment.Withdraw(Convert.ToDouble(input.Text.Substring(1)), dateTimePicker1.Value))
                            {
                                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: Insufficient funds", true, -20, this.Location, this.Size);
                                FMB.ShowDialog();
                                Ref_Investment.Populate_Matrix();
                            }
                        }
                        Ref_Investment.Populate_Matrix();
                        this.Close();
                    }
                    else
                    {
                        // if account selected, check if funds exist
                        if (payFrom.Text != "None")
                        {
                            Grey_Out();
                            using (var form = new Yes_No_Dialog(parent, "Do you wish to transfer funds from '" + payFrom.Text + "' to '" + Ref_Investment.Name + "'?", "Warning", "No", "Yes", 15, this.Location, this.Size))
                            {
                                var result2 = form.ShowDialog();
                                if (result2 == DialogResult.OK)
                                {
                                    if (form.ReturnValue1 == "1")
                                    {
                                        Payment Reference_Pay = List_Of_Payments[payFrom.Items.IndexOf(payFrom.Text) - 1];

                                        if (Reference_Pay.Balance >= Convert.ToDouble(input.Text.Substring(1)))
                                        {
                                            Create_Payment_Options("Withdrawal", Convert.ToDouble(input.Text.Substring(1)), dateTimePicker1.Value, "Moved to investment '" + Ref_Investment.Name + "'", Reference_Pay);
                                            Reference_Pay.Balance -= Convert.ToDouble(input.Text.Substring(1));
                                            Ref_Investment.Deposit(Convert.ToDouble(input.Text.Substring(1)), dateTimePicker1.Value);
                                            Ref_Investment.Populate_Matrix();
                                            this.Close();
                                        }
                                        else
                                        {
                                            Form_Message_Box FMB = new Form_Message_Box(parent, "Insufficient funds to withdraw", true, 0, this.Location, this.Size);
                                            FMB.ShowDialog();
                                        }
                                    }
                                }
                            }
                            Grey_In();
                        }
                        else
                        {
                            Ref_Investment.Deposit(Convert.ToDouble(input.Text.Substring(1)), dateTimePicker1.Value);
                            Ref_Investment.Populate_Matrix();
                            this.Close();
                        }
                    }
                }
            }
        }

        private void Create_Payment_Options(string Type_, double Amount_, DateTime Date_, string Note_, Payment Ref_Payment)
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

        private void button3_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Enabled = !dateTimePicker1.Enabled;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void action_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            label7.Text = action_box.Text == "Contribute" ? "From Account" : "To Account";
        }
    }
}