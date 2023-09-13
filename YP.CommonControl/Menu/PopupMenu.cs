using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Drawing.Text;
using System.Collections;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using YP.CommonControl.Common;

namespace YP.CommonControl.Menu
{
    /// <summary>
    /// define the popup ment so that it can support theme
    /// </summary>
    public class PopupMenu : System.Windows.Forms.ContextMenuStrip, IItemsContainer
    {
        #region ..PopupMenu
        /// <summary>
        /// create a popup menu and attach the control
        /// </summary>
        /// <param name="parentControl"></param>
        public PopupMenu(Control parentControl):base()
        {
            this._parentControl = parentControl;
            this.DropShadowEnabled = false;
        }

        internal PopupMenu()
            : base()
        {
            this.DropShadowEnabled = false;
        }
        #endregion

        #region ..private fields
        Control _parentControl = null;
        #endregion

        #region ..OnPaint
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (this.DesignMode)
                return;
            e.Graphics.Clear(Color.FromArgb(120, this.BackColor));
            Color color = this.BackColor;
            Rectangle rect = new Rectangle(e.ClipRectangle.Location, new Size(MenuCommand.leftMargin, e.ClipRectangle.Height));
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(100, this.BackColor)))
            {
                e.Graphics.FillRectangle(Brushes.White, e.ClipRectangle);
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
            }

            using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.White, color, LinearGradientMode.Horizontal))
                e.Graphics.FillRectangle(brush, rect);
        }
        #endregion

        #region ..OnBackColorChanged
        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);
            foreach (ToolStripItem item in this.Items)
                item.BackColor = this.BackColor;
        }
        #endregion

        #region ..OnOpening
        protected override void OnOpening(CancelEventArgs e)
        {
            if (this._parentControl != null)
                this.BackColor = this._parentControl.BackColor;
            base.OnOpening(e);
            foreach (ToolStripItem item in this.Items)
            {
                if (item is IUpdateable)
                    (item as IUpdateable).InvokeUpdate();
            }
        }
        #endregion
    }
}