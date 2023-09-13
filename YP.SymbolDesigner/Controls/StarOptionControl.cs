using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using YP.VectorControl;
using System.Windows.Forms;

namespace YP.SymbolDesigner.Controls
{
    public partial class StarOptionControl : UserControl
    {
        #region ..Constructor
        public StarOptionControl()
        {
            InitializeComponent();
        }
        #endregion

        #region ..properties
        public Star Value
        {
            set
            {
                this.vertexPicker.Value = value.NumberOfVertexes;
                this.indentPicker.Value = (int)(value.Indent * 100);
            }
            get
            {
                int num = (int)this.vertexPicker.Value;
                int indent = (int)this.indentPicker.Value;
                float ind = (float)indent / 100f;
                return new Star(num, ind);
            }
        }
        #endregion
    }
}
