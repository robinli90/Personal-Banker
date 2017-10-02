using System;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Opulos.Core.UI {

[StructLayout(LayoutKind.Sequential)]
public struct RECT {
	public int Left, Top, Right, Bottom;
}

public class SnapPoint {

	public double AlignY = 0;
	public double ChildHeightFactor = 0;
	public double ParentHeightFactor = 0;
	public int OffsetConstantY = 0;

	public double AlignX = 0;
	public double ChildWidthFactor = 0;
	public double ParentWidthFactor = 0;
	public int OffsetConstantX = 0;

	///<summary>For efficiency, a GetWindowRect call is avoided if the childRect is not needed for the Location calculation.
	///Subclasses can override this method if needed.</summary>
	public virtual bool NeedsChildRect {
		get {
			return AlignY != 0 || ChildHeightFactor != 0 || AlignX != 0 || ChildWidthFactor != 0;
		}
	}

	public virtual Point GetLocation(RECT childRect, RECT ownerRect) {
		int h1 = childRect.Bottom - childRect.Top;
		int h2 = ownerRect.Bottom - ownerRect.Top;
		int availH = Math.Max(0, h2 - h1);
		int y = (int) Math.Round(AlignY * availH + ChildHeightFactor * h1 + ParentHeightFactor * h2 + OffsetConstantY);

		int w1 = childRect.Right - childRect.Left;
		int w2 = ownerRect.Right - ownerRect.Left;
		int availW = Math.Max(0, w2 - w1);
		int x = (int) Math.Round(AlignX * availW + ChildWidthFactor * w1 + ParentWidthFactor * w2 + OffsetConstantX);

		return new Point(x + ownerRect.Left, y + ownerRect.Top);
	}
}

public static class SnapWindowEx {

	private static Hashtable htData = new Hashtable();

	///<summary>Snaps the child window to the top level owner window, relative to the hWndSnap window. As
	///the top level owner window is moved, resized, minimized or maximized, the child window moves too.</summary>
	///<param name="child">The control that is automtically moved, e.g. a ToolStripDropDown window.</param>
	///<param name="hWndTopLevel">A handle to a top-level window, e.g. a Form control's Handle.</param>
	///<param name="hWndParent">A handle to a control that child window is relatively positioned, e.g. a TextBox or ComoBox.</param>
	///<param name="snapPoint">Parameters that specify the X and Y location.</param>
	public static void SnapWindow(this Control child, IntPtr hWndParent, IntPtr hWndTopLevel, SnapPoint snapPoint) {
		if (child.IsHandleCreated)
			SetOwner(child.Handle, hWndParent, hWndTopLevel, snapPoint);
		else {
			child.HandleCreated += delegate {
				SetOwner(child.Handle, hWndParent, hWndTopLevel, snapPoint);
			};
		}
	}

	public static void SetOwner(IntPtr hWndChild, IntPtr hWndParent, IntPtr hWndTopLevel, SnapPoint snapPoint) {
		Data d = (Data) htData[hWndChild];
		if (d != null) {
			d.nwOwner.ReleaseHandle();
			if (hWndTopLevel != IntPtr.Zero)
				d.nwOwner = new OwnerNW(hWndTopLevel, d);
			else {
				d.nwChild.ReleaseHandle();
				htData.Remove(hWndChild);
			}
		}
		else {
			if (hWndTopLevel == IntPtr.Zero)
				return;

			d = new Data(snapPoint, hWndChild, hWndTopLevel, hWndParent);
			htData[hWndChild] = d;
		}
	}

	private class Data {
		public ChildNW nwChild = null;
		public OwnerNW nwOwner = null;
		public IntPtr SnapHandle;
		public SnapPoint snapPoint = null;

		public Data(SnapPoint snapPoint, IntPtr hWndChild, IntPtr hWndOwner, IntPtr hWndSnap) {
			this.snapPoint = snapPoint;
			SnapHandle = hWndSnap;
			nwChild = new ChildNW(hWndChild, this);
			nwOwner = new OwnerNW(hWndOwner, this);
		}
	}

	private class OwnerNW : NativeWindow {
		Data data;
		public OwnerNW(IntPtr hWnd, Data d) {
			AssignHandle(hWnd);
			data = d;	
		}

		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);

			// several different approaches were tried, such as WM_MOVING, WM_MOVED, WM_WINDOWPOSCHANGING
			// WM_MOVING and WM_MOVED do not account for resizing the window. WM_WINDOWPOSCHANGING fires
			// false events (e.g. clicking on the title bar near (but not on) the minimize window button).
			if (m.Msg == WM_WINDOWPOSCHANGED) {
				WINDOWPOS pos = (WINDOWPOS) Marshal.PtrToStructure(m.LParam, typeof(WINDOWPOS));
				// -32000 means the window is minimized.
				if (pos.x > -32000 && pos.y > -32000) {
					RECT rChild = new RECT();
					RECT rOwner = new RECT();
					if (data.SnapHandle == Handle) {
						rOwner.Top = pos.y;
						rOwner.Left = pos.x;
						rOwner.Bottom = pos.y + pos.cy;
						rOwner.Right = pos.x + pos.cx;
					}
					else {
						GetWindowRect(data.SnapHandle, ref rOwner);
					}

					SnapPoint sp = data.snapPoint;
					if (sp.NeedsChildRect)
						GetWindowRect(data.nwChild.Handle, ref rChild);

					Point pt = sp.GetLocation(rChild, rOwner);
					SetWindowPos(data.nwChild.Handle, IntPtr.Zero, pt.X, pt.Y, 0, 0, SWP_NOOWNERZORDER | SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
				}
			}
		}
	}

	private class ChildNW : NativeWindow {
		Data data;

		public ChildNW(IntPtr hWnd, Data d) {
			AssignHandle(hWnd);
			data = d;
		}
		protected override void WndProc(ref Message m) {
			base.WndProc(ref m);

			if (m.Msg == WM_SHOWWINDOW) {
				RECT rChild = new RECT();
				RECT rOwner = new RECT();
				GetWindowRect(data.SnapHandle, ref rOwner);
				SnapPoint sp = data.snapPoint;
				if (sp.NeedsChildRect)
					GetWindowRect(Handle, ref rChild);

				// must do this, otherwise when the window is invisible (e.g. owner is minimized) then its location resets
				Point pt = sp.GetLocation(rChild, rOwner);
				SetWindowPos(Handle, IntPtr.Zero, pt.X, pt.Y, 0, 0, SWP_NOOWNERZORDER | SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE);
			}
		}
	}

	[DllImport("user32.dll", SetLastError=true)]
	private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

	[DllImport("user32.dll", SetLastError=true)]
	private static extern bool GetWindowRect(IntPtr hwnd, ref RECT lpRect);

	private const int SWP_NOOWNERZORDER = 0x0200;
	private const int SWP_NOSIZE = 0x0001;
	private const int SWP_NOZORDER = 0x0004;
	private const int SWP_NOACTIVATE = 0x0010;

	private const int WM_WINDOWPOSCHANGED = 0x47;
	private const int WM_SHOWWINDOW = 0x18;

	[StructLayout(LayoutKind.Sequential)]
	private struct WINDOWPOS {
		public IntPtr hwnd;
		public IntPtr hwndInsertAfter;
		public int x;
		public int y;
		public int cx;
		public int cy;
		public uint flags;
	}
}

}