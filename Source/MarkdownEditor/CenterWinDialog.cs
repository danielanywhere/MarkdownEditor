/*
 * Copyright (c). 2010 Hans Passant.
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

/*

Usage:
private void button1_Click(object sender, EventArgs e)
{
	using (new CenterWinDialog(this))
	{
		MessageBox.Show("Nobugz waz here");
	}
}

Note that this code works for any of the Windows dialogs.
MessageBox, OpenFormDialog, FolderBrowserDialog, PrintDialog,
ColorDialog, FontDialog, PageSetupDialog, SaveFileDialog.

 */

namespace MarkdownEditor
{
	class CenterWinDialog : IDisposable
	{
		private int mTries = 0;
		private Form mOwner;

		public CenterWinDialog(Form owner)
		{
			mOwner = owner;
			owner.BeginInvoke(new MethodInvoker(findDialog));
		}

		private void findDialog()
		{
			// Enumerate windows to find the message box
			if(mTries < 0) return;
			EnumThreadWndProc callback = new EnumThreadWndProc(checkWindow);
			if(EnumThreadWindows(GetCurrentThreadId(), callback, IntPtr.Zero))
			{
				if(++mTries < 10) mOwner.BeginInvoke(new MethodInvoker(findDialog));
			}
		}
		private bool checkWindow(IntPtr hWnd, IntPtr lp)
		{
			// Checks if <hWnd> is a dialog
			StringBuilder sb = new StringBuilder(260);
			GetClassName(hWnd, sb, sb.Capacity);
			if(sb.ToString() != "#32770") return true;
			// Got it
			Rectangle frmRect = new Rectangle(mOwner.Location, mOwner.Size);
			RECT dlgRect;
			GetWindowRect(hWnd, out dlgRect);
			MoveWindow(hWnd,
					frmRect.Left + (frmRect.Width - dlgRect.Right + dlgRect.Left) / 2,
					frmRect.Top + (frmRect.Height - dlgRect.Bottom + dlgRect.Top) / 2,
					dlgRect.Right - dlgRect.Left,
					dlgRect.Bottom - dlgRect.Top, true);
			return false;
		}
		public void Dispose()
		{
			mTries = -1;
		}

		// P/Invoke declarations
		private delegate bool EnumThreadWndProc(IntPtr hWnd, IntPtr lp);
		[DllImport("user32.dll")]
		private static extern bool EnumThreadWindows(int tid, EnumThreadWndProc callback, IntPtr lp);
		[DllImport("kernel32.dll")]
		private static extern int GetCurrentThreadId();
		[DllImport("user32.dll")]
		private static extern int GetClassName(IntPtr hWnd, StringBuilder buffer, int buflen);
		[DllImport("user32.dll")]
		private static extern bool GetWindowRect(IntPtr hWnd, out RECT rc);
		[DllImport("user32.dll")]
		private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int w, int h, bool repaint);
		private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
	}

}
