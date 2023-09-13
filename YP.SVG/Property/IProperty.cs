using System;
using System.Reflection;

namespace YP.SVG.Property
{
	/// <summary>
	/// 定义Property通用接口，该接口用于增加控件关联属性框的属性项
	/// </summary>
	public interface IProperty
	{
		/// <summary>
		/// 获取该属性项的类别
		/// </summary>
		string Category{get;}

		/// <summary>
		/// gets a value indicates whether the property is readonly
		/// </summary>
		bool IsReadOnly{get;}

		/// <summary>
		/// 获取属性的默认值
		/// </summary>
		object DefaultValue{get;}

		/// <summary>
		/// 获取该属性的描述
		/// </summary>
		string Description{get;}

		/// <summary>
		/// 如果需要调用自定义的设计器，指定设计器的类型，否则为空
		/// </summary>
		Type EditorType{get;}

		/// <summary>
		/// 获取显示在属性框中的属性名称
		/// </summary>
		string Name{get;}

		/// <summary>
		/// 获取改属性对应的Xml对象的AttributeName
		/// </summary>
		string AttributeName{get;}

		/// <summary>
		/// 指定属性的类型
		/// </summary>
		Type PropertyType{get;}

		/// <summary>
		/// gets or sets the converter type
		/// </summary>
		string ConverterTypeName{get;set;}

		/// <summary>
		/// 当属性绑定到对象时，获取属性的值
		/// </summary>
		/// <param name="document">当前绑定的文档</param>
//		/// <returns></returns>
//		object GetPropertyValue();
//
//		/// <summary>
//		/// 当属性改变时，设置属性值
//		/// </summary>
//		/// <param name="value">属性值</param>
//		void SetPropertyValue(object value);
	}
}
