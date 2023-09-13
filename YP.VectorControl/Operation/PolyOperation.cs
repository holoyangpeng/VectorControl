using System;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using YP.VectorControl.Forms;

namespace YP.VectorControl.Operation
{
	#region ..操作
	internal enum NodeOperator
	{
		/// <summary>
		/// 绘制
		/// </summary>
		Draw,
		/// <summary>
		/// 添加锚点
		/// </summary>
		AddAnchor,
		/// <summary>
		/// 删除锚点
		/// </summary>
		DelAnchor,
		/// <summary>
		/// 改变锚点
		/// </summary>
		ChangeAnchor,
		/// <summary>
		/// 移动锚点
		/// </summary>
		MoveAnchor,
		/// <summary>
		/// 转换锚点
		/// </summary>
		ConvertAnchor,
		/// <summary>
		/// 改变末端锚点位置
		/// </summary>
		ChangeEndAnchor,
		/// <summary>
		/// 关闭路径
		/// </summary>
		CloseFigure,
		/// <summary>
		/// 移动控制点
		/// </summary>
		MoveControl,
		/// <summary>
		/// 移动路径
		/// </summary>
		MovePath,
		/// <summary>
		/// 选择锚点
		/// </summary>
		Select,
		/// <summary>
		/// 移动中心点
		/// </summary>
		CenterPoint,
		None
	}
	#endregion

	/// <summary>
	/// PolyOperation 的摘要说明。
	/// </summary>
	internal class PolyOperation:NodeEditOperation
	{
		#region ..构造及消除
		internal PolyOperation(Canvas mousearea):base(mousearea)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.doc = this.mouseArea.SVGDocument;
			this.nodeOperator = NodeOperator.Draw;
//			this.pen = new Pen(this.mouseArea.selectedColor,0.5f);
//			this.Finish = false;
//			this.mouseArea.RefreshContent = false;
		}

		internal PolyOperation(Canvas mousearea,YP.SVG.BasicShapes.SVGPointsElement element):this(mousearea)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.graph = element;
			this.pointList = (YP.SVG.DataType.SVGPointList)element.Points;
			this.nodeOperator = NodeOperator.Draw;
//			this.mouseArea.RefreshContent = false;
//			this.Finish = false;
		}

		public override void Dispose()
		{
			this.currentSegs = null;
//			this.pen.Dispose();
//			this.pen = null;
			this.pointstring = null;
			this.pointList = null;
			this.mouseArea.validContent = true;
			base.Dispose ();
		}

		#endregion

		#region ..properties
        internal override bool NeedAlignToGrid
        {
            get
            {
                return true;
            }
        }
		/// <summary>
		/// 判断当前是否为空
		/// </summary>
		internal override bool IsEmpty
		{
			get
			{
				return this.graph == null ;
			}
		}

		NodeOperator Operate
		{
			set
			{
				NodeOperator op = value;
				string tooltip = string.Empty;
				switch(op)
				{
					case NodeOperator.Draw:
						if(OperatorHelper.IsPointsOperation(this.mouseArea.CurrentOperator))
							this.mouseArea.Cursor = Forms.Cursors.PolyDraw;
						else
							op = NodeOperator.Select;
						//						if(!this.tooltips.ContainsKey("polydraw"))
						//							this.tooltips.Add("polydraw",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("polydraw"));
						//						tooltip =(string)this.tooltips["polydraw"];
						break;
					case NodeOperator.DelAnchor:
						if(OperatorHelper.IsPointsOperation(this.mouseArea.CurrentOperator))
							this.mouseArea.Cursor = Forms.Cursors.PolyDel;
						else
							op = NodeOperator.Select;
						
						//						if(!this.tooltips.ContainsKey("polydel"))
						//							this.tooltips.Add("polydel",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("polydel"));
						//						tooltip =(string)this.tooltips["polydel"];
						break;
					case NodeOperator.MoveAnchor:
						this.mouseArea.Cursor = Forms.Cursors.NodeEdit;
						if(this.pointindex >= 0)
							this.mouseArea.Cursor = Forms.Cursors.MoveControl;
						//						if(!this.tooltips.ContainsKey("polymovepoint"))
						//							this.tooltips.Add("polymovepoint",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("polymovepoint"));
						//						tooltip =(string)this.tooltips["polymovepoint"];
						break;
					case NodeOperator.MovePath:
						this.mouseArea.Cursor = Forms.Cursors.MovePath;
						//						if(!this.tooltips.ContainsKey("polymmovepath"))
						//							this.tooltips.Add("polymmovepath",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("polymmovepath"));
						//						tooltip =(string)this.tooltips["polymmovepath"];
						break;
					case NodeOperator.AddAnchor:
						if(OperatorHelper.IsPointsOperation(this.mouseArea.CurrentOperator))
							this.mouseArea.Cursor = Forms.Cursors.PolyAdd ;
						else
							op = NodeOperator.Select;
						
						//						if(!this.tooltips.ContainsKey("polyadd"))
						//							this.tooltips.Add("polyadd",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("polyadd"));
						//						tooltip =(string)this.tooltips["polyadd"];
						break;
				}
				this.nodeOperator = op;

				//				if((Control.ModifierKeys & Keys.Control) != Keys.Control)
				//				{
				//					if(!this.tooltips.ContainsKey("ctrlpoly"))
				//						this.tooltips.Add("ctrlpoly",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("ctrlpoly"));
				//					tooltip +=(string)this.tooltips["ctrlpoly"];
				//				}
				//
				//				this.mouseArea.PicturePanel.ToolTip(tooltip,(byte)YP.SVGDom.Interface.TipType.ToolTip);
			}
		}
		#endregion

		#region ..私有变量
		internal YP.SVG.BasicShapes.SVGPointsElement graph = null;
		YP.SVG.Document.SVGDocument doc = null;
		int pointindex = -1;
		ArrayList currentSegs = new ArrayList();
