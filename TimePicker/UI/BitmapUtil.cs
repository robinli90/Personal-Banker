using System;
using System.Drawing;
using System.Windows.Forms;

namespace Opulos.Core.UI {

public enum DrawMethod {
	Graphics,
	TextRenderer
}

public static class BitmapUtil {

	// returns a rectangle because it's possible that the top,left are outside of the 0,0 requested position.
	public static Bitmap CreateIcon(String text, Font font, Color? foreColor = null, DrawMethod drawMethod = DrawMethod.Graphics, bool antialias = true) {
		if (String.IsNullOrEmpty(text))
			return new Bitmap(1, 1);

		if (foreColor == null)
			foreColor = Color.Black;

		Bitmap bm = new Bitmap(1,1);
		Graphics graphics = Graphics.FromImage(bm);
		graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
		Size size = (drawMethod == DrawMethod.Graphics ? graphics.MeasureString(text, font).ToSize() : TextRenderer.MeasureText(text, font));
		int w = size.Width;
		int h = size.Height;
		if (w == 0 || h == 0) return new Bitmap(1, 1);
		Bitmap bitmap = new Bitmap(w, h, graphics);
		bm.Dispose();

		Graphics g2 = Graphics.FromImage(bitmap);
		if (antialias)
			g2.TextRenderingHint = graphics.TextRenderingHint;
		if (drawMethod == DrawMethod.Graphics) {
			using (var b = new SolidBrush(foreColor.Value))
				g2.DrawString(text, font, b, 0, 0);
		}
		else
			TextRenderer.DrawText(g2, text, font, new Point(0, 0), foreColor.Value, Color.White);

		int left, right, top, bottom;
		left = right = top = bottom = -1;

		// scanning saves about 50-60% of having to scan the entire image

		for (int i = 0; i < w; i++) {
			for (int j = 0; j < h; j++) {
				Color c = bitmap.GetPixel(i, j);
				if (c.A != 0 || c.R != 0 || c.G != 0 || c.B != 0) {
					left = i;
					break;
				}
			}
			if (left >= 0) break;
		}

		if (left == -1)
			return new Bitmap(1, 1);

		for (int i = w - 1; i >= 0; i--) {
			for (int j = 0; j < h; j++) {
				Color c = bitmap.GetPixel(i, j);
				if (c.A != 0 || c.R != 0 || c.G != 0 || c.B != 0) {
					right = i;
					break;
				}
			}
			if (right >= 0) break;
		}

		for (int j = 0; j < h; j++) {
			for (int i = left; i <= right; i++) {
				Color c = bitmap.GetPixel(i, j);
				if (c.A != 0 || c.R != 0 || c.G != 0 || c.B != 0) {
					top = j;
					break;
				}
			}
			if (top >= 0) break;
		}

		for (int j = h - 1; j >= 0; j--) {
			for (int i = left; i <= right; i++) {
				Color c = bitmap.GetPixel(i, j);
				if (c.A != 0 || c.R != 0 || c.G != 0 || c.B != 0) {
					bottom = j;
					break;
				}
			}
			if (bottom >= 0) break;
		}

		var r = new Rectangle(left, top, (right - left) + 1, (bottom - top) + 1);
		//g2.DrawRectangle(Pens.Red, r.X, r.Y, r.Width - 1, r.Height - 1);
		Bitmap b2 = bitmap.Clone(r, bitmap.PixelFormat);
		bitmap.Dispose();
		graphics.Dispose();
		g2.Dispose();
		return b2;
	}
}
}