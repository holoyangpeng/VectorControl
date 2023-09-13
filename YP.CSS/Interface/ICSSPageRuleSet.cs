using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// 定义@Page规则的一般行为
	/// </summary>
	public interface ICSSPageRuleSet:Interface.ICSSRuleSet
	{
		/// <summary>
		/// 获取规则内容
		/// </summary>
		Interface.ICSSRuleSetContent RuleSetContent{get;}
	}
}
