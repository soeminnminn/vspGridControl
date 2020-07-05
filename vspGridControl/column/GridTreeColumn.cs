using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Microsoft.SqlServer.Management.UI.Grid
{
    public class GridTreeColumn : GridColumn
    {
        #region Variables
        protected bool m_bVertical;
        protected StringFormat m_myStringFormat;
        protected TextFormatFlags m_textFormat;
        #endregion

        protected GridTreeColumn()
        {
            this.m_myStringFormat = new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoWrap);
            this.m_textFormat = GridConstants.DefaultTextFormatFlags;
        }

        public GridTreeColumn(GridColumnInfo ci, int nWidthInPixels, int colIndex)
            : base(ci, nWidthInPixels, colIndex)
        {
            this.m_myStringFormat = new StringFormat(StringFormatFlags.LineLimit | StringFormatFlags.NoWrap);
            this.m_textFormat = GridConstants.DefaultTextFormatFlags;
            this.m_myStringFormat.HotkeyPrefix = HotkeyPrefix.None;
            this.m_myStringFormat.Trimming = StringTrimming.EllipsisCharacter;
            this.m_myStringFormat.LineAlignment = StringAlignment.Center;
            this.SetStringFormatRTL(GridColumn.s_defaultRTL);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (this.m_myStringFormat != null)
            {
                this.m_myStringFormat.Dispose();
                this.m_myStringFormat = null;
            }
        }

        public override void DrawCell(Graphics g, Brush bkBrush, SolidBrush textBrush, Font textFont, Rectangle rect, IGridStorage storage, long nRowIndex)
        {
            g.FillRectangle(bkBrush, rect);
            rect.Inflate(-GridColumn.CELL_CONTENT_OFFSET, 0);
            int level;
            bool expanded;
            bool hasChildren;
            string label;
            storage.GetCellDataForTree(nRowIndex, base.m_myColumnIndex, out level, out expanded, out hasChildren, out label);

            if (rect.Width > 0)
            {
                int x = rect.X + (level * SystemInformation.SmallIconSize.Width);
                int y = rect.Y;

                Rectangle rcPlusMinus = new Rectangle(x, y, SystemInformation.SmallIconSize.Width, rect.Height);
                if (hasChildren)
                {
                    this.DrawTreePlusMinus(g, rcPlusMinus, expanded);
                }

                Rectangle rcText = new Rectangle(x + rcPlusMinus.Width, y, rect.Width - rcPlusMinus.Width, rect.Height);
                if (this.m_bVertical)
                {
                    this.DrawTextStringForVerticalFonts(g, textBrush, textFont, rcText, label, nRowIndex, false);
                }
                else
                {
                    TextRenderer.DrawText(g, label, textFont, rcText, textBrush.Color, this.m_textFormat);
                }
            }
        }

        private void DrawTextStringForVerticalFonts(Graphics g, SolidBrush textBrush, Font textFont, Rectangle rect, string text, long nRowIndex, bool useGdiPlus)
        {
            using (Matrix matrix = new Matrix(0f, -1f, 1f, 0f, (float)(rect.X - rect.Y), (float)((rect.X + rect.Y) + rect.Height)))
            {
                new Rectangle(rect.X, rect.Y, rect.Height, rect.Width);
                g.Transform = matrix;
                if (useGdiPlus)
                {
                    g.DrawString(text, textFont, textBrush, rect, this.m_myStringFormat);
                }
                else
                {
                    TextRenderer.DrawText(g, text, textFont, rect, textBrush.Color, this.m_textFormat);
                }
                g.ResetTransform();
            }
        }

        private void DrawTreePlusMinus(Graphics graphics, Rectangle rcPaint, bool opened)
        {
            VisualStyleElement element = DrawManager.GetTreePlusMinus(opened ? ButtonState.Checked : ButtonState.Normal);
            if (element != null)
            {
                VisualStyleRenderer renderer = new VisualStyleRenderer(element);
                renderer.DrawBackground(graphics, rcPaint);
            }
            else
            {
                Bitmap bitmap = opened ? GridConstants.TreeMinusBitmap : GridConstants.TreePlusBitmap;
                graphics.DrawImage(bitmap, new Point(rcPaint.Left + ((rcPaint.Width - 16) / 2), rcPaint.Top + ((rcPaint.Height - 16) / 2)));
            }
        }

        public override string GetAccessibleValue(long nRowIndex, IGridStorage storage)
        {
            return storage.GetCellDataAsString(nRowIndex, base.m_myColumnIndex);
        }

        public override void PrintCell(Graphics g, Brush bkBrush, SolidBrush textBrush, Font textFont, Rectangle rect, IGridStorage storage, long nRowIndex)
        {
            g.FillRectangle(bkBrush, rect.X - 1, rect.Y, rect.Width, rect.Height);
            rect.Inflate(-GridColumn.CELL_CONTENT_OFFSET, 0);
            if (rect.Width > 0)
            {
                int level = 0;
                bool expanded = false;
                bool hasChildren = false;
                string label = "";
                storage.GetCellDataForTree(nRowIndex, base.m_myColumnIndex, out level, out expanded, out hasChildren, out label);

                if (this.m_bVertical)
                {
                    this.DrawTextStringForVerticalFonts(g, textBrush, textFont, rect, label, nRowIndex, true);
                }
                else
                {
                    g.DrawString(label, textFont, textBrush, rect, this.m_myStringFormat);
                }
            }
        }

        public override void ProcessNewGridFont(Font gridFont)
        {
            if (gridFont.GdiVerticalFont)
            {
                this.m_myStringFormat.FormatFlags |= StringFormatFlags.DirectionVertical;
                this.m_bVertical = true;
            }
            else
            {
                this.m_myStringFormat.FormatFlags &= ~StringFormatFlags.DirectionVertical;
                this.m_bVertical = false;
            }
        }

        public override void SetRTL(bool bRightToLeft)
        {
            this.SetStringFormatRTL(bRightToLeft);
        }

        private void SetStringFormatRTL(bool bRTL)
        {
            if (bRTL)
            {
                this.m_myStringFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                this.m_textFormat |= TextFormatFlags.RightToLeft;
            }
            else
            {
                this.m_myStringFormat.FormatFlags &= ~StringFormatFlags.DirectionRightToLeft;
                this.m_textFormat &= ~TextFormatFlags.RightToLeft;
            }
            GridConstants.AdjustFormatFlagsForAlignment(ref this.m_textFormat, base.m_myAlign);
            if (base.m_myAlign == HorizontalAlignment.Left)
            {
                this.m_myStringFormat.Alignment = StringAlignment.Near;
            }
            else if (base.m_myAlign == HorizontalAlignment.Center)
            {
                this.m_myStringFormat.Alignment = StringAlignment.Center;
            }
            else
            {
                this.m_myStringFormat.Alignment = StringAlignment.Far;
            }
        }
    }
}
