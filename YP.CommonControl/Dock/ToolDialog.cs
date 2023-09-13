using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace YP.CommonControl.Dock
{
    /// <summary>
    /// define the dialog to store the user group
    /// </summary>
    internal class ToolDialog:Common.BaseForm,IMessageFilter
    {
        #region ..Constructor
        public ToolDialog(LayoutManager manager)
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.ShowInTaskbar = false;
            this._manager = manager;
            _zone = new Zone();
            this._zone.Float = true;
            _zone.Dock = System.Windows.Forms.DockStyle.Fill;
            _zone.Groups.Inserted += new Common.CollectionWithEvents.CollectionEventHandler(Groups_Inserted);
            _zone.Groups.Removed += new Common.CollectionWithEvents.CollectionEventHandler(Groups_Removed);
            this.Controls.Add(_zone);
            Application.AddMessageFilter(this);
        }
        #endregion

        #region ..private fields
        LayoutManager _manager = null;
        GroupCollection _groups = new GroupCollection();
        Zone _zone;
        Rectangle[] reverseRects = null;
       // bool mouseDown = false;
        #endregion

        #region ..properties
        public GroupCollection Groups
        {
            get
            {
                return this._zone.Groups;
            }
        }

        #endregion

        #region ..WndProc
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            //if double click on the title, quit the floating
            if (m.Msg == (int)Win32.Enum.Msgs.WM_NCLBUTTONDBLCLK)
            {
                this.ReturnToDock();
                return;
            }
                //right mouse up
            else if (m.Msg == (int)Win32.Enum.Msgs.WM_NCRBUTTONDOWN )
            {
                this._manager.groupContext.Tag = this;
                this._manager.groupContext.Show(System.Windows.Forms.Control.MousePosition);
                return;
            }
            
            base.WndProc(ref m);
        }
        #endregion

        #region ..ReturnToDock
        internal void ReturnToDock()
        {
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            //remember the selected index
            System.Collections.Hashtable hs = new System.Collections.Hashtable();
            //hide the contents before
            for (int i = 0; i < this._zone.Groups.Count; i++)
            {
                foreach (Content c in _zone.Groups[i].Contents)
                {
                    //c.Float = false;
                    list.Add(c);
                    hs[c] = _zone.Groups[i].SelectedIndex;
                }
                Group group = _zone.Groups[i];
                this._manager.HideGroup(group);
                group.ShowTitle = true;
                i--;
            }

            //show the content 
            foreach (Content c in list)
            {
                c.Float = false;
                this._manager.ShowContent(c);
            }
            //select the origin selected index
            foreach (Content c in hs.Keys)
                c.ParentGroup.SelectedIndex = (int)hs[c];
        }
        #endregion

        #region ..ToolDialog_TextChanged
        void ToolDialog_TextChanged(object sender, EventArgs e)
        {
            Group group = sender as Group;
            this.Text = group.Text;
        }
        #endregion

        #region ..Groups_Inserted
        void Groups_Inserted(int index, object value)
        {
            Group group = value as Group;
            group.ShowTitle = false;
            group.TextChanged += new EventHandler(group_TextChanged);
            UpdateDialogStatus();
        }
        #endregion

        #region ..Groups_Removed
        void Groups_Removed(int index, object value)
        {
            (value as Group).TextChanged -= new EventHandler(group_TextChanged);
            UpdateDialogStatus();
        }
        #endregion

        #region ..group_TextChanged
        void group_TextChanged(object sender, EventArgs e)
        {
            this.Text = (sender as Group).Text;
        }
        #endregion

        #region ..UpdateDialogStatus
        void UpdateDialogStatus()
        {
            if (this._zone.Groups.Count > 0)
            {
                this.Show();
                //if the zone doesn't exist in the container ,add it
                if (!this.Controls.Contains(this._zone))
                {
                    _zone.Dock = System.Windows.Forms.DockStyle.Fill;
                    this.Controls.Add(_zone);
                }
            }
            else
                this.Hide();
        }
        #endregion

        #region ..OnClosing
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            //hide the contents before close
            for (int i = 0; i < this._zone.Groups.Count; i++)
            {
                this._manager.HideGroup(_zone.Groups[i]);
                i--;
            }

            //set cancel as true so that don't close the form, just hide
            e.Cancel = true;
            base.OnClosing(e);
            this.Hide();
        }
        #endregion

        #region ..FillReverseRectangle
        void FillReverseRectangles()
        {
            if (this.reverseRects != null)
            {
                foreach (Rectangle rect in this.reverseRects)
                    // ControlPaint.DrawReversibleFrame(rect,Color.Black, FrameStyle.Thick);//(rect, Color.FromArgb(30, SystemColors.Highlight));
                    ControlPaint.FillReversibleRectangle(rect, Color.FromArgb(10, SystemColors.Highlight));
            }
        }
        #endregion

        public bool PreFilterMessage(ref Message m)
        {
            //if (m.Msg == (int)Common.Win32.Enum.Msgs.WM_NCLBUTTONDOWN)
            //{
            //    if(m.HWnd == this.Handle)
            //        this.mouseDown = true;
            //}
            //else if (m.Msg == (int)Common.Win32.Enum.Msgs.WM_NCLBUTTONUP)
            //{
            //    if (m.HWnd == this.Handle)
            //        this.mouseDown = false;
            //}
            
            return false;
        }
    }
}
