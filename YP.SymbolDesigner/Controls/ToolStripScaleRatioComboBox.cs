using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YP.SymbolDesigner.Controls
{
    public class ToolStripScaleRatioComboBox:ToolStripComboBox
    {
        #region ..Constructor
        public ToolStripScaleRatioComboBox()
        {
            this.Items.AddRange(new string[] { "10%", "20%", "40%", "60%", "80%", "100%",
                "150%", "200%", "250%", "300%", "400%", "500%", "600%" });

            this.Text = "100%";
        }
        #endregion

        #region ..properties
        /// <summary>
        /// 获取或设置当前百分比
        /// </summary>
        public float Value
        {
            get
            {
                float a = 1;
                string text = this.Text.Replace("%", string.Empty);
                if (float.TryParse(text, out a))
                    return a  / 100.0f;
                return 1;
            }
            set
            {
                float temp = value < 0.05f ? 0.05f : value;
                string text = string.Format("{0}%", (int)(temp * 100));
                this.Text = text;
            }
        }
        #endregion
    }
}
