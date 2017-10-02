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
    public partial class AddNewMonth : Form
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
        public AddNewMonth(Receipt _parent, BudgetAllocation BA, Point g = new Point(), Size s = new Size())
        {
            refBA = BA;
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

            // Add months
            for (int i = 1; i <= 12; i++)
            {
                monthBox.Items.Add(i);
            }

            try
            {
                // Add years +- 5 from now and earliest order date by 2
                for (int i = parent.Order_List.Min(x => x.Date.Year) - 2; i <= DateTime.Now.Year + 5; i++)
                {
                    yearBox.Items.Add(i);
                }
            }
            catch (Exception ex)
            {
                for (int i = DateTime.Now.Year - 2; i <= DateTime.Now.Year + 5; i++)
                {
                    yearBox.Items.Add(i);
                }
            }

            monthBox.Text = DateTime.Now.Month.ToString();
            yearBox.Text = DateTime.Now.Year.ToString();

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
            textBox4.BackColor = randomColor; 
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            int month = Convert.ToInt32(monthBox.Text);
            int year = Convert.ToInt32(yearBox.Text);
            if (!parent.BudgetEntryList.Any(x => x.Month == month && x.Year == year))
            {
                refBA.setMonth = month;
                refBA.setYear = year;
                parent.BudgetEntryList.Add(new BudgetEntry(month, year, IncomeMode.Manual, 0));
                Close();
            }
            else
            {
                Grey_Out();
                Form_Message_Box FMB =
                    new Form_Message_Box(parent, "Error: Budget exists for that month", true, -10, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
        }

        private void close_button_Click_1(object sender, EventArgs e)
        {
            this.Dispose();
            this.Close();
        }
    }
}
