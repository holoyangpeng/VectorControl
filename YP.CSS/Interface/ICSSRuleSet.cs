using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// 实现CSS中一个单独的CSS声明块的一般行为
	/// </summary>
	public interface ICSSRuleSet
	{
		/// <summary>
		/// 获取CSS规则的类型
		/// </summary>
		byte CSSType{get;}

		/// <summary>
		/// 父级RuleSet
		/// </summary>
		Interface.ICSSRuleSet ParentRule{get;}

		/// <summary>
		/// 获取规则的文本表达
		/// </summary>
		string CSSText{get;}

		/// <summary>
		/// 获取所属的StyleSheet
		/// </summary>
		Base.StyleSheets.IStyleSheet ParentStyleSheet{get;}

		/// <summary>
		/// 匹配指定的节点
		/// </summary>
		/// <param name="element">类型节点</param>
		/// <param name="content">规则内容</param>
		void MatchStyleable(Base.Interface.IStyleElement element,CSS.CSSRuleSetContent content);
	}
}
