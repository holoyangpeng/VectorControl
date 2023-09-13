using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
using YP.CommonControl;

namespace YP.CommonControl.Dock
{
    /// <summary>
    /// define a content group
    /// </summary>
    public class Group:Common.BaseControl,IMessageFilter
    {
        #region ..define the button click style
        public enum ButtonClickStyle
        {
            /// <summary>
            /// Make the group max
            /// </summary>
            Max,
            /// <summary>
            /// Expand the group
            /// </summary>
            Expand,
            /// <summary>
            /// Collapse the group
            /// </summary>
            Collapse,
            /// <summary>
            /// Toggle the auto hide status
            /// </summary>
            AutoHide,
            /// <summary>
            /// Close the group
            /// </summary>
            Close,
            /// <summary>
            /// None
            /// </summary>
            None
        }
        #endregion

        #region ..Constructor
        internal Group()
        {
            //add the control into the message filter
            Application.AddMessageFilter(this);

            this.AutoSize = false;
            this._contents = new ContentCollection(this);
            //create the tab control
            this._tabControl = new YP.CommonControl.TabControl.TabControl();
            this._tabControl.ShowControlBox =  false;
            this._tabControl.ShowNavigateButton = true;
            this._tabControl.PositionTop = false;
            this._tabControl.IDEBorder = false;
            (this._tabControl.TabPages as TabControl.TabPageCollection).Removed += new Common.CollectionWithEvents.CollectionEventHandler(Group_Removed);
            this.Controls.Add(this._tabControl);

            this.DefineFont(SystemInformation.MenuFont);

            //reset the location of the size of the tab control
            this.UpdateTableControl();

            //create the control box
            this.CreateControlBox();

            //create the events for the tab control
            this._tabControl.SelectedIndexChanged += new EventHandler(_tabControl_SelectedIndexChanged);

            //update the min max
            this.UpdateMinMaxStatus();

            //create the event for the contents
            this.Contents.Inserted += new Common.CollectionWithEvents.CollectionEventHandler(Contents_Inserted);
            this.Contents.Removed += new Common.CollectionWithEvents.CollectionEventHandler(Contents_Removed);
            this.Contents.Cleared += new Common.CollectionWithEvents.ClearEventHandler(Contents_Cleared);
            this.AutoSize = false;
            this.Margin = new Padding(0, 0, 0, 0);

            SetStyle(ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
        }

        void Group_Removed(int index, object value)
        {
            if (this._tabControl.TabPages.Count == 0 && this.ParentGroups != null)
                this.ParentGroups.Remove(this);
        }

        void _tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Text = this._tabControl.SelectedTab.Text;
            this.InvalidateTitle();
        }
        #endregion 

        #region ..Dispose
        protected override void Dispose(bool disposing)
        {
            Application.RemoveMessageFilter(this);
            base.Dispose(disposing);
        }
        #endregion

        #region ..event
        /// <summary>
        /// define the handler of button click
        /// </summary>
        public delegate void ButtonClickEventHandler(object sender, ButtonClickStyle click);

        /// <summary>
        /// occurs when click the button 
        /// </summary>
        public event ButtonClickEventHandler ButtonClick;

        /// <summary>
        /// occurs when the status of active changed 
        /// </summary>
        internal event EventHandler ActivedChanged;
        #endregion

        #region ..const fields
        //define the height of the caption
        internal static int titleHeight = 16;
        //define the margin between control and title
        internal static readonly int topmargin = 0;
        //define the size of the control box
        Size buttonSize = new Size(14, 13);
        #endregion

        #region ..private fields
        ContentCollection _contents = null;
        YP.CommonControl.TabControl.TabControl _tabControl = null;
        Common.PopupButton btnlock;
        Common.PopupButton btnclose;
        Common.PopupButton btnExpand;
        Common.PopupButton btnMax;
        //if show the title
        bool _showTitle = true;
        //if show the control box
        bool _showControlBox = true;
        //rememble the old status of the actived
        bool _oldActived = false;
        //rememble the old size of the control
        Size _oldSize = Size.Empty;
        bool maxstate = true;
        //remember the parent collection
        GroupCollection _parentGroups = null;
        LayoutStyle _layout = LayoutStyle.Left;
        //indicates whether the group is in auto hide
        bool _autoHide = false;

