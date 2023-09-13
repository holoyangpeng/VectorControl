using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// ����Ƕȵ�һ����Ϊ
	/// </summary>
	public interface ISVGAngle:ISVGType
	{
		/// <summary>
		/// ��ȡ�Ƕȵ�����
		/// </summary>
		AngleType UnitType{get;}

		/// <summary>
		/// ��ȡ�ø��������ĽǶ�ֵ���������Ͳ��ö�
		/// </summary>
		float Value{get;set;}

		/// <summary>
		/// ��ȡ�ø��������ĽǶ�ֵ���������Ͳ��ñ����ĽǶ�����
		/// </summary>
		float ValueInSpecifiedUnits{get;set;}

		/// <summary>
		/// ��ȡ��ʾ�Ƕ�ֵ���ַ���
		/// </summary>
		string ValueAsString{get;set;}

		/// <summary>
		/// ����Ƕ�ֵ����ָ���ĽǶ����ͺ�ָ����ֵ
		/// </summary>
		/// <param name="unitType">�Ƕ�����</param>
		/// <param name="angleValue">����������ʾ�Ƕ�ֵ</param>
		void NewValueSpecifiedUnits(AngleType unitType, float angleValue);
		
		/// <summary>
		/// ���Ƕ�ת��Ϊ�ض��ĽǶ�����
		/// </summary>
		/// <param name="unitType">��ת���ĽǶ�����</param>
		void ConvertToSpecifiedUnits(AngleType unitType);
	}
}
