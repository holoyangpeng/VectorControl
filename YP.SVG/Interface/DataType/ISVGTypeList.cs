using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// 定义SVG中的列表类型的一般行为
	/// </summary>
	public interface ISVGTypeList:ISVGType
	{
		/// <summary>
		/// 获取列表数目
		/// </summary>
		int NumberOfItems{get;}

		/// <summary>
		/// 清空列表项
		/// </summary>
		void Clear();

		/// <summary>
		/// 清空当前列表项，并用指定的ISVGType初始化列表
		/// </summary>
		ISVGType Initialize(ISVGType newItem);

		/// <summary>
		/// 获取索引处的值
		/// </summary>
		ISVGType GetItem(int index);

		/// <summary>
		/// 在指定的索引处插入SvgNumber项
		/// </summary>
		ISVGType InsertItemBefore(ISVGType newItem, int index);

		/// <summary>
		/// 用指定的ISVGType替换指定索引处的项
		/// </summary>
		ISVGType ReplaceItem(ISVGType newItem, int index);

		/// <summary>
		/// 移除指定索引处的项
		/// </summary>
		ISVGType RemoveItem(int index);

		/// <summary>
		/// 在列表末尾添加ISVGType项
		/// </summary>
		ISVGType AppendItem(ISVGType newItem);

		/// <summary>
		/// 确定某个元素是否在列表中
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		bool Contains(object child);

		/// <summary>
		/// 返回指定项在列表中的索引
		/// </summary>
		/// <param name="svgType"></param>
		/// <returns></returns>
		int IndexOf(Interface.DataType.ISVGType svgType);
	}
}
