using System;

namespace YP.Base.CSS.Interface.DataType
{
	/// <summary>
	/// ����CSS��Ƶ�ʵ�һ����Ϊ
	/// </summary>
	public interface ICSSFrequence
	{
		/// <summary>
		/// ��ȡƵ��ֵ
		/// </summary>
		float Value{get;}

		/// <summary>
		/// ��ȡƵ������
		/// </summary>
		byte FrequenceType{get;}
	}
}
