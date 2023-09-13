using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace YP.CommonControl.Dock
{
    /// <summary>
    /// define a new split container to layout the zone
    /// </summary>
    internal class ZoneContainer:System.Windows.Forms.SplitContainer
    {
        #region ..Constructor
        public ZoneContainer():base()
        {
            this.Panel1.ControlRemoved += new ControlEventHandler(Panel1_ControlRemoved);
            this.Panel2.ControlRemoved += new ControlEventHandler(Panel2_ControlRemoved);
            this.Panel1.ControlAdded += new ControlEventHandler(Panel1_ControlAdded);
            this.Panel2.ControlAdded += new ControlEventHandler(Panel2_ControlAdded);
            this.SplitterWidth = 3;
            this.Panel1.Margin = new Padding(0);
            this.Panel2.Margin = new Padding(0);
            this.Margin = new Padding(0);

            this.SplitterMoving += new SplitterCancelEventHandler(ZoneContainer_SplitterMoving);
        }
        #endregion

        #region ..const fields
        internal const int MinSize = 60;
        #endregion

        #region ..PanelControlAdded
        void Panel2_ControlAdded(object sender, ControlEventArgs e)
        {
            if (this.FixedPanel == FixedPanel.Panel2 && this.Panel2.Controls.Count > 0)
                this.Panel2Collapsed = false;
        }

        void Panel1_ControlAdded(object sender, ControlEventArgs e)
        {
            if (this.FixedPanel == FixedPanel.Panel1 && this.Panel1.Controls.Count > 0)
                this.Panel1Collapsed = false;
        }
        #endregion

        #region ..PanelControlRemoved
        void Panel2_ControlRemoved(object sender, ControlEventArgs e)
        {
            this.SuspendLayout();
            if (this.FixedPanel == FixedPanel.Panel2 && this.Panel2.Controls.Count == 0)
                this.Panel2Collapsed = true;
            this.ResumeLayout(true);
        }

        void Panel1_ControlRemoved(object sender, ControlEventArgs e)
        {
            this.SuspendLayout();
            if (this.FixedPanel == FixedPanel.Panel1 && this.Panel1.Controls.Count == 0)
                this.Panel1Collapsed = true;
            this.ResumeLayout(true);
        }
        #endregion

        #region ..GetZone
        /// <summary>
        /// gets the zone display in the container
        /// </summary>
        /// <returns></returns>
        internal Zone GetZone()
        {
            SplitterPanel panel = null;
            //get the fixed panel
            if (this.FixedPanel == FixedPanel.Panel1)
                panel = this.Panel1;
            else if (this.FixedPanel == FixedPanel.Panel2)
                panel = this.Panel2;
            //find the zone in the panel
            if (panel != null)
            {
                foreach (Control c in panel.Controls)
                {
                    if (c is Zone)
                        return c as Zone;
                }
            }
            return null;
        }
        #endregion

        #region ..OnPaint
        protected override void OnPaint(PaintEventArgs e)
        {
            LayoutManager manager = LayoutManager.FindManagerForControl(this);
            if (manager != null && manager.GradientBackground)
            {
                Color startColor = this.BackColor;
                Color endColor = this.BackColor;
                Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
                //vertical
                if (this.Orientation == Orientation.Vertical)
                {
                    if (this.FixedPanel == FixedPanel.Panel2)
                        startColor = endColor = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.LightLight(this.BackColor)));
                    using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, startColor, endColor, System.Drawing.Drawing2D.LinearGradientMode.Vertical))
                        e.Graphics.FillRectangle(brush, rect);
                }
                //horizontal
                else
                {
                    endColor = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.LightLight(this.BackColor)));

                    using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, startColor, endColor, System.Drawing.Drawing2D.LinearGradientMode.Horizontal))
                        e.Graphics.FillRectangle(brush, rect);
                }
            }
            //base.OnPaint(e);
        }
        #endregion

        #region ..ZoneContainer_SplitterMoving
        void ZoneContainer_SplitterMoving(object sender, SplitterCancelEventArgs e)
        {
            //ensure the min size
            if (this.Orientation == Orientation.Vertical)
                e.Cancel = e.MouseCursorX < 30 || e.MouseCursorX > this.Width - 30;
            else
                e.Cancel = e.MouseCursorY < 30 || e.MouseCursorY > this.Height - 30;
        }
        #endregion
    }
}
