using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.BasicShapes
{
	/// <summary>
	/// ʵ��SVG�еĵ㼯������һ����󶼾߱���Points����
	/// </summary>
	public abstract class SVGPointsElement:SVGTransformableElement
	{
		#region ..���켰����
		public SVGPointsElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.points = new DataType.SVGPointList("");//,this);
		}
		#endregion

		#region ..˽�б���
		DataType.SVGPointList points;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ���ɶ���·���ĵ㼯
		/// </summary>
		public Interface.CTS.ISVGPointList Points
		{
			get
			{
				return this.points;
			}
		}
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
                    case "points":
                        this.points = new DataType.SVGPointList(attributeValue);//,this);
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
//			if(string.Compare(attributeName,"points")==0)
//				return this.points;
//			return base.GetAnimatedAttribute(attributeName);
//		}
		#endregion

		#region ..ת��Ϊ·������
		/// <summary>
		/// ת��Ϊ·������
		/// </summary>
        public YP.SVG.Interface.Paths.ISVGPathElement ConvertToPath()
		{
			YP.SVG.Paths.SVGPathElement path = (YP.SVG.Paths.SVGPathElement)this.OwnerDocument.CreateElement(this.OwnerDocument.Prefix,"path",this.OwnerDocument.NamespaceURI);
			bool old = this.OwnerDocument.AcceptNodeChanged;
			this.OwnerDocument.AcceptNodeChanged = false;
			foreach(System.Xml.XmlAttribute attribute in this.Attributes)
			{
				string name = attribute.Name;
				string valuestr = attribute.Value;
				if(string.Compare(name,"d") != 0 &&string.Compare(name,"id") != 0)
				{
					path.InternalSetAttribute(name,valuestr);
				}
			}
			string pathstr = YP.SVG.Paths.SVGPathElement.GetPathString((this as SVG.Interface.ISVGPathable).GPath);
			path.InternalSetAttribute("d",pathstr);
			pathstr = null;
//			bool old1 = this.OwnerDocument.inLoadProcess;
//			this.OwnerDocument.inLoadProcess = true;
//			for(int i = 0;i<this.ChildNodes.Count;i ++)
//			{
//				YP.SVGDom.Animation.SVGAnimationElement anim = this.ChildNodes[i] as YP.SVGDom.Animation.SVGAnimationElement;
//				string name = anim.GetAttribute("attributeName").Trim();
//				if(anim != null &&string.Compare(name,"d") != 0 &&string.Compare(name,"id") != 0)
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
            if (string.Compare(attributeName,"points")==0)
                return AttributeChangedResult.VisualChanged | AttributeChangedResult.GraphicsPathChanged;
            return base.AttributeChangeTest(attributeName);
        }
        #endregion
	}
}
