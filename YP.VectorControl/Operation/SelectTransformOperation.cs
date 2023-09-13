using System;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using YP.VectorControl.Forms;

namespace YP.VectorControl.Operation
{
	#region ..定义选择变换时鼠标的位置
	/// <summary>
	/// 定义选择变换时鼠标的位置
	/// </summary>
	internal enum MousePoint
	{
		ScaleTopLeft,
		ScaleTopMiddle,
		ScaleTopRight,
		ScaleMiddleLeft,
		ScaleMiddleRight,
		ScaleBottomLeft,
		ScaleBottomMiddle,
		ScaleBottomRight,
	
		ScaleFromCenter,

		SkewXTop,
		SkewXBottom,

		SkewXFromCenter,

		SkewYLeft,
		SkewYRight,

		SkewYFromCenter,

		Rotate,

		RotateFromCenter,

		Translate,

		CenterPoint,

		ShapeMovePoint,
		ShapeMoveControl,
		ShapeMoveLine,

		Flip,
		LabelPoint,
		None
	}
	#endregion

	/// <summary>
	/// SelectTransformOperation 的摘要说明。
	/// </summary>
	internal class SelectTransformOperation:ConnectorOperation
	{
		#region ..构造及消除
		internal SelectTransformOperation(Canvas mousearea):base(mousearea)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.mouseArea.DoubleClick += new EventHandler(mouseArea_DoubleClick);
			this.mouseArea.Click += new EventHandler(mouseArea_Click);
            this.mouseArea.SVGDocument.SelectCollection.CollectionChanged += new SVG.CollectionChangedEventHandler(Selection_CollectionChanged);
            boundPen = new Pen(this.mouseArea.SelectedPen.Color, 1);
            this.boundPen.DashPattern = new float[] { 3, 3, 3 };
		}

		public override void Dispose()
		{
			this.mouseArea.DoubleClick -= new EventHandler(mouseArea_DoubleClick);
            this.mouseArea.SVGDocument.SelectCollection.CollectionChanged -= new SVG.CollectionChangedEventHandler(Selection_CollectionChanged);
			this.mouseArea.Click -= new EventHandler(mouseArea_Click);
			if(this.toBeSelectedPath != null)
				this.toBeSelectedPath.Dispose();
			this.toBeSelectedPath = null;
            boundPen.Dispose();
			if(this.selectMatrix != null)
				this.selectMatrix.Dispose();
			this.selectMatrix = null;
			if(this.selectTransform != null)
				this.selectTransform.Dispose();
			this.selectTransform = null;
			base.Dispose ();
		}
		#endregion

		#region ..私有变量
        Size parentInflactSize = new Size(5, 5);
        Pen boundPen = null;
		PointF startPoint = Point.Empty;
        PointF startLabelPoint = PointF.Empty;
		MousePoint currentMousePoint = MousePoint.None;
		SVG.Interface.ISVGPathable toBeSelectedRender = null;
		GraphicsPath toBeSelectedPath = new GraphicsPath();
		Matrix selectMatrix = new Matrix();
		SVG.SVGElementCollection selectCollection = new SVG.SVGElementCollection();
		Matrix selectTransform = new Matrix();
		float rotateangle = 0;
		bool drawGuides = false;
		bool hguides = false;
		bool vguides = false;
		System.DateTime firstMouseDown;
		int vguideindex = -1;
		int hguideindex = -1;
		bool selectchanged = false;
		int anchorIndex = -1;
		PointF snapPoint = PointF.Empty;
		bool validclick = true;
		MousePoint oriPoint = MousePoint.None;
		System.DateTime mouseTime =System.DateTime.Now.AddMinutes(-2);
		PointF firstCnt = PointF.Empty,secondCnt = PointF.Empty;
        SVG.DocumentStructure.SVGGElement preGroup = null;
        Operation childOperation = null;
		#endregion

		#region ..常量
		const int MinLength = 6;
		#endregion

		#region ..properties
        Operation ChildOperation
        {
            set
            {
                if (this.childOperation != value)
                {
                    if (this.childOperation != null)
                        this.childOperation.Dispose();
                    this.childOperation = value;
                }
            }
        }

		/// <summary>
		/// 获取或设置鼠标位置
		/// </summary>
		MousePoint CurrentMousePoint
		{
			set
			{
				this.oriPoint = value;
				bool scale = (this.mouseArea.TransformBehavior & TransformBehavior.Scale) == TransformBehavior.Scale;
				bool translate = (this.mouseArea.TransformBehavior& TransformBehavior.Translate) == TransformBehavior.Translate;
				bool skew = (this.mouseArea.TransformBehavior & TransformBehavior.Skew) == TransformBehavior.Skew;
				bool rotate = (this.mouseArea.TransformBehavior & TransformBehavior.Rotate) == TransformBehavior.Rotate;
				int index = -1;
				switch(value)
				{
					case MousePoint.CenterPoint:
						this.mouseArea.Cursor = Forms.Cursors.CenterPoint;
						this.snapPoint = this.mouseArea.CenterPoint;
						
						break;
					case MousePoint.LabelPoint:
						this.mouseArea.Cursor = System.Windows.Forms.Cursors.SizeAll;
						break;
					case MousePoint.Translate:
						this.mouseArea.Cursor = Forms.Cursors.Drag;
						if(!translate)
							value = MousePoint.None;
						break;
					case MousePoint.ScaleBottomLeft:
						index = 6;
						this.mouseArea.Cursor = Forms.Cursors.TopRightScale;
						if(!scale)
							value = MousePoint.None;
						break;
					case MousePoint.ScaleTopRight:
						index = 2;
						this.mouseArea.Cursor = Forms.Cursors.TopRightScale;
						if(!scale)
							value = MousePoint.None;
						break;
					case MousePoint.ScaleTopMiddle:
						this.mouseArea.Cursor = Forms.Cursors.HScale;
						index = 1;
						break;
					case MousePoint.ScaleBottomMiddle:
						index = 5;
						this.mouseArea.Cursor = Forms.Cursors.HScale;
						if(!scale)
							value = MousePoint.None;
						break;
					case MousePoint.ScaleTopLeft:
						index = 0;
						this.mouseArea.Cursor = Forms.Cursors.TopLeftScale;
						if(!scale)
							value = MousePoint.None;
						break;
					case MousePoint.ScaleBottomRight:
						index = 4;
						this.mouseArea.Cursor = Forms.Cursors.TopLeftScale;
						if(!scale)
							value = MousePoint.None;
						break;
					case MousePoint.ScaleMiddleLeft:
						index = 7;
						this.mouseArea.Cursor = Forms.Cursors.VScale;
						if(!scale)
							value = MousePoint.None;
						break;		
					case MousePoint.ScaleMiddleRight:
						index = 3;
						this.mouseArea.Cursor = Forms.Cursors.VScale;
						if(!scale)
							value = MousePoint.None;
						break;		
					case MousePoint.SkewXBottom:
					case MousePoint.SkewXTop:
						this.mouseArea.Cursor = Forms.Cursors.SkewX;
						if(!skew)
							value = MousePoint.None;
						break;
					case MousePoint.SkewYLeft:
					case MousePoint.SkewYRight:
						this.mouseArea.Cursor = Forms.Cursors.SkewY;
						if(!skew)
							value = MousePoint.None;
						break;
					case MousePoint.Rotate:
						this.mouseArea.Cursor = Forms.Cursors.Rotate;
						if(!rotate)
							value = MousePoint.None;
						break;
					case MousePoint.None:
						this.mouseArea.Cursor = this.mouseArea.DefaultCursor;
						break;
				}
				this.currentMousePoint = value;
				if(index >= 0)
					this.snapPoint = this.mouseArea.GetSnapPoint(index);
			}
			get
			{
				return this.currentMousePoint;
			}
		}
		#endregion

