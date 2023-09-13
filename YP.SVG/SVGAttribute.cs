using System;
using System.Xml;

namespace YP.SVG
{
	/// <summary>
	/// SVG文档中的XmlAttribute
	/// </summary>
	public class SVGAttribute:System.Xml.XmlAttribute		
	{
		#region ..构造及消除
		public SVGAttribute(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.ownerDocument = doc;
		}
		#endregion

		#region ..私有变量
		Document.SVGDocument ownerDocument = null;
		#endregion

		#region ..重写属性
		public override string Value
		{
			get
			{
				return base.Value;
			}
			set
			{
                if (base.Value != value)
                {
                    base.Value = value;
                }
			}
		}
		#endregion
	}
}
