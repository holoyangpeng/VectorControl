using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// 定义CSS规则块的集合操作
	/// </summary>
	public interface ICSSRuleSetList
	{
		/// <summary>
		/// 获取集合数目
		/// </summary>
		int Count{get;}

		/// <summary>
		/// 获取指定项
		/// </summary>
		/// <param name="index">索引</param>
		Interface.ICSSRuleSet GetItem(int index);

		/// <summary>
		/// 匹配指定的节点
		/// </summary>
		/// <param name="element">类型节点</param>
		/// <param name="content">规则内容</param>
		void MatchStyleable(Base.Interface.IStyleElement element,CSS.CSSRuleSetContent content);
	}
}
