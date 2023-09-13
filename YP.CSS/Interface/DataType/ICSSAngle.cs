using System;

namespace YP.Base.CSS.Interface.DataType
{
	/// <summary>
	/// 定义CSS中角度的一般行为
	/// </summary>
	public interface ICSSAngle
	{
		/// <summary>
		/// 获取角度值
		/// </summary>
		float Value{get;}

		/// <summary>
		/// 获取角度类型
		/// </summary>
		byte AngleType{get;}
	}
}
