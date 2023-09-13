using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using YP.SVG;
using YP.SVG.Property;

namespace YP.SymbolDesigner.Document
{
    class PropertyGenerator : IPropertyGenerator
    {
        public virtual IProperty[] GeneratePropertiesForElement(SVGElement element)
        {
            List<IProperty> properties = new List<IProperty>();
            if(element != null)
            {
                properties.Add(new CustomPropertyItem("ID", typeof(string), "常规", "id"));

                if(element is SVGStyleable)
                {
                    properties.Add(new CustomPropertyItem( "Fill", typeof(Color), "填充", "设置对象的填充色", "fill"));
                    properties.Add(new CustomPropertyItem( "FillOpacity", typeof(float), "填充", "设置对象的填充透明度", "fill-opacity"));
                    properties.Add(new CustomPropertyItem( "PatternColor", typeof(Color), "填充", "设置对象的填充图案色", "hatchcolor"));

                    //append the stroke properties
                    properties.Add(new CustomPropertyItem( "Stroke", typeof(Color), "线条", "设置对象的线条色", "stroke"));
                    properties.Add(new CustomPropertyItem( "StrokeOpacity", typeof(float), "线条", "设置对象的线条透明度", "stroke-opacity"));
                    properties.Add(new CustomPropertyItem( "StrokeDashArray", typeof(string), "线条", "设置对象的线条样式,值是一个用空格\"　\"分割的数字字符串，如\"3,1,3\"，如果不应用样式，则为\"none\"", "stroke-dasharray"));
                    properties.Add(new CustomPropertyItem( "StrokeWidth", typeof(float), "线条", "设置对象的线条宽度", "stroke-width"));

                    //append shadow properties
                    properties.Add(new CustomPropertyItem( "ShowShadow", typeof(bool), "阴影", "是否显示阴影", "shadow"));
                    properties.Add(new CustomPropertyItem( "ShadowColor", typeof(Color), "阴影", "阴影颜色", "shadowColor"));
                    properties.Add(new CustomPropertyItem( "ShadowOpacity", typeof(float), "阴影", "设置阴影的透明度", "shadowOpacity"));
                    properties.Add(new CustomPropertyItem( "XOffset", typeof(float), "阴影", "设置阴影的横向偏移", "xOffset"));
                    properties.Add(new CustomPropertyItem( "YOffset", typeof(float), "阴影", "设置阴影的纵向偏移", "yOffset"));


                    //append the label properties
                    properties.Add(new CustomPropertyItem( "TextColor", typeof(Color), "文本", "指定文本块的颜色", "text-color"));
                    properties.Add(new CustomPropertyItem( "TextAnchor", typeof(SVG.Alignment), "文本", "文本块在其容器空间内的水平对齐方式,", "text-anchor"));
                    properties.Add(new CustomPropertyItem( "Vectical-Align", typeof(SVG.VerticalAlignment), "文本", "文本块在其容器空间内竖直对齐方式", "vertical-align"));

                    //append the font properties
                    properties.Add(new CustomPropertyItem( "Font", typeof(string), "字体", "设置绘制文本的字体名称", string.Empty, "YP.Canvas.Forms.FontEditor", "", "font-family"));
                    properties.Add(new CustomPropertyItem( "FontSize", typeof(float), "字体", "设置用来绘制文本的字体的大小", "12", "YP.Canvas.Forms.ListFontSize", "", "font-size"));
                    properties.Add(new CustomPropertyItem( "Bold", typeof(bool), "字体", "确定文本是否采用粗体绘制", "font-weight"));
                    properties.Add(new CustomPropertyItem( "Italic", typeof(bool), "字体", "确定文本是否采用斜体绘制", "font-style"));
                    properties.Add(new CustomPropertyItem( "Underline", typeof(bool), "字体", "确定文本是否有下划线", "text-decoration"));
                }
            }

            return properties.ToArray();
        }
    }
}
