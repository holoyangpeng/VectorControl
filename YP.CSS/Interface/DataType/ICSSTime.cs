using System;

namespace YP.Base.CSS.Interface.DataType
{
	/// <summary>
	/// 定义CSS中时间的一般行为
	/// </summary>
	public interface ICSSTime
	{
		/// <summary>
		/// 获取时间值
		/// </summary>
		float Value{get;}

		/// <summary>
		/// 获取时间类型
		/// </summary>
		byte TimeType{get;}
	}
}
