using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace Financial_Journal
{
    public partial class Month_Over_Month_Graph : Form
    {

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int start_margin = 55;
            int margin1 = start_margin + 15;   //
            int label_margin = start_margin - 45;   //

            int x_axis_height = Start_Size.Height / 2;

            Color DrawForeColor = Color.White;
            Color HighlightColor = Color.FromArgb(76, 76, 76);

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(122, 122, 122));
            SolidBrush BlueBrush = new SolidBrush(Color.LightBlue);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            SolidBrush PurpleBrush = new SolidBrush(Color.MediumPurple);
            SolidBrush RedBrush = new SolidBrush(Color.LightPink);
            SolidBrush OrangeBrush = new SolidBrush(Color.Orange);
            SolidBrush LightOrangeBrush = new SolidBrush(Color.FromArgb(255, 200,0));

            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 1);
            Pen Blue_Pen = new Pen(BlueBrush, 1);
            Pen Green_Pen = new Pen(GreenBrush, 1);
            Pen Red_Pen = new Pen(RedBrush, 1);
            Pen Orange_Pen = new Pen(OrangeBrush, 1);
            Pen Purple_Pen = new Pen(PurpleBrush, 1);

            Font graph_font = new Font("MS Reference Sans Serif", 8, FontStyle.Regular);
            Font axis_font = new Font("MS Reference Sans Serif", 6, FontStyle.Regular);

            int Category_Tick_Interval = (Start_Size.Width - 80 - margin1)/(parent.category_box.Items.Count + 1);

            // Remove previously created buttons
            Button_List.ForEach(button => button.Dispose());
            Button_List.ForEach(button => this.Controls.Remove(button));
            Button_List = new List<Button>();

            roundButton_Button_List.ForEach(button => button.Dispose());
            roundButton_Button_List.ForEach(button => this.Controls.Remove(button));
            roundButton_Button_List = new List<roundButton>();

            // If has order
            if (true)
            {
                int y_axis_tick_spread = 34;

                // Draw X-axis
                e.Graphics.DrawLine(p, start_margin, x_axis_height, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval), x_axis_height);
                e.Graphics.DrawString("(Categories)", graph_font, WritingBrush, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval) + 5, x_axis_height - 7); // label

                // Draw Y-axis
                e.Graphics.DrawLine(p, margin1, 60, margin1, Start_Size.Height - 60);
                e.Graphics.DrawString("Month-Over-Month", graph_font, WritingBrush, start_margin - 38, 40); // label
                e.Graphics.DrawString("Net Increase (Current vs. Last Month)", graph_font, WritingBrush, start_margin - 27, x_axis_height + (y_axis_tick_spread) * 6 - 16); // label

                // Draw X-axis Ticks
                for (int i = 1; i <= parent.category_box.Items.Count; i++)
                {
                    e.Graphics.DrawLine(Grey_Pen, (margin1 + i * Category_Tick_Interval), x_axis_height + (y_axis_tick_spread) * 5, (margin1 + i * Category_Tick_Interval), x_axis_height - (y_axis_tick_spread) * 5); // Grey vertical lines
                    e.Graphics.DrawLine(p, (margin1 + i * Category_Tick_Interval), x_axis_height - 5, (margin1 + i * Category_Tick_Interval), x_axis_height + 5); // draw ticks
                }
                e.Graphics.DrawLine(Grey_Pen, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval), x_axis_height + (y_axis_tick_spread) * 5, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval), x_axis_height - (y_axis_tick_spread) * 5); // draw ticks

                double Max_Value = new double[]  {c_month_box.Checked ? Current_Month_Line.Get_Peak_Value() : 0, 
                                                 l_month_box.Checked ? Last_Month_Line.Get_Peak_Value() : 0, 
                                                 ytd_box.Checked ? Current_Year_Line.Get_Peak_Value() : 0 }.Max();

                    //Math.Max(Current_Year_Line.Get_Peak_Value(), 1, 2, 3);

                // New year adjust maximum since current year is fresh
                if (Max_Value < Last_Month_Line.Get_Peak_Value()) Max_Value = Last_Month_Line.Get_Peak_Value();

                // Above x-axis ticks
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height - (y_axis_tick_spread) * 1, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 1);
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Math.Round((Max_Value / 5 * 1), 2).ToString()), graph_font, WritingBrush, label_margin, x_axis_height - (y_axis_tick_spread) * 1 - 9); // label
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height - (y_axis_tick_spread) * 2, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 2);
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Math.Round((Max_Value / 5 * 2), 2).ToString()), graph_font, WritingBrush, label_margin, x_axis_height - (y_axis_tick_spread) * 2 - 9); // label
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height - (y_axis_tick_spread) * 3, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 3);
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Math.Round((Max_Value / 5 * 3), 2).ToString()), graph_font, WritingBrush, label_margin, x_axis_height - (y_axis_tick_spread) * 3 - 9); // label
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height - (y_axis_tick_spread) * 4, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 4);
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Math.Round((Max_Value / 5 * 4), 2).ToString()), graph_font, WritingBrush, label_margin, x_axis_height - (y_axis_tick_spread) * 4 - 9); // label
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height - (y_axis_tick_spread) * 5, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 5);
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Math.Round((Max_Value / 5 * 5), 2).ToString()), graph_font, WritingBrush, label_margin, x_axis_height - (y_axis_tick_spread) * 5 - 9); // label

                // Draw guidance lines
                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 1, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval), x_axis_height - (y_axis_tick_spread) * 1);
                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 2, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval), x_axis_height - (y_axis_tick_spread) * 2);
                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 3, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval), x_axis_height - (y_axis_tick_spread) * 3);
                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 4, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval), x_axis_height - (y_axis_tick_spread) * 4);
                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 5, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval), x_axis_height - (y_axis_tick_spread) * 5);

                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height + (y_axis_tick_spread) * 1, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval), x_axis_height + (y_axis_tick_spread) * 1);
                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height + (y_axis_tick_spread) * 2, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval), x_axis_height + (y_axis_tick_spread) * 2);
                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height + (y_axis_tick_spread) * 3, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval), x_axis_height + (y_axis_tick_spread) * 3);
                e.Graphics.DrawString("(Net Increase)", graph_font, WritingBrush, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval) + 5, x_axis_height + (y_axis_tick_spread) * 5 - 7); // label
                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height + (y_axis_tick_spread) * 4, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval), x_axis_height + (y_axis_tick_spread) * 4);
                e.Graphics.DrawLine(p, margin1 + 5, x_axis_height + (y_axis_tick_spread) * 5, (margin1 + (parent.category_box.Items.Count + 1) * Category_Tick_Interval), x_axis_height + (y_axis_tick_spread) * 5); // Center axis

                label_margin -= 5;
                // Below x-axis ticks
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height + (y_axis_tick_spread) * 1, margin1 + 5, x_axis_height + (y_axis_tick_spread) * 1);
                e.Graphics.DrawString("+$400Δ", graph_font, WritingBrush, label_margin, x_axis_height + (y_axis_tick_spread) * 1 - 9); // label
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height + (y_axis_tick_spread) * 2, margin1 + 5, x_axis_height + (y_axis_tick_spread) * 2);
                e.Graphics.DrawString("+$300Δ", graph_font, WritingBrush, label_margin, x_axis_height + (y_axis_tick_spread) * 2 - 9); // label
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height + (y_axis_tick_spread) * 3, margin1 + 5, x_axis_height + (y_axis_tick_spread) * 3);
                e.Graphics.DrawString("+$200Δ", graph_font, WritingBrush, label_margin, x_axis_height + (y_axis_tick_spread) * 3 - 9); // label
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height + (y_axis_tick_spread) * 4, margin1 + 5, x_axis_height + (y_axis_tick_spread) * 4);
                e.Graphics.DrawString("+$100Δ", graph_font, WritingBrush, label_margin, x_axis_height + (y_axis_tick_spread) * 4 - 9); // label
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height + (y_axis_tick_spread) * 5, margin1 + 5, x_axis_height + (y_axis_tick_spread) * 5);
                e.Graphics.DrawString("$0Δ", graph_font, WritingBrush, label_margin + 24, x_axis_height + (y_axis_tick_spread) * 5 - 9); // label

                int upper_bound = x_axis_height - (y_axis_tick_spread) * 5;

                int radius = 3;

                // Above main x-axis

                if (c_month_box.Checked)
                {
                    #region Current Month Values
                    // draw dots data above axis (current month)
                    for (int i = 1; i <= parent.category_box.Items.Count; i++)
                    {
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Current_Month_Line.p_point[parent.category_box.Items[i - 1].ToString()]) - radius;

                        //ToolTip ToolTip1 = new ToolTip();
                        //ToolTip1.InitialDelay = 1;
                        //ToolTip1.ReshowDelay = 1;

                        roundButton b = new roundButton();
                        b.BackColor = BlueBrush.Color;
                        b.ForeColor = BlueBrush.Color;
                        b.Name = "cm" + parent.category_box.Items[i - 1].ToString();
                        b.Size = new Size(6,6);
                        b.Radius = 1;
                        b.Location = new Point((int)(Point_X), (int)(Point_Y));
                        //b.Click += new EventHandler(this.On_Click);
                        b.MouseEnter += new EventHandler(this.On_Click);
                        this.Controls.Add(b);
                        roundButton_Button_List.Add(b);
                        //ToolTip1.SetToolTip(b, parent.category_box.Items[i - 1].ToString() + Environment.NewLine + "(" + "$" + String.Format("{0:0.00}", ((double)Current_Month_Line.p_point[parent.category_box.Items[i - 1].ToString()] - (double)Last_Month_Line.p_point[parent.category_box.Items[i - 1].ToString()])) + ")");

                        /*
                        e.Graphics.DrawEllipse(Blue_Pen, Point_X, Point_Y, radius * 2, radius * 2);
                        e.Graphics.FillEllipse(BlueBrush, Point_X, Point_Y, radius * 2, radius * 2);
                         * */
                    }
                    // draw dots data above axis (current month)
                    for (int i = 1; i <= parent.category_box.Items.Count - 1; i++)
                    {
                        // First Set
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Current_Month_Line.p_point[parent.category_box.Items[i - 1].ToString()]) - radius;

                        // Second Set
                        float Point_X2 = (margin1 + (i + 1) * Category_Tick_Interval) - radius;
                        float Point_Y2 = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Current_Month_Line.p_point[parent.category_box.Items[i].ToString()]) - radius;


                        e.Graphics.DrawLine(Blue_Pen, Point_X + 2, Point_Y + 2, Point_X2 + 2, Point_Y2 + 2);
                    }
                    #endregion
                }

                if (l_month_box.Checked)
                {
                    #region Last Month Values
                    // draw dots data above axis (current month)
                    for (int i = 1; i <= parent.category_box.Items.Count; i++)
                    {
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Last_Month_Line.p_point[parent.category_box.Items[i - 1].ToString()]) - radius;

                        roundButton b = new roundButton();
                        b.BackColor = GreenBrush.Color;
                        b.ForeColor = GreenBrush.Color;
                        b.Name = "lm" + parent.category_box.Items[i - 1].ToString();
                        b.Size = new Size(6, 6);
                        b.Radius = 1;
                        b.Location = new Point((int)(Point_X), (int)(Point_Y));
                        //b.Click += new EventHandler(this.On_Click);
                        b.MouseEnter += new EventHandler(this.On_Click);
                        this.Controls.Add(b);
                        roundButton_Button_List.Add(b);
                        /*
                        e.Graphics.DrawEllipse(Green_Pen, Point_X, Point_Y, radius * 2, radius * 2);
                        e.Graphics.FillEllipse(GreenBrush, Point_X, Point_Y, radius * 2, radius * 2);
                        */
                    }
                    // draw dots data above axis (current month)
                    for (int i = 1; i <= parent.category_box.Items.Count - 1; i++)
                    {
                        // First Set
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Last_Month_Line.p_point[parent.category_box.Items[i - 1].ToString()]) - radius;

                        // Second Set
                        float Point_X2 = (margin1 + (i + 1) * Category_Tick_Interval) - radius;
                        float Point_Y2 = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Last_Month_Line.p_point[parent.category_box.Items[i].ToString()]) - radius;


                        e.Graphics.DrawLine(Green_Pen, Point_X + 2, Point_Y + 2, Point_X2 + 2, Point_Y2 + 2);
                    }
                    #endregion
                }

                if (ytd_box.Checked)
                {
                    #region Current Year Values
                // draw dots data above axis (current month)
                for (int i = 1; i <= parent.category_box.Items.Count; i++)
                {
                    float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                    float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Current_Year_Line.p_point[parent.category_box.Items[i - 1].ToString()]) - radius;

                    roundButton b = new roundButton();
                    b.BackColor = RedBrush.Color;
                    b.ForeColor = RedBrush.Color;
                    b.Name = "yd" + parent.category_box.Items[i - 1].ToString();
                    b.Size = new Size(6, 6);
                    b.Radius = 1;
                    b.Location = new Point((int)(Point_X), (int)(Point_Y));
                    //b.Click += new EventHandler(this.On_Click);
                    b.MouseEnter += new EventHandler(this.On_Click);
                    this.Controls.Add(b);
                    roundButton_Button_List.Add(b);
                }
                // draw dots data above axis (current month)
                for (int i = 1; i <= parent.category_box.Items.Count - 1; i++)
                {
                    // First Set
                    float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                    float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Current_Year_Line.p_point[parent.category_box.Items[i - 1].ToString()]) - radius;

                    // Second Set
                    float Point_X2 = (margin1 + (i + 1) * Category_Tick_Interval) - radius;
                    float Point_Y2 = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Current_Year_Line.p_point[parent.category_box.Items[i].ToString()]) - radius;


                    e.Graphics.DrawLine(Red_Pen, Point_X + 2, Point_Y + 2, Point_X2 + 2, Point_Y2 + 2);
                }
                #endregion
                }

                if (ytd_avg_box.Checked)
                {
                    #region YTD-AVG Values
                    // draw dots data above axis (current month)
                    for (int i = 1; i <= parent.category_box.Items.Count; i++)
                    {
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Current_Year_Line.p_point[parent.category_box.Items[i - 1].ToString()] / DateTime.Now.Month) - radius;
                        e.Graphics.DrawEllipse(Purple_Pen, Point_X, Point_Y, radius * 2, radius * 2);
                        e.Graphics.FillEllipse(PurpleBrush, Point_X, Point_Y, radius * 2, radius * 2);
                    }
                    // draw dots data above axis (current month)
                    for (int i = 1; i <= parent.category_box.Items.Count - 1; i++)
                    {
                        // First Set
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Current_Year_Line.p_point[parent.category_box.Items[i - 1].ToString()]/DateTime.Now.Month) - radius;

                        // Second Set
                        float Point_X2 = (margin1 + (i + 1) * Category_Tick_Interval) - radius;
                        float Point_Y2 = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Current_Year_Line.p_point[parent.category_box.Items[i].ToString()] / DateTime.Now.Month) - radius;


                        e.Graphics.DrawLine(Purple_Pen, Point_X + 2, Point_Y + 2, Point_X2 + 2, Point_Y2 + 2);
                    }
                    #endregion
                }

                
                // Below main x-axis
                x_axis_height = x_axis_height + (y_axis_tick_spread) * 5; // Re-renter x-axis height
                upper_bound = x_axis_height - (y_axis_tick_spread) * 4;
                int lower_bound = x_axis_height + (y_axis_tick_spread) * 0;


                #region Current vs Last Month Values
                // draw dots data above axis (current month)
                for (int i = 1; i <= parent.category_box.Items.Count; i++)
                {
                    //float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                    //float Point_Y = (float)Get_Relative_Percent_Height(upper_bound, lower_bound, x_axis_height, Last_Month_Line.p_point[parent.category_box.Items[i - 1].ToString()], Current_Month_Line.p_point[parent.category_box.Items[i - 1].ToString()]) - radius;
                    //e.Graphics.DrawEllipse(Orange_Pen, Point_X, Point_Y, radius * 2, radius * 2);
                    //e.Graphics.FillEllipse(OrangeBrush, Point_X, Point_Y, radius * 2, radius * 2);


                    // First Set
                    int Point_X = (margin1 + i * Category_Tick_Interval);
                    int Point_Y = (int)Get_Relative_Percent_Height(upper_bound + 15, lower_bound, x_axis_height, Last_Month_Line.p_point[parent.category_box.Items[i - 1].ToString()], Current_Month_Line.p_point[parent.category_box.Items[i - 1].ToString()]);

                    // Bottom points
                    int Bottom_Y = x_axis_height;

                    if (Bottom_Y - Point_Y == 0)
                    {
                        Bottom_Y += 1;
                    }
                        
                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Rectangle r = new Rectangle(Point_X - 10, Point_Y, 20, Bottom_Y - Point_Y);
                    Rectangle r2 = new Rectangle(Point_X - 10 + 1, Point_Y + 1, 20 - 1, Bottom_Y - Point_Y - 1);
                    e.Graphics.DrawRectangle(p, r);
                    SolidBrush ref_brush = (Bottom_Y - Point_Y) > y_axis_tick_spread * 2.5 ? RedBrush : (Bottom_Y - Point_Y) > y_axis_tick_spread ? LightOrangeBrush : GreenBrush;
                    e.Graphics.FillRectangle(ref_brush, r2);

                    Button b = new Button();
                    b.BackColor = ref_brush.Color;
                    b.FlatAppearance.BorderSize = 0;
                    b.Name = parent.category_box.Items[i - 1].ToString();
                    b.FlatStyle = FlatStyle.Flat;
                    b.Size = new Size(20 - 1, Bottom_Y - Point_Y - 1);
                    b.Location = new Point(Point_X - 10 + 1, Point_Y + 1);
                    this.Controls.Add(b);
                    Button_List.Add(b);
                    ToolTip1.SetToolTip(b, parent.category_box.Items[i - 1].ToString() + Environment.NewLine + "(" + "$" + String.Format("{0:0.00}", ((double)Current_Month_Line.p_point[parent.category_box.Items[i - 1].ToString()] - (double)Last_Month_Line.p_point[parent.category_box.Items[i - 1].ToString()])) + ")");

                }
                #endregion

                // Draw axis label at end (to ensure top-most behaviour)
                for (int i = 1; i <= parent.category_box.Items.Count; i++)
                {
                    e.Graphics.DrawLine(p, (margin1 + i * Category_Tick_Interval), x_axis_height, (margin1 + i * Category_Tick_Interval), x_axis_height + 5); // draw ticks

                    SizeF size = TextRenderer.MeasureText(parent.category_box.Items[i - 1].ToString(), graph_font);

                    DrawDiagonalString(e.Graphics,
                        parent.category_box.Items[i - 1].ToString(),
                        axis_font, WritingBrush,
                        new Point((int)(((margin1 + i * Category_Tick_Interval))) - 6, x_axis_height + 12 - (y_axis_tick_spread) * 5),
                        20
                        );

                }

            }
            else
            {
            }

            // Dispose all objects
            p.Dispose();
            Grey_Pen.Dispose();
            GreyBrush.Dispose();
            BlueBrush.Dispose();
            RedBrush.Dispose();
            GreenBrush.Dispose();
            PurpleBrush.Dispose();
            OrangeBrush.Dispose();
            LightOrangeBrush.Dispose();
            Blue_Pen.Dispose();
            Green_Pen.Dispose();
            Red_Pen.Dispose();
            Purple_Pen.Dispose();
            Orange_Pen.Dispose();
            WritingBrush.Dispose();
            graph_font.Dispose();
            axis_font.Dispose();
            base.OnPaint(e);

            // Force handle reset
            MouseInput.ScrollWheel(-1);
        }

        private List<Button> Button_List = new List<Button>();
        private List<roundButton> roundButton_Button_List = new List<roundButton>();
        private Point Mouse_Location = new Point();

        private void On_Click(object sender, EventArgs e)
        {
            roundButton b = (roundButton)sender;

            if (b.Name.StartsWith("cm")) // current month
            {
                Mouse_Location = Cursor.Position;
                Thread.Sleep(250);
                if (Cursor.Position.X > Mouse_Location.X - 5 && Cursor.Position.X < Mouse_Location.X + 5 && 
                    Cursor.Position.Y > Mouse_Location.Y - 5 && Cursor.Position.Y < Mouse_Location.Y + 5 
                    )
                {
                    Expenditures g = new Expenditures(parent);
                    string Info_String = g.Parse_Dictionary_To_String(parent.Master_Item_List.Where(x => x.Category == b.Name.Substring(2) && x.Date.Month == DateTime.Now.Month && x.Date.Year == DateTime.Now.Year).ToList());
                    Financial_Journal.Category_Summary FJCS = new Financial_Journal.Category_Summary(b.Name.Substring(2), new Point(Cursor.Position.X - 5, Cursor.Position.Y - 5), Info_String, true);
                    FJCS.ShowDialog();
                    //g.Dispose();
                }
            }
            else if (b.Name.StartsWith("lm")) // last month
            {
                Mouse_Location = Cursor.Position;
                Thread.Sleep(250);
                if (Cursor.Position.X > Mouse_Location.X - 5 && Cursor.Position.X < Mouse_Location.X + 5 &&
                    Cursor.Position.Y > Mouse_Location.Y - 5 && Cursor.Position.Y < Mouse_Location.Y + 5
                    )
                {
                    Expenditures g = new Expenditures(parent);
                    string Info_String = g.Parse_Dictionary_To_String(parent.Master_Item_List.Where(x => x.Category == b.Name.Substring(2) && x.Date.Month == DateTime.Now.AddMonths(-1).Month && x.Date.Year == DateTime.Now.AddMonths(-1).Year).ToList());
                    Financial_Journal.Category_Summary FJCS = new Financial_Journal.Category_Summary(b.Name.Substring(2), new Point(Cursor.Position.X - 5, Cursor.Position.Y - 5), Info_String, true);
                    FJCS.ShowDialog();
                    //g.Dispose();
                }
            }
            else if (b.Name.StartsWith("yd")) // last year
            {
                Mouse_Location = Cursor.Position;
                Thread.Sleep(250);
                if (Cursor.Position.X > Mouse_Location.X - 5 && Cursor.Position.X < Mouse_Location.X + 5 &&
                    Cursor.Position.Y > Mouse_Location.Y - 5 && Cursor.Position.Y < Mouse_Location.Y + 5
                    )
                {
                    Expenditures g = new Expenditures(parent);
                    string Info_String = g.Parse_Dictionary_To_String(parent.Master_Item_List.Where(x => x.Category == b.Name.Substring(2) && x.Date.Year == DateTime.Now.Year).ToList());
                    Financial_Journal.Category_Summary FJCS = new Financial_Journal.Category_Summary(b.Name.Substring(2), new Point(Cursor.Position.X - 5, Cursor.Position.Y - 5), Info_String, true);
                    FJCS.ShowDialog();
                    //g.Dispose();
                }
            }
        }

        // For sub x-axis percent display
        public double Get_Relative_Percent_Height(int upper_bound, int lower_bound, double axis_height, double past_month, double curr_month)
        {
            double difference = (past_month - curr_month);
            /*
            if (past_month == 0)
            {
                if (curr_month > 0)
                    return upper_bound; //upper limit (aka infinity)
                else
                    return axis_height;
            }
            else
            {*/
                double return_value = 0;
                if (past_month > curr_month)
                {
                    /*
                    double height_diff = lower_bound - axis_height;
                    return_value = axis_height + (height_diff) * (1 - (height_diff - difference) / height_diff);
                    if (return_value > lower_bound) return_value = lower_bound;*/
                    return_value = lower_bound;
                }
                else
                {
                    // above axis
                    return_value = axis_height + (axis_height - upper_bound) * (1 - (400 - difference) / 400);
                    if (return_value < upper_bound) return_value = upper_bound;
                }
                return return_value;
                /*
                 = axis_height - (past_month > curr_month ? lower_bound : upper_bound) * ((past_month - curr_month) / past_month) * (past_month > curr_month ? -1 : 1);
                if (return_value < (-upper_bound + axis_height)) return (-upper_bound + axis_height);
                else if (return_value > (lower_bound + axis_height)) return (lower_bound + axis_height);
                else return return_value;*/
            //}
        }

        //bound height, negative value = below axis
        public double Get_Relative_Data_Height(double bound_height, double axis_height, double max_value, double value)
        {
            return axis_height - (axis_height - bound_height) * (1 - (max_value - value) / max_value) * (bound_height > 0 ? 1 : -1);
        }

        public void DrawDiagonalString(Graphics G, string S, Font F, Brush B, PointF P, int Angle)
        {

            SizeF MySize = TextRenderer.MeasureText(S, F);
            float Extra_Height = 0;
            Extra_Height += (S.Length >= 10 ? (S.Length >= 18 ? MySize.Width / 7 : MySize.Width / 7): 0);
            //SizeF MySize = G.MeasureString(S, F);
            G.TranslateTransform(P.X + MySize.Width / 2, P.Y + MySize.Height / 2 + Extra_Height);
            G.RotateTransform(Angle);
            G.DrawString(S, F, B, new PointF(-MySize.Width / 2, -MySize.Height / 2));
            G.RotateTransform(-Angle);
            G.TranslateTransform(-P.X - MySize.Width / 2, -P.Y - MySize.Height / 2 - Extra_Height);
        }

        Receipt parent;
        Size Start_Size = new Size();
        int Start_Location_Offset = 45;

        Line_Plot Current_Month_Line = new Line_Plot();
        Line_Plot Last_Month_Line = new Line_Plot();
        Line_Plot Current_Year_Line = new Line_Plot();


        public Month_Over_Month_Graph(Receipt _parent)
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Get_Current_Month_Line();
            Get_Last_Month_Line();
            Get_Current_Year_Line();
            Start_Size = new Size(this.Width - 50, this.Height);
            Set_Form_Color(parent.Frame_Color);

        }
        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
        }

        private void minimize_button_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            parent.MOMG_open = false;
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

        public void Get_Current_Month_Line()
        {
            Current_Month_Line = new Line_Plot() { Line_Color = Color.LightBlue };
            foreach (string category in parent.category_box.Items)
            {
                Current_Month_Line.p_point.Add(category, Get_Category_Total(category, DateTime.Now.Month));
            }
        }

        public void Get_Last_Month_Line()
        {
            Last_Month_Line = new Line_Plot() { Line_Color = Color.LightGray };
            foreach (string category in parent.category_box.Items)
            {
                Last_Month_Line.p_point.Add(category, Get_Category_Total(category, DateTime.Now.AddMonths(-1).Month, DateTime.Now.AddMonths(-1).Year));
            }
        }

        public void Get_Current_Year_Line()
        {
            Current_Year_Line = new Line_Plot() { Line_Color = Color.White };
            foreach (string category in parent.category_box.Items)
            {
                Current_Year_Line.p_point.Add(category, Get_Category_Total(category));
            }
        }

        public double Get_Category_Total(string Category, int Month = -1, int Year = -1)
        {
            return parent.Master_Item_List.Where(x => x.Category == Category && (Month < 0 ? true : (x.Date.Month == Month)) && (Year > 0 ? x.Date.Year == Year : x.Date.Year == DateTime.Now.Year)).ToList().Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));
        }

        public double Get_All_Total(int Month = -1, int Year = -1)
        {
            return parent.Master_Item_List.Where(x => (Month < 0 ? true : (x.Date.Month == Month)) && (Year > 0 ? x.Date.Year == Year : x.Date.Year == DateTime.Now.Year)).ToList().Sum(x => x.Get_Current_Amount(parent.Get_Tax_Amount(x)));
        }

        private void c_month_box_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
            Update();
        }

        private void l_month_box_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
            Update();
        }

        private void ytd_box_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
            Update();
        }

        private void ytd_avg_box_CheckedChanged(object sender, EventArgs e)
        {

            Invalidate();
            Update();
        }


        public void Refresh_Window()
        {
            Get_Current_Month_Line();
            Get_Last_Month_Line();
            Get_Current_Year_Line();
            Invalidate();
            Update();
        }
    }

    public class Line_Plot
    {
        // Key = Category, Value = value of plot line
        public Dictionary<string, double> p_point = new Dictionary<string, double>();
        public Color Line_Color { get; set; }

        public Line_Plot()
        {
        }

        public double Get_Peak_Value()
        {
            return p_point.Count == 0 ? 0 : p_point.Max(x => x.Value);
        }

        public double Get_Trough_Value()
        {
            return p_point.Count == 0 ? 0 : p_point.Min(x => x.Value);
        }
    }

    public static class GraphicsExtensions
    {
        public static void DrawCircle(this Graphics g, Pen pen,
                                      float centerX, float centerY, float radius)
        {
            g.DrawEllipse(pen, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }

        public static void FillCircle(this Graphics g, Brush brush,
                                      float centerX, float centerY, float radius)
        {
            g.FillEllipse(brush, centerX - radius, centerY - radius,
                          radius + radius, radius + radius);
        }
    }

    public class roundButton : UserControl
    {
        public int Radius { get; set; }

        // Draw the new button. 
        protected override void OnPaint(PaintEventArgs e)
        {
            /*
            Graphics graphics = e.Graphics;

            Pen myPen = new Pen(base.ForeColor);
            SolidBrush myBrush = new SolidBrush(base.BackColor);

            // Draw the button in the form of a circle
            graphics.DrawEllipse(myPen, this.Location.X, this.Location.Y, Radius * 2, Radius * 2);
            graphics.FillEllipse(myBrush, this.Location.X, this.Location.Y, Radius * 2, Radius * 2);

            myPen.Dispose();
            myBrush.Dispose();
            */
            base.OnPaint(e);
        }
    }
}
