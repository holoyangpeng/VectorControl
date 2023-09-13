using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// 定义SVG中的字符串的一般行为
	/// </summary>
	public interface ISVGString:Interface.DataType.ISVGType
	{
		/// <summary>
		/// 获取字符串值
		/// </summary>
		string Value{get;}
	}
}
