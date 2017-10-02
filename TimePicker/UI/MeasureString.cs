using Opulos.Core.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Opulos.Core.UI {

public static class MeasureString {

	private static Hashtable ht = new Hashtable();

	// returns a rectangle because it's possible that the top,left are outside of the 0,0 requested position.
	public static Rectangle Measure(String text, Graphics graphics, Font font, DrawMethod drawMethod = DrawMethod.Graphics, TextFormatFlags textFormatFlags = TextFormatFlags.Default, Rectangle? rect = null) {
		if (String.IsNullOrEmpty(text))
			return Rectangle.Empty;


		MultiKey mk = new MultiKey(text, graphics.TextRenderingHint, font, drawMethod, textFormatFlags, rect);
		//if (drawMethod == DrawMethod.Graphics)
		//	mk = new MultiKey(text, graphics.TextRenderingHint, font, drawMethod);
		//else {
		//	mk = new 

		Object o = ht[mk];
		if (o != null)
			return (Rectangle) o;

		Size size = Size.Empty;
		if (rect.HasValue) {
			var r2 = rect.Value;
			size = r2.Size;
			r2.Location = Point.Empty;
			rect = r2;
		}
		else
			size = (drawMethod == DrawMethod.Graphics ? graphics.MeasureString(text, font).ToSize() : TextRenderer.MeasureText(graphics, text, font, Size.Empty, textFormatFlags));

		int w = size.Width;
		int h = size.Height;
		if (w == 0 || h == 0) return Rectangle.Empty;
		Bitmap bitmap = new Bitmap(w, h, graphics);

		Graphics g2 = Graphics.FromImage(bitmap);
		g2.TextRenderingHint = graphics.TextRenderingHint;
		g2.SmoothingMode = graphics.SmoothingMode;
		g2.Clear(Color.White);
		if (drawMethod == DrawMethod.Graphics) {
			g2.DrawString(text, font, Brushes.Black, 0, 0);
		}
		else {
			// always specify a bounding rectangle. Otherwise if flags contains VerticalCenter it will be half cutoff above point (0,0)
			Rectangle r2 = (rect.HasValue ? rect.Value : new Rectangle(Point.Empty, size));
			TextRenderer.DrawText(g2, text, font, r2, Color.Black, Color.White, textFormatFlags);
		}

		int left, right, top, bottom;
		left = right = top = bottom = -1;

		// scanning saves about 50-60% of having to scan the entire image

		for (int i = 0; i < w; i++) {
			for (int j = 0; j < h; j++) {
				Color c = bitmap.GetPixel(i, j);
				if (c.R != 255 || c.G != 255 || c.B != 255) {
					left = i;
					break;
				}
			}
			if (left >= 0) break;
		}

		if (left == -1)
			return Rectangle.Empty;

		for (int i = w - 1; i >= 0; i--) {
			for (int j = 0; j < h; j++) {
				Color c = bitmap.GetPixel(i, j);
				if (c.R != 255 || c.G != 255 || c.B != 255) {
					right = i;
					break;
				}
			}
			if (right >= 0) break;
		}

		for (int j = 0; j < h; j++) {
			for (int i = left; i <= right; i++) {
				Color c = bitmap.GetPixel(i, j);
				if (c.R != 255 || c.G != 255 || c.B != 255) {
					top = j;
					break;
				}
			}
			if (top >= 0) break;
		}

		for (int j = h - 1; j >= 0; j--) {
			for (int i = left; i <= right; i++) {
				Color c = bitmap.GetPixel(i, j);
				if (c.R != 255 || c.G != 255 || c.B != 255) {
					bottom = j;
					break;
				}
			}
			if (bottom >= 0) break;
		}

		g2.Dispose();
		bitmap.Dispose();

		var r = new Rectangle(left, top, (right - left) + 1, (bottom - top) + 1);
		ht[mk] = r;
		return r;
	}
}


}
