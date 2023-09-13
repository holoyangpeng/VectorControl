using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace YP.CommonControl.Dock
{
    /// <summary>
    /// define the recorder to store the information to revert the content
    /// </summary>
    internal class ContentRecorder : AutoHideRecorder
    {
        #region ..Constructor
        public ContentRecorder(SplitterPanel parentPanel)
            : base(parentPanel)
        {

        }
        #endregion

        #region ..private fields
        private Group _parentGroup = null;
        private IGroupContainer _floatGroupContainer = null;
        private Group _floatParentGroup = null;
        #endregion

        #region ..public properties
        /// <summary>
        /// gets or sets the parent group when content is float
        /// </summary>
        public Group FloatParentGroup
        {
            set
            {
                this._floatParentGroup = value;
            }
            get
            {
                return this._floatParentGroup;
            }
        }

        /// <summary>
        /// if the content is float, remember the group container
        /// </summary>
        public IGroupContainer FloatGroupContainer
        {
            set
            {
                this._floatGroupContainer = value;
            }
            get
            {
                return this._floatGroupContainer;
            }
        }

        /// <summary>
        /// gets the origin parent group of the content
        /// </summary>
        public Group ParentGroup
        {
            set
            {
                this._parentGroup = value;
            }
            get
            {
                return this._parentGroup;
            }
        }
        #endregion
    }
}
