using System.Windows.Forms;

namespace PlagueEngine.Editor.Controls
{
    public class BetterPropertyGrid : PropertyGrid
    {
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int TAB = 9;
        private const int SHIFT = 16;
        private bool isShiftDown = false;

        public BetterPropertyGrid()
        {
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            int wParam = m.WParam.ToInt32();

            switch (m.Msg)
            {
                case WM_KEYDOWN:
                    if (wParam == SHIFT)
                    {
                        isShiftDown = true;
                        return true;
                    }
                    break;
                case WM_KEYUP:
                    if (wParam == TAB)
                    {
                        moveSelectedGridItem(!isShiftDown);
                        return true;
                    }
                    else if (wParam == SHIFT)
                    {
                        isShiftDown = false;
                        return true;
                    }
                    break;
            }
            return ProcessKeyEventArgs(ref m);
        }

        protected override bool ProcessTabKey(bool forward)
        {
            return true;
        }

        private void moveSelectedGridItem(bool forward)
        {
            if (SelectedGridItem != null && SelectedGridItem.Parent != null)
            {
                GridItemCollection allItems = SelectedGridItem.Parent.GridItems;
                int currentIndex = -1;
                for (int i = 0; i < allItems.Count; i++)
                {
                    if (allItems[i] == SelectedGridItem)
                    {
                        currentIndex = i;
                        break;
                    }
                }
                if (forward)
                {
                    if (currentIndex >= 0 && currentIndex < allItems.Count - 1)
                    {
                        SelectedGridItem = allItems[currentIndex + 1];
                    }
                }
                else
                {
                    if (currentIndex >= 1 && currentIndex < allItems.Count)
                    {
                        SelectedGridItem = allItems[currentIndex - 1];
                    }
                }
            }
        }
    }
}