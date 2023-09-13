using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.BasicShapes
{
	/// <summary>
	/// ʵ��ֱ�߶���
	/// </summary>
	public class SVGLineElement:SVGTransformableElement,
        Interface.BasicShapes.ISVGLineElement,
        Interface.BasicShapes.ISVGBasicShape,
        Interface.ISVGPathable,
        Interface.ISVGTextBlockContainer,
        Interface.ISVGContainer,
        Interface.ILineElement
	{
		#region ..���켰����
		public SVGLineElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.x1 = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.y1 = new DataType.SVGLength("0",this,LengthDirection.Vect);
			this.x2 = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.y2 = new DataType.SVGLength("0",this,LengthDirection.Vect);
            this.render = new Render.SVGDirectRenderer(this);
		}
		#endregion

		#region ..˽�б���
		DataType.SVGLength x1,x2,y1,y2;
        float? distance = null;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ʾֱ�߶����X1���ԣ���ʼ�����꣩
		/// </summary>
        public SVG.DataType.SVGLength X1
		{
			get
			{
				return this.x1;
			}
            set
            {
                this.SetAttribute("x1", value.ValueAsString, false);
            }
		}

		public override bool FillShadow
		{
			get
			{
				return false;
			}
		}

		public override bool SupportMarker
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// ��ʾֱ�߶����Y1���ԣ���ʼ�����꣩
		/// </summary>
        public SVG.DataType.SVGLength Y1
		{
			get
			{
				return this.y1;
			}
            set
            {
                this.SetAttribute("y1", value.ValueAsString, false);
            }
		}

		/// <summary>
		/// ��ʾֱ�߶����X2���ԣ���ֹ�����꣩
		/// </summary>
        public SVG.DataType.SVGLength X2
		{
			get
			{
				return this.x2;
			}
            set
            {
                this.SetAttribute("x2", value.ValueAsString, false);
            }
		}

		/// <summary>
		/// ��ʾֱ�߶����Y2���ԣ���ֹ�����꣩
		/// </summary>
        public SVG.DataType.SVGLength Y2
		{
			get
			{
				return this.y2;
			}
            set
            {
                this.SetAttribute("y2", value.ValueAsString, false);
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
                        graphicsPath = new GraphicsPath();
                    this.graphicsPath.Reset();
                    float x1 = this.x1.Value;
                    float y1 = this.y1.Value;
                    float x2 = this.x2.Value;
                    float y2 = this.y2.Value;
                    graphicsPath.AddLine(x1, y1, x2, y2);
                    //this.CreateConnectPoint();
                    this.anchors = new PointF[] { new PointF(x1, y1), new PointF(x2, y2) };
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
					case "x1":
						this.x1 = new DataType.SVGLength(attributeValue,this,LengthDirection.Hori);
						break;
					case "y1":
						this.y1  = new DataType.SVGLength(attributeValue,this,LengthDirection.Vect);
						break;
					case "x2":
						this.x2 = new DataType.SVGLength(attributeValue,this,LengthDirection.Hori);
						break;
					case "y2":
						this.y2 = new DataType.SVGLength(attributeValue,this,LengthDirection.Vect);
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
				if(string.Compare(name,"x1") != 0 &&string.Compare(name,"x2") != 0 &&string.Compare(name,"y1") != 0 &&string.Compare(name,"y2") != 0 &&string.Compare(name,"id") != 0)
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
            if (string.Compare(attributeName,"x1")==0 ||string.Compare(attributeName,"x2") ==0 ||string.Compare(attributeName,"y1") ==0 ||string.Compare(attributeName,"y2") ==0)
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

        #region ..ILineElement
        float Interface.ILineElement.Distance
        {
            get
            {
                if (!this.distance.HasValue || this.CurrentTime != this.OwnerDocument.CurrentTime)
                {
                    PointF[] ps = new PointF[] { new PointF(x1.Value, y1.Value), new PointF(x2.Value, y2.Value) };
                    //this.TotalTransform.TransformPoints(ps);
                    this.distance = PathHelper.Distance(ps[0], ps[1]);
                }
                return this.distance.Value;
            }
        }

        PointF[] Interface.ILineElement.GetAnchorsWithDistance(float distance)
        {
            PointF[] ps = new PointF[] { new PointF(x1.Value, y1.Value), new PointF(x2.Value, y2.Value), new PointF((x1.Value + x2.Value) / 2, (y1.Value + y2.Value)/2 )};
            //this.TotalTransform.TransformPoints(ps);
            float totalDistane = PathHelper.Distance(ps[0], ps[1]);
            if(totalDistane == 0)
                return ps;
            distance = distance < 0 ? 0 : distance;
            distance = distance > totalDistane ? totalDistane : distance;
            ps[2] = new PointF(ps[0].X + distance / totalDistane * (ps[1].X - ps[0].X), ps[0].Y + distance / totalDistane * (ps[1].Y - ps[0].Y));
            
            return ps;
        }
        #endregion
    }
}
