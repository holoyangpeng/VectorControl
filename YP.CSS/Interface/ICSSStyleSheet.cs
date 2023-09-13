using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// 定义一个CSS StyleSheet的一般行为，这一类StyleSheet的ContentType 通常声明为“text/css"
	/// </summary>
	public interface ICSSStyleSheet:Base.StyleSheets.IStyleSheet
	{
		/// <summary>
		/// 获取子级CSS块集合
		/// </summary>
		Interface.ICSSRuleSetList ChildRuleSets{get;}

		/// <summary>
		/// 创建一个CSSRuleSet
		/// </summary>
		/// <param name="rulesetname">CSS规则块的名称</param>
		Interface.ICSSRuleSet CreateRuleSet(string rulesetname);


		/// <summary>
		/// 获取父级的RuleSet，如果该StyleSheet通过@Import建立的话
		/// </summary>
		Interface.ICSSRuleSet ParentRule{get;}
	}
}
