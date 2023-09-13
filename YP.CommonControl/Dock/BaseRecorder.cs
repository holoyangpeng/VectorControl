using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace YP.CommonControl.Dock
{
    /// <summary>
    /// record the infomation for the common action
    /// </summary>
    internal abstract class BaseRecorder
    {
        #region ..Constructor
        public BaseRecorder(SplitterPanel parentPanel)
        {
            this._parentPanel = parentPanel;
        }
        #endregion

        #region ..private fields
        //remember the origin splitterpanel which contais the group
        SplitterPanel _parentPanel;
        #endregion

        #region ..public properties
        /// <summary>
        /// sets or gets the parent panel which contains the group before auto hide
        /// </summary>
        internal SplitterPanel ParentPanel
        {
            set
            {
                this._parentPanel = value;
            }
            get
            {
                return this._parentPanel;
            }
        }
        #endregion
    }
}
