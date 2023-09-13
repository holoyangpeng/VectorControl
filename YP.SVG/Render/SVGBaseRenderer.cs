using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Forms;

namespace YP.SVG.Render
{
    public class SVGBaseRenderer:Interface.ISVGRenderer
    {
        #region ..Line
        public class Line
        {
            #region ..私有变量
            int offset = 0;
            int length = 0;
            int delimeterlength = 0;
            #endregion

            #region ..构造及消除
            public Line(int offset, int length)
            {
                this.offset = offset;
                this.length = length;
                this.delimeterlength = 0;
            }
            #endregion

            #region ..公共字段
            public PointF Position = PointF.Empty;
            #endregion

            #region ..公共属性
            public int Offset
            {
                get
                {
                    return offset;
                }
                set
                {
                    this.offset = value;
                }
            }

            public int Length
            {
                get
                {
                    return this.length;
                }
                set
                {
                    this.length = value;
                }
            }

            public int DelimeterLength
            {
                set
                {
                    this.delimeterlength = value;
                }
                get
                {
                    return this.delimeterlength;
                }
            }
            #endregion
        }
        #endregion

        #region ..static fields
        static string tabString = "    ";
        #endregion

        #region ..Constructor
        public SVGBaseRenderer(SVGTransformableElement ownerElement)
        {
            this.ownerElement = ownerElement;
            this.labelStringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap | StringFormatFlags.NoClip;
            this.labelStringFormat.LineAlignment = StringAlignment.Center;
            this.labelStringFormat.Alignment = StringAlignment.Center;
        }
        #endregion

        #region ..properties
        public SVGTransformableElement OwnerElement
        {
            get
            {
                return this.ownerElement;
            }
        }

        public Cache.RenderCache Cache
        {
            get
            {
                return this.cache;
            }
            set
            {
                this.cache = value;
            }
        }

        public StyleContainer.StyleContainer StyleContainer
        {
            get
            {
                return this.ownerElement.StyleContainer;
            }
        }

        public Document.SVGDocument OwnerDocument
        {
            get
            {
                return this.ownerElement.OwnerDocument;
            }
        }
        #endregion

        #region ..Label

        #region ..private fields
        
        bool vertical = false;
        string labelText = string.Empty;
        bool labelParsed = true;
        StringFormat labelStringFormat = new StringFormat(StringFormat.GenericTypographic);
        RectangleF labelBounds = RectangleF.Empty;
        float labelRotate = 0;
        Font labelFont = new Font("SimSun", 12);
        PointF labelCenterPoint = PointF.Empty;
        //System.Drawing.SolidBrush labelBrush = new SolidBrush(Color.Black);
        SVGTransformableElement ownerElement;
        Alignment labelAlign = Alignment.Center;
        List<Line> labelLines = new List<Line>();
        Cache.RenderCache cache = new YP.SVG.Cache.RenderCache();
        #endregion

        #region ..properties
        public virtual bool DrawLabelBounds { set; get; }

        public virtual string LabelText
        {
            set
            {
                if (this.labelText != value)
                {
                    this.labelText = value;
                    ParseLines();
                }
            }
            get
            {
                return this.labelText;
            }
        }

        /// <summary>
        /// 获取文本标志的边框
        /// </summary>
        public RectangleF LabelBounds
        {
            get
            {
                return this.labelBounds;
            }
            internal set
            {
                this.labelBounds = value;
            }
        }

        /// <summary>
        /// gets the center point of the label
        /// </summary>
        public PointF LabelCenterPoint
        {
            get
            {
                return this.labelCenterPoint;
            }
            internal set
            {
                this.labelCenterPoint = value;
            }
        }

        /// <summary>
        /// 获取文本标志的旋转角
        /// </summary>
        public float LabelRotateAngle
        {
            get
            {
                return this.labelRotate;
            }
            set
            {
                this.labelRotate = value;
            }
        }

        public Font LabelFont
        {
            get
            {
                return this.labelFont;
            }
        }
        #endregion

        #region ..DrawLabel
        public virtual void DrawLabel(Graphics g, StyleContainer.StyleOperator sp)
        {
            this.DrawLabel(g, sp, null);
        }

