using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Collections;

namespace YP.VectorControl.Operation
{
	/// <summary>
	/// 实现Bezier操作
	/// </summary>
	internal class BezierSplineOperation:NodeEditOperation//,Interface.IBeizerSplineOperation
	{
		#region ..构造及消除
		internal BezierSplineOperation(Canvas mousearea):base(mousearea)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.nodeOperator = NodeOperator.Draw;
		}

		internal BezierSplineOperation(Canvas mousearea,SVG.Interface.ISVGPathSegListElement svgPathSegListElement):this(mousearea)
		{
			this.svgPathSegListElement = svgPathSegListElement;
			if(this.svgPathSegListElement != null)
			{
				((SVG.SVGStyleable)this.svgPathSegListElement).UpdateElement();
			}
			this.nodeOperator = NodeOperator.Draw;
		}

		static BezierSplineOperation()
		{
		}

		public override void Dispose()
		{
			this.sourcePath.Dispose();
			this.sourcePath = null;
			this.pathstring =null;
			if(this.totalReverse != null)
				this.totalReverse.Dispose();
			this.totalReverse = null;
			this.mouseArea.validContent = true;
			base.Dispose ();
		}

		#endregion

		#region ..私有变量
		internal SVG.Interface.ISVGPathSegListElement svgPathSegListElement = null;
//		Matrix totalTransform = new Matrix();
		GraphicsPath sourcePath = new GraphicsPath();
		SVG.Paths.SVGPathSegList svgPathSegList = new SVG.Paths.SVGPathSegList(string.Empty);
//		Pen pen = null;
		SVG.Paths.SVGPathSeg preSeg = null;
//		static Pen selectPen = new Pen(Color.Black,6);
		bool movePreControl = false;
//		SVGDom.Paths.SVGPathSegMove refSegMove = null;
		ArrayList currentSegs = new ArrayList ();
		float time = 0;
		string pathstring = string.Empty;
		internal int operateTime = -1;
		float delta = 0.5f;
		
		System.Drawing.Drawing2D.GraphicsPath totalReverse = new GraphicsPath();
        bool selectionAdd = false;
		#endregion

		#region ..常量
		const int lineDelta = 6;
		
		#endregion

		#region ..私有属性
		/// <summary>
		/// 判断当前是否为空
		/// </summary>
		internal override bool IsEmpty
		{
			get
			{
				return this.svgPathSegListElement == null ;
			}
		}

