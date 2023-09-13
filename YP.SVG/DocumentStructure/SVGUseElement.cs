using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.DocumentStructure
{
	/// <summary>
	/// 实现Use对象
	/// </summary>
    public class SVGUseElement : SVGTransformableElement,
        Interface.DocumentStructure.ISVGUseElement, 
        Interface.ISVGPathable, 
        Interface.ISVGTextBlockContainer,
        Interface.ISVGContainer
	{
		#region ..构造及消除
		public SVGUseElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.href = new DataType.SVGString(string.Empty,string.Empty);//);//,this);
			this.x = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.y = new DataType.SVGLength("0",this,LengthDirection.Vect);
			this.width = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.height = new DataType.SVGLength("0",this,LengthDirection.Vect);

            this.render = new Render.SVGUseRenderer(this);
		}
		#endregion

		#region ..私有变量
		SVGElementInstance instanceroot = null;
		DataType.SVGString href ;
		DataType.SVGLength x,y,width,height;
		string prehref = string.Empty;
		bool createpath = false;
		#endregion

		#region ..公共属性
		/// <summary>
		/// determine whether use the default connected point
		/// </summary>
		public override bool UseDefaultConnectedPoint
		{
			get
			{
				
				//check whether use the default connected point
				bool useDefaultConnectPoint = this.OwnerDocument.UseDefaultConnectPoint;
				if(this.HasAttribute(createDefaultConnectPoint))
					useDefaultConnectPoint = this.GetAttribute(createDefaultConnectPoint).Trim().ToLower() != "false";
				else
				{
					YP.SVG.DocumentStructure.SVGElementInstance instance = (YP.SVG.DocumentStructure.SVGElementInstance)this.InstanceRoot;
					YP.SVG.SVGElement svgg = instance.CorrespondingElement as SVGElement  ;
					if(svgg != null && svgg.GetAttribute(createDefaultConnectPoint).Trim().Length > 0)
						useDefaultConnectPoint = svgg.GetAttribute(createDefaultConnectPoint).Trim().ToLower() != "false";
				}
				return useDefaultConnectPoint;
			}
		}

		/// <summary>
		/// gets the connect points
		/// </summary>
        public override DataType.SVGPointList BaseConnectionPoints
		{
			get
			{
                string str = this.ConnectionPointStr;
                if (str != null && str.Length > 0)
                    return new DataType.SVGPointList(str);
                return base.BaseConnectionPoints;
			}
		}

		/// <summary>
		/// 获取图片源指向
		/// </summary>
		public Interface.DataType.ISVGString Href
		{
			get
			{
				return this.href;
			}
		}

		/// <summary>
		/// 表示图片的X属性
		/// </summary>
		public DataType.SVGLength X
		{
			get
			{
				return this.x;//new DataType.SVGLength(this.GetAttribute("X"), this, LengthDirection.Hori);
			}
            set
            {
                this.SetAttribute("x", value.ValueAsString, false);
            }
		}

		/// <summary>
		/// 表示图片的Y属性
		/// </summary>
        public DataType.SVGLength Y
		{
			get
			{
				return this.y;//new DataType.SVGLength(this.GetAttribute("Y"), this,LengthDirection.Vect);
			}
            set
            {
                this.SetAttribute("y", value.ValueAsString, false);
            }
		}
		
		/// <summary>
		/// 表示图片的Width对象
		/// </summary>
        public DataType.SVGLength Width
		{
			get
			{
				return this.width ;//new DataType.SVGLength(this.GetAttribute("Width"), this,LengthDirection.Hori);
			}
            set
            {
                this.SetAttribute("width", value.ValueAsString, false);
            }
		}
		
		/// <summary>
		/// 表示图片的Height属性
		/// </summary>
        public DataType.SVGLength Height
		{
			get
			{
				return this.height;//new DataType.SVGLength(this.GetAttribute("Height"), this, LengthDirection.Vect);
			}
            set
            {
                this.SetAttribute("height", value.ValueAsString, false);
            }
		}

		/// <summary>
		/// 获取对象所对应的对象实例
		/// </summary>
		public Interface.DocumentStructure.ISVGElementInstance InstanceRoot
		{
			get
			{
				string targ = this.Href.Value;
				if(targ != this.prehref || instanceroot == null)
				{
					
					System.Xml.XmlAttribute attri = this.Attributes["href"];
					if(attri != null && attri.Prefix != "xlink")
						attri.Prefix = "xlink";
					this.prehref = targ;
					System.Xml.XmlNode refnode = this.OwnerDocument.GetReferencedNode(targ);
					if(refnode is Interface.ISVGElement)
					{
						YP.SVG.SVGElement element = refnode as YP.SVG.SVGElement;
						//判断是否重复引用
                        this.instanceroot = new SVGElementInstance((Interface.ISVGElement)refnode,this,null);
					}
				}
				return this.instanceroot;
			}
		}

		/// <summary>
		/// 当节点的Href被动画时,返回当前所对应的对象实例
		/// </summary>
		public Interface.DocumentStructure.ISVGElementInstance AnimatedInstanceRoot
		{
			get
			{
				throw new NotImplementedException("SvgUseElement.animatedInstanceRoot");
			}
		}

        /// <summary>
        /// 获取引用节点对象
        /// </summary>
        public SVGElement RefElement
        {
            get
            {
                if(InstanceRoot != null)
                    return InstanceRoot.CorrespondingElement as SVGElement;
                return null;
            }
            set
            {
                if (value == null)
                    this.InternalRemoveAttribute("xlink:href");
                else
                {
                    string id = value.ID;
                    if (id == null || id.Length == 0)
                    {
                        value.ID = Guid.NewGuid().ToString();
                        id = value.ID;
                    }
                    this.SetAttribute("xlink:href", "#" + id, false);
                }
            }
        }

        public override string ConnectionPointStr
        {
            get
            {
                if (this.HasAttribute("connectPoints"))
                    return this.GetAttribute("connectPoints");
                else
                {
                    YP.SVG.DocumentStructure.SVGElementInstance instance = (YP.SVG.DocumentStructure.SVGElementInstance)this.InstanceRoot;
                    YP.SVG.SVGElement svgg = instance.CorrespondingElement as SVGElement;
                    if (svgg != null && svgg.GetAttribute("connectPoints").Trim().Length > 0)
                        return svgg.GetAttribute("connectPoints");
                }
                return string.Empty;
            }
            set
            {
                base.ConnectionPointStr = value;
            }
        }

        /// <summary>
        /// 获取引用节点对象的ID
        /// </summary>
        public string ReferencedSymbolName
        {
            get
            {
                if (this.RefElement != null)
                    return this.RefElement.ID;
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取对象的可连接性
        /// </summary>
        public override bool Connectable
        {
            get
            {
                return true;
            }
        }
		#endregion

        #region ..ISVGPathable
        Render.SVGUseRenderer render;

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
                if (this.graphicsPath == null)
                {
                    
                    SVGElementInstance instance = (SVGElementInstance)this.InstanceRoot;
                    if (instance != null)
                    {
                        this.graphicsPath = new GraphicsPath();
                        SVGGElement svgg = instance.BackGroundDrawElement;
                        this.graphicsPath.Reset();
                        GraphicsPath path = (svgg as Interface.ISVGPathable).GPath;
                        if (path != null && path.PointCount > 0)
                            this.graphicsPath.AddPath(path, false);
                        path = null;
                        if (this.graphicsPath != null)
                            this.graphicsPath.FillMode = FillMode.Winding;
                    }
                }
                return this.graphicsPath;
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
                switch (attributeName)
                {
                    case "href":
                    case "xlink:href":
                        this.href = new DataType.SVGString(attributeValue, string.Empty);//,this);
                        this.createpath = false;
                        break;
                    case "x":
                        this.x = new DataType.SVGLength(attributeValue, this, LengthDirection.Hori);
                        break;
                    case "y":
                        this.y = new DataType.SVGLength(attributeValue, this, LengthDirection.Vect);
                        break;
                    case "width":
                        this.width = new DataType.SVGLength(attributeValue, this, LengthDirection.Hori);
                        break;
                    case "height":
                        this.height = new DataType.SVGLength(attributeValue, this, LengthDirection.Vect);
                        break;
                    default:
                        base.SetSVGAttribute(attributeName, attributeValue);
                        break;
                }
            }
            catch (Exception e)
            {
                this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e.Message }, ExceptionLevel.Normal));
            }
			
		}
		#endregion

		#region ..获取可动画属性
		/// <summary>
		/// 获取可动画属性
		/// </summary>
		/// <param name="attributeName">属性名称</param>
		/// <returns></returns>
