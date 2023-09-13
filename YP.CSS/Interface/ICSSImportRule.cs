using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// 定义@import类型规则的一般行为
	/// </summary>
	public interface ICSSImportRuleSet:Interface.ICSSRuleSet
	{
		/// <summary>
		/// 获取指向的StyleSheet
		/// </summary>
		Interface.ICSSStyleSheet RefStyleSheet{get;}
	}
}
