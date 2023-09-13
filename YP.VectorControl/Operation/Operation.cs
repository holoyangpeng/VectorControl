using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;
using YP.VectorControl.Forms;

namespace YP.VectorControl.Operation
{
	/// <summary>
	/// 定义各种不同操作方式下的鼠标操作的抽象基类
	/// </summary>
	internal abstract class Operation:DisposeBase
	{
		#region ..构造及消除
		internal Operation(Canvas mousearea)
		{
			this.mouseArea = mousearea;
			this.reversePath.FillMode = FillMode.Alternate;
			this.AttachMouseEvent();
			this.widepen = new Pen(Color.White,this.mouseArea.grapSize);
            this.reversePen = new Pen(this.mouseArea.SelectedPen.Color, 1);
            this.reverseConnectorIndicatorPen = new Pen(this.mouseArea.HighlightAnchor, ConnectAnchorWidth);
            this.reverseSnapIndicatorPen = new Pen(this.mouseArea.HighlightAnchor, 1);
            this.reverseSnapIndicatorPen.DashPattern = new float[] { 3, 3, 3 };
		}

		/// <summary>
		/// 消除次对象
		/// </summary>
		public override void Dispose()
		{
			if(this.mouseArea != null)
			{
				this.mouseArea.MouseDown -= new MouseEventHandler(this.OnMouseDown);
				this.mouseArea.MouseUp -= new MouseEventHandler(this.OnMouseUp);
				this.mouseArea.MouseMove -= new MouseEventHandler(this.OnMouseMove);
                this.mouseArea.MouseLeave -= new EventHandler(mouseArea_MouseLeave);
				this.mouseArea.Paint -= new PaintEventHandler(this.OnPaint);
			}
            this.reverseConnectorIndicatorPen.Dispose();
            this.reversePen.Dispose();
            this.reverseSnapIndicatorPen.Dispose();
			if(this.widepen != null)
				this.widepen.Dispose();
			this.mouseArea.firstPoint = Point.Empty;
			this.disposed = true;
			this.mouseArea.validContent = true;
			base.Dispose();
		}
		#endregion

        #region ..const fields
        protected const float ConnectAnchorWidth = 3;
        protected const float SnapIndicatorExtensionLength = 20;
        float[] reverseDashArray = new float[] { 3, 3, 3 };
        #endregion

        #region ..继承变量
        protected Canvas mouseArea = null;
		protected bool mousedown = false;
		protected bool attach = false;
		protected GraphicsPath reversePath = new GraphicsPath();
		protected GraphicsPath reverseFillPath = new GraphicsPath();
        protected GraphicsPath reverseSnapIndicatorPath = new GraphicsPath();
		protected GraphicsPath reverseConnetorIndicatorPath = new GraphicsPath();
		protected bool useDash = true;
		protected SolidBrush reverseFillBrush = new SolidBrush(Color.DodgerBlue);
        protected Pen reverseConnectorIndicatorPen = null;
        protected Pen reversePen = null;
        protected Pen reverseSnapIndicatorPen = null;
		internal bool Finish = true;
		protected PointF segPoint = Point.Empty;
		protected bool disposed = false;
		protected bool mustMouseDown = true;
		Pen widepen;
		#endregion

		#region ..属性
        protected bool NeedAlignToElement
        {
            get
            {
                return (this.mouseArea.VisualAlignment & VisualAlignment.Element) == VisualAlignment.Element;
            }
        }

        /// <summary>
        /// 是否默认对齐Grid
        /// </summary>
        internal virtual bool NeedAlignToGrid
        {
            get
            {
                return false;
            }
        }

		/// <summary>
		/// 判断当前文档是否有效
		/// </summary>
		protected bool IsValidDocument
		{
			get
			{
				if(this.disposed)
					return false;
				bool valid = this.mouseArea.SVGDocument != null && this.mouseArea.SVGDocument.IsValid;
				if(this.mouseArea.CurrentOperator == Operator.Shape)
					valid = valid && this.mouseArea.templateShape != null ;
				
				if(!valid)
					this.mouseArea.Cursor = System.Windows.Forms.Cursors.No;
//				else
//					this.mouseArea.Cursor = this.mouseArea.DefaultCursor;
				return valid;
			}
		}

		internal virtual bool Disposed
		{
			get
			{
				return this.disposed;
			}
		}

		internal virtual bool EditText
		{
			get
			{
				return false;
			}
		}
		#endregion

