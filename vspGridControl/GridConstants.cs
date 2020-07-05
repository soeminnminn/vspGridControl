using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Microsoft.SqlServer.Management.UI.Grid
{
    public sealed class GridConstants
    {
        // Fields
        private static Bitmap s_CheckedBitmap;
        private static Bitmap s_DisabledBitmap;
        private static Bitmap s_IntermidiateBitmap;
        private static Bitmap s_UncheckedBitmap;
        private static Bitmap s_TreePlusBitmap;
        private static Bitmap s_TreeMinusBitmap;

        public const int StandardCheckBoxSize = 13;
        public const int StandardTreeGlyphSize = 16;

        public static ushort[] treePlusBoxBmp = { 0xffff, 0xffff, 0xffff, 0x0fe0, 0xefef, 0xefee, 0xefee, 0x2fe8, 0xefee, 0xefee, 0xefef, 0x0fe0, 0xffff, 0xffff, 0xffff, 0xffff };
        public static ushort[] treeMinusBoxBmp = { 0xffff, 0xffff, 0xffff, 0x0fe0, 0xefef, 0xefef, 0xefef, 0x2fe8, 0xefef, 0xefef, 0xefef, 0x0fe0, 0xffff, 0xffff, 0xffff, 0xffff };

        public const string TName = "GridControl";

        [CLSCompliant(false)]
        public const uint trERR = 0x40000000;
        [CLSCompliant(false)]
        public const uint trL1 = 1;
        [CLSCompliant(false)]
        public const uint trL2 = 2;
        [CLSCompliant(false)]
        public const uint trL3 = 4;
        [CLSCompliant(false)]
        public const uint trL4 = 8;
        [CLSCompliant(false)]
        public const uint trWARN = 0x20000000;

        // Methods
        internal static void AdjustFormatFlagsForAlignment(ref TextFormatFlags inputFlags, HorizontalAlignment ha)
        {
            switch (ha)
            {
                case HorizontalAlignment.Left:
                    inputFlags &= ~TextFormatFlags.Right;
                    inputFlags &= ~TextFormatFlags.HorizontalCenter;
                    return;

                case HorizontalAlignment.Right:
                    inputFlags &= ~TextFormatFlags.GlyphOverhangPadding;
                    inputFlags &= ~TextFormatFlags.HorizontalCenter;
                    inputFlags |= TextFormatFlags.Right;
                    return;

                case HorizontalAlignment.Center:
                    inputFlags &= ~TextFormatFlags.Right;
                    inputFlags &= ~TextFormatFlags.GlyphOverhangPadding;
                    inputFlags |= TextFormatFlags.HorizontalCenter;
                    return;
            }
        }

        private static void GetIntermidiateCheckboxBitmap(Bitmap bmp)
        {
            Rectangle bounds = new Rectangle(0, 0, 13, 13);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.Clear(Color.Transparent);
                if (Application.RenderWithVisualStyles)
                {
                    VisualStyleElement checkBox = DrawManager.GetCheckBox(ButtonState.Flat);
                    if ((checkBox != null) && VisualStyleRenderer.IsElementDefined(checkBox))
                    {
                        new VisualStyleRenderer(checkBox).DrawBackground(graphics, bounds);
                        return;
                    }
                }
                ControlPaint.DrawMixedCheckBox(graphics, bounds, ButtonState.Checked);
            }
        }

        private static void GetStdCheckBitmap(Bitmap bmp, ButtonState state)
        {
            Rectangle bounds = new Rectangle(0, 0, StandardCheckBoxSize, StandardCheckBoxSize);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.Clear(Color.Transparent);
                if (Application.RenderWithVisualStyles)
                {
                    VisualStyleElement element = DrawManager.GetCheckBox(state);
                    if ((element != null) && VisualStyleRenderer.IsElementDefined(element))
                    {
                        new VisualStyleRenderer(element).DrawBackground(graphics, bounds);
                        return;
                    }
                }
                ControlPaint.DrawCheckBox(graphics, bounds, state);
            }
        }

        internal static void RegenerateCheckBoxBitmaps()
        {
            Bitmap checkedCheckBoxBitmap = CheckedCheckBoxBitmap;
            Bitmap uncheckedCheckBoxBitmap = UncheckedCheckBoxBitmap;
            Bitmap intermidiateCheckBoxBitmap = IntermidiateCheckBoxBitmap;
            Bitmap disabledCheckBoxBitmap = DisabledCheckBoxBitmap;
        }

        private static void GetStdTreeBitmap(Bitmap bmp, ButtonState state)
        {
            Rectangle bounds = new Rectangle(0, 0, StandardTreeGlyphSize, StandardTreeGlyphSize);
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                graphics.Clear(Color.Transparent);
                if (Application.RenderWithVisualStyles)
                {
                    VisualStyleElement element = DrawManager.GetTreePlusMinus(state);
                    if ((element != null) && VisualStyleRenderer.IsElementDefined(element))
                    {
                        new VisualStyleRenderer(element).DrawBackground(graphics, bounds);
                        return;
                    }
                }

                CheckState checkState = ((state & ButtonState.Checked) == ButtonState.Checked) ? CheckState.Checked : CheckState.Unchecked;
                ushort[] boxBmp = treePlusBoxBmp;
                if (checkState == CheckState.Checked)
                {
                    boxBmp = treeMinusBoxBmp;
                }
                IntPtr bmpPtr = NativeMethods.CreateBitmap(16, 16, 1, 1, boxBmp);
                using (Bitmap bmpGlyph = new Bitmap(16, 16))
                {
                    Graphics gBmp = Graphics.FromImage(bmpGlyph);
                    gBmp.FillRectangle(new SolidBrush(Color.Purple), new Rectangle(0, 0, 16, 16));
                    gBmp.FillRectangle(SystemBrushes.Window, new Rectangle(3, 3, 8, 8));

                    using (Bitmap bmpMark = Bitmap.FromHbitmap(bmpPtr))
                    {
                        bmpMark.MakeTransparent(Color.White);
                        gBmp.DrawImage(bmpMark, Point.Empty);
                    }
                    gBmp.DrawRectangle(new Pen(Color.Gray), new Rectangle(3, 3, 8, 8));
                    bmpGlyph.MakeTransparent(Color.Purple);
                    graphics.DrawImage(bmpGlyph, new Point(bounds.Left + ((bounds.Width - 16) / 2), bounds.Top + ((bounds.Height - 16) / 2)));
                }
                bmpPtr = IntPtr.Zero;
            }
        }

        // Properties
        public static TextFormatFlags DefaultTextFormatFlags
        {
            get
            {
                return (TextFormatFlags.PreserveGraphicsClipping | TextFormatFlags.WordEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter);
            }
        }

        public static Bitmap CheckedCheckBoxBitmap
        {
            get
            {
                if (s_CheckedBitmap == null)
                {
                    s_CheckedBitmap = new Bitmap(StandardCheckBoxSize, StandardCheckBoxSize);
                    GetStdCheckBitmap(s_CheckedBitmap, ButtonState.Checked);
                }
                return s_CheckedBitmap;
            }
        }

        public static Bitmap DisabledCheckBoxBitmap
        {
            get
            {
                if (s_DisabledBitmap == null)
                {
                    s_DisabledBitmap = new Bitmap(StandardCheckBoxSize, StandardCheckBoxSize);
                    GetStdCheckBitmap(s_DisabledBitmap, ButtonState.Inactive);
                }
                return s_DisabledBitmap;
            }
        }

        public static Bitmap IntermidiateCheckBoxBitmap
        {
            get
            {
                if (s_IntermidiateBitmap == null)
                {
                    s_IntermidiateBitmap = new Bitmap(StandardCheckBoxSize, StandardCheckBoxSize);
                    GetIntermidiateCheckboxBitmap(s_IntermidiateBitmap);
                }
                return s_IntermidiateBitmap;
            }
        }

        public static Bitmap UncheckedCheckBoxBitmap
        {
            get
            {
                if (s_UncheckedBitmap == null)
                {
                    s_UncheckedBitmap = new Bitmap(StandardCheckBoxSize, StandardCheckBoxSize);
                    GetStdCheckBitmap(s_UncheckedBitmap, ButtonState.Normal);
                }
                return s_UncheckedBitmap;
            }
        }

        public static Bitmap TreePlusBitmap
        {
            get
            {
                if (s_TreePlusBitmap == null)
                {
                    s_TreePlusBitmap = new Bitmap(StandardTreeGlyphSize, StandardTreeGlyphSize);
                    GetStdTreeBitmap(s_TreePlusBitmap, ButtonState.Normal);
                }
                return s_TreePlusBitmap;
            }
        }

        public static Bitmap TreeMinusBitmap
        {
            get
            {
                if (s_TreeMinusBitmap == null)
                {
                    s_TreeMinusBitmap = new Bitmap(StandardTreeGlyphSize, StandardTreeGlyphSize);
                    GetStdTreeBitmap(s_TreeMinusBitmap, ButtonState.Checked);
                }
                return s_TreeMinusBitmap;
            }
        }
    }

    public class EditableCellType
    {
        public const int ComboBox = 2;
        public const int Editor = 1;
        public const int FirstCustomEditableCellType = 0x400;
        public const int ListBox = 3;
        public const int ReadOnly = 0;
        public const int SpinBox = 4;
    }

    public class GridColumnType
    {
        // Fields
        public const int Text = 1;
        public const int Button = 2;
        public const int Bitmap = 3;
        public const int Checkbox = 4;
        public const int Hyperlink = 5;
        public const int Tree = 6;
        public const int FirstCustomColumnType = 0x400;
    }
}