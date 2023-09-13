using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// ����ISVGLength��һ����Ϊ
	/// </summary>
	public interface ISVGLength:ISVGType
	{
		/// <summary>
		/// ��ȡ�����ó��ȵ�λ
		/// </summary>
		LengthType UnitType{get;set;}

		/// <summary>
		/// ��ȡ�ø��������ĽǶ�ֵ�����������û����굥λ�����´����Խ�����ValueInSpecifiedUnits��ValueString����ͬ���ı�
		/// </summary>
		float Value{get;set;}

		/// <summary>
		/// ��ȡ�ø��������ĽǶ�ֵ���������Ͳ��ñ����ĳ�������
		/// </summary>
		float ValueInSpecifiedUnits{get;}

		/// <summary>
		/// ��ȡ��ʾ����ֵ���ַ���
		/// </summary>
		string ValueAsString{get;}

		/// <summary>
		/// ���賤��ֵ����ָ���ĳ������ͺ�ָ����ֵ
		/// </summary>
		/// <param name="unitType">��������</param>
		/// <param name="angleValue">����������ʾ����ֵ</param>
		void NewValueSpecifiedUnits (LengthType unitType, float angleValue);
		
		/// <summary>
		/// ������ת��Ϊ�ض��ĳ�������
		/// </summary>
		/// <param name="unitType">��ת���ĳ�������</param>
		void ConvertToSpecifiedUnits (LengthType unitType);
	}
}
