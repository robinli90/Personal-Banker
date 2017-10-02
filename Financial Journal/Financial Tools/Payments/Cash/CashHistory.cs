using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Objects;

namespace Financial_Journal
{
    public partial class CashHistory : Form
    {
        private bool repaintButtons = true;
        private List<Button> dynamicButtons = new List<Button>();

        // Panel paint
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (repaintButtons)
            {
                dynamicButtons.ForEach(button => button.Image.Dispose());
                dynamicButtons.ForEach(button => button.Dispose());
                dynamicButtons.ForEach(button => bufferedPanel3.Controls.Remove(button));
                dynamicButtons = new List<Button>();
            }

            e.Graphics.Clear(BackColor);

            // allow scroll transformation
            e.Graphics.TranslateTransform(bufferedPanel3.AutoScrollPosition.X, bufferedPanel3.AutoScrollPosition.Y);

            int data_height = 15;
            int row_count = 0;
            int height_offset = 1;
            int start_height = 0;

            int buttonMargin = 5;
            int dateMargin = 47;
            int amountMargin = amountLabel.Left - 5;
            int balanceMargin = balanceLabel.Left - 7;

            Color DrawForeColor = Color.White;

            SolidBrush WritingBrush = new SolidBrush(DrawForeColor);

            Pen p = new Pen(WritingBrush, 1);
            Font f = new Font("MS Reference Sans Serif", 8F, FontStyle.Regular);
            Font f_italic = new Font("MS Reference Sans Serif", 8F, FontStyle.Italic);

            List<Objects.CashHistory> refCHList = Cash.GetHistoriesBetweenDates(fromDate.Value, toDate.Value, true);

            foreach (Objects.CashHistory CH in refCHList)
            {
                if (repaintButtons)
                {
                    ToolTip ToolTip1 = new ToolTip();
                    ToolTip1.InitialDelay = 1;
                    ToolTip1.ReshowDelay = 1;

                    Button dynamicButton = new Button();
                    dynamicButton.BackColor = BackColor;
                    dynamicButton.ForeColor = BackColor;
                    dynamicButton.FlatStyle = FlatStyle.Flat;
                    dynamicButton.Image = !CH.GetID().StartsWith("O") ? (CH.GetAmount() < 0 ? Properties.Resources.atm : Properties.Resources.deposit) :
                        (Properties.Resources.qlshoppingCart);
                    dynamicButton.Size = new Size(35, 35);
                    dynamicButton.Location = new Point(buttonMargin,
                        start_height + height_offset + (row_count * data_height) - 3);
                    dynamicButton.Name = CH.GetID();
                    dynamicButton.Text = "";
                    dynamicButton.Enabled = false;
                    dynamicButton.Click += dynamicButtonClick;
                    dynamicButtons.Add(dynamicButton);
                    if (CH.GetID().StartsWith("O"))
                    {
                        ToolTip1.SetToolTip(dynamicButton, "View this order");
                        dynamicButton.Enabled = true;
                    }
                    bufferedPanel3.Controls.Add(dynamicButton);
                }

                string viewStr;
                if (CH.GetID().Length == 0)
                {
                    viewStr = CH.GetAmount() < 0 ? "Used Cash" : "Deposit";
                }
                else
                {
                    viewStr = CH.GetID().StartsWith("O") ? "Purchase from " + parent.Order_List.First(x => x.OrderID == CH.GetID().Substring(1)).Location : 
                        CH.GetID() == "SB" ? "Balance directly modified" : 
                        String.Format("Transferred {0} {1}", (CH.GetAmount() < 0 ? "to" : "from"), CH.GetID().Substring(1));
                }
                
                e.Graphics.DrawString(CH.GetDate().ToShortDateString() + (CH.GetMemo().Length > 0 ? " - " + CH.GetMemo() : ""), f, WritingBrush, dateMargin,
                    start_height + height_offset + (row_count * data_height));
                height_offset += data_height / 2;
                e.Graphics.DrawString(CH.GetAmountStr(), f, WritingBrush, amountMargin,
                    start_height + height_offset + (row_count * data_height));
                e.Graphics.DrawString(CH.GetBalanceStr(), f, WritingBrush, balanceMargin,
                    start_height + height_offset + (row_count * data_height));
                height_offset += data_height / 2;
                e.Graphics.DrawString(viewStr, f, WritingBrush, dateMargin,
                    start_height + height_offset + (row_count * data_height));
                row_count++;
                height_offset += 10;
            }

            if (refCHList.Count == 0)
            {
                e.Graphics.DrawString("No cash transactions from", f, new SolidBrush(Color.Gray), bufferedPanel3.Width / 2 - 70,
                    bufferedPanel3.Height / 2 - 30);
                e.Graphics.DrawString(String.Format("{0} to {1}", fromDate.Value.ToShortDateString(), toDate.Value.ToShortDateString()), f, new SolidBrush(Color.Gray), bufferedPanel3.Width / 2 - 62,
                    bufferedPanel3.Height / 2 - 15);
            }


            // Resize panel
            bufferedPanel1.AutoScrollMinSize = new Size(bufferedPanel3.Width,
                start_height + height_offset + row_count * data_height);

            // Force resize only if too big
            if (start_height + height_offset + row_count * data_height > bufferedPanel1.Height)
                bufferedPanel3.Height = new Size(bufferedPanel1.Width,
                    start_height + height_offset + row_count * data_height).Height;

            // Dispose all objects
            p.Dispose();
            WritingBrush.Dispose();
            f.Dispose();
            f_italic.Dispose();

            repaintButtons = false;
        }

        private void dynamicButtonClick(object sender, EventArgs e)
        {
            Button b = (Button) sender;
            if (b.Name.StartsWith("O") && b.Name.Length == 10)
            {
                Grey_Out();
                Receipt_Report RP = new Receipt_Report(parent, parent.Order_List.First(x => x.OrderID == b.Name.Substring(1)), new Point(this.Left + 300, this.Top + 40), null, true, this.Location, this.Size);
                RP.ShowDialog();
                Grey_In();

                repaintButtons = true;
                bufferedPanel3.Invalidate();
            }
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public CashHistory(Receipt _parent, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
        }

        private bool formLoaded = false;

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);

            fromDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            toDate.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            bufferedPanel3.Paint += new PaintEventHandler(panel1_Paint);
            bufferedPanel3.Invalidate();

            fromDate.TextChanged += (o, ex) => bufferedPanel3.Invalidate();
            toDate.TextChanged += (o, ex) => bufferedPanel3.Invalidate();

            formLoaded = true;

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
            WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
        {
            Dispose();
            Close();
        }

        public void Set_Form_Color(Color randomColor)
        {
            textBox1.BackColor = randomColor;
            textBox2.BackColor = randomColor;
            textBox3.BackColor = randomColor;
            textBox4.BackColor = randomColor; label5.ForeColor = Color.Silver;
        }

        private void toDate_ValueChanged(object sender, EventArgs e)
        {
            if (!formLoaded) return;

            if (toDate.Value < fromDate.Value)
            {
                toDate.Value = fromDate.Value;
            }
            repaintButtons = true;
            bufferedPanel3.Invalidate();
        }

        private void fromDate_ValueChanged(object sender, EventArgs e)
        {
            if (!formLoaded) return;

            if (toDate.Value < fromDate.Value)
            {
                fromDate.Value = toDate.Value;
            }
            repaintButtons = true;
            bufferedPanel3.Invalidate();
        }
    }
}
