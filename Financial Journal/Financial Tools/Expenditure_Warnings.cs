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
    public partial class Expenditure_Warnings : Form
    {

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int data_height = 27;
            int start_height = Start_Size.Height + 15;
            int height_offset = 9;

            int start_margin = 15;              //Category
            int margin1 = start_margin + 150;   //Intervals
            int margin2 = margin1 + 150;        //Reference

            int row_count = 0;

            Color DrawForeColor = Color.White;
            Color BackColor = Color.FromArgb(64, 64, 64);
            Color HighlightColor = Color.FromArgb(76, 76, 76);

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(88, 88, 88));
            SolidBrush RedBrush = new SolidBrush(Color.LightPink);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 2);

            Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
            Font f = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
            Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);

            // If has order
            if (parent.Warnings_Dictionary.Count > 0)
            {

                // Draw gray header line
                e.Graphics.DrawLine(Grey_Pen, start_margin, start_height - 15, this.Width - 16, start_height - 15);

                // Header
                e.Graphics.DrawString("Category", f_header, WritingBrush, start_margin + 25, start_height + (row_count * data_height));
                e.Graphics.DrawString("Intervals", f_header, WritingBrush, margin1 + 18, start_height + (row_count * data_height));
                e.Graphics.DrawString("Monthly Reference", f_header, WritingBrush, margin2 + 9, start_height + (row_count * data_height));
                row_count += 1;

                int item_index = 0;

                // Remove existing buttons
                Delete_Button_List.ForEach(button => button.Image.Dispose());
                Delete_Button_List.ForEach(button => button.Dispose());
                Delete_Button_List.ForEach(button => this.Controls.Remove(button));
                Delete_Button_List = new List<Button>();

                // For each refund item
                foreach (KeyValuePair<string, Warning> warning in parent.Warnings_Dictionary)
                {
                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button delete_button = new Button();
                    delete_button.BackColor = this.BackColor;
                    delete_button.ForeColor = this.BackColor;
                    delete_button.FlatStyle = FlatStyle.Flat;
                    delete_button.Image = global::Financial_Journal.Properties.Resources.delete;
                    delete_button.Size = new Size(29, 29);
                    delete_button.Location = new Point(this.Width - 42, start_height + height_offset + (row_count * data_height) - 5);
                    delete_button.Name = warning.Key;
                    delete_button.Text = "";
                    delete_button.Click += new EventHandler(this.view_order_Click);
                    Delete_Button_List.Add(delete_button);
                    ToolTip1.SetToolTip(delete_button, "Delete " + warning.Key + " warning");
                    this.Controls.Add(delete_button);

                    e.Graphics.DrawString(warning.Value.Category, f, WritingBrush, start_margin + 6, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString((warning.Value.First_Level.ToString() + "%, " + warning.Value.Second_Level.ToString() + "%, " + warning.Value.Final_Level.ToString()) + "%", f, WritingBrush, margin1, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(("Based on " + (warning.Value.Warning_Type == "Percent" ? "" : "$") +
                        (warning.Value.Warning_Type == "Percent" ? 
                        warning.Value.Warning_Amt + "% of income" : warning.Value.Warning_Amt + " of category spending"))
                        , f, WritingBrush, margin2 - 30, start_height + height_offset + (row_count * data_height));
                    
                    row_count++;
                    item_index++;
                }

                row_count++;
                this.Height = start_height + height_offset + row_count * data_height;
            }
            else
            {
                this.Height = Start_Size.Height;
            }

            // Dispose all objects
            p.Dispose();
            Grey_Pen.Dispose();   
            GreenBrush.Dispose();
            RedBrush.Dispose();
            GreyBrush.Dispose();
            WritingBrush.Dispose();
            f_asterisk.Dispose();
            f.Dispose();
            f_strike.Dispose();
            f_total.Dispose();
            f_header.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        private void view_order_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            parent.Warnings_Dictionary.Remove(b.Name);
            category_box.Items.Clear();
            foreach (string item in parent.category_box.Items)
            {
                if (!parent.Warnings_Dictionary.ContainsKey(item))
                {
                    category_box.Items.Add(item);
                }
            }
            Invalidate();
            Update();
        }

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;
        List<Button> Delete_Button_List = new List<Button>();

        public Expenditure_Warnings(Receipt _parent)
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            foreach (string item in parent.category_box.Items)
            {
                if (!parent.Warnings_Dictionary.ContainsKey(item))
                {
                    category_box.Items.Add(item);
                }
            }

            first_level_box.SelectedIndexChanged -= new EventHandler(first_level_box_SelectedIndexChanged);
            secondary_level_box.SelectedIndexChanged -= new EventHandler(secondary_level_box_SelectedIndexChanged);
            final_level_box.SelectedIndexChanged -= new EventHandler(final_level_box_SelectedIndexChanged);

            for (int i = 40; i <= 95; i += 5)
            {
                first_level_box.Items.Add(i.ToString() + "%");
                secondary_level_box.Items.Add(i.ToString() + "%");
                final_level_box.Items.Add(i.ToString() + "%");
            }

            final_level_box.Items.Add("100%");
            first_level_box.Text = "60%";
            secondary_level_box.Text = "90%";
            final_level_box.Text = "100%";

            first_level_box.SelectedIndexChanged += new EventHandler(first_level_box_SelectedIndexChanged);
            secondary_level_box.SelectedIndexChanged += new EventHandler(secondary_level_box_SelectedIndexChanged);
            final_level_box.SelectedIndexChanged += new EventHandler(final_level_box_SelectedIndexChanged);


            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            ToolTip1.SetToolTip(Add_button, "Add warning");
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

        // dollar value check
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (dollar_check.Checked == false)
            {
                dollar_check.Checked = true;
            }
            else
            {
                percent_check.CheckedChanged -= new EventHandler(checkBox2_CheckedChanged);
                percent_check.Checked = false;
                //dollar_check.Enabled = false;
                percent_check.Enabled = true;
                percent_check.CheckedChanged += new EventHandler(checkBox2_CheckedChanged);
                limit_by_price_box.Enabled = true;
                limit_by_percent.Enabled = false;
            }
        }

        // percent income check
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (percent_check.Checked == false)
            {
                percent_check.Checked = true;
            }
            else
            {
                dollar_check.CheckedChanged -= new EventHandler(checkBox1_CheckedChanged);
                dollar_check.Checked = false;
                //percent_check.Enabled = false;
                dollar_check.Enabled = true;
                dollar_check.CheckedChanged += new EventHandler(checkBox1_CheckedChanged);
                limit_by_price_box.Enabled = false;
                limit_by_percent.Enabled = true;
            }
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (category_box.Text.Length > 0 &&
                (dollar_check.Checked || percent_check.Checked) &&
                (limit_by_price_box.Text.Length > 0 || limit_by_percent.Text.Length > 0))
            {
                Warning warn = new Warning();
                warn.Category = category_box.Text;
                warn.First_Level = Convert.ToDouble(first_level_box.Text.Substring(0, 2));
                warn.Second_Level = Convert.ToDouble(secondary_level_box.Text.Substring(0, 2));
                warn.Final_Level = Convert.ToDouble(final_level_box.Text.Substring(0, final_level_box.Text.Length - 1));
                warn.Warning_Type = percent_check.Checked ? "Percent" : "Price";
                warn.Warning_Amt = Convert.ToDouble(percent_check.Checked ? limit_by_percent.Text : limit_by_price_box.Text);
                parent.Warnings_Dictionary.Add(warn.Category, warn);

                category_box.Items.Clear();
                foreach (string item in parent.category_box.Items)
                {
                    if (!parent.Warnings_Dictionary.ContainsKey(item))
                    {
                        category_box.Items.Add(item);
                    }
                }
            }
            Invalidate();
            Update();
        }

        private void first_level_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            Get_Combobox_Value(first_level_box);
        }

        private void secondary_level_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            Get_Combobox_Value(secondary_level_box);
        }

        private void final_level_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            Get_Combobox_Value(final_level_box);
        }

        private void Get_Combobox_Value(AdvancedComboBox ACB)
        {
            double lower_bound = 0;
            double upper_bound = 0;
            double ref_percent = Convert.ToDouble(ACB.Text.Substring(0, ACB.Text.Length - 1));

            if (ACB.Name.Contains("first_level"))
            {
                lower_bound = Convert.ToDouble(ACB.Items[0].ToString().Substring(0, 2));
                upper_bound = Convert.ToDouble(secondary_level_box.Text.Substring(0, 2));
            }
            else if (ACB.Name.Contains("secondary_level"))
            {
                lower_bound = Convert.ToDouble(first_level_box.Text.Substring(0, 2));
                upper_bound = Convert.ToDouble(final_level_box.Text.Substring(0, final_level_box.Text.Length - 1));
            }
            else if (ACB.Name.Contains("final_level"))
            {
                lower_bound = Convert.ToDouble(secondary_level_box.Text.Substring(0, 2));
                upper_bound = Convert.ToDouble("100");
            }

            if (ref_percent <= lower_bound) ACB.Text = (lower_bound + 5).ToString() + "%";
            if (ref_percent >= upper_bound) ACB.Text = upper_bound == 100 ? "100" : (upper_bound - 5).ToString() + "%";
        }

        private void limit_by_percent_TextChanged(object sender, EventArgs e)
        {
            if (limit_by_percent.Text.All(char.IsDigit))
            {
            }
            else
            {
                // If letter in SO_number box, do not output and move CARET to end
                limit_by_percent.Text = limit_by_percent.Text.Substring(0, limit_by_percent.Text.Length - 1);
                limit_by_percent.SelectionStart = limit_by_percent.Text.Length;
                limit_by_percent.SelectionLength = 0;
            }
        }

        // Return the token count within string given token
        private int Get_Char_Count(string comparison_text, char reference_char)
        {
            int count = 0;
            foreach (char c in comparison_text)
            {
                if (c == reference_char)
                {
                    count++;
                }
            }
            return count;
        }


        private void limit_by_price_box_TextChanged(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;

            if ((box.Text.Length > 0) && ((Get_Char_Count(box.Text, Convert.ToChar(".")) > 1) || (box.Text[0].ToString() == ".") || (!((box.Text.Substring(box.Text.Length - 1).All(char.IsDigit))) && !(box.Text[box.Text.Length - 1].ToString() == "."))))
            {
                box.TextChanged -= new System.EventHandler(limit_by_price_box_TextChanged);
                box.Text = box.Text.Substring(0, box.Text.Length - 1);
                box.SelectionStart = box.Text.Length;
                box.SelectionLength = 0;
                box.TextChanged += new System.EventHandler(limit_by_price_box_TextChanged);
            }
            else if (box.Text.Length > 1)
            {
            }
        }
    }

    public class Warning
    {
        public string Category { get; set; }
        public double First_Level { get; set; }
        public double Second_Level { get; set; }
        public double Final_Level { get; set; }
        public string Warning_Type { get; set; }
        public double Warning_Amt { get; set; }

        public Warning()
        {

        }
    }
}
