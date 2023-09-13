using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using YP.VectorControl.Forms;

namespace YP.VectorControl.Operation.LabelText
{
	/// <summary>
	/// LabelTextOperation 的摘要说明。
	/// </summary>
	internal class LabelTextOperation:Operation
	{
		#region ..构造及消除
        public LabelTextOperation(Canvas mousearea)
            : this(mousearea, Operator.None)
        {
        }

		public LabelTextOperation(Canvas mousearea,Operator preOp ):base(mousearea)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.mouseArea.KeyPress += new KeyPressEventHandler(mouseArea_KeyPress);
            this.mouseArea.SVGDocument.NodeRemoved += new System.Xml.XmlNodeChangedEventHandler(SVGDocument_NodeRemoved);
			this.caret = new Caret();
			this.caretthread = new Thread(new ThreadStart(this.CaretMethod));
			this.caretthread.IsBackground = true;
			this.caretthread.Start();
			this.caret.OffsetChanged += new EventHandler(caret_OffsetChanged);
			this.selection.SelectionChanged += new EventHandler(selection_SelectionChanged);
			this.mouseArea.validContent = false;
			this.AttachAction();
            this.preOperator = preOp;
            
		}

		public LabelTextOperation(Canvas mousearea,SVG.Text.SVGTextBlockElement render, Operator preOp, PointF? viewPoint):this(mousearea, preOp)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.CaretRender = render;

