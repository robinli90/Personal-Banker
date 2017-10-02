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
    public partial class Contacts : Form
    {
        public bool hasImported = false;
        public List<Contact> Pre_Import_Contact_List = new List<Contact>();

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                return cp;
            }
        } 

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        public List<Contact> CO_Filtered_List = new List<Contact>();
        private List<Button> Interactive_Button_List = new List<Button>();
        bool Paint = false;
        public bool repaint_buttons = true;

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            if (repaint_buttons)
            {

                Interactive_Button_List.ForEach(button => button.Image.Dispose());
                Interactive_Button_List.ForEach(button => button.Dispose());
                Interactive_Button_List.ForEach(button => bufferedPanel3.Controls.Remove(button));
                Interactive_Button_List = new List<Button>();
            }

            //if (Paint)
            if (true)
            {

                e.Graphics.TranslateTransform(bufferedPanel3.AutoScrollPosition.X, bufferedPanel3.AutoScrollPosition.Y);

                var panel = bufferedPanel3;
                int data_height = 16;
                int start_height = 0;
                int start_margin = 15;              // Item
                int height_offset = 9;
                int row_count = 0;

                int main_margin = start_margin + 50;

                Color DrawForeColor = Color.White;
                Color BackColor = Color.FromArgb(64, 64, 64);
                Color HighlightColor = Color.FromArgb(76, 76, 76);

                SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
                SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(88, 88, 88));
                SolidBrush RedBrush = new SolidBrush(Color.FromArgb(150, 150, 150));
                SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
                Pen p = new Pen(WritingBrush, 1);
                Pen Grey_Pen = new Pen(GreyBrush, 1);

                Font f_asterisk = new Font("MS Reference Sans Serif", 7, FontStyle.Regular);
                Font f = new Font("MS Reference Sans Serif", 8, FontStyle.Regular);
                Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
                Font f_total = new Font("MS Reference Sans Serif", 8, FontStyle.Bold);
                Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);
                Font f_italic = new Font("MS Reference Sans Serif", 9, FontStyle.Italic);
                Font f_9_bold = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
                Font f_10_bold = new Font("MS Reference Sans Serif", 10, FontStyle.Bold);
                Font f_10 = new Font("MS Reference Sans Serif", 10, FontStyle.Regular);
                Font f_9 = new Font("MS Reference Sans Serif", 9, FontStyle.Regular);
                Font f_14_bold = new Font("MS Reference Sans Serif", 14, FontStyle.Bold);
                Font f_12_bold = new Font("MS Reference Sans Serif", 11, FontStyle.Bold);

                int Contact_Count = 0;

                string alphabet = "abcdefghijklmnopqrstuvwxyz";

                foreach (char c in alphabet)
                {
                    height_offset += 9;
                    List<Contact> Temp = parent.Contact_List.Where(x => x.First_Name.Length > 0 && x.First_Name.ToLower()[0] == c).ToList();
                    Temp = Temp.OrderBy(x => x.First_Name).ToList();
                    e.Graphics.DrawLine(Grey_Pen, start_margin, start_height + height_offset + (row_count * data_height), bufferedPanel1.Width - 50, start_height + height_offset + (row_count * data_height));
                    e.Graphics.DrawString(c.ToString().ToUpper(), f_total, RedBrush, bufferedPanel1.Width - 40, start_height + height_offset + (row_count * data_height) - 8);
                    height_offset += 9;

                    foreach (Contact C in Temp)
                    {
                        if (repaint_buttons)
                        {
                            ToolTip ToolTip1 = new ToolTip();
                            ToolTip1.InitialDelay = 1;
                            ToolTip1.ReshowDelay = 1;

                            // icon
                            Button Interactive_Button = new Button();
                            Interactive_Button.BackColor = this.BackColor;
                            Interactive_Button.ForeColor = parent.Frame_Color;
                            Interactive_Button.FlatStyle = FlatStyle.Flat;
                            Interactive_Button.Image = Get_Association_Icon(C.Association);
                            Interactive_Button.Size = new Size(35, 35);
                            Interactive_Button.Location = new Point(main_margin - 40, start_height + height_offset + (row_count * data_height));
                            //Interactive_Button.AutoScrollOffset = Interactive_Button.Location;
                            Interactive_Button.Name = "i" + Contact_Count.ToString();
                            Interactive_Button.Text = "";
                            Interactive_Button.Click += new EventHandler(this.dynamic_button_click);
                            Interactive_Button_List.Add(Interactive_Button);
                            //Interactive_Button.Anchor = AnchorStyles.Top;
                            ToolTip1.SetToolTip(Interactive_Button, C.Association);
                            bufferedPanel3.Controls.Add(Interactive_Button);

                            if (Editing_Contacts)
                            {
                                //delete
                                Interactive_Button = new Button();
                                Interactive_Button.BackColor = this.BackColor;
                                Interactive_Button.ForeColor = this.BackColor;
                                Interactive_Button.FlatStyle = FlatStyle.Flat;
                                Interactive_Button.Image = global::Financial_Journal.Properties.Resources.delete;
                                Interactive_Button.Size = new Size(28, 30);
                                Interactive_Button.Location = new Point(bufferedPanel3.Width - 80, start_height + height_offset + (row_count * data_height));
                                //Interactive_Button.AutoScrollOffset = Interactive_Button.Location;
                                Interactive_Button.Name = "d" + Contact_Count.ToString();
                                Interactive_Button.Text = "";
                                Interactive_Button.Click += new EventHandler(this.dynamic_button_click);
                                Interactive_Button_List.Add(Interactive_Button);
                                //Interactive_Button.Anchor = AnchorStyles.Top;
                                ToolTip1.SetToolTip(Interactive_Button, "Delete " + C.First_Name + " " + C.Last_Name);
                                bufferedPanel3.Controls.Add(Interactive_Button);

                                //edit
                                Interactive_Button = new Button();
                                Interactive_Button.BackColor = this.BackColor;
                                Interactive_Button.ForeColor = this.BackColor;
                                Interactive_Button.FlatStyle = FlatStyle.Flat;
                                Interactive_Button.Image = global::Financial_Journal.Properties.Resources.edit;
                                Interactive_Button.Size = new Size(28, 30);
                                Interactive_Button.Location = new Point(bufferedPanel3.Width - 110, start_height + height_offset + (row_count * data_height));
                                //Interactive_Button.AutoScrollOffset = Interactive_Button.Location;
                                Interactive_Button.Name = "e" + Contact_Count.ToString();
                                Interactive_Button.Text = "";
                                Interactive_Button.Click += new EventHandler(this.dynamic_button_click);
                                Interactive_Button_List.Add(Interactive_Button);
                                //Interactive_Button.Anchor = AnchorStyles.Top;
                                ToolTip1.SetToolTip(Interactive_Button, "Edit " + C.First_Name + " " + C.Last_Name);
                                bufferedPanel3.Controls.Add(Interactive_Button);
                            }

                            if (Link_Name.Length > 0)  // if linking
                            {
                                //LINKING PERSON
                                Interactive_Button = new Button();
                                Interactive_Button.BackColor = this.BackColor;
                                Interactive_Button.ForeColor = this.BackColor;
                                Interactive_Button.FlatStyle = FlatStyle.Flat;
                                Interactive_Button.Image = global::Financial_Journal.Properties.Resources.manager;
                                Interactive_Button.Size = new Size(28, 30);
                                Interactive_Button.Location = new Point(bufferedPanel3.Width - 140 + (Editing_Contacts ? 0 : 60), start_height + height_offset + (row_count * data_height));
                                //Interactive_Button.AutoScrollOffset = Interactive_Button.Location;
                                Interactive_Button.Name = "l" + Contact_Count.ToString();
                                Interactive_Button.Text = "";
                                Interactive_Button.Click += new EventHandler(this.dynamic_button_click);
                                Interactive_Button_List.Add(Interactive_Button);
                                //Interactive_Button.Anchor = AnchorStyles.Top;
                                ToolTip1.SetToolTip(Interactive_Button, "Link " + C.First_Name + " " + C.Last_Name);
                                bufferedPanel3.Controls.Add(Interactive_Button);
                            }

                            Contact_Count++;
                        }


                        e.Graphics.DrawString(C.First_Name + " " + C.Last_Name, f_total, WritingBrush, main_margin, start_height + height_offset + (row_count * data_height));
                        row_count++;
                        // Below line
                        int ref_line = row_count;
                        if (C.Phone_No_Primary.Length > 0)
                        {
                            e.Graphics.DrawString("Primary #: " + C.Phone_No_Primary, f, WritingBrush, main_margin + 10, start_height + height_offset + (row_count * data_height));
                            row_count++;
                        } 
                        if (C.Phone_No_Second.Length > 0)
                        {
                            e.Graphics.DrawString("Secondary #: " + C.Phone_No_Second, f, WritingBrush, main_margin + 10, start_height + height_offset + (row_count * data_height));
                            row_count++;
                        }
                        if (C.Email.Length > 0)
                        {
                            e.Graphics.DrawString("Email: " + C.Email, f, WritingBrush, main_margin + 10, start_height + height_offset + (row_count * data_height));
                            row_count++;
                            
                            if (C.Email_Second.Length > 0)
                            {
                                e.Graphics.DrawString("Other Email: " + C.Email_Second, f, WritingBrush, main_margin + 10, start_height + height_offset + (row_count * data_height));
                                row_count++;
                            }
                        }
                        // Add spacing if no detail provided above.
                        if (row_count == ref_line)
                            row_count++;

                        // Add offset between contacts
                        if (C != Temp[Temp.Count - 1]) height_offset += 10;
                    }
                }

                // Resize panel
                bufferedPanel1.AutoScrollMinSize = new Size(bufferedPanel3.Width, start_height + height_offset + row_count * data_height);
                bufferedPanel3.Height = new Size(bufferedPanel1.Width, start_height + height_offset + row_count * data_height).Height;

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

                Paint = false;
                repaint_buttons = false;
            }
        }


        private void dynamic_button_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            if (b.Name.StartsWith("d") || b.Name.StartsWith("e") || b.Name.StartsWith("i") || b.Name.StartsWith("l"))
            {
                Contact Ref_C = parent.Contact_List.OrderBy(x => x.First_Name[0]).ToList().OrderBy(z => z.First_Name).ToList()[Convert.ToInt32(b.Name.Substring(1))];
                if (b.Name.StartsWith("d")) // delete
                {
                    Editing = false;
                    Grey_Out();
                    using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to delete " + Ref_C.First_Name + " " + Ref_C.Last_Name + "?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                    {
                        var result21 = form1.ShowDialog();
                        if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                        {
                            #region Remove from CE/AI/SI hash values

                            // Remove from CE
                            foreach (Calendar_Events CE in parent.Calendar_Events_List)
                                if (Ref_C.Hash_Value == CE.Contact_Hash_Value)
                                    CE.Contact_Hash_Value = "";

                            // Remove from AI/SI
                            foreach (Agenda_Item AI in parent.Agenda_Item_List)
                            {
                                if (Ref_C.Hash_Value == AI.Contact_Hash_Value)
                                {
                                    AI.Contact_Hash_Value = "";
                                }
                                foreach (Shopping_Item SI in AI.Shopping_List)
                                {
                                    if (Ref_C.Hash_Value == SI.Contact_Hash_Value)
                                    {
                                        SI.Contact_Hash_Value = "";
                                    }
                                }
                            }
                            #endregion

                            parent.Contact_List.Remove(Ref_C);
                            repaint_buttons = true;
                            Invalidate();
                            bufferedPanel3.Invalidate();
                        }
                    }
                    Grey_In();
                }
                else if (b.Name.StartsWith("e")) //edit
                {
                    Add_button.Image = global::Financial_Journal.Properties.Resources.floppy;

                    // Disable all other buttons
                    if (!bufferedPanel2.Visible) button1.PerformClick();
                    Editing = true;
                    Ref_Contact = Ref_C;
                    email.Text = Ref_C.Email;
                    email2.Text = Ref_C.Email_Second;
                    firstname.Text = Ref_C.First_Name;
                    lastname.Text = Ref_C.Last_Name;
                    primary.Text = Ref_C.Phone_No_Primary;
                    secondary.Text = Ref_C.Phone_No_Second;
                    association.Text = Ref_C.Association;

                }
                else if (b.Name.StartsWith("i")) //index
                {
                    Ref_C.Cycle_Association();
                    b.Image = Get_Association_Icon(Ref_C.Association);
                }
                else if (b.Name.StartsWith("l")) //link
                {
                    Grey_Out();
                    using (var form1 = new Yes_No_Dialog(parent, "Are you sure you want to link " + Ref_C.First_Name + " " + Ref_C.Last_Name + " with " + Link_Name + "?", "Warning", "No", "Yes", 15, this.Location, this.Size))
                    {
                        var result21 = form1.ShowDialog();
                        if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                        {
                            this.Return_Contact = Ref_C;
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                    }
                    Grey_In();
                }
            }
        }

        public Contact Return_Contact;

        private Image Get_Association_Icon(string Association)
        {
            switch (Association)
            {
                case "Friend":
                    return global::Financial_Journal.Properties.Resources.C_Friends;
                case "Family":
                    return global::Financial_Journal.Properties.Resources.C_Family;
                case "Work":
                    return global::Financial_Journal.Properties.Resources.C_Work;
                case "Service":
                    return global::Financial_Journal.Properties.Resources.C_Service;
                case "Other":
                    return global::Financial_Journal.Properties.Resources.C_Other;
            }
            return global::Financial_Journal.Properties.Resources.C_Friends;
        }

        Receipt parent;
        Size Start_Size = new Size();
        string Link_Name = "";

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Contacts(Receipt _parent, Point g = new Point(), Size s = new Size(), string Link_Name_ = "")
        {
            repaint_buttons = true;
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));

            Link_Name = Link_Name_;
        }

        Contact Ref_Contact;
        
        public bool Editing_Contacts = false;

        private void Receipt_Load(object sender, EventArgs e)
        {
            Paint = true;
            // Mousedown anywhere to drag

            if (Link_Name.Length == 0)
            {
                this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            }

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

            bufferedPanel1.AutoScroll = false;
            bufferedPanel1.HorizontalScroll.Enabled = false;
            bufferedPanel1.HorizontalScroll.Visible = false;
            bufferedPanel1.HorizontalScroll.Maximum = 0;
            bufferedPanel1.AutoScroll = true;

            bufferedPanel3.Paint += new PaintEventHandler(panel1_Paint);
        
            association.Items.Add("Family");
            association.Items.Add("Friend");
            association.Items.Add("Work");
            association.Items.Add("Service");
            association.Items.Add("Other");

            association.SelectedIndex = 0;

            bufferedPanel3.Invalidate();


            textBox6.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxEnterKey_KeyPress);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(88, 88, 88));
            Pen Grey_Pen = new Pen(GreyBrush, 2);

            // Draw gray header line
            e.Graphics.DrawLine(Grey_Pen, 10, bufferedPanel1.Top - 10, this.Width - 10, bufferedPanel1.Top - 10);

            // Dispose all objects
            Grey_Pen.Dispose();
            GreyBrush.Dispose();
            base.OnPaint(e);

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
            //minimize_button.ForeColor = randomColor;
            //close_button.ForeColor = randomColor;
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bufferedPanel2.Visible = true;
            label10.Visible = false;
            button1.Visible = false;
            Paint = true;
            Invalidate();
            bufferedPanel3.Invalidate();
            this.Height += bufferedPanel2.Height - 27;
            TFLP.Size = new Size(this.Width - 2, this.Height - 2);
        }


        private void search_desc_button_Click(object sender, EventArgs e)
        {
            bufferedPanel2.Visible = false;
            label10.Visible = true;
            button1.Visible = true;
            Paint = true;
            Invalidate();
            bufferedPanel3.Invalidate();
            this.Height -= bufferedPanel2.Height - 27;
            TFLP.Size = new Size(this.Width - 2, this.Height - 2);
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            if (firstname.Text.Length > 0 && !Editing)
            {
                Random OrderID_Gen = new Random();
                string hashID = OrderID_Gen.Next(100000000, 999999999).ToString();

                // Remove hash collision
                while (parent.Contact_List.Any(x => x.Hash_Value == hashID))
                {
                    hashID = OrderID_Gen.Next(100000000, 999999999).ToString();
                }

                Contact C = new Contact()
                {
                    First_Name = firstname.Text.Trim(),
                    Last_Name = lastname.Text.Trim(),
                    Email = email.Text.Trim(),
                    Email_Second = email2.Text.Trim(),
                    Phone_No_Primary = primary.Text.Trim(),
                    Phone_No_Second = secondary.Text.Trim(),
                    Association = association.Text.Trim(),
                    Hash_Value = hashID
                };

                parent.Contact_List.Add(C);
            }
            else
            {
                Contact Ref_C = parent.Contact_List.FirstOrDefault(x => x == Ref_Contact);
                Ref_C.First_Name = firstname.Text.Trim();
                Ref_C.Last_Name = lastname.Text.Trim();
                Ref_C.Email = email.Text.Trim();
                Ref_C.Email_Second = email2.Text.Trim();
                Ref_C.Phone_No_Primary = primary.Text.Trim();
                Ref_C.Phone_No_Second = secondary.Text.Trim();
                Ref_C.Association = association.Text;
                Ref_C.Hash_Value = Ref_Contact.Hash_Value;
                //Editing
                Editing = false;
                Add_button.Image = global::Financial_Journal.Properties.Resources.checkout;

            }
            search_desc_button.PerformClick();
            firstname.Text = lastname.Text = email.Text = email2.Text = primary.Text = secondary.Text = "";
            Paint = true;
            this.Invalidate();
            bufferedPanel3.Invalidate();
            repaint_buttons = true;

        }

        bool Editing = false;
        string Searching_Name = "";

        // If press enter on length box, activate add (nmemonics)
        private void textboxEnterKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox g = (TextBox)sender;
            if (e.KeyChar == (char)Keys.Enter)
            {
                search_button.PerformClick();
            }
        }

        int scrollIndex = 0;

        private void search_button_Click(object sender, EventArgs e)
        {
            Searching_Name = textBox6.Text;
            
            string alphabet = "abcdefghijklmnopqrstuvwxyz";
            int Contact_Count = 0;

            List<int> scrollHeightList = new List<int>();

            foreach (char c in alphabet)
            {
                List<Contact> Temp = parent.Contact_List.Where(x => x.First_Name.ToLower()[0] == c).ToList();
                Temp = Temp.OrderBy(x => x.First_Name).ToList();

                foreach (Contact C in Temp)
                {
                    if (C.Contains_Str(Searching_Name))
                    {
                        scrollHeightList.Add(Contact_Count);
                    }
                    Contact_Count++;
                }
            }

            // Remove redundant entries
            scrollHeightList = scrollHeightList.Distinct().ToList();

            //if (Searching_Name.Length > 0)
            if (scrollHeightList.Count > 0)
            {
                Control c = Interactive_Button_List.FirstOrDefault(x => x.Name == "i" + scrollHeightList[scrollIndex]);

                bufferedPanel1.AutoScrollPosition = c.Location == null ? new Point() : c.Location;
                Searching_Name = "";

                // Iterate next
                if (scrollHeightList.Count - scrollIndex > 1)
                {
                    scrollIndex++;
                }
            }
        }

        // settings button
        private void button2_Click(object sender, EventArgs e)
        {
            Grey_Out();
            ContactImpExp CIE = new ContactImpExp(parent, this, this.Location, this.Size);
            CIE.ShowDialog();
            Grey_In();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            scrollIndex = 0;
        }

        private void firstname_TextChanged(object sender, EventArgs e)
        {

        }
    }

    public class Contact
    {
        public string Last_Name { get; set; }
        public string First_Name { get; set; }
        public string Email { get; set; }
        public string Email_Second { get; set; }
        public string Phone_No_Primary { get; set; }
        public string Phone_No_Second { get; set; }
        public string Hash_Value { get; set; }
        public string Association { get; set; }

        public Contact()
        {
            Email = Email_Second = Phone_No_Primary = Phone_No_Second = "";
        }

        public override string ToString()
        {
            return First_Name + " " + Last_Name + Environment.NewLine +
                ((Phone_No_Primary.Length > 0) ? (Phone_No_Primary + Environment.NewLine) : "") +
                ((Phone_No_Second.Length > 0) ? (Phone_No_Second + Environment.NewLine) : "") +
                ((Email.Length > 0) ? (Email + Environment.NewLine) : "") + 
                ((Email_Second.Length > 0) ? (Email_Second + Environment.NewLine) : "");
        }

        public string ToStringIndent(int indent_factor = 4, bool scaling = false)
        {
            string indent = new String(' ', indent_factor);
            string Lines = "";
            Lines += (indent + "Contact: " + First_Name + " " + Last_Name + Environment.NewLine);
            Lines += ((scaling ? indent : "") + ((Phone_No_Primary.Length > 0) ? (indent + Phone_No_Primary + Environment.NewLine) : ""));
            Lines += ((scaling ? indent : "") + ((Phone_No_Second.Length > 0) ? (indent + Phone_No_Second + Environment.NewLine) : ""));
            Lines += ((scaling ? indent : "") + ((Email.Length > 0) ? (indent + Email + Environment.NewLine) : ""));
            Lines += ((scaling ? indent : "") + ((Email_Second.Length > 0) ? (indent + Email_Second + Environment.NewLine) : ""));

            return Lines;
        }

        /// <summary>
        /// Indent factor base is 4 by default, scaling is stepped after name
        /// </summary>
        /// <param name="indent_factor"></param>
        /// <param name="scaling"></param>
        /// <returns="KEYVALUEPAIR<int, string> where int = number of lines, string = actual ToString value"></returns>
        public List<string> ToStringList(int indent_factor = 4, bool scaling = false)
        {
            string indent = new String(' ', indent_factor);

            List<string> Lines = new List<string>();
            Lines.Add(indent + "Contact: " + First_Name + " " + Last_Name);
            Lines.Add((scaling ? indent : "") + ((Phone_No_Primary.Length > 0) ? (indent + Phone_No_Primary) : ""));
            Lines.Add((scaling ? indent : "") + ((Phone_No_Second.Length > 0) ? (indent + Phone_No_Second) : ""));
            Lines.Add((scaling ? indent : "") + ((Email.Length > 0) ? (indent + Email) : ""));
            Lines.Add((scaling ? indent : "") + ((Email_Second.Length > 0) ? (indent + Email_Second) : ""));

            // Strip empty lines and return value
            return Lines.Where(x => x.Length > ((scaling ? indent : "") + indent).Length).ToList(); ;
        }

        public bool Contains_Str(string str)
        {
            return (First_Name.ToLower().Contains(str.ToLower()) ||
                Last_Name.ToLower().Contains(str.ToLower()) ||
                Email.ToLower().Contains(str.ToLower()) ||
                Email_Second.ToLower().Contains(str.ToLower()) ||
                Phone_No_Primary.ToLower().Contains(str.ToLower()) ||
                Phone_No_Second.ToLower().Contains(str.ToLower()));
        }

        public void Cycle_Association()
        {
            List<string> Asso = new List<string>() { "Family", "Friend", "Work", "Service", "Other" };
            int Current_Int = Asso.IndexOf(Association);

            if (Current_Int + 1 >= Asso.Count)
            {
                Current_Int = 0;
            }
            else
            {
                Current_Int++;
            }

            Association = Asso[Current_Int];
        }


        public Contact Clone_Contact()
        {
            return System.MemberwiseClone.Copy(this);
        }

    }
}