        public void DrawLabel(Graphics g, StyleContainer.StyleOperator sp, Font font)
        {
            //使用TextBlock绘制Label，本方法暂时不用
        }
        #endregion

        #region ..GetStringFormat
        public void ParseStringFormat(YP.SVG.StyleContainer.StyleOperator sp, StringFormat stringFormat)
        {
            this.labelAlign = Alignment.Center;
            stringFormat.Alignment = StringAlignment.Center;
            //if (this.ownerElement.HasAttribute("text-anchor"))
            //{
            string anchor = sp.TextStyle.text_anchor.Value;
                switch (anchor.ToLower())
                {
                    case "start":
                    case "left":
                        stringFormat.Alignment = StringAlignment.Near;
                        this.labelAlign = Alignment.Left;
                        break;
                    case "end":
                    case "right":
                        stringFormat.Alignment = StringAlignment.Far;
                        this.labelAlign = Alignment.Right;
                        break;
                    default:
                        this.labelAlign = Alignment.Center;
                        stringFormat.Alignment = StringAlignment.Center;
                        break;
                }
            //}

            string anchor1 = sp.TextStyle.baseline_shift.Value;

            stringFormat.LineAlignment = StringAlignment.Center;
            //if (this.ownerElement.HasAttribute("vertical-align"))
            //{
            string align =  sp.TextStyle.vertical_align.Value;
            switch (align)
            {
                //case "middle":
                case "top":
                    stringFormat.LineAlignment = StringAlignment.Near;
                    break;
                //case "sub":
                case "bottom":
                    stringFormat.LineAlignment = StringAlignment.Far;
                    break;
                default:
                    stringFormat.LineAlignment = StringAlignment.Center;
                    break;
            }
            //}
            
            //overflow
            if (sp.VisualMediaStyle.overflow != "hidden")
                stringFormat.FormatFlags = StringFormatFlags.NoClip;
            else
                stringFormat.FormatFlags = StringFormatFlags.FitBlackBox;

            string dir = string.Empty;
            if (!sp.TextStyle.writing_mode.IsEmpty)
                dir = sp.TextStyle.writing_mode.Value.Trim();
            if (string.Compare(dir,"tb")==0 ||string.Compare(dir,"tb-rl") ==0)
                stringFormat.FormatFlags = stringFormat.FormatFlags | StringFormatFlags.DirectionVertical;

            if (sp.VisualMediaStyle.wrap == "nowrap")
                stringFormat.FormatFlags = stringFormat.FormatFlags | StringFormatFlags.NoWrap;

            stringFormat.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | stringFormat.FormatFlags;
        }
        #endregion

        #region ..GetFont
        public Font GetFont(StyleContainer.StyleOperator sp)
        {
            string name = "SimSun";
            float size = 12;
            System.Drawing.FontStyle style = System.Drawing.FontStyle.Regular;
            if (sp.FontStyle.FontWeigth == "bold" || sp.FontStyle.FontWeigth == "bolder")
                style = style | System.Drawing.FontStyle.Bold;
            if (sp.FontStyle.Font_Style == "italic")
                style = style | System.Drawing.FontStyle.Italic;
            if (sp.TextStyle.text_decoration == "underline")
                style = style | System.Drawing.FontStyle.Underline;
            if (!sp.FontStyle.FontFamily.IsEmpty)
                name = sp.FontStyle.FontFamily.Value;
            if (!sp.FontStyle.FontSize.IsEmpty)
                size = sp.FontStyle.FontSize.Value;
            //size = size * 0.718f;
            return new Font(name, size, style);
        }
        #endregion

        #region ..ParseLines
        void ParseLines()
        {
            if (!this.labelParsed)
                return;
            this.labelParsed = false;
            this.labelLines.Clear();
            this.ParseLines(this.labelText, labelLines);
        }

