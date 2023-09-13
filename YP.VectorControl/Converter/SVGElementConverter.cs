using System;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Drawing;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;


namespace YP.VectorControl.Converter
{
	/// <summary>
	/// SVGElementConverter 的摘要说明。
	/// </summary>
	internal class SVGElementConverter:System.ComponentModel.TypeConverter
	{
		#region ..PropertyDescriptorCollection
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection collection1 = TypeDescriptor.GetProperties(typeof(SVG.SVGElement), attributes);
			string[] textArray1 = new string[]{"TagName"} ;
			return collection1.Sort(textArray1);
		}
		#endregion

		#region ..override GetPropertiesSupported
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return false;
		}

		public override bool GetPropertiesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		#endregion

		#region ..override Convert
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof(string))
			{
				return false;
			}
			return base.CanConvertFrom(context, sourceType);
		}
 
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		#endregion

		#region ..override ConvertFrom
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			return null;
		}
		#endregion

		#region ..override ConvertTo
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("无效的目标类型");
			}
			if (destinationType == typeof(string))
			{
				Console.Write("fasd");
			}
			if ((destinationType == typeof(InstanceDescriptor))&& (value is SVG.SVGElement))
			{
                SVG.SVGElement svgElm = value as SVG.SVGElement;
				int num3 = 1;
				object[] objArray1 = new object[num3];
				Type[] typeArray1 = new Type[num3];
                objArray1[0] = svgElm.Name;
				typeArray1[0] = typeof(string);
				

				MemberInfo info1 = typeof(SVG.SVGElement).GetConstructor(typeArray1);
				if (info1 != null)
				{
					InstanceDescriptor a = new InstanceDescriptor(info1, objArray1);
					return a;
				}
				
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		#endregion
	}
}
