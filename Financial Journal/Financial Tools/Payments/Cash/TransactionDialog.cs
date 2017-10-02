using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Objects;

namespace Financial_Journal
{
    public partial class TransactionDialog : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }

        private TransactionType _transactionType;

        Receipt parent;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public TransactionDialog(Receipt _parent, TransactionType tt, Point g = new Point(), Size s = new Size())
        {
            _transactionType = tt;
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {

            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            switch (_transactionType)
            {
                case TransactionType.Deposit:
                {
                    transferCheck.Text = "Withdraw from Account?";
                    enterLabel.Text = "ADD";
                    titleLabel.Text = "Add Money";
                    maxLabel.Visible = false;
                    break;
                }
                case TransactionType.Withdraw:
                {
                    transferCheck.Text = "Deposit into Account?";
                    enterLabel.Text = "TRANSFER";
                    titleLabel.Text = "Use Money";
                    maxLabel.Text = String.Format("max. ${0:0.00}", Cash.GetCurrentBalance());
                    break;
                }
            }

            dateTimePicker1.Value = DateTime.Now;

            accountBox.Items.Clear();

            foreach (Payment P in parent.Payment_List)
            {
                accountBox.Items.Add(P.ToString());
            }

            if (accountBox.Items.Count > 0) accountBox.SelectedIndex = 0;

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

        public string memoText = "";

        public void Set_Form_Color(Color randomColor)
        {
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; titleLabel.ForeColor = Color.Silver;
        }

        private void transferCheck_CheckedChanged(object sender, EventArgs e)
        {
            accountBox.Enabled = transferCheck.Checked;
            maxLabel.Visible = (_transactionType == TransactionType.Deposit && transferCheck.Checked) || _transactionType == TransactionType.Withdraw;

            if (TransactionType.Deposit == _transactionType && transferCheck.Checked)
            {
                Payment refPayment = parent.Payment_List.First(x => x.ToString() == accountBox.Text);

                maxLabel.Text = String.Format("max. ${0:0.00}", refPayment.Balance);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (amountBox.Text.Length > 1 && Convert.ToDouble(amountBox.Text.Substring(1)) > 0)
            {
                switch (_transactionType)
                {
                    case TransactionType.Withdraw:
                    {
                        #region Withdraw funds
                        double amount = Convert.ToDouble(amountBox.Text.Substring(1));
                        Payment refPayment = new Payment(); ;

                        if (amount > Cash.GetCurrentBalance())
                        {

                            Grey_Out();
                            // Get monthly income and compare
                            Form_Message_Box FMB =
                                new Form_Message_Box(parent,
                                    "You do not have enough funds to complete this transaction", true, 0,
                                    Location, Size);
                            FMB.ShowDialog();
                            Grey_In();
                            return;
                        }

                        if (transferCheck.Checked)
                        {
                            refPayment = parent.Payment_List.First(x => x.ToString() == accountBox.Text);

                            parent.Payment_Options_List.Add(new Payment_Options()
                            {
                                Type = "Deposit",
                                Amount = amount,
                                Date = dateTimePicker1.Value,
                                Note = String.Format("Cash Deposit {0}", memoText.Length > 0 ? "- " + memoText : ""),
                                Hidden_Note = "",
                                Ending_Balance = refPayment.Balance + amount,
                                Payment_Bank = refPayment.Bank,
                                Payment_Company = refPayment.Company,
                                Payment_Last_Four = refPayment.Last_Four
                            });

                            refPayment.Balance += amount;
                        }
                        //Cash.AddCashHistory(dateTimePicker1.Value, memoText, -amount, "");
                        Cash.AddCashHistory(dateTimePicker1.Value, memoText, -amount, (transferCheck.Checked ? "T" + refPayment.ToString() : ""));

                        parent.Background_Save();
                        Close();
                        break;
                        #endregion
                    }
                    case TransactionType.Deposit:
                    {
                        #region Deposit funds
                        double amount = Convert.ToDouble(amountBox.Text.Substring(1));
                        Payment refPayment = new Payment();;

                        if (transferCheck.Checked)
                        {
                            refPayment = parent.Payment_List.First(x => x.ToString() == accountBox.Text);

                            if (amount > refPayment.Balance)
                            {

                                Grey_Out();
                                // Get monthly income and compare
                                Form_Message_Box FMB =
                                    new Form_Message_Box(parent,
                                        String.Format("You do not have enough funds in '{0}' to complete this transaction", refPayment.ToString()), true, 0,
                                        Location, Size);
                                FMB.ShowDialog();
                                Grey_In();
                                return;
                            }

                            parent.Payment_Options_List.Add(new Payment_Options()
                            {
                                Type = "Withdrawal",
                                Amount = amount,
                                Date = dateTimePicker1.Value,
                                Note = String.Format("Cash Withdrawal {0}", memoText.Length > 0 ? "- " + memoText : ""),
                                Hidden_Note = "",
                                Ending_Balance = refPayment.Balance + amount,
                                Payment_Bank = refPayment.Bank,
                                Payment_Company = refPayment.Company,
                                Payment_Last_Four = refPayment.Last_Four
                            });

                            refPayment.Balance -= amount;

                        }
                        Cash.AddCashHistory(dateTimePicker1.Value, memoText, amount, (transferCheck.Checked ? "T" + refPayment.ToString() : ""));
                        parent.Background_Save();
                        Close();
                        break;
                        #endregion
                    }
                }
            }
        }

        private void memoButton_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Input_Box_Small(parent, "Enter the remark", memoText,
                "OK", null, this.Location, this.Size, 4, true))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK && form.Pass_String.ToLower().Trim() != "remark")
                {
                    memoText = form.Pass_String;
                }
            }
            Grey_In();
        }

        private void amountBox_TextChanged(object sender, EventArgs e)
        {
            parent.textBox6_TextChanged(sender, e);
        }

        private void accountBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TransactionType.Deposit == _transactionType && transferCheck.Checked)
            {
                Payment refPayment = parent.Payment_List.First(x => x.ToString() == accountBox.Text);

                maxLabel.Text = String.Format("max. ${0:0.00}", refPayment.Balance);
            }
        }
    }
}
