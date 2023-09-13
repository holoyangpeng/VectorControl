using System;

namespace YP.SVG.DocumentStructure
{
	/// <summary>
	/// SVGStyleElement ��ժҪ˵����
	/// </summary>
	public class SVGStyleElement:SVG.SVGElement//,Interface.DocumentStructure.ISVGStyleElement
	{
		#region ..Constructor
		public SVGStyleElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#endregion
		
		#region ..private fields
		Base.CSS.CSSStyleSheet styleSheet = null;
		System.Collections.ArrayList appliedElements = new System.Collections.ArrayList();
		#endregion

		#region ..˽������
		/// <summary>
		/// ��ȡStyleSheet
		/// </summary>
		Base.CSS.CSSStyleSheet StyleSheet
		{
			get
			{
				if(this.styleSheet == null)
					this.styleSheet = new Base.CSS.CSSStyleSheet(this);
				return this.styleSheet;
			}
		}
		#endregion

		#region ..��ȡStyle
		/// <summary>
		/// ƥ��ָ����SVGStyleable�ڵ�
		/// </summary>
		/// <param name="svgStyle">���ͽڵ�</param>
		/// <param name="content">CSS��������</param>
		public void MatchStyleable(SVG.SVGStyleable svgStyle,Base.CSS.CSSRuleSetContent content)
		{
			if(this.StyleSheet != null)
			{
				this.styleSheet.MatchStyleable(svgStyle,content);
				if(!this.appliedElements.Contains(svgStyle))
					this.appliedElements.Add(svgStyle);
			}
		}
		#endregion

		#region ..override properties
		public override string InnerText
		{
			get
			{
				return base.InnerText;
			}
			set
			{
				if(base.InnerText != value)
				{
					base.InnerText = value;
					RefreshStyle();
				}
			}
		}

		public override string InnerXml
		{
			get
			{
				return base.InnerXml;
			}
			set
			{
				if(base.InnerXml != value)
				{
					base.InnerXml = value;
					RefreshStyle();
				}
			}
		}
		#endregion

		#region ..RefreshStyle
		public virtual void RefreshStyle()
		{
			this.OwnerDocument.CSSStyleChanged = true;
			this.styleSheet = null;
			return;
		}
		#endregion
	}
}
