using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义单元路径列表
	/// </summary>
	public interface ISVGPathSegList:Interface.DataType.ISVGTypeList
	{
//		/// <summary>
//		/// 获取列表数目
//		/// </summary>
//		int NumberOfItems{get;}
//
//		/// <summary>
//		/// 清空列表项
//		/// </summary>
//		void Clear();

		/// <summary>
		/// 获取列表中所有单元命令链接而成的字符串
		/// </summary>
		string PathSegTypeAsString{get;}

		/// <summary>
		/// 获取路径数据的文本表达
		/// </summary>
		string PathString{get;}

//		/// <summary>
//		/// 清空当前列表项，并用指定的ISVGPathSeg初始化列表
//		/// </summary>
//		ISVGPathSeg Initialize(ISVGPathSeg newItem);
//
//		/// <summary>
//		/// 获取索引处的值
//		/// </summary>
//		ISVGPathSeg GetItem(int index);
//
//		/// <summary>
//		/// 在指定的索引处插入ISVGPathSeg项
//		/// </summary>
//		ISVGPathSeg InsertItemBefore(ISVGPathSeg newItem, int index);
//
//		/// <summary>
//		/// 用指定的ISVGPathSeg替换指定索引处的项
//		/// </summary>
//		ISVGPathSeg ReplaceItem(ISVGPathSeg newItem, int index);
//
//		/// <summary>
//		/// 移除指定索引处的项
//		/// </summary>
//		ISVGPathSeg RemoveItem(int index);
//
//		/// <summary>
//		/// 在列表末尾添加ISVGPathSeg项
//		/// </summary>
//		ISVGPathSeg AppendItem(ISVGPathSeg newItem);

		/// <summary>
		/// 获取指定项的前一项
		/// </summary>
		ISVGPathSeg PreviousSibling(ISVGPathSeg svgPathSeg);

		/// <summary>
		/// 获取指定项的后一项
		/// </summary>
		ISVGPathSeg NextSibling(ISVGPathSeg svgPathSeg);

		/// <summary>
		/// 获取正常化转换之后的列表
		/// </summary>
		ISVGPathSegList NormalSVGPathSegList{get;}
	}
}
