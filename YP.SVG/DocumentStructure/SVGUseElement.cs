using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.DocumentStructure
{
	/// <summary>
	/// ʵ��Use����
	/// </summary>
    public class SVGUseElement : SVGTransformableElement,
        Interface.DocumentStructure.ISVGUseElement, 
        Interface.ISVGPathable, 
        Interface.ISVGTextBlockContainer,
        Interface.ISVGContainer
	{
		#region ..���켰����
		public SVGUseElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.href = new DataType.SVGString(string.Empty,string.Empty);//);//,this);
			this.x = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.y = new DataType.SVGLength("0",this,LengthDirection.Vect);
			this.width = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.height = new DataType.SVGLength("0",this,LengthDirection.Vect);

            this.render = new Render.SVGUseRenderer(this);
		}
		#endregion

		#region ..˽�б���
		SVGElementInstance instanceroot = null;
		DataType.SVGString href ;
		DataType.SVGLength x,y,width,height;
		string prehref = string.Empty;
		bool createpath = false;
		#endregion

		#region ..��������
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
		/// ��ȡͼƬԴָ��
		/// </summary>
		public Interface.DataType.ISVGString Href
		{
			get
			{
				return this.href;
			}
		}

		/// <summary>
		/// ��ʾͼƬ��X����
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
		/// ��ʾͼƬ��Y����
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
		/// ��ʾͼƬ��Width����
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
		/// ��ʾͼƬ��Height����
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
		/// ��ȡ��������Ӧ�Ķ���ʵ��
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
						//�ж��Ƿ��ظ�����
                        this.instanceroot = new SVGElementInstance((Interface.ISVGElement)refnode,this,null);
					}
				}
				return this.instanceroot;
			}
		}

		/// <summary>
		/// ���ڵ��Href������ʱ,���ص�ǰ����Ӧ�Ķ���ʵ��
		/// </summary>
		public Interface.DocumentStructure.ISVGElementInstance AnimatedInstanceRoot
		{
			get
			{
				throw new NotImplementedException("SvgUseElement.animatedInstanceRoot");
			}
		}

        /// <summary>
        /// ��ȡ���ýڵ����
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
        /// ��ȡ���ýڵ�����ID
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
        /// ��ȡ����Ŀ�������
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
        /// ��ȡ�����GDI·��
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

		#region ..���Բ���
		/// <summary>
		/// �����Է����޸�ʱ�����¶�������
		/// </summary>
		/// <param name="attributeName">��������</param>
		/// <param name="attributeValue">����ֵ</param>
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

		#region ..��ȡ�ɶ�������
		/// <summary>
		/// ��ȡ�ɶ�������
		/// </summary>
		/// <param name="attributeName">��������</param>
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

		#region ..�任
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

		#region ..��ȡê�㼯
		/// <summary>
		/// ��ȡê�㼯
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
        /// �жϽڵ��Ƿ�����Ч���Ӽ��ڵ�
        /// </summary>
        /// <param name="child">�Ӽ��ڵ�</param>
        /// <returns></returns>
        bool Interface.ISVGContainer.ValidChild(Interface.ISVGElement child)
        {
            return child is Text.SVGTextBlockElement;
        }
        #endregion
	}
}

