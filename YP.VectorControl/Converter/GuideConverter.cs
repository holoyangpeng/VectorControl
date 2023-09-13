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
	/// GuideConverter 的摘要说明。
	/// </summary>
	internal class GuideConverter:System.ComponentModel.TypeConverter
	{
		#region ..构造及消除
		public GuideConverter()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..PropertyDescriptorCollection
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (value is Guide)
            {
                PropertyDescriptorCollection collection1 = TypeDescriptor.GetProperties(typeof(Guide), attributes);
                string[] textArray1 = new string[] { "Visible", "Lock", "Color", "Snap" };
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
			object obj1 = propertyValues["Visible"];
			object obj2 = propertyValues["Lock"];
			//object obj3 = propertyValues["Snap"];
			object obj4 = propertyValues["Color"];
			
			if (obj1 == null)
			{
				obj1 = true;
			}
			if (obj2 == null)
			{
				obj2 = false;
			}
            //if (obj3 == null)
            //{
            //    obj3 = false;
            //}
			if (obj4 == null)
			{
				obj4 = System.Drawing.Color.LightGray;
			}

			Guide guide = new Guide((bool)obj1,(bool)obj2,(Color)obj4);
			
			return guide;
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
			
			bool visible = true;
			bool islock = false;//new Size(10,10);
			Color color = Color.Blue;
			if(textArray1.Length > 1)
			{
				visible = bool.Parse(textArray1[0]);
			}
			if(textArray1.Length > 2)
			{
				islock = bool.Parse(textArray1[1]);
			}
			if(textArray1.Length > 3)
			{
				System.Drawing.ColorConverter cc = new System.Drawing.ColorConverter();
				color = (Color)cc.ConvertFrom(context,culture,textArray1[2]);
			}
            //if(textArray1.Length > 4)
            //{
            //    snap = bool.Parse(textArray1[3]);
            //}
			return new Guide(visible,islock,color);
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
                value = new Guide();
			if (destinationType == typeof(string))
			{
				Guide grid1 = (Guide) value;
				if (culture == null)
				{
					culture = CultureInfo.CurrentCulture;
				}
				string text1 = culture.TextInfo.ListSeparator + " ";
				string[] textArray1 = new string[3];
				int num2 = 0;
				textArray1[num2++] = Convert.ToString(grid1.Visible);
				textArray1[num2++] = Convert.ToString(grid1.Lock);
				System.Drawing.ColorConverter cc = new ColorConverter();
				textArray1[num2++] = cc.ConvertToString(context,culture,grid1.Color);
				string a=  string.Join(text1, textArray1);
				return a;
			}
			if ((destinationType == typeof(InstanceDescriptor)) && (value is Guide))
			{
				Guide grid1 = (Guide) value;
				int num3 = 3;
				object[] objArray1 = new object[num3];
				Type[] typeArray1 = new Type[num3];
				objArray1[0] = grid1.Visible;
				typeArray1[0] = typeof(bool);
				
				objArray1[1] = grid1.Lock;
				typeArray1[1] = typeof(bool);
				
				objArray1[2] = grid1.Color;
				typeArray1[2] = typeof(Color);
				
				ConstructorInfo info1 = typeof(Guide).GetConstructor(typeArray1);
				return new InstanceDescriptor(info1, objArray1);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}
		#endregion
	}
}
