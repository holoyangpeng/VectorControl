using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.BasicShapes
{
	/// <summary>
	/// 实现圆Circle对象
	/// </summary>
    public class SVGCircleElement : SVGTransformableElement, 
        Interface.ISVGPathable, 
        Interface.BasicShapes.ISVGCircleElement, 
        Interface.BasicShapes.ISVGBasicShape,
        Interface.ISVGTextBlockContainer,
        Interface.ISVGContainer
	{
		#region ..构造及消除
		public SVGCircleElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.cx = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.cy = new DataType.SVGLength("0",this,LengthDirection.Vect);
			this.r = new DataType.SVGLength("0",this,LengthDirection.Viewport);
            this.render = new Render.SVGDirectRenderer(this);
		}
		#endregion

		#region ..私有变量
		DataType.SVGLength cx,cy,r;
        Render.SVGDirectRenderer render;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取圆心横坐标
		/// </summary>
		public DataType.SVGLength Cx
		{
			get
			{
				return this.cx;
			}
            set
            {
                this.SetAttribute("cx", value.ValueAsString, false);
            }
		}

		/// <summary>
		/// 获取圆心纵坐标
		/// </summary>
        public DataType.SVGLength Cy
		{
			get
			{
				return this.cy;
			}
            set
            {
                this.SetAttribute("cy", value.ValueAsString, false);
            }
		}

		/// <summary>
		/// 获取圆半径
		/// </summary>
		public DataType.SVGLength R
		{
			get
			{
				return this.r;
			}
            set
            {
                this.SetAttribute("r", value.ValueAsString, false);
            }
		}

        public override bool Connectable
        {
            get
            {
                return true;
            }
        }
		#endregion

        #region ..ISVGPathable
        public override Render.SVGBaseRenderer SVGRenderer
        {
            get { return this.render; }
        }

        /// <summary>
        /// 获取对象的GDI路径
        /// </summary>
        System.Drawing.Drawing2D.GraphicsPath Interface.ISVGPathable.GPath
        {
            get
            {
                if (graphicsPath == null)
                {
                    if (this.graphicsPath == null)
                        graphicsPath = new GraphicsPath();
                    this.graphicsPath.Reset();
                    float cx = this.Cx.Value;
                    float cy = this.Cy.Value;
                    float r = this.R.Value;
                    if (r > 0)
                    {
                        this.graphicsPath.AddEllipse(cx - r, cy - r, 2 * r, 2 * r);
                    }
                    this.anchors = new PointF[] { new PointF(cx, cy), new PointF(cx + r, cy) };
                }
                return graphicsPath;
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

		#region ..获取可动画属性
		/// <summary>
		/// 获取可动画属性
		/// </summary>
		/// <param name="attributeName">属性名称</param>
		/// <returns></returns>
//		public override Interface.DataType.ISVGType GetAnimatedAttribute(string attributeName)
//		{
//			switch(attributeName)
//			{
//				case "cx":
//					return this.Cx;
//				case "cy":
//					return this.Cy;
//				case "r":
//					return this.R;
//			}
//			return base.GetAnimatedAttribute(attributeName);
//		}
		#endregion

		#region ..属性操作
		/// <summary>
		/// 当属性发生修改时，更新对象属性
		/// </summary>
		/// <param name="attributeName">属性名称</param>
		/// <param name="attributeValue">属性值</param>
		public override void SetSVGAttribute(string attributeName,string attributeValue)
		{
			try
			{
				switch(attributeName)
				{
					case "cx":
						this.cx = new DataType.SVGLength(attributeValue,this,LengthDirection.Hori);
						break;
					case "cy":
						this.cy = new DataType.SVGLength(attributeValue,this,LengthDirection.Vect);
						break;
					case "r":
						this.r = new DataType.SVGLength(attributeValue,this,LengthDirection.Viewport);
						break;
					default:
						base.SetSVGAttribute(attributeName,attributeValue);
						break;
				}
			}
			catch(Exception e)
			{
				this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[]{e.Message},ExceptionLevel.Normal)); 
			}
		}
		#endregion

        #region ..Interface.BasicShapes.ISVGBasicShape
        /// <summary>
        /// 转换为路径对象
        /// </summary>
        SVG.Interface.Paths.ISVGPathElement Interface.BasicShapes.ISVGBasicShape.ConvertToPath()
		{
			YP.SVG.Paths.SVGPathElement path = (YP.SVG.Paths.SVGPathElement)this.OwnerDocument.CreateElement(this.OwnerDocument.Prefix,"path",this.OwnerDocument.NamespaceURI);
			bool old = this.OwnerDocument.AcceptNodeChanged;
			this.OwnerDocument.AcceptNodeChanged = false;
			foreach(System.Xml.XmlAttribute attribute in this.Attributes)
			{
				string name = attribute.Name;
				string valuestr = attribute.Value;
				if(string.Compare(name,"cx") != 0 &&string.Compare(name,"cy") != 0 &&string.Compare(name,"r") != 0 &&string.Compare(name,"id") != 0)
				{
					path.InternalSetAttribute(name,valuestr);
				}
			}
			string pathstr = YP.SVG.Paths.SVGPathElement.GetPathString((this as SVG.Interface.ISVGPathable).GPath);
			path.InternalSetAttribute("d",pathstr);
			this.OwnerDocument.AcceptNodeChanged = old;
			return path;
		}
		#endregion

        #region ..AttributeChangeTest
        public override AttributeChangedResult AttributeChangeTest(string attributeName)
        {
            if (string.Compare(attributeName,"cx")==0 ||string.Compare(attributeName,"cy") ==0 ||string.Compare(attributeName,"r") ==0)
                return AttributeChangedResult.VisualChanged | AttributeChangedResult.GraphicsPathChanged;
            return base.AttributeChangeTest(attributeName);
        }
        #endregion

        #region ..ISVGViewPort
        DataType.SVGViewport Interface.ISVGTextBlockContainer.Viewport
        {
            get 
            {
                return PathHelper.GetViewport(this);
            }
        }
        #endregion

        #region ..Interface.ISVGContainer
        /// <summary>
        /// 判断节点是否是有效的子级节点
        /// </summary>
        /// <param name="child">子级节点</param>
        /// <returns></returns>
        bool Interface.ISVGContainer.ValidChild(Interface.ISVGElement child)
        {
            return child is Text.SVGTextBlockElement;
        }
        #endregion
    }
}