		#region ..绑定事件
		protected void AttachMouseEvent()
		{
			if(this.mouseArea != null && !this.attach)
			{
				this.mouseArea.MouseDown += new MouseEventHandler(this.OnMouseDown);
				this.mouseArea.MouseUp += new MouseEventHandler(this.OnMouseUp);
				this.mouseArea.MouseMove += new MouseEventHandler(this.OnMouseMove);
                this.mouseArea.MouseLeave += new EventHandler(mouseArea_MouseLeave);
				this.mouseArea.Paint += new PaintEventHandler(this.OnPaint);
			}
			this.attach = true;
		}

        protected virtual void mouseArea_MouseLeave(object sender, EventArgs e)
        {
            GraphicsPath[] pathes = { this.reversePath, this.reverseSnapIndicatorPath, this.reverseConnetorIndicatorPath };
            foreach (GraphicsPath path in pathes)
            {
                if (path != null)
                {
                    this.XORDrawPath(this.reversePath);
                    path.Reset();
                }
            }
        }
		#endregion 

		#region ..鼠标事件
		/// <summary>
		/// OnMouseDown
		/// </summary>
		/// <param name="e"></param>
		protected abstract void OnMouseDown(object sender,MouseEventArgs e);

		/// <summary>
		/// OnMouseMove
		/// </summary>
		/// <param name="e"></param>
		protected abstract void OnMouseMove(object sender,MouseEventArgs e);

		/// <summary>
		/// OnMouseUp
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnMouseUp(object sender,MouseEventArgs e)
		{
			this.Finish = true;
		}

		#endregion

		#region ..绘制事件
		/// <summary>
		/// OnPaint
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected abstract void OnPaint(object sender,PaintEventArgs e);
		
		protected void DrawXorPath(PaintEventArgs e)
		{
			if(this.mouseArea.SVGDocument == null || this.mouseArea.CurrentScene == null)
				return;
			if((!this.mousedown || Control.MouseButtons == MouseButtons.None) && !OperatorHelper.IsPieOperator(this.mouseArea.CurrentOperator) && this.mouseArea.CurrentOperator != Operator.Image && this.mustMouseDown &&!(this is DragDropEventHandler))
			{
				this.reversePath.Reset();
				this.reverseFillPath.Reset();
                this.reverseConnetorIndicatorPath.Reset();
				return;
			}
			if(!this.mouseArea.validContent)
			{
				try
				{
                    if (this.reverseFillPath.PointCount > 1)
                    {
                        this.reverseFillPath.FillMode = FillMode.Winding;

                        e.Graphics.FillPath(reverseFillBrush, this.reverseFillPath);
                    }

					if(this.reversePath.PointCount > 1)
					{
                        e.Graphics.DrawPath(this.reversePen,this.reversePath);
					}
					if(this.reverseConnetorIndicatorPath.PointCount > 1)
					{
                        e.Graphics.DrawPath(reverseConnectorIndicatorPen, this.reverseConnetorIndicatorPath);
					}
                    if (this.reverseSnapIndicatorPath.PointCount > 1)
                    {
                        e.Graphics.DrawPath(reverseSnapIndicatorPen, this.reverseSnapIndicatorPath);
                    }
				}
				catch
				{
					
				}
			}
		}
		#endregion

		#region ..改变属性事件
		protected abstract void OnAdaptAttribute(object sender,AdaptAttributeEventArgs e);
		#endregion

		#region ..键盘事件
		public virtual bool ProcessDialogKey(Keys keyData)
		{
			return false;
		}
		#endregion

		#region ..异或绘图
        protected void XORDrawPath(GraphicsPath path)
        {
            this.XORDrawPath(path, false);
        }
		protected void XORDrawPath(GraphicsPath path, bool validContent)
		{	
			if(path!= null && path.PointCount > 1)
			{
				path.FillMode = FillMode.Winding;
				RectangleF rect = path.GetBounds();
                this.mouseArea.validContent = validContent;
				int r = this.mouseArea.grapSize;
				Rectangle rect1 = new Rectangle((int)rect.X - r,(int)rect.Y - r,(int)rect.Width + 2 * r,(int)rect.Height +2 * r);
				if(!rect1.IsEmpty)
					this.mouseArea.Invalidate(rect1);
			}
			
		}
		#endregion

		#region ..重置
		/// <summary>
		/// 重置
		/// </summary>
		internal virtual void Reset()
		{
		}
		#endregion

