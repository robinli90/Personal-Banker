using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using PdfSharp.Drawing;
using PdfSharp.Forms;
using PdfSharp.Pdf;
using PdfSharp.Fonts;
using PdfSharp.Internal;
using PdfSharp.Charting;
using PdfSharp.SharpZipLib;
using PdfSharp.Drawing.Pdf;
using PdfSharp.Drawing.Layout;

namespace Financial_Journal
{
    public class PDFGenerator_Budget
    {
        private BudgetAllocation parent;

        int pageCount = 1;

        public int page_width = 586;
        public int extension_col = 586 - 120;
        public int itemchg_col = 586 - 200;
        public int page_footer_boundary = 760;
        PdfDocument document = new PdfDocument();

        private static int col1 = 22;
        private static int col2 = col1 + 235;
        private static int col3 = col2 + 105;
        private static int col4 = col3 + 85;
        private static int col5 = col4 + 85;

        int start_height = 100;
        int height_offset = 1;
        int row_count = 0;

        private int index = 0;

        PdfPage page;

        public PDFGenerator_Budget(BudgetAllocation parent_)
        {
            index = 0;
            int data_height = 15;

            parent = parent_;

            string fileName = String.Format("Budget_Allocation_Report ({0})", parent_.budgetMonth.Text);

            document.Info.Author = "Personal Banker";
            document.Info.Subject = "Budget Allocation Report";
            document.Info.Title = "Budget Allocation Report";

            page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XFont font = new XFont("MS Reference Sans Serif", 10, XFontStyle.Bold);
            XFont fontreg = new XFont("MS Reference Sans Serif", 10, XFontStyle.Regular);
            XFont smallfont = new XFont("MS Reference Sans Serif", 7, XFontStyle.Bold);
            XFont smallfontreg = new XFont("MS Reference Sans Serif", 7, XFontStyle.Regular);
            XFont bigfontbold = new XFont("MS Reference Sans Serif", 25, XFontStyle.Bold);
            XTextFormatter tf = new XTextFormatter(gfx);

            DrawHeader(gfx, page);


            while (index < parent.dataGridView1.Rows.Count)
            {
                string row1 = parent.dataGridView1[0, index].Value.ToString();
                string row2 = parent.dataGridView1[1, index].Value.ToString();
                string row3 = parent.dataGridView1[2, index].Value.ToString();
                string row4 = parent.dataGridView1[3, index].Value.ToString();
                string row5 = parent.dataGridView1[4, index].Value.ToString();
                string row6 = parent.dataGridView1[5, index].Value.ToString();


                if (row6.Contains("HeaderRowHere"))
                {
                    DrawRow(ref gfx, page, start_height + height_offset + (row_count++ * data_height), row1, row2, row3,
                        row4, row5, true);
                    // extra lines
                }
                else
                {

                    DrawRow(ref gfx, page, start_height + height_offset + (row_count++ * data_height), row1, row2, row3,
                        row4, row5);
                }
                index++;
            }



            //DrawRow(ref gfx, page, start_height + height_offset + (row_count++ * data_height), s.DieType + " Die (" + Math.Round(s.Diameter * (parent.isMetric ? 25.4 : 1), 2) + unit + " X " + Math.Round(s.Thickness * (parent.isMetric ? 25.4 : 1), 2) + unit + ") - " + s.HoleCount + " Cavit" + (s.HoleCount > 1 ? "ies" : "y"), needsCallQuote ? CFQ : (Ref_Base.Price + s.CavityPrice).ToString(), needsCallQuote ? CFQ : (s.BasePrice).ToString(), false, !needsCallQuote);

            #region Summation line
            row_count++;
            //DrawTotal(ref gfx, page, start_height + height_offset + (row_count++ * data_height), Ref_Order, surcharge_total);
            #endregion

            document.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName + ".pdf"));

            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName + ".pdf"));
        }

        private XGraphics NextPage(ref XGraphics gfx, PdfDocument document)
        {
            PdfPage newPage = document.AddPage();
            gfx = XGraphics.FromPdfPage(newPage);
            DrawHeader(gfx, newPage, true);
            return gfx;
        }

        private void DrawRow(ref XGraphics gfx, PdfPage page, int height, string row1, string row2, string row3,
            string row4, string row5, bool isHeader = false)
        {

            if (isHeader)
            {
                height += 10; // more space between headers
                height_offset += 10;
            }

            XFont font = new XFont("MS Reference Sans Serif", 10, XFontStyle.Regular);
            XFont fontreg = new XFont("MS Reference Sans Serif", 10, XFontStyle.Regular);
            XFont smallfont = new XFont("MS Reference Sans Serif", 7, XFontStyle.Bold);
            XFont smallfontreg = new XFont("MS Reference Sans Serif", 7, XFontStyle.Regular);
            XFont bigfontbold = new XFont("MS Reference Sans Serif", 25, XFontStyle.Bold);
            XTextFormatter tf = new XTextFormatter(gfx);
            XRect rect;

            if (ifNewPage(height))
            {
                rect = new XRect(page_width - 150, page_footer_boundary - 10, 150, 8);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Right;
                tf.DrawString("Report continues on next page...", smallfontreg, XBrushes.Black, rect, XStringFormats.TopLeft);

                row_count = 0;
                height_offset = 0;
                NextPage(ref gfx, document);
            }

            rect = new XRect(col1, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(row1, font, XBrushes.Black, rect, XStringFormats.TopLeft);

            rect = new XRect(col2, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(row2, font, XBrushes.Black, rect, XStringFormats.TopLeft);

            rect = new XRect(col3, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(row3, font, XBrushes.Black, rect, XStringFormats.TopLeft);

            rect = new XRect(col4, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(row4, font, XBrushes.Black, rect, XStringFormats.TopLeft);

            rect = new XRect(col5 - (row5.Contains("-$") ? 5 : 0), height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(row5, font, (row5.Contains("-$") ? XBrushes.Red : (row5.Contains("$0") ? XBrushes.Black : XBrushes.Green)), rect, XStringFormats.TopLeft);

            // if is header, draw line beneath header line
            if (isHeader)
            {
                gfx.DrawLine(XPens.Black, col1, height + 15, page_width, height + 15);
                height_offset += 1;
            }

        }

        private bool ifNewPage(int curHeight, int heightBuffer = 55)
        {
            if (curHeight >= (page_footer_boundary - heightBuffer))
            {
                return true;
            }
            else
            {
                return false;;
            }
        }

        void DrawImage(XGraphics gfx, Image jpegSamplePath, int x, int y, int width, int height)
        {
            XImage image = XImage.FromGdiPlusImage(jpegSamplePath);
            gfx.DrawImage(image, x, y, width, height);
        }

        private void DrawHeader(XGraphics gfx, PdfPage page, bool ignoreMainHeader = false)
        {
            XFont font = new XFont("MS Reference Sans Serif", 10, XFontStyle.Bold);
            XFont fontreg = new XFont("MS Reference Sans Serif", 10, XFontStyle.Regular);
            XFont smallfont = new XFont("MS Reference Sans Serif", 7, XFontStyle.Bold);
            XFont smallfontreg = new XFont("MS Reference Sans Serif", 7, XFontStyle.Regular);
            XFont bigfontbold = new XFont("MS Reference Sans Serif", 25, XFontStyle.Bold);
            XTextFormatter tf = new XTextFormatter(gfx);

            #region Standard Header
            //XRect rect = new XRect(20, 20, 100, 100);
            //DrawImage(gfx, global::ExcoPricingTool.Properties.Resources.EXCO, 22, 20, 85, 25);
            //gfx.DrawRectangle(XBrushes.White, rect);
            //tf.DrawString("EXCO", bigfontbold, XBrushes.Black, rect, XStringFormats.TopLeft);

            XRect rect = new XRect(col1, 20, 500, 100);

            if (!ignoreMainHeader)
            {

                gfx.DrawRectangle(XBrushes.White, rect);
                tf.DrawString(String.Format("BUDGET ALLOCATION REPORT ({0})", parent.budgetMonth.Text.ToUpper()), font,
                    XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(col1, 35, 300, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString(String.Format("Total amount available for budgetting: {0}", parent.line1.Text), fontreg,
                    XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(col1, 49, 300, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString(String.Format("Total amount budgetted: {0}", parent.line2.Text), fontreg, XBrushes.Black,
                    rect, XStringFormats.TopLeft);

                rect = new XRect(col1, 63, 300, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString(String.Format("Current budget vs. total amount: {0}", parent.line3.Text), fontreg,
                    XBrushes.Black, rect, XStringFormats.TopLeft);

                // primary header
                gfx.DrawLine(XPens.Black, 22, 33, page_width, 33);

                // secondary header
                gfx.DrawLine(XPens.Black, 22, 78, page_width, 78);
            }
            else
            {
                start_height = 50;
            }

            rect = new XRect(col2, (ignoreMainHeader ? 20 : 83), 300, 300);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString("Prev. Actual", font, XBrushes.Black, rect, XStringFormats.TopLeft);

            rect = new XRect(col3, (ignoreMainHeader ? 20 : 83), 300, 100);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString("Budget", font, XBrushes.Black, rect, XStringFormats.TopLeft);

            rect = new XRect(col4, (ignoreMainHeader ? 20 : 83), 300, 100);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString("Actual", font, XBrushes.Black, rect, XStringFormats.TopLeft);

            rect = new XRect(col5, (ignoreMainHeader ? 20 : 83), 300, 100);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString("Balance", font, XBrushes.Black, rect, XStringFormats.TopLeft);
            #endregion

            #region Footer
            rect = new XRect(page_width - 46, page_footer_boundary + 3, 86, 50);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.DrawString("Page " + pageCount++, smallfontreg, XBrushes.Black, rect, XStringFormats.TopLeft);
            #endregion
            
            // footer line
            gfx.DrawLine(XPens.Black, 22, page_footer_boundary, page_width, page_footer_boundary);
        }

        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;

            System.Reflection.PropertyInfo aProp =
                typeof(System.Windows.Forms.Control).GetProperty(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance);

            aProp.SetValue(c, true, null);
        }
    }
}

