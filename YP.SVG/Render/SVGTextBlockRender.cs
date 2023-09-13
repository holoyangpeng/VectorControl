using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Collections;

namespace YP.SVG.Render
{
    public class SVGTextBlockRender : SVGDirectRenderer
    {
        #region ..Constructor
        public SVGTextBlockRender(Text.SVGTextBlockElement ownerElement)
            : base(ownerElement)
        {

        }
        #endregion

        #region ..static fields
        static float padding = 5;
        
        #endregion

        #region ..private fields
        Brush textBrush = null;
        string preText = string.Empty;
        Hashtable charBounds = new Hashtable();
        Font textFont = new Font("Microsoft YaHei",13);
        StringFormat textFormat = new StringFormat(StringFormat.GenericTypographic);
        RectangleF elementBounds = RectangleF.Empty;
        #endregion

        #region ..properties
        StyleContainer.StyleContainer originalStyle = null;
        public override bool DrawLabelBounds
        {
            get
            {
                return base.DrawLabelBounds;
            }
            set
            {
                if (base.DrawLabelBounds != value)
                {
                    base.DrawLabelBounds = value;

                    this.OwnerDocument.RefreshOriginalElement(this.OwnerElement);
                    if (value)
                    {
                        //overflow是剪切时，做一个Feak动作，将overflow改为visible，编辑状态全部显示文本
                        if (this.OwnerElement.StyleContainer.VisualMediaStyle.overflow == "hidden")
                        {
                            originalStyle = new SVG.StyleContainer.StyleContainer(this.OwnerElement.StyleContainer);
                            SVG.StyleContainer.StyleContainer st = new SVG.StyleContainer.StyleContainer(this.OwnerElement.StyleContainer);
                            SVG.StyleContainer.VisualMediaStyle vm = new StyleContainer.VisualMediaStyle(st.VisualMediaStyle);
                            vm.overflow = new DataType.SVGString("visible");
                            st.VisualMediaStyle = vm;
                            this.OwnerElement.StyleContainer = st;
                            this.OwnerElement.UpdatePath(false);
                            this.OwnerDocument.RefreshElement(this.OwnerElement, true);
                        }
                    }
                    else if (originalStyle != null)
                    {
                        {
                            this.OwnerElement.StyleContainer = originalStyle;
                            this.OwnerElement.UpdatePath(false);
                        }
                        this.OwnerDocument.RefreshElement(this.OwnerElement,true);
                    }
                }
            }
        }

        public override string LabelText
        {
            set
            {
                this.OwnerElement.InnerText = value;
            }
            get
            {
                return this.OwnerElement.InnerText;
            }
        }
        #endregion