		#region ..获取两点之间的倾斜角
		/// <summary>
		/// 获取两点之间的切线角
		/// </summary>
		/// <returns></returns>
		protected float GetAngle(PointF startpoint,PointF endpoint)
		{
            return SVG.PathHelper.GetAngle(startpoint, endpoint);
		}
		#endregion

		#region ..InPoint
		/// <summary>
		/// 判断两点是否重合
		/// </summary>
		/// <param name="p1">第一个点</param>
		/// <param name="p2">第二个点</param>
		/// <returns>两点是否重合</returns>
		protected virtual bool InPoint(PointF p1,PointF p2)
		{
			double d = Math.Sqrt(Math.Pow(p2.X - p1.X,2) + Math.Pow(p2.Y - p1.Y,2));
			return d<4;
		}

		/// <summary>
		/// 判断两点是否重合
		/// </summary>
		/// <param name="p1">第一个点</param>
		/// <param name="p2">第二个点</param>
		/// <returns>两点是否重合</returns>
		protected virtual bool InPoint(PointF p1,PointF p2,float delta)
		{
			double d = Math.Sqrt(Math.Pow(p2.X - p1.X,2) + Math.Pow(p2.Y - p1.Y,2));
			return d<delta;
		}
		#endregion

		#region ..重置颜色
		protected virtual void ResetColor()
		{
            this.reversePen.Color = this.reverseFillBrush.Color = this.mouseArea.SelectedPen.Color;
            this.reverseConnectorIndicatorPen.Color = this.mouseArea.HighlightAnchor;
		}
		#endregion

		#region ..寻找连接目标
        protected SVG.Interface.ISVGPathable GetConnectTarget(PointF p, ref PointF cntPoint, ref int connectIndex, ConnectionTargetType type)
        {
            RectangleF bounds = RectangleF.Empty;
            return GetConnectTarget(p, ref cntPoint, ref bounds, ref connectIndex, type, null);
        }

        protected SVG.Interface.ISVGPathable GetConnectTarget(PointF p, ref PointF cntPoint, ref RectangleF bounds, ref int connectIndex, ConnectionTargetType type)
        {
            return GetConnectTarget(p, ref cntPoint, ref bounds, ref connectIndex, type, null);
        }

        protected SVG.Interface.ISVGPathable GetConnectTarget(PointF p, ref PointF cntPoint, ref int connectIndex, ConnectionTargetType type, SVG.BasicShapes.SVGConnectionElement connectElement)
        {
            RectangleF bounds = RectangleF.Empty;
            return GetConnectTarget(p, ref cntPoint, ref bounds, ref connectIndex, type, connectElement);
        }

		/// <summary>
		/// 查找指定位置上的可以连接的对象
		/// </summary>
		/// <param name="p">工作区的坐标位置</param>
		/// <param name="cntPoint">如果存在连接对象，记录其连接位置，工作区坐标系</param>
		/// <returns></returns>
		protected SVG.Interface.ISVGPathable GetConnectTarget(PointF p,ref PointF cntPoint, ref RectangleF bounds, ref int connectIndex, ConnectionTargetType type,SVG.BasicShapes.SVGConnectionElement connectElement)
		{
			SVG.SVGElementCollection list = this.mouseArea.connectableElements;
            connectIndex = -1;
			for(int i = list.Count - 1;i>=0;i--)
			{
				SVG.SVGTransformableElement render = list[i] as SVG.SVGTransformableElement;
                SVG.Interface.ISVGPathable pathable = render as SVG.Interface.ISVGPathable;
				if(render != null && pathable != null)
				{
                    if (render.OwnerSvgElement == null)
                    {
                        list.Remove(render);
                        i++;
                    }
					PointF[] ps = render.ConnectionPoints;
					if(ps != null)
					{
						ps = ps.Clone() as PointF[];
                        using (Matrix matrix = this.mouseArea.GetTotalTransformForElement(render))
                        {
                            using (GraphicsPath path = pathable.GPath.Clone() as GraphicsPath)
                            {
                                path.Transform(matrix);
                                if (ps.Length > 0)
                                {
                                    matrix.TransformPoints(ps);
                                    for (int j = 0; j < ps.Length; j++)
                                    {
                                        if (this.InPoint(p, ps[j], 6))
                                        {
                                            cntPoint = ps[j];
                                            //if the type is not none,
                                            if (type != ConnectionTargetType.None)
                                            {
                                                if (!this.mouseArea.TrytoConnectElement(render, j, ps.Length ,type, connectElement == null ? null : connectElement))
                                                    continue;
                                            }
                                            connectIndex = j;
                                            return pathable;
                                        }
                                    }
                                }
                                if (path.IsVisible(p))
                                {
                                    //if the type is not none,
                                    if (type != ConnectionTargetType.None)
                                    {
                                        if (!this.mouseArea.TrytoConnectElement(render, -1, ps.Length, type, connectElement == null ? null : connectElement))
                                            continue;
                                    }
                                    bounds = path.GetBounds();
                                    return pathable;
                                }
                            }
                        }
					}
				}
			}
			return null;
		}
		#endregion

