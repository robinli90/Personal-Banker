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
    public partial class Agenda : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            base.OnFormClosing(e);
        }

        bool Paint = false;
        private List<Button> Outer_Delete_Item_Buttons = new List<Button>();
        private List<CheckBox> Outer_Check_Boxes = new List<CheckBox>();
        private List<TextBox> Outer_Text_Box = new List<TextBox>();
        private List<Button> Inner_Delete_Item_Buttons = new List<Button>();
        private List<CheckBox> Inner_Check_Boxes = new List<CheckBox>();
        private List<TextBox> Inner_Text_Box = new List<TextBox>();
        private List<FlowLayoutPanel> FLP_List = new List<FlowLayoutPanel>();

        private int Entries_Per_Page = 2;
        int Pages_Required = 0;
        int Current_Page = 0;

        List<Agenda_Item> Paint_List = new List<Agenda_Item>();

        protected override void OnPaint(PaintEventArgs e)
        {
            int data_height = 29;
            int start_height = 50;
            int start_margin = 55;              // Item
            int height_offset = 9;

            int margin1 = start_margin + 20;   //Price

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

            int item_index = 0;

            if (Paint)
            {
                //Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)parent.Agenda_Item_List.Count() / (decimal)Entries_Per_Page));
                Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)Paint_List.Count() / (decimal)Entries_Per_Page));
                next_page_button.Visible = Pages_Required > 1 && Current_Page < Pages_Required - 1;

                //  Clear existing panels
                Outer_Delete_Item_Buttons.ForEach(button => button.Image.Dispose());
                Outer_Delete_Item_Buttons.ForEach(button => button.Dispose());
                Outer_Delete_Item_Buttons.ForEach(button => this.Controls.Remove(button));
                Outer_Delete_Item_Buttons = new List<Button>();
                Outer_Check_Boxes.ForEach(x => x.Dispose());
                Outer_Check_Boxes.ForEach(x => this.Controls.Remove(x));
                Outer_Check_Boxes = new List<CheckBox>();
                Outer_Text_Box.ForEach(x => x.Dispose());
                Outer_Text_Box.ForEach(x => this.Controls.Remove(x));
                Outer_Text_Box = new List<TextBox>();
                Inner_Delete_Item_Buttons.ForEach(button => button.Image.Dispose());
                Inner_Delete_Item_Buttons.ForEach(button => button.Dispose());
                Inner_Delete_Item_Buttons.ForEach(button => this.Controls.Remove(button));
                Inner_Delete_Item_Buttons = new List<Button>();
                Inner_Check_Boxes.ForEach(x => x.Dispose());
                Inner_Check_Boxes.ForEach(x => this.Controls.Remove(x));
                Inner_Check_Boxes = new List<CheckBox>();
                Inner_Text_Box.ForEach(x => x.Dispose());
                Inner_Text_Box.ForEach(x => this.Controls.Remove(x));
                Inner_Text_Box = new List<TextBox>();
                FLP_List.ForEach(x => x.Dispose());
                FLP_List.ForEach(x => this.Controls.Remove(x));
                FLP_List = new List<FlowLayoutPanel>();


                //foreach (Agenda_Item AI in parent.Agenda_Item_List.GetRange(Current_Page * Entries_Per_Page, (parent.Agenda_Item_List.Count - Entries_Per_Page * Current_Page) >= Entries_Per_Page ? Entries_Per_Page : (parent.Agenda_Item_List.Count % Entries_Per_Page)))
                foreach (Agenda_Item AI in Paint_List.GetRange(Current_Page * Entries_Per_Page, (Paint_List.Count - Entries_Per_Page * Current_Page) >= Entries_Per_Page ? Entries_Per_Page : (Paint_List.Count % Entries_Per_Page)))
                {
                    #region outer agenda item
                    FlowLayoutPanel FLP = new FlowLayoutPanel()
                    {
                        Location = new Point(start_margin, start_height + height_offset + (row_count * data_height)),
                        FlowDirection = FlowDirection.LeftToRight,
                        AutoSize = true,
                        Height = data_height,
                    };

                    CheckBox CB = new CheckBox()
                        {
                            Checked = AI.Check_State,
                            FlatStyle = FlatStyle.Flat,
                            BackColor = Color.FromArgb(65, 65, 65),
                            ForeColor = Color.White,
                            Name = "CB" + AI.ID,
                            AutoSize = true,
                            Anchor = AnchorStyles.None
                        };


                    TextBox TB = new TextBox()
                        {
                            BackColor = Color.FromArgb(65, 65, 65),
                            ForeColor = AI.Check_State ? Color.Gray : Color.White,
                            Height = data_height,
                            BorderStyle = BorderStyle.None,
                            Text = AI.Name,
                            Name = "TB" + AI.ID,
                            Width = 50,
                            Font = new Font("MS Reference Sans Serif", 11, AI.Check_State ? FontStyle.Strikeout : FontStyle.Regular),
                            Anchor = AnchorStyles.None,
                            ReadOnly = !editItems.Checked
                        };

                    // Resize
                    Size size = TextRenderer.MeasureText(TB.Text, TB.Font);
                    TB.Width = size.Width;
                    TB.TextChanged += new EventHandler(textBox_TextChanged);
                    TB.Click += new EventHandler(textBox_Click);
                    CB.CheckedChanged += new EventHandler(this.checkChanged);

                    Button delete_button = new Button()
                    {
                        BackColor = this.BackColor,
                        ForeColor = this.BackColor,
                        FlatStyle = FlatStyle.Flat,
                        Image = global::Financial_Journal.Properties.Resources.delete,
                        Enabled = true,
                        Size = new Size(28, 28),
                        Name = "DB" + AI.ID,
                        Text = "",
                        Anchor = AnchorStyles.None
                    };

                    delete_button.Click += new EventHandler(this.dynamic_button_click);
                    Outer_Delete_Item_Buttons.Add(delete_button);
                    Outer_Check_Boxes.Add(CB);
                    Outer_Text_Box.Add(TB);

                    FLP.Controls.Add(CB);
                    FLP.Controls.Add(TB);
                    if (editItems.Checked) FLP.Controls.Add(delete_button);

                    this.Controls.Add(FLP);
                    FLP_List.Add(FLP);

                    row_count++;

                    int inner_item_index = 0;

                    #endregion

                    #region Adding interior existing shopping items
                    foreach (Shopping_Item SI in AI.Shopping_List)
                    {
                        FLP = new FlowLayoutPanel()
                        {
                            Location = new Point(margin1, start_height + height_offset + (row_count * data_height)),
                            FlowDirection = FlowDirection.LeftToRight,
                            AutoSize = true,
                            Height = data_height,
                        };

                        CB = new CheckBox()
                        {
                            Checked = SI.Check_State,
                            FlatStyle = FlatStyle.Flat,
                            BackColor = Color.FromArgb(65, 65, 65),
                            ForeColor = Color.White,
                            Name = "CB" + "_" + inner_item_index.ToString() + "_" + AI.ID,
                            AutoSize = true,
                            Anchor = AnchorStyles.None
                        };


                        TB = new TextBox()
                        {
                            BackColor = Color.FromArgb(65, 65, 65),
                            ForeColor = SI.Check_State ? Color.Gray : Color.White,
                            Height = data_height,
                            BorderStyle = BorderStyle.None,
                            Text = SI.Name,
                            Name = "TB" + "_" + inner_item_index.ToString() + "_" + AI.ID,
                            Width = 50,
                            Font = new Font("MS Reference Sans Serif", 8, SI.Check_State ? FontStyle.Strikeout : FontStyle.Regular),
                            Anchor = AnchorStyles.None,
                            ReadOnly = !editItems.Checked
                        };

                        TB.TextChanged += new EventHandler(textBox_TextChanged1);
                        // Resize
                        size = TextRenderer.MeasureText(TB.Text, TB.Font);
                        TB.Width = size.Width;
                        TB.Click += new EventHandler(textBox_Click);

                        delete_button = new Button()
                        {
                            BackColor = this.BackColor,
                            ForeColor = this.BackColor,
                            FlatStyle = FlatStyle.Flat,
                            Image = global::Financial_Journal.Properties.Resources.delete,
                            Enabled = true,
                            Size = new Size(28, 28),
                            Name = "DB" + "_" + inner_item_index.ToString() + "_" + AI.ID,
                            Text = "",
                            Anchor = AnchorStyles.None
                        };

                        CB.CheckedChanged += new EventHandler(this.checkChanged1);
                        delete_button.Click += new EventHandler(this.dynamic_button_click1);
                        Inner_Delete_Item_Buttons.Add(delete_button);
                        Inner_Check_Boxes.Add(CB);
                        Inner_Text_Box.Add(TB);

                        FLP.Controls.Add(CB);
                        FLP.Controls.Add(TB);
                        if (editItems.Checked) FLP.Controls.Add(delete_button);

                        this.Controls.Add(FLP);
                        FLP_List.Add(FLP);
                        row_count++;
                        inner_item_index++;
                    }
                    #endregion

                    #region Add Shopping Item Template
                    if (editItems.Checked)
                    {
                        FlowLayoutPanel FLP_Template1 = new FlowLayoutPanel()
                        {
                            Location = new Point(margin1 + 10, start_height + height_offset + (row_count * data_height)),
                            FlowDirection = FlowDirection.LeftToRight,
                            AutoSize = true,
                            Height = data_height,
                        };

                        CheckBox CB_Template1 = new CheckBox()
                        {
                            Text = "",
                            FlatStyle = FlatStyle.Flat,
                            BackColor = Color.FromArgb(65, 65, 65),
                            ForeColor = Color.White,
                            Name = "CB" + "_" + inner_item_index.ToString() + "_" + AI.ID + "_temp",
                            AutoSize = true,
                            Anchor = AnchorStyles.None,
                            Enabled = false
                        };

                        TextBox TB_Template1 = new TextBox()
                        {
                            BackColor = Color.FromArgb(65, 65, 65),
                            ForeColor = Color.FromArgb(95, 95, 95),
                            Height = data_height,
                            BorderStyle = BorderStyle.None,
                            Text = "Entry",
                            Name = "TB" + "_" + inner_item_index.ToString() + "_" + AI.ID + "_temp",
                            Width = 50,
                            Font = new Font("MS Reference Sans Serif", 8, FontStyle.Regular),
                            Anchor = AnchorStyles.None,
                        };
                        TB_Template1.TextChanged += new EventHandler(textBox_TextChanged1);
                        TB_Template1.Click += new EventHandler(textBox_Click1);
                        TB_Template1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxEnterKey_KeyPress1);

                        // Resize
                        size = TextRenderer.MeasureText(TB_Template1.Text, TB_Template1.Font);
                        TB_Template1.Width = size.Width;

                        Inner_Check_Boxes.Add(CB_Template1);
                        Inner_Text_Box.Add(TB_Template1);

                        FLP_Template1.Controls.Add(CB_Template1);
                        FLP_Template1.Controls.Add(TB_Template1);

                        this.Controls.Add(FLP_Template1);
                        FLP_List.Add(FLP_Template1);

                        if (template_ID == AI.ID && added_new_shopping_item) this.ActiveControl = TB_Template1;

                        row_count++;
                    }
                    #endregion

                    item_index++;
                }

                #region Add Agenda Item Template
                
                if (editItems.Checked)
                {
                    FlowLayoutPanel FLP_Template = new FlowLayoutPanel()
                    {
                        Location = new Point(start_margin + 10, start_height + height_offset + (row_count * data_height)),
                        FlowDirection = FlowDirection.LeftToRight,
                        AutoSize = true,
                        Height = data_height,
                    };

                    CheckBox CB_Template = new CheckBox()
                        {
                            Text = "",
                            FlatStyle = FlatStyle.Flat,
                            BackColor = Color.FromArgb(65, 65, 65),
                            ForeColor = Color.White,
                            Name = "CB" + item_index.ToString() + "_temp",
                            AutoSize = true,
                            Anchor = AnchorStyles.None,
                            Enabled = false
                        };

                    TextBox TB_Template = new TextBox()
                        {
                            BackColor = Color.FromArgb(65, 65, 65),
                            ForeColor = Color.FromArgb(95, 95, 95),
                            Height = data_height,
                            BorderStyle = BorderStyle.None,
                            Text = "To-Do",
                            Name = "TB" + item_index.ToString() + "_temp",
                            Width = 50,
                            Font = new Font("MS Reference Sans Serif", 11, FontStyle.Regular),
                            Anchor = AnchorStyles.None,
                        };
                    TB_Template.TextChanged += new EventHandler(textBox_TextChanged);
                    TB_Template.Click += new EventHandler(textBox_Click);
                    TB_Template.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxEnterKey_KeyPress);

                    Outer_Check_Boxes.Add(CB_Template);
                    Outer_Text_Box.Add(TB_Template);

                    FLP_Template.Controls.Add(CB_Template);
                    FLP_Template.Controls.Add(TB_Template);

                    this.Controls.Add(FLP_Template);
                    FLP_List.Add(FLP_Template);

                    if (!added_new_shopping_item) this.ActiveControl = TB_Template;

                    row_count++;
                    item_index++;
                }
                #endregion

                row_count += 2;

                if (row_count == 0)
                {
                    this.Height = Start_Size.Height;
                }
                else
                {
                    this.Height = start_height + height_offset + row_count * data_height;
                }
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
            f_italic.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
            Paint = false;
            added_new_shopping_item = false;
        }

        #region outer template event handlers

        private void dynamic_button_click(object sender, EventArgs e)
        {
            Button TB = (Button)sender;
            Expenses Ref_Expense = new Expenses();

            string Agenda_ID = TB.Name.Substring(2);

            parent.Agenda_Item_List.Remove(parent.Agenda_Item_List.FirstOrDefault(x => x.ID == Convert.ToInt32(Agenda_ID)));

            // Descend all the IDs with ID > currently removed ID

            for (int i = 0; i < parent.Agenda_Item_List.Count; i++)
            {
                if (parent.Agenda_Item_List[i].ID > Convert.ToInt32(Agenda_ID))
                {
                    parent.Agenda_Item_List[i].ID -= 1;
                    parent.Agenda_Item_List[i].Shopping_List.ForEach(x => x.ID -= 1);
                }
            }

            Paint = true;
            Invalidate();
        }

        // Get Random Order ID
        Random OrderID_Gen = new Random();

        // Return available hash value for Agenda Item
        public string Return_Available_Hash_AI()
        {
            string RandomHash = OrderID_Gen.Next(100000000, 999999999).ToString();

            while (parent.Agenda_Item_List.Any(x => x.Hash_Value == RandomHash))
            {
                RandomHash = OrderID_Gen.Next(100000000, 999999999).ToString();
            }

            return RandomHash;
        }

        // Return available hash value for Shopping Item
        public string Return_Available_Hash_SI()
        {
            string RandomHash = OrderID_Gen.Next(100000000, 999999999).ToString();

            List<Shopping_Item> Temp_SI_List = new List<Shopping_Item>();
            parent.Agenda_Item_List.ForEach(x => Temp_SI_List.AddRange(x.Shopping_List));

            while (Temp_SI_List.Any(x => x.Hash_Value == RandomHash))
            {
                RandomHash = OrderID_Gen.Next(100000000, 999999999).ToString();
            }

            return RandomHash;
        }

        // If press enter on length box, activate add (nmemonics)
        private void textboxEnterKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox g = (TextBox)sender;
            if (e.KeyChar == (char)Keys.Enter && g.Text.Length > 0)
            {
                if (g.Text.Length > 0 && g.Text != "To-Do")
                {
                    parent.Agenda_Item_List.Insert(0, new Agenda_Item()
                        {
                            Name = g.Text,
                            Check_State = false,
                            ID = parent.Agenda_Item_List.Count + 1,
                            Date = DateTime.Now,
                            Calendar_Date = new DateTime(1800, 1, 1),
                            Hash_Value = Return_Available_Hash_AI(),
                            Contact_Hash_Value = ""
                        });
                    Paint = true;
                    Invalidate();
                }
            }
        }

        private void checkChanged(object sender, EventArgs e)
        {
            CheckBox CB = (CheckBox)sender;
            string Agenda_ID = CB.Name.Substring(2);
            parent.Agenda_Item_List.FirstOrDefault(x => x.ID == Convert.ToInt32(Agenda_ID)).Check_State = CB.Checked;

            // Check all child shopping items if CHECKED
            Inner_Check_Boxes.ForEach(x => x.CheckedChanged -= checkChanged1);
            if (CB.Checked)
            {
                Inner_Check_Boxes.Where(x => x.Name.Split(new string[] { "_" }, StringSplitOptions.None)[2] == Agenda_ID && !x.Name.Contains("temp")).ToList().ForEach(x => x.Checked = true);
                parent.Agenda_Item_List.FirstOrDefault(x => x.ID == Convert.ToInt32(Agenda_ID)).Shopping_List.ForEach(x => x.Check_State = true);
                Inner_Text_Box.Where(x => x.Name.Split(new string[] { "_" }, StringSplitOptions.None)[2] == Agenda_ID && !x.Name.Contains("temp")).ToList().ForEach(x => x.Font = new Font("MS Reference Sans Serif", 8, CB.Checked ? FontStyle.Strikeout : FontStyle.Regular));
                Inner_Text_Box.Where(x => x.Name.Split(new string[] { "_" }, StringSplitOptions.None)[2] == Agenda_ID && !x.Name.Contains("temp")).ToList().ForEach(x => x.ForeColor = CB.Checked ? Color.Gray : Color.White);
            }
            else
            {
                Inner_Check_Boxes.Where(x => x.Name.Split(new string[] { "_" }, StringSplitOptions.None)[2] == Agenda_ID && !x.Name.Contains("temp")).ToList().ForEach(x => x.Checked = false);
                parent.Agenda_Item_List.FirstOrDefault(x => x.ID == Convert.ToInt32(Agenda_ID)).Shopping_List.ForEach(x => x.Check_State = false);
                Inner_Text_Box.Where(x => x.Name.Split(new string[] { "_" }, StringSplitOptions.None)[2] == Agenda_ID && !x.Name.Contains("temp")).ToList().ForEach(x => x.Font = new Font("MS Reference Sans Serif", 8, CB.Checked ? FontStyle.Strikeout : FontStyle.Regular));
                Inner_Text_Box.Where(x => x.Name.Split(new string[] { "_" }, StringSplitOptions.None)[2] == Agenda_ID && !x.Name.Contains("temp")).ToList().ForEach(x => x.ForeColor = CB.Checked ? Color.Gray : Color.White);          
            }

            Outer_Text_Box.FirstOrDefault(x => x.Name == "TB" + Agenda_ID).Font = new Font("MS Reference Sans Serif", 11, CB.Checked ? FontStyle.Strikeout : FontStyle.Regular);
            Outer_Text_Box.FirstOrDefault(x => x.Name == "TB" + Agenda_ID).ForeColor = CB.Checked ? Color.Gray : Color.White;
            Inner_Check_Boxes.ForEach(x => x.CheckedChanged += checkChanged1);
        }

        #region TextBox UI
        bool Interacted = false;

        private void textBox_Click(object sender, EventArgs e)
        {
            TextBox TB = (TextBox)sender;
            if (TB.Text == "To-Do" || TB.Text == "......")
            {
                TB.Text = "";
                TB.ForeColor = Color.White;
                TB.BackColor = Color.FromArgb(65, 65, 65);
                Interacted = true;
            }
        }


        private void textBox_TextChanged(object sender, EventArgs e)
        {
            TextBox TB = (TextBox)sender;

            TB.ForeColor = Color.White;
            if (TB.Text.Length == 0 && !Interacted)
            {
                TB.Text = "To-Do";
                TB.ForeColor = Color.FromArgb(95, 95, 95);
            }
            if (TB.Text.IndexOf("To-Do") > 0)
            {
                TB.Text = TB.Text.Substring(0, TB.Text.IndexOf("To-Do")) + TB.Text.Substring(TB.Text.IndexOf("To-Do") + 5);
                if (TB.Text.Length > 0)
                {
                    TB.SelectionStart = TB.Text.Length;
                    TB.SelectionLength = 0;
                }
            }
            TB.BackColor = Color.FromArgb(65, 65, 65);
            Size size = TextRenderer.MeasureText(TB.Text, TB.Font);
            TB.Width = size.Width + 10;
            Interacted = false;

            if (editItems.Checked && !TB.Name.Contains("temp"))
            {

                string Agenda_ID = TB.Name.Substring(2);
                parent.Agenda_Item_List.FirstOrDefault(x => x.ID == Convert.ToInt32(Agenda_ID)).Name = TB.Text;

            }
        }
        #endregion
        #endregion

        // Template ID is to automatically target correct template box
        int template_ID = -1;

        bool added_new_shopping_item = false;
        #region inner template event handlers

        private void checkChanged1(object sender, EventArgs e)
        {
            CheckBox CB = (CheckBox)sender;
            string[] Info = CB.Name.Split(new string[] { "_" }, StringSplitOptions.None);
            parent.Agenda_Item_List.FirstOrDefault(x => x.ID == Convert.ToInt32(Info[2])).Shopping_List[Convert.ToInt32(Info[1])].Check_State = CB.Checked;

            // Check if all the boxes in parent shopping list is checked; if so check parent
            bool check_main = parent.Agenda_Item_List.FirstOrDefault(x => x.ID == Convert.ToInt32(Info[2])).Shopping_List.Where(x => x.Check_State == false).Count() == 0;
            parent.Agenda_Item_List.FirstOrDefault(x => x.ID == Convert.ToInt32(Info[2])).Check_State = check_main;

            // Change font and strike out
            Inner_Text_Box.First(x => x.Name == "TB_" + Info[1] + "_" + Info[2]).Font = new Font("MS Reference Sans Serif", 8, CB.Checked ? FontStyle.Strikeout : FontStyle.Regular);
            Inner_Text_Box.First(x => x.Name == "TB_" + Info[1] + "_" + Info[2]).ForeColor = CB.Checked ? Color.Gray : Color.White;

            Outer_Check_Boxes.ForEach(x => x.CheckedChanged -= checkChanged);

            Outer_Check_Boxes.FirstOrDefault(x => x.Name == "CB" + Info[2]).Checked = check_main;

            Outer_Text_Box.First(x => x.Name == "TB" + Info[2]).Font = new Font("MS Reference Sans Serif", 11, CB.Checked && check_main ? FontStyle.Strikeout : FontStyle.Regular);
            Outer_Text_Box.First(x => x.Name == "TB" + Info[2]).ForeColor = CB.Checked && check_main ? Color.Gray : Color.White;
            
            Outer_Check_Boxes.ForEach(x => x.CheckedChanged += checkChanged);
        }

        private void dynamic_button_click1(object sender, EventArgs e)
        {
            Button TB = (Button)sender;
            Expenses Ref_Expense = new Expenses();

            string[] Info = TB.Name.Split(new string[] { "_" }, StringSplitOptions.None);

            parent.Agenda_Item_List.FirstOrDefault(x => x.ID == Convert.ToInt32(Info[2])).Shopping_List.RemoveAt(Convert.ToInt32(Info[1]));

            // Check if all the boxes in parent shopping list is checked; if so check parent
            bool check_main = parent.Agenda_Item_List.FirstOrDefault(x => x.ID == Convert.ToInt32(Info[2])).Shopping_List.Where(x => x.Check_State == false).Count() == 0;
            parent.Agenda_Item_List.FirstOrDefault(x => x.ID == Convert.ToInt32(Info[2])).Check_State = check_main;

            Outer_Check_Boxes.ForEach(x => x.CheckedChanged -= checkChanged);

            Outer_Check_Boxes.FirstOrDefault(x => x.Name == "CB" + Info[2]).Checked = check_main;

            Outer_Text_Box.First(x => x.Name == "TB" + Info[2]).Font = new Font("MS Reference Sans Serif", 11, FontStyle.Regular);
            Outer_Text_Box.First(x => x.Name == "TB" + Info[2]).ForeColor = Color.White;

            Outer_Check_Boxes.ForEach(x => x.CheckedChanged += checkChanged);

            Paint = true;
            Invalidate();
        }

        // If press enter on length box, activate add (nmemonics)
        private void textboxEnterKey_KeyPress1(object sender, KeyPressEventArgs e)
        {
            TextBox g = (TextBox)sender;
            if (e.KeyChar == (char)Keys.Enter && g.Text.Length > 0)
            {
                if (g.Text.Length > 0 && g.Text != "Entry")
                {
                    // Add shopping item to internal list
                    string[] Info = g.Name.Split(new string[] { "_" }, StringSplitOptions.None);
                    parent.Agenda_Item_List.FirstOrDefault(x => x.ID == Convert.ToInt32(Info[2])).Shopping_List.Add(new Shopping_Item
                    {
                        Name = g.Text,
                        Check_State = false,
                        ID = Convert.ToInt32(Info[2]),
                        Calendar_Date = new DateTime(1800, 1, 1),
                        Hash_Value = Return_Available_Hash_SI(),
                        Contact_Hash_Value = ""
                    });

                    Outer_Check_Boxes.ForEach(x => x.CheckedChanged -= checkChanged);

                    parent.Agenda_Item_List.First(x => x.ID == Convert.ToInt32(Info[2])).Check_State = false;
                    Outer_Check_Boxes.First(x => x.Name == "CB" + Info[2]).Checked = false;
                    Outer_Text_Box.First(x => x.Name == "TB" + Info[2]).Font = new Font("MS Reference Sans Serif", 11, FontStyle.Regular);
                    Outer_Text_Box.First(x => x.Name == "TB" + Info[2]).ForeColor = Color.White;

                    Outer_Check_Boxes.ForEach(x => x.CheckedChanged += checkChanged);

                    Paint = true;
                    added_new_shopping_item = true;
                    template_ID = Convert.ToInt32(Info[2]);
                    Invalidate();
                }
            }
        }

        bool Interacted1 = false;

        private void textBox_Click1(object sender, EventArgs e)
        {
            TextBox TB = (TextBox)sender;
            if (TB.Text == "Entry" || TB.Text == "......")
            {
                TB.Text = "";
                TB.ForeColor = Color.White;
                TB.BackColor = Color.FromArgb(65, 65, 65);
                Interacted1 = true;
            }
        }

        private void textBox_TextChanged1(object sender, EventArgs e)
        {
            TextBox TB = (TextBox)sender;

            TB.ForeColor = Color.White;
            if (TB.Text.Length == 0 && !Interacted1)
            {
                TB.Text = "Entry";
                TB.ForeColor = Color.FromArgb(95, 95, 95);
            }
            if (TB.Text.IndexOf("Entry") > 0)
            {
                TB.Text = TB.Text.Substring(0, TB.Text.IndexOf("Entry")) + TB.Text.Substring(TB.Text.IndexOf("Entry") + 5);
                if (TB.Text.Length > 0)
                {
                    TB.SelectionStart = TB.Text.Length;
                    TB.SelectionLength = 0;
                }
            }
            TB.BackColor = Color.FromArgb(65, 65, 65);
            Size size = TextRenderer.MeasureText(TB.Text, TB.Font);
            TB.Width = size.Width + 10;
            Interacted1 = false;

            if (editItems.Checked && !TB.Name.Contains("temp"))
            {
                string[] Info = TB.Name.Split(new string[] { "_" }, StringSplitOptions.None);
                parent.Agenda_Item_List.FirstOrDefault(x => x.ID == Convert.ToInt32(Info[2])).Shopping_List[Convert.ToInt32(Info[1])].Name = TB.Text;
            }
        }
        #endregion

        Receipt parent;
        Size Start_Size = new Size();

        public Agenda(Receipt _parent, Agenda_Item Ref_Agenda = null, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);

            if (Ref_Agenda != null)
            {
                Paint_List.Add(Ref_Agenda);
                this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
            }
            else
            {
                Paint_List = parent.Agenda_Item_List;
                //this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
                this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 3) - (this.Height / 2));
            }
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            Paint = true;

            editItems.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            editItems.Size = new Size(68, 25);
            editItems.OnText = "On";
            editItems.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            editItems.OnForeColor = Color.White;
            editItems.OffText = "Off";
            editItems.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            editItems.OffForeColor = Color.White;

            Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)Paint_List.Count() / (decimal)Entries_Per_Page));
            next_page_button.Visible = Pages_Required > 1;
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

        private void editItems_CheckedChanged(object sender, EventArgs e)
        {
            Paint = true;
            Invalidate();
        }

        private void next_page_button_Click(object sender, EventArgs e)
        {
            if (Current_Page + 1 < Pages_Required)
            {
                Current_Page++;
                back_page_button.Visible = true;
                Paint = true;
                Invalidate();
                if (Pages_Required == Current_Page + 1) next_page_button.Visible = false;
            }
        }

        private void back_page_button_Click(object sender, EventArgs e)
        {
            if (Current_Page >= 1)
            {
                Current_Page--;
                next_page_button.Visible = true;
                Paint = true;
                Invalidate();
                if (0 == Current_Page) back_page_button.Visible = false;
            }
        }
    }

    public class Shopping_Item
    {
        public string Name { get; set; }
        public bool Check_State { get; set; }
        public int ID { get; set; }
        public string Hash_Value { get; set; }
        public string Contact_Hash_Value { get; set; }
        public DateTime Calendar_Date { get; set; }
        public bool Time_Set { get; set; }
        public bool Toggle_IUO { get; set; } // internal use only
    }

    public class Agenda_Item
    {
        public List<Shopping_Item> Shopping_List { get; set; }

        public bool Check_State { get; set; }
        public bool Overwrite { get; set; }
        public int ID { get; set; }
        public string Hash_Value { get; set; }
        public string Contact_Hash_Value { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public DateTime Calendar_Date { get; set; }
        public bool Time_Set { get; set; }

        public Agenda_Item Clone_Agenda()
        {
            return System.MemberwiseClone.Copy(this);
        }

        /// <summary>
        /// Derive the list of checkbox and the item names for recall later
        /// </summary>
        public Agenda_Item()
        {
            this.Shopping_List = new List<Shopping_Item>();
            this.Overwrite = false;
        }
    }
}
