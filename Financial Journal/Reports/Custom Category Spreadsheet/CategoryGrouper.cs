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
    public partial class CategoryGrouper : Form
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

        Receipt parent;

        List<string> tempProfiles = new List<string>();
        public string selectedCategory = "";

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public CategoryGrouper(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            majorCategoryFlowPanel.HorizontalScroll.Maximum = 0;
            majorCategoryFlowPanel.AutoScroll = false;
            majorCategoryFlowPanel.VerticalScroll.Visible = false;
            majorCategoryFlowPanel.AutoScroll = true;

            // Double buffering layout panels
            SetDoubleBuffered(majorCategoryFlowPanel);
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            #region Fade Box
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
            #endregion

            Set_ListBox_Handlers();
            PopulateProfiles();
            PopulateMajorCategories();
        }

        private void PopulateProfiles(string selectItemName="")
        {
            profileBox.Items.Clear();

            List<string> unorderedProfiles = new List<string>();

            // Add existing
            foreach (string profileName in parent.GroupedCategoryList.Select(x => x._ProfileName).Distinct())
            {
                unorderedProfiles.Add(profileName);
            }

            // Add temporary
            foreach (string profileName in tempProfiles)
            {
                unorderedProfiles.Add(profileName);
            }

            // Sort by name
            foreach (string profileName in unorderedProfiles.OrderBy(y => y))
            {
                profileBox.Items.Add(profileName);
            }

            // Select first index
            if (profileBox.Items.Count > 0)
            {
                if (selectItemName.Length == 0)
                {
                    profileBox.SelectedIndex = 0;
                }
                else
                {
                    profileBox.SelectedIndex = profileBox.Items.IndexOf(selectItemName);
                }
            }
        }

        private List<Button> buttonList = new List<Button>();

        private void PopulateMajorCategories(string selectCategory = "")
        {
            // Clear list boxes
            groupedBox.Items.Clear();
            availableBox.Items.Clear();
            groupedBoxExpenses.Items.Clear();
            availableBoxExpenses.Items.Clear();

            majorCategoryFlowPanel.Controls.Clear();
            SuspendDrawing(majorCategoryFlowPanel);

            buttonList.ForEach(button => button.Visible = false);
            buttonList.ForEach(button => majorCategoryFlowPanel.Controls.Remove(button));
            buttonList.ForEach(button => button.Dispose());
            buttonList = new List<Button>();

            foreach (string categoryName in getGroupedCategories(profileBox.Text).Select(y => y._GroupName).OrderBy(z => z))
            {
                Size s = new Size(majorCategoryFlowPanel.Width - 2, 26);
                Button DB = new Button();
                DB.Size = s;
                DB.Text = categoryName;
                DB.Name = categoryName;
                DB.FlatStyle = FlatStyle.Flat;
                DB.Margin = new Padding(0);
                DB.Padding = new Padding(0);
                DB.ForeColor = Color.White;
                DB.TextAlign = ContentAlignment.MiddleLeft;
                DB.Click += new EventHandler(DBButtonClick);

                buttonList.Add(DB);
            }

            if (selectCategory.Length == 0 && buttonList.Count > 0)
            {
                buttonList[0].ForeColor = Color.Yellow; // select first item
                buttonList[0].PerformClick();
            }
            else if (selectCategory.Length > 0 && buttonList.Count > 0)
            {
                int buttonIndex = buttonList.IndexOf(buttonList.First(x => x.Name == selectCategory));
                buttonList[buttonIndex].ForeColor = Color.Yellow; // select first item
                buttonList[buttonIndex].PerformClick();
            }

            majorCategoryFlowPanel.Controls.AddRange(buttonList.ToArray());
            ResumeDrawing(majorCategoryFlowPanel);
        }

        List<ListBox> ListBox_List = new List<ListBox>();

        private void Set_ListBox_Handlers()
        {
            ListBox_List = new List<ListBox>();

            ListBox_List.Add(availableBox);
            ListBox_List.Add(groupedBox);
            ListBox_List.Add(availableBoxExpenses);
            ListBox_List.Add(groupedBoxExpenses);

            ListBox_List.ForEach(x =>
                {
                    x.DrawMode = DrawMode.OwnerDrawFixed;
                    x.DrawItem += listBox1_DrawItem;
                    x.DoubleClick += listBox1_DoubleClick;
                    x.SelectedIndexChanged += new System.EventHandler(listBox1_SelectedIndexChanged);
                }
            );
        }

        private void DBButtonClick(object sender, EventArgs e)
        {
            // Deselect all buttons
            buttonList.ForEach(x => x.ForeColor = Color.White);

            Button DB = (Button)sender;
            DB.ForeColor = Color.Yellow;
            selectedCategory = DB.Name;

            PopulateListBoxes();

        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            ListBox box = (ListBox)sender;
            int index = box.SelectedIndex;

            if (index >= 0)
            {
                if (box.Name == "availableBox")
                {
                    AddCategoryToMajor(availableBoxList[index]);
                }
                else if (box.Name == "groupedBox")
                {
                    RemoveCategoryFromMajor(groupedBoxList[index]);
                }
                else if (box.Name == "availableBoxExpenses")
                {
                    AddExpenseToMajor(availableBoxListExpenses[index]);
                }
                else if (box.Name == "groupedBoxExpenses")
                {
                    RemoveExpenseFromMajor(groupedBoxListExpenses[index]);
                }
                PopulateListBoxes();
            }
        }

        /// <summary>
        /// Add a category to groupedBox
        /// </summary>
        /// <param name="category"></param>
        private void AddCategoryToMajor(string category)
        {
            parent.GroupedCategoryList.First(x => x._ProfileName == profileBox.Text && x._GroupName == selectedCategory).SubCategoryList.Add(category);
        }

        /// <summary>
        /// Remove a category from groupedBox
        /// </summary>
        /// <param name="category"></param>
        private void RemoveCategoryFromMajor(string category)
        {
            parent.GroupedCategoryList.First(x => x._ProfileName == profileBox.Text && x._GroupName == selectedCategory).SubCategoryList.Remove(category);
        }

        /// <summary>
        /// Add a category to groupedBox
        /// </summary>
        /// <param name="category"></param>
        private void AddExpenseToMajor(string category)
        {
            parent.GroupedCategoryList.First(x => x._ProfileName == profileBox.Text && x._GroupName == selectedCategory).SubExpenseList.Add(category);
        }

        /// <summary>
        /// Remove a category from groupedBox
        /// </summary>
        /// <param name="category"></param>
        private void RemoveExpenseFromMajor(string category)
        {
            parent.GroupedCategoryList.First(x => x._ProfileName == profileBox.Text && x._GroupName == selectedCategory).SubExpenseList.Remove(category);
        }

        private void PopulateListBoxes()
        {
            PopulateCurrentGrouped(selectedCategory);
            PopulateAvailableCategories(selectedCategory);
        }

        private List<string> availableBoxList = new List<string>();
        private List<string> groupedBoxList = new List<string>();
        private List<string> availableBoxListExpenses = new List<string>();
        private List<string> groupedBoxListExpenses = new List<string>();

        private void PopulateCurrentGrouped(string groupName)
        {
            #region Categories
            groupedBox.Items.Clear();
            groupedBoxList = new List<string>();

            foreach (string categoryName in getGroupedCategories(profileBox.Text)
                .First(x => x._GroupName == groupName)
                .SubCategoryList.Where(y => y.Length > 0).OrderBy(z => z))
            {
                groupedBox.Items.Add(categoryName);
                groupedBoxList.Add(categoryName);
            }
            #endregion

            #region Expenses
            groupedBoxExpenses.Items.Clear();
            groupedBoxListExpenses = new List<string>();

            foreach (string categoryName in getGroupedCategories(profileBox.Text)
                .First(x => x._GroupName == groupName)
                .SubExpenseList.Where(y => y.Length > 0).OrderBy(z => z))
            {
                groupedBoxExpenses.Items.Add(categoryName);
                groupedBoxListExpenses.Add(categoryName);
            }
            #endregion
        }

        private void PopulateAvailableCategories(string groupName)
        {
            #region Categories
            availableBox.Items.Clear();
            availableBoxList = new List<string>();

            foreach (string category in parent.category_box.Items)
            {
                bool hasCategory = false;
                // Check if any of the groupedCategories in current profile already implemented this category
                foreach (GroupedCategory GC in getGroupedCategories(profileBox.Text))
                {
                    if (GetGroupedCategories(profileBox.Text, GC._GroupName).Any(x => x == category))
                    {
                        hasCategory = true;
                        break;
                    }
                }

                if (!hasCategory)
                {
                    availableBox.Items.Add(category);
                    availableBoxList.Add(category);
                }
            }
            #endregion

            #region Expenses
            availableBoxExpenses.Items.Clear();
            availableBoxListExpenses = new List<string>();

            foreach (string category in parent.Expenses_List.Select(x => x.Expense_Name))
            {
                bool hasCategory = false;
                // Check if any of the groupedCategories in current profile already implemented this category
                foreach (GroupedCategory GC in getGroupedCategories(profileBox.Text))
                {
                    if (GetGroupedExpenses(profileBox.Text, GC._GroupName).Any(x => x == category))
                    {
                        hasCategory = true;
                        break;
                    }
                }

                if (!hasCategory)
                {
                    availableBoxExpenses.Items.Add(category);
                    availableBoxListExpenses.Add(category);
                }
            }
            #endregion
        }

        public List<string> GetGroupedCategories(string profileName, string majorCategoryName)
        {
            return parent.GroupedCategoryList
                .First(x => x._ProfileName == profileName && x._GroupName == majorCategoryName).SubCategoryList;
        }

        public List<string> GetGroupedExpenses(string profileName, string majorCategoryName)
        {
            return parent.GroupedCategoryList
                .First(x => x._ProfileName == profileName && x._GroupName == majorCategoryName).SubExpenseList;
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
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        private void addProfileButton_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Input_Box_Small(parent, "Enter Profile name", "", "OK", null, this.Location, this.Size,
                0, true))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string profileName = form.Pass_String;
                    if (!profileBox.Items.Contains(profileName))
                    {
                        tempProfiles.Add(profileName);
                        PopulateProfiles(profileName);
                    }
                    else
                    {
                        Form_Message_Box FMB =
                            new Form_Message_Box(parent, "Profile with the same name already exists!", true, 0,
                                this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                }
            }
            Grey_In();
        }

        /// <summary>
        /// Get all the grouped categories with the profileName
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        private List<GroupedCategory> getGroupedCategories(string profileName)
        {
            return parent.GroupedCategoryList.Where(x => x._ProfileName == profileName).ToList();
        }

        private void button3_Click(object sender, EventArgs e)
        {

            Grey_Out();
            using (var form = new Input_Box_Small(parent, "Enter major category name", "", "OK", null, this.Location, this.Size,
                0, true))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string groupName = form.Pass_String;
                    if (!getGroupedCategories(profileBox.Text).Any(x => x._GroupName == groupName))
                    {
                        parent.GroupedCategoryList.Add(new GroupedCategory(profileBox.Text, groupName));
                        PopulateMajorCategories(groupName);
                    }
                    else
                    {
                        Form_Message_Box FMB =
                            new Form_Message_Box(parent, "Group with the same name already exists!", true, 0,
                                this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                }
            }
            Grey_In();
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
            e.Graphics.DrawString(RefBox.Items[e.Index].ToString(), e.Font, Brushes.White, e.Bounds, StringFormat.GenericDefault);
            // If the ListBox has focus, draw a focus rectangle around the selected item.
            e.DrawFocusRectangle();

            f_strike.Dispose();
        }
        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Yes_No_Dialog(parent,
                "Are you sure you wish to delete '" + selectedCategory + "'", "Warning",
                "No", "Yes", 0, this.Location, this.Size))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (form.ReturnValue1 == "1")
                    {
                        for (int i = parent.GroupedCategoryList.Count - 1; i >= 0; i--)
                        {
                            if (parent.GroupedCategoryList[i]._ProfileName == profileBox.Text &&
                                parent.GroupedCategoryList[i]._GroupName == selectedCategory)
                            {
                                parent.GroupedCategoryList.RemoveAt(i);
                            }
                        }
                        PopulateMajorCategories();
                    }
                }
            }
            Grey_In();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int index = groupedBox.SelectedIndex;

            if (index >= 0)
            {
                RemoveCategoryFromMajor(groupedBoxList[index]);
                PopulateListBoxes();
                try
                {
                    groupedBox.SelectedIndex = index;
                }
                catch
                {
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

            int index = availableBox.SelectedIndex;

            if (index >= 0)
            {
                AddCategoryToMajor(availableBoxList[index]);
                PopulateListBoxes();

                try
                {
                    availableBox.SelectedIndex = index;
                }
                catch
                {
                }
            }
        }

        private void profileBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateMajorCategories();
        }

        public static void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }

        private void deleteProfile_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Yes_No_Dialog(parent,
                "Are you sure you wish to delete the current profile?", "Warning",
                "No", "Yes", 0, this.Location, this.Size))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    if (form.ReturnValue1 == "1")
                    {
                        parent.GroupedCategoryList = parent.GroupedCategoryList.Where(x => x._ProfileName != profileBox.Text)
                            .ToList();
                        PopulateProfiles();
                        PopulateMajorCategories();
                    }
                }
            }
            Grey_In();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Input_Box_Small(parent, "Enter new Profile name", profileBox.Text, "OK", null, this.Location, this.Size,
                0, true))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK && form.Pass_String != profileBox.Text)
                {
                    string origProfileName = profileBox.Text;
                    string profileName = form.Pass_String;
                    if (!profileBox.Items.Contains(profileName))
                    {
                        foreach (GroupedCategory GC in parent.GroupedCategoryList)
                        {
                            if (GC._ProfileName == origProfileName)
                                GC._ProfileName = profileName;
                        }
                        PopulateProfiles(profileName);
                    }
                    else
                    {
                        Form_Message_Box FMB =
                            new Form_Message_Box(parent, "Profile with the same name already exists!", true, 0,
                                this.Location, this.Size);
                        FMB.ShowDialog();
                    }
                }
            }
            Grey_In();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int index = groupedBoxExpenses.SelectedIndex;

            if (index >= 0)
            {
                RemoveExpenseFromMajor(groupedBoxListExpenses[index]);
                PopulateListBoxes();
                try
                {
                    groupedBoxExpenses.SelectedIndex = index;
                }
                catch
                {
                }
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            int index = availableBoxExpenses.SelectedIndex;

            if (index >= 0)
            {
                AddExpenseToMajor(availableBoxListExpenses[index]);
                PopulateListBoxes();

                try
                {
                    availableBoxExpenses.SelectedIndex = index;
                }
                catch
                {
                }
            }
        }


    }
}