        void ParseLines(string text, List<Line> list)
        {
            string temp = text;
            if (temp.Length > 0)
            {
                int start = 0;
                for (int i = 0; i < temp.Length; i++)
                {
                    switch (temp[i])
                    {
                        case '\r':
                            if (i + 1 < temp.Length - 1 && temp[i + 1] == '\n')
                            {
                                Line line1 = new Line(start, i - start);
                                list.Add(line1);
                                line1.DelimeterLength = 2;
                                start = i + 2;
                                i++;
                            }
                            else
                            {
                                Line line1 = new Line(start, i - start);
                                list.Add(line1);
                                line1.DelimeterLength = 1;
                                start = i + 1;
                            }
                            break;
                        case '\n':
                            Line line = new Line(start, i - start);
                            list.Add(line);
                            line.DelimeterLength = 1;
                            start = i + 1;
                            break;
                    }
                }
                Line line2 = new Line(start, temp.Length - start);
                list.Add(line2);
            }
        }
        #endregion

        #region ..GetLineText
        /// <summary>
        /// gets the text of the line
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        string GetLineText(Line line)
        {
            if (line.Length > 0)
            {
                int offset = line.Offset;
                int length = Math.Min(offset + line.Length, this.labelText.Length) - offset;
                return this.labelText.Substring(offset, length);
            }
            return string.Empty;
        }
        #endregion

        #region ..GetPointAtIndex
        public virtual PointF GetPointAtIndex(int offset, Graphics g, ref PointF endPoint)
        {
            try
            {
                RectangleF bounds = this.GetBoundsForLine(offset, g);
                PointF p = new PointF(bounds.Right, bounds.Top);
                PointF p1 = new PointF(p.X, p.Y + bounds.Height);
                RectangleF rect = this.labelBounds;
                PointF[] ps = new PointF[] { p, p1 };
                using (System.Drawing.Drawing2D.Matrix matrix = new Matrix())
                {
                    matrix.RotateAt(this.labelRotate, this.labelCenterPoint);
                    matrix.TransformPoints(ps);
                }
                endPoint = ps[1];
                p = ps[0];
                ps = null;
                return p;
            }
            catch (System.Exception e1)
            {
                System.Diagnostics.Debug.Assert(true, e1.Message);
            }
            return Point.Empty;
        }
        #endregion

        #region ..InsertStr
        public virtual void InsertStr(int offset, string str)
        {
            if (str.Length == 0)
                return;
            bool old = this.labelParsed;
            this.labelParsed = false;
            int index = this.FindLineIndexAtOffset(offset);
            List<Line> list = new List<Line>();
            this.ParseLines(str, list);
            if (list.Count == 0)
                return;
            if (index >= 0 && index < this.labelLines.Count)
            {
                Line line1 = (Line)this.labelLines[index];
                int delta = str.Length;
                if (list.Count == 1)
                {
                    line1.Length += delta;
                }
                else
                {
                    Line templine = null;

                    for (int i = 1; i < list.Count; i++)
                    {
                        templine = (Line)list[i];
                        templine.Offset += offset;
                        if (i == list.Count - 1)
                        {
                            templine.DelimeterLength = line1.DelimeterLength;
                            templine.Length += line1.Offset + line1.Length - offset;
                        }
                        this.labelLines.Insert(index + i, templine);
                    }
                    templine = (Line)list[0];
                    line1.Length = offset + templine.Length - line1.Offset;
                    line1.DelimeterLength = templine.DelimeterLength;
                }
                this.AdaptOffset(index + list.Count, delta);
            }
            else
            {
                this.labelLines.AddRange(list);
            }
            list = null;
            string temp = this.labelText;
            offset = (int)Math.Max(0, Math.Min(offset, temp.Length));
            temp = temp.Insert(offset, str);
            this.ownerElement.InternalSetAttribute("labelText", temp);
            this.labelText = temp;
            temp = null;
            this.labelParsed = old;
        }
        #endregion

