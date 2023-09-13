using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace YP.CommonControl.Dock
{
    /// <summary>
    /// record the information to revert the auto hide control
    /// </summary>
    internal class AutoHideRecorder:BaseRecorder
    {
        #region ..Constructor
        public AutoHideRecorder(SplitterPanel parentPanel):base(parentPanel)
        {
        }
        #endregion

        #region ..private fields
        private IGroupContainer _groupContainer = null;
        #endregion

        #region ..public properties
        /// <summary>
        /// sets or gets the container to store the parent group
        /// </summary>
        public IGroupContainer GroupContainer
        {
            get
            {
                return this._groupContainer;
            }
            set
            {
                this._groupContainer = value;
            }
        }
        #endregion
    }
}
