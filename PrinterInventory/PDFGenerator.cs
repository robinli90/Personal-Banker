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

namespace PrinterInventory
{
    public enum EntryType
    {
        Receive,
        Remove
    }

    public enum ReportType
    {
        Inventory,
        Usage
    }

    public class PDFGenerator
    {

        Main parent;

        int pageCount = 1;

        // Date culture
        System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

        public int page_width = 586;
        public int start_width = 22;
        public int extension_col = 586 - 120;
        public int itemchg_col = 586 - 200;
        public int page_footer_boundary = 750;
        PdfDocument document = new PdfDocument();
        
        int height_offset = 1;
        int row_count = 0;
        PdfPage page;

        public PDFGenerator(Main parent_, ReportType reportType, bool showZeroInventory = false)
        {
            string fileName = "";

            if (reportType == ReportType.Usage)
            {
                int start_height = 88;

                #region Usage report
                parent = parent_;

                int data_height = 8;

                fileName = "EXCO_Cartridge_Usage_Report (" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" +
                                  DateTime.Now.Day + "_" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" +
                                  DateTime.Now.Second + DateTime.Now.Millisecond + ")";

                document.Info.Author = "EXCO";
                document.Info.Subject = "Cartridge Usage Report";
                document.Info.Title = "Exco Cartridge Usage Report";

                page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("MS Reference Sans Serif", 10, XFontStyle.Bold);
                XFont fontreg = new XFont("MS Reference Sans Serif", 10, XFontStyle.Regular);
                XFont smallfont = new XFont("MS Reference Sans Serif", 7, XFontStyle.Bold);
                XFont smallfontreg = new XFont("MS Reference Sans Serif", 7, XFontStyle.Regular);
                XFont bigfontbold = new XFont("MS Reference Sans Serif", 25, XFontStyle.Bold);
                XTextFormatter tf = new XTextFormatter(gfx);

                DrawHeader(gfx, page, reportType);

                List<InternalPrintCartridge> IPCList = new List<InternalPrintCartridge>();

                foreach (Cartridge c in parent.EntireCartridgeList)
                {
                    IPCList.Add(new InternalPrintCartridge()
                    {
                        Brand = c.Brand,
                        EntryDate = c.ReceiveDate,
                        EntryType = EntryType.Receive,
                        Memo = c.Memo,
                        Model = c.Model,
                        Price = c.Price,
                        RemoveNote = "",
                        Requisitioner = c.Requisitioner
                    });

                    if (c.RemoveDate.Year > 1801)
                    {
                        IPCList.Add(new InternalPrintCartridge()
                        {
                            Brand = c.Brand,
                            EntryDate = c.RemoveDate,
                            EntryType = EntryType.Remove,
                            Memo = c.Memo,
                            Model = c.Model,
                            Price = 0,
                            RemoveNote = c.RemoveMemo,
                            Requisitioner = ""
                        });
                    }
                }

                #region Print Reports

                foreach (InternalPrintCartridge IPC in IPCList.OrderByDescending(x => x.EntryDate).ToList())
                {
                    DrawRowUsage(ref gfx, page, start_height + height_offset + (row_count++ * data_height), IPC.Brand,
                        IPC.Model, IPC.Memo, IPC.Price, IPC.EntryDate, IPC.RemoveNote, IPC.Requisitioner,
                        IPC.Requisitioner == "" ? EntryType.Remove : EntryType.Receive);

                    if (ifNewPage(start_height + height_offset + (row_count++ * data_height)))
                    {
                        row_count = 0;
                        height_offset = 0;
                        NextPage(ref gfx, document);
                    }
                }

                #endregion
                #endregion
            }
            else if (reportType == ReportType.Inventory)
            {
                int start_height = 88;

                #region Inventory report
                parent = parent_;

                int data_height = 8;

                fileName = "EXCO_Cartridge_Inventory_Report (" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" +
                           DateTime.Now.Day + "_" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" +
                           DateTime.Now.Second + DateTime.Now.Millisecond + ")";

                document.Info.Author = "EXCO";
                document.Info.Subject = "Cartridge Inventory Report";
                document.Info.Title = "Exco Cartridge Inventory Report";

                page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);
                XFont font = new XFont("MS Reference Sans Serif", 10, XFontStyle.Bold);
                XFont fontreg = new XFont("MS Reference Sans Serif", 10, XFontStyle.Regular);
                XFont smallfont = new XFont("MS Reference Sans Serif", 7, XFontStyle.Bold);
                XFont smallfontreg = new XFont("MS Reference Sans Serif", 7, XFontStyle.Regular);
                XFont bigfontbold = new XFont("MS Reference Sans Serif", 25, XFontStyle.Bold);
                XTextFormatter tf = new XTextFormatter(gfx);

                DrawHeader(gfx, page, reportType);

                List<InternalPrintCartridge> IPCList = new List<InternalPrintCartridge>();

                foreach (Cartridge c in parent.CartridgeList.Where(x => x.Quantity > (showZeroInventory ? -1 : 0)))
                {
                    List<Cartridge> refList = parent.GetMasterCartridgesList(c).Where(x => x.Price > 0).ToList();

                    double avgPrice = refList.Sum(x => x.Price) / refList.Count;

                    DrawRowInventory(ref gfx, page, start_height + height_offset + (row_count++ * data_height),
                        c.Brand, c.Model, c.Memo, avgPrice, c.Quantity, c.ReceiveDate, c.Requisitioner);
                        

                    if (ifNewPage(start_height + height_offset + (row_count++ * data_height)))
                    {
                        row_count = 0;
                        height_offset = 0;
                        NextPage(ref gfx, document);
                    }
                }

                #endregion
            }

