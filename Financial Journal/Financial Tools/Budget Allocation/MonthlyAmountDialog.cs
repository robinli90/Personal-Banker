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
    public partial class MonthlyAmountDialog : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;
        private BudgetAllocation refBA;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public MonthlyAmountDialog(Receipt _parent, BudgetAllocation _refBA, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            refBA = _refBA;
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            if (refBA.RefBudgetEntry.TargetBudget > 0 && refBA.RefBudgetEntry.IncomeMode == IncomeMode.Manual)
            {
                manualTarget.Image = Properties.Resources.greencredit;
            }
            else if (refBA.RefBudgetEntry.TargetBudget > 0 && refBA.RefBudgetEntry.IncomeMode == IncomeMode.Automatic)
            {
                autoTarget.Image = Properties.Resources.greenpiggy;
            }

            // Set savings value
            if (parent.Savings.Structure == "Percentage")
            {
                savingsTarget.Text = refBA.GetDollarFormat(parent.Monthly_Income * (parent.Savings.Ref_Value / 100));
            }
            else if (parent.Savings.Structure == "Amount")
            {
                savingsTarget.Text = refBA.GetDollarFormat(parent.Savings.Ref_Value);
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

        public double returnAmount = 0;

        private void xMonths_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Input_Box_Small(parent,
                "Enter target amount", "", "OK", null, this.Location,
                this.Size, 0, true))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    try
                    {
                        returnAmount = Convert.ToDouble(form.Pass_String);
                        refBA.RefBudgetEntry.IncomeMode = IncomeMode.Manual;
                        DialogResult = DialogResult.OK;
                        Close();
                    }
                    catch // Non double value
                    {
                        Form_Message_Box FMB = new Form_Message_Box(parent, "Invalid Amount Entered", true, -30, this.Location, this.Size);
                        FMB.ShowDialog();
                    } 
                }
            }
            Grey_In();
        }

        private void threeMonths_Click(object sender, EventArgs e)
        {
            using (Savings_Helper SH = new Savings_Helper(parent))
            {
                if (parent.Savings.Structure == "Percentage")
                {
                    returnAmount = SH.Get_Monthly_Salary(refBA.RefBudgetEntry.Month, refBA.RefBudgetEntry.Year) - parent.Monthly_Income * (parent.Savings.Ref_Value / 100);
                }
                else if (parent.Savings.Structure == "Amount")
                {
                    double incomeAmt = SH.Get_Monthly_Salary(refBA.RefBudgetEntry.Month, refBA.RefBudgetEntry.Year);
                    returnAmount = incomeAmt - parent.Savings.Ref_Value;
                }

                refBA.RefBudgetEntry.IncomeMode = IncomeMode.Automatic;
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
