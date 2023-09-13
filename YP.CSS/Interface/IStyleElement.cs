using System;
using System.Collections;

namespace YP.Base.Interface
{
	/// <summary>
	/// 定义支持CSS的Web元素所具备的一般属性和方法
	/// </summary>
	public interface IStyleElement
	{
		/// <summary>
		/// 判断对象是否符合指定xpath
		/// </summary>
		/// <param name="xpath"></param>
		/// <returns></returns>
		bool MatchXPath(string xpath);
	}
}
