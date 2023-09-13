using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Render
{
    public class SVGDirectRenderer:SVGBaseRenderer, Interface.ISVGRenderer
    {
        #region ..static fields
        static Regex re = new Regex("[\\s\\,]+");
        #endregion

        #region ..Constructor
        public SVGDirectRenderer(SVGTransformableElement ownerElement):base(ownerElement)
        {
            
        }
        #endregion

        #region ..Draw
        public override void Draw(System.Drawing.Graphics g, StyleContainer.StyleOperator sp)
        {
            if (this.OwnerDocument.IsStopRender || !(this.OwnerElement is Interface.ISVGPathable))
                return;
            Interface.ISVGPathable pathElement = this.OwnerElement as Interface.ISVGPathable;
            GraphicsPath gp = pathElement.GPath;

            if (!BeforeDrawing(g,sp))
            {
                this.DrawTextBlock(g, sp, this.OwnerElement);
                return;
            }
            if (this.OwnerDocument.DrawElementWithCache(this.OwnerElement))
            {
                this.DrawWithCache(g, sp);
                return;
            }
            sp.BeginStyleContainer(this.OwnerElement);
            System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
            System.Drawing.Text.TextRenderingHint hint = g.TextRenderingHint;
            InterpolationMode mode1 = g.InterpolationMode;
            GraphicsContainer c = g.BeginContainer();
            g.SmoothingMode = mode;
            g.TextRenderingHint = hint;
            g.InterpolationMode = mode1;
            try
            {
                if (sp.ViewVisible && this.StyleContainer.VisualMediaStyle.visiblility != "hidden" && this.StyleContainer.VisualMediaStyle.display != "none")
                {
                    using (GraphicsPath gp1 = gp.Clone() as GraphicsPath)
                    {
                        if ((DataType.SVGString)sp.FillStyle.fillrule == "evenodd")
                            gp1.FillMode = System.Drawing.Drawing2D.FillMode.Alternate;
                        else
                            gp1.FillMode = System.Drawing.Drawing2D.FillMode.Winding;
                        this.OwnerElement.Clip(g, sp);
                       
                        if (!sp.BoundView)
                        {
                            //FillPath，Fill应发生在Transform之前
                            this.DrawShadow(sp, g, gp1);
                            Paint.SVGPaint svgpaint = (Paint.SVGPaint)sp.FillStyle.svgPaint;
                            if (!svgpaint.IsEmpty && gp1.PointCount > 2)
                                this.FillPath(g, svgpaint, gp1, this.OwnerElement, sp);

                            this.OwnerElement.TransformPath(g, gp1, sp);
                            Paint.SVGPaint svgstroke = (Paint.SVGPaint)sp.StrokeStyle.svgStroke;
                            if (!svgstroke.IsEmpty)
                                this.StrokePath(g, svgstroke, gp1, this.OwnerElement, sp);
                        }
                        else
                        {
                            this.OwnerElement.TransformPath(g, gp1, sp);
                            g.DrawPath(sp.outlinePen, gp1);
                        }

                        if (this.OwnerElement.MarkerStart != null)
                            this.OwnerElement.MarkerStart.MarkerStart(g, gp1, sp);

                        if (this.OwnerElement.MarkerEnd != null)
                            this.OwnerElement.MarkerEnd.MarkerEnd(g, gp1, sp);

                        //this.ResetPath(g, gp, sp);
                        this.DrawBackgroundImage(gp1, g);
                        this.DrawLabel(g, sp);
                        this.DrawTextBlock(g, sp, this.OwnerElement);
                        this.DrawConnect(g, sp);
                    }
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

        #region ..AddToRenderElements
        public virtual void AddToRenderElements(StyleContainer.StyleOperator sp)
        {
            if (sp.AddRender && sp.renderElements != null 
                && !sp.renderElements.Contains(this.OwnerElement)
                && this.StyleContainer != null
                && this.StyleContainer.VisualMediaStyle.visiblility != "hidden"
                && this.StyleContainer.VisualMediaStyle.display != "none")
                sp.renderElements.Add(this.OwnerElement);
        }
        #endregion

        #region ..DrawShadow
        /// <summary>
        /// Draw shadow for the shape
        /// </summary>
        /// <param name="g"></param>
        /// <param name="shadowPath">the path of the shadow</param>
        public virtual void DrawShadow(StyleContainer.StyleOperator sp, Graphics g, System.Drawing.Drawing2D.GraphicsPath shadowPath)
        {
            System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
            System.Drawing.Text.TextRenderingHint hint = g.TextRenderingHint;
            InterpolationMode mode1 = g.InterpolationMode;
            GraphicsContainer c = g.BeginContainer();
            g.SmoothingMode = mode;
            g.TextRenderingHint = hint;
            g.InterpolationMode = mode1;
            if (!this.OwnerDocument.ScaleStroke)
            {
                g.Transform = this.OwnerElement.TotalTransform.Clone();
                g.MultiplyTransform(sp.coordTransform, MatrixOrder.Append);
            }
            try
            {
                if (sp.DrawShadow && sp.ShadowStyle.DrawShadow && shadowPath != null)
                {
                    float opacity = (float)Math.Max(0, Math.Min(sp.ShadowStyle.Opacity, 1));
                    using (Brush brush = new SolidBrush(Color.FromArgb((int)(255f * opacity), sp.ShadowStyle.ShadowColor)))
                    {
                        using (Pen pen = new Pen(brush))
                        {
                            this.ApplyStrokeStyleToPen(sp, pen);
                            SmoothingMode mode3 = g.SmoothingMode;
                            GraphicsContainer c1 = g.BeginContainer();
                            g.SmoothingMode = mode3;
                            g.SetClip(shadowPath, CombineMode.Exclude);
                            g.TranslateTransform(sp.ShadowStyle.XOffset, sp.ShadowStyle.YOffset);
                            if (this.OwnerElement.FillShadow)
                                g.FillPath(brush, shadowPath);
                            g.DrawPath(pen, shadowPath);
                            g.EndContainer(c1);
                            this.Cache.CacheShadowPen = pen.Clone() as Pen;
                            this.Cache.CacheShadowBrush = brush.Clone() as Brush;
                        }
                    }
                }
            }
            finally
            {
                g.EndContainer(c);
            }
        }
        #endregion

        #region ..FillPath(SVGPaint)
        /// <summary>
        /// 用指定的svgcolor填充路径
        /// </summary>
        /// <param name="g"></param>
        /// <param name="svgPaint"></param>
        /// <param name="path"></param>
        public void FillPath(Graphics g, Interface.DataType.ISVGColor svgPaint, GraphicsPath path, SVGStyleable ownerElement, StyleContainer.StyleOperator sp)
        {
            SmoothingMode mode = g.SmoothingMode;
            GraphicsContainer c = g.BeginContainer();
            g.SmoothingMode = mode;
            if (!this.OwnerDocument.ScaleStroke)
            {
                g.Transform = this.OwnerElement.TotalTransform.Clone();
                g.MultiplyTransform(sp.coordTransform, MatrixOrder.Append);
            }
            try
            {
                float alpha = 1;

                alpha = sp.FillStyle.fillOpacity.Value;

                float opacity = sp.ClipStyle.opacity.Value;
                if (opacity != 1)
                    alpha = opacity;

                Interface.ISVGPathable render = ownerElement as Interface.ISVGPathable;
                if (render == null)
                    return;
                ulong paintType = (ulong)PaintType.SVG_PAINTTYPE_RGBCOLOR;
                if (svgPaint is Paint.SVGPaint)
                    paintType = ((Paint.SVGPaint)svgPaint).PaintType;
                switch (paintType)
                {
                    case (ulong)PaintType.SVG_PAINTTYPE_NONE:
                        this.Cache.CacheBrush = null;
                        break;
                    case (ulong)PaintType.SVG_PAINTTYPE_URI:
                        string id = ((Paint.SVGPaint)svgPaint).Uri;
                        SVG.GradientsAndPatterns.SVGPaintTransformElement element = ownerElement.OwnerDocument.GetReferencedNode(id, new string[] { "linearGradient", "radialGradient", "pattern" }) as SVG.GradientsAndPatterns.SVGPaintTransformElement;
                        if (element != null)
                        {
                            RectangleF rect = RectangleF.Empty;
                            using (GraphicsPath tempPath = path.Clone() as GraphicsPath)
                            {
                                //for the pattern, update the rect
                                rect = tempPath.GetBounds();
                                rect.Inflate(1, 1);

                                this.Cache.CacheBrush = element.GetBrush(this.OwnerElement, Rectangle.Truncate(rect), opacity);
                                {
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
                                    //this.Cache.CacheBrush = brush.Clone() as Brush;
                                }
                            }
                        }
                        break;

                    #region ..单色绘制
                    case (ulong)PaintType.SVG_PAINTTYPE_RGBCOLOR:
                        alpha = (float)Math.Max(0, Math.Min(alpha * 255f, 255));
                        this.Cache.CacheBrush = this.GetBrush(sp, (render as SVG.Interface.ISVGPathable).GPath.GetBounds());
                        {
                            if (this.Cache.CacheBrush != null)
                            {
                                //using (Matrix matrix = (render as SVGTransformableElement).TotalTransform.Clone())
                                //{
                                if (this.Cache.CacheBrush is System.Drawing.Drawing2D.LinearGradientBrush)
                                {

                                    g.FillPath(this.Cache.CacheBrush, (render as SVG.Interface.ISVGPathable).GPath);
                                }
                                else if (this.Cache.CacheBrush is System.Drawing.Drawing2D.PathGradientBrush)
                                {
                                    g.FillPath(new SolidBrush((this.Cache.CacheBrush as PathGradientBrush).InterpolationColors.Colors[0]), (render as SVG.Interface.ISVGPathable).GPath);
                                    g.FillPath(this.Cache.CacheBrush, (render as SVG.Interface.ISVGPathable).GPath);
                                }
                                else
                                {
                                    if (this.Cache.CacheBrush is HatchBrush)
                                        g.RenderingOrigin = Point.Round(path.GetBounds().Location);
                                    g.FillPath(this.Cache.CacheBrush, path);
                                }
                                //}
                            }
                        }
                        break;
                    #endregion
                }
            }
            finally
            {
                g.EndContainer(c);
            }
        }
        #endregion

        #region ..StrokePath(SVGPaint)
        public void StrokePath(Graphics g, Interface.DataType.ISVGColor svgPaint, GraphicsPath path, SVGStyleable ownerElement, StyleContainer.StyleOperator sp)
        {
            if (svgPaint == null)
                return;
            float alpha = 1;
            //			float opacity;

            alpha = sp.StrokeStyle.strokeOpacity.Value;

            float opacity = sp.ClipStyle.opacity.Value;
            if (opacity != 1)
                alpha = opacity;

            #region stroke-width
            float strokeWidth = sp.StrokeStyle.strokewidth.Value;
            #endregion

            ulong paintType = (ulong)PaintType.SVG_PAINTTYPE_RGBCOLOR;
            if (svgPaint is Paint.SVGPaint)
                paintType = ((Paint.SVGPaint)svgPaint).PaintType;

            using (Pen pen = new Pen(Color.Empty, strokeWidth))
            {
                this.ApplyStrokeStyleToPen(sp, pen);
                this.DrawBorder(g, path, strokeWidth);

                using (Brush brush = this.GetBrush(svgPaint, path.GetBounds(), sp, ownerElement, opacity))
                {
                    if (brush != null)
                    {
                        pen.Brush = brush;
                        this.Cache.CachePen = pen.Clone() as Pen;
                        this.Cache.GradientColorBlend = null;
                        //如果是Perpendicular
                        if ((brush is LinearGradientBrush)
                            && !sp.StrokeStyle.stroke_gradientMode.IsEmpty 
                            && sp.StrokeStyle.stroke_gradientMode == "perpendicular" 
                            && strokeWidth > 2 && opacity > 0 && path.PointCount > 1)
                        {
                            this.Cache.GradientColorBlend = (brush as LinearGradientBrush).InterpolationColors;
                            DrawPerpendicularStroke(g, this.Cache.GradientColorBlend, path, strokeWidth);
                        }
                        else
                        {
                            g.DrawPath(pen, path);
                        }
                    }
                    else
                        this.Cache.CachePen = null;
                }
            }
        }
        #endregion

        #region ..ApplyStrokeStyleToPen
        /// <summary>
        /// Apply the stroke style to the pen
        /// </summary>
        /// <param name="sp"></param>
        /// <param name="pen"></param>
        public void ApplyStrokeStyleToPen(StyleContainer.StyleOperator sp, Pen pen)
        {
            float strokeWidth = sp.StrokeStyle.strokewidth.Value;

            #region Line caps
            //string lineCap = sp.StrokeStyle.stroke_linecap.Value;//ownerElement.GetAttribute("stroke-linecap");
            if (sp.StrokeStyle.stroke_linecap == "round")
                pen.StartCap = pen.EndCap = LineCap.Round;
            else if (sp.StrokeStyle.stroke_linecap == "square")
                pen.StartCap = pen.EndCap = LineCap.Square;
            else
                pen.StartCap = pen.EndCap = LineCap.Flat;

            #endregion

            #region Line joins
            // TODO: stroke-miterlimit - MiterLimit
            //string lineJoin = sp.StrokeStyle.stroke_linejoin.Value;//ownerElement.GetAttribute("stroke-linejoin");
            if (sp.StrokeStyle.stroke_linejoin == "round")
                pen.LineJoin = LineJoin.Round;
            else if (sp.StrokeStyle.stroke_linejoin == "bevel")
                pen.LineJoin = LineJoin.Bevel;
            else pen.LineJoin = LineJoin.Miter;
       
            #endregion

            #region miter limit

            // TODO: miter limit does not work exactly as ASV for certain values.

            //				string miterLimitStr = ;
            //				if(miterLimitStr == null || miterLimitStr.Length == 0) miterLimitStr = "4";
            float miterLimit = sp.StrokeStyle.stroke_miterlimit.Value;//DataType.SVGNumber.ParseNumberStr(miterLimitStr);
            if (miterLimit < 1)
            {
                this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new string[] { "stroke-miterlimit can not be less then 1" }, ExceptionLevel.Normal));
                miterLimit = 4;
            }
            pen.MiterLimit = miterLimit;
            //				miterLimitStr = null;
            #endregion

            #region Dash array
            // TODO: this should be SvgLength values, not just floats
            string dashArray = sp.StrokeStyle.stroke_dasharray.Value.Trim();//ownerElement.GetAttribute("stroke-dasharray");
            if (dashArray != null && dashArray.Length > 0 && string.Compare(dashArray,"none") != 0)
            {

                dashArray = re.Replace(dashArray, ",");
                string[] sDashArray = dashArray.Split(new char[] { ',' });
                float[] fDashArray = new float[sDashArray.GetLength(0)];

                bool valid = true;
                for (int i = 0; i < sDashArray.GetLength(0); i++)
                {
                    //divide by strokeWidth to take care of the difference between Svg and GDI+
                    fDashArray[i] = new DataType.SVGLength(sDashArray[i], this.OwnerElement, LengthDirection.Viewport).Value / strokeWidth;//  DataType.SVGNumber.ParseNumberStr(sDashArray[i]) / strokeWidth;
                    if (fDashArray[i] == 0)
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    if (fDashArray.GetLength(0) % 2 == 1)
                    {
                        //odd number of values, duplicate
                        float[] tmpArray = new float[fDashArray.GetLength(0) * 2];
                        fDashArray.CopyTo(tmpArray, 0);
                        fDashArray.CopyTo(tmpArray, fDashArray.GetLength(0));

                        fDashArray = tmpArray;
                        tmpArray = null;
                    }

                    pen.DashPattern = fDashArray;
                }
                sDashArray = null;
                fDashArray = null;
            }
            dashArray = null;
            #endregion

            #region Dash offset
            //TODO: this should be a SvgLength, not a float
            //				string dashOffset = sp.StrokeStyle.stroke_dashoffset.Value;//ownerElement.GetAttribute("stroke-dashoffset");

            float offset = sp.StrokeStyle.stroke_dashoffset.Value;
            if (offset > 0)
            {
                //divide by strokeWidth to take care of the difference between Svg and GDI+
                pen.DashOffset = offset / strokeWidth;
            }
            //				dashOffset = null;
            #endregion

            pen.Alignment = PenAlignment.Outset;
        }
        #endregion

        #region ..GetBrush
        public System.Drawing.Brush GetBrush(Interface.DataType.ISVGColor svgPaint, RectangleF bounds, StyleContainer.StyleOperator sp, SVGStyleable ownerElement, float opacity)
        {
            ulong paintType = (ulong)PaintType.SVG_PAINTTYPE_RGBCOLOR;
            if (svgPaint is Paint.SVGPaint)
                paintType = ((Paint.SVGPaint)svgPaint).PaintType;
            switch (paintType)
            {
                case (ulong)PaintType.SVG_PAINTTYPE_NONE:
                    return null;
                case (ulong)PaintType.SVG_PAINTTYPE_URI:
                    string id = ((SVG.Paint.SVGPaint)svgPaint).Uri;
                    SVG.GradientsAndPatterns.SVGPaintTransformElement element = ownerElement.OwnerDocument.GetReferencedNode(id, new string[] { "linearGradient", "radialGradient", "pattern" }) as SVG.GradientsAndPatterns.SVGPaintTransformElement;
                    if (element != null)
                        return element.GetBrush(this.OwnerElement, Rectangle.Truncate(bounds), opacity);
                    break;

                #region ..单色绘制
                case (ulong)PaintType.SVG_PAINTTYPE_RGBCOLOR:
                    float alpha = (float)Math.Max(0, Math.Min(opacity * 255f, 255));
                    if (svgPaint.RgbColor != null)
                    {
                        Color color = ((DataType.RGBColor)svgPaint.RgbColor).GDIColor;
                        return new SolidBrush(Color.FromArgb(Convert.ToInt32(alpha), color));
                    }
                    break;
                #endregion
            }
            return null;
        }

        System.Drawing.Brush GetBrush(StyleContainer.StyleOperator sp, RectangleF rect)
        {
            string temp = sp.FillStyle.HatchStyle.Value;
            HatchStyle hatch = HatchStyle.None;
            if (temp != null)
            {
                if (System.Enum.IsDefined(typeof(HatchStyle), temp))
                    hatch = (HatchStyle)System.Enum.Parse(typeof(HatchStyle), temp, true);
            }
            Color color = Color.White;
            if (sp.FillStyle.svgPaint.PaintType == (ulong)PaintType.SVG_PAINTTYPE_RGBCOLOR)
                color = ((DataType.RGBColor)sp.FillStyle.svgPaint.RgbColor).GDIColor;
            float alpha = sp.FillStyle.fillOpacity.Value;
            alpha = (float)Math.Max(0, Math.Min(alpha * 255f, 255));
            color = Color.FromArgb((int)alpha, color);
            if (hatch == HatchStyle.None)
                return new SolidBrush(color);
            else if ((int)hatch < 56)
            {
                Color c = sp.FillStyle.HatchColor.GDIColor;
                c = Color.FromArgb((int)alpha, c);
                System.Drawing.Drawing2D.HatchStyle style1 = System.Drawing.Drawing2D.HatchStyle.Cross;
                style1 = (System.Drawing.Drawing2D.HatchStyle)System.Enum.Parse(typeof(System.Drawing.Drawing2D.HatchStyle), hatch.ToString(), false);
                return new HatchBrush(style1, c, color);
            }
            else
            {
                Brush brush = null;
                ColorBlend bl = new ColorBlend();
                Color c = sp.FillStyle.HatchColor.GDIColor;
                c = Color.FromArgb((int)alpha, c);
                using (System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath())
                {
                    Matrix matrix = new Matrix();
                    switch (hatch)
                    {
                        case HatchStyle.Center:
                            path.AddEllipse(rect);
                            brush = new System.Drawing.Drawing2D.PathGradientBrush(path);
                            //							matrix.Multiply((brush as LinearGradientBrush).Transform);
                            bl.Positions = new float[] { 0, 1 };
                            bl.Colors = new Color[] { color, c };
                            ((PathGradientBrush)brush).InterpolationColors = bl;
                            ((PathGradientBrush)brush).CenterPoint = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
                            (brush as PathGradientBrush).WrapMode = WrapMode.Clamp;
                            //							(brush as LinearGradientBrush).Transform = matrix;
                            break;
                        case HatchStyle.VerticalCenter:
                            brush = new LinearGradientBrush(rect, color, c, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                            matrix.Multiply((brush as LinearGradientBrush).Transform);
                            bl.Positions = new float[] { 0, 0.5f, 1 };
                            bl.Colors = new Color[] { color, c, color };
                            ((LinearGradientBrush)brush).InterpolationColors = bl;
                            (brush as LinearGradientBrush).Transform = matrix;
                            break;
                        case HatchStyle.HorizontalCenter:
                            brush = new LinearGradientBrush(rect, color, c, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            matrix.Multiply((brush as LinearGradientBrush).Transform);
                            bl.Positions = new float[] { 0, 0.5f, 1 };
                            bl.Colors = new Color[] { color, c, color };
                            ((LinearGradientBrush)brush).InterpolationColors = bl;
                            (brush as LinearGradientBrush).Transform = matrix;
                            break;
                        case HatchStyle.DiagonalLeft:
                            brush = new LinearGradientBrush(rect, color, c, System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
                            matrix.RotateAt(45, new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f));
                            bl.Positions = new float[] { 0, 0.5f, 1 };
                            bl.Colors = new Color[] { color, c, color };
                            ((LinearGradientBrush)brush).InterpolationColors = bl;
                            (brush as LinearGradientBrush).Transform = matrix;
                            break;
                        case HatchStyle.DiagonalRight:

                            brush = new LinearGradientBrush(rect, color, c, System.Drawing.Drawing2D.LinearGradientMode.BackwardDiagonal);
                            matrix.RotateAt(-45, new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f));
                            bl.Positions = new float[] { 0, 0.5f, 1 };
                            bl.Colors = new Color[] { color, c, color };
                            ((LinearGradientBrush)brush).InterpolationColors = bl;
                            (brush as LinearGradientBrush).Transform = matrix;
                            break;
                        case HatchStyle.LeftRight:

                            brush = new LinearGradientBrush(rect, color, c, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
                            matrix.Multiply((brush as LinearGradientBrush).Transform);
                            bl.Positions = new float[] { 0, 1 };
                            bl.Colors = new Color[] { color, c };
                            ((LinearGradientBrush)brush).InterpolationColors = bl;
                            (brush as LinearGradientBrush).Transform = matrix;
                            break;
                        case HatchStyle.TopBottom:

                            brush = new LinearGradientBrush(rect, color, c, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
                            matrix.Multiply((brush as LinearGradientBrush).Transform);
                            bl.Positions = new float[] { 0, 1 };
                            bl.Colors = new Color[] { color, c };
                            ((LinearGradientBrush)brush).InterpolationColors = bl;
                            (brush as LinearGradientBrush).Transform = matrix;
                            break;
                    }
                    bl = null;
                    return brush;
                }
            }
        }
        #endregion

        #region ..Connection
        public override PointF[] ConnectionPoints
        {
            get
            {
                if (this.OwnerElement.ConnectionChanged)
                {
                    this.CreateConnectPoint();
                    this.OwnerElement.ConnectionChanged = false;
                }
                return connectionPoints;
            }
        }

        public virtual void DrawConnect(System.Drawing.Graphics g, StyleContainer.StyleOperator sp)
        {
            this.DrawConnectPoint(g, sp, true);
        }

        public virtual void DrawConnectPoint(System.Drawing.Graphics g, StyleContainer.StyleOperator sp, bool drawConnectPoint)
        {
            if (!this.OwnerElement.Connectable)
                return;
            //update the connect points
            if (this.OwnerElement.ConnectionChanged)
            {
                this.CreateConnectPoint();
                this.OwnerElement.ConnectionChanged= false;
            }
            if (sp.connectElements != null)
            {
                PointF[] ps = this.OwnerElement.ConnectionPoints;
                if (ps != null)
                {
                    if (sp.AddConnectableElements)
                    {
                        if (sp.drawConnects && drawConnectPoint && ps.Length > 0)
                        {
                            int r = sp.connectSnap / 2;
                            ps = ps.Clone() as PointF[];
                            this.OwnerElement.TotalTransform.TransformPoints(ps);
                            sp.coordTransform.TransformPoints(ps);
                            for (int i = 0; i < ps.Length; i++)
                            {
                                PointF p = ps[i];
                                if (!sp.OnPaintConnectablePoint(g, this.OwnerElement, p, i))
                                {
                                    g.DrawLine(sp.connectPen, p.X - r, p.Y - r, p.X + r, p.Y + r);
                                    g.DrawLine(sp.connectPen, p.X + r, p.Y - r, p.X - r, p.Y + r);//,p.Y - 5,p.X,p.Y + 5);
                                }
                            }
                        }

                        if (!sp.connectElements.Contains(this.OwnerElement))
                            sp.connectElements.Add(this.OwnerElement);
                    }
                }
            }
        }

        #region ..CreateConnectPoint
        public virtual void CreateConnectPoint()
        {
            if (!this.OwnerElement.Connectable)
                return;
            Interface.ISVGPathable pathElement = this.OwnerElement as Interface.ISVGPathable;
            if (pathElement == null)
                return;
            if (pathElement.GPath != null && pathElement.GPath.PointCount > 1)
            {
                using (System.Drawing.Drawing2D.GraphicsPath path = pathElement.GPath.Clone() as GraphicsPath)
                {
                    path.Flatten();
                    RectangleF rect = path.GetBounds();
                    //if use the default connected Point
                    PointF[] ps = null;
                    if (this.OwnerElement.UseDefaultConnectedPoint)
                        ps = new PointF[] { new PointF(rect.X + rect.Width / 2f, rect.Y), new PointF(rect.Right, rect.Height / 2f + rect.Y), new PointF(rect.X + rect.Width / 2f, rect.Bottom), new PointF(rect.X, rect.Y + rect.Height / 2f) };//,new PointF(rect.X + rect.Width / 2,rect.Y + rect.Height / 2f)};
                    //if not
                    else
                    {
                        SVG.DataType.SVGPointList points = this.OwnerElement.BaseConnectionPoints;
                        ps = new PointF[points.NumberOfItems];
                        for (int i = 0; i < points.NumberOfItems; i++)
                        {
                            DataType.SVGPoint p = (DataType.SVGPoint)points.GetItem(i);
                            float x = (float)Math.Max(0, Math.Min(p.X / 100f, 1));
                            float y = (float)Math.Max(0, Math.Min(p.Y / 100f, 1));
                            ps[i] = new PointF(rect.X + rect.Width * x, rect.Y + rect.Height * y);
                        }
                    }

                    this.connectionPoints = ps;
                }
            }

            this.OwnerElement.ConnectionChanged = false;
        }
        #endregion
        #endregion

        #region ..DrawWithCache
        public virtual void DrawWithCache(Graphics g, StyleContainer.StyleOperator sp)
        {
            System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
            System.Drawing.Text.TextRenderingHint hint = g.TextRenderingHint;
            InterpolationMode mode1 = g.InterpolationMode;
            GraphicsContainer c = g.BeginContainer();
            g.SmoothingMode = mode;
            g.InterpolationMode = mode1;
            g.TextRenderingHint = hint;
            try
            {
                Interface.ISVGPathable pathElement = this.OwnerElement as Interface.ISVGPathable;
                GraphicsPath gp = pathElement.GPath;

                if (this.StyleContainer.ViewVisible && this.StyleContainer.VisualMediaStyle.visiblility != "hidden" && this.StyleContainer.VisualMediaStyle.display != "none")
                {
                    this.OwnerElement.Clip(g, sp);
                    using (GraphicsPath gp1 = gp.Clone() as GraphicsPath)
                    {
                        if (!sp.BoundView)
                        {
                            GraphicsContainer c1 = g.BeginContainer();
                            g.SmoothingMode = mode;
                            g.InterpolationMode = mode1;
                            g.TextRenderingHint = hint;
                            if (!this.OwnerDocument.ScaleStroke)
                            {
                                g.Transform = this.OwnerElement.TotalTransform.Clone();
                                g.MultiplyTransform(sp.coordTransform, MatrixOrder.Append);
                            }
                            this.DrawShadowWithCache(sp, g, gp1);
                            if (this.Cache.CachePen != null)
                                this.DrawBorder(g, gp1, this.Cache.CachePen.Width);
                            if (this.Cache.CacheBrush != null)
                            {
                                if (this.Cache.CacheBrush is PathGradientBrush)
                                {
                                    ColorBlend cl = (this.Cache.CacheBrush as PathGradientBrush).InterpolationColors;
                                    if (cl.Colors.Length > 0)
                                    {
                                        using (SolidBrush brush1 = new SolidBrush(cl.Colors[0]))
                                            g.FillPath(brush1, gp1);
                                    }
                                }
                                if (this.Cache.CacheBrush is HatchBrush)
                                    g.RenderingOrigin = Point.Round(gp1.GetBounds().Location);
                                g.FillPath(this.Cache.CacheBrush, gp1);
                            }
                            g.EndContainer(c1);
                            if (this.Cache.CachePen != null)
                            {
                                this.OwnerElement.TransformPath(g, gp1, sp);

                                //perpendicular，垂直Stroke Gradient
                                if (this.Cache.CachePen.Brush is LinearGradientBrush && this.Cache.GradientColorBlend != null)
                                {
                                    DrawPerpendicularStroke(g, this.Cache.GradientColorBlend, gp1, this.Cache.CachePen.Width);
                                }
                                else
                                    g.DrawPath(this.Cache.CachePen, gp1);
                            }
                        }
                        else
                        {
                            this.OwnerElement.TransformPath(g, gp1, sp);
                            g.DrawPath(sp.outlinePen, gp1);
                        }

                        if (this.OwnerElement.MarkerStart != null)
                            this.OwnerElement.MarkerStart.MarkerStart(g, gp1, sp);

                        if (this.OwnerElement.MarkerEnd != null)
                            this.OwnerElement.MarkerEnd.MarkerEnd(g, gp1, sp);

                        this.DrawBackgroundImage(gp1, g);
                        this.DrawLabel(g, sp, this.Cache.CacheFont);
                        this.DrawTextBlock(g, sp, this.OwnerElement);
                        this.DrawConnect(g, sp);
                    }
                }
            }
            catch (System.Exception e1)
            {
                this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e1.Message, e1.StackTrace }, ExceptionLevel.Normal));
            }
            finally
            {
                this.AddToRenderElements(sp);
                this.OwnerElement.CurrentTime = this.OwnerDocument.CurrentTime;
                g.EndContainer(c);
            }
        }
        #endregion

        #region ..DrawShadowWithCache
        /// <summary>
        /// draw current shadow with shadow
        /// </summary>
        /// <param name="g"></param>
        /// <param name="shadowPath"></param>
        public virtual void DrawShadowWithCache(StyleContainer.StyleOperator sp, Graphics g, System.Drawing.Drawing2D.GraphicsPath shadowPath)
        {
            if (sp.DrawShadow && this.StyleContainer.ShadowStyle.DrawShadow && shadowPath != null)
            {
                System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
                System.Drawing.Drawing2D.GraphicsContainer c = g.BeginContainer();
                g.SmoothingMode = mode;
                g.SetClip(shadowPath, CombineMode.Exclude);
                g.TranslateTransform(this.StyleContainer.ShadowStyle.XOffset, this.StyleContainer.ShadowStyle.YOffset);
                if (this.OwnerElement.FillShadow && this.Cache.CacheShadowBrush != null)
                    g.FillPath(this.Cache.CacheShadowBrush, shadowPath);
                if (this.Cache.CacheShadowPen != null)
                    g.DrawPath(this.Cache.CacheShadowPen, shadowPath);
                g.EndContainer(c);
            }
        }
        #endregion

        #region ..BeforeDrawing
        public override bool BeforeDrawing(Graphics g, StyleContainer.StyleOperator sp)
        {
            bool drawing = base.BeforeDrawing(g, sp);
            if (!drawing)
            {
                this.DrawConnectPoint(g, sp, false);
                this.AddToRenderElements(sp);
                //this.OwnerElement.CurrentTime = this.OwnerDocument.CurrentTime;
            }
            return drawing;
        }
        #endregion

        #region ..DrawTextBlock
        public void DrawTextBlock(Graphics g, StyleContainer.StyleOperator sp, SVGTransformableElement ownerElement)
        {
            ownerElement.ResetGraphicsPathIncludingTextBlock();

            foreach (SVGElement child in ownerElement.ChildElements)
            {
                if (child is Text.SVGTextBlockElement && (child as SVGTransformableElement).SVGRenderer != null)
                {
                    Text.SVGTextBlockElement block = child as Text.SVGTextBlockElement;
                    //block.TotalTransform.Reset();
                    block.SVGRenderer.Draw(g, sp);
                    if (block.GPath != null && block.GPath.PointCount > 1)
                    {
                        ownerElement.GraphicsPathIncludingTextBlock.StartFigure();
                        using (GraphicsPath path1 = block.GPath.Clone() as GraphicsPath)
                        {
                            path1.Transform(block.TotalTransform);
                            ownerElement.GraphicsPathIncludingTextBlock.AddPath(path1, false);
                        }
                    }
                }
            }
        }
        #endregion

        #region ..UpdateTransform
        /// <summary>
        /// 当对应的图元TotalTransform发生改变时，更新对应Renderer的信息
        /// </summary>
        public override void UpdateTotalTransform()
        {
            //如果Cache Brush是Gradient，则Transform更新的时候，不能使用Cache
            if (this.Cache.CacheBrush is LinearGradientBrush || this.Cache.CacheBrush is PathGradientBrush)
                this.OwnerElement.UpdateElement(false);
        }
        #endregion

        #region ..DrawPerpendicularStroke
        void DrawPerpendicularStroke(Graphics g, ColorBlend blend,GraphicsPath origialPath, float strokeWidth)
        {
            if (blend == null || origialPath == null
                || origialPath.PointCount < 2 || strokeWidth < 2)
                return;
            Color startColor = Color.Black, endColor = Color.Black;
            if (blend.Colors.Length > 0)
            {
                startColor = blend.Colors[0];
                endColor = blend.Colors.Last();
            }
            SmoothingMode mode = SmoothingMode.HighQuality;
            using (Pen upPen = new Pen(endColor), dnPen = new Pen(startColor))
            {
                using (Matrix matrix = new Matrix())
                {
                    using (GraphicsPath path1 = origialPath.Clone() as GraphicsPath)//, path3 = path.Clone() as GraphicsPath)
                    {
                        using (GraphicsPath path2 = new GraphicsPath())
                        {
                            using (Pen pen1 = new Pen(Color.White, 1))
                            {
                                path1.Flatten(new Matrix(), 0.25f);
                                PointF? ptUpPrev = null, ptDnPrev = null;
                                PointF ptUpStart = PointF.Empty, ptDnStart = PointF.Empty;
                                var points = path1.PathPoints;
                                var types = path1.PathTypes;
                                var startIndex = 0;
                                for (int index = 0; index < points.Length; index++)
                                {
                                    var ptUp = PointF.Empty;
                                    var ptDn = PointF.Empty;
                                    PointF p = points[index];
                                    PointF? prePoint = null;
                                    PointF? nextPoint = null;
                                    var type = types[index];
                                    if (type == (byte)PathPointType.Start)
                                    {
                                        startIndex = index;
                                        //寻找下一个结束节点
                                        int nextIndex = Array.IndexOf(types, (byte)PathPointType.CloseSubpath, index + 1);
                                        if (nextIndex < 0)
                                            nextIndex = Array.IndexOf(types, (byte)(PathPointType.Line | PathPointType.CloseSubpath), index + 1);
                                        if (nextIndex > 0 && points[nextIndex] != p)
                                            prePoint = points[nextIndex];
                                    }
                                    else if(index - 1>=0)
                                        prePoint = points[index-1];

                                    {
                                        if ((type & (byte)PathPointType.CloseSubpath) == (byte)PathPointType.CloseSubpath)
                                            nextPoint = points[startIndex];
                                    }

                                    int oriIndex = index;
                                    bool close = false;
                                    do
                                    {
                                        if (index + 1 < points.Length)
                                            nextPoint = points[index + 1];
                                        if (!prePoint.HasValue)
                                        {
                                            if (nextPoint.HasValue)
                                            {
                                                PointF p1 = nextPoint.Value;
                                                Vector vector = new Vector(p1.X - p.X, p1.Y - p.Y);
                                                Vector vector90 = new Vector(vector.Y, -vector.X);
                                                vector90.Normalize();

                                                float x = p.X + strokeWidth / 2 * vector90.X;
                                                float y = p.Y + strokeWidth / 2 * vector90.Y;

                                                ptUp = new PointF(x, y);
                                                ptDn = new PointF(p.X - strokeWidth / 2 * vector90.X, p.Y - strokeWidth / 2 * vector90.Y);

                                                ptDnPrev = null;
                                                ptUpPrev = null;
                                            }
                                        }
                                        else
                                        {
                                            PointF ptPrev = prePoint.Value;
                                            Vector vect1 = new Vector(ptPrev.X - p.X, ptPrev.Y - p.Y);
                                            double angle1 = Math.Atan2(vect1.Y, vect1.X);
                                            double angle2 = angle1;
                                            if (nextPoint.HasValue)
                                            {
                                                PointF p1 = nextPoint.Value;
                                                Vector vect2 = new Vector(p1.X - p.X, p1.Y - p.Y);
                                                angle2 = Math.Atan2(vect2.Y, vect2.X);
                                                double diff = angle2 - angle1;

                                                if (diff < 0)
                                                    diff += 2 * Math.PI;

                                                double angle = angle1 + diff / 2;
                                                double angle3 = Vector.AngleBetween(vect1, vect2);
                                                angle3 = Math.Abs(angle3) / 2;
                                                var width = (float)(strokeWidth / Math.Sin(angle3 / 180 * Math.PI));
                                                Vector vect = new Vector((float)Math.Cos(angle), (float)Math.Sin(angle));
                                                vect.Normalize();
                                                ptUp = new PointF(p.X + width / 2 * vect.X, p.Y + width / 2 * vect.Y);
                                                ptDn = new PointF(p.X - width / 2 * vect.X, p.Y - width / 2 * vect.Y);
                                                angle2 = angle;
                                            }
                                            else
                                            {
                                                var vector90 = new Vector(-vect1.Y, vect1.X);
                                                vector90.Normalize();
                                                ptUp.X = p.X + (strokeWidth / 2) * vector90.X;
                                                ptUp.Y = p.Y + (strokeWidth / 2) * vector90.Y;
                                                ptDn.X = p.X - (strokeWidth / 2) * vector90.X;
                                                ptDn.Y = p.Y - (strokeWidth / 2) * vector90.Y;
                                            }

                                            if (ptDnPrev.HasValue && ptUpPrev.HasValue)
                                            {
                                                GraphicsContainer c = g.BeginContainer();
                                                g.SmoothingMode = mode;
                                                matrix.Reset();
                                                var angle = (int)Math.Round(-180 * angle1 / Math.PI, 0);
                                                matrix.RotateAt(angle, ptPrev);

                                                PointF[] ps = new PointF[] { ptUpPrev.Value, ptUp, ptDn, ptDnPrev.Value };
                                                matrix.TransformPoints(ps);
                                                path2.Reset();
                                                path2.AddLines(ps);

                                                if (!path2.GetBounds().IsEmpty)
                                                {
                                                    matrix.Reset();
                                                    matrix.RotateAt(-angle, ptPrev);
                                                    g.Transform = matrix;
                                                    using (LinearGradientBrush brush1 = new LinearGradientBrush(path2.GetBounds(), Color.White, Color.Black, LinearGradientMode.Vertical))
                                                    {
                                                        brush1.InterpolationColors = blend;

                                                        g.FillPath(brush1, path2);
                                                        pen1.Brush = brush1;
                                                        g.DrawPath(pen1, path2);
                                                        g.DrawLine(upPen, ps[0].X, ps[0].Y, ps[1].X, ps[1].Y);
                                                        g.DrawLine(dnPen, ps[2].X, ps[2].Y, ps[3].X, ps[3].Y);
                                                    }
                                                }
                                                g.EndContainer(c);
                                            }
                                        }

                                        close = false;
                                        //当CloseSubPath时，绘制结束路径
                                        if(type == (byte)(PathPointType.CloseSubpath | PathPointType.Line))
                                        {
                                            prePoint = p;
                                            p = points[startIndex];
                                            index = startIndex;
                                            close = true;
                                            ptUpPrev = ptUp;
                                            ptDnPrev = ptDn;
                                            type = (byte)PathPointType.CloseSubpath;
                                        }
                                    }
                                    while (close);

                                    index = oriIndex;
                                    ptUpPrev = ptUp;
                                    ptDnPrev = ptDn;

                                    if (type == (byte)PathPointType.Start)
                                    {
                                        ptUpStart = ptUp;
                                        ptDnStart = ptDn;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
