using System;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using YP.SVG.Interface;
using YP.VectorControl.Forms;

namespace YP.VectorControl.Operation
{
	/// <summary>
	/// 实现形状得基本绘制功能
	/// </summary>
	internal class ShapeOperation:Operation//,Interface.IShapeOperation
	{
		#region ..构造及消除
		internal ShapeOperation(Canvas mousearea):base(mousearea)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.openFileDialog1.Filter = "Jpeg文件|*.jpg|PNG文件|*.png|GIF文件|*.gif|SVG文件|*.svg|所有文件|*.*";
			this.openFileDialog1.Title = "选择图片源";
		}

		public override void Dispose()
		{
			this.openFileDialog1.Dispose();
			this.openFileDialog1 = null;
			this.presetShapes = null;
			base.Dispose ();
		}

		#endregion

		#region ..私有变量
        PointF startPoint = PointF.Empty;
        PointF? endPoint = null;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		System.Collections.Hashtable presetShapes = new Hashtable();
		RectangleF pieRect = RectangleF.Empty;
		bool secondMouse = false;
		float startArcAngle = 0;
		float endArcAngle = 0;
		PointF lastPoint = PointF.Empty;
		ArrayList list = new ArrayList();
		float minx = 0;
		float miny = 0;
		#endregion

		#region ..自由曲线控制常量
		/// <summary>
		/// 控制点之间的最小距离
		/// </summary>
		const float minDis = 10;
		/// <summary>
		/// 控制点之间的最大距离
		/// </summary>
		const float maxDis = 100;
		/// <summary>
		/// 控制逼近参量
		/// </summary>
		const float extension = 0.5f;
		#endregion

		#region ..属性
        internal override bool NeedAlignToGrid
        {
            get
            {
                return true;
            }
        }

		/// <summary>
		/// 判断当前是否开始构造扇形
		/// </summary>
		internal bool StartArc
		{
			get
			{
				return this.secondMouse;
			}
		}
		#endregion

		#region ..Mouse Event
		/// <summary>
		/// OnMouseDown
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(object sender,MouseEventArgs e)
		{
			try
			{
				if(!this.IsValidDocument)
					return;

				if(e.Button == MouseButtons.Left)
					this.mousedown = true;
				this.list.Clear();
                this.startPoint = new PointF(e.X, e.Y);
                endPoint = null;
				this.reversePath.Reset();
			}
			catch{}
		}

		/// <summary>
		/// OnMouseMove
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(object sender,MouseEventArgs e)
		{
            try
            {
                if (!this.IsValidDocument)
                    return;
                this.mouseArea.Cursor = this.mouseArea.DefaultCursor;
                if (e.Button == MouseButtons.Left && this.mousedown)
                {
                    this.XORDrawPath(this.reversePath);
                    this.XORDrawPath(this.reverseSnapIndicatorPath);
                    this.reversePath.Reset();
                    this.reverseSnapIndicatorPath.Reset();

                    PointF current = new PointF(e.X, e.Y);

                    #region ..吸附到
                    bool pie = OperatorHelper.IsPieOperator(this.mouseArea.CurrentOperator);
                    if ((!pie || (pie && !this.secondMouse)) && (this.mouseArea.CurrentOperator != Operator.Line))
                    {
                        this.reversePath.AddLine(this.startPoint, current);
                        RectangleF bounds = this.reversePath.GetBounds();
                        ElementAlign align = ElementAlign.Middle | ElementAlign.Center;
                        if (e.X > this.startPoint.X)
                            align = align | ElementAlign.Right;
                        else
                            align = align | ElementAlign.Left;

                        if (e.Y > this.startPoint.Y)
                            align = align | ElementAlign.Bottom;
                        else
                            align = align | ElementAlign.Top;

                        AlignResult[] results = this.AlignToElement(this.reverseSnapIndicatorPath, this.reversePath, align, null,false,false,false);

                        if (results != null)
                        {
                            using (Matrix temp = new Matrix())
                            {
                                temp.Translate(-startPoint.X, -startPoint.Y);
                                foreach (AlignResult result in results)
                                {
                                    if (result.Horizontal && bounds.Width > 0 )
                                    {
                                        float value = result.Value;
                                        float width = value - bounds.Left;
                                        if (result.SourcePos == ElementAlign.Left)
                                            width = bounds.Right - value;
                                        else if (result.SourcePos == ElementAlign.Center)
                                        {
                                            width = 2 * (float)Math.Abs(value - startPoint.X);
                                        }
                                        float scalex = width / bounds.Width;
                                        temp.Scale(scalex, 1, MatrixOrder.Append);
                                    }
                                    else if(bounds.Height > 0)
                                    {
                                        float value = result.Value;
                                        float width = value - bounds.Top;
                                        if (result.SourcePos == ElementAlign.Top)
                                            width = bounds.Bottom - value;
                                        else if (result.SourcePos == ElementAlign.Middle)
                                            width = 2 * (float)Math.Abs(value - startPoint.Y);

                                        float scaleY = width / bounds.Height;
                                        temp.Scale(1, scaleY, MatrixOrder.Append);
                                    }
                                }
                                temp.Translate(startPoint.X, startPoint.Y, MatrixOrder.Append);
                                this.reversePath.Transform(temp);
                                bounds = this.reversePath.GetBounds();
                                float x = bounds.Right, y = bounds.Bottom;
                                if (e.Y <= this.startPoint.Y)
                                    y = bounds.Top;
                                if (e.X <= this.startPoint.X)
                                    x = bounds.Left;

                                this.reversePath.Reset();
                                endPoint = current = new PointF(x, y);
                            }
                        }
                        else
                            endPoint = null;
                    }
                    #endregion

                    if (this.list.Count > 0)
                    {
                        PointF p = (PointF)this.list[this.list.Count - 1];
                        if (Math.Sqrt(Math.Pow(current.X - p.X, 2) + Math.Pow(current.Y - p.Y, 2)) > 6)
                        {
                            this.list.Add(current);
                            this.minx = (int)Math.Min(this.minx, current.X);
                            this.miny = (int)Math.Min(current.Y, this.miny);
                        }
                    }
                    this.CreateDrawPath(this.startPoint, current, this.mouseArea.CurrentOperator);

                }
                else if (OperatorHelper.IsPieOperator(this.mouseArea.CurrentOperator) && this.secondMouse)
                {
                    this.XORDrawPath(this.reversePath);
                    this.reversePath.Reset();
                    this.CreateDrawPath(this.startPoint, new PointF(e.X, e.Y), this.mouseArea.CurrentOperator);
                }
               
                this.XORDrawPath(this.reversePath);
                this.XORDrawPath(this.reverseSnapIndicatorPath);
            }
            catch (System.Exception e1)
            {
                this.mouseArea.SVGDocument.OnExceptionOccured(new SVG.ExceptionOccuredEventArgs(new object[] { e1.Message, e1.StackTrace }, SVG.ExceptionLevel.Normal));
            }
		}

