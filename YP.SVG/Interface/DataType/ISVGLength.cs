using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// 定义ISVGLength的一般行为
	/// </summary>
	public interface ISVGLength:ISVGType
	{
		/// <summary>
		/// 获取或设置长度单位
		/// </summary>
		LengthType UnitType{get;set;}

		/// <summary>
		/// 获取用浮点数表达的角度值，度量采用用户坐标单位，更新此属性将导致ValueInSpecifiedUnits和ValueString属性同步改变
		/// </summary>
		float Value{get;set;}

		/// <summary>
		/// 获取用浮点数表达的角度值，度量类型采用本来的长度类型
		/// </summary>
		float ValueInSpecifiedUnits{get;}

		/// <summary>
		/// 获取表示长度值的字符串
		/// </summary>
		string ValueAsString{get;}

		/// <summary>
		/// 重设长度值，用指定的长度类型和指定的值
		/// </summary>
		/// <param name="unitType">长度类型</param>
		/// <param name="angleValue">浮点数，表示长度值</param>
		void NewValueSpecifiedUnits (LengthType unitType, float angleValue);
		
		/// <summary>
		/// 将长度转化为特定的长度类型
		/// </summary>
		/// <param name="unitType">需转换的长度类型</param>
		void ConvertToSpecifiedUnits (LengthType unitType);
	}
}