        #region ..RemoveString
        public virtual void RemoveString(int offset, int length)
        {
            if (length == 0)
                return;
            bool old = this.labelParsed;
            this.labelParsed = false;
            string temp = this.labelText;
            offset = (int)Math.Max(0, Math.Min(temp.Length - 1, offset));
            length = (int)Math.Max(0, Math.Min(temp.Length - offset, length));
            int index = this.FindLineIndexAtOffset(offset);
            int index1 = this.FindLineIndexAtOffset(offset + length);


            temp = temp.Remove(offset, length);
            Line line = (Line)this.labelLines[index];
            if (offset + length > line.Offset + line.Length)
                line.DelimeterLength = 0;
            if (index == index1)
            {
                line = (Line)this.labelLines[index];
                int l = line.Length;
                line.Length = l - length;
            }
            else
            {
                line = (Line)this.labelLines[index];
                Line line1 = (Line)this.labelLines[index1];
                line.Length = offset - line.Offset + line1.Offset + line1.Length - offset - length;
                line.DelimeterLength = line1.DelimeterLength;
            }
            this.AdaptOffset(index1 + 1, -length);
            for (int i = index + 1; i <= index1; i++)
            {
                line = (this.labelLines[index + 1] as Line);
                this.labelLines.Remove(line);
                line = null;
            }
            this.labelText = temp;
            temp = null;
            this.labelParsed = old;
        }
        #endregion

        #region ..FindLineAtIndex
        public int FindLineIndexAtOffset(int offset)
        {
            for (int i = 0; i < this.labelLines.Count; i++)
            {
                Line line = (Line)this.labelLines[i];
                if (offset >= line.Offset && offset <= line.Offset + line.Length)
                    return i;
            }
            return this.labelLines.Count - 1;
        }

        public Line FindLineAtOffset(int offset)
        {
            int index = this.FindLineIndexAtOffset(offset);
            if (index >= 0 && index < this.labelLines.Count)
                return this.labelLines[index] as Line;
            return null;
        }
        #endregion

        #region ..AdaptOffset
        void AdaptOffset(int index, int delta)
        {
            for (int i = index; i < this.labelLines.Count; i++)
            {
                Line line = (Line)this.labelLines[i];
                int offset = line.Offset;
                line.Offset = offset + delta;
            }
        }
        #endregion

        #region ..FindOffsetAtPoint
        public virtual int FindOffsetAtPoint(PointF p, Graphics g)
        {
            RectangleF rect = this.labelBounds;
            if (this.labelLines.Count > 0)
            {
                try
                {
                    using (System.Drawing.Drawing2D.Matrix matrix = new Matrix())
                    {
                        matrix.RotateAt(this.labelRotate, this.labelCenterPoint);
                        matrix.Invert();
                        PointF[] ps = new PointF[] { p };
                        matrix.TransformPoints(ps);
                        p = ps[0];
                        ps = null;
                        if (!this.vertical)
                        {
                            float lineheight = g.MeasureString("a", this.labelFont, 20000, this.labelStringFormat).Height;
                            float top = rect.Y + rect.Height / 2f - lineheight * this.labelLines.Count / 2f;
                            int lineindex = (int)((p.Y - top) / lineheight);
                            top = top + lineindex * lineheight;
                            lineindex = (int)Math.Max(0, Math.Min(lineindex, this.labelLines.Count - 1));
                            Line line = (Line)this.labelLines[lineindex];

                            string temp = this.GetLineText(line);
                            float left = this.GetBoundsForLine(line.Offset, g).X;
                            return line.Offset + this.FindIndexAtLength(temp, p.X - left, g);
                        }
                        else
                        {
                            float lineheight = g.MeasureString("a", this.labelFont, 20000, this.labelStringFormat).Width;
                            float left = rect.X + rect.Width / 2f - lineheight * this.labelLines.Count / 2f;
                            int lineindex = (int)((p.X - left) / lineheight);
                            left = left + lineindex * lineheight;
                            lineindex = (int)Math.Max(0, Math.Min(lineindex, this.labelLines.Count - 1));
                            Line line = (Line)this.labelLines[lineindex];

                            string temp = this.GetLineText(line);//this.labelText.Substring(offset,length);
                            float width1 = g.MeasureString(temp.Replace("\t", tabString), this.labelFont, 2000000, this.labelStringFormat).Height;
                            float top = rect.Y + rect.Height / 2f - width1 / 2f;
                            return line.Offset + this.FindIndexAtLength(temp, p.Y - top, g);
                        }
                    }
                }
                catch (System.Exception e1)
                {
                    System.Diagnostics.Debug.Assert(true, e1.Message);
                }
            }
            return -1;
        }
        #endregion

