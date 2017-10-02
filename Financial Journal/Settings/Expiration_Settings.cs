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
    public partial class Expiration_Settings : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;
        Size Start_Size = new Size();


        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Expiration_Settings(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

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

            dataGridView1.KeyUp += dataGridView1_KeyUp;

            dataGridView1.CellMouseEnter += new DataGridViewCellEventHandler(dataGridView1_CellMouseEnter);
            dataGridView1.ShowCellToolTips = false;

            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
            dataGridView1.GridColor = Color.White;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(98, 110, 110);

            location.SelectedIndexChanged -= location_SelectedIndexChanged;

            #region Add view buttons
            DataGridViewButtonColumn buttons = new DataGridViewButtonColumn();
            {
                buttons.HeaderText = "";
                buttons.Text = "X";// "🔍";
                buttons.UseColumnTextForButtonValue = true;
                buttons.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;
                buttons.Width = 23;
                buttons.FlatStyle = FlatStyle.Flat;
                buttons.CellTemplate.Style.BackColor = Color.FromArgb(64, 64, 64);
                buttons.CellTemplate.Style.ForeColor = Color.White;
                buttons.DisplayIndex = 4;
            }
            dataGridView1.Columns.Insert(3, buttons);
            #endregion

            location.Items.Add("General");
            foreach (string g in parent.location_box.Items)
            {
                location.Items.Add(g);
            }

            location.SelectedIndex = 0;
            Populate_Locations("General");

            // Apply style
            enable_exp_warnings.Style = JCS.ToggleSwitch.ToggleSwitchStyle.Modern;
            enable_exp_warnings.Size = new Size(68, 25);
            enable_exp_warnings.OnText = "On";
            enable_exp_warnings.OnFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            enable_exp_warnings.OnForeColor = Color.White;
            enable_exp_warnings.OffText = "Off";
            enable_exp_warnings.OffFont = new Font(this.Font.FontFamily, 10, FontStyle.Bold);
            enable_exp_warnings.OffForeColor = Color.White;

            enable_exp_warnings.Checked = parent.Settings_Dictionary["ENABLE_EXPIRATION_WARNINGS"] == "1";

            location.SelectedIndexChanged += location_SelectedIndexChanged;
        }

        ToolTip ToolTip1 = new ToolTip();

        /// <summary>
        /// Set tool tip for header
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            ToolTip1.Dispose();
            ToolTip1 = new ToolTip();
            ToolTip1.InitialDelay = 1;
            ToolTip1.ReshowDelay = 1;

            if (e.RowIndex == -1 && e.ColumnIndex == 1)
            {
                
                ToolTip1.SetToolTip(this.dataGridView1, "Days between purchase date and expiration date");
            }
            else if (e.RowIndex == -1 && e.ColumnIndex == 2)
            {
                ToolTip1.SetToolTip(this.dataGridView1, "Number of warning days before expiration date");
            }
            else
            {
                //Diagnostics.WriteLine("ENTER: " + e.RowIndex + ", " + e.ColumnIndex);
                ToolTip1.Hide(this.dataGridView1);
            }
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            // Check if the KeyCode value has the Keys.Enter flag set
            if ((e.KeyCode & Keys.Enter) == Keys.Enter)
            {
                // Add row only if last row is populated completely
                dataGridView1.Rows.Add();

                // Remove last delete button
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    DataGridViewCellStyle style = new DataGridViewCellStyle();
                    style.Padding = new Padding(0, 0, (row == dataGridView1.Rows[dataGridView1.Rows.Count - 1] ? 1000 : 0), 0);
                    style.BackColor = Color.FromArgb(76, 76, 76);
                    style.ForeColor = Color.White;
                    row.Cells[3].Style = style;
                }

                dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];

                // Prevent the key event from being passed on to the control
                e.Handled = true;
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.dataGridView1.ClearSelection();

            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                if (e.ColumnIndex == 3 && e.RowIndex >= 0)
                {
                    Grey_Out();
                    dataGridView1.Rows.RemoveAt(e.RowIndex);
                    Grey_In();
                }
            }
        }

        string Previous_Selected_Location = "General";

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            this.dataGridView1.ClearSelection();
        }

        private void enable_exp_warnings_CheckedChanged(object sender, EventArgs e)
        {
            JCS.ToggleSwitch Switch = (JCS.ToggleSwitch)sender;
            parent.Settings_Dictionary["ENABLE_EXPIRATION_WARNINGS"] = Switch.Checked ? "1" : "0";
            parent.Invalidate();
        }

        private void location_SelectedIndexChanged(object sender, EventArgs e)
        {
            Save_Location_Expiration(Previous_Selected_Location);
            Populate_Locations(location.Text);
            Previous_Selected_Location = location.Text;
        }

        private void Populate_Locations(string Location)
        {
            dataGridView1.Rows.Clear();
            Location = (Location == "General" ? "General_Expiration" : Location);

            foreach (Expiration_Entry EE in parent.Expiration_List.Where(x => x.Location == Location).ToList())
            {
                dataGridView1.Rows.Add(EE.Item_Name, EE.Exp_Date_Count, EE.Warning_Date_Count);
            }

            // Add empty row
            dataGridView1.Rows.Add();

            // Remove last delete button
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Padding = new Padding(0, 0, (row == dataGridView1.Rows[dataGridView1.Rows.Count - 1] ? 1000 : 0), 0);
                style.BackColor = Color.FromArgb(76, 76, 76);
                style.ForeColor = Color.White;
                row.Cells[3].Style = style;
            }
        }

        private void Save_Location_Expiration(string Location)
        {
            Location = (Location == "General" ? "General_Expiration" : Location);
            parent.Expiration_List = parent.Expiration_List.Where(x => x.Location != Location).ToList();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (dataGridView1.Rows.IndexOf(row) < dataGridView1.Rows.Count)
                {
                    try
                    {
                        Expiration_Entry EE = new Expiration_Entry();
                        EE.Item_Name = row.Cells[0].Value.ToString();
                        EE.Last_Warn_Date = DateTime.Now;
                        EE.Exp_Date_Count = Convert.ToInt32(row.Cells[1].Value.ToString());
                        // If warning longer than exp date count, then set as same date
                        EE.Warning_Date_Count = Convert.ToInt32(row.Cells[2].Value.ToString()) > Convert.ToInt32(row.Cells[1].Value.ToString()) ? Convert.ToInt32(row.Cells[1].Value.ToString()) : Convert.ToInt32(row.Cells[2].Value.ToString());
                        EE.Location = Location == "General" ? "General_Expiration" : Location;
                        parent.Expiration_List.Add(EE);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void excel_button_Click(object sender, EventArgs e)
        {
            Save_Location_Expiration(location.Text);
            parent.Background_Save();
        }

        // Loading presets
        private void button1_Click(object sender, EventArgs e)
        {
            Grey_Out();
            using (var form = new Yes_No_Dialog(parent, "Are you sure you want to load defaults? This will reset your 'General' list", "Warning", "No", "Yes", 15, this.Location, this.Size))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK && form.ReturnValue1 == "1")
                {
                    Save_Location_Expiration(location.Text);
                    parent.Expiration_List = parent.Expiration_List.Where(x => x.Location != "General_Expiration").ToList();
                    parent.Expiration_List.AddRange(Get_Default_Entries());
                    Populate_Locations("General");
                }
            }
            Grey_In();
        }

        private List<Expiration_Entry> Get_Default_Entries()
        {
            List<Expiration_Entry> returnList = new List<Expiration_Entry>();

            returnList.Add(new Expiration_Entry() { Item_Name = "Eggs", Exp_Date_Count = 21, Warning_Date_Count = 5, Location = "General_Expiration" }); 
            returnList.Add(new Expiration_Entry() { Item_Name = "Bread", Exp_Date_Count = 6, Warning_Date_Count = 5, Location = "General_Expiration" }); 
            returnList.Add(new Expiration_Entry() { Item_Name = "Bagel", Exp_Date_Count = 3, Warning_Date_Count = 5, Location = "General_Expiration" }); 
            returnList.Add(new Expiration_Entry() { Item_Name = "Milk", Exp_Date_Count = 5, Warning_Date_Count = 5, Location = "General_Expiration" }); 
            returnList.Add(new Expiration_Entry() { Item_Name = "Butter", Exp_Date_Count = 30, Warning_Date_Count = 5, Location = "General_Expiration" }); 
            returnList.Add(new Expiration_Entry() { Item_Name = "Celery", Exp_Date_Count = 7, Warning_Date_Count = 5, Location = "General_Expiration" }); 
            returnList.Add(new Expiration_Entry() { Item_Name = "Asparagus", Exp_Date_Count = 7, Warning_Date_Count = 5, Location = "General_Expiration" }); 
            returnList.Add(new Expiration_Entry() { Item_Name = "Lettuce", Exp_Date_Count = 7, Warning_Date_Count = 5, Location = "General_Expiration" }); 
            returnList.Add(new Expiration_Entry() { Item_Name = "Broccoli", Exp_Date_Count = 7, Warning_Date_Count = 5, Location = "General_Expiration" }); 
            returnList.Add(new Expiration_Entry() { Item_Name = "Corn", Exp_Date_Count = 7, Warning_Date_Count = 5, Location = "General_Expiration" }); 

            return returnList;
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

    }

    public class Expiration_Entry
    {
        public string Item_Name { get; set; }
        public int Exp_Date_Count { get; set; }
        public int Warning_Date_Count { get; set; }
        public string Location { get; set; }
        public DateTime Last_Warn_Date { get; set; }

        public override string ToString()
        {
            return Item_Name + " (" + Location + ")";
        }

        public Expiration_Entry()
        {
            Last_Warn_Date = DateTime.Now;
        }
    }
}
