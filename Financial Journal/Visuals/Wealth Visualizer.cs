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
    public partial class Wealth_Visualizer : Form
    {
        int Load_Width = 0;
        List<ToolTip> ToolTip_List = new List<ToolTip>();

        // Converting month number to name
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Calculate_Months();
            Get_Line_Plots();

            int Extra_Width = 300;

            if (Month_Count > 15) 
                this.Width = Load_Width + Extra_Width;
            else
                this.Width = Load_Width;

            int start_margin = 65;
            int margin1 = start_margin + 15;   //
            int label_margin = start_margin - 55;   //

            int x_axis_height = Start_Size.Height / 3 * 2;

            Color DrawForeColor = Color.White;
            Color HighlightColor = Color.FromArgb(76, 76, 76);

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);
            SolidBrush GreyBrush = new SolidBrush(Color.FromArgb(122, 122, 122));
            SolidBrush BlueBrush = new SolidBrush(Color.LightBlue);
            SolidBrush GreenBrush = new SolidBrush(Color.LightGreen);
            SolidBrush PurpleBrush = new SolidBrush(Color.MediumPurple);
            SolidBrush RedBrush = new SolidBrush(Color.LightPink);
            SolidBrush OrangeBrush = new SolidBrush(Color.Orange);
            SolidBrush LightOrangeBrush = new SolidBrush(Color.FromArgb(255, 200, 0));

            Pen p = new Pen(WritingBrush, 1);
            Pen Grey_Pen = new Pen(GreyBrush, 1);
            Pen Blue_Pen = new Pen(BlueBrush, 1);
            Pen Green_Pen = new Pen(GreenBrush, 1);
            Pen Red_Pen = new Pen(RedBrush, 1);
            Pen Orange_Pen = new Pen(OrangeBrush, 1);
            Pen Purple_Pen = new Pen(PurpleBrush, 1);

            Font graph_font = new Font("MS Reference Sans Serif", 8, FontStyle.Regular);
            Font axis_font = new Font("MS Reference Sans Serif", 6, FontStyle.Regular);

            int Category_Tick_Interval = (Start_Size.Width - 80 - margin1) / (Month_Count + 1) + (Month_Count > 15 ? Extra_Width / Month_Count : 0);

            // Remove previously created buttons
            ToolTip_List.ForEach(x => x.Dispose());
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
                e.Graphics.DrawLine(p, start_margin, x_axis_height, (margin1 + (Month_Count + 1) * Category_Tick_Interval), x_axis_height);
                e.Graphics.DrawString("Time", graph_font, WritingBrush, (margin1 + (Month_Count + 1) * Category_Tick_Interval) + 5, x_axis_height - 7); // label

                // Draw Y-axis
                e.Graphics.DrawLine(p, margin1, 60, margin1, x_axis_height + (y_axis_tick_spread) * 1 + 15);
                e.Graphics.DrawString("$", graph_font, WritingBrush, start_margin + 10, 40); // label
                //e.Graphics.DrawString("Net Increase (Current vs. Last Month)", graph_font, WritingBrush, start_margin - 27, x_axis_height + (y_axis_tick_spread) * 6 - 16); // label

                // Draw X-axis Ticks
                for (int i = 1; i <= Month_Count; i++)
                {
                    // vertical guidance line
                    e.Graphics.DrawLine(Grey_Pen, (margin1 + i * Category_Tick_Interval), x_axis_height + (y_axis_tick_spread) * 1, (margin1 + i * Category_Tick_Interval), x_axis_height - (y_axis_tick_spread) * 5); // Grey vertical lines
                    e.Graphics.DrawLine(p, (margin1 + i * Category_Tick_Interval), x_axis_height - 5, (margin1 + i * Category_Tick_Interval), x_axis_height + 5); // draw ticks
                }

                // rightest vertical guidance line
                e.Graphics.DrawLine(Grey_Pen, (margin1 + (Month_Count + 1) * Category_Tick_Interval), x_axis_height + (y_axis_tick_spread) * 1, (margin1 + (Month_Count + 1) * Category_Tick_Interval), x_axis_height - (y_axis_tick_spread) * 5); // draw ticks

                // Get max value depending on relative or not
                double Max_Value = relative_box.Checked ? Enumerable.Max(new double[] {
                                            Income_Relative_Line.Get_Peak_Value(), 
                                            Expense_Relative_Line.Get_Peak_Value(), 
                                            Expenditure_Relative_Line.Get_Peak_Value(), 
                                            Wealth_Relative_Line.Get_Peak_Value()
                                        }) : 
                                        Enumerable.Max(new double[] {
                                            Income_Net_Line.Get_Peak_Value(), 
                                            Expense_Net_Line.Get_Peak_Value(), 
                                            Expenditure_Net_Line.Get_Peak_Value(), 
                                            Wealth_Net_Line.Get_Peak_Value()
                                        });


                // Above x-axis ticks
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height - (y_axis_tick_spread) * 1, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 1);
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Math.Round((Max_Value / 5), 2).ToString()), graph_font, WritingBrush, label_margin, x_axis_height - (y_axis_tick_spread) * 1 - 9); // label
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height - (y_axis_tick_spread) * 2, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 2);
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Math.Round((Max_Value / 5 * 2), 2).ToString()), graph_font, WritingBrush, label_margin, x_axis_height - (y_axis_tick_spread) * 2 - 9); // label
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height - (y_axis_tick_spread) * 3, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 3);
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Math.Round((Max_Value / 5 * 3), 2).ToString()), graph_font, WritingBrush, label_margin, x_axis_height - (y_axis_tick_spread) * 3 - 9); // label
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height - (y_axis_tick_spread) * 4, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 4);
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Math.Round((Max_Value / 5 * 4), 2).ToString()), graph_font, WritingBrush, label_margin, x_axis_height - (y_axis_tick_spread) * 4 - 9); // label
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height - (y_axis_tick_spread) * 5, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 5);
                e.Graphics.DrawString("$" + String.Format("{0:0.00}", Math.Round((Max_Value / 5 * 5), 2).ToString()), graph_font, WritingBrush, label_margin, x_axis_height - (y_axis_tick_spread) * 5 - 9); // label

                // Draw guidance lines
                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 1, (margin1 + (Month_Count + 1) * Category_Tick_Interval), x_axis_height - (y_axis_tick_spread) * 1);
                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 2, (margin1 + (Month_Count + 1) * Category_Tick_Interval), x_axis_height - (y_axis_tick_spread) * 2);
                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 3, (margin1 + (Month_Count + 1) * Category_Tick_Interval), x_axis_height - (y_axis_tick_spread) * 3);
                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 4, (margin1 + (Month_Count + 1) * Category_Tick_Interval), x_axis_height - (y_axis_tick_spread) * 4);
                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height - (y_axis_tick_spread) * 5, (margin1 + (Month_Count + 1) * Category_Tick_Interval), x_axis_height - (y_axis_tick_spread) * 5);

                // Draw guidance lines below
                e.Graphics.DrawLine(Grey_Pen, margin1 + 5, x_axis_height + (y_axis_tick_spread) * 1, (margin1 + (Month_Count + 1) * Category_Tick_Interval), x_axis_height + (y_axis_tick_spread) * 1);
                

                label_margin -= 5;
                // Below x-axis ticks
                e.Graphics.DrawLine(p, margin1 - 5, x_axis_height + (y_axis_tick_spread) * 1, margin1 + 5, x_axis_height + (y_axis_tick_spread) * 1);
                e.Graphics.DrawString("-$" + String.Format("{0:0.00}", Math.Round((Max_Value / 5), 2).ToString()), graph_font, WritingBrush, label_margin, x_axis_height + (y_axis_tick_spread) * 1 - 9); // label
                

                int upper_bound = x_axis_height - (y_axis_tick_spread) * 5;

                int radius = 3;

                // Above main x-axis

                // If net data
                if (!relative_box.Checked)
                {
                    #region Net Income Line
                    Pen temp_pen = new Pen(new SolidBrush(Income_Net_Line.Line_Color));
                    // draw dots data above axis (current month)
                    for (int i = 1; i <= Month_Count; i++)
                    {
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Income_Net_Line.p_point[Get_Date(i)]) - radius;

                        roundButton b = new roundButton();
                        b.BackColor = Income_Net_Line.Line_Color;
                        b.ForeColor = Income_Net_Line.Line_Color;
                        b.Name = "in" + i.ToString();
                        b.Size = new Size(6, 6);
                        b.Radius = 1;
                        b.Location = new Point((int)(Point_X), (int)(Point_Y));
                        //b.Click += new EventHandler(this.On_Click);
                        b.MouseEnter += new EventHandler(this.On_Click);
                        this.Controls.Add(b);
                        roundButton_Button_List.Add(b);
                        ToolTip ToolTip1 = new ToolTip();
                        ToolTip1.InitialDelay = 1;
                        ToolTip1.ReshowDelay = 1;
                        ToolTip1.SetToolTip(b, "$" + String.Format("{0:0.00}", Income_Net_Line.p_point[Get_Date(i)]));
                        ToolTip_List.Add(ToolTip1);
                    }

                    // Connect the lines
                    for (int i = 1; i <= Month_Count - 1; i++)
                    {
                        // First Set
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Income_Net_Line.p_point[Get_Date(i)]) - radius;

                        // Second Set
                        float Point_X2 = (margin1 + (i + 1) * Category_Tick_Interval) - radius;
                        float Point_Y2 = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Income_Net_Line.p_point[Get_Date(i + 1)]) - radius;

                        e.Graphics.DrawLine(temp_pen, Point_X + 2, Point_Y + 2, Point_X2 + 2, Point_Y2 + 2);
                    }
                    #endregion

                    #region Net Investment Line

                    if (showInvestments.Checked)
                    {
                        temp_pen = new Pen(new SolidBrush(Investment_Net_Line.Line_Color));
                        // draw dots data above axis (current month)
                        for (int i = 1; i <= Month_Count; i++)
                        {
                            float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                            float Point_Y =
                                (float) Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value,
                                    Investment_Net_Line.p_point[Get_Date(i)]) - radius;

                            roundButton b = new roundButton();
                            b.BackColor = Investment_Net_Line.Line_Color;
                            b.ForeColor = Investment_Net_Line.Line_Color;
                            b.Name = "in" + i.ToString();
                            b.Size = new Size(6, 6);
                            b.Radius = 1;
                            b.Location = new Point((int) (Point_X), (int) (Point_Y));
                            //b.Click += new EventHandler(this.On_Click);
                            b.MouseEnter += new EventHandler(this.On_Click);
                            this.Controls.Add(b);
                            roundButton_Button_List.Add(b);
                            ToolTip ToolTip1 = new ToolTip();
                            ToolTip1.InitialDelay = 1;
                            ToolTip1.ReshowDelay = 1;
                            ToolTip1.SetToolTip(b,
                                "$" + String.Format("{0:0.00}", Investment_Net_Line.p_point[Get_Date(i)]));
                            ToolTip_List.Add(ToolTip1);
                        }

                        // Connect the lines
                        for (int i = 1; i <= Month_Count - 1; i++)
                        {
                            // First Set
                            float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                            float Point_Y =
                                (float) Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value,
                                    Investment_Net_Line.p_point[Get_Date(i)]) - radius;

                            // Second Set
                            float Point_X2 = (margin1 + (i + 1) * Category_Tick_Interval) - radius;
                            float Point_Y2 =
                                (float) Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value,
                                    Investment_Net_Line.p_point[Get_Date(i + 1)]) - radius;

                            e.Graphics.DrawLine(temp_pen, Point_X + 2, Point_Y + 2, Point_X2 + 2, Point_Y2 + 2);
                        }
                    }

                    #endregion

                    #region Net Expense Line
                    temp_pen = new Pen(new SolidBrush(Expense_Net_Line.Line_Color));
                    // draw dots data above axis (current month)
                    for (int i = 1; i <= Month_Count; i++)
                    {
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Expense_Net_Line.p_point[Get_Date(i)]) - radius;

                        roundButton b = new roundButton();
                        b.BackColor = Expense_Net_Line.Line_Color;
                        b.ForeColor = Expense_Net_Line.Line_Color;
                        b.Name = "en" + i.ToString();
                        b.Size = new Size(6, 6);
                        b.Radius = 1;
                        b.Location = new Point((int)(Point_X), (int)(Point_Y));
                        //b.Click += new EventHandler(this.On_Click);
                        b.MouseEnter += new EventHandler(this.On_Click);
                        this.Controls.Add(b);
                        roundButton_Button_List.Add(b);
                        ToolTip ToolTip1 = new ToolTip();
                        ToolTip1.InitialDelay = 1;
                        ToolTip1.ReshowDelay = 1;
                        ToolTip1.SetToolTip(b, "$" + String.Format("{0:0.00}", Expense_Net_Line.p_point[Get_Date(i)]));
                        ToolTip_List.Add(ToolTip1);
                    }

                    // Connect the lines
                    for (int i = 1; i <= Month_Count - 1; i++)
                    {
                        // First Set
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Expense_Net_Line.p_point[Get_Date(i)]) - radius;

                        // Second Set
                        float Point_X2 = (margin1 + (i + 1) * Category_Tick_Interval) - radius;
                        float Point_Y2 = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Expense_Net_Line.p_point[Get_Date(i + 1)]) - radius;

                        e.Graphics.DrawLine(temp_pen, Point_X + 2, Point_Y + 2, Point_X2 + 2, Point_Y2 + 2);
                    }
                    temp_pen.Dispose();

                    if (separate_box.Checked)
                    {
                        temp_pen = new Pen(new SolidBrush(Expenditure_Net_Line.Line_Color));
                        // draw dots data above axis (current month)
                        for (int i = 1; i <= Month_Count; i++)
                        {
                            float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                            float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Expenditure_Net_Line.p_point[Get_Date(i)]) - radius;

                            roundButton b = new roundButton();
                            b.BackColor = Expenditure_Net_Line.Line_Color;
                            b.ForeColor = Expenditure_Net_Line.Line_Color;
                            b.Name = "en" + i.ToString();
                            b.Size = new Size(6, 6);
                            b.Radius = 1;
                            b.Location = new Point((int)(Point_X), (int)(Point_Y));
                            //b.Click += new EventHandler(this.On_Click);
                            b.MouseEnter += new EventHandler(this.On_Click);
                            this.Controls.Add(b);
                            roundButton_Button_List.Add(b);
                            ToolTip ToolTip1 = new ToolTip();
                            ToolTip1.InitialDelay = 1;
                            ToolTip1.ReshowDelay = 1;
                            ToolTip1.SetToolTip(b, "$" + String.Format("{0:0.00}", Expenditure_Net_Line.p_point[Get_Date(i)]));
                            ToolTip_List.Add(ToolTip1);
                        }

                        // Connect the lines
                        for (int i = 1; i <= Month_Count - 1; i++)
                        {
                            // First Set
                            float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                            float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Expenditure_Net_Line.p_point[Get_Date(i)]) - radius;

                            // Second Set
                            float Point_X2 = (margin1 + (i + 1) * Category_Tick_Interval) - radius;
                            float Point_Y2 = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Expenditure_Net_Line.p_point[Get_Date(i + 1)]) - radius;

                            e.Graphics.DrawLine(temp_pen, Point_X + 2, Point_Y + 2, Point_X2 + 2, Point_Y2 + 2);
                        }
                        temp_pen.Dispose();
                    }

                    #endregion

                    #region Net Wealth Line
                    temp_pen = new Pen(new SolidBrush(Wealth_Net_Line.Line_Color));
                    // draw dots data above axis (current month)
                    for (int i = 1; i <= Month_Count; i++)
                    {
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Wealth_Net_Line.p_point[Get_Date(i)]) - radius;

                        roundButton b = new roundButton();
                        b.BackColor = Wealth_Net_Line.Line_Color;
                        b.ForeColor = Wealth_Net_Line.Line_Color;
                        b.Name = "wn" + i.ToString();
                        b.Size = new Size(6, 6);
                        b.Radius = 1;
                        b.Location = new Point((int)(Point_X), (int)(Point_Y));
                        //b.Click += new EventHandler(this.On_Click);
                        b.MouseEnter += new EventHandler(this.On_Click);
                        this.Controls.Add(b);
                        roundButton_Button_List.Add(b);
                        ToolTip ToolTip1 = new ToolTip();
                        ToolTip1.InitialDelay = 1;
                        ToolTip1.ReshowDelay = 1;
                        ToolTip1.SetToolTip(b, "$" + String.Format("{0:0.00}", Wealth_Net_Line.p_point[Get_Date(i)]));
                        ToolTip_List.Add(ToolTip1);
                    }

                    // Connect the lines
                    for (int i = 1; i <= Month_Count - 1; i++)
                    {
                        // First Set
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Wealth_Net_Line.p_point[Get_Date(i)]) - radius;

                        // Second Set
                        float Point_X2 = (margin1 + (i + 1) * Category_Tick_Interval) - radius;
                        float Point_Y2 = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Wealth_Net_Line.p_point[Get_Date(i + 1)]) - radius;

                        e.Graphics.DrawLine(temp_pen, Point_X + 2, Point_Y + 2, Point_X2 + 2, Point_Y2 + 2);
                    }
                    temp_pen.Dispose();
                    #endregion
                }
                else
                {
                    #region Relative Income Line
                    Pen temp_pen = new Pen(new SolidBrush(Income_Relative_Line.Line_Color));
                    // draw dots data above axis (current month)
                    for (int i = 1; i <= Month_Count; i++)
                    {
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Income_Relative_Line.p_point[Get_Date(i)]) - radius;


                        roundButton b = new roundButton();
                        b.BackColor = Income_Relative_Line.Line_Color;
                        b.ForeColor = Income_Relative_Line.Line_Color;
                        b.Name = "in" + i.ToString();
                        b.Size = new Size(6, 6);
                        b.Radius = 1;
                        b.Location = new Point((int)(Point_X), (int)(Point_Y));
                        //b.Click += new EventHandler(this.On_Click);
                        b.MouseEnter += new EventHandler(this.On_Click);
                        this.Controls.Add(b);
                        roundButton_Button_List.Add(b);
                        ToolTip ToolTip1 = new ToolTip();
                        ToolTip1.InitialDelay = 1;
                        ToolTip1.ReshowDelay = 1;
                        ToolTip1.SetToolTip(b, "$" + String.Format("{0:0.00}", Income_Relative_Line.p_point[Get_Date(i)]));
                        ToolTip_List.Add(ToolTip1);

                    }

                    // Connect the lines
                    for (int i = 1; i <= Month_Count - 1; i++)
                    {
                        // First Set
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Income_Relative_Line.p_point[Get_Date(i)]) - radius;

                        // Second Set
                        float Point_X2 = (margin1 + (i + 1) * Category_Tick_Interval) - radius;
                        float Point_Y2 = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Income_Relative_Line.p_point[Get_Date(i + 1)]) - radius;

                        e.Graphics.DrawLine(temp_pen, Point_X + 2, Point_Y + 2, Point_X2 + 2, Point_Y2 + 2);
                    }
                    #endregion

                    #region Relative Investment Line

                    if (showInvestments.Checked)
                    {
                        temp_pen = new Pen(new SolidBrush(Investment_Relative_Line.Line_Color));
                        // draw dots data above axis (current month)
                        for (int i = 1; i <= Month_Count; i++)
                        {
                            float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                            float Point_Y =
                                (float) Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value,
                                    Investment_Relative_Line.p_point[Get_Date(i)]) - radius;


                            roundButton b = new roundButton();
                            b.BackColor = Investment_Relative_Line.Line_Color;
                            b.ForeColor = Investment_Relative_Line.Line_Color;
                            b.Name = "in" + i.ToString();
                            b.Size = new Size(6, 6);
                            b.Radius = 1;
                            b.Location = new Point((int) (Point_X), (int) (Point_Y));
                            //b.Click += new EventHandler(this.On_Click);
                            b.MouseEnter += new EventHandler(this.On_Click);
                            this.Controls.Add(b);
                            roundButton_Button_List.Add(b);
                            ToolTip ToolTip1 = new ToolTip();
                            ToolTip1.InitialDelay = 1;
                            ToolTip1.ReshowDelay = 1;
                            ToolTip1.SetToolTip(b,
                                "$" + String.Format("{0:0.00}", Investment_Relative_Line.p_point[Get_Date(i)]));
                            ToolTip_List.Add(ToolTip1);

                        }

                        // Connect the lines
                        for (int i = 1; i <= Month_Count - 1; i++)
                        {
                            // First Set
                            float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                            float Point_Y =
                                (float) Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value,
                                    Investment_Relative_Line.p_point[Get_Date(i)]) - radius;

                            // Second Set
                            float Point_X2 = (margin1 + (i + 1) * Category_Tick_Interval) - radius;
                            float Point_Y2 =
                                (float) Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value,
                                    Investment_Relative_Line.p_point[Get_Date(i + 1)]) - radius;

                            e.Graphics.DrawLine(temp_pen, Point_X + 2, Point_Y + 2, Point_X2 + 2, Point_Y2 + 2);
                        }
                    }

                    #endregion

                    #region Relative Expense Line
                    temp_pen = new Pen(new SolidBrush(Expense_Relative_Line.Line_Color));
                    // draw dots data above axis (current month)
                    for (int i = 1; i <= Month_Count; i++)
                    {
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Expense_Relative_Line.p_point[Get_Date(i)]) - radius;

                        roundButton b = new roundButton();
                        b.BackColor = Expense_Relative_Line.Line_Color;
                        b.ForeColor = Expense_Relative_Line.Line_Color;
                        b.Name = "en" + i.ToString();
                        b.Size = new Size(6, 6);
                        b.Radius = 1;
                        b.Location = new Point((int)(Point_X), (int)(Point_Y));
                        //b.Click += new EventHandler(this.On_Click);
                        b.MouseEnter += new EventHandler(this.On_Click);
                        this.Controls.Add(b);
                        roundButton_Button_List.Add(b);
                        ToolTip ToolTip1 = new ToolTip();
                        ToolTip1.InitialDelay = 1;
                        ToolTip1.ReshowDelay = 1;
                        ToolTip1.SetToolTip(b, "$" + String.Format("{0:0.00}", Expense_Relative_Line.p_point[Get_Date(i)]));
                        ToolTip_List.Add(ToolTip1);
                    }

                    // Connect the lines
                    for (int i = 1; i <= Month_Count - 1; i++)
                    {
                        // First Set
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Expense_Relative_Line.p_point[Get_Date(i)]) - radius;

                        // Second Set
                        float Point_X2 = (margin1 + (i + 1) * Category_Tick_Interval) - radius;
                        float Point_Y2 = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Expense_Relative_Line.p_point[Get_Date(i + 1)]) - radius;

                        e.Graphics.DrawLine(temp_pen, Point_X + 2, Point_Y + 2, Point_X2 + 2, Point_Y2 + 2);
                    }
                    temp_pen.Dispose();


                    if (separate_box.Checked)
                    {
                        temp_pen = new Pen(new SolidBrush(Expenditure_Relative_Line.Line_Color));
                        // draw dots data above axis (current month)
                        for (int i = 1; i <= Month_Count; i++)
                        {
                            float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                            float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Expenditure_Relative_Line.p_point[Get_Date(i)]) - radius;

                            roundButton b = new roundButton();
                            b.BackColor = Expenditure_Relative_Line.Line_Color;
                            b.ForeColor = Expenditure_Relative_Line.Line_Color;
                            b.Name = "en" + i.ToString();
                            b.Size = new Size(6, 6);
                            b.Radius = 1;
                            b.Location = new Point((int)(Point_X), (int)(Point_Y));
                            //b.Click += new EventHandler(this.On_Click);
                            b.MouseEnter += new EventHandler(this.On_Click);
                            this.Controls.Add(b);
                            roundButton_Button_List.Add(b);
                            ToolTip ToolTip1 = new ToolTip();
                            ToolTip1.InitialDelay = 1;
                            ToolTip1.ReshowDelay = 1;
                            ToolTip1.SetToolTip(b, "$" + String.Format("{0:0.00}", Expenditure_Relative_Line.p_point[Get_Date(i)]));
                            ToolTip_List.Add(ToolTip1);
                        }

                        // Connect the lines
                        for (int i = 1; i <= Month_Count - 1; i++)
                        {
                            // First Set
                            float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                            float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Expenditure_Relative_Line.p_point[Get_Date(i)]) - radius;

                            // Second Set
                            float Point_X2 = (margin1 + (i + 1) * Category_Tick_Interval) - radius;
                            float Point_Y2 = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Expenditure_Relative_Line.p_point[Get_Date(i + 1)]) - radius;

                            e.Graphics.DrawLine(temp_pen, Point_X + 2, Point_Y + 2, Point_X2 + 2, Point_Y2 + 2);
                        }
                        temp_pen.Dispose();
                    }
                    #endregion

                    #region Relative Wealth Line
                    temp_pen = new Pen(new SolidBrush(Wealth_Relative_Line.Line_Color));
                    // draw dots data above axis (current month)
                    for (int i = 1; i <= Month_Count; i++)
                    {
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Wealth_Relative_Line.p_point[Get_Date(i)]) - radius;

                        roundButton b = new roundButton();
                        b.BackColor = Wealth_Relative_Line.Line_Color;
                        b.ForeColor = Wealth_Relative_Line.Line_Color;
                        b.Name = "wn" + i.ToString();
                        b.Size = new Size(6, 6);
                        b.Radius = 1;
                        b.Location = new Point((int)(Point_X), (int)(Point_Y));
                        //b.Click += new EventHandler(this.On_Click);
                        b.MouseEnter += new EventHandler(this.On_Click);
                        this.Controls.Add(b);
                        roundButton_Button_List.Add(b);
                        ToolTip ToolTip1 = new ToolTip();
                        ToolTip1.InitialDelay = 1;
                        ToolTip1.ReshowDelay = 1;
                        ToolTip1.SetToolTip(b, "$" + String.Format("{0:0.00}", Wealth_Relative_Line.p_point[Get_Date(i)]));
                        ToolTip_List.Add(ToolTip1);
                    }

                    // Connect the lines
                    for (int i = 1; i <= Month_Count - 1; i++)
                    {
                        // First Set
                        float Point_X = (margin1 + i * Category_Tick_Interval) - radius;
                        float Point_Y = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Wealth_Relative_Line.p_point[Get_Date(i)]) - radius;

                        // Second Set
                        float Point_X2 = (margin1 + (i + 1) * Category_Tick_Interval) - radius;
                        float Point_Y2 = (float)Get_Relative_Data_Height(upper_bound, x_axis_height, Max_Value, Wealth_Relative_Line.p_point[Get_Date(i + 1)]) - radius;

                        e.Graphics.DrawLine(temp_pen, Point_X + 2, Point_Y + 2, Point_X2 + 2, Point_Y2 + 2);
                    }
                    temp_pen.Dispose();
                    #endregion
                }

                x_axis_height = x_axis_height + (y_axis_tick_spread) * 5; // Re-renter x-axis height

                // Draw axis label at end (to ensure top-most behaviour)
                for (int i = 1; i <= Month_Count; i++)
                {
                    SizeF size = TextRenderer.MeasureText(Get_Date(i), graph_font);

                    DrawDiagonalString(e.Graphics,
                        Get_Date(i),
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
                    string Info_String = g.Parse_Dictionary_To_String(parent.Master_Item_List.Where(x => x.Category == b.Name.Substring(2) && x.Date.Month == DateTime.Now.Month).ToList());
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
                    string Info_String = g.Parse_Dictionary_To_String(parent.Master_Item_List.Where(x => x.Category == b.Name.Substring(2) && x.Date.Month == DateTime.Now.AddMonths(-1).Month).ToList());
                    Financial_Journal.Category_Summary FJCS = new Financial_Journal.Category_Summary(b.Name.Substring(2), new Point(Cursor.Position.X - 5, Cursor.Position.Y - 5), Info_String, true);
                    FJCS.ShowDialog();
                    //g.Dispose();
                }
            }
            else if (b.Name.StartsWith("yd")) // last month
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

        public string Get_Date(int Relative_Month_Count)
        {
            DateTime From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, 1);
            From_Date = From_Date.AddMonths(Relative_Month_Count - 1);
            return mfi.GetMonthName(From_Date.Month) + " " + From_Date.Year;
        }

        public DateTime Get_Date_DateTime(int Relative_Month_Count)
        {
            DateTime From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, 1);
            From_Date = From_Date.AddMonths(Relative_Month_Count - 1);
            return From_Date;
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
            Extra_Height += (S.Length >= 10 ? (S.Length >= 18 ? MySize.Width / 6 : MySize.Width / 7) : 8);
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



        public Wealth_Visualizer(Receipt _parent)
        {
            this.Location = new Point(_parent.Location.X + Start_Location_Offset, _parent.Location.Y + Start_Location_Offset - 15);
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Start_Size = new Size(this.Width - 50, this.Height);
            Set_Form_Color(parent.Frame_Color);

            /*
            bufferedPanel1.Controls.Add(label1);
            bufferedPanel1.Controls.Add(label2);
            bufferedPanel1.Controls.Add(from_month);
            bufferedPanel1.Controls.Add(from_year);
            bufferedPanel1.Controls.Add(to_year);
            bufferedPanel1.Controls.Add(to_month);
            bufferedPanel1.Controls.Add(seperate_box);
            bufferedPanel1.Controls.Add(relative_box);
            */
        }
        private void Receipt_Load(object sender, EventArgs e)
        {
            Load_Width = this.Width; 

            // Mousedown anywhere to drag
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            for (int i = 1; i < 13; i++)
            {
                from_month.Items.Add(mfi.GetMonthName(i));
                to_month.Items.Add(mfi.GetMonthName(i));
            }

            // Add years to box (only get the years where purchases have been made)
            List<string> Years = new List<string>();
            foreach (Order order in parent.Order_List)
            {
                if (!Years.Contains(order.Date.Year.ToString()))
                {
                    Years.Add(order.Date.Year.ToString());
                }
            }

            Years = Years.OrderBy(x => Convert.ToInt32(x)).ToList();
            Years.ForEach(x => from_year.Items.Add(x));
            Years.ForEach(x => to_year.Items.Add(x));

            from_month.Text = mfi.GetMonthName(DateTime.Now.Month);
            to_year.Text = DateTime.Now.Year.ToString();
            from_year.Text = from_year.Items.Contains((DateTime.Now.Year - 1).ToString()) ? (DateTime.Now.Year - 1).ToString() : (DateTime.Now.Year).ToString();
            to_month.Text = mfi.GetMonthName(DateTime.Now.Month);


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

        Line_Plot Income_Net_Line = new Line_Plot();
        Line_Plot Investment_Net_Line = new Line_Plot();
        Line_Plot Expense_Net_Line = new Line_Plot();
        Line_Plot Expenditure_Net_Line = new Line_Plot();
        Line_Plot Wealth_Net_Line = new Line_Plot();
        Line_Plot Income_Relative_Line = new Line_Plot();
        Line_Plot Investment_Relative_Line = new Line_Plot();
        Line_Plot Expense_Relative_Line = new Line_Plot();
        Line_Plot Expenditure_Relative_Line = new Line_Plot();
        Line_Plot Wealth_Relative_Line = new Line_Plot();

        public void Get_Line_Plots()
        {
            if (!relative_box.Checked)
            {
                double aggregate_value = 0;
                // Net Income
                Income_Net_Line = new Line_Plot() { Line_Color = Color.LightBlue };
                for (int i = 1; i < Month_Count + 1; i++)
                {
                    // Get Dynamic Income
                    using (Savings_Helper SH = new Savings_Helper(parent))
                    {
                        aggregate_value += SH.Get_Monthly_Salary(Get_Date_DateTime(i).Month, Get_Date_DateTime(i).Year);
                    }
                    Income_Net_Line.p_point.Add(Get_Date(i), aggregate_value);
                }
                aggregate_value = 0;

                // Net Investment
                Investment_Net_Line = new Line_Plot() { Line_Color = Color.Orange };
                if (showInvestments.Checked)
                {
                    for (int i = 1; i < Month_Count + 1; i++)
                    {
                        aggregate_value = 0;
                        // Add Investments
                        foreach (Investment IV in parent.Investment_List)
                        {
                            aggregate_value += IV.Get_Amt_Since_Period_Start(Get_Date_DateTime(i));
                        }
                        Investment_Net_Line.p_point.Add(Get_Date(i), aggregate_value);
                    }
                    aggregate_value = 0;
                }

                // Net Expense
                if (separate_box.Checked)
                {
                    // Net Expense
                    Expense_Net_Line = new Line_Plot() { Line_Color = Color.LightPink };
                    Expenditure_Net_Line = new Line_Plot() { Line_Color = Color.Cyan };

                    for (int i = 1; i < Month_Count + 1; i++)
                    {
                        double Total_Expenses = parent.Expenses_List.Sum(x => x.Get_Total_Paid(Get_Date_DateTime(i), Get_Date_DateTime(i).AddMonths(1)));
                        aggregate_value += Total_Expenses;
                        Expense_Net_Line.p_point.Add(Get_Date(i), aggregate_value);
                    }
                    aggregate_value = 0;
                    // Net Expenditure
                    for (int i = 1; i < Month_Count + 1; i++)
                    {
                        double Total_Expenditure = parent.Order_List.Where(x => x.Date.Month == Get_Date_DateTime(i).Month && x.Date.Year == Get_Date_DateTime(i).Year).ToList().Sum(x => x.Order_Taxes + x.Order_Total_Pre_Tax);
                        aggregate_value += Total_Expenditure;
                        Expenditure_Net_Line.p_point.Add(Get_Date(i), aggregate_value);
                    }
                }
                else
                {
                    // Net Expense
                    Expense_Net_Line = new Line_Plot() { Line_Color = Color.LightPink };
                    for (int i = 1; i < Month_Count + 1; i++)
                    {
                        double Total_Expenses = parent.Expenses_List.Sum(x => x.Get_Total_Paid(Get_Date_DateTime(i), Get_Date_DateTime(i).AddMonths(1)));
                        double Total_Expenditure = parent.Order_List.Where(x => x.Date.Month == Get_Date_DateTime(i).Month && x.Date.Year == Get_Date_DateTime(i).Year).ToList().Sum(x => x.Order_Taxes + x.Order_Total_Pre_Tax);
                        aggregate_value += Total_Expenses + Total_Expenditure;
                        Expense_Net_Line.p_point.Add(Get_Date(i), aggregate_value);
                    }
                }

                aggregate_value = 0;
                // Net Wealth
                Wealth_Net_Line = new Line_Plot() { Line_Color = Color.LightGreen };
                for (int i = 1; i < Month_Count + 1; i++)
                {
                    aggregate_value = (Income_Net_Line.p_point[Get_Date(i)] + (showInvestments.Checked ? Investment_Net_Line.p_point[Get_Date(i)] : 0) - Expense_Net_Line.p_point[Get_Date(i)] - (separate_box.Checked ? Expenditure_Net_Line.p_point[Get_Date(i)] : 0));
                    Wealth_Net_Line.p_point.Add(Get_Date(i), aggregate_value);
                }
            }
            else
            {

                // Relative Income
                Income_Relative_Line = new Line_Plot() { Line_Color = Color.LightBlue };
                for (int i = 1; i < Month_Count + 1; i++)
                {
                    // Get Dynamic Income
                    using (Savings_Helper SH = new Savings_Helper(parent))
                    {
                        Income_Relative_Line.p_point.Add(Get_Date(i), SH.Get_Monthly_Salary(Get_Date_DateTime(i).Month, Get_Date_DateTime(i).Year));
                    }
                }

                // Relative Investment
                Investment_Relative_Line = new Line_Plot() { Line_Color = Color.Orange };
                if (showInvestments.Checked)
                {
                    for (int i = 1; i < Month_Count + 1; i++)
                    {
                        double aggregate_value = 0;
                        // Add Investments
                        foreach (Investment IV in parent.Investment_List)
                        {
                            aggregate_value += IV.Get_Amt_Since_Period_Start(Get_Date_DateTime(i));
                        }

                        Investment_Relative_Line.p_point.Add(Get_Date(i), aggregate_value);
                    }
                }

                if (separate_box.Checked)
                {
                    // Relative Expense
                    Expense_Relative_Line = new Line_Plot() { Line_Color = Color.LightPink };
                    Expenditure_Relative_Line = new Line_Plot() { Line_Color = Color.Cyan };
                    for (int i = 1; i < Month_Count + 1; i++)
                    {
                        double Total_Expenses = parent.Expenses_List.Sum(x => x.Get_Total_Paid(Get_Date_DateTime(i), Get_Date_DateTime(i).AddMonths(1)));
                        Expense_Relative_Line.p_point.Add(Get_Date(i), Total_Expenses);
                    }
                    // Relative Expenditure
                    for (int i = 1; i < Month_Count + 1; i++)
                    {
                        double Total_Expenditure = parent.Order_List.Where(x => x.Date.Month == Get_Date_DateTime(i).Month && x.Date.Year == Get_Date_DateTime(i).Year).ToList().Sum(x => x.Order_Taxes + x.Order_Total_Pre_Tax);
                        Expenditure_Relative_Line.p_point.Add(Get_Date(i), Total_Expenditure);
                    }
                }
                else
                {
                    // Relative Expense
                    Expense_Relative_Line = new Line_Plot() { Line_Color = Color.LightPink };
                    for (int i = 1; i < Month_Count + 1; i++)
                    {
                        double Total_Expenses = parent.Expenses_List.Sum(x => x.Get_Total_Paid(Get_Date_DateTime(i), Get_Date_DateTime(i).AddMonths(1)));
                        double Total_Expenditure = parent.Order_List.Where(x => x.Date.Month == Get_Date_DateTime(i).Month && x.Date.Year == Get_Date_DateTime(i).Year).ToList().Sum(x => x.Order_Taxes + x.Order_Total_Pre_Tax);
                        Expense_Relative_Line.p_point.Add(Get_Date(i), Total_Expenses + Total_Expenditure);
                    }
                }

                // Relative Expense
                Wealth_Relative_Line = new Line_Plot() { Line_Color = Color.LightGreen };
                for (int i = 1; i < Month_Count + 1; i++)
                {
                    Wealth_Relative_Line.p_point.Add(Get_Date(i), (Income_Relative_Line.p_point[Get_Date(i)] + (showInvestments.Checked ? Investment_Relative_Line.p_point[Get_Date(i)] : 0)) - Expense_Relative_Line.p_point[Get_Date(i)] - (separate_box.Checked ? Expenditure_Relative_Line.p_point[Get_Date(i)] : 0));
                }
            }

            label8.Visible = separate_box.Checked;
            label9.Visible = showInvestments.Checked;

            label3.ForeColor = relative_box.Checked ? Income_Relative_Line.Line_Color : Income_Net_Line.Line_Color;
            label4.ForeColor = relative_box.Checked ? Expense_Relative_Line.Line_Color : Expense_Net_Line.Line_Color;
            label6.ForeColor = relative_box.Checked ? Wealth_Relative_Line.Line_Color : Wealth_Net_Line.Line_Color;
            label8.ForeColor = relative_box.Checked ? Expenditure_Relative_Line.Line_Color : Expenditure_Net_Line.Line_Color;
            label9.ForeColor = relative_box.Checked ? Investment_Relative_Line.Line_Color : Investment_Net_Line.Line_Color;


            label3.Text = relative_box.Checked ? "Relative Income" : "Aggregate Income";
            label4.Text = relative_box.Checked ? "Relative Expenses" : "Aggregate Expenses";
            label6.Text = relative_box.Checked ? "Monthly Residual" : "Aggregate Wealth";
            label8.Text = relative_box.Checked ? "Relative Expenditure" : "Aggregate Expenditure";
            label9.Text = relative_box.Checked ? "Relative Investments" : "Aggregate Investments";


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
            //Update();
        }

        public void Refresh_Window()
        {
            Invalidate();
            //Update();
        }

        private void from_month_SelectedIndexChanged(object sender, EventArgs e)
        {

            Invalidate();
            //Update();
        }

        private void from_year_SelectedIndexChanged(object sender, EventArgs e)
        {

            Invalidate();
            //Update();
        }

        private void to_month_SelectedIndexChanged(object sender, EventArgs e)
        {

            Invalidate();
            //Update();
        }

        private void to_year_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            Invalidate();
            //Update();
        }

        int Month_Count = 0;

        private void Calculate_Months()
        {
            Month_Count = 0;

            DateTime From_Date = new DateTime(Convert.ToInt32(from_year.Text), from_month.SelectedIndex + 1, 1);
            DateTime To_Date = new DateTime(Convert.ToInt32(to_year.Text), to_month.SelectedIndex + 1, 1);

            // If invalid date selection, set dates to be the same
            if (From_Date > To_Date)
            {
                from_month.Text = to_month.Text = mfi.GetMonthName(DateTime.Now.Month);
                from_year.Text = to_year.Text = (DateTime.Now.Year).ToString();
            }
            else
            {
                Month_Count = ((To_Date.Year - From_Date.Year) * 12) + To_Date.Month - From_Date.Month + 1;
            }
        }

        private void seperate_box_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
            //Update();
        }

        private void showInvestments_CheckedChanged(object sender, EventArgs e)
        {
            Invalidate();
            //Update();
        }
    }
}
