using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using YP.SVG;
using YP.SVG.DocumentStructure;
using YP.SVG.BasicShapes;
using YP.SVG.Interface;
using YP.VectorControl.Forms;

namespace YP.VectorControl.Operation
{
	/// <summary>
	/// DragDropEventHandler 的摘要说明。
	/// </summary>
	internal class DragDropEventHandler:Operation
	{
		#region ..构造及消除
		public DragDropEventHandler(Canvas mouseArea):base(mouseArea)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.mouseArea.DragEnter += new System.Windows.Forms.DragEventHandler(drawArea_DragEnter);
			this.mouseArea.DragOver += new System.Windows.Forms.DragEventHandler(drawArea_DragOver);
			this.mouseArea.DragDrop += new System.Windows.Forms.DragEventHandler(drawArea_DragDrop);
			this.mouseArea.DragLeave += new EventHandler(drawArea_DragLeave);
		}

		public override void Dispose()
		{
			this.mouseArea.DragEnter -= new System.Windows.Forms.DragEventHandler(drawArea_DragEnter);
			this.mouseArea.DragOver -= new System.Windows.Forms.DragEventHandler(drawArea_DragOver);
			this.mouseArea.DragDrop -= new System.Windows.Forms.DragEventHandler(drawArea_DragDrop);
			this.mouseArea.DragLeave -= new EventHandler(drawArea_DragLeave);
			base.Dispose ();
		}

		#endregion

		#region..私有变量
		SVG.Interface.ISVGPathable target = null;
		SVG.Interface.ISVGPathable endtarget = null;
		PointF firstCnt = PointF.Empty,secondCnt = PointF.Empty;
        int firstIndex = -1, secondIndex = -1;
        PointF alignPoint = Point.Empty;
		#endregion

