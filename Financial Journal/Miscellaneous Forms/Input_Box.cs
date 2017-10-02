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
    public partial class Input_Box : Form
    {
        Receipt parent;
        int Start_Location_Offset = 25;
        object Pass_Object = null;

        public Input_Box(Receipt _parent, string Label_String, string Preset_Text = "", object J = null, Point g = new Point(), Size s = new Size())
        {
            /*
            if (Label_String.Contains("Set total discount amount"))
            {
                Start_Location_Offset = 250;
                this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset/2);
            }
            else
            {
                this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset);
            }
            */


            InitializeComponent();
            if (Label_String.Contains("Add new category:"))
            {
                label5.Text = "Add new Category";
            }
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            label2.Text = Label_String;
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            input.Text = Preset_Text;

            // Pass through allows for input within input (Add link, open add category input box)
            Pass_Object = J;

            if (label2.Text.Contains("Set total discount amount") || label2.Text.Contains("profile name"))
            {
                close_button.Visible = false;
                this.Size = new Size(181, 113);
                if (label2.Text.Contains("profile name"))
                    Add_button.Text = label2.Text.Contains("copy") ? "Add" : "Edit";
            }
            else if (label2.Text.Contains("emo"))
            {
                input.MaxLength = 70;
            }

            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // If location, enable link modes
            if (label2.Text.Contains("existing tax"))
            {
                button1.Visible = true;
                if (parent.Tax_Exempt_Order)
                {
                    button1.Text = "Tax Order";
                }
            }
            else if (label2.Text.Contains("ocation"))
            {
                link_box.Items.Add("");
                foreach (string g in parent.category_box.Items)
                {
                    link_box.Items.Add(g);
                }

                link_box.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
                label3.Visible = true;
                link_box.Text = "";// link_box.Items[0].ToString();
            }

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

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (input.Text.Length > 0)
            {
                if (label2.Text.Contains("profile name"))
                {
                    try
                    {
                        parent.Pass_Through_String = input.Text;
                    }
                    catch
                    {
                    }
                }
                else if (label2.Text.Contains("Set total discount amount"))
                { 
                    try
                    {
                        parent.Discount_Transfer_Amount = Convert.ToDouble(input.Text);
                    }
                    catch
                    {
                    }
                }
                else if (label2.Text.Contains("ocation"))
                {
                    bool Contains_Location = false;
                    foreach (Company g in parent.Company_List)
                    {
                        if (g.Name == input.Text)
                        {
                            Contains_Location = true;
                        }
                    }

                    if (!Contains_Location)
                    {
                        parent.Location_List.Add(new Location() { Name = input.Text, Refund_Days = 0});
                        parent.Company_List.Add(new Company() { Name = input.Text });
                        parent.location_box.Items.Add(input.Text);
                        parent.location_box.Text = input.Text;
                        parent.location_box.Focus();
                    }
                    else
                    {
                        Grey_Out();
                        Form_Message_Box FMB = new Form_Message_Box(parent, "Location exists already", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                        Grey_In();
                    }

                    if (parent.Link_Location.ContainsKey(input.Text) && link_box.Text.Length > 0)
                    {
                        Grey_Out();
                        Form_Message_Box FMB = new Form_Message_Box(parent, "Location already has existing link", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                        Grey_In();
                    }
                    else if (link_box.Text.Length > 0)
                    {
                        parent.Link_Location.Add(input.Text, link_box.Text); 
                        parent.location_box.Text = link_box.Text; 
                        parent.item_desc.Focus();
                        parent.category_box.Text = link_box.Text;
                    }
                }
                else if (label2.Text.Contains("ategory"))
                {
                    if (parent.Category_List.Contains(input.Text))
                    {
                        Grey_Out();
                        // Get monthly income and compare
                        Form_Message_Box FMB = new Form_Message_Box(parent, "Category Exists Already", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                        Grey_In();
                    }
                    else
                    {
                        parent.Category_List.Add(input.Text);
                        parent.category_box.Items.Add(input.Text);
                        parent.category_box.Text = input.Text;
                        parent.category_box.Focus();
                        if (Pass_Object is Input_Box)
                        {
                            parent.Pass_Through_String = parent.Remove_Character(input.Text, ',');
                        }
                    }
                }
                else if (label2.Text.Contains("memo") && label2.Text.Contains("item"))
                {
                    parent.Temp_Memo = input.Text;
                }
                else if (label2.Text.Contains("memo") && label2.Text.Contains("order"))
                {
                    parent.Order_Memo = input.Text;
                }
                else
                {
                    try
                    {
                        parent.Tax_Exempt_Order = false;
                        parent.Tax_Override_Amt = Convert.ToDouble(input.Text);
                        parent.Invalidate();
                        parent.Update();
                    }
                    catch
                    {
                    }
                }
                this.Close();
                parent.Focus();
            }
            else if (label2.Text.Contains("Set total discount amount"))
            {
                parent.Discount_Transfer_Amount = 0;
                this.Close();
                parent.Focus();
            }
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

        // tax exempt button
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "Tax Order")
            {
                parent.Tax_Exempt_Order = false;
            }
            else
            {
                parent.Tax_Exempt_Order = true;
            }
            parent.Invalidate();
            parent.Update();
            parent.Focus();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Input_Box IB = new Input_Box(parent, "Add new category:", "", this, this.Location, this.Size);
            IB.ShowDialog();
            Grey_In();
            
            link_box.Items.Clear();
            foreach (string g in parent.category_box.Items)
            {
                link_box.Items.Add(g);
            }
            if (parent.Pass_Through_String.Length > 0) link_box.Text = parent.Pass_Through_String;

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

        private void button3_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Form_Message_Box FMB = new Form_Message_Box(parent, "By linking a category, the category will be automatically selected when adding new items", true, 0, this.Location, this.Size);
            FMB.ShowDialog();
            Grey_In();
        }

        private void input_TextChanged(object sender, EventArgs e)
        {

        }
    }
}