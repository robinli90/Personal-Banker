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
    public partial class Add_Asset : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            parent.Activate();
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

        Receipt parent;
        private string Asset_Category_;
        Asset_Item Ref_AI;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public Add_Asset(Receipt _parent, string Category, Asset_Item Ref_Asset_Item = null, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            Asset_Category_ = Category;
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));

            Ref_AI = Ref_Asset_Item;
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
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

            string[] arr = SuggestStrings("");
            AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
            collection.AddRange(arr);
            itemlocation.AutoCompleteCustomSource = collection;
            itemlocation.AutoCompleteSource = AutoCompleteSource.CustomSource;

            label5.Text += " to " + Asset_Category_;

            dateTimePicker1.Value = DateTime.Now;

            // If edit
            if (Ref_AI != null)
            {
                itemname.Text = Ref_AI.Name;
                itemlocation.Text = Ref_AI.Purchase_Location;
                item_price.Text = "$" + Ref_AI.Cost;
                dateTimePicker1.Value = Ref_AI.Purchase_Date;
                itemserial.Text = Ref_AI.Serial_Identification;
                itemnotes.Text = Ref_AI.Note;

                button3.Image = global::Financial_Journal.Properties.Resources.save;
                label28.Text = "SAVE ITEM";
                label28.Left -= 2;

                if (Ref_AI.Remove_Date.Year > 1801) // If sold or disposed
                {
                    if (Ref_AI.Selling_Amount > 0)
                    {
                        label10.Text = "Item has been sold for $" + String.Format("{0:0.00}", Ref_AI.Selling_Amount);
                        label11.Text = "on " + Ref_AI.Remove_Date.ToShortDateString();
                    }
                    else
                    {
                        label10.Text = "Item has been disposed/given away";
                        label11.Text = "on " + Ref_AI.Remove_Date.ToShortDateString();
                    }

                    foreach (Control c in this.Controls)
                    {
                        if (c is TextBox)
                        {
                            c.Enabled = false;
                        }
                    }

                    dateTimePicker1.Enabled = false;
                }

                // If import, disallow editing of certain fields
                if (Ref_AI.OrderID.Length > 0)
                {
                    item_price.Enabled = false;
                    itemlocation.Enabled = false;
                    dateTimePicker1.Enabled = false;
                    itemname.Enabled = false;

                }

                dateTimePicker1.Value = Ref_AI.Purchase_Date;
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

        private void close_button_Click_1(object sender, EventArgs e)
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

        private void button3_Click(object sender, EventArgs e)
        {
            if (itemname.Text.Length > 0 && item_price.Text.Length > 1 && itemlocation.Text.Length > 0)
            {
                // If not edit
                if (Ref_AI == null)
                {
                    Asset_Item AI = new Asset_Item()
                    {
                        Name = itemname.Text,
                        Purchase_Date = dateTimePicker1.Value,
                        Cost = Convert.ToDouble(item_price.Text.Substring(1)),
                        Serial_Identification = itemserial.Text,
                        Note = itemnotes.Text,
                        Asset_Category = Asset_Category_,
                        Purchase_Location = itemlocation.Text
                    };
                    parent.Asset_List.Add(AI);
                }
                else // if idit
                {
                    Asset_Item AI = new Asset_Item()
                    {
                        Name = itemname.Text,
                        Purchase_Date = dateTimePicker1.Value,
                        Cost = Convert.ToDouble(item_price.Text.Substring(1)),
                        Serial_Identification = itemserial.Text,
                        Note = itemnotes.Text,
                        Asset_Category = Asset_Category_,
                        Purchase_Location = itemlocation.Text,
                        Remove_Date = Ref_AI.Remove_Date,
                        OrderID = Ref_AI.OrderID,
                        Selling_Amount = Ref_AI.Selling_Amount
                    };
                    parent.Asset_List.Remove(Ref_AI);
                    parent.Asset_List.Add(AI);
                }
                this.Close();
            }
            else
            {
                Grey_Out();
                Form_Message_Box FMB = new Form_Message_Box(parent, "Missing information", true, 0, this.Location, this.Size);
                FMB.ShowDialog();
                Grey_In();
            }
        }

        private void item_price_TextChanged(object sender, EventArgs e)
        {
            parent.textBox6_TextChanged(sender, e);
        }

        private void itemlocation_TextChanged(object sender, EventArgs e)
        {

        }

        private string[] SuggestStrings(string text)
        {
            return parent.Order_List.Select(x => x.Location).Distinct().ToArray();
        }
    }
}
