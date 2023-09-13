using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YP.SymbolDesigner.Controls
{
    public partial class Navigator : UserControl
    {
        #region ..Constructor
        public Navigator()
        {
            InitializeComponent();
        }
        #endregion

        #region ..private fields
        YP.VectorControl.Canvas vectorControl = null;
        #endregion

        #region ..public properties
        public YP.VectorControl.Canvas VectorControl
        {
            set
            {
                if (this.vectorControl != value)
                {
                    this.Controls.Clear();
                    this.vectorControl = value;
                    if (this.vectorControl != null)
                    {
                        this.vectorControl.ThumbnailView.Dock = DockStyle.Fill;
                        this.vectorControl.ThumbnailView.BackColor = Color.White;
                        this.Controls.Add(this.vectorControl.ThumbnailView);
                    }
                }
            }
        }
        #endregion
    }
}
