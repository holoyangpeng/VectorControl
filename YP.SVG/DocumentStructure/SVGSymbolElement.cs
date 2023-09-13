using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.DocumentStructure
{
	/// <summary>
	/// SVGSymbolElement ��ժҪ˵����
	/// </summary>
	public class SVGSymbolElement:YP.SVG.SVGStyleable,
        Interface.DocumentStructure.ISVGSymbolElement,
        Interface.ISVGContainer,
        Interface.IOutlookBarPath
    {
        #region ..����
        public const string SymbolAppendModeAttributeString = "appendMode";
        public const string SymbolAppendMode_UseRef = "useRef";
        public const string SymbolAppendMode_AppendDirectly = "directAppend";
        #endregion

        #region ..���켰����
        public SVGSymbolElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#endregion

		#region ..˽�б���
		YP.SVG.SVGElementCollection childRenders = new YP.SVG.SVGElementCollection();
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ�Ӽ����ƽڵ�
		/// </summary>
		public YP.SVG.SVGElementCollection ChildElements
		{
			get
			{
				return this.childRenders;
			}
		}

		public Interface.DataType.ISVGRect ViewBox
		{
			get
			{
				return new DataType.SVGRect(GetAttribute("viewBox"));//,this);
			}
		}

		public Interface.CTS.ISVGPreserveAspectRatio PreserveAspectRatio
		{
			get
			{
				return new DataType.SVGPreserveAspectRatio(GetAttribute("preserveAspectRatio"));//,this);
			}
		}
		#endregion

		#region ..�жϽڵ��Ƿ�����Ч���Ӽ��ڵ�
		/// <summary>
		/// �жϽڵ��Ƿ�����Ч���Ӽ��ڵ�
		/// </summary>
		/// <param name="child">�Ӽ��ڵ�</param>
		/// <returns></returns>
		public bool ValidChild(Interface.ISVGElement child)
		{
			return child is SVGTransformableElement;
		}
		#endregion

		#region ..�Ի�
		/// <summary>
		/// �Ի�
		/// </summary>
		/// <param name="g"></param>
		public System.Drawing.Drawing2D.GraphicsPath DrawChilds(System.Drawing.Graphics g,YP.SVG.StyleContainer.StyleOperator sp)
		{
			System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath();
            foreach (SVGTransformableElement render in this.childRenders)
			{
				if(g != null && render.SVGRenderer != null)
                    render.SVGRenderer.Draw(g, sp);
                if (render is SVG.Interface.ISVGPathable)
                {
                    path.StartFigure();
                    GraphicsPath path1 = (render as SVG.Interface.ISVGPathable).GPath;
                    if (path1 != null && path1.PointCount > 1)
                    {
                        using (path1 = path1.Clone() as GraphicsPath)
                        {
                            path1.Transform(render.Transform.FinalMatrix.GetGDIMatrix());
                            path.AddPath(path1, false);
                        }
                    }
                }
			}
			return path;
		}
		#endregion

        #region ..����
        public void Render(System.Drawing.Graphics g)
        {
            using (Matrix matrix = new Matrix())
                this.Render(g, matrix);
        }

        public void Render(System.Drawing.Graphics g, System.Drawing.Drawing2D.Matrix matrix)
        {
            using (YP.SVG.StyleContainer.StyleOperator sp = new YP.SVG.StyleContainer.StyleOperator(true))
            {
                sp.DrawShadow = false;
                sp.BeginStyleContainer(this);

                this.OwnerDocument.EnableCache = false;
                try
                {
                    SmoothingMode mode = g.SmoothingMode;
                    GraphicsContainer c = g.BeginContainer();
                    g.SmoothingMode = mode;
                    if (this.OwnerDocument.ScaleStroke)
                        g.Transform = matrix.Clone();
                    else
                    {
                        sp.UseCoordTransform = true;
                        sp.coordTransform = matrix.Clone();
                    }

                    for (int i = 0; i < this.ChildElements.Count; i++)
                    {
                        SVGTransformableElement render = this.ChildElements[i] as SVGTransformableElement;
                        render.SVGRenderer.Draw(g, sp);
                    }
                    g.EndContainer(c);
                }
                finally
                {
                    this.OwnerDocument.EnableCache = true;
                }
                sp.EndContainer(this);
            }

            this.OwnerDocument.DrawDemoString(g);
        }
        #endregion

        #region ..IoutlookBarPath
        GraphicsPath graphicsPath = null;
        GraphicsPath Interface.IOutlookBarPath.GPath
        {
            get 
            {
                if (this.graphicsPath == null)
                {
                    this.graphicsPath = new GraphicsPath();
                    SVG.SVGElementCollection childs = this.ChildElements;
                    if (this.refedElement is DocumentStructure.SVGSymbolElement)
                        childs = (this.refedElement as DocumentStructure.SVGSymbolElement).ChildElements;
                    for (int i = 0; i < childs.Count; i++)
                    {
                        SVG.Interface.ISVGPathable render = childs[i] as SVG.Interface.ISVGPathable;
                        if ((render as SVG.Interface.ISVGPathable).GPath == null)
                            continue;
                        using (GraphicsPath path = (GraphicsPath)(render as SVG.Interface.ISVGPathable).GPath.Clone())
                        {
                            if (path == null || path.PointCount <= 1)
                                continue;
                            path.Transform((render as SVGTransformableElement).GDITransform);
                            this.graphicsPath.StartFigure();
                            this.graphicsPath.AddPath(path, false);
                        }
                    }
                    this.graphicsPath.FillMode = FillMode.Winding;
                }
                return this.graphicsPath;
            }
        }

        string Interface.IOutlookBarPath.Title
        {
            get 
            {
                return this.GetAttribute("title");
            }
        }
        #endregion

        #region ..RenderToGraphics
        /// <summary>
        /// ��Symbol��������ݻ��Ƶ�ָ�����豸������
        /// </summary>
        /// <param name="g">��Ҫ�������ݵ��豸����</param>
        public void RenderToGraphics(Graphics g)
        {
            using (Matrix matrix = new Matrix())
                this.Render(g, matrix);
        }
        #endregion

        #region ..RenderInRectangle
        /// <summary>
        /// ��Symbol��������ݻ��Ƶ�ָ�����豸�����ϣ���������������ָ���ľ��α߽���
        /// </summary>
        /// <param name="g">��Ҫ�����ڶ����ݵ��豸</param>
        /// <param name="bounds">Ҫ���Ƶľ��α߽�</param>
        public void RenderInRectangle(Graphics g, Rectangle bounds)
        {
            this.RenderInRectangle(g, bounds, true);
        }

        /// <summary>
        /// ��Symbol��������ݻ��Ƶ�ָ�����豸�����ϣ���������������ָ���ľ��α߽���
        /// </summary>
        /// <param name="g">��Ҫ�����ڶ����ݵ��豸</param>
        /// <param name="bounds">Ҫ���Ƶľ��α߽�</param>
        /// <param name="lockProportion">�Ƿ������ݺ�ȣ�����������ݻ��Զ������������οռ�</param>
        public void RenderInRectangle(Graphics g, Rectangle bounds, bool lockProportion)
        {
            if (bounds.IsEmpty)
                return;
            bounds = new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            Rectangle srect = bounds;
            using (GraphicsPath path = (this as Interface.IOutlookBarPath).GPath.Clone() as GraphicsPath)
            {
                path.Flatten();
                using (Matrix matrix = new Matrix())
                {
                    RectangleF rect = path.GetBounds();
                    if (rect.Width == 0)
                        rect.Width = srect.Width;
                    if (rect.Height == 0)
                        rect.Height = srect.Height;
                    if (!rect.IsEmpty)
                    {
                        float cx = rect.X + rect.Width / 2f;
                        float cy = rect.Y + rect.Height / 2f;
                        matrix.Translate(srect.X + srect.Width / 2f - cx, srect.Y + srect.Height / 2f - cy);
                        matrix.Translate(cx, cy);
                        float scalex = (srect.Height) / rect.Width;
                        float scaley = (srect.Height) / rect.Height;
                        if (lockProportion)
                        {
                            float scale = (float)Math.Min(scalex, scaley);
                            matrix.Scale(scale, scale);
                        }
                        else
                            matrix.Scale(scalex, scaley);
                        matrix.Translate(-cx, -cy);
                        this.Render(g, matrix);
                    }
                }
            }
        }
        #endregion

        #region ..GetContentBounds
        /// <summary>
        /// ��ȡsymbolͼԪ��ͼԪ���ݵı߽�
        /// </summary>
        /// <returns>���ݱ߽�</returns>
        public RectangleF GetContentBounds()
        {
            GraphicsPath path = (this as Interface.IOutlookBarPath).GPath;
            if (path != null && path.PointCount > 1)
                return path.GetBounds();
            return RectangleF.Empty;
        }
        #endregion
    }
}
