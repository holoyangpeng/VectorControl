using System;
using System.Reflection;

namespace YP.SVG.Property
{
	/// <summary>
	/// ����Propertyͨ�ýӿڣ��ýӿ��������ӿؼ��������Կ��������
	/// </summary>
	public interface IProperty
	{
		/// <summary>
		/// ��ȡ������������
		/// </summary>
		string Category{get;}

		/// <summary>
		/// gets a value indicates whether the property is readonly
		/// </summary>
		bool IsReadOnly{get;}

		/// <summary>
		/// ��ȡ���Ե�Ĭ��ֵ
		/// </summary>
		object DefaultValue{get;}

		/// <summary>
		/// ��ȡ�����Ե�����
		/// </summary>
		string Description{get;}

		/// <summary>
		/// �����Ҫ�����Զ�����������ָ������������ͣ�����Ϊ��
		/// </summary>
		Type EditorType{get;}

		/// <summary>
		/// ��ȡ��ʾ�����Կ��е���������
		/// </summary>
		string Name{get;}

		/// <summary>
		/// ��ȡ�����Զ�Ӧ��Xml�����AttributeName
		/// </summary>
		string AttributeName{get;}

		/// <summary>
		/// ָ�����Ե�����
		/// </summary>
		Type PropertyType{get;}

		/// <summary>
		/// gets or sets the converter type
		/// </summary>
		string ConverterTypeName{get;set;}

		/// <summary>
		/// �����԰󶨵�����ʱ����ȡ���Ե�ֵ
		/// </summary>
		/// <param name="document">��ǰ�󶨵��ĵ�</param>
//		/// <returns></returns>
//		object GetPropertyValue();
//
//		/// <summary>
//		/// �����Ըı�ʱ����������ֵ
//		/// </summary>
//		/// <param name="value">����ֵ</param>
//		void SetPropertyValue(object value);
	}
}
