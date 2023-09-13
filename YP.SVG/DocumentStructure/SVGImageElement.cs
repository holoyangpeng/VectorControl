using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace YP.SVG.DocumentStructure
{
	/// <summary>
	/// ʵ��Image����
	/// </summary>
    public class SVGImageElement : SVGTransformableElement, 
        Interface.ISVGPathable, 
        Interface.DocumentStructure.ISVGImageElement, 
        Interface.ISVGTextBlockContainer,
        Interface.ISVGBoundElement,
        Interface.ISVGContainer
	{
		#region ..���켰����
		public SVGImageElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.href = new DataType.SVGString("",string.Empty);//,this);
			this.x = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.y = new DataType.SVGLength("0",this,LengthDirection.Vect);
			this.width = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.height = new DataType.SVGLength("0",this,LengthDirection.Vect);

            this.render = new Render.SVGImageRenderer(this);
		}
		#endregion

		#region ..˽�б���
		System.Drawing.Bitmap imagesource ;
		DataType.SVGString href ;
		DataType.SVGLength x,y,width,height;
        DataType.SVGPreserveAspectRatio preserveAspectRatio;
		string orisource = string.Empty;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡͼƬԴָ��
		/// </summary>
		public DataType.SVGString Href
		{
			get
			{
				return this.href;
			}
            set
            {
                this.SetAttribute("href", value.Value, false);
            }
		}

		/// <summary>
		/// ��ȡͼƬԴ
		/// </summary>
		public System.Drawing.Bitmap ImageSource
		{
			get
			{
				if(this.orisource != this.href.Value)
					this.ParseImageSource(this.href.Value);
                this.orisource = this.href.Value;
				return this.imagesource;
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

		public Interface.CTS.ISVGPreserveAspectRatio PreserveAspectRatio
		{
			get
			{
				if(this.preserveAspectRatio == null)
                    this.preserveAspectRatio = new DataType.SVGPreserveAspectRatio("");//,this);
				return this.preserveAspectRatio;// new SVGPreserveAspectRatio(GetAttribute("preserveAspectRatio"));
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
        Render.SVGImageRenderer render;

        public override Render.SVGBaseRenderer SVGRenderer
        {
            get { return this.render; }
        }

        /// <summary>
        /// ��ȡ����·��
        /// </summary>
        System.Drawing.Drawing2D.GraphicsPath Interface.ISVGPathable.GPath
        {
            get
            {
                if (this.graphicsPath == null)
                {
                    float x = this.x.Value;
                    float y = this.y.Value;
                    float width = this.width.Value;
                    float height = this.height.Value;
                    if (width == 0 && this.imagesource != null)
                        width = this.imagesource.Width;
                    if (height == 0 && this.imagesource != null)
                        height = this.imagesource.Height;
                    if (this.graphicsPath == null)
                        this.graphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
                    this.graphicsPath.Reset();
                    this.graphicsPath.AddRectangle(new RectangleF(x, y, width, height));
                    this.anchors = new PointF[] { new PointF(x, y), new PointF(x + width, y), new PointF(x + width, y + height), new PointF(x, y + height) };
                    //this.CreateConnectPoint();
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
					case "href":
						this.href = new DataType.SVGString(attributeValue,string.Empty);//,this);
						this.ParseImageSource(attributeValue);
						break;
					case "xlink:href":
						this.href = new DataType.SVGString(attributeValue,string.Empty);//,this);
						this.ParseImageSource(attributeValue);
						break;
					case "x":
						this.x = new DataType.SVGLength(attributeValue,this,LengthDirection.Hori);
						break;
					case "y":
						this.y = new DataType.SVGLength(attributeValue,this,LengthDirection.Vect);
						break;
					case "width":
						this.width = new DataType.SVGLength(attributeValue,this,LengthDirection.Hori);
						break;
					case "height":
						this.height =  new DataType.SVGLength(attributeValue,this,LengthDirection.Vect);
						break;
					case "preserveAspectRatio":
						this.preserveAspectRatio = new DataType.SVGPreserveAspectRatio(attributeValue);//,this);
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

		#region ..����ͼƬԴ
		/// <summary>
		/// ����ͼƬԴ
		/// </summary>
		/// <param name="href">�ο���ַ</param>
		void ParseImageSource(string href)
		{
			if(this.HasAttribute("href"))
			{
				System.Xml.XmlAttribute attri = this.Attributes["href"];
				if(attri != null && attri.Prefix != "xlink")
					attri.Prefix = "xlink";
			}
			this.imagesource = Common.ImageHelper.ParseImageSource(this.OwnerDocument,href);
		}
		#endregion

		#region ..��ͼƬԴת��ΪBase64��Դ
		/// <summary>
		/// ��ͼƬԴת��ΪBase64��Դ
		/// </summary>
		public void ConvertImageTo64()
		{
			try
			{
				string href = this.Href.Value;
				string baseuri = this.OwnerDocument.BaseURI;
				if(!Common.ImageHelper.base64SourceParser.IsMatch(href))
				{
					Uri uri = new Uri(new Uri(baseuri),href);
					System.IO.MemoryStream stream = this.OwnerDocument.GetReferencedFile(uri) as System.IO.MemoryStream;
					if(stream != null)
					{
						string source = ConvertToBase64(stream);
						this.InternalSetAttribute("href",Document.SVGDocument.XLinkNamespace,source);
					}
					uri = null;
				}
			}
			catch
			{
				
			}
		}
		#endregion

		#region ..ConvertToBase64
		public static string ConvertToBase64(System.IO.MemoryStream stream)
		{
			if(stream != null)
			{
				byte[] bs = stream.GetBuffer();
				if(bs != null)
				{
					string source = System.Convert.ToBase64String(bs);
					if(source.Trim().Length > 0)
					{
						source = "data:image/jpg;base64," + source;
					}
					return source;
				}
			}
			return string.Empty;
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
