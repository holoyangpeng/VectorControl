using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.DocumentStructure
{
	/// <summary>
	/// ʵ��SVG�е�G����
	/// </summary>
    public class SVGGElement : SVGTransformableElement, 
        Interface.DocumentStructure.ISVGGElement, 
        Interface.ISVGContainer, 
        Interface.ISVGPathable
        //Interface.ISVGTextBlockContainer
	{
		#region ..���켰����
        public SVGGElement(string prefix, string localname, string ns, Document.SVGDocument doc)
            : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
            
            render = new Render.SVGGroupRenderer(this);
        }
		#endregion

		#region ..˽�б���
		
		#endregion

		#region ..properties
        public override string Label
        {
            get
            {
                return this.GetAttribute("labelText");
            }
            set
            {
                this.SetAttribute("labelText", value, false);
            }
        }
		#endregion

        #region ..ISVGPathable
        Render.SVGGroupRenderer render;

        public override Render.SVGBaseRenderer SVGRenderer
        {
            get { return this.render; }
        }

        public override GraphicsPath GraphicsPathIncludingTextBlock
        {
            get
            {
                if (this.graphicsPathIncludingTextBlock == null)
                {
                    this.graphicsPathIncludingTextBlock = new GraphicsPath();
                    foreach (SVGTransformableElement child in this.ChildElements)
                    {
                        if (child.GraphicsPathIncludingTextBlock != null && child.GraphicsPathIncludingTextBlock.PointCount > 1)
                        {
                            this.graphicsPathIncludingTextBlock.StartFigure();
                            this.graphicsPathIncludingTextBlock.AddPath(child.GraphicsPathIncludingTextBlock, false);
                        }
                    }
                }
                this.graphicsPathIncludingTextBlock.FillMode = FillMode.Winding;
                return this.graphicsPathIncludingTextBlock;
            }
        }

        /// <summary>
        /// ��ȡ����·��
        /// </summary>
        System.Drawing.Drawing2D.GraphicsPath Interface.ISVGPathable.GPath
        {
            get
            {
                //when the gp is null indicates that the Draw function is not called
                //create the path
                return this.GetGPath();
            }
        }

        //Interface.ISVGRenderer Interface.ISVGPathable.Render
        //{
        //    get
        //    {
        //        return this.render;
        //    }
        //}
        #endregion

        #region ..UpdateTextBlockPath
        public override void UpdateTextBlockPath()
        {
            base.UpdateTextBlockPath();
            this.finalBounds = null;
        }
        #endregion

        #region ..CalculateFinalBounds
        public override bool CalculateFinalBounds()
        {
            if (base.CalculateFinalBounds())
            {
                RectangleF rect = this.finalBounds.Value;
                
                RectangleF bounds = this.GraphicsPathIncludingTextBlock.GetBounds();
                if (bounds.Width > 0 || bounds.Height > 0)
                {
                    float left = rect.X < bounds.X ? rect.X : bounds.X;
                    float right = rect.Right > bounds.Right ? rect.Right : bounds.Right;
                    float top = rect.Y < bounds.Y ? rect.Y : bounds.Y;
                    float bottom = rect.Bottom > bounds.Bottom ? rect.Bottom : bounds.Bottom;
                    this.finalBounds = new RectangleF(left, top, right - left, bottom - top);
                }
                return true;
            }
            return false;
        }
        #endregion

        #region ..ValidChild
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

		#region ..��ȡ�߽�
		/// <summary>
		/// ��ȡ·���߽�
		/// </summary>
		/// <param name="path">·��</param>
		/// <returns></returns>
		public static RectangleF GetBounds(GraphicsPath path)
		{
			RectangleF rect = RectangleF.Empty;
			using(GraphicsPath path1 = (GraphicsPath)path.Clone())
			{
				path1.Flatten();
				rect = path1.GetBounds();
			}
			return rect;
		}
		#endregion

        #region ..�ӽڵ�������ɾ��
        public override void childRenders_CollectionChanged(object sender, CollectionChangedEventArgs e)
        {
            base.childRenders_CollectionChanged(sender, e);
            //Load�����У���ʼ��
            if (this.OwnerDocument.inLoadProcess && e.Action == CollectionChangeAction.Insert)
            {
                this.InitializeGPath();
                return;
            }
            if (e.Action == CollectionChangeAction.Insert || e.Action == CollectionChangeAction.Remove)
            {
                this.UpdatePath(true);
                this.UpdateElement(true);
            }

        }
        #endregion

        #region ..InitializeGPath
        public void InitializeGPath()
        {
            InitializeGPath(false);
        }

        public void InitializeGPath(bool last)
        {
            
        }
        #endregion

        #region ..UpdateElementWithAttribute
        /// <summary>
        /// ��attribute�����ı�ʱ�����½ڵ��Լ��������ڵ�
        /// </summary>
        /// <param name="attributeName"></param>
        public override void UpdateElementWithAttribute(string attributeName)
        {
            base.UpdateElementWithAttribute(attributeName);

            foreach (SVGElement elm in this.ChildElements)
            {
                if (elm is SVGStyleable && !elm.HasAttribute(attributeName))
                    (elm as SVGStyleable).UpdateElementWithAttribute(attributeName);
                    
            }
        }
        #endregion

        #region ..GetGPath
        public GraphicsPath GetGPath()
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
                    if (render.GPath == null)
                        continue;
                    using (GraphicsPath path = (GraphicsPath)render.GPath.Clone())
                    {
                        if (path == null || path.PointCount <= 1)
                            continue;
                        if (render is BasicShapes.SVGBranchElement)
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
        #endregion

        #region ..ISVGViewPort
        //DataType.SVGViewport Interface.ISVGTextBlockContainer.Viewport
        //{
        //    get
        //    {
        //        return PathHelper.GetViewport(this);
        //    }
        //}
        #endregion

        #region ..OnTransformChanged
        public override void OnTransformChanged()
        {
            base.OnTransformChanged();
        }
        #endregion
    }
}
