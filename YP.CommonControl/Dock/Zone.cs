using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections;

using YP.CommonControl;

namespace YP.CommonControl.Dock
{
    /// <summary>
    /// define the zone to layout the group
    /// when you want to remove a group from the zone, please dont't remove from controls directly,
    /// using the _groups to remove 
    /// </summary>
    public class Zone : Common.BaseControl, IGroupContainer
    {
        #region ..Constructor
        internal  Zone()
        {
            //top down flow layout
            this.Margin = new Padding(0, 3, 0, 3);
            //create the events for the _groups
            this._groups.Inserted += new Common.CollectionWithEvents.CollectionEventHandler(_groups_Inserted);
            this._groups.Removed += new Common.CollectionWithEvents.CollectionEventHandler(Groups_Removed);
            this._groups.Cleared += new Common.CollectionWithEvents.ClearEventHandler(_groups_Cleared);
            this.oriSize = this.Size;
        }
        #endregion

        #region ..private fields
        GroupCollection _groups = new GroupCollection();
        Size oriSize = Size.Empty;
        //if start to resize the group
        bool mouseDown = false;
        //the group want to be resized
        Group resizeGroup = null;
        //the max size the resizeable group can be resized
        Size maxResize = Size.Empty;
        Point startPoint = Point.Empty;
        Size startSize = Size.Empty;
        bool resizeTop = false;
        bool _float = false;
        #endregion

        #region ..const fields
        internal const int GroupBottomMargin = 3;
        Padding groupMargin = new Padding(0, 0, 0, 3);
        #endregion

        #region ..public properties
        /// <summary>
        /// gets or sets a value indicates whether the zone is float
        /// </summary>
        internal bool Float
        {
            set
            {
                this._float = value;
            }
            get
            {
                return this._float;
            }
        }

        /// <summary>
        /// gets the child _groups collection
        /// </summary>
        public GroupCollection Groups
        {
            get
            {
                return this._groups;
            }
        }
        #endregion

        #region ..events for the _groups collection
        void _groups_Cleared()
        {
            this.Controls.Clear();
        }

        void Groups_Removed(int index, object value)
        {
            Group group = value as Group;
            if (this.Controls.Contains(group))
            {
                group.ButtonClick -= new Group.ButtonClickEventHandler(group_ButtonClick);
                //pre check the other group
                if (index == this._groups.Count)
                    index--;
                PreRemoveGroup(index,group);
                this.Controls.Remove(group);
                this.FlowLayoutGroups();
            }
        }

        void _groups_Inserted(int index, object value)
        {
            Group group = value as Group;
            if (group != null)
            {
                this.SuspendLayout();
                group.Left = 0;
                group.Width = this.Width;
                //set the size as minimize when inserted
                group.Height = group.MinHeight;
                //anchor the group to the edge
                this.Controls.Add(group);
                this.Controls.SetChildIndex(group, index);
                group.ButtonClick += new Group.ButtonClickEventHandler(group_ButtonClick);
                group.Anchor = AnchorStyles.Left | AnchorStyles.Right;
                this.FlowLayoutGroups();
                this.ExpandGroup(group);
                this.ResumeLayout(true);
            }
        }
        #endregion

        #region ..group_ButtonClick
        void group_ButtonClick(object sender, Group.ButtonClickStyle click)
        {
            Group g = sender as Group;
            LayoutManager manager = LayoutManager.FindManagerForControl(this);
            switch (click)
            {
                case Group.ButtonClickStyle.Expand:
                    this.ExpandGroup(g);
                    break;
                case Group.ButtonClickStyle.Collapse:
                    this.CollapseGroup(g);
                    break;
                case  Group.ButtonClickStyle.Max:
                    this.MaximizeGroup(g);
                    break;
                case Group.ButtonClickStyle.Close:
                    if (manager != null)
                        manager.HideGroup(g);
                    break;
                case Group.ButtonClickStyle.AutoHide:
                    if (manager != null)
                        manager.ToggleAutoHide(g);
                    break;
            }
        }
        #endregion

        #region ..ExpandGroup
        /// <summary>
        /// expand the group
        /// </summary>
        /// <param name="group"></param>
        internal void ExpandGroup(Group group)
        {
            //get the target size
            Size size = group.LastNormalSize;
            //if is empty size, get the initial size
            if (size.IsEmpty)
                size = group.DisplaySize;
            this.ExpandGroup(group, size);
        }