        #region ..FindIndexAtLength
        int FindIndexAtLength(string text, float length, Graphics g)
        {
            if (length <= 0)
                return 0;
            for (int i = 1; i <= text.Length; i++)
            {
                string temp = text.Substring(0, i);
                float width = g.MeasureString(temp.Replace("\t", tabString), this.labelFont, 2000000, this.labelStringFormat).Width;
                if (width > length)
                    return i - 1;
                temp = null;
            }
            return text.Length;
        }
        #endregion

        #region ..FindRelativeOffset
        public int FindRelativeOffset(float length, Line line1, bool up, System.Drawing.Graphics g)
        {
            int index = this.labelLines.IndexOf(line1);
            if (index < 0)
                return -1;
            if (up)
                index--;
            else
                index++;
            if (index >= 0 && index < this.labelLines.Count)
            {
                Line line = this.labelLines[index] as Line;
                string temp = this.GetLineText(line);//this.labelText.Substring(line.Offset,line.Length);
                float width1 = g.MeasureString(temp.Replace("\t", tabString), this.labelFont, 2000000, this.labelStringFormat).Width;
                RectangleF rect = this.labelBounds;
                float left = -width1 / 2f;
                int offset1 = this.FindIndexAtLength(temp.Replace("\t", tabString), length - left, g);
                temp = null;
                return line.Offset + offset1;
            }
            return -1;
        }
        #endregion

        #region ..CalculateLengthAtOffset
        /// <summary>
        /// 计算指定索引处的字符里Label中心点的长度
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="g"></param>
        /// <returns></returns>
        public virtual float CalculateLengthAtOffset(int offset, Graphics g)
        {
            int index = this.FindLineIndexAtOffset(offset);
            if (index >= 0 && index < this.labelLines.Count)
            {
                Line line1 = (Line)this.labelLines[index];
                string temp = this.GetLineText(line1);//this.labelText.Substring(line1.Offset,line1.Length);
                RectangleF rect = this.labelBounds;
                PointF p = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
                float width = g.MeasureString(temp.Replace("\t", tabString), this.labelFont, 200000, this.labelStringFormat).Width;
                temp = temp.Substring(0, offset - line1.Offset);
                float width1 = g.MeasureString(temp.Replace("\t", tabString), this.labelFont, 200000, this.labelStringFormat).Width;
                return -width / 2f + width1;
            }
            return float.MaxValue;
        }
        #endregion

        #region ..MoveVertical
        /// <summary>
        /// 获取当前索引上下平移一行时的索引位置
        /// </summary>
        /// <param name="up"></param>
        /// <param name="g"></param>
        /// <param name="offset"></param>
        /// <param name="originalPos"></param>
        /// <returns></returns>
        public virtual int MoveVertical(bool up, Graphics g, int offset, float originalPos)
        {
            SVG.Render.SVGBaseRenderer.Line line = this.FindLineAtOffset(offset);
            return this.FindRelativeOffset(originalPos, line, up, g);
        }
        #endregion