		NodeOperator CurrentOperate
		{
			set
			{
				NodeOperator op = value;
				string tool = string.Empty;
				switch(op)
				{
					case NodeOperator.Draw:
						if(this.mouseArea.CurrentOperator == Operator.Path)
							this.mouseArea.Cursor = Forms.Cursors.Path;
						else
							op = NodeOperator.Select;
						break;
					case NodeOperator.DelAnchor:
						if(this.mouseArea.CurrentOperator == Operator.Path)
							this.mouseArea.Cursor = Forms.Cursors.DelAnchor;
						else
							op = NodeOperator.Select;
						
						break;
					case NodeOperator.AddAnchor:
						if(this.mouseArea.CurrentOperator == Operator.Path)
							this.mouseArea.Cursor = Forms.Cursors.AddAnchor;
						else
							op = NodeOperator.Select;
						break;
					case NodeOperator.MovePath:
						this.mouseArea.Cursor = Forms.Cursors.MovePath;
						break;
					case NodeOperator.MoveAnchor:
					case NodeOperator.MoveControl:
						this.mouseArea.Cursor = Forms.Cursors.NodeEdit;
						if(this.preSeg != null)
							this.mouseArea.Cursor = Forms.Cursors.MoveControl;
						if((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
							this.mouseArea.Cursor = Forms.Cursors.AnchorMove ;
						//						if(!this.tooltips.ContainsKey("moveanchor"))
						//							this.tooltips.Add("moveanchor",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("moveanchor").Trim());
						//
						//						tool = this.tooltips["moveanchor"].ToString().Trim();
						//						if((this.preInfo != null && value == NodeOperator.MoveAnchor) || (this.moveinfo != null && value == NodeOperator.MoveControl ) ||  value == NodeOperator.SelectPath || value == NodeOperator.MovePath)
						//						{
						//							if(this.mouseArea.CurrentOperation != ToolOperation.ConvertAnchor && Control.ModifierKeys != Keys.Alt)
						//								this.mouseArea.Cursor = Cursors.ShapeDragCursor;
						//							else
						//								this.mouseArea.Cursor = Cursors.AnchorMoveCursor;
						//						}
						break;
					case NodeOperator.ChangeEndAnchor:
						this.mouseArea.Cursor = Forms.Cursors.ChangeEnd;
						break;
					case NodeOperator.ChangeAnchor:
						this.mouseArea.Cursor = Forms.Cursors.ChangeControl;
						//						if(this.nodeOperator == NodeOperator.ChangeEndAnchor)
						//							this.mouseArea.Cursor = Cursors.ChangeEnd;
						//						if(!this.tooltips.ContainsKey("changeendanchor"))
						//							this.tooltips.Add("changeendanchor",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("changeendanchor").Trim());
						//
						//						tool = this.tooltips["changeendanchor"].ToString().Trim();
						break;
					case NodeOperator.CloseFigure:
						this.mouseArea.Cursor = Forms.Cursors.CloseBezier;
						//						if(!this.tooltips.ContainsKey("closefigure"))
						//							this.tooltips.Add("closefigure",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("closefigure").Trim());
						//
						//						tool = this.tooltips["closefigure"].ToString().Trim();
						break;
					case NodeOperator.ConvertAnchor:
						this.mouseArea.Cursor = Forms.Cursors.Anchor;
						if(this.preSeg != null)
							this.mouseArea.Cursor = Forms.Cursors.AnchorMove;
						break;
					case NodeOperator.Select:
						//						if(!this.tooltips.ContainsKey("bezierselect"))
						//							this.tooltips.Add("bezierselect",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("bezierselect").Trim());
						//
						//						tool = this.tooltips["bezierselect"].ToString().Trim();
						this.mouseArea.Cursor = Forms.Cursors.Path;
						break;
					case NodeOperator.None:
						this.mouseArea.Cursor = Forms.Cursors.Path;
						//						if(!this.tooltips.ContainsKey("bezierdraw"))
						//							this.tooltips.Add("bezierdraw",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("bezierdraw").Trim());
						//
						//						tool = this.tooltips["bezierdraw"].ToString().Trim();
						break;
					case NodeOperator.CenterPoint:
						
						//						if(this.incenter)
						//						{
						//							if(!this.tooltips.ContainsKey("beziercenterpoint"))
						//								this.tooltips.Add("beziercenterpoint",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("beziercenterpoint").Trim());
						//
						//							tool = this.tooltips["beziercenterpoint"].ToString().Trim();
						//							this.mouseArea.Cursor = Cursors.CenterPointCursor;
						//						}
						//						else
						//						{
						//							if(!this.tooltips.ContainsKey("bezierselectcenterpoint"))
						//								this.tooltips.Add("bezierselectcenterpoint",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("bezierselectcenterpoint").Trim());
						//
						//							tool = this.tooltips["bezierselectcenterpoint"].ToString().Trim();
						//							this.mouseArea.Cursor = Cursors.selectCursor;
						//						}
						break;
					default:
						this.mouseArea.Cursor = Forms.Cursors.Path;
						break;
				}
				this.nodeOperator = op;
				//				string temp = string.Empty;
				//				if(this.mouseArea.CurrentOperation != ToolOperation.ShapeTransform)
				//				{
				//					switch(Control.ModifierKeys)
				//					{
				//						case Keys.Control:
				//							if(!this.tooltips.ContainsKey("bezierctrl"))
				//								this.tooltips.Add("bezierctrl",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("bezierctrl").Trim());
				//
				//							temp = this.tooltips["bezierctrl"].ToString().Trim();
				//							//						temp = "按住Alt，Shift键以获取辅助选项";
				//							break;
				//						case Keys.Alt:
				//							if(!this.tooltips.ContainsKey("bezieraltl"))
				//								this.tooltips.Add("bezieraltl",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("bezieraltl").Trim());
				//
				//							temp = this.tooltips["bezieraltl"].ToString().Trim();
				//							break;
				//						case Keys.Shift:
				//							if(!this.tooltips.ContainsKey("beziershift"))
				//								this.tooltips.Add("beziershift",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("beziershift").Trim());
				//
				//							temp = this.tooltips["beziershift"].ToString().Trim();
				//							break;
				//						case Keys.Control | Keys.Shift:
				//							temp = string.Empty;
				//							break;
				//						default:
				//							if(!this.tooltips.ContainsKey("beziernokeys"))
				//								this.tooltips.Add("beziernokeys",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("beziernokeys").Trim());
				//
				//							temp = this.tooltips["beziernokeys"].ToString().Trim();
				//							break;
				//
				//					}
				//				}
				//				if(this.currentGraph is SVGDom.Animate.MotionAnimate && Control.ModifierKeys != (Keys.Control | Keys.Shift))
				//				{
				//					if(!this.tooltips.ContainsKey("beziermotionkey"))
				//						this.tooltips.Add("beziermotionkey",SimpleSVG.Resource.DrawAreaConfig.GetLabelForName("beziermotionkey").Trim());
				//
				//					temp += this.tooltips["beziermotionkey"].ToString().Trim();
				//				}
				//
				//				this.mouseArea.PicturePanel.ToolTip(tool+temp,(byte)SVGDom.Interface.TipType.ToolTip);
			}
		}
		#endregion
		
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

			
				if(this.svgPathSegListElement != null && (this.svgPathSegListElement as SVG.SVGElement).ParentNode == null)
				{
					this.svgPathSegListElement = null;
					this.svgPathSegList.Clear();
				}

				if(this.svgPathSegListElement == null)
					this.CurrentOperate = NodeOperator.Draw;
				this.operateTime = this.mouseArea.SVGDocument.CurrentTime;
				if(e.Button == MouseButtons.Left && this.nodeOperator != NodeOperator.Select)
				{
					switch(this.nodeOperator)
					{
							#region ..正常绘制
						case NodeOperator.Draw:
							if(this.svgPathSegListElement == null)
							{
								SVG.Document.SVGDocument doc = this.mouseArea.SVGDocument;
								this.svgPathSegListElement = (SVG.Paths.SVGPathElement)doc.CreateElement(doc.Prefix,"path",doc.NamespaceURI);
                                //(this.svgPathSegListElement as SVGDom.SVGTransformableElement).TotalTransform = (this.mouseArea.SVGDocument.DocumentElement as SVGDom.DocumentStructure.SVGGElement).TotalTransform.Clone();
//								this.totalTransform.Reset();
							}
							break;
							#endregion

							#region ..移动锚点
						case NodeOperator.MoveAnchor:
                            //selectionAdd = (Control.ModifierKeys | Keys.Control) == Keys.Control;
							if(this.preSeg != null)
							{
								if(!this.currentSegs.Contains(this.preSeg))
								{
                                    if (!selectionAdd)
									this.currentSegs.Clear();
									this.currentSegs.Add(this.preSeg);
								}
							}
                            else if (!selectionAdd)
								this.currentSegs.Clear();
							this.mouseArea.Invalidate();
							break;
							#endregion

							#region ..删除锚点
						case NodeOperator.DelAnchor:
							if(this.preSeg != null)
							{
								
								this.currentSegs.Clear();
								var current = this.DeleteAnchor(this.preSeg);
								if(current != null)
									this.currentSegs.Add(current);
								this.sourcePath = this.svgPathSegList.GetGDIPath();
								if(this.svgPathSegListElement is SVG.Paths.SVGPathElement)
								{
									this.UpdatePath();
								}
							
								this.mouseArea.Invalidate();
							}
							break;
							#endregion

							#region ..插入锚点
						case NodeOperator.AddAnchor:
							this.currentSegs.Clear();
							if(this.preSeg != null)
							{
								SVG.Paths.SVGPathSeg pre = (SVG.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(this.preSeg);
								PointF preanchor = pre.GetLastPoint(this.svgPathSegList);
								PointF anchor = this.preSeg.GetLastPoint(this.svgPathSegList);
								PointF p = this.PointToView(new PointF(e.X,e.Y));
								if(this.preSeg is SVG.Paths.SVGPathSegLine)
								{
									SVG.Paths.SVGPathSegLine seg = new SVG.Paths.SVGPathSegLinetoAbs(p.X,p.Y);
									this.svgPathSegList.InsertItemBefore(seg,this.svgPathSegList.IndexOf(this.preSeg));
									this.sourcePath = this.svgPathSegList.GetGDIPath();
									this.currentSegs.Add(seg);
								}
								else if(this.preSeg is SVG.Paths.SVGPathSegCurve)
								{
									SVG.Paths.SVGPathSegCurve c = (SVG.Paths.SVGPathSegCurve)this.preSeg;
									PointF control1 = c.GetFirstControl(this.svgPathSegList);
									PointF control2 = c.GetSecondControl(this.svgPathSegList);
									float t = 0;
									PointF[] ps = SplitBezierAtPoint(preanchor,control1,control2,anchor,p,out t);
									if(ps.Length == 5 && t >= 0 && t<= 1)
									{
										SVG.Paths.SVGPathSegCurve c1 = new SVG.Paths.SVGPathSegCurvetoCubicAbs(ps[2].X,ps[2].Y,ps[0].X,ps[0].Y,ps[1].X,ps[1].Y);
										SVG.Paths.SVGPathSegCurve c2 = new SVG.Paths.SVGPathSegCurvetoCubicAbs(anchor.X,anchor.Y,ps[3].X,ps[3].Y,ps[4].X,ps[4].Y);
										int index = this.svgPathSegList.IndexOf(c);
										this.svgPathSegList.ReplaceItem(c2,index);
										this.svgPathSegList.InsertItemBefore(c1,index);
										this.sourcePath = this.svgPathSegList.GetGDIPath();
										this.currentSegs.Add(c1);
									}
								}
								if(this.svgPathSegListElement is SVG.Paths.SVGPathElement)
								{
									this.UpdatePath();
								}
								this.mouseArea.Invalidate();
							}
							break;
							#endregion

							#region ..封闭路径
						case NodeOperator.CloseFigure:
							if(this.svgPathSegList.NumberOfItems > 1)
							{
								this.reversePath.Reset();
								ArrayList rectList = new ArrayList();
								this.CreateReversePath(new PointF(e.X,e.Y),rectList);
                                using (Matrix totalMatrix = this.mouseArea.GetTotalTransformForElement(this.svgPathSegListElement as SVG.SVGTransformableElement))//.TotalTransform.Clone())
								{
//									totalMatrix.Multiply(this.totalTransform);
									if(this.reversePath.PointCount >1)
										this.reversePath.Transform(totalMatrix);
									if(rectList.Count > 0)
									{
										//添加控制矩形
										PointF[] rectPs = new PointF[rectList.Count];
										rectList.CopyTo(rectPs);
										totalMatrix.TransformPoints(rectPs);
										for(int i = 0;i<rectPs.Length;i++)
										{
											PointF rectP = rectPs[i];
											this.reversePath.AddRectangle(new RectangleF(rectP.X - 1,rectP.Y,2,2));
										}
										rectPs = null;
									}
									rectList = null;
								}
								this.mouseArea.firstPoint = Point.Empty;
								this.XORDrawPath(this.reversePath);
							}
							break;
							#endregion

							#region ..转换锚点
						case NodeOperator.ConvertAnchor:
							if(this.preSeg != null)
							{
								#region ..注释代码
								//							SVGDom.Paths.SVGPathSeg pre = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(this.preSeg);
								//							SVGDom.Paths.SVGPathSeg next = null;
								//							SVGDom.Paths.SVGPathSegMove segMove = null;
								//							SVGDom.Paths.SVGPathSeg ori = this.preSeg;
								//							if(this.preSeg is SVGDom.Paths.SVGPathSegClosePath)
								//							{
								//								segMove = this.svgPathSegList.GetRelativeStartPathSeg(this.preSeg);
								//								next = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(segMove);
								//								this.preSeg = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(this.preSeg);
								//							}
								//							else
								//								next = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(this.preSeg);
								//							PointF preanchor = PointF.Empty;
								//							if(pre != null)
								//								preanchor = pre.GetLastPoint(this.svgPathSegList);
								//							PointF precontrol = preanchor;
								//							if(this.preSeg is SVGDom.Paths.SVGPathSegCurve)
								//								precontrol = ((SVGDom.Paths.SVGPathSegCurve)this.preSeg).GetFirstControl(this.svgPathSegList);
								//							PointF anchor = this.preSeg.GetLastPoint(this.svgPathSegList);
								//							PointF nextanchor = PointF.Empty;
								//							PointF nextcontrol = PointF.Empty;
								//							if(next != null)
								//							{
								//								nextanchor = next.GetLastPoint(this.svgPathSegList);
								//								nextcontrol = next.GetRelativePreControl(this.svgPathSegList);
								//							}
								//							this.currentSegs.Clear();
								//
								//							int index = this.svgPathSegList.IndexOf(this.preSeg);
								//							SVGDom.Paths.SVGPathSeg c = new SVGDom.Paths.SVGPathSegCurvetoCubicAbs(anchor.X,anchor.Y,precontrol.X,precontrol.Y,anchor.X,anchor.Y);
								//							this.svgPathSegList.ReplaceItem(c,index);
								//							this.currentSegs.Add(c);
								//
								//							if(segMove != null)
								//							{
								//								segMove.SetRelativeNextControl(anchor);
								//								segMove.SetRelativePreControl(anchor);
								//							}
								//							
								//							if(next != null)
								//							{
								//								SVGDom.Paths.SVGPathSeg c1 = new SVGDom.Paths.SVGPathSegCurvetoCubicAbs(nextanchor.X,nextanchor.Y,anchor.X,anchor.Y,nextcontrol.X,nextcontrol.Y);
								//								index = this.svgPathSegList.IndexOf(next);
								//								this.svgPathSegList.ReplaceItem(c1,index);
								//							}
								//							this.preSeg = c;
								//							if(ori is SVGDom.Paths.SVGPathSegClosePath)
								//								this.preSeg = ori;
								//							    this.sourcePath = this.svgPathSegList.GetGDIPath();
								//							if(this.svgPathSegListElement is SVGDom.Paths.SVGPathElement)
								//							{
								//								this.UpdatePath();
								//							}
								//							this.mouseArea.Invalidate();
								#endregion

								this.reversePath.Reset();
								ArrayList rectList = new ArrayList();
								this.CreateReversePath(new PointF(e.X,e.Y),rectList);
                                using (Matrix totalMatrix = this.mouseArea.GetTotalTransformForElement(this.svgPathSegListElement as SVG.SVGTransformableElement))//.TotalTransform.Clone())
								{
									//									totalMatrix.Multiply(this.totalTransform);
									if(this.reversePath.PointCount >1)
										this.reversePath.Transform(totalMatrix);
									if(rectList.Count > 0)
									{
										//添加控制矩形
										PointF[] rectPs = new PointF[rectList.Count];
										rectList.CopyTo(rectPs);
										totalMatrix.TransformPoints(rectPs);
										for(int i = 0;i<rectPs.Length;i++)
										{
											PointF rectP = rectPs[i];
											this.reversePath.AddRectangle(new RectangleF(rectP.X - 1,rectP.Y,2,2));
										}
										rectPs = null;
									}
									rectList = null;
								}
								this.XORDrawPath(this.reversePath);
							}
							break;
							#endregion

							#region ..移动路径
						case NodeOperator.MovePath:
							if(this.preSeg != null)
							{
								SVG.Paths.SVGPathSeg pre = (SVG.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(this.preSeg);
								this.time = -1;
								if(pre != null)
								{
									PointF preanchor = pre.GetLastPoint(this.svgPathSegList);
									PointF firstcontrol = pre.GetRelativeNextControl(this.svgPathSegList);
									PointF nextcontrol = this.preSeg.GetRelativePreControl(this.svgPathSegList);
									PointF anchor = this.preSeg.GetLastPoint(this.svgPathSegList);
									float t = 0;
									SplitBezierAtPoint(preanchor,firstcontrol,nextcontrol,anchor,this.PointToView(new PointF(e.X,e.Y)),out t);
									this.time = (float)Math.Round(t,1);
								}
								this.currentSegs.Clear();
								this.mouseArea.Invalidate();
							}
							break;
							#endregion
					}
					this.reversePath.Reset();
					this.mousedown = true;
					if(this.segPoint.IsEmpty)
						this.startPoint = new Point(e.X,e.Y);
					else
						this.startPoint = Point.Round(this.PointToScreen(this.segPoint));
				
					if(this.nodeOperator == NodeOperator.Draw)
					{
						this.Finish = false;
					}
					else
					{
						this.Finish = true;
						this.mouseArea.firstPoint = Point.Round(this.startPoint);
					}
				}	
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
				if(e.Button == MouseButtons.None)
				{
					this.Finish = true;
					this.preSeg = null;
					PointF p = new PointF(e.X,e.Y);
					PointF[] ps = this.svgPathSegList.GetAnchors();
					this.TransformPoints(ps);
					if (ps.Length == 0)
						return;
					this.segPoint = PointF.Empty;
					using(GraphicsPath path = new GraphicsPath())
					{
						if((Control.ModifierKeys & Keys.Control) == Keys.Control || (Control.ModifierKeys & Keys.Alt ) == Keys.Alt || this.mouseArea.CurrentOperator == Operator.NodeEdit)
						{
						
							foreach(SVG.Paths.SVGPathSeg seg in this.currentSegs)
							{
								PointF anchor = seg.GetLastPoint(this.svgPathSegList);
								PointF nextcontrol = PointF.Empty;
								PointF precontrol = PointF.Empty;
								//椭圆弧
								if(seg is SVG.Paths.SVGPathSegArc)
								{
									SVG.Paths.SVGPathSegArc arc = seg as SVG.Paths.SVGPathSegArc;
									this.CalculateArcControl(arc,out precontrol,out nextcontrol);
								}
									//贝赛尔
								else// if(seg is SVGDom.Paths.SVGPathSegMove || seg is SVGDom.Paths.SVGPathSegCurve || seg is SVGDom.Paths.SVGPathSegClosePath)
								{
									nextcontrol = seg.GetRelativeNextControl(this.svgPathSegList);
									precontrol = seg.GetRelativePreControl(this.svgPathSegList);
								}
								//							else if(seg is SVGDom.Paths.SVGPathSegLine || seg is SVGDom.Paths.SVGPathSegMove)
								//							{
								//								nextcontrol = seg.GetRelativeNextControl(this.svgPathSegList);
								//							}
							
								this.movePreControl = false;
							
								bool preIn = this.InPoint(anchor,precontrol,delta);
								bool nextIn = this.InPoint(anchor,nextcontrol,delta);
								if(!nextcontrol.IsEmpty && this.InPoint(this.PointToScreen(nextcontrol),p) && !nextIn)//Math.Abs(nextcontrol.X - p.X) <= 3 && Math.Abs(nextcontrol.Y - p.Y) <= 3 && Point.Round(nextcontrol) != anchor)
								{
									this.preSeg = seg;
									this.CurrentOperate = NodeOperator.MoveControl;
								
									return;
								}

								if(!precontrol.IsEmpty && this.InPoint(this.PointToScreen(precontrol),p) && !preIn)//Math.Abs(precontrol.X - p.X) <= 3 && Math.Abs(precontrol.Y - p.Y) <= 3 && Point.Round(precontrol) != anchor)
								{
									this.preSeg = seg;
									this.CurrentOperate = NodeOperator.MoveControl;
									this.movePreControl = true;
									return;
								}

							}
						}

						SVG.Paths.SVGPathSeg next = null;
						SVG.Paths.SVGPathSeg closeSeg = null;
						int num = 0;
                        using (Matrix totalTransform = this.mouseArea.GetTotalTransformForElement(this.svgPathSegListElement as SVG.SVGTransformableElement))
                        {
                            for (int i = ps.Length - 1; i >= 0; i--)
                            {
                                PointF tempp = ps[i];
                                SVG.Paths.SVGPathSeg seg = (SVG.Paths.SVGPathSeg)this.svgPathSegList.GetItem(i);
                                bool move = seg is SVG.Paths.SVGPathSegMove;
                                if (this.InPoint(tempp, p))//Math.Abs(tempp.X - p.X) <= 3 && Math.Abs(tempp.Y - p.Y )<=3)
                                {
                                    if (i < this.svgPathSegList.NumberOfItems)
                                        this.preSeg = seg;
                                    this.segPoint = seg.GetLastPoint(this.svgPathSegList);
                                    if (move)
                                    {
                                        move = closeSeg == null;
                                        closeSeg = null;
                                    }
                                    bool change = next is SVG.Paths.SVGPathSegMove || next == null;
                                    if (change)
                                        change = !(seg is SVG.Paths.SVGPathSegClosePath);
                                    if ((Control.ModifierKeys & Keys.Control) == Keys.Control || this.mouseArea.CurrentOperator == Operator.NodeEdit)
                                        this.CurrentOperate = NodeOperator.MoveAnchor;
                                    else if (move && num >= 1)
                                    {
                                        this.segPoint = seg.GetLastPoint(this.svgPathSegList);
                                        this.Finish = true;
                                        this.CurrentOperate = NodeOperator.CloseFigure;
                                    }
                                    else if (change && !(seg is SVG.Paths.SVGPathSegMove))
                                        this.CurrentOperate = NodeOperator.ChangeEndAnchor;
                                    else if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
                                        this.CurrentOperate = NodeOperator.ConvertAnchor;
                                    else
                                        this.CurrentOperate = NodeOperator.DelAnchor;
                                    return;
                                }
                                if (move)
                                    closeSeg = null;

                                path.Reset();
                                using (GraphicsPath temppath = seg.GetGDIPath(this.svgPathSegList))
                                {
                                    if (temppath != null && temppath.PointCount > 1)
                                        path.AddPath(temppath, false);
                                }

                                if (path != null && path.PointCount > 1 && (Control.ModifierKeys & Keys.Alt) != Keys.Alt)
                                {
                                    path.Transform(totalTransform);
                                    if (path.IsOutlineVisible(p, this.mouseArea.SelectedPen))
                                    {
                                        if (i < this.svgPathSegList.NumberOfItems)
                                            this.preSeg = (SVG.Paths.SVGPathSeg)this.svgPathSegList.GetItem(i);
                                        if ((Control.ModifierKeys & Keys.Control) == Keys.Control || this.mouseArea.CurrentOperator == Operator.NodeEdit)
                                            this.CurrentOperate = NodeOperator.MovePath;
                                        else
                                            this.CurrentOperate = NodeOperator.AddAnchor;
                                        return;
                                    }
                                }

                                next = seg;
                                if (seg is SVG.Paths.SVGPathSegMove)
                                {
                                    next = null;
                                    num = 0;
                                }
                                else
                                    num++;
                                if (seg is SVG.Paths.SVGPathSegClosePath)
                                    closeSeg = seg;
                            }


                            if ((Control.ModifierKeys & Keys.Control) == Keys.Control || this.mouseArea.CurrentOperator == Operator.NodeEdit)
                                this.CurrentOperate = NodeOperator.MoveAnchor;
                            else if ((Control.ModifierKeys & Keys.Alt) == Keys.Alt)
                                this.CurrentOperate = NodeOperator.ConvertAnchor;
                            else
                            {
                                this.CurrentOperate = NodeOperator.Draw;
                                if (this.currentSegs.Count > 0)
                                    this.mouseArea.firstPoint = Point.Round(this.PointToScreen((this.currentSegs[this.currentSegs.Count - 1] as SVG.Paths.SVGPathSeg).GetLastPoint(this.svgPathSegList)));
                                this.Finish = false;
                            }
                        }
					}
				}
					#endregion

					#region ..左键移动
				else if(e.Button == MouseButtons.Left && this.mousedown && this.nodeOperator != NodeOperator.Select)
				{
					this.mouseArea.Cursor = Forms.Cursors.Drag;
					PointF p = new PointF(e.X,e.Y );
					if(Math.Abs(e.X - this.startPoint.X) < 1 && Math.Abs(e.Y - this.startPoint.Y) < 1)
						return;
					int r = controlWidth / 2;
					using(Matrix totalMatrix = this.mouseArea.GetTotalTransformForElement(this.svgPathSegListElement as SVG.SVGTransformableElement))//.TotalTransform.Clone(),transform = new Matrix())
					{
						ArrayList rectList;
						switch(this.nodeOperator)
						{
								#region ..正常添加
							case NodeOperator.Draw:
								this.mouseArea.firstPoint = Point.Round(this.startPoint);
								this.XORDrawPath(this.reversePath);
								this.XORDrawPath(this.reverseFillPath);
								this.reversePath.Reset();
								this.reverseFillPath.Reset();
								bool createnew = false;
								if(this.svgPathSegList.NumberOfItems == 0 || this.currentSegs.Count == 0)
									createnew = true;
								else if(this.svgPathSegList.GetItem(this.svgPathSegList.NumberOfItems -1) is SVG.Paths.SVGPathSegClosePath)
								{
									createnew = true;
								}
							
								if(!createnew)
								{
									SVG.Paths.SVGPathSeg seg = (SVG.Paths.SVGPathSeg)this.svgPathSegList.GetItem(this.svgPathSegList.NumberOfItems -1);
									PointF middle = this.startPoint;
									PointF pre = seg.GetLastPoint(this.svgPathSegList);
									PointF control = seg.GetRelativeNextControl(this.svgPathSegList);
									if(control.IsEmpty)
										control = pre;
									PointF[] ps = new PointF[]{pre,control};
									totalMatrix.TransformPoints(ps);
									pre = ps[0];
									control = ps[1];
									PointF p1 = new PointF(2 * middle.X - e.X,2 * middle.Y - e.Y);
									this.reversePath.AddBezier(pre,control,p1,middle);
									this.reversePath.StartFigure();
									this.reversePath.AddLine(p1,p);
								
									this.reverseFillPath.AddRectangle(new RectangleF( middle.X - r,middle.Y - r,2*r,2*r));
									this.reverseFillPath.AddRectangle(new RectangleF(p.X - r,p.Y - r,2*r,2*r));
									this.reverseFillPath.AddRectangle(new RectangleF(p1.X - r,p1.Y - r,2*r,2*r));
								
									ps = null;
								}
								else
								{
									PointF middle = this.startPoint;
									PointF p1 = new PointF(2 * middle.X - e.X,2 * middle.Y - e.Y);
									this.reverseFillPath.AddRectangle(new RectangleF( middle.X - r,middle.Y - 1,2*r,2*r));
									this.reverseFillPath.AddRectangle(new RectangleF(p.X - r,p.Y - r,2*r,2*r));
									this.reverseFillPath.AddRectangle(new RectangleF(p1.X - r,p1.Y - r,2*r,2*r));
									this.reversePath.AddLine(p1,p);
								}
								this.XORDrawPath(this.reversePath);
								this.XORDrawPath(this.reverseFillPath);
								break;
								#endregion

								#region ..移动锚点
							case NodeOperator.MoveAnchor:
								this.XORDrawPath(this.reversePath);
								this.XORDrawPath(this.reverseFillPath);
								this.reversePath.Reset();
								this.reverseFillPath.Reset();
								//记录矩形点
								rectList = new ArrayList();
								this.CreateReversePath(new PointF(e.X,e.Y),rectList);
								if(this.reversePath.PointCount >1)
									this.reversePath.Transform(totalMatrix);

								if(rectList.Count > 0)
								{
									//添加控制矩形
									PointF[] rectPs = new PointF[rectList.Count];
									rectList.CopyTo(rectPs);
									totalMatrix.TransformPoints(rectPs);
									for(int i = 0;i<rectPs.Length;i++)
									{
										PointF rectP = rectPs[i];
										this.reverseFillPath.AddRectangle(new RectangleF(rectP.X - r,rectP.Y - r,controlWidth,controlWidth));
									}
									rectPs = null;
								}
								rectList = null;

								if(this.currentSegs.Count == 0 || selectionAdd)
								{
									float left = (float)Math.Min(e.X,this.startPoint.X);
									float top = (float)Math.Min(e.Y,this.startPoint.Y);
									float right = (float)Math.Max(e.X,this.startPoint.X);
									float bottom = (float)Math.Max(e.Y,this.startPoint.Y);
									this.reversePath.AddRectangle(new RectangleF(left,top,right - left,bottom - top));
								}
								this.XORDrawPath(this.reversePath);
								this.XORDrawPath(this.reverseFillPath);
								break;
								#endregion

								#region ..移动路径
							case NodeOperator.MovePath:
								if(this.preSeg != null)
								{
									this.XORDrawPath(this.reversePath);
									this.XORDrawPath(this.reverseFillPath);
									this.reversePath.Reset();
									this.reverseFillPath.Reset();
									SVG.Paths.SVGPathSeg pre = (SVG.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(this.preSeg);
									if(pre != null)
									{
										PointF preanchor = pre.GetLastPoint(this.svgPathSegList);
										PointF firstcontrol = pre.GetRelativeNextControl(this.svgPathSegList);
										PointF nextcontrol = this.preSeg.GetRelativePreControl(this.svgPathSegList);
										PointF anchor = this.preSeg.GetLastPoint(this.svgPathSegList);
										if(firstcontrol.X == preanchor.X)
											firstcontrol.X += 0.1f;
										if(nextcontrol.X == anchor.X)
											nextcontrol.X += 0.1f;

										#region ..移动SVGPathSegCurve
										if(this.time >= 0.1 && this.time <= 0.9 && this.preSeg is SVG.Paths.SVGPathSegCurve)
										{
											bool equal1 = Math.Abs(preanchor.X - firstcontrol.X) < 0.5f;
											bool equal2 = Math.Abs(anchor.X - nextcontrol.X) < 0.5f;
											PointF[] ps = new PointF[]{preanchor,firstcontrol,nextcontrol,anchor};
											totalMatrix.TransformPoints(ps);
											preanchor = ps[0];
											firstcontrol = ps[1];
											nextcontrol = ps[2];
											anchor = ps[3];

											#region ..求P2,P3
											float t = this.time;
											float a = (float)Math.Pow( 1- this.time,3);
											float b = 3 *(1-t) *(1-t) * t;
											float c = 3 * t * t * (1 - t);
											float d = t * t * t;
											float k2 = (float)(firstcontrol.Y - preanchor.Y ) / (float)(firstcontrol.X - preanchor.X);
											float b2 = -k2 * preanchor.X + preanchor.Y;
										
											float k3 = (float)(nextcontrol.Y - anchor.Y ) / (float)(nextcontrol.X - anchor.X);
											float b3 = -k3 * anchor.X + anchor.Y;
											float px = e.X - a * preanchor.X - d * anchor.X;
											float py = e.Y -a * preanchor.Y - d * anchor.Y - b * b2 - c * b3;
											float x3 = e.X;
											if(c * (k2 - k3)!= 0)
												x3 = (k2 * px - py)/ (c * (k2 - k3));
								
											float y3 = k3 * x3 + b3;
											float x2 = (px - c * x3) / b;
											float y2 = k2 * x2 + b2;

											if(equal1)
											{
												x2 = preanchor.X;
												y2 = firstcontrol.Y + e.Y - this.startPoint.Y;
											}
										
											if(equal2)
											{
												y3 = nextcontrol.Y + e.Y - this.startPoint.Y;
												x3 = anchor.X;
											}

											#region ..当第一个改变方向时
											if((x2 - preanchor.X) * (firstcontrol.X - preanchor.X) < 0 && pre is SVG.Paths.SVGPathSegCurve)
											{
												SVG.Paths.SVGPathSeg prepre = (SVG.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(pre);
												PointF prepreanchor = PointF.Empty;
												PointF preprecontrol = PointF.Empty;
												if(prepre != null)
												{
													prepreanchor = prepre.GetLastPoint(this.svgPathSegList);
													preprecontrol = prepre.GetRelativeNextControl(this.svgPathSegList);
												}
												if(!prepreanchor.IsEmpty )
												{
													PointF prenextcontrol = pre.GetRelativePreControl(this.svgPathSegList);
													ps = new PointF[]{prepreanchor,preprecontrol,prenextcontrol};
													totalMatrix.TransformPoints(ps);
													float angle1 = (float)Math.Round(Math.Atan2(ps[2].Y - preanchor.Y,ps[2].X - preanchor.X),3);
													float angle2 = (float)Math.Round(Math.Atan2(preanchor.Y - firstcontrol.Y,preanchor.X - firstcontrol.X),3);
													if(angle1 == angle2)
													{
														float r1 = (float)Math.Sqrt(Math.Pow(ps[2].X - preanchor.X,2) + Math.Pow(ps[2].Y - preanchor.Y ,2));
														float r2 = (float)Math.Sqrt(Math.Pow(preanchor.X - x2,2) + Math.Pow (preanchor.Y - y2,2));
														if(r2 > 0 )
														{
															float rx = (preanchor.X - x2) / r2;
															float x = preanchor.X + rx * r1;
															float ry = (preanchor.Y - y2) / r2;
															float y = preanchor.Y + ry * r1;
															this.reversePath.StartFigure();
															this.reversePath.AddBezier(ps[0],ps[1],new PointF(x,y),preanchor);
															this.reversePath.StartFigure();
															this.reversePath.AddLine(preanchor,new PointF(x,y));
															this.reversePath.AddRectangle(new RectangleF(x - 1,y -1,2,2));
															this.reversePath.StartFigure();
															this.reversePath.AddLine(ps[0],ps[1]);
															this.reversePath.AddRectangle(new RectangleF(ps[1].X - 1,ps[1].Y - 1,2,2));
														}
													}
												}
											}
											#endregion
									
											#region ..当第二个锚点改变方向时
											if((x3 - anchor.X) * (nextcontrol.X - anchor.X) < 0 && this.preSeg is SVG.Paths.SVGPathSegCurve)
											{
												SVG.Paths.SVGPathSeg nextnext = (SVG.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(this.preSeg);
												PointF prepreanchor = PointF.Empty;
												PointF preprecontrol = PointF.Empty;
												if(nextnext != null)
												{
													prepreanchor = nextnext.GetLastPoint(this.svgPathSegList);
													preprecontrol = nextnext.GetRelativePreControl(this.svgPathSegList);
												}
												if(!prepreanchor.IsEmpty )
												{
													PointF prenextcontrol = this.preSeg.GetRelativeNextControl(this.svgPathSegList);
													ps = new PointF[]{prepreanchor,preprecontrol,prenextcontrol};
													totalMatrix.TransformPoints(ps);
													float angle1 = (float)Math.Round(Math.Atan2(ps[2].Y - anchor.Y,ps[2].X - anchor.X),3);
													float angle2 = (float)Math.Round(Math.Atan2(anchor.Y - nextcontrol.Y,anchor.X - nextcontrol.X),3);
													if(angle1 == angle2)
													{
														float r1 = (float)Math.Sqrt(Math.Pow(ps[2].X - anchor.X,2) + Math.Pow(ps[2].Y - anchor.Y ,2));
														float r2 = (float)Math.Sqrt(Math.Pow(anchor.X - x3,2) + Math.Pow (anchor.Y - y3,2));
														if(r2 > 0 )
														{
															float rx = (anchor.X - x3) / r2;
															float x = anchor.X + rx * r1;
															float ry = (anchor.Y - y3) / r2;
															float y = anchor.Y + ry * r1;
															this.reversePath.StartFigure();
															this.reversePath.AddBezier(anchor,new PointF(x,y),ps[1],ps[0]);
															this.reversePath.StartFigure();
															this.reversePath.AddLine(anchor,new PointF(x,y));
															this.reversePath.AddRectangle(new RectangleF(x - 1,y -1,2,2));
															this.reversePath.StartFigure();
															this.reversePath.AddLine(ps[0],ps[1]);
															this.reversePath.AddRectangle(new RectangleF(ps[1].X - 1,ps[1].Y - 1,2,2));
														}
													}
													ps = null;
												}
											}
											#endregion

											this.reversePath.AddBezier(preanchor,new PointF(x2,y2),new PointF(x3,y3),anchor);
											this.reversePath.StartFigure();
											this.reversePath.AddLine(preanchor,new PointF(x2,y2));
											this.reverseFillPath.AddRectangle(new RectangleF(x2 - 1*r,y2 - 1*r,2*r,2*r));
											this.reversePath.StartFigure();
											this.reversePath.AddLine(anchor,new PointF(x3,y3));
											this.reverseFillPath.AddRectangle(new RectangleF(x3 - 1*r,y3 - 1*r,2*r,2*r));
											#endregion
										}
											#endregion

											#region ..移动SVGPathLine
										else if(this.preSeg is SVG.Paths.SVGPathSegLine || this.preSeg is SVG.Paths.SVGPathSegClosePath)
										{
											PointF end = this.PointToView(p);
											PointF start = this.PointToView(this.startPoint);
											SVG.Paths.SVGPathSeg next = this.svgPathSegList.NextSibling(this.preSeg) as SVG.Paths.SVGPathSeg;
											if(pre is SVG.Paths.SVGPathSegMove)
											{
												SVG.Paths.SVGPathSeg pre1 = this.svgPathSegList.GetRelativeClosePathSeg(this.preSeg);
												if(pre1 != null)
												{
													SVG.Paths.SVGPathSeg pre2 = this.svgPathSegList.PreviousSibling(pre1) as SVG.Paths.SVGPathSeg;
													if(pre2 != null && pre1.GetLastPoint(this.svgPathSegList) == pre2.GetLastPoint(this.svgPathSegList))
														pre = pre2;
													else
														pre = pre1;
												}
											}
											if(this.preSeg is SVG.Paths.SVGPathSegClosePath)
											{
												next = this.svgPathSegList.GetRelativeStartPathSeg(this.preSeg);
												next = this.svgPathSegList.NextSibling(next) as SVG.Paths.SVGPathSeg;
											}
											else if(next is SVG.Paths.SVGPathSegClosePath)
											{
												if(next.GetLastPoint(this.svgPathSegList) == this.preSeg.GetLastPoint(this.svgPathSegList))
												{
													next = this.svgPathSegList.GetRelativeStartPathSeg(this.preSeg);
													next = this.svgPathSegList.NextSibling(next) as SVG.Paths.SVGPathSeg;
												}
											}

											PointF tempp1 = this.preSeg.GetLastPoint(this.svgPathSegList);
											tempp1.X += end.X - start.X;
											tempp1.Y += end.Y - start.Y;

											if(pre != null)
											{
												PointF tempp = pre.GetLastPoint(this.svgPathSegList);
												tempp.X += end.X - start.X;
												tempp.Y += end.Y - start.Y;
												this.CreateMovePath(pre,false,tempp);
												this.reversePath.StartFigure();
												this.reversePath.AddLine(tempp,tempp1);
												PointF[] ps1 = new PointF[]{tempp,tempp1};
												totalMatrix.TransformPoints(ps1);
												this.reverseFillPath.AddRectangle(new RectangleF(ps1[0].X - r,ps1[0].Y - r,2 * r,2*r));
												this.reverseFillPath.AddRectangle(new RectangleF(ps1[1].X - r,ps1[1].Y - r,2 * r,2*r));
											}

											if(next != null)
												this.CreateMovePath(next,true,tempp1);
											
											this.reversePath.Transform(totalMatrix);
										
											#region ..注释代码
											//										SVGDom.Paths.SVGPathSeg prepre = null;
											//										if(pre is SVGDom.Paths.SVGPathSegMove)
											//										{
											//											prepre = this.svgPathSegList.GetRelativeClosePathSeg(pre);
											//											if(prepre != null)
											//												prepre = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(prepre);
											//										}
											//										else
											//										{
											//											prepre = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(pre);
											//										}
											//										SVGDom.Paths.SVGPathSeg next = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(this.preSeg);
											//										PointF prepreanchor = PointF.Empty;
											//										PointF preprecontrol = PointF.Empty;
											//										PointF nextanchor = PointF.Empty;
											//										PointF nextcontrol2 = PointF.Empty;
											//										PointF nextcontrol1 = this.preSeg.GetRelativeNextControl(this.svgPathSegList);
											//
											//										if(prepre != null)
											//										{
											//											prepreanchor = prepre.GetLastPoint(this.svgPathSegList);
											//											preprecontrol = prepre.GetRelativeNextControl(this.svgPathSegList);
											//										}
											//
											//										if(next != null)
											//										{
											//											nextanchor = next.GetLastPoint(this.svgPathSegList);
											//											nextcontrol2 = next.GetRelativePreControl(this.svgPathSegList);
											//										}
											//
											//										PointF prenextcontrol = pre.GetRelativePreControl(this.svgPathSegList);
											//										PointF[] ps = new PointF[]{prepreanchor,preprecontrol,prenextcontrol,preanchor,anchor,nextcontrol1,nextcontrol2,nextanchor};
											//										totalMatrix.TransformPoints(ps);
											//										prenextcontrol = ps[2];
											//										preanchor = ps[3];
											//										anchor = ps[4];
											//										nextcontrol1 = ps[5];
											//										transform.Translate(e.X - this.startPoint.X,e.Y - this.startPoint.Y);
											//										PointF[] ps1 = new PointF[]{prenextcontrol,preanchor,anchor,nextcontrol1};
											//										transform.TransformPoints(ps1);
											//										if(!prepreanchor.IsEmpty)
											//										{
											//											this.reversePath.AddBezier(ps[0],ps[1],ps1[0],ps1[1]);
											//											this.reversePath.StartFigure();
											//										}
											//										this.reversePath.AddLine(ps1[1],ps1[2]);
											//									
											//										if(!nextanchor.IsEmpty)
											//										{
											//											this.reversePath.StartFigure();
											//											this.reversePath.AddBezier(ps1[2],ps1[3],ps[6],ps[7]);
											//										}
											//										ps = null;
											//										ps1 = null;
											#endregion
										}
										#endregion
									}
									this.XORDrawPath(this.reversePath);
									this.XORDrawPath(this.reverseFillPath);
								}
								break;
								#endregion

								#region ..移动控制点
							case NodeOperator.MoveControl:
							case NodeOperator.ChangeEndAnchor:
								//							if(this.nodeOperator == NodeOperator.ChangeEndAnchor && this.svgPathSegList.NumberOfItems > 1)
								//							{
								//								this.preSeg = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.GetItem(this.svgPathSegList.NumberOfItems -1);
								//								this.movePreControl = false;
								//							}
								if(this.preSeg != null)
								{
									this.XORDrawPath(this.reversePath);
									this.XORDrawPath(this.reverseFillPath);
									this.reverseFillPath.Reset();
									this.reversePath.Reset();

									rectList = new ArrayList();
									this.CreateReversePath(new PointF(e.X,e.Y),rectList);
									if(this.reversePath.PointCount >1)
										this.reversePath.Transform(totalMatrix);

									if(rectList.Count > 0)
									{
										//添加控制矩形
										PointF[] rectPs = new PointF[rectList.Count];
										rectList.CopyTo(rectPs);
										totalMatrix.TransformPoints(rectPs);
										for(int i = 0;i<rectPs.Length;i++)
										{
											PointF rectP = rectPs[i];
											this.reverseFillPath.StartFigure();
											this.reverseFillPath.AddRectangle(new RectangleF(rectP.X - 1*r,rectP.Y-r,2*r,2*r));
								
										}
										rectPs = null;
									}
									rectList = null;

									#region ..注释代码
									//								SVGDom.Paths.SVGPathSeg oriPre = this.preSeg;
									//								SVGDom.Paths.SVGPathSeg seg = null;//(SVGDom.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(this.preSeg);
									//								if(!(this.preSeg is SVGDom.Paths.SVGPathSegClosePath))
									//									seg = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(this.preSeg);
									//								else
									//								{
									//									seg = this.svgPathSegList.GetRelativeStartPathSeg(this.preSeg);
									//									if(seg != null)
									//									{
									//										PointF tempp1 = seg.GetLastPoint(this.svgPathSegList);
									//										PointF tempp2 = this.preSeg.GetLastPoint(this.svgPathSegList);
									//										if(Math.Abs(tempp1.X - tempp2.X ) <1 && Math.Abs(tempp1.Y - tempp2.Y ) < 1)
									//											seg = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(seg);
									//									}
									//									oriPre = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(this.preSeg);
									//								}
									//								PointF anchor = oriPre.GetLastPoint(this.svgPathSegList);
									//								PointF precontrol = oriPre.GetRelativePreControl(this.svgPathSegList);
									//								PointF nextcontrol = oriPre.GetRelativeNextControl(this.svgPathSegList);
									//								SVGDom.Paths.SVGPathSeg pre = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(oriPre);
									//								PointF preanchor = PointF.Empty;
									//								PointF precontrol1 = PointF.Empty;
									//								if(pre != null)
									//								{
									//									preanchor = pre.GetLastPoint(this.svgPathSegList);
									//									precontrol1 = pre.GetRelativeNextControl(this.svgPathSegList);
									//								}
									//
									//								PointF nextanchor = PointF.Empty;
									//								PointF nextcontrol2 = PointF.Empty;
									//								if(seg != null)
									//								{
									//									nextanchor = seg.GetLastPoint(this.svgPathSegList);
									//									nextcontrol2 = seg.GetRelativePreControl(this.svgPathSegList);
									//								}
									//								PointF[] ps = new PointF[]{preanchor,precontrol1,precontrol,anchor,nextcontrol,nextcontrol2,nextanchor};
									//								SVGDom.Paths.SVGPathSeg cseg = oriPre;
									//								SVGDom.Paths.SVGPathSeg nseg = seg;
									//								if(!this.movePreControl)
									//								{
									//									cseg = seg;
									//									nseg = oriPre;
									//									ps = new PointF[]{nextanchor,nextcontrol2,nextcontrol,anchor,precontrol,precontrol1,preanchor};
									//								}
									//								totalMatrix.TransformPoints(ps);
									//								if(cseg is SVGDom.Paths.SVGPathSegCurve)
									//								{
									//									this.reversePath.AddBezier(ps[0],ps[1],new PointF(e.X,e.Y),ps[3]);
									//								}
									//								this.reversePath.StartFigure();
									//								this.reversePath.AddLine(ps[3],new PointF(e.X,e.Y));
									//								this.reversePath.AddRectangle(new RectangleF(e.X - 1,e.Y - 1,2,2));
									//
									////								float angle1 = (float)Math.Round(Math.Atan2(nextcontrol.Y - anchor.Y,nextcontrol.X - anchor.X),3);
									////								float angle2 = (float)Math.Round(Math.Atan2(anchor.Y - precontrol.Y,anchor.X - precontrol.X),3);
									//								bool inline = this.InLine(nextcontrol,anchor,precontrol);
									//								if( inline && (Control.ModifierKeys & Keys.Alt) != Keys.Alt)
									//								{
									//									float r1 = (float)Math.Sqrt(Math.Pow(ps[3].X - ps[4].X,2) + Math.Pow(ps[3].Y - ps[4].Y ,2));
									//									float r2 = (float)Math.Sqrt(Math.Pow(ps[3].X - e.X,2) + Math.Pow (ps[3].Y - e.Y,2));
									//									if(r2 > 0 )
									//									{
									//										float rx = (ps[3].X - e.X) / r2;
									//										float x = ps[3].X + rx * r1;
									//										float ry = (ps[3].Y - e.Y) / r2;
									//										float y = ps[3].Y + ry * r1;
									//										if(nseg is SVGDom.Paths.SVGPathSegCurve )
									//											this.reversePath.AddBezier(ps[3],new PointF(x,y),ps[5],ps[6]);
									//										this.reversePath.StartFigure();
									//										this.reversePath.AddLine(new PointF(x,y),ps[3]);
									//										this.reversePath.AddRectangle(new RectangleF(x - 2,y - 2,4,4));
									//									}
									//								}
									#endregion

									this.XORDrawPath(this.reversePath);
									this.XORDrawPath(this.reverseFillPath);
								}
								break;
								#endregion

								#region ..封闭路径
							case NodeOperator.CloseFigure:
								#region ..注释代码
								if(this.preSeg is SVG.Paths.SVGPathSegCurve)
								{
									this.XORDrawPath(this.reversePath);
									this.reversePath.Reset();
								
									SVG.Paths.SVGPathSegCurve c = (SVG.Paths.SVGPathSegCurve)this.preSeg;
									PointF anchor = c.GetLastPoint(this.svgPathSegList);
									PointF control2 = c.GetSecondControl(this.svgPathSegList);
									PointF control1 = c.GetFirstControl(this.svgPathSegList);
									PointF preanchor = PointF.Empty;
									PointF nextcontrol1 = PointF.Empty,nextcontrol2 = PointF.Empty,nextanchor = PointF.Empty;
									SVG.Paths.SVGPathSeg svgMove = this.svgPathSegList.GetRelativeStartPathSeg(this.preSeg);
									if(svgMove != null && (Control.ModifierKeys & Keys.Alt) != Keys.Alt)
									{
										SVG.Paths.SVGPathSeg next = (SVG.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(svgMove);
										if(next is SVG.Paths.SVGPathSegCurve)
										{
											nextcontrol1 = ((SVG.Paths.SVGPathSegCurve)next).GetFirstControl(this.svgPathSegList);
											nextcontrol2 = ((SVG.Paths.SVGPathSegCurve)next).GetSecondControl(this.svgPathSegList);
											nextanchor = ((SVG.Paths.SVGPathSegCurve)next).GetLastPoint(this.svgPathSegList);
										}
									}
									SVG.Paths.SVGPathSeg seg = (SVG.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(this.preSeg);
									if(seg != null)
										preanchor = seg.GetLastPoint(this.svgPathSegList);
									PointF[] ps = new PointF[]{anchor,control1,control2,preanchor,nextcontrol1,nextcontrol2,nextanchor};
									totalMatrix.TransformPoints(ps);
									PointF p1 = new PointF( 2 * ps[0].X - e.X,2 * ps[0].Y - e.Y);
									if(!preanchor.IsEmpty)
										this.reversePath.AddBezier(ps[3],ps[1],p1,ps[0]);
									this.reversePath.StartFigure();
									this.reversePath.AddLine(p1,ps[0]);
									this.reversePath.AddRectangle(new RectangleF(p1.X - 1,p1.Y - 1,2,2));
									if(!nextanchor.IsEmpty && !nextcontrol2.IsEmpty)
									{
										float r1 = (float)Math.Sqrt(Math.Pow(ps[0].X - ps[4].X,2) + Math.Pow(ps[0].Y - ps[4].Y ,2));
										float r2 = (float)Math.Sqrt(Math.Pow(ps[0].X - p1.X,2) + Math.Pow (ps[0].Y - p1.Y,2));
										if(r2 > 0 )
										{
											float rx = (ps[0].X - p1.X) / r2;
											float x = ps[0].X + rx * r1;
											float ry = (ps[0].Y - p1.Y) / r2;
											float y = ps[0].Y + ry * r1;

											this.reversePath.AddBezier(ps[0],new PointF(x,y),ps[5],ps[6]);
											this.reversePath.StartFigure();
											this.reversePath.AddLine(new PointF(x,y),ps[0]);
											this.reversePath.AddRectangle(new RectangleF(x - 1,y - 1,2,2));
										}
									}
									ps = null;
									this.XORDrawPath(this.reversePath);
								}
								#endregion
								this.XORDrawPath(this.reversePath);
								this.XORDrawPath(this.reverseFillPath);
								this.reversePath.Reset();
								this.reverseFillPath.Reset();
								rectList = new ArrayList();
								this.CreateReversePath(new PointF(e.X,e.Y),rectList);
								if(this.reversePath.PointCount >1)
									this.reversePath.Transform(totalMatrix);
								if(rectList.Count > 0)
								{
									//添加控制矩形
									PointF[] rectPs = new PointF[rectList.Count];
									rectList.CopyTo(rectPs);
									totalMatrix.TransformPoints(rectPs);
									for(int i = 0;i<rectPs.Length;i++)
									{
										PointF rectP = rectPs[i];
										this.reverseFillPath.AddRectangle(new RectangleF(rectP.X - 1*r,rectP.Y-r,2*r,2*r));
									}
									rectPs = null;
								}
								rectList = null;
								this.XORDrawPath(this.reversePath);
								this.XORDrawPath(this.reverseFillPath);
								break;
								#endregion

								#region ..转换锚点
							case NodeOperator.ConvertAnchor:
								if(this.preSeg != null)
								{
									this.XORDrawPath(this.reversePath);
									this.XORDrawPath(this.reverseFillPath);
									this.reverseFillPath.Reset();
									this.reversePath.Reset();
									rectList = new ArrayList();
									this.CreateReversePath(new PointF(e.X,e.Y),rectList);
									if(this.reversePath.PointCount >1)
										this.reversePath.Transform(totalMatrix);
									if(rectList.Count > 0)
									{
										//添加控制矩形
										PointF[] rectPs = new PointF[rectList.Count];
										rectList.CopyTo(rectPs);
										totalMatrix.TransformPoints(rectPs);
										for(int i = 0;i<rectPs.Length;i++)
										{
											PointF rectP = rectPs[i];
											this.reverseFillPath.AddRectangle(new RectangleF(rectP.X - r,rectP.Y-r,2*r,2*r));
										}
										rectPs = null;
									}
									rectList = null;

									#region ..注释代阿
									//								SVGDom.Paths.SVGPathSeg next = null;
									//								SVGDom.Paths.SVGPathSegMove segMove = null;
									//								SVGDom.Paths.SVGPathSeg ori = this.preSeg;
									//								if(this.preSeg is SVGDom.Paths.SVGPathSegClosePath)
									//								{
									//									segMove = this.svgPathSegList.GetRelativeStartPathSeg(this.preSeg);
									//									next = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(segMove);
									//									ori = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(ori);
									//								}
									//								else
									//									next = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(ori);
									//								SVGDom.Paths.SVGPathSeg pre = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(ori);
									//								PointF preanchor = PointF.Empty;
									//								if(pre != null)
									//									preanchor = pre.GetLastPoint(this.svgPathSegList);
									//								PointF precontrol = preanchor;
									//								if(ori is SVGDom.Paths.SVGPathSegCurve)
									//									precontrol = ((SVGDom.Paths.SVGPathSegCurve)ori).GetFirstControl(this.svgPathSegList);
									//								PointF anchor = ori.GetLastPoint(this.svgPathSegList);
									//								PointF nextanchor = PointF.Empty;
									//								if(next != null)
									//									nextanchor = next.GetLastPoint(this.svgPathSegList);
									//								PointF nextcontrol = nextanchor;
									//								this.currentSegs.Clear();
									//								if(next is SVGDom.Paths.SVGPathSegCurve)
									//								{
									//									nextcontrol = ((SVGDom.Paths.SVGPathSegCurve)next).GetSecondControl(this.svgPathSegList);
									//								}
									//
									//								PointF[] ps = new PointF[]{preanchor,precontrol,anchor,nextcontrol,nextanchor};
									//								totalMatrix.TransformPoints(ps);
									//								PointF p1 = new PointF( 2 * ps[2].X - e.X,2 * ps[2].Y - e.Y);
									//								if(!preanchor.IsEmpty)
									//									this.reversePath.AddBezier(ps[0],ps[1],p1,ps[2]);
									//								this.reversePath.StartFigure();
									//								this.reversePath.AddLine(p1,ps[2]);
									//								this.reversePath.AddRectangle(new RectangleF(p1.X - 1,p1.Y - 1,2,2));
									//								this.reversePath.StartFigure();
									//								if(!nextanchor.IsEmpty)
									//								{
									//									this.reversePath.AddBezier(ps[2],p,ps[3],ps[4]);
									//									this.reversePath.StartFigure();
									//									this.reversePath.AddLine(ps[2],p);
									//									this.reversePath.AddRectangle(new RectangleF(e.X - 1,e.Y - 1,2,2));
									//								}
									#endregion

									this.XORDrawPath(this.reversePath);
									this.XORDrawPath(this.reverseFillPath);
								}
								break;
								#endregion
						}
					}
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
				bool deal = false;
				if(e.Button == MouseButtons.Left && this.mousedown && this.nodeOperator != NodeOperator.Select)
				{
					float ex = e.X;
					float ey = e.Y;
					if(Math.Abs(ex - this.startPoint.X) <= 1 && Math.Abs(ey - this.startPoint.Y)<= 1)
					{
						ex = this.startPoint.X;
						ey = this.startPoint.Y;
					}
					//				this.Invalidate();
					PointF p = this.PointToView(new PointF(ex,ey));
					PointF start = this.PointToView(this.startPoint);
					using(Matrix transform = new Matrix())
					{
						switch(this.nodeOperator)
						{
								#region ..正常添加
							case NodeOperator.Draw:
								this.XORDrawPath(this.reversePath);
								this.XORDrawPath(this.reverseFillPath);
								bool createnew = false;
							
								if(this.svgPathSegList.NumberOfItems == 0 || this.currentSegs.Count == 0)
									createnew = true;
								else if(this.svgPathSegList.GetItem(this.svgPathSegList.NumberOfItems -1) is SVG.Paths.SVGPathSegClosePath)
									createnew = true;
								this.currentSegs.Clear();
								if(createnew)
								{
									PointF middle = this.PointToView(this.startPoint);
									PointF p1 = new PointF((float)Math.Round(2 * middle.X - p.X,0),(float)Math.Round(2 * middle.Y - p.Y,0));
									SVG.Paths.SVGPathSegMove seg = new SVG.Paths.SVGPathSegMovetoAbs((float)Math.Round(middle.X,0),(float)Math.Round(middle.Y,0));
									((SVG.Paths.SVGPathSegMove)seg).SetRelativeNextControl(p);
									((SVG.Paths.SVGPathSegMove)seg).SetRelativePreControl(p1);
									this.svgPathSegList.AppendItem(seg);
									this.currentSegs.Add(seg);
									this.sourcePath = this.svgPathSegList.GetGDIPath();
									deal  = true;
								}
								else
								{
									PointF middle = this.PointToView(this.startPoint);
									PointF p1 = new PointF(2 * middle.X - p.X,2 * middle.Y - p.Y);
									SVG.Paths.SVGPathSeg seg = (SVG.Paths.SVGPathSeg)this.svgPathSegList.GetItem(this.svgPathSegList.NumberOfItems -1);
									PointF pre = seg.GetLastPoint(this.svgPathSegList);
									PointF control = seg.GetRelativeNextControl(this.svgPathSegList);
									if(control.IsEmpty)
										control = pre;
									SVG.Paths.SVGPathSeg seg2 = this.CreateSeg(pre,control,p1,middle);//new SVGDom.Paths.SVGPathSegCurvetoCubicAbs(middle.X,middle.Y,control.X,control.Y,p1.X,p1.Y);
									if(seg2 != null)
									{
										this.svgPathSegList.AppendItem(seg2);
										this.currentSegs.Add(seg2);
										this.sourcePath = this.svgPathSegList.GetGDIPath();
										seg2.SetRelativeNextControl(p);
										deal  = true;
									}
								}
								this.mouseArea.firstPoint = Point.Round(this.startPoint);
								break;
								#endregion

								#region ..移动锚点
							case NodeOperator.MoveAnchor:
								this.XORDrawPath(this.reversePath);
								this.XORDrawPath(this.reverseFillPath);

								#region ..注释代码
								//							ArrayList list = (ArrayList)this.currentSegs.Clone();
								//							SVGDom.Paths.SVGPathSegList list1 = this.svgPathSegList.Clone();
								//							for(int i = 0;i<this.currentSegs.Count;i++)
								//							{
								//								SVGDom.Paths.SVGPathSeg preSeg1 = (SVGDom.Paths.SVGPathSeg)this.currentSegs[i];
								//								PointF middle = preSeg1.GetLastPoint(list1);
								//								PointF pre = preSeg1.GetRelativePreControl(list1);
								//								PointF nextcontrol = preSeg1.GetRelativeNextControl(this.svgPathSegList);
								//								PointF start = this.PointToView(this.startPoint);
								//								PointF end = this.PointToView(new PointF(e.X,e.Y));
								//								Matrix matrix = new Matrix();
								//								matrix.Translate(end.X - start.X,end.Y - start.Y);
								//								SVGDom.Paths.SVGPathSeg next = null;
								//								SVGDom.Paths.SVGPathSegMove segMove = null;
								//								SVGDom.Paths.SVGPathSeg ori = preSeg1;
								//
								//								if(!(preSeg1 is SVGDom.Paths.SVGPathSegClosePath))
								//								{
								//									next = (SVGDom.Paths.SVGPathSeg)list1.NextSibling(preSeg1);
								//								}
								//								else
								//								{
								//									next = segMove = list1.GetRelativeStartPathSeg(preSeg1);
								//									preSeg1 = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(preSeg1);
								//									if(next != null)
								//									{
								//										PointF tempp1 = next.GetLastPoint(list1);
								//										PointF tempp2 = preSeg1.GetLastPoint(list1);
								//										if(Math.Abs(tempp1.X - tempp2.X ) <1 && Math.Abs(tempp1.Y - tempp2.Y ) < 1)
								//											next = (SVGDom.Paths.SVGPathSeg)list1.NextSibling(next);
								//									}
								//								}
								//
								//								SVGDom.Paths.SVGPathSeg prepreSeg = (SVGDom.Paths.SVGPathSeg)list1.PreviousSibling(preSeg1);
								//
								//								if(next is SVGDom.Paths.SVGPathSegCurve)
								//								{
								//									nextcontrol = ((SVGDom.Paths.SVGPathSegCurve)next).GetFirstControl(list1);
								//								}
								//							
								//								int index1 = list.IndexOf(next);
								//								PointF[] ps = new PointF[]{pre,middle,nextcontrol};
								//								matrix.TransformPoints(ps);
								//								SVGDom.Paths.SVGPathSeg replaceseg = null;
								//							
								//								if(preSeg1 is SVGDom.Paths.SVGPathSegCurve)
								//								{
								//									PointF precontrol1 = ((SVGDom.Paths.SVGPathSegCurve)preSeg1).GetFirstControl(list1);
								//									PointF[] ps1 = new PointF[]{precontrol1};
								//									if(this.currentSegs.Contains(prepreSeg))
								//										matrix.TransformPoints(ps1);
								//									replaceseg = new SVGDom.Paths.SVGPathSegCurvetoCubicAbs(ps[1].X,ps[1].Y,ps1[0].X,ps1[0].Y,ps[0].X,ps[0].Y);
								//								}
								//								else if(preSeg1 is SVGDom.Paths.SVGPathSegLine)
								//								{
								//									replaceseg = new SVGDom.Paths.SVGPathSegLinetoAbs(ps[1].X,ps[1].Y);
								//								}
								//								else if(preSeg1 is SVGDom.Paths.SVGPathSegMove)
								//								{
								//									replaceseg = new SVGDom.Paths.SVGPathSegMovetoAbs(ps[1].X,ps[1].Y);
								//									((SVGDom.Paths.SVGPathSegMove)replaceseg).SetRelativePreControl(ps[0]);
								//								}
								//							
								//								int index = 0;
								//								index = this.svgPathSegList.IndexOf(preSeg1);
								//								int index3 = list1.IndexOf(preSeg1);
								//								this.svgPathSegList .ReplaceItem(replaceseg,index);
								//								replaceseg.SetRelativeNextControl(ps[2]);
								//							
								//								index = list.IndexOf(preSeg1);
								//								if(index>= 0 && index < list.Count)
								//								{
								//									list.Insert(index,replaceseg);
								//									list.Remove(preSeg1);
								//								}
								//							
								//								if(next is SVGDom.Paths.SVGPathSegCurve && !this.currentSegs.Contains(next))
								//								{
								//									SVGDom.Paths.SVGPathSegCurve c1 = (SVGDom.Paths.SVGPathSegCurve)next;
								//									PointF nextanchor = next.GetLastPoint(list1);
								//									PointF nextcontrol2 = c1.GetSecondControl(list1);
								//									SVGDom.Paths.SVGPathSeg c = this.CreateSeg(ps[1],ps[2],nextcontrol2,nextanchor);//new SVGDom.Paths.SVGPathSegCurvetoCubicAbs(nextanchor.X,nextanchor.Y,ps[2].X,ps[2].Y,nextcontrol2.X,nextcontrol2.Y);
								//									if(c != null)
								//									{
								//										c.SetRelativeNextControl(next.GetRelativeNextControl(this.svgPathSegList));
								//										index = this.svgPathSegList.IndexOf(next);
								//										this.svgPathSegList.ReplaceItem(c,index);
								//									}
								//								}
								//								if(segMove != null && !this.currentSegs.Contains(segMove))
								//								{
								//									SVGDom.Paths.SVGPathSegMove m = new SVGDom.Paths.SVGPathSegMovetoAbs(ps[1].X,ps[1].Y);
								//									index = this.svgPathSegList.IndexOf(segMove);
								//									this.svgPathSegList.ReplaceItem(m,index);
								//									m.SetRelativeNextControl(ps[2]);
								//									m.SetRelativePreControl(ps[0]);
								//								}
								//
								//								if(ori != null)
								//								{
								//									ori.SetRelativeNextControl(ps[2]);
								//								}
								//							}
								#endregion
						
								//选择单元
								if(this.currentSegs.Count == 0 || selectionAdd)
								{
									//								PointF start = this.PointToView(this.startPoint);
									float left = (float)Math.Min(p.X,start.X);
									float top = (float)Math.Min(p.Y,start.Y);
									float right = (float)Math.Max(p.X,start.X);
									float bottom = (float)Math.Max(p.Y,start.Y);
									RectangleF rect = new RectangleF(left,top,right - left,bottom - top);
									PointF[] ps = this.svgPathSegList.GetAnchors();
									//this.currentSegs.Clear();
									for(int i = ps.Length - 1;i>= 0;i--)
									{
										PointF tempp = ps[i];
										if(rect.Contains(tempp))
											this.currentSegs.Add((SVG.Paths.SVGPathSeg)this.svgPathSegList.GetItem(i));
									}
									this.mouseArea.Invalidate();
								}
								else
								{
									//								this.currentSegs = (ArrayList)list.Clone();
									this.UpdateSegList(new PointF(e.X,e.Y));
									this.sourcePath = this.svgPathSegList.GetGDIPath();
									deal  = true;
								}
								break;
								#endregion

								#region ..移动路径
							case NodeOperator.MovePath:
								if(this.preSeg != null)
								{
									this.XORDrawPath(this.reversePath);
									this.XORDrawPath(this.reverseFillPath);
									SVG.Paths.SVGPathSeg pre = (SVG.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(this.preSeg);
									if(pre != null)
									{
										PointF preanchor = pre.GetLastPoint(this.svgPathSegList);
										PointF firstcontrol = pre.GetRelativeNextControl(this.svgPathSegList);
										PointF nextcontrol = this.preSeg.GetRelativePreControl(this.svgPathSegList);
										PointF anchor = this.preSeg.GetLastPoint(this.svgPathSegList);
										if(firstcontrol.X == preanchor.X)
											firstcontrol.X += 0.1f;
										if(nextcontrol.X == anchor.X)
											nextcontrol.X += 0.1f;
								
										//									PointF[] ps = new PointF[]{preanchor,firstcontrol,nextcontrol,anchor};
										//									(this.svgPathSegListElement as SVGDom.SVGTransformableElement).TotalTransform.TransformPoints(ps);
										#region ..移动SVGPathSegCurve
										if(this.preSeg is SVG.Paths.SVGPathSegCurve)
										{
											bool equal1 = Math.Abs(preanchor.X - firstcontrol.X) < 0.5f;
											bool equal2 = Math.Abs(anchor.X - nextcontrol.X) < 0.5f;

											#region ..求P2,P3
											if(this.time >= 0.1 && this.time <= 0.9)
											{
												float t = this.time;
												float a = (float)Math.Pow( 1- this.time,3);
												float b = 3 *(1-t) *(1-t) * t;
												float c = 3 * t * t * (1 - t);
												float d = t * t * t;
												float k2 = (float)(firstcontrol.Y - preanchor.Y ) / (float)(firstcontrol.X - preanchor.X);
												float b2 = -k2 * preanchor.X + preanchor.Y;
										
												float k3 = (float)(nextcontrol.Y - anchor.Y ) / (float)(nextcontrol.X - anchor.X);
												float b3 = -k3 * anchor.X + anchor.Y;
												float px = p.X - a * preanchor.X - d * anchor.X;
												float py = p.Y -a * preanchor.Y - d * anchor.Y - b * b2 - c * b3;
												float x3 = p.X;
												if(c * (k2 - k3)!= 0)
													x3 = (k2 * px - py)/ (c * (k2 - k3));
								
												float y3 = k3 * x3 + b3;
												float x2 = (px - c * x3) / b;
												float y2 = k2 * x2 + b2;

												if(equal1)
												{
													x2 = preanchor.X;
													y2 = firstcontrol.Y + p.Y - start.Y;
												}
										
												if(equal2)
												{
													y3 = nextcontrol.Y + p.Y - start.Y;
													x3 = anchor.X;
												}
                                        
												int index = 0;
												PointF[] ps = new PointF[0];

												#region ..当第一个改变方向时
												if((x2 - preanchor.X) * (firstcontrol.X - preanchor.X) < 0 && pre is SVG.Paths.SVGPathSegCurve)
												{
													SVG.Paths.SVGPathSeg prepre = (SVG.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(pre);
													PointF prepreanchor = PointF.Empty;
													PointF preprecontrol = PointF.Empty;
													if(prepre != null)
													{
														prepreanchor = prepre.GetLastPoint(this.svgPathSegList);
														preprecontrol = prepre.GetRelativeNextControl(this.svgPathSegList);
													}
													if(!prepreanchor.IsEmpty )
													{
														PointF prenextcontrol = pre.GetRelativePreControl(this.svgPathSegList);
														ps = new PointF[]{prepreanchor,preprecontrol,prenextcontrol};
														float angle1 = (float)Math.Round(Math.Atan2(ps[2].Y - preanchor.Y,ps[2].X - preanchor.X),3);
														float angle2 = (float)Math.Round(Math.Atan2(preanchor.Y - firstcontrol.Y,preanchor.X - firstcontrol.X),3);
														if(angle1 == angle2)
														{
															float r1 = (float)Math.Sqrt(Math.Pow(ps[2].X - preanchor.X,2) + Math.Pow(ps[2].Y - preanchor.Y ,2));
															float r2 = (float)Math.Sqrt(Math.Pow(preanchor.X - x2,2) + Math.Pow (preanchor.Y - y2,2));
															if(r2 > 0 )
															{
																float rx = (preanchor.X - x2) / r2;
																float x = preanchor.X + rx * r1;
																float ry = (preanchor.Y - y2) / r2;
																float y = preanchor.Y + ry * r1;
																index = this.svgPathSegList.IndexOf(pre);
																SVG.Paths.SVGPathSegCurve tempc = new SVG.Paths.SVGPathSegCurvetoCubicAbs(preanchor.X,preanchor.Y,preprecontrol.X,preprecontrol.Y,x,y);
																this.svgPathSegList.ReplaceItem(tempc,index);
															}
														}
													}
												}
												#endregion
									
												#region ..当第二个锚点改变方向时
												if((x3 - anchor.X) * (nextcontrol.X - anchor.X) < 0 && this.preSeg is SVG.Paths.SVGPathSegCurve)
												{
													SVG.Paths.SVGPathSeg nextnext = (SVG.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(this.preSeg);
													PointF prepreanchor = PointF.Empty;
													PointF preprecontrol = PointF.Empty;
													if(nextnext != null)
													{
														prepreanchor = nextnext.GetLastPoint(this.svgPathSegList);
														preprecontrol = nextnext.GetRelativePreControl(this.svgPathSegList);
													}
													if(!prepreanchor.IsEmpty )
													{
														PointF prenextcontrol = this.preSeg.GetRelativeNextControl(this.svgPathSegList);
														ps = new PointF[]{prepreanchor,preprecontrol,prenextcontrol};
														float angle1 = (float)Math.Round(Math.Atan2(ps[2].Y - anchor.Y,ps[2].X - anchor.X),3);
														float angle2 = (float)Math.Round(Math.Atan2(anchor.Y - nextcontrol.Y,anchor.X - nextcontrol.X),3);
														if(angle1 == angle2)
														{
															float r1 = (float)Math.Sqrt(Math.Pow(ps[2].X - anchor.X,2) + Math.Pow(ps[2].Y - anchor.Y ,2));
															float r2 = (float)Math.Sqrt(Math.Pow(anchor.X - x3,2) + Math.Pow (anchor.Y - y3,2));
															if(r2 > 0 )
															{
																float rx = (anchor.X - x3) / r2;
																float x = anchor.X + rx * r1;
																float ry = (anchor.Y - y3) / r2;
																float y = anchor.Y + ry * r1;

																index = this.svgPathSegList.IndexOf(nextnext);
																SVG.Paths.SVGPathSegCurve tempc = new SVG.Paths.SVGPathSegCurvetoCubicAbs(ps[0].X,ps[0].Y,x,y,ps[1].X,ps[1].Y);
																this.svgPathSegList.ReplaceItem(tempc,index);
															}
														}
													}
												}
												#endregion

												index = this.svgPathSegList.IndexOf(this.preSeg);
												SVG.Paths.SVGPathSegCurve c1 = new SVG.Paths.SVGPathSegCurvetoCubicAbs(anchor.X,anchor.Y,x2,y2,x3,y3);
												c1.SetRelativeNextControl(this.preSeg.GetRelativeNextControl(this.svgPathSegList));
												this.svgPathSegList.ReplaceItem(c1,index);
												this.currentSegs.Clear();
												this.currentSegs.Add(c1);
												deal = true;
												ps = null;
												this.sourcePath = this.svgPathSegList.GetGDIPath();
												ps = null;
											}
											#endregion
										}
											#endregion
								
											#region ..移动SVGPathLine
										else if(this.preSeg is SVG.Paths.SVGPathSegLine || this.preSeg is SVG.Paths.SVGPathSegClosePath)
										{
											PointF end = p;//this.PointToView(p);
											//										PointF start = this.PointToView(this.startPoint);
											SVG.Paths.SVGPathSeg next = this.svgPathSegList.NextSibling(this.preSeg) as SVG.Paths.SVGPathSeg;
											int index = this.svgPathSegList.IndexOf(this.preSeg);
											if(pre != null)
											{
												PointF tempp = pre.GetLastPoint(this.svgPathSegList);
												tempp.X += end.X - start.X;
												tempp.Y += end.Y - start.Y;
												this.UpdateMovePathSeg(pre,tempp);

												if(pre is SVG.Paths.SVGPathSegMove)
												{
													SVG.Paths.SVGPathSeg pre1 = this.svgPathSegList.GetRelativeClosePathSeg(this.preSeg);
													if(pre1 != null)
													{
														SVG.Paths.SVGPathSeg pre2 = this.svgPathSegList.PreviousSibling(pre1) as SVG.Paths.SVGPathSeg;
														if(pre2 != null && pre.GetLastPoint(this.svgPathSegList) == pre2.GetLastPoint(this.svgPathSegList))
															this.UpdateMovePathSeg(pre2,tempp);
													}
												}
											}
											PointF tempp1 = this.preSeg.GetLastPoint(this.svgPathSegList);
											tempp1.X += end.X - start.X;
											tempp1.Y += end.Y - start.Y;

											if(this.preSeg is SVG.Paths.SVGPathSegClosePath)
											{
												next = this.svgPathSegList.GetRelativeStartPathSeg(this.preSeg);
												if(next != null)
													this.UpdateMovePathSeg(next,tempp1);
											}
											else 
											{
												if(next is SVG.Paths.SVGPathSegClosePath && next.GetLastPoint(this.svgPathSegList) == this.preSeg.GetLastPoint(this.svgPathSegList))
												{
													next = this.svgPathSegList.GetRelativeStartPathSeg(next);
													if(next != null)
														this.UpdateMovePathSeg(next,tempp1);
												}
												this.UpdateMovePathSeg(this.preSeg,tempp1);
											}
											if(index>= 0 && index < this.svgPathSegList.NumberOfItems)
											{
												this.currentSegs.Clear();
												this.currentSegs.Add(this.svgPathSegList.GetItem(index));
											}
											deal = true;
											this.sourcePath = this.svgPathSegList.GetGDIPath();

											#region ..注释代码
											//										SVGDom.Paths.SVGPathSeg prepre = null;
											//										SVGDom.Paths.SVGPathSeg closeSeg = null;
											//										if(pre is SVGDom.Paths.SVGPathSegMove)
											//										{
											//											closeSeg = prepre = this.svgPathSegList.GetRelativeClosePathSeg(pre);
											//											if(prepre != null)
											//												prepre = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(prepre);
											//										}
											//										else
											//											prepre = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(pre);
											//										SVGDom.Paths.SVGPathSeg next = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(this.preSeg);
											//										PointF prepreanchor = PointF.Empty;
											//										PointF preprecontrol = PointF.Empty;
											//										PointF nextanchor = PointF.Empty;
											//										PointF nextcontrol2 = PointF.Empty;
											//										PointF nextcontrol1 = this.preSeg.GetRelativeNextControl(this.svgPathSegList);
											//										if(prepre != null)
											//										{
											//											prepreanchor = prepre.GetLastPoint(this.svgPathSegList);
											//											preprecontrol = prepre.GetRelativeNextControl(this.svgPathSegList);
											//										}
											//
											//										if(next != null)
											//										{
											//											nextanchor = next.GetLastPoint(this.svgPathSegList);
											//											nextcontrol2 = next.GetRelativePreControl(this.svgPathSegList);
											//										}
											//
											//										int index = 0;
											//										PointF prenextcontrol = pre.GetRelativePreControl(this.svgPathSegList);
											//										PointF[] ps = new PointF[]{prepreanchor,preprecontrol,prenextcontrol,preanchor,anchor,nextcontrol1,nextcontrol2,nextanchor};
											//										PointF start = this.PointToView(this.startPoint);
											//										transform.Translate(p.X - start.X,p.Y - start.Y);
											//										PointF[] ps1 = new PointF[]{prenextcontrol,preanchor,anchor,nextcontrol1};
											//										transform.TransformPoints(ps1);
											//										if(pre is SVGDom.Paths.SVGPathSegMove)
											//										{
											//											index = this.svgPathSegList.IndexOf(pre);
											//											SVGDom.Paths.SVGPathSegMove move = new SVGDom.Paths.SVGPathSegMovetoAbs(ps1[1].X,ps1[1].Y);
											//											this.svgPathSegList.ReplaceItem(move,index);
											//											move.SetRelativePreControl(ps1[0]);
											//
											//											if(closeSeg != null)
											//											{
											//												index = this.svgPathSegList.IndexOf(closeSeg);
											//												SVGDom.Paths.SVGPathSeg tempc = this.CreateSeg(ps[0],ps[1],ps1[0],ps1[1]);
											//												if(tempc != null)
											//												{
											//													this.svgPathSegList.ReplaceItem(tempc,index);
											//													tempc.SetRelativeNextControl(ps1[1]);
											//												}
											//											}
											//										}
											//										else if(!prepreanchor.IsEmpty)
											//										{
											//											index = this.svgPathSegList.IndexOf(pre);
											//											SVGDom.Paths.SVGPathSeg seg = this.CreateSeg(ps[0],ps[1],ps1[0],ps1[1]);
											//											if(seg != null)
											//											{
											//												this.svgPathSegList.ReplaceItem(seg,index);
											//											}
											//										}
											//									
											//									
											//										index = this.svgPathSegList.IndexOf(this.preSeg);
											//										SVGDom.Paths.SVGPathSeg seg1 = this.CreateSeg(ps1[1],ps1[1],ps1[2],ps1[2]);
											//										if(seg1 != null)
											//										{
											//											this.svgPathSegList.ReplaceItem(seg1,index);
											//											seg1.SetRelativeNextControl(ps1[3]);
											//										}
											//									
											//										if(!nextanchor.IsEmpty)
											//										{
											//											index = this.svgPathSegList.IndexOf(next);
											//											SVGDom.Paths.SVGPathSeg seg = this.CreateSeg(ps1[2],ps1[3],ps[6],ps[7]);
											//											if(seg != null)
											//											{
											//												this.svgPathSegList.ReplaceItem(seg,index);
											//											}
											//										}
											//
											//										deal = true;
											//										this.sourcePath = this.svgPathSegList.GetGDIPath();
											#endregion
										}
									
										#endregion
									}
								}
								break;
								#endregion

								#region ..移动控制点
							case NodeOperator.MoveControl:
							case NodeOperator.ChangeEndAnchor:
								if(this.preSeg != null)
								{
									this.UpdateSegList(new PointF(e.X,e.Y));
									this.sourcePath = this.svgPathSegList.GetGDIPath();
									deal = true;
								}
								if(this.nodeOperator == NodeOperator.ChangeEndAnchor && this.svgPathSegList.NumberOfItems > 1)
								{
									if(!this.currentSegs.Contains(this.preSeg))
									{
										this.currentSegs.Clear();
										this.currentSegs.Add(this.preSeg);
									}
								}
								break;
								#endregion

								#region ..封闭路径
							case NodeOperator.CloseFigure:
								if(this.preSeg != null)
								{
									this.XORDrawPath(this.reversePath);
									this.XORDrawPath(this.reverseFillPath);
									this.currentSegs.Clear();
								
									#region ..注释代码
									//								if(this.preSeg is SVGDom.Paths.SVGPathSegCurve )
									//								{
									//									//								if(Math.Abs(e.X - this.startPoint.X) >= 1 && Math.Abs(e.Y - this.startPoint.Y) >= 1)
									//									//								{
									//									SVGDom.Paths.SVGPathSeg pre = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(this.preSeg);
									//									PointF preanchor = PointF.Empty;
									//									if(pre != null)
									//										preanchor = pre.GetLastPoint(this.svgPathSegList);
									//									PointF anchor = this.preSeg.GetLastPoint(this.svgPathSegList);
									//									PointF firstcontrol1 = ((SVGDom.Paths.SVGPathSegCurve)this.preSeg).GetFirstControl(this.svgPathSegList);
									//									PointF p1 = new PointF(2 * anchor.X - p.X,2 * anchor.Y - p.Y);
									//									SVGDom.Paths.SVGPathSeg c = this.CreateSeg(preanchor,firstcontrol1,p1,anchor);//new SVGDom.Paths.SVGPathSegCurvetoCubicAbs(anchor.X,anchor.Y,firstcontrol1.X,firstcontrol1.Y,2 * anchor.X - p.X,2 * anchor.Y - p.Y);
									//									if(c != null && !this.InPoint(this.startPoint,new PointF(e.X,e.Y)))
									//									{
									//										int index = this.svgPathSegList.IndexOf(this.preSeg);
									//										this.svgPathSegList.ReplaceItem(c,index);
									//									
									//										SVGDom.Paths.SVGPathSegMove segMove = this.svgPathSegList.GetRelativeStartPathSeg(c);
									//										if(segMove != null && (Keys.Alt & Control.ModifierKeys) != Keys.Alt)
									//										{
									//											segMove.SetRelativePreControl(p1);	
									//											SVGDom.Paths.SVGPathSeg seg1 = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(this.preSeg);								
									//											SVGDom.Paths.SVGPathSeg seg = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(segMove);
									//											if(seg is SVGDom.Paths.SVGPathSegCurve)
									//											{
									//												PointF nextcontrol2 = ((SVGDom.Paths.SVGPathSegCurve)seg).GetSecondControl(this.svgPathSegList);
									//												PointF control1 = ((SVGDom.Paths.SVGPathSegCurve)seg).GetFirstControl(this.svgPathSegList);
									//												PointF nextanchor = ((SVGDom.Paths.SVGPathSegCurve)seg).GetLastPoint(this.svgPathSegList);
									//												float r1 = (float)Math.Sqrt(Math.Pow(anchor.X - control1.X,2) + Math.Pow(anchor.Y - control1.Y  ,2));
									//												float r2 = (float)Math.Sqrt(Math.Pow(anchor.X - p1.X,2) + Math.Pow (anchor.Y - p1.Y,2));
									//												if(r2 > 0 )
									//												{
									//													float rx = (anchor.X - p1.X) / r2;
									//													float x = anchor.X + rx * r1;
									//													float ry = (anchor.Y - p1.Y) / r2;
									//													float y = anchor.Y + ry * r1;
									//													p = new PointF(x,y);
									//													SVGDom.Paths.SVGPathSegCurve c2 = new SVGDom.Paths.SVGPathSegCurvetoCubicAbs(nextanchor.X,nextanchor.Y,x,y,nextcontrol2.X,nextcontrol2.Y);
									//													index = this.svgPathSegList.IndexOf(seg);
									//													segMove.SetRelativePreControl(new PointF(x,y));
									//													segMove.SetRelativePreControl(p1);
									//													c.SetRelativeNextControl(new PointF(x,y));
									//													this.svgPathSegList.ReplaceItem(c2,index);
									//												}
									//											}
									//										}
									//									}
									//								
									//
									//									SVGDom.Paths.SVGPathSegClosePath c1 = new SVGDom.Paths.SVGPathSegClosePath();
									//									this.svgPathSegList.AppendItem(c1);
									//									c1.SetRelativeNextControl(p);
									//									this.currentSegs.Clear();
									//									this.currentSegs.Add(c1);
									//								}
									#endregion

									this.UpdateSegList(new PointF(ex,ey));
									deal = true;
									this.currentSegs.Add(this.preSeg);
									this.sourcePath = this.svgPathSegList.GetGDIPath();
								}
								break;
								#endregion

								#region ..转换锚点
							case NodeOperator.ConvertAnchor:
								if(this.preSeg != null)
								{
									this.XORDrawPath(this.reversePath);
									this.XORDrawPath(this.reverseFillPath);

									#region ..注释代码
									//								SVGDom.Paths.SVGPathSeg next = null;
									//								SVGDom.Paths.SVGPathSegMove segMove = null;
									//								SVGDom.Paths.SVGPathSeg ori = this.preSeg;
									//								if(this.preSeg is SVGDom.Paths.SVGPathSegClosePath)
									//								{
									//									segMove = this.svgPathSegList.GetRelativeStartPathSeg(this.preSeg);
									//									next = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(segMove);
									//									this.preSeg = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(this.preSeg);
									//								}
									//								else
									//									next = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(this.preSeg);
									//								SVGDom.Paths.SVGPathSeg pre = (SVGDom.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(this.preSeg);
									//								PointF preanchor = PointF.Empty;
									//								if(pre != null)
									//									preanchor = pre.GetLastPoint(this.svgPathSegList);
									//								PointF precontrol = preanchor;
									//								if(this.preSeg is SVGDom.Paths.SVGPathSegCurve)
									//									precontrol = ((SVGDom.Paths.SVGPathSegCurve)this.preSeg).GetFirstControl(this.svgPathSegList);
									//								PointF anchor = this.preSeg.GetLastPoint(this.svgPathSegList);
									//								PointF nextanchor = PointF.Empty;
									//								PointF nextcontrol = PointF.Empty;
									//								if(next != null)
									//								{
									//									nextanchor = next.GetLastPoint(this.svgPathSegList);
									//									nextcontrol = next.GetRelativePreControl(this.svgPathSegList);
									//								}
									//
									//								PointF p1 = new PointF( 2 * anchor.X - p.X,2 * anchor.Y - p.Y);
									//								int index = this.svgPathSegList.IndexOf(this.preSeg);
									//								SVGDom.Paths.SVGPathSeg c = new SVGDom.Paths.SVGPathSegCurvetoCubicAbs(anchor.X,anchor.Y,precontrol.X,precontrol.Y,p1.X,p1.Y);
									//								this.svgPathSegList.ReplaceItem(c,index);
									//								c.SetRelativeNextControl(p);
									//								if(ori != null)
									//									ori.SetRelativeNextControl(p);
									//								if(segMove != null)
									//								{
									//									segMove.SetRelativeNextControl(p1);
									//									segMove.SetRelativePreControl(p);
									//								}
									//								this.currentSegs.Clear();
									//								this.currentSegs.Add(c);
									//								if(next != null)
									//								{
									//									SVGDom.Paths.SVGPathSeg c1 = new SVGDom.Paths.SVGPathSegCurvetoCubicAbs(nextanchor.X,nextanchor.Y,p.X,p.Y,nextcontrol.X,nextcontrol.Y);
									//									index = this.svgPathSegList.IndexOf(next);
									//									this.svgPathSegList.ReplaceItem(c1,index);
									//								}
									#endregion
									this.UpdateSegList(new PointF(e.X,e.Y));
									if(!this.currentSegs.Contains(this.preSeg))
									{
										this.currentSegs.Clear();
										this.currentSegs.Add(this.preSeg);
									}
									deal = true;
									this.sourcePath = this.svgPathSegList.GetGDIPath();
								}
								break;
								#endregion
						}

					}
				}

				if(deal)
				{
					if(this.svgPathSegListElement is SVG.Paths.SVGPathElement)
						this.UpdatePath();
					this.mouseArea.Invalidate();
				}
				this.preSeg = null;
				this.movePreControl = false;
				this.reversePath.Reset();
				this.mousedown = false;
				this.mouseArea.Cursor = this.mouseArea.DefaultCursor;
				//			if(select)
				//			{
				//				this.parentOperation.SelectElement(new PointF(e.X,e.Y));
				//			}
			}
			catch{}

            this.selectionAdd = false;
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
			
				if(this.svgPathSegListElement != null)
				{
					using(Matrix matrix = this.mouseArea.GetTotalTransformForElement(this.svgPathSegListElement as SVG.SVGTransformableElement))//.TotalTransform.Clone())
					{
                        using (GraphicsPath drawPath = new GraphicsPath())
                        {
                            GraphicsContainer c = e.Graphics.BeginContainer();

                            if (this.operateTime != this.mouseArea.SVGDocument.CurrentTime)
                            {
                                if (this.svgPathSegListElement != null && (this.svgPathSegListElement as SVG.SVGElement).ParentNode == null)
                                    this.svgPathSegListElement = null;
                                if (this.svgPathSegListElement != null)
                                {
                                    this.svgPathSegList = ((SVG.Paths.SVGPathSegList)this.svgPathSegListElement.SVGPathSegList.NormalSVGPathSegList);
                                    this.sourcePath = this.svgPathSegList.GetGDIPath();

                                    if (this.svgPathSegList.NumberOfItems > 0 && this.currentSegs.Count > 0)
                                    {
                                        this.currentSegs.Clear();
                                        this.currentSegs.Add(this.svgPathSegList.GetItem(this.svgPathSegList.NumberOfItems - 1));
                                    }
                                    else
                                        this.currentSegs.Clear();
                                }
                                else
                                {
                                    this.svgPathSegList.Clear();
                                    this.sourcePath.Reset();
                                    this.currentSegs.Clear();
                                }
                            }

                            this.operateTime = this.mouseArea.SVGDocument.CurrentTime;
                            e.Graphics.Transform = matrix;

                            int r = this.mouseArea.grapSize * 2 / 3;
                            using (Brush brush = new SolidBrush(this.mouseArea.HighlightAnchor))
                            {
                                using (Pen pen = new Pen(ControlPaint.DarkDark(this.mouseArea.HighlightAnchor), 1))
                                {
                                    #region ..选择
                                    ArrayList points = new ArrayList();
                                    for (int i = 0; i < this.currentSegs.Count; i++)//(SVGDom.Paths.SVGPathSeg seg in this.currentSegs)
                                    {
                                        SVG.Paths.SVGPathSeg seg = this.currentSegs[i] as SVG.Paths.SVGPathSeg;
                                        if (this.svgPathSegList.Contains(seg))
                                        {
                                            PointF p = seg.GetLastPoint(this.svgPathSegList);
                                            //								p = new PointF((int)Math.Round(p.X,0),(int)Math.Round(p.Y,0));
                                            points.Add(p);

                                            SVG.Interface.Paths.ISVGPathSeg pre, next;
                                            this.SplitSelectSeg(seg, out pre, out next);
                                            this.DrawSegControl(pre as SVG.Paths.SVGPathSeg, points, p, true, pre != next, drawPath);
                                            this.DrawSegControl(next as SVG.Paths.SVGPathSeg, points, p, false, pre != next, drawPath);
                                        }
                                    }

                                    e.Graphics.EndContainer(c);

                                    e.Graphics.Transform.Reset();
                                    if (drawPath.PointCount > 1)
                                    {
                                        drawPath.Transform(matrix);
                                        e.Graphics.DrawPath(this.mouseArea.SelectedPen, drawPath);
                                    }

                                    PointF[] ps = ((SVG.Paths.SVGPathSegList)this.svgPathSegList).GetAnchors();
                                    if (ps.Length > 1)
                                    {
                                        PointF[] ps1 = ps.Clone() as PointF[];
                                        matrix.TransformPoints(ps);

                                        //							using(Brush brush = new SolidBrush(this.mouseArea.selectedColor))
                                        //							{
                                        for (int i = 0; i < ps.Length; i++)//(PointF p in ps)
                                        {
                                            PointF p = ps[i];
                                            e.Graphics.DrawRectangle(pen, p.X - r, p.Y - r, 2 * r, 2 * r);
                                            e.Graphics.FillRectangle(this.mouseArea.UnselectedBrush, p.X - r, p.Y - r, 2 * r, 2 * r);
                                        }
                                        //							}
                                        ps1 = null;
                                    }

                                    if (points.Count > 0)
                                    {
                                        PointF[] ps1 = new PointF[points.Count];
                                        points.CopyTo(ps1);
                                        matrix.TransformPoints(ps1);
                                        for (int i = 0; i < ps1.Length; i++)//(PointF p in ps1)
                                        {
                                            PointF p = ps1[i];
                                            e.Graphics.DrawRectangle(pen, p.X - r, p.Y - r, 2 * r, 2 * r);
                                            e.Graphics.FillRectangle(brush, p.X - r, p.Y - r, 2 * r, 2 * r);
                                        }
                                    }
                                    ps = null;
                                    points = null;
                                    #endregion
                                }
                            }
                        }
					}
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

		#region ..转换坐标
		/// <summary>
		/// 将屏幕坐标转换为当前视图坐标
		/// </summary>
		PointF PointToView(PointF screenPoint)
		{
//			return this.mouseArea.PointToVirtualView(screenPoint);
			PointF[] ps = new PointF[]{screenPoint};
			using(Matrix matrix = this.mouseArea.GetTotalTransformForElement(this.svgPathSegListElement as SVG.SVGTransformableElement))//.TotalTransform.Clone())
			{
//				matrix.Multiply(this.totalTransform);
				matrix.Invert();
				matrix.TransformPoints(ps);
			}
			PointF p = ps[0];//new PointF((float)Math.Round(ps[0].X,0),(float)Math.Round(ps[0].Y,0));
			ps = null;
			return p;
		}

		void TransformPoints(PointF[] points)
		{
			if(points == null || points.Length == 0)
				return;
            using (Matrix matrix = this.mouseArea.GetTotalTransformForElement(this.svgPathSegListElement as SVG.SVGTransformableElement))//.TotalTransform.Clone())
			{
//				matrix.Multiply(this.totalTransform);
				matrix.TransformPoints(points);
			}
		}

		PointF PointToScreen(PointF viewPoint)
		{
			PointF[] ps = new PointF[]{viewPoint};
            using (Matrix matrix = this.mouseArea.GetTotalTransformForElement(this.svgPathSegListElement as SVG.SVGTransformableElement))//.TotalTransform.Clone())
			{
//				matrix.Multiply(this.totalTransform);
				matrix.TransformPoints(ps);
			}
			PointF p = ps[0];//new PointF((float)Math.Round(ps[0].X,0),(float)Math.Round(ps[0].Y,0));
			ps = null;
			return p;
		}
		#endregion

		#region ..在曲线上某点分割曲线
		/// <summary>
		/// 在曲线上某点分割曲线
		/// </summary>
		/// <param name="start">第一个锚点</param>
		/// <param name="control1">第一控制点</param>
		/// <param name="control2">第二控制点</param>
		/// <param name="end">第二个锚点</param>
		/// <param name="point">要求的点</param>
		/// <returns></returns>
		internal static PointF[] SplitBezierAtPoint(PointF start,PointF control1,PointF control2,PointF end,PointF point,out float t)
		{
			GraphicsPath path = new GraphicsPath();
			path.AddBezier(start,control1,control2,end);
			using(Pen pen = new Pen(Color.Black,5))
			{
				if(!path.IsOutlineVisible(point,pen))
				{
					t = -1;
					path.Dispose();
					return new PointF[0];
				}
				PointF p1 = PointF.Empty;
				PointF p2 = PointF.Empty;
				PointF p3 = PointF.Empty;
				PointF p4 = PointF.Empty;
				PointF p6 = PointF.Empty;

				t = 0.5f;
				PointF[] points = SplitBezierAtT(start,control1,control2,end,t);
				PointF p5 = points[2];
				float d = (float)Math.Sqrt(Math.Pow(p5.X - point.X,2) + Math.Pow(p5.Y - point.Y,2));
			
				while(d > 3)
				{
					path = new GraphicsPath();
					path.AddBezier(start,points[0],points[1],points[2]);
					if(path.IsOutlineVisible(point,pen))
					{
						t = t / 2f;
					}
					else
					{
						t = t + t/ 2f;
					}
					points = SplitBezierAtT(start,control1,control2,end,t);
					p5 = points[2];
					d = (float)Math.Sqrt(Math.Pow(p5.X - point.X,2) + Math.Pow(p5.Y - point.Y,2));
				}
			
				path.Dispose();
				return points;
			}
		}
		#endregion

		#region ..在指定t时刻点分割贝赛尔曲线
		/// <summary>
		/// 在指定t时刻点分割贝赛尔曲线
		/// </summary>
		/// <param name="start">第一锚点</param>
		/// <param name="control1">第一控制单</param>
		/// <param name="control2">第二控制点</param>
		/// <param name="end">第二锚点</param>
		/// <param name="t">时刻t</param>
		/// <returns>分割后的曲线控制点数组</returns>
		internal static PointF[] SplitBezierAtT(PointF start,PointF control1,PointF control2,PointF end,float t)
		{
			PointF p1 = PointF.Empty;
			PointF p2 = PointF.Empty;
			PointF p3 = PointF.Empty;
			PointF p4 = PointF.Empty;
			PointF p5 = PointF.Empty;
			PointF p6 = PointF.Empty;

			p1 = new PointF(start.X * (1-t) + control1.X *t,start.Y * (1-t) + control1.Y * t);
			p2 = new PointF(control1.X *(1-t) + control2.X*t,control2.Y * t+ control1.Y *(1-t));
			p3 = new PointF(end.X*t + control2.X *(1-t),end.Y * t+ control2.Y * (1-t));
			p4 = new PointF(p1.X*(1-t) + p2.X*t,p1.Y *(1-t) + p2.Y *t);
			p5 = new PointF(p2.X *(1-t)+ p3.X*t,p2.Y*(1-t) + p3.Y *t);
			p6 = new PointF(p4.X *(1-t)+ p5.X*t,p5.Y *t+ p4.Y *(1-t));
			
			return new PointF[]{p1,p4,p6,p5,p3};
		}
		#endregion

		#region ..创建路径单元
		/// <summary>
		/// 创建路径单元
		/// </summary>
		/// <param name="p1">第一锚点</param>
		/// <param name="p2">第一控制点</param>
		/// <param name="p3">第二控制点</param>
		/// <param name="p4">第二锚点</param>
		SVG.Paths.SVGPathSeg CreateSeg(PointF p1,PointF p2,PointF p3,PointF p4)
		{
			p1 = new PointF((float)Math.Round(p1.X,0),(float)Math.Round(p1.Y,0));
			p2 = new PointF((float)Math.Round(p2.X,0),(float)Math.Round(p2.Y,0));
			p3 = new PointF((float)Math.Round(p3.X,0),(float)Math.Round(p3.Y,0));
			p4 = new PointF((float)Math.Round(p4.X,0),(float)Math.Round(p4.Y,0));
			if(p1 == p2 && p3 == p4)
			{
				if(p2 == p3)
					return null;
				else //if(p2.X != p4.X && p2.Y != p4.Y)
					return new SVG.Paths.SVGPathSegLinetoAbs(p3.X,p3.Y);
//				else if(p2.X == p4.X)
//					return new SVGDom.Paths.SVGPathSegLinetoVerticalAbs(p4.Y);
//				else if(p2.X == p3.X)
//					return new SVGDom.Paths.SVGPathSegLinetoHorizontalAbs(p4.Y);
//				else
//					return null;
			}
//			else if(p1 == p2)
//			{
//				return new SVGDom.Paths.SVGPathSegCurvetoCubicSmoothAbs(p4.X,p4.Y,p3.X,p3.Y);
//			}
//			else if(p3 == p4)
//				return new SVGDom.Paths.SVGPathSegCurvetoQuadraticAbs(p4.X,p4.Y,p2.X,p2.Y);
//			else
				return new SVG.Paths.SVGPathSegCurvetoCubicAbs(p4.X,p4.Y,p2.X,p2.Y,p3.X,p3.Y);
		}
		#endregion

		#region ..处理数据
		void UpdatePath()
		{
			if(this.svgPathSegListElement == null)
				return;
			SVG.SVGElement element = (SVG.SVGElement)this.svgPathSegListElement;

			string str = this.svgPathSegList.PathString;
			SVG.Document.SVGDocument doc = this.mouseArea.SVGDocument;
			bool old = doc.AcceptNodeChanged;
			//如果对象为SVGStyleable，根据情况改变其属性或生成动画
//			if(element is SVGDom.SVGStyleable)
//				((SVGDom.SVGStyleable)element).UpdateAttribute("d",str);
//			else
//			{
				doc.AcceptNodeChanged = true;
				element.InternalSetAttribute("d",str.Trim());
//			}
			if(element.ParentNode == null)
			{
                this.svgPathSegListElement = (SVG.Interface.ISVGPathSegListElement)this.mouseArea.AddElement((SVG.SVGElement)this.svgPathSegListElement);
                //(this.svgPathSegListElement as SVGDom.SVGTransformableElement).TotalTransform = (this.mouseArea.SVGDocument.DocumentElement as SVGDom.DocumentStructure.SVGGElement).TotalTransform.Clone();
			}
            else
            {
                if (this.svgPathSegListElement is SVG.SVGTransformableElement)
                    ((SVG.SVGTransformableElement)this.svgPathSegListElement).UpdatePath();
            }
            this.mouseArea.selectChanged = true;
			doc.InvokeUndos();
			doc.AcceptNodeChanged = old;

			this.pathstring = str;
		}
		#endregion

		#region ..重置
		/// <summary>
		/// 重置
		/// </summary>
		internal override void Reset()
		{
			this.svgPathSegListElement = null;
			this.currentSegs.Clear();
//			this.svgPathSegList.Clear();
			this.mouseArea.Invalidate();
			base.Reset();
		}
		#endregion

		#region ..判断三点是否在一条直线上
		/// <summary>
		/// 判断三点是否在一条直线上
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="p3"></param>
		/// <returns></returns>
		bool InLine(PointF p1,PointF p2,PointF p3)
		{
			if((int)p1.X == (int)p3.X)
				return (int)Math.Abs(p2.X - p1.X) <= lineDelta;
			else if((int)p1.Y == (int)p3.Y)
				return (int)Math.Abs(p2.Y - p1.Y) <= lineDelta;
			else
			{
				float k = (p3.Y - p1.Y) / (p3.X - p1.X);
				float a = p3.Y - k * p3.X;
				float dis = k * p2.X + a;
				dis = (float)Math.Abs(dis - p2.Y);
				return dis <= lineDelta;
			}
		}
		#endregion

		#region ..绘制操作手柄
		void DrawSegControl(SVG.Paths.SVGPathSeg seg,System.Collections.ArrayList points,PointF start,bool preControl,bool hasNext,GraphicsPath path)
		{
			if(seg == null)
				return;
			short type = seg.PathSegType;
			PointF p1 = PointF.Empty;
			if(!hasNext && !preControl)
			{
				p1 = seg.GetRelativeNextControl(this.svgPathSegList);
			}
			else
			{
				switch(type)
				{
						//闭合
					case (short)SVG.PathSegmentType.PATHSEG_CLOSEPATH:
						break;

						//直线
					case (short)SVG.PathSegmentType.PATHSEG_LINETO_ABS:
					case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_ABS:
					case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_REL:
					case (short)SVG.PathSegmentType.PATHSEG_LINETO_REL:
					case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_ABS:
					case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_REL:

						break;
						//起始
					case (short)SVG.PathSegmentType.PATHSEG_MOVETO_ABS:
					case (short)SVG.PathSegmentType.PATHSEG_MOVETO_REL:
						if(preControl)
							p1 = seg.GetRelativePreControl(this.svgPathSegList);
						break;

						//贝赛尔连接
					case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_ABS:
					case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_REL:
					case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_ABS:
					case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_REL:
					case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_ABS:
					case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_REL:
					case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_ABS:
					case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_REL:
						if(!preControl)
							p1 = (seg as SVG.Paths.SVGPathSegCurve).GetFirstControl(this.svgPathSegList);
						else
							p1 = (seg as SVG.Paths.SVGPathSegCurve).GetSecondControl(this.svgPathSegList);
						break;

						//椭圆弧连接
					case (short)SVG.PathSegmentType.PATHSEG_ARC_ABS:
					case (short)SVG.PathSegmentType.PATHSEG_ARC_REL:
						SVG.Paths.SVGPathSegArc arc = seg as SVG.Paths.SVGPathSegArc;
						if(arc != null && preControl)
						{
							float angle = (float)(arc.Angle / 180f * Math.PI);
							float angle1 = angle + (float)Math.PI / 2f;
							float r1 = arc.R1;
							float r2 = arc.R2;
							p1 = arc.GetLastPoint(this.svgPathSegList);
							path.StartFigure();
							PointF arcP1,arcP2;
							this.CalculateArcControl(arc,out arcP1,out arcP2);
							path.AddLine(p1,arcP1);
							path.StartFigure();
							path.AddLine(p1,arcP2);
							points.Add(arcP1);
							points.Add(arcP2);
							p1 = PointF.Empty;
						}
						break;
				}
			}
			if(!p1.IsEmpty && !this.InPoint(start,p1,delta))
			{
				path.StartFigure();
				path.AddLine(p1,start);
				points.Add(p1);
			}
		}
		#endregion

		#region ..分割选中对象，以确定操作改变的对象
		void SplitSelectSeg(SVG.Paths.SVGPathSeg seg,out SVG.Interface.Paths.ISVGPathSeg pre,out SVG.Interface.Paths.ISVGPathSeg next)
		{
			pre = null;
			next = null;
			if(seg == null)
				return;
			
			short type = seg.PathSegType;
			SVG.Interface.Paths.ISVGPathSeg pre1;
			SVG.Interface.Paths.ISVGPathSeg next1;
			switch(type)
			{
					//闭合
				case (short)SVG.PathSegmentType.PATHSEG_CLOSEPATH:
					pre = seg;
					pre1 = this.svgPathSegList.PreviousSibling(seg);
					if(pre1 != null)
					{
						if(this.InPoint(pre1.GetLastPoint(this.svgPathSegList),seg.GetLastPoint(this.svgPathSegList),delta))
							pre = pre1;
					}

					next1 = this.svgPathSegList.GetRelativeStartPathSeg(seg);
					if(next1 != null)
					{
						next = this.svgPathSegList.NextSibling(next1);
//						if(this.svgPathSegList.NextSibling(next1) is SVGDom.Paths.SVGPathSegCurve)
//							next = next1;
					}
					break;

					//直线
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_REL:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_REL:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_REL:
					pre = seg;
					next = seg;
					next1 = this.svgPathSegList.NextSibling(seg);
					if(next1 != null && !(next1 is SVG.Paths.SVGPathSegMove))
					{
						bool inpoint = this.InPoint(next1.GetLastPoint(this.svgPathSegList),seg.GetLastPoint(this.svgPathSegList),delta);
						if(inpoint)
							inpoint = (next1 is SVG.Paths.SVGPathSegClosePath);
						if(!inpoint)
							next = next1;
					}
					break;
					//起始
				case (short)SVG.PathSegmentType.PATHSEG_MOVETO_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_MOVETO_REL:
					pre = seg;
					next = seg;
					next1 = this.svgPathSegList.NextSibling(seg);
					if(next1 != null && !(next1 is SVG.Paths.SVGPathSegMove))
						next = next1;
					pre1 = this.svgPathSegList.GetRelativeClosePathSeg(seg);
					if(pre1 != null)
					{
						next1 = this.svgPathSegList.PreviousSibling(pre1);
						if(this.InPoint(next1.GetLastPoint(this.svgPathSegList),pre1.GetLastPoint(this.svgPathSegList),delta))
							pre = next1;
					}
					break;

					//贝赛尔连接
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_REL:
					pre = seg;
					next = seg;
					next1 = this.svgPathSegList.NextSibling(seg);
					if(next1 != null && !(next1 is SVG.Paths.SVGPathSegMove))
					{
						bool inpoint = this.InPoint(next1.GetLastPoint(this.svgPathSegList),seg.GetLastPoint(this.svgPathSegList),delta);
						if(inpoint)
							inpoint = (next1 is SVG.Paths.SVGPathSegClosePath);
						if(!inpoint)
							next = next1;
					}
					break;

					//椭圆弧连接
				case (short)SVG.PathSegmentType.PATHSEG_ARC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_ARC_REL:
					pre = seg;
					next = this.svgPathSegList.NextSibling(seg);
					break;
			}
		}
		#endregion

		#region ..创建移动轨迹
		/// <summary>
		/// 创建移动轨迹
		/// </summary>
		/// <param name="endPoint"></param>
		/// <param name="rectList">记录矩形点</param>
		void CreateReversePath(PointF endPoint,ArrayList rectList)
		{
			switch(this.nodeOperator)
			{
					#region ..移动锚点
				case NodeOperator.MoveAnchor:
                    if (selectionAdd)
                        return;
					//记录改变开始锚点的路径单元
					System.Collections.ArrayList startSegs = new ArrayList();
					//记录改变结束锚点的路径单元
					System.Collections.ArrayList endSegs = new ArrayList();
					//记录无关联影响的路径单元
					System.Collections.ArrayList singleSegs = new ArrayList();
					for(int i = 0;i<this.currentSegs.Count;i++)
					{
						SVG.Paths.SVGPathSeg seg = this.currentSegs[i] as SVG.Paths.SVGPathSeg;
						if(seg != null)
						{
							//确定操作影响的两侧组成单元
							SVG.Interface.Paths.ISVGPathSeg pre,next;
							this.SplitSelectSeg(seg,out pre,out next);
							if(pre != next)
							{
								if(!startSegs.Contains(next))
									startSegs.Add(next);
								if(!endSegs.Contains(pre))
									endSegs.Add(pre);
							}
							else
							{
								if(!endSegs.Contains(pre))
									endSegs.Add(pre);
							}
						}
					}

					using(Matrix matrix = new Matrix())
					{
						PointF start = this.PointToView(this.startPoint);
						PointF end = this.PointToView(endPoint);
						matrix.Translate(end.X - start.X,end.Y - start.Y);
						//更新结尾锚点发生改变的路径单元
						for(int i = 0;i<endSegs.Count;i++)
						{
							SVG.Paths.SVGPathSeg seg = endSegs[i] as SVG.Paths.SVGPathSeg;
							if(seg == null)
								continue;
							//如果开始锚点也发生改变，则整个单元都发生改变
							if(startSegs.Contains(seg))
								this.CreateWholePath(seg,matrix,rectList);
							else
								this.CreateEndPath(seg,matrix,rectList);
						}

						//更新开始锚点改变的路径单元
						for(int i = 0;i<startSegs.Count;i++)
						{
							SVG.Paths.SVGPathSeg seg = startSegs[i] as SVG.Paths.SVGPathSeg;
							if(seg == null)
								continue;
							//如果结尾锚点也发生改变，则整个单元都发生改变
							if(endSegs.Contains(seg))
								this.CreateWholePath(seg,matrix,rectList);
							else
								this.CreateStartPath(seg,matrix,rectList);
						}

//						if(this.reversePath.PointCount > 1)
//							this.reversePath.Transform(this.totalTransform);
					}
					startSegs = null;
					endSegs = null;
					break;
					#endregion

					#region ..移动控制点
				case NodeOperator.MoveControl:
				case NodeOperator.ChangeEndAnchor:
					if(this.preSeg != null)
					{
						//椭圆弧
						if(this.preSeg is SVG.Paths.SVGPathSegArc)
						{
							SVG.Paths.SVGPathSegArc arc = this.preSeg as SVG.Paths.SVGPathSegArc;
							SVG.Interface.Paths.ISVGPathSeg pre = this.svgPathSegList.PreviousSibling(arc);
							if(pre == null)
								break;
							PointF start = pre.GetLastPoint(this.svgPathSegList);
							PointF first,second;
							this.CalculateArcControl(arc,out first,out second);
							endPoint = this.PointToView(endPoint);
							PointF anchor = arc.GetLastPoint(this.svgPathSegList);
							PointF endPoint1 = PointF.Empty;
							float angle = 0;
							float r2 = arc.R2;
							if(this.movePreControl)
							{
								float r1 = (float)Math.Sqrt(Math.Pow(endPoint.X - anchor.X,2) + Math.Pow(endPoint.Y - anchor.Y,2));
								angle = this.GetAngle(anchor,endPoint);
								this.CreateArcPath(arc,start,anchor,r1,arc.R2,angle,rectList);
								
							}
							else
							{
								float r1 = (float)Math.Sqrt(Math.Pow(endPoint.X - anchor.X,2) + Math.Pow(endPoint.Y - anchor.Y,2));
								angle = this.GetAngle(anchor,endPoint) - 90;
								this.CreateArcPath(arc,start,anchor,arc.R1,r1,angle,rectList);
								angle = angle - 90;
								r2 = arc.R1;
							}
							this.reversePath.StartFigure();
							this.reversePath.AddLine(anchor,endPoint);
							this.reversePath.StartFigure();
							angle = (float)((angle + 90) / 180 * Math.PI);
							endPoint1 = new PointF((float)(anchor.X + r2 * Math.Cos(angle)),(float)(anchor.Y + r2 *Math.Sin(angle)));
							this.reversePath.AddLine(anchor,endPoint1);
//							rectList.Add(endPoint);
							rectList.Add(anchor);
							rectList.Add(endPoint1);
						}
						
						else
						{
							SVG.Interface.Paths.ISVGPathSeg pre,next;
							this.SplitSelectSeg(this.preSeg,out pre,out next);
							endPoint = this.PointToView(endPoint);
							PointF nextP = PointF.Empty;

							#region ..处理第一个单元
//							if(pre is SVGDom.Paths.SVGPathSegCurve || pre is SVGDom.Paths.SVGPathSegMove)
//							{
								SVG.Interface.Paths.ISVGPathSeg pre1 = this.svgPathSegList.PreviousSibling(pre);
								#region ..起始,直线等
								
								//起始
								if(!(pre is SVG.Paths.SVGPathSegCurve) && (pre is SVG.Paths.SVGPathSegMove || pre1 != null))
								{
//									SVGDom.Paths.SVGPathSegMove move = pre as SVGDom.Paths.SVGPathSegMove;
									PointF linestart = pre.GetLastPoint(this.svgPathSegList);
									PointF preP = (pre as SVG.Paths.SVGPathSeg).GetRelativePreControl(this.svgPathSegList);
									//移动第一个单元
									if(this.movePreControl)
									{
										if(!preP.IsEmpty)
										{
											this.reversePath.StartFigure();
											this.reversePath.AddLine(endPoint,linestart);
											rectList.Add(linestart);
											rectList.Add(endPoint);
											nextP = (pre as SVG.Paths.SVGPathSeg).GetRelativeNextControl(this.svgPathSegList);
											if(!nextP.IsEmpty)
											{
												nextP = this.GetRelativePoint(linestart,preP,nextP,endPoint);
												this.reversePath.StartFigure();
												this.reversePath.AddLine(linestart,nextP);
												rectList.Add(nextP);
											}
											
										}
									}
										//移动第二个单元
									else
									{
										nextP = (pre as SVG.Paths.SVGPathSeg).GetRelativeNextControl(this.svgPathSegList);
										if(!nextP.IsEmpty)
										{
											preP = this.GetRelativePoint(linestart,nextP,preP,endPoint);
											if(!preP.IsEmpty)
											{
												this.reversePath.StartFigure();
												this.reversePath.AddLine(linestart,preP);
												rectList.Add(preP);
											}
											this.reversePath.StartFigure();
											this.reversePath.AddLine(linestart,endPoint);
											rectList.Add(linestart);
											
											rectList.Add(nextP);
											nextP = endPoint;
										}
									}
								}
								#endregion
									//贝赛尔
								else if(pre1 != null)
								{
									PointF start = pre1.GetLastPoint(this.svgPathSegList);
									

									#region ..贝赛尔
									SVG.Paths.SVGPathSegCurve curve = pre as SVG.Paths.SVGPathSegCurve;
									PointF anchor = curve.GetLastPoint(this.svgPathSegList);
									PointF first = curve.GetFirstControl(this.svgPathSegList);
									PointF second = curve.GetSecondControl(this.svgPathSegList);
									nextP = curve.GetRelativeNextControl(this.svgPathSegList);
									//移动上一个单元的第二个控制点
									if(this.movePreControl)
									{
										this.reversePath.StartFigure();
										this.reversePath.AddBezier(start,first,endPoint,anchor);
										this.reversePath.StartFigure();
										this.reversePath.AddLine(anchor,endPoint);
										rectList.Add(endPoint);
										rectList.Add(anchor);
										if(!nextP.IsEmpty)
										{
											nextP = this.GetRelativePoint(anchor,second,nextP,endPoint);
											this.reversePath.StartFigure();
											this.reversePath.AddLine(anchor,nextP);
											rectList.Add(nextP);
										}
									}
										//移动下一个单元的第一个控制点
									else
									{
										if(!nextP.IsEmpty)
										{
											second = this.GetRelativePoint(anchor,nextP,second,endPoint);
											this.reversePath.StartFigure();
											this.reversePath.AddBezier(start,first,second,anchor);
											this.reversePath.StartFigure();
											this.reversePath.AddLine(second,anchor);
											rectList.Add(second);
											rectList.Add(anchor);
											rectList.Add(endPoint);
											this.reversePath.StartFigure();
											this.reversePath.AddLine(anchor,endPoint);
											nextP = endPoint;
										}
									}	
									#endregion
								}
//							}
							#endregion

							#region ..处理第二个单元
							if(!nextP.IsEmpty && next is SVG.Paths.SVGPathSegCurve && next != pre)
							{
								SVG.Paths.SVGPathSegCurve curve = next as SVG.Paths.SVGPathSegCurve;
								pre = this.svgPathSegList.PreviousSibling(curve);
								if(pre == null)
									break;
								PointF start = pre.GetLastPoint(this.svgPathSegList);
								PointF first = curve.GetFirstControl(this.svgPathSegList);
								PointF second = curve.GetSecondControl(this.svgPathSegList);
								PointF anchor = curve.GetLastPoint(this.svgPathSegList);
								this.reversePath.StartFigure();
								this.reversePath.AddBezier(start,nextP,second,anchor);
							}
							#endregion
						}
						
					}
					break;
					#endregion

					#region ..封闭路径
				case NodeOperator.CloseFigure:
					if(this.preSeg is SVG.Paths.SVGPathSegMove)
					{
						SVG.Paths.SVGPathSegMove move = this.preSeg as SVG.Paths.SVGPathSegMove;
						SVG.Paths.SVGPathSeg unclose = FindUnCloseSegForStartSeg(move);
						if(unclose != null)
						{
							PointF start = unclose.GetLastPoint(this.svgPathSegList);	
							PointF first = unclose.GetRelativeNextControl(this.svgPathSegList);
							if(first.IsEmpty)
								first = start;
							this.reversePath.StartFigure();
							PointF end = move.GetLastPoint(this.svgPathSegList);
							endPoint = this.PointToView(endPoint);
							endPoint = new PointF(2 * end.X - endPoint.X ,2 * end.Y - endPoint.Y);
							this.reversePath.AddBezier(start,first,endPoint,end);
							rectList.Add(endPoint);
							rectList.Add(end);
							this.reversePath.StartFigure();
							this.reversePath.AddLine(end,endPoint);
							//如果存在拖动
							if(!this.InPoint(end,endPoint,delta) && (Control.ModifierKeys & Keys.Alt) != Keys.Alt)
							{
								SVG.Paths.SVGPathSeg next = this.svgPathSegList.NextSibling(move) as SVG.Paths.SVGPathSeg;
								if(next is SVG.Paths.SVGPathSegCurve)
								{
									SVG.Paths.SVGPathSegCurve curve = next as SVG.Paths.SVGPathSegCurve;
									first = curve.GetFirstControl(this.svgPathSegList);
									PointF second = curve.GetSecondControl(this.svgPathSegList);
									PointF anchor = curve.GetLastPoint(this.svgPathSegList);
									PointF rnext = move.GetRelativePreControl(this.svgPathSegList);
									first = this.GetRelativePoint(end,rnext,first,endPoint);
									this.reversePath.StartFigure();
									this.reversePath.AddBezier(end,first,second,anchor);
									this.reversePath.StartFigure();
									this.reversePath.AddLine(end,first);
									rectList.Add(first);
								}
							}
						}
					}
					break;
					#endregion

					#region ..转换锚点
				case NodeOperator.ConvertAnchor:
					if(this.preSeg != null)
					{
						SVG.Interface.Paths.ISVGPathSeg pre,next;
						this.SplitSelectSeg(this.preSeg,out pre,out next);
						endPoint = this.PointToView(endPoint);
						
//						PointF nextA
						//转换第一个单元
						if(pre is SVG.Paths.SVGPathSegCurve || pre is SVG.Paths.SVGPathSegLine)
						{
							SVG.Paths.SVGPathSeg pre1 = this.svgPathSegList.PreviousSibling(pre) as SVG.Paths.SVGPathSeg;
							if(pre1 == null)
								break;
							PointF start = pre1.GetLastPoint(this.svgPathSegList);
							PointF anchor = pre.GetLastPoint(this.svgPathSegList);
							PointF first = start;
							if(pre is SVG.Paths.SVGPathSegCurve)
								first = (pre as SVG.Paths.SVGPathSegCurve).GetFirstControl(this.svgPathSegList);
							PointF second = new PointF(2 * anchor.X - endPoint.X,2 * anchor.Y - endPoint.Y);
							this.reversePath.StartFigure();
							this.reversePath.AddBezier(start,first,second,anchor);
							this.reversePath.StartFigure();
							this.reversePath.AddLine(anchor,second);
							rectList.Add(anchor);
							rectList.Add(second);
						}

						//转换第二个单元
						if(next != pre && (next is SVG.Paths.SVGPathSegCurve || next is SVG.Paths.SVGPathSegLine))
						{
							SVG.Paths.SVGPathSeg pre1 = this.svgPathSegList.PreviousSibling(next) as SVG.Paths.SVGPathSeg;
							if(pre1 == null)
								break;
							PointF start = pre1.GetLastPoint(this.svgPathSegList);
							PointF anchor = next.GetLastPoint(this.svgPathSegList);
							PointF first = endPoint;
							PointF second = anchor;
							if(next is SVG.Paths.SVGPathSegCurve)
								second = (next as SVG.Paths.SVGPathSegCurve).GetSecondControl(this.svgPathSegList);
//							PointF second = new PointF(2 * anchor.X - endPoint.X,2 * anchor.Y - endPoint.Y);
							this.reversePath.StartFigure();
							this.reversePath.AddBezier(start,first,second,anchor);
							this.reversePath.StartFigure();
							this.reversePath.AddLine(start,first);
							rectList.Add(first);
//							rectList.Add(second);
						}
					}
					break;
					#endregion
			}
		}
		#endregion

		#region ..创建整体移动的单元路径
		/// <summary>
		/// 创建整体移动的单元路径
		/// </summary>
		/// <param name="seg"></param>
		/// <param name="matrix">标志改变的变换矩阵</param>
		void CreateWholePath(SVG.Paths.SVGPathSeg seg,Matrix matrix,System.Collections.ArrayList ctlPoints)
		{
			if(seg == null)
				return ;
			short type = (short)seg.PathSegType;
			PointF start = PointF.Empty;
			SVG.Interface.Paths.ISVGPathSeg pre = this.svgPathSegList.PreviousSibling(seg);
			if(pre != null)
			{
				//确定开始端点
				start = pre.GetLastPoint(this.svgPathSegList);
			}
			bool dealend = false;
			switch(type)
			{
					//闭合
				case (short)SVG.PathSegmentType.PATHSEG_CLOSEPATH:
					if(!start.IsEmpty && !this.InPoint(seg.GetLastPoint(this.svgPathSegList),start,delta))
					{
						PointF p1 = seg.GetLastPoint(this.svgPathSegList);
						this.reversePath.StartFigure();
						if(this.currentSegs.Contains(pre))
							this.reversePath.AddLine(p1.X + matrix.OffsetX,p1.Y + matrix.OffsetY,start.X + matrix.OffsetX,start.Y + matrix.OffsetY);
						else
							this.reversePath.AddLine(p1.X + matrix.OffsetX,p1.Y + matrix.OffsetY,start.X,start.Y);
						ctlPoints.Add(new PointF(p1.X + matrix.OffsetX,p1.Y + matrix.OffsetY));
					}
					break;

					//直线
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_REL:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_REL:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_REL:
					SVG.Paths.SVGPathSegLine line = seg as SVG.Paths.SVGPathSegLine;
					if(line != null && !start.IsEmpty)
					{
						PointF[] ps = new PointF[]{start,line.GetLastPoint(this.svgPathSegList)};
						matrix.TransformPoints(ps);
						this.reversePath.StartFigure();
						this.reversePath.AddLine(ps[0],ps[1]);
						ctlPoints.Add(ps[0]);
						ctlPoints.Add(ps[1]);
						dealend = true;
					}
					break;

					//起始
				case (short)SVG.PathSegmentType.PATHSEG_MOVETO_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_MOVETO_REL:
					
					break;

					//贝赛尔连接
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_REL:
					SVG.Paths.SVGPathSegCurve curve = seg as SVG.Paths.SVGPathSegCurve;
					if(curve != null && !start.IsEmpty)
					{
						PointF p1 = curve.GetFirstControl(this.svgPathSegList);
						PointF p2 = curve.GetSecondControl(this.svgPathSegList);
						PointF p3 = curve.GetLastPoint(this.svgPathSegList);
						PointF[] ps = new PointF[]{start,p1,p2,p3};
						matrix.TransformPoints(ps);
						this.reversePath.StartFigure();
						this.reversePath.AddBezier(ps[0],ps[1],ps[2],ps[3]);
						this.reversePath.StartFigure();
						this.reversePath.AddLine(ps[0],ps[1]);
//						this.reversePath.AddRectangle(new RectangleF(ps[1].X - 1,ps[1].Y - 1,2,2));
						this.reversePath.StartFigure();
						this.reversePath.AddLine(ps[2],ps[3]);
						ctlPoints.Add(ps[1]);
						ctlPoints.Add(ps[2]);
						ctlPoints.Add(ps[3]);
						dealend = true;
//						this.reversePath.AddRectangle(new RectangleF(ps[2].X - 1,ps[2].Y - 1,2,2));
						ps = null;
					}
					break;

					//椭圆弧连接
				case (short)SVG.PathSegmentType.PATHSEG_ARC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_ARC_REL:
					SVG.Paths.SVGPathSegArc arc = seg as SVG.Paths.SVGPathSegArc;
					if(arc != null)
					{
						PointF p1 = arc.GetLastPoint(this.svgPathSegList);
						p1 = new PointF(p1.X + matrix.OffsetX,p1.Y + matrix.OffsetY);
						ctlPoints.Add(p1);
						CreateArcPath(arc,new PointF(start.X + matrix.OffsetX,start.Y + matrix.OffsetY),p1,ctlPoints);					
						PointF first,second;
						this.CalculateArcControl(arc,out first,out second,p1);
						if(!first.IsEmpty)
						{
							this.reversePath.StartFigure();
							this.reversePath.AddLine(p1,first);
							ctlPoints.Add(first);
						}
						if(!second.IsEmpty)
						{
							this.reversePath.StartFigure();
							this.reversePath.AddLine(p1,second);
							ctlPoints.Add(second);
						}
					}
					break;
			}
			if(dealend)
			{
				PointF endP = seg.GetRelativeNextControl(this.svgPathSegList);
				if(!endP.IsEmpty)
				{
					PointF startP = seg.GetLastPoint(this.svgPathSegList);
					ctlPoints.Add(new PointF(endP.X + matrix.OffsetX,endP.Y + matrix.OffsetY));
					this.reversePath.StartFigure();
					this.reversePath.AddLine(endP.X + matrix.OffsetX,endP.Y + matrix.OffsetY,startP.X + matrix.OffsetX,startP.Y +matrix.OffsetY);
				}
			}
		}
		#endregion

		#region ..创建移动结尾锚点的单元路径
		/// <summary>
		/// 创建移动结尾锚点的单元路径
		/// </summary>
		/// <param name="seg">单元路径</param>
		/// <param name="matrix">标志改变的变换矩阵</param>
		void CreateEndPath(SVG.Paths.SVGPathSeg seg,Matrix matrix,ArrayList ctrlPoints)
		{
			if(seg == null)
				return ;
			short type = (short)seg.PathSegType;
			PointF start = PointF.Empty;
			SVG.Interface.Paths.ISVGPathSeg pre = this.svgPathSegList.PreviousSibling(seg);
			if(pre != null)
			{
				//确定开始端点
				start = pre.GetLastPoint(this.svgPathSegList);
			}
			//注意处理单元处于路径最末的情况，这是注意加上后续控制手柄
			PointF endPoint = PointF.Empty;
			bool endWith = false;
			SVG.Interface.Paths.ISVGPathSeg next = this.svgPathSegList.NextSibling(seg);
			if(next == null || next is SVG.Paths.SVGPathSegMove)
			{
				endWith = true;
				endPoint = seg.GetRelativeNextControl(this.svgPathSegList);
			}
			switch(type)
			{
					//闭合
				case (short)SVG.PathSegmentType.PATHSEG_CLOSEPATH:
					endWith = false;
					SVG.Paths.SVGPathSegClosePath close = seg as SVG.Paths.SVGPathSegClosePath;
					if(close != null && !start.IsEmpty)
					{
						PointF p1 = close.GetLastPoint(this.svgPathSegList);
						if(!this.InPoint(p1,start,delta))
						{
							this.reversePath.StartFigure();
							p1.X += matrix.OffsetX;
							p1.Y += matrix.OffsetY;
							this.reversePath.AddLine(p1,start);
							ctrlPoints.Add(p1);
						}
					}
					break;

					//直线
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_REL:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_REL:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_REL:
					this.reversePath.StartFigure();
					SVG.Paths.SVGPathSegLine line = seg as SVG.Paths.SVGPathSegLine;
					if(line != null&& !start.IsEmpty)
					{
						PointF p2 = line.GetLastPoint(this.svgPathSegList);
//						if(this.InPoint(p2,start))
//						{
//							start.X += matrix.OffsetX;
//							start.Y += matrix.OffsetY;
//						}
						p2.X += matrix.OffsetX;
						p2.Y += matrix.OffsetY;
						
						this.reversePath.StartFigure();
						this.reversePath.AddLine(start,p2);
						this.reversePath.StartFigure();
						ctrlPoints.Add(p2);
						ctrlPoints.Add(start);
//						this.reversePath.AddRectangle(new RectangleF(p2.X - 1,p2.Y - 1,2,2));
					}
					break;
					//起始
				case (short)SVG.PathSegmentType.PATHSEG_MOVETO_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_MOVETO_REL:
					SVG.Paths.SVGPathSegMove move = seg as SVG.Paths.SVGPathSegMove;
					if(move != null)
					{
						PointF p1 = move.GetRelativePreControl(this.svgPathSegList);
						if(p1.IsEmpty)
							break;
						PointF p2 = move.GetLastPoint(this.svgPathSegList);
						this.reversePath.StartFigure();
						p1.X += matrix.OffsetX;
						p2.X += matrix.OffsetX;
						p1.Y += matrix.OffsetY;
						p2.Y += matrix.OffsetY;
						bool inpoint = this.InPoint(p1,p2,delta);
						if(!inpoint)
						{
							this.reversePath.AddLine(p1,p2);
							ctrlPoints.Add(p1);
						}
						ctrlPoints.Add(p2);

					}
					break;

					//贝赛尔连接
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_REL:
					SVG.Paths.SVGPathSegCurve curve = seg as SVG.Paths.SVGPathSegCurve;
					if(curve != null && !start.IsEmpty)
					{
						PointF p1 = curve.GetFirstControl(this.svgPathSegList);
						PointF p2 = curve.GetSecondControl(this.svgPathSegList);
						PointF p3 = curve.GetLastPoint(this.svgPathSegList);
						//第二控制点和锚点发生改变
						PointF[] ps = new PointF[]{p2,p3};
						matrix.TransformPoints(ps);
						this.reversePath.StartFigure();
						this.reversePath.AddBezier(start,p1,ps[0],ps[1]);
						this.reversePath.StartFigure();
						this.reversePath.AddLine(ps[0],ps[1]);
						ctrlPoints.Add(ps[0]);
						ctrlPoints.Add(ps[1]);
//						ctrlPoints.Add(ps[2]);
						ps = null;
					}
					break;

					//椭圆弧连接
				case (short)SVG.PathSegmentType.PATHSEG_ARC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_ARC_REL:
					SVG.Paths.SVGPathSegArc arc = seg as SVG.Paths.SVGPathSegArc;
					if(arc != null && !start.IsEmpty)
					{
						PointF p = arc.GetLastPoint(this.svgPathSegList);
						p = new PointF(p.X + matrix.OffsetX,p.Y + matrix.OffsetY);
						CreateArcPath(arc,start,p,ctrlPoints);
						ctrlPoints.Add(p);
						PointF first,second;
						this.CalculateArcControl(arc,out first,out second,p);
						if(!first.IsEmpty)
						{
							this.reversePath.StartFigure();
							this.reversePath.AddLine(p,first);
							ctrlPoints.Add(first);
						}
						if(!second.IsEmpty)
						{
							this.reversePath.StartFigure();
							this.reversePath.AddLine(p,second);
							ctrlPoints.Add(second);
						}
					}
					break;
			}

			if(endWith && !endPoint.IsEmpty)
			{
				PointF end = seg.GetLastPoint(this.svgPathSegList);
				if(!this.InPoint(end,endPoint,delta))
				{
					PointF[] ps1 = new PointF[]{end,endPoint};
					matrix.TransformPoints(ps1);
					this.reversePath.StartFigure();
					this.reversePath.AddLine(ps1[0],ps1[1]);
					ctrlPoints.Add(ps1[1]);
//					this.reversePath.AddRectangle(new RectangleF(ps1[1].X - 1,ps1[1].Y - 1,2,2));
					ps1= null;
				}
			}
		}
		#endregion

		#region ..创建移动开始锚点的单元路径
		/// <summary>
		/// 创建移动开始锚点的单元路径
		/// </summary>
		/// <param name="seg">单元路径</param>
		/// <param name="matrix">标志改变的变换矩阵</param>
		void CreateStartPath(SVG.Paths.SVGPathSeg seg,Matrix matrix,ArrayList ctrlPoints)
		{
			if(seg == null)
				return ;
			short type = (short)seg.PathSegType;
			SVG.Interface.Paths.ISVGPathSeg pre = this.svgPathSegList.PreviousSibling(seg);
			if(pre == null)
				return;
			//确定其实锚点
			PointF start = pre.GetLastPoint(this.svgPathSegList);
			switch(type)
			{
					//闭合
				case (short)SVG.PathSegmentType.PATHSEG_CLOSEPATH:
					SVG.Paths.SVGPathSegClosePath close = seg as SVG.Paths.SVGPathSegClosePath;
					if(close != null)
					{
						PointF p1 = close.GetLastPoint(this.svgPathSegList);
						if(!this.InPoint(p1,start,delta))
						{
							start.X += matrix.OffsetX;
							start.Y += matrix.OffsetY;
							this.reversePath.StartFigure();
							this.reversePath.AddLine(start,p1);
							
						}
					}
					break;

					//直线
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_REL:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_REL:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_REL:
					SVG.Paths.SVGPathSegLine line = seg as SVG.Paths.SVGPathSegLine;
					if(line != null)
					{
						PointF p1 = line.GetLastPoint(this.svgPathSegList);
						if(!this.InPoint(p1,start,delta))
						{
							start.X += matrix.OffsetX;
							start.Y += matrix.OffsetY;
							this.reversePath.StartFigure();
							this.reversePath.AddLine(start,p1);
						}
					}

					break;
					//起始
				case (short)SVG.PathSegmentType.PATHSEG_MOVETO_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_MOVETO_REL:

					break;

					//贝赛尔连接
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_REL:
					SVG.Paths.SVGPathSegCurve curve = seg as SVG.Paths.SVGPathSegCurve;
					if(curve != null)
					{
						PointF p1 = curve.GetFirstControl(this.svgPathSegList);
						PointF p2 = curve.GetSecondControl(this.svgPathSegList);
						PointF p3 = curve.GetLastPoint(this.svgPathSegList);
						//前一个锚点和第一控制点发生改变
						PointF[] ps = new PointF[]{start,p1};
						matrix.TransformPoints(ps);
						this.reversePath.StartFigure();
						this.reversePath.AddBezier(ps[0],ps[1],p2,p3);
						this.reversePath.StartFigure();
						this.reversePath.AddLine(ps[0],ps[1]);
//						this.reversePath.AddRectangle(new RectangleF(ps[1].X - 1,ps[1].Y - 1,2,2));
						ctrlPoints.Add(ps[1]);
						ps = null;
					}
					break;

					//椭圆弧连接
				case (short)SVG.PathSegmentType.PATHSEG_ARC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_ARC_REL:
					SVG.Paths.SVGPathSegArc arc = seg as SVG.Paths.SVGPathSegArc;
					if(arc != null)
					{
						PointF anchor = arc.GetLastPoint(this.svgPathSegList);
						CreateArcPath(arc,new PointF(start.X + matrix.OffsetX,start.Y + matrix.OffsetY),anchor,ctrlPoints);
						PointF first,second;
						this.CalculateArcControl(arc,out first,out second,anchor);
					}
					break;
			}
		}
		#endregion

		#region ..更新路径数据
		/// <summary>
		/// 编辑之后，更新路径单元列表信息，并更新路径数据
		/// </summary>
		/// <param name="endPoint"></param>
		void UpdateSegList(PointF endPoint)
		{
			//判断两点是否重合
			if(this.InPoint(this.startPoint,endPoint,1.414f) && this.nodeOperator != NodeOperator.CloseFigure)
				return;
			switch(this.nodeOperator)
			{
					#region ..移动锚点
				case NodeOperator.MoveAnchor:
					//记录改变开始锚点的路径单元
					System.Collections.ArrayList startSegs = new ArrayList();
					//记录改变结束锚点的路径单元
					System.Collections.ArrayList endSegs = new ArrayList();
					//记录无关联影响的路径单元
					System.Collections.ArrayList singleSegs = new ArrayList();
					for(int i = 0;i<this.currentSegs.Count;i++)
					{
						SVG.Paths.SVGPathSeg seg = this.currentSegs[i] as SVG.Paths.SVGPathSeg;
						if(seg != null)
						{
							//确定操作影响的两侧组成单元
							SVG.Interface.Paths.ISVGPathSeg pre,next;
							this.SplitSelectSeg(seg,out pre,out next);
							if(pre != next)
							{
								if(!startSegs.Contains(next))
									startSegs.Add(next);
								if(!endSegs.Contains(pre))
									endSegs.Add(pre);
							}
							else
							{
								if(!endSegs.Contains(pre))
									endSegs.Add(pre);
							}
						}
					}

					using(Matrix matrix = new Matrix())
					{
						PointF start = this.PointToView(this.startPoint);
						PointF end = this.PointToView(endPoint);
						matrix.Translate(end.X - start.X,end.Y - start.Y);
						ArrayList dealedlist = new ArrayList();
						//更新结尾锚点发生改变的路径单元
						for(int i = 0;i<endSegs.Count;i++)
						{
							SVG.Paths.SVGPathSeg seg = endSegs[i] as SVG.Paths.SVGPathSeg;
							if(seg == null || dealedlist.Contains(seg))
								continue;
							//如果开始锚点也发生改变，则整个单元都发生改变
							if(startSegs.Contains(seg))
								this.UpdateWholeSeg(seg,matrix,dealedlist);
							else
								this.UpdateEndAnchor(seg,matrix,dealedlist);
							dealedlist.Add(seg);
						}

						//更新开始锚点改变的路径单元
						for(int i = 0;i<startSegs.Count;i++)
						{
							SVG.Paths.SVGPathSeg seg = startSegs[i] as SVG.Paths.SVGPathSeg;
							if(seg == null || dealedlist.Contains(seg))
								continue;
							//如果结尾锚点也发生改变，则整个单元都发生改变
							if(endSegs.Contains(seg))
								this.UpdateWholeSeg(seg,matrix,dealedlist);
							else
								this.UpdateStartAnchor(seg,matrix,dealedlist);
							dealedlist.Add(seg);
						}
						dealedlist = null;

						//						if(this.reversePath.PointCount > 1)
						//							this.reversePath.Transform(this.totalTransform);
					}
					startSegs = null;
					endSegs = null;
					break;
					#endregion

					#region ..移动控制点
				case NodeOperator.MoveControl:
				case NodeOperator.ChangeEndAnchor:
					if(this.preSeg != null)
					{
						//椭圆弧
						#region ..椭圆弧
						if(this.preSeg is SVG.Paths.SVGPathSegArc)
						{
							SVG.Paths.SVGPathSegArc arc = this.preSeg as SVG.Paths.SVGPathSegArc;
							SVG.Interface.Paths.ISVGPathSeg pre = this.svgPathSegList.PreviousSibling(arc);
							if(pre == null)
								break;
							endPoint = this.PointToView(endPoint);
							PointF anchor = arc.GetLastPoint(this.svgPathSegList);
							float angle = 0;
							if(this.movePreControl)
							{
								float r1 = (float)Math.Sqrt(Math.Pow(endPoint.X - anchor.X,2) + Math.Pow(endPoint.Y - anchor.Y,2));
								angle = this.GetAngle(anchor,endPoint);
								arc.R1 = r1;
							}
							else
							{
								float r1 = (float)Math.Sqrt(Math.Pow(endPoint.X - anchor.X,2) + Math.Pow(endPoint.Y - anchor.Y,2));
								angle = this.GetAngle(anchor,endPoint) - 90;
								arc.R2 = r1;
							}
							arc.Angle = angle;
						}
							#endregion
						else
						{
							SVG.Interface.Paths.ISVGPathSeg pre,next;
							this.SplitSelectSeg(this.preSeg,out pre,out next);
							endPoint = this.PointToView(endPoint);
							PointF nextP = PointF.Empty;

							#region ..处理第一个单元
							
							SVG.Interface.Paths.ISVGPathSeg pre1 = this.svgPathSegList.PreviousSibling(pre);
							#region ..起始
							//起始
								
							if(!(pre is SVG.Paths.SVGPathSegCurve) && (pre is SVG.Paths.SVGPathSegMove || pre1 != null))
							{
//								SVGDom.Paths.SVGPathSegMove move = pre as SVGDom.Paths.SVGPathSegMove;
								PointF linestart = pre.GetLastPoint(this.svgPathSegList);
								PointF preP = (pre as SVG.Paths.SVGPathSeg).GetRelativePreControl(this.svgPathSegList);
								//移动第一个单元
								if(this.movePreControl)
								{
									if(!preP.IsEmpty)
									{
										//SegMove
										if(pre is SVG.Paths.SVGPathSegMove)
											(pre as SVG.Paths.SVGPathSegMove).SetRelativePreControl(endPoint);
										nextP = (pre as SVG.Paths.SVGPathSeg).GetRelativeNextControl(this.svgPathSegList);
										if(!nextP.IsEmpty)
										{
											nextP = this.GetRelativePoint(linestart,preP,nextP,endPoint);
											(pre as SVG.Paths.SVGPathSeg).SetRelativeNextControl(nextP);
										}
									}
								}
									//移动第二个单元
								else 
								{
									
									nextP = (pre as SVG.Paths.SVGPathSeg).GetRelativeNextControl(this.svgPathSegList);
									if(!nextP.IsEmpty )
									{
										(pre as SVG.Paths.SVGPathSeg).SetRelativeNextControl(endPoint);
										preP = this.GetRelativePoint(linestart,nextP,preP,endPoint);
										if(pre is SVG.Paths.SVGPathSegMove)
											(pre as SVG.Paths.SVGPathSegMove).SetRelativePreControl(preP);
										nextP = endPoint;
									}
								}
							}
								#endregion
								//贝赛尔
							else if(pre1 != null)
							{
								PointF start = pre1.GetLastPoint(this.svgPathSegList);

								#region ..贝赛尔
								SVG.Paths.SVGPathSegCurve curve = pre as SVG.Paths.SVGPathSegCurve;
								PointF anchor = curve.GetLastPoint(this.svgPathSegList);
								PointF first = curve.GetFirstControl(this.svgPathSegList);
								PointF second = curve.GetSecondControl(this.svgPathSegList);
								nextP = curve.GetRelativeNextControl(this.svgPathSegList);
								//移动上一个单元的第二个控制点
								if(this.movePreControl)
								{
									curve.X2 = endPoint.X;
									curve.Y2 = endPoint.Y;
									if(!nextP.IsEmpty)
									{
										nextP = this.GetRelativePoint(anchor,second,nextP,endPoint);
										curve.SetRelativeNextControl(nextP);
									}
								}
									//移动下一个单元的第一个控制点
								else
								{
									if(!nextP.IsEmpty)
									{
										second = this.GetRelativePoint(anchor,nextP,second,endPoint);
										curve.X2 = second.X;
										curve.Y2 = second.Y;
										curve.SetRelativeNextControl(endPoint);
										nextP = endPoint;
									}
								}	
								#endregion
							}
							#endregion

							#region ..处理第二个单元
							if(!nextP.IsEmpty && next is SVG.Paths.SVGPathSegCurve && next != pre)
							{
								SVG.Paths.SVGPathSegCurve curve = next as SVG.Paths.SVGPathSegCurve;
								curve.X1 = nextP.X;
								curve.Y1 = nextP.Y;
							}
							#endregion
						}
					}
					break;
					#endregion

					#region ..封闭路径
				case NodeOperator.CloseFigure:
					if(this.preSeg is SVG.Paths.SVGPathSegMove)
					{
						SVG.Paths.SVGPathSegMove move = this.preSeg as SVG.Paths.SVGPathSegMove;
						SVG.Paths.SVGPathSeg unclose = FindUnCloseSegForStartSeg(move);
						if(unclose != null)
						{
							PointF start = unclose.GetLastPoint(this.svgPathSegList);	
							PointF first = unclose.GetRelativeNextControl(this.svgPathSegList);
							if(first.IsEmpty)
								first = start;
							PointF end = move.GetLastPoint(this.svgPathSegList);
							PointF rnext = move.GetRelativePreControl(this.svgPathSegList);
							endPoint = this.PointToView(endPoint);
							endPoint = new PointF(2 * end.X - endPoint.X ,2 * end.Y - endPoint.Y);
							int index = this.svgPathSegList.IndexOf(unclose);
							SVG.Paths.SVGPathSeg seg = this.CreateSeg(start,first,endPoint,end);
							this.svgPathSegList.InsertItemBefore(seg,index + 1);
							SVG.Paths.SVGPathSegClosePath close = new SVG.Paths.SVGPathSegClosePath();
							this.svgPathSegList.InsertItemBefore(close,index+2);
						
							//如果存在拖动
							if(!this.InPoint(end,endPoint,delta) && (Control.ModifierKeys & Keys.Alt) != Keys.Alt)
							{
								SVG.Paths.SVGPathSeg next = this.svgPathSegList.NextSibling(move) as SVG.Paths.SVGPathSeg;
								if(next is SVG.Paths.SVGPathSegCurve)
								{
									SVG.Paths.SVGPathSegCurve curve = next as SVG.Paths.SVGPathSegCurve;
									first = curve.GetFirstControl(this.svgPathSegList);
									
									first = this.GetRelativePoint(end,rnext,first,endPoint);
									move.SetRelativeNextControl(first);
									curve.X1 = first.X;
									curve.Y1 = first.Y;
								}
							}
							this.preSeg = close;
							move.SetRelativePreControl(endPoint);
						}
					}
					break;
					#endregion

					#region ..转换锚点
				case NodeOperator.ConvertAnchor:
					if(this.preSeg != null)
					{
						SVG.Interface.Paths.ISVGPathSeg pre,next;
						this.SplitSelectSeg(this.preSeg,out pre,out next);
						endPoint = this.PointToView(endPoint);
						
						//转换第一个单元
						if(pre is SVG.Paths.SVGPathSegCurve || pre is SVG.Paths.SVGPathSegLine)
						{
							SVG.Paths.SVGPathSeg pre1 = this.svgPathSegList.PreviousSibling(pre) as SVG.Paths.SVGPathSeg;
							if(pre1 == null)
								break;
							PointF start = pre1.GetLastPoint(this.svgPathSegList);
							PointF anchor = pre.GetLastPoint(this.svgPathSegList);
							PointF first = start;
							if(pre is SVG.Paths.SVGPathSegCurve)
							{
								SVG.Paths.SVGPathSegCurve curve = pre as SVG.Paths.SVGPathSegCurve;
								curve.X2 = 2 * anchor.X - endPoint.X;
								curve.Y2 = 2 * anchor.Y - endPoint.Y;
							}
							else
							{
								PointF second = new PointF(2 * anchor.X - endPoint.X,2 * anchor.Y - endPoint.Y);
								SVG.Paths.SVGPathSeg seg = this.CreateSeg(start,first,second,anchor);
								int index = this.svgPathSegList.IndexOf(pre);
								this.svgPathSegList.ReplaceItem(seg,index);
								this.preSeg = seg;
							}
						}

						//转换第二个单元
						if(next != pre && (next is SVG.Paths.SVGPathSegCurve || next is SVG.Paths.SVGPathSegLine))
						{
							SVG.Paths.SVGPathSeg pre1 = this.svgPathSegList.PreviousSibling(next) as SVG.Paths.SVGPathSeg;
							if(pre1 == null)
								break;
							PointF start = pre1.GetLastPoint(this.svgPathSegList);
							PointF anchor = next.GetLastPoint(this.svgPathSegList);
							PointF first = endPoint;
							PointF second = anchor;
							if(next is SVG.Paths.SVGPathSegCurve)
							{
								SVG.Paths.SVGPathSegCurve curve = next as SVG.Paths.SVGPathSegCurve;
								curve.X1 = endPoint.X;
								curve.Y1 = endPoint.Y;
							}
							else
							{
								
								SVG.Paths.SVGPathSeg seg = this.CreateSeg(start,first,second,anchor);
								int index = this.svgPathSegList.IndexOf(next);
								this.svgPathSegList.ReplaceItem(seg,index);
							}
						}
					}
					break;
					#endregion
			}
		}
		#endregion

		#region ..更新移动结尾锚点的路径单元数据
		/// <summary>
		/// 更新移动结尾锚点的路径单元数据
		/// </summary>
		/// <param name="seg">路径单元</param>
		/// <param name="matrix">变换矩阵</param>
		void UpdateEndAnchor(SVG.Paths.SVGPathSeg seg,Matrix matrix,System.Collections.ArrayList dealed)
		{
			if(seg == null)
				return ;
			short type = (short)seg.PathSegType;
			PointF start = PointF.Empty;
			SVG.Interface.Paths.ISVGPathSeg pre = this.svgPathSegList.PreviousSibling(seg);
			if(pre != null)
			{
				//确定开始端点
				start = pre.GetLastPoint(this.svgPathSegList);
			}
			//注意处理单元处于路径最末的情况，这是注意加上后续控制手柄
			PointF endPoint = PointF.Empty;
			bool endWith = false;
			SVG.Interface.Paths.ISVGPathSeg next = this.svgPathSegList.NextSibling(seg);
			if(next == null || next is SVG.Paths.SVGPathSegMove)
			{
				endWith = true;
				endPoint = seg.GetRelativeNextControl(this.svgPathSegList);
			}
			switch(type)
			{
					//闭合
				case (short)SVG.PathSegmentType.PATHSEG_CLOSEPATH:
					break;

					//直线
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_REL:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_REL:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_REL:
					SVG.Paths.SVGPathSegLine line = seg as SVG.Paths.SVGPathSegLine;
					if(line != null&& !start.IsEmpty)
					{
						PointF p2 = line.GetLastPoint(this.svgPathSegList);
						p2.X += matrix.OffsetX;
						p2.Y += matrix.OffsetY;
						line.X = p2.X;
						line.Y = p2.Y;
					}
					break;
					//起始
				case (short)SVG.PathSegmentType.PATHSEG_MOVETO_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_MOVETO_REL:
					SVG.Paths.SVGPathSegMove move = seg as SVG.Paths.SVGPathSegMove;
					if(move != null)
					{
						PointF p1 = move.GetRelativePreControl(this.svgPathSegList);
						PointF p2 = move.GetLastPoint(this.svgPathSegList);

						move.X = p2.X + matrix.OffsetX;
						move.Y = p2.Y + matrix.OffsetY;
						if(!p1.IsEmpty)
							move.SetRelativePreControl(new PointF(p1.X + matrix.OffsetX ,p1.Y + matrix.OffsetY));
					}
					break;

					//贝赛尔连接
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_REL:
					SVG.Paths.SVGPathSegCurve curve = seg as SVG.Paths.SVGPathSegCurve;
					if(curve != null && !start.IsEmpty)
					{
						PointF p1 = curve.GetFirstControl(this.svgPathSegList);
						PointF p2 = curve.GetSecondControl(this.svgPathSegList);
						PointF p3 = curve.GetLastPoint(this.svgPathSegList);
						//第二控制点和锚点发生改变
						PointF[] ps = new PointF[]{p2,p3};
						matrix.TransformPoints(ps);
						curve.X2 = ps[0].X;
						curve.Y2 = ps[0].Y;
						curve.X = ps[1].X;
						curve.Y = ps[1].Y;
						ps = null;
						//注意处理封闭情形
						if(next is SVG.Paths.SVGPathSegClosePath)
						{
							SVG.Paths.SVGPathSegMove m = this.svgPathSegList.GetRelativeStartPathSeg(seg);
							if(m != null && this.InPoint(m.GetLastPoint(this.svgPathSegList),p3,delta))
								m.SetRelativePreControl(p2);
						}
					}
					break;

					//椭圆弧连接
				case (short)SVG.PathSegmentType.PATHSEG_ARC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_ARC_REL:
					SVG.Paths.SVGPathSegArc arc = seg as SVG.Paths.SVGPathSegArc;
					if(arc != null)
					{
						arc.X += matrix.OffsetX;
						arc.Y += matrix.OffsetY;
					}
					break;
			}

			if(endWith && !endPoint.IsEmpty)
			{
				PointF end = seg.GetLastPoint(this.svgPathSegList);
				if(!this.InPoint(end,endPoint,delta))
				{
					PointF[] ps1 = new PointF[]{end,endPoint};
					matrix.TransformPoints(ps1);
					if(!endPoint.IsEmpty)
						seg.SetRelativeNextControl(ps1[1]);
					ps1= null;
				}
			}
		}
		#endregion

		#region ..更新移动开始锚点的路径单元数据
		/// <summary>
		/// 更新移动开始锚点的路径单元数据
		/// </summary>
		/// <param name="seg">路径单元</param>
		/// <param name="matrix">变换矩阵</param>
		void UpdateStartAnchor(SVG.Paths.SVGPathSeg seg,Matrix matrix,System.Collections.ArrayList dealed)
		{
			if(seg == null)
				return ;
			short type = (short)seg.PathSegType;
			SVG.Interface.Paths.ISVGPathSeg pre = this.svgPathSegList.PreviousSibling(seg);
			if(pre == null)
				return;
			//确定其实锚点
			PointF start = pre.GetLastPoint(this.svgPathSegList);
			switch(type)
			{
					//闭合
				case (short)SVG.PathSegmentType.PATHSEG_CLOSEPATH:
					break;

					//直线
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_REL:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_REL:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_REL:
					break;
					//起始
				case (short)SVG.PathSegmentType.PATHSEG_MOVETO_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_MOVETO_REL:

					break;

					//贝赛尔连接
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_REL:
					SVG.Paths.SVGPathSegCurve curve = seg as SVG.Paths.SVGPathSegCurve;
					if(curve != null)
					{
						PointF p1 = curve.GetFirstControl(this.svgPathSegList);
						PointF p2 = curve.GetSecondControl(this.svgPathSegList);
						PointF p3 = curve.GetLastPoint(this.svgPathSegList);
						//前一个锚点和第一控制点发生改变
						PointF[] ps = new PointF[]{start,p1};
						matrix.TransformPoints(ps);
						curve.X1 = ps[1].X;
						curve.Y1 = ps[1].Y;
						ps = null;
					}
					break;

					//椭圆弧连接
				case (short)SVG.PathSegmentType.PATHSEG_ARC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_ARC_REL:
					
					break;
			}

			//注意处理单元前为Move的情况，这时也需要更新该单元
			if(pre is SVG.Paths.SVGPathSegMove&& !dealed.Contains(pre))
			{
				SVG.Paths.SVGPathSegMove move = pre as SVG.Paths.SVGPathSegMove;
				move.X = start.X + matrix.OffsetX;
				move.Y = start.Y + matrix.OffsetY;
				PointF p = move.GetRelativeNextControl(this.svgPathSegList);
				if(!p.IsEmpty)
					move.SetRelativeNextControl(new PointF(p.X + matrix.OffsetX,p.Y + matrix.OffsetY));
				dealed.Add(pre);
			}
		}
		#endregion

		#region ..更新全部路径单元
		/// <summary>
		/// 更新全部路径单元
		/// </summary>
		/// <param name="seg">路径单元</param>
		/// <param name="matrix">变换矩阵</param>
		void UpdateWholeSeg(SVG.Paths.SVGPathSeg seg,Matrix matrix,System.Collections.ArrayList dealed)
		{
			if(seg == null)
				return ;
			short type = (short)seg.PathSegType;
			PointF start = PointF.Empty;
			SVG.Interface.Paths.ISVGPathSeg pre = this.svgPathSegList.PreviousSibling(seg);
			if(pre != null)
			{
				//确定开始端点
				start = pre.GetLastPoint(this.svgPathSegList);
			}
			SVG.Interface.Paths.ISVGPathSeg next = this.svgPathSegList.NextSibling(seg);
			switch(type)
			{
					//闭合
				case (short)SVG.PathSegmentType.PATHSEG_CLOSEPATH:

					break;

					//直线
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_HORIZONTAL_REL:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_REL:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_LINETO_VERTICAL_REL:
					SVG.Paths.SVGPathSegLine line = seg as SVG.Paths.SVGPathSegLine;
					if(line != null && !start.IsEmpty)
					{
						line.X += matrix.OffsetX;
						line.Y += matrix.OffsetY;

						if(next == null)
						{
							PointF p1 = line.GetRelativeNextControl(this.svgPathSegList);
							if(!p1.IsEmpty)
								line.SetRelativeNextControl(new PointF(p1.X + matrix.OffsetX,p1.Y + matrix.OffsetY));
						}
					}
					break;

					//起始
				case (short)SVG.PathSegmentType.PATHSEG_MOVETO_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_MOVETO_REL:
					SVG.Paths.SVGPathSegMove move = seg as SVG.Paths.SVGPathSegMove;
					if(move != null)
					{
						PointF p1 = move.GetRelativePreControl(this.svgPathSegList);
						PointF p2 = move.GetLastPoint(this.svgPathSegList);
						move.X = p2.X + matrix.OffsetX;
						move.Y = p2.Y + matrix.OffsetY;
						if(!p1.IsEmpty)
							move.SetRelativePreControl(new PointF(p1.X + matrix.OffsetX ,p1.Y + matrix.OffsetY));
					}
					break;

					//贝赛尔连接
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_REL:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_REL:
					SVG.Paths.SVGPathSegCurve curve = seg as SVG.Paths.SVGPathSegCurve;
					if(curve != null && !start.IsEmpty)
					{
						PointF p1 = curve.GetFirstControl(this.svgPathSegList);
						PointF p2 = curve.GetSecondControl(this.svgPathSegList);
						PointF p3 = curve.GetLastPoint(this.svgPathSegList);
						curve.X += matrix.OffsetX;
						curve.X1 += matrix.OffsetX;
						curve.X2 += matrix.OffsetX;
						curve.Y += matrix.OffsetY;
						curve.Y1 += matrix.OffsetY;
						curve.Y2 += matrix.OffsetY;

						//注意处理封闭情形
						if(next is SVG.Paths.SVGPathSegClosePath)
						{
							SVG.Paths.SVGPathSegMove m = this.svgPathSegList.GetRelativeStartPathSeg(seg);
							if(m != null && this.InPoint(m.GetLastPoint(this.svgPathSegList),p3,delta))
								m.SetRelativePreControl(new PointF(curve.X2,curve.Y2));
						}
							//如果后面不存在路径单元
						else if(next == null || next is SVG.Paths.SVGPathSegMove)
						{
							p1 = curve.GetRelativeNextControl(this.svgPathSegList);
							if(!p1.IsEmpty)
								curve.SetRelativeNextControl(new PointF(p1.X + matrix.OffsetX,p1.Y + matrix.OffsetY));
						}
					}
					break;

					//椭圆弧连接
				case (short)SVG.PathSegmentType.PATHSEG_ARC_ABS:
				case (short)SVG.PathSegmentType.PATHSEG_ARC_REL:
					SVG.Paths.SVGPathSegArc arc = seg as SVG.Paths.SVGPathSegArc;
					if(arc != null)
					{
						arc.X += matrix.OffsetX;
						arc.Y += matrix.OffsetY;
					}
					break;
			}

			//注意处理单元前为Move的情况，这时也需要更新该单元
			if(pre is SVG.Paths.SVGPathSegMove && !dealed.Contains(pre))
			{
				SVG.Paths.SVGPathSegMove move = pre as SVG.Paths.SVGPathSegMove;
				move.X = start.X + matrix.OffsetX;
				move.Y = start.Y + matrix.OffsetY;
				PointF p = move.GetRelativeNextControl(this.svgPathSegList);
				if(!p.IsEmpty)
					move.SetRelativeNextControl(new PointF(p.X + matrix.OffsetX,p.Y + matrix.OffsetY));
				p = move.GetRelativePreControl(this.svgPathSegList);
				if(!p.IsEmpty)
					move.SetRelativePreControl(new PointF(p.X + matrix.OffsetX,p.Y + matrix.OffsetY));  
				dealed.Add(pre);
			}
		}
		#endregion

		#region ..根据Arc创建路径
		void CreateArcPath(SVG.Paths.SVGPathSegArc arc,PointF startPoint,PointF endPoint,ArrayList rectList)
		{
			this.CreateArcPath(arc,startPoint,endPoint,arc.R1,arc.R2,arc.Angle,rectList);
		}

		void CreateArcPath(SVG.Paths.SVGPathSegArc arc,PointF startPoint,PointF endPoint,float r1,float r2,float angle,ArrayList rectList)
		{
			float x=0 ,y=0,width=0,height=0,startangle=0,sweepangle=0;
			SVG.Paths.SVGPathSegArc.CalcuteArc(ref x,ref y,ref width,ref height,ref startangle,ref sweepangle,startPoint,endPoint,r1,r2,angle,arc.LargeArcFlag,arc.SweepFlag);
			using(GraphicsPath path = new GraphicsPath())
			{
				path.StartFigure();
				path.AddArc(x,y,width,height,startangle,sweepangle);
				
				float cx = x + width/2f;
				float cy = y + height /2f;

				using(Matrix matrix = new Matrix())
				{
					matrix.Translate(-cx,-cy);
					path.Transform(matrix);

					matrix.Reset();
					matrix.Rotate(angle);
					path.Transform(matrix);

					matrix.Reset();
					matrix.Translate(cx,cy);
					path.Transform(matrix);
				}
				if(path.PointCount > 1)
				{
					this.reversePath.StartFigure();
					this.reversePath.AddPath(path,false);
				}
				rectList.Add(endPoint);
			}
		}
		#endregion

		#region ..获取Arc两个控制点
		/// <summary>
		/// 获取Arc两个控制点
		/// </summary>
		/// <param name="arc"></param>
		/// <param name="first"></param>
		/// <param name="second"></param>
		void CalculateArcControl(SVG.Paths.SVGPathSegArc arc,out PointF first,out PointF second,PointF anchor)
		{
			float angle = (float)(arc.Angle / 180f * Math.PI);
			float angle1 = angle + (float)Math.PI / 2f;
			float r1 = arc.R1;
			float r2 = arc.R2;
			PointF p1 = anchor;
			first = new PointF(p1.X + r1 * (float)Math.Cos(angle),p1.Y + r1 * (float)Math.Sin(angle));
			second = new PointF(p1.X + r2 * (float)Math.Cos(angle1),p1.Y + r2 * (float)Math.Sin(angle1));
		}

		void CalculateArcControl(SVG.Paths.SVGPathSegArc arc,out PointF first,out PointF second)
		{
			this.CalculateArcControl(arc,out first,out second,arc.GetLastPoint(this.svgPathSegList));
		}
		#endregion

		#region ..获取移动后的对称点
		/// <summary>
		/// 获取移动后的对称点
		/// </summary>
		/// <param name="anchor">相对点</param>
		/// <param name="p1">移动点</param>
		/// <param name="p2">对称点</param>
		/// <param name="endP1">移动过的点</param>
		/// <returns></returns>
		PointF GetRelativePoint(PointF anchor,PointF p1,PointF p2,PointF endP1)
		{
			//保证三点在一条直线上
			bool inline = this.InLine(p1,anchor,p2);
			if( inline && (Control.ModifierKeys & Keys.Alt) != Keys.Alt)
			{
				float r1 = (float)Math.Sqrt(Math.Pow(p2.X - anchor.X,2) + Math.Pow(p2.Y - anchor.Y,2));
				float r2 = (float)Math.Sqrt(Math.Pow(anchor.X - endP1.X,2) + Math.Pow (anchor.Y - endP1.Y,2));
				if(r2 > 0 )
				{
					float rx = (anchor.X - endP1.X) / r2;
					float x = anchor.X + rx * r1;
					float ry = (anchor.Y - endP1.Y) / r2;
					float y = anchor.Y + ry * r1;
					return new PointF(x,y);
				}
			}
			return p2;
		}
		#endregion

		#region ..获取与指定的SegMove相对应的待封闭对象
		/// <summary>
		/// 获取与指定的SegMove相对应的待封闭对象
		/// </summary>
		/// <param name="move"></param>
		/// <returns></returns>
		SVG.Paths.SVGPathSeg FindUnCloseSegForStartSeg(SVG.Paths.SVGPathSegMove move)
		{
			SVG.Interface.Paths.ISVGPathSeg next = this.svgPathSegList.NextSibling(move);
			SVG.Interface.Paths.ISVGPathSeg temp = null;
			while(next != null && !(next is SVG.Paths.SVGPathSegMove))
			{
				if(next is SVG.Paths.SVGPathSegClosePath)
				{
					temp = null;
					break;
				}
				temp = next;
				next = this.svgPathSegList.NextSibling(next);
			}
			
			return temp as SVG.Paths.SVGPathSeg;
		}
		#endregion

		#region ..移动路径时，创建移动路径
		void CreateMovePath(SVG.Paths.SVGPathSeg seg,bool start,PointF p)
		{
			SVG.Paths.SVGPathSeg pre = this.svgPathSegList.PreviousSibling(seg) as SVG.Paths.SVGPathSeg;
			if(start && pre == null)
				return;
			PointF prep = pre.GetLastPoint(this.svgPathSegList);
			//直线
			if(seg is SVG.Paths.SVGPathSegLine)
			{
				if(start)
				{
					this.reversePath.StartFigure();
					this.reversePath.AddLine(p,seg.GetLastPoint(this.svgPathSegList));
				}
				else
				{
					this.reversePath.StartFigure();
					this.reversePath.AddLine(prep,p);
				}
			}
				//贝赛尔
			else if(seg is SVG.Paths.SVGPathSegCurve)
			{
				SVG.Paths.SVGPathSegCurve cur = seg as SVG.Paths.SVGPathSegCurve;
				PointF anchor = cur.GetLastPoint(this.svgPathSegList);
				PointF first = cur.GetFirstControl(this.svgPathSegList);
				PointF second = cur.GetSecondControl(this.svgPathSegList);
				if(start)
				{
					this.reversePath.StartFigure();
					this.reversePath.AddBezier(p,first,second,anchor);
				}
				else
				{
					this.reversePath.StartFigure();
					this.reversePath.AddBezier(prep,first,second,p);
				}
			}
				//闭合
			else if(seg is SVG.Paths.SVGPathSegClosePath)
			{
				if(start)
				{
					this.reversePath.StartFigure();
					this.reversePath.AddLine(p,seg.GetLastPoint(this.svgPathSegList));
				}
				else
				{
					this.reversePath.StartFigure();
					this.reversePath.AddLine(prep,p);
				}
			}
				//椭圆弧
			else if(seg is SVG.Paths.SVGPathSegArc)
			{
				SVG.Paths.SVGPathSegArc arc = seg as SVG.Paths.SVGPathSegArc;
				PointF startPoint = p;
				PointF endPoint = arc.GetLastPoint(this.svgPathSegList);
				if(!start)
				{
					startPoint = prep;
					endPoint = p;
				}
				float x = 0,y = 0,width = 0,height = 0,startangle = 0,sweepangle = 0;
				SVG.Paths.SVGPathSegArc.CalcuteArc(ref x,ref y,ref width,ref height,ref startangle,ref sweepangle,startPoint,endPoint,arc.R1,arc.R2,arc.Angle,arc.LargeArcFlag,arc.SweepFlag);
				using(GraphicsPath path = new GraphicsPath())
				{
					path.StartFigure();
					path.AddArc(x,y,width,height,startangle,sweepangle);
				
					float cx = x + width/2f;
					float cy = y + height /2f;

					using(Matrix matrix = new Matrix())
					{
						matrix.Translate(-cx,-cy);
						path.Transform(matrix);

						matrix.Reset();
						matrix.Rotate(arc.Angle);
						path.Transform(matrix);

						matrix.Reset();
						matrix.Translate(cx,cy);
						path.Transform(matrix);
					}
					if(path.PointCount > 1)
					{
						this.reversePath.StartFigure();
						this.reversePath.AddPath(path,false);
					}
//					rectList.Add(endPoint);
				}
			}
		}
		#endregion

		#region ..移动路径时,更新路径单元
		void UpdateMovePathSeg(SVG.Paths.SVGPathSeg seg,PointF p)
		{
			int index = this.svgPathSegList.IndexOf(seg);
			if(seg is SVG.Paths.SVGPathSegLine)
			{
				SVG.Paths.SVGPathSegLine line = new SVG.Paths.SVGPathSegLinetoAbs(p.X,p.Y);
				this.svgPathSegList.ReplaceItem(line,index);
			}
				//贝赛尔
			else if(seg is SVG.Paths.SVGPathSegCurve)
			{
				SVG.Paths.SVGPathSegCurve cur = seg as SVG.Paths.SVGPathSegCurve;
				if(cur != null)
				{
					PointF first = cur.GetFirstControl(this.svgPathSegList);
					PointF second = cur.GetSecondControl(this.svgPathSegList);
					SVG.Paths.SVGPathSeg cur1 = new SVG.Paths.SVGPathSegCurvetoCubicAbs(p.X,p.Y,first.X,first.Y,second.X,second.Y);
					this.svgPathSegList.ReplaceItem(cur1,index);
				}
			}
				//闭合
			else if(seg is SVG.Paths.SVGPathSegClosePath)
			{

			}
				//椭圆弧
			else if(seg is SVG.Paths.SVGPathSegArc)
			{
				SVG.Paths.SVGPathSegArc arc = seg as SVG.Paths.SVGPathSegArc;
				SVG.Paths.SVGPathSegArc arc1 = new SVG.Paths.SVGPathSegArcAbs(p.X,p.Y,arc.R1,arc.R2,arc.Angle,arc.LargeArcFlag,arc.SweepFlag);
				this.svgPathSegList.ReplaceItem(arc1,index);
			}
			else if(seg is SVG.Paths.SVGPathSegMove)
			{
				SVG.Paths.SVGPathSegMove move = new SVG.Paths.SVGPathSegMovetoAbs(p.X,p.Y);
				this.svgPathSegList.ReplaceItem(move,index);
			}
		}
		#endregion

		#region ..Invalidate
		/// <summary>
		/// 绘制对象发生更改
		/// </summary>
		internal override void Invalidate()
		{
			this.operateTime = -1;
		}
		#endregion

		#region ..ProcessDialogKey
		public override bool ProcessDialogKey(Keys keyData)
		{
			if(this.svgPathSegListElement != null && this.svgPathSegList != null)
			{
				if(keyData == (Keys.Control | Keys.A))
				{
					this.currentSegs.Clear();
					for(int i = 0;i<this.svgPathSegList.NumberOfItems;i++)
						this.currentSegs.Add(this.svgPathSegList.GetItem(i));
				
					this.mouseArea.Invalidate();
					return true;
				}
				else if(keyData == Keys.Escape)
				{
					this.currentSegs.Clear();
					this.mouseArea.Invalidate();
					return true;
				}
                else if (keyData == Keys.Delete)
                {
                    if (this.svgPathSegListElement != null)
                    {
                        foreach (SVG.Paths.SVGPathSeg seg in this.currentSegs)
                        {
                            this.DeleteAnchor(seg);
                        }
                        this.sourcePath = this.svgPathSegList.GetGDIPath();
                        if (this.svgPathSegListElement is SVG.Paths.SVGPathElement)
                            this.UpdatePath();
                        this.currentSegs.Clear();
                        this.mouseArea.Invalidate();
                        return true;
                    }
                }
			}
			
			return base.ProcessDialogKey(keyData);
		}

		#endregion

        #region ..DeleteAnchor
        SVG.Paths.SVGPathSeg DeleteAnchor(SVG.Paths.SVGPathSeg delSeg)
        {
            var seg = delSeg;
            SVG.Paths.SVGPathSeg pre = null;
            SVG.Paths.SVGPathSeg next = null;
            SVG.Paths.SVGPathSeg current = null;
            if (!(seg is SVG.Paths.SVGPathSegClosePath))
            {
                pre = (SVG.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(seg);
                PointF nextr = PointF.Empty;

                PointF prer = PointF.Empty;
                if (pre != null)
                {
                    nextr = pre.GetRelativeNextControl(this.svgPathSegList);
                    prer = pre.GetRelativePreControl(this.svgPathSegList);
                }

                next = (SVG.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(seg);
                int index = this.svgPathSegList.IndexOf(seg);
                this.svgPathSegList.RemoveItem(index);
                if (next is SVG.Paths.SVGPathSegCurve && pre != null)
                {
                    SVG.Paths.SVGPathSegCurve c = (SVG.Paths.SVGPathSegCurve)next;
                    PointF anchor = c.GetLastPoint(this.svgPathSegList);
                    PointF sec = c.GetSecondControl(this.svgPathSegList);
                    next = (SVG.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(next);
                    if (next is SVG.Paths.SVGPathSegClosePath && pre is SVG.Paths.SVGPathSegMove)
                    {
                        this.svgPathSegList.RemoveItem(this.svgPathSegList.IndexOf(c));
                        current = pre;
                        if (pre is SVG.Paths.SVGPathSegMove)
                            ((SVG.Paths.SVGPathSegMove)pre).SetRelativeNextControl(nextr);
                        ((SVG.Paths.SVGPathSegMove)pre).SetRelativePreControl(prer);
                        index = this.svgPathSegList.IndexOf(next);
                        if (index >= 0)
                            this.svgPathSegList.RemoveItem(index);
                    }
                    else
                    {
                        PointF prep = nextr;
                        SVG.Paths.SVGPathSegCurve c1 = new SVG.Paths.SVGPathSegCurvetoCubicAbs(anchor.X, anchor.Y, prep.X, prep.Y, sec.X, sec.Y);
                        this.svgPathSegList.ReplaceItem(c1, this.svgPathSegList.IndexOf(c));
                        current = c1;
                    }
                }
            }
            else
            {
                SVG.Paths.SVGPathSeg ori = seg;
                SVG.Paths.SVGPathSegMove segMove = this.svgPathSegList.GetRelativeStartPathSeg(seg);
                pre = (SVG.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(seg);
                if (pre != null)
                {
                    seg = pre;
                    pre = (SVG.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(seg);
                }
                if (segMove != null)
                {
                    next = segMove;
                    PointF tempp1 = next.GetLastPoint(this.svgPathSegList);
                    PointF tempp2 = seg.GetLastPoint(this.svgPathSegList);
                    int index = this.svgPathSegList.IndexOf(seg);
                    if (Math.Abs(tempp1.X - tempp2.X) < 1 && Math.Abs(tempp1.Y - tempp2.Y) < 1)
                    {
                        next = (SVG.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(segMove);

                        PointF precontrol = PointF.Empty;
                        if (pre != null)
                            precontrol = pre.GetRelativeNextControl(this.svgPathSegList);
                        pre = (SVG.Paths.SVGPathSeg)this.svgPathSegList.NextSibling(next);
                        PointF anchor = next.GetLastPoint(this.svgPathSegList);
                        PointF nextcontrol = next.GetRelativePreControl(this.svgPathSegList);
                        index = this.svgPathSegList.IndexOf(segMove);
                        SVG.Paths.SVGPathSegMove m = new SVG.Paths.SVGPathSegMovetoAbs(anchor.X, anchor.Y);
                        this.svgPathSegList.ReplaceItem(m, index);
                        index = this.svgPathSegList.IndexOf(next);
                        this.svgPathSegList.RemoveItem(index);
                        index = this.svgPathSegList.IndexOf(seg);
                        pre = (SVG.Paths.SVGPathSeg)this.svgPathSegList.PreviousSibling(seg);
                        PointF p = next.GetRelativeNextControl(this.svgPathSegList);
                        if (pre != m)
                        {
                            SVG.Paths.SVGPathSeg c = new SVG.Paths.SVGPathSegCurvetoCubicAbs(anchor.X, anchor.Y, precontrol.X, precontrol.Y, nextcontrol.X, nextcontrol.Y);
                            this.svgPathSegList.ReplaceItem(c, index);
                            c.SetRelativeNextControl(p);
                            current = c;
                        }
                        else
                        {
                            this.svgPathSegList.RemoveItem(index);
                            index = this.svgPathSegList.IndexOf(ori);
                            if (index >= 0)
                                this.svgPathSegList.RemoveItem(index);
                            current = m;
                        }
                        m.SetRelativeNextControl(p);
                        m.SetRelativePreControl(nextcontrol);
                    }
                    else
                    {

                    }
                }
            }
            return current;
        }
        #endregion

        #region ..getSelectSegPath
        internal SVG.Paths.SVGPathElement getSelectSegPath()
        {
            if (this.currentSegs != null)
            {
                SVG.Paths.SVGPathElement path = this.svgPathSegListElement.OwnerDocument.CreateElement("path") as SVG.Paths.SVGPathElement;
                ArrayList indexes = new ArrayList();
                foreach (SVG.Paths.SVGPathSeg seg in this.currentSegs)
                {
                    indexes.Add(this.svgPathSegList.IndexOf(seg));
                }

                SVG.Paths.SVGPathSeg[] segs = new SVG.Paths.SVGPathSeg[this.currentSegs.Count];
                this.currentSegs.CopyTo(segs, 0);
                int[] index = new int[indexes.Count];
                indexes.CopyTo(index);
                Array.Sort(index, segs);
                string d = string.Empty;
                for (int i = 0; i < segs.Length; i++)
                {
                    SVG.Paths.SVGPathSeg seg = segs[i];
                    if (i == 0)
                    {
                        if (!(seg is SVG.Paths.SVGPathSegMove))
                        {
                            PointF p = seg.GetRelativePreControl(this.svgPathSegList);
                            d += string.Format("m %f %f", p.X, p.Y);
                        }
                    }

                    d += seg.ToString();
                }
                path.InternalSetAttribute("d", d);
                return path;
            }
            return null;
        }
        #endregion
    }
}