		#region ..OnMouseDown
		protected override void OnMouseDown(object sender, MouseEventArgs e)
		{
			try
			{
				
				if(!this.IsValidDocument)
					return;
				base.OnMouseDown(sender,e);
				if(this.cntOperator != ConnectorOperation.ConnectOperator.None)
					return;
				validclick = true;
				if(e.Button == MouseButtons.Left)
				{
					this.mousedown = e.Button == MouseButtons.Left;
                    this.startPoint = new Point(e.X, e.Y);
					if((e.X <= this.mouseArea.ruleLength || this.mouseArea.ruleLength >= e.Y) && this.mouseArea.ShowRule)
					{
						this.vguides = this.hguides = this.mousedown = false;
						if(e.X > this.mouseArea.ruleLength)
						{
							this.drawGuides = true;
							this.hguides = true;
							this.mousedown = true;
						}
						else if(e.Y > this.mouseArea.ruleLength)
						{
							this.drawGuides = true;
							this.vguides = true;
							this.mousedown = true;
						}
						else
						{
							this.drawGuides = true;
							this.vguides = this.hguides = true;
							this.mousedown = true;
						}
						return;
					}
					if(this.vguideindex >= 0 || this.hguideindex >= 0 || this.anchorIndex >= 0)
						return;
					this.selectCollection = (SVG.SVGElementCollection)this.mouseArea.SVGDocument.SelectCollection;
					if(this.currentMousePoint == MousePoint.None && this.toBeSelectedRender != null)
					{
						this.CurrentMousePoint = MousePoint.Translate;
						this.toBeSelectedPath.Reset();
						this.toBeSelectedPath.AddPath((this.toBeSelectedRender as SVG.Interface.ISVGPathable).GPath,false);
						this.toBeSelectedPath.Transform(this.mouseArea.GetTotalTransformForElement((SVG.SVGTransformableElement)this.toBeSelectedRender));//.TotalTransform);
						SVG.SVGElementCollection list = (SVG.SVGElementCollection)this.mouseArea.SVGDocument.SelectCollection ;
						if(!list.Contains(this.toBeSelectedRender) && this.toBeSelectedRender != null)
						{
							if((Control.ModifierKeys & Keys.Shift)== Keys.Shift)
								list.Add(this.toBeSelectedRender);
							else
								this.mouseArea.SVGDocument.ChangeSelectElement((SVG.SVGElement)this.toBeSelectedRender);
							validclick = false;
						}
					
						this.mouseArea.Cursor = Forms.Cursors.Drag;
					}
					this.reversePath.Reset();

                    if (this.mouseArea.NeedDrawLabelIndicator)
                        this.startLabelPoint = (this.mouseArea.SVGDocument.SelectCollection[0] as SVG.SVGTransformableElement).LabelPoint;
					this.firstMouseDown = System.DateTime.Now;
				}
				else
				{
					this.toBeSelectedRender = null;
				}
			}
			catch(System.Exception e1)
			{
				System.Diagnostics.Trace.Assert(true,e1.Message);
			}
			
		}
		#endregion

