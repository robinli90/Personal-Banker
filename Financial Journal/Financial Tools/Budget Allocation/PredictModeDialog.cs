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
    public partial class PredictModeDialog : Form
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
        public PredictModeDialog(Receipt _parent, BudgetAllocation _refBA, Point g = new Point(), Size s = new Size())
        {
            refBA = _refBA;
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

            includeZeroValues.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            includeZeroValues.Size = new Size(68, 25);
            includeZeroValues.OnText = "On";
            includeZeroValues.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            includeZeroValues.OnForeColor = Color.White;
            includeZeroValues.OffText = "Off";
            includeZeroValues.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            includeZeroValues.OffForeColor = Color.White;

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

        public int monthPredictionCount = 1;

        private void xMonths_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new MonthSelector(parent, Location, Size))
            {
                var result = form.ShowDialog();
                monthPredictionCount = form.selectedIndex;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private void lastMonths_Click(object sender, EventArgs e)
        {
            monthPredictionCount = 1;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void threeMonths_Click(object sender, EventArgs e)
        {
            monthPredictionCount = 3;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime minDate = parent.Order_List.Min(x => x.Date);
            monthPredictionCount = MonthDiff(DateTime.Now, minDate);
            DialogResult = DialogResult.OK;
            Close();
        }

        private int MonthDiff(DateTime date1, DateTime date2)
        {
            return ((date1.Year - date2.Year) * 12) + date1.Month - date2.Month;
        }

        private void useAnotherPeriod_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new BudgetSelector(parent,
                parent.BudgetEntryList.Where(x => x != refBA.RefBudgetEntry).Select(x => String.Format("{0}, {1}", refBA.mfi.GetMonthName(x.Month), x.Year))
                    .ToList(), // select all but current one
                Location, Size))
            {
                var result = form.ShowDialog();
                if (form.DialogResult == DialogResult.OK)
                {
                    refBA.refBudgetText = form.returnValue;
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            Grey_In();
        }

        private void close_button_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }

        private void deposit_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}