            if (viewPoint.HasValue)
            {
                Point p = Point.Round(this.mouseArea.PointViewToClient(viewPoint.Value));
                MouseEventArgs e = new MouseEventArgs(MouseButtons.Left, 1, p.X, p.Y, 0);
                this.OnMouseDown(null, e);
                this.mousedown = false;
            }
		}

        public override void Dispose()
        {
            if (disposed)
                return;
            base.Dispose();
            this.caretthread.Abort();
            this.caret.Visible = false;
            this.disposed = true;
            this.caret.OffsetChanged -= new EventHandler(caret_OffsetChanged);
            this.selection.SelectionChanged -= new EventHandler(selection_SelectionChanged);
            if (this.caretRender != null)
            {
                if (!(this.caretRender.ParentElement is SVG.DocumentStructure.SVGGElement))
                {
                    this.mouseArea.SVGDocument.ChangeSelectElement(this.caretRender.ParentElement);
                    this.mouseArea.onlyInvalidateSelection = false;
                }
                this.caretRender.SVGRenderer.DrawLabelBounds = false;
            }
            this.mouseArea.validContent = true;
            this.mouseArea.onlyInvalidateSelection = false;
            if (this.mouseArea.SVGDocument != null)
                this.mouseArea.SVGDocument.NodeRemoved -= new System.Xml.XmlNodeChangedEventHandler(SVGDocument_NodeRemoved); 
            this.mouseArea.KeyPress -= new KeyPressEventHandler(mouseArea_KeyPress);
            
        }
		#endregion

		#region ..私有变量
        Operator preOperator = Operator.None;
		SVG.SVGTransformableElement labelElement = null;
		SVG.SVGTransformableElement preRender = null;
		PointF startPoint = PointF.Empty;
		PointF endPoint = PointF.Empty;
		System.DateTime firstDown ;
		SVG.Text.SVGTextBlockElement caretRender = null;
		//new System.Drawing.Drawing2D.GraphicsPath reversePath = new GraphicsPath();
		Caret caret = null;
		PointF caretPoint = PointF.Empty,caretPoint1 = PointF.Empty;
		Thread caretthread;
		System.Collections.Hashtable actions = new System.Collections.Hashtable();
		bool inupdate = false;
		bool invert = false;
		int startOffset = -1;
		Selection selection = new Selection();
		bool causeSelect = false;
		ArrayList undos = new ArrayList();
		#endregion

		#region ..properties
		/// <summary>
		/// 获取或设置不要编辑标记文本的对象
		/// </summary>
		internal SVG.SVGTransformableElement LabelElement
		{
			set
			{
				this.labelElement = value;
			}
			get
			{
				return this.labelElement;
			}
		}

		Matrix TotalTransform
		{
			get
			{
                if (this.caretRender != null && this.caretRender.OwnerDocument.ScaleStroke)
                    return this.mouseArea.GetTotalTransformForElement(this.caretRender);//.TotalTransform;
				return this.mouseArea.CoordTransform;
			}
		}

		internal override bool EditText
		{
			get
			{
				return this.caretRender != null && this.caretRender.ParentNode != null;
			}
		}

		internal SVG.Text.SVGTextBlockElement CaretRender
		{
			get
			{
				return this.caretRender;
			}
			set
			{
				if(this.caretRender != value)
				{
                    if (this.caretRender != null)
                    {
                        this.caretRender.Selected = false;
                        this.caretRender.SVGRenderer.DrawLabelBounds = false;
                    }
					this.caretRender = value;
                    if (this.caretRender != null)
                    {
                        if (this.caretRender.OwnerDocument.SelectCollection.Count > 1 || this.caretRender.OwnerDocument.SelectCollection[0] != this.caretRender)
                            this.caretRender.OwnerDocument.ChangeSelectElement(this.caretRender);
                        this.caretRender.SVGRenderer.DrawLabelBounds = true;
                    }

                    if (this.caretRender == null && this.preOperator == Operator.Transform)
                        this.ChangeToTransform();
                   
					this.mouseArea.Invalidate();
				}

				if(EditText)
				{
					this.preRender = null;
				}
			}
		}

		internal Caret Caret
		{
			get
			{
				return this.caret;
			}
		}

		internal Selection Selection
		{
			get
			{
				return this.selection;
			}
		}
		#endregion

        #region ..override OnPaint
        protected override void OnPaint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			this.DrawXorPath(e);
            try
            {
                if (EditText)
                {
                    PaintContent(e.Graphics);
                    this.PaintCaret(e.Graphics);
                    this.PaintSelection(e.Graphics);
                }

            }
            catch (System.Exception e1)
            {
                System.Diagnostics.Debug.Assert(true, e1.Message);
            }
		}
		#endregion

        #region ..PaintContent
        void PaintContent(Graphics g)
        {
            if (EditText && this.caretRender is SVG.Text.SVGTextBlockElement && this.caretRender.ParentNode != null)
            {
                using (Matrix matrix = this.mouseArea.CoordTransform)
                {
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    SVG.Text.SVGTextBlockElement textBlock = this.caretRender as SVG.Text.SVGTextBlockElement;
                    if (textBlock.SVGRenderer.DrawLabelBounds)
                    {
                        using (Pen pen = new Pen(Color.White, 0.5f))
                        {
                            if (textBlock.GPath != null && textBlock.GPath.PointCount > 1)
                            {
                                using (GraphicsPath path1 = textBlock.GPath.Clone() as GraphicsPath)
                                {
                                    path1.Transform(textBlock.TotalTransform);
                                    path1.Transform(matrix);
                                    g.FillPath(Brushes.White, path1);
                                    pen.DashStyle = DashStyle.Dash;
                                    pen.Color = Color.Gray;
                                    g.DrawPath(pen, path1);
                                }
                            }
                        }

                        (textBlock.SVGRenderer as SVG.Render.SVGTextBlockRender).DrawContent(g, this.mouseArea.CoordTransform, true);
                    }
                }
            }
        }
        #endregion

        #region ..override MouseEvent
        protected override void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            if (disposed)
                return;
			if(e.Button == MouseButtons.Left)
			{
				this.mousedown = true;
				this.endPoint = this.startPoint = new PointF(e.X,e.Y);
                //sender == null指明模拟mousedown
                if (sender != null)
                {
                    if (this.caretRender != null && this.preRender != this.caretRender && this.preRender != this.caretRender.ViewPortElement)
                    {
                        if (this.preRender != null)
                        {
                            if (!this.mouseArea.SVGDocument.SelectCollection.Contains(this.preRender))
                            {
                                this.mouseArea.InvalidateSelection();
                                this.mouseArea.SVGDocument.ChangeSelectElement(this.preRender);
                            }
                        }
                        this.CaretRender = null;
                    }
                    else if (this.preRender == null)
                    {
                        if(this.caretRender != null && !(this.caretRender.ParentElement is SVG.DocumentStructure.SVGGElement))
                            this.ChangeToTransform();
                    }
                }
                if (EditText)
                {
                    using (System.Drawing.Graphics g = this.mouseArea.CreateGraphics())
                    {
                        PointF p = this.mouseArea.PointClientToView(new PointF(e.X, e.Y));
                        this.caretRender.SVGRenderer.DrawLabelBounds = true;
                        int index = this.caretRender.SVGRenderer.FindOffsetAtPoint(p, g);
                        this.startOffset = index;
                        if (index >= 0)
                        {
                            this.causeSelect = true;
                            if ((Control.ModifierKeys & Keys.Shift) != Keys.Shift)
                            {
                                this.selection.AdaptSelection(index, 0);
                                this.caret.Offset = index;
                            }
                            else
                            {
                                this.startOffset = this.caret.OriOffset;
                                this.caret.Offset = index;
                            }
                        }
                    }
                }
                else
                {
                    PointF p = new PointF(e.X, e.Y);
                    var result = this.mouseArea.SnapPointToGuide(ref p);
                    if ( result == GuideResult.None)
                        p = this.mouseArea.SnapPointToGrid(p,(result & GuideResult.X) != GuideResult.X,(result & GuideResult.Y) != GuideResult.Y);

                    this.endPoint = this.startPoint = p;// this.mouseArea.SnapPointToGuide(this.mouseArea.SnapPointToGrid(new PointF(e.X, e.Y)));
                }
				this.firstDown = System.DateTime.Now;
			}
		}

		protected override void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(this.disposed)
				return;

			bool allowMove = this.mouseArea.AllowToMoveLabel;

			#region ..无键
			if(e.Button == MouseButtons.None)
			{
				this.preRender = null;
				
				SVG.SVGTransformableElement render = this.GetRenderAtPoint(new PointF(e.X,e.Y),true);
				
				if(render != null)
				{
					if((render != this.caretRender || this.caretRender == null) && allowMove)
					{
						if(render.Label.Length > 0)
							this.mouseArea.Cursor = System.Windows.Forms.Cursors.SizeAll;
					}
					else if(render == this.caretRender || (this.caretRender != null && render == this.caretRender.ViewPortElement))
						this.mouseArea.Cursor = System.Windows.Forms.Cursors.IBeam;
					this.preRender = render;
					return;
				}
				else
					this.mouseArea.Cursor = this.mouseArea.DefaultCursor;
			}
				#endregion

				#region ..左键
				//非编辑状态
            else if (e.Button == MouseButtons.Left)
            {
                PointF p = this.mouseArea.PointClientToView(new PointF(e.X, e.Y));

                //非编辑模式，添加
                if (this.caretRender == null && this.mousedown)
                {
                    this.XORDrawPath(this.reversePath);
                    this.XORDrawPath(this.reverseSnapIndicatorPath);
                    this.reverseSnapIndicatorPath.Reset();
                    float left = (float)Math.Min(startPoint.X, e.X);
                    float top = (float)Math.Min(startPoint.Y, e.Y);
                    float right = (float)Math.Max(startPoint.X, e.X);
                    float bottom = (float)Math.Max(startPoint.Y, e.Y);
                    this.reversePath.Reset();
                    this.reversePath.AddRectangle(new RectangleF(left, top, right - left, bottom - top));

                    RectangleF bounds = this.reversePath.GetBounds();
                    //对齐
                     AlignResult[] results = this.AlignToElement(this.reverseSnapIndicatorPath, this.reversePath, ElementAlign.All, null,true,true);

                     if (results != null)
                     {
                         using (Matrix temp = new Matrix())
                         {
                             temp.Translate(-startPoint.X, -startPoint.Y);
                             foreach (AlignResult result in results)
                             {
                                 if (result.Horizontal && bounds.Width > 0)
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
                                 else if (bounds.Height > 0)
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

                             endPoint = new PointF(x, y);
                         }
                     }
                    this.XORDrawPath(this.reversePath);
                }
                //编辑状态
                else if (this.mousedown && EditText)
                {
                    using (System.Drawing.Graphics g = this.mouseArea.CreateGraphics())
                    {
                        int index = this.caretRender.SVGRenderer.FindOffsetAtPoint(p, g);
                        if (index >= 0)
                        {
                            if (this.startOffset >= 0)
                            {
                                int right = (int)Math.Max(index, this.startOffset);
                                index = (int)Math.Min(index, this.startOffset);
                                selection.AdaptSelection(index, right - index);
                            }
                            this.caret.Offset = index;
                        }
                    }
                }
            }
			#endregion
		}

		protected override void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(this.disposed)
				return;
            //if(this.caretRender == null)
            //    this.mouseArea.CurrentOperator = this.mouseArea.oldOperator;
            if (this.mousedown)
            {
                //添加
                if (this.caretRender == null)
                {
                    this.XORDrawPath(this.reversePath);
                    this.XORDrawPath(this.reverseSnapIndicatorPath);
                    this.reverseSnapIndicatorPath.Reset();
                    this.reversePath.Reset();
                    var doc = this.mouseArea.SVGDocument;
                    bool old = doc.AcceptNodeChanged;
                    doc.AcceptNodeChanged = false;
                    var textBlock = this.mouseArea.SVGDocument.CreateElement("textBlock") as SVG.SVGElement;
                    PointF p1 = this.mouseArea.PointClientToView(this.startPoint);
                    PointF p2 = this.mouseArea.PointClientToView(this.endPoint);
                    float left = (float)Math.Min(p1.X, p2.X);
                    float top = (float)Math.Min(p1.Y, p2.Y);
                    float right = (float)Math.Max(p1.X, p2.X);
                    float bottom = (float)Math.Max(p1.Y, p2.Y);
                    
                    float width = right - left;
                    if (width < 2)
                    {
                        width = 100;
                        if (this.mouseArea.TextBlockStyle.Alignment == SVG.Alignment.Center)
                            left = left - width / 2;
                    }
                    float height = bottom - top;

                    if (height < 2)
                    {
                        using (Font font = new Font(this.mouseArea.TextStyle.FontName, this.mouseArea.TextStyle.Size))
                            height = font.GetHeight();
                    }
                    textBlock.InternalSetAttribute("x", left.ToString());
                    textBlock.InternalSetAttribute("y", top.ToString());
                    textBlock.InternalSetAttribute("width", width.ToString());
                    textBlock.InternalSetAttribute("height", height.ToString());
                    doc.AcceptNodeChanged = true;
                    this.CaretRender = this.mouseArea.AddElement(textBlock) as SVG.Text.SVGTextBlockElement;
                    doc.InvokeUndos();
                }
                //编辑状态
                else if (EditText)
                {
                    using (System.Drawing.Graphics g = this.mouseArea.CreateGraphics())
                    {
                        PointF p = this.mouseArea.PointClientToView(new PointF(e.X, e.Y));
                        int index = this.caretRender.SVGRenderer.FindOffsetAtPoint(p, g);
                        if (index >= 0)
                        {
                            this.caret.Offset = index;
                            if (this.startOffset >= 0)
                            {
                                int right = (int)Math.Max(index, this.startOffset);
                                index = (int)Math.Min(index, this.startOffset);

                                if ((Control.ModifierKeys & Keys.Shift) != Keys.Shift)
                                    selection.AdaptSelection(index, right - index);
                            }
                        }
                    }
                }
            }
            
			this.preRender = null;
			this.mouseArea.validContent = true;
			this.mousedown = false;
			this.causeSelect = false;
		}
		#endregion

		#region ..OnAdatpAttribute
		protected override void OnAdaptAttribute(object sender, AdaptAttributeEventArgs e)
		{
		}

		#endregion

		#region ..获取在某点的对象
		SVG.SVGTransformableElement GetRenderAtPoint(PointF point)
		{
			return this.GetRenderAtPoint(point,false);
		}

		SVG.SVGTransformableElement GetRenderAtPoint(PointF point,bool includeRender)
		{
            point = this.mouseArea.PointClientToView(point);
            if (this.caretRender != null)
            {
                using (GraphicsPath path = this.caretRender.GPath.Clone() as GraphicsPath)
                {
                    path.Transform(this.caretRender.TotalTransform);
                    if (path.IsVisible(point))
                        return this.caretRender;
                }
            }
			using(System.Drawing.Graphics g = this.mouseArea.CreateGraphics())
			{
				SVG.SVGElementCollection list = this.mouseArea.renderElements;
				using(System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath())
				{
					using (System.Drawing.Drawing2D.Matrix matrix = new Matrix())
					{
						for(int i = list.Count - 1;i>=0;i--)
						{
							SVG.SVGTransformableElement render = list[i] as SVG.SVGTransformableElement;
							if(render == null || render is SVG.Text.SVGTextContentElement)
								continue;

                            path.Reset();
                            if (render.GraphicsPathIncludingTextBlock != null && render.GraphicsPathIncludingTextBlock.PointCount > 1)
                            {
                                path.AddRectangle(render.GraphicsPathIncludingTextBlock.GetBounds());
                                if (path.IsVisible(point))
                                    return render;
                            }
							if(includeRender && this.mouseArea.HitTest(render,point))
							{
                                return render;
                                //path.Reset();
                                //if ((render as SVGDom.Interface.ISVGPathable).GPath != null && (render as SVGDom.Interface.ISVGPathable).GPath.PointCount > 1)
                                //{
                                //    path.AddPath((render as SVGDom.Interface.ISVGPathable).GPath, false);
                                //    path.Transform(render.TotalTransform);
                                //    if (path.IsVisible(point) || path.IsOutlineVisible(point, this.mouseArea.SelectedPen))
                                //    {
                                //        return render;
                                //    }
                                //}
							}
						}
						
					}
				}
			}
			return null;
		}
		#endregion

		#region ..更新标示文本位置
		void UpdateLabelText()
		{
			if(this.preRender != null)
			{
				//this.mouseArea.InvalidateLabelText(this.preRender);
                using (System.Drawing.Drawing2D.Matrix matrix = this.mouseArea.GetTotalTransformForElement(this.preRender))//.TotalTransform.Clone())
				{
					matrix.Invert();
					PointF[] ps = new PointF[]{this.startPoint,this.endPoint};
					matrix.TransformPoints(ps);
					RectangleF rect = (this.preRender as SVG.Interface.ISVGPathable).GPath.GetBounds();
					bool old = this.mouseArea.SVGDocument.AcceptNodeChanged;
					this.mouseArea.SVGDocument.AcceptNodeChanged = true;
					float x = ps[1].X - ps[0].X;
					float y = ps[1].Y - ps[0].Y;
					x = x + this.preRender.LabelPoint.X;
					y += this.preRender.LabelPoint.Y;
					preRender.InternalSetAttribute("labelX",x.ToString());
					preRender.InternalSetAttribute("labelY",y.ToString());
					this.mouseArea.SVGDocument.AcceptNodeChanged = old;
					this.mouseArea.SVGDocument.InvokeUndos();
				}
			}
		}
		#endregion

		#region ..PaintCaret
		void PaintCaret(Graphics g)
		{
			if(EditText && this.caretRender.ParentNode != null && this.selection.IsEmpty && this.caret.Visible && !this.inupdate)
			{
				int offset = this.caret.Offset;
				this.caretPoint = this.caretRender.SVGRenderer.GetPointAtIndex(offset,g,ref this.caretPoint1);
                PointF[] ps = new PointF[] { this.caretPoint, this.caretPoint1 };
                this.mouseArea.CoordTransform.TransformPoints(ps);
                this.caretPoint = ps[0];
                this.caretPoint1 = ps[1];
				g.DrawLine(Pens.Black,ps[0],this.caretPoint1);
			}
		}
		#endregion

		#region ..PaintSelection
		void PaintSelection(Graphics g)
		{
			if(EditText && !this.selection.IsEmpty)
			{
                RectangleF[] rects = this.caretRender.SVGRenderer.GetRegion(this.selection.Offset, this.selection.Length, g);
				if(rects == null || rects.Length == 0)
					return;
				System.Drawing.Drawing2D.GraphicsContainer c = g.BeginContainer();
				g.SmoothingMode = SmoothingMode.HighSpeed;
				using(System.Drawing.Drawing2D.Matrix matrix = new Matrix())
				{
                    matrix.Multiply(this.caretRender.SVGRenderer.GetLabelTransform());
                    matrix.Multiply(this.mouseArea.CoordTransform, MatrixOrder.Append);
					using(System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath())
					{
						System.IntPtr hdc = g.GetHdc();
						Win32.SetROP2(hdc,6);
						for(int i = 0;i<rects.Length;i++)
						{
							path.Reset();
							path.AddRectangle(rects[i]);
							path.Transform(matrix);
							Win32.Win32PolyPolygon(hdc,path);
						}
						g.ReleaseHdc(hdc);
					}
				}
				g.EndContainer(c);
			}
		}
		#endregion

		#region ..CaretThread
		void CaretMethod()
		{
			while(true)
			{
                if (EditText && !this.inupdate)
                {
                    this.caret.Visible = !this.caret.Visible;
                    using (System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath())
                    {
                        path.AddLine(this.caretPoint, this.caretPoint1);
                        RectangleF rect = path.GetBounds();
                        this.mouseArea.Invalidate(new Rectangle((int)rect.X - 1, (int)rect.Y - 1, (int)rect.Width + 4, (int)rect.Height + 4));
                    }
                    Thread.Sleep(400);
                }
			}
		}
		#endregion

		#region ..KeyPress
		private void mouseArea_KeyPress(object sender, KeyPressEventArgs e)
		{
			if(EditText)
			{
				char ch = e.KeyChar;
				if(ch < ' ')
					return;
				this.Insert(this.caret.Offset,ch.ToString());
				this.InvokeUndo();
			}
		}
		#endregion

		#region ..RemoveString
		internal void RemoveString(int index,int length)
		{
			if(EditText)
			{
				this.BeginUpdate();
                this.mouseArea.InvalidateElement(this.caretRender as SVG.Interface.ISVGPathable);
				index = (int)Math.Max(0,Math.Min(this.caretRender.Label.Length,index));
				if(this.mouseArea.SVGDocument.AcceptNodeChanged)
				{
					DeleteUndo undo = new DeleteUndo(this,index,length);
					this.undos.Add(undo);
				}
                this.caretRender.SVGRenderer.RemoveString(index, length);
				this.caret.Offset = index;
                this.mouseArea.InvalidateElement(this.caretRender as SVG.Interface.ISVGPathable);
				this.EndUpdate();
			}
		}
		#endregion

		#region ..ProcessDialogKey
		public override bool ProcessDialogKey(Keys keyData)
		{
			Keys key = keyData;

			if(this.actions.ContainsKey(key))
			{
				this.causeSelect = true;
				Action action = this.actions[key] as Action;
				invert = action is Down || action is Up;
				action.Execute(this);
				this.invert = false;
				this.causeSelect = false;
				this.InvokeUndo();
				return true;
			}
			//选择，剪贴板
			switch(keyData)
			{
				case Keys.Control | Keys.A:
					this.SelectAll();
					return true;
				case Keys.Escape:
					this.SelectNone();
					return true;
				case Keys.Control | Keys.C:
					this.Copy();
					return true;
				case Keys.Control | Keys.X:
					this.Copy();
					this.ClearSelect();
					this.InvokeUndo();
					return true;
				case Keys.Control | Keys.V:
					this.Paste();
					this.InvokeUndo();
					return true;
			}
			return false;
		}

		#endregion

        #region ..AttachAction
        void AttachAction()
		{
			this.actions[Keys.Delete] = new Delete();
			this.actions[Keys.Back] = new BackSpace();
			this.actions[Keys.Left] = new Left();
			this.actions[Keys.Right] = new Right();
			this.actions[Keys.Tab] = new Tab();
			this.actions[Keys.Enter] = new Enter();
			this.actions[Keys.Up] = new Up();
			this.actions[Keys.Down] = new Down();
			this.actions[Keys.Home] = new Home();
			this.actions[Keys.End] = new End();
			this.actions[Keys.Shift | Keys.End] = new End();
			this.actions[Keys.Shift | Keys.Home] = new Home();
			this.actions[Keys.Shift | Keys.Left] = new Left();
			this.actions[Keys.Shift | Keys.Right] = new Right();
			this.actions[Keys.Shift | Keys.Up] = new Up();
			this.actions[Keys.Shift | Keys.Down] = new Down();
		}
		#endregion

        #region ..caret_OffsetChanged
        private void caret_OffsetChanged(object sender, EventArgs e)
		{
			if(!this.invert)
				this.caret.OldLength = float.MaxValue;
			if(!this.causeSelect || (Control.ModifierKeys & Keys.Shift) != Keys.Shift)
				this.caret.OriOffset = this.caret.Offset;
			else
			{
				int offset = this.caret.OriOffset;
				int offset1 = this.caret.Offset;
				int min = (int)Math.Min(offset,offset1);
				int max = (int)Math.Max(offset,offset1);
				this.selection.AdaptSelection(min,max - min);
			}
            this.InvalidateRegion(this.caret.OldOffset, 2);
			caret.Visible = true;
            this.InvalidateRegion(this.caret.Offset,2);
            caret.Visible = true;
		}
		#endregion

		#region ..InvalidateOffset
		void InvalidateOffset(int offset)
		{
			using(System.Drawing.Graphics g = this.mouseArea.CreateGraphics())
			{
                if(EditText)
                {
					PointF p = PointF.Empty;
					PointF p1 = this.caretRender.SVGRenderer.GetPointAtIndex(offset,g,ref p);
                    PointF[] ps = { p, p1 };
                    this.mouseArea.CoordTransform.TransformPoints(ps);
                    p = ps[0];
                    p = ps[1];
					using(System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath())
					{
						path.AddLine(p,p1);
						RectangleF rect = path.GetBounds();
						this.mouseArea.Invalidate(new Rectangle((int)rect.X -1,(int)rect.Y - 1,(int)rect.Width + 4,(int)rect.Height + 4));
					}
                }
			}
		}
		#endregion

		#region ..Insert
		internal void Insert(int offset,string str)
		{
			if(EditText)
			{
                if (!this.selection.IsEmpty)
                {
                    this.ClearSelect();
                    offset = this.caret.Offset;
                }
				this.BeginUpdate();
                this.mouseArea.InvalidateElement(this.caretRender as SVG.Interface.ISVGPathable);
				offset = (int)Math.Max(0,Math.Min(offset,this.caretRender.Label.Length));
				if(this.mouseArea.SVGDocument.AcceptNodeChanged)
				{
					InsertUndo undo = new InsertUndo(this,offset,str);
					this.undos.Add(undo);
				}
				this.caretRender.SVGRenderer.InsertStr(offset,str); 
				this.caret.Offset = offset + str.Length;

                this.mouseArea.InvalidateElement(this.caretRender as SVG.Interface.ISVGPathable);
				this.EndUpdate();

				this.mouseArea.InvokeEditTextEvent(str);
			}
		}
		#endregion

		#region ..BeginUpdate
		void BeginUpdate()
		{
			this.inupdate = true;
		}
		#endregion

		#region ..EndUpdate
		void EndUpdate()
		{
			this.inupdate = false;
		}
		#endregion

        #region ..CalculateLengthAtOffset
        /// <summary>
        /// 上下移动时，计算开始坐标
        /// </summary>
        internal void CalculateLengthAtOffset()
		{
			if(this.caretRender == null)
				return;
			using(System.Drawing.Graphics g = this.mouseArea.CreateGraphics())
				this.caret.OldLength = this.caretRender.SVGRenderer.CalculateLengthAtOffset(this.caret.Offset,g);
		}
		#endregion

		#region ..MoveVectically
		internal void MoveVertically(bool up)
		{
			if(EditText)
			{
				if(this.caret.OldLength == float.MaxValue)
					this.CalculateLengthAtOffset();
				using(Graphics g = this.mouseArea.CreateGraphics())
				{
                    int offset = this.caretRender.SVGRenderer.MoveVertical(up, g, this.caret.Offset, this.caret.OldLength);
					if(offset >=0)
						this.caret.Offset= offset;
				}
			}
		}
		#endregion

        #region ..selection_SelectionChanged
        private void selection_SelectionChanged(object sender, EventArgs e)
		{
			this.InvalidateRegion(this.selection.OldOffset,this.selection.OldLength);
			this.InvalidateRegion(this.selection.Offset,this.selection.Length);
		}
		#endregion

		#region ..刷新区域
		void InvalidateRegion(int offset,int length)
		{
			if(length == 0)
				return;
			if(EditText)
			{
				using(System.Drawing.Graphics g = this.mouseArea.CreateGraphics())
				{
                    RectangleF[] rects = this.caretRender.SVGRenderer.GetRegion(offset, length, g);
					if(rects != null && rects.Length > 0)
					{
						using(System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath())
						{
							using(System.Drawing.Drawing2D.Matrix matrix = new Matrix())
							{
                                matrix.Multiply(this.caretRender.SVGRenderer.GetLabelTransform());
                                matrix.Multiply(this.mouseArea.CoordTransform, MatrixOrder.Append);
								for(int i = 0;i<rects.Length;i++)
								{
									RectangleF rect = rects[i];
									rect = new RectangleF(rect.X - 1,rect.Y -1,rect.Width + 2,rect.Height + 2);
									path.AddRectangle(rect);
								}
								path.FillMode = FillMode.Winding;
								path.Transform(matrix);
								using(System.Drawing.Region rg = new Region(path))
									this.mouseArea.Invalidate(rg);
							}
						}
					}
				}
			}
		}
		#endregion

		#region ..清除选区
		internal void ClearSelect()
		{
			if(!this.selection.IsEmpty)
			{
				this.RemoveString(this.selection.Offset,this.selection.Length);
				this.selection.AdaptSelection(this.selection.Offset,0);
                this.caret.Offset = this.selection.Offset;
			}
		}
		#endregion

		#region ..Copy
		internal void Copy()
		{
			if(EditText && !this.selection.IsEmpty)
			{
				string temp = this.caretRender.Label.Substring(this.selection.Offset,this.selection.Length);
				DataObject o = new DataObject(DataFormats.Text,temp);
				System.Windows.Forms.Clipboard.SetDataObject(o);
			}
		}
		#endregion

		#region ..Paste
		internal void Paste()
		{
			if(EditText)
			{
				IDataObject o = System.Windows.Forms.Clipboard.GetDataObject();
				if(o.GetDataPresent(DataFormats.Text))
				{
					string text = o.GetData(DataFormats.Text).ToString();
					this.Insert(this.caret.Offset,text);
				}
			}
		}
		#endregion

		#region ..全选
		internal void SelectAll()
		{
			if(EditText)
			{
				this.caret.Offset = 0;
				this.selection.AdaptSelection(0,this.caretRender.Label.Length);
			}
		}
		#endregion

        #region ..SelectNone
        internal void SelectNone()
		{
			if(EditText)
				this.selection.AdaptSelection(this.caret.Offset,0);
		}
		#endregion

        #region ..ExecuteBehaviorPresent
        internal override bool ExecuteBehaviorPresent(Behavior behavior)
		{
			if(this.caretRender == null)
				return base.ExecuteBehaviorPresent(behavior);

			bool a = true;
			switch(behavior)
			{
				case Behavior.AdjustLayer:
					a = false;
					break;
				case Behavior.AlignElements:
					a = false;
					break;
				case Behavior.Distriute:
					a = false;
					break;
				case Behavior.Group:
					a = false;
					break;
				case Behavior.UnGroup:
					a = false;
					break;
				case Behavior.AdjustSize:
					a = false;
					break;
				case Behavior.SelectNone:
					a = true;
					break;
				case Behavior.Copy:
				case Behavior.Cut:
					a = !this.selection.IsEmpty;
					break;
				case Behavior.Paste:
					IDataObject data = System.Windows.Forms.Clipboard.GetDataObject();
					a = data.GetDataPresent(DataFormats.Text);
					break;
				case Behavior.Undo:
					a = true;
					break;
				case Behavior.Redo:
					a = true;
					break;
				case Behavior.Delete:
					a = true;
					break;
				case Behavior.SelectAll:
					a = true;
					break;
				case Behavior.Transform:
					a = false;
					break;
			}
			return a;
		}
		#endregion

		#region ..InvokeUndo
		internal void InvokeUndo()
		{
			if(this.undos.Count > 0)
			{
				bool old = this.mouseArea.SVGDocument.AcceptNodeChanged;
				this.mouseArea.SVGDocument.AcceptNodeChanged = true;
				CaretUndo undo = new CaretUndo(this,this.caret.Offset);
				SVG.Interface.IUndoOperation[] list = new SVG.Interface.IUndoOperation[this.undos.Count];
				this.undos.CopyTo(list);
				this.mouseArea.SVGDocument.PushUndos(list);
				this.mouseArea.SVGDocument.PushUndo(undo);
				this.mouseArea.SVGDocument.InvokeUndos();
				this.mouseArea.SVGDocument.AcceptNodeChanged = old;
			}
			this.undos.Clear();
		}
		#endregion

        #region ..ChangeToTransform
        void ChangeToTransform()
		{
            this.mouseArea.ChangeToTransform();
            this.mouseArea.InvalidateSelection();
		}
		#endregion

		#region ..PointToView
		/// <summary>
		/// Convert the point to the view screen
		/// </summary>
		/// <param name="p"></param>
		/// <returns></returns>
		PointF PointToView(PointF p)
		{
			using(Matrix matrix = new Matrix())
			{
				if(this.TotalTransform != null)
					matrix.Multiply(this.TotalTransform);
				matrix.Invert();
				PointF[] ps = new PointF[]{p};
				matrix.TransformPoints(ps);
				p = ps[0];
			}
			return p;
		}
		#endregion

        #region ..Reset
        internal override void Reset()
        {
            this.ChangeToTransform();
            base.Reset();
        }
        #endregion

        #region ..SVGDocument_NodeRemoved
        /// <summary>
        /// 对象被删除后，返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SVGDocument_NodeRemoved(object sender, System.Xml.XmlNodeChangedEventArgs e)
        {
            if (e.Node is SVG.SVGElement)
            {
                if (this.caretRender != null && !this.caretRender.InDom())
                    this.Reset();
            }
        }
        #endregion
    }
}