		#region ..OnMouseMove
		protected override void OnMouseMove(object sender, MouseEventArgs e)
		{
			try
			{
				if(!this.IsValidDocument)
					return;

				#region ..无按键移动
				if(e.Button == MouseButtons.None)
				{
					System.TimeSpan ts = System.DateTime.Now - this.mouseTime;
					if(ts.Milliseconds < 2)
						return;
					base.OnMouseMove(sender,e);
					if(this.cntOperator != ConnectorOperation.ConnectOperator.None)
						return;
                    this.vguideindex = this.hguideindex = -1;
					snapPoint = PointF.Empty;
					this.currentMousePoint = MousePoint.None;
					Point p = this.mouseArea.PointClientToView(new Point(e.X,e.Y));

					this.toBeSelectedPath.Reset();
					this.toBeSelectedRender = null;

					using(Pen selectPen = new Pen(Color.Black,Canvas.PenWidth))
					{
						selectPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;

						#region ..如果当前存在选区，则选择操作对象
						SVG.SVGElementCollection list = (SVG.SVGElementCollection)this.mouseArea.SVGDocument.SelectCollection;
						if(list.Count > 0)
						{
							Operator op = this.mouseArea.CurrentOperator;

							#region ..变形工具
							if(OperatorHelper.IsTransformOperator(this.mouseArea.CurrentOperator))
							{
								this.CurrentMousePoint = this.GetMousePoint(new Point(e.X,e.Y));
								if(this.currentMousePoint != MousePoint.None)
								{
									this.toBeSelectedPath = (GraphicsPath)this.mouseArea.SelectPath.Clone();
									if(this.currentMousePoint == MousePoint.Translate)
										this.mouseArea.Cursor = Forms.Cursors.Drag;
									return;
								}
							}
							#endregion
						}
						#endregion

						#region ..否则，判断预选择对象
                        SVG.Interface.ISVGPathable pathable = this.GetElement(p) as SVG.Interface.ISVGPathable;
                        if (pathable != null)
                        {
                            if(((pathable as SVG.SVGStyleable).ViewStyle & SVG.ViewStyle.Lock) != SVG.ViewStyle.Lock)
                            this.mouseArea.Cursor = Forms.Cursors.DragInfo;
                            this.toBeSelectedRender = pathable;
                            this.toBeSelectedPath.Reset();
                            this.toBeSelectedPath.AddPath((pathable as SVG.Interface.ISVGPathable).GPath, false);
                            this.selectMatrix.Reset();
                            this.selectMatrix.Multiply(this.mouseArea.GetTotalTransformForElement((SVG.SVGTransformableElement)pathable));
                            return;
                        }
                        #endregion
                    }
					this.CurrentMousePoint = MousePoint.None;
					
                    #region ..参考线移动判断
                    
                    if (this.mouseArea.Guide.Visible && !this.mouseArea.Guide.Lock)
                    {
                        //垂直参考线
                        if (this.mouseArea.vGuides.Count > 0)
                        {
                            for (int i = 0; i < this.mouseArea.vGuides.Count; i++)
                            {
                                int a = (int)this.mouseArea.vGuides[i];
                                if (Math.Abs(p.X - a) < Canvas.SnapMargin)
                                {
                                    this.vguideindex = i;
                                    this.mouseArea.Cursor = Forms.Cursors.VGuide;
                                    return;
                                }
                            }
                        }

                        //水平参考线
                        if (this.mouseArea.hGuides.Count > 0)
                        {
                            for (int i = 0; i < this.mouseArea.hGuides.Count; i++)
                            {
                                int a = (int)this.mouseArea.hGuides[i];
                                if (Math.Abs(p.Y - a) < Canvas.SnapMargin)
                                {
                                    this.hguideindex = i;
                                    this.mouseArea.Cursor = Forms.Cursors.HGuide;
                                    return;
                                }
                            }
                        }
                    }
                    #endregion
				}
					#endregion

					#region ..左键移动
				else if(e.Button == MouseButtons.Left && this.mousedown)
				{
					#region ..Connect
					if(this.cntOperator != ConnectorOperation.ConnectOperator.None)
						base.OnMouseMove(sender,e);
						#endregion

						#region ..辅助线
					else if(this.drawGuides && this.mousedown)
					{
						this.XORDrawPath(this.reversePath);
						this.XORDrawPath(this.reverseFillPath);
						this.reversePath.Reset();
						this.reverseFillPath.Reset();
						Point p = this.mouseArea.PointClientToView(new Point(e.X,e.Y));
						this.useDash = false;
						//					this.reverseColor = Color.Blue;
						//					this.reverseColor1 = Color.Blue;
						if(this.hguides)
						{
							this.reversePath.AddLine(0,e.Y,this.mouseArea.Width,e.Y);
							this.reverseFillPath.AddString(p.Y.ToString(),SystemInformation.MenuFont.FontFamily,(int)FontStyle.Regular,12,new Point(e.X + 14,e.Y),StringFormat.GenericTypographic);
						}

						if(this.vguides)
						{
							this.reversePath.StartFigure();
							this.reversePath.AddLine(e.X,0,e.X,this.mouseArea.Height);
							this.reverseFillPath.AddString(p.X.ToString(),SystemInformation.MenuFont.FontFamily,(int)FontStyle.Regular,12,new Point(e.X,e.Y - 14),StringFormat.GenericTypographic);
						}
					
						this.XORDrawPath(this.reversePath);
						this.XORDrawPath(this.reverseFillPath);
					}
					else if(this.vguideindex >= 0 || this.hguideindex >= 0)
					{
						this.XORDrawPath(this.reversePath);
						this.XORDrawPath(this.reverseFillPath);
						this.reverseFillPath.Reset();
						this.reversePath.Reset();
						Point p = this.mouseArea.PointClientToView(new Point(e.X,e.Y));
						if(this.hguideindex >= 0)
						{
							this.reverseFillPath.AddString(p.Y.ToString(),SystemInformation.MenuFont.FontFamily,(int)FontStyle.Regular,12,new Point(e.X + 14,e.Y),StringFormat.GenericTypographic);
							this.reversePath.AddLine(0,e.Y,this.mouseArea.Width,e.Y);
						}
						else if(this.vguideindex >= 0)
						{
							this.reversePath.AddLine(e.X,0,e.X,this.mouseArea.Height);
							this.reverseFillPath.AddString(p.X.ToString(),SystemInformation.MenuFont.FontFamily,(int)FontStyle.Regular,12,new Point(e.X,e.Y - 14),StringFormat.GenericTypographic);
						}
						this.XORDrawPath(this.reversePath);
						this.XORDrawPath(this.reverseFillPath);
					}
						#endregion

						#region ..移动中心点
					else if(this.currentMousePoint == MousePoint.CenterPoint)
					{
						if(this.mouseArea.SVGDocument.SelectCollection.Count == 1 && this.mouseArea.SVGDocument.SelectCollection[0] is SVG.BasicShapes.SVGConnectionElement)
							return;
						this.XORDrawPath(this.reverseFillPath);
						this.reverseFillPath.Reset();
						this.reverseFillPath.AddEllipse(e.X - 2,e.Y - 2,4,4);
						this.XORDrawPath(this.reverseFillPath);
					}
						#endregion

						#region ..移动文本标志点
                    else if (this.currentMousePoint == MousePoint.LabelPoint)
                    {
                        bool old = this.mouseArea.SVGDocument.AcceptNodeChanged;
                        this.mouseArea.SVGDocument.AcceptNodeChanged = false;
                        PointF start = this.mouseArea.PointClientToView(this.startPoint);
                        Point p = this.mouseArea.PointClientToView(new Point(e.X,e.Y));
                        SVG.SVGTransformableElement labelElement = this.mouseArea.SVGDocument.SelectCollection[0] as SVG.SVGTransformableElement;
                        labelElement.InternalSetAttribute("labelX", (startLabelPoint.X + p.X - start.X).ToString());
                        labelElement.InternalSetAttribute("labelY", (startLabelPoint.Y + p.Y - start.Y).ToString());
                        this.mouseArea.SVGDocument.AcceptNodeChanged = old;
                        this.mouseArea.SVGDocument.InvokeUndos();
                    }
						#endregion

						#region ..操作活动节点
                    else if (this.currentMousePoint != MousePoint.None && this.toBeSelectedPath != null)
                    {
                        if (this.mouseArea.SVGDocument.SelectCollection.Count == 1 && this.mouseArea.SVGDocument.SelectCollection[0] is SVG.BasicShapes.SVGConnectionElement)
                        {
                            return;
                        }

                        if (this.mouseArea.SVGDocument.SelectCollection.ContainsLockedElement)
                            return;

                        System.TimeSpan ts = System.DateTime.Now - this.firstMouseDown;
                        if (ts.Milliseconds <= SystemInformation.DoubleClickTime && Math.Abs(e.X - this.startPoint.X) < SystemInformation.DoubleClickSize.Width && Math.Abs(e.Y - this.startPoint.Y) < SystemInformation.DoubleClickSize.Height)
                        {
                            System.Diagnostics.Debug.Assert(true, "双击");
                            return;
                        }

                        this.XORDrawPath(this.reversePath);
                        this.XORDrawPath(this.reverseFillPath);
                        this.XORDrawPath(this.reverseSnapIndicatorPath);
                        this.reversePath.Reset();
                        this.reverseFillPath.Reset();
                        this.reverseSnapIndicatorPath.Reset();
                        Matrix transform = this.mouseArea.SelectMatrix;
                        RectangleF rect1 = this.mouseArea.SelectPath.GetBounds();
                        //检测最小边界
                        float minX = 0;
                        float minY = 0;
                        PointF[] ps1 = new PointF[] { new PointF(0, 0), new PointF(0, Canvas.MinGrap), new PointF(Canvas.MinGrap, 0) };
                        transform.TransformPoints(ps1);
                        minX = (float)Math.Sqrt(Math.Pow(ps1[2].X - ps1[0].X, 2) + Math.Pow(ps1[2].Y - ps1[0].Y, 2));
                        minX = (float)(Canvas.MinGrap * Canvas.MinGrap) / minX;
                        minY = (float)Math.Sqrt(Math.Pow(ps1[1].Y - ps1[0].Y, 2) + Math.Pow(ps1[1].X - ps1[0].X, 2));
                        minY = (float)(Canvas.MinGrap * Canvas.MinGrap) / minY;
                        ps1 = null;
                        if (rect1.Width < minX)
                        {
                            rect1.X = rect1.X + rect1.Width / 2 - minX / 2;
                            rect1.Width = minX;
                        }
                        if (rect1.Height < minY)
                        {
                            rect1.Y = rect1.Y + rect1.Height / 2 - minY / 2;
                            rect1.Height = minY;
                        }
                        PointF[] boundsPoints = new PointF[] { new PointF(rect1.X, rect1.Y), new PointF(rect1.X, rect1.Y + rect1.Height / 2), new PointF(rect1.X, rect1.Bottom), new PointF(rect1.X + rect1.Width / 2, rect1.Y), new PointF(rect1.Right, rect1.Y), new PointF(rect1.Right, rect1.Y + rect1.Height / 2), new PointF(rect1.Right, rect1.Bottom), new PointF(rect1.X + rect1.Width / 2, rect1.Bottom), new PointF(rect1.X + rect1.Width / 2, rect1.Y + rect1.Height / 2), this.mouseArea.oldselectPoint };
                        PointF[] ps = new PointF[] { this.startPoint, new Point(e.X, e.Y), this.mouseArea.CenterPoint };
                        ElementAlign align = ElementAlign.None;
                        PointF start = PointF.Empty, centerPoint = PointF.Empty;
                        bool hori = true, vert = true;
                         bool? baseX = null;
                        if (transform.IsInvertible)
                        {
                            Matrix matrix = transform.Clone();
                            matrix.Invert();
                            if (this.currentMousePoint != MousePoint.Rotate)
                                matrix.TransformPoints(ps);
                            start = ps[0];
                            PointF end = ps[1];
                            centerPoint = ps[2];
                            float x = 0, y = 0;

                            switch (this.currentMousePoint)
                            {
                                #region ..偏移
                                case MousePoint.Translate:
                                    this.selectTransform.Reset();
                                    selectTransform.Translate(end.X - start.X, end.Y - start.Y);
                                    align = ElementAlign.All;
                                    break;
                                #endregion

                                #region ..缩放
                                case MousePoint.ScaleTopLeft:
                                case MousePoint.ScaleTopRight:
                                case MousePoint.ScaleBottomLeft:
                                case MousePoint.ScaleBottomRight:
                                case MousePoint.ScaleMiddleLeft:
                                case MousePoint.ScaleMiddleRight:
                                case MousePoint.ScaleTopMiddle:
                                case MousePoint.ScaleBottomMiddle:
                                   
                                    if (this.currentMousePoint == MousePoint.ScaleTopLeft)
                                    {
                                        centerPoint = boundsPoints[6];
                                        start = boundsPoints[0];
                                    }
                                    else if (this.currentMousePoint == MousePoint.ScaleTopRight)
                                    {
                                        centerPoint = boundsPoints[2];
                                        start = boundsPoints[4];
                                    }
                                    else if (this.currentMousePoint == MousePoint.ScaleBottomLeft)
                                    {
                                        centerPoint = boundsPoints[4];
                                        start = boundsPoints[2];
                                    }
                                    else if (this.currentMousePoint == MousePoint.ScaleBottomRight)
                                    {
                                        centerPoint = boundsPoints[0];
                                        start = boundsPoints[6];
                                    }
                                    else if (this.currentMousePoint == MousePoint.ScaleMiddleLeft)
                                    {
                                        centerPoint = boundsPoints[5];
                                        start = boundsPoints[1];
                                        vert = false;
                                    }
                                    else if (this.currentMousePoint == MousePoint.ScaleMiddleRight)
                                    {
                                        centerPoint = boundsPoints[1];
                                        start = boundsPoints[5];
                                        vert = false;
                                    }
                                    else if (this.currentMousePoint == MousePoint.ScaleTopMiddle)
                                    {
                                        centerPoint = boundsPoints[7];
                                        start = boundsPoints[3];
                                        hori = false;
                                    }
                                    else if (this.currentMousePoint == MousePoint.ScaleBottomMiddle)
                                    {
                                        centerPoint = boundsPoints[3];
                                        start = boundsPoints[7];
                                        hori = false;
                                    }

                                    bool valid = false;
                                    x = y = 1;
                                    if ((this.currentMousePoint == MousePoint.ScaleMiddleLeft || this.currentMousePoint == MousePoint.ScaleMiddleRight) && start.X - centerPoint.X != 0 && end.X != centerPoint.X)
                                    {
                                        valid = true;
                                        x = (float)(end.X - centerPoint.X) / (float)(start.X - centerPoint.X);
                                        y = 1;
                                    }
                                    else if ((this.currentMousePoint == MousePoint.ScaleTopMiddle || this.currentMousePoint == MousePoint.ScaleBottomMiddle) && start.Y - centerPoint.Y != 0 && end.Y != centerPoint.Y)
                                    {
                                        valid = true;
                                        y = (float)(end.Y - centerPoint.Y) / (float)(start.Y - centerPoint.Y);
                                        x = 1;
                                    }
                                    else if (start.X != centerPoint.X && start.Y != centerPoint.Y && end.X != centerPoint.X && end.Y != centerPoint.Y)
                                    {
                                        valid = true;
                                        x = (float)(end.X - centerPoint.X) / (float)(start.X - centerPoint.X);
                                        y = (float)(end.Y - centerPoint.Y) / (float)(start.Y - centerPoint.Y);
                                        if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                                        {
                                            baseX = x < y;
                                            x = y = (float)Math.Min(x, y);
                                        }
                                    }
                                    x = (float)Math.Round(x, 2);
                                    y = (float)Math.Round(y, 2);
                                    if (valid)
                                    {
                                        this.selectTransform.Reset();
                                        selectTransform.Translate(centerPoint.X, centerPoint.Y);
                                        selectTransform.Scale(x, y);
                                        selectTransform.Translate(-centerPoint.X, -centerPoint.Y);
                                    }
                                    break;
                                #endregion

                                #region ..扭曲
                                case MousePoint.SkewXBottom:
                                case MousePoint.SkewXTop:
                                case MousePoint.SkewYLeft:
                                case MousePoint.SkewYRight:
                                    //							if((Control.ModifierKeys & Keys.Control) == Keys.Control)
                                    //							{
                                    //									if(this.currentMousePoint == MousePoint.SkewXBottom)
                                    //									{
                                    //										centerPoint = boundsPoints[0]; 
                                    //										//									start = boundsPoints[6];
                                    //									}
                                    //									else if(this.currentMousePoint == MousePoint.SkewXTop)
                                    //									{
                                    //										centerPoint = boundsPoints[6];
                                    //										//									start = boundsPoints[0]; 
                                    //									}
                                    //									else if(this.currentMousePoint == MousePoint.SkewYLeft)
                                    //									{
                                    //										centerPoint = boundsPoints[4];
                                    //										//									start = boundsPoints[0];
                                    //									}
                                    //									else if(this.currentMousePoint == MousePoint.SkewYRight)
                                    //									{
                                    //										centerPoint = boundsPoints[0];
                                    //										//									start = boundsPoints[4];
                                    //									}
                                    //							}

                                    x = (float)(end.X - start.X) / (float)(boundsPoints[6].Y - boundsPoints[4].Y);
                                    y = (float)(end.Y - start.Y) / (float)(boundsPoints[6].X - boundsPoints[2].X);
                                    if (this.currentMousePoint == MousePoint.SkewXTop || this.currentMousePoint == MousePoint.SkewXBottom)
                                    {
                                        y = 0;
                                        if (this.currentMousePoint == MousePoint.SkewXTop)
                                            x = -x;
                                    }
                                    else if (this.currentMousePoint == MousePoint.SkewYLeft || this.currentMousePoint == MousePoint.SkewYRight)
                                    {
                                        if (this.currentMousePoint == MousePoint.SkewYLeft)
                                            y = -y;
                                        x = 0;
                                    }

                                    x = (x == 0 ? 0 : (float)x / (float)Math.Abs(x) * (float)Math.Min(Math.Abs(x), 3));
                                    y = (y == 0 ? 0 : (float)y / (float)Math.Abs(y) * (float)Math.Min(Math.Abs(y), 3));
                                    this.selectTransform.Reset();
                                    selectTransform.Translate(centerPoint.X, centerPoint.Y);
                                    selectTransform.Shear(x, y);
                                    selectTransform.Translate(-centerPoint.X, -centerPoint.Y);
                                    break;
                                #endregion

                                #region ..旋转
                                case MousePoint.Rotate:
                                    //如果按下Ctrl键，则以其相对点为中心点
                                    //							if((Control.ModifierKeys & Keys.Control) == Keys.Control)
                                    //								centerPoint = boundsPoints[this.rotateindex];

                                    float angle = 0;
                                    float startangle = 0;
                                    if (start.X == centerPoint.X)
                                        startangle = (float)Math.PI / 2f;
                                    else
                                        startangle = (float)Math.Atan((float)(start.Y - centerPoint.Y) / (float)(start.X - centerPoint.X));
                                    float endangle = 0;
                                    if (end.X == centerPoint.X)
                                        endangle = (float)Math.PI / 2f;
                                    else
                                        endangle = (float)Math.Atan((float)(end.Y - centerPoint.Y) / (float)(end.X - centerPoint.X));
                                    angle = (float)((endangle - startangle) / Math.PI) * 180;

                                    if ((end.X - centerPoint.X) * (start.X - centerPoint.X) < 0)
                                        angle += 180;

                                    if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                                    {
                                        if (angle != 0)
                                        {
                                            angle = angle / (float)Math.Abs(angle) * (((int)((float)Math.Abs(angle) + 23) / 45) * 45);
                                        }
                                    }

                                    this.rotateangle = angle;
                                    this.selectTransform.Reset();
                                    this.selectTransform.RotateAt(angle, centerPoint);
                                    break;
                                #endregion
                            }
                            if (this.currentMousePoint == MousePoint.Rotate)
                                transform.Multiply(this.selectTransform, MatrixOrder.Append);
                            else
                                transform.Multiply(this.selectTransform);
                            if (this.mouseArea.ShowSelectionHighlightOutline && this.mouseArea.SelectPath.PointCount > 1)
                            {
                                if (this.mouseArea.SelectPath.PointCount > Canvas.MaxPathPoint)
                                    this.reversePath.AddRectangle(this.mouseArea.SelectPath.GetBounds());
                                else
                                    this.reversePath.AddPath(this.mouseArea.SelectPath, false);
                            }
                            if (this.mouseArea.ShowSelectedBounds)
                            {
                                this.reversePath.StartFigure();
                                this.reversePath.AddRectangle(rect1);
                            }
                            this.reversePath.Transform(transform);
                            transform.TransformPoints(boundsPoints);
                            PointF[] temp = { centerPoint };
                            transform.TransformPoints(temp);
                            centerPoint = temp[0];
                            int r = this.mouseArea.grapSize / 2;
                            int startIndex = 0;
                            int grapLength = boundsPoints.Length;
                            if (!this.mouseArea.ShowCenterPointGrap)
                                grapLength--;
                            if (!this.mouseArea.ShowResizeGrap)
                                startIndex = boundsPoints.Length - 2;
                        }

                        #region ..对齐到对象
                        RectangleF bounds = RectangleF.Empty;//this.reversePath.GetBounds();
                        using (GraphicsPath path = this.mouseArea.SelectPath.Clone() as GraphicsPath)
                        {
                            path.Transform(transform);
                            bounds = path.GetBounds();

                            //对齐到对象
                            if (this.mouseArea.SVGDocument.SelectCollection.Count > 0 && (bounds.Width > 0 || bounds.Height > 0))
                            {
                                Point nativePoint = e.Location;
                                if (e is TLMouseEventArgs)
                                    nativePoint = (e as TLMouseEventArgs).NativePoint;
                                AlignResult[] results = null;

                                using (Matrix temp = new Matrix())
                                {
                                    switch (this.currentMousePoint)
                                    {
                                        case MousePoint.Translate:
                                            results = this.AlignToElement(this.reverseSnapIndicatorPath, path, align, this.mouseArea.SVGDocument.SelectCollection.ToArray(), this.mouseArea.SVGDocument.SelectCollection.Count == 1, true,true);
                                            if (results != null)
                                            {
                                                foreach (AlignResult result in results)
                                                {
                                                    if (result.Horizontal)
                                                        temp.Translate(result.Delta, 0, MatrixOrder.Append);
                                                    else
                                                        temp.Translate(0, result.Delta, MatrixOrder.Append);
                                                }
                                            }
                                            break;
                                        case MousePoint.ScaleTopLeft:
                                        case MousePoint.ScaleTopRight:
                                        case MousePoint.ScaleBottomLeft:
                                        case MousePoint.ScaleBottomRight:
                                        case MousePoint.ScaleMiddleLeft:
                                        case MousePoint.ScaleMiddleRight:
                                        case MousePoint.ScaleTopMiddle:
                                        case MousePoint.ScaleBottomMiddle:
                                            align = ElementAlign.None;
                                            if ((hori && !baseX.HasValue) || (baseX.HasValue && baseX.Value))
                                            {
                                                align = align | ElementAlign.Center;
                                                if (e.X > centerPoint.X)
                                                    align = align | ElementAlign.Right;
                                                else
                                                    align = align | ElementAlign.Left;
                                            }
                                            if ((vert&&!baseX.HasValue) || (baseX.HasValue && !baseX.Value))
                                            {
                                                align = align | ElementAlign.Middle;
                                                if (e.Y > centerPoint.Y)
                                                    align = align | ElementAlign.Bottom;
                                                else
                                                    align = align | ElementAlign.Top;
                                            }
                                            results = this.AlignToElement(this.reverseSnapIndicatorPath, path, align, this.mouseArea.SVGDocument.SelectCollection.ToArray(),this.mouseArea.SVGDocument.SelectCollection.Count == 1,true,true);
                                            temp.Translate(-centerPoint.X, -centerPoint.Y);
                                            if (results != null)
                                            {
                                                foreach (AlignResult result in results)
                                                {
                                                    float scaleX = 1, scaleY = 1;
                                                    if (result.Horizontal && bounds.Width > 0)
                                                    {
                                                        float value = result.Value;
                                                        float width = value - bounds.Left;
                                                        if (result.SourcePos == ElementAlign.Left)
                                                            width = bounds.Right - value;
                                                        else if (result.SourcePos == ElementAlign.Center)
                                                        {
                                                            width = 2 * (float)Math.Abs(value - centerPoint.X);
                                                        }
                                                        scaleX = width / bounds.Width;
                                                    }
                                                    else if (bounds.Height > 0)
                                                    {
                                                        float value = result.Value;
                                                        float width = value - bounds.Top;
                                                        if (result.SourcePos == ElementAlign.Top)
                                                            width = bounds.Bottom - value;
                                                        else if (result.SourcePos == ElementAlign.Middle)
                                                            width = 2 * (float)Math.Abs(value - centerPoint.Y);

                                                        scaleY = width / bounds.Height;
                                                    }
                                                    //shift
                                                    if (Control.ModifierKeys == Keys.Shift)
                                                    {
                                                        if (baseX.HasValue)
                                                        {
                                                            if (baseX.Value)
                                                                scaleY = scaleX;
                                                            else
                                                                scaleX = scaleY;
                                                        }
                                                        else
                                                            scaleX = scaleY = 1;
                                                    }
                                                    temp.Scale(scaleX, scaleY, MatrixOrder.Append);
                                                }
                                            }
                                            temp.Translate(centerPoint.X, centerPoint.Y, MatrixOrder.Append);
                                            break;

                                    }
                                    this.reversePath.Transform(temp);
                                    this.reverseFillPath.Transform(temp);

                                    if (!temp.IsIdentity)
                                    {
                                        transform.Multiply(temp, MatrixOrder.Append);

                                        temp.Reset();
                                        temp.Multiply(this.mouseArea.SelectMatrix);
                                        temp.Invert();
                                        this.selectTransform = transform.Clone();
                                        this.selectTransform.Multiply(temp, MatrixOrder.Append);
                                    }
                                }
                            }
                        }
                        #endregion

                        transform.Dispose();
                        transform = null;
                        this.XORDrawPath(this.reversePath);
                        this.XORDrawPath(this.reverseFillPath);
                        this.XORDrawPath(this.reverseSnapIndicatorPath);
                    }
                        #endregion

                    #region ..选择
                    else if (this.oriPoint == MousePoint.None)
                    {
                        this.XORDrawPath(this.reversePath);
                        this.reversePath.Reset();
                        float left = (float)Math.Min(e.X, this.startPoint.X);
                        float right = (float)Math.Max(e.X, this.startPoint.X);
                        float top = (float)Math.Min(e.Y, this.startPoint.Y);
                        float bottom = (float)Math.Max(e.Y, this.startPoint.Y);
                        RectangleF rect = new RectangleF(left, top, right - left, bottom - top);
                        this.reversePath.AddRectangle(rect);
                        this.XORDrawPath(this.reversePath);
                    }
					#endregion
				}
				#endregion
			}
			catch(System.Exception e1)
            {
                this.mouseArea.SVGDocument.OnExceptionOccured(new SVG.ExceptionOccuredEventArgs(new object[] { e1.Message, e1.StackTrace }, SVG.ExceptionLevel.Normal));
            }
			finally
			{

			}
		}
		#endregion

