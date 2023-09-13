using System;

namespace YP.Base.CSS.Interface.DataType
{
	/// <summary>
	/// 定义CSS中频率的一般行为
	/// </summary>
	public interface ICSSFrequence
	{
		/// <summary>
		/// 获取频率值
		/// </summary>
		float Value{get;}

		/// <summary>
		/// 获取频率类型
		/// </summary>
		byte FrequenceType{get;}
	}
}