		/// <summary>
		/// OnMouseUp
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(object sender,MouseEventArgs e)
		{
			try
			{
				base.OnMouseUp(sender,e);
				if(!this.IsValidDocument)
					return;
                this.XORDrawPath(this.reversePath);
                this.XORDrawPath(this.reverseSnapIndicatorPath);
				if(e.Button == MouseButtons.Left && this.mousedown)
				{
					bool pie = OperatorHelper.IsPieOperator(this.mouseArea.CurrentOperator);
					if(!pie || (pie && this.secondMouse))
					{
						SVG.Interface.ISVGPathable path = this.CreateGraph(this.startPoint, this.endPoint.HasValue ? this.endPoint.Value : new PointF(e.X,e.Y),this.mouseArea.CurrentOperator);
						if(path != null)
						{
							bool old = this.mouseArea.SVGDocument.AcceptNodeChanged;
							this.mouseArea.SVGDocument.AcceptNodeChanged = true;
							this.mouseArea.AddElement((SVG.SVGElement)path);
							this.mouseArea.SVGDocument.AcceptNodeChanged = old;
						}
                        this.endPoint = null;
                        this.reversePath.Reset();
                        this.reverseSnapIndicatorPath.Reset();
					}
					if(pie)
						this.secondMouse = !this.secondMouse;
					else
						this.secondMouse = false;
				}
				this.mousedown = false;
                this.XORDrawPath(this.reversePath);
                this.XORDrawPath(this.reverseSnapIndicatorPath);
				this.mouseArea.validContent = true;
			}
			catch(System.Exception e1)
			{
                this.mouseArea.SVGDocument.OnExceptionOccured(new SVG.ExceptionOccuredEventArgs(new object[] { e1.Message, e1.StackTrace }, SVG.ExceptionLevel.Normal));
			}
		}
		#endregion

		#region ..绘制事件
		/// <summary>
		/// OnPaint
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnPaint(object sender,PaintEventArgs e)
		{
			this.DrawXorPath(e);
		}
		#endregion

		#region ..改变属性事件
		protected override void OnAdaptAttribute(object sender,AdaptAttributeEventArgs e)
		{
		}
		#endregion