		#region ..OnMouseUp
		protected override void OnMouseUp(object sender, MouseEventArgs e)
		{
			try
			{
				if(!this.IsValidDocument)
					return;
				if((e.Button == MouseButtons.Left || e.Button == MouseButtons.Right) && this.mousedown)
				{
					#region ..Connect
					if(this.cntOperator != ConnectorOperation.ConnectOperator.None)
						base.OnMouseUp(sender,e);
						#endregion

						#region ..辅助线
					else if(this.drawGuides && this.mousedown)
					{
						this.XORDrawPath(this.reversePath);
						this.XORDrawPath(this.reverseFillPath);
						if(this.hguides)
						{
							if(e.Y > this.mouseArea.ruleLength&& e.Y <= this.mouseArea.Height)
							{
								Point p = this.mouseArea.PointClientToView(new Point(e.X,e.Y));
								int index = this.mouseArea.hGuides.IndexOf(p.Y);
								if(index < 0)
									this.mouseArea.hGuides.Add(p.Y);
							}
						}

						if(this.vguides) 
						{
							if(e.X >= this.mouseArea.ruleLength && e.X <= this.mouseArea.Width)
							{
								Point p = this.mouseArea.PointClientToView(new Point(e.X,e.Y));
								int index = this.mouseArea.vGuides.IndexOf(p.X);
								if(index < 0)
									this.mouseArea.vGuides.Add(p.X);
							}
						}
					}
					else if(this.vguideindex >= 0 || this.hguideindex >= 0)
					{
						this.XORDrawPath(this.reversePath);
						this.XORDrawPath(this.reverseFillPath);
						Point p = this.mouseArea.PointClientToView(new Point(e.X,e.Y));
						if(this.hguideindex >= 0)
						{
							int a = (int)this.mouseArea.hGuides[this.hguideindex];
							PointF p1 = this.mouseArea.PointViewToClient(new PointF(0,a));
							this.mouseArea.Invalidate(new Rectangle(0,(int)p1.Y - 1,this.mouseArea.Width,2));
							if(e.Y < 0 || e.Y > this.mouseArea.Height)
								this.mouseArea.hGuides.RemoveAt(this.hguideindex);
							else
								this.mouseArea.hGuides[this.hguideindex] = p.Y;
						}
						else if(this.vguideindex >= 0)
						{
							int a = (int)this.mouseArea.vGuides[this.vguideindex];
							PointF p1 = this.mouseArea.PointViewToClient(new PointF(a,0));
							this.mouseArea.Invalidate(new Rectangle((int)p1.X - 1,0,2,this.mouseArea.Height));
							if(e.X < 0 || e.X > this.mouseArea.Width)
								this.mouseArea.vGuides.RemoveAt(this.vguideindex);
							else
								this.mouseArea.vGuides[this.vguideindex] = p.X;
						}
					}
						#endregion

						#region ..移动中心点
					else if(this.currentMousePoint == MousePoint.CenterPoint)
					{
						if(this.mouseArea.SVGDocument.SelectCollection.Count == 1 && this.mouseArea.SVGDocument.SelectCollection[0] is SVG.BasicShapes.SVGConnectionElement)
							return;
						this.XORDrawPath(this.reverseFillPath);
						PointF p = this.mouseArea.CenterPoint;
						this.mouseArea.Invalidate(new Rectangle((int)p.X - 6,(int)p.Y - 6,14,14));
						using(Matrix matrix = this.mouseArea.SelectMatrix.Clone())
						{
							matrix.Invert();
							Point[] ps = new Point[]{new Point(e.X,e.Y)};
							matrix.TransformPoints(ps);
							matrix.Dispose();
							this.mouseArea.SetNewCenterPoint(ps[0]);
							this.mouseArea.Invalidate(new Rectangle(e.X - 5,e.Y - 5,10,10));
							ps = null;
						}
					}
						#endregion

						#region ..移动文本标志点
                    else if (this.currentMousePoint == MousePoint.LabelPoint)
                    {
                        this.XORDrawPath(this.reversePath);
                        if (this.mouseArea.SVGDocument.SelectCollection.Count == 1 && this.mouseArea.SVGDocument.SelectCollection[0] is SVG.SVGTransformableElement)
                        {
                            SVG.SVGTransformableElement render = this.mouseArea.SVGDocument.SelectCollection[0] as SVG.SVGTransformableElement;
                            PointF[] ps = new PointF[] { new PointF(e.X, e.Y) };
                            using (System.Drawing.Drawing2D.Matrix matrix = render.TotalTransform.Clone())
                            {
                                matrix.Invert();
                                matrix.TransformPoints(ps);
                                bool old = this.mouseArea.SVGDocument.AcceptNodeChanged;
                                this.mouseArea.SVGDocument.AcceptNodeChanged = true;
                                
                                this.mouseArea.SVGDocument.AcceptNodeChanged = old;
                                this.mouseArea.SVGDocument.InvokeUndos();
                            }
                        }
                    }
						#endregion

						#region ..操作活动节点
					else if(this.currentMousePoint != MousePoint.None && this.toBeSelectedPath != null && this.mousedown && e.Button == MouseButtons.Left)
					{
						if(this.mouseArea.SVGDocument.SelectCollection.Count == 1 && this.mouseArea.SVGDocument.SelectCollection[0] is SVG.BasicShapes.SVGConnectionElement)
						{
							return;
						}
                        if (this.mouseArea.SVGDocument.SelectCollection.ContainsLockedElement)
                            return;
						this.XORDrawPath(this.reversePath);
						this.XORDrawPath(this.reverseFillPath);
                        this.XORDrawPath(this.reverseSnapIndicatorPath);
						System.TimeSpan ts = System.DateTime.Now - this.firstMouseDown;
						if(ts.Milliseconds <= SystemInformation.DoubleClickTime && Math.Abs(e.X - this.startPoint.X)<SystemInformation.DoubleClickSize.Width && Math.Abs(e.Y - this.startPoint.Y) < SystemInformation.DoubleClickSize.Height)
						{
							//System.Diagnostics.Debug.Assert(true,"双击");
							return;
						}
						if(e.X == this.startPoint.X && e.Y == this.startPoint.Y)
						{
							if(this.selectchanged)
								this.mouseArea.InvalidateSelection();
							this.reversePath.Reset();
							return;
						}
						this.mouseArea.InvalidateSelection();
                        //如果是操作子节点，刷新父节点边界
                        if (this.IsChildSelected())
                            this.InvalidateParentBound();
						Rectangle rect1 = Rectangle.Round(this.mouseArea.SelectPath.GetBounds());
						PointF[] bounds = new PointF[]{new Point(rect1.X,rect1.Y),new Point(rect1.X,rect1.Y + rect1.Height / 2),new Point(rect1.X,rect1.Bottom),new Point(rect1.X + rect1.Width / 2,rect1.Y),new Point(rect1.Right,rect1.Y),new Point(rect1.Right,rect1.Y + rect1.Height /2),new Point(rect1.Right,rect1.Bottom),new Point(rect1.X + rect1.Width / 2,rect1.Bottom),new Point(rect1.X + rect1.Width / 2,rect1.Y + rect1.Height / 2)};
						Matrix transform = this.mouseArea.SelectMatrix.Clone();
						PointF[] ps = new PointF[]{this.startPoint,new Point(e.X,e.Y),this.mouseArea.CenterPoint};
						Matrix matrix = transform.Clone();
						matrix.Invert();
						if(this.currentMousePoint != MousePoint.Rotate)
						{
							matrix.TransformPoints(ps);
						}
						PointF start = ps[0];
						PointF end = ps[1];
						PointF centerPoint = ps[2];

						PointF[] orips = (PointF[])ps.Clone();
						SVG.SVGElementCollection list = (SVG.SVGElementCollection)this.mouseArea.SVGDocument.SelectCollection;
				
						matrix.Reset();
						int time = this.mouseArea.SVGDocument.CurrentTime;
						SVG.Document.SVGDocument doc = this.mouseArea.SVGDocument;
						Matrix oriMatrix = new Matrix();
						bool old = doc.AcceptNodeChanged;

						#region ..循环选择列表
						for(int i = 0;i<list.Count;i++)//foreach(SVGDom.SVGElement element in list)
						{
							SVG.SVGElement element = list[i] as SVG.SVGElement;
							if(!(element is SVG.Interface.ISVGPathable))
								continue;
							matrix.Reset();
	
							//if there is some connects in the childs of the elements,update them
							doc.AcceptNodeChanged = false;
							System.Xml.XmlNodeList childs = element.GetElementsByTagName("connect");
                            for (int j = -1; j < childs.Count; j++)
                            {
                                SVG.Interface.ISVGPathable render = null;
                                if (j == -1)
                                    render = element as SVG.Interface.ISVGPathable;
                                else
                                    render = childs[j] as SVG.Interface.ISVGPathable;
                                if (render == null)
                                    continue;
                                SVG.SVGTransformableElement transformelement = (SVG.SVGTransformableElement)render;
                                using (Matrix temp2 = this.mouseArea.GetTotalTransformForElement(transformelement))//.TotalTransform.Clone();
                                {
                                    using (Matrix temp3 = temp2.Clone())
                                    {
                                        oriMatrix = temp2.Clone();
                                        oriMatrix.Invert();
                                        oriMatrix.Multiply(this.mouseArea.SelectMatrix);
                                        temp2.Reset();
                                        temp2.Multiply(oriMatrix);
                                        temp2.Invert();
                                        ps = (PointF[])orips.Clone();
                                        PointF[] boundsPoints = (PointF[])bounds.Clone();

                                        oriMatrix.TransformPoints(boundsPoints);
                                        oriMatrix.TransformPoints(ps);
                                        start = ps[0];
                                        end = ps[1];
                                        centerPoint = ps[2];

                                        string transform1 = string.Empty;//element.GetAttribute("transform").Trim();

                                        if (this.currentMousePoint != MousePoint.Rotate)
                                        {
                                            temp2.Multiply(this.selectTransform, MatrixOrder.Append);
                                            temp2.Multiply(oriMatrix, MatrixOrder.Append);
                                        }
                                        else
                                        {
                                            temp2.Reset();
                                            temp2.Multiply(temp3);//.TotalTransform.Clone();
                                            temp2.Invert();
                                            temp2.Multiply(this.selectTransform);
                                            temp2.Multiply(temp3);
                                        }
                                        Matrix tempmatrix = transformelement.Transform.FinalMatrix.GetGDIMatrix();
                                        tempmatrix.Multiply(temp2);

                                        doc.AcceptNodeChanged = true;

                                        this.UpdateElement((SVG.SVGStyleable)render, tempmatrix);
                                        tempmatrix.Dispose();
                                        tempmatrix = null;
                                        boundsPoints = null;
                                    }
                                }
                            }
						}
				
						this.mouseArea.SVGDocument.InvokeUndos();
						bool rotate = this.currentMousePoint == MousePoint.Rotate;
						this.mouseArea.TransformSelect(this.selectTransform,rotate?MatrixOrder.Append:MatrixOrder.Prepend);
						doc.AcceptNodeChanged = old;
						#endregion

						if(this.currentMousePoint == MousePoint.Rotate)
							transform.Multiply(this.selectTransform,MatrixOrder.Append);
						else
							transform.Multiply(this.selectTransform);

						PointF center1 = this.mouseArea.oldselectPoint;
						ps = new PointF[]{center1};
						transform.TransformPoints(ps);
						this.mouseArea.ChangeCenterPoint = Point.Round(ps[0]);
						bounds = null;
						ps = null;
						transform.Dispose();
						transform = null;
						matrix.Dispose();
						matrix = null;
						orips = null;
						oriMatrix.Dispose();
						oriMatrix = null;
						this.mouseArea.selectChanged = true;
						this.mouseTime = System.DateTime.Now;

                        //如果是操作子节点，刷新父节点边界
                        if (this.IsChildSelected())
                            this.InvalidateParentBound();
					}
						#endregion

						#region ..选择
					else if(this.oriPoint == MousePoint.None)
					{
//						this.mouseArea.InvalidateSeleciton();
						this.XORDrawPath(this.reversePath);
						PointF end = new Point(e.X,e.Y);
						PointF start = this.startPoint;
						float left = (float)Math.Min(end.X ,start.X);
						float right = (float)Math.Max(end.X,start.X);
						float top = (float)Math.Min(end.Y,start.Y);
						float bottom = (float)Math.Max(end.Y,start.Y);
						RectangleF rect = new RectangleF(left,top,right - left,bottom - top);
						SVG.SVGElement root = this.mouseArea.SVGDocument.CurrentScene;
						if(root is SVG.Interface.ISVGContainer)
						{
							SVG.SVGElementCollection list2 = ((SVG.Interface.ISVGContainer)root).ChildElements;
							SVG.SVGElementCollection list3 = new SVG.SVGElementCollection();
							SVG.SVGElementCollection selection = this.mouseArea.SVGDocument.SelectCollection;
							using(GraphicsPath path1 = new GraphicsPath())
							{
								for(int i = 0;i<list2.Count;i++)
								{
									SVG.Interface.ISVGPathable render = list2[i] as SVG.Interface.ISVGPathable;
									if(render == null || (render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount < 2)
										continue;

                                    SVG.SVGStyleable styleElm = render as SVG.SVGStyleable;
                                    //if ((styleElm.ViewStyle & SVG.ViewStyle.Lock) == SVG.ViewStyle.Lock)
                                    //    continue;
									path1.Reset();
									path1.AddPath((render as SVG.Interface.ISVGPathable).GPath,false);
									path1.Flatten();
                                    path1.Transform(this.mouseArea.GetTotalTransformForElement((SVG.SVGTransformableElement)render));//.TotalTransform);
									RectangleF rect1 = path1.GetBounds();
									if(rect.Contains(rect1))
										list3.Add(render);
								}
							}
							if(list3.Count > 0)
								this.mouseArea.SVGDocument.ChangeSelectElements(list3);
							else
								this.mouseArea.SVGDocument.ChangeSelectElements(null as SVG.SVGElementCollection);
							this.mouseArea.selectChanged = true;
						}
					}
					#endregion
				}
			}
			catch(System.Exception e2)
			{
                this.mouseArea.SVGDocument.OnExceptionOccured(new SVG.ExceptionOccuredEventArgs(new object[] { e2.Message, e2.StackTrace }, SVG.ExceptionLevel.Normal));
			}
			finally
			{
				this.mousedown = false;
				this.useDash = true;
				this.drawGuides = false;
				this.hguides = false;
				this.vguides = false;
				this.vguideindex = this.hguideindex = -1;
				this.reversePath.Reset();
				this.reverseFillPath.Reset();
				this.reverseSnapIndicatorPath.Reset();
				this.anchorIndex = -1;
				this.mouseArea.validContent = true;
				if(e.Button != MouseButtons.Left)
					this.currentMousePoint = MousePoint.None;
			}
			this.ResetColor();
		}
		#endregion

		#region ..OnPaint
		protected override void OnPaint(object sender, PaintEventArgs e)
		{
			base.OnPaint(sender,e);
            //选择子对象时，绘制父边界
            if (this.IsChildSelected())
            {
                SVG.DocumentStructure.SVGGElement group = this.mouseArea.Selection[0].ParentElement as SVG.DocumentStructure.SVGGElement;
                SVG.Interface.ISVGPathable pathable = group as SVG.Interface.ISVGPathable;
                preGroup = group;
                if (group != null && pathable != null)
                {
                    e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                    RectangleF bounds = pathable.GPath.GetBounds();
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        int r = this.mouseArea.grapSize;
                        bounds.Inflate(Canvas.ExpandSelectionSize);
                        path.AddRectangle(bounds);
                        path.Transform(group.TotalTransform);
                        path.Transform(this.mouseArea.CoordTransform);
                        e.Graphics.DrawPath(boundPen, path);
                    }
                }
            }

		}
		#endregion

