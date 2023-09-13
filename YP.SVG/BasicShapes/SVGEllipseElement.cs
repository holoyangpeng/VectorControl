using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.BasicShapes
{
	/// <summary>
	/// ��Բ/Բ
	/// </summary>
	public class SVGEllipseElement:SVGTransformableElement,
        Interface.BasicShapes.ISVGEllipseElement,
        Interface.BasicShapes.ISVGBasicShape,
        Interface.ISVGPathable, 
        Interface.ISVGTextBlockContainer,
        Interface.ISVGContainer
	{
		#region ..���켰����
		public SVGEllipseElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.cx = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.cy = new DataType.SVGLength("0",this,LengthDirection.Vect);
			this.rx = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.ry = new DataType.SVGLength("0",this,LengthDirection.Vect);
            render = new Render.SVGDirectRenderer(this);
		}
		#endregion

		#region ..˽�б���
		DataType.SVGLength cx,cy,rx,ry;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ʾ��Բ�����Cx����
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
		/// ��ʾ��Բ�����Cy����
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
		/// ��ʾ��Բ�����Rx����
		/// </summary>
        public DataType.SVGLength Rx
		{
			get
			{
				return this.rx;
			}
            set
            {
                this.SetAttribute("rx", value.ValueAsString, false);
            }
		}
		
		/// <summary>
		/// ��ʾ��Բ�����Ry����
		/// </summary>
        public DataType.SVGLength Ry
		{
			get
			{
				return this.ry;
			}
            set
            {
                this.SetAttribute("ry", value.ValueAsString, false);
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
        Render.SVGDirectRenderer render;

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
                    if (this.graphicsPath == null)
                        this.graphicsPath = new GraphicsPath();
                    this.graphicsPath.Reset();
                    float cx = this.Cx.Value;
                    float cy = this.Cy.Value;
                    float rx = this.Rx.Value;
                    float ry = this.Ry.Value;
                    if (rx != 0 && ry != 0)
                    {
                        graphicsPath.AddEllipse(cx - rx, cy - ry, 2 * rx, 2 * ry);
                    }
                    //this.CreateConnectPoint();
                    this.anchors = new PointF[] { new PointF(cx, cy), new PointF(cx, cy - ry), new PointF(cx + rx, cy) };
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
				switch(attributeName)
				{
					case "cx":
						this.cx = new DataType.SVGLength(attributeValue,this,LengthDirection.Hori);
						break;
					case "cy":
						this.cy = new DataType.SVGLength(attributeValue,this,LengthDirection.Vect);
						break;
					case "rx":
						this.rx = new DataType.SVGLength(attributeValue,this,LengthDirection.Hori);
						break;
					case "ry":
						this.ry = new DataType.SVGLength(attributeValue,this,LengthDirection.Vect);
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
//				case "rx":
//					return this.Rx;
//				case "ry":
//					return this.ry;
//			}
//			return base.GetAnimatedAttribute(attributeName);
//		}
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
				if(string.Compare(name,"cx") != 0 &&string.Compare(name,"cy") != 0 &&string.Compare(name,"rx") != 0 &&string.Compare(name,"ry") != 0 &&string.Compare(name,"id") != 0)
				{
					path.InternalSetAttribute(name,valuestr);
//					path.SetSVGAttribute(name,valuestr);
				}
				name = null;
				valuestr = null;
			}
			string pathstr = YP.SVG.Paths.SVGPathElement.GetPathString((this as SVG.Interface.ISVGPathable).GPath);
			path.InternalSetAttribute("d",pathstr);
//			path.SetSVGAttribute("d",pathstr);
			pathstr = null;
//			bool old1 = this.OwnerDocument.inLoadProcess;
//			this.OwnerDocument.inLoadProcess = true;
//			for(int i = 0;i<this.ChildNodes.Count;i ++)
//			{
//				YP.SVGDom.Animation.SVGAnimationElement anim = this.ChildNodes[i] as YP.SVGDom.Animation.SVGAnimationElement;
//				string name = anim.GetAttribute("attributeName").Trim();
//				if(anim != null &&string.Compare(name,"cx") != 0 &&string.Compare(name,"cy") != 0 &&string.Compare(name,"rx") != 0 &&string.Compare(name,"ry") != 0 &&string.Compare(name,"id") != 0)
//				{
//					path.InternalAppendChild(anim);
//					this.OwnerDocument.AttachAnimate(anim);
//					i --;
//				}
//			}
//			this.OwnerDocument.AttachAnimates();
//			this.OwnerDocument.inLoadProcess = old1;
			this.OwnerDocument.AcceptNodeChanged = old;
			return path;
		}
		#endregion

        #region ..AttributeChangeTest
        public override AttributeChangedResult AttributeChangeTest(string attributeName)
        {
            if (attributeName=="cx" || attributeName=="cy" || attributeName=="rx" ||attributeName=="ry")
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
