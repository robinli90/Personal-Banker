using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Financial_Journal
{
    public partial class Hobby_Management : Form
    {

        #region Forcing Windows to suspend and resume drawing the flow layout panels
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);

        private const int WM_SETREDRAW = 11;

        public static void SuspendDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(Control parent)
        {
            SendMessage(parent.Handle, WM_SETREDRAW, true, 0);
            parent.Refresh();
        }

        /// <summary>
        /// Suspends painting for the target control. Do NOT forget to call EndControlUpdate!!!
        /// </summary>
        /// <param name="control">visual control</param>
        public static void BeginControlUpdate(Control control)
        {
            Message msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                  IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        /// <summary>
        /// Resumes painting for the target control. Intended to be called following a call to BeginControlUpdate()
        /// </summary>
        /// <param name="control">visual control</param>
        public static void EndControlUpdate(Control control)
        {
            // Create a C "true" boolean as an IntPtr
            IntPtr wparam = new IntPtr(1);
            Message msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
                  IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);
            control.Invalidate();
            control.Refresh();
        }
        #endregion

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;

        protected override void OnPaint(PaintEventArgs e)
        {
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
            rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
            //e.Graphics.FillRectangle(Brushes.DarkBlue, rc);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MAXIMIZE = 0xF030;

            switch (m.Msg)
            {
                case WM_SYSCOMMAND:
                    int command = m.WParam.ToInt32() & 0xfff0;
                    if (command == SC_MAXIMIZE)
                    {
                        this.isMaximized = true;
                        parent.Hobby_Window_Maximized = true;
                    }
                    else
                    {
                        parent.Hobby_Window_Maximized = false;
                    }
                    break;
            }

            if (m.Msg == 0x84)
            {  // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption)
                {
                    m.Result = (IntPtr)2;  // HTCAPTION
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }
            }

            base.WndProc(ref m);
        }
        
        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;
        bool isMaximized = false;

        public Hobby_Management(Receipt _parent, bool Maximized = false)
        {
            parent = _parent;
            isMaximized = Maximized;
            Max_Container_Width = Convert.ToInt32(parent.Settings_Dictionary["HOBBY_MGMT_X"]);
            Max_Container_Height = Convert.ToInt32(parent.Settings_Dictionary["HOBBY_MGMT_Y"]);

            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);

            freeAgentFlowPanel.HorizontalScroll.Maximum = 0;
            freeAgentFlowPanel.AutoScroll = false;
            freeAgentFlowPanel.VerticalScroll.Visible = false;
            freeAgentFlowPanel.AutoScroll = true;
            
             GlobalMouseHandler gmh = new GlobalMouseHandler();
             gmh.MouseUpForm += new MouseUpOnForm(gmh_TheMouseMoved);
             Application.AddMessageFilter(gmh);

        }

        FadeControl TFLP;
        FadeControl TFLP2;

        public void Populate_Profiles()
        {
            if (parent.Hobby_Profile_List.Count > 0)
            {
                // Reset and remove listener
                profile_list.SelectedIndexChanged -= profile_list_SelectedIndexChanged;
                profile_list.Items.Clear();
                // Add existing hobby profiles
                parent.Hobby_Profile_List.Distinct().ToList().ForEach(x => profile_list.Items.Add(x));
                profile_list.Text = parent.Hobby_Profile_List[0];
                profile_list.SelectedIndexChanged += profile_list_SelectedIndexChanged;
            }
            else
            {
                profile_list.Items.Clear();
                profile_list.Text = "";
            }
        }

        private void textClick(object sender, EventArgs e)
        {
            PhantomText1.SelectAll();
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            if (isMaximized) this.WindowState = FormWindowState.Maximized;

            ToolTip1.SetToolTip(Add_button, "Add a new group");
            ToolTip1.SetToolTip(clear_all_button, "Remove all items from groups");
            ToolTip1.SetToolTip(settings_button, "Manage hobby settings");
            ToolTip1.SetToolTip(sort_button, "Sort all items in groups by price (descending)");
            ToolTip1.SetToolTip(button1, "Add a phantom item");
            ToolTip1.SetToolTip(import_phantom_profile, "Import Phantom Items from another profile");
            ToolTip1.SetToolTip(button3, "Help Me!");
            ToolTip1.SetToolTip(highlight, "Highlight All Phantom Items");

            Previous_Profile_Index = "1";

            bank_box.MouseClick += new MouseEventHandler(mouseClick);

            // Populate profiles
            Populate_Profiles();

            // Double buffering layout panels
            SetDoubleBuffered(freeAgentFlowPanel);

            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            PhantomText1.Click += new EventHandler(textClick);

            // Add the main panels
            this.DoubleBuffered = true;
            garbagePanel.SendToBack();
            garbagePanel.BackColor = Color.FromArgb(68, 68, 68);
            TFLP = new FadeControl();
            TFLP.Size = garbagePanel.Size;
            TFLP.Location = garbagePanel.Location;
            TFLP.DragEnter += new DragEventHandler(flowLayoutPanel_DragEnter);
            TFLP.DragDrop += new DragEventHandler(flowLayoutPanel_DragDrop);
            TFLP.Visible = true;
            TFLP.BackColor = this.BackColor;
            TFLP.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            TFLP.AllowDrop = true;
            TFLP.BringToFront();
            this.Controls.Add(TFLP);
            TFLP.BringToFront();

            Reset_Panels();

            if (Hobby_Item_List.Count > 0)
            {
                Dictionary<string, int> Hobby_Count = new Dictionary<string, int>();
                // get max hobby by finding unique values
                List<string> Unique_Hobbies = Hobby_Item_List.Select(x => x.Category).Distinct().ToList();

                foreach (string g in Unique_Hobbies)
                {
                    Hobby_Count.Add(g, Hobby_Item_List.Count(x => x.Category == g));
                }

                category_box.Text = Hobby_Count.FirstOrDefault(x => x.Value == Hobby_Count.Values.Max()).Key;
            }


            // Fade Box
            TFLP2 = new FadeControl();
            TFLP2.Size = new Size(this.Width - 2, this.Height - 2);
            TFLP2.Location = new Point(999, 999);
            TFLP2.Visible = true;
            TFLP2.BackColor = this.BackColor;
            TFLP2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            TFLP2.AllowDrop = true;
            TFLP2.BringToFront();
            this.Controls.Add(TFLP2);
            TFLP2.BringToFront();

            TFLP2.Opacity = 90;

            this.ResizeEnd += Hobby_Management_Resize;
            this.ResizeBegin += Hobby_Management_Resize_Begin; 
            this.ResizeEnd += Hobby_Management_Resize_End; 
        }

        private void Hobby_Management_Resize_End(object sender, EventArgs e)
        {
            textBox1.Visible = textBox4.Visible = true;
            Grey_In();
            this.ResumeLayout();
        }

        private void Hobby_Management_Resize_Begin(object sender, EventArgs e)
        {
            textBox1.Visible = textBox4.Visible = false;
            this.Grey_Out();
            this.SuspendLayout();
        }

        private void Hobby_Management_Resize(object sender, EventArgs e)
        {
        }

        public bool Load_Complete = false;

        public string Previous_Profile_Index = "";

        public void Transfer_To_Parent()
        {
            // Perform a quick save
            foreach (Container c in Container_List)
            {
                c.Name = FLP_Title[Convert.ToInt32(c.ID) - 1].Text == "" ? "No_Name" : FLP_Title[Convert.ToInt32(c.ID) - 1].Text;
            }

            // Automatically descend container numbers that don't exist (newly deleted ones)
            for (int i = Container_List.Count - 1; i >= 0; i--)
            {
                // If empty list, descend all container IDs above
                if (Container_List[i].Hobby_Item_List.Count == 0)
                {
                    Container_List.RemoveAt(i);
                    for (int j = i; j <= Container_List.Count - 1; j++)
                    {
                        Container_List[j].Hobby_Item_List.ForEach(x => x.Container_ID -= 1);
                        Container_List[j].ID = (Convert.ToInt32(Container_List[j].ID) - 1).ToString();
                    }
                }
            }

            // Sort by name first
            Hobby_Item_List = Hobby_Item_List.OrderBy(x => x.Name).ToList();

            // Sort by highest cost
            Hobby_Item_List = Hobby_Item_List.OrderByDescending(x => x.Price).ToList();

            // Add rest of hobby list back to master list
            List<Hobby_Item> Temp_List = parent.Master_Hobby_Item_List.Where(x => x.Profile_Number != Previous_Profile_Index).ToList();
            Temp_List.AddRange(Hobby_Item_List);

            // Sync back to parent hobby list
            parent.Master_Hobby_Item_List = Temp_List; // sync existing ones

            if (parent.Master_Container_Dict.Count > 0)
            {
                parent.Master_Container_Dict[Previous_Profile_Index] = Container_List;
            }
        }

        public void Reset_Panels()
        {

            SuspendDrawing(mainLayoutPanel);
            // Download new sizing parameters
            Max_Container_Width = Convert.ToInt32(parent.Settings_Dictionary["HOBBY_MGMT_X"]);
            Max_Container_Height = Convert.ToInt32(parent.Settings_Dictionary["HOBBY_MGMT_Y"]);

            // Perform only after load is complete as to not remove parent list
            if (Load_Complete)
            {
                Transfer_To_Parent();
            }

            // Dispose of images & other objects
            Swap_Button_List.ForEach(button => button.Image.Dispose());
            Swap_Button_List.ForEach(button => button.Dispose());
            Delete_Button_List.ForEach(button => button.Image.Dispose());
            Delete_Button_List.ForEach(button => button.Dispose());

            // Remove existing controls
            mainLayoutPanel.Controls.Clear();

            // Dispose all cleared objects
            Grouped_Button_List.ForEach(x => x.Dispose());
            FLP_List.ForEach(x => x.Dispose());
            Interior_FLP_List.ForEach(x => x.Dispose());
            FLP_Text_Total.ForEach(x => x.Dispose());
            FLP_Title.ForEach(x => x.Dispose());

            // Reset variables
            Button_List = new List<DragButton>();
            Grouped_Button_List = new List<DragButton>();
            Container_Count = 1;
            Swap_Button_List = new List<Button>();
            Delete_Button_List = new List<Button>();
            FLP_List = new List<FlowLayoutPanel>();
            Interior_FLP_List = new List<FlowLayoutPanel>();
            FLP_Text_Total = new List<Label>();
            FLP_Title = new List<TextBox>();
            Hobby_Item_List = new List<Hobby_Item>();
            Container_List = new List<Container>();
            Free_Agent_Hobby_List = new List<Hobby_Item>();

            Hobby_Item_List = parent.Master_Hobby_Item_List.Where(x => x.Profile_Number == Get_Current_Profile_Index()).ToList();

            // Create containers based on maximum container ID found in Hobby_List
            int Max_Container_ID = Hobby_Item_List.Count == 0 ? 0 : Hobby_Item_List.Max(x => x.Container_ID);

            // If profile exists, create containers
            if (parent.Master_Container_Dict.Count > 0)
            {
                for (int i = 0; i < (parent.Master_Container_Dict[Get_Current_Profile_Index()]).Count; i++)
                {
                    Add_Container(parent.Master_Container_Dict[Get_Current_Profile_Index()][i].Name);
                }

                foreach (Hobby_Item item in Hobby_Item_List)
                {
                    Container_List[item.Container_ID - 1].Add_Hobby(item);
                }
                // Populate containers with existing Items
                //foreach (Hobby_Item item in Hobby_Item_List)
                /*
                for (int i = 0; i < Hobby_Item_List.Count; i++)
                {
                    Hobby_Item item = Hobby_Item_List[i];
                    int Master_Count = parent.Master_Item_List.Where(x => x.Category == item.Category && x.Name == item.Name).Sum(x => x.Quantity);
                    int Container_Count2 = Container_List[item.Container_ID - 1].Hobby_Item_List.Where(x => x.Category == item.Category && x.Name == item.Name).ToList().Count();
                    // Verify non-ghost entries (Extra hobby items that shouldn't exist but are in containers)
                    if (Master_Count - Container_Count2 > 0)
                    {
                        Container_List[item.Container_ID - 1].Add_Hobby(item);
                    }
                    else
                    {
                        Diagnostics.WriteLine("Ghost found and removed: " + item.Name);
                        Hobby_Item_List.RemoveAt(i);
                        // Reduce index by one to not skip
                        i--;
                    }
                }*/

                // Automatically descend container numbers that don't exist anymore (empty containers)
                for (int i = Container_List.Count - 1; i >= 0; i--)
                {
                    // If empty list, descend all container IDs above
                    if (Container_List[i].Hobby_Item_List.Count == 0)
                    {
                        Container_List.RemoveAt(i);
                        for (int j = i; j <= Container_List.Count - 1; j++)
                        {
                            Container_List[j].Hobby_Item_List.ForEach(x => x.Container_ID -= 1);
                            Container_List[j].ID = (Convert.ToInt32(Container_List[j].ID) - 1).ToString();
                        }
                    }
                }

                // Add buttons to containers based on hobby items count
                foreach (Container c in Container_List)
                {
                    int item_count = 0;
                    foreach (Hobby_Item item in c.Hobby_Item_List)
                    {
                        double Tax_Rate = parent.Tax_Rules_Dictionary.ContainsKey(item.Category) ? Convert.ToDouble(parent.Tax_Rules_Dictionary[item.Category]) : parent.Tax_Rate;
                        Size s = new Size(freeAgentFlowPanel.Width - 2, 26);
                        DragButton DB = new DragButton();
                        DB.AllowDrag = true;
                        DB.Size = s;
                        DB.Width = Max_Container_Width - 12;
                        DB.Text = item.Name + " ($" + String.Format("{0:0.00}", item.Price) + ")";
                        DB.Name = c.ID + "_" + item_count.ToString() + "_" + item.Unique_ID; // container number, item number, random seed
                        DB.FlatStyle = FlatStyle.Flat;
                        DB.Margin = new Padding(0);
                        DB.Padding = new Padding(0);
                        DB.ForeColor = (Highlight_Phantom && item.OrderID == "999999999") ? Color.Yellow : Color.White;
                        DB.Click += new EventHandler(DBButtonClick);

                        //DB.MouseUp += new MouseEventHandler(FadeIn_MouseUp);
                        Interior_FLP_List[Convert.ToInt32(c.ID) - 1].Controls.Add(DB);
                        Grouped_Button_List.Add(DB);
                        item_count++;
                    }

                    // Update total label
                    FLP_Text_Total[Convert.ToInt32(c.ID) - 1].Text = "TOTAL: $" + String.Format("{0:0.00}", Container_List[Convert.ToInt32(c.ID) - 1].Total);
                    FLP_Title[Convert.ToInt32(c.ID) - 1].Text = Container_List[Convert.ToInt32(c.ID) - 1].Name;

                    // Update Container Totals
                    Update_Total();
                }
            }

            Sync_Function();

            ResumeDrawing(mainLayoutPanel);

            Load_Complete = true;


            // Get max hobby in current profile
            category_box.Items.Clear();

            foreach (string g in parent.category_box.Items)
            {
                category_box.Items.Add(g);
            }

            // Select most appropriate current category based on maximum counts
            Dictionary<string, int> Max_Category_Count_Per_Profile = new Dictionary<string,int>();
            if (category_box.Items.Count > 0)
            {
                string Profile_Index = Get_Current_Profile_Index();
                foreach (string category in parent.category_box.Items)
                {
                    Max_Category_Count_Per_Profile.Add(category, Hobby_Item_List.Where(x => x.Profile_Number == Profile_Index && x.Category == category).ToList().Count);
                }
                var max = Max_Category_Count_Per_Profile.Aggregate((l, r) => l.Value > r.Value ? l : r).Key; // aggregate stores last time a value has swapped, giving location of kvp.key

                /* SImplifying above LINQ
                 * 
                            var l = results[0];
                            for(int i=1; i<results.Count(); ++i)
                            {
                                var r = results[i];
                                if(r.Value > l.Value)
                                    l = r;        
                            }
                            var max = l.Key;
                 * 
                 */

                category_box.Text = max.ToString();
                //category_box.Text = category_box.Items[0].ToString();
            }


        }

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            Transfer_To_Parent();

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

        // Master hobby list
        private List<Hobby_Item> Hobby_Item_List = new List<Hobby_Item>();

        private List<Item> Mirror_Master_Item_List = new List<Item>();
        private List<Container> Container_List = new List<Container>();

        // Free agent hobby list
        private List<Hobby_Item> Free_Agent_Hobby_List = new List<Hobby_Item>();

        private string Get_Current_Profile_Index()
        {
            //Diagnostics.WriteLine((profile_list.Items.IndexOf(profile_list.Text) + 1).ToString());
            if (!parent.Master_Container_Dict.ContainsKey((profile_list.Items.IndexOf(profile_list.Text) + 1).ToString()))
            {
                profile_list.Text = profile_list.Items[0].ToString();
            }
            return (profile_list.Items.IndexOf(profile_list.Text) + 1).ToString();
        }

        private void Sync_Function()
        {
            freeAgentFlowPanel.Controls.Clear();
            SuspendDrawing(freeAgentFlowPanel);
            
            //Button_List.ForEach(button => button.Image.Dispose());
            Button_List.ForEach(button => button.Visible = false);
            Button_List.ForEach(button => freeAgentFlowPanel.Controls.Remove(button));
            Button_List.ForEach(button => button.Dispose());
            Button_List = new List<DragButton>();
            
            Mirror_Master_Item_List = new List<Item>();
            parent.Master_Item_List.ForEach(x => Mirror_Master_Item_List.Add(x.Copy_Item()));
            Free_Agent_Hobby_List = new List<Hobby_Item>();

            foreach (Hobby_Item HI in Hobby_Item_List)
            {
                foreach (Item item in Mirror_Master_Item_List)
                {
                    if (item.OrderID == HI.OrderID && item.Name == HI.Name)
                    {
                        item.Quantity--;
                    }
                }
            }

            // Filter out non-existing items and current categories
            Mirror_Master_Item_List = Mirror_Master_Item_List.Where(x => x.Quantity > 0 && x.Status == "0" && category_box.Text == x.Category).ToList();

            Mirror_Master_Item_List = Mirror_Master_Item_List.OrderBy(x => x.Name).ToList();

            foreach (Item item in Mirror_Master_Item_List)
            {
                double itemOrigQuantity = item.Quantity;

                while (item.Quantity > 0)
                {
                    string button_name = Get_Next_Random();
                    double Tax_Rate = parent.Tax_Rules_Dictionary.ContainsKey(item.Category) ? Convert.ToDouble(parent.Tax_Rules_Dictionary[item.Category]) : parent.Tax_Rate;
                    Size s = new Size(freeAgentFlowPanel.Width - 2, 26);
                    DragButton DB = new DragButton();
                    DB.AllowDrag = true;
                    DB.Size = s;
                    DB.Text = item.Name + " ($" + String.Format("{0:0.00}", (item.Price * (1 + Tax_Rate)) - (item.Discount_Amt > 0 ? (item.Discount_Amt / parent.Master_Item_List.FirstOrDefault(x => x.Name == item.Name && x.OrderID == item.OrderID).Quantity) : 0)) + ")";
                    DB.Name = button_name.ToString();
                    DB.FlatStyle = FlatStyle.Flat;
                    DB.Margin = new Padding(0);
                    DB.Padding = new Padding(0);
                    DB.ForeColor = (Highlight_Phantom && item.OrderID == "999999999") ? Color.Yellow : Color.White;
                    DB.Click += DBButtonClick;

                    //DB.MouseUp += new MouseEventHandler(FadeIn_MouseUp);
                    //freeAgentFlowPanel.Controls.Add(DB);
                    Button_List.Add(DB);

                    item.Quantity -= 1;

                    Hobby_Item Temp_HI = new Hobby_Item()
                    {
                        Name = item.Name,
                        OrderID = item.OrderID,
                        Category = item.Category,
                        Price = (item.Price * (1 + Tax_Rate)) - (item.Discount_Amt > 0 ? (item.Discount_Amt / parent.Master_Item_List.FirstOrDefault(x => x.Name == item.Name && x.OrderID == item.OrderID).Quantity) : 0),
                        Container_ID = 0,
                        Unique_ID = button_name,
                        Profile_Number = "0" //Get_Current_Profile_Index()
                    };

                    Free_Agent_Hobby_List.Add(Temp_HI);
                }
            }

            Free_Agent_Total = Free_Agent_Hobby_List.Sum(x => x.Price);

            label4.Text = "Sum of above items: $" + String.Format("{0:0.00}", Free_Agent_Total);

            freeAgentFlowPanel.Controls.AddRange(Button_List.ToArray());
            ResumeDrawing(freeAgentFlowPanel);
        }

        double Free_Agent_Total = 0;

        List<DragButton> Button_List = new List<DragButton>();
        List<DragButton> Grouped_Button_List = new List<DragButton>();


        int Max_Container_Width = 0;
        int Max_Container_Height = 0;
        //int Max_Container_Height = 200;
        int Container_Count = 1;

        // Add new container
        private void Add_button_Click(object sender, EventArgs e)
        {
            if (parent.Hobby_Profile_List.Count == 0)
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Please add a new profile before trying to add a container", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                using (var form1 = new Hobby_MGMT_Settings(parent, this, this.Location, this.Size))
                {
                    var result21 = form1.ShowDialog();
                    if (result21 == DialogResult.OK && form1.ReturnValue == "1")
                    {
                        Reset_Panels();
                    }
                }
                Grey_In();
            }
            else
            {
                Add_Container("", true);
            }
        }

        List<Button> Swap_Button_List = new List<Button>();
        List<Button> Delete_Button_List = new List<Button>();
        List<FlowLayoutPanel> FLP_List = new List<FlowLayoutPanel>();
        List<FlowLayoutPanel> Interior_FLP_List = new List<FlowLayoutPanel>();
        List<Label> FLP_Text_Total = new List<Label>();
        List<TextBox> FLP_Title = new List<TextBox>();

        private void Add_Container(string text = "", bool Set_Title_Focus = false)
        {
            MyFlowLayoutPanel FLP = new MyFlowLayoutPanel();
            FLP.AllowDrop = true;
            FLP.DragEnter += new DragEventHandler(flowLayoutPanel_DragEnter);
            FLP.DragDrop += new DragEventHandler(flowLayoutPanel_DragDrop);
            FLP.BorderStyle = BorderStyle.FixedSingle;
            FLP.FlowDirection = FlowDirection.TopDown;
            FLP.BackColor = Color.FromArgb(85, 85, 85);
            FLP.ForeColor = Color.White;
            FLP.AutoSize = false;
            FLP.MinimumSize = new Size(Max_Container_Width, Max_Container_Height);
            FLP.Size = FLP.MinimumSize;
            FLP.MaximumSize = new Size(Max_Container_Width, Max_Container_Height);
            FLP.Padding = new Padding(0);
            FLP.AutoScroll = false;

            #region TitleArea

            MyFlowLayoutPanel Title_FLP = new MyFlowLayoutPanel();
            Title_FLP.FlowDirection = FlowDirection.LeftToRight;
            Title_FLP.BorderStyle = BorderStyle.None;
            Title_FLP.BackColor = Color.FromArgb(85, 85, 85);
            Title_FLP.Padding = new Padding(0, 0, 0, 0);
            Title_FLP.ForeColor = Color.White;
            Title_FLP.AutoSize = false;
            Title_FLP.Size = new Size(Max_Container_Width - 10, 28);

            int button_width = 25;
            
            // Add total label below
            /*
            Label leftspace = new Label()
            {
                //Size = new Size(Title_FLP.Height, Title_FLP.Height)
                Size = new Size(button_width, button_width)
            };*/


            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            Button left_space = new Button();
            left_space.BackColor = Color.FromArgb(85, 85, 85);
            left_space.ForeColor = Color.FromArgb(85, 85, 85);
            left_space.FlatStyle = FlatStyle.Flat;
            left_space.Image = global::Financial_Journal.Properties.Resources.swap;
            left_space.Size = new Size(23, 19);//leftspace.Width - 2, leftspace.Width - 2);
            left_space.Name = "s" + Container_Count.ToString();
            left_space.Text = "23";
            left_space.Click += new EventHandler(this.dynamic_button_click);
            ToolTip1.SetToolTip(left_space, "Move this container");

            // Title block
            TextBox temp_box = new TextBox()
                {
                    Font = new Font("Microsoft Sans Serif", 13.25F, FontStyle.Bold),
                    ForeColor = FLP.ForeColor,
                    Size = new Size(Title_FLP.Width - 14 - button_width * 2, Title_FLP.Height),
                    BorderStyle = BorderStyle.None,
                    BackColor = FLP.BackColor,
                    TextAlign = HorizontalAlignment.Center,
                    Text = text.Length == 0 ? "Insert_Group_Title" : text
                    
                };
            

            Button delete_button = new Button();
            delete_button.BackColor = Color.FromArgb(85, 85, 85);
            delete_button.ForeColor = Color.FromArgb(85, 85, 85);
            delete_button.FlatStyle = FlatStyle.Flat;
            delete_button.Image = global::Financial_Journal.Properties.Resources.delete_hobby;
            delete_button.Size = new Size(23,19);//leftspace.Width - 2, leftspace.Width - 2);
            delete_button.Name = "d" + Container_Count.ToString();
            delete_button.Text = "23";
            delete_button.Click += new EventHandler(this.dynamic_button_click);
            ToolTip1.SetToolTip(delete_button, "Delete this container");

            Swap_Button_List.Add(left_space);
            Delete_Button_List.Add(delete_button);
            Title_FLP.Controls.Add(left_space);
            Title_FLP.Controls.Add(temp_box);
            Title_FLP.Controls.Add(delete_button);

            #endregion

            FLP.Controls.Add(Title_FLP);
            FLP_Title.Add(temp_box);

            // Add total label below
            Label temp = new Label()
            {
                Text = "$0",
                Font = new Font("Microsoft Sans Serif", 11f, FontStyle.Regular),
                ForeColor = FLP.ForeColor,
                Size = new Size(Max_Container_Width - 10, 25),
                BorderStyle = BorderStyle.None,
                BackColor = FLP.BackColor,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            };

            FLP_Text_Total.Add(temp);
            FLP.Controls.Add(temp);

            MyFlowLayoutPanel Interior_FLP = new MyFlowLayoutPanel();
            Interior_FLP.AllowDrop = true;
            Interior_FLP.DragEnter += new DragEventHandler(flowLayoutPanel_DragEnter);
            Interior_FLP.DragDrop += new DragEventHandler(flowLayoutPanel_DragDrop);
            Interior_FLP.BorderStyle = BorderStyle.FixedSingle;
            Interior_FLP.FlowDirection = FlowDirection.TopDown;
            Interior_FLP.BackColor = Color.FromArgb(85, 85, 85);
            Interior_FLP.ForeColor = Color.White;
            Interior_FLP.AutoSize = true;
            Interior_FLP.Name = "Container_" + Container_Count;
            Interior_FLP.MinimumSize = new Size(Max_Container_Width - 10, Max_Container_Height - 68);
            Interior_FLP.Size = Interior_FLP.MinimumSize;
            Interior_FLP.MaximumSize = new Size(Max_Container_Width - 10, Max_Container_Height - 68);
            Interior_FLP.Padding = new Padding(0);
            Interior_FLP.WrapContents = false;
            Interior_FLP.HorizontalScroll.Maximum = 0;
            Interior_FLP.AutoScroll = false;
            Interior_FLP.VerticalScroll.Visible = false;
            Interior_FLP.AutoScroll = true;
            Interior_FLP.DragEnter += new DragEventHandler(drag_Fade_Out);
            Interior_FLP.DragDrop += new DragEventHandler(drag_Fade_In);

            FLP.Controls.Add(Interior_FLP);
            mainLayoutPanel.Controls.Add(FLP);
            Container_List.Add(new Container() { ID = Container_Count.ToString(), Name = text, Total = 0, Hobby_Item_List = new List<Hobby_Item>()});

            // Add dynamic lists for access later
            Interior_FLP_List.Add(Interior_FLP);
            FLP_List.Add(FLP);

            Container_Count++;

            if (Set_Title_Focus) temp_box.Focus();
        }

        int Swap_Index = -1;

        private void dynamic_button_click(object sender, EventArgs e)
        {
            Button b = (Button)sender;

            if (b.Name.StartsWith("d"))
            {
                Grey_Out();
                using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to delete this container?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                {
                    var result21 = form1.ShowDialog();
                    if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                    {
                        for (int i = Hobby_Item_List.Count - 1; i >= 0; i--)
                        {
                            if (Hobby_Item_List[i].Container_ID.ToString() == b.Name.Substring(1))
                            {
                                Free_Agent_Total += Hobby_Item_List[i].Price;
                                Hobby_Item_List.RemoveAt(i);
                            }
                        }
                        Container_List[Convert.ToInt32(b.Name.Substring(1)) - 1].Hobby_Item_List = new List<Hobby_Item>();
                        Reset_Panels();
                    }
                }
                Grey_In();
            }
            else if (b.Name.StartsWith("s") && Swap_Index < 0)
            {
                // Reset colors first
                Swap_Button_List.ForEach(x => x.Image = global::Financial_Journal.Properties.Resources.swap);

                // Set focus to current red one
                b.Image = global::Financial_Journal.Properties.Resources.swapred;
                b.BringToFront();

                Swap_Index = Convert.ToInt32(b.Name.Substring(1));
            }
            else if (b.Name.StartsWith("s") && Swap_Index >= 0 && Swap_Index != Convert.ToInt32(b.Name.Substring(1)))
            {
                // Reset colors first
                Swap_Button_List.ForEach(x => x.Image = global::Financial_Journal.Properties.Resources.swap);

                int Source_Index = Swap_Index;
                int Destination_Index = Convert.ToInt32(b.Name.Substring(1));

                // Swap all source index hobby ID to a temporary number say 999999 first (typical swap algorith)
                Hobby_Item_List.Where(x => x.Container_ID == Source_Index).ToList().ForEach(y => y.Container_ID = 999999);

                // Change all destination hobby ID to source_index
                Hobby_Item_List.Where(x => x.Container_ID == Destination_Index).ToList().ForEach(y => y.Container_ID = Source_Index);

                // Change all source_index hobby ID to destination
                Hobby_Item_List.Where(x => x.Container_ID == 999999).ToList().ForEach(y => y.Container_ID = Destination_Index);

                // Adjust index to 0-start 
                Source_Index--;
                Destination_Index--;

                // Swap container_list index
                Container temp = Container_List[Source_Index];
                temp.ID = (Destination_Index + 1).ToString();
                Container_List[Source_Index] = Container_List[Destination_Index];
                Container_List[Source_Index].ID = (Source_Index + 1).ToString();
                Container_List[Destination_Index] = temp;

                // Swap container_list index
                string temp2 = FLP_Title[Source_Index].Text;
                FLP_Title[Source_Index].Text = FLP_Title[Destination_Index].Text;
                FLP_Title[Destination_Index].Text = temp2;

                Swap_Index = -1;
                Reset_Panels();
            }
            else
            {
                // Reset colors first
                Swap_Button_List.ForEach(x => x.Image = global::Financial_Journal.Properties.Resources.swap);
                Swap_Index = -1;
            }
        }

        #region FadeIn/Out Functionalities

        private async void FadeIn(FadeControl o, int interval = 80)
        {
            //Object is not fully invisible. Fade it in
            while (o.Opacity < 99)
            {
                await Task.Delay(interval);
                o.Opacity += 20;
                //o.Opacity += 0.03;
            }
            o.Opacity = 100; //make fully visible    
        }

        private async void FadeOut(FadeControl o, int interval = 80)
        {
            //Object is fully visible. Fade it out
            while (o.Opacity > 1)
            {
                await Task.Delay(interval);
                o.Opacity -= 20;
            }
            o.Opacity = 0; //make fully invisible
        }

        private void drag_Fade_In(object sender, DragEventArgs e)
        {
            FadeIn(TFLP, 1);
        }

        private void drag_Fade_Out(object sender, DragEventArgs e)
        {
            FadeOut(TFLP, 1);
        }

        #endregion

        void flowLayoutPanel_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        public static void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        public void Update_Total()
        {
            label3.Text = "Hobby Total: $" + String.Format("{0:0.00}", Container_List.Sum(x => x.Total));
        }

        private void DBButtonClick(object sender, EventArgs e)
        {
            DragButton DB = (DragButton)sender;
            PhantomText1.Text = DB.Text.Substring(0, DB.Text.LastIndexOf("($") - 1);
            PhantomText2.Text = DB.Text.Substring(DB.Text.LastIndexOf("($") + 1, DB.Text.LastIndexOf(")") - (DB.Text.LastIndexOf("($") + 1));
        }

        // Main drag/drop function
        private void flowLayoutPanel_DragDrop(object sender, DragEventArgs e)
        {
            DragButton data = (DragButton)e.Data.GetData(typeof(DragButton));
            FlowLayoutPanel _destination = (FlowLayoutPanel)sender;
            FlowLayoutPanel _source = (FlowLayoutPanel)data.Parent;

            Hobby_Item ref_item;

            Point p1 = _destination.PointToClient(new Point(e.X, e.Y));

            if (p1.Y > 0)
            {
                // If garbage panel
                if (_source.Name != "freeAgentFlowPanel" && (_destination == garbagePanel || _destination == TFLP))
                {
                    int ContainerID = Convert.ToInt32(data.Name.Split(new string[] { "_" }, StringSplitOptions.None)[0]);
                    int Item_Index = Convert.ToInt32(data.Name.Split(new string[] { "_" }, StringSplitOptions.None)[1]);
                    string Seed_Number = data.Name.Split(new string[] { "_" }, StringSplitOptions.None)[2];

                    Hobby_Item item = Container_List[ContainerID - 1].Hobby_Item_List.FirstOrDefault(x => x.Unique_ID == Seed_Number);

                    if (item != null)
                    {
                        // remove from source 
                        Container_List[ContainerID - 1].Remove_Hobby(item);
                        Hobby_Item_List.Remove(item);

                        FLP_Text_Total[item.Container_ID - 1].Text = "TOTAL: $" + String.Format("{0:0.00}", Container_List[item.Container_ID - 1].Total); 
                        data.Name = item.Container_ID + "_" + Item_Index + "_" + Seed_Number;

                        // Update Container Totals
                        Update_Total();

                        // Move item back to free agent
                        string button_name = Get_Next_Random();
                        double Tax_Rate = parent.Tax_Rules_Dictionary.ContainsKey(item.Category) ? Convert.ToDouble(parent.Tax_Rules_Dictionary[item.Category]) : parent.Tax_Rate;
                        Size s = new Size(freeAgentFlowPanel.Width - 2, 26);
                        DragButton DB = new DragButton();
                        DB.AllowDrag = true;
                        DB.Size = s;
                        DB.Text = item.Name + " ($" + String.Format("{0:0.00}", item.Price) + ")";
                        DB.Name = button_name.ToString();
                        DB.FlatStyle = FlatStyle.Flat;
                        DB.Margin = new Padding(0);
                        DB.Padding = new Padding(0);
                        DB.ForeColor = (Highlight_Phantom && item.OrderID == "999999999") ? Color.Yellow : Color.White;
                        DB.Click += new EventHandler(DBButtonClick);

                        freeAgentFlowPanel.Controls.Add(DB);
                        Button_List.Add(DB);
                        Grouped_Button_List.Remove(Grouped_Button_List.FirstOrDefault(x => x.Name.Contains(button_name)));

                        Hobby_Item Temp_HI = new Hobby_Item()
                        {
                            Name = item.Name,
                            OrderID = item.OrderID,
                            Category = item.Category,
                            Price = item.Price,
                            Container_ID = 0,
                            Unique_ID = button_name,
                            Profile_Number = Get_Current_Profile_Index()
                        };

                        Free_Agent_Hobby_List.Add(Temp_HI);
                        Free_Agent_Total += Temp_HI.Price;

                        label4.Text = "Sum of above items: $" + String.Format("{0:0.00}", Free_Agent_Total);
                    }


                    data.Dispose();

                    _destination.Invalidate();
                    _source.Invalidate();
                    freeAgentFlowPanel.Update();
                }
                // from free agent to container
                else if (_source != _destination && _source.Name == "freeAgentFlowPanel" && _destination.Name.Contains("Container"))
                {
                    #region free agent to container
                    if (_source == freeAgentFlowPanel)
                    {
                        Grouped_Button_List.Add(data);
                        Button_List.Remove(data);
                    }
                    // Add control to panel
                    _destination.Controls.Add(data);
                    //_destination.Width = _destination.Width - 8;
                    data.Size = new Size(_destination.Width - 2, data.Height);

                    // Reorder
                    Point p = _destination.PointToClient(new Point(e.X, e.Y));
                    var item = _destination.GetChildAtPoint(p);
                    int index = _destination.Controls.GetChildIndex(item, false);
                    _destination.Controls.SetChildIndex(data, index);

                    // Move from one list to another and set container ID
                    ref_item = Free_Agent_Hobby_List.FirstOrDefault(x => x.Unique_ID == data.Name);
                    ref_item.Container_ID = Convert.ToInt32(_destination.Name.Substring(10));
                    ref_item.Profile_Number = Get_Current_Profile_Index();
                    // add to destination container
                    Container_List[ref_item.Container_ID - 1].Add_Hobby(ref_item);
                    Hobby_Item_List.Add(ref_item);
                    data.Name = ref_item.Container_ID + "_" + (Container_List[ref_item.Container_ID - 1].Hobby_Item_List.Count - 1).ToString() + "_" + data.Name;

                    // Update Total price
                    Free_Agent_Total -= ref_item.Price;
                    label4.Text = "Sum of above items: $" + String.Format("{0:0.00}", Free_Agent_Total);

                    // Update total label
                    FLP_Text_Total[ref_item.Container_ID - 1].Text = "TOTAL: $" + String.Format("{0:0.00}", Container_List[ref_item.Container_ID - 1].Total);

                    // Update Container Totals
                    Update_Total();

                    //Free_Agent_Hobby_List.RemoveAt(Convert.ToInt32(data.Name));

                    _source.Invalidate();
                    _destination.Invalidate();
#endregion
                }
                // from container to container
                else if (_source != _destination && _source.Name.Contains("Container") && _destination.Name.Contains("Container"))
                {
                    #region container to container
                    if (_source == freeAgentFlowPanel)
                    {
                        Grouped_Button_List.Add(data);
                        Button_List.Remove(data);
                    }
                    // Add control to panel
                    _destination.Controls.Add(data);
                    //_destination.Width = _destination.Width - 8;
                    data.Size = new Size(_destination.Width - 2, data.Height);

                    // Reorder
                    Point p = _destination.PointToClient(new Point(e.X, e.Y));
                    var item = _destination.GetChildAtPoint(p);
                    int index = _destination.Controls.GetChildIndex(item, false);
                    _destination.Controls.SetChildIndex(data, index);

                    // Move from one list to another and set container ID
                    int ContainerID = Convert.ToInt32(data.Name.Split(new string[] { "_" }, StringSplitOptions.None)[0]);
                    int Item_Index = Convert.ToInt32(data.Name.Split(new string[] { "_" }, StringSplitOptions.None)[1]);
                    string Seed_Number = data.Name.Split(new string[] { "_" }, StringSplitOptions.None)[2];

                    ref_item = Container_List[ContainerID - 1].Hobby_Item_List.FirstOrDefault(x => x.Unique_ID == Seed_Number);

                    if (ref_item != null)
                    {
                        // remove from source 
                        Container_List[ContainerID - 1].Remove_Hobby(ref_item);
                        ref_item.Container_ID = Convert.ToInt32(_destination.Name.Substring(10));
                        data.Name = ref_item.Container_ID + "_" + Item_Index + "_" + Seed_Number;
                        // add to destimation container
                        Container_List[ref_item.Container_ID - 1].Add_Hobby(ref_item);

                        // Update total labels for source and destination
                        FLP_Text_Total[ref_item.Container_ID - 1].Text = "TOTAL: $" + String.Format("{0:0.00}", Container_List[ref_item.Container_ID - 1].Total);
                        FLP_Text_Total[ContainerID - 1].Text = "TOTAL: $" + String.Format("{0:0.00}", Container_List[ContainerID - 1].Total);

                        data.ForeColor = (Highlight_Phantom && ref_item.OrderID == "999999999") ? Color.Yellow : Color.White;

                        // Update Container Totals
                        Update_Total();
                    }

                    _source.Invalidate();
                    _destination.Invalidate();
#endregion
                }
                else
                {
                    FadeIn(TFLP, 1);
                    // Just add the control to the new panel.
                    // No need to remove from the other panel,
                    // this changes the Control.Parent property.
                    Point p = _destination.PointToClient(new Point(e.X, e.Y));
                    var item = _destination.GetChildAtPoint(p);
                    int index = _destination.Controls.GetChildIndex(item, false);
                    _destination.Controls.SetChildIndex(data, index);

                    //_destination.Invalidate();
                }
            }
            try
            {
                _source.Update();
                _destination.Update();
                FadeIn(TFLP, 1);
            }
            catch
            {
            }
        }

        private void memo_button_Click(object sender, EventArgs e)
        {
            category_box.Enabled = !category_box.Enabled;
        }

        private void category_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            Sync_Function();
        }

        private List<int> Random_Numbers = new List<int>();
        Random r = new Random();

        private string Get_Next_Random()
        { 
            int randNum = r.Next(100000000, 999999999);
            while (Random_Numbers.Contains(randNum))
            {
                Random_Numbers.Add(randNum);
                randNum = r.Next(100000000, 999999999);
            }

            return randNum.ToString();
        }

        private void maximize_button_Click(object sender, EventArgs e)
        {
            // Minimized
            if (maximize_button.Text == "❒")
            {
                maximize_button.Text = "☐";
                this.WindowState = FormWindowState.Normal;
                parent.Hobby_Window_Maximized = false;
            }
            // Maximized
            else
            {
                maximize_button.Text = "❒";
                this.WindowState = FormWindowState.Maximized;
                parent.Hobby_Window_Maximized = true;
            }
        }

        private void clear_all_button_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form1 = new Yes_No_Dialog(parent, "Are you sure you wish to remove all items from groups?", "Warning", "No", "Yes", 0, this.Location, this.Size))
            {
                var result21 = form1.ShowDialog();
                if (result21 == DialogResult.OK && form1.ReturnValue1 == "1")
                {
                    foreach (DragButton data in Grouped_Button_List)
                    {
                        int ContainerID = Convert.ToInt32(data.Name.Split(new string[] { "_" }, StringSplitOptions.None)[0]);
                        int Item_Index = Convert.ToInt32(data.Name.Split(new string[] { "_" }, StringSplitOptions.None)[1]);
                        string Seed_Number = data.Name.Split(new string[] { "_" }, StringSplitOptions.None)[2];

                        Hobby_Item item = Container_List[ContainerID - 1].Hobby_Item_List.FirstOrDefault(x => x.Unique_ID == Seed_Number);

                        if (item != null)
                        {
                            // remove from source 
                            Container_List[ContainerID - 1].Remove_Hobby(item);
                            Hobby_Item_List.Remove(item);

                            FLP_Text_Total[item.Container_ID - 1].Text = "TOTAL: $" + String.Format("{0:0.00}", Container_List[item.Container_ID - 1].Total);
                            data.Name = item.Container_ID + "_" + Item_Index + "_" + Seed_Number;

                            // Update Container Totals
                            Update_Total();

                            // Move item back to free agent
                            string button_name = Get_Next_Random();
                            double Tax_Rate = parent.Tax_Rules_Dictionary.ContainsKey(item.Category) ? Convert.ToDouble(parent.Tax_Rules_Dictionary[item.Category]) : parent.Tax_Rate;
                            Size s = new Size(freeAgentFlowPanel.Width - 2, 26);
                            DragButton DB = new DragButton();
                            DB.AllowDrag = true;
                            DB.Size = s;
                            DB.Text = item.Name + " ($" + String.Format("{0:0.00}", item.Price) + ")";
                            DB.Name = button_name.ToString();
                            DB.FlatStyle = FlatStyle.Flat;
                            DB.Margin = new Padding(0);
                            DB.Padding = new Padding(0);
                            DB.ForeColor = Color.White;
                            DB.Click += new EventHandler(DBButtonClick);

                            freeAgentFlowPanel.Controls.Add(DB);
                            Button_List.Add(DB);

                            Hobby_Item Temp_HI = new Hobby_Item()
                            {
                                Name = item.Name,
                                OrderID = item.OrderID,
                                Category = item.Category,
                                Price = item.Price,
                                Container_ID = 0,
                                Unique_ID = button_name,
                                Profile_Number = Get_Current_Profile_Index()
                            };

                            Free_Agent_Hobby_List.Add(Temp_HI);
                            Free_Agent_Total += item.Price;

                            data.Dispose();
                        }
                    }

                    Grey_Out();
                    using (var form = new Yes_No_Dialog(parent, "Do you want to remove all the empty groups?", "Warning", "No", "Yes", 0, this.Location, this.Size))
                    {
                        var result2 = form.ShowDialog();
                        if (result2 == DialogResult.OK && form.ReturnValue1 == "1")
                        {
                            //parent.Reload_Hobby_Window = true;
                            Reset_Panels();
                            //close_button.PerformClick();
                        }
                    }
                    Grey_In();
                }
            }
            Grey_In();
            this.Invalidate();
            Interior_FLP_List.ForEach(x => x.Invalidate());
            Interior_FLP_List.ForEach(x => x.Update());
        }

        private void garbagePanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void mouseClick(object sender, MouseEventArgs e)
        {
            bank_box.Text = "";
        }

        private void bank_box_TextChanged(object sender, EventArgs e)
        {
            // Reset all colors
            int first_index = -1;
            int index = 0; 
            Button_List.ForEach(x => x.ForeColor = Color.White);
            if (bank_box.Text.Length > 0)
            {
                foreach (DragButton DB in Button_List)
                {
                    if ((DB.Text).ToLower().Contains(bank_box.Text.ToLower()))
                    {
                        if (first_index < 0) first_index = index;
                        DB.ForeColor = ColorTranslator.FromHtml("#EB5071");
                    }
                    index++;   
                }
            }
            if (first_index >= 0) freeAgentFlowPanel.ScrollControlIntoView(Button_List[first_index]);
        }

        void gmh_TheMouseMoved()
        {
            FadeIn(TFLP, 1);
        }

        private void settings_button_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form1 = new Hobby_MGMT_Settings(parent, this, this.Location, this.Size))
            {
                var result21 = form1.ShowDialog();
                if (result21 == DialogResult.OK && form1.ReturnValue == "1")
                {
                    //parent.Reload_Hobby_Window = true;
                    //close_button.PerformClick();
                    Reset_Panels();
                }
                else if (result21 == DialogResult.OK)
                {
                }
            }
            Grey_In();
        }

        private void sort_button_Click(object sender, EventArgs e)
        {
            Reset_Panels();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }
        

        private void profile_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            Reset_Panels();
            Previous_Profile_Index = (profile_list.Items.IndexOf(profile_list.Text) + 1).ToString();
            //Diagnostics.WriteLine(Previous_Profile_Index);
            SendKeys.Send("{TAB}");
        }

        bool Showing_Phantom_Item_Adder = false;

        TextBox PhantomText1 = new TextBox();
        TextBox PhantomText2 = new TextBox();
        Label PhantomLabel1 = new Label();
        Label PhantomLabel2 = new Label();
        Panel PhantomPanel = new Panel();
        Button Add_Phantom_Item_Button = new Button();

        // enable phantom item additions
        private void button1_Click(object sender, EventArgs e)
        {

            if (!Showing_Phantom_Item_Adder)
            {
                PhantomPanel = new Panel()
                    {
                        Size = new Size(freeAgentFlowPanel.Width, 70),
                        BackColor = Color.FromArgb(65, 65, 65),

                        ForeColor = Color.White,
                        BorderStyle = BorderStyle.FixedSingle,
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Left,

                        Location = new Point(freeAgentFlowPanel.Location.X, freeAgentFlowPanel.Location.Y + freeAgentFlowPanel.Height + 5)
                    };

                int TextBox_Width = 95;
                int Label_Width = 60;

                PhantomLabel1 = new Label()
                {
                    Text = "Name:",
                    Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular),
                    ForeColor = PhantomPanel.ForeColor,
                    Size = new Size(Label_Width, PhantomPanel.Height / 2 - 2),
                    BorderStyle = BorderStyle.None,
                    BackColor = PhantomPanel.BackColor,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleRight,
                    Location = new Point(0, 3)
                };

                PhantomLabel2 = new Label()
                {
                    Text = "Price:",
                    Font = new Font("Microsoft Sans Serif", 10, FontStyle.Regular),
                    ForeColor = PhantomPanel.ForeColor,
                    Size = new Size(Label_Width, PhantomPanel.Height / 2 - 2),
                    BorderStyle = BorderStyle.None,
                    BackColor = PhantomPanel.BackColor,
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleRight,
                    Location = new Point(0, PhantomPanel.Height / 2)
                };


                PhantomText1 = new TextBox()
                {
                    Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular),
                    ForeColor = PhantomPanel.ForeColor,
                    Size = new Size(TextBox_Width, PhantomPanel.Height / 2 - 2),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = PhantomPanel.BackColor,
                    TextAlign = HorizontalAlignment.Right,
                    Text = "",
                    Location = new Point(PhantomLabel1.Width + 6, 7)
                };

                PhantomText2 = new TextBox()
                {
                    Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Regular),
                    ForeColor = PhantomPanel.ForeColor,
                    Size = new Size(TextBox_Width, PhantomPanel.Height / 2 - 2),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = PhantomPanel.BackColor,
                    TextAlign = HorizontalAlignment.Right,
                    Text = "$",
                    Location = new Point(PhantomLabel1.Width + 6, PhantomPanel.Height / 2 + 3)
                };

                PhantomText1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxEnterKey_KeyPress1);
                PhantomText2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textboxEnterKey_KeyPress);
                PhantomText2.TextChanged += new EventHandler(textBox_TextChanged1);

                ToolTip ToolTip1 = new ToolTip();
                ToolTip1.InitialDelay = 1;
                ToolTip1.ReshowDelay = 1;

                Add_Phantom_Item_Button = new Button();
                Add_Phantom_Item_Button.BackColor = this.BackColor;
                Add_Phantom_Item_Button.ForeColor = this.BackColor;
                Add_Phantom_Item_Button.FlatStyle = FlatStyle.Flat;
                Add_Phantom_Item_Button.Image = global::Financial_Journal.Properties.Resources.addphantom;
                Add_Phantom_Item_Button.Enabled = true;
                Add_Phantom_Item_Button.Size = new Size(35, 35);
                Add_Phantom_Item_Button.Location = new Point(PhantomText1.Left + PhantomText1.Width + 20, PhantomText1.Top + 12);
                Add_Phantom_Item_Button.Name = "add_phantom_item_button";
                Add_Phantom_Item_Button.Text = "";
                Add_Phantom_Item_Button.Click += new EventHandler(this.add_phantom_item_click);
                ToolTip1.SetToolTip(Add_Phantom_Item_Button, "Add phantom item");

                freeAgentFlowPanel.Height -= (PhantomPanel.Height + 5);
                PhantomPanel.Top -= (PhantomPanel.Height) + 5;
                PhantomPanel.Controls.Add(PhantomLabel1);
                PhantomPanel.Controls.Add(PhantomLabel2);
                PhantomPanel.Controls.Add(PhantomText1);
                PhantomPanel.Controls.Add(PhantomText2);
                PhantomPanel.Controls.Add(Add_Phantom_Item_Button);
                this.Controls.Add(PhantomPanel);
            }
            else
            {
                freeAgentFlowPanel.Height += (PhantomPanel.Height + 5);
                PhantomLabel1.Dispose();
                PhantomLabel2.Dispose();
                Add_Phantom_Item_Button.Dispose();
                Add_Phantom_Item_Button.Image.Dispose();
                PhantomPanel.Dispose();
                PhantomText1.Dispose();
                PhantomText2.Dispose();
            }

            Showing_Phantom_Item_Adder = !Showing_Phantom_Item_Adder;
        }

        // Add phantom item to list
        private void add_phantom_item_click(object sender, EventArgs e)
        {
            if (PhantomText1.Text.Length > 0 && PhantomText2.Text.Length > 1 && PhantomText2.Text[PhantomText2.Text.Length - 1] != '.')
            {
                string button_name = Get_Next_Random();

                Hobby_Item Temp_HI = new Hobby_Item()
                {
                    Name = PhantomText1.Text,
                    OrderID = "999999999",
                    Category = category_box.Text,
                    Price = Convert.ToDouble(PhantomText2.Text.Substring(1)),
                    Container_ID = 0,
                    Unique_ID = button_name,
                    Profile_Number = Get_Current_Profile_Index()
                };

                Free_Agent_Hobby_List.Add(Temp_HI);

                double Tax_Rate = parent.Tax_Rules_Dictionary.ContainsKey(Temp_HI.Category) ? Convert.ToDouble(parent.Tax_Rules_Dictionary[Temp_HI.Category]) : parent.Tax_Rate;
                Size s = new Size(freeAgentFlowPanel.Width - 2, 26);
                DragButton DB = new DragButton();
                DB.AllowDrag = true;
                DB.Size = s;
                DB.Text = Temp_HI.Name + " ($" + String.Format("{0:0.00}", (Temp_HI.Price)) + ")";
                DB.Name = button_name.ToString();
                DB.FlatStyle = FlatStyle.Flat;
                DB.Margin = new Padding(0);
                DB.Padding = new Padding(0);
                DB.ForeColor = Color.White;
                DB.Click += new EventHandler(DBButtonClick);

                Free_Agent_Total += Temp_HI.Price;
                label4.Text = "Sum of above items: $" + String.Format("{0:0.00}", Free_Agent_Total);

                freeAgentFlowPanel.Controls.Add(DB);
                Button_List.Add(DB);

                PhantomText1.Text = "";
                PhantomText2.Text = "";

                this.ActiveControl = PhantomText1;
            }
        }


        // If press enter on length box, activate add (nmemonics)
        private void textboxEnterKey_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox g = (TextBox)sender;
            if (e.KeyChar == (char)Keys.Enter && g.Text.Length > 0)
            {
                Add_Phantom_Item_Button.PerformClick();
            }
        }

        // If press enter on length box, activate add (nmemonics)
        private void textboxEnterKey_KeyPress1(object sender, KeyPressEventArgs e)
        {
            TextBox g = (TextBox)sender;
            if (e.KeyChar == (char)Keys.Enter && g.Text.Length > 0)
            {
                SendKeys.Send("{TAB}");
            }
        }

        private void textBox_TextChanged1(object sender, EventArgs e)
        {
            TextBox TB = (TextBox)sender;

            if (!(TB.Text.StartsWith("$")))
            {
                if (parent.Get_Char_Count(TB.Text, Convert.ToChar("$")) == 1)
                {
                    string temp = TB.Text;
                    TB.Text = temp.Substring(1) + temp[0];
                    TB.SelectionStart = TB.Text.Length;
                    TB.SelectionLength = 0;
                }
                else
                {
                    TB.Text = "$" + TB.Text;
                }
            }
            else if ((TB.Text.Length > 1) && ((parent.Get_Char_Count(TB.Text, Convert.ToChar(".")) > 1) || (TB.Text[1].ToString() == ".") || (parent.Get_Char_Count(TB.Text, Convert.ToChar("$")) > 1) || (!((TB.Text.Substring(TB.Text.Length - 1).All(char.IsDigit))) && !(TB.Text[TB.Text.Length - 1].ToString() == "."))))
            {
                TB.TextChanged -= new System.EventHandler(textBox_TextChanged1);
                TB.Text = TB.Text.Substring(0, TB.Text.Length - 1);
                TB.SelectionStart = TB.Text.Length;
                TB.SelectionLength = 0;
                TB.TextChanged += new System.EventHandler(textBox_TextChanged1);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Grey_Out();
            Form_Message_Box FMB = new Form_Message_Box(parent, "Create a phantom item. This item allows you to add items not entered with receipt. Any uncontained phantom items will be deleted", true, 25, this.Location, this.Size);
            FMB.ShowDialog();
            Grey_In();
        }

        #region Printing area

        int index = 0;
        string Current_Container_ID = "";

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int column1 = 35;           // Item name
            int column2 = column1 + 220; // Category
            int column3 = column2 + 180; // Quantity
            int column4 = column3 + 100; // Refunded status
            int column5 = column4 + 166; // Price
            int starty = 10;
            int dataheight = 15;
            int height = starty + starty;

            StringFormat format1 = new StringFormat();
            format1.Alignment = StringAlignment.Center;

            Pen p = new Pen(Brushes.Black, 2.5f);
            Font f2 = new Font("MS Reference Sans Serif", 9f);
            Font f4 = new Font("MS Reference Sans Serif", 10f, FontStyle.Bold);
            Font f5 = new Font("MS Reference Sans Serif", 9f, FontStyle.Italic);
            Font f3 = new Font("MS Reference Sans Serif", 12f, FontStyle.Bold);
            Font f1 = new Font("MS Reference Sans Serif", 14.0f, FontStyle.Bold);

            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(122, 122, 122));
            SolidBrush LightGreyBrush = new SolidBrush(Color.FromArgb(200, 200, 200));
            Pen Grey_Pen = new Pen(GreyBrush, 1);
            Pen Light_Grey_Pen = new Pen(LightGreyBrush, 1);

            if (index == 0)
            {
                e.Graphics.DrawString("SUMMARY REPORT FOR " + profile_list.Text.ToUpper(), f1, Brushes.Black, new Rectangle(10, height, 650, dataheight * 2));
                height += dataheight;
                height += dataheight;
                height += dataheight;
            }

            while (index < Print_Hobby_Item_List.Count)
            {
                Hobby_Item ref_item = Print_Hobby_Item_List[index];
                if (height > e.MarginBounds.Height + 90)// + 20)
                {
                    height = starty;
                    e.HasMorePages = true;
                    return;
                }

                if (ref_item.Container_ID.ToString() != Current_Container_ID)
                {
                    column1 -= 20;
                    height += 4;
                    height += 3;
                    e.Graphics.DrawLine(Grey_Pen, column1, height, 840, height);
                    height += 3;
                    Current_Container_ID = ref_item.Container_ID.ToString();
                    Container ref_Container = parent.Master_Container_Dict[ref_item.Profile_Number].FirstOrDefault(x => x.ID == ref_item.Container_ID.ToString());

                    e.Graphics.DrawString(ref_Container.Name, f4, Brushes.Black, new Rectangle(column1, height, 650, dataheight));
                    //e.Graphics.DrawString("Date: " + ref_Order.Date.ToShortDateString(), f4, Brushes.Black, new Rectangle(column3 - 47, height, 650, dataheight));
                    //e.Graphics.DrawString("Quantity: " + ref_Order.Order_Quantity.ToString(), f4, Brushes.Black, new Rectangle(650, height, 650, dataheight));
                    e.Graphics.DrawString("Total: $" + String.Format("{0:0.00}", ref_Container.Total), f4, Brushes.Black, new Rectangle(675, height, 650, dataheight));
                    height += dataheight;
                    column1 += 20;
                    height += 3;
                }
                e.Graphics.DrawLine(Light_Grey_Pen, column1, height + dataheight + 2, 800, height + dataheight + 2);
                e.Graphics.DrawString(ref_item.Name, f2, Brushes.Black, new Rectangle(column1, height, 650, dataheight));//, format1);
                e.Graphics.DrawString(ref_item.Category == "Outdoor" ? "" : ref_item.Category, f2, Brushes.Black, new Rectangle(column3, height, 650, dataheight));//, format1);
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", ref_item.Price), f2, Brushes.Black, new Rectangle(column5 + 30, height, 625, dataheight));//, format1);

                height += dataheight;
                index++;
            }
            height += dataheight;
            height += dataheight;
            e.Graphics.DrawString("Grand Total: $" + String.Format("{0:0.00}", Hobby_Total), f3, Brushes.Black, new Rectangle(600, height, 625, dataheight + 10));
        }

        private List<Container> Print_Containers = new List<Container>();
        private List<Hobby_Item> Print_Hobby_Item_List = new List<Hobby_Item>();
        double Hobby_Total = 0;

        private void button2_Click(object sender, EventArgs e)
        {
            // Reset print credentials
            Reset_Panels();
            Hobby_Total = 0;
            index = 0;
            Current_Container_ID = "";

            Print_Containers = new List<Container>();
            Print_Hobby_Item_List = new List<Hobby_Item>();


            string Profile_Index = Get_Current_Profile_Index();
            foreach (Container C in parent.Master_Container_Dict[Profile_Index])
            {
                Print_Containers.Add(C);

                // populate print hobby item list (lay them down)
                foreach (Hobby_Item HI in C.Hobby_Item_List)
                {
                    Print_Hobby_Item_List.Add(HI);
                }

                Hobby_Total += C.Total;
            }

            if (Print_Hobby_Item_List.Count > 0)
            {
                Grey_Out();
                using (var form = new Yes_No_Dialog(parent, "Are you sure you wish to print current purchase list?", "Warning", "Preview", "Print", 0, this.Location, this.Size))
                {
                    var result = form.ShowDialog();
                    if (result == DialogResult.OK)
                    {

                        if (form.ReturnValue1 == "1")
                        {
                            printDocument1.Print();
                        }
                        else
                        {
                            printPreviewDialog1.TopMost = true;
                            printPreviewDialog1.ShowDialog();
                        }
                    }
                }
                Grey_In();
            }
            else
            {
                // Get monthly income and compare
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Error: There is nothing to print", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
        }

        private void Grey_Out()
        {
            TFLP2.Size = new Size(this.Width - 2, this.Height - 2);
            TFLP2.Location = new Point(1, 1);
            SuspendLayout();
        }

        private void Grey_In()
        {
            TFLP2.Location = new Point(3000, 1000);
            ResumeLayout();
        }

        private void printPreviewDialog1_Load(object sender, EventArgs e) { }
        #endregion

        bool Highlight_Phantom = false;

        private void import_phantom_profile_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Import_Phantom(parent, this, profile_list.Text, this.Location, this.Size))
            {
                var result2 = form.ShowDialog();
                if (result2 == DialogResult.OK)
                {
                    int Profile_Index = profile_list.Items.IndexOf(form.ReturnValue1) + 1;

                    foreach (Hobby_Item HI in parent.Master_Hobby_Item_List.Where(x => x.Profile_Number == Profile_Index.ToString() && x.OrderID == "999999999").ToList())
                    {
                        string button_name = Get_Next_Random();

                        Hobby_Item Temp_HI = new Hobby_Item()
                        {
                            Name = HI.Name,
                            OrderID = "999999999",
                            Category = HI.Category,
                            Price = HI.Price,
                            Container_ID = 0,
                            Unique_ID = button_name,
                            Profile_Number = HI.Profile_Number
                        };

                        Free_Agent_Hobby_List.Add(Temp_HI);

                        double Tax_Rate = parent.Tax_Rules_Dictionary.ContainsKey(Temp_HI.Category) ? Convert.ToDouble(parent.Tax_Rules_Dictionary[Temp_HI.Category]) : parent.Tax_Rate;
                        Size s = new Size(freeAgentFlowPanel.Width - 2, 26);
                        DragButton DB = new DragButton();
                        DB.AllowDrag = true;
                        DB.Size = s;
                        DB.Text = Temp_HI.Name + " ($" + String.Format("{0:0.00}", (Temp_HI.Price)) + ")";
                        DB.Name = button_name.ToString();
                        DB.FlatStyle = FlatStyle.Flat;
                        DB.Margin = new Padding(0);
                        DB.Padding = new Padding(0);
                        DB.ForeColor = Color.Yellow;
                        DB.Click += new EventHandler(DBButtonClick);

                        Free_Agent_Total += Temp_HI.Price;

                        freeAgentFlowPanel.Controls.Add(DB);
                        Button_List.Add(DB);
                    }

                    label4.Text = "Sum of above items: $" + String.Format("{0:0.00}", Free_Agent_Total);
                }
            }
            Grey_In();
        }

        private void highlight_Click(object sender, EventArgs e)
        {
            if (Highlight_Phantom)
            {
                highlight.Image = global::Financial_Journal.Properties.Resources.highlighter;
                Highlight_Phantom = false;
            }
            else
            {
                highlight.Image = global::Financial_Journal.Properties.Resources.highlighter_Yel;
                Highlight_Phantom = true;
                // refresh
            }
            Reset_Panels();
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }
    }

    public class Container
    {
        public List<Hobby_Item> Hobby_Item_List = new List<Hobby_Item>();
        public string ID { get; set; }
        public string Name { get; set; }
        public double Total { get; set; }

        public void Add_Hobby(Hobby_Item HI)
        {
            Hobby_Item_List.Add(HI);
            Total += HI.Price;
        }

        public void Remove_Hobby(Hobby_Item HI)
        {
            Hobby_Item_List.Remove(HI);
            Total -= HI.Price;
        }

    }

    class MyFlowLayoutPanel : FlowLayoutPanel
    {
        public MyFlowLayoutPanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.AllPaintingInWmPaint, true);
            //this.DoubleBuffered = true;
        }
        protected override void OnScroll(ScrollEventArgs se)
        {
            //this.Invalidate();
            //base.OnScroll(se);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }
    }

    public delegate void MouseUpOnForm();

    public class GlobalMouseHandler : IMessageFilter
    {
        private const int WM_LBUTTONUP = 0x0202;

        public event MouseUpOnForm MouseUpForm;

        #region IMessageFilter Members

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONUP)
            {
                if (MouseUpForm != null)
                {
                    MouseUpForm();
                }
            }
            // Always allow message to continue to the next filter control
            return false;
        }

        #endregion
    }

    public class Hobby_Item
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string OrderID { get; set; }
        public double Price { get; set; }
        public int Container_ID { get; set; }
        public string Unique_ID { get; set; }
        public string Profile_Number { get; set; }


        public Hobby_Item Copy_Item()
        {
            return System.MemberwiseClone.Copy(this);
        }

    }

    public partial class DragButton : Button //DataGridView
    {
        public bool _isDragging = false;
        private int _DDradius = 40;
        public bool AllowDrag { get; set; }
        private int _mX = 0;
        private int _mY = 0;


        protected override void OnGotFocus(EventArgs e)
        {
            this.BackColor = Color.FromArgb(66, 66, 66);
            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            //this.BackColor = Color.Transparent;
            this.BackColor = Color.FromArgb(86, 86, 86);
            base.OnLostFocus(e);
        }

        protected override void OnClick(EventArgs e)
        {
            this.Focus();
            base.OnClick(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.Focus();
            base.OnMouseDown(e);
            _mX = e.X;
            _mY = e.Y;
            this._isDragging = false;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            //this.BackColor = Color.FromArgb(86, 86, 86);
            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!_isDragging)
            {

                // This is a check to see if the mouse is moving while pressed.
                // Without this, the DragDrop is fired directly when the control is clicked, now you have to drag a few pixels first.
                if (e.Button == MouseButtons.Left && _DDradius > 0 && this.AllowDrag)
                {
                    int num1 = _mX - e.X;
                    int num2 = _mY - e.Y;
                    if (((num1 * num1) + (num2 * num2)) > _DDradius)
                    {
                        DoDragDrop(this, DragDropEffects.All);
                        _isDragging = true;
                        return;
                    }
                }
                base.OnMouseMove(e);
                Parent.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isDragging = false;
            base.OnMouseUp(e);
        }
    }
}
