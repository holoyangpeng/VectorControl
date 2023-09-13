using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace YP.SymbolDesigner.Controls
{
    public class ToolStripFontSizeComboBox:ToolStripComboBox
    {
        #region ..ToolStripFontSizeComboBox
        public ToolStripFontSizeComboBox()
        {
            object[] values = new object[] { 8f, 10f, 12f, 14f, 16f, 18f, 20f, 24f, 28f, 36f, 46f, 60f };
            this.Items.AddRange(values);
            this.Text = "14";
        }
        #endregion

        #region ..properties
        public float SelectedValue
        {
            get
            {
                float a = 14;
                if (float.TryParse(this.Text, out a))
                    return a;
                return 14;
            }
            set
            {
                this.Text = value.ToString();
            }
        }
        #endregion
    }
}
