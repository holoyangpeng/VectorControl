using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace YP.SymbolDesigner.Controls
{
    public class ToolStripFontNameComboBox:ToolStripComboBox
    {
        #region ..构造函数
        public ToolStripFontNameComboBox()
        {
            FontFamily[] families = System.Drawing.FontFamily.Families;
            for (int j = 0; j < families.Length; j++)
            {
                this.Items.Add(families[j].Name);
            }

            this.Text = "微软雅黑";

            this.ComboBox.DrawMode = DrawMode.OwnerDrawVariable;
            this.ComboBox.DrawItem += new DrawItemEventHandler(ComboBox_DrawItem);
            this.ComboBox.MeasureItem += new MeasureItemEventHandler(ComboBox_MeasureItem);

            this.ComboBox.Height = 300;
        }
        #endregion

        #region ..ComboBox_DrawItem
        void ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            string text = this.Items[e.Index] as string;
            using (Font font = GetFont(text))
            {
                using (StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
                {
                    sf.FormatFlags = StringFormatFlags.NoWrap;
                    using (SolidBrush brush = new SolidBrush(e.ForeColor))
                        e.Graphics.DrawString(text, font, brush, e.Bounds, sf);
                }
            }
        }
        #endregion

        #region ..ComboBox_MeasureItem
        void ComboBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            string text = this.Items[e.Index] as string;
            using (Font font = this.GetFont(text))
            {
                e.ItemHeight = TextRenderer.MeasureText(text, font).Height + 6;
            }
        }
        #endregion

        #region ..GetFont
        Font GetFont(string text)
        {
            FontStyle style = FontStyle.Regular;
            using (FontFamily family = new FontFamily(text))
            {
                FontStyle[] styles = { FontStyle.Regular, FontStyle.Bold, FontStyle.Italic };
                
                foreach (FontStyle style1 in styles)
                    if (family.IsStyleAvailable(style1))
                    {
                        style = style1;
                        break;
                    }
            }

            return new Font(text, 12, style);
        }
        #endregion
    }
}
