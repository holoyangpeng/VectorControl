using System;
using System.Collections;

namespace YP.Base.Interface
{
	/// <summary>
	/// 定义一般元素所具备的属性，方法
	/// </summary>
	public interface IWebElement
	{
		/// <summary>
		/// 获取对象所有子级节点的文本列表
		/// </summary>
		string InnerText{get;}

		/// <summary>
		/// 获取节点的BaseURI
		/// </summary>
		string BaseURI{get;}

		/// <summary>
		/// 获取指定属性的原始值
		/// </summary>
		/// <param name="name">属性名称</param>
		/// <returns></returns>
		string GetAttribute(string attributeName);
	}
}
