using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;

namespace Financial_Journal
{
    public partial class Salary_Calculator : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;
        Size Start_Size = new Size();
        TextBox Ref_Box = new TextBox();

        public Salary_Calculator(Receipt _parent, bool allow_set = true, Point g = new Point(), Size s = new Size())
        {
            //this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 3) * 2, g.Y + (s.Height / 2) - (this.Height / 2));
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));

            if (!allow_set)
            {
                button4.Enabled = false;
                Add_button.Enabled = false;
            }
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;
            ToolTip1.SetToolTip(erase_button, "Clear Fields");
            ToolTip1.SetToolTip(Add_button, "Set as current salary");
            
            overtime_after_hours_box.SelectedIndexChanged -= new System.EventHandler(overtime_after_hours_box_SelectedIndexChanged);
            multiplier_box.SelectedIndexChanged -= new System.EventHandler(multiplier_box_SelectedIndexChanged);
            hours_per_day_box.SelectedIndexChanged -= new System.EventHandler(hours_per_day_box_SelectedIndexChanged);

            // Preset hours
            for (double i = 0.5; i <= 24; i += 0.5)
            {
                hours_per_day_box.Items.Add(i.ToString());
                overtime_after_hours_box.Items.Add(i.ToString());
            }

            // Preset multipliers
            for (double i = 0.5; i <= 2.5; i += 0.1)
            {
                multiplier_box.Items.Add(i.ToString());
            }


            hours_per_day_box.Text = "8";
            overtime_after_hours_box.Text = "9";
            multiplier_box.Text = "1.5";

            overtime_after_hours_box.SelectedIndexChanged += new System.EventHandler(overtime_after_hours_box_SelectedIndexChanged);
            multiplier_box.SelectedIndexChanged += new System.EventHandler(multiplier_box_SelectedIndexChanged);
            hours_per_day_box.SelectedIndexChanged += new System.EventHandler(hours_per_day_box_SelectedIndexChanged);

            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            // Remove handlers
            daily_box.TextChanged -= new System.EventHandler(box_TextChanged);
            monthly_box.TextChanged -= new System.EventHandler(box_TextChanged);
            hourly_box.TextChanged -= new System.EventHandler(box_TextChanged);
            yearly_box.TextChanged -= new System.EventHandler(box_TextChanged);
            weekly_box.TextChanged -= new System.EventHandler(box_TextChanged);


            Ref_Box = hourly_box;

            // Save information
            monthly_box.Text = "$" + (parent.Settings_Dictionary.ContainsKey("INCOME_MONTHLY") ? parent.Settings_Dictionary["INCOME_MONTHLY"] : "0"); // monthly income
            hourly_box.Text = "$" + (parent.Settings_Dictionary.ContainsKey("INCOME_HOURLY") ? parent.Settings_Dictionary["INCOME_HOURLY"] : "0");
            weekly_box.Text = "$" + (parent.Settings_Dictionary.ContainsKey("INCOME_WEEKLY") ? parent.Settings_Dictionary["INCOME_WEEKLY"] : "0");
            yearly_box.Text = "$" + (parent.Settings_Dictionary.ContainsKey("INCOME_YEARLY") ? parent.Settings_Dictionary["INCOME_YEARLY"] : "0");
            daily_box.Text = "$" + (parent.Settings_Dictionary.ContainsKey("INCOME_DAILY") ? parent.Settings_Dictionary["INCOME_DAILY"] : "0");

            hours_per_day_box.Text = parent.Settings_Dictionary.ContainsKey("WORK_HPD") ? parent.Settings_Dictionary["WORK_HPD"] : "8";
            overtime_after_hours_box.Text = parent.Settings_Dictionary.ContainsKey("WORK_OHPD") ? parent.Settings_Dictionary["WORK_OHPD"] : "9";
            multiplier_box.Text = parent.Settings_Dictionary.ContainsKey("WORK_OMULTI") ? parent.Settings_Dictionary["WORK_OMULTI"] : "1.5";
            income_tax_box.Text = parent.Settings_Dictionary.ContainsKey("INCOME_TAX_RATE") ? parent.Settings_Dictionary["INCOME_TAX_RATE"] : "0";

            // Override existing keyboard handlers
            daily_box.TextChanged += new System.EventHandler(box_TextChanged);
            monthly_box.TextChanged += new System.EventHandler(box_TextChanged);
            hourly_box.TextChanged += new System.EventHandler(box_TextChanged);
            yearly_box.TextChanged += new System.EventHandler(box_TextChanged);
            weekly_box.TextChanged += new System.EventHandler(box_TextChanged);

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


        public void Calculate(object Textbox = null)
        {
            // Preset to calculate based on hourly wage. If textbox supplied, calculate based on this textbox
            

            if (Textbox != null)
            {
                Ref_Box = (TextBox)Textbox;
            }
            else
            {
                // Nudge for other boxes and refreshes
                string pass_string = hourly_box.Text;
                //hourly_box.Text = "";
                hourly_box.Text = pass_string;
            }

            double Ref_Value = 0;

            // Populate calculation variables
            try
            {
                Ref_Value = Convert.ToDouble(Ref_Box.Text.Substring(1));
            }
            catch
            {
                Ref_Value = 10;
            }
            double Overtime_Multiplier = Convert.ToDouble(multiplier_box.Text);
            double Hours_Per_Day = Convert.ToDouble(hours_per_day_box.Text);
            double Overtime_Min_Hours = Convert.ToDouble(overtime_after_hours_box.Text);
            double Base_Hourly_Wage = 0;

            double Hours_Multiplier = 0; // to adjust for number of hours in calculation later

            // Get base multiplier for days to weeks to years conversion
            if (Ref_Box.Name.Contains("hourly")) { Hours_Multiplier = 1;}
            if (Ref_Box.Name.Contains("daily")) { Hours_Multiplier = Hours_Per_Day;}
            if (Ref_Box.Name.Contains("weekly")) { Hours_Multiplier = Hours_Per_Day * 5; }
            if (Ref_Box.Name.Contains("monthly")) { Hours_Multiplier = Hours_Per_Day * 21.741; }
            if (Ref_Box.Name.Contains("yearly")) { Hours_Multiplier = Hours_Per_Day * 21.741 * 12; }


            // Get base hours (if more hours than overtime, set base to overtime min)
            double Base_Hours = (Hours_Per_Day > Overtime_Min_Hours ? Overtime_Min_Hours : Hours_Per_Day);

            // Get overtime hours
            double Overtime_Hours = (Hours_Per_Day > Overtime_Min_Hours ? Hours_Per_Day - Overtime_Min_Hours : 0);

            //  Daily amount ... DAILY = base_hourly_wage * (regular_hours + ot_hours * multiplier)  <-- simplify this in one line
            //double Bracketted_Value = ((Base_Hours / Hours_Per_Day) + ((Overtime_Hours / Hours_Per_Day) / Overtime_Multiplier)) * Hours_Multiplier;
            double Bracketted_Value = Base_Hours + Overtime_Hours * Overtime_Multiplier;

            if (income_tax_box.Text.Length == 0) income_tax_box.Text = "0";

            if (Ref_Box == hourly_box)
            {
                Base_Hourly_Wage = (Get_Amt_From_Textbox(Ref_Box));// * Bracketted_Value) / Hours_Per_Day;
            }
            else
            {
                Base_Hourly_Wage = (Get_Amt_From_Textbox(Ref_Box)) / Bracketted_Value * (Hours_Per_Day/Hours_Multiplier);
            }
            Base_Hourly_Wage -= Base_Hourly_Wage * (Convert.ToDouble(income_tax_box.Text)) / 100;
                

            if (Ref_Box != daily_box) daily_box.TextChanged -= new System.EventHandler(box_TextChanged);
            if (Ref_Box != monthly_box) monthly_box.TextChanged -= new System.EventHandler(box_TextChanged);
            hourly_box.TextChanged -= new System.EventHandler(box_TextChanged);
            if (Ref_Box != yearly_box) yearly_box.TextChanged -= new System.EventHandler(box_TextChanged);
            if (Ref_Box != weekly_box) weekly_box.TextChanged -= new System.EventHandler(box_TextChanged);

            if (Ref_Box != hourly_box) hourly_box.Text = String.Format("{0:C}", (Base_Hourly_Wage));
            if (Ref_Box != daily_box) daily_box.Text = String.Format("{0:C}", (Base_Hourly_Wage * Bracketted_Value));
            if (Ref_Box != weekly_box) weekly_box.Text = String.Format("{0:C}", (Base_Hourly_Wage * Bracketted_Value * 5));
            if (Ref_Box != monthly_box) monthly_box.Text = String.Format("{0:C}", (Base_Hourly_Wage * Bracketted_Value * 21.741));
            if (Ref_Box != yearly_box) yearly_box.Text = String.Format("{0:C}", (Base_Hourly_Wage * Bracketted_Value * 21.741 * 12));


            if (Ref_Box != daily_box) daily_box.TextChanged += new System.EventHandler(box_TextChanged);
            if (Ref_Box != monthly_box) monthly_box.TextChanged += new System.EventHandler(box_TextChanged);
            hourly_box.TextChanged += new System.EventHandler(box_TextChanged);
            if (Ref_Box != yearly_box) yearly_box.TextChanged += new System.EventHandler(box_TextChanged);
            if (Ref_Box != weekly_box) weekly_box.TextChanged += new System.EventHandler(box_TextChanged);


            // Apply Income Tax
        }

        public double Get_Amt_From_Textbox(TextBox t, int substring_index = 1)
        {
            try
            {
                return Convert.ToDouble(t.Text.Substring(substring_index));
            }
            catch
            {
                return 0;
            }
        }


        private void monthly_box_TextChanged(object sender, EventArgs e) { }
        private void yearly_box_TextChanged(object sender, EventArgs e) { }
        private void weekly_box_TextChanged(object sender, EventArgs e) { }
        private void hourly_box_TextChanged(object sender, EventArgs e) { }
        private void daily_box_TextChanged(object sender, EventArgs e) { }

        private void box_TextChanged(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;
            Ref_Box = box;

            box.TextChanged -= new System.EventHandler(box_TextChanged);

            float num;
            bool isValid = float.TryParse(box.Text,
                NumberStyles.Currency,
                CultureInfo.GetCultureInfo("en-US"), // cached
                out num);

            if (!(box.Text.StartsWith("$")))
            {
                if (Get_Char_Count(box.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = box.Text;
                    box.Text = temp.Substring(1) + temp[0];
                    box.SelectionStart = box.Text.Length;
                    box.SelectionLength = 0;
                }
                else
                {
                    box.Text = "$" + box.Text;
                }
            }
            else if ((box.Text.Length > 1) && ((Get_Char_Count(box.Text, Convert.ToChar(".")) > 1) || (box.Text[1].ToString() == ".") || (Get_Char_Count(box.Text, Convert.ToChar("$")) > 1) || (!((box.Text.Substring(box.Text.Length - 1).All(char.IsDigit))) && !(box.Text[box.Text.Length - 1].ToString() == "."))))
            {
                box.TextChanged -= new System.EventHandler(box_TextChanged);
                box.Text = box.Text.Substring(0, box.Text.Length - 1);
                box.SelectionStart = box.Text.Length;
                box.SelectionLength = 0;
                box.TextChanged += new System.EventHandler(box_TextChanged);
            }
            else if (!isValid)
            {
                box.TextChanged -= new System.EventHandler(box_TextChanged);
                box.Text = "$0";
                box.TextChanged += new System.EventHandler(box_TextChanged);
            }
            else if (box.Text.Length > 1)
            {
            }

            box.TextChanged += new System.EventHandler(box_TextChanged);
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

        private void multiplier_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void hours_per_day_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void overtime_after_hours_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            Calculate();
        }

        private void income_tax_box_TextChanged(object sender, EventArgs e)
        {
            if (income_tax_box.Text.All(char.IsDigit))
            {
                Calculate();
            }
            else
            {
                // If letter in SO_number box, do not output and move CARET to end
                income_tax_box.Text = income_tax_box.Text.Substring(0, income_tax_box.Text.Length - 1);
                income_tax_box.SelectionStart = income_tax_box.Text.Length;
                income_tax_box.SelectionLength = 0;
            }
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

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (parent.Monthly_Income != Convert.ToDouble(monthly_box.Text.Substring(1)))
            {
                Grey_Out();
                using (var form = new Yes_No_Dialog(parent, "Are you sure you wish to set this as your current salary?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {

                        if (form.ReturnValue1 == "1")
                        {// Hours per day
                            if (parent.Settings_Dictionary.ContainsKey("WORK_HPD"))
                            {
                                parent.Settings_Dictionary["WORK_HPD"] = hours_per_day_box.Text;
                            }
                            else
                            {
                                parent.Settings_Dictionary.Add("WORK_HPD", hours_per_day_box.Text);
                            }

                            // Overtime Hours per day
                            if (parent.Settings_Dictionary.ContainsKey("WORK_OHPD"))
                            {
                                parent.Settings_Dictionary["WORK_OHPD"] = overtime_after_hours_box.Text;
                            }
                            else
                            {
                                parent.Settings_Dictionary.Add("WORK_OHPD", overtime_after_hours_box.Text);
                            }

                            // Overtime Multiplier
                            if (parent.Settings_Dictionary.ContainsKey("WORK_OMULTI"))
                            {
                                parent.Settings_Dictionary["WORK_OMULTI"] = multiplier_box.Text;
                            }
                            else
                            {
                                parent.Settings_Dictionary.Add("WORK_OMULTI", multiplier_box.Text);
                            }

                            // Overtime Multiplier
                            if (parent.Settings_Dictionary.ContainsKey("INCOME_TAX_RATE"))
                            {
                                parent.Settings_Dictionary["INCOME_TAX_RATE"] = income_tax_box.Text;
                            }
                            else
                            {
                                parent.Settings_Dictionary.Add("INCOME_TAX_RATE", income_tax_box.Text);
                            }

                            // Personal Income monthly
                            if (parent.Settings_Dictionary.ContainsKey("INCOME_MONTHLY"))
                            {
                                parent.Settings_Dictionary["INCOME_MONTHLY"] = monthly_box.Text.Substring(1);
                            }
                            else
                            {
                                parent.Settings_Dictionary.Add("INCOME_MONTHLY", monthly_box.Text.Substring(1));
                            }
                            parent.Monthly_Income = Convert.ToDouble(monthly_box.Text.Substring(1));

                            // Personal Income Weekly
                            if (parent.Settings_Dictionary.ContainsKey("INCOME_WEEKLY"))
                            {
                                parent.Settings_Dictionary["INCOME_WEEKLY"] = weekly_box.Text.Substring(1);
                            }
                            else
                            {
                                parent.Settings_Dictionary.Add("INCOME_WEEKLY", weekly_box.Text.Substring(1));
                            }

                            // Personal Income Daily
                            if (parent.Settings_Dictionary.ContainsKey("INCOME_DAILY"))
                            {
                                parent.Settings_Dictionary["INCOME_DAILY"] = daily_box.Text.Substring(1);
                            }
                            else
                            {
                                parent.Settings_Dictionary.Add("INCOME_DAILY", daily_box.Text.Substring(1));
                            }

                            // Personal Income Hourly
                            if (parent.Settings_Dictionary.ContainsKey("INCOME_HOURLY"))
                            {
                                parent.Settings_Dictionary["INCOME_HOURLY"] = hourly_box.Text.Substring(1);
                            }
                            else
                            {
                                parent.Settings_Dictionary.Add("INCOME_HOURLY", hourly_box.Text.Substring(1));
                            }

                            // Personal Income Yearly
                            if (parent.Settings_Dictionary.ContainsKey("INCOME_YEARLY"))
                            {
                                parent.Settings_Dictionary["INCOME_YEARLY"] = yearly_box.Text.Substring(1);
                            }
                            else
                            {
                                parent.Settings_Dictionary.Add("INCOME_YEARLY", yearly_box.Text.Substring(1));
                            }

                            // Update change time
                            if (parent.Settings_Dictionary.ContainsKey("INCOME_CHANGE_LOG"))
                            {
                                parent.Settings_Dictionary["INCOME_CHANGE_LOG"] += (parent.Settings_Dictionary["INCOME_CHANGE_LOG"] == "" ? (monthly_box.Text.Substring(1).Replace(",", "")) : "," + (monthly_box.Text.Substring(1)).Replace(",", "")) + "," + DateTime.Now.ToShortDateString();
                            }
                            else
                            {
                                parent.Settings_Dictionary.Add("INCOME_CHANGE_LOG", monthly_box.Text.Substring(1).Replace(",", "") + "," + DateTime.Now.ToShortDateString());
                            }
                        }
                        else
                        {
                            // Restore saved defaults
                            monthly_box.Text = "$" + (parent.Settings_Dictionary.ContainsKey("INCOME_MONTHLY") ? parent.Settings_Dictionary["INCOME_MONTHLY"] : "0"); // monthly income
                            hourly_box.Text = "$" + (parent.Settings_Dictionary.ContainsKey("INCOME_HOURLY") ? parent.Settings_Dictionary["INCOME_HOURLY"] : "0");
                            weekly_box.Text = "$" + (parent.Settings_Dictionary.ContainsKey("INCOME_WEEKLY") ? parent.Settings_Dictionary["INCOME_WEEKLY"] : "0");
                            yearly_box.Text = "$" + (parent.Settings_Dictionary.ContainsKey("INCOME_YEARLY") ? parent.Settings_Dictionary["INCOME_YEARLY"] : "0");
                            daily_box.Text = "$" + (parent.Settings_Dictionary.ContainsKey("INCOME_DAILY") ? parent.Settings_Dictionary["INCOME_DAILY"] : "0");

                            hours_per_day_box.Text = parent.Settings_Dictionary.ContainsKey("WORK_HPD") ? parent.Settings_Dictionary["WORK_HPD"] : "8";
                            overtime_after_hours_box.Text = parent.Settings_Dictionary.ContainsKey("WORK_OHPD") ? parent.Settings_Dictionary["WORK_OHPD"] : "9";
                            multiplier_box.Text = parent.Settings_Dictionary.ContainsKey("WORK_OMULTI") ? parent.Settings_Dictionary["WORK_OMULTI"] : "1.5";
                            income_tax_box.Text = parent.Settings_Dictionary.ContainsKey("INCOME_TAX_RATE") ? parent.Settings_Dictionary["INCOME_TAX_RATE"] : "0";
                        }
                    }
                }
                Grey_In();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            hourly_box.Text = "$0";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Calculate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Visible = false;
            parent.Settings_Dictionary["INCOME_MANUAL"] = "1";
            Salary_Manual SM = new Salary_Manual(parent, parent.Location, parent.Size);
            SM.ShowDialog();
            SM.Activate();
            this.Close();
        }
    }
}
