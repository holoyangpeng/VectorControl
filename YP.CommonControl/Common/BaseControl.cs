using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace YP.CommonControl.Common
{
    /// <summary>
    /// define the base class for all the user control
    /// </summary>
    public partial class BaseControl : UserControl
    {
        #region ..Constructor
        public BaseControl()
        {
            InitializeComponent();
        }
        #endregion

        #region ..protected fields
        protected Pen darkPen = new Pen(SystemColors.ControlDark);
        protected Pen lightPen = new Pen(SystemColors.ControlLight);
        protected Pen backPen = new Pen(SystemColors.Control);
        protected Pen lightlightPen = new Pen(SystemColors.ControlLightLight);
        protected SolidBrush lightBrush = new SolidBrush(SystemColors.ControlLight);
        protected SolidBrush lightlightBrush = new SolidBrush(SystemColors.ControlLightLight);
        protected SolidBrush darkBrush = new SolidBrush(SystemColors.ControlDark);
        protected SolidBrush darkdarkBrush = new SolidBrush(SystemColors.ControlDarkDark);
        protected Pen darkdarkPen = new Pen(SystemColors.ControlDarkDark);
        /// <summary>
        /// not implement
        /// </summary>
        protected bool ideBorder = false;
        #endregion

        #region ..properties
        protected virtual Color DarkDarkColor
        {
            get
            {
                return ControlPaint.DarkDark(this.BackColor);
            }
        }

        protected virtual Color LightLightColor
        {
            get
            {
                return ControlPaint.LightLight(this.BackColor);
            }
        }

        protected virtual Color LightColor
        {
            get
            {
                return ControlPaint.Light(this.BackColor);
            }
        }

        /// <summary>
        /// return a boolean value indicates whether the control has a child focus or has child is popuping
        /// </summary>
        public virtual bool HasFocus
        {
            get
            {
                bool focus = this.ContainsFocus;
                if (!focus)
                {
                    foreach (System.Windows.Forms.Control c in this.Controls)
                    {
                        if (c is BaseControl)
                            focus = focus || (c as BaseControl).HasFocus;
                        else
                            focus = focus || c.ContainsFocus;

                        if (focus)
                            break;
                    }
                }
                return focus;
            }
        }

        protected virtual Color DarkColor
        {
            get
            {
                return ControlPaint.Dark(this.BackColor);
            }
        }

        protected virtual Color LightLightLightLightColor
        {
            get
            {
                return ControlPaint.LightLight(ControlPaint.LightLight(this.BackColor));
            }
        }
        #endregion

    }
}
