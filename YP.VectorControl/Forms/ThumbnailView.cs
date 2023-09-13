using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using YP.SVG;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// 实现缩略图视图控制
	/// </summary>
	[ToolboxItem(false)]
	[ComVisible(false)]
	[Browsable(false)]
	public class ThumbnailView : System.Windows.Forms.Label
	{
		#region ..Constructor
		/// <summary> 
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		internal ThumbnailView(Canvas vectorControl)
		{
			// 该调用是 Windows.Forms 窗体设计器所必需的。
			InitializeComponent();

			// TODO: 在 InitializeComponent 调用后添加任何初始化
			this.canvas = vectorControl;
            this.canvas.Document.ElementRemoved += new SVGElementChangedEventHandler(Document_ElementRemoved);
            this.canvas.Document.ElementsChanged += new CollectionChangedEventHandler(Document_ElementsChanged);
			this.canvas.ScaleChanged += new EventHandler(_vectorControl_ScaleChanged);
            this.canvas.SceneChanged += new EventHandler(_vectorControl_SceneChanged);
			SetStyle(ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw,true);
			this.Cursor = Cursors.Hand;
		}

		/// <summary> 
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
            
            this.canvas.ScaleChanged -= new EventHandler(_vectorControl_ScaleChanged);
            this.canvas.SceneChanged -= new EventHandler(_vectorControl_SceneChanged);
			base.Dispose( disposing );
		}

		#region 组件设计器生成的代码
		/// <summary> 
		/// 设计器支持所需的方法 - 不要使用代码编辑器 
		/// 修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
		#endregion

		#region ..private fields
		Canvas canvas = null;
		Matrix coordTransform = new Matrix();
		Bitmap bmp = null;
		float preScale = 1;
		bool mouseDown = false;
		bool first = false;
		PointF prePoint = PointF.Empty;
		PointF startPoint = PointF.Empty;
		PointF focusPoint = PointF.Empty;
        List<Rectangle> clipRectangles = new List<Rectangle>();
		#endregion

		#region ..const fields
		const int margin = 10;
		#endregion

		#region ..public properties
		/// <summary>
		/// gets the vectorcontrol associates with the control
		/// </summary>
		internal Canvas Canvas
		{
			get
			{
				return this.canvas;
			}
		}
		#endregion

		#region ..OnPaint
		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if(this.Size.Width <= 2 * margin || this.ClientSize.Height <= 2 * margin)
				return;

			if(this.canvas != null)
			{
				Color color = this.canvas.BackColor ;
				Size size = this.canvas.DocumentSize;
				//if the size is not empty
				if(!size.IsEmpty)
				{
					this.PaintContent(e.ClipRectangle);
					if(this.bmp != null)
						e.Graphics.DrawImageUnscaled(this.bmp,0,0,this.ClientSize.Width,this.ClientSize.Height);
					e.Graphics.ResetTransform();
					e.Graphics.MultiplyTransform(this.coordTransform);
					PointF[] ps = new PointF[]{new PointF(0,0),new PointF(this.canvas.ClientSize.Width,this.canvas.ClientSize.Height)};
					this.canvas.PointsClientToView(ps);
					e.Graphics.DrawRectangle(Pens.Red,ps[0].X,ps[0].Y,(ps[1].X - ps[0].X),(ps[1].Y - ps[0].Y) );
				}
			}

            this.clipRectangles.Clear();
		}
		#endregion

        #region ..Document_ElementsChanged
        void Document_ElementRemoved(object sender, SVGElementChangedEventArgs e)
        {
            this.InvalidateElements(new SVGElement[]{e.Element});
        }

        void Document_ElementsChanged(object sender, CollectionChangedEventArgs e)
        {
            this.InvalidateElements(e.ChangeElements);
        }
        #endregion

        #region ..InvalidateElements
        void InvalidateElements(SVGElement[] elements)
        {
            this.validContent = true;
            using (Matrix matrix = this.coordTransform.Clone())
            {
                SVGElementCollection list = new SVGElementCollection(elements);
                this.canvas.InvalidateElements(this, list, matrix);
            }
            this.bmp = null;
        }
        #endregion

        #region ..MouseEvent
        protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown (e);
			this.mouseDown = e.Button == MouseButtons.Left;
			first = true;
			this.DealMouseEvent(new Point(e.X,e.Y));
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove (e);
			this.DealMouseEvent(new Point(e.X,e.Y));
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);
			this.DealMouseEvent(new Point(e.X,e.Y));
			this.mouseDown = false;
		}
		#endregion

		#region ..DealMouseEvent
		void DealMouseEvent(Point p)
		{
			if(!this.mouseDown)
				return;
			using(Matrix coord = this.coordTransform.Clone())
			{
				coord.Invert();
				Point[] ps = new Point[]{p};
				coord.TransformPoints(ps);
				p = ps[0];

				if(this.first)
				{
					this.startPoint = this.canvas.AutoScrollPosition;	
					PointF[] ps1 = new PointF[]{new PointF(0,0),new PointF(this.canvas.ClientSize.Width,this.canvas.ClientSize.Height)};
					this.canvas.PointsClientToView(ps1);
					focusPoint = new PointF(ps1[0].X + (ps1[1].X - ps1[0].X ) / 2f,ps1[0].Y + (ps1[1].Y - ps1[0].Y) /2f);
				}

				float deltaX = p.X - this.focusPoint.X;
				float deltaY = p.Y - this.focusPoint.Y;
				deltaX = deltaX * this.canvas.ScaleRatio;
				deltaY = deltaY * this.canvas.ScaleRatio;
				
				p = Point.Ceiling(new PointF(-this.startPoint.X + deltaX,-this.startPoint.Y + deltaY));
				this.SetScrollPos(p);

				first = false;
			}
		}
		#endregion

		#region ..SetScrollPos
		void SetScrollPos(Point point)
		{
			Point p = this.canvas.AutoScrollPosition;
			if(this.canvas.ShowRule && (p.X != point.X || p.Y != point.Y))
			{
				if(p.Y != point.Y)
					this.canvas.Invalidate(new Rectangle(0,0,this.canvas.ruleLength,this.canvas.Height - SystemInformation.HorizontalScrollBarHeight));
				if(p.X != point.X)
					this.canvas.Invalidate(new Rectangle(0,0,this.canvas.Width - SystemInformation.VerticalScrollBarWidth,this.canvas.ruleLength));
			}
			Size size = this.canvas.AutoScrollMinSize;
			if(size.Width < this.canvas.Width)
				point.X = 0;
			if(size.Height < this.canvas.Height)
				point.Y = 0;
			this.canvas.AutoScrollPosition = point;
			this.canvas.scrolled = true;
		}
		#endregion

		#region ..PaintContent
        bool validContent = true;
		void PaintContent(Rectangle clipRect)
		{
			bool paint = this.bmp == null;
			paint = paint || preScale !=this.canvas.ScaleRatio;
            var doc = this.canvas.Document;
			if(!paint || !this.canvas.finishLoaded || !validContent)
            {
                this.clipRectangles.Clear();
				return;
            }
            this.validContent = false;
			if(this.bmp == null)
				this.bmp = new Bitmap(this.ClientSize.Width,this.ClientSize.Height);
			using(Graphics g = Graphics.FromImage(bmp))
			{
				Color color = this.canvas.BackColor ;
				Size size = this.canvas.DocumentSize;
				//if the size is not empty
                if (!size.IsEmpty)
                {
                    float scale = (float)(this.Width - 2 * margin) / (float)size.Width;
                    scale = (float)Math.Min(scale, (float)(this.Height - 2 * margin) / (float)size.Height);

                    //calculate the width and the height
                    int width = (int)(size.Width * scale);
                    int height = (int)(size.Height * scale);

                    //calculate the bound
                    int center = this.ClientSize.Width / 2;
                    int middle = this.ClientSize.Height / 2;
                    RectangleF viewRectangle = this.canvas.viewRectangle;
                    RectangleF rect = new RectangleF(center - width / 2f, middle - height / 2, width, height);
                   
                    this.coordTransform.Reset();

                    this.coordTransform.Translate(rect.X, rect.Y);
                    this.coordTransform.Scale(scale, scale);
                    GraphicsContainer c = g.BeginContainer();
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using (SVG.StyleContainer.StyleOperator sp = doc.CreateStyleOperator())
                    {
                        if (this.clipRectangles.Count == 0)
                            this.clipRectangles.Add(clipRect);

                        List<Rectangle> rects = new List<Rectangle>();
                        if (doc.RootElement != null)
                        {
                            //Normalize the clip rectangles
                            for (int i = 0; i < this.clipRectangles.Count; i++)
                            {
                                Rectangle rect1 = this.clipRectangles[i];
                                rect1.Intersect(clipRect);
                                if (rect1.IsEmpty)
                                    continue;
                                rects.Add(rect1);
                            }
                            this.clipRectangles.Clear();
                            if (rects.Count == 0)
                                return;
                        }

                        //当rects对象超过最大数值时，不再做过滤动作
                        if (rects.Count < Canvas.MaxInvalidateRects)
                        {
                            //Draw the previous screenshot with the offset
                            g.DrawImageUnscaledAndClipped(this.bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

                            if (rects.Count > 0)
                            {
                                g.SetClip(Rectangle.Empty);
                                sp.ClipRegion.MakeEmpty();

                                //Only invalidate the clip rectangle
                                for (int i = 0; i < rects.Count; i++)
                                {
                                    Rectangle rect1 = rects[i];
                                    g.SetClip(rect1, CombineMode.Union);
                                }

                                Region rg = g.Clip.Clone() as Region;
                                using (Matrix matrix = coordTransform.Clone())
                                {
                                    matrix.Invert();
                                    rg.Transform(matrix);
                                    sp.ClipRegion.Union(rg);
                                }
                            }
                        }
                        g.Clear(this.BackColor);

                        g.ResetTransform();
                        g.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width, rect.Height);
                        g.Transform = this.coordTransform.Clone();

                        SVGTransformableElement root = doc.CurrentScene as SVGTransformableElement;
                        if (root != null && root.SVGRenderer != null)
                            root.SVGRenderer.Draw(g, sp);
                        else if (doc.CurrentScene is SVG.Interface.ISVGContainer)
                        {
                            var childs = (doc.CurrentScene as SVG.Interface.ISVGContainer).ChildElements;
                            for (int i = 0; i < childs.Count; i++)
                            {
                                SVG.SVGTransformableElement render = childs[i] as SVG.SVGTransformableElement;
                                render.SVGRenderer.Draw(g, sp);
                            }
                        }
                    }
                    g.EndContainer(c);
                }
			}
		}
		#endregion

		#region ..OnResize
		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);
			this.bmp = null;
            this.validContent = true;
		}
		#endregion

		#region .._vectorControl_ScaleChanged
		private void _vectorControl_ScaleChanged(object sender, EventArgs e)
		{
			this.canvas.scrolled = true;
		}
		#endregion

        #region .._vectorControl_SceneChanged
        void _vectorControl_SceneChanged(object sender, EventArgs e)
        {
            this.bmp = null;
            this.validContent = true;
            this.Invalidate();
        }
        #endregion

        #region ..OnInvalidated
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            Rectangle rect = e.InvalidRect;
            if (!this.clipRectangles.Contains(rect))
                this.clipRectangles.Add(rect);
        }
        #endregion

        #region .._vectorControl_Loaded
        void _vectorControl_Loaded(object sender, EventArgs e)
        {
            this.Invalidate();
        }
        #endregion
    }
}
