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
    public partial class Export_Filter : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            #region Exclude the contacts that are removed and remove from CE/AI/SI
            List<string> Excluded_Contact_Hash = new List<string>();
            CO_Exclude.ForEach(x => Excluded_Contact_Hash.Add(x.Hash_Value));

            // Remove from CE
            foreach (Calendar_Events CE in CE_Include)
                if (Excluded_Contact_Hash.Contains(CE.Contact_Hash_Value))
                    CE.Contact_Hash_Value = "";

            // Remove from AI/SI
            foreach (Agenda_Item AI in AI_Include)
            {
                if (Excluded_Contact_Hash.Contains(AI.Contact_Hash_Value))
                {
                    AI.Contact_Hash_Value = "";
                }
                foreach (Shopping_Item SI in AI.Shopping_List)
                {
                    if (Excluded_Contact_Hash.Contains(SI.Contact_Hash_Value))
                    {
                        SI.Contact_Hash_Value = "";
                    }
                }
            }
            #endregion

            // Transfer new lists over
            parent_Calendar.CE_Filtered_List = CE_Include.OrderByDescending(x => x.Date).ToList();
            AI_Include.ForEach(x => x.Shopping_List.OrderByDescending(y => y.Calendar_Date).ToList());
            parent_Calendar.AI_Filtered_List = AI_Include.OrderByDescending(x => x.Calendar_Date).ToList();

            // Transfer contacts
            parent_Calendar.CO_Filtered_List = CO_Include;

            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;
        Size Start_Size = new Size();
        Calendar parent_Calendar;

        private List<Calendar_Events> CE_Include = new List<Calendar_Events>();
        private List<Calendar_Events> CE_Exclude = new List<Calendar_Events>();
        private List<Agenda_Item> AI_Include = new List<Agenda_Item>();
        private List<Agenda_Item> AI_Exclude = new List<Agenda_Item>();
        private List<Contact> CO_Include = new List<Contact>();
        private List<Contact> CO_Exclude = new List<Contact>();


        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Export_Filter(Receipt _parent, Calendar parent_Calendar2, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            parent_Calendar = parent_Calendar2;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));

            parent_Calendar.CE_Filtered_List.ForEach(x => CE_Include.Add(x.Clone_CE()));
            parent_Calendar.AI_Filtered_List.ForEach(x => AI_Include.Add(x.Clone_Agenda()));
            parent_Calendar.CO_Filtered_List.ForEach(x => CO_Include.Add(x.Clone_Contact()));

        }

        private void PopulateBoxes()
        {
            SuspendLayout();
            for (int i = 0; i < ListBox_List.Count; i++)
            {
                ListBox Ref_Box = ListBox_List[i];

                // Remove all items from associated box
                Ref_Box.Items.Clear();

                switch (i)
                {
                    case 0:
                        Populate_Calendar(Ref_Box, CE_Include);
                        break;
                    case 1:
                        Populate_Agenda(Ref_Box, AI_Include);
                        break;
                    case 2:
                        Populate_Calendar(Ref_Box, CE_Exclude);
                        break;
                    case 3:
                        Populate_Agenda(Ref_Box, AI_Exclude);
                        break;
                    case 4:
                        Populate_Contacts(Ref_Box, CO_Include);
                        break;
                    case 5:
                        Populate_Contacts(Ref_Box, CO_Exclude);
                        break;
                }
            }
            ResumeLayout();
        }

        /// <summary>
        /// Get the actual index from the List<Agenda_Item> 
        /// </summary>
        /// <param name="LB"></param>
        /// <param name="ListBox_Index"></param>
        /// <returns></returns>
        private int Get_Actual_Agenda_Index(ListBox LB, int ListBox_Index)
        {
            int index = 0;
            for (int i = ListBox_Index; i >= 0; i--)
            {
                if (!Is_Shopping(LB.Items[i].ToString()))
                    index++;
            }
            return index - 1;
        }
        
        /// <summary>
        /// Main transfer switch
        /// </summary>
        /// <param name="LB"></param>
        /// <param name="Selection_Index"></param>
        private void Transfer(ListBox LB, int Selection_Index)
        {
            int LB_Index = ListBox_List.IndexOf(LB);
            int Secondary_Index = Get_Agenda_Index(LB, Selection_Index);

            switch (LB_Index)
            {
                case 0:
                    CE_Exclude.Add(CE_Include[Selection_Index]);
                    CE_Include.RemoveAt(Selection_Index);
                    break;
                case 1:
                    Transfer_Agenda(LB, Selection_Index);
                    break;
                case 2:
                    CE_Include.Add(CE_Exclude[Selection_Index]);
                    CE_Exclude.RemoveAt(Selection_Index);
                    break;
                case 3:
                    Transfer_Agenda(LB, Selection_Index);
                    break;
                case 4:
                    CO_Exclude.Add(CO_Include[Selection_Index]);
                    CO_Include.RemoveAt(Selection_Index);
                    break;
                case 5:
                    CO_Include.Add(CO_Exclude[Selection_Index]);
                    CO_Exclude.RemoveAt(Selection_Index);
                    break;
            }

            PopulateBoxes();

            #region Select box
            try
            {
                LB.SelectedIndex = Selection_Index;
            }
            catch
            {
                try
                {
                    LB.SelectedIndex = Selection_Index - 1;
                }
                catch
                {
                    LB.SelectedIndex = Selection_Index - 2;
                }
            }
            #endregion
        }

        

        /// <summary>
        /// Algorithm to transfer either entire agenda item or just one shopping item over
        /// </summary>
        /// <param name="LB"></param>
        /// <param name="Selection_Index"></param>
        private void Transfer_Agenda(ListBox LB, int Selection_Index)
        {
            int Secondary_Index = Get_Agenda_Index(LB, Selection_Index);
            // include to exclude
            if (LB.Name.Contains("2"))
            {
                // case transfer entire agenda over
                if (Selection_Index == Secondary_Index)
                {
                    Agenda_Item Ref_AI = AI_Include[Get_Actual_Agenda_Index(LB, Secondary_Index)];
                    if (AI_Exclude.Any(x => x.Hash_Value == Ref_AI.Hash_Value)) // If already have same agenda, append leftover list to existing list
                    {
                        AI_Exclude.First(x => x.Hash_Value == Ref_AI.Hash_Value).Shopping_List.AddRange(Ref_AI.Shopping_List);
                    }
                    else // else, just add entire list to exclusion list
                    {
                        AI_Exclude.Add(Ref_AI);
                    }
                    AI_Include.Remove(Ref_AI);
                }
                else // transfer one shopping item
                {
                    Agenda_Item Ref_AI = AI_Include[Get_Actual_Agenda_Index(LB, Secondary_Index)];
                    Shopping_Item Ref_SI = Ref_AI.Shopping_List.FirstOrDefault(x => LB.Items[Selection_Index].ToString().Contains(x.Name));
                    // Check if already existing item with same hash value (same hash = same agenda with new different SI list)
                    if (!AI_Exclude.Any(x => x.Hash_Value == Ref_AI.Hash_Value))
                    {
                        // Get ref shopping item with same name;
                        AI_Exclude.Add(new Agenda_Item()
                        {
                            Check_State = Ref_AI.Check_State,
                            Overwrite = Ref_AI.Overwrite,
                            ID = Ref_AI.ID,
                            Hash_Value = Ref_AI.Hash_Value,
                            Name = Ref_AI.Name,
                            Date = Ref_AI.Date,
                            Calendar_Date = Ref_AI.Calendar_Date,
                            Time_Set = Ref_AI.Time_Set,
                            Shopping_List = new List<Shopping_Item>() { Ref_SI }
                        });
                    }
                    else
                    {
                        AI_Exclude.FirstOrDefault(x => x.Hash_Value == Ref_AI.Hash_Value).Shopping_List.Add(Ref_SI);
                    }
                    // Remove reference
                    Ref_AI.Shopping_List.Remove(Ref_SI);

                    // If empty shopping list, remove entire list
                    if (!Ref_AI.Shopping_List.Any(x => x.Calendar_Date.Year > 1800))
                    {
                        AI_Include.Remove(Ref_AI);
                    }
                }
            }
            // Exclude to include
            else
            {// case transfer entire agenda over
                // case transfer entire agenda over
                if (Selection_Index == Secondary_Index)
                {
                    Agenda_Item Ref_AI = AI_Exclude[Get_Actual_Agenda_Index(LB, Secondary_Index)];
                    if (AI_Include.Any(x => x.Hash_Value == Ref_AI.Hash_Value)) // If already have same agenda, append leftover list to existing list
                    {
                        AI_Include.First(x => x.Hash_Value == Ref_AI.Hash_Value).Shopping_List.AddRange(Ref_AI.Shopping_List);
                    }
                    else // else, just add entire list to exclusion list
                    {
                        AI_Include.Add(Ref_AI);
                    }
                    AI_Exclude.Remove(Ref_AI);
                }
                else // transfer one shopping item
                {
                    Agenda_Item Ref_AI = AI_Exclude[Get_Actual_Agenda_Index(LB, Secondary_Index)];
                    Shopping_Item Ref_SI = Ref_AI.Shopping_List.FirstOrDefault(x => LB.Items[Selection_Index].ToString().Contains(x.Name));
                    // Check if already existing item with same hash value (same hash = same agenda with new differen SI list)
                    if (!AI_Include.Any(x => x.Hash_Value == Ref_AI.Hash_Value))
                    {
                        // Get ref shopping item with same name;
                        AI_Include.Add(new Agenda_Item()
                        {
                            Check_State = Ref_AI.Check_State,
                            Overwrite = Ref_AI.Overwrite,
                            ID = Ref_AI.ID,
                            Hash_Value = Ref_AI.Hash_Value,
                            Name = Ref_AI.Name,
                            Date = Ref_AI.Date,
                            Calendar_Date = Ref_AI.Calendar_Date,
                            Time_Set = Ref_AI.Time_Set,
                            Shopping_List = new List<Shopping_Item>() { Ref_SI }
                        });
                    }
                    else
                    {
                        AI_Include.FirstOrDefault(x => x.Hash_Value == Ref_AI.Hash_Value).Shopping_List.Add(Ref_SI);
                    }
                    // Remove reference
                    Ref_AI.Shopping_List.Remove(Ref_SI);

                    // If empty shopping list, remove entire list
                    if (!Ref_AI.Shopping_List.Any(x => x.Calendar_Date.Year > 1800))
                    {
                        AI_Exclude.Remove(Ref_AI);
                    }
                }
            }
        }

        /// <summary>
        /// Return the agenda index in the Listbox items enumerable
        /// </summary>
        /// <param name="LB"></param>
        /// <param name="Selection_Index"></param>
        /// <returns></returns>
        private int Get_Agenda_Index(ListBox LB, int Selection_Index)
        {
            if (Is_Shopping(LB.Items[Selection_Index].ToString()))
            {
                while (Is_Shopping(LB.Items[Selection_Index].ToString()))
                {
                    Selection_Index--;
                }
            }
            return Selection_Index;
        }

        private string Completed_Item = "                                                                                         |C|";

        private void Populate_Calendar(ListBox LB, List<Calendar_Events> CE_List)
        {
            CE_List.ForEach(x => LB.Items.Add(x.Title + " (" + x.Date.ToShortDateString() + ")"));
        }

        private void Populate_Contacts(ListBox LB, List<Contact> C_List)
        {
            C_List.ForEach(x => LB.Items.Add(x.ToString()));
        }

        private void Populate_Agenda(ListBox LB, List<Agenda_Item> AI_List)
        {
            AI_List.ForEach(x =>
                {
                    LB.Items.Add(x.Name + (x.Calendar_Date.Year > 1800 ? " (" + x.Calendar_Date.ToShortDateString() + ")" : "") + (x.Check_State ? Completed_Item : ""));
                    x.Shopping_List.Where(w => w.Calendar_Date.Year > 1800).ToList().ForEach(y => LB.Items.Add(Indent + y.Name + " (" + y.Calendar_Date.ToShortDateString() + ")" + (y.Check_State ? Completed_Item : "")));
                }
            );
        }

        List<ListBox> ListBox_List = new List<ListBox>();

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

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

            for (int i = 0; i < 10; i++)
            {
            }


            Set_ListBox_Handlers();

            PopulateBoxes();

            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            ToolTip1.SetToolTip(left_1, "Include Item");
            ToolTip1.SetToolTip(right_1, "Exclude Item");
            ToolTip1.SetToolTip(left_2, "Include Item");
            ToolTip1.SetToolTip(right_2, "Exclude Item");
            ToolTip1.SetToolTip(button1, "Exclude Completed Items");
            ToolTip1.SetToolTip(left_3, "Include Item");
            ToolTip1.SetToolTip(right_3, "Exclude Item");
        }


        private string Indent = "    ";

        private bool Is_Shopping(string s)
        {
            return s.StartsWith(Indent);
        }

        private void Set_ListBox_Handlers()
        {
            ListBox_List = new List<ListBox>();

            ListBox_List.Add(listBox1);
            ListBox_List.Add(listBox2);
            ListBox_List.Add(listBox3);
            ListBox_List.Add(listBox4);
            ListBox_List.Add(listBox5);
            ListBox_List.Add(listBox6);
             
            ListBox_List.ForEach(x =>
                {
                    x.DrawMode = DrawMode.OwnerDrawFixed;
                    x.DrawItem += listBox1_DrawItem;
                    x.DoubleClick += listBox1_DoubleClick;
                    x.SelectedIndexChanged += new System.EventHandler(listBox1_SelectedIndexChanged);
                }
            );
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            ListBox box = (ListBox)sender;

            int Box_Index = Convert.ToInt32(box.Name.Substring(box.Name.Length - 1)) - 1;

            int index = ListBox_List[Box_Index].SelectedIndex;

            if (index >= 0)
            {
                Transfer(box, index);
            }
        }

        #region ListBox Format Area

        private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ListBox RefBox = (ListBox)sender;
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox RefBox = (ListBox)sender;

            Font f_strike = new Font(e.Font, FontStyle.Strikeout);

            if (e.Index < 0) return;
            //if the item state is selected them change the back color 
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                e = new DrawItemEventArgs(e.Graphics,
                                          e.Font,
                                          e.Bounds,
                                          e.Index,
                                          e.State ^ DrawItemState.Selected,
                                          e.ForeColor,
                                          Color.FromArgb(84, 84, 84));//Choose the color

            // Draw the background of the ListBox control for each item.
            e.DrawBackground();
            // Draw the current item text
            e.Graphics.DrawString(RefBox.Items[e.Index].ToString(), RefBox.Items[e.Index].ToString().Contains(Completed_Item) ? f_strike : e.Font, Brushes.White, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();

            f_strike.Dispose();
        }
        #endregion

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

        private void right_1_Click(object sender, EventArgs e)
        {
            ListBox box = ListBox_List[0];

            #region Transfer
            int Box_Index = Convert.ToInt32(box.Name.Substring(box.Name.Length - 1)) - 1;
            int index = ListBox_List[Box_Index].SelectedIndex;
            if (index >= 0)
            {
                Transfer(box, index);
            }
            #endregion
        }

        private void left_1_Click(object sender, EventArgs e)
        {
            ListBox box = ListBox_List[2];

            #region Transfer
            int Box_Index = Convert.ToInt32(box.Name.Substring(box.Name.Length - 1)) - 1;
            int index = ListBox_List[Box_Index].SelectedIndex;
            if (index >= 0)
            {
                Transfer(box, index);
            }
            #endregion
        }

        private void right_2_Click(object sender, EventArgs e)
        {

            ListBox box = ListBox_List[1];

            #region Transfer
            int Box_Index = Convert.ToInt32(box.Name.Substring(box.Name.Length - 1)) - 1;
            int index = ListBox_List[Box_Index].SelectedIndex;
            if (index >= 0)
            {
                Transfer(box, index);
            }
            #endregion
        }

        private void left_2_Click(object sender, EventArgs e)
        {
            ListBox box = ListBox_List[3];

            #region Transfer
            int Box_Index = Convert.ToInt32(box.Name.Substring(box.Name.Length - 1)) - 1;
            int index = ListBox_List[Box_Index].SelectedIndex;
            if (index >= 0)
            {
                Transfer(box, index);
            }
            #endregion
        }

        // Automatic transfer
        private void button1_Click(object sender, EventArgs e)
        {
            ListBox Ref_Box = ListBox_List[1];
            for (int i = Ref_Box.Items.Count - 1; i > -1; i--)
            {
                if (Ref_Box.Items.Count <= i) i--;
                if (Ref_Box.Items[i].ToString().Contains(Completed_Item))
                {
                    Transfer(Ref_Box, i);
                }
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void left_3_Click(object sender, EventArgs e)
        {

            ListBox box = ListBox_List[5];

            #region Transfer
            int Box_Index = Convert.ToInt32(box.Name.Substring(box.Name.Length - 1)) - 1;
            int index = ListBox_List[Box_Index].SelectedIndex;
            if (index >= 0)
            {
                Transfer(box, index);
            }
            #endregion
        }

        private void right_3_Click(object sender, EventArgs e)
        {

            ListBox box = ListBox_List[4];

            #region Transfer
            int Box_Index = Convert.ToInt32(box.Name.Substring(box.Name.Length - 1)) - 1;
            int index = ListBox_List[Box_Index].SelectedIndex;
            if (index >= 0)
            {
                Transfer(box, index);
            }
            #endregion
        }

    }
}
