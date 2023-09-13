using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections;

namespace YP.VectorControl.Operation
{
	/// <summary>
	/// NodeOperation 的摘要说明。
	/// </summary>
	internal class NodeEditOperation:ConnectorOperation
	{
		#region ..构造及消除
		public NodeEditOperation(Canvas mousearea):base(mousearea)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//

			this.totalTransform = this.mouseArea.CoordTransform.Clone();
//			this.pen = new Pen(this.mouseArea.selectedColor,0.5f);
			this.controlWidth = this.mouseArea.grapSize;
		}

		public override void Dispose()
		{
			this.totalTransform.Dispose();
			this.totalTransform = null;
//			this.pen.Dispose();
			base.Dispose();
			if(this.childOperation != null)
				this.childOperation.Dispose();
		}
		#endregion

		#region ..私有变量
		protected int controlWidth = 4;
		SVG.SVGTransformableElement renderElement;
		protected NodeOperator nodeOperator = NodeOperator.Select;
		Matrix totalTransform = new Matrix();
		PointF[] anchors = new PointF[0];
		protected PointF startPoint = PointF.Empty;
		int anchorIndex = -1;
		float rx = 0,ry = 0;
		NodeEditOperation childOperation;
		protected NodeEditOperation parentOperation;
		PointF firstCnt = PointF.Empty,secondCnt = PointF.Empty;
        
		#endregion

		#region ..properties
        internal override bool NeedAlignToGrid
        {
            get
            {
                return this.nodeOperator != NodeOperator.None && this.nodeOperator != NodeOperator.Select;
            }
        }

		/// <summary>
		/// 设置当前操作方式
		/// </summary>
		protected virtual NodeOperator CurrentOperator
		{
			set
			{
				this.nodeOperator = value;
				switch(this.nodeOperator)
				{
					case NodeOperator.MoveAnchor:
						this.mouseArea.Cursor = Forms.Cursors.MoveControl;
						break;
					case NodeOperator.MovePath:
						this.mouseArea.Cursor = Forms.Cursors.MovePath;
						break;
					default:
						this.mouseArea.Cursor = this.mouseArea.DefaultCursor;
						break;
				}
			}
			
		}

		/// <summary>
		/// 判断当前是否为空
		/// </summary>
		internal virtual bool IsEmpty
		{
			get
			{
				return false;
			}
		}


		/// <summary>
		/// 正在操作的对象
		/// </summary>
		internal SVG.SVGTransformableElement RenderElement
		{
			set
			{
				if(this.renderElement != value)
				{
					this.renderElement = value;
					this.nodeOperator = NodeOperator.Select;
					if(this.renderElement != null)
					{
						if(this.mouseArea.SVGDocument.SelectCollection.Count != 1 || !this.mouseArea.SVGDocument.SelectCollection.Contains(this.renderElement))
						this.mouseArea.SVGDocument.ChangeSelectElement((SVG.SVGElement)this.renderElement);
					}
					else
						this.mouseArea.SVGDocument.ChangeSelectElements(null as SVG.SVGElementCollection);
					if(this.renderElement is SVG.Paths.SVGPathElement)
					{
						if(this.childOperation != null)
						{
							if(this.childOperation is BezierSplineOperation && (this.childOperation as BezierSplineOperation).svgPathSegListElement == value)
								return;
							this.childOperation.Dispose();
						}
						
						this.childOperation = new BezierSplineOperation(this.mouseArea,(SVG.Paths.SVGPathElement)this.renderElement);
						this.childOperation.parentOperation = this;
					}
					else if(this.renderElement is SVG.BasicShapes.SVGPointsElement)
					{
						if(this.childOperation != null )
						{
							if(this.childOperation is PolyOperation && (this.childOperation as PolyOperation).graph == value)
								return;
							this.childOperation.Dispose();
						}
						this.childOperation = new PolyOperation(this.mouseArea,(SVG.BasicShapes.SVGPointsElement)this.renderElement);
						this.childOperation.parentOperation = this;
					}
					else
					{
						if(this.childOperation != null)
							this.childOperation.Dispose();
						this.childOperation = null;
						this.mouseArea.Invalidate();
					}
				}
			}
		}
		#endregion

		#region ..鼠标事件

