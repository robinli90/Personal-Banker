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
    public partial class Hobby_MGMT_Settings : Form
    {

        private List<Button> Delete_Item_Buttons = new List<Button>();
        private List<Button> Edit_Item_Button = new List<Button>();
        private List<Button> Duplicate_Item_Button = new List<Button>();

        protected override void OnPaint(PaintEventArgs e)
        {
            
            int data_height = 26;
            int start_height = label6.Top + 40;
            int start_margin = 15;              // Item
            int height_offset = 9;

            int margin1 = start_margin + 80;   //Price

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
            Font f_italic = new Font("MS Reference Sans Serif", 9, FontStyle.Italic);


            Delete_Item_Buttons.ForEach(button => button.Image.Dispose());
            Delete_Item_Buttons.ForEach(button => button.Dispose());
            Delete_Item_Buttons.ForEach(button => this.Controls.Remove(button));
            Delete_Item_Buttons = new List<Button>();
            Edit_Item_Button.ForEach(button => button.Image.Dispose());
            Edit_Item_Button.ForEach(button => button.Dispose());
            Edit_Item_Button.ForEach(button => this.Controls.Remove(button));
            Edit_Item_Button = new List<Button>();
            Duplicate_Item_Button.ForEach(button => button.Image.Dispose());
            Duplicate_Item_Button.ForEach(button => button.Dispose());
            Duplicate_Item_Button.ForEach(button => this.Controls.Remove(button));
            Duplicate_Item_Button = new List<Button>();

            e.Graphics.DrawLine(Grey_Pen, start_margin, start_height - 65, this.Width - 15, start_height - 65);
            e.Graphics.DrawLine(Grey_Pen, start_margin, start_height, this.Width - 15, start_height);

            height_offset += 9;
            e.Graphics.DrawString("Profile Name", f_header, WritingBrush, start_margin + 15, start_height + height_offset + (row_count * data_height));
            row_count++;

            int item_index = 0;

            foreach (string g in parent.Hobby_Profile_List)
            {
                ToolTip ToolTip1 = new ToolTip();
                ToolTip1.InitialDelay = 1;
                ToolTip1.ReshowDelay = 1;

                Button delete_button = new Button();
                delete_button.BackColor = this.BackColor;
                delete_button.ForeColor = this.BackColor;
                delete_button.FlatStyle = FlatStyle.Flat;
                delete_button.Image = global::Financial_Journal.Properties.Resources.delete;
                delete_button.Enabled = true;
                delete_button.Size = new Size(29, 29);
                delete_button.Location = new Point(this.Width - 50, start_height + height_offset + (row_count * data_height) - 6);
                delete_button.Name = "d" + item_index.ToString();
                delete_button.Text = "";
                delete_button.Click += new EventHandler(this.dynamic_button_click);
                //if (payment.Total > 0) delete_button.Enabled = false;
                Delete_Item_Buttons.Add(delete_button);
                ToolTip1.SetToolTip(delete_button, "Delete " + g);
                this.Controls.Add(delete_button);

                Button Edit_Button = new Button();
                Edit_Button.BackColor = this.BackColor;
                Edit_Button.ForeColor = this.BackColor;
                Edit_Button.FlatStyle = FlatStyle.Flat;
                Edit_Button.Image = global::Financial_Journal.Properties.Resources.edit;
                Edit_Button.Enabled = true;
                Edit_Button.Size = new Size(29, 29);
                Edit_Button.Location = new Point(this.Width - 85, start_height + height_offset + (row_count * data_height) - 6);
                Edit_Button.Name = "e" + item_index.ToString();
                Edit_Button.Text = "";
                Edit_Button.Click += new EventHandler(this.dynamic_button_click);
                //if (payment.Total > 0) Edit_Button.Enabled = false;
                Edit_Item_Button.Add(Edit_Button);
                ToolTip1.SetToolTip(Edit_Button, "Edit " + g);
                this.Controls.Add(Edit_Button);

                Button Duplicate_Button = new Button();
                Duplicate_Button.BackColor = this.BackColor;
                Duplicate_Button.ForeColor = this.BackColor;
                Duplicate_Button.FlatStyle = FlatStyle.Flat;
                Duplicate_Button.Image = global::Financial_Journal.Properties.Resources.duplicate;
                Duplicate_Button.Enabled = true;
                Duplicate_Button.Size = new Size(29, 29);
                Duplicate_Button.Location = new Point(this.Width - 120, start_height + height_offset + (row_count * data_height) - 6);
                Duplicate_Button.Name = "c" + item_index.ToString();
                Duplicate_Button.Text = "";
                Duplicate_Button.Click += new EventHandler(this.dynamic_button_click);
                //if (payment.Total > 0) Duplicate_Button.Enabled = false;
                Duplicate_Item_Button.Add(Duplicate_Button);
                ToolTip1.SetToolTip(Duplicate_Button, "Duplicate " + g);
                this.Controls.Add(Duplicate_Button);

                e.Graphics.DrawString(g, f, WritingBrush, start_margin + 15, start_height + height_offset + (row_count * data_height));
                row_count++;
                item_index++;
            }
            height_offset += 18;

            this.Height = start_height + height_offset + row_count * data_height;

            TFLP.Size = new Size(this.Width - 2, this.Height - 2);

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
            f_italic.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        private void dynamic_button_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            Expenses Ref_Expense = new Expenses();

            int Profile_Index = Convert.ToInt32(b.Name.Substring(1));

            if (b.Name.StartsWith("d")) // delete
            {
                Grey_Out();
                using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to delete this profile?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                {
                    var result21 = form1.ShowDialog();
                    if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                    {

                        parent.Hobby_Profile_List.RemoveAt(Profile_Index);
                        parent.Master_Container_Dict.Remove((Profile_Index + 1).ToString());
                        parent.Master_Hobby_Item_List = parent.Master_Hobby_Item_List.Where(x => x.Profile_Number != (Profile_Index + 1).ToString()).ToList();

                        //int Removed_Profile_Index = Profile_Index + 1;

                        // Get all the indices in the dictionary
                        List<int> KVP_Indices = new List<int>();
                        foreach (KeyValuePair<string, List<Container>> KVP in parent.Master_Container_Dict)
                        {
                            KVP_Indices.Add(Convert.ToInt32(KVP.Key));
                        }

                        KVP_Indices = KVP_Indices.OrderBy(x => x).ToList();

                        for (int i = 0; i < KVP_Indices.Count; i++)
                        {
                            if (KVP_Indices[i] > Profile_Index) //Removed_Profile_Index);
                            {
                                // Shift one index down for hobby items
                                foreach (Hobby_Item HI in parent.Master_Hobby_Item_List)
                                {
                                    // If greater than removed profile, reduce profile number by one
                                    if (HI.Profile_Number == KVP_Indices[i].ToString())
                                    {
                                        HI.Profile_Number = (KVP_Indices[i] - 1).ToString();
                                    }
                                }

                                // Move one index down for dictionary
                                parent.Master_Container_Dict.Add((KVP_Indices[i] - 1).ToString(), parent.Master_Container_Dict[(KVP_Indices[i].ToString())]);
                                parent.Master_Container_Dict.Remove(KVP_Indices[i].ToString());
                            }
                        }

                        Hobby_Parent.Load_Complete = false;
                        Hobby_Parent.Reset_Panels();
                        Hobby_Parent.Populate_Profiles();
                        Invalidate();
                    }
                }
                Grey_In();
            }
            else if (b.Name.StartsWith("e")) // edit
            {
                Grey_Out();
                string original = parent.Hobby_Profile_List[Profile_Index];
                Input_Box IB = new Input_Box(parent, "Edit profile name", parent.Hobby_Profile_List[Profile_Index], this, this.Location, this.Size);
                IB.ShowDialog();
                if (parent.Pass_Through_String != original)
                {
                    while (parent.Hobby_Profile_List.Contains(parent.Pass_Through_String) && original != parent.Pass_Through_String)
                    {
                        Grey_Out();
                        Form_Message_Box FMB = new Form_Message_Box(parent, "This is in use. Please choose another profile name", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                        IB = new Input_Box(parent, "Edit profile name", parent.Hobby_Profile_List[Profile_Index], this, this.Location, this.Size);
                        IB.ShowDialog();
                        Grey_In();
                    }
                    parent.Hobby_Profile_List[Profile_Index] = parent.Pass_Through_String;
                    Hobby_Parent.Reset_Panels();
                    Hobby_Parent.Populate_Profiles();
                    Invalidate();
                }
                Grey_In();
            }
            else if (b.Name.StartsWith("c")) // duplicate
            {
                Grey_Out();
                string original = parent.Hobby_Profile_List[Profile_Index];
                Input_Box IB = new Input_Box(parent, "Choose copy profile name", parent.Hobby_Profile_List[Profile_Index] + " - Copy", this, this.Location, this.Size);
                IB.ShowDialog();
                if (parent.Pass_Through_String != original)
                {
                    while (parent.Hobby_Profile_List.Contains(parent.Pass_Through_String))
                    {
                        Grey_Out();
                        Form_Message_Box FMB = new Form_Message_Box(parent, "This is in use. Please choose another copy profile name", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                        IB = new Input_Box(parent, "Choose copy profile name", parent.Hobby_Profile_List[Profile_Index], this, this.Location, this.Size);
                        IB.ShowDialog();
                        Grey_In();
                    }

                    // Create new hobby list with edit name
                    parent.Hobby_Profile_List.Add(parent.Pass_Through_String);

                    // Get all the items to copy using memberwise clone
                    List<Hobby_Item> Copy_Items = new List<Hobby_Item>();
                    parent.Master_Hobby_Item_List.ForEach(x => Copy_Items.Add(x.Copy_Item()));

                    // Filter list to only relevant items
                    Copy_Items = Copy_Items.Where(x => x.Profile_Number == (Profile_Index + 1).ToString()).ToList();

                    // Set new profile ID
                    Copy_Items.ForEach(x => x.Profile_Number = (parent.Master_Container_Dict.Count() + 1).ToString());

                    // Append new list to parent list
                    parent.Master_Hobby_Item_List.AddRange(Copy_Items);

                    // Copy container information
                    List<Container> Container_List = parent.Master_Container_Dict[(Profile_Index + 1).ToString()];

                    // Create new container with new index
                    parent.Master_Container_Dict.Add((parent.Master_Container_Dict.Count() + 1).ToString(), Container_List);

                    Hobby_Parent.Reset_Panels();
                    Hobby_Parent.Populate_Profiles();
                    Invalidate();
                }
                Grey_In();
            }
        }

        /*
         * Resizing form
         * 
        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;

        protected override void OnPaint(PaintEventArgs e) {
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
            rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
            //e.Graphics.FillRectangle(Brushes.DarkBlue, rc);
        }

        protected override void WndProc(ref Message m) 
        {
            if (m.Msg == 0x84) {  // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption) {
                    m.Result = (IntPtr)2;  // HTCAPTION
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip) {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }
            }
            base.WndProc(ref m);
        }
        */

        public string ReturnValue = "0";
        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;
        Hobby_Management Hobby_Parent;

        public Hobby_MGMT_Settings(Receipt _parent, Hobby_Management HM, Point g = new Point(), Size s = new Size())
        {
            Hobby_Parent = HM;

            //this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;
            ToolTip1.SetToolTip(Add_button, "Apply changes");
            ToolTip1.SetToolTip(search_desc_button, "Reset to default");
            ToolTip1.SetToolTip(button1, "Reset to default");

            width.Text = parent.Settings_Dictionary["HOBBY_MGMT_X"];
            height.Text = parent.Settings_Dictionary["HOBBY_MGMT_Y"];

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

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            this.ReturnValue = "0";
            //this.ReturnValue2 = DateTime.Now.ToString(); //example
            this.DialogResult = DialogResult.OK;
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

        private void Add_button_Click(object sender, EventArgs e)
        {
            // Adjust minimum values
            if (Convert.ToInt32(width.Text) < 100) width.Text = "150";
            if (Convert.ToInt32(height.Text) < 100) height.Text = "150";

            parent.Settings_Dictionary["HOBBY_MGMT_X"] = width.Text;
            parent.Settings_Dictionary["HOBBY_MGMT_Y"] = height.Text;

            this.ReturnValue = "1";
            //this.ReturnValue2 = DateTime.Now.ToString(); //example
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void width_TextChanged(object sender, EventArgs e)
        {
            if (width.Text.All(char.IsDigit))
            {
            }
            else
            {
                // If letter in SO_number box, do not output and move CARET to end
                width.Text = width.Text.Substring(0, width.Text.Length - 1);
                width.SelectionStart = width.Text.Length;
                width.SelectionLength = 0;
            }
        }

        private void height_TextChanged(object sender, EventArgs e)
        {
            if (height.Text.All(char.IsDigit))
            {
            }
            else
            {
                // If letter in SO_number box, do not output and move CARET to end
                height.Text = height.Text.Substring(0, height.Text.Length - 1);
                height.SelectionStart = height.Text.Length;
                height.SelectionLength = 0;
            }
        }

        private void search_desc_button_Click(object sender, EventArgs e)
        {
            width.Text = "250";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            height.Text = "305";
        }

        private void add_profile_Click(object sender, EventArgs e)
        {
            if (profile.Text.Length > 0)
            {
                if (parent.Hobby_Profile_List.Contains(profile.Text))
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "You already have a profile with the same name", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }
                else
                {
                    parent.Hobby_Profile_List.Add(profile.Text);
                    parent.Master_Container_Dict.Add((parent.Master_Container_Dict.Count() + 1).ToString(), new List<Container>());
                }
                profile.Text = "";
                Invalidate();
                Hobby_Parent.Populate_Profiles();
            }
        }
    }
}
