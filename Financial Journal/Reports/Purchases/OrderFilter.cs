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
    public partial class OrderFilter : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {

            this.Dispose();
            this.Close();
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;
        public Dictionary<string, string> filterSettings = new Dictionary<string, string>();

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public OrderFilter(Receipt _parent, Dictionary<string, string> _filter2Settings, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            filterSettings = _filter2Settings;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void PopulateBoxes()
        {
            #region Populate date
            for (int i = 1; i < 13; i++)
            {
                from_month.Items.Add(mfi.GetMonthName(i));
                to_month.Items.Add(mfi.GetMonthName(i));
            }

            // Add years to box (only get the years where purchases have been made)
            List<string> Years = new List<string>();
            foreach (Order order in parent.Order_List)
            {
                if (!Years.Contains(order.Date.Year.ToString()))
                {
                    Years.Add(order.Date.Year.ToString());
                }
            }

            Years = Years.OrderBy(x => Convert.ToInt32(x)).ToList();
            Years.ForEach(x => from_year.Items.Add(x));
            Years.ForEach(x => to_year.Items.Add(x));
            #endregion

            #region Populate locations, category and payments

            locationBox.Items.Clear();
            paymentBox.Items.Clear();

            locationBox.Items.Add("All");
            categoryBox.Items.Add("All");
            paymentBox.Items.Add("All");

            foreach (string location in parent.location_box.Items)
            {
                locationBox.Items.Add(location);
            }

            foreach (string category in parent.Master_Item_List.Select(x => x.Category).Distinct().OrderBy(x => x))
            {
                categoryBox.Items.Add(category);
            }

            parent.Payment_List.ForEach(x => paymentBox.Items.Add(x.ToString()));
            paymentBox.Items.Add("Cash");
            paymentBox.Items.Add("Other");

            #endregion

            SetCurrentValues();
        }

        private void SetCurrentValues()
        {
            #region set to current value
            locationBox.Text = filterSettings["location"];
            paymentBox.Text = filterSettings["payment"];
            categoryBox.Text = filterSettings["category"];
            itemName.Text = filterSettings["itemName"];

            from_month.Text = mfi.GetMonthName(Convert.ToInt32(filterSettings["fromMonth"]));
            to_month.Text = mfi.GetMonthName(Convert.ToInt32(filterSettings["toMonth"]));
            to_year.Text = filterSettings["toYear"];
            from_year.Text = filterSettings["fromYear"];
            #endregion
        }

        // Converting month number to name
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            PopulateBoxes();


            //to_year.Text = DateTime.Now.Year.ToString();
            //from_year.Text = mfi.GetMonthName(DateTime.Now.Month);
            //to_month.Text = DateTime.Now.Year.ToString();

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

        private void SetFilterValues()
        {
            filterSettings = new Dictionary<string, string>();

            // Default is current month
            filterSettings.Add("fromMonth", (from_month.Items.IndexOf(from_month.Text) + 1).ToString());
            filterSettings.Add("toMonth", (to_month.Items.IndexOf(to_month.Text) + 1).ToString());
            filterSettings.Add("fromYear", from_year.Text);
            filterSettings.Add("toYear", to_year.Text);
            filterSettings.Add("category", categoryBox.Text);
            filterSettings.Add("payment", paymentBox.Text);
            filterSettings.Add("location", locationBox.Text);
            filterSettings.Add("itemName", itemName.Text);
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
            SetFilterValues();
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

        private void close_entry_button_Click(object sender, EventArgs e)
        {
            itemName.Text = "";
        }

        private void sortOptions_Click(object sender, EventArgs e)
        {
            filterSettings["fromMonth"] = DateTime.Now.Month.ToString();
            filterSettings["toMonth"] = DateTime.Now.Month.ToString();
            filterSettings["fromYear"] = DateTime.Now.Year.ToString();
            filterSettings["toYear"] = DateTime.Now.Year.ToString();
            filterSettings["category"] = "All";
            filterSettings["payment"] = "All";
            filterSettings["location"] = "All";
            filterSettings["itemName"] = "";
            SetCurrentValues();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetFilterValues();
            Close();
        }

        private void itemName_TextChanged(object sender, EventArgs e)
        {
            from_month.SelectedIndex = 0;
            to_month.SelectedIndex = 11;
            from_year.SelectedIndex = 0;
            to_year.SelectedIndex = to_year.Items.Count - 1;
        }
    }
}