//		Pen pen = null;//new Pen(Color.Blue,0.5f);
		YP.SVG.DataType.SVGPointList pointList = new YP.SVG.DataType.SVGPointList(new YP.SVG.DataType.SVGPoint[0]);
		string pointstring = string.Empty;
		internal int CurrentTime = -1;
		#endregion

		#region ..鼠标事件
		#region ..OnMouseDown
		/// <summary>
		/// OnMouseDown
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(object sender,MouseEventArgs e)
		{
			if(!this.IsValidDocument)
				return;
			if(e.Button == MouseButtons.Left && this.nodeOperator != NodeOperator.Select)
				this.mousedown = true;
			else
				return;
			try
			{
				Matrix m = null;
				if(this.graph != null)
                    m = this.mouseArea.GetTotalTransformForElement(this.graph);//.TotalTransform.Clone();
				else
					m = this.mouseArea.CoordTransform.Clone();
				if(!m.IsInvertible)
					return;
				m.Invert();
				PointF[] p1 = new PointF[]{new PointF(e.X,e.Y)};
				m.TransformPoints(p1);
				this.startPoint = p1[0];
				string pointstr = string.Empty;
				bool deal = false;
				if(this.graph == null)
					this.Operate = NodeOperator.Draw;
				switch(this.nodeOperator)
				{
						#region ..添加节点
					case NodeOperator.Draw:
						bool newgraph = false;
						if(this.graph == null)
							newgraph = true;
						if(newgraph)
						{
							string a = null;
							if(this.mouseArea.CurrentOperator == Operator.Polygon)
								a = "polygon";
							else
								a = "polyline";
							this.graph = (YP.SVG.BasicShapes.SVGPointsElement)this.mouseArea.SVGDocument.CreateElement(this.mouseArea.SVGDocument.Prefix,a,this.mouseArea.SVGDocument.NamespaceURI);
							a = null;
							this.pointList = new YP.SVG.DataType.SVGPointList(new YP.SVG.DataType.SVGPoint[0]);
						}
						this.currentSegs.Clear();
						this.currentSegs.Add(this.pointList.NumberOfItems);
						this.pointList.AppendItem(new YP.SVG.DataType.SVGPoint(this.startPoint.X,this.startPoint.Y));
						deal = true;
						break;
						#endregion

						#region ..插入节点
					case NodeOperator.AddAnchor:
						if(this.graph != null)
						{					
							this.currentSegs.Clear();
							if((this.pointindex >= 0 && this.pointindex < this.pointList.NumberOfItems) || (this.pointindex == this.pointList.NumberOfItems && this.graph is YP.SVG.BasicShapes.SVGPolygonElement))
							{
								this.currentSegs.Add(this.pointindex);
								this.pointList.InsertItemBefore(new YP.SVG.DataType.SVGPoint(this.startPoint.X,this.startPoint.Y),this.pointindex);
								deal = true;
							}
						}
						break;
						#endregion

						#region ..删除节点
					case NodeOperator.DelAnchor:
						if(this.graph != null)
						{
							if(this.pointindex >= 0 && this.pointindex < this.graph.Points.NumberOfItems)
							{
								this.pointList.RemoveItem(this.pointindex);
								pointstr = string.Empty;
								deal = true;
							}
						}
						break;
						#endregion

						#region ..移动锚点
					case NodeOperator.MoveAnchor:
						if(this.pointindex >= 0)
						{
							if(!this.currentSegs.Contains(this.pointindex))
							{
								this.currentSegs.Clear();
								this.currentSegs.Add(this.pointindex);
							}
						}
						else
							this.currentSegs.Clear();
						break;
						#endregion
				}
				pointstr = null;
				p1 = null;
				m.Dispose();
				m = null;
				if(deal)
					this.UpdatePointsData();
				this.startPoint = new PointF(e.X,e.Y);
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

				#region ..无按键移动
				if(e.Button == MouseButtons.None && this.graph != null)
				{
					this.Finish=true;
					PointF[] p2 = new PointF[]{new PointF(e.X,e.Y)};
                    using (Matrix m = this.mouseArea.GetTotalTransformForElement(this.graph))//.TotalTransform.Clone();
                    {
                        m.Invert();
                        m.TransformPoints(p2);
                        PointF p = p2[0];
                        PointF[] p1 = this.pointList.GetGDIPoints();
                        //				int i = 0;
                        this.pointindex = -1;
                        GraphicsPath path = new GraphicsPath();
                        Pen pen = new Pen(Color.Black, 3);
                        for (int i = 0; i < p1.Length; i++)//foreach(PointF tempp in p1)
                        {
                            PointF tempp = p1[i];
                            if (Math.Abs(tempp.X - p.X) <= 3 && Math.Abs(tempp.Y - p.Y) <= 3)
                            {
                                this.pointindex = i;

                                if ((Control.ModifierKeys & Keys.Control) == Keys.Control || this.mouseArea.CurrentOperator == Operator.NodeEdit)
                                    this.Operate = NodeOperator.MoveAnchor;
                                else
                                    this.Operate = NodeOperator.DelAnchor;
                                this.segPoint = tempp;
                                pen.Dispose();
                                path.Dispose();
                                p1 = null;
                                p2 = null;
                                return;
                            }

                            PointF pre = PointF.Empty;
                            if (i - 1 >= 0)
                                pre = p1[i - 1];

                            if (!pre.IsEmpty && pre != tempp)
                            {
                                path.Reset();
                                path.AddLine(pre, tempp);
                                if (path.IsOutlineVisible(p, pen))
                                {
                                    this.pointindex = i;
                                    if ((Control.ModifierKeys & Keys.Control) == Keys.Control || this.mouseArea.CurrentOperator == Operator.NodeEdit)
                                        this.Operate = NodeOperator.MovePath;
                                    else
                                        this.Operate = NodeOperator.AddAnchor;
                                    //							this.segPoint = p1;
                                    pen.Dispose();
                                    path.Dispose();
                                    pen.Dispose();
                                    path.Dispose();
                                    p1 = null;
                                    p2 = null;
                                    return;
                                }
                            }

                            if ((i == p1.Length - 1) && this.graph is YP.SVG.BasicShapes.SVGPolygonElement)
                            {
                                if (i == p1.Length - 1)
                                    pre = p1[0];

                                if (!pre.IsEmpty && pre != tempp)
                                {
                                    path.Reset();
                                    path.AddLine(pre, tempp);
                                    if (path.IsOutlineVisible(p, pen))
                                    {
                                        this.pointindex = p1.Length;
                                        if ((Control.ModifierKeys & Keys.Control) == Keys.Control || this.mouseArea.CurrentOperator == Operator.NodeEdit)
                                            this.Operate = NodeOperator.MovePath;
                                        else
                                            this.Operate = NodeOperator.AddAnchor;
                                        pen.Dispose();
                                        path.Dispose();
                                        pen.Dispose();
                                        path.Dispose();
                                        p1 = null;
                                        p2 = null;
                                        return;
                                    }
                                }
                            }

                            //					i ++;
                        }
                        pen.Dispose();
                        path.Dispose();
                        pen.Dispose();
                        path.Dispose();

                        p2 = null;

                        if ((Control.ModifierKeys & Keys.Control) == Keys.Control || this.mouseArea.CurrentOperator == Operator.NodeEdit)
                            this.Operate = NodeOperator.MoveAnchor;
                        else
                        {
                            if (p1.Length > 0)
                            {
                                PointF[] ps = new PointF[] { p1[p1.Length - 1] };
                                this.mouseArea.GetTotalTransformForElement(this.graph).TransformPoints(ps);
                                this.mouseArea.firstPoint = Point.Round(ps[0]);
                                ps = null;
                            }
                            this.Operate = NodeOperator.Draw;
                            this.Finish = false;

                        }
                        p1 = null;
                    }
				}
					#endregion			

					#region ..左键移动
				else if(e.Button == MouseButtons.Left && this.mousedown && this.graph != null && this.nodeOperator != NodeOperator.Select)
				{
					PointF[] ps = this.pointList.GetGDIPoints();
					if(ps.Length > 0)
                        this.mouseArea.GetTotalTransformForElement(this.graph).TransformPoints(ps);
					Matrix transform = new Matrix();
					transform.Translate(e.X - this.startPoint.X,e.Y - this.startPoint.Y);
					int r = this.mouseArea.grapSize / 2;
					switch(this.nodeOperator)
					{
							#region ..移动路径
						case NodeOperator.MovePath:
							if(this.graph != null)
							{
								this.XORDrawPath(this.reversePath);
								this.XORDrawPath(this.reverseFillPath);
								this.reversePath.Reset();
								this.reverseFillPath.Reset();
								if(this.pointindex > 0 && this.pointindex <= ps.Length )
								{
									PointF prepreanchor = PointF.Empty;
									PointF preanchor = PointF.Empty;
									PointF anchor = PointF.Empty;
									PointF nextanchor = PointF.Empty;
									if(this.pointindex < ps.Length)
									{
										anchor = ps[this.pointindex];
										preanchor = ps[this.pointindex - 1];

										if(this.pointindex - 2 >= 0)
											prepreanchor = ps[this.pointindex - 2];
										else if(this.graph is YP.SVG.BasicShapes.SVGPolygonElement && ps.Length > 2)
											prepreanchor = ps[ps.Length -1];

										if(this.pointindex + 1 < ps.Length)
											nextanchor = ps[this.pointindex + 1];
										else if(this.graph is YP.SVG.BasicShapes.SVGPolygonElement && ps.Length > 2)
											nextanchor = ps[0];

									}
									else if(this.graph is YP.SVG.BasicShapes.SVGPolygonElement)
									{
										anchor = ps[ps.Length -1];
										preanchor = ps[0];
										prepreanchor = ps[1];
										nextanchor = ps[ps.Length - 2];
									}
									PointF[] ps1 = new PointF[]{anchor,preanchor};
									transform.TransformPoints(ps1);
									if(!prepreanchor.IsEmpty)
										this.reversePath.AddLine(prepreanchor,ps1[1]);
									this.reversePath.AddLine(ps1[1],ps1[0]);
									if(!nextanchor.IsEmpty)
										this.reversePath.AddLine(ps1[0],nextanchor);

									this.reverseFillPath.AddRectangle(new RectangleF(ps1[0].X - r,ps1[0].Y - r,2*r,2*r));
									this.reverseFillPath.AddRectangle(new RectangleF(ps1[1].X - r,ps1[1].Y - r,2*r,2*r));
								}
								this.XORDrawPath(this.reversePath);
								this.XORDrawPath(this.reverseFillPath);
							}
							break;
							#endregion

							#region ..移动锚点
						case NodeOperator.MoveAnchor:
							if(this.graph != null )
							{
								this.XORDrawPath(this.reversePath);
								this.XORDrawPath(this.reverseFillPath);
								this.reversePath.Reset();
								this.reverseFillPath.Reset();
								foreach(int i in this.currentSegs)
								{
									if(i < 0 || i > ps.Length -1)
										continue;
									PointF p = ps[i];
									PointF[] ps1 = new PointF[]{p};
									transform.TransformPoints(ps1);
									p = ps1[0];
									PointF pre = PointF.Empty;
									PointF next = PointF.Empty;
									int preindex = i - 1;
									if(i - 1 >= 0 && i - 1 < ps.Length)
										pre = ps[i-1];
									else if(i == 0 && i<ps.Length && this.graph is YP.SVG.BasicShapes.SVGPolygonElement)
									{
										preindex = ps.Length - 1;
										pre = ps[ps.Length -1];
									}

									int nextindex = i + 1;
									if(i + 1 >= 0 && i + 1 < ps.Length)
										next = ps[i + 1];
									else if(i == ps.Length - 1&& i>= 0 && this.graph is YP.SVG.BasicShapes.SVGPolygonElement)
									{
										nextindex = 0;
										next = ps[0];
									}
									this.reverseFillPath.AddRectangle(new RectangleF( p.X - 1*r,p.Y - 1*r,2*r,2*r));
									this.reversePath.StartFigure();
									if(!pre.IsEmpty)
									{
										if(this.currentSegs.Contains(preindex))
										{
											PointF[] ps2 = new PointF[]{pre};
											transform.TransformPoints(ps2);
											pre = ps2[0];
										}
										this.reversePath.AddLine(pre,p);
									}
									if(!next.IsEmpty)
									{
										if(this.currentSegs.Contains(nextindex))
										{
											PointF[] ps2 = new PointF[]{next};
											transform.TransformPoints(ps2);
											next = ps2[0];
										}
										this.reversePath.AddLine(p,next);
									}
								
								}
							
								if(this.currentSegs.Count == 0)
								{
									float left = (float)Math.Min(e.X,this.startPoint.X);
									float top = (float)Math.Min(e.Y,this.startPoint.Y);
									float right = (float)Math.Max(e.X,this.startPoint.X);
									float bottom = (float)Math.Max(e.Y,this.startPoint.Y);
									this.reversePath.AddRectangle(new RectangleF(left,top,right - left,bottom - top));
								}
								this.XORDrawPath(this.reversePath);
								this.XORDrawPath(this.reverseFillPath);
							}
							break;
							#endregion
					}
					transform.Dispose();
					transform = null;
					ps =null;
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

				bool select = false;
				if(e.Button == MouseButtons.Left && this.mousedown && this.graph != null&& this.nodeOperator != NodeOperator.Select)
				{
					string pointstr = string.Empty;
                    Matrix m = this.mouseArea.GetTotalTransformForElement(this.graph);//.TotalTransform.Clone();
					m.Invert();
					Matrix transform = new Matrix();
					PointF[] ps1 = new PointF[]{new PointF(e.X,e.Y),this.startPoint};
					m.TransformPoints(ps1);
					transform.Translate(ps1[0].X - ps1[1].X,ps1[0].Y - ps1[1].Y);
					//				YP.SVGDom.DataType.SVGPointList list = (YP.SVGDom.DataType.SVGPointList)this.graph.Points.AnimatedPoints;
					PointF[] ps = this.pointList.GetGDIPoints();
					bool deal = false;
					switch(this.nodeOperator)
					{
							#region ..移动路径
						case NodeOperator.MovePath:
							if(this.graph != null)
							{
								this.XORDrawPath(this.reversePath);
								if(this.pointindex > 0 && this.pointindex <= this.pointList.NumberOfItems)
								{
									PointF preanchor = PointF.Empty;
									PointF anchor = PointF.Empty;
									if(this.pointindex < ps.Length)
									{
										anchor = ps[this.pointindex];
										preanchor = ps[this.pointindex - 1];
										ps1 = new PointF[]{anchor,preanchor};
										transform.TransformPoints(ps1);
										this.pointList.ReplaceItem(new YP.SVG.DataType.SVGPoint(ps1[0].X,ps1[0].Y),this.pointindex);
										this.pointList.ReplaceItem(new YP.SVG.DataType.SVGPoint(ps1[1].X,ps1[1].Y),this.pointindex - 1);
									}
									else if(this.graph is YP.SVG.BasicShapes.SVGPolygonElement)
									{
										anchor = ps[ps.Length -1];
										preanchor = ps[0];
										ps1 = new PointF[]{anchor,preanchor};
										transform.TransformPoints(ps1);
										this.pointList.ReplaceItem(new YP.SVG.DataType.SVGPoint(ps1[0].X,ps1[0].Y),this.pointList.NumberOfItems - 1);
										this.pointList.ReplaceItem(new YP.SVG.DataType.SVGPoint(ps1[1].X,ps1[1].Y),0);
									}
									deal  = true;
								}
							}
							break;
							#endregion

							#region ..移动锚点
						case NodeOperator.MoveAnchor:
							if(this.graph != null)
							{
								this.XORDrawPath(this.reversePath);
							
								foreach(int i in this.currentSegs)
								{
									ps1 = new PointF[]{ps[i]};
									transform.TransformPoints(ps1);
									this.pointList.ReplaceItem(new YP.SVG.DataType.SVGPoint(ps1[0].X,ps1[0].Y),i);
									deal = true;
								}							

								if(this.currentSegs.Count == 0)
								{
									ps1 = new PointF[]{this.startPoint,new PointF(e.X,e.Y)};
									m.TransformPoints(ps1);
									PointF end = ps1[1];
									PointF start = ps1[0];
									float left = (float)Math.Min(end.X,start.X);
									float top = (float)Math.Min(end.Y,start.Y);
									float right = (float)Math.Max(end.X,start.X);
									float bottom = (float)Math.Max(end.Y,start.Y);
									RectangleF rect = new RectangleF(left,top,right - left,bottom - top);
									int i = 0;
									foreach(PointF p in ps)
									{
										if(rect.Contains(p))
											this.currentSegs.Add(i);
										i ++;
									}

									if(this.currentSegs.Count == 0 && this.parentOperation != null)
									{
										select = true;
									}
								}
							}
							break;
							#endregion
					}

					if(deal)
					{
						this.UpdatePointsData();
					}
					ps = null;
					ps1 = null;
			
					transform.Dispose();
					transform = null;
					m.Dispose();
					m = null;
				}
				this.mousedown = false;
				this.reversePath.Reset();
				this.mouseArea.Invalidate();
				if(select)
				{
					this.parentOperation.SelectElement(new PointF(e.X,e.Y));
				}
				this.mouseArea.validContent = true;
				this.mouseArea.firstPoint = new Point(e.X,e.Y);
			}
			catch{}
		}
		#endregion

		#endregion

		#region ..改变属性事件
		protected override void OnAdaptAttribute(object sender,AdaptAttributeEventArgs e)
		{
		}
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
				this.DrawXorPath(e);
				e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				//检测编辑对象是否以被删除
				if(this.graph != null && (this.graph as YP.SVG.SVGElement).ParentNode == null)
				{
					this.graph = null;
					this.pointList.Clear();
				
				}
				if(this.graph != null)
				{
					if(this.CurrentTime != this.mouseArea.SVGDocument.CurrentTime)
					{
						if(this.graph != null && this.graph.ParentNode == null)
							this.graph = null;
						this.currentSegs.Clear();
						if(this.graph != null)
							this.pointList = new YP.SVG.DataType.SVGPointList(((YP.SVG.DataType.SVGPointList)this.graph.Points).GetPoints());
						else
							this.pointList.Clear();
					}
					PointF[] ps = this.pointList.GetGDIPoints();
					if(ps.Length == 0)
						return;
                    this.mouseArea.GetTotalTransformForElement(this.graph).TransformPoints(ps);
					if(ps.Length > 1 && this.mouseArea.ShowSelectionHighlightOutline)
					{
						e.Graphics.DrawLines(this.mouseArea.SelectedPen,ps);
						if(this.graph is YP.SVG.BasicShapes.SVGPolygonElement)
							e.Graphics.DrawLine(this.mouseArea.SelectedPen,ps[0],ps[ps.Length -1]);
					
					}
					if(this.pointindex >= 0 && !this.currentSegs.Contains(this.pointindex))
						this.currentSegs.Add(this.pointindex);
					int r = this.mouseArea.grapSize * 2 / 3;
                    using (Brush brush = new SolidBrush(this.mouseArea.HighlightAnchor))
                    {
                        using (Pen pen = new Pen(ControlPaint.DarkDark(this.mouseArea.HighlightAnchor), 1))
                        {
                            for (int i = 0; i < ps.Length; i++)
                            {
                                PointF p = ps[i];
                                bool contains = this.currentSegs.Contains(i);

                                e.Graphics.DrawRectangle(pen, p.X - r, p.Y - r, 2 * r, 2 * r);
                                e.Graphics.FillRectangle(contains ? brush : this.mouseArea.UnselectedBrush, p.X - r, p.Y - r, 2 * r, 2 * r);
                            }

                            ps = null;
                        }
                    }
				}
			
				this.CurrentTime = this.mouseArea.SVGDocument.CurrentTime;
			}
			catch{}
		}
		#endregion

		#region ..开始新绘制
		/// <summary>
		/// 开始新绘制
		/// </summary>
		internal void StartPoly()
		{
			this.graph = null;
			this.mouseArea.firstPoint= Point.Empty;
			this.mouseArea.Invalidate ();
			this.nodeOperator = NodeOperator.Draw;
		}
		#endregion

		#region ..更新数据
		void UpdatePointsData()
		{
			if(this.graph == null)
				return;
			this.mouseArea.InvalidateElement(this.graph as SVG.Interface.ISVGPathable );
			string pointstr = this.pointList.ToString();
			bool old = doc.AcceptNodeChanged;

			this.graph.UpdateAttribute("points",pointstr);

			if(this.graph.ParentNode == null)
				this.graph = (YP.SVG.BasicShapes.SVGPointsElement)this.mouseArea.AddElement(this.graph);
			this.mouseArea.selectChanged = true;
			doc.InvokeUndos();
			doc.AcceptNodeChanged = old;
			this.pointstring = pointstr;
			pointstr = null;
		}
		#endregion

		#region ..重置
		/// <summary>
		/// 重置
		/// </summary>
		internal override void Reset()
		{
			this.graph = null;
			this.pointList.Clear();
			this.mouseArea.Invalidate();
			
			base.Reset();
		}
		#endregion

		#region ..Invalidate
		/// <summary>
		/// 绘制对象发生更改
		/// </summary>
		internal override void Invalidate()
		{
			this.CurrentTime = -1;
		}
		#endregion

		#region ..ProcessDialogKey
		public override bool ProcessDialogKey(Keys keyData)
		{
			
			if(this.graph != null && this.pointList != null)
			{
				if(keyData == (Keys.Control | Keys.A))
				{
					this.currentSegs.Clear();
					for(int i = 0;i<this.pointList.NumberOfItems;i++)
						this.currentSegs.Add(i);
				
					this.mouseArea.Invalidate();
					return true;
				}
				else if(keyData == Keys.Escape)
				{
					this.currentSegs.Clear();
					this.mouseArea.Invalidate();
					return true;
				}
			}
			
			return base.ProcessDialogKey(keyData);
		}

		#endregion
	}
}
