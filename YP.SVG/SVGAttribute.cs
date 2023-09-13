using System;
using System.Xml;

namespace YP.SVG
{
	/// <summary>
	/// SVG�ĵ��е�XmlAttribute
	/// </summary>
	public class SVGAttribute:System.Xml.XmlAttribute		
	{
		#region ..���켰����
		public SVGAttribute(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.ownerDocument = doc;
		}
		#endregion

		#region ..˽�б���
		Document.SVGDocument ownerDocument = null;
		#endregion

		#region ..��д����
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
