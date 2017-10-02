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
    public partial class Manage_Prev_Period_Range_Selector : Form
    {

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public Manage_Prev_Period_Range_Selector(Receipt _parent, Point g = new Point(), Size s = new Size())
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

        // Converting month number to name
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);


            for (int i = 1; i < 13; i++)
            {
                from_month.Items.Add(mfi.GetMonthName(i));
                to_month.Items.Add(mfi.GetMonthName(i));
            }

            for (int i = DateTime.Now.AddYears(-5).Year; i <= DateTime.Now.Year; i++)
            {
                from_year.Items.Add(i);
                to_year.Items.Add(i);
            }

            from_month.Text = mfi.GetMonthName(DateTime.Now.AddMonths(-7).Month);
            to_year.Text = DateTime.Now.Year.ToString();
            try
            {
                from_year.Text = DateTime.Now.AddMonths(-7).Year.ToString();
            }
            catch
            {
                from_year.SelectedIndex = from_year.Items.Count - 1;
            }
            to_month.Text = mfi.GetMonthName(DateTime.Now.Month);

            Loaded = true;
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

        private void button2_Click(object sender, EventArgs e)
        {
            Calculate_Months();
            this.Visible = false;
            Manage_Prev_Period MPP = new Manage_Prev_Period(parent, From_Date, To_Date.AddMonths(1).AddDays(-1)); // Get start of month to end of end month
            MPP.ShowDialog();
            this.Close();
        }

        private void to_month_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        private void from_month_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        private void from_year_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        private void to_year_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate_Months();
        }

        bool Loaded = false;
        DateTime From_Date;
        DateTime To_Date;

        private void Calculate_Months()
        {
            // Only check after loaded
            if (Loaded)
            {

                From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, 1);
                To_Date = new DateTime(Convert.ToInt32(to_year.Text), to_month.SelectedIndex + 1, 1);

                // If invalid date selection, set dates to be the same
                if (From_Date > To_Date)
                {
                    from_month.Text = to_month.Text = mfi.GetMonthName(DateTime.Now.Month);
                    from_year.Text = to_year.Text = (DateTime.Now.Year).ToString();

                    From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, 1);
                    To_Date = new DateTime(Convert.ToInt32(to_year.Text), to_month.SelectedIndex + 1, 1);
                }
                else
                {
                }
            }
        }
    }
}