		#region ..OnMouseDown
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
				base.OnMouseDown(sender,e);
				if(this.cntOperator != ConnectorOperation.ConnectOperator.None)
					return;
				if(this.childOperation != null && this.nodeOperator != NodeOperator.Select && this.nodeOperator != NodeOperator.MovePath)
				{
					return;
				}
				if(e.Button == MouseButtons.Left)
				{
					this.mousedown = true;
				}
				this.startPoint = new PointF(e.X,e.Y);
                if (this.mouseArea.anchorPoint.HasValue)
                    this.startPoint = this.mouseArea.PointViewToClient(this.mouseArea.anchorPoint.Value);
				this.reversePath.Reset();
			}
			catch{}

		}
		#endregion

		#region ..OnMouseMove
		/// <summary>
		/// OnMouseMove
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(object sender,MouseEventArgs e)
		{
			try
			{
				if(!this.IsValidDocument)
					return;
				if(this.childOperation != null && this.nodeOperator != NodeOperator.Select && this.nodeOperator != NodeOperator.MovePath)
					return;
				PointF p = this.PointToView(new PointF(e.X,e.Y));
				
				#region ..无键
				if(e.Button == MouseButtons.None)
				{
					base.OnMouseMove(sender,e);
                    this.mouseArea.anchorPoint = null;
					if(this.cntOperator != ConnectorOperation.ConnectOperator.None)
						return;
					if(this.renderElement != null)
					{
						this.anchorIndex = -1;
						int r = this.controlWidth / 2;
						for(int i = this.anchors.Length - 1;i>=0;i--)
						{
							PointF p1 = this.anchors[i];
							RectangleF r1 = new RectangleF(p1.X - r,p1.Y - r,this.controlWidth,this.controlWidth);
							if(r1.Contains(p))
							{
								this.anchorIndex = i;
                                PointF[] ps = { p1 };
                                this.renderElement.TotalTransform.TransformPoints(ps);
                                this.mouseArea.anchorPoint = ps[0];
								this.CurrentOperator = NodeOperator.MoveAnchor;
								return;
							}
						}
						using(GraphicsPath path = (GraphicsPath)(this.renderElement as SVG.Interface.ISVGPathable).GPath.Clone())
						{
							using(Pen pen = new Pen(Color.Black,Canvas.dragIndent))
							{
								path.Transform(this.totalTransform);
								p = new PointF(e.X,e.Y);
								if(path.IsOutlineVisible(p,pen))
								{
									this.CurrentOperator = NodeOperator.MovePath;
									return;
								}
							}
						}
					}
					this.CurrentOperator = NodeOperator.Select;
				}
					#endregion
			
					#region ..左键
				else if(e.Button == MouseButtons.Left && this.mousedown)
				{
					int r1 = this.mouseArea.grapSize / 2;

					//check whether allows to edit the node
					ProtectType type = this.mouseArea.GetProtectTypeForElement(this.renderElement as SVG.SVGElement);
					if((type & ProtectType.NodeEdit) == ProtectType.NodeEdit)
						return;
					#region ..Connect
					if(this.cntOperator != ConnectorOperation.ConnectOperator.None)
						base.OnMouseMove(sender,e);
					#endregion

					#region ..移动锚点
					else if(this.nodeOperator == NodeOperator.MoveAnchor)
					{
						#region ..圆
						if(this.renderElement is SVG.BasicShapes.SVGCircleElement)
						{
							this.XORDrawPath(this.reversePath);
							this.reversePath.Reset();
							this.XORDrawPath(this.reverseFillPath);
							this.reverseFillPath.Reset();
							PointF end = this.PointToView(new PointF(e.X,e.Y));
							PointF zero = this.anchors[0];
							float r = 0;
							switch(this.anchorIndex)
							{
								case 0:
									r = (float)Math.Abs(this.anchors[1].X - this.anchors[0].X);
									this.reversePath.AddEllipse(end.X - r,end.Y - r,2 * r,2 *r);
									this.reversePath.AddLine(end,new PointF(end.X + r,end.Y));
									this.reversePath.Transform(this.totalTransform);
									this.reversePath.AddEllipse(e.X - 2.5f,e.Y - 2.5f,5,5);
									break;
								case 1:
									r = (float)Math.Sqrt(Math.Pow(end.X - this.anchors[0].X,2) + Math.Pow(end.Y - this.anchors[0].Y,2));
									this.reversePath.AddEllipse(this.anchors[0].X - r,this.anchors[0].Y - r,2 * r,2 * r);
									this.reversePath.AddLine(this.anchors[0],end);
									this.reversePath.Transform(this.totalTransform);
									this.reversePath.AddEllipse(e.X - 2.5f,e.Y - 2.5f,5,5);
									break;
							}
							PointF[] ps = new PointF[]{zero,new PointF(zero.X + r,zero.Y),new PointF(zero.X,zero.Y - r)};
							this.totalTransform.TransformPoints(ps);
							for(int i = 0;i<ps.Length;i++)
							{
								this.reverseFillPath.AddRectangle(new RectangleF(ps[i].X - r1,ps[i].Y - r1,2 * r1,2 * r1));
							}
						
							this.reversePath.StartFigure();
							this.reversePath.AddLine(ps[0],ps[1]);
							this.reversePath.StartFigure();
							this.reversePath.AddLine(ps[0],ps[2]);
							this.XORDrawPath(this.reversePath);
							this.XORDrawPath(this.reverseFillPath);
							ps = null;
						}
							#endregion

							#region ..椭圆
						else if(this.renderElement is SVG.BasicShapes.SVGEllipseElement)
						{
							this.XORDrawPath(this.reversePath);
							this.reversePath.Reset();
							this.XORDrawPath(this.reverseFillPath);
							this.reverseFillPath.Reset();
							PointF end = this.PointToView(new PointF(e.X,e.Y));
							PointF zero = this.anchors[0];
							float rx1 = 0,ry1 = 0;
							switch(this.anchorIndex)
							{
								case 0:
									rx = (float)Math.Abs(this.anchors[2].X - this.anchors[0].X);
									ry = (float)Math.Abs(this.anchors[1].Y - this.anchors[0].Y);
									this.reversePath.AddEllipse(end.X - rx,end.Y - ry,2 * rx,2 *ry);
									this.reversePath.AddLine(end,new PointF(end.X + rx,end.Y));
									this.reversePath.StartFigure();
									this.reversePath.AddLine(end,new PointF(end.X,end.Y - ry));
									this.reversePath.Transform(this.totalTransform);
									zero = end;
									break;
								case 1:
									rx = (float)Math.Abs(this.anchors[2].X - this.anchors[0].X);
									float temp1 = (float)Math.Round(1 - (float)Math.Pow(end.X - this.anchors[0].X,2) / (float)Math.Pow(rx,2),2);
									if(temp1 != 0)
									{
										ry1 = (float)Math.Round(Math.Sqrt(Math.Pow(end.Y - this.anchors[0].Y,2) / (float)temp1),2);
										if(!float.IsNaN(ry1))
											ry = ry1;
										if(ry != 0)
											this.reversePath.AddEllipse(this.anchors[0].X - rx,this.anchors[0].Y - ry,2 * rx,2 * ry);
									}
									this.reversePath.AddLine(this.anchors[0],end);
									this.reversePath.Transform(this.totalTransform);
									break;
								case 2:
									ry = (float)Math.Abs(this.anchors[1].Y - this.anchors[0].Y);
									float temp2 = (float)Math.Round(1 - (float)Math.Pow(end.Y - this.anchors[0].Y,2) / (float)Math.Pow(ry,2),2);
									if(temp2 != 0)
									{
										rx1 = (float)Math.Round(Math.Sqrt(Math.Pow (end.X - this.anchors[0].X,2) / (float)temp2),2);
										if(!float.IsNaN(rx1))
											rx = rx1;
										if(rx != 0)
											this.reversePath.AddEllipse(this.anchors[0].X - rx,this.anchors[0].Y - ry,2 * rx,2 * ry);
									}
									this.reversePath.AddLine(this.anchors[0],end);
									this.reversePath.Transform(this.totalTransform);
									break;
							}
							PointF[] ps = new PointF[]{zero,new PointF(zero.X + rx,zero.Y),new PointF(zero.X,zero.Y - ry)};
							this.totalTransform.TransformPoints(ps);
							for(int i = 0;i<ps.Length;i++)
							{
								this.reverseFillPath.AddRectangle(new RectangleF(ps[i].X - r1,ps[i].Y - r1,2 * r1,2 * r1));
							}
						
							this.reversePath.StartFigure();
							this.reversePath.AddLine(ps[0],ps[1]);
							this.reversePath.StartFigure();
							this.reversePath.AddLine(ps[0],ps[2]);
							this.XORDrawPath(this.reversePath);
							this.XORDrawPath(this.reverseFillPath);
							ps = null;
						}
							#endregion

							#region ..直线
						else if(this.renderElement is SVG.BasicShapes.SVGLineElement)
						{
							this.XORDrawPath(this.reversePath);
							this.reversePath.Reset();

							this.XORDrawPath(this.reverseFillPath);
							this.reverseFillPath.Reset();
							PointF end = this.PointToView(new PointF(e.X,e.Y));
							PointF p1 = this.anchors[0];
							if(this.anchorIndex == 0)
								p1 = this.anchors[1];
							this.reversePath.AddLine(end,p1);
							this.reversePath.Transform(this.totalTransform);
							this.reverseFillPath.AddRectangle(new RectangleF(e.X - r1,e.Y - r1,2*r1,2*r1));
							this.XORDrawPath(this.reversePath);
							this.XORDrawPath(this.reverseFillPath);
						}
							#endregion

							#region ..矩形、图片
						else if(this.renderElement is SVG.Interface.ISVGBoundElement)
						{
							this.XORDrawPath(this.reversePath);
							this.reversePath.Reset();
							this.XORDrawPath(this.reverseFillPath);
							this.reverseFillPath.Reset();
							PointF end = this.PointToView(new PointF(e.X,e.Y));
							float left = 0,top = 0,bottom = 0,right = 0;
							float rx = 0,ry = 0;
							if(this.renderElement is SVG.BasicShapes.SVGRectElement)
							{
								rx = this.anchors[1].X - this.anchors[4].X;
								ry = this.anchors[5].Y - this.anchors[0].Y;
							}
							RectangleF rect = RectangleF.Empty;
							switch(this.anchorIndex)
							{
									#region ..0
								case 0:
									left = (float)Math.Min(this.anchors[2].X,end.X);
									top = (float)Math.Min(this.anchors[2].Y,end.Y);
									bottom = (float)Math.Max(this.anchors[2].Y,end.Y);
									right = (float)Math.Max(this.anchors[2].X,end.X);
									rect = new RectangleF(left,top,right - left,bottom - top);
									if(rx != 0 || ry != 0)
									{
										if(rx == 0F) rx = ry;
										else if(ry == 0F) ry = rx;

										rx = Math.Min(rect.Width/2, rx);
										ry = Math.Min(rect.Height/2, ry);

										float a = rect.X + rect.Width - rx;
										this.reversePath.AddLine(rect.X + rx, rect.Y, a, rect.Y);
										this.reversePath.AddArc(a-rx, rect.Y, rx*2, ry*2, 270, 90);
				
										float right1 = rect.X + rect.Width;	// rightmost X
										float b = rect.Y + rect.Height - ry;

										this.reversePath.AddLine(right1, rect.Y + ry, right1, b);
										this.reversePath.AddArc(right1 - rx*2, b-ry, rx*2, ry*2, 0, 90);
				
										this.reversePath.AddLine(right1 - rx, rect.Y + rect.Height, rect.X + rx, rect.Y + rect.Height);
										this.reversePath.AddArc(rect.X, b-ry, rx*2, ry*2, 90, 90);
				
										this.reversePath.AddLine(rect.X, b, rect.X, rect.Y + ry);
										this.reversePath.AddArc(rect.X, rect.Y, rx*2, ry*2, 180, 90);
				
										this.reversePath.CloseFigure();
									}
									else
									{
										this.reversePath.AddRectangle(rect);
									}
									this.reversePath.Transform(this.totalTransform);								
									break;
									#endregion

									#region ..1
								case 1:
									left = (float)Math.Min(this.anchors[0].X,end.X);
									top = (float)Math.Min(this.anchors[2].Y,end.Y);
									bottom = (float)Math.Max(this.anchors[2].Y,end.Y);
									right = (float)Math.Max(this.anchors[0].X,end.X);
									rect = new RectangleF(left,top,right - left,bottom - top);
									if(rx != 0 && ry != 0)
									{
										if(rx == 0F) rx = ry;
										else if(ry == 0F) ry = rx;

										rx = Math.Min(rect.Width/2, rx);
										ry = Math.Min(rect.Height/2, ry);

										float a = rect.X + rect.Width - rx;
										this.reversePath.AddLine(rect.X + rx, rect.Y, a, rect.Y);
										this.reversePath.AddArc(a-rx, rect.Y, rx*2, ry*2, 270, 90);
				
										float right1 = rect.X + rect.Width;	// rightmost X
										float b = rect.Y + rect.Height - ry;

										this.reversePath.AddLine(right1, rect.Y + ry, right1, b);
										this.reversePath.AddArc(right1 - rx*2, b-ry, rx*2, ry*2, 0, 90);
				
										this.reversePath.AddLine(right1 - rx, rect.Y + rect.Height, rect.X + rx, rect.Y + rect.Height);
										this.reversePath.AddArc(rect.X, b-ry, rx*2, ry*2, 90, 90);
				
										this.reversePath.AddLine(rect.X, b, rect.X, rect.Y + ry);
										this.reversePath.AddArc(rect.X, rect.Y, rx*2, ry*2, 180, 90);
				
										this.reversePath.CloseFigure();
									}
									else
									{
										this.reversePath.AddRectangle(rect);
									}
									this.reversePath.Transform(this.totalTransform);
									break;
									#endregion

									#region ..2
								case 2:
									left = (float)Math.Min(this.anchors[0].X,end.X);
									top = (float)Math.Min(this.anchors[0].Y,end.Y);
									bottom = (float)Math.Max(this.anchors[0].Y,end.Y);
									right = (float)Math.Max(this.anchors[0].X,end.X);
									rect = new RectangleF(left,top,right - left,bottom - top);
									if(rx != 0 && ry != 0)
									{
										if(rx == 0F) rx = ry;
										else if(ry == 0F) ry = rx;

										rx = Math.Min(rect.Width/2, rx);
										ry = Math.Min(rect.Height/2, ry);

										float a = rect.X + rect.Width - rx;
										this.reversePath.AddLine(rect.X + rx, rect.Y, a, rect.Y);
										this.reversePath.AddArc(a-rx, rect.Y, rx*2, ry*2, 270, 90);
				
										float right1 = rect.X + rect.Width;	// rightmost X
										float b = rect.Y + rect.Height - ry;

										this.reversePath.AddLine(right1, rect.Y + ry, right1, b);
										this.reversePath.AddArc(right1 - rx*2, b-ry, rx*2, ry*2, 0, 90);
				
										this.reversePath.AddLine(right1 - rx, rect.Y + rect.Height, rect.X + rx, rect.Y + rect.Height);
										this.reversePath.AddArc(rect.X, b-ry, rx*2, ry*2, 90, 90);
				
										this.reversePath.AddLine(rect.X, b, rect.X, rect.Y + ry);
										this.reversePath.AddArc(rect.X, rect.Y, rx*2, ry*2, 180, 90);
				
										this.reversePath.CloseFigure();
									}
									else
									{
										this.reversePath.AddRectangle(rect);
									}
									this.reversePath.Transform(this.totalTransform);
									break;
									#endregion

									#region ..3
								case 3:
									left = (float)Math.Min(this.anchors[2].X,end.X);
									top = (float)Math.Min(this.anchors[0].Y,end.Y);
									bottom = (float)Math.Max(this.anchors[0].Y,end.Y);
									right = (float)Math.Max(this.anchors[2].X,end.X);
									rect = new RectangleF(left,top,right - left,bottom - top);
									if(rx != 0 && ry != 0)
									{
										if(rx == 0F) rx = ry;
										else if(ry == 0F) ry = rx;

										rx = Math.Min(rect.Width/2, rx);
										ry = Math.Min(rect.Height/2, ry);

										float a = rect.X + rect.Width - rx;
										this.reversePath.AddLine(rect.X + rx, rect.Y, a, rect.Y);
										this.reversePath.AddArc(a-rx, rect.Y, rx*2, ry*2, 270, 90);
				
										float right1 = rect.X + rect.Width;	// rightmost X
										float b = rect.Y + rect.Height - ry;

										this.reversePath.AddLine(right1, rect.Y + ry, right1, b);
										this.reversePath.AddArc(right1 - rx*2, b-ry, rx*2, ry*2, 0, 90);
				
										this.reversePath.AddLine(right1 - rx, rect.Y + rect.Height, rect.X + rx, rect.Y + rect.Height);
										this.reversePath.AddArc(rect.X, b-ry, rx*2, ry*2, 90, 90);
				
										this.reversePath.AddLine(rect.X, b, rect.X, rect.Y + ry);
										this.reversePath.AddArc(rect.X, rect.Y, rx*2, ry*2, 180, 90);
				
										this.reversePath.CloseFigure();
									}
									else
									{
										this.reversePath.AddRectangle(rect);
									}
									this.reversePath.Transform(this.totalTransform);
									break;
									#endregion

									#region ..4,5
								case 4:
								case 5:
									rect = new RectangleF(this.anchors[0].X,this.anchors[0].Y,this.anchors[1].X - this.anchors[0].X,this.anchors[2].Y - this.anchors[1].Y);
									if(this.anchorIndex == 4)
										ry = rx = (int)Math.Max(0,this.anchors[1].X - end.X);
									if(this.anchorIndex == 5)
										ry = rx = (int)Math.Max(0,end.Y - this.anchors[1].Y);
									if(rx != 0 && ry != 0)
									{
										if(rx == 0F) rx = ry;
										else if(ry == 0F) ry = rx;

										rx = Math.Min(rect.Width/2, rx);
										ry = Math.Min(rect.Height/2, ry);

										float a = rect.X + rect.Width - rx;
										this.reversePath.AddLine(rect.X + rx, rect.Y, a, rect.Y);
										this.reversePath.AddArc(a-rx, rect.Y, rx*2, ry*2, 270, 90);
				
										float right1 = rect.X + rect.Width;	// rightmost X
										float b = rect.Y + rect.Height - ry;

										this.reversePath.AddLine(right1, rect.Y + ry, right1, b);
										this.reversePath.AddArc(right1 - rx*2, b-ry, rx*2, ry*2, 0, 90);
				
										this.reversePath.AddLine(right1 - rx, rect.Y + rect.Height, rect.X + rx, rect.Y + rect.Height);
										this.reversePath.AddArc(rect.X, b-ry, rx*2, ry*2, 90, 90);
				
										this.reversePath.AddLine(rect.X, b, rect.X, rect.Y + ry);
										this.reversePath.AddArc(rect.X, rect.Y, rx*2, ry*2, 180, 90);
				
										this.reversePath.CloseFigure();
									}
									else
									{
										this.reversePath.AddRectangle(rect);
									}
									this.reversePath.Transform(this.totalTransform);
									break;
									#endregion
							}
							this.XORDrawPath(this.reversePath);
							if(!rect.IsEmpty)
							{
								PointF[] ps = new PointF[]{rect.Location,new PointF(rect.Right,rect.Y),new PointF(rect.Right,rect.Bottom),new PointF(rect.Left,rect.Bottom),new PointF(rect.Right - rx,rect.Top),new PointF(rect.Right,rect.Top + ry)};
								this.totalTransform.TransformPoints(ps);
								for(int i = 0;i<ps.Length;i++)
								{
									this.reverseFillPath.AddRectangle(new RectangleF(ps[i].X  - r1,ps[i].Y - r1,2*r1,2*r1));
								}
								ps = null;
							}
							this.XORDrawPath(this.reverseFillPath);
						}
							#endregion
					}
						#endregion
				
						#region ..移动路径
					else if(this.nodeOperator == NodeOperator.MovePath)
					{
						#region ..非连接线
						if(!(this.renderElement is SVG.Interface.BasicShapes.ISVGConnectElement))
						{
							this.ResetColor();
							this.XORDrawPath(this.reversePath);
							this.XORDrawPath(this.reverseFillPath);
							this.reverseFillPath.Reset();
							this.reversePath.Reset();
							this.reversePath = (GraphicsPath)(this.renderElement as SVG.Interface.ISVGPathable).GPath.Clone();
							PointF start = this.PointToView(this.startPoint);
							PointF end = this.PointToView(new PointF(e.X,e.Y));
							PointF[] ps = this.anchors.Clone() as PointF[];
							using(Matrix matrix = new Matrix())
							{
								matrix.Translate(end.X - start.X,end.Y - start.Y);
								this.reversePath.Transform(matrix);
                                if(ps.Length > 0)
								    matrix.TransformPoints(ps);
							}
							this.reversePath.Transform(this.totalTransform);
                            if(ps.Length > 0)
							this.totalTransform.TransformPoints(ps);
							for(int i = 0;i<ps.Length;i++)
							{
								this.reverseFillPath.AddRectangle(new RectangleF(ps[i].X  - r1,ps[i].Y - r1,2*r1,2*r1));
							}
							ps = null;
							this.XORDrawPath(this.reversePath);
							this.XORDrawPath(this.reverseFillPath);
						}
							#endregion
					}
						#endregion

						#region ..选择
					else if(this.nodeOperator == NodeOperator.Select)
					{
						this.XORDrawPath(this.reversePath);
						this.reversePath.Reset();
						float left = (float)Math.Min(this.startPoint.X,e.X);
						float bottom = (float)Math.Max(this.startPoint.Y,e.Y);
						float top = (float)Math.Min(this.startPoint.Y,e.Y);
						float right = (float)Math.Max(this.startPoint.X,e.X);
						this.reversePath.AddRectangle(new RectangleF(left,top,right - left,bottom - top));
						this.XORDrawPath(this.reversePath);
					}
					#endregion
				}
				#endregion
			}
			catch{}
		}
		#endregion

		#region ..OnMouseUp
		/// <summary>
		/// OnMouseUp
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(object sender,MouseEventArgs e)
		{
			try
			{
				if(!this.IsValidDocument)
					return;
				if(e.Button == MouseButtons.Left && this.mousedown)
				{
					//check whether allows to edit the node
					ProtectType type = this.mouseArea.GetProtectTypeForElement(this.renderElement as SVG.SVGElement);
					if((type & ProtectType.NodeEdit) == ProtectType.NodeEdit)
						return;

					#region ..Connect
					if(this.cntOperator != ConnectorOperation.ConnectOperator.None)
						base.OnMouseMove(sender,e);
						#endregion

						#region ..选择
					else if(this.nodeOperator == NodeOperator.Select )
					{
						this.XORDrawPath(this.reversePath);
						if(Math.Abs(this.startPoint.X - e.X) <= 1&&Math.Abs(this.startPoint.Y - e.Y)<= 1)
						{
							this.SelectElement(new PointF(e.X,e.Y));
						}
					}
						#endregion

						#region ..移动
					else if(this.nodeOperator != NodeOperator.None)
					{
						//if is not connector
						if(!(this.renderElement is SVG.Interface.BasicShapes.ISVGConnectElement))
						{
							this.XORDrawPath(this.reversePath);
							this.reversePath.Reset();
							if(this.renderElement != null)
							{
								this.UpdatePath(new PointF(e.X,e.Y));
							}
						}
					}
					#endregion
				}
				this.mousedown = false;
				this.reversePath.Reset();
				this.reverseFillPath.Reset();
				this.reverseConnetorIndicatorPath.Reset();
				this.mouseArea.validContent = true;
			}
			catch{}
			this.ResetColor();
            base.OnMouseUp(sender, e);
		}
		#endregion

		#endregion

        #region ..OnPaint
        /// <summary>
		/// OnPaint
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected override void OnPaint(object sender,PaintEventArgs e)
		{
			try
			{
				if(this.HasOneConnector)
				{
					base.OnPaint(sender,e);
					return;
				}
				if(this.childOperation != null)
					return;
				this.DrawXorPath(e);
				e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				if(this.renderElement != null && ((SVG.SVGElement)this.renderElement).ParentNode == null)
					this.renderElement = null;
				if(this.renderElement == null)
					return;
				PointF[] ps = (PointF[])this.renderElement.GetAnchors().Clone();
				if(ps.Length == 0)
					return;
				this.anchors = (PointF[])ps.Clone();
                this.totalTransform = this.mouseArea.GetTotalTransformForElement((SVG.SVGTransformableElement)this.renderElement);//.TotalTransform.Clone();
				totalTransform.TransformPoints(ps);
				float r = this.mouseArea.grapSize * 2f /3f;
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                using (Brush brush = new SolidBrush(this.mouseArea.HighlightAnchor))
                {
                    using (Pen pen = new Pen(ControlPaint.DarkDark(this.mouseArea.HighlightAnchor), 1))
                    {
                        if (this.renderElement is SVG.BasicShapes.SVGCircleElement)
                        {
                            
                            e.Graphics.DrawLine(this.mouseArea.SelectedPen, ps[0], ps[1]);
                            for (int i = 0; i < ps.Length; i++)//(PointF p1 in ps)
                            {
                                PointF p1 = ps[i];
                                e.Graphics.DrawEllipse(pen, p1.X - r, p1.Y - r, r * 2, r * 2);
                                e.Graphics.FillEllipse(brush, p1.X - r, p1.Y - r, r * 2, r * 2);

                            }
                        }
                        else if (this.renderElement is SVG.BasicShapes.SVGEllipseElement)
                        {
                            e.Graphics.DrawLine(this.mouseArea.SelectedPen, ps[0], ps[1]);
                            e.Graphics.DrawLine(this.mouseArea.SelectedPen, ps[0], ps[2]);
                            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                            for (int i = 0; i < ps.Length; i++)//(PointF p1 in ps)
                            {
                                PointF p1 = ps[i];
                                e.Graphics.DrawEllipse(pen, p1.X - r, p1.Y - r, r * 2, r * 2);
                                e.Graphics.FillEllipse(brush, p1.X - r, p1.Y - r, r * 2, r * 2);
                            }
                        }
                        else
                        {

                            for (int i = 0; i < ps.Length; i++)//(PointF p1 in ps)
                            {
                                PointF p1 = ps[i];
                                e.Graphics.DrawRectangle(pen, p1.X - r, p1.Y - r, 2 * r, 2 * r);
                                e.Graphics.FillRectangle(brush, p1.X - r, p1.Y - r, 2 * r, 2 * r);
                                
                                //e.Graphics.DrawLine(this.mouseArea.SelectedPen, p1.X - r / 2, p1.Y - r / 2, p1.X + r / 2, p1.Y + r / 2);
                                //e.Graphics.DrawLine(this.mouseArea.SelectedPen, p1.X + r / 2, p1.Y - r / 2, p1.X - r / 2, p1.Y + r / 2);
                            }
                        }
                    }
                    ps = null;
                }
			}
			catch{}
		}
		#endregion

		#region ..改变属性事件
		protected override void OnAdaptAttribute(object sender,AdaptAttributeEventArgs e)
		{
		}
		#endregion

		#region ..寻求特定的直接绘制对象
		/// <summary>
		/// 获取在指定点的直接绘制对象
		/// </summary>
		/// <param name="p">视图坐标</param>
		/// <param name="parent">父对象</param>
        SVG.SVGTransformableElement GetDirectRenderAtPoint(PointF p, SVG.Interface.ISVGContainer parent)
		{
			return this.GetDirectRenderAtPoint(p,parent,null,true);
		}

		/// <summary>
		/// 获取在指定点的直接绘制对象
		/// </summary>
		/// <param name="p">视图坐标</param>
		/// <param name="parent">父对象</param>
        SVG.SVGTransformableElement GetDirectRenderAtPoint(PointF p, SVG.Interface.ISVGContainer parent, SVG.SVGElement exceptElement, bool selection)
		{
			SVG.SVGElementCollection list = this.mouseArea.SVGDocument.SelectCollection;
            PointF viewpoint = this.mouseArea.PointClientToView(p);
			if(selection)
			{
				for(int i = list.Count - 1;i>=0;i--)
				{
					SVG.SVGElement element = (SVG.SVGElement)list[i];
					if(element is SVG.Interface.ISVGPathable && element != exceptElement)
					{
						using(GraphicsPath path = (GraphicsPath)((SVG.Interface.ISVGPathable)element).GPath.Clone())
						{
							path.Transform(((SVG.SVGTransformableElement)element).TotalTransform);
                            if(element is SVG.DocumentStructure.SVGGElement && (path.IsVisible(p) || path.IsOutlineVisible(p,Pens.Black)))
                                return this.GetDirectRenderAtPoint(p,(SVG.DocumentStructure.SVGGElement)element,null,false);
                            else if (element is SVG.SVGStyleable && this.mouseArea.HitTest(element as SVG.SVGStyleable,viewpoint))
                                return element as SVG.SVGTransformableElement;
						}
					}
				}
			}
			list = parent.ChildElements;
			for(int i = list.Count - 1;i>=0;i--)
			{
				SVG.SVGElement element = (SVG.SVGElement)list[i];
				if(element is SVG.Interface.ISVGPathable && element != exceptElement)
				{
					using(GraphicsPath path = (GraphicsPath)((SVG.Interface.ISVGPathable)element).GPath.Clone())
					{
                        path.Transform(((SVG.SVGTransformableElement)element).TotalTransform);
                        if (element is SVG.DocumentStructure.SVGGElement && (path.IsVisible(p) || path.IsOutlineVisible(p, Pens.Black)))
                            return this.GetDirectRenderAtPoint(p, (SVG.DocumentStructure.SVGGElement)element, null, false);
                        else if (element is SVG.SVGStyleable && this.mouseArea.HitTest(element as SVG.SVGStyleable, viewpoint))
                            return element as SVG.SVGTransformableElement;
					}
				}
			}
			return null;
		}
		#endregion

		#region ..更新数据
		protected void UpdatePath(PointF endpoint)
		{
			if(this.nodeOperator != NodeOperator.MoveAnchor && this.nodeOperator != NodeOperator.MovePath)
				return;
			this.mouseArea.InvalidateSelection();
			SVG.Document.SVGDocument doc = this.mouseArea.SVGDocument;
			bool old = doc.AcceptNodeChanged;
			int anchor = this.anchorIndex;

//			bool adaptattri = true;
			SVG.SVGStyleable element = this.renderElement as SVG.SVGStyleable;
			//记录改变的属性
			Hashtable hash = new Hashtable();

			#region ..分别对各种情况进行分析，确定在这过程中的改变属性
			#region ..圆
			if(this.renderElement is SVG.BasicShapes.SVGCircleElement)
			{
				PointF end = this.PointToView(endpoint);
				if(this.nodeOperator == NodeOperator.MovePath)
				{
					anchor = 0;
					PointF start = this.PointToView(this.startPoint);
					end = new PointF(this.anchors[0].X + end.X - start.X,this.anchors[0].Y + end.Y - start.Y);
				}
				doc.AcceptNodeChanged = true;
//				this.mouseArea.InvalidateElement((SVGDom.BasicShapes.SVGCircleElement)this.renderElement);
				switch(anchor)
				{
					case 0:
						hash.Add("cx",end.X.ToString());
						hash.Add("cy",end.Y.ToString());
						break;
					case 1:
						float r = (float)Math.Sqrt(Math.Pow(end.X - this.anchors[0].X,2) + Math.Pow(end.Y - this.anchors[0].Y,2));
						hash.Add("r",r.ToString());
						break;
				}
			}
			#endregion

			#region ..椭圆
			else if(this.renderElement is SVG.BasicShapes.SVGEllipseElement)
			{
				PointF end = this.PointToView(endpoint);
				if(this.nodeOperator == NodeOperator.MovePath)
				{
					anchor = 0;
					PointF start = this.PointToView(this.startPoint);
					end = new PointF(this.anchors[0].X + end.X - start.X,this.anchors[0].Y + end.Y - start.Y);
				}
				doc.AcceptNodeChanged = true;
//				this.mouseArea.InvalidateElement((SVGDom.BasicShapes.SVGEllipseElement)this.renderElement);
				
				switch(anchor)
				{
					case 0:
						hash.Add("cx",end.X.ToString());
						hash.Add("cy",end.Y.ToString());
						break;
					case 1:
						rx = (float)Math.Abs(this.anchors[2].X - this.anchors[0].X);
						float temp1 = (float)Math.Round(1 - (float)Math.Pow(end.X - this.anchors[0].X,2) / (float)Math.Pow(rx,2),2);
						if(temp1 != 0)
						{
							float ry1 = (float)Math.Round(Math.Sqrt(Math.Pow(end.Y - this.anchors[0].Y,2) / (float)temp1),2);
							if(!float.IsNaN(ry1))
								ry = ry1;
							hash.Add("ry",ry.ToString());
						}
						break;
					case 2:
						ry = (float)Math.Abs(this.anchors[1].Y - this.anchors[0].Y);
						float temp2 = (float)Math.Round(1 - (float)Math.Pow(end.Y - this.anchors[0].Y,2) / (float)Math.Pow(ry,2),2);
						if(temp2 != 0)
						{
							float rx1 = (float)Math.Round(Math.Sqrt(Math.Pow (end.X - this.anchors[0].X,2) / (float)temp2),2);
							if(!float.IsNaN(rx1))
								rx = rx1;
							hash.Add("rx",rx.ToString());
						}
						break;
				}
//				this.renderElement.UpdatePath();
//				this.mouseArea.InvalidateElement((SVGDom.BasicShapes.SVGEllipseElement)this.renderElement);
//				this.mouseArea.Invalidate();
//				doc.InvokeElementChanged(new Base.Interface.ElementChangedEventArgs(this.renderElement,this.renderElement.ParentElement,this.renderElement.ParentElement,Base.ElementChangedAction.Change));
			}
			#endregion

			#region ..直线
			else if(this.renderElement is SVG.BasicShapes.SVGLineElement)
			{
				doc.AcceptNodeChanged = true;
				this.mouseArea.InvalidateElement((SVG.BasicShapes.SVGLineElement)this.renderElement);
				PointF end = this.PointToView(endpoint);
				if(anchor == 0)
				{
					hash.Add("x1",end.X.ToString());
					hash.Add("y1",end.Y.ToString());
				}
				else if(anchor == 1)
				{
					hash.Add("x2",end.X.ToString());
					hash.Add("y2",end.Y.ToString());
				}
				else if(this.nodeOperator == NodeOperator.MovePath)
				{
					PointF start = this.PointToView(this.startPoint);
					float x1 = this.anchors[0].X + end.X - start.X;
					float y1 = this.anchors[0].Y + end.Y - start.Y;
					float x2 = this.anchors[1].X + end.X - start.X;
					float y2 = this.anchors[1].Y + end.Y - start.Y;
					hash.Add("x1",x1.ToString());
					hash.Add("y1",y1.ToString());
					hash.Add("x2",x2.ToString());
					hash.Add("y2",y2.ToString());
				}
			}
			#endregion

			#region ..矩形、图片
			else if(this.renderElement is SVG.Interface.ISVGBoundElement)
			{
				float left1 = this.anchors[0].X,top1 = this.anchors[0].Y,right1 = this.anchors[2].X,bottom1 = this.anchors[2].Y;
				float width1 = right1 - left1;
				float height1 = bottom1 - right1;
//				this.mouseArea.InvalidateElement((SVGDom.Interface.ISVGPathElement)this.renderElement);
				PointF end = this.PointToView(endpoint);
				float left = 0,top = 0,bottom = 0,right = 0;
				switch(anchor)
				{
						#region ..0
					case 0:
						left = (float)Math.Min(this.anchors[2].X,end.X);
						top = (float)Math.Min(this.anchors[2].Y,end.Y);
						bottom = (float)Math.Max(this.anchors[2].Y,end.Y);
						right = (float)Math.Max(this.anchors[2].X,end.X);
						if(left != left1)
							hash.Add("x",left.ToString());
						if(top != top1)
							hash.Add("y",top.ToString());
						if(right - left != width1)	
							hash.Add("width",(right - left).ToString());
						if(bottom - top != height1)
							hash.Add("height",(bottom - top).ToString());
						break;
						#endregion

						#region ..1
					case 1:
						left = (float)Math.Min(this.anchors[0].X,end.X);
						top = (float)Math.Min(this.anchors[2].Y,end.Y);
						bottom = (float)Math.Max(this.anchors[2].Y,end.Y);
						right = (float)Math.Max(this.anchors[0].X,end.X);
						if(left != left1)
							hash.Add("x",left.ToString());
						if(top != top1)
							hash.Add("y",top.ToString());
						if(right - left != width1)		
							hash.Add("width",(right - left).ToString());
						if(bottom - top != height1)
							hash.Add("height",(bottom - top).ToString());
						break;
						#endregion

						#region ..2
					case 2:
						left = (float)Math.Min(this.anchors[0].X,end.X);
						top = (float)Math.Min(this.anchors[0].Y,end.Y);
						bottom = (float)Math.Max(this.anchors[0].Y,end.Y);
						right = (float)Math.Max(this.anchors[0].X,end.X);
						if(left != left1)
							hash.Add("x",left.ToString());
						if(top != top1)
							hash.Add("y",top.ToString());
						if(right - left != width1)	
							hash.Add("width",(right - left).ToString());
						if(bottom - top != height1)
							hash.Add("height",(bottom - top).ToString());
						break;
						#endregion

						#region ..3
					case 3:
						left = (float)Math.Min(this.anchors[2].X,end.X);
						top = (float)Math.Min(this.anchors[0].Y,end.Y);
						bottom = (float)Math.Max(this.anchors[0].Y,end.Y);
						right = (float)Math.Max(this.anchors[2].X,end.X);
						if(left != left1)
							hash.Add("x",left.ToString());
						if(top != top1)
							hash.Add("y",top.ToString());
						if(right - left != width1)	
							hash.Add("width",(right - left).ToString());
						if(bottom - top != height1)
							hash.Add("height",(bottom - top).ToString());
						break;
						#endregion 

						#region ..4,5
					case 4:
						rx = (int)Math.Max(0,this.anchors[1].X - end.X);
						hash.Add("rx",rx.ToString());
						hash.Add("ry",rx.ToString());
						break;
					case 5:
						rx = (int)Math.Max(0,end.Y - this.anchors[1].Y);
						hash.Add("rx",rx.ToString());
						hash.Add("ry",rx.ToString());
						break;
						#endregion
				}
				if(this.nodeOperator == NodeOperator.MovePath)
				{
					PointF start = this.PointToView(this.startPoint);
					left = this.anchors[0].X + end.X - start.X;
					top = this.anchors[0].Y + end.Y - start.Y;
					if(left != left1)
						hash["x"] = left.ToString();
					if(top != top1)
						hash["y"] = top.ToString();
				}
			}
			#endregion
			#endregion

			doc.AcceptNodeChanged = true;

			//更新属性或生成动画
			foreach(string atri in hash.Keys)
			{
				element.UpdateAttribute(atri,hash[atri].ToString());
			}
			this.mouseArea.selectChanged = true;
			this.mouseArea.UpdateSelectInfo();
			this.mouseArea.InvalidateSelection();
			doc.InvokeUndos();
			doc.AcceptNodeChanged = old;
		}
		#endregion

		#region ..坐标转换
		PointF PointToView(PointF p)
		{
			Matrix matrix = this.totalTransform.Clone();
			matrix.Invert();
			PointF[] ps = new PointF[]{p};
			matrix.TransformPoints(ps);
			matrix.Dispose();
			return ps[0];
		}
		#endregion

		#region ..SelectElement
		internal void SelectElement(PointF p)
		{
			switch(this.nodeOperator)
			{
				case NodeOperator.Select:
					if(this.mouseArea.SVGDocument.CurrentScene is SVG.Interface.ISVGContainer)
					{
						SVG.SVGElement except = (SVG.SVGElement)(this.childOperation == null?null:this.renderElement);
                        SVG.SVGTransformableElement render = this.GetDirectRenderAtPoint(p, (SVG.Interface.ISVGContainer)this.mouseArea.SVGDocument.CurrentScene, null, true) as SVG.SVGTransformableElement;
						if(render != null)
							this.RenderElement = render;
					}
					break;
			}
		}
		#endregion

		#region ..Reset
		internal override void Reset()
		{
			if(this.childOperation != null)
			{
				this.childOperation.Reset();
				this.childOperation.Dispose();
			}
			this.childOperation = null;
			parentOperation = null;
			this.renderElement = null;
			this.mouseArea.firstPoint = Point.Empty;
			base.Reset();
		}
		#endregion

		#region ..Invalidate
		/// <summary>
		/// 绘制对象发生更改
		/// </summary>
		internal override void Invalidate()
		{
			if(this.childOperation != null)
			{
				this.childOperation.Invalidate();
			}
			
		}
		#endregion

		#region ..ProcessDialogKey
		public override bool ProcessDialogKey(Keys keyData)
		{
			if(this.childOperation != null)
			{
				bool b = this.childOperation.ProcessDialogKey(keyData);
				if(b)
					return true;
			}
			return base.ProcessDialogKey(keyData);
		}
		#endregion
	}
}