        Size _displaySize = Size.Empty;
        //remeber the intialize index of the group
        int _groupIndex = 0;
        Size preSize = Size.Empty;
        #endregion

        #region ..public properties
        /// <summary>
        /// 获取或设置一个值，该值只是当前组是否成最大化显示
        /// </summary>
        internal bool MaxState
        {
            set
            {
                this.maxstate = value;
                this.btnMax.Visible = !this.maxstate && this._showControlBox;
                if (this.maxstate)
                    this._oldSize = this.preSize;
            }
            get
            {
                return this.maxstate;
            }
        }

        /// <summary>
        /// sets or gets a value indicates whether show the title
        /// </summary>
        internal bool ShowTitle
        {
            get
            {
                return this._showTitle;
            }
            set
            {
                if (this._showTitle != value)
                {
                    this._showTitle = value;
                    UpdateButtonStatus();
                    this.Invalidate();
                }
            }
        }

        /// <summary>
        /// gets a value indicates whether the group is in in auto hide style
        /// </summary>
        internal bool AutoHide
        {
            get
            {
                return this._autoHide;
            }
        }

        /// <summary>
        /// sets or gets a value indicates whether show the button of the tab control
        /// </summary>
        internal bool ShowTabButton
        {
            set
            {
                this._tabControl.ShowTabButton = value;
            }
            get
            {
                return this._tabControl.ShowTabButton;
            }
        }

        /// <summary>
        /// 控制是否显示ControlBox;
        /// </summary>
        internal bool ShowControlBox
        {
            get
            {
                return this._showControlBox;
            }
            set
            {
                if (this._showControlBox != value)
                {
                    this._showControlBox = value;
                    this.UpdateMinMaxStatus();
                    this.UpdateControlBox();
                }
            }
        }

        /// <summary>
        /// gets the current selected index of the tab control
        /// </summary>
        internal int SelectedIndex
        {
            get
            {
                return this._tabControl.SelectedIndex;
            }
            set
            {
                this._tabControl.SelectedIndex = value;
            }
        }

        /// <summary>
        /// gets or sets the selected content
        /// </summary>
        public Content SelectedContent
        {
            set
            {
                int index = this.Contents.IndexOf(value);
                this.SelectedIndex = index;
            }
            get
            {
                return (this._tabControl.SelectedTab as TabControl.TabPage).Tag as Content;
            }
        }

        /// <summary>
        /// judge whether the current status if min
        /// </summary>
        internal bool IsMinimize
        {
            get
            {
                return this.Size.Height <= this.MinHeight;
            }
        }

        /// <summary>
        /// gets the min height of the group
        /// </summary>
        internal int MinHeight
        {
            get
            {
                return titleHeight + 2; 
            }
        }

        /// <summary>
        /// sets or gets a value indicates whether display the expand button
        /// </summary>
        internal bool ShowExpand
        {
            set
            {
                this.btnExpand.Visible = value;
            }
            get
            {
                return this.btnExpand.Visible;
            }
        }

        /// <summary>
        /// sets or gets a value indicates whether show the btn max
        /// </summary>
        internal bool ShowMaxMin
        {
            set
            {
                this.btnMax.Visible = value;
            }
            get
            {
                return this.btnMax.Visible;
            }
        }

        /// <summary>
        /// get the display size
        /// </summary>
        internal Size DisplaySize
        {
            get
            {
                return this._displaySize;
            }
            set
            {
                this._displaySize = value;
            }
        }

        /// <summary>
        /// get the size of the control before it is minimize
        /// </summary>
        internal Size LastNormalSize
        {
            get
            {
                if (this._oldSize.IsEmpty && this._parentGroups != null)
                    this._oldSize = this.DisplaySize;
                return this._oldSize;
            }
            set
            {
                this._oldSize = value;
            }
        }

        /// <summary>
        /// gets the parent group collection
        /// </summary>
        internal GroupCollection ParentGroups
        {
            get
            {
                return _parentGroups;
            }
            set
            {
                this._parentGroups = value;
            }
        }

        /// <summary>
        /// gets the collection of the contents
        /// </summary>
        public ContentCollection Contents
        {
            get
            {
                return this._contents;
            }
        }

