using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;

namespace YP.CommonControl.Dock
{
    #region ..LayoutStle
    /// <summary>
    /// define the layout style
    /// </summary>
    public enum LayoutStyle
    {
        /// <summary>
        /// left layout
        /// </summary>
        Left,
        /// <summary>
        /// top layout
        /// </summary>
        Top,
        /// <summary>
        /// right layout
        /// </summary>
        Right,
        /// <summary>
        /// bottom layout
        /// </summary>
        Bottom
    }
    #endregion

    /// <summary>
    /// define the class the layout the dock control
    /// </summary>
    public class LayoutManager:Common.BaseControl 
    {
        #region ..static fields
		internal readonly static ImageList _imageList;

        static LayoutManager()
		{
            _imageList = Common.ResourceHelper.LoadBitmapStrip(Type.GetType("YP.CommonControl.Dock.LayoutManager"), "YP.CommonControl.Resources.ImagesCaptionIDE.bmp", new Size(12, 11), new Point(0, 0));
		}
		#endregion

        #region ..Constructor
        public LayoutManager()
        {
            this._leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this._rightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this._topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._leftPanel.Visible = this._rightPanel.Visible = this._topPanel.Visible = this._bottomPanel.Visible = false;
            this._leftPanel.VisibleChanged += new EventHandler(_leftPanel_VisibleChanged);
            this._rightPanel.VisibleChanged += new EventHandler(_leftPanel_VisibleChanged);
            this._topPanel.VisibleChanged += new EventHandler(_leftPanel_VisibleChanged);
            this._bottomPanel.VisibleChanged += new EventHandler(_leftPanel_VisibleChanged);
            
            this.Controls.AddRange(new System.Windows.Forms.Control[] { this._bottomPanel, this._topPanel  , this._leftPanel,this._rightPanel});
            this.EnsureBorderPanelRight();

            this.CreateDockGroupContext();
        }

        void _leftPanel_VisibleChanged(object sender, EventArgs e)
        {
            this._leftPanel.Invalidate();
            this._bottomPanel.Invalidate();
            this._topPanel.Invalidate();
            this._rightPanel.Invalidate();
        }
        #endregion