		#region ..AdaptAttribute
		protected override void OnAdaptAttribute(object sender, AdaptAttributeEventArgs e)
		{

		}
		#endregion

		#region ..GetMousePoint
		MousePoint GetMousePoint(Point screenPoint)
		{
			int index = 0;
			return this.mouseArea.GetMousePos(screenPoint,ref index);
		}
		#endregion

		#region ..mouseArea_DoubleClick
		private void mouseArea_DoubleClick(object sender, EventArgs e)
		{
			Point p = this.mouseArea.PointClientToView(this.mouseArea.PointToClient(Control.MousePosition));
            SVG.SVGTransformableElement pathable = this.GetElement(p) as SVG.SVGTransformableElement;
            SVG.Text.SVGTextBlockElement textBlock = null;
            while (textBlock == null && pathable != null)
            {
                if ((pathable as SVG.SVGTransformableElement).SVGRenderer is SVG.Render.SVGDirectRenderer)
                {
                    textBlock = pathable as SVG.Text.SVGTextBlockElement;
                    if (textBlock == null)
                        textBlock = this.GetElement(p, pathable as SVG.SVGLocatableElement) as SVG.Text.SVGTextBlockElement;
                    if (textBlock != null)
                        this.mouseArea.ChangeToTextEdit(textBlock, p);
                    else
                        this.mouseArea.ChangeToTextEdit(pathable as SVG.SVGTransformableElement, p);
                    return;
                }
                else if (pathable is SVG.DocumentStructure.SVGGElement)
                    pathable = this.GetElement(p, pathable) as SVG.SVGTransformableElement;
            }
		}
		#endregion