        /// <summary>
        /// get the parent zone
        /// </summary>
        internal Zone ParentZone
        {
            get
            {
                if (this.Parent is Zone)
                    return this.Parent as Zone;
                return null;
            }
        }

        /// <summary>
        /// 判断当前控件是否处于活动状态
        /// </summary>
        internal bool IsActived
        {
            get
            {
                bool focused = this.ContainsFocus;
                return focused;
            }
        }
        #endregion

        #region ..private (internal) properties
        /// <summary>
        /// gets or sets the intialize index of the group
        /// </summary>
        internal int GroupIndex
        {
            set
            {
                this._groupIndex = value;
            }
            get
            {
                return this._groupIndex;
            }
        }

        /// <summary>
        /// sets or gets the layout of the group
        /// </summary>
        internal LayoutStyle LayoutStyle
        {
            get
            {
                return this._layout;
            }
            set
            {
                this._layout = value;
            }
        }
        #endregion

        #region ..UpdateTableControl
        /// <summary>
        /// reset the tabcontrol
        /// </summary>
        internal void UpdateTableControl()
        {
            if (this._tabControl != null)
            {
                int title = this._showTitle ? titleHeight : 0;
                this._tabControl.Location = new Point(0, title + topmargin + 1);
                this._tabControl.Size = new Size(this.Width, this.Height - title - topmargin - 1);
                this._tabControl.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Top;
            }
        }
        #endregion

        #region ..OnPaint
        /// <summary>
        /// OnPaint
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            //control the tab control's button
            this._tabControl.Visible = !this.IsMinimize;
            if (this._parentGroups != null)
                this.btnMax.Visible = this.ShowMaxMin && this._parentGroups.Count > 1;
            this._tabControl.ShowTabButton = this._tabControl.ShowTabButton && this._contents.Count > 1;
            using (GraphicsPath path = this.GetTitleRegion())
            {
                bool focused = this.IsActived;
                SynControlBoxStatus(focused);
                if (path != null)
                {
                    Color color = SystemColors.ControlText;
                    Color topColor = this.BackColor;
                    if (focused)
                    {
                        topColor = Color.FromArgb(230, SystemColors.Highlight);
                        color = SystemColors.HighlightText;
                    }
                    Color lightColor = this.LightLightLightLightColor;
                    RectangleF rect = path.GetBounds();
                    using (Brush temp = new LinearGradientBrush(rect, lightColor, topColor, LinearGradientMode.Vertical))
                    {
                        e.Graphics.FillPath(Brushes.White, path);
                        e.Graphics.FillPath(temp, path);
                    }

                    int left = this.btnlock.Left;
                    if (this._showControlBox)
                        left = this.btnExpand.Left;
                    string text = this.Text;
                    using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
                    {
                        sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
                        sf.LineAlignment = StringAlignment.Far;
                        e.Graphics.DrawString(text, this.Font, new SolidBrush(color), new RectangleF(5, topmargin + 1, left - 5, titleHeight - 1), sf);
                    }
                    using (Pen pen = new Pen(ControlPaint.Dark(this.BackColor)))
                        e.Graphics.DrawPath(pen, path);
                }
            }
        }
        #endregion

        #region ..GetTitleRegion
        /// <summary>
        /// get the region of the title
        /// </summary>
        /// <returns></returns>
        GraphicsPath GetTitleRegion()
        {
            if (!this._showTitle)
                return null;
            GraphicsPath gp = new GraphicsPath();
            RectangleF rect = new RectangleF(1, topmargin, this.Width - 2, titleHeight);
            float rx = 1, ry = 1;
            float a = rect.X + rect.Width - rx;
            gp.AddLine(rect.X + rx, rect.Y, a, rect.Y);
            gp.AddArc(a - rx, rect.Y, rx * 2, ry * 2, 270, 90);

            float right = rect.X + rect.Width;	// rightmost X
            float b = rect.Y + rect.Height - ry;

            gp.AddLine(right, rect.Y + ry, right, b);
            gp.AddArc(right - rx * 2, b - ry, rx * 2, ry * 2, 0, 90);

            gp.AddLine(right - rx, rect.Y + rect.Height, rect.X + rx, rect.Y + rect.Height);
            gp.AddArc(rect.X, b - ry, rx * 2, ry * 2, 90, 90);

            gp.AddLine(rect.X, b, rect.X, rect.Y + ry);
            gp.AddArc(rect.X, rect.Y, rx * 2, ry * 2, 180, 90);

            gp.CloseFigure();
            return gp;
        }
        #endregion