        void ExpandGroup(Group group, System.Drawing.Size size)
        {
            this.SuspendLayout();
            int delta = 0;
            int index = this._groups.IndexOf(group);
            int height = group.Height;
            int bottom = group.Height + group.Top;

            #region ..缩减其余组的多余高度补充高度差
            if (this._groups.Count > 0)
                bottom = this._groups[this._groups.Count - 1].Top + this._groups[this._groups.Count - 1].Height;

            //缩减组后面的组的多余高度以补充高度差
            for (int i = index + 1; i < this._groups.Count; i++)
            {
                Group g = this._groups[i];
                g.MaxState = false;
                Size displaySize = g.DisplaySize;
                //当前组是否最小化?
                if (g.Height > g.MinHeight)
                {
                    delta = g.Height - displaySize.Height;
                    if (height + delta >= size.Height)
                    {
                        delta = size.Height - height;
                        height = size.Height;
                        g.Height -= delta;
                        break;
                    }
                    else
                    {
                        g.Height = displaySize.Height;
                        height += delta;
                    }
                }
            }
            //如果Zone底部存在空隙，加入空隙高度
            delta = this.Height - bottom;
            height += delta;

            //如果高度差还未补足，对前面的组执行缩减操作
            if (height < size.Height)
            {
                for (int i = index - 1; i >= 0; i--)
                {
                    Group g = this._groups[i];
                    Size displaySize = g.DisplaySize;

                    //组是否最小化
                    if (g.Height > g.MinHeight)
                    {
                        delta = g.Height - displaySize.Height;
                        if (height + delta >= size.Height)
                        {
                            delta = size.Height - height;
                            height = size.Height;
                            g.Height -= delta;
                            break;
                        }
                        else
                        {
                            g.Height = displaySize.Height;
                            height += delta;
                        }
                    }
                }
            }
            #endregion

            #region ..如果高度差还未补足，最小化某些组以补足
            //如果高度差还未补足，最小化某些组以补足
            if (height < size.Height)
            {
                //从后面的组开始
                for (int i = index + 1; i < this._groups.Count; i++)
                {
                    Group g = this._groups[i];
                    //判断组是否最小化
                    if (g.Height > g.MinHeight)
                    {
                        delta = g.Height - g.MinHeight;
                        height += delta;
                        g.Height = g.MinHeight;
                        if (height >= size.Height)
                            break;
                    }
                }
                if (height < size.Height)
                {
                    //如果还未完成，继续对前面的组进行操作
                    for (int i = index - 1; i >= 0; i--)
                    {
                        Group g = this._groups[i];
                        //判断组是否最小化
                        if (g.Height > g.MinHeight)
                        {
                            delta = g.Height - g.MinHeight;
                            height += delta;
                            g.Height = g.MinHeight;
                            if (height >= size.Height)
                                break;
                        }
                    }
                }
            }
            #endregion

            #region ..将计算的高度赋予组
            //判断高度是否小于组需要的显示高度，如果是，将组设置为最小化
            Size displaySize1 = group.DisplaySize;
            if (height < displaySize1.Height)
                height = group.MinHeight;
            group.Height = height;
            #endregion

            this.FlowLayoutGroups();
            this.ResumeLayout(true);
        }
        #endregion

        #region ..CollapseGroup
        /// <summary>
        /// Collapse group
        /// </summary>
        /// <param name="group"></param>
        internal void CollapseGroup(Group group)
        {
            this.SuspendLayout();
            int delta = group.Height - group.MinHeight;
            group.Height = group.MinHeight;
            if (delta > 0)
            {
                bool add = false;
                int index = this._groups.IndexOf(group);
                for (int i = index + 1; i < this._groups.Count; i++)
                {
                    Group g = this._groups[i] as Group;
                    if (g.Height > g.MinHeight)
                    {
                        g.Height += delta;
                        add = true;
                        break;
                    }
                }

                if (!add)
                {
                    for (int i = index - 1; i >= 0; i--)
                    {
                        Group g = this._groups[i] as Group;
                        if (g.Height > g.MinHeight)
                        {
                            g.Height += delta;
                            add = true;
                            break;
                        }
                    }
                }
            }
            this.FlowLayoutGroups();
            this.ResumeLayout(true);
        }
        #endregion

