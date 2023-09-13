using System;
using System.Drawing;
using System.Reflection;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace YP.SVG.BasicShapes
{
	/// <summary>
	/// ����
	/// </summary>
    public class SVGRectElement : SVGTransformableElement, 
        Interface.BasicShapes.ISVGRectElement, 
        Interface.ISVGPathable, 
        Interface.BasicShapes.ISVGBasicShape, 
        Interface.ISVGBoundElement, 
        Interface.ISVGTextBlockContainer,
        Interface.ISVGContainer
	{
		#region ..���켰����
		public SVGRectElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.x = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.y = new DataType.SVGLength("0",this,LengthDirection.Vect);
			this.rx = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.ry = new DataType.SVGLength("0",this,LengthDirection.Vect);
			this.width = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.height = new DataType.SVGLength("0",this,LengthDirection.Vect);
            this.render = new Render.SVGDirectRenderer(this);
		}
		#endregion

		#region ..˽�б���
		DataType.SVGLength x,y,width,height,rx,ry;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ʾ���ζ����X����
		/// </summary>
		public DataType.SVGLength X
		{
			get
			{
				return this.x;
			}
            set
            {
                this.SetAttribute("x", value.ValueAsString, false);
            }
		}

		/// <summary>
		/// ��ʾ���ζ����Y����
		/// </summary>
        public DataType.SVGLength Y
		{
			get
			{
				return this.y;
			}
            set
            {
                this.SetAttribute("y", value.ValueAsString, false);
            }
		}

		/// <summary>
		/// ��ʾ���ζ����Rx����
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
		/// ��ʾ���ζ����Ry����
		/// </summary>
		[Category("��״")]
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
		/// ��ʾ���ζ����Width����
		/// </summary>
        public DataType.SVGLength Width
		{
			get
			{
				return this.width;
			}
            set
            {
                this.SetAttribute("width", value.ValueAsString, false);
            }
		}
		
		/// <summary>
		/// ��ʾ���ζ����Height����
		/// </summary>
		public DataType.SVGLength Height
		{
			get
			{
				return this.height;
			}
            set
            {
                this.SetAttribute("height", value.ValueAsString, false);
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
                if (graphicsPath == null)// || ((this.x.HasAnimate || this.y.HasAnimate || this.width.HasAnimate || this.height .HasAnimate || this.rx.HasAnimate || this.ry.HasAnimate) && this.CurrentTime != this.OwnerDocument.CurrentTime))
                {
                    if (this.graphicsPath == null)
                        graphicsPath = new GraphicsPath();
                    this.graphicsPath.Reset();
                    graphicsPath.StartFigure();
                    float rx = this.Rx.Value;
                    float ry = this.Ry.Value;
                    float x = this.x.Value;
                    float y = this.y.Value;
                    float width = this.width.Value;
                    float height = this.height.Value;
                    RectangleF rect = new RectangleF(x, y, width, height);
                    if (rx == 0F && ry == 0F)
                    {
                        graphicsPath.AddRectangle(rect);
                    }
                    else
                    {
                        if (rx == 0F) rx = ry;
                        else if (ry == 0F) ry = rx;

                        rx = Math.Min(rect.Width / 2, rx);
                        ry = Math.Min(rect.Height / 2, ry);

                        float a = rect.X + rect.Width - rx;
                        graphicsPath.AddLine(rect.X + rx, rect.Y, a, rect.Y);
                        graphicsPath.AddArc(a - rx, rect.Y, rx * 2, ry * 2, 270, 90);

                        float right = rect.X + rect.Width;	// rightmost X
                        float b = rect.Y + rect.Height - ry;

                        graphicsPath.AddLine(right, rect.Y + ry, right, b);
                        graphicsPath.AddArc(right - rx * 2, b - ry, rx * 2, ry * 2, 0, 90);

                        graphicsPath.AddLine(right - rx, rect.Y + rect.Height, rect.X + rx, rect.Y + rect.Height);
                        graphicsPath.AddArc(rect.X, b - ry, rx * 2, ry * 2, 90, 90);

                        graphicsPath.AddLine(rect.X, b, rect.X, rect.Y + ry);
                        graphicsPath.AddArc(rect.X, rect.Y, rx * 2, ry * 2, 180, 90);

                        graphicsPath.CloseFigure();
                    }
                    //this.CreateConnectPoint();
                    
                    this.anchors = new PointF[] { new PointF(x, y), new PointF(x + width, y), new PointF(x + width, y + height), new PointF(x, y + height), new PointF(x + width - rx, y), new PointF(x + width, y + ry) };
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
					case "x":
						this.x = new DataType.SVGLength(attributeValue,this,LengthDirection.Hori);
						break;
					case "y":
						this.y = new DataType.SVGLength(attributeValue,this,LengthDirection.Vect);
						break;
					case "rx":
						this.rx = new DataType.SVGLength(attributeValue,this,LengthDirection.Hori);
						break;
					case "ry":
						this.ry = new DataType.SVGLength(attributeValue,this,LengthDirection.Vect);
						break;
					case "width":
						this.width = new DataType.SVGLength(attributeValue,this,LengthDirection.Hori);
						break;
					case "height":
						this.height = new DataType.SVGLength(attributeValue,this,LengthDirection.Vect);
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
//				case "x":
//					return this.X;
//				case "y":
//					return this.Y;
//				case "width":
//					return this.Width;
//				case "height":
//					return this.Height;
//				case "rx":
//					return this.rx;
//				case "ry":
//					return this.ry;
//			}
//			return base.GetAnimatedAttribute(attributeName);
//		}
		#endregion

		#region ..ת��Ϊ·������
		/// <summary>
		/// ת��Ϊ·������
		/// </summary>
		public SVG.Interface.Paths.ISVGPathElement ConvertToPath()
		{
			SVG.Paths.SVGPathElement path = (SVG.Paths.SVGPathElement)this.OwnerDocument.CreateElement(this.OwnerDocument.Prefix,"path",this.OwnerDocument.NamespaceURI);
			bool old = this.OwnerDocument.AcceptNodeChanged;
			this.OwnerDocument.AcceptNodeChanged = false;
			foreach(System.Xml.XmlAttribute attribute in this.Attributes)
			{
				string name = attribute.Name;
				string valuestr = attribute.Value;
				if(string.Compare(name,"x") != 0 &&string.Compare(name,"y") != 0 &&string.Compare(name,"width") != 0 &&string.Compare(name,"height") != 0 &&string.Compare(name,"rx") != 0 &&string.Compare(name,"ry") != 0 &&string.Compare(name,"id") != 0)
				{
					path.InternalSetAttribute(name,valuestr);
				}
			}
			string pathstr = SVG.Paths.SVGPathElement.GetPathString((this as Interface.ISVGPathable).GPath);
			path.InternalSetAttribute("d",pathstr);
			this.OwnerDocument.AcceptNodeChanged = old;
			return path;
		}
		#endregion

        #region ..AttributeChangeTest
        public override AttributeChangedResult AttributeChangeTest(string attributeName)
        {
            if (string.Compare(attributeName,"x")==0  ||string.Compare(attributeName,"y") ==0
                ||string.Compare(attributeName,"rx") ==0 ||string.Compare(attributeName,"ry") ==0
                ||string.Compare(attributeName,"width") ==0 ||string.Compare(attributeName,"height") ==0)
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