            document.Save(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName + ".pdf"));

            Process.Start(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName + ".pdf"));
        }

        private XGraphics NextPage(ref XGraphics gfx, PdfDocument document)
        {
            PdfPage newPage = document.AddPage();
            gfx = XGraphics.FromPdfPage(newPage);
            DrawHeader(gfx, newPage);
            return gfx;
        }

        private void DrawRowUsage(ref XGraphics gfx, PdfPage page, int height, string brand, string model, string memo, double price, DateTime entryDate, string removeNote, string requisitioner, EntryType entryType)
        {
            XFont font = new XFont("MS Reference Sans Serif", 9, XFontStyle.Regular);
            XFont fontreg = new XFont("MS Reference Sans Serif", 10, XFontStyle.Regular);
            XFont smallfont = new XFont("MS Reference Sans Serif", 7, XFontStyle.Bold);
            XFont smallfontreg = new XFont("MS Reference Sans Serif", 7, XFontStyle.Regular);
            XFont bigfontbold = new XFont("MS Reference Sans Serif", 25, XFontStyle.Bold);
            XTextFormatter tf = new XTextFormatter(gfx);
            XRect rect;

            if (ifNewPage(height))
            {
                rect = new XRect(page_width - 150, page_footer_boundary - 19, 150, 8);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Right;
                tf.DrawString("Report continues on next page...", smallfontreg, XBrushes.Black, rect, XStringFormats.TopLeft);

                row_count = 0;
                height_offset = 0;
                NextPage(ref gfx, document);
            }

            XBrush refBrush = entryType == EntryType.Receive ? XBrushes.Black : XBrushes.Red;

            rect = new XRect(start_width, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(entryType.ToString(), font, refBrush, rect, XStringFormats.TopLeft);

            rect = new XRect(start_width + 70, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(entryDate.ToString(), font, refBrush, rect, XStringFormats.TopLeft);

            rect = new XRect(start_width + 205, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(model + " (" + brand + ")", font, refBrush, rect, XStringFormats.TopLeft);

            rect = new XRect(start_width + 320, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(memo, font, refBrush, rect, XStringFormats.TopLeft);

            rect = new XRect(start_width + 410, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(price > 0 ? "$" + String.Format("{0:0.00}", price) : "     -", font, refBrush, rect,
                XStringFormats.TopLeft);

            rect = new XRect(start_width + 485, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(requisitioner.Length > 0 ? "Req: " + requisitioner : removeNote, font, refBrush, rect,
                XStringFormats.TopLeft);
        }
 
        private void DrawRowInventory(ref XGraphics gfx, PdfPage page, int height, string brand, string model, string memo, double price, int quantity, DateTime entryDate, string requisitioner)
        {
            XFont font = new XFont("MS Reference Sans Serif", 9, XFontStyle.Regular);
            XFont fontreg = new XFont("MS Reference Sans Serif", 10, XFontStyle.Regular);
            XFont smallfont = new XFont("MS Reference Sans Serif", 7, XFontStyle.Bold);
            XFont smallfontreg = new XFont("MS Reference Sans Serif", 7, XFontStyle.Regular);
            XFont bigfontbold = new XFont("MS Reference Sans Serif", 25, XFontStyle.Bold);
            XTextFormatter tf = new XTextFormatter(gfx);
            XRect rect;

            if (ifNewPage(height))
            {
                rect = new XRect(page_width - 150, page_footer_boundary - 19, 150, 8);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Right;
                tf.DrawString("Report continues on next page...", smallfontreg, XBrushes.Black, rect, XStringFormats.TopLeft);

                row_count = 0;
                height_offset = 0;
                NextPage(ref gfx, document);
            }

            XBrush refBrush = XBrushes.Black;

            rect = new XRect(start_width, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(entryDate.ToString(), font, refBrush, rect, XStringFormats.TopLeft);

            rect = new XRect(start_width + 140, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(model + " (" + brand + ")", font, refBrush, rect, XStringFormats.TopLeft);

            rect = new XRect(start_width + 260, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(memo, font, refBrush, rect, XStringFormats.TopLeft);

            rect = new XRect(start_width + 375, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(quantity.ToString(), font, refBrush, rect, XStringFormats.TopLeft);

            rect = new XRect(start_width + 415, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(price > 0 ? "$" + String.Format("{0:0.00}", price) : "     -", font, refBrush, rect,
                XStringFormats.TopLeft);

            rect = new XRect(start_width + 490, height, 400, 18);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(requisitioner, font, refBrush, rect,
                XStringFormats.TopLeft);
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

        private void DrawHeader(XGraphics gfx, PdfPage page, ReportType reportType=ReportType.Usage)
        {
            XFont font = new XFont("MS Reference Sans Serif", 10, XFontStyle.Bold);
            XFont fontreg = new XFont("MS Reference Sans Serif", 9, XFontStyle.Bold);
            XFont smallfont = new XFont("MS Reference Sans Serif", 7, XFontStyle.Bold);
            XFont smallfontreg = new XFont("MS Reference Sans Serif", 7, XFontStyle.Regular);
            XFont bigfontbold = new XFont("MS Reference Sans Serif", 25, XFontStyle.Bold);
            XTextFormatter tf = new XTextFormatter(gfx);

            #region Standard Header
            XRect rect = new XRect(20, 20, 100, 100);
            DrawImage(gfx, global::PrinterInventory.Properties.Resources.EXCO, 22, 20, 85, 25);
            //gfx.DrawRectangle(XBrushes.White, rect);
            //tf.DrawString("EXCO", bigfontbold, XBrushes.Black, rect, XStringFormats.TopLeft);

            rect = new XRect(start_width, 48, 150, 100);
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.DrawString("EXCO TOOLING SOLUTIONS", smallfont, XBrushes.Black, rect, XStringFormats.TopLeft);

            #endregion
            
            if (reportType == ReportType.Usage)
            {
                rect = new XRect(page_width - 176, 45, 350, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.DrawString("PRINTER CARTRIDGE USAGE REPORT", fontreg, XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(start_width, 70, 300, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString("Action", font, XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(start_width + 70, 70, 300, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString("Date", font, XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(start_width + 205, 70, 300, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString("Model (Brand)", font, XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(start_width + 320, 70, 100, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString("Memo", font, XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(start_width + 410, 70, 100, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString("Price(ea)", font, XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(start_width + 485, 70, 100, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString("Other note", font, XBrushes.Black, rect, XStringFormats.TopLeft);
            }
            else if (reportType == ReportType.Inventory)
            {
                rect = new XRect(page_width - 196, 45, 350, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.DrawString("PRINTER CARTRIDGE INVENTORY REPORT", fontreg, XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(start_width, 70, 300, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.DrawString("Last Receive Date", font, XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(start_width + 140, 70, 300, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.DrawString("Model (Brand)", font, XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(start_width + 260, 70, 100, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString("Memo", font, XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(start_width + 370, 70, 100, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString("Qty", font, XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(start_width + 410, 70, 100, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString("Price(avg)", font, XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(start_width + 490, 70, 100, 100);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Left;
                tf.DrawString("Requisitor", font, XBrushes.Black, rect, XStringFormats.TopLeft);
            }

            #region Footer
            rect = new XRect(page_width - 46, page_footer_boundary + 3, 40, 50);
            tf.Alignment = XParagraphAlignment.Right;
            gfx.DrawRectangle(XBrushes.White, rect);
            tf.DrawString("Page " + pageCount++, smallfontreg, XBrushes.Black, rect, XStringFormats.TopLeft);

                rect = new XRect(start_width, page_footer_boundary + 3, 300, 10);
                gfx.DrawRectangle(XBrushes.White, rect);
                tf.Alignment = XParagraphAlignment.Left;
            if (reportType == ReportType.Usage)
            {
                tf.DrawString("Entire usage report", smallfontreg, XBrushes.Black, rect, XStringFormats.TopLeft);
            }
            else if (reportType == ReportType.Inventory)
            {
                tf.DrawString("Entire inventory report", smallfontreg, XBrushes.Black, rect, XStringFormats.TopLeft);
            }
            #endregion

            XPen g = new XPen(Color.Black, 0.5);

            // primary header
            gfx.DrawLine(XPens.Black, 22, 58, page_width, 58);

            // footer line
            gfx.DrawLine(XPens.Black, 22, page_footer_boundary, page_width, page_footer_boundary);
        }

        double MetricFactor = 1;


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

    public class InternalPrintCartridge
    {
        public double Price { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Memo { get; set; }
        public DateTime EntryDate { get; set; }
        public string RemoveNote { get; set; }
        public string Requisitioner { get; set; }
        public EntryType EntryType { get; set; }
    }
}
