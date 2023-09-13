using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// 定义SVG数据类型的一般行为
	/// </summary>
	public interface ISVGType
	{
		/// <summary>
		/// 获取或设置数据的默认值
		/// </summary>
		string DefaultValue{get;}

		/// <summary>
		/// 获取类型值的文本表达
		/// </summary>
		/// <returns></returns>
		string ToString();
	}
}
