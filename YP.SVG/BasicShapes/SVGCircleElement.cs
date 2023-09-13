using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.BasicShapes
{
	/// <summary>
	/// ʵ��ԲCircle����
	/// </summary>
    public class SVGCircleElement : SVGTransformableElement, 
        Interface.ISVGPathable, 
        Interface.BasicShapes.ISVGCircleElement, 
        Interface.BasicShapes.ISVGBasicShape,
        Interface.ISVGTextBlockContainer,
        Interface.ISVGContainer
	{
		#region ..���켰����
		public SVGCircleElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.cx = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.cy = new DataType.SVGLength("0",this,LengthDirection.Vect);
			this.r = new DataType.SVGLength("0",this,LengthDirection.Viewport);
            this.render = new Render.SVGDirectRenderer(this);
		}
		#endregion

		#region ..˽�б���
		DataType.SVGLength cx,cy,r;
        Render.SVGDirectRenderer render;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡԲ�ĺ�����
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
		/// ��ȡԲ��������
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
		/// ��ȡԲ�뾶
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
        /// ��ȡ�����GDI·��
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

		#region ..��ȡ�ɶ�������
		/// <summary>
		/// ��ȡ�ɶ�������
		/// </summary>
		/// <param name="attributeName">��������</param>
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
        /// ת��Ϊ·������
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
