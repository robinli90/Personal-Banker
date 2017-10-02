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
    public partial class Start_Stop_Dialog : Form
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            int data_height = 23;
            int start_height = Start_Size.Height - 50;
            int start_margin = 15; // Date
            int height_offset = 9;

            int margin1 = start_margin + 225 + (Draw_Type == "Expenses" ? 0 : 50);   //Date
            int margin2 = margin1 + 100;   //Action
            int margin3 = margin2 + 150;   //Action

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
            Font f_italic = new Font("MS Reference Sans Serif", 9, FontStyle.Italic);
            Font f_total = new Font("MS Reference Sans Serif", 9, FontStyle.Bold);
            Font f_header = new Font("MS Reference Sans Serif", 10, FontStyle.Underline);
            Font f_title = new Font("MS Reference Sans Serif", 11, FontStyle.Bold);

            // If has order
            if (true)
            {

                // Draw gray header line
                //e.Graphics.DrawLine(Grey_Pen, start_margin, start_height - 10, this.Width -15, start_height - 10);


                if (Draw_Type == "Expenses")
                {
                    // Header
                    e.Graphics.DrawString("Action Date", f_header, WritingBrush, start_margin + 35, start_height + (row_count * data_height));
                    e.Graphics.DrawString("Action", f_header, WritingBrush, margin1, start_height + (row_count * data_height));
                    row_count += 1;

                    int item_index = 0;


                    // For each refund item
                    foreach (DateTime date in Ref_Exp.Date_Sequence)
                    {

                        e.Graphics.DrawString(date.ToString(), f, WritingBrush, start_margin + 6, start_height + height_offset + (row_count * data_height));
                        e.Graphics.DrawString(item_index % 2 == 0 ? "Stopped" : "Started", f, item_index % 2 == 0 ? RedBrush : GreenBrush, margin1, start_height + height_offset + (row_count * data_height));

                        row_count++;
                        item_index++;
                    }
                }
                else if (Draw_Type == "Account_History")
                {
                    this.Width = Start_Size.Width + 300;

                    // Header
                    e.Graphics.DrawString("Payer/Payee", f_header, WritingBrush, start_margin + 35, start_height + (row_count * data_height));
                    e.Graphics.DrawString("Start Date", f_header, WritingBrush, margin1, start_height + (row_count * data_height));
                    e.Graphics.DrawString("Completion Date", f_header, WritingBrush, margin2, start_height + (row_count * data_height));
                    e.Graphics.DrawString("Amount", f_header, WritingBrush, margin3, start_height + (row_count * data_height));
                    row_count += 1;

                    int item_index = 0;



                    // For each refund item
                    foreach (Account a in parent.Account_List.Where(x => x.Status == 0))
                    {
                        
                        
                        if (a.Remark.Length < 30)
                        {
                            e.Graphics.DrawString(a.Payer + " (" + a.Type + ")", f, (a.Type == "Payable" ? RedBrush : GreenBrush), start_margin + 20, start_height + height_offset + (row_count * data_height));
                            height_offset += 8;
                            e.Graphics.DrawString(a.Start_Date.ToShortDateString(), f, (a.Type == "Payable" ? RedBrush : GreenBrush), margin1, start_height + height_offset + (row_count * data_height));
                            e.Graphics.DrawString(a.Inactive_Date.ToShortDateString(), f, (a.Type == "Payable" ? RedBrush : GreenBrush), margin2 + 20, start_height + height_offset + (row_count * data_height));
                            e.Graphics.DrawString(a.Amount, f, (a.Type == "Payable" ? RedBrush : GreenBrush), margin3 + 10 - (a.Amount.Length > 2 ? 6 : 0), start_height + height_offset + (row_count * data_height));

                            height_offset -= 8;
                            height_offset += data_height - 5;
                            e.Graphics.DrawString(a.Remark, f_italic, (a.Type == "Payable" ? RedBrush : GreenBrush), start_margin + 37, start_height + height_offset + (row_count * data_height));
                            row_count++;
                        }
                        else
                        {
                            e.Graphics.DrawString(a.Payer + " (" + a.Type + ")", f, (a.Type == "Payable" ? RedBrush : GreenBrush), start_margin + 20, start_height + height_offset + (row_count * data_height));
                            height_offset += data_height - 5;
                            e.Graphics.DrawString(a.Start_Date.ToShortDateString(), f, (a.Type == "Payable" ? RedBrush : GreenBrush), margin1, start_height + height_offset + (row_count * data_height));
                            e.Graphics.DrawString(a.Inactive_Date.ToShortDateString(), f, (a.Type == "Payable" ? RedBrush : GreenBrush), margin2 + 20, start_height + height_offset + (row_count * data_height));
                            e.Graphics.DrawString(a.Amount, f, (a.Type == "Payable" ? RedBrush : GreenBrush), margin3 + 10 - (a.Amount.Length > 2 ? 6 : 0), start_height + height_offset + (row_count * data_height));

                            height_offset -= data_height - 5;
                            height_offset += data_height - 5;
                            e.Graphics.DrawString(a.Remark.Substring(0, 30) + "-", f_italic, (a.Type == "Payable" ? RedBrush : GreenBrush), start_margin + 37, start_height + height_offset + (row_count * data_height));
                            height_offset += data_height - 5;
                            e.Graphics.DrawString(a.Remark.Substring(30), f_italic, (a.Type == "Payable" ? RedBrush : GreenBrush), start_margin + 37, start_height + height_offset + (row_count * data_height));
                            row_count++;
                        }
                        item_index++;
                    }
                }

                row_count++;
                this.Height = start_height + height_offset + row_count * data_height;
            }
            else
            {
                this.Height = Start_Size.Height;
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
            f_italic.Dispose();
            f_total.Dispose();
            f_header.Dispose();
            base.OnPaint(e);
        }

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;
        Expenses Ref_Exp = new Expenses();
        string Draw_Type = "Expenses";

        public Start_Stop_Dialog(Receipt _parent, Expenses ref_Exp, string dt = "Expenses")
        {
            Draw_Type = dt;
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Ref_Exp = ref_Exp;
            Start_Size = this.Size;
            Set_Form_Color(parent.Frame_Color);
            this.Invalidate();
            this.Update();
            if (Draw_Type != "Expenses") label5.Text = "Account History";
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            //label3.Text = "List of changes made to " + Ref_Exp.Expense_Name;
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
    }
}
