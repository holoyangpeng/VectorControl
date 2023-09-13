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
	/// MarginConverter 的摘要说明。
	/// </summary>
	internal class MarginConverter:System.ComponentModel.TypeConverter
	{
		#region ..构造及消除
		public MarginConverter()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..PropertyDescriptorCollection
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (value is Margin)
            {
                PropertyDescriptorCollection collection1 = TypeDescriptor.GetProperties(typeof(Margin), attributes);
                string[] textArray1 = new string[] { "Left", "Top", "Right", "Bottom" };
                return collection1.Sort(textArray1);
            }
            return base.GetProperties(context, value, attributes);
        }
		#endregion

		#region ..override GetPropertiesSupported
		public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
		{
			return true;
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
				return true;
			}
			return base.CanConvertFrom(context, sourceType);
		}
 
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
            if (destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string))
			{
				return true;
			}
			return base.CanConvertTo(context, destinationType);
		}
		#endregion

		#region ..override CreateInstance
		public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
		{
			object obj1 = propertyValues["Left"];
			object obj2 = propertyValues["Top"];
			object obj3 = propertyValues["Right"];
			object obj4 = propertyValues["Bottom"];
			if (obj1 == null)
			{
				obj1 = 0;
			}
			if (obj2 == null)
			{
				obj2 = 0;
			}
			if (obj3 == null)
			{
				obj3 = 0;
			}
			if (obj4 == null)
			{
				obj4 = 0;
			}
			Margin margin = new Margin((int)obj1,(int)obj2,(int)obj3,(int)obj4);
			return margin;
		}
		#endregion

		#region ..override ConvertFrom
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (!(value is string))
			{
				return base.ConvertFrom(context, culture, value);
			}
			string text1 = ((string) value).Trim();
			if (text1.Length == 0)
			{
				return null;
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentCulture;
			}
			char ch1 = culture.TextInfo.ListSeparator[0];
			char[] chArray1 = new char[1] { ch1 } ;
			string[] textArray1 = text1.Split(chArray1);
			if (textArray1.Length < 1)
			{
				throw new ArgumentException("参数不对");
			}
			int left = 0,top = 0,right = 0,bottom = 0;
			if(textArray1.Length > 1)
			{
				left = int.Parse(textArray1[0]);
			}
			if(textArray1.Length > 2)
			{
				top = int.Parse(textArray1[1]);
			}
			if(textArray1.Length > 3)
			{
				right = int.Parse(textArray1[2]);
			}
			if(textArray1.Length > 4)
			{
				bottom = int.Parse(textArray1[3]);
			}
			return new Margin(left,right,top,bottom);
		}
		#endregion

		#region ..override ConvertTo
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == null)
			{
				throw new ArgumentNullException("无效的目标类型");
			}
            if (value == null)
                value = new Margin();
			if (destinationType == typeof(string))
			{
				Margin grid1 = (Margin) value;
				if (culture == null)
				{
					culture = CultureInfo.CurrentCulture;
				}
				
				string text1 = culture.TextInfo.ListSeparator + " ";
				string[] textArray1 = new string[4];
				int num2 = 0;
				textArray1[num2++] = grid1.Left.ToString();
				textArray1[num2++] = grid1.Top.ToString();
				textArray1[num2++] = grid1.Right.ToString();
				textArray1[num2++] = grid1.Bottom.ToString();
				return string.Join(text1, textArray1);
			}
			if ((destinationType == typeof(InstanceDescriptor))&& (value is Margin))
			{
				Margin grid1 = (Margin) value;
				int num3 = 4;
				object[] objArray1 = new object[num3];
				Type[] typeArray1 = new Type[num3];
				objArray1[0] = grid1.Left;
				typeArray1[0] = typeof(int);

				objArray1[1] = grid1.Top;
				typeArray1[1] = typeof(int);
				objArray1[2] = grid1.Right;
				typeArray1[2] = typeof(int);

				objArray1[3] = grid1.Bottom;
				typeArray1[3] = typeof(int);

				ConstructorInfo info1 = typeof(Margin).GetConstructor(typeArray1);
					InstanceDescriptor a = new InstanceDescriptor(info1, objArray1);
					return a;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		#endregion
	}
}
