using System;

namespace YP.Base.CSS
{
	/// <summary>
	/// Enum 的摘要说明。
	/// </summary>
	public class Enum
	{
		internal Enum()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		#region ..定义CSS角度类型
		/// <summary>
		/// 定义CSS角度类型
		/// </summary>
		internal enum CSSAngleType
		{
			/// <summary>
			/// 度
			/// </summary>
			Degree,

			/// <summary>
			/// 梯度
			/// </summary>
			Grad,

			/// <summary>
			/// 弧度
			/// </summary>
			Rad,

			/// <summary>
			/// 未指定
			/// </summary>
			UnExpected
		}
		#endregion

		#region ..定义时间类型
		/// <summary>
		/// 定义时间类型
		/// </summary>
		internal enum CSSTimeType
		{
			/// <summary>
			/// 秒
			/// </summary>
			CSS_CSSTimeType_S,

			/// <summary>
			/// 毫秒
			/// </summary>
			CSS_CSSTimeType_MS
		}
		#endregion

		#region ..定义CSS频率类型
		internal enum FrequenceType
		{
			/// <summary>
			/// 赫兹
			/// </summary>
			CSS_CSSFrequenceType_HZ,

			/// <summary>
			/// 千赫
			/// </summary>
			CSS_CSSFrequenceType_KHZ
		}
		#endregion

		#region ..定义CSS长度类别
		/// <summary>
		/// 定义CSS长度类别
		/// </summary>
		internal enum CSSLengthType
		{
			CSSLengthType_EMS,
			CSSLengthType_EXS,
			CSSLengthType_PX,
			CSSLengthType_CM,
			CSSLengthType_MM,
			CSSLengthType_IN,
			CSSLengthType_PT,
			CSSLengthType_PC,
		}
		#endregion

		#region ..定义CSS规则类别
		internal enum CSSRuleType
		{
			UNKNOWN_RULE = 0,
			STYLE_RULE = 1,
			CHARSET_RULE = 2,
			IMPORT_RULE = 3,
			MEDIA_RULE = 4,
			PAGE_RULE = 5,
		}
		#endregion
	}
}
