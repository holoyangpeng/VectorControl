using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.Drawing.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Microsoft.Win32;
using YP.CommonControl.Common;

namespace YP.CommonControl.Menu
{
    /// <summary>
    /// define the menucontrol so that it can support the theme
    /// </summary>
    public class MenuControl : System.Windows.Forms.MenuStrip, IItemsContainer
    {
        #region ..Constructor
        public MenuControl()
        {
            this.CanOverflow = true;
        }
        #endregion

        #region ..private fields
        bool _drawGradientBackground = false;
        #endregion

        #region ..public properties
        /// <summary>
        /// gets or sets a value indicates whether fill the gradient background
        /// </summary>
        public bool GradientBackground
        {
            set
            {
                if (this._drawGradientBackground != value)
                {
                    this._drawGradientBackground = value;
                    this.Invalidate();
                }
            }
            get
            {
                return this._drawGradientBackground;
            }
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

        #region ..OnPaint
        protected override void OnPaint(PaintEventArgs e)
        {
            if (this._drawGradientBackground)
            {
                Color startColor = this.BackColor;
                Color endColor = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.LightLight(this.BackColor)));
                Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, startColor, endColor, System.Drawing.Drawing2D.LinearGradientMode.Horizontal))
                    e.Graphics.FillRectangle(brush, rect);
            }
            base.OnPaint(e);
        }
        #endregion
    }
}
