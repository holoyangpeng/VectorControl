using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace YP.CommonControl.Dock
{
    /// <summary>
    /// define the holder to store the group when auto hide
    /// </summary>
    internal class AutoHidePanel:Common.BaseControl
    {
        #region ..Constructor
        public AutoHidePanel(BorderPanel borderPanel)
        {
            this.AutoSize = false;
            this._timer.Interval = 25;
            this._timer.Tick += new EventHandler(_timer_Tick);
            //SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            this._borderPanel = borderPanel;
        }
        #endregion

        #region ..private fields
        Group _group = null;
        bool fadeIn = true;
        System.Windows.Forms.Timer _timer = new Timer();
        Size fadeSize = Size.Empty;
        bool _fading = false;
        BorderPanel _borderPanel = null;
        Content _previousContent = null;
        //if start to split
        bool _split = false;
        //if press the mouse down
        bool _mouseDown = false;
        //remember the reverse rectangle
        Rectangle _reverseRect = Rectangle.Empty;
        //the reverse size
        int _reverseSize = 3;
        bool start = false;
        #endregion

        #region ..const fields
        const int FadeInstant = 25;
        const int SplitterSize = 4;
        #endregion

        #region ..public properties
        /// <summary>
        /// gets a value indicates whether the panel is fading
        /// </summary>
        public bool Fading
        {
            get
            {
                return this._fading;
            }
        }

        /// <summary>
        /// gets or sets the group you want to slide show
        /// </summary>
        public Group Group
        {
            set
            {
                if (this._group != value)
                {
                    //remove the previous group
                    if (this._group != null && this.Controls.Contains(this._group))
                    {
                        this.Controls.Remove(this._group);
                    }
                    this._group = value;
                    //add the group to the controls
                    if (this._group != null && !this.Controls.Contains(this._group))
                    {
                        this.fadeSize = this._group.LastNormalSize;
                        this.SuspendLayout();
                        switch (this._borderPanel.Dock)
                        {
                            case DockStyle.Left:
                                this.fadeSize = new Size(this.fadeSize.Width + SplitterSize, this.fadeSize.Height);
                                this.Padding = new Padding(0, 0, SplitterSize, 0);
                                break;
                            case DockStyle.Right:
                                this.fadeSize = new Size(this.fadeSize.Width + SplitterSize, this.fadeSize.Height);
                                this.Padding = new Padding(SplitterSize, 0, 0, 0);
                                break;
                            case DockStyle.Top:
                                this.fadeSize = new Size(this.fadeSize.Width, this.fadeSize.Height + SplitterSize);
                                this.Padding = new Padding(0, 0, 0, SplitterSize);
                                break;
                            case DockStyle.Bottom:
                                this.fadeSize = new Size(this.fadeSize.Width, this.fadeSize.Height + SplitterSize);
                                this.Padding = new Padding(0, SplitterSize, 0, 0);
                                break;
                        }
                        this._group.Location = new Point(this.Padding.Left, this.Padding.Top);
                        if (this._borderPanel.IsVertical())
                            this._group.Size = new Size(this._group.LastNormalSize.Width, this.Height - this.Padding.Top - this.Padding.Bottom);
                        else
                            this._group.Size = new Size(this.Width - this.Padding.Left - this.Padding.Right, this._group.LastNormalSize.Height);
                        this._group.Dock = DockStyle.Fill;
                        this._group.ShowExpand = this._group.ShowMaxMin = false;
                        this._group.ShowTabButton = false;
                        this.Controls.Add(this._group);
                        this.ResumeLayout(true);
                    }
                }
            }
            get
            {
                return this._group;
            }
        }
        #endregion

        #region ..FadeIn
        /// <summary>
        /// fade show the group
        /// </summary>
        /// <param name="index">the current index of the selected content </param>
        public void FadeIn(int index)
        {
            if (this._group != null)
                this._group.SelectedIndex = index;

            Content c = this._group.Contents[index];
            if (this._previousContent != c)
            {
                if (this._borderPanel.IsVertical())
                    this.Width = 0;
                else
                    this.Height = 0;
            }
            this._previousContent = c;
            this.fadeIn = true;
            this._fading = true;
            this._timer.Start();
        }
        #endregion

        #region ..FadeOver
        /// <summary>
        /// hide the panel
        /// </summary>
        public void FadeOver()
        {
            if (this.fadeIn)
            {
                this._fading = true;
                this.fadeIn = false;
                this._timer.Start();
            }
        }
        #endregion

        #region .._timer_Tick
        void _timer_Tick(object sender, EventArgs e)
        {
            Application.DoEvents();
            if (!this.start)
            {
                this.start = true;
                return;
            }
            this.SuspendLayout();
            Size size = fadeSize;
            int x = this.Left;
            int y = this.Top;
            int width = this.Width;
            int height = this.Height;
            Rectangle bounds = this._borderPanel.GetContentRegion();
            this._group.Dock = DockStyle.None;
            switch (this._group.LayoutStyle)
            {
                   //left
                case LayoutStyle.Left:
                    if (this.fadeIn)
                        width += FadeInstant;
                    else
                        width -= FadeInstant;
                    width = (int)Math.Min(Math.Max(0,width), size.Width);
                    x = bounds.Right;
                    y = bounds.Top;
                    height = bounds.Height;
                    if (width == size.Width || width == 0)
                        this._fading = false;
                    break;
                    //right
                case LayoutStyle.Right:
                    if (this.fadeIn)
                        width += FadeInstant;
                    else
                        width -= FadeInstant;
                    width = (int)Math.Min(Math.Max(0, width), size.Width);
                    x = bounds.Left - width;
                    y = bounds.Top;
                    height = bounds.Height;
                    if (width == size.Width || width == 0)
                        this._fading = false;
                    break;
                    //top
                case LayoutStyle.Top:
                    if (this.fadeIn)
                        height += FadeInstant;
                    else
                        height -= FadeInstant;
                    height = (int)Math.Min(Math.Max(0, height), size.Height);
                    x = bounds.Left;
                    y = bounds.Bottom;
                    width = bounds.Width;
                    if (height == size.Height || height == 0)
                        this._fading = false;
                    break;
                    //bottom
                case LayoutStyle.Bottom:
                    if (this.fadeIn)
                        height += FadeInstant;
                    else
                        height -= FadeInstant;
                    height = (int)Math.Min(Math.Max(0, height), size.Height);
                    x = bounds.Left;
                    y = bounds.Top - height;
                    width = bounds.Width;
                    if (height == size.Height || height == 0)
                        this._fading = false;
                    break;
            }
            this.SetBounds(x, y, width, height);
            this._group.Location = new Point(this.Padding.Left, this.Padding.Top);
      //      this._group.Size = this._group.LastNormalSize;

            if (!this.Fading)
            {
                this._timer.Stop();
                start = false;
                this._group.Dock = DockStyle.Fill;
                this._group.Invalidate(true);
            }
            this.ResumeLayout(true);
            
        }
        #endregion

        #region .._group_ActivedChanged
        void _group_ActivedChanged(object sender, EventArgs e)
        {
            Group group = sender as Group;
            if (this.Width > 0 && this.Height > 0 && !group.IsActived && !this._borderPanel.Capture )
                this.FadeOver();
        }
        #endregion

        #region ..OnControlAdded
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            //if the control is group, add the activedchanged event handler
            if(e.Control is Group)
                (e.Control as Group).ActivedChanged += new EventHandler(_group_ActivedChanged);
        }
        #endregion

        #region ..OnControlRemoved
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            //if the control is group, remove the activedchanged event handler
            if (e.Control is Group)
                (e.Control as Group).ActivedChanged -= new EventHandler(_group_ActivedChanged);
        }
        #endregion

        #region ..OnPaint
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            switch (this._borderPanel.Dock)
            {
                case DockStyle.Left:
                    e.Graphics.DrawLine(this.darkPen, this.Width - 2, 0, this.Width - 2, this.Height);
                    e.Graphics.DrawLine(this.darkdarkPen, this.Width - 1, 0, this.Width - 1, this.Height);
                    break;
                case DockStyle.Right:
                    e.Graphics.DrawLine(lightlightPen, 1, 0, 1, this.Height);
                    break;
                case DockStyle.Top:
                    e.Graphics.DrawLine(this.darkPen, 0, this.Height - 2, this.Width, this.Height - 2);
                    e.Graphics.DrawLine(this.darkdarkPen, 0, this.Height - 1, this.Width, this.Height - 1);
                    break;
                case DockStyle.Bottom:
                    e.Graphics.DrawLine(this.lightlightPen, 0, 1, this.Width, 1);
                    break;
            }
            
        }
        #endregion

        #region ..OnMouseDown
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this._mouseDown = e.Button == MouseButtons.Left;
            this._reverseRect = Rectangle.Empty;
        }
        #endregion

        #region ..OnMouseMove
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            //split
            if (e.Button == MouseButtons.None)
            {
                this._split = false;
                Rectangle rect = Rectangle.Empty;

                //get the bounds
                switch (this._borderPanel.Dock)
                {
                    case DockStyle.Left:
                        rect = new Rectangle(this.Width - SplitterSize, 0, SplitterSize, this.Height);
                        break;
                    case DockStyle.Right:
                        rect = new Rectangle(0, 0, SplitterSize, this.Height);
                        break;
                    case DockStyle.Top:
                        rect = new Rectangle(0, this.Height - SplitterSize, this.Width, SplitterSize);
                        break;
                    case DockStyle.Bottom:
                        rect = new Rectangle(0, 0, this.Width, SplitterSize);
                        break;
                }

                //if the point in the split bounds
                Point p = new Point(e.X, e.Y);
                if (rect.Contains(p))
                {
                    this.Cursor = this._borderPanel.IsVertical() ? Cursors.VSplit : Cursors.HSplit;
                    this._split = true;
                }
                else
                    this.Cursor = Cursors.Default;
            }
            else if(this._split && this._mouseDown)
            {
                //find the layout manager
                LayoutManager manager = LayoutManager.FindManagerForControl(this);
                Rectangle bounds = manager.RectangleToScreen(new Rectangle(0, 0, manager.Width, manager.Height));
                ControlPaint.DrawReversibleFrame(this._reverseRect, Color.Black, FrameStyle.Thick);
                Point p = this.PointToScreen(new Point(e.X, e.Y));
                //if vertical
                if (this._borderPanel.IsVertical())
                {
                    int x = (int)Math.Max(bounds.X + ZoneContainer.MinSize, Math.Min(bounds.Right - ZoneContainer.MinSize, p.X));
                    Point p1 = this.PointToScreen(new Point(0,0));
                    Point p2 = this.PointToScreen(new Point(0,this.Height));
                    this._reverseRect = new Rectangle(x - this._reverseSize / 2, p1.Y, this._reverseSize, p2.Y - p1.Y);
                }
                else
                {
                    int y = (int)Math.Max(bounds.Y + ZoneContainer.MinSize, Math.Min(bounds.Bottom - ZoneContainer.MinSize, p.Y));
                    Point p1 = this.PointToScreen(new Point(0, 0));
                    Point p2 = this.PointToScreen(new Point(this.Width, this.Height));
                    this._reverseRect = new Rectangle(p1.X, y - this._reverseSize / 2, p2.X - p1.X, this._reverseSize);
                }

                ControlPaint.DrawReversibleFrame(this._reverseRect, Color.Black, FrameStyle.Thick);
            }
        }
        #endregion

        #region ..OnMouseUp
        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this._split && this._mouseDown)
            {
                ControlPaint.DrawReversibleFrame(this._reverseRect, Color.Black, FrameStyle.Thick);
                this._reverseRect = Rectangle.Empty;
                LayoutManager manager = LayoutManager.FindManagerForControl(this);
                Rectangle bounds = manager.RectangleToScreen(new Rectangle(0, 0, manager.Width, manager.Height));
                Point p = this.PointToScreen(new Point(e.X, e.Y));
                //calculate the result point
                int x = (int)Math.Max(bounds.X + ZoneContainer.MinSize, Math.Min(bounds.Right - ZoneContainer.MinSize, p.X));
                int y = (int)Math.Max(bounds.Y + ZoneContainer.MinSize, Math.Min(bounds.Bottom - ZoneContainer.MinSize, p.Y));
                this.SuspendLayout();
                bounds = this.Bounds;
                int width = bounds.Width;
                int height = bounds.Height;
                switch (this._borderPanel.Dock)
                {
                        //left
                    case DockStyle.Left:
                        this.SetBounds(bounds.X, bounds.Y, x - this.PointToScreen(new Point(0, 0)).X, bounds.Height);
                        this.fadeSize = this.Bounds.Size;
                        break;
                        //right
                    case DockStyle.Right:
                        width = this.PointToScreen(new Point(this.Width, this.Height)).X - x;
                        this.SetBounds(bounds.Right - width, bounds.Y, width, bounds.Height);
                        this.fadeSize = this.Bounds.Size;
                        break;
                        //top
                    case DockStyle.Top:
                        this.SetBounds(bounds.X, bounds.Y, bounds.Width, y - this.PointToScreen(new Point(0, 0)).Y);
                        this.fadeSize = this.Bounds.Size;
                        break;
                        //bottom
                    case DockStyle.Bottom:
                        height = this.PointToScreen(new Point(0, this.Height)).Y - y;
                        this.SetBounds(bounds.X, bounds.Bottom - height, bounds.Width, height);
                        fadeSize = this.Bounds.Size;
                        break;
                }
                if (this._group != null)
                    this._group.LastNormalSize = this._group.Size;
                this.ResumeLayout(true);
            }
            base.OnMouseUp(e);
            this.Cursor = Cursors.Default;
        }
        #endregion
    }
}