//		public override Interface.DataType.ISVGType GetAnimatedAttribute(string attributeName)
//		{
//				switch(attributeName.Trim())
//				{
//					case "x":
//						return this.x;
//					case "y":
//						return this.y;
//					case "width":
//						return this.width;
//					case "height":
//						return this.height ;
//					case "href":
//					case "xlink:href":
//						return this.href;
//				}
//				return base.GetAnimatedAttribute(attributeName);
//		}
		#endregion

		#region ..变换
		/// <summary>
		/// gets the temp matrix
		/// </summary>
		public override Matrix GDITransform
		{
			get
			{
				if(this.CurrentTime != this.OwnerDocument.CurrentTime)
				{
					this.tempmatrix = this.Transform.FinalMatrix.GetGDIMatrix();
					this.tempmatrix.Translate(this.x.Value,this.y.Value);
				}
				return this.tempmatrix;
			}
		}
		#endregion

		#region ..获取锚点集
		/// <summary>
		/// 获取锚点集
		/// </summary>
		public override PointF[] GetAnchors()
		{
			return new PointF[0];
		}
		#endregion

		#region ..UpdateElement
        public override void UpdateElement()
		{
			if(this.instanceroot != null)
				this.instanceroot.UpdateRefElement(); 
			base.UpdateElement ();
		}
		#endregion

		#region ..UpdatePath
        public override void UpdatePath()
		{
		}
		#endregion

		#region ..Clone
		public override System.Xml.XmlNode CloneNode(bool deep)
		{
			System.Xml.XmlNode node = base.CloneNode(deep);
			YP.SVG.DocumentStructure.SVGUseElement render = node as YP.SVG.DocumentStructure.SVGUseElement;
			if(render != null)
			{
				render.createpath = this.createpath;
			}
			return node;
		}
		#endregion

        #region ..AttributeChangeTest
        public override AttributeChangedResult AttributeChangeTest(string attributeName)
        {
            if (string.Compare(attributeName,"x")==0 ||string.Compare(attributeName,"y") ==0 ||string.Compare(attributeName,"width") ==0 ||string.Compare(attributeName,"height") ==0 ||string.Compare(attributeName,"href") ==0 ||string.Compare(attributeName,"xlink:href") ==0)
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

