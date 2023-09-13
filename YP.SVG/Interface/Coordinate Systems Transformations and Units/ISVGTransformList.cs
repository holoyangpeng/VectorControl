using System;

namespace YP.SVG.Interface.CTS
{
	/// <summary>
	/// 定义一系列的SVGTransform对象，并定义其一般行为
	/// </summary>
	public interface ISVGTransformList:Interface.DataType.ISVGTypeList
	{
//		/// <summary>
//		/// 获取列表数目
//		/// </summary>
//		int NumberOfItems{get;}
//
		/// <summary>
		/// 获取最终的扭曲
		/// </summary>
		CTS.ISVGMatrix FinalMatrix{get;}
//
//		/// <summary>
//		/// 清空列表项
//		/// </summary>
//		void Clear();
//
//		/// <summary>
//		/// 清空当前列表项，并用指定的ISVGTransform初始化列表
//		/// </summary>
//		ISVGTransform Initialize(ISVGTransform newItem);
//
//		/// <summary>
//		/// 获取索引处的值
//		/// </summary>
//		ISVGTransform GetItem(int index);
//
//		/// <summary>
//		/// 在指定的索引处插入SvgNumber项
//		/// </summary>
//		ISVGTransform InsertItemBefore(ISVGTransform newItem, int index);
//
//		/// <summary>
//		/// 用指定的ISVGTransform替换指定索引处的项
//		/// </summary>
//		ISVGTransform ReplaceItem(ISVGTransform newItem, int index);
//
//		/// <summary>
//		/// 移除指定索引处的项
//		/// </summary>
//		ISVGTransform RemoveItem(int index);
//
//		/// <summary>
//		/// 在列表末尾添加ISVGTransform项
//		/// </summary>
//		ISVGTransform AppendItem(ISVGTransform newItem);

	}
}
