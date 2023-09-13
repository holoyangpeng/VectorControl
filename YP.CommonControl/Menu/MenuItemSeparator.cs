using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.CommonControl.Menu
{
    /// <summary>
    /// define the separator so that it can support theme
    /// </summary>
    internal class MenuItemSeparator : System.Windows.Forms.ToolStripSeparator, IUpdateable
    {
        #region ..OnPaint
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            Color color = this.BackColor;
            int leftMargin = MenuCommand.leftMargin - 2;
            Rectangle rect = new Rectangle(e.ClipRectangle.Location, new Size(leftMargin, e.ClipRectangle.Height));
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(120, this.BackColor)))
            {
                e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle);
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
            }
            using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.White, color, LinearGradientMode.Horizontal))
                e.Graphics.FillRectangle(brush, rect);

            e.Graphics.DrawLine(SystemPens.InactiveCaption, leftMargin + 8, e.ClipRectangle.Y + e.ClipRectangle.Height / 2, e.ClipRectangle.Right, e.ClipRectangle.Y + e.ClipRectangle.Height / 2);
        }
        #endregion

        #region ..events
        /// <summary>
        /// occurs when the menu item show
        /// </summary>
        public event EventHandler Update;
        #endregion

        #region ..OnUpdate
        protected virtual void OnUpdate()
        {
            if (this.Update != null)
                this.Update(this, EventArgs.Empty);
        }
        #endregion

        #region ..InvokeUpdate
        /// <summary>
        /// invoke the update method
        /// </summary>
        public void InvokeUpdate()
        {
            this.OnUpdate();
        }
        #endregion
    }
}