        #region ..FillGroup
        /// <summary>
        /// fill the group so that it can fill the region left
        /// </summary>
        /// <param name="group"></param>
        internal void FillGroup(Group group)
        {
            this.SuspendLayout();
            int height = 0;
            int top = 0;
            int[] heights = new int[this._groups.Count];
            int i = 0;
            int index = this._groups.IndexOf(group);
            if (index < 0)
                return;
            foreach (Group g in this._groups)
            {
                height += g.Margin.Top;
                if (g != group)
                {
                    //if (g.Height != g.MinHeight)
                    //    g.Height = g.DisplaySize.Height;
                    heights[i] = g.MinHeight;
                    height += heights[i];
                }
                i++;
                height += g.Margin.Bottom;
            }
            int actualHeight = this.Height - height;
            //if the minimal height is upper the actual height, just set all as min height
            if (actualHeight < group.MinHeight)
                heights[index] = group.MinHeight;
            else
            {
                int delta = actualHeight - group.DisplaySize.Height;
                i = 0;
                while (delta > 0 &&i < this._groups.Count)
                {
                    if (i != index)
                    {
                        Group g = this._groups[i];
                        int delta1 = g.DisplaySize.Height - g.MinHeight;
                        if (delta >= delta1)
                        {
                            heights[i] = g.DisplaySize.Height;
                            actualHeight -= delta1;
                        }

                        delta -= delta1;
                    }
                    i++;
                }
            }

            heights[index] = actualHeight;
            i = 0;
            foreach (Group g in this._groups)
            {
                g.Height = heights[i];
                top += g.Margin.Top;
                g.Top = top;
                top += g.Height;
                top += g.Margin.Bottom;
                i++;
            }

            this.FlowLayoutGroups();
            this.ResumeLayout(false);
            this.Refresh();
        }
        #endregion

        #region ..MaximizeGroup
        /// <summary>
        /// Maximize the group
        /// </summary>
        /// <param name="group"></param>
        internal void MaximizeGroup(Group group)
        {
            this.SuspendLayout();
            int height = 0;
            foreach (Group g in this._groups)
            {
                height += g.Margin.Top;
                //minimize the other group
                if (g != group)
                {
                    g.Height = group.MinHeight;
                    height += g.Height;
                }
                height += g.Margin.Bottom;
            }

            Size displaySize = group.DisplaySize;
            height = this.Height - height;

            //if height < display height, minimize it
            if (height < displaySize.Height)
                height = group.MinHeight;

            group.Height = (int)Math.Max(group.MinHeight, height);
            group.MaxState = true;
            this.FlowLayoutGroups();
            this.ResumeLayout(true);
        }
        #endregion

        #region ..PreSetGroupSize
        /// <summary>
        /// when you want to set the group as the special size, update the other _groups
        /// </summary>
        /// <param name="group">the group you want to set it's size</param>
        /// <param name="size">the size you want to set</param>
        void PreSetGroupSize(Group group, Size size)
        {

        }
        #endregion

        #region ..OnControlAdded
        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            //set the width of control as current width
            e.Control.Width = this.Width;
            e.Control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            e.Control.SizeChanged += new EventHandler(Control_SizeChanged);
        }

        void Control_SizeChanged(object sender, EventArgs e)
        {
            Control c = sender as Control;
            //make sure the control fill the width 
            if (c != null)
                c.Width = this.Width;
        }
        #endregion

