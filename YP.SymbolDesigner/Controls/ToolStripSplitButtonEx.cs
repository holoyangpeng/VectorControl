using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace YP.SymbolDesigner.Controls
{
    public class ToolStripSplitButtonEx:ToolStripSplitButton
    {
        #region ..events
        public event EventHandler CheckedChanged;
        #endregion

        #region ..properties
        private bool @checked = false;
        public bool Checked 
        {
            set
            {
                if (@checked != value)
                {
                    @checked = value;
                    this.Invalidate();
                    OnCheckedChanged();
                }
            }
            get
            {
                return @checked;
            }
        }

        public bool CheckOnClick { set; get; }
        #endregion

        #region ..OnPaint
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Checked)
            {
                Rectangle rect = ButtonBounds;
                using (Brush br = new LinearGradientBrush(rect,
                                                          ProfessionalColors.ButtonCheckedGradientBegin,
                                                          ProfessionalColors.ButtonCheckedGradientEnd,
                                                          LinearGradientMode.Vertical))
                {
                    e.Graphics.FillRectangle(br, rect);
                }
                rect.Width--;
                rect.Height--;
                e.Graphics.DrawRectangle(new System.Drawing.Pen(ProfessionalColors.ButtonSelectedHighlightBorder), rect);
            }
            base.OnPaint(e);
        }
        #endregion

        #region ..OnClick
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (this.CheckOnClick)
                this.Checked = !Checked;
        }
        #endregion

        #region ..OnCheckedChanged
        protected virtual void OnCheckedChanged()
        {
            if (this.CheckedChanged != null)
                this.CheckedChanged(this, EventArgs.Empty);
        }
        #endregion
    }
}
