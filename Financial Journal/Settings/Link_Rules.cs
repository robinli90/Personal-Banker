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
    public partial class Link_Rules : Form
    {

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

            TFLP.Size = new Size(this.Size.Width - 2, this.Size.Height - 2);
            //e.Graphics.FillRectangle(Brushes.DarkBlue, rc);
        }

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        public Link_Rules(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            //this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2) - 12);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            Set_Form_Color(parent.Frame_Color);
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            dataGridView1.CellClick += new DataGridViewCellEventHandler(dataGridView1_CellContentClick);
            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);
            dataGridView1.GridColor = Color.White;
            dataGridView1.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(98, 110, 110);

            // Disable sorting
            foreach (DataGridViewColumn column in dataGridView1.Columns) { column.SortMode = DataGridViewColumnSortMode.NotSortable; }

            // Add buttons
            DataGridViewButtonColumn buttons = new DataGridViewButtonColumn();
            {
                buttons.HeaderText = "";
                buttons.Text = "X";
                buttons.UseColumnTextForButtonValue = true;
                buttons.AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.None;
                buttons.Width = 23;
                buttons.FlatStyle = FlatStyle.Flat;
                buttons.CellTemplate.Style.BackColor = Color.FromArgb(64, 64, 64);
                buttons.CellTemplate.Style.ForeColor = Color.White;
                buttons.DisplayIndex = 2;
            }

            //dataGridView1.Columns.Add(buttons);
            dataGridView1.Columns.Insert(2, buttons);

            // Load same category
            foreach (string g in parent.category_box.Items)
            {
                    category_box.Items.Add(g);
            }
            if (category_box.Items.Count > 0) category_box.Text = category_box.Items[0].ToString();

            Populate_Link_Rules();

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

        private void Populate_Location_Box()
        {
            location_box.Items.Clear();

            // Load same location
            foreach (string g in parent.location_box.Items)
            {
                if (!parent.Link_Location.ContainsKey(g))
                    location_box.Items.Add(g);
            }
            if (location_box.Items.Count > 0) location_box.Text = location_box.Items[0].ToString();

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

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0 && e.ColumnIndex == 2)
            {
                parent.Link_Location.Remove(dataGridView1[0, e.RowIndex].Value.ToString());
                Populate_Link_Rules();
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            this.dataGridView1.ClearSelection();
        }

        private void Populate_Link_Rules()
        {
            Populate_Location_Box(); 

            this.Size = Start_Size;
            dataGridView1.Rows.Clear();
            
            foreach (KeyValuePair<string, string> Key in parent.Link_Location)
            {
                dataGridView1.Rows.Add(Key.Key, Key.Value, "");

                // Datagrid Button Style Setting Dynamically
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Font = new Font(dataGridView1.Font.FontFamily, dataGridView1.Font.Size, FontStyle.Bold);
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.ApplyStyle(style);
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].ToolTipText = "Delete Entry";
            }
            //this.Height += dataGridView1.Rows.Count * 22;
        }

        // Return the token count within string given token
        private int Get_Char_Count(string comparison_text, char reference_char)
        {
            int count = 0;
            foreach (char c in comparison_text)
            {
                if (c == reference_char)
                {
                    count++;
                }
            }
            return count;
        }

        private void Add_button_Click(object sender, EventArgs e)
        {
            try
            {
                if (!parent.Link_Location.ContainsKey(location_box.Text))
                {
                    parent.Link_Location.Add(location_box.Text, category_box.Text);
                    Populate_Link_Rules();
                }
                else
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Location already has existing link rule", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }
            }
            catch
            {
            }
            
            /*
            try
            {
                if (Convert.ToDouble(tax_rate.Text.Substring(1)) < 100)
                {
                    if (!parent.Tax_Rules_Dictionary.ContainsKey(category_box.Text))
                    {
                        parent.Tax_Rules_Dictionary.Add(category_box.Text, (Convert.ToDouble(tax_rate.Text.Substring(1)) / 100).ToString());
                        Populate_Link_Rules();
                    }
                    else
                    {
                        Form_Message_Box FMB = new Form_Message_Box(parent, "Category already has existing tax rule");
                        FMB.ShowDialog();
                    }
                }
                else
                {
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Invalid Rate Value");
                    FMB.ShowDialog();
                }
            }
            catch
            {
            }*/
        }
    }
}
