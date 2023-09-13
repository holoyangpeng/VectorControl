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
    public partial class RectOptionControl : UserControl
    {
        #region ..Constructor
        public RectOptionControl()
        {
            InitializeComponent();
        }
        #endregion

        #region ..properties
        public int Value
        {
            set
            {
                this.numericUpDown1.Value = value;
            }
            get
            {
                return (int)this.numericUpDown1.Value;
            }
        }
        #endregion
    }
}
