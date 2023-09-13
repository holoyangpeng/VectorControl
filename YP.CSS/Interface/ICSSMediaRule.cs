using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// 定义@Media规则的一般行为
	/// </summary>
	public interface ICSSMediaRuleSet:Interface.ICSSRuleSet
	{
		/// <summary>
		/// 获取子级的规则列表
		/// </summary>
		Interface.ICSSRuleSetList ChildRuleSets{get;}
	}
}
