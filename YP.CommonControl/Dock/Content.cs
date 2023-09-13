using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace YP.CommonControl.Dock
{
    /// <summary>
    /// define a content
    /// </summary>
    public class Content
    {
        #region ..Constructor
        public Content(string title,Control c)
        {
            this._title = title;
            this._control = c;
        }
        #endregion

        #region ..private fields
        string _title = string.Empty;
        Control _control = null;
        System.Drawing.Image _image = null;
        object _tag = null;
        //remember the intialize index of the content
        int _contentIndex = 0;
        bool _float = false;
        #endregion

        #region ..Event
        /// <summary>
        /// occurs when the content changes
        /// </summary>
        internal event EventHandler Changed;
        #endregion

        #region ..internal fields
        internal Group _parentGroup = null;
        #endregion

        #region ..internal properties
        /// <summary>
        /// gets or sets a value indicates whether the float status of the content
        /// </summary>
        internal bool Float
        {
            get
            {
                return this._float;
            }
            set
            {
                this._float = value;
            }
        }

        /// <summary>
        /// gets or sets the content index 
        /// </summary>
        internal int ContentIndex
        {
            set
            {
                this._contentIndex = value;
            }
            get
            {
                return this._contentIndex;
            }
        }
        #endregion

        #region ..public properties
        /// <summary>
        /// gets or sets the title of the content
        /// </summary>
        public string Title
        {
            set
            {
                if (this._title != value)
                {
                    this._title = value;
                    this.OnChanged();
                }
            }
            get
            {
                return this._title;
            }
        }

        /// <summary>
        /// gets or sets the data of the element
        /// </summary>
        public object Tag
        {
            set
            {
                this._tag = value;
            }
            get
            {
                return this._tag;
            }
        }

        /// <summary>
        /// sets or gets the image of the content
        /// </summary>
        public Image Image
        {
            set
            {
                if (this._image != value)
                {
                    this._image = value;
                    this.OnChanged();
                }
            }
            get
            {
                return this._image;
            }
        }

        /// <summary>
        /// gets the assiatted control
        /// </summary>
        public Control Control
        {
            get
            {
                return this._control;
            }
            set
            {
                if (this._control != value)
                {
                    this._control = value;
                    this.OnChanged();
                }
            }
        }

        /// <summary>
        /// gets the parent group
        /// </summary>
        public Group ParentGroup
        {
            get
            {
                return this._parentGroup;
            }
        }
        #endregion

        #region ..OnChanged
        protected virtual void OnChanged()
        {
            if (this.Changed != null)
                this.Changed(this, EventArgs.Empty);
        }
        #endregion
    }
}