		#region ..UpdateElement
		void UpdateElement(SVG.SVGStyleable style,Matrix matrix)
		{
			if(!(style is SVG.BasicShapes.SVGConnectionElement))
			{
				string transform1 = " matrix("+matrix.Elements[0].ToString()+","+matrix.Elements[1].ToString()+","+matrix.Elements[2].ToString()+","+matrix.Elements[3].ToString()+","+matrix.Elements[4].ToString()+","+matrix.Elements[5].ToString()+")";	

				style.InternalSetAttribute("transform",transform1.Trim());
			}
			else
				(style as SVG.BasicShapes.SVGConnectionElement).TransformConnection(matrix);
		}
		#endregion

		#region ..UpdateChildConnects
		/// <summary>
		/// using the matrix to transform the connect childs of the style
		/// </summary>
		/// <param name="style"></param>
		/// <param name="matrix"></param>
		void UpdateChildConnects(SVG.SVGStyleable style,Matrix matrix)
		{
			//Update all connect childs
			bool old = style.OwnerDocument.AcceptNodeChanged;
			style.OwnerDocument.AcceptNodeChanged = false;
			System.Xml.XmlNodeList childs = style.GetElementsByTagName("connect");
			style.OwnerDocument.AcceptNodeChanged = old;
			foreach(SVG.BasicShapes.SVGConnectionElement child in childs)
				child.UpdateControlPoints(matrix);
		}
		#endregion

