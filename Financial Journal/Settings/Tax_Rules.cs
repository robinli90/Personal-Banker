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
    public partial class Tax_Rules : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;
        Size Start_Size = new Size();

        public Tax_Rules(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            Set_Form_Color(parent.Frame_Color);

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
                if (!parent.Tax_Rules_Dictionary.ContainsKey(g))
                    category_box.Items.Add(g);
            }
            if (category_box.Items.Count > 0) category_box.Text = category_box.Items[0].ToString();

            tax_box.Text = parent.Tax_Rate.ToString();
            Populate_Tax_Rules();


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
                parent.Tax_Rules_Dictionary.Remove(dataGridView1[0, e.RowIndex].Value.ToString());
                Populate_Tax_Rules();
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            this.dataGridView1.ClearSelection();
        }

        private void Populate_Tax_Rules()
        {
            this.Size = Start_Size;
            dataGridView1.Rows.Clear();
            
            foreach (KeyValuePair<string, string> Key in parent.Tax_Rules_Dictionary)
            {
                dataGridView1.Rows.Add(Key.Key, Key.Value, "");

                // Datagrid Button Style Setting Dynamically
                DataGridViewCellStyle style = new DataGridViewCellStyle();
                style.Font = new Font(dataGridView1.Font.FontFamily, dataGridView1.Font.Size, FontStyle.Bold);
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].Style.ApplyStyle(style);
                dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[2].ToolTipText = "Delete Entry";
            }
            this.Height += dataGridView1.Rows.Count * 22;
        }

        private void tax_rate_TextChanged(object sender, EventArgs e)
        {
            if (!(tax_rate.Text.StartsWith("%")))
            {
                if (Get_Char_Count(tax_rate.Text, Convert.ToChar("%")) == 1)
                {
                    string temp = tax_rate.Text;
                    tax_rate.Text = temp.Substring(1) + temp[0];
                    tax_rate.SelectionStart = tax_rate.Text.Length;
                    tax_rate.SelectionLength = 0;
                }
                else
                {
                    tax_rate.Text = "%" + tax_rate.Text;
                }
            }
            else if ((tax_rate.Text.Length > 1) && ((Get_Char_Count(tax_rate.Text, Convert.ToChar(".")) > 1) || (tax_rate.Text[1].ToString() == ".") || (Get_Char_Count(tax_rate.Text, Convert.ToChar("%")) > 1) || (!((tax_rate.Text.Substring(tax_rate.Text.Length - 1).All(char.IsDigit))) && !(tax_rate.Text[tax_rate.Text.Length - 1].ToString() == "."))))
            {
                tax_rate.TextChanged -= new System.EventHandler(tax_rate_TextChanged);
                tax_rate.Text = tax_rate.Text.Substring(0, tax_rate.Text.Length - 1);
                tax_rate.SelectionStart = tax_rate.Text.Length;
                tax_rate.SelectionLength = 0;
                tax_rate.TextChanged += new System.EventHandler(tax_rate_TextChanged);
            }
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
                if (tax_rate.Text.Length > 1 && Convert.ToDouble(tax_rate.Text.Substring(1)) < 100)
                {
                    if (!parent.Tax_Rules_Dictionary.ContainsKey(category_box.Text))
                    {
                        parent.Tax_Rules_Dictionary.Add(category_box.Text, (Convert.ToDouble(tax_rate.Text.Substring(1)) / 100).ToString());
                        Populate_Tax_Rules();
                    }
                    else
                    {
                        Grey_Out();
                        Form_Message_Box FMB = new Form_Message_Box(parent, "Category already has existing tax rule", true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                        Grey_In();
                    }
                }
                else
                {
                    Grey_Out();
                    Form_Message_Box FMB = new Form_Message_Box(parent, "Invalid Rate Value", true, 0, this.Location, this.Size);
                    FMB.ShowDialog();
                    Grey_In();
                }
            }
            catch
            {
            }
        }

        private void tax_box_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToDouble(tax_box.Text) <= 1 && Convert.ToDouble(tax_box.Text) >= 0)
                {
                    parent.Tax_Rate = Convert.ToDouble(tax_box.Text);
                    parent.Settings_Dictionary["GENERAL_TAX_RATE"] = tax_box.Text;
                }
            }
            catch
            {
            }
        }

        private void memo_button_Click(object sender, EventArgs e)
        {
            tax_box.Enabled = true;
            memo_button.Enabled = false;
        }


    }
}