        #region ..DefineFont
        /// <summary>
        /// calcuate the title height according to current font
        /// </summary>
        void DefineFont(Font font)
        {
            titleHeight = font.Height + 2;
            //update the tab control
            this.UpdateTableControl();
        }
        #endregion

        #region ..CreateControlBox
        /// <summary>
        /// 定义控制按钮
        /// </summary>
        void CreateControlBox()
        {
            this.btnMax = new Common.PopupButton();
            //this.btnMax.ToolTipText = resourcemanager.GetResourceString("YP.CommonControl.DockControl.ContentGroup.btnMax.ToolTipText");//"Maximum";
            this.btnMax.Location = new Point(this.Width - 60, topmargin + 1);
//            this.btnMax.GotFocus += new EventHandler(btnMax_GotFocus);
            this.btnMax.Size = this.buttonSize;
            this.Controls.Add(this.btnMax);
            this.btnMax.Visible = false;

            this.btnExpand = new Common.PopupButton();
            //this.btnExpand.ToolTipText = resourcemanager.GetResourceString("YP.CommonControl.DockControl.ContentGroup.btnExpand.ToolTipText");//"Expand/Collapse";
            this.btnExpand.Location = new Point(this.Width - 45, topmargin + 1);
            this.btnExpand.Size = this.buttonSize;
 //           this.btnExpand.GotFocus += new EventHandler(btnMax_GotFocus);
            this.Controls.Add(this.btnExpand);
            this.btnExpand.Visible = false;

            this.btnlock = new Common.PopupButton();
            //this.btnlock.ToolTipText = resourcemanager.GetResourceString("YP.CommonControl.DockControl.ContentGroup.btnlock.ToolTipText");//"Auto Hide";
            this.btnlock.Location = new Point(this.Width - 30, topmargin + 1);
            this.btnlock.Size = buttonSize;
 //           this.btnlock.GotFocus += new EventHandler(btnMax_GotFocus);
            this.Controls.Add(this.btnlock);

            this.btnclose = new Common.PopupButton();
            //this.btnclose.ToolTipText = resourcemanager.GetResourceString("YP.CommonControl.DockControl.ContentGroup.btnclose.ToolTipText");//"Close";
            this.btnclose.Location = new Point(this.Width - 15, topmargin + 1);
 //           this.btnclose.GotFocus += new EventHandler(btnMax_GotFocus);
            this.btnclose.Size = buttonSize;
            this.btnExpand.Anchor = this.btnMax.Anchor = this.btnlock.Anchor = this.btnclose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnMax.Click += new EventHandler(this.OnButtonClick);
            this.btnExpand.Click += new EventHandler(this.OnButtonClick);
            this.btnlock.Click += new EventHandler(this.OnButtonClick);
            this.btnclose.Click += new EventHandler(this.OnButtonClick);
            this.Controls.Add(this.btnclose);

            if (LayoutManager._imageList != null)
            {
                this.btnlock.Image = LayoutManager._imageList.Images[3];
                this.btnclose.Image = LayoutManager._imageList.Images[0];
            }
        }    
        #endregion

        #region ..SynControlBoxStatus
        /// <summary>
        /// set the status of the contorl box
        /// </summary>
        /// <param name="actived"></param>
        void SynControlBoxStatus(bool actived)
        {
            bool actived1 = this.btnMax.ImageAttributes != null;
            if (actived != actived1)
                this.UpdateControlBox();
        }
        #endregion

        #region ..UpdateControlBox
        /// <summary>
        /// update the control box so that they looks good
        /// </summary>
        void UpdateControlBox()
        {
            bool focused = this.IsActived;
            //if focus, reverse the image's button
            if (focused)
            {
                ImageAttributes atr = this.CreateImageMap();
                this.btnclose.ImageAttributes = atr;
                this.btnlock.ImageAttributes = atr;
                this.btnExpand.ImageAttributes = atr;
                this.btnMax.ImageAttributes = atr;
            }
            else
            {
                this.btnclose.ImageAttributes = null;
                this.btnlock.ImageAttributes = null;
                this.btnExpand.ImageAttributes = null;
                this.btnMax.ImageAttributes = null;
            }
        }
        #endregion

