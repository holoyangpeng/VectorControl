using System;

namespace YP.SVG.DocumentStructure
{
	/// <summary>
	/// SVGStyleElement 的摘要说明。
	/// </summary>
	public class SVGStyleElement:SVG.SVGElement//,Interface.DocumentStructure.ISVGStyleElement
	{
		#region ..Constructor
		public SVGStyleElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion
		
		#region ..private fields
		Base.CSS.CSSStyleSheet styleSheet = null;
		System.Collections.ArrayList appliedElements = new System.Collections.ArrayList();
		#endregion

		#region ..私有属性
		/// <summary>
		/// 获取StyleSheet
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

		#region ..获取Style
		/// <summary>
		/// 匹配指定的SVGStyleable节点
		/// </summary>
		/// <param name="svgStyle">类型节点</param>
		/// <param name="content">CSS规则内容</param>
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
