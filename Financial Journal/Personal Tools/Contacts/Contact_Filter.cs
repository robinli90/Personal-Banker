using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Financial_Journal
{
    public partial class Contact_Filter : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Transfer contacts
            parent_Contacts.CO_Filtered_List = CO_Include;

            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;
        Size Start_Size = new Size();
        Contacts parent_Contacts;
        bool isExport = false;

        private List<Contact> CO_Include = new List<Contact>();
        private List<Contact> CO_Exclude = new List<Contact>();


        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Contact_Filter(Receipt _parent, Contacts c, Point g = new Point(), Size s = new Size(), bool isExport = false)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            parent_Contacts = c;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));

            parent_Contacts.CO_Filtered_List.ForEach(x => CO_Include.Add(x.Clone_Contact()));

            button2.Visible = !isExport;

            if (!isExport)
            {
                label8.Text = label8.Text.Replace("Export", "Import");
            }

            this.isExport = isExport;
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
                        Populate_Contacts(Ref_Box, CO_Include);
                        break;
                    case 1:
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

            switch (LB_Index)
            {
                case 0:
                    CO_Exclude.Add(CO_Include[Selection_Index]);
                    CO_Include.RemoveAt(Selection_Index);
                    break;
                case 1:
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

        // Hidden 
        private string Completed_Item = "                                                                                         |C|";

        private void Populate_Contacts(ListBox LB, List<Contact> C_List)
        {
            C_List.ForEach(x => LB.Items.Add(x.First_Name + " " + x.Last_Name + (x.Phone_No_Primary.Length > 0 ? (" - " + x.Phone_No_Primary) : "")));
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

            Set_ListBox_Handlers();

            PopulateBoxes();

            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            ToolTip1.SetToolTip(left_3, "Include Item");
            ToolTip1.SetToolTip(right_3, "Exclude Item");
            ToolTip1.SetToolTip(button1, "Shift all items over");
            ToolTip1.SetToolTip(button2, "Transfer duplicate contacts");

            if (!isExport) button2.PerformClick();
        }


        private string Indent = "    ";

        private bool Is_Shopping(string s)
        {
            return s.StartsWith(Indent);
        }

        private void Set_ListBox_Handlers()
        {
            ListBox_List = new List<ListBox>();

            ListBox_List.Add(listBox0);
            ListBox_List.Add(listBox1);
             
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

            int Box_Index = Convert.ToInt32(box.Name.Substring(box.Name.Length - 1));

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

        private void left_3_Click(object sender, EventArgs e)
        {

            ListBox box = ListBox_List[1];

            #region Transfer
            int Box_Index = Convert.ToInt32(box.Name.Substring(box.Name.Length - 1));
            int index = ListBox_List[Box_Index].SelectedIndex;
            if (index >= 0)
            {
                Transfer(box, index);
            }
            #endregion
        }

        private void right_3_Click(object sender, EventArgs e)
        {

            ListBox box = ListBox_List[0];

            #region Transfer
            int Box_Index = Convert.ToInt32(box.Name.Substring(box.Name.Length - 1));
            int index = ListBox_List[Box_Index].SelectedIndex;
            if (index >= 0)
            {
                Transfer(box, index);
            }
            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Grey_Out();
            
            // Force form to redraw
            Application.DoEvents();

            if (secondThreadFormHandle == IntPtr.Zero)
            {
                Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size, "SHIFTING", "CONTACTS")
                {
                };
                form.HandleCreated += SecondFormHandleCreated;
                form.HandleDestroyed += SecondFormHandleDestroyed;
                form.RunInNewThread(false);
            }

            ListBox_List.ForEach(x => x.BeginUpdate());

            ListBox Ref_Box = ListBox_List[0].Items.Count > ListBox_List[1].Items.Count ? ListBox_List[0] : ListBox_List[1];

            if (Ref_Box.Items.Count > 0)
            {
                for (int i = Ref_Box.Items.Count - 1; i > -1; i--)
                {
                    Transfer(Ref_Box, i);
                }
            }

            if (secondThreadFormHandle != IntPtr.Zero)
            {
                PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
            ListBox_List.ForEach(x => x.EndUpdate());


            this.Grey_In();
        }

        #region handler thread

        private IntPtr secondThreadFormHandle;

        void SecondFormHandleCreated(object sender, EventArgs e)
        {
            Control second = sender as Control;
            secondThreadFormHandle = second.Handle;
            second.HandleCreated -= SecondFormHandleCreated;
        }

        void SecondFormHandleDestroyed(object sender, EventArgs e)
        {
            Control second = sender as Control;
            secondThreadFormHandle = IntPtr.Zero;
            second.HandleDestroyed -= SecondFormHandleDestroyed;
        }

        const int WM_CLOSE = 0x0010;
        [DllImport("User32.dll")]
        extern static IntPtr PostMessage(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam);
        #endregion

        private void button2_Click(object sender, EventArgs e)
        {
            this.Activate();
            this.Grey_Out();

            using (var form2 = new Yes_No_Dialog(parent, "Filter duplicate contacts. Continue?", "Warning", "No", "Yes", 0, this.Location, this.Size))
            {
                var result = form2.ShowDialog();
                if (result == DialogResult.OK && form2.ReturnValue1 == "1") // Actually print
                {

                    ListBox_List.ForEach(x => x.BeginUpdate());
                    // Combine full names list used to check for redundant entries
                    List<string> Full_Names = new List<string>();
                    parent.Contact_List.ForEach(x => Full_Names.Add(x.First_Name + " " + x.Last_Name));

                    ListBox Ref_Box = ListBox_List[0];
                    if (Ref_Box.Items.Count > 0)
                    {
                        if (secondThreadFormHandle == IntPtr.Zero)
                        {
                            Loading_Form form = new Loading_Form(parent, new Point(this.Location.X, this.Location.Y), this.Size, "FILTERING", "CONTACTS")
                            {
                            };
                            form.HandleCreated += SecondFormHandleCreated;
                            form.HandleDestroyed += SecondFormHandleDestroyed;
                            form.RunInNewThread(false);
                        }

                        for (int i = Ref_Box.Items.Count - 1; i > -1; i--)
                        {
                            if (Ref_Box.Items.Count <= i) i--;
                            if (Full_Names.Any(x => (Ref_Box.Items[i].ToString().StartsWith(x))))
                            {
                                Transfer(Ref_Box, i);
                            }
                        }
                    }

                    if (secondThreadFormHandle != IntPtr.Zero)
                    {
                        PostMessage(secondThreadFormHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    }
                    ListBox_List.ForEach(x => x.EndUpdate());
                }
            }

            this.Grey_In();
        }
    }
}