        #region ..CreateImageMap
        /// <summary>
        /// 创建控制按钮色彩转换信息
        /// </summary>
        System.Drawing.Imaging.ImageAttributes CreateImageMap()
        {
            System.Drawing.Imaging.ImageAttributes imgatr = new System.Drawing.Imaging.ImageAttributes();
            ColorMap map = new ColorMap();
            map.OldColor = Color.Black;
            map.NewColor = SystemColors.ActiveCaptionText;
            imgatr.SetRemapTable(new ColorMap[] { map }, ColorAdjustType.Bitmap);
            return imgatr;
        }
        #endregion

        #region ..UpdateMinMaxStatus
        /// <summary>
        /// update the status of min or max
        /// </summary>
        void UpdateMinMaxStatus()
        {
            if (LayoutManager._imageList != null)
			{
                this.btnMax.Image = LayoutManager._imageList.Images[1];
                if (this.IsMinimize)
                    this.btnExpand.Image = LayoutManager._imageList.Images[7];
				else
                    this.btnExpand.Image = LayoutManager._imageList.Images[6];
			}
        }
        #endregion

        #region ..OnResize
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            //if min size, hide the tabcontrol
            this._tabControl.Visible = !this.IsMinimize;

            //update the min or max 
            UpdateMinMaxStatus();