		#region ..IsScale
		bool IsScale(MousePoint point)
		{
			switch(point)
			{
				case MousePoint.ScaleTopLeft:
				case MousePoint.ScaleTopRight:
				case MousePoint.ScaleBottomLeft:
				case MousePoint.ScaleBottomRight:
				case MousePoint.ScaleMiddleLeft:
				case MousePoint.ScaleMiddleRight:
				case MousePoint.ScaleTopMiddle:
				case MousePoint.ScaleBottomMiddle:
					return true;
			}
			return false;
		}
		#endregion

		#region ..ShiftSnap
		internal override bool ShiftSnap
		{
			get
			{
				return this.currentMousePoint == MousePoint.Translate;
			}
		}
		#endregion

		#region ..mouseArea_Click
		private void mouseArea_Click(object sender, EventArgs e)
		{
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift 
                && validclick && this.oriPoint == MousePoint.Translate)
            {
                SVG.SVGElementCollection list = this.mouseArea.SVGDocument.SelectCollection;
                Point p = this.mouseArea.PointToClient(Control.MousePosition);
                float a = this.mouseArea.SelectedPen.Width;
                this.mouseArea.SelectedPen.Width = 5;
                this.mouseArea.SelectedPen.Alignment = PenAlignment.Center;
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    SVG.Interface.ISVGPathable render = list[i] as SVG.Interface.ISVGPathable;
                    if (render != null)
                    {
                        using (System.Drawing.Drawing2D.GraphicsPath path = (render as SVG.Interface.ISVGPathable).GPath.Clone() as System.Drawing.Drawing2D.GraphicsPath)
                        {
                            path.Transform(this.mouseArea.GetTotalTransformForElement(render as SVG.SVGTransformableElement));//.TotalTransform);
                            this.mouseArea.SelectedPen.Width = (render as SVG.SVGStyleable).StrokeStyle.strokewidth.Value + 2;
                            if (path.IsVisible(p) || path.IsOutlineVisible(p, this.mouseArea.SelectedPen))
                            {
                                list.Remove(render);
                                this.mouseArea.SelectedPen.Width = a;
                                return;
                            }
                        }
                    }
                }
                this.mouseArea.SelectedPen.Width = a;
            }
            else if (validclick)
            {
                //选择子对象
                if (this.mouseArea.SVGDocument.SelectCollection.Count == 1 
                    && this.mouseArea.SVGDocument.SelectCollection[0] is SVG.DocumentStructure.SVGGElement)
                {
                    Point p = this.mouseArea.PointClientToView(this.mouseArea.PointToClient(Control.MousePosition));
                    SVG.DocumentStructure.SVGGElement g = this.mouseArea.SVGDocument.SelectCollection[0] as SVG.DocumentStructure.SVGGElement;
                    using (GraphicsPath path = new GraphicsPath())
                    {
                        foreach (SVG.SVGElement elm in g.ChildElements)
                        {
                            SVG.SVGTransformableElement transformable = elm as SVG.SVGTransformableElement;
                            SVG.Interface.ISVGPathable pathable = elm as SVG.Interface.ISVGPathable;
                            if (transformable != null && pathable != null && pathable.GPath.PointCount > 1)
                            {
                                path.Reset();
                                path.AddPath(pathable.GPath, false);
                                path.Transform(transformable.TotalTransform);
                                if (path.IsVisible(p) || path.IsOutlineVisible(p,this.mouseArea.SelectedPen))
                                {
                                    this.mouseArea.SVGDocument.ChangeSelectElement(transformable);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
		}
		#endregion

        #region ..Selection_CollectionChanged
        void Selection_CollectionChanged(object sender, SVG.CollectionChangedEventArgs e)
        {
            if (this.IsChildSelected())
            {
                this.InvalidateParentBound();
            }
            else if (this.preGroup != null)
            {
                this.InvalidateParentBound(this.preGroup);
                this.preGroup = null;
            }
        }
        #endregion

        #region ..IsChildSelected
        bool IsChildSelected()
        {
            return (this.mouseArea.Selection.Count == 1 && this.mouseArea.Selection[0].ParentElement != this.mouseArea.SVGDocument.CurrentScene);
        }
        #endregion

        #region ..InvalidateParentBound
        void InvalidateParentBound()
        {
            if (this.IsChildSelected())
            {
                SVG.DocumentStructure.SVGGElement group = this.mouseArea.Selection[0].ParentElement as SVG.DocumentStructure.SVGGElement;
                this.InvalidateParentBound(group);
                preGroup = group;
            }
            else
                preGroup = null;
        }

        void InvalidateParentBound(SVG.DocumentStructure.SVGGElement group)
        {
            SVG.Interface.ISVGPathable pathable = group as SVG.Interface.ISVGPathable;
            if (group != null && pathable != null)
            {
                RectangleF bounds = pathable.GPath.GetBounds();
                using (GraphicsPath path = new GraphicsPath())
                {
                    using (Pen pen = new Pen(Color.Black, 3))
                    {
                        pen.Alignment = PenAlignment.Center;
                        int r = this.mouseArea.grapSize;
                        bounds.Inflate(new SizeF(r + 1, r + 1));
                        this.mouseArea.Invalidate(Rectangle.Round(bounds));
                        path.AddRectangle(bounds);
                        path.Widen(pen);
                        using (Region rg = new Region(path))
                        {
                            rg.Transform(group.TotalTransform);
                            rg.Transform(this.mouseArea.CoordTransform);
                            this.mouseArea.Invalidate(rg);
                        }
                    }
                }
            }
        }
        #endregion

        #region ..GetElement
        SVG.SVGElement GetElement(PointF viewPoint)
        {
            return GetElement(viewPoint, null);
        }

        SVG.SVGElement GetElement(PointF viewPoint, SVG.SVGElement parentElement)
        {
            SVG.SVGElement root = (SVG.SVGElement)this.mouseArea.SVGDocument.CurrentScene;
            if (parentElement != null)
                root = parentElement;
            SVG.SVGElementCollection list2 = new SVG.SVGElementCollection();
            if (root is SVG.Interface.ISVGContainer)
                list2.AddRange(((SVG.Interface.ISVGContainer)root).ChildElements);
            //如果当前在操作子节点
            if (this.IsChildSelected())
            {
                SVG.SVGTransformableElement parent = this.mouseArea.Selection[0].ParentElement as SVG.SVGTransformableElement;
                SVG.Interface.ISVGContainer container = parent as SVG.Interface.ISVGContainer;
                if (list2.Contains(parent))
                    list2.Remove(parent);
                if (container != null)
                    list2.InsertRange(0, container.ChildElements);
            }
            using (GraphicsPath path = new GraphicsPath())
            {
                for (int i = list2.Count - 1; i >= 0; i--)
                {
                    if (list2[i] is SVG.Interface.ISVGPathable)
                    {
                        if (this.mouseArea.HitTest(list2[i] as SVG.SVGStyleable, viewPoint))
                        {
                            return list2[i] as SVG.SVGTransformableElement;
                        }
                    }
                }
            }
            return null;
        }
        #endregion
    }
}
