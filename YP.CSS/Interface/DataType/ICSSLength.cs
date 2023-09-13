using System;

namespace YP.Base.CSS.Interface.DataType
{
	/// <summary>
	/// 定义CSS中长度的一般行为
	/// </summary>
	public interface ICSSLength
	{
		/// <summary>
		/// 获取长度值
		/// </summary>
		float Value{get;}

		/// <summary>
		/// 获取长度方向
		/// </summary>
		byte Direction{get;}
	}
}
