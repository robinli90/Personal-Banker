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
    public partial class Input_Box_Small : Form
    {
        Receipt parent;
        int Start_Location_Offset = 25;
        object Pass_Object = null;

        public string Pass_String = "";

        public Input_Box_Small(Receipt _parent, string Label_String, string Preset_Text = "", string Button_Text = "Add", object J = null, Point g = new Point(), Size s = new Size(), int Grow_Height = 0, bool showClose = false)
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
            parent = _parent;

            input.Text = Preset_Text;

            if (Label_String.Contains("Search for item"))
            {
                label5.Text = "Item Lookup";
                close_button.Visible = true;

                #region Populate Autocomplete
                //SuggestStrings will have the logic to return array of strings either from cache/db
                string[] arr = SuggestStrings("");
                AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                collection.AddRange(arr);
                input.AutoCompleteCustomSource = collection;
                input.AutoCompleteSource = AutoCompleteSource.CustomSource;
                #endregion
            }

            if (Label_String.Contains("Add New Asset Category using name"))
            {
                label5.Text = "Add new Asset Category";
                close_button.Visible = true;
            }

            if (showClose)
            {
                close_button.Visible = true;
            }

            else if (Label_String.Contains("Please enter recipient email address") || Label_String.Contains("Please enter a password for the export file"))
            {
                close_button.Visible = true;
            }
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            label2.Text = Label_String;
            Set_Form_Color(parent.Frame_Color);
            Add_button.Text = Button_Text;

            // Pass through allows for input within input (Add link, open add category input box)
            Pass_Object = J;

            if (label2.Text.Contains("Set total discount amount") || label2.Text.Contains("profile name"))
            {
                close_button.Visible = false;
                minimize_button.Visible = false;
                this.Size = new Size(181, 113);
                if (label2.Text.Contains("profile name"))
                    Add_button.Text = label2.Text.Contains("copy") ? "Add" : "Edit";
            }
            else if (label2.Text.Contains("emo"))
            {
                input.MaxLength = 70;
            }

            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
            this.Height += Grow_Height;
        }


        private string[] SuggestStrings(string text)
        {
            return parent.Master_Item_List.Select(x => x.Name).Distinct().ToArray();
        }

        private void Receipt_Load(object sender, EventArgs e)
        {

            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            this.input.KeyUp += input_KeyUp;
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

            if (label2.Text.Contains("assword"))
            {
                input.PasswordChar = '*';
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


        // If press enter on length box, activate add (nmemonics)
        private void input_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox g = (TextBox)sender;
            if (e.KeyCode == Keys.Enter)
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
                this.Pass_String = input.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
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


        private void button2_Click(object sender, EventArgs e)
        {
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
        }

        private void input_TextChanged(object sender, EventArgs e)
        {

        }
    }
}