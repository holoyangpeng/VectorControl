using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using YP.SVG.Text;

namespace YP.SVG.Render
{
    public class SVGTextRenderer:SVGDirectRenderer
    {
        #region ..static
        static System.Windows.Forms.Label lb = new System.Windows.Forms.Label();
        static Regex textRegex = new Regex(@"^\d", RegexOptions.Compiled);
        #endregion

        #region ..Constructor
        public SVGTextRenderer(Text.SVGTextContentElement ownerElement)
            : base(ownerElement)
        {
            this.Render = this.ParseContent = true;
        }
        #endregion

        #region ..private fields
        List<Text.TextContentInfo> textContentInfos = new List<Text.TextContentInfo>();
        GraphicsPath oriPath = new GraphicsPath();
        ArrayList poses = new ArrayList();
        Cache.TextCache textCache = new Cache.TextCache();
        #endregion

        #region ..public properties
        /// <summary>
        /// 确定是否绘制
        /// </summary>
        public bool Render {set;get;}

        public bool ParseContent { set; get; }
        #endregion

        #region ..Draw
        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="g">画布</param>
        /// <param name="sp">类型容器</param>
        public override void Draw(System.Drawing.Graphics g, StyleContainer.StyleOperator sp)
        {
            Text.SVGTextElement textelement = this.OwnerElement as Text.SVGTextElement;
            if (textelement == null)
                return;
            if (!this.Render)
            {
                this.OwnerElement.TransformPath(g, null, sp);
                return;
            }

            if (this.OwnerDocument.IsStopRender)
                return;

            GraphicsPath gp = (textelement as Interface.ISVGPathable).GPath;
            
            if (gp != null)
            {
                if (!BeforeDrawing(g, sp))
                    return;
            }

            try
            {
                if (sp.ViewVisible)
                {
                    //if in editing, don't draw with cache
                    if (this.OwnerDocument.DrawElementWithCache(this.OwnerElement)
                        && !textelement.InEdit)
                    {
                        this.DrawWithCache(g, sp);
                        return;
                    }
                    textelement.NeedUpdateCSSStyle = false;
                    this.textCache.Caches.Clear();
                    float lineheight = 0;
                    gp.Reset();
                    System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
                    System.Drawing.Text.TextRenderingHint hint = g.TextRenderingHint;
                    GraphicsContainer c = g.BeginContainer();
                    g.TextRenderingHint = hint;
                    System.Collections.ArrayList containers = new System.Collections.ArrayList();
                    Interface.ISVGElement preelement = null;
                    sp.BeginStyleContainer(this.OwnerElement);
                    g.SmoothingMode = mode;
                    SVGElementCollection list = new SVGElementCollection();

                    if (sp.ViewVisible && sp.VisualMediaStyle.visiblility != "hidden" && sp.VisualMediaStyle.display != "none")
                    {
                        containers.Add(textelement);
                        preelement = textelement;
                        PointF refp = PointF.Empty;
                        string shiftby = string.Empty;
                        if (!sp.TextStyle.baseline_shift.IsEmpty)
                            shiftby = sp.TextStyle.baseline_shift.Value.Trim();

                        float size = this.GetFontSize(sp, textelement);
                        System.Drawing.FontStyle style = this.GetFontStyle(sp);
                        FontFamily family = this.GetFontFamily(sp);
                        lineheight = family.GetLineSpacing(System.Drawing.FontStyle.Regular) / family.GetEmHeight(System.Drawing.FontStyle.Regular);
                        StringFormat sf = this.GetStringFormat(sp);
                        Font font = new Font(family, size, style);
                        float shift = SVGTextContentElement.GetReferencedShiftBy(size, shiftby);
                        if (!list.Contains(textelement))
                        {
                            list.Add(textelement);
                        }

                        this.OwnerElement.Clip(g, sp);
                        DataType.SVGLengthList x = null, y = null, dx = null, dy = null;
                        int xindex = 0, yindex = 0, dxindex = 0, dyindex = 0;
                        x = (DataType.SVGLengthList)textelement.X;
                        y = (DataType.SVGLengthList)textelement.Y;
                        dx = (DataType.SVGLengthList)textelement.Dx;
                        dy = (DataType.SVGLengthList)textelement.Dy;
                        //if(this.CreateTransform)
                        this.OwnerElement.TransformPath(g, null, sp);

                        #region ..绘制
                        for (int i = 0; i < textelement.TextContentInfos.Count; i++)//(Text.TextContentInfo info in textelement.TextContentInfos)
                        {
                            Text.TextContentInfo info = (Text.TextContentInfo)textelement.TextContentInfos[i];
                            Text.SVGTextContentElement text = info.OwnerTextContentElement;
                            if (text != preelement)
                            {
                                if (text.ParentNode != preelement)
                                {
                                    if (containers.Contains(preelement))
                                    {
                                        sp.EndContainer(preelement as SVGStyleable);
                                        containers.Remove(preelement);
                                    }
                                }
                                if (!containers.Contains(text))
                                {
                                    sp.BeginStyleContainer(text);
                                    containers.Add(text);
                                }
                                preelement = text;
                                size = this.GetFontSize(sp, textelement);
                                style = this.GetFontStyle(sp);
                                family = this.GetFontFamily(sp);
                                lineheight = (float)Math.Max(family.GetLineSpacing(System.Drawing.FontStyle.Regular) / family.GetEmHeight(System.Drawing.FontStyle.Regular), 0);
                                sf = this.GetStringFormat(sp);
                                font = new Font(family, size, style);
                                if (!sp.TextStyle.baseline_shift.IsEmpty)
                                {
                                    shiftby = sp.TextStyle.baseline_shift.Value.Trim();
                                    if (text.GetAttribute("baseline-shift").Trim().Length > 0 && !list.Contains(text))
                                        shift += SVGTextContentElement.GetReferencedShiftBy(size, shiftby);
                                    else
                                        shift = SVGTextContentElement.GetReferencedShiftBy(size, shiftby);
                                }
                            }
                            if (!list.Contains(text))
                            {
                                if (text is Text.SVGTextPositionElement)
                                {
                                    Text.SVGTextPositionElement postext = (Text.SVGTextPositionElement)text;
                                    if (postext.X.NumberOfItems > 0)
                                    {
                                        xindex = 0;
                                        x = (DataType.SVGLengthList)postext.X;
                                    }
                                    if (postext.Y.NumberOfItems > 0)
                                    {
                                        yindex = 0;
                                        y = (DataType.SVGLengthList)postext.Y;
                                    }
                                    if (postext.Dx.NumberOfItems > 0)
                                    {
                                        dxindex = 0;
                                        dx = (DataType.SVGLengthList)postext.Dx;
                                    }
                                    if (postext.Dy.NumberOfItems > 0)
                                    {
                                        dyindex = 0;
                                        dy = (DataType.SVGLengthList)postext.Dy;
                                    }
                                }
                                list.Add(text);
                            }
                            info.TextFont = font;
                            info.TextStringFormat = (StringFormat)sf.Clone();
                            info.X = x;
                            info.Y = y;
                            info.Dx = dx;
                            info.Dy = dy;

                            using (GraphicsPath path = info.GetGraphicsPath(g, shift, ref refp, ref xindex, ref yindex, ref dxindex, ref dyindex, sp))
                            {
                                if (path.PointCount == 0)
                                    continue;
                                shiftby = string.Empty;
                                gp.AddPath(path, false);
                                Cache.TextItemCache cache = new Cache.TextItemCache();
                                cache.CachePath = path.Clone() as GraphicsPath;
                                this.Cache = cache;
                                if (!sp.BoundView)
                                {
                                    this.DrawShadow(sp, g, path);
                                    Paint.SVGPaint svgpaint = (Paint.SVGPaint)sp.FillStyle.svgPaint;
                                    if (!svgpaint.IsEmpty)
                                        this.FillPath(g, svgpaint, path, textelement, sp);

                                    this.OwnerElement.TransformPath(g, path, sp);
                                    Paint.SVGPaint svgstroke = (Paint.SVGPaint)sp.StrokeStyle.svgStroke;
                                    if (!svgstroke.IsEmpty)
                                        this.StrokePath(g, svgstroke, path, textelement, sp);
                                }
                                else
                                {
                                    this.OwnerElement.TransformPath(g, path, sp);
                                    g.DrawPath(sp.outlinePen, path);
                                }

                                this.textCache.Caches.Add(cache);
                            }
                        }
                        family.Dispose();
                        font.Dispose();
                        sf.Dispose();
                        sf = null;
                        font = null;
                        family = null;
                        shiftby = null;
                        list = null;
                        #endregion
                    }
                    g.EndContainer(c);
                    foreach (SVGStyleable sc1 in containers)
                        sp.EndContainer(sc1);
                }
            }
            catch (System.Exception e1)
            {
                this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e1.Message, e1.StackTrace }, ExceptionLevel.Normal));
            }
            finally
            {
                if (textelement is Text.SVGTextElement)
                    this.AddToRenderElements(sp);
                this.OwnerElement.CurrentTime = this.OwnerDocument.CurrentTime;
            }
        }
        #endregion

        #region ..DrawWithCache
        public override void DrawWithCache(Graphics g, SVG.StyleContainer.StyleOperator sp)
        {
            SmoothingMode mode = g.SmoothingMode;
            TextRenderingHint hint = g.TextRenderingHint;
            GraphicsContainer c = g.BeginContainer();
            g.SmoothingMode = mode;
            g.TextRenderingHint = hint;
            SVGTextElement textElement = this.OwnerElement as SVGTextElement;
            try
            {
                if (this.StyleContainer.VisualMediaStyle.visiblility != "hidden" && StyleContainer.VisualMediaStyle.display != "none")
                {
                    //if(this.CreateTransform)
                    this.OwnerElement.TransformPath(g, null, sp);
                    this.OwnerElement.Clip(g, sp);
                    foreach (Cache.TextItemCache temp in this.textCache.Caches)
                    {
                        if (temp.CachePath == null)
                            continue;
                        using (GraphicsPath gp1 = temp.CachePath.Clone() as GraphicsPath)
                        {
                            if (!sp.BoundView)
                            {
                                GraphicsContainer c1 = g.BeginContainer();
                                g.SmoothingMode = mode;
                                if (!this.OwnerDocument.ScaleStroke)
                                {
                                    g.Transform = this.OwnerElement.TotalTransform.Clone();
                                    g.MultiplyTransform(sp.coordTransform, MatrixOrder.Append);
                                }
                                this.DrawShadowWithCache(sp, g, gp1);
                                if (temp.CacheBrush != null)
                                    g.FillPath(temp.CacheBrush, gp1);
                                g.EndContainer(c1);

                                this.OwnerElement.TransformPath(g, gp1, sp);
                                if (temp.CachePen != null)
                                    g.DrawPath(temp.CachePen, gp1);
                            }
                            else
                            {
                                this.OwnerElement.TransformPath(g, gp1, sp);
                                g.DrawPath(sp.outlinePen, gp1);
                            }
                            this.AddToRenderElements(sp);
                        }
                    }
                }

            }
            catch (System.Exception e1)
            {
                this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e1.Message }, ExceptionLevel.Normal));
            }
            finally
            {
                this.OwnerElement.CurrentTime = this.OwnerDocument.CurrentTime;
                g.EndContainer(c);
            }
        }
        #endregion

        #region ..GetFontStyle
        /// <summary>
        /// 判断字形信息
        /// </summary>
        /// <param name="sp">当前类型容器</param>
        public System.Drawing.FontStyle GetFontStyle(StyleContainer.StyleOperator sp)
        {
            System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
            string fontWeight = "normal";
            if (!sp.FontStyle.FontWeigth.IsEmpty)
                fontWeight = sp.FontStyle.FontWeigth.Value;
            if (string.Compare(fontWeight,"bold" ) == 0
                || string.Compare(fontWeight,"bolder" ) == 0
                || string.Compare(fontWeight ,"600" ) == 0
                || string.Compare(fontWeight,"700") == 0
                || string.Compare(fontWeight,"800") == 0
                || string.Compare(fontWeight,"900") == 0)
            {
                style = style | System.Drawing.FontStyle.Bold;
            }

            string font_style = "normal";
            if (!sp.FontStyle.Font_Style.IsEmpty)
                font_style = sp.FontStyle.Font_Style.Value;
            if (string.Compare(font_style,"italic")==0)
            {
                style = style | System.Drawing.FontStyle.Italic;
            }

            string textDeco = string.Empty;
            if (!sp.TextStyle.text_decoration.IsEmpty)
                textDeco = sp.TextStyle.text_decoration.Value.Trim();
            if (string.Compare(textDeco,"line-through")==0)
            {
                style = style | System.Drawing.FontStyle.Strikeout;
            }
            else if (string.Compare(textDeco,"underline")==0)
            {
                style = style | System.Drawing.FontStyle.Underline;
            }
            fontWeight = null;
            textDeco = null;
            font_style = null;
            return style;
        }
        #endregion

        #region ..GetFontFamily
        /// <summary>
        /// 判断字样
        /// </summary>
        /// <param name="sp">当前类型容器</param>
        public System.Drawing.FontFamily GetFontFamily(StyleContainer.StyleOperator sp)
        {
            string fontFamily = "Arial";
            if (!sp.FontStyle.FontFamily.IsEmpty)
                fontFamily = sp.FontStyle.FontFamily.Value;
            string[] fontNames;
            if (fontFamily.Length == 0) fontNames = new string[1] { "Arial" };
            else
            {
                fontNames = fontFamily.Split(new char[1] { ',' });
            }

            System.Drawing.FontFamily family;

            foreach (string fn in fontNames)
            {
                try
                {
                    string fontName = fn.Trim(new char[2] { ' ', '\'' });
                    if (string.Compare(fontName,"serif") == 0)
                        family = FontFamily.GenericSerif;
                    else if (string.Compare(fontName,"sans-serif")==0)
                        family = FontFamily.GenericSansSerif;
                    else if (string.Compare(fontName,"monospace") ==0)
                        family = FontFamily.GenericMonospace;
                    else
                        family = new FontFamily(fontName);
                    fontName = null;
                    fontFamily = null;
                    fontNames = null;
                    return family;
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.Assert(true, e.Message);
                }
            }
            fontFamily = null;
            fontNames = null;
            return new FontFamily("Arial");
        }
        #endregion

        #region ..GetStringFormat
        public System.Drawing.StringFormat GetStringFormat(StyleContainer.StyleOperator sp)
        {
            return GetStringFormat(this.OwnerElement, sp);
        }

        /// <summary>
        /// 判断文本布局方式
        /// </summary>
        /// <param name="sp">当前类型容器</param>
        public static System.Drawing.StringFormat GetStringFormat(SVGTransformableElement render, StyleContainer.StyleOperator sp)
        {
            StringFormat sf = new StringFormat(StringFormat.GenericTypographic);
            sf.LineAlignment = StringAlignment.Near;
            bool doAlign = true;
            if (render is Interface.Text.ISVGTSpanElement || render is Interface.Text.ISVGTRefElement)
            {
                Interface.Text.ISVGTextPositioningElement posElement = (Interface.Text.ISVGTextPositioningElement)render;
                if (posElement.X.NumberOfItems == 0)
                    doAlign = false;
            }

            if (doAlign)
            {
                string anchor = string.Empty;
                if (!sp.TextStyle.text_anchor.IsEmpty)
                    anchor = sp.TextStyle.text_anchor.Value;
                if (string.Compare(anchor,"middle")==0)
                    sf.Alignment = StringAlignment.Center;
                else if (string.Compare(anchor ,"end") ==0)
                    sf.Alignment = StringAlignment.Far;
                else if (string.Compare(anchor ,"start")==0)
                    sf.Alignment = StringAlignment.Near;
                anchor = null;
            }

            //			string dir = string.Empty;
            //			if(sp.TextStyle.direction != null)
            //				dir = sp.TextStyle.direction.Value.Trim();
            //			if(string.Compare(dir,"rtl")==0)
            //			{
            //				if(sf.Alignment == StringAlignment.Far)
            //					sf.Alignment = StringAlignment.Near;
            //				else if(sf.Alignment == StringAlignment.Near)
            //					sf.Alignment = StringAlignment.Far;
            //				sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            //			}

            string dir = string.Empty;
            if (!sp.TextStyle.writing_mode.IsEmpty)
                dir = sp.TextStyle.writing_mode.Value.Trim();
            if (string.Compare(dir,"tb" ) == 0|| string.Compare(dir,"tb-rl")==0)
            {
                sf.FormatFlags = sf.FormatFlags | StringFormatFlags.DirectionVertical;
                //				sf.LineAlignment = StringAlignment.Center;
            }
            else if (string.Compare(dir,"rl" ) == 0|| string.Compare(dir , "rl-tb")==0)
            {
                sf.FormatFlags = sf.FormatFlags | StringFormatFlags.DirectionRightToLeft;
            }

            dir = null;
            sf.FormatFlags = sf.FormatFlags | StringFormatFlags.MeasureTrailingSpaces;

            return sf;
        }
        #endregion

        #region ..GetFontSize
        /// <summary>
        /// 计算字体尺寸
        /// </summary>
        /// <param name="fontsizestr">代表尺寸的字符串</param>
        /// <returns></returns>
        public float GetFontSize(string fontsizestr, Interface.ISVGElement element)
        {
            string str = fontsizestr;
            float fontSize = 12;
            if (str.EndsWith("%"))
            {
                // percentage of inherited value
            }
            else if (textRegex.IsMatch(str))
            {
                // svg length
                fontSize = new DataType.SVGLength(str, element, LengthDirection.Viewport).Value;
            }
            else if (string.Compare(str,"larger")==0)
            {
            }
            else if (string.Compare(str,"smaller")==0)
            {

            }
            else
            {
                fontSize = new DataType.SVGNumber(str, "12").Value;
            }
            str = null;
            return fontSize;
        }
        /// <summary>
        /// 判断字体尺寸
        /// </summary>
        /// <param name="sp">当前类型容器</param>
        /// <returns></returns>
        public float GetFontSize(StyleContainer.StyleOperator sp, Interface.ISVGElement element)
        {
            string str = string.Empty;
            if (!sp.FontStyle.FontSize.IsEmpty)
            {
                string str1 = ((DataType.SVGLength)sp.FontStyle.FontSize).OriValueStr;
                if (string.Compare(str1,"larger")==0)
                {
                    return 45;
                }
                else if (string.Compare(str1,"smaller")==0)
                    return 8;
                else
                    return sp.FontStyle.FontSize.Value;
            }
            str = null;
            return 12;
        }
        #endregion

        #region ..GetTextContentInfoOfChar
        /// <summary>
        /// 获取指定索引处字符所在TextContentInfo
        /// </summary>
        public TextContentInfo GetTextContentInfoOfChar(ref int charnum)
        {
            TextContentInfo posinfo = null;
            int i = 0;
            int index = 0;
            foreach (TextContentInfo info in this.textContentInfos)
            {

                if (i + info.TextContent.Length > charnum || index == this.textContentInfos.Count - 1)
                {
                    charnum -= i;
                    posinfo = info;
                    break;
                }
                i += info.TextContent.Length;
                index++;
            }
            return posinfo;
        }
        #endregion

        #region ..RefreshPath
        /// <summary>
        /// 刷新文本路径
        /// </summary>
        /// <param name="g"></param>
        public void RefreshPath(Graphics g, GraphicsPath gp)
        {
            gp.Reset();
            using (StyleContainer.StyleOperator sp = new StyleContainer.StyleOperator(this.StyleContainer))
            {
                System.Collections.ArrayList containers = new System.Collections.ArrayList();
                YP.SVG.Interface.ISVGElement preelement = null;
                sp.BeginStyleContainer(this.OwnerElement);
                SVG.SVGElementCollection list = new SVG.SVGElementCollection();
                Text.SVGTextPositionElement textelement = (Text.SVGTextPositionElement)this.OwnerElement;
                containers.Add(textelement);
                preelement = textelement;
                PointF refp = PointF.Empty;
                string shiftby = string.Empty;
                if (!sp.TextStyle.baseline_shift.IsEmpty)
                    shiftby = sp.TextStyle.baseline_shift.Value.Trim();

                float size = this.GetFontSize(sp, textelement);
                System.Drawing.FontStyle style = this.GetFontStyle(sp);
                FontFamily family = this.GetFontFamily(sp);
                StringFormat sf = this.GetStringFormat(sp);
                Font font = new Font(family, size, style);
                float shift = SVGTextContentElement.GetReferencedShiftBy(size, shiftby);
                if (!list.Contains(textelement))
                    list.Add(textelement);

                YP.SVG.DataType.SVGLengthList x = null, y = null, dx = null, dy = null;
                int xindex = 0, yindex = 0, dxindex = 0, dyindex = 0;

                x = (YP.SVG.DataType.SVGLengthList)textelement.X;
                y = (YP.SVG.DataType.SVGLengthList)textelement.Y;
                dx = (YP.SVG.DataType.SVGLengthList)textelement.Dx;
                dy = (YP.SVG.DataType.SVGLengthList)textelement.Dy;

                foreach (YP.SVG.Text.TextContentInfo info in textelement.TextContentInfos)
                {
                    YP.SVG.Text.SVGTextContentElement text = info.OwnerTextContentElement;
                    if (text != preelement)
                    {
                        if (text.ParentNode != preelement)
                        {
                            if (containers.Contains(preelement))
                                containers.Remove(preelement);
                        }
                        if (!containers.Contains(text))
                            containers.Add(text);
                        preelement = text;
                        size = this.GetFontSize(sp, textelement);
                        style = this.GetFontStyle(sp);
                        family = this.GetFontFamily(sp);
                        sf = this.GetStringFormat(sp);
                        font = new Font(family, size, style);
                        if (!sp.TextStyle.baseline_shift.IsEmpty)
                        {
                            shiftby = sp.TextStyle.baseline_shift.Value.Trim();
                            if (text.GetAttribute("baseline-shift").Trim().Length > 0 && !list.Contains(text))
                                shift += SVGTextContentElement.GetReferencedShiftBy(size, shiftby);
                            else
                                shift = SVGTextContentElement.GetReferencedShiftBy(size, shiftby);
                        }
                    }
                    if (!list.Contains(text))
                    {
                        if (text is YP.SVG.Text.SVGTextPositionElement)
                        {
                            YP.SVG.Text.SVGTextPositionElement postext = (YP.SVG.Text.SVGTextPositionElement)text;
                            if (postext.X.NumberOfItems > 0)
                            {
                                xindex = 0;
                                x = (YP.SVG.DataType.SVGLengthList)postext.X;
                            }
                            if (postext.Y.NumberOfItems > 0)
                            {
                                yindex = 0;
                                y = (YP.SVG.DataType.SVGLengthList)postext.Y;
                            }
                            if (postext.Dx.NumberOfItems > 0)
                            {
                                dxindex = 0;
                                dx = (YP.SVG.DataType.SVGLengthList)postext.Dx;
                            }
                            if (postext.Dy.NumberOfItems > 0)
                            {
                                dyindex = 0;
                                dy = (YP.SVG.DataType.SVGLengthList)postext.Dy;
                            }
                        }
                        list.Add(text);
                    }
                    info.TextFont = font;
                    info.TextStringFormat = (StringFormat)sf.Clone();
                    info.X = x;
                    info.Y = y;
                    info.Dx = dx;
                    info.Dy = dy;
                    using (GraphicsPath path = info.GetGraphicsPath(g, shift, ref refp, ref xindex, ref yindex, ref dxindex, ref dyindex, sp))
                    {
                        if (path.PointCount == 0)
                            continue;
                        shiftby = string.Empty;
                        gp.AddPath(path, true);
                    }
                }
            }
        }
        #endregion

        #region ..override ExportLabelElement
        public override SVGTextElement ExportLabelElement(Graphics g)
        {
            return null;
        }
        #endregion
    }
}