        #region ..Draw
        public override void Draw(System.Drawing.Graphics g, StyleContainer.StyleOperator sp)
        {
            if (this.OwnerDocument.IsStopRender)
                return;
            Text.SVGTextBlockElement pathElement = this.OwnerElement as Text.SVGTextBlockElement;
            sp.BeginStyleContainer(pathElement);
            GraphicsPath gp1 = pathElement.GPath;
            SmoothingMode mode = g.SmoothingMode;
            InterpolationMode interpolationMode = g.InterpolationMode;
            System.Drawing.Text.TextRenderingHint hint = g.TextRenderingHint;
            GraphicsContainer c = g.BeginContainer();
            g.SmoothingMode = mode;
            g.TextRenderingHint = hint;
            g.InterpolationMode = interpolationMode;
           
                if (!BeforeDrawing(g,sp))
                {
                    return;
                }
                if (this.OwnerDocument.DrawElementWithCache(this.OwnerElement))
                {
                    this.DrawWithCache(g, sp);
                    return;
                }
                try
                {
                this.OwnerElement.NeedUpdateCSSStyle = false;
                
                if (sp.ViewVisible && this.StyleContainer.VisualMediaStyle.visiblility != "hidden" && this.StyleContainer.VisualMediaStyle.display != "none")
                {
                    this.OwnerElement.Clip(g, sp);
                    using (GraphicsPath path = gp1.Clone() as GraphicsPath)
                    {
                        this.DrawShadow(sp, g, path);
                        if (!sp.BoundView)
                        {
                            Paint.SVGPaint svgpaint = (Paint.SVGPaint)sp.FillStyle.svgPaint;
                            if (!svgpaint.IsEmpty && path.PointCount > 2)
                                this.FillPath(g, svgpaint, path, this.OwnerElement, sp);

                            this.OwnerElement.TransformPath(g, path, sp);
                            Paint.SVGPaint svgstroke = (Paint.SVGPaint)sp.StrokeStyle.svgStroke;
                            if (!svgstroke.IsEmpty)
                                this.StrokePath(g, svgstroke, path, this.OwnerElement, sp);
                        }
                        else
                        {
                            this.OwnerElement.TransformPath(g, path, sp);
                            g.DrawPath(sp.outlinePen, path);
                        }
                    }
                    this.DrawContent(g, sp.coordTransform, false, sp);
                    this.charBounds.Clear();
                }
            }
            catch (System.Exception e1)
            {
                this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e1.Message }, ExceptionLevel.Normal));
            }
            finally
            {
                sp.EndContainer(this.OwnerElement);
                this.AddToRenderElements(sp);
                this.OwnerElement.CurrentTime = this.OwnerDocument.CurrentTime;
                g.EndContainer(c);
            }
        }
        #endregion

        #region ..DrawContent
        public void DrawContent(Graphics g, Matrix coordTransform, bool useCache)
        {
            DrawContent(g, coordTransform, useCache, null);
        }
        public void DrawContent(Graphics g, Matrix coordTransform, bool useCache, StyleContainer.StyleOperator sp)
        {
            System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
            System.Drawing.Text.TextRenderingHint hint = g.TextRenderingHint;
            InterpolationMode mode1 = g.InterpolationMode;
            GraphicsContainer c = g.BeginContainer();
            g.SmoothingMode = mode;
            g.InterpolationMode = mode1;
            g.TextRenderingHint = hint;
            using (Matrix matrix = this.GetLabelTransform())
            {
                if (sp == null)
                    sp = new SVG.StyleContainer.StyleOperator(this.StyleContainer);
                matrix.Multiply(coordTransform, MatrixOrder.Append);
                g.Transform = matrix;
                {
                    if (sp.BoundView)
                    {
                        g.DrawString(this.OwnerElement.InnerText, this.textFont, sp.outlinePen.Brush, this.LabelBounds, this.textFormat);
                    }
                    else
                    {
                        if (useCache && textBrush != null)
                        {
                            if (this.DrawLabelBounds)
                                g.DrawString(this.OwnerElement.InnerText, this.textFont, Brushes.Black, this.LabelBounds, textFormat);
                            else
                                g.DrawString(this.OwnerElement.InnerText, this.textFont, textBrush, this.LabelBounds, textFormat);
                        }
                        else
                        {
                            textBrush = this.GetBrush(this.StyleContainer.TextStyle.text_color, this.LabelBounds, sp, this.OwnerElement, 1);
                            {
                                if (this.DrawLabelBounds)
                                    g.DrawString(this.OwnerElement.InnerText, this.textFont, Brushes.Black, this.LabelBounds, textFormat);
                                else
                                    g.DrawString(this.OwnerElement.InnerText, this.textFont, textBrush, this.LabelBounds, textFormat);
                            }
                        }
                    }
                }
            }
            g.EndContainer(c);
        }
        #endregion

        #region ..GetPath
        public void GetPath(Text.SVGTextBlockElement textBlock, GraphicsPath path)
        {
            GetPath(textBlock, path, true);
        }

        public void GetPath(Text.SVGTextBlockElement textBlock, GraphicsPath path, bool needInitial)
        {
            if (textBlock == null)
                return;
            string text = textBlock.InnerText;
            
            if(needInitial)
                this.CalculateGDIInfo(textBlock);
            RectangleF textBounds = this.LabelBounds;
            using (System.Windows.Forms.Label tempGraphicsOwner = new System.Windows.Forms.Label())
            {
                using (Graphics g = tempGraphicsOwner.CreateGraphics())
                {
                    using (Matrix matrix = this.GetLabelTransform())
                    {
                        SizeF size = g.MeasureString(text, this.textFont, textBounds.Size, textFormat);
                        size.Width = size.Width < textBounds.Width ? textBounds.Width : size.Width;
                        float y1 = 0;
                        switch (textFormat.LineAlignment)
                        {
                            case StringAlignment.Center:
                                y1 = textBounds.Y + textBounds.Height / 2 - size.Height / 2;
                                break;
                            case StringAlignment.Near:
                                y1 = textBounds.Y;
                                break;
                            case StringAlignment.Far:
                                y1 = textBounds.Y + textBounds.Height - size.Height;
                                break;
                        }

                        float x1 = 0;
                        switch (textFormat.Alignment)
                        {
                            case StringAlignment.Center:
                                x1 = textBounds.X + textBounds.Width / 2 - size.Width / 2;
                                break;
                            case StringAlignment.Near:
                                x1 = textBounds.X;
                                break;
                            case StringAlignment.Far:
                                x1 = textBounds.X + textBounds.Width - size.Width;
                                break;
                        }
                        float x = textBounds.X < x1 ? textBounds.X : x1;
                        float y = textBounds.Y < y1 ? textBounds.Y : y1;
                        float right = textBounds.Right > x1 + size.Width ? textBounds.Right : x1 + size.Width;
                        float bottom = textBounds.Bottom > y1 + size.Height ? textBounds.Bottom : y1 + size.Height;
                        path.AddRectangle(new RectangleF(x, y, right - x, bottom - y));
                        path.Transform(matrix);

                        matrix.Reset();
                        matrix.Multiply(this.OwnerElement.TotalMatrix);
                        //if (this.OwnerElement.ParentElement is DocumentStructure.SVGGElement)
                        //    matrix.Multiply((this.OwnerElement.ParentElement as SVGTransformableElement).GDITransform);
                        if (!matrix.IsIdentity)
                        {
                            //using (Matrix temp = this.OwnerElement.GDITransform.Clone())
                            //{
                            matrix.Invert();
                            path.Transform(matrix);
                            //}
                        }
                    }
                }
            }
        }
        #endregion

        #region ..GetPointAtIndex
        public override PointF GetPointAtIndex(int offset, Graphics g, ref PointF endPoint)
        {
            RectangleF rect = GetBoundsForOffset(offset, g);
            if (!rect.IsEmpty)
            {
                string text = this.OwnerElement.Label;
                int offset1 = offset;
                offset = offset >= text.Length ? text.Length - 1 : offset;
                offset = offset < 0 ? 0 : offset;
                var end = (offset1 >= text.Length && textFormat.Alignment != StringAlignment.Far && text.Length > 0)
                   || (offset1 == text.Length - 1 && textFormat.Alignment == StringAlignment.Far)
                   || (offset < text.Length && text[offset] == '\n');
                PointF[] ps = { rect.Location, new PointF(rect.Left, rect.Y + rect.Height) };
                var vert = ((this.textFormat.FormatFlags & StringFormatFlags.DirectionVertical) == StringFormatFlags.DirectionVertical);
                if(vert)
                    ps = new PointF[]{rect.Location, new PointF(rect.Right, rect.Y)};
                if (end)
                {
                    ps = new PointF[] { new PointF(rect.Right, rect.Y), new PointF(rect.Right, rect.Y + rect.Height) };
                    if (vert)
                        ps = new PointF[] { new PointF(rect.X, rect.Bottom), new PointF(rect.Right, rect.Bottom) };
                }
                using (Matrix matrix = this.GetLabelTransform())
                {
                    matrix.TransformPoints(ps);
                    endPoint = ps[1];
                    return ps[0];
                }
            }

            return PointF.Empty;
        }
        #endregion

        #region ..CalculateLengthAtOffset
        public override float CalculateLengthAtOffset(int offset, Graphics g)
        {
             RectangleF rect = GetBoundsForOffset(offset, g);
             if (!rect.IsEmpty)
             {
                 return rect.X + rect.Width / 2f;
             }
            return float.MaxValue;
        }
        #endregion

        #region ..MoveVertical
        public override int MoveVertical(bool up, Graphics g, int offset, float originalPos)
        {
            RectangleF bounds = this.GetBoundsForOffset(offset, g);
            if (!bounds.IsEmpty)
            {
                PointF p = new PointF(originalPos, bounds.Y + bounds.Height / 2);
                if (up)
                    p.Y -= bounds.Height;
                else
                    p.Y += bounds.Height;

                return this.FindOffsetAtPoint(p, g);
            }
            return offset;
        }
        #endregion

        #region ..GetRegion
        public override RectangleF[] GetRegion(int offset, int length, Graphics g)
        {
            if (length == 0)
                return null;

            try
            {
                Text.SVGTextBlockElement pathElement = this.OwnerElement as Text.SVGTextBlockElement;
                if (pathElement != null)
                {
                    string text = pathElement.Label;
                    int offset1 = offset;
                    offset = offset >= text.Length ? text.Length - 1 : offset;
                    offset = offset < 0 ? 0 : offset;
                    //处理换行
                    while (offset > 0 && text[offset] == '\n')
                    {
                        offset--;
                        length++;
                    }
                    while (length > 2 && offset+length-1 < text.Length && text[offset+length - 1] == '\n')
                        length--;
                    List<CharacterRange> ranges = new List<CharacterRange>();
                    int count = offset + length;
                    count = count > text.Length ? text.Length : count;
                    count = count - offset;
                    ranges.Add(new CharacterRange(offset, 1));
                    if (count > 1)
                    {
                        if (offset + count - 1 - offset > 1)
                            ranges.Add(new CharacterRange(offset + 1, offset + count - 1 - offset - 1));
                        ranges.Add(new CharacterRange(offset + count - 1, 1));
                    }
                    this.textFormat.SetMeasurableCharacterRanges(ranges.ToArray());
                    Region[] regions = g.MeasureCharacterRanges(text, this.textFont, this.LabelBounds, this.textFormat);

                    bool vert = (this.textFormat.FormatFlags & StringFormatFlags.DirectionVertical) == StringFormatFlags.DirectionVertical;
                    List<RectangleF> rects = new List<RectangleF>();
                    if (regions != null && regions.Length > 0)
                    {
                        Region rg = regions[0];
                        RectangleF rect = rg.GetBounds(g);
                        //rects.Add(rect);
                        float firstLeft = rect.Left;
                        float firstTop = rect.Top;
                        float firstBottom = rect.Bottom;
                        float firstRight = rect.Right;
                        float lastTop = rect.Top;
                        float lastLeft = rect.Left;
                        float lastRight = rect.Right;
                        float lastBottom = rect.Bottom;

                        if (regions.Length >= 2)
                        {
                            rg = regions.Last();
                            RectangleF rect1 = rg.GetBounds(g);
                            lastRight = rect1.Right;
                            lastLeft = rect1.Left;
                            lastTop = rect1.Top;
                            lastBottom = rect1.Bottom;
                        }
                        float left = firstLeft;
                        float right = lastRight;
                        float top = firstTop;
                        float bottom = lastBottom;
                        if (regions.Length == 3)
                        {
                            rg = regions[1];
                            rect = rg.GetBounds(g);
                            left = rect.Left;
                            right = rect.Right;
                            top = rect.Top;
                            bottom = rect.Bottom;
                        }

                        rect = new RectangleF(firstLeft, firstTop, right - firstLeft, firstBottom - firstTop);
                        if (vert)
                            rect = new RectangleF(firstLeft, firstTop, firstRight - firstLeft, bottom - firstTop);
                        if (!rect.IsEmpty)
                            rects.Add(rect);
                        rect = new RectangleF(left, firstBottom, right - left, lastTop - firstBottom);
                        if (vert)
                            rect = new RectangleF(firstRight, top, lastLeft - firstRight, bottom - top);
                        if (!vert && rect.Height < 0)
                            rect = new RectangleF(left, lastTop, right - left, firstBottom - lastTop);
                        else if(vert && rect.Width < 0)
                                rect = new RectangleF(lastLeft, top, firstRight - lastLeft, bottom - top);
                        if (!rect.IsEmpty)
                            rects.Add(rect);
                        rect = new RectangleF(left, lastTop, lastRight - left, lastBottom - lastTop);
                        if (vert)
                            rect = new RectangleF(lastLeft, top, lastRight - lastLeft, lastBottom - top);
                        if (!rect.IsEmpty)
                            rects.Add(rect);
                    }
                    return rects.ToArray();

                }
            }
            catch (System.Exception e1)
            {
                this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e1.Message, e1.StackTrace }, ExceptionLevel.Normal));
            }
            return null;
        }
        #endregion

        #region ..InsertStr
        public override void InsertStr(int offset, string str)
        {
            if (str.Length == 0)
                return;

            string text = this.LabelText;
            if (offset >= 0 && offset < text.Length)
                text= text.Insert(offset, str);
            else
                text = text + str;

            this.OwnerElement.InnerText = text;
        }
        #endregion

        #region ..RemoveString
        public override void RemoveString(int offset, int length)
        {
            if (length == 0)
                return;

            string text = this.LabelText;
            if (offset >= 0 && offset < text.Length)
                text = text.Remove(offset, length);
           
            this.OwnerElement.InnerText = text;
        }
        #endregion

        #region ..FindOffsetAtPoint
        public override int FindOffsetAtPoint(PointF p, Graphics g)
        {
            using (Matrix matrix = this.GetLabelTransform())
            {
                matrix.Invert();
                PointF[] ps = { p };
                matrix.TransformPoints(ps);
                p = ps[0];
            }
            Text.SVGTextBlockElement pathElement = this.OwnerElement as Text.SVGTextBlockElement;
            if (pathElement != null)
            {
                string text = pathElement.Label;

                int start = 0;
                int end = start + text.Length - 1;

                #region ..折半
                //while (start < end)
                //{
                //    CharacterRange[] ranges;
                //    Region[] regions;
                //    RectangleF startBounds = RectangleF.Empty;
                //    RectangleF endBounds = RectangleF.Empty;
                //    if (end == start + 1)
                //    {
                //        ranges = new CharacterRange[]{new CharacterRange(start,1), new CharacterRange(end,1)};
                //        regions = g.MeasureCharacterRanges(text, this.textFont, this.LabelBounds, this.textFormat);
                //        if (regions != null && regions.Length == 2)
                //        {
                //            startBounds = regions[0].GetBounds(g);
                //            endBounds = regions[1].GetBounds(g);
                //            if (startBounds.Contains(p))
                //            {
                //                if (Math.Abs(p.X - startBounds.X) > Math.Abs(startBounds.Right - p.X))
                //                    return end;
                //                return start;
                //            }
                //            if (Math.Abs(p.X - endBounds.X) > Math.Abs(endBounds.Right - p.X))
                //                return end + 1;
                //            return end;
                //        }
                //    }
                //    if (end == start)
                //        return end;
                //    int middle = (start + end) / 2;
                //    ranges = new CharacterRange[]{ new CharacterRange(start, middle - start + 1), new CharacterRange(middle, end - middle + 1) };
                //    this.textFormat.SetMeasurableCharacterRanges(ranges);
                //    regions = g.MeasureCharacterRanges(text, this.textFont, this.LabelBounds, this.textFormat);
                //    startBounds = regions[0].GetBounds(g);
                //    endBounds = regions[1].GetBounds(g);
                //    if (regions[0].IsVisible(p))
                //        end = middle;
                //    else
                //        start = middle;
                //}
                #endregion

                bool vert = (this.textFormat.FormatFlags & StringFormatFlags.DirectionVertical) == StringFormatFlags.DirectionVertical;
                RectangleF preBounds = RectangleF.Empty;
                for (int i = 0; i < text.Length; i++)
                {
                    
                    RectangleF bounds = this.GetBoundsForOffset(i, g);
                    //处理换行
                    if ((!vert && p.X <= bounds.X && p.Y <= bounds.Bottom) || (vert && p.Y <= bounds.Y && p.X <= bounds.Right))
                        return i;
                    if (!preBounds.IsEmpty 
                        && ((!vert && p.X >= bounds.Right && p.Y <= bounds.Bottom 
                        && bounds.Y > (preBounds.Y + preBounds.Height /2) 
                        && p.Y >= preBounds.Top && p.Y <= preBounds.Bottom)
                        || (vert
                        && p.Y >= bounds.Bottom && p.X <= bounds.Right
                        && bounds.X > (preBounds.X + preBounds.Width / 2)
                        && p.X >= preBounds.X && p.X <= preBounds.Right
                        )))
                        return i;
                    if (bounds.Contains(p))
                    {
                        if ((!vert && Math.Abs(bounds.Left - p.X) < Math.Abs(bounds.Right - p.X)) || (vert && Math.Abs(bounds.Top - p.Y) < Math.Abs(bounds.Bottom - p.Y)))
                        {
                            if(i < text.Length -1 || (i == text.Length - 1 && textFormat.Alignment != StringAlignment.Far))
                                return i;
                        }
                        return i + 1;
                    }
                    if(!bounds.IsEmpty)
                        preBounds = bounds;
                }

                return text.Length;
            }
            return -1;
        }
        #endregion

        #region ..DrawWithCache
        public override void DrawWithCache(Graphics g, StyleContainer.StyleOperator sp)
        {
            System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
            System.Drawing.Text.TextRenderingHint hint = g.TextRenderingHint;
            InterpolationMode mode1 = g.InterpolationMode;
            GraphicsContainer c = g.BeginContainer();
            g.SmoothingMode = mode;
            g.InterpolationMode = mode1;
            g.TextRenderingHint = hint;
            SVG.Text.SVGTextBlockElement textBlock = this.OwnerElement as Text.SVGTextBlockElement;

            this.OwnerElement.Clip(g, sp);

            using (GraphicsPath path = textBlock.GPath.Clone() as GraphicsPath)
            {
                
                if (!sp.BoundView)
                {
                    
                    if (this.Cache.CacheBrush != null)
                    {
                        GraphicsContainer c1 = g.BeginContainer();
                        g.SmoothingMode = mode;
                        if (!this.OwnerDocument.ScaleStroke)
                        {
                            g.Transform = this.OwnerElement.TotalTransform.Clone();
                            g.MultiplyTransform(sp.coordTransform, MatrixOrder.Append);
                        }
                        this.DrawShadowWithCache(sp, g, path);
                        if (this.Cache.CacheBrush is PathGradientBrush)
                        {
                            ColorBlend cl = (this.Cache.CacheBrush as PathGradientBrush).InterpolationColors;
                            if (cl.Colors.Length > 0)
                            {
                                using (SolidBrush brush1 = new SolidBrush(cl.Colors[0]))
                                    g.FillPath(brush1, path);
                            }
                        }
                        g.FillPath(this.Cache.CacheBrush, path);
                        g.EndContainer(c1);
                    }
                    if (this.Cache.CachePen != null)
                    {
                        this.OwnerElement.TransformPath(g, path, sp);
                        g.DrawPath(this.Cache.CachePen, path);
                    }
                }
                else
                {
                    this.OwnerElement.TransformPath(g, path, sp);
                    g.DrawPath(sp.outlinePen, path);
                }
            }
            this.DrawContent(g, sp.coordTransform,true,sp);
            g.EndContainer(c);
        }
        #endregion

        #region ..GetBoundsForOffset
        RectangleF GetBoundsForOffset(int offset, Graphics g)
        {
            string text = this.OwnerElement.Label;
            if (text != preText)
                this.charBounds.Clear();
            preText = text;
            RectangleF rect = RectangleF.Empty;
            if (this.charBounds.ContainsKey(offset))
                rect = (RectangleF)this.charBounds[offset];
            else
            {
                //处理空字符串
                if (text.Length == 0)
                    text = "a";
                using (StringFormat sf = new StringFormat(this.textFormat))
                {
                    int offset1 = offset;
                    offset = offset >= text.Length ? text.Length - 1 : offset;
                    offset = offset < 0 ? 0 : offset;

                    //处理换行
                    int offset2 = offset;
                    while (offset2 > 0 && text[offset2] == '\n')
                        offset2--;
                    List<CharacterRange> ranges = new List<CharacterRange>();
                    if (offset2 > 0)
                        ranges.Add(new CharacterRange(0, offset2));
                    int count = ranges.Count;
                    ranges.Add(new CharacterRange(offset2, 1));
                    if (offset2 + 1 < text.Length)
                        ranges.Add(new CharacterRange(offset2 + 1, text.Length - offset2 - 1));
                    sf.SetMeasurableCharacterRanges(ranges.ToArray());
                    Region[] rg = g.MeasureCharacterRanges(text, this.textFont, this.LabelBounds, sf);
                    if (rg != null && rg.Length > count)
                    {
                        rect = rg[count].GetBounds(g);
                        this.charBounds[offset] = rect;
                    }
                }
            }
            return rect;
        }
        #endregion

        #region ..CalculateGDIInfo
        void CalculateGDIInfo(Text.SVGTextBlockElement textBlock)
        {
            charBounds.Clear();
            float rotateAngle = 0;
            PointF centerPoint = PointF.Empty;
            using (System.Windows.Forms.Label tempGraphicsOwner = new System.Windows.Forms.Label())
            {
                using (Graphics g = tempGraphicsOwner.CreateGraphics())
                {
                    using (SVG.StyleContainer.StyleOperator sp = new SVG.StyleContainer.StyleOperator(textBlock.StyleContainer))
                    {
                        if (!this.OwnerElement.IsActive)
                        {
                            if (this.OwnerElement.ParentElement is SVGStyleable)
                                sp.BeginStyleContainer(this.OwnerElement.ParentElement as SVGStyleable);
                        }
                        sp.BeginStyleContainer(this.OwnerElement);
                        using (Font font = this.GetFont(sp))
                        {
                            this.textFont = font.Clone() as Font;
                            this.ParseStringFormat(sp, textFormat);

                            string text = textBlock.InnerText;
                            float x = textBlock.X.Value;
                            float y = textBlock.Y.Value;
                            float width = textBlock.Width.Value;
                            float height = textBlock.Height.Value;

                            centerPoint = new PointF(x + width / 2, y + height / 2);
                            rotateAngle = 0;
                            RectangleF textBounds = RectangleF.Empty;
                            if (text != null)
                            {
                                SVGElement viewPortElement = textBlock.ViewPortElement as SVGElement;
                                Interface.ISVGTextBlockContainer viewPort = viewPortElement as Interface.ISVGTextBlockContainer;
                                textBounds = new RectangleF(x, y, width, height);
                                PointF[] anglePos = { PointF.Empty, PointF.Empty };
                                if (viewPort != null)
                                {
                                    //viewport对象是线条
                                    if (viewPort is Interface.ILineElement)
                                    {
                                        Interface.ILineElement lineElement = viewPort as Interface.ILineElement;
                                        float distance = padding, totalDistance = lineElement.Distance;
                                        if (textFormat.Alignment == StringAlignment.Center)
                                            distance = totalDistance / 2;
                                        else if (textFormat.Alignment == StringAlignment.Far)
                                            distance = totalDistance - padding;

                                        PointF[] ps = lineElement.GetAnchorsWithDistance(distance);

                                        if (ps != null && ps.Length >= 3)
                                        {
                                            PointF p0 = ps[0];
                                            PointF p1 = ps[1];
                                            PointF p2 = ps[2];
                                            anglePos = new PointF[] { p0, new PointF(p0.X + 2, p0.Y) };
                                            rotateAngle = PathHelper.GetAngle(p0, p1);
                                            centerPoint = p2;
                                            textBounds = new RectangleF(centerPoint.X, centerPoint.Y, 0, 0);

                                            //连接线，angle一直为0
                                            if (viewPort is SVG.BasicShapes.SVGBranchElement)
                                                rotateAngle = 0;
                                        }
                                    }
                                    else
                                    {
                                        DataType.SVGViewport vp = viewPort.Viewport;
                                        textBounds = new RectangleF(vp.Bounds.X + textBounds.X, vp.Bounds.Y + textBounds.Y, textBounds.Width, textBounds.Height);
                                        RectangleF rect1 = vp.Bounds.GDIRect;
                                        rotateAngle = vp.RotateAngle;
                                        anglePos = new PointF[] { textBounds.Location, new PointF(textBounds.Right, textBounds.Top) };
                                        centerPoint = new PointF(rect1.X + rect1.Width / 2, rect1.Y + rect1.Height / 2);
                                    }
                                }
                                //根据变形矫正绘制区域
                                Matrix matrix1 = this.OwnerElement.TotalMatrix;
                                if (!matrix1.IsIdentity)
                                {
                                    using (Matrix matrix = new Matrix())
                                    {
                                        matrix.RotateAt(rotateAngle, centerPoint);
                                        matrix.Multiply(matrix1, MatrixOrder.Append);
                                        PointF[] ps = new PointF[]{textBounds.Location, 
                                        new PointF(textBounds.Right, textBounds.Top),
                                        new PointF(textBounds.Right, textBounds.Bottom),
                                        new PointF(textBounds.Left, textBounds.Bottom),
                                        new PointF(textBounds.Left + textBounds.Width / 2, textBounds.Top + textBounds.Height/2)};
                                        matrix.TransformPoints(ps);
                                        matrix.TransformPoints(anglePos);
                                        float width1 = PathHelper.Distance(ps[0], ps[1]);
                                        float height1 = PathHelper.Distance(ps[1], ps[2]);
                                        centerPoint = ps[ps.Length - 1];
                                        if (anglePos[0] != anglePos[1])
                                            rotateAngle = PathHelper.GetAngle(anglePos[0], anglePos[1]);
                                        textBounds = new RectangleF(centerPoint.X - width1 / 2, centerPoint.Y - height1 / 2, width1, height1);
                                    }
                                }
                                this.LabelRotateAngle = rotateAngle;
                                this.LabelCenterPoint = centerPoint;
                                this.LabelBounds = textBounds;
                            }
                        }
                        sp.EndContainer(this.OwnerElement);
                        if (!this.OwnerElement.IsActive)
                        {
                            if (this.OwnerElement.ParentElement is SVGStyleable)
                                sp.EndContainer(this.OwnerElement.ParentElement as SVGStyleable);
                        }
                    }
                }
            }
        }
        #endregion

        #region ..AddToRenderElements
        public override void AddToRenderElements(StyleContainer.StyleOperator sp)
        {
            if(this.OwnerElement.ParentElement is DocumentStructure.SVGGElement)
                base.AddToRenderElements(sp);
        }
        #endregion

        #region ..ExportNativeSVG
        /// <summary>
        /// ExportNativeSVG
        /// </summary>
        /// <returns></returns>
        public void ExportNativeSVG(SVGElement groupElm)
        {
            //create
            if (this.OwnerElement.InnerText.Trim().Length > 0)
            {
                SVG.SVGElement path = this.OwnerDocument.CreateElement("path") as SVG.SVGElement;
                string str = SVG.Paths.SVGPathElement.GetPathString((this.OwnerElement as Text.SVGTextBlockElement).GPath);
                path.InternalSetAttribute("d", str);
                path.SetAttribute("transform", new DataType.SVGMatrix(this.OwnerElement.TotalTransform).ToString());
                groupElm.AppendChild(path);

                SVGElement textElement = this.OwnerDocument.CreateElement("text") as SVG.SVGElement;
                if (this.StyleContainer != null)
                {
                    textElement.SetAttribute("fill", SVG.ColorHelper.GetColorStringInHex(this.StyleContainer.TextStyle.text_color.RgbColor.GDIColor).ToLower());
                    textElement.SetAttribute("font-size", (this.StyleContainer.FontStyle.FontSize.Value * 4.0f / 3.0f).ToString());
                }
                textElement.SetAttribute("stroke", "none");
                string text = this.OwnerElement.InnerText.Trim();
                textElement = groupElm.AppendChild(textElement) as SVGElement;
                textElement.SetAttribute("transform", string.Format("rotate({0} {1} {2})", this.LabelRotateAngle, this.LabelCenterPoint.X, this.LabelCenterPoint.Y));
                RectangleF? rect = null;
                SVGElement preElm = textElement;
                StringBuilder sb = new StringBuilder();
                using (System.Windows.Forms.Label lb = new System.Windows.Forms.Label())
                {
                    using (Graphics g = lb.CreateGraphics())
                    {
                        float accent = this.textFont.FontFamily.GetCellDescent(this.textFont.Style);
                        float height = this.textFont.FontFamily.GetEmHeight(this.textFont.Style);
                        accent = accent / height * this.textFont.Size + 0.5f;
                        for (int i = 0; i < text.Length; i++)
                        {
                            RectangleF offset = this.GetBoundsForOffset(i, g);
                            //开始
                            if (!rect.HasValue)
                            {
                                textElement.SetAttribute("x", offset.X.ToString());
                                
                                textElement.SetAttribute("y", (offset.Bottom - accent).ToString());
                                rect = offset;
                            }
                            //换行
                            else if (Math.Abs(offset.Bottom - rect.Value.Bottom) > rect.Value.Height / 2)
                            {
                                preElm.InnerText = sb.ToString();
                                sb.Remove(0, sb.Length);
                                preElm = this.OwnerDocument.CreateElement("tspan") as SVGElement;
                                preElm = textElement.AppendChild(preElm) as SVGElement;
                                preElm.SetAttribute("y", (offset.Bottom - accent).ToString());
                                preElm.SetAttribute("x", (offset.X).ToString());
                                rect = offset;
                            }
                            sb.Append(text[i]);
                        }

                        if (preElm != null && sb.Length > 0)
                            preElm.InnerText = sb.ToString();
                    }
                }
            }
            
        }
        #endregion

        public override void CloneRender(SVGBaseRenderer render)
        {
            base.CloneRender(render);
            SVGTextBlockRender render1 = render as SVGTextBlockRender;
            if(render1 != null)
            {
                if(this.textBrush != null)
                render1.textBrush = this.textBrush.Clone() as System.Drawing.Brush;
                if(this.textFont != null)
                render1.textFont = this.textFont.Clone() as System.Drawing.Font;
                if(this.textFormat != null)
                render1.textFormat = new StringFormat(this.textFormat);
            }
            render.LabelRotateAngle = this.LabelRotateAngle;
            render.LabelCenterPoint = this.LabelCenterPoint;
            render.LabelBounds = this.LabelBounds;
        }
    }
}
