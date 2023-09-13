using System;

namespace YP.SVG.Interface.DocumentStructure
{
	/// <summary>
	/// ISVGElementInstanceList 的摘要说明。
	/// </summary>
	public interface ISVGElementInstanceList
	{
		/// <summary>
		/// 获取列表长度
		/// </summary>
		ulong Length{get;}

		/// <summary>
		/// 获取列表特定的项
		/// </summary>
		ISVGElementInstance Item ( ulong index );
	}
}