		#region ..Drag事件
		private void drawArea_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			this.mouseArea.InDragDrop = true;
			this.mouseArea.ResetOperation();
            alignPoint = Point.Empty;
			if(e.Data.GetDataPresent(Clipboard.DragDropObject.ClipboardSymbolString))
			{
				this.mouseArea.ResetOperation();
                object o = e.Data.GetData(Clipboard.DragDropObject.ClipboardSymbolString);
                if (o is IOutlookBarPath)
				{
					e.Effect = e.Effect | System.Windows.Forms.DragDropEffects.Copy;
					System.Drawing.Drawing2D.GraphicsPath path = null;
					bool cnt = o is SVGConnectionElement;
                    path = (o as IOutlookBarPath).GPath;
					this.ResetColor();
					this.target = null;
					if(path!= null && path.PointCount > 1)
					{
						this.reversePath.AddPath(path,false);
						RectangleF rect = this.reversePath.GetBounds();
                        Point nativePoint = new Point(e.X, e.Y);
						Point p1 = this.mouseArea.PointToClient(new Point(e.X,e.Y));
						Point p = this.mouseArea.PointClientToView(p1);
						using(System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix())
						{
							matrix.Translate(p.X - rect.X - rect.Width / 2,p.Y - rect.Y - rect.Height / 2);							//							matrix.Multiply(this.mouseArea.CoordTransform,MatrixOrder.Append);
							this.reversePath.Transform(matrix);
							this.reversePath.Transform(this.mouseArea.CoordTransform);
                            //连接线
                            if (cnt)
                            {
                                PointF cntPoint = PointF.Empty;
                                RectangleF bounds = RectangleF.Empty;
                                firstIndex = -1;
                                this.target = this.GetConnectTarget(this.reversePath.PathPoints[0], ref cntPoint, ref bounds, ref firstIndex, ConnectionTargetType.StartElement, (o as SVGConnectionElement));//,Enum.ConnectTargetType.StartElement,(o as SVGConnectionElement));
                                int r = Canvas.InfoGrap;
                                if (this.target != null)
                                {
                                    if (firstIndex >= 0 || bounds.IsEmpty)
                                        this.reverseConnetorIndicatorPath.AddRectangle(new RectangleF(cntPoint.X - r / 2, cntPoint.Y - r / 2, r, r));
                                    else
                                        this.reverseConnetorIndicatorPath.AddRectangle(bounds);
                                    this.firstCnt = cntPoint;
                                }

                                secondIndex = -1;
                                this.endtarget = this.GetConnectTarget(this.reversePath.PathPoints[this.reversePath.PointCount - 1], ref cntPoint, ref bounds, 
                                    ref secondIndex, ConnectionTargetType.EndElement, 
                                    (o as SVGConnectionElement));//,Enum.ConnectTargetType.EndElement,(o as SVGConnectionElement));
                                if (this.endtarget != null)
                                {
                                    if (secondIndex >= 0 || bounds.IsEmpty)
                                        this.reverseConnetorIndicatorPath.AddRectangle(new RectangleF(cntPoint.X - r / 2, cntPoint.Y - r / 2, r, r));
                                    else
                                        this.reverseConnetorIndicatorPath.AddRectangle(bounds);
                                    this.secondCnt = cntPoint;
                                }
                            }
                            else
                            {
                                this.reverseSnapIndicatorPath.Reset();
                                RectangleF bounds = this.reversePath.GetBounds();

                                AlignResult[] results = this.AlignToElement(this.reverseSnapIndicatorPath, reversePath, ElementAlign.All);

                                if (results != null)
                                {
                                    foreach (AlignResult result in results)
                                    {
                                        if (result.Horizontal)
                                            matrix.Translate(result.Delta, 0, MatrixOrder.Append);
                                        else
                                            matrix.Translate(0, result.Delta, MatrixOrder.Append);
                                    }
                                }
                                this.reversePath.Transform(matrix);
                            }
						}
						this.XORDrawPath(this.reverseConnetorIndicatorPath);
                        this.XORDrawPath(this.reverseSnapIndicatorPath);
						this.XORDrawPath(this.reversePath);
					}
				}
			}
		}


		private void drawArea_DragOver(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if(e.Data.GetDataPresent(Clipboard.DragDropObject.ClipboardSymbolString))
			{
				object o  = e.Data.GetData(Clipboard.DragDropObject.ClipboardSymbolString);
				if(o is IOutlookBarPath)
				{				
					e.Effect = e.Effect | System.Windows.Forms.DragDropEffects.Copy;
                    this.XORDrawPath(this.reverseConnetorIndicatorPath);
                    this.XORDrawPath(this.reverseSnapIndicatorPath);
                    this.XORDrawPath(this.reversePath);
					this.reverseConnetorIndicatorPath.Reset();
                    this.reverseSnapIndicatorPath.Reset();
					this.reversePath.Reset();
					System.Drawing.Drawing2D.GraphicsPath path = null;
					bool cnt = o is SVGConnectionElement;
                    path = (o as IOutlookBarPath).GPath;
					
					this.ResetColor();
					Point p1 = this.mouseArea.PointToClient(new Point(e.X,e.Y));
					Point p = this.mouseArea.PointClientToView(p1);
					this.target = null;
					if(path!= null && path.PointCount > 1)
					{
						this.reversePath.AddPath(path,false);
						RectangleF rect = this.reversePath.GetBounds();
                        using (System.Drawing.Drawing2D.Matrix matrix = new System.Drawing.Drawing2D.Matrix())
                        {
                            matrix.Translate(p.X - rect.X - rect.Width / 2, p.Y - rect.Y - rect.Height / 2);
                            this.reversePath.Transform(matrix);
                            this.reversePath.Transform(this.mouseArea.CoordTransform);

                            #region ..连接线
                            //连接线
                            if (cnt)
                            {
                                PointF cntPoint = PointF.Empty;
                                RectangleF bounds = RectangleF.Empty;
                                firstIndex = -1;
                                this.target = this.GetConnectTarget(this.reversePath.PathPoints[0], ref cntPoint, ref bounds, ref firstIndex, ConnectionTargetType.StartElement, (o as SVGConnectionElement));//,Enum.ConnectTargetType.StartElement,(o as SVGConnectionElement));
                                int r = Canvas.InfoGrap;
                                if (this.target != null)
                                {
                                    if (firstIndex >= 0 || bounds.IsEmpty)
                                        this.reverseConnetorIndicatorPath.AddRectangle(new RectangleF(cntPoint.X - r / 2, cntPoint.Y - r / 2, r, r));
                                    else
                                        this.reverseConnetorIndicatorPath.AddRectangle(bounds);
                                    this.firstCnt = cntPoint;
                                }

                                secondIndex = -1;
                                this.endtarget = this.GetConnectTarget(this.reversePath.PathPoints[this.reversePath.PointCount - 1], ref cntPoint, ref bounds, ref secondIndex, ConnectionTargetType.EndElement, (o as SVGConnectionElement));//,Enum.ConnectTargetType.EndElement,(o as SVGConnectionElement));
                                if (this.endtarget != null)
                                {
                                    if (secondIndex >= 0 || bounds.IsEmpty)
                                        this.reverseConnetorIndicatorPath.AddRectangle(new RectangleF(cntPoint.X - r / 2, cntPoint.Y - r / 2, r, r));
                                    else
                                        this.reverseConnetorIndicatorPath.AddRectangle(bounds);
                                    this.secondCnt = cntPoint;
                                }
                            }
                            #endregion

                            #region ..图元
                            else
                            {
                                reverseSnapIndicatorPath.Reset();
                                RectangleF bounds = this.reversePath.GetBounds();
                                AlignResult[] results = this.AlignToElement(this.reverseSnapIndicatorPath, reversePath, ElementAlign.All, null,true,true);

                                if (results != null)
                                {
                                    alignPoint = PointF.Empty;
                                    matrix.Reset();
                                    foreach (AlignResult result in results)
                                    {
                                        if (result.Horizontal)
                                        {
                                            alignPoint.X += result.Delta;
                                            matrix.Translate(result.Delta, 0, MatrixOrder.Append);
                                        }
                                        else
                                        {
                                            matrix.Translate(0, result.Delta, MatrixOrder.Append);
                                            alignPoint.Y += result.Delta;
                                        }
                                    }
                                    this.reversePath.Transform(matrix);
                                }
                            }
                            #endregion
                        }
					}
					this.XORDrawPath(this.reversePath);
					this.XORDrawPath(this.reverseConnetorIndicatorPath);
                    this.XORDrawPath(this.reverseSnapIndicatorPath);
				}
				
			}
		}

		private void drawArea_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if(e.Data.GetDataPresent(Clipboard.DragDropObject.ClipboardSymbolString))
			{
				object o  = e.Data.GetData(Clipboard.DragDropObject.ClipboardSymbolString);
				if(o is IOutlookBarPath)
				{
					this.mouseArea.Focus();

					#region ..图元
					if(o is SVGSymbolElement)
					{
						this.XORDrawPath(this.reversePath);
						this.XORDrawPath(this.reverseConnetorIndicatorPath);
                        this.XORDrawPath(this.reverseSnapIndicatorPath);
                        this.reverseSnapIndicatorPath.Reset();
						this.reverseConnetorIndicatorPath.Reset();
						this.reversePath.Reset();

                        SVGSymbolElement sym = o as SVGSymbolElement;
                        PointF p1 = this.mouseArea.PointToClient(new Point(e.X, e.Y));
                        p1 = new PointF(p1.X + alignPoint.X, p1.Y + alignPoint.Y);
                        PointF p = this.mouseArea.PointClientToView(p1);
                        this.mouseArea.DropSymbol(sym, p,true);
					}
						#endregion

						#region ..连接线
					else if(o is SVGConnectionElement)
					{
						PointF start = PointF.Empty;
						PointF end = PointF.Empty;
						if(this.reversePath.PointCount > 1)
						{
							start = this.reversePath.PathPoints[0];
							end = this.reversePath.PathPoints[this.reversePath.PointCount - 1];
						}
						this.XORDrawPath(this.reversePath);
						this.XORDrawPath(this.reverseConnetorIndicatorPath);

						if((o as SVGConnectionElement) != null)
						{					
							SVG.Document.SVGDocument doc = this.mouseArea.SVGDocument;
							bool old = doc.AcceptNodeChanged;
							doc.AcceptNodeChanged = false;
							SVG.BasicShapes.SVGConnectionElement element = doc.ImportNode((o as SVGConnectionElement),true) as SVG.BasicShapes.SVGConnectionElement ;
							
							start = this.mouseArea.PointToVirtualView(start);
							end = this.mouseArea.PointToVirtualView(end);
							element.InternalSetAttribute("x1",start.X.ToString());
							element.InternalSetAttribute("y1",start.Y.ToString());
							element.InternalSetAttribute("x2",end.X.ToString());
							element.InternalSetAttribute("y2",end.Y.ToString());

							if(this.target != null)
							{
								SVG.SVGTransformableElement element1 = this.target as SVG.SVGTransformableElement;
								string id = element1.GetAttribute("id");
								if(id == null || id.Trim().Length == 0)
								{
									id = this.mouseArea.SVGDocument.CreateID(element1,this.mouseArea.SVGDocument.RootElement as SVG.SVGElement);
									element1.InternalSetAttribute("id",id);
								}
//								doc.AcceptNodeChanged = false;
								id = "#" + id;
								using(System.Drawing.Drawing2D.Matrix matrix = this.mouseArea.GetTotalTransformForElement(element1))//.TotalTransform.Clone())
								{
									PointF[] ps = element1.ConnectionPoints.Clone() as  PointF[];
									matrix.TransformPoints(ps);
									int index = Array.IndexOf(ps,this.firstCnt);
									if(index >= 0 && index < ps.Length)
										id += "." + index.ToString();
									ps = null;
								}
								element.InternalSetAttribute("start",id);
							}

							if(this.endtarget != null)
							{
								SVG.SVGTransformableElement element1 = this.endtarget as SVG.SVGTransformableElement;
								string id = element1.GetAttribute("id");
//								doc.AcceptNodeChanged = true;
								if(id == null || id.Trim().Length == 0)
								{
									id = this.mouseArea.SVGDocument.CreateID(element1,this.mouseArea.SVGDocument.RootElement as SVG.SVGElement);
									element1.InternalSetAttribute("id",id);
								}
//								doc.AcceptNodeChanged = false;
								id = "#" + id;
                                using (System.Drawing.Drawing2D.Matrix matrix = this.mouseArea.GetTotalTransformForElement(element1))//.TotalTransform.Clone())
								{
                                    PointF[] ps = element1.ConnectionPoints.Clone() as PointF[];
									matrix.TransformPoints(ps);
									int index = Array.IndexOf(ps,this.secondCnt);
									if(index >= 0 && index < ps.Length)
										id += "." + index.ToString();
									ps = null;
								}
								element.InternalSetAttribute("end",id);
							}
                            doc.AcceptNodeChanged = false;
                            ElementDroppedEventArgs e1 = new ElementDroppedEventArgs(element);
                            this.mouseArea.OnElementDropped(e1);
							doc.AcceptNodeChanged = true;
                            if(e1.DroppedInstance != null)
                                element = this.mouseArea.AddElement(e1.DroppedInstance, true, true) as SVG.BasicShapes.SVGConnectionElement;
							doc.AcceptNodeChanged = old;
							doc.InvokeUndos();
						}
						this.reverseConnetorIndicatorPath.Reset();
						this.reversePath.Reset();
					}
					#endregion
				}
			}
			this.mouseArea.validContent = true;
			this.target = null;
			this.mouseArea.InDragDrop = false;
            alignPoint = Point.Empty;
		}

		private void drawArea_DragLeave(object sender, EventArgs e)
		{
			this.mouseArea.InDragDrop = false;
            this.XORDrawPath(this.reverseConnetorIndicatorPath);
			this.XORDrawPath(this.reversePath);
			this.reversePath.Reset();
            this.reverseConnetorIndicatorPath.Reset();
			this.target = null;
			this.mouseArea.validContent = true;
            alignPoint = Point.Empty;
		}
		#endregion

		#region ..鼠标事件
		/// <summary>
		/// OnMouseDown
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseDown(object sender,MouseEventArgs e)
		{
		}

		/// <summary>
		/// OnMouseMove
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseMove(object sender,MouseEventArgs e)
		{
		}

		/// <summary>
		/// OnMouseUp
		/// </summary>
		/// <param name="e"></param>
		protected override void OnMouseUp(object sender,MouseEventArgs e)
		{
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
	}
}

