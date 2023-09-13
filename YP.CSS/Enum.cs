using System;

namespace YP.Base.CSS
{
	/// <summary>
	/// Enum ��ժҪ˵����
	/// </summary>
	public class Enum
	{
		internal Enum()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}

		#region ..����CSS�Ƕ�����
		/// <summary>
		/// ����CSS�Ƕ�����
		/// </summary>
		internal enum CSSAngleType
		{
			/// <summary>
			/// ��
			/// </summary>
			Degree,

			/// <summary>
			/// �ݶ�
			/// </summary>
			Grad,

			/// <summary>
			/// ����
			/// </summary>
			Rad,

			/// <summary>
			/// δָ��
			/// </summary>
			UnExpected
		}
		#endregion

		#region ..����ʱ������
		/// <summary>
		/// ����ʱ������
		/// </summary>
		internal enum CSSTimeType
		{
			/// <summary>
			/// ��
			/// </summary>
			CSS_CSSTimeType_S,

			/// <summary>
			/// ����
			/// </summary>
			CSS_CSSTimeType_MS
		}
		#endregion

		#region ..����CSSƵ������
		internal enum FrequenceType
		{
			/// <summary>
			/// ����
			/// </summary>
			CSS_CSSFrequenceType_HZ,

			/// <summary>
			/// ǧ��
			/// </summary>
			CSS_CSSFrequenceType_KHZ
		}
		#endregion

		#region ..����CSS�������
		/// <summary>
		/// ����CSS�������
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

		#region ..����CSS�������
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
