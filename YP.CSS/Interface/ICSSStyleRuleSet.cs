using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// ICSSStyleRule 的摘要说明。
	/// </summary>
	public interface ICSSStyleRuleSet:Interface.ICSSRuleSet
	{
		/// <summary>
		/// 获取选择器的文本表达
		/// </summary>
		string SelectorText{get;set;}

		/// <summary>
		/// 获取规则内容
		/// </summary>
		Interface.ICSSRuleSetContent RuleSetContent{get;}
	}
}
