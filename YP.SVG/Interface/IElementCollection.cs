using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// 定义元素集合的一般属性和方法。
	/// 多个Web元素在一起就构成了元素集合，利用IElementCollection实现对集合元素的操作
	/// </summary>
	public interface IElementCollection
	{
		/// <summary>
		/// 对集合进行添加、插入、删除、清除操作时，报告修改事件，请求处理。
		/// </summary>
		event CollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// 获取或设置索引处的对象
		/// </summary>
		SVGElement this[int index]{get;set;}

		/// <summary>
		/// 获取集合长度
		/// </summary>
		int Count{get;}

		/// <summary>
		/// 清除集合中的所有项
		/// </summary>
		void Clear();
	}
}
