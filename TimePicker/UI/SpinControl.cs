using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Opulos.Core.UI {

///<summary>Different visual styles for the up-down buttons.</summary>
public enum SpinButtonStyle {
	Custom = -1,
	Flat = 0,
	Popup = 1,
	Standard = 2,
	System = 3,
	Modern = 10,
}

///<summary>
///The SpinControl consists of two buttons, an up button and a down button. Images are used to for the arrows.
///Images are used instead of text, because the text does not center in the middle of the buttons. The images
///are dynamically generated using the ArrowFont and ArrowUpText, ArrowDownText values. The images are trimmed
///so that they are exact size, making them easy to center.
///</summary>
public class SpinControl : UserControl, IUpDown {

	///<summary>If true, then the width of the buttons and arrows are not dynamically calculated. If false, then
	///the images and button widths are calculated when this control receives a Font Changed event.
	///</summary>
	public bool FixedWidth { get; set; }

	///<summary>If FixedWidth is false, then the width of the up-down arrow images change when the Font Size is changed. The images
	///are rendered using a Font Size that is a fraction of the parent Font Size.</summary>
	public float ArrowFontSizeFactor { get; set; }

	///<summary>For small Fonts, multiplying this control's Font Size by the ArrowFontSizeFactor could produce a font that is too small.
	///Thus, this value specifies the minimum font size used to render the arrows. The default value is 8f.</summary>
	public float ArrowMinimumFontSize { get; set; }

	///<summary>If FixedWidth is false, then the width of the up-down buttons change when the Font Size is changed. The widths of the buttons
	///is determined by the width of the arrow images, multiplied by this factor. The width of the arrow images also change when the
	///Font is changed, and depend on the ArrowFontSizeFactor. The default value for ButtonWidthFactor is 2f, which creates whitespace
	///on either side of the arrow. If this value is 1f, then width of the buttons will equal the width of the arrows (but only after
	///the MinimumSize is reached).</summary>
	public float ButtonWidthFactor { get; set; }

	///<summary>An option to automatically focus on the parent of this control after a button is clicked. The default value is true,
	///which mimics the native up-down control. However, this means it is not possible to stay focused on the buttons. The default
	///buttons also have the TabStop property set to false.</summary>
	public bool FocusParentOnClick { get; set; }

	///<summary>Instead of adding a listener directly to the button, it is better to add the listener to this control (in case the button is changed).</summary>
	public event EventHandler UpClicked;

	///<summary>Instead of adding a listener directly to the button, it is better to add the listener to this control (in case the button is changed).</summary>
	public event EventHandler DownClicked;

	///<summary>The font to use to render the arrow images. The default is Marlett 8pt font.</summary>
	public Font ArrowFont = new Font("Marlett", 8f, FontStyle.Regular);

	///<summary>The text to use to render the up arrow image using the ArrowFont.</summary>
	public String ArrowUpText = "t";

	///<summary>The text to use to render the down arrow image using the ArrowFont.</summary>
	public String ArrowDownText = "u";

	///<summary>The image painted on the up button in the enabled state.</summary>
	public Bitmap ArrowUpImage = null;

	///<summary>The image painted on the down button in the enabled state.</summary>
	public Bitmap ArrowDownImage = null;

	///<summary>The image painted on the up button in the disabled state.</summary>
	public Bitmap ArrowUpImageDisabled = null;

	///<summary>The image painted on the down button in the disabled state.</summary>
	public Bitmap ArrowDownImageDisabled = null;

	///<summary>An option to use have the text appear smoothly when rendered as an image. The default value is true.</summary>
	public bool AntiAliasArrows { get; set; }

	// Button states: 0=Normal 1=Pressed 2=Hot(Mouse Over)
	private int upButtonState = 0;
	private int downButtonState = 0;
	private SpinButtonStyle arrowStyle;
	private Button upButton = null;
	private Button downButton = null;


	public SpinControl() {
		Dock = DockStyle.Right;
		//BackColor = SystemColors.Control;
        BackColor = Color.FromArgb(64, 64, 64);
		Cursor = Cursors.Default;
		Margin = Padding.Empty;
		Padding = Padding.Empty;
		MinimumSize = new Size(16, 0);
		TabStop = false;
		//---
		UpButton = new SpinButton(this, false);
		DownButton = new SpinButton(this, false);
		ArrowMinimumFontSize = 8f;
		AntiAliasArrows = true;
		ArrowFontSizeFactor = 0.4f;
		ButtonWidthFactor = 2.0f;
		ButtonStyle = SpinButtonStyle.Modern;
		FocusParentOnClick = false; // true;
		Width = TextRenderer.MeasureText(ArrowUpText, ArrowFont).Width;
	}

	public Button UpButton {
		get {
			return upButton;
		}
		set {
			if (value == null)
				throw new ArgumentNullException("UpButton");

			if (upButton != null) {
				upButton.Click -= UpButton_Click;
				upButton.Paint -= UpButton_Paint;
				upButton.MouseDown -= UpButton_MouseDown;
				upButton.MouseUp -= UpButton_MouseUp;
				upButton.MouseEnter -= UpButton_MouseEnter;
				upButton.MouseLeave -= UpButton_MouseLeave;
				Controls.Remove(upButton);
			}

			upButton = value;
			upButton.Click += UpButton_Click;
			upButton.Paint += UpButton_Paint;
			upButton.MouseDown += UpButton_MouseDown;
			upButton.MouseUp += UpButton_MouseUp;
			upButton.MouseEnter += UpButton_MouseEnter;
			upButton.MouseLeave += UpButton_MouseLeave;
			Controls.Add(upButton);
			Controls.SetChildIndex(upButton, 0);
		}
	}

	public Button DownButton {
		get {
			return downButton;
		}
		set {
			if (value == null)
				throw new ArgumentNullException("DownButton");

			if (downButton != null) {
				downButton.Click -= DownButton_Click;
				downButton.Paint -= DownButton_Paint;
				downButton.MouseDown -= DownButton_MouseDown;
				downButton.MouseUp -= DownButton_MouseUp;
				downButton.MouseEnter -= DownButton_MouseEnter;
				downButton.MouseLeave -= DownButton_MouseLeave;
				Controls.Remove(downButton);
			}

			downButton = value;
			downButton.Click += DownButton_Click;
			downButton.Paint += DownButton_Paint;
			downButton.MouseDown += DownButton_MouseDown;
			downButton.MouseUp += DownButton_MouseUp;
			downButton.MouseEnter += DownButton_MouseEnter;
			downButton.MouseLeave += DownButton_MouseLeave;
			Controls.Add(downButton);
		}
	}

	private void ResizeButtons() {
		Size s = this.Size;
		int w = s.Width;
		int h = s.Height;
		int h2 = h / 2;

		int uh = h2; // up button height
		int dh = h2; // down button height
		int dy = h2;

		if (h % 2 == 1) {
			// Modern buttons need to be kept the same size
			// otherwise one arrow is drawn larger than the other
			if (ButtonStyle == SpinButtonStyle.Modern)
				dy++;
			else
				dh++;
		}

		// fine tune the some pixels based on the style for best visual representation
		if (UpButton.FlatStyle == FlatStyle.Standard || UpButton.FlatStyle == FlatStyle.System) {
			uh++;
			dh++;
		}
		else if (UpButton.FlatStyle == FlatStyle.Flat || UpButton.FlatStyle == FlatStyle.Popup) {
			dy++; // move the down button down one, otherwise the borders touch
			dh--;
		}

		this.UpButton.Bounds = new Rectangle(0, 0, w, uh);
		this.DownButton.Bounds = new Rectangle(0, dy, w, dh);
	}

	protected void UpdateArrowImages() {
		if (ButtonStyle == SpinButtonStyle.Custom)
			return;

		DisposeImages();
		//ArrowUpImage = BitmapUtil.CreateIcon(ArrowUpText, ArrowFont, antialias:AntiAliasArrows);
		//ArrowDownImage = BitmapUtil.CreateIcon(ArrowDownText, ArrowFont, antialias:AntiAliasArrows);
		//ArrowUpImageDisabled = BitmapUtil.CreateIcon(ArrowUpText, ArrowFont, SystemColors.GrayText, antialias:AntiAliasArrows);
		//ArrowDownImageDisabled = BitmapUtil.CreateIcon(ArrowDownText, ArrowFont, SystemColors.GrayText, antialias:AntiAliasArrows);
	}
    
	protected override void OnParentBackColorChanged(EventArgs e) {
		base.OnParentBackColorChanged(e);
		if (ButtonStyle == SpinButtonStyle.Standard) {
			UpButton.UseVisualStyleBackColor = true;
			DownButton.UseVisualStyleBackColor = true;
		}
	}

	protected override void OnParentChanged(EventArgs e) {
		base.OnParentChanged(e);
		if (ButtonStyle == SpinButtonStyle.Standard) {
			UpButton.UseVisualStyleBackColor = true;
			DownButton.UseVisualStyleBackColor = true;
		}
	}

	protected override void OnFontChanged(EventArgs e) {
		base.OnFontChanged(e);
		// Modern still needs to update the Images so that the size recalculated
		if (!FixedWidth && ButtonStyle != SpinButtonStyle.Custom) {
			Font f = ArrowFont;
			ArrowFont = new Font(f.FontFamily, Math.Max(ArrowMinimumFontSize, ArrowFontSizeFactor * this.Font.Size), f.Style, f.Unit, f.GdiCharSet, f.GdiVerticalFont);
			UpdateArrowImages();
			//int w2 = (int) Math.Round(ButtonWidthFactor * Math.Max(ArrowUpImage.Width, ArrowDownImage.Width));
			//Width = Math.Max(w2, MinimumSize.Width);
			ResizeButtons();
			f.Dispose(); // old arrow font
		}
	}
    
	void UpButton_MouseLeave(object sender, EventArgs e) {
		upButtonState = 0;
	}
	void UpButton_MouseUp(object sender, MouseEventArgs e) {
		upButtonState = 0;
		UpButton.Refresh();
	}
	void UpButton_MouseDown(object sender, MouseEventArgs e) {
		upButtonState = 1;
	}
	void UpButton_MouseEnter(object sender, EventArgs e) {
		upButtonState = 2;
	}
	void DownButton_MouseUp(object sender, MouseEventArgs e) {
		downButtonState = 0;
		DownButton.Refresh();
	}
	void DownButton_MouseLeave(object sender, EventArgs e) {
		downButtonState = 0;
	}
	void DownButton_MouseDown(object sender, MouseEventArgs e) {
		downButtonState = 1;
	}
	void DownButton_MouseEnter(object sender, EventArgs e) {
		downButtonState = 2;
	}
	void UpButton_Click(object sender, EventArgs e) {
		Up();
		if (FocusParentOnClick)
			Parent.Focus();
	}
	void DownButton_Click(object sender, EventArgs e) {
		Down();
		if (FocusParentOnClick)
			Parent.Focus();
	}

	public virtual void Down() {
		if (DownClicked != null)
			DownClicked(this, EventArgs.Empty);
	}

	public virtual void Up() {
		if (UpClicked != null)
			UpClicked(this, EventArgs.Empty);
	}

	protected override void OnResize(EventArgs e) {
		ResizeButtons();
		base.OnResize(e);
	}

	void UpButton_Paint(object sender, PaintEventArgs e) {
		if (ButtonStyle == SpinButtonStyle.Modern) {
			var r = new Rectangle(Point.Empty, UpButton.Size);
			var state = GetState(upButtonState, UpButton);
			DrawModernArrow(e.Graphics, r, state);
		}
		else if (ButtonStyle != SpinButtonStyle.Custom) {
			DrawUpButtonImage(e.Graphics);
		}
	}

	void DownButton_Paint(object sender, PaintEventArgs e) {
		if (ButtonStyle == SpinButtonStyle.Modern) {
			var r = new Rectangle(Point.Empty, DownButton.Size);
			var state = GetState(downButtonState, DownButton);
			DrawModernArrow(e.Graphics, r, state);
		}
		else if (ButtonStyle != SpinButtonStyle.Custom) {
			DrawDownButtonImage(e.Graphics);
		}
	}

	protected void DrawUpButtonImage(Graphics g) {
		DrawUpButtonImage(UpButton, (UpButton.Enabled ? ArrowUpImage : ArrowUpImageDisabled), g);
	}

	protected void DrawDownButtonImage(Graphics g) {
		DrawDownButtonImage(DownButton, (DownButton.Enabled ? ArrowDownImage : ArrowDownImageDisabled), g);
	}

	private static void DrawUpButtonImage(Button upButton, Image upButtonImage, Graphics g) {
		Padding p = upButton.Padding;
		Size s = upButton.Size;
		int aw = s.Width - p.Horizontal;
		int ah = s.Height - p.Vertical;
		var ih = upButtonImage.Size;
		int x = p.Left + (aw - ih.Width) / 2;
		int dy = ah - ih.Height;
		int dy2 = dy/2;
		int y = p.Top + dy2;
		g.DrawImageUnscaled(upButtonImage, x, y);
	}

	private static void DrawDownButtonImage(Button downButton, Image downButtonImage, Graphics g) {
		Padding p = downButton.Padding;
		Size s = downButton.Size;
		int aw = s.Width - p.Horizontal;
		int ah = s.Height - p.Vertical;
		var ih = downButtonImage.Size;
		int x = p.Left + (aw - ih.Width) / 2;
		var dy = (ah - ih.Height);
		int y = p.Top + dy / 2 + (dy % 2 == 1 ? 1 : 0);
		g.DrawImageUnscaled(downButtonImage, x, y);
	}

	private static void DrawModernArrow(Graphics g, Rectangle r, ScrollBarArrowButtonState state) {
		if (r.Width < 17 || r.Height < 18) {
			// ScrollBarRenderer does a terrible job at small sizes. So the image is drawn onto a larger
			// rectangle, and then the middle of that image is cropped out. Note: > 20, the a larger arrow
			// is used.
			using (Bitmap bmp = new Bitmap(Math.Max(20, r.Width), Math.Max(20, r.Height))) {
				using (var g2 = Graphics.FromImage(bmp)) {
					ScrollBarRenderer.DrawArrowButton(g2, new Rectangle(0, 0, bmp.Width, bmp.Height), state);
					using (Bitmap bmp2 = bmp.Clone(new Rectangle((bmp.Width - r.Width) / 2, (bmp.Height - r.Height) / 2, r.Width, r.Height), bmp.PixelFormat)) {
						g.DrawImageUnscaled(bmp2, 0, 0);
					}
				}
			}
		}
		else
			ScrollBarRenderer.DrawArrowButton(g, r, state);
	}

	private ScrollBarArrowButtonState GetState(int state, Button b) {
		if (b == DownButton) {
			if (b.Enabled) {
				if (state == 0)
					return ScrollBarArrowButtonState.DownNormal;
				if (state == 1)
					return ScrollBarArrowButtonState.DownPressed;
				return ScrollBarArrowButtonState.DownHot;
			}
			return ScrollBarArrowButtonState.DownDisabled;
		}
		if (b.Enabled) {
			if (state == 0)
				return ScrollBarArrowButtonState.UpNormal;
			if (state == 1)
				return ScrollBarArrowButtonState.UpPressed;
			return ScrollBarArrowButtonState.UpHot;
		}
		return ScrollBarArrowButtonState.UpDisabled;
	}

	///<summary>
	///Sets the visual presentation of the control.
	///Note: For System style, the button can still get the focus, even though SetStyle(Selectable, false)
	///is set. So it is not recommended to use System style.
	///</summary>
	public SpinButtonStyle ButtonStyle {
		get {
			return arrowStyle;
		}
		set {
			arrowStyle = value;
			if (value != SpinButtonStyle.Custom) {
				if (value == SpinButtonStyle.Modern) {
					UpButton.FlatStyle = FlatStyle.Flat;
					DownButton.FlatStyle = FlatStyle.Flat;
				}
				else {
					UpButton.FlatStyle = (FlatStyle) value;
					DownButton.FlatStyle = (FlatStyle) value;
				}
				ResizeButtons();
				UpdateArrowImages();
				Refresh();

				if (value == SpinButtonStyle.Standard) {
					UpButton.UseVisualStyleBackColor = true;
					DownButton.UseVisualStyleBackColor = true;
				}
			}
			else {
				ResizeButtons();
				Refresh();
			}
		}
	}

	public class SpinButton : Button {
		SpinControl Owner;
		private bool _showFocusCue = false; 
		public SpinButton(SpinControl owner, bool showFocusCue) {
			Owner = owner;
			SetShowFocusCue(showFocusCue);
			FlatStyle = FlatStyle.Standard;
			Margin = Padding.Empty;
            BackColor = Color.FromArgb(64, 64, 64);
            ForeColor = BackColor;
			//BackColor = SystemColors.ControlLight;
			DoubleBuffered = true;
			TabStop = false;
			SetStyle(ControlStyles.Selectable, false);
			this.AddRepeatingExponential();
		}

		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
            this.BackColor = (Enabled ? BackColor : BackColor);
            this.ForeColor = (Enabled ? BackColor : BackColor);
			if (Enabled && FlatStyle == FlatStyle.Standard)
				UseVisualStyleBackColor = true;
		}

		protected override bool ShowFocusCues {
			get {
				return _showFocusCue;
			}
		}

		public void SetShowFocusCue(bool showFocusCue) {
			_showFocusCue = showFocusCue;
		}

		private const int WM_PAINT = 0x000F;
		protected override void WndProc(ref Message m) {
			if (m.Msg == WM_PAINT && FlatStyle == FlatStyle.System) {
				base.WndProc(ref m);
				using (Graphics g = Graphics.FromHwnd(m.HWnd)) {
					if (Owner.UpButton == this)
						Owner.DrawUpButtonImage(g);
					else
						Owner.DrawDownButtonImage(g);
				}
				return;
			}
			base.WndProc(ref m);
		}
	}

	protected override void Dispose(bool disposing) {
		base.Dispose(disposing);
		if (disposing) {
			DisposeImages();
			if (ArrowFont != null)
				ArrowFont.Dispose();
			ArrowFont = null;
		}
	}

	private void DisposeImages() {
		if (ArrowUpImage != null) ArrowUpImage.Dispose();
		if (ArrowDownImage != null) ArrowDownImage.Dispose();
		if (ArrowUpImageDisabled != null) ArrowUpImageDisabled.Dispose();
		if (ArrowDownImageDisabled != null) ArrowDownImageDisabled.Dispose();

		ArrowUpImage = null;
		ArrowDownImage = null;
		ArrowUpImageDisabled = null;
		ArrowDownImageDisabled = null;
	}
}


}