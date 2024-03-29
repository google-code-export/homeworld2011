﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace PlagueEngine.Tools
{
    public class ScrollingRichTextBox : System.Windows.Forms.RichTextBox
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(
          IntPtr hWnd,
          uint Msg,
          IntPtr wParam,
          IntPtr lParam);

        private const int WM_VSCROLL = 277;
        private const int SB_LINEUP = 0;
        private const int SB_LINEDOWN = 1;
        private const int SB_TOP = 6;
        private const int SB_BOTTOM = 7;
      
        public void ScrollToBottom()
        {
            SendMessage(Handle, WM_VSCROLL, new IntPtr(SB_BOTTOM), IntPtr.Zero);
        }

        public void ScrollToTop()
        {
            SendMessage(Handle, WM_VSCROLL, new IntPtr(SB_TOP), IntPtr.Zero);
        }

        public void ScrollLineDown()
        {
            SendMessage(Handle, WM_VSCROLL, new IntPtr(SB_LINEDOWN), IntPtr.Zero);
        }

        public void ScrollLineUp()
        {
            SendMessage(Handle, WM_VSCROLL, new IntPtr(SB_LINEUP), IntPtr.Zero);
        }
    }
}