        #region ..GetBoundsForLine
        /// <summary>
        /// get the label bounds for the line 
        /// </summary>
        /// <param name="offset">the index of char</param>
        /// <param name="g">the graphics</param>
        /// <returns>the bounds for part of line</returns>
        private RectangleF GetBoundsForLine(int offset, Graphics g)
        {
            offset = (int)Math.Max(0, Math.Min(offset, this.labelText.Length));
            RectangleF rect = this.labelBounds;
            float lineHeight = 0;
            if (!this.vertical)
                lineHeight = g.MeasureString("a", this.labelFont, 20000, this.labelStringFormat).Height;
            else
                lineHeight = g.MeasureString("a", this.labelFont, 20000, this.labelStringFormat).Width;
            int index = this.FindLineIndexAtOffset(offset);
            float left = rect.X;
            float lineWidth = 0;
            float top = rect.Y;
            if (index >= 0 && index < this.labelLines.Count)
            {
                int count = this.labelLines.Count;
                if (!this.vertical)
                {
                    top = top + index * lineHeight;
                    Line line = (Line)this.labelLines[index];
                    string temp = this.GetLineText(line);
                    float width = 0;
                    float width1 = 0;
                    //calculate the distance
                    if (offset > line.Offset)
                    {
                        string temp1 = temp.Substring(0, offset - line.Offset);
                        width1 = g.MeasureString(temp1.Replace("\t", tabString), this.labelFont, 2000000, this.labelStringFormat).Width;
                    }
                    switch (this.labelAlign)
                    {
                        case Alignment.Left:
                            left = rect.X;
                            break;
                        case Alignment.Right:
                            width = g.MeasureString(temp.Replace("\t", tabString), this.labelFont, 2000000, this.labelStringFormat).Width;
                            left = rect.Right - width;
                            break;
                        default:
                            left = rect.X + rect.Width / 2f;
                            width = g.MeasureString(temp.Replace("\t", tabString), this.labelFont, 2000000, this.labelStringFormat).Width;
                            left = left - width / 2f;
                            break;
                    }
                    lineWidth = width1;
                }
                else
                {
                    left = left + index * lineHeight;
                    Line line = (Line)this.labelLines[index];
                    string temp = this.GetLineText(line);
                    float width = 0;
                    width = g.MeasureString(temp.Replace("\t", tabString), this.labelFont, 2000000, this.labelStringFormat).Height;
                    float width1 = 0;
                    if (offset > line.Offset)
                    {
                        temp = temp.Substring(0, offset - line.Offset);
                        width1 = g.MeasureString(temp.Replace("\t", tabString), this.labelFont, 2000000, this.labelStringFormat).Height;
                    }
                    top = top - width / 2f + width1;
                }
            }
            else
            {
                top = top + rect.Height / 2 - lineHeight / 2f;
            }

            return new RectangleF(left, top, lineWidth, lineHeight);
        }
        #endregion