		#region ..更新连接线
		protected void UpdateConnects(SVG.SVGTransformableElement element)
		{
			//this.mouseArea.InvalidateConnects(element);
		}
		#endregion

		#region ..ShiftSnap
		internal virtual bool ShiftSnap
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region ..Invalidate
		internal virtual void Invalidate()
		{
		}
		#endregion

        #region ..ExecuteBehaviorPresent
        internal virtual bool ExecuteBehaviorPresent(Behavior behavior)
		{
			return true;
		}
		#endregion

        #region ..AlignToElement
        protected enum ElementAlign
        {
            None = 0,
            Top = 1,
            Bottom = 2,
            Middle = 4,
            Left = 8,
            Right = 16,
            Center = 32,
            All = 63
        }

        protected struct AlignResult
        {
            public ElementAlign SourcePos;
            public ElementAlign DestPos;
            public bool Horizontal;
            public float Value;
            public float Delta;
            public SVG.SVGElement AlignElement;

            public AlignResult(ElementAlign source, ElementAlign dest, bool hori, float value, float delta)
            {
                SourcePos = source;
                DestPos = dest;
                Horizontal = hori;
                Value = value;
                this.Delta = delta;
                AlignElement = null;
            }
        }

        protected AlignResult[] AlignToElement(GraphicsPath alignPath, GraphicsPath basePath, ElementAlign align)
        {
            return AlignToElement(alignPath, basePath, align, null,true, true,true);
        }

        protected AlignResult[] AlignToElement(GraphicsPath alignPath, GraphicsPath basePath, ElementAlign align, SVG.SVGElement[] excludeElement, bool snapGrid, bool snapGuide)
        {
            return this.AlignToElement(alignPath, basePath, align, excludeElement, true, snapGrid, snapGuide);
        }

