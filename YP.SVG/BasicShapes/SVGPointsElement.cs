using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.BasicShapes
{
	/// <summary>
	/// 实现SVG中的点集对象，这一类对象都具备了Points属性
	/// </summary>
	public abstract class SVGPointsElement:SVGTransformableElement
	{
		#region ..构造及消除
		public SVGPointsElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.points = new DataType.SVGPointList("");//,this);
		}
		#endregion

		#region ..私有变量
		DataType.SVGPointList points;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取构成对象路径的点集
		/// </summary>
		public Interface.CTS.ISVGPointList Points
		{
			get
			{
				return this.points;
			}
		}
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

		#region ..获取可动画属性
		/// <summary>
		/// 获取可动画属性
		/// </summary>
		/// <param name="attributeName">属性名称</param>
		/// <returns></returns>
//		public override Interface.DataType.ISVGType GetAnimatedAttribute(string attributeName)
//		{
//			if(string.Compare(attributeName,"points")==0)
//				return this.points;
//			return base.GetAnimatedAttribute(attributeName);
//		}
		#endregion

		#region ..转换为路径对象
		/// <summary>
		/// 转换为路径对象
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