        #region ..ExportLabelElement
        public virtual YP.SVG.Text.SVGTextElement ExportLabelElement(Graphics g)
        {
            if (this.LabelText.Trim().Length > 0)
            {
                bool old = this.OwnerDocument.AcceptNodeChanged;
                this.OwnerDocument.AcceptNodeChanged = false;
                YP.SVG.Text.SVGTextElement elm = this.OwnerDocument.CreateElement(this.ownerElement.Prefix, "text", this.ownerElement.NamespaceURI) as YP.SVG.Text.SVGTextElement;
                if (this.ownerElement.HasAttribute("font-family"))
                    elm.InternalSetAttribute("font-family", this.ownerElement.GetAttribute("font-family"));
                else
                    elm.InternalSetAttribute("font-family", "SimSun");
                if (this.ownerElement.HasAttribute("font-size"))
                    elm.InternalSetAttribute("font-size", this.ownerElement.GetAttribute("font-size"));
                else
                    elm.InternalSetAttribute("font-size", "12");
                if (this.ownerElement.HasAttribute("font-style"))
                    elm.InternalSetAttribute("font-style", this.ownerElement.GetAttribute("font-style"));
                if (this.ownerElement.HasAttribute("text-decoration"))
                    elm.InternalSetAttribute("text-decoration", this.ownerElement.GetAttribute("text-decoration"));
                if (this.ownerElement.HasAttribute("font-weight"))
                    elm.InternalSetAttribute("font-weight", this.ownerElement.GetAttribute("font-weight"));
                if (this.ownerElement.HasAttribute("writing-mode"))
                    elm.InternalSetAttribute("writing-mode", this.ownerElement.GetAttribute("writing-mode"));

                float x = this.labelBounds.X;
                float y = this.labelBounds.Y;
                PointF[] ps = new PointF[] { new PointF(x + this.labelBounds.Width / 2f, y) };
                if (this.vertical)
                    ps = new PointF[] { new PointF(x, y) };

                using (System.Drawing.Drawing2D.Matrix matrix = this.ownerElement.TotalTransform.Clone())
                {
                    matrix.Invert();
                    matrix.Multiply(this.ownerElement.Transform.FinalMatrix.GetGDIMatrix(), MatrixOrder.Append);
                    matrix.TransformPoints(ps);
                    x = ps[0].X;
                    y = ps[0].Y;
                }
                ps = null;

                float height = this.labelBounds.Height / this.labelLines.Count;

                elm.InternalSetAttribute("fill", YP.SVG.ColorHelper.GetColorStringInHex(this.ownerElement.LabelBrush.Color));
                if (!this.vertical)
                {
                    Line line = (Line)this.labelLines[0];
                    string temp = this.GetLineText(line).Replace("\t", tabString);
                    float width = g.MeasureString(temp, this.labelFont, 2000000, this.labelStringFormat).Width;
                    float x1 = x - width / 2f;
                    elm.InternalSetAttribute("x", x1.ToString());
                    elm.InternalSetAttribute("y", (y + height / 2).ToString());
                    elm.InternalAppendChild(this.OwnerDocument.CreateTextNode(temp));
                    if (this.labelRotate != 0)
                        elm.InternalSetAttribute("transform", "rotate(" + this.labelRotate.ToString() + " " + x.ToString() + " " + y.ToString() + ")");
                    for (int i = 1; i < this.labelLines.Count; i++)
                    {
                        line = (Line)this.labelLines[i];
                        YP.SVG.SVGElement elm1 = this.OwnerDocument.CreateElement(this.ownerElement.Prefix, "tspan", this.ownerElement.NamespaceURI) as YP.SVG.SVGElement;
                        temp = this.GetLineText(line).Replace("\t", tabString); ;
                        width = g.MeasureString(temp, this.labelFont, 2000000, this.labelStringFormat).Width;
                        x1 = x - width / 2f;
                        elm1.InternalSetAttribute("x", x1.ToString());
                        elm1.InternalSetAttribute("dy", height.ToString());
                        if (temp.Length > 0)
                            elm1.InnerXml = temp;
                        elm.InternalAppendChild(elm1);
                    }
                }
                else
                {
                    float width = this.labelBounds.Width / this.labelLines.Count;
                    Line line = (Line)this.labelLines[0];
                    string temp = this.GetLineText(line).Replace("\t", tabString);
                    height = g.MeasureString(temp, this.labelFont, 2000000, this.labelStringFormat).Height;
                    float y1 = y - height / 2f;
                    elm.InternalSetAttribute("x", (x + width / 2).ToString());
                    elm.InternalSetAttribute("y", y1.ToString());
                    elm.InternalAppendChild(this.OwnerDocument.CreateTextNode(temp));
                    if (this.labelRotate != 0)
                        elm.InternalSetAttribute("transform", "rotate(" + this.labelRotate.ToString() + " " + x.ToString() + " " + y.ToString() + ")");
                    for (int i = 1; i < this.labelLines.Count; i++)
                    {
                        line = (Line)this.labelLines[i];
                        YP.SVG.SVGElement elm1 = this.OwnerDocument.CreateElement(this.ownerElement.Prefix, "tspan", this.ownerElement.NamespaceURI) as YP.SVG.SVGElement;
                        temp = this.GetLineText(line).Replace("\t", tabString); ;
                        height = g.MeasureString(temp, this.labelFont, 2000000, this.labelStringFormat).Height;
                        y1 = y - height / 2f;
                        elm1.InternalSetAttribute("dx", width.ToString());
                        elm1.InternalSetAttribute("y", y1.ToString());
                        if (temp.Length > 0)
                            elm1.InnerXml = temp;
                        elm.InternalAppendChild(elm1);
                    }
                }
                elm.InternalSetAttribute("xml:space", "preserve");
                this.ownerElement.InternalRemoveAttribute("labelFont");
                this.ownerElement.InternalRemoveAttribute("labelText");
                this.ownerElement.InternalRemoveAttribute("labelColor");
                this.OwnerDocument.AcceptNodeChanged = old;
                return elm;
            }
            return null;
        }
        #endregion