        protected AlignResult[] AlignToElement(GraphicsPath alignPath, GraphicsPath basePath, ElementAlign align, SVG.SVGElement[] excludeElements,bool snapElement, bool snapGrid, bool snapGuide)
        {
            if (align == ElementAlign.None || basePath == null)
                return null;
            List<AlignResult> finalResults = new List<AlignResult>();
            AlignResult? xResult = null, yResult = null;
            if (basePath.PointCount > 1 && this.NeedAlignToElement && snapElement && this.mouseArea.renderElements.Count < Canvas.MaxCalculateElements)
            {
                SVG.SVGElement excludeElement = null;
                if (excludeElements != null && excludeElements.Length > 0)
                    excludeElement = excludeElements[0];
                RectangleF bounds = basePath.GetBounds();
                if (bounds.Height > 0 || bounds.Width > 0)
                {
                    alignPath.Reset();
                    
                    float? x = null, y = null;
                    //水平
                    if ((align & ElementAlign.Center) != ElementAlign.Center && (align & ElementAlign.Left) != ElementAlign.Left && (align & ElementAlign.Right) != ElementAlign.Right)
                        x = 0;
                    if ((align & ElementAlign.Middle) != ElementAlign.Middle && (align & ElementAlign.Top) != ElementAlign.Top && (align & ElementAlign.Bottom) != ElementAlign.Bottom)
                        y = 0;
                    SVG.SVGTransformableElement xElement = null, yElement = null;
                    GraphicsPath xPath = new GraphicsPath(), yPath = new GraphicsPath();
                    using (Matrix coordTransform = this.mouseArea.CoordTransform.Clone())
                    {
                        using (GraphicsPath path = new GraphicsPath())
                        {
                            foreach (SVG.SVGTransformableElement elm in this.mouseArea.renderElements)
                            {
                                SVG.Interface.ISVGPathable pathable = elm as SVG.Interface.ISVGPathable;
                                if (elm == excludeElement || pathable == null || pathable.GPath == null || pathable.GPath.PointCount < 2 || IsChildOfElement(elm, excludeElement))
                                    continue;
                                float? xTemp = null, yTemp = null;
                                GraphicsPath xTempPath = new GraphicsPath(), yTempPath = new GraphicsPath();
                                path.Reset();
                                path.AddPath(pathable.GPath, false);
                                path.Transform(elm.TotalTransform);
                                path.Transform(coordTransform);

                                bool xCenter = false, yMiddle = false;
                                RectangleF rect = path.GetBounds();
                                //middle
                                if ((align & ElementAlign.Middle) == ElementAlign.Middle)// && Math.Abs((rect.Bottom + rect.Top) / 2 - (bounds.Bottom + bounds.Top) / 2) < Canvas.SnapMargin)
                                {
                                    float[] targets = new float[] { rect.Top + rect.Height / 2};
                                    ElementAlign[] aligns = { ElementAlign.Middle};
                                    for (int i = 0; i < targets.Length; i++)
                                    {
                                        float target = targets[i];
                                        float middle = (bounds.Bottom + bounds.Top) / 2;
                                        if (Math.Abs(target - middle) < Canvas.SnapMargin)
                                        {
                                            float delta = target - middle;
                                            if (delta < 1)
                                                continue;
                                            float min = rect.Left < bounds.Left ? rect.Left : bounds.Left;
                                            float max = rect.Right < bounds.Right ? bounds.Right : rect.Right;
                                            if (!yTemp.HasValue)
                                                yTemp = delta;
                                            if (yTemp.Value >= delta)
                                            {
                                                if (yTemp.Value > delta)
                                                    yTempPath.Reset();
                                                yTempPath.StartFigure();
                                                yTempPath.AddLine(min - SnapIndicatorExtensionLength, target, max + SnapIndicatorExtensionLength, target);
                                                yTemp = delta;
                                                yResult = new AlignResult(ElementAlign.Middle, aligns[i], false, target, delta);
                                                //如果是中心对齐，判断左右是否对齐
                                                if (i == 0)
                                                {
                                                    yMiddle = true;

                                                    if (bounds.Top + delta == rect.Top)
                                                    {
                                                        yTempPath.StartFigure();
                                                        yTempPath.AddLine(min - SnapIndicatorExtensionLength, rect.Top, max + SnapIndicatorExtensionLength, rect.Top);

                                                        yTempPath.StartFigure();
                                                        yTempPath.AddLine(min - SnapIndicatorExtensionLength, rect.Bottom, max + SnapIndicatorExtensionLength, rect.Bottom);
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }

                                //center
                                if ((align & ElementAlign.Center) == ElementAlign.Center)// && Math.Abs((rect.Left + rect.Right) / 2 - (bounds.Left + bounds.Right) / 2) < Canvas.SnapMargin)
                                {
                                    float[] targets = new float[] { rect.Left + rect.Width / 2};
                                    ElementAlign[] aligns = { ElementAlign.Center };
                                    for (int i = 0; i < targets.Length; i++)
                                    {
                                        float target = targets[i];
                                        float center = (bounds.Left + bounds.Right) / 2;
                                        if (Math.Abs(target - center) < Canvas.SnapMargin)
                                        {
                                            float delta = target - center;
                                            if (delta < 1)
                                                continue;
                                            float min = rect.Top < bounds.Top ? rect.Top : bounds.Top;
                                            float max = rect.Bottom < bounds.Bottom ? bounds.Bottom : rect.Bottom;
                                            if (!xTemp.HasValue)
                                                xTemp = delta;
                                            if (xTemp.Value >= delta)
                                            {
                                                if (xTemp.Value > delta)
                                                    xTempPath.Reset();
                                                xTempPath.StartFigure();
                                                xTempPath.AddLine(target, min - SnapIndicatorExtensionLength, target, max + SnapIndicatorExtensionLength);
                                                xTemp = delta;
                                                xResult = new AlignResult(ElementAlign.Center, aligns[i], true, target, delta);

                                                //如果是中心对齐，判断左右是否对齐
                                                if (i == 0)
                                                {
                                                    xCenter = true;

                                                    if (bounds.Left + delta == rect.Left)
                                                    {
                                                        xTempPath.StartFigure();
                                                        xTempPath.AddLine(rect.Left, min - SnapIndicatorExtensionLength, rect.Left, max + SnapIndicatorExtensionLength);

                                                        xTempPath.StartFigure();
                                                        xTempPath.AddLine(rect.Right, min - SnapIndicatorExtensionLength, rect.Right, max + SnapIndicatorExtensionLength);
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }

                                if (!xCenter)
                                {
                                    //left margin
                                    if ((align & ElementAlign.Left) == ElementAlign.Left)
                                    {
                                        float[] targets = new float[] { rect.Left, };
                                        ElementAlign[] aligns = { ElementAlign.Left };
                                        for (int i = 0; i < targets.Length; i++)
                                        {
                                            float target = targets[i];
                                            if (Math.Abs(target - bounds.Left) < Canvas.SnapMargin)
                                            {
                                                float delta = target - bounds.Left;
                                                if (delta < 1)
                                                    continue;
                                                float min = rect.Top < bounds.Top ? rect.Top : bounds.Top;
                                                float max = rect.Bottom < bounds.Bottom ? bounds.Bottom : rect.Bottom;
                                                if (!xTemp.HasValue)
                                                    xTemp = delta;
                                                if (xTemp.Value >= delta)
                                                {
                                                    if (xTemp.Value > delta)
                                                        xTempPath.Reset();
                                                    xTempPath.StartFigure();
                                                    xTempPath.AddLine(target, min - SnapIndicatorExtensionLength, target, max + SnapIndicatorExtensionLength);
                                                    xTemp = delta;
                                                    xResult = new AlignResult(ElementAlign.Left, aligns[i], true, target, delta);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    //right
                                    if ((align & ElementAlign.Right) == ElementAlign.Right)
                                    {
                                        float[] targets = new float[] { rect.Right};
                                        ElementAlign[] aligns = { ElementAlign.Right };
                                        for (int i = 0; i < targets.Length; i++)
                                        {
                                            float target = targets[i];
                                            if (Math.Abs(target - bounds.Right) < Canvas.SnapMargin)
                                            {
                                                float delta = target - bounds.Right;
                                                if (delta < 1)
                                                    continue;
                                                float min = rect.Top < bounds.Top ? rect.Top : bounds.Top;
                                                float max = rect.Bottom < bounds.Bottom ? bounds.Bottom : rect.Bottom;
                                                if (!xTemp.HasValue)
                                                    xTemp = delta;
                                                if (xTemp.Value >= delta)
                                                {
                                                    if (xTemp.Value > delta)
                                                        xTempPath.Reset();
                                                    xTempPath.StartFigure();
                                                    xTempPath.AddLine(target, min - SnapIndicatorExtensionLength, target, max + SnapIndicatorExtensionLength);
                                                    xTemp = delta;
                                                    xResult = new AlignResult(ElementAlign.Right, aligns[i], true, target, delta);
                                                }

                                                break;
                                            }
                                        }
                                    }
                                }

                                if (!yMiddle)
                                {
                                    //top
                                    if ((align & ElementAlign.Top) == ElementAlign.Top)
                                    {
                                        float[] targets = new float[] { rect.Top };
                                        ElementAlign[] aligns = { ElementAlign.Top};
                                        for (int i = 0; i < targets.Length; i++)
                                        {
                                            float target = targets[i];
                                            if (Math.Abs(target - bounds.Top) < Canvas.SnapMargin)
                                            {
                                                float delta = target - bounds.Top;
                                                float min = rect.Left < bounds.Left ? rect.Left : bounds.Left;
                                                float max = rect.Right < bounds.Right ? bounds.Right : rect.Right;
                                                if (!yTemp.HasValue)
                                                    yTemp = delta;
                                                if (yTemp.Value >= delta)
                                                {
                                                    if (yTemp.Value > delta)
                                                        yTempPath.Reset();
                                                    yTempPath.StartFigure();
                                                    yTempPath.AddLine(min - SnapIndicatorExtensionLength, target, max + SnapIndicatorExtensionLength, target);
                                                    yTemp = delta;
                                                    yResult = new AlignResult(ElementAlign.Top, aligns[i], false, target, delta);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                    //bottom
                                    if ((align & ElementAlign.Bottom) == ElementAlign.Bottom)//&& Math.Abs(rect.Bottom - bounds.Bottom) < Canvas.SnapMargin)
                                    {
                                        float[] targets = new float[] { rect.Bottom };
                                        ElementAlign[] aligns = { ElementAlign.Bottom };
                                        for (int i = 0; i < targets.Length; i++)
                                        {
                                            float target = targets[i];
                                            if (Math.Abs(target - bounds.Bottom) < Canvas.SnapMargin)
                                            {
                                                float delta = target - bounds.Bottom;
                                                float min = rect.Left < bounds.Left ? rect.Left : bounds.Left;
                                                float max = rect.Right < bounds.Right ? bounds.Right : rect.Right;
                                                if (!yTemp.HasValue)
                                                    yTemp = delta;
                                                if (yTemp.Value >= delta)
                                                {
                                                    if (yTemp.Value > delta)
                                                        yTempPath.Reset();
                                                    yTempPath.StartFigure();
                                                    yTempPath.AddLine(min - SnapIndicatorExtensionLength, target, max + SnapIndicatorExtensionLength, target);
                                                    yTemp = delta;
                                                    yResult = new AlignResult(ElementAlign.Bottom, aligns[i], false, target, delta);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }

                                //取小值
                                if (xTemp.HasValue)
                                {
                                    if (!x.HasValue)
                                        x = xTemp.Value;
                                    if (x.Value >= xTemp.Value)
                                    {
                                        xElement = elm;
                                        xPath.Reset();
                                        xPath.AddPath(xTempPath, false);
                                    }
                                }

                                if (yTemp.HasValue)
                                {
                                    if (!y.HasValue)
                                        y = yTemp.Value;
                                    if (y.Value >= yTemp.Value)
                                    {
                                        yElement = elm;
                                        yPath.Reset();
                                        yPath.AddPath(yTempPath, false);
                                    }
                                }
                            }
                        }
                    }
                    if (xPath.PointCount > 1)
                    {
                        alignPath.StartFigure();
                        alignPath.AddPath(xPath, false);
                    }

                    if (yPath.PointCount > 1)
                    {
                        alignPath.StartFigure();
                        alignPath.AddPath(yPath, false);
                    }
                }
            }

            if (snapGuide && (!xResult.HasValue || !yResult.HasValue))
                this.AlignElementToGuide(basePath, align, ref xResult, ref yResult);

            if (snapGrid && (!xResult.HasValue || !yResult.HasValue))
                this.AlignElementToGrid(basePath, align, ref xResult, ref yResult);

            if (xResult.HasValue)
                finalResults.Add(xResult.Value);
            if (yResult.HasValue)
                finalResults.Add(yResult.Value);

            if (finalResults.Count > 0)
                return finalResults.ToArray();

            return null;
        }
        #endregion

        #region ..AlignToGrid
        /// <summary>
        /// 对齐对象到网格
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="align"></param>
        /// <returns></returns>
        protected void AlignElementToGrid(GraphicsPath basePath, ElementAlign align, ref AlignResult? xResult, ref AlignResult? yResult)
        {
            if ((this.mouseArea.VisualAlignment & VisualAlignment.Grid) == VisualAlignment.Grid)
            {
                if (align == ElementAlign.None || basePath == null)
                    return;
                if (basePath.PointCount > 1)
                {
                    RectangleF bounds = basePath.GetBounds();
                    if (bounds.Width > 1 && bounds.Height > 0)
                    {
                        List<float> xValues = new List<float>();
                        List<float> yValues = new List<float>();

                        List<ElementAlign> xAligns = new List<ElementAlign>();
                        List<ElementAlign> yAligns = new List<ElementAlign>();
                        if ((align & ElementAlign.Left) == ElementAlign.Left)
                        {
                            xValues.Add(bounds.Left);
                            xAligns.Add(ElementAlign.Left);
                        }

                        if ((align & ElementAlign.Right) == ElementAlign.Right)
                        {
                            xAligns.Add(ElementAlign.Right);
                            xValues.Add(bounds.Right);
                        }

                        if ((align & ElementAlign.Top) == ElementAlign.Top)
                        {
                            yAligns.Add(ElementAlign.Top);
                            yValues.Add(bounds.Top);
                        }
                        if ((align & ElementAlign.Bottom) == ElementAlign.Bottom)
                        {
                            yAligns.Add(ElementAlign.Bottom);
                            yValues.Add(bounds.Bottom);
                        }

                        float? xValue = null, yValue = null;
                        if (!xResult.HasValue)
                        {
                            for (int i = 0; i < xValues.Count; i++)
                            {
                                float x = xValues[i];
                                PointF p = new PointF(x, 0);
                                PointF p1 = this.mouseArea.SnapPointToGrid(p,true,false);
                                float delta = p1.X - p.X;
                                if (delta == 0)
                                    continue;
                                if (!xValue.HasValue)
                                    xValue = delta;

                                if (Math.Abs(xValue.Value) >= Math.Abs(delta))
                                {
                                    xValue = delta;
                                    ElementAlign align1 = xAligns[i];
                                    xResult = new AlignResult(align1, align1, true, p1.X, delta);
                                }
                            }
                        }

                        if (!yResult.HasValue)
                        {
                            for (int i = 0; i < yValues.Count; i++)
                            {
                                float y = yValues[i];
                                PointF p = new PointF(0, y);
                                PointF p1 = this.mouseArea.SnapPointToGrid(p,false,true);
                                float delta = p1.Y - p.Y;
                                if (delta == 0)
                                    continue;
                                if (!yValue.HasValue)
                                    yValue = delta;

                                if (Math.Abs(yValue.Value) >= Math.Abs(delta))
                                {
                                    yValue = delta;
                                    ElementAlign align1 = yAligns[i];
                                    yResult = new AlignResult(align1, align1, false, p1.Y, delta);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region ..AlignElementToGuide
        /// <summary>
        /// 对齐对象到参考线
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="align"></param>
        /// <returns></returns>
        protected void AlignElementToGuide(GraphicsPath basePath, ElementAlign align, ref AlignResult? xResult, ref AlignResult? yResult)
        {
            if ((this.mouseArea.VisualAlignment & VisualAlignment.Guide) == VisualAlignment.Guide)
            {
                if (align == ElementAlign.None || basePath == null)
                    return;
                if (basePath.PointCount > 1)
                {
                    RectangleF bounds = basePath.GetBounds();
                    if (bounds.Width > 1 && bounds.Height > 0)
                    {
                        List<float> xValues = new List<float>();
                        List<float> yValues = new List<float>();

                        List<ElementAlign> xAligns = new List<ElementAlign>();
                        List<ElementAlign> yAligns = new List<ElementAlign>();
                        if ((align & ElementAlign.Left) == ElementAlign.Left)
                        {
                            xValues.Add(bounds.Left);
                            xAligns.Add(ElementAlign.Left);
                        }

                        if ((align & ElementAlign.Right) == ElementAlign.Right)
                        {
                            xAligns.Add(ElementAlign.Right);
                            xValues.Add(bounds.Right);
                        }

                        if ((align & ElementAlign.Top) == ElementAlign.Top)
                        {
                            yAligns.Add(ElementAlign.Top);
                            yValues.Add(bounds.Top);
                        }
                        if ((align & ElementAlign.Bottom) == ElementAlign.Bottom)
                        {
                            yAligns.Add(ElementAlign.Bottom);
                            yValues.Add(bounds.Bottom);
                        }

                        float? xValue = null, yValue = null;
                        if (!xResult.HasValue && this.mouseArea.vGuides.Count > 0)
                        {
                            for (int i = 0; i < xValues.Count; i++)
                            {
                                float x = xValues[i];
                                PointF p = new PointF(x, 0);
                                PointF p1 = p;
                                var result = this.mouseArea.SnapPointToGuide(ref p1,true,false);
                                float delta = p1.X - p.X;
                                if (result == GuideResult.None)
                                    continue;
                                if (!xValue.HasValue)
                                    xValue = delta;

                                if (Math.Abs(xValue.Value) >= Math.Abs(delta))
                                {
                                    xValue = delta;
                                    ElementAlign align1 = xAligns[i];
                                    xResult = new AlignResult(align1, align1, true, p1.X, delta);
                                }
                            }
                        }

                        if (!yResult.HasValue && this.mouseArea.hGuides.Count > 0)
                        {
                            for (int i = 0; i < yValues.Count; i++)
                            {
                                float y = yValues[i];
                                PointF p = new PointF(0, y);
                                PointF p1 = p;
                                var result = this.mouseArea.SnapPointToGuide(ref p1,false, true);
                                float delta = p1.Y - p.Y;
                                if (result ==  GuideResult.None)
                                    continue;
                                if (!yValue.HasValue)
                                    yValue = delta;

                                if (Math.Abs(yValue.Value) >= Math.Abs(delta))
                                {
                                    yValue = delta;
                                    ElementAlign align1 = yAligns[i];
                                    yResult = new AlignResult(align1, align1, false, p1.Y, delta);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region ..IsChildOfElement
        bool IsChildOfElement(SVG.SVGElement child, SVG.SVGElement parent)
        {
            if (parent == null || child == null || !(parent is SVG.Interface.ISVGContainer))
                return false;

            if (child.ParentElement == parent)
                return true;
            return IsChildOfElement(child.ParentElement, parent);
        }
        #endregion
    }
}