        #region ..private fields
        ZoneCollection _zones = new ZoneCollection();
        BorderPanel _leftPanel = new BorderPanel();
        BorderPanel _rightPanel = new BorderPanel();
        BorderPanel _topPanel = new BorderPanel();
        BorderPanel _bottomPanel = new BorderPanel();
        System.Collections.ArrayList _containers = new System.Collections.ArrayList();
        System.Collections.ArrayList _floatForms = new ArrayList();
        bool _endLayout = false;
        //store the information of the auto hide groups
        System.Collections.Hashtable _autoHideRecorder = new Hashtable();
        //store the information of the conent
        System.Collections.Hashtable _contentRecorder = new Hashtable();
        bool _drawGradientBackground = false;
        //rememer the order of the groups
        System.Collections.Hashtable _groupsOrder = new Hashtable();
        internal Menu.PopupMenu groupContext;
        //remember the order the contents
        System.Collections.Hashtable _contentsGroups = new Hashtable();
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
                    this.Invalidate(true);
                }
            }
            get
            {
                return this._drawGradientBackground;
            }
        }
        #endregion

        #region ..AddContentToLayout
        /// <summary>
        /// Add a content to the layout
        /// Notice,this method must be used before you call the AddControlToMainArea
        /// </summary>
        /// <param name="c">the content you want to add</param>
        /// <param name="style">the layout style of the content</param>
        /// <param name="displaySize">the size of the content</param>
        public void AddContentToLayout(Content c,LayoutStyle style,Size displaySize)
        {
            if (this._endLayout)
                return;

            //create the new container
            ZoneContainer container = new ZoneContainer();
            container.Dock = DockStyle.Fill;
            Size oldSize = displaySize;
            container.Panel2MinSize = container.Panel1MinSize = 0;
            displaySize = EnsureSizeToContainGroup(displaySize);
            container.Size = displaySize;
            
            //create a new group to display the content
            Group group = this.CreateContainerGroupForContent(c);
            group.LayoutStyle = style;

            //update the container
            switch (style)
            {
                    //left
                case LayoutStyle.Left:
                    container.Orientation = Orientation.Vertical;
                    container.FixedPanel = FixedPanel.Panel1;
                    container.SplitterDistance = displaySize.Width;
                    break;
                    //right
                case LayoutStyle.Right:
                    container.Orientation = Orientation.Vertical;
                    container.FixedPanel = FixedPanel.Panel2;
                    container.SplitterDistance = container.Width - displaySize.Width;
                    break;
                    //top
                case LayoutStyle.Top:
                    container.Orientation = Orientation.Horizontal;
                    container.FixedPanel = FixedPanel.Panel1;
                    container.SplitterDistance = displaySize.Height;
                    break;
                    //bottom
                case LayoutStyle.Bottom :
                    container.Orientation = Orientation.Horizontal;
                    container.FixedPanel = FixedPanel.Panel2;
                    container.SplitterDistance = container.Height - displaySize.Height;
                    break;
            }

            if (container.Orientation == Orientation.Vertical)
                container.Size = new Size(displaySize.Width + container.SplitterWidth, displaySize.Height);
            else
                container.Size = new Size(displaySize.Width, displaySize.Height + container.SplitterWidth);

            //create the zone to layout the group
            Zone zone = new Zone();
            group.DisplaySize = oldSize;
            zone.Groups.Add(group);
            zone.Dock = DockStyle.Fill;

            if (container.FixedPanel == FixedPanel.Panel2)
                container.Panel2.Controls.Add(zone);
            else if (container.FixedPanel == FixedPanel.Panel1)
                container.Panel1.Controls.Add(zone);

            //if there are container before, add the new container into it's main area
            if (this._containers.Count > 0)
            {
                ZoneContainer previousContainer = this._containers[this._containers.Count - 1] as ZoneContainer;
                if (previousContainer.FixedPanel == FixedPanel.Panel1)
                    previousContainer.Panel2.Controls.Add(container);
                else if (previousContainer.FixedPanel == FixedPanel.Panel2)
                    previousContainer.Panel1.Controls.Add(container);
            }
                //else,add the container as top container
            else
            {
                this.Controls.Add(container);
            }

            //add the container to the list
            if (!this._containers.Contains(container))
                this._containers.Add(container);

            this.EnsureBorderPanelRight();
        }
        #endregion

        #region ..InsertContentBefore
        /// <summary>
        /// Insert a new content before the speical content
        /// </summary>
        /// <param name="c">the content you want to insert</param>
        /// <param name="displaySize">the size of the content</param>
        /// <param name="refContent">the content you want to insert the new content before</param>
        public void InsertContentBefore(Content c, Size displaySize,Content refContent)
        {
            if (c == refContent)
                return;
            //if the zone for the ref content is not null
            if (refContent != null && refContent.ParentGroup != null && refContent.ParentGroup.ParentZone != null)
            {
                Zone zone = refContent.ParentGroup.ParentZone;
                int index = zone.Groups.IndexOf(refContent.ParentGroup);

                //create the group to display the content
                Group group = this.CreateContainerGroupForContent(c);
                group.LayoutStyle = refContent.ParentGroup.LayoutStyle;
                group.DisplaySize = displaySize;
                zone.Groups.Insert(group, index);
            }
        }
        #endregion

        #region ..InsertContentAfter
        /// <summary>
        /// Insert a new content after the speical content
        /// </summary>
        /// <param name="c">the content you want to insert</param>
        /// <param name="displaySize">the size of the content</param>
        /// <param name="refContent">the content you want to insert the new content after</param>
        public void InsertContentAfter(Content c, Size displaySize, Content refContent)
        {
            if (c == refContent)
                return;
            //if the zone for the ref content is not null
            if (refContent != null && refContent.ParentGroup != null && refContent.ParentGroup.ParentZone != null)
            {
                Zone zone = refContent.ParentGroup.ParentZone;

                int index = zone.Groups.IndexOf(refContent.ParentGroup);

                //create the group to display the content
                Group group = this.CreateContainerGroupForContent(c);
                group.LayoutStyle = refContent.ParentGroup.LayoutStyle;
                group.DisplaySize = displaySize;
                zone.Groups.Insert(group, index + 1);
            }
        }
        #endregion

        #region ..AddControlIntoMainArea
        /// <summary>
        /// Add the control into the main area of the layout manager,
        /// Notice,the AddContentToLayout won't do nothing after this method is called
        /// </summary>
        /// <param name="control">ghe control you want to add</param>
        public void AddControlIntoMainArea(Control control)
        {
            _endLayout = true;
            this.RecordTheControlOrder();
            //find the last container
            if (this._containers.Count > 0)
            {
                control.Dock = DockStyle.Fill;
                Panel panel = new Panel();
                panel.Dock = DockStyle.Fill;
                panel.BackColor = SystemColors.ControlDark;
                ZoneContainer container = this._containers[this._containers.Count - 1] as ZoneContainer;
                if (container.FixedPanel == FixedPanel.Panel1)
                    container.Panel2.Controls.Add(panel);
                else if (container.FixedPanel == FixedPanel.Panel2)
                    container.Panel1.Controls.Add(panel);
                panel.Controls.Add(control);
                panel.Paint += new PaintEventHandler(LayoutManager_Paint);
            }
        }

        void LayoutManager_Paint(object sender, PaintEventArgs e)
        {
            Control c = sender as Control;
            ControlPaint.DrawBorder(e.Graphics, new Rectangle(0, 0, c.Width, c.Height), SystemColors.ControlDarkDark, 1, ButtonBorderStyle.Inset, SystemColors.ControlDarkDark, 1, ButtonBorderStyle.Inset, SystemColors.ControlLight, 1, ButtonBorderStyle.Inset, SystemColors.ControlLight, 1, ButtonBorderStyle.Inset);
        }
        #endregion

        #region ..EnsureBorderPanelRight
        /// <summary>
        /// adjust the border panels to make they display right
        /// </summary>
        void EnsureBorderPanelRight()
        {
            this._topPanel.SendToBack();
            this._bottomPanel.SendToBack();
            this._leftPanel.SendToBack();
            this._rightPanel.SendToBack();
        }
        #endregion

        #region ..CreateContainerGroupForContent
        /// <summary>
        /// create a new group to display the content
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        Group CreateContainerGroupForContent(Content c)
        {
            Group group = new Group();
            group.ShowExpand = group.ShowMaxMin = true;
            group.Contents.Add(c);
            return group;
        }
        #endregion

        #region ..ToggleAutoHide
        /// <summary>
        /// Toggle the auto hide status of the group
        /// Notice, please call this function after you call the AddControlIntoMainArea
        /// </summary>
        /// <param name="group"></param>
        public void ToggleAutoHide(Group group)
        {
            //if not end layout
            if (!this._endLayout)
                return;
            //auto hide
            if (group.AutoHide)
            {
                //get the original splitterPanel
                if (this._autoHideRecorder.ContainsKey(group))
                {
                    AutoHideRecorder recorder = this._autoHideRecorder[group] as AutoHideRecorder;
                    SplitterPanel panel = recorder.ParentPanel;

                    //if there are zones in the panel
                    if (panel.Controls.Count > 0 && panel.Controls[0] is Zone)
                    {
                        Zone zone = panel.Controls[0] as Zone;
                        zone.SuspendLayout();
                        int index = group.GroupIndex;
                        bool add = false;
                        for (int i = 0; i < zone.Groups.Count; i++)
                        {
                            Group g = zone.Groups[i];
                            if (g.GroupIndex > group.GroupIndex)
                            {
                                add = true;
                                zone.Groups.Insert(group, i);
                                break;
                            }
                        }
                        if(!add)
                            zone.Groups.Add(group);
                        //adjust the index of the zone
                        zone.ResumeLayout(true);
                    }
                    //else, create a new zone
                    else
                    {
                        Zone zone = recorder.GroupContainer as Zone;
                        zone.Groups.Add(group);
                        panel.Controls.Add(zone);
                    }
                    group.ShowExpand = group.ShowMaxMin = true;
                    group.ShowTabButton = true;
                    group.SetAutoHideStatus(false);
                    group.PerformButtonClick(Group.ButtonClickStyle.Expand);
                }
            }
            else
            {
                BorderPanel panel = this.FindBorderPanelForGroup(group);
                if (panel != null)
                {
                    //remember the parent container before auto hide
                    SplitterPanel parentPanel = this.GetSplitterPanelForGroup(group);
                    if (!this._autoHideRecorder.ContainsKey(group))
                        this._autoHideRecorder[group] = new AutoHideRecorder(parentPanel);
                    AutoHideRecorder recorder = this._autoHideRecorder[group] as AutoHideRecorder;
                    recorder.ParentPanel = parentPanel;
                    recorder.GroupContainer = group.ParentZone;
                    group.SetAutoHideStatus(true);
                    panel.Groups.Add(group);
                }
            }
        }
        #endregion

        #region ..ShowContent
        /// <summary>
        /// Show the Content
        /// </summary>
        /// <param name="content">the content you want to show</param>
        public void ShowContent(Content content)
        {
            //if group has no parent
            if (content != null && !this.IsContentVisible(content))
            {
                //get the recorder
                if (this._contentRecorder.ContainsKey(content))
                {
                    ContentRecorder recorder = this._contentRecorder[content] as ContentRecorder;
                    SplitterPanel panel = recorder.ParentPanel;
                    Group parentGroup = recorder.ParentGroup;
                    if (content.Float)
                        parentGroup = recorder.FloatParentGroup;
                    
                    #region ..revert the content
                    //if there are some contents in the parent grup
                    if (parentGroup.Contents.Count > 0)
                    {
                        bool add = false;
                        for (int i = 0; i < parentGroup.Contents.Count; i++)
                        {
                            if (parentGroup.Contents[i].ContentIndex > content.ContentIndex)
                            {
                                add = true;
                                parentGroup.Contents.Insert(content, i);
                                break;
                            }
                        }

                        if (!add)
                            parentGroup.Contents.Add(content);
                    }
                    //else, add the content into the parent group
                    else
                    {
                        parentGroup.Contents.Add(content);
                    }
                    #endregion

                    #region ..revert the group
                    //if parent group has been removed, revert it,
                    if (parentGroup.Parent == null)
                    {
                        IGroupContainer container = recorder.GroupContainer;
                        IGroupContainer floatContainer = recorder.FloatGroupContainer;
                        //if container is border panel
                        if (container is BorderPanel)
                            container.Groups.Add(parentGroup);
                        //else if the container is zone
                        else if (floatContainer != null && content.Float)
                        {
                            floatContainer.Groups.Add(parentGroup);
                        }
                        else if (panel != null)
                        {
                            //the zone display on the panel
                            if (panel.Controls.Count > 0 && panel.Controls[0] is Zone)
                            {
                                Zone zone = panel.Controls[0] as Zone;
                                zone.SuspendLayout();
                                int index = parentGroup.GroupIndex;
                                bool add = false;
                                for (int i = 0; i < zone.Groups.Count; i++)
                                {
                                    Group g = zone.Groups[i];
                                    if (g.GroupIndex > parentGroup.GroupIndex)
                                    {
                                        add = true;
                                        zone.Groups.Insert(parentGroup, i);
                                        break;
                                    }
                                }
                                if (!add)
                                    zone.Groups.Add(parentGroup);
                                //adjust the index of the zone
                                zone.ResumeLayout(true);
                            }
                            //or add the zone into the panel
                            else
                            {
                                Zone zone = container as Zone;
                                zone.Groups.Add(parentGroup);
                                panel.Controls.Add(zone);
                            }
                        }
                    }
                    #endregion
                }
            }
        }
        #endregion

        #region ..HideContent
        /// <summary>
        /// Hide the content
        /// </summary>
        /// <param name="content">the content you want to hide</param>
        public void HideContent(Content content)
        {
            if (!this.IsContentVisible(content))
                return;
            //record the content
            Group group = content.ParentGroup;
            //remember the parent container before auto hide
            SplitterPanel parentPanel = this.GetSplitterPanelForGroup(group);
            //if (parentPanel == null)
            //    return;
            //get the previous and the next group
            Group previous = null;
            Group next = null;
            if (group.ParentGroups != null)
            {
                int index = group.ParentGroups.IndexOf(group);
                if (index > 0)
                    previous = group.ParentGroups[index - 1];
                if (index < group.ParentGroups.Count - 1)
                    next = group.ParentGroups[index + 1];
            }

            Content previousContent = null, nextContent = null;
            if (content.ParentGroup.Contents != null)
            {
                int index = group.Contents.IndexOf(content);
                if (index > 0)
                    previousContent = group.Contents[index - 1];
                if (index < group.Contents.Count - 1)
                    nextContent = group.Contents[index + 1];
            }
            if (!this._contentRecorder.ContainsKey(content))
                this._contentRecorder[content] = new ContentRecorder(parentPanel);
            ContentRecorder recorder = this._contentRecorder[content] as ContentRecorder;
            if(parentPanel != null)
                recorder.ParentPanel = parentPanel;
            if (content.Float)
                recorder.FloatParentGroup = group;
            else 
                recorder.ParentGroup = group;
            if (group.AutoHide)
                recorder.GroupContainer = this.FindBorderPanelForGroup(group);
            else
            {
                Zone zone = group.ParentZone;
                //if the content is float
                if (zone.Float)
                    recorder.FloatGroupContainer = zone;
                else
                    recorder.GroupContainer = zone;
            }
            if (content._parentGroup != null && content._parentGroup.Contents.Contains(content))
                content._parentGroup.Contents.Remove(content);  
        }
        #endregion

        #region ..HideGroup
        /// <summary>
        /// Hide the group
        /// </summary>
        /// <param name="group">the group you want to hide</param>
        public void HideGroup(Group group)
        {
            for (int i = 0; i < group.Contents.Count; i++)
            {
                this.HideContent(group.Contents[i]);
                i--;
            }
        }
        #endregion

        #region ..static FindManagerForControl
        /// <summary>
        /// find the top layout manager for the control
        /// </summary>
        /// <returns></returns>
        internal static LayoutManager FindManagerForControl(Control c)
        {
            Control parent = c.Parent;
            while (parent != null)
            {
                if (parent is LayoutManager)
                    return parent as LayoutManager;
                parent = parent.Parent;
            }
            return null;
        }
        #endregion

        #region ..FindBorderPanelForGroup
        /// <summary>
        /// find the border panel to the group to auto hide
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        BorderPanel FindBorderPanelForGroup(Group group)
        {
            switch (group.LayoutStyle)
            {
                case LayoutStyle.Left:
                    return this._leftPanel;
                case LayoutStyle.Right:
                    return this._rightPanel;
                case LayoutStyle.Top:
                    return this._topPanel;
                case LayoutStyle.Bottom:
                    return this._bottomPanel;
            }
            return null;
        }
        #endregion

        #region ..GetContentRegionForBorderPanel
        /// <summary>
        /// get the valid conent region of the border panel
        /// </summary>
        /// <param name="panel"></param>
        /// <returns></returns>
        internal Rectangle GetContentRegionForBorderPanel(BorderPanel panel)
        {
            Rectangle rect = panel.Bounds;
           //the left and right border
            if (panel.IsVertical())
            {
                int top = rect.Top;
                int bottom = rect.Bottom;
                //if top is visi
                if (this._topPanel.Visible)
                    top += this._topPanel.Height;
                if (this._bottomPanel.Visible)
                    bottom -= this._bottomPanel.Height;
                rect = new Rectangle(rect.Left, top, rect.Width, bottom - top);
            }
            return rect;
        }
        #endregion

        #region ..EnsureSizeToContainGroup
        /// <summary>
        /// gets a size to ensure it can contain the group
        /// </summary>
        /// <param name="groupSize">the display size of the group</param>
        /// <returns></returns>
        Size EnsureSizeToContainGroup(Size groupSize)
        {
            groupSize.Height += Group.titleHeight + Group.topmargin;
            groupSize.Height += TabControl.TabControl.TabHeight;
            groupSize.Height += Zone.GroupBottomMargin;
            return groupSize;
        }
        #endregion

        #region ..GetSplitterPanelForGroup
        /// <summary>
        /// get the splitter contains the group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        SplitterPanel GetSplitterPanelForGroup(Group group)
        {
            Control parent = group.Parent;
            while (parent != null)
            {
                if (parent is SplitterPanel)
                    return parent as SplitterPanel;
                parent = parent.Parent;
            }
            return null;
        }
        #endregion

        #region ..AutoHideAll
        /// <summary>
        /// auto hide all the the contents
        /// </summary>
        public void AutoHideAll()
        {
            //crycle in the all the container
            foreach (ZoneContainer container in this._containers)
            {
                //get the zone in the container
                Zone zone = container.GetZone();
                if (zone != null)
                {
                    //auto hide all the group in the zone
                    for (int i = 0; i < zone.Groups.Count; i++)
                    {
                        Group group = zone.Groups[i];
                        this.ToggleAutoHide(group);
                        i--;
                    }
                }
            }
        }
        #endregion

        #region ..ShowAllContents
        /// <summary>
        /// show all the contents
        /// </summary>
        public void ShowAllContents()
        {
            foreach (Content c in this._contentRecorder.Keys)
                this.ShowContent(c);
        }
        #endregion

        #region ..HideAllContents
        /// <summary>
        /// Hide all the contents
        /// </summary>
        public void HideAllContents()
        {
            //crycle in the all the container
            foreach (ZoneContainer container in this._containers)
            {
                //get the zone in the container
                Zone zone = container.GetZone();
                if (zone != null)
                {
                    //auto hide all the group in the zone
                    for (int i = 0; i < zone.Groups.Count; i++)
                    {
                        Group group = zone.Groups[i];
                        this.HideGroup(group);
                        i--;
                    }
                }
            }

            //crycle in all the border panel
            BorderPanel[] panels = new BorderPanel[] { this._leftPanel, this._topPanel, this._rightPanel, this._bottomPanel };
            foreach (BorderPanel panel in panels)
            {
                //auto hide all the group in the zone
                for (int i = 0; i < panel.Groups.Count; i++)
                {
                    Group group = panel.Groups[i];
                    this.HideGroup(group);
                    i--;
                }
            }

            //crycle all the float forms
            foreach (ToolDialog dlg in this._floatForms)
            {
                //auto hide all the group in the zone
                for (int i = 0; i < dlg.Groups.Count; i++)
                {
                    Group group = dlg.Groups[i];
                    this.HideGroup(group);
                    i--;
                }
            }
        }
        #endregion

        #region ..HasDockedGroup
        /// <summary>
        /// judge whether there is any group docked
        /// </summary>
        /// <returns></returns>
        public bool HasDockedGroup()
        {
            //crycle in the all the container
            foreach (ZoneContainer container in this._containers)
            {
                //get the zone in the container
                Zone zone = container.GetZone();
                if (zone != null && zone.Groups.Count > 0)
                    return true;
            }
            return false;
        }
        #endregion

        #region ..IsContentVisible
        /// <summary>
        /// judge whether the content is visible
        /// </summary>
        /// <param name="content">the content you want to judge</param>
        /// <returns></returns>
        public bool IsContentVisible(Content content)
        {
            if (content != null && content._parentGroup != null)
                return content._parentGroup.Contents.Contains(content) ;
            return false;
        }
        #endregion

        #region ..RecordTheControlOrder
        /// <summary>
        /// if end layout, record the control order in the manager
        /// </summary>
        void RecordTheControlOrder()
        {
            if (!this._endLayout)
                return;
            foreach (ZoneContainer  container in this._containers)
            {
                Zone zone = container.GetZone();
                if (zone != null)
                {
                    for (int i = 0; i < zone.Groups.Count; i++)
                    {
                        Group group = zone.Groups[i];
                        //remember the intialize index of the group
                        group.GroupIndex = i;

                        for (int j = 0; j < group.Contents.Count; j++)
                            group.Contents[j].ContentIndex = j;
                    }
                }
            }
        }
        #endregion

        #region ..FloatContent
        /// <summary>
        /// toglle the conent to float
        /// </summary>
        /// <param name="group"></param>
        internal ToolDialog FloatContent(Content content)
        {
            Size size = Size.Empty;
            //remember the original size
            if (content.ParentGroup != null)
                size = content.ParentGroup.LastNormalSize;

            //find the tool dialog
            ToolDialog dlg = null;
            if (this._contentRecorder.ContainsKey(content))
            {
                ContentRecorder recorder = this._contentRecorder[content] as ContentRecorder;
                //if the float recorder exists
                Zone zone = recorder.FloatGroupContainer as Zone;
                if (zone != null && zone.ParentForm is ToolDialog)
                    dlg = zone.ParentForm as ToolDialog;
            }

            //if dlg is null ,create new
            if (dlg == null)
            {
                dlg = new ToolDialog(this);
                dlg.Owner = this.FindForm();
                dlg.ClientSize = size;
            }
            this.FloatContent(content, dlg);

            return dlg;
        }

        /// <summary>
        /// Toggle the content to float
        /// </summary>
        /// <param name="content">the content you want to float</param>
        /// <param name="dlg">the target float dlg</param>
        internal void FloatContent(Content content, ToolDialog dlg)
        {
            this.HideContent(content);
            content.Float = true;

            //if tool dlg has the group
            Group group = null;
            if (dlg.Groups.Count > 0)
                group = dlg.Groups[0];
            if (group == null)
            {
                group = new Group();
                group.LastNormalSize = dlg.ClientSize;

                if (!dlg.Groups.Contains(group))
                    dlg.Groups.Add(group);
            }
            dlg.Text = content.Title;
            group.Contents.Add(content);
            dlg.Show();
            if (!this._floatForms.Contains(dlg))
            {
                this._floatForms.Add(dlg);
            }
        }
        #endregion

        #region ..FloatGroup
        /// <summary>
        /// toggle the group to float
        /// </summary>
        /// <param name="group">the group you want to float</param>
        public void FloatGroup(Group group)
        {
            ToolDialog dlg = null;
            Content firstContent = group.Contents.Count == 0 ? null : group.Contents[0];
            int index = group.SelectedIndex;
            for (int i = 0; i < group.Contents.Count; i++)
            {
                Content c = group.Contents[i];
                //if dlg is null
                if (dlg == null)
                    dlg = this.FloatContent(c);
                else
                    this.FloatContent(c, dlg);
                i--;
            }
            //revert the origin selected index
            if (firstContent != null && firstContent.ParentGroup != null)
                firstContent.ParentGroup.SelectedIndex = index;
            //ToolDialog dlg = new ToolDialog(this);
            //dlg.Owner = this.FindForm();
            //int index = group.SelectedIndex;
            ////remember the origin contents
            //System.Collections.ArrayList list = new ArrayList();
            //list.AddRange(group.Contents);
            //Size size = group.LastNormalSize;
            //for (int i = 0; i < group.Contents.Count; i++)
            //{
            //    Content c = group.Contents[i];
            //    //hide the content first
            //    this.HideContent(c);
            //    c.Float = true;
            //    i--;
            //}

            ////add the the contents into the group
            //foreach (Content c in list)
            //    group.Contents.Add(c);
            //dlg.Groups.Add(group);
            //dlg.ClientSize = size;
            //dlg.Text = group.Text;
            //group.SelectedIndex = index;
            //if (group.ParentZone != null)
            //    group.ParentZone.ExpandGroup(group);
            //dlg.Show();

            ////store the dlg into the list
            //if (!_floatForms.Contains(dlg))
            //    _floatForms.Add(dlg);
        }
        #endregion

        #region ..GetDockRegion
        /// <summary>
        /// Get the docked region when drag the group
        /// </summary>
        /// <param name="point">the screent point</param>
        /// <param name="exceptDlg">the dlg should except</param>
        /// <returns></returns>
        internal Rectangle[] GetDockedRegion(Point point,ToolDialog exceptDlg)
        {
            //crycle all the tool dialogs
            int height = SystemInformation.ToolWindowCaptionHeight;
            foreach (ToolDialog dlg in this._floatForms)
            {
                if (dlg == exceptDlg)
                    continue;
                Rectangle rect = dlg.ClientRectangle;
                Rectangle rect1 = dlg.RectangleToScreen(rect);
                rect = new Rectangle(rect1.X, rect1.Y - height, rect1.Width, height);
                
                //if the caption area contains point 
                if (rect.Contains(point))
                {
                    Rectangle[] rects = new Rectangle[2];
                    rects[0] = new Rectangle(rect1.X, rect1.Y, rect1.Width, rect1.Height - TabControl.TabControl.TabHeight);
                    rects[1] = new Rectangle(rect1.X, rect1.Bottom - TabControl.TabControl.TabHeight, (int)Math.Min(rect1.Width, 60), TabControl.TabControl.TabHeight);
                    return rects;
                }
            }
            return null;
        }
        #endregion

        #region ..CreateDockGroupContext
        void CreateDockGroupContext()
        {
            this.groupContext = new Menu.PopupMenu(this);
            //this.groupContext.Items.AddRange(Menu.MenuItemFactory.CreateItems("YP.Resource.Menu.DockGroupMenu.xml", new EventHandler(this.SelectItem) ,new EventHandler(this.UpdateItem),null));

            //this.groupContext.Opening += new System.ComponentModel.CancelEventHandler(groupContext_Opening);
        }

        void groupContext_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //deal the last command
            Menu.MenuCommand cmd = this.groupContext.Items[this.groupContext.Items.Count - 1] as Menu.MenuCommand;
            System.Xml.XmlElement element = cmd.Tag as System.Xml.XmlElement;
            if (element == null)
                return;
            string caption = string.Empty;
            //get the current selected panel
            if (this.groupContext.Tag is Control)
                caption = (this.groupContext.Tag as Control).Text;
            System.Collections.ArrayList list = new ArrayList();
            if (this.groupContext.Tag is ToolDialog)
                list.AddRange((this.groupContext.Tag as ToolDialog).Groups);
            else if (this.groupContext.Tag is Group)
                list.Add(this.groupContext.Tag as Group);
            Group[] exceptedGroups = new Group[list.Count];
            list.CopyTo(exceptedGroups);

            cmd.Text = element.GetAttribute("Origin").Replace("{0}", caption);

            cmd.Items.Clear();
            //if (list.Count > 0)
            //    cmd.Tag = (list[0] as Group).SelectedContent;
            cmd.Items.AddRange(this.CreateAllAvaiableItems(exceptedGroups));
            cmd.Visible = cmd.Items.Count > 0;
        }
        #endregion

        #region ..UpdateItem
        void UpdateItem(object sender, EventArgs e)
        {
            Menu.MenuCommand cmd = sender as Menu.MenuCommand;
            System.Xml.XmlElement element = cmd.Tag as System.Xml.XmlElement;
            if (element == null)
                return;
            string action = element.GetAttribute("Method").Trim().ToLower();
            bool isFloat = this.groupContext.Tag is ToolDialog;
            bool autoHide = this.groupContext.Tag is Group && (this.groupContext.Tag as Group).AutoHide;
           
            switch (action)
            {
                case "float":
                    cmd.Checked = isFloat && !autoHide;
                    cmd.Enabled = !autoHide;
                    break;
                case "dock":
                    cmd.Checked = !isFloat &&!autoHide;
                    cmd.Enabled = !autoHide;
                    break;
                case "autohide":
                    cmd.Enabled = !isFloat;
                    cmd.Checked = autoHide;
                    break;
                case "hide":
                    
                    break;
                case "movepanel":
                    
                    break;
            }
        }
        #endregion

        #region ..SelectItem
        void SelectItem(object sender, EventArgs e)
        {
            Menu.MenuCommand cmd = sender as Menu.MenuCommand;
            System.Xml.XmlElement element = cmd.Tag as System.Xml.XmlElement;
            if (element == null)
                return;
            string action = element.GetAttribute("Method").Trim().ToLower();
            System.Collections.ArrayList list = new ArrayList();
            //add the group into the list
            if (this.groupContext.Tag is ToolDialog)
                list.AddRange((this.groupContext.Tag as ToolDialog).Groups);
            else if (this.groupContext.Tag is Group)
                list.Add(this.groupContext.Tag as Group);

            switch (action)
            {
                case "dock":
                    if (this.groupContext.Tag is ToolDialog)
                        (this.groupContext.Tag as ToolDialog).ReturnToDock();
                    break;
                case "float":
                    if (this.groupContext.Tag is Group)
                        this.FloatGroup(this.groupContext.Tag as Group);
                    break;
                case "autohide":
                    foreach (Group group in list)
                        this.ToggleAutoHide(group);
                    break;
                case "hide":
                    foreach (Group group in list)
                        this.HideGroup(group);
                    break;
            }
        }
        #endregion

        #region ..CreateAllAvaiableItems
        /// <summary>
        /// create the items for all the current avaiable palette
        /// </summary>
        /// <param name="exceptGroup">the group want to be excepted</param>
        /// <returns></returns>
        ToolStripItem[] CreateAllAvaiableItems(Group[] exceptGroups)
        {
            System.Collections.ArrayList list = new ArrayList();
            //crycle all the container zone
            foreach (ZoneContainer container in this._containers)
            {
                Zone zone = container.GetZone();
                if (zone == null)
                    continue;
                //add all the group
                foreach (Group group in zone.Groups)
                {
                    if (Array.IndexOf(exceptGroups, group) < 0)
                        list.Add(group);
                }
            }

            //crycle all the tool dialogs
            foreach (ToolDialog dlg in this._floatForms)
            {
                if (dlg.Visible)
                {
                    //add all the group
                    foreach (Group group in dlg.Groups)
                    {
                        if (Array.IndexOf(exceptGroups, group) < 0)
                            list.Add(group);
                    }
                }
            }

            //crycle all the border panel
            BorderPanel[] panels = new BorderPanel[] { this._leftPanel, this._rightPanel, this._topPanel, this._bottomPanel };
            foreach (BorderPanel panel in panels)
            {
                if (panel.Visible)
                {
                    //add all the group
                    foreach (Group group in panel.Groups)
                    {
                        if (Array.IndexOf(exceptGroups, group) < 0)
                            list.Add(group);
                    }
                }
            }

            ToolStripItem[] items = new ToolStripItem[list.Count];
            int i = 0;
            foreach (Group group in list)
            {
                ToolStripItem item = Menu.MenuItemFactory.CreateMenuItem(group.Text,new EventHandler(this.MovePanel));
                item.Tag = group;
                items[i] = item;
                i++;
            }

            return items;
        }
        #endregion

        #region ..MovePanel
        void MovePanel(object sender, EventArgs e)
        {
            //find the target group
            ToolStripItem item = sender as ToolStripItem;
            Group group = item.Tag as Group;
            if (group == null)
                return;
            Content moveContent = null;
            //find the content want to move
            if(this.groupContext.Tag is Group)
                moveContent = (this.groupContext.Tag as Group).SelectedContent;

            if (moveContent != null)
            {
                this.HideContent(moveContent);
                group.Contents.Add(moveContent);
            }
        }
        #endregion
    }
}
