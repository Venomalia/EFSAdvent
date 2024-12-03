using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace EFSAdvent
{
    public class CheckedListBoxColorable : CheckedListBox
    {
        public readonly Dictionary<object, Color> Colors;

        public CheckedListBoxColorable()
        {
            Colors = new Dictionary<object, Color>();
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DoubleBuffered = true;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            Color foreColor = e.ForeColor;

            if (e.Index < Items.Count && Colors.TryGetValue(Items[e.Index], out Color customColor))
                foreColor = customColor;

            var tweakedEventArgs = new DrawItemEventArgs(e.Graphics, e.Font, e.Bounds, e.Index, e.State, foreColor, e.BackColor);
            base.OnDrawItem(tweakedEventArgs);
        }

    }
}
