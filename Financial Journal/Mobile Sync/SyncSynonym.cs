using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Org.BouncyCastle.Asn1.Cmp;

namespace Financial_Journal
{
    public partial class SyncSynonym : Form
    {
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            //parent.Background_Save();
            parent.Activate();
            base.OnFormClosing(e);
        }

        Receipt parent;

        private MobileSync.InfoType _refType;
        private string _comparisonItem;

        /// <summary>
        /// Spawn in dead center (dialog convection)
        /// </summary>
        /// <param name="_parent"></param>
        /// <param name="g"></param>
        /// <param name="s"></param>
        public SyncSynonym(Receipt _parent, string comparisonItem, MobileSync.InfoType refType, Point g = new Point(), Size s = new Size())
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            parent = _parent;
            Set_Form_Color(parent.Frame_Color);
            this.Location = new Point(g.X + (s.Width / 2) - (this.Width / 2), g.Y + (s.Height / 2) - (this.Height / 2));
            _refType = refType;
            _comparisonItem = comparisonItem;

            categoryName.Text = _comparisonItem;
        }

        private void Receipt_Load(object sender, EventArgs e)
        {
            // Mousedown anywhere to drag
            //this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form_MouseDown);
            
            switch (_refType)
            {
                case MobileSync.InfoType.Payment:
                {
                    parent.Payment_List.ForEach(x => associateBox.Items.Add(x.ToString()));
                    associateBox.Items.Add("Cash"); associateBox.Items.Add("Other"); 
                    newCategory.Visible = label2.Visible = categoryName.Visible = false; // disable
                    associateCategory.Checked = true;
                    this.Height = 142;
                    label8.Text = String.Format("We could not find '{0}' from your existing {1}. Please add an association:", _comparisonItem, "payments");
                    newCategory.Text = "Create a new payment:";
                    label2.Text = "New payment name";
                    associateCategory.Text = "Create an association between this payment and another payment:";
                    break;
                }
                case MobileSync.InfoType.Category:
                {
                    foreach (string category in parent.category_box.Items)
                    {
                        associateBox.Items.Add(category);
                    }
                    label8.Text = String.Format("We could not find '{0}' from your existing {1}. You can do the following:", _comparisonItem, "categories");
                    break;
                }
                case MobileSync.InfoType.Location:
                {
                    foreach (string location in parent.location_box.Items)
                    {
                        associateBox.Items.Add(location);
                    }
                    label8.Text = String.Format("We could not find '{0}' from your existing {1}. You can do the following:", _comparisonItem, "locations");
                    newCategory.Text = "Create a new location:";
                    label2.Text = "New location name";
                    associateCategory.Text = "Create an association between this location and another location:";
                    break;
                }
            }

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
            this.WindowState = FormWindowState.Minimized;
        }

        private void close_button_Click(object sender, EventArgs e)
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

        private void newCategory_CheckedChanged(object sender, EventArgs e)
        {
            associateCategory.CheckedChanged -= associateCategory_CheckedChanged;
            newCategory.CheckedChanged -= newCategory_CheckedChanged;

            associateBox.Enabled = false;
            if (newCategory.Checked)
            {categoryName.Enabled = true;
                associateCategory.Checked = false;
            }
            associateCategory.CheckedChanged += associateCategory_CheckedChanged;
            newCategory.CheckedChanged += newCategory_CheckedChanged;
        }

        private void associateCategory_CheckedChanged(object sender, EventArgs e)
        {
            associateCategory.CheckedChanged -= associateCategory_CheckedChanged;
            newCategory.CheckedChanged -= newCategory_CheckedChanged;

            categoryName.Enabled = false;
            if (associateCategory.Checked)
            {associateBox.Enabled = true;
                newCategory.Checked = false;
            }

            associateCategory.CheckedChanged += associateCategory_CheckedChanged;
            newCategory.CheckedChanged += newCategory_CheckedChanged;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (associateCategory.Checked || newCategory.Checked)
            {
                bool error = false;
                if (associateCategory.Checked)
                {
                    parent.AssociationList.Add(new Association()
                    {
                        InfoType = _refType,
                        LinkSource = _comparisonItem,
                        LinkDestination = associateBox.Text
                    });
                }
                else // if create new infotype;
                {
                    string errorMsg = "";
                    if (categoryName.Text.Length > 0)
                    {
                        switch (_refType)
                        {
                            case MobileSync.InfoType.Payment:
                            {
                                // Cannot add new payment type; can only associate to existing
                                break;
                            }
                            case MobileSync.InfoType.Category:
                            {
                                foreach (string category in parent.category_box.Items)
                                {
                                    if (category.ToLower() == categoryName.Text.ToLower())
                                    {
                                        error = true;
                                    }
                                }
                                if (!error)
                                {
                                    parent.category_box.Items.Add(categoryName.Text);
                                }
                                break;
                            }
                            case MobileSync.InfoType.Location:
                            {
                                if (parent.Location_List
                                    .Any(x => x.Name.ToLower() == categoryName.Text.ToLower())) // has existing
                                    error = true;
                                else
                                {
                                    parent.Location_List.Add(new Location() { Name = categoryName.Text, Refund_Days = 0});
                                    parent.location_box.Items.Add(categoryName.Text);
                                }
                                break;
                            }

                        }

                        if (error)
                        {
                            errorMsg = "Error. You already have a " + _refType.ToString().ToLower() +
                                       " with this name";
                        }
                        else if (_refType != MobileSync.InfoType.Payment)
                        {
                            parent.AssociationList.Add(new Association()
                            {
                                InfoType = _refType,
                                LinkSource = _comparisonItem,
                                LinkDestination = categoryName.Text
                            });
                        }
                    }
                    else
                    {
                        errorMsg = "Error. Missing " + _refType.ToString().ToLower() + " name";
                    }

                    if (errorMsg.Length > 0)
                    {
                        Grey_Out();
                        Form_Message_Box FMB =
                            new Form_Message_Box(parent, errorMsg, true, 0, this.Location, this.Size);
                        FMB.ShowDialog();
                        Grey_In();
                    }
                }

                if (!error)
                {
                    Close();
                }
            }
        }

        private void associateBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            associateCategory.Checked = true;
        }

        private void categoryName_TextChanged(object sender, EventArgs e)
        {
            newCategory.Checked = true;
        }
    }
}