        #region ..GetRegion
        public virtual RectangleF[] GetRegion(int offset, int length, Graphics g)
        {
            if (length == 0)
                return null;
            int index = this.FindLineIndexAtOffset(offset);
            int index1 = this.FindLineIndexAtOffset(offset + length);
            System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath();
            RectangleF rect = this.labelBounds;
            float height = rect.Height / this.labelLines.Count;
            ArrayList list = new ArrayList();
            for (int i = index; i <= index1; i++)
            {
                Line line = (Line)this.labelLines[i];

                int endIndex = i < index1 ? line.Offset + line.Length : offset + length;
                RectangleF bounds = this.GetBoundsForLine(endIndex, g);
                if (i == index)
                {
                    RectangleF rect1 = this.GetBoundsForLine(offset, g);
                    bounds = new RectangleF(rect1.Right, bounds.Top, bounds.Width - rect1.Width, bounds.Height);
                }
                list.Add(bounds);
            }
            if (list.Count > 0)
            {
                RectangleF[] rects = new RectangleF[list.Count];
                list.CopyTo(rects);
                return rects;
            }
            return null;
        }
        #endregion

        #region ..GetLabelTransform
        public virtual Matrix GetLabelTransform()
        {
            Matrix matrix = new Matrix();
            matrix.RotateAt(this.labelRotate, this.labelCenterPoint);
            return matrix;
        }
        #endregion
        #endregion

        #region ..BackGroundImage
        #region ..private fields
        Bitmap backgroundImageSource = null;
        #endregion

        #region ..properties
        public Bitmap BackgroundImageSource
        {
            set
            {
                this.backgroundImageSource = value;
            }
        }
        #endregion

        #region ..DrawBackgroundImage
        /// <summary>
        /// Draw the background image
        /// </summary>
        public virtual void DrawBackgroundImage(GraphicsPath gp, Graphics g)
        {
            if (gp != null && this.backgroundImageSource != null)
            {
                GraphicsContainer c = g.BeginContainer();
                g.SetClip(gp);
                Rectangle rect = Rectangle.Round(gp.GetBounds());
                g.DrawImage(this.backgroundImageSource, rect);
                g.EndContainer(c);
            }
        }
        #endregion
        #endregion

        #region ..Border
        #region ..private fields
        //border
        Paint.SVGPaint? borderPaint = null;
        #endregion

        #region ..properties
        public Paint.SVGPaint BorderPaint
        {
            set
            {
                this.borderPaint = value;
            }
        }
        #endregion

        #region ..DrawBorder
        /// <summary>
        /// 绘制形状边界
        /// </summary>
        /// <param name="g"></param>
        /// <param name="path"></param>
        public virtual void DrawBorder(Graphics g, GraphicsPath path, float strokeWidth)
        {
            //if (this.Cache.CachePen != null)
            //{
            //    float strokeWidth = this.Cache.CachePen.Width;
                if (strokeWidth > 1 && borderPaint.HasValue)
                {
                    using (Pen pen = new Pen(borderPaint.Value.RgbColor.GDIColor, strokeWidth))
                    {
                        using (GraphicsPath tempp = path.Clone() as GraphicsPath)
                        {
                            tempp.Widen(pen);
                            pen.Width = 1;
                            pen.LineJoin = LineJoin.Round;
                            g.DrawPath(pen, tempp);
                        }
                    }
                }
            //}
        }
        #endregion
        #endregion

        #region ..CloneRender
        public virtual void CloneRender(SVGBaseRenderer render)
        {
            render.labelBounds = this.labelBounds;
            render.labelLines = this.labelLines;
            render.labelText = this.LabelText;
            render.labelRotate = this.labelRotate;
            render.labelFont = this.labelFont;
            if (this.vertical)
                render.vertical = this.vertical;
        }
        #endregion

        #region ..Connection
        #region ..fields
        public PointF[] connectionPoints = { };
        #endregion

        public virtual PointF[] ConnectionPoints
        {
            get
            {
                return connectionPoints;
            }
        }
        #endregion

        #region ..Draw
        public virtual void Draw(Graphics g, StyleContainer.StyleOperator sp)
        {
           
        }
        #endregion

        #region ..BeforeDrawing
        public virtual bool BeforeDrawing(Graphics g, StyleContainer.StyleOperator sp)
        {
            return this.ownerElement.BeforeDrawing(g,sp);
        }
        #endregion

        #region ..UpdateTransform
        /// <summary>
        /// 当对应的图元TotalTransform发生改变时，更新对应Renderer的信息
        /// </summary>
        public virtual void UpdateTotalTransform()
        {
        }
        #endregion
    }
}
