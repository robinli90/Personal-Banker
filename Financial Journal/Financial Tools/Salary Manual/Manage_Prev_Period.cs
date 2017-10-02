using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Financial_Journal
{
    public partial class Manage_Prev_Period : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            //parent.Background_Save();
            base.OnFormClosing(e);
        }
        /*
         * Resizing form
         * 
        private const int cGrip = 16;      // Grip size
        private const int cCaption = 32;   // Caption bar height;

        protected override void OnPaint(PaintEventArgs e) {
            Rectangle rc = new Rectangle(this.ClientSize.Width - cGrip, this.ClientSize.Height - cGrip, cGrip, cGrip);
            ControlPaint.DrawSizeGrip(e.Graphics, this.BackColor, rc);
            rc = new Rectangle(0, 0, this.ClientSize.Width, cCaption);
            //e.Graphics.FillRectangle(Brushes.DarkBlue, rc);
        }

        protected override void WndProc(ref Message m) 
        {
            if (m.Msg == 0x84) {  // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32() & 0xffff, m.LParam.ToInt32() >> 16);
                pos = this.PointToClient(pos);
                if (pos.Y < cCaption) {
                    m.Result = (IntPtr)2;  // HTCAPTION
                    return;
                }
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip) {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }
            }
            base.WndProc(ref m);
        }
        */

        private List<Button> Icon_Button = new List<Button>();
        bool Loaded = false;
        List<PayPeriod> Paint_List = new List<PayPeriod>();

        protected override void OnPaint(PaintEventArgs e)
        {
            // Remove existing buttons
            Icon_Button.ForEach(button => this.Controls.Remove(button));
            Icon_Button.ForEach(button => button.Image.Dispose());
            Icon_Button.ForEach(button => button.Dispose());
            Icon_Button = new List<Button>();

            int start_height = 30;
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
            Font f_strike = new Font("MS Reference Sans Serif", 9, FontStyle.Strikeout);
            Font f_total = new Font("MS Reference Sans Serif", 8, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);
            Font f_title = new Font("MS Reference Sans Serif", 8, FontStyle.Bold);

            int col = 0;
            int col_spacing = 10;
            int rectangle_width = 165, rectangle_height = 65;

            int data_height = rectangle_height + col_spacing;

            //List<PayPeriod> Test = parent.Income_Company_List.First(x => x.Default).Intervals;
            Paint_List = parent.Income_Company_List.First(x => x.Default).Intervals.Where(x => x.Pay_Date >= Start_Date && x.Pay_Date <= End_Date).ToList();

            if (Loaded && Paint_List.Count > 0)
            {
                foreach (PayPeriod PP in Paint_List)
                {
                    // Reset to next row
                    if (col == 6) { col = 0; row_count++; }

                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    int x = start_margin + col * col_spacing + col * rectangle_width;
                    int y = start_height + height_offset + row_count * data_height;

                    DrawRoundedRectangle(e.Graphics, p, new Rectangle(x, y, rectangle_width, rectangle_height), 3);
                    e.Graphics.DrawString("Pay Period #" + PP.Pay_Period, f_total, WritingBrush, x + 35, y + 10);
                    e.Graphics.DrawString("Date : " + PP.Pay_Date.ToShortDateString(), f, WritingBrush, x + 35, y + 25);
                    e.Graphics.DrawString("Amount : $" + String.Format("{0:0.00}", PP.Amount), f, WritingBrush, x + 35, y + 40);

                    Button edit_button = new Button();
                    edit_button.BackColor = this.BackColor;
                    edit_button.ForeColor = this.BackColor;
                    edit_button.FlatStyle = FlatStyle.Flat;
                    edit_button.Size = new Size(29, 29);
                    edit_button.Image = global::Financial_Journal.Properties.Resources.edit;
                    edit_button.Location = new Point(x + 5, y + 22);
                    edit_button.Name = PP.Pay_Period.ToString();
                    edit_button.Text = "";
                    edit_button.Click += new EventHandler(this.view_order_Click);
                    Icon_Button.Add(edit_button);
                    ToolTip1.SetToolTip(edit_button, "Edit Pay Period #" + PP.Pay_Period);
                    this.Controls.Add(edit_button);

                    col++;

                }
            }

            row_count++;
            col = Paint_List.Count > 6 ? 6 : Paint_List.Count;
            this.Height = start_height + height_offset + row_count * data_height;
            this.Width = start_margin * 2 + col * col_spacing + col * rectangle_width - col_spacing;

            TFLP.Size = new Size(this.Width - 2, this.Height - 2);

            // Dispose all objects
            p.Dispose();
            Grey_Pen.Dispose();
            GreenBrush.Dispose();
            RedBrush.Dispose();
            GreyBrush.Dispose();
            WritingBrush.Dispose();
            f.Dispose();
            f_asterisk.Dispose();
            f_strike.Dispose();
            f_total.Dispose();
            f_header.Dispose();
            f_italic.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);

        }

        public bool Advanced_To_Next = false;

        private void view_order_Click(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            Grey_Out();
            Deposit_Input_Box DIB = new Deposit_Input_Box(parent, this, parent.Income_Company_List.FirstOrDefault(x => x.Default).Intervals[Convert.ToInt32(b.Name) - 1], this.Location, this.Size);
            DIB.ShowDialog();

            int next_factor = 1;
            while (Advanced_To_Next && parent.Income_Company_List.FirstOrDefault(x => x.Default).Intervals.Count > Convert.ToInt32(b.Name) - 1 + next_factor)
            {
                Deposit_Input_Box DIB2 = new Deposit_Input_Box(parent, this, parent.Income_Company_List.FirstOrDefault(x => x.Default).Intervals[Convert.ToInt32(b.Name) - 1 + next_factor], this.Location, this.Size);
                DIB2.ShowDialog();
                next_factor++;
                if (Convert.ToInt32(b.Name) - 1 + next_factor >= Paint_List[Paint_List.Count - 1].Pay_Period)
                {
                    Advanced_To_Next = false;
                }
            }
            Grey_In();
            Invalidate();


        }

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;
        //CustomIncome Ref_CI;
        DateTime Start_Date;
        DateTime End_Date;
        public Manage_Prev_Period(Receipt _parent, DateTime Start_Date_, DateTime End_Date_)
        {
            Start_Date = Start_Date_;
            End_Date = End_Date_;
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            //Ref_CI = parent.Income_Company_List.FirstOrDefault(x => x.Default);
        }

        FadeControl TFLP;

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

            TFLP.Opacity = 65;

            Loaded = true;
            Invalidate();
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

        private void Grey_Out()
        {
            TFLP.Location = new Point(1, 1);
        }

        private void Grey_In()
        {
            TFLP.Location = new Point(1000, 1000);
        }

        public static void DrawRoundedRectangle(Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (pen == null)
                throw new ArgumentNullException("pen");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }

        public static void FillRoundedRectangle(Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (brush == null)
                throw new ArgumentNullException("brush");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.FillPath(brush, path);
            }
        }

        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }
}
