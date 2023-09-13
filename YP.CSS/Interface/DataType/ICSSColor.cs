using System;

namespace YP.Base.CSS.Interface.DataType
{
	/// <summary>
	/// 定义CSS中颜色的一般行为
	/// </summary>
	public interface ICSSColor
	{
		/// <summary>
		/// 获取红色分量值
		/// </summary>
		int R{get;}

		/// <summary>
		/// 获取绿色分量值
		/// </summary>
		int G{get;}

		/// <summary>
		/// 获取蓝色分量值
		/// </summary>
		int B{get;}
	}
}
