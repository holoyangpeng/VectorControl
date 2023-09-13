using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace YP.CommonControl.Dock
{
    /// <summary>
    /// define the control which the border dock with when it it auto hide
    /// </summary>
    internal class BorderPanel : Common.BaseControl, IGroupContainer
    {
        #region ..Constructor
        public BorderPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            this._groups.Inserted += new Common.CollectionWithEvents.CollectionEventHandler(_groups_Inserted);
            this._groups.Removed += new Common.CollectionWithEvents.CollectionEventHandler(_groups_Removed);
            this._groups.Cleared += new Common.CollectionWithEvents.ClearEventHandler(_groups_Cleared);
        }
       
        #endregion

        #region ..private fields
        GroupCollection _groups = new GroupCollection();
        AutoHidePanel _autoHidePanel = null;
        Hashtable _contentBounds = new Hashtable();
        Content _selectedConent = null;
        #endregion

        #region ..const
        const int imageSize = 16;
        const int preMargin = 2;
        const int itemMargin = 2;
        //the border between the item with the border of the panel
        const int borderMargin = 2;
        const int groupMargin = 6;
        const int ItemHeight = 21;
        #endregion

        #region ..public properties
        /// <summary>
        /// gets the child groups
        /// </summary>
        public GroupCollection Groups
        {
            get
            {
                return this._groups;
            }
        }
        #endregion

        #region ..OnPaint
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            base.OnPaint(e);
            this._contentBounds.Clear();
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            LayoutManager manager = LayoutManager.FindManagerForControl(this);
            if (manager != null && manager.GradientBackground)
            {
                Color startColor = this.BackColor;
                Color endColor = this.BackColor;
                switch (this.Dock)
                {
                    case DockStyle.Bottom:

                        endColor = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.LightLight(this.BackColor)));
                        break;
                    case DockStyle.Right:
                        startColor = endColor = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.LightLight(this.BackColor)));
                        break;
                }
                using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, startColor, endColor, (this.IsVertical() ? System.Drawing.Drawing2D.LinearGradientMode.Vertical : System.Drawing.Drawing2D.LinearGradientMode.Horizontal)))
                    e.Graphics.FillRectangle(brush, rect);
            }
            else
            {
                using (Brush brush = new SolidBrush(ControlPaint.LightLight(ControlPaint.LightLight(this.BackColor))))
                    e.Graphics.FillRectangle(brush, rect);
            }
            //draw the group items
            rect = this.GetContentRegion();
            float pos = rect.Y - this.Top;
            if (!this.IsVertical())
                pos = rect.X - this.Left;
            pos += preMargin;
            foreach (Group group in this._groups)
            {
                this.DrawGroup(e.Graphics, group, ref pos);
                pos += groupMargin;
            }

            #region ..draw the border
            //draw the border
            //switch (this.Dock)
            //{
            //    case DockStyle.Left:
            //        e.Graphics.DrawLine(this.darkPen, this.Width - 1, 0, this.Width - 1, this.Height);
            //        break;
            //    case DockStyle.Right:
            //        e.Graphics.DrawLine(this.darkPen, 0, 0, 0, this.Height);
            //        break;
            //    case DockStyle.Top:
            //        e.Graphics.DrawLine(this.darkPen, 0, this.Height - 1, this.Width, this.Height - 1);
            //        break;
            //    case DockStyle.Bottom:
            //        e.Graphics.DrawLine(this.darkPen, 0, 0, this.Width, 0);
            //        break;
            //}
            #endregion
        }
        #endregion

        #region ..DrawGroup
        /// <summary>
        /// Draw the content of the group on the panel
        /// </summary>
        /// <param name="g">the graphics you want to draw contents on</param>
        /// <param name="pos">the pos start to draw the group</param>
        /// <param name="group">the group you want to draw</param>
        void DrawGroup(Graphics g,Group group, ref float pos)
        {
            int borderCornerSize = 2;
            using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
            {
                //cycle to draw the contents of the group
                foreach (Content c in group.Contents)
                {
                    sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
                    int left = borderMargin;
                    int right = this.Width;
                    int top = 0, bottom = 0;
                    float height = 1;

                    #region ..DrawBorder
                    using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
                    {
                        switch (this.Dock)
                        {
                            //right border
                            case DockStyle.Right:
                                left = borderMargin;
                                right = this.Width;
                                top = (int)pos;
                                //calcuate the height of the item associate with the content
                                height = g.MeasureString(c.Title, this.Font, 20000, sf).Width + imageSize + 2 * itemMargin;
                                //draw the border
                                path.AddLine(right,pos,left + borderCornerSize, pos);
                                path.AddLine(left + borderCornerSize, pos, left, pos + borderCornerSize);
                                path.AddLine(left, pos + borderCornerSize, left, pos + height - borderCornerSize);
                                path.AddLine(left, pos + height - borderCornerSize, left + borderCornerSize, pos + height);
                                path.AddLine(left + borderCornerSize, pos + height, right, pos + height);
                                bottom = (int)(pos + height);
                                break;
                            //left border
                            case DockStyle.Left:
                                left = 0;
                                right = this.Width - borderMargin;
                                top = (int)pos;
                                //calcuate the height of the item associate with the content
                                height = g.MeasureString(c.Title, this.Font, 20000, sf).Width + imageSize + 2 * itemMargin;
                                //draw the border
                                path.AddLine(left, pos, right - borderCornerSize, pos);
                                path.AddLine(right - borderCornerSize, pos, right, pos + borderCornerSize);
                                path.AddLine(right, pos + borderCornerSize, right, pos + height - borderCornerSize);
                                path.AddLine(right, pos + height - borderCornerSize, right - borderCornerSize, pos + height);
                                path.AddLine(right - borderCornerSize, pos + height,left, pos + height);
                                bottom = (int)(pos + height);
                                break;
                            //top border
                            case DockStyle.Top:
                                top = 0;
                                bottom = this.Height - borderMargin;
                                left = (int)pos;
                                //calcuate the height of the item associate with the content
                                height = g.MeasureString(c.Title, this.Font, 20000, sf).Width + imageSize + 2 * itemMargin;
                                //draw the border
                                path.AddLine(pos, top, pos, bottom - borderCornerSize);
                                path.AddLine(pos, bottom - borderCornerSize, pos + borderCornerSize, bottom);
                                path.AddLine(pos + borderCornerSize, bottom, pos + height - borderCornerSize, bottom);
                                path.AddLine(pos + height - borderCornerSize, bottom, pos + height, bottom - borderCornerSize);
                                path.AddLine(pos + height, bottom - borderCornerSize, pos + height, top);
                                right = (int)(pos + height);
                                break;
                            //bottom border
                            case DockStyle.Bottom:
                                top = borderMargin;
                                bottom = this.Height;
                                left = (int)pos;
                                //calcuate the height of the item associate with the content
                                height = g.MeasureString(c.Title, this.Font, 20000, sf).Width + imageSize + 2 * itemMargin;
                                //draw the border
                                path.AddLine(pos, bottom,pos, top + borderCornerSize);
                                path.AddLine(pos, top + borderCornerSize, pos + borderCornerSize, top);
                                path.AddLine(pos + borderCornerSize, top, pos + height - borderCornerSize, top);
                                path.AddLine(pos + height - borderCornerSize, top, pos + height, top + borderCornerSize);
                                path.AddLine(pos + height, top + borderCornerSize, pos + height, bottom);
                                right = (int)(pos + height);
                                break;
                        }
                        using (Brush brush = new SolidBrush(this.BackColor))
                            g.FillPath(brush, path);
                        g.DrawPath(this.darkPen, path);
                    }
                    #endregion

                    #region ..DrawContent
                    float pos1 = pos;
                    //add the item margin
                    pos1 += itemMargin;
                    pos += height;
                    this._contentBounds[c] = new RectangleF(left, top, right - left, bottom - top);
                    if (this.IsVertical())
                    {
                        //draw image
                        if (c.Image != null)
                        {
                            float center = left + ItemHeight / 2f;
                            float middle = pos1 + c.Image.Height / 2f;
                            RectangleF rect = new RectangleF(center - imageSize / 2f, middle - imageSize / 2f, c.Image.Width, c.Image.Height);
                            g.DrawImage(c.Image, rect, new RectangleF(0, 0, c.Image.Width, c.Image.Height), GraphicsUnit.Pixel);
                        }
                        pos1 += imageSize;

                        //draw text
                        sf.FormatFlags = StringFormatFlags.DirectionVertical | sf.FormatFlags;
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        g.DrawString(c.Title, this.Font, this.darkdarkBrush, new RectangleF(left, pos1, right - left, pos - pos1),sf);
                    }
                    else
                    {
                        //draw image
                        if (c.Image != null)
                        {
                            float center = pos1 + c.Image.Width / 2f;
                            float middle = top + ItemHeight / 2f;
                            RectangleF rect = new RectangleF(center - imageSize / 2f, middle - imageSize / 2f, c.Image.Width, c.Image.Height);
                            g.DrawImage(c.Image, rect, new RectangleF(0, 0, c.Image.Width, c.Image.Height), GraphicsUnit.Pixel);
                        }

                        pos1 += imageSize;

                        //draw text
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;
                        g.DrawString(c.Title, this.Font, this.darkdarkBrush, new RectangleF(pos1, top, pos - pos1, bottom - top), sf);
                    }
                    #endregion
                }
            }
        }
        #endregion

        #region ..IsVertical
        /// <summary>
        /// judge whether the layout of the control is vertical
        /// </summary>
        /// <returns></returns>
        internal bool IsVertical()
        {
            return this.Dock == DockStyle.Left || this.Dock == DockStyle.Right;
        }
        #endregion

        #region ..OnDockChanged
        protected override void OnDockChanged(EventArgs e)
        {
            base.OnDockChanged(e);
            if (this.IsVertical())
                this.Width = ItemHeight;
            else
                this.Height = ItemHeight;
        }
        #endregion

        #region ..OnClick
        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            SelectAndFadeContent();
        }
        #endregion

        #region ..FindGroupAtPoint
        /// <summary>
        /// find the group at the client point
        /// </summary>
        /// <param name="p">the client point</param>
        /// <param name="c">the content of the group</param>
        /// <returns></returns>
        Group FindGroupAtPoint(Point p,out Content content)
        {
            foreach (Content c in this._contentBounds.Keys)
            {
                RectangleF bounds = (RectangleF)this._contentBounds[c];
                if (bounds.Contains(p))
                {
                    content = c;
                    return c.ParentGroup;
                }
            }
            content = null;
            return null;
        }
        #endregion

        #region ..SelecteContentAndToView
        /// <summary>
        /// select the content and bring the content to view
        /// </summary>
        /// <param name="c"></param>
        void SelecteContentAndToView(Content c)
        {
            if (_selectedConent != c || this._autoHidePanel == null || this._autoHidePanel.Width == 0 || this._autoHidePanel.Height == 0 || !this._autoHidePanel.Visible )
            {
                _selectedConent = c;
                this.CreateAutoHidePanel();
                this._autoHidePanel.Visible = true;
                this._autoHidePanel.BringToFront();
                if (c.ParentGroup != null)
                {
                    this._autoHidePanel.Group = c.ParentGroup;
                    this._autoHidePanel.FadeIn(c.ParentGroup.Contents.IndexOf(c));
                    this._autoHidePanel.Group.Focus();
                }
            }
        }
        #endregion

        #region ..CreateAutoHidePanel
        /// <summary>
        /// create the a auto hide panel for this border panel
        /// </summary>
        /// <returns></returns>
        void CreateAutoHidePanel()
        {
            if (this._autoHidePanel == null)
                this._autoHidePanel = new AutoHidePanel(this);

            LayoutManager manager = LayoutManager.FindManagerForControl (this);
            Rectangle bounds = this.GetContentRegion();
            if (manager != null)
            {
                switch (this.Dock)
                {
                    case DockStyle.Left:
                        this._autoHidePanel.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Bottom;
                        this._autoHidePanel.Size = new Size(0, bounds.Height);
                        this._autoHidePanel.Location = new Point(bounds.Right, bounds.Top);
                        break;
                    case DockStyle.Right:
                        this._autoHidePanel.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
                        this._autoHidePanel.Size = new Size(0, bounds.Height);
                        this._autoHidePanel.Location = new Point(bounds.Left, bounds.Top);
                        break;
                    case DockStyle.Top:
                        this._autoHidePanel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                        this._autoHidePanel.Size = new Size(bounds.Width ,0);
                        this._autoHidePanel.Location = new Point(bounds.Left, bounds.Bottom);
                        break;
                    case DockStyle.Bottom:
                        this._autoHidePanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                        this._autoHidePanel.Size = new Size(bounds.Width, 0);
                        this._autoHidePanel.Location = new Point(bounds.Left, bounds.Top );
                        break;
                }

                if (!manager.Controls.Contains(this._autoHidePanel))
                    manager.Controls.Add(this._autoHidePanel);
                this._autoHidePanel.BringToFront();
            }
        }
        #endregion

        #region ..GetContentRegion
        /// <summary>
        /// get the content region
        /// </summary>
        /// <returns></returns>
        internal Rectangle GetContentRegion()
        {
            LayoutManager manager = this.Parent as LayoutManager;
            return manager.GetContentRegionForBorderPanel(this);
        }
        #endregion

        #region ..OnMouseMove
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if(this._autoHidePanel != null && this._autoHidePanel.Width > 0 && this._autoHidePanel.Height > 0)
                SelectAndFadeContent();
        }
        #endregion

        #region ..OnMouseHover
        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            SelectAndFadeContent();
        }
        #endregion

        #region ..SelectAndFadeContent
        void SelectAndFadeContent()
        {
            //find the group
            Point p = this.PointToClient(Control.MousePosition);
            Content c = null;
            Group group = this.FindGroupAtPoint(p, out c);
            if (group != null && c != null)
                this.SelecteContentAndToView(c);
        }
        #endregion

        #region ..OnMouseDown
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            //ensute the group focus after mouse down
            if (this._autoHidePanel != null && this._autoHidePanel.Visible && this._autoHidePanel.Group != null)
                this._autoHidePanel.Group.Focus();
        }
        #endregion

        #region ..events of Groups
        void _groups_Removed(int index, object value)
        {
            Group group = value as Group;
            if (this._autoHidePanel != null && this._autoHidePanel.Group == group)
            {
                this._autoHidePanel.Group = null;
                //set the size as empty
                if (this.IsVertical())
                    this._autoHidePanel.Width = 0;
                else
                    this._autoHidePanel.Height = 0;
            }

            if (group != null)
            {
                group.Contents.Inserted -= new Common.CollectionWithEvents.CollectionEventHandler(Contents_Inserted);
                group.Contents.Removed -= new Common.CollectionWithEvents.CollectionEventHandler(Contents_Inserted);
            }
            this.Visible = this._groups.Count > 0;
            this.Invalidate();
        }

        void _groups_Cleared()
        {
            this.Visible = this._groups.Count > 0;
            this.Invalidate();
        }

        void _groups_Inserted(int index, object value)
        {
            this.Visible = this._groups.Count > 0;
            Group group = value as Group;
            if (group != null)
            {
                //when the contents of the group change, invalidate the control
                group.Contents.Inserted += new Common.CollectionWithEvents.CollectionEventHandler(Contents_Inserted);
                group.Contents.Removed += new Common.CollectionWithEvents.CollectionEventHandler(Contents_Inserted);
            }
            this.Invalidate();
        }
        #endregion

        #region ..events of the Contents
        void Contents_Inserted(int index, object value)
        {
            this.Invalidate();
        }
        #endregion
    }
}
    