		#region ..根据当前绘图模式和开始结束点创建路径
		/// <summary>
		/// 根据当前绘图模式和开始结束点创建路径
		/// </summary>
		/// <param name="endpoint">结束点</param>
		/// <param name="startpoint">开始点</param>
		/// <param name="operation">操作方式</param>
		void CreateDrawPath(PointF startpoint,PointF endpoint,Operator op)
		{
			startpoint = this.mouseArea.PointToVirtualView(startpoint);
			endpoint = this.mouseArea.PointToVirtualView(endpoint);
			using(GraphicsPath path =  new GraphicsPath())
			{
				float left = (float)Math.Min(startpoint.X,endpoint.X);
				float top = (float)Math.Min(startpoint.Y,endpoint.Y);
				float right = (float)Math.Max(startpoint.X,endpoint.X);
				float bottom = (float)Math.Max(startpoint.Y,endpoint.Y);
				float width = right - left;
				float height = bottom - top;
				float r = 0,rx = 0,ry = 0;
				float angle = 0;
				switch(op)
				{
						#region ..矩形
					case Operator.Rectangle:
						angle = this.mouseArea.RadiusOfRectangle;
						rx = angle;
						ry = angle;
						if((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
						{
							width = height = (float)Math.Max(width,height);
							left = startpoint.X - endpoint.X < 0 ? startpoint.X:startpoint.X - height;
							top = startpoint.Y - endpoint.Y < 0?startpoint.Y : startpoint.Y - width;
						}

						RectangleF rect = new RectangleF(left,top,width,height);

						if(width < 0.5f || height < 0.5f)
							return;

						if(rx == 0F) rx = ry;
						else if(ry == 0F) ry = rx;

						rx = Math.Min(rect.Width/2, rx);
						ry = Math.Min(rect.Height/2, ry);

						if(rx == 0 && ry == 0)
						{
							path.AddRectangle(rect);
						}
						else
						{
							float a = rect.X + rect.Width - rx;
							path.AddLine(rect.X + rx, rect.Y, a, rect.Y);
							path.AddArc(a-rx, rect.Y, rx*2, ry*2, 270, 90);
				
							float b = rect.Y + rect.Height - ry;

							path.AddLine(rect.Right, rect.Y + ry, right, b);
							path.AddArc(rect.Right - rx*2, b-ry, rx*2, ry*2, 0, 90);
				
							path.AddLine(rect.Right - rx, rect.Y + rect.Height, rect.X + rx, rect.Y + rect.Height);
							path.AddArc(rect.X, b-ry, rx*2, ry*2, 90, 90);
				
							path.AddLine(rect.X, b, rect.X, rect.Y + ry);
							path.AddArc(rect.X, rect.Y, rx*2, ry*2, 180, 90);
							path.CloseFigure();
						}
						break;
						#endregion

						#region ..椭圆
					case Operator.Ellipse:
					case Operator.Pie:
					case Operator.Arc:
						if(!this.secondMouse)
						{
							this.lastPoint = endpoint;
							if(Control.ModifierKeys == Keys.Shift)
							{
								r = (float)Math.Max(right - left,bottom - top);
								path.AddEllipse((startpoint.X - endpoint.X < 0 ? startpoint.X:startpoint.X - r),(startpoint.Y - endpoint.Y < 0?startpoint.Y : startpoint.Y - r),r,r);
							}
							else if(Control.ModifierKeys == (Keys.Shift | Keys.Control))
							{
								r = (float)Math.Max(right - left,bottom - top);
								left = startpoint.X - r;
								top = startpoint.Y - r;
								path.AddEllipse(left,top,2 * r,2 * r);
							}
							else
							{
								if(Control.ModifierKeys == Keys.Control)
								{
									left = startpoint.X - Math.Abs(endpoint.X - startpoint.X);
									top = startpoint.Y - Math.Abs(endpoint.Y - startpoint.Y);
									width = (float)Math.Abs(endpoint.X - startpoint.X) * 2;
									height = (float)Math.Abs(endpoint.Y - startpoint.Y) * 2;
								}
						
								path.AddEllipse(left,top,width,height);
							}
							this.pieRect = path.GetBounds();
						}
							//第二次，构造扇形
						else
						{
							if(!this.pieRect.IsEmpty)
							{
								startpoint = new PointF(this.pieRect.X + this.pieRect.Width / 2f,this.pieRect.Y + this.pieRect.Height / 2);
								PointF sweeppoint = endpoint;
								endpoint = this.lastPoint;//new PointF(this.pieRect.X + this.pieRect.Width,this.pieRect.Y + this.pieRect.Height);
								float startangle = this.GetAngle(startpoint,endpoint);
								float endangle = this.GetAngle(startpoint,sweeppoint);
								endangle = endangle - startangle;
								if(endangle <= 0)
									endangle += 360;
								if(endangle == 360)
									path.AddEllipse(this.pieRect.X,this.pieRect.Y,this.pieRect.Width,this.pieRect.Height);
								else
									path.AddPie(this.pieRect.X,this.pieRect.Y,this.pieRect.Width,this.pieRect.Height,startangle,endangle);
								this.startArcAngle = startangle;
								this.endArcAngle = endangle + startangle;
							}
						}
						break;
						#endregion

						#region ..直线
					case Operator.Line:
//						if(Math.Abs(startpoint.X - endpoint.X ) > 2  || Math.Abs(startpoint.Y - endpoint.Y ) > 2 )
//						{
							
//							if(list.Count == 0)
//								list.Add(startpoint);
//							list.Add(endpoint);
//							PointF[] ps = new PointF[list.Count];
//							list.CopyTo(ps);
//							path.AddLines(ps);
							path.AddLine(startpoint,endpoint);
//						}
						break;
						#endregion

						#region ..正多边形
					case Operator.Star:
						int number = this.mouseArea.Star.NumberOfVertexes;
//						number = (int)Math.Max(3,((SVGDom.BasicShapes.SVGPolygonElement)this.mouseArea.preGraph).LineNumber);
						double j1 = 2 * Math.PI / number;
						GraphicsPath temp1 = new GraphicsPath();
						double x = 0,y=0;
						double r1 = Math.Sqrt(Math.Pow(startpoint.X - endpoint.X,2) + Math.Pow(startpoint.Y - endpoint.Y,2));
						float indent = this.mouseArea.Star.Indent;//(float)Math.Round(((SVGDom.BasicShapes.SVGPolygonElement)this.mouseArea.preGraph).Indent,1);
						int length = number;
						if(indent <1)
							length = number * 2;
						Point[] points = new Point[length];
						if(r1>0)
						{
							double startangle = Math.Asin((endpoint.Y - startpoint.Y) / r1);
							if(endpoint.X < startpoint.X)
								startangle = Math.PI - startangle;
							Point indentPoint = Point.Empty;
							double j3;
							for(int i = 0; i < length ;i+=(length / number))
							{	
								j3 = startangle + j1 * (i / (length / number));
								x = r1 * Math.Cos(j3);
								y = r1 * Math.Sin(j3);

								points[i] = new Point((int)(startpoint.X + x ),(int)(startpoint.Y + y));
								if(length == 2 * number)
								{
									if(!indentPoint.IsEmpty)
									{
										Point temppoint = new  Point((indentPoint.X + points[i].X ) / 2,(indentPoint.Y + points[i].Y ) /2);
										temppoint = new Point((int)(startpoint.X + indent * (temppoint.X - startpoint.X )),(int)(startpoint.Y + indent * (temppoint.Y - startpoint.Y)));
										points[i - 1] = temppoint;
									}
									indentPoint = points[i];
									if(i == 2 * number - 2)
									{
										Point temppoint = new  Point((points[0].X + points[i].X ) / 2,(points[0].Y + points[i].Y ) /2);
										temppoint = new Point((int)(startpoint.X + indent * (temppoint.X - startpoint.X )),(int)(startpoint.Y + indent * (temppoint.Y - startpoint.Y)));
										points[i + 1] = temppoint;
									}
								}
							}
							path.AddPolygon(points);
							//						path.StartFigure();
							//						path.AddLine(startpoint,endpoint);
						}
						break;
						#endregion

						#region ..图片
					case Operator.Image:
//						if((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
//							width = height = (float)Math.Min(width,height);
						path.AddRectangle(new RectangleF(left,top,width,height));
						break;
						#endregion

						#region ..自由曲线
//					case Operator.Pencil:
//						if(this.list.Count >1)
//						{
//							path.Reset();
//							PointF[] ps = new PointF[this.list.Count];
//							this.list.CopyTo(ps);
////							this.minx = (int)Math.Min(this.minx,endpoint.X);
////							this.miny = (int)Math.Min(this.miny,endpoint.Y);
//							path.AddLines(ps);
//							ps = null;
//						}
//						break;
						#endregion

						#region ..预设
					case Operator.Shape:
						if(this.mouseArea.templateShape != null)
						{
							left = startpoint.X;
							top = startpoint.Y;
							right = endpoint.X;
							bottom = endpoint.Y;

							width = right - left;
							height = bottom - top;
                            path.AddPath((this.mouseArea.templateShape as IOutlookBarPath).GPath, false);
							RectangleF rect2 = SVG.DocumentStructure.SVGGElement.GetBounds(path);
							using(Matrix matrix = new Matrix())
							{
								matrix.Translate(left - rect2.X,top - rect2.Y);
								matrix.Translate(rect2.X ,rect2.Y );
								matrix.Scale((float)width/ (float)rect2.Width,(float)height /(float)rect2.Height);
								matrix.Translate(-rect2.X ,-rect2.Y);
								path.Transform(matrix);
							}
						}
						break;
						#endregion

				}
				this.reversePath = (GraphicsPath)path.Clone();
//				if(this.mouseArea.CurrentOperator != Operator.Pencil)
					this.reversePath.Transform(this.mouseArea.CoordTransform);
			}
		}
		#endregion

		#region ..根据当前绘图模式和开始结束点创建Graph对象
		/// <summary>
		/// 根据当前绘图模式和开始结束点创建Graph对象
		/// </summary>
		/// <param name="endpoint">结束点</param>
		/// <param name="startpoint">开始点</param>
		/// <param name="operation">操作方式</param>
		SVG.Interface.ISVGPathable CreateGraph(PointF startpoint,PointF endpoint,Operator op)
		{
			PointF start = this.mouseArea.PointToVirtualView(startpoint);
			PointF end = this.mouseArea.PointToVirtualView(endpoint);
			float left = (float)Math.Round(Math.Min(start.X,end.X),2) ;
			float top = (float)Math.Round(Math.Min(start.Y,end.Y),2);
			float right = (float)Math.Round(Math.Max(start.X,end.X),2);
			float bottom = (float)Math.Round(Math.Max(start.Y,end.Y) ,2);
			float width = right - left;
			float height = bottom - top;
			float rx = 0,ry = 0;
			float scale = this.mouseArea.ScaleRatio;

			SVG.SVGElement path = null;
			SVG.Document.SVGDocument doc = this.mouseArea.SVGDocument;
			bool old = doc.AcceptNodeChanged;
			doc.AcceptNodeChanged = false;
			switch(op)
			{
					#region ..椭圆（圆）
				case Operator.Ellipse:

					if(Control.ModifierKeys == Keys.Shift)
					{
						float r = (float)Math.Max(right - left,bottom - top) /(2f);
						if( r == 0)
							return null;
						left = (start.X - end.X < 0 ? start.X:start.X - 2*r);
						top = (start.Y - end.Y < 0 ? start.Y :start.Y - 2* r);
						float cx = left + r;
						float cy = top + r;
						path = doc.CreateElement("circle")  as SVG.SVGElement;
						SVG.BasicShapes.SVGCircleElement c = (SVG.BasicShapes.SVGCircleElement)path;
						
						c.InternalSetAttribute("cx",cx.ToString());
						c.InternalSetAttribute("cy",cy.ToString());
						c.InternalSetAttribute("r",r.ToString());
						
					}
					else if(Control.ModifierKeys == (Keys.Control | Keys.Shift))
					{
						path = doc.CreateElement("circle") as SVG.SVGElement;
						SVG.BasicShapes.SVGCircleElement c = (SVG.BasicShapes.SVGCircleElement)path;
						float r = (float)Math.Max(Math.Abs(start.Y - end.Y),Math.Abs(start.X - end.X));
						if(r == 0)
							return null;

						c.InternalSetAttribute("cx",start.X.ToString());
						c.InternalSetAttribute("cy",start.Y.ToString());
						c.InternalSetAttribute("r",r.ToString());
						
					}
					else
					{
						path = doc.CreateElement("ellipse") as SVG.SVGElement;
						SVG.BasicShapes.SVGEllipseElement e = (SVG.BasicShapes.SVGEllipseElement)path;
						if(Control.ModifierKeys == Keys.Control)
						{
							e.InternalSetAttribute("cx",start.X.ToString());
							e.InternalSetAttribute("cy",start.Y.ToString());
							e.InternalSetAttribute("rx",Math.Round(end.X - start.X,0).ToString());
							e.InternalSetAttribute("ry",Math.Round(end.Y - start.Y,0).ToString());
						
						}
						else
						{
							float cx = left + width/2f;
							float cy = top + height/ 2f;
							rx = width / 2f;
							ry = height /2f;

							e.InternalSetAttribute("cx",cx.ToString());
							e.InternalSetAttribute("cy",cy.ToString());
							e.InternalSetAttribute("rx",rx.ToString());
							e.InternalSetAttribute("ry",ry.ToString());
						
						}
					}
					break;
					#endregion

					#region ..矩形
				case Operator.Rectangle:
					float angle = this.mouseArea.RadiusOfRectangle;
//					angle = (int)Math.Max(0,((SVGDom.BasicShapes.SVGRectElement)this.mouseArea.preGraph).Angle);
					rx = angle;
					ry = angle;
					if(width == 0 || height == 0)
						return null;
					if((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
					{
						width = height = (float)Math.Max(width,height);
						left = (start.X - end.X < 0 ? start.X:start.X - width);
						top = (start.Y - end.Y < 0 ? start.Y :start.Y - height);
					}

					if(rx > width/2)
					{
						rx = width;
					}

					if(ry > height/2)
					{
						ry = height;
					}
					path = doc.CreateElement("rect") as SVG.SVGElement;
					SVG.BasicShapes.SVGRectElement rect1 = (SVG.BasicShapes.SVGRectElement)path;
					rect1.InternalSetAttribute("x",rect1.NamespaceURI,left.ToString());
					rect1.InternalSetAttribute("y",top.ToString());
					rect1.InternalSetAttribute("width",width.ToString());
					rect1.InternalSetAttribute("height",height.ToString());

					if(rx > 0 || ry > 0)
					{
						rect1.InternalSetAttribute("rx",rx.ToString());
						rect1.InternalSetAttribute("ry",ry.ToString());
					}

					if(rx > 0 || ry > 0)
					{
					}
					break;
					#endregion

					#region ..正多边形
				case Operator.Star:
					int number = this.mouseArea.Star.NumberOfVertexes;
//					number = (int)Math.Max(3,((SVGDom.BasicShapes.SVGPolygonElement)this.mouseArea.preGraph).LineNumber);
					double j1 = 2 * Math.PI / number;
					GraphicsPath temp1 = new GraphicsPath();
					double x = 0,y=0;
					double r1 = Math.Sqrt(Math.Pow(start.X - end.X,2) + Math.Pow(start.Y - end.Y,2));
					float indent = this.mouseArea.Star.Indent;//(float)Math.Round(((SVGDom.BasicShapes.SVGPolygonElement)this.mouseArea.preGraph).Indent,1);
					int length = number;
					if(indent < 1)
						length = number * 2;
					PointF[] points1 = new PointF[length];

					if(r1>0)
					{
						double startangle = Math.Asin((end.Y - start.Y) / r1);
						if(end.X < start.X)
							startangle = Math.PI - startangle;
						PointF indentPoint = PointF.Empty;
						double j3;
						for(int i = 0; i < length ;i+=(length / number))
						{	
							j3 = startangle + j1 * (i / (length / number));
							x = r1 * Math.Cos(j3);
							y = r1 * Math.Sin(j3);

							points1[i] = new PointF((int)(start.X + x ),(int)(start.Y + y));
							if(length == 2 * number)
							{
								if(!indentPoint.IsEmpty)
								{
									PointF temppoint = new  PointF((indentPoint.X + points1[i].X ) / 2,(indentPoint.Y + points1[i].Y ) /2);
									temppoint = new PointF((int)(start.X + indent * (temppoint.X - start.X )),(int)(start.Y + indent * (temppoint.Y - start.Y)));
									points1[i - 1] = temppoint;
								}
								indentPoint = points1[i];
								if(i == 2 * number - 2)
								{
									PointF temppoint = new  PointF((points1[0].X + points1[i].X ) / 2,(points1[0].Y + points1[i].Y ) /2);
									temppoint = new PointF((int)(start.X + indent * (temppoint.X - start.X )),(int)(start.Y + indent * (temppoint.Y - start.Y)));
									points1[i + 1] = temppoint;
								}
							}
						}
						string text = string.Empty;
						for(int i = 0;i<points1.Length;i++)
						{
							PointF p = points1[i];
							text += p.X.ToString()+" " + p.Y.ToString();
							if(i < points1.Length - 1)
								text += ",";
						}
						path = doc.CreateElement("polygon") as SVG.SVGElement;
						SVG.BasicShapes.SVGPolygonElement poly = (SVG.BasicShapes.SVGPolygonElement)path;
						poly.InternalSetAttribute("points",text);
						poly.InternalSetAttribute("fill-rule","evenodd");
					}
					else 
						return null;
					break;
					#endregion

					#region ..直线
				case Operator.Line:
//					if(Math.Abs(start.X - end.X ) > 2 * scale || Math.Abs(start.Y - end.Y ) > 2 * scale)
//					{
//						if((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
//						{
//							float k = (float)Math.Atan2(end.Y - start.Y,end.X - start.X) * 180 / (float)Math.PI;
//							k = (int)Math.Round(k/ 45f,0) * 45;
//							if(k == 90 || k == -90)
//								end = new PointF(start.X,end.Y);
//							else
//							{
//								k = (float)Math.Tan(k / 180f * Math.PI);
//								end = new PointF(end.X ,start.Y + ((end.X - start.X ) * k));
//							}
//						}
						path = doc.CreateElement("line") as SVG.SVGElement;
						SVG.BasicShapes.SVGLineElement l = (SVG.BasicShapes.SVGLineElement)path;
						l.InternalSetAttribute("x1",start.X.ToString());
						l.InternalSetAttribute("y1",start.Y.ToString());
						l.InternalSetAttribute("x2",end.X.ToString());
						l.InternalSetAttribute("y2",end.Y.ToString());
//					}
					break;
					#endregion

					#region ..图片
				case Operator.Image:
					path = null;
                    Form form = this.mouseArea.FindForm();
                    if (this.openFileDialog1.ShowDialog(form) == DialogResult.OK)
					{
						string filename = this.openFileDialog1.FileName;
						if(System.IO.File.Exists(filename))
						{
							path = (SVG.DocumentStructure.SVGImageElement)doc.CreateElement("image");
							SVG.DocumentStructure.SVGImageElement image = (SVG.DocumentStructure.SVGImageElement)path;
							image.InternalSetAttribute("x",left.ToString());
							image.InternalSetAttribute("y",top.ToString());
							image.InternalSetAttribute("width",width.ToString());
							image.InternalSetAttribute("height",height.ToString());
							image.InternalSetAttribute("href",SVG.Document.SVGDocument.XLinkNamespace,filename.ToString());
						}
					}
					break;
					#endregion

					#region ..预设
				case Operator.Shape:
					if(width < 1 && height < 1)
						break;
					path = (SVG.SVGElement)doc.CreateElement("path");
					if(this.mouseArea.templateShape != null)
					{
						left = start.X;
						top = start.Y;
						right = end.X;
						bottom = end.Y;

						width = right - left;
						height = bottom - top;
                        GraphicsPath path2 = (this.mouseArea.templateShape as IOutlookBarPath).GPath.Clone() as GraphicsPath;
						RectangleF rect2 = SVG.DocumentStructure.SVGGElement.GetBounds(path2);
						using(Matrix matrix = new Matrix())
						{
							matrix.Translate(left - rect2.X,top - rect2.Y);
							matrix.Translate(rect2.X ,rect2.Y );
							matrix.Scale((float)width/ (float)rect2.Width,(float)height /(float)rect2.Height);
							matrix.Translate(-rect2.X ,-rect2.Y);
							path.InternalSetAttribute("d",this.mouseArea.templateShape.GetAttribute("d"));

							if(!matrix.IsIdentity)
							{
								string transform = new SVG.DataType.SVGMatrix(matrix).ToString();
								path.InternalSetAttribute("transform",transform);
								transform = null;
							}
						}
						path2.Dispose();
						path2 = null;
					}
					break;
					#endregion

					#region ..扇形
				case Operator.Pie:
				case Operator.Arc:
					if(!this.pieRect.IsEmpty)
					{
						path = doc.CreateElement("path") as SVG.SVGElement;
						SVG.SVGElement element = (SVG.SVGElement)path;
						startpoint = new PointF(this.pieRect.X + this.pieRect.Width / 2f,this.pieRect.Y + this.pieRect.Height / 2);
						PointF sweeppoint = this.mouseArea.PointToVirtualView(endpoint);
						endpoint = this.lastPoint;//new PointF(this.pieRect.X + this.pieRect.Width,this.pieRect.Y + this.pieRect.Height);
						float startangle = this.startArcAngle;//this.GetAngle(startpoint,endpoint);
						float endangle = this.endArcAngle;//this.GetAngle(startpoint,sweeppoint);
						if(endangle > 360)
							endangle -= 360;
						//x=rx * cos(a) + cx;y=ry * sin(a) + cy;
						float rx1 = this.pieRect.Width / 2f;
						float ry1 = this.pieRect.Height / 2f;
						float cx = startpoint.X;
						float cy = startpoint.Y;
						string largeflag = (endangle - startangle > 180 || endangle < startangle) ? "1":"0";
						startangle = (float)(startangle/180f * Math.PI);
						endangle = (float)(endangle / 180f * Math.PI);
						PointF temp = this.GetCrossPoint(this.pieRect,startpoint,endpoint);
//						float x1= (float)(cx + rx1 * Math.Cos(startangle));
//						float y1 = (float)(cy + ry1 * Math.Sin(startangle));
						float x1 = temp.X;
						float y1 = temp.Y;
						temp = this.GetCrossPoint(this.pieRect,startpoint,sweeppoint);
//						float x2 = (float)(cx + rx1 * Math.Cos(endangle));
//						float y2 = (float)(cy + ry1 * Math.Sin(endangle));
						float x2 = temp.X;
						float y2 = temp.Y;
						
						string pathdata = "M"+cx.ToString() + " " +cy.ToString()+"L"+x1.ToString()+" "+y1.ToString()+"A"+rx1.ToString()+" "+ry1.ToString()+" 0 " + largeflag+" 1 " + x2.ToString()+" "+y2.ToString()+"Z";
//						pathdata += "M" + cx.ToString() +" " + cy.ToString() +"L"+ endpoint.X.ToString() + " "+endpoint.Y.ToString();
//						pathdata += "M" + cx.ToString() +" " + cy.ToString() +"L"+ x1.ToString() + " "+y1.ToString();
//
//						pathdata += "M" + cx.ToString() +" " + cy.ToString() +"L"+ sweeppoint.X.ToString() + " "+sweeppoint.Y.ToString();
//						pathdata += "M" + cx.ToString() +" " + cy.ToString() +"L"+ x2.ToString() + " "+y2.ToString();
						if(op == Operator.Arc)
							pathdata = "M"+x1.ToString()+" "+y1.ToString()+"A"+rx1.ToString()+" "+ry1.ToString()+" 0 " + largeflag+" 1 " + x2.ToString()+" "+y2.ToString();
                        element.InternalSetAttribute("d",pathdata);
						this.pieRect = RectangleF.Empty;
					}
					break;
					#endregion

					#region ..自由曲线
//				case Operator.Pencil:
//					if(this.list.Count > 1)
//					{
//						//提取控制点,在x = minx,y = miny控制的范围中，提取x或y方向上的转折点
//						ArrayList list1 = new ArrayList();
////						PointF[] ps = new PointF[list.Count];
////						list.CopyTo(ps);
//
//						list1.Add(this.list[0]);
//						bool xlower = false;
//						float oldx = ((PointF)this.list[0]).X - this.minx;
//						bool ylower = false;
//						float oldy = ((PointF)this.list[0]).Y - this.miny;
//						int pre = 0;
//						for(int i = 1;i<this.list.Count;i++)
//						{
//							PointF p = (PointF)this.list[i];
//							float xdis = p.X - this.minx;
//							float ydis = p.Y - this.miny;
//							bool xl = xdis < oldx;
//							bool yl = ydis < oldy;
//							bool add = (xl != xlower || yl != ylower) && i > 1;
//							if(add || i == this.list.Count - 1)
//							{
//								PointF tempp = (PointF)list1[list1.Count - 1];
//								float dis = (float)Math.Sqrt(Math.Pow(tempp.X - p.X,2) + Math.Pow(tempp.Y - p.Y,2));
//								if(dis < minDis&& i != this.list.Count -1)
//									continue;
//								//转折点为前一个点
//								int h = i - 1;
//								if(h == pre)
//								{
//									pre = i;
//									list.Add(list[i]);
//								}
//								else
//								{
//									p = (PointF)list[i - 1];
//									int a = (int)(dis / maxDis);
//									bool insert = false;
//									//如果两个控制点之间距离大于maxDis，从中提取控制点加入
//									if(a > 1)
//									{
//										PointF prep = tempp;
//										dis = dis / (a + 2f); 
//										//按照等分距离规则，从中提取控制点
//										for(int j = pre + 1;j < h;j++)
//										{
//											PointF tp =(PointF)list[j];
//											if(Math.Sqrt(Math.Pow(tp.X - prep.X ,2) +Math.Pow(tp.Y - prep.Y,2)) >= dis && Math.Sqrt(Math.Pow(tp.X - p.X,2) + Math.Pow(tp.Y - p.Y,2)) >= dis)
//											{
//												list1.Add(tp);
//												prep = tp;
//												insert = true;
//											}
//										}
//									}
//									//如果距离不超过最大距离，提起其重点控制曲线形状
//									if(dis > 30 && !insert)
//									{
//										int mid = (int)((pre + h) / 2);
//										if(mid != pre && mid != h)
//										{
//											PointF tempp1 = (PointF)list[mid];
//											if(Math.Sqrt(Math.Pow(tempp1.X - tempp.X,2) + Math.Pow(tempp1.Y - tempp.Y,2)) >15  && Math.Sqrt(Math.Pow(p.X - tempp1.X,2) + Math.Pow(p.Y - tempp1.Y,2)) > 15)
//												list1.Add(list[mid]);
//										}
//									}
//									pre = h;
//									list1.Add(p);
//								}
//							}
//							oldx = xdis;
//							oldy = ydis;
//							xlower = xl;
//							ylower = yl;
//						}
//						PointF p1 = (PointF)list1[0];
//						PointF p2 = (PointF)this.list[this.list.Count - 1];
//						bool close = false;
//						if(Math.Sqrt(Math.Pow(p1.X - p2.X,2) + Math.Pow(p1.Y - p2.Y,2)) <= 10 && list1.Count > 2)
//						{
//							close = true;
//							if(list1.Count > 3)
//								list1.RemoveAt(list1.Count - 1);
//						}
//
//						if(list1.Count > 1)
//						{
//							//利用GDI+的Curve曲线，近似逼近曲线
//							PointF[] ps = new PointF[list1.Count];
//							list1.CopyTo(ps);
//							string c = string.Empty;
//							using(GraphicsPath path1 = new GraphicsPath())
//							{
//								if(!close)
//									path1.AddCurve(ps,extension);
//								else
//									path1.AddClosedCurve(ps,extension);
//								using(Matrix m = this.mouseArea.CoordTransform.Clone())
//								{
//									m.Invert();
//									path1.Transform(m);
//								}
//								c = SVGDom.Paths.SVGPathElement.GetPathString(path1);
//							}
//							if(c.Length > 0)
//							{
//								path = doc.CreateElement("path") as SVGDom.SVGElement;
//								((SVGDom.SVGElement)path).AddSVGAttribute("d",c);
//								((SVGDom.SVGElement)path).InternalSetAttribute("d",c);
//							}
//							c = null;
//							ps = null;
//						}
//					}
//					break;
					#endregion

			}
			doc.AcceptNodeChanged = old;
			return path as SVG.Interface.ISVGPathable;
		}
		#endregion

		#region ..路径转字符串
		/// <summary>
		/// 获取表示路径数据的字符传
		/// </summary>
		/// <param name="path">路径</param>
		/// <returns>路径数据字符串</returns>
		string GetPathString(System.Drawing.Drawing2D.GraphicsPath path)
		{
			string text = String.Empty;
			System.Text.StringBuilder t = new System.Text.StringBuilder();
			for(int i=0;i<path.PointCount;i++)
			{
				PointF point = path.PathPoints[i];
				byte type = path.PathTypes[i];
				switch(type)
				{
					case (byte)PathPointType.Start:
						text += "M ";
						text += point.X.ToString()+" "+point.Y.ToString()+" ";
						break;
					case (byte)PathPointType.Line:
						text += "L ";
						text += point.X.ToString()+" "+point.Y.ToString()+" ";
						break;
					case (byte)PathPointType.Bezier3:
						text += "C ";
						for(int j=i;j<= System.Math.Min(i+2,path.PathPoints.Length-1);j++)
						{
							point = path.PathPoints[j];
							text += point.X.ToString() + " " + point.Y.ToString()+" ";
							if(path.PathTypes[j] == 131)
								text += "Z";
						}
						i=  System.Math.Min(i+2,path.PathPoints.Length-1);
						break;
					case (byte)PathPointType.CloseSubpath:
						text += "Z";
						break;
					case 131:
						text += "C ";
						for(int j=i;j<= System.Math.Min(i+2,path.PathPoints.Length-1);j++)
						{
							point = path.PathPoints[j];
							text += point.X.ToString() + " " + point.Y.ToString()+" ";
						}
						text += "Z";
						i=  System.Math.Min(i+2,path.PathPoints.Length-1);
						break;
					case 129:
						text += "L ";
						text += point.X.ToString()+" "+point.Y.ToString()+" ";
						text += "Z";
						break;
				}
			}

			return text;
		}
		#endregion

		#region ..获取直线与椭圆的交点
		PointF GetCrossPoint(RectangleF ellipseRect,PointF start,PointF end)
		{
			if(end.X != start.X )
			{
				float k = (end.Y - start.Y ) / (end.X - start.X);
				float alpha = start.Y - k * start.X;
				float cx = ellipseRect.X + ellipseRect.Width / 2f;
				float cy = ellipseRect.Y + ellipseRect.Height / 2f;
				float rx = ellipseRect.Width / 2f;
				float ry = ellipseRect.Height/2f;
				float a = (float)(1f / (rx * rx) + (k * k) / (ry * ry));
				float b = (float)(2 * k * (alpha - cy) / (ry * ry) - 2 * cx / (rx * rx));
				float c = (float)((cx * cx) / (rx * rx) + (alpha -cy) * (alpha - cy) / (ry * ry) - 1);

				float x = ((float)Math.Sqrt(b * b - 4 * a * c) - b) / (2 * a);
				float x1 = (-(float)Math.Sqrt(b * b - 4 * a * c) - b) / (2 * a);
				if(end.X < start.X)
					x = x1;
				float y = k * x + alpha;
				return new PointF(x,y);
			}
			return PointF.Empty;
		}
		#endregion
	}
}
