using System;

namespace YP.Base.StyleSheets
{
	/// <summary>
	/// 定义StyleSheet集合的一般行为
	/// </summary>
	public interface IStyleSheetList
	{
		/// <summary>
		/// 获取集合数目
		/// </summary>
		int Count{get;}

		/// <summary>
		/// 获取或设置指定索引处的项
		/// </summary>
		StyleSheets.IStyleSheet this[int index]{set;get;}
	}
}
