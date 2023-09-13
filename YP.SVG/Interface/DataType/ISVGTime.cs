using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// 定义Svg总时间
	/// </summary>
	public interface ISVGTime
	{
		/// <summary>
		/// 获取时间类别
		/// </summary>
		byte SVGTimeType{get;}

		/// <summary>
		/// 获取时间值
		/// </summary>
		int Value{get;}

		/// <summary>
		/// 判断对象是否与指定的对象相等
		/// </summary>
		/// <param name="refTime">欲比较的对象</param>
		/// <returns></returns>
		bool Equal(ISVGTime refTime);
	}
}
