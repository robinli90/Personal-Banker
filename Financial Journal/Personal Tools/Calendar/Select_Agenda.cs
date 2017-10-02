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
    public partial class Select_Agenda : Form
    {
        private List<Button> Icon_Button = new List<Button>();
        private List<Shopping_Item> Toggle_SI = new List<Shopping_Item>();
        private List<Agenda_Item> Toggle_AI = new List<Agenda_Item>();

        private bool Check_If_AI_Exists(Agenda_Item AI)
        {
            return (Toggle_AI.Contains(AI));
        }

        private bool Check_If_SI_Exists(Shopping_Item SI)
        {
            return (Toggle_SI.Contains(SI));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Remove existing buttons
            Icon_Button.ForEach(button => this.Controls.Remove(button));
            Icon_Button.ForEach(button => button.Image.Dispose());
            Icon_Button.ForEach(button => button.Dispose());
            Icon_Button = new List<Button>();

            int data_height = 25;
            int start_height = 55;
            int start_margin = 15;
            int height_offset = 9;
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
            Font f = new Font("MS Reference Sans Serif", 8, FontStyle.Regular);
            Font f_italic = new Font("MS Reference Sans Serif", 8, FontStyle.Italic);
            Font f_strike = new Font("MS Reference Sans Serif", 8, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);
            Font f_title = new Font("MS Reference Sans Serif", 11, FontStyle.Bold);

            int item_index = 0;

            // If has order
            if (Paint)
            {

                // We only want the items that have not been added or incomplete (or check if any children is unchecked
                List<Agenda_Item> Agenda_Paint_List = parent.Agenda_Item_List.Where(x => (!x.Check_State || x.Shopping_List.Any(y => !y.Check_State)) && (x.Calendar_Date.Year < 1801 || x.Shopping_List.Any(y => y.Calendar_Date.Year < 1801))).ToList();

                // Sort descending
                Agenda_Paint_List = Agenda_Paint_List.OrderByDescending(x => x.Date).ToList();


                Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)Agenda_Paint_List.Count() / (decimal)Entries_Per_Page));
                next_page_button.Visible = Pages_Required > 1 && Current_Page < Pages_Required - 1;

                foreach (Agenda_Item AI in Agenda_Paint_List.GetRange(Current_Page * Entries_Per_Page, (Agenda_Paint_List.Count - Entries_Per_Page * Current_Page) >= Entries_Per_Page ? Entries_Per_Page : (Agenda_Paint_List.Count % Entries_Per_Page)))
                {
                    
                    int inner_index = 0;

                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button refund_button = new Button();
                    refund_button.BackColor = this.BackColor;
                    refund_button.ForeColor = this.BackColor;
                    refund_button.FlatStyle = FlatStyle.Flat;
                    refund_button.Image = Check_If_AI_Exists(AI) ? global::Financial_Journal.Properties.Resources.unck : global::Financial_Journal.Properties.Resources.ck;
                    refund_button.Size = new Size(22, 22);
                    refund_button.Location = new Point(start_margin + 10, start_height + height_offset + (row_count * data_height) - 4);
                    refund_button.Name = "AI_" + AI.ID + "_" + (Check_If_AI_Exists(AI) ? "1" : "0");
                    refund_button.Text = "";
                    refund_button.Click += new EventHandler(this.view_order_Click);
                    Icon_Button.Add(refund_button);
                    ToolTip1.SetToolTip(refund_button, !Check_If_AI_Exists(AI) ? "Select this item" : "Deselect this item");

                    if (AI.Calendar_Date.Year < 1801) this.Controls.Add(refund_button);

                    e.Graphics.DrawString(AI.Name, (AI.Calendar_Date.Year < 1801) ? f : f_strike, WritingBrush, start_margin + (AI.Calendar_Date.Year < 1801 ? 32 : 10), start_height + height_offset + (row_count * data_height));

                    row_count++;

                    foreach (Shopping_Item SI in AI.Shopping_List)
                    {

                        refund_button = new Button();
                        refund_button.BackColor = this.BackColor;
                        refund_button.ForeColor = this.BackColor;
                        refund_button.FlatStyle = FlatStyle.Flat;
                        refund_button.Image = Check_If_SI_Exists(SI) ? global::Financial_Journal.Properties.Resources.unck : global::Financial_Journal.Properties.Resources.ck;
                        refund_button.Size = new Size(22, 22);
                        refund_button.Location = new Point(start_margin + 30, start_height + height_offset + (row_count * data_height) - 4);
                        refund_button.Name = "SI_" + AI.ID + "_" + inner_index + "_" + (Check_If_SI_Exists(SI) ? "1" : "0");
                        refund_button.Text = "";
                        refund_button.Click += new EventHandler(this.view_order_Click);
                        Icon_Button.Add(refund_button);
                        ToolTip1.SetToolTip(refund_button, !Check_If_SI_Exists(SI) ? "Select this item" : "Deselect this item");

                        if (SI.Calendar_Date.Year < 1801) this.Controls.Add(refund_button);

                        e.Graphics.DrawString(SI.Name, (SI.Calendar_Date.Year < 1801) ? f : f_strike, WritingBrush, start_margin + (SI.Calendar_Date.Year < 1801 ? 52 : 30), start_height + height_offset + (row_count * data_height));

                        row_count++;
                        inner_index++;
                    }

                }
                
                height_offset += 8;
            }

            row_count++;
            this.Height = start_height + height_offset + row_count * data_height;

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

        Receipt parent;
        Size Start_Size = new Size();

        private void view_order_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            
            if (b.Name.StartsWith("AI_"))
            {
                string[] temp = b.Name.Split(new string[] { "_" }, StringSplitOptions.None);
                // If already in list, remove it
                if (temp[2] == "1")
                {
                    Toggle_AI.Remove(Toggle_AI.First(x => x.ID == Convert.ToInt32(temp[1])));
                }
                else
                {
                    Toggle_AI.Add(parent.Agenda_Item_List.First(x => x.ID == Convert.ToInt32(temp[1])));
                }
            }
            else if (b.Name.StartsWith("SI_"))
            {
                string[] temp = b.Name.Split(new string[] { "_" }, StringSplitOptions.None);
                // If already in list, remove it
                if (temp[3] == "1")
                {
                    Toggle_SI.Remove(Toggle_SI.First(x => x == parent.Agenda_Item_List.First(y => y.ID == Convert.ToInt32(temp[1])).Shopping_List[Convert.ToInt32(temp[2])]));
                }
                else
                {
                    Toggle_SI.Add(parent.Agenda_Item_List.First(x => x.ID == Convert.ToInt32(temp[1])).Shopping_List[Convert.ToInt32(temp[2])]);
                }
            }
            Invalidate();
        }
        /*
        public Form_Template(Receipt _parent);
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
        }*/

        DateTime Ref_Date;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Select_Agenda(Receipt _parent, DateTime Ref_D, Point g = new Point(), Size s = new Size())
        {
            Ref_Date = Ref_D;
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);

            this.Location = new Point(g.X + s.Width / 2 - this.Width / 2, g.Y + 15);
            label1.Text = "Select the agenda item(s) you want to add";
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            
            Paint = true;
            //Pages_Required = Convert.ToInt32(Math.Ceiling((decimal)parent.Agenda_Item_List.Count() / (decimal)Entries_Per_Page));
            //next_page_button.Visible = Pages_Required > 1;
        }

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            // Set calendar dates
            foreach (Agenda_Item AI in parent.Agenda_Item_List.Where(x => Toggle_AI.Contains(x) || x.Shopping_List.Where(y => Toggle_SI.Contains(y)).ToList().Count > 0).ToList())
            {
                if (Toggle_AI.Contains(AI)) AI.Calendar_Date = Ref_Date;

                AI.Shopping_List.Where(x => Toggle_SI.Contains(x)).ToList().ForEach(x => x.Calendar_Date = Ref_Date);
            }

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

        private int Entries_Per_Page = 3;
        int Pages_Required = 0;
        int Current_Page = 0;
        bool Paint = false;

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
}