        #region ..OnControlRemoved
        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);
            //remove the delegate
            e.Control.SizeChanged -= new EventHandler(Control_SizeChanged);

            //if all the controls are removed, remove this from it's parent
            if (this.Controls.Count == 0 && this.Parent != null && !this.Float )
                this.Parent.Controls.Remove(this);
        }
        #endregion

        #region ..PreRemoveGroup
        /// <summary>
        /// when the group will be removed, update the other _groups
        /// </summary>
        void PreRemoveGroup(int index,Group group)
        {
            this.SuspendLayout();
            if (index >= 0 && index < this._groups.Count )
            {
                Group preGroup = null;
                //find the group want to change the size
                for(int i = index ;i<this._groups.Count;i++)
                {
                    Group g = this._groups[i];
                    if(!g.IsMinimize)
                    {
                        preGroup = g;
                        break;
                    }
                }

                //if pregroup is null, find it in the above groups
                if (preGroup == null)
                {
                    for (int i = 0; i < index; i++)
                    {
                        Group g = this._groups[i];
                        if (!g.IsMinimize)
                        {
                            preGroup = g;
                            break;
                        }
                    }
                }

                //if pregroup == null ,return
                if (preGroup == null)
                    preGroup = this._groups[index];

                //cycle the collection
                int height = 0;
                foreach (Group g in this._groups)
                {
                    if (g == group)
                        continue;
                    height += g.Margin.Top;
                    if (g != preGroup)
                        height += g.Height;
                    height += g.Margin.Bottom;
                }

                height = (int)Math.Max(preGroup.MinHeight, this.Height - height);
                if(height < preGroup.DisplaySize.Height)
                    height = preGroup.MinHeight;
                preGroup.Height = height;
            }

            this.ResumeLayout(true);
        }
        #endregion

        #region ..FlowLayoutGroups
        /// <summary>
        /// Arrange Group so that flow layout the group
        /// </summary>
        void FlowLayoutGroups()
        {
            int height = 0;
            ArrayList list = new ArrayList();
            list.AddRange(this._groups );
            for(int i = 0;i<this._groups.Count;i ++)//each (Group group in this.Groups)
            {
                Group group = this._groups[i];

                //adjust the margin
                if (i < this._groups.Count - 1)
                    group.Margin = groupMargin;
                else
                    group.Margin = new Padding(0);

                group.MaxState = false;
                height += group.Margin.Top;
                if (group.Height == group.MinHeight )
                    list.Remove(group);
                group.Top = height;
                height += group.Height;
                height += group.Margin.Bottom;
            }
            if (list.Count == 1)
                (list[0] as Group).MaxState = true;
        }
        #endregion

        #region ..OnResize
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            
            if (this.FindForm() != null && this.FindForm().WindowState == FormWindowState.Minimized)
                return;
                int delta = this.Size.Height - this.oriSize.Height;
                int top = 0;
                ArrayList list = new ArrayList();
                ArrayList list1 = new ArrayList();
                list1.AddRange(this._groups);
                foreach (Group g in this._groups)
                {
                    top += g.Margin.Top;
                    g.Top = top;
                    g.MaxState = false;
                    if (g.Height == g.MinHeight)
                        list1.Remove(g);
                    if (g.Height > g.MinHeight)
                    {
                        int h = g.Height;
                        h += delta;
                        if (h > g.DisplaySize.Height)
                        {
                            g.Height = h;
                            delta = 0;
                        }
                        else
                        {
                            delta += g.Height - g.DisplaySize.Height;
                            g.Height = g.DisplaySize.Height;
                        }
                    }
                    top += g.Height;
                    if (g.Height > g.MinHeight)
                        list.Add(g);
                    top += g.Margin.Bottom;
                }
                if (delta < 0 && list.Count > 0)
                {
                    int index = this._groups.IndexOf(list[0] as Group);
                    top = ((Group)list[0]).Top;
                    for (int i = index; i < this._groups.Count; i++)
                    {
                        Group g = this._groups[i] as Group;
                        top += g.Margin.Top;
                        g.Top = top;
                        if (g.Height > g.MinHeight)
                        {
                            if (delta < 0 && !this.Float)
                            {
                                delta += g.Height - g.MinHeight;
                                g.Height = g.MinHeight;
                            }
                            else
                                g.Height += delta;
                        }
                        top += g.Height;
                        top += g.Margin.Bottom;
                    }
                }
                if (list1.Count == 1)
                    ((Group)list1[0]).MaxState = true;
            this.oriSize = this.Size;
        }
        #endregion

        #region ..OnMouseDown
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.mouseDown = e.Button == MouseButtons.Left;
            //rememble the origin size and origin point
            if (this.mouseDown && this.resizeGroup != null)
            {
                this.startPoint = new Point(e.X, e.Y);
                this.startSize = this.resizeGroup.Size;
                resizeTop = e.Y < this.resizeGroup.Bottom;
            }
        }
        #endregion

        #region ..OnMouseMove
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            #region ..MouseButtons.None, find the group can be resized
            if (!this.mouseDown)
            {
                //reset the resizeable firstly
                this.resizeGroup = null;
                maxResize = Size.Empty;
                int top = 0;
                bool resizeable = false;
                int minHeight = 0;
                //judge whether the group can be resized
                for (int i = 0; i < this._groups.Count; i++)
                {
                    Group group = this._groups[i];
                    //if some group is max, return
                    if (group.MaxState)
                    {
                        resizeGroup = null;
                        return;
                    }

                    top += group.Margin.Top;
                    minHeight += group.Margin.Top;
                    top += group.Height;
                    
                    if (!group.IsMinimize)
                        minHeight += group.DisplaySize.Height;
                    else
                        minHeight += group.MinHeight;

                    //if the mouse is locate the botto margin
                    if (top <= e.Y && (top + group.Margin.Bottom >= e.Y))
                    {
                        //if the group is not min
                        if (!group.IsMinimize)
                            this.resizeGroup = group;
                        else if (i < this._groups.Count - 1)
                            this.resizeGroup = this._groups[i + 1];
                    }

                    //if group is not max or min,
                    if (!group.IsMinimize)
                    {
                        Size displaySize = group.DisplaySize;
                        Size currentSize = group.Size;

                        resizeable = resizeable || currentSize.Height > displaySize.Height;
                    }
                    

                    top += group.Margin.Bottom;
                    minHeight += group.Margin.Bottom;

                    //if the group can be resized, break
                    //if (this.resizeGroup != null && resizeable)
                    //    break;
                    //or continue judge
                }

                if (this.resizeGroup != null && resizeable && !this.resizeGroup.MaxState && !this.resizeGroup.IsMinimize)
                {
                    this.Cursor = Cursors.HSplit;

                    //re calcuate the delta as the absolute size
                    this.maxResize = new Size(this.resizeGroup.Width, this.Height - minHeight + this.resizeGroup.DisplaySize.Height);
                }
                else
                    this.Cursor = Cursors.Default;
            }
            #endregion

            #region ..MouseButtons.Left, resize the group
            else if (this.resizeGroup != null)
            {
                int delta = e.Y - this.startPoint.Y;
                Size size = this.startSize;
                if (this.resizeTop)
                    size = new Size(this.Width, this.startSize.Height - delta);
                else
                    size = new Size(this.Width, this.startSize.Height + delta);
                size = new Size(this.Width, (int)Math.Min(this.maxResize.Height, Math.Max(this.resizeGroup.DisplaySize .Height, size.Height)));
                delta = size.Height - this.resizeGroup.Height;
                //adjust the size of the group
                this.AdjustGroupSize(this.resizeGroup, delta);
            }
            #endregion
        }
        #endregion

        #region ..OnMouseUp
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            this.resizeGroup = null;
            this.mouseDown = false;
        }
        #endregion

        #region ..OnMouseLeave
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.Cursor = Cursors.Default;
        }
        #endregion

        #region ..AdjustGroupSize
        /// <summary>
        /// adjust the size of the group using the delta
        /// </summary>
        /// <param name="delta">the delta you want to changed</param>
        void AdjustGroupSize(Group group,int delta)
        {
            if (delta == 0)
                return;
            this.SuspendLayout();
            int index = this._groups.IndexOf(group);
            if (index >= 0)
            {
                //this.SuspendLayout();
                group.Size = new Size(this.Width, group.Height + delta);

                //update the size of the other groups to flow layout 

                //the previous groups
                for (int i = 0; i < this._groups.Count ; i++)
                {
                    Group g = this._groups[i];
                    if (g == group)
                        continue;
                    //only un minimize window can be resized
                    if (!g.IsMinimize)
                    {
                        int maxdelta = g.Height - g.DisplaySize.Height;
                        int delta1 = (int)Math.Min(maxdelta, delta);
                        g.Height -= delta1;
                        delta -= delta1;
                    }

                    if (delta == 0)
                        break;
                }

                group.Height += delta;
                //re layout the groups
                this.FlowLayoutGroups();
                this.Invalidate(true);
                this.Update();
            }
            this.ResumeLayout(true);
        }
        #endregion
    }
}
