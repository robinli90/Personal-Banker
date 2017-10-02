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
    public partial class CashView : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public CashView(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void UpdateBalanceLabel()
        {
            balanceLabel.Text = Cash.GetCurrentBalanceStr();

            balanceLabel.ForeColor = Cash.GetCurrentBalance() >= 0 ? SystemColors.Control : Color.LightCoral;
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            UpdateBalanceLabel();

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

        public void Set_Form_Color(Color randomColor)
        {
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        private void withdrawCash_Click(object sender, EventArgs e)
        {
            Grey_Out();
            if (Cash.GetCurrentBalance() > 0)
            {
                TransactionDialog CD = new TransactionDialog(parent, TransactionType.Withdraw, Location, Size);
                CD.ShowDialog();
                UpdateBalanceLabel();
            }
            else
            {
                Form_Message_Box FMB =
                    new Form_Message_Box(parent,
                        "You do not have enough funds to withdraw money", true, 0,
                        Location, Size);
                FMB.ShowDialog();
            }
            Grey_In();
        }

        private void depositCash_Click(object sender, EventArgs e)
        {
            Grey_Out();
            TransactionDialog CD = new TransactionDialog(parent, TransactionType.Deposit, Location, Size);
            CD.ShowDialog();
            UpdateBalanceLabel();
            Grey_In();
        }

        private void viewHistory_Click(object sender, EventArgs e)
        {
            CashHistory CH = new CashHistory(parent, Location, Size);
            CH.ShowDialog();
            UpdateBalanceLabel();
        }

        private void setBalance_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Input_Box_Small(parent, "Set balance to:", "$",
                "OK", null, this.Location, this.Size, 14, true))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    try
                    {
                        // Parse initial dollar sign
                        if (form.Pass_String.StartsWith("$")) form.Pass_String = form.Pass_String.Substring(1);

                        double amount = Convert.ToDouble(form.Pass_String);

                        if (amount <= 0) throw new Exception();

                        Cash.AddCashHistory(DateTime.Now.Date, "Balance set (orig. " + Cash.GetCurrentBalanceStr() + ")",
                            amount, "SB");

                        UpdateBalanceLabel();

                        parent.Background_Save();
                    }
                    catch (Exception ex)
                    {
                        Form_Message_Box FMB = new Form_Message_Box(parent, "Error. The value provided is not a valid amount", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                }
            }
            Grey_In();
        }
    }
}