            //remember the last display size of the group
            bool record = this.Parent != null && this.Width > 0 && this.Height > this.MinHeight;
            //if parent is auto hide panel, remember only when the panel is not in fading
            if (Parent is AutoHidePanel || this.MaxState)
                record = false;
            if (record)
            {
                preSize = this._oldSize;
                this._oldSize = this.Size;
            }
        }
        #endregion

        #region ..OnButtonClick
        protected virtual void OnButtonClick(ButtonClickStyle style)
        {
            if (this.ButtonClick != null)
                this.ButtonClick(this, style);
        }

        void OnButtonClick(object sender, EventArgs e)
        {
            Common.PopupButton btn = sender as Common.PopupButton;
            ButtonClickStyle style = ButtonClickStyle.None;
            if (btn == this.btnclose)
            {
                LayoutManager manager = LayoutManager.FindManagerForControl(this);

                if (manager != null && (this._tabControl.SelectedTab as TabControl.TabPage).Tag is Content)
                    manager.HideContent((this._tabControl.SelectedTab as TabControl.TabPage).Tag as Content);  
            }
            else if (btn == this.btnlock)
            {
                LayoutManager manager = LayoutManager.FindManagerForControl(this);
                if (manager != null)
                    manager.ToggleAutoHide(this);
            }
            else if (btn == this.btnMax)
            {
                style = ButtonClickStyle.Max;
            }
            else if (btn == this.btnExpand)
            {
                style = ButtonClickStyle.Expand;
                if (!this.IsMinimize)
                    style = ButtonClickStyle.Collapse;
            }
            if (style != ButtonClickStyle.None)
                this.OnButtonClick(style);
        }
        #endregion

        #region ..events for the contents
        void Contents_Inserted(int index, object value)
        {
            Content c = value as Content;
            Control control = (c.Control == null ? new Label() : c.Control);
            TabControl.TabPage page = new YP.CommonControl.TabControl.TabPage(c.Title, control, c.Image);
            page.Tag = c;
            c.Tag = page;
            this._tabControl.TabPages.Insert(index, page);
            c.Changed += new EventHandler(c_ControlChanged);
            this.ShowTabButton = this.Contents.Count > 1;
        }

        void Contents_Cleared()
        {
            for (int i = 0; i < this._tabControl.TabPages.Count; i++)
            {
                TabControl.TabPage page = this._tabControl.TabPages[i] as TabControl.TabPage;
                (page.Tag as Content).Changed -= new EventHandler(c_ControlChanged);
                this._tabControl.TabPages.RemoveAt(0);
                i--;
            }
            this.ShowTabButton = this.Contents.Count > 1;
        }

        void Contents_Removed(int index, object value)
        {
            Content c = value as Content;
            if (c != null)
            {
                TabControl.TabPage page = c.Tag as TabControl.TabPage;
                if (page != null)
                    this._tabControl.TabPages.Remove(page);
                c.Changed -= new EventHandler(c_ControlChanged);
            }
            this.ShowTabButton = this.Contents.Count > 1;
        }
        #endregion

        #region ..UpdateTabPagesWhenContentsChanged
        /// <summary>
        /// update (or create) the tab pages when the contents changed
        /// </summary>
        void UpdateTabPagesWhenContentsChanged()
        {

        }
        #endregion

        #region ..InvalidateTitle
        /// <summary>
        /// Invalidate the title region
        /// </summary>
        internal void InvalidateTitle()
        {
            if (this.IsDisposed)
                return;
            Rectangle rect = new Rectangle(0, topmargin - 1, this.Width, titleHeight + 3);
            this.Invalidate(rect);
            this.UpdateControlBox();
        }
        #endregion

        #region ..PreFilterMessage
        // Summary:
        //     Filters out a message before it is dispatched.
        //
        // Parameters:
        //   m:
        //     The message to be dispatched. You cannot modify this message.
        //
        // Returns:
        //     true to filter the message and stop it from being dispatched; false to allow
        //     the message to continue to the next filter or control.
        public bool PreFilterMessage(ref Message m)
        {
            if (!this.IsDisposed && this.Visible )
            {
                if (this._oldActived != this.IsActived)
                {
                    this.InvalidateTitle();
                    OnActivedChanged();
                }
                this._oldActived = this.IsActived;
            }
            return false;
        }
        #endregion

        #region ..PerformButtonClick
        /// <summary>
        /// invokd the button click
        /// </summary>
        public void PerformButtonClick(ButtonClickStyle style)
        {
            this.OnButtonClick(style);
        }
        #endregion

        #region ..MakeGroupFill
        /// <summary>
        /// meke the group fill the white area of it's parent zone
        /// </summary>
        public void MakeGroupFill()
        {
            if (this.ParentZone != null)
                this.ParentZone.FillGroup(this);
        }
        #endregion

        #region ..SetAutoHideStatus
        /// <summary>
        /// set the value indicates whether the group is auto hide
        /// </summary>
        /// <param name="autoHide"></param>
        internal void SetAutoHideStatus(bool autoHide)
        {
            //record the old parent before auto hide so that it can revert
            this._autoHide = autoHide;
            if(this._autoHide)
                this.btnlock.Image = LayoutManager._imageList.Images[4];
            else
                this.btnlock.Image = LayoutManager._imageList.Images[3];
        }
        #endregion

        #region ..OnActiveChanged
        protected virtual void OnActivedChanged()
        {
            if (this.ActivedChanged != null)
                this.ActivedChanged(this, EventArgs.Empty);
        }
        #endregion

        #region ..c_ControlChanged
        void c_ControlChanged(object sender, EventArgs e)
        {
            Content c = sender as Content;
            if (c.Tag is TabControl.TabPage)
            {
                TabControl.TabPage page = c.Tag as TabControl.TabPage;
                page.Control = c.Control;
                page.Image = c.Image;
                page.Text = c.Title;
            }
        }
        #endregion

        #region ..DoubleClick
        /// <summary>
        /// double click
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            if (!this.AutoHide)
            {
                //get the client point
                Point point = this.PointToClient(Control.MousePosition);

                //get the title region
                using (GraphicsPath path = this.GetTitleRegion())
                {
                    if (path.IsVisible(point))
                    {
                        LayoutManager manager = LayoutManager.FindManagerForControl(this);
                        manager.FloatGroup(this);
                    }
                }
            }
        }
        #endregion

        #region ..UpdateButtonStatus
        void UpdateButtonStatus()
        {
            this.btnclose.Visible = this.btnExpand.Visible = this.btnMax.Visible = this.btnlock.Visible = this._showTitle;
            UpdateTableControl();
        }
        #endregion

        #region ..OnMouseUp
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            //find the layout manager
            LayoutManager manager = LayoutManager.FindManagerForControl(this);
            //right click
            if (manager != null && e.Button == MouseButtons.Right)
            {
                manager.groupContext.Tag = this;
                manager.groupContext.Show(Control.MousePosition);
            }
        }
        #endregion
    }
}
