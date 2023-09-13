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
	/// GridConverter 的摘要说明。
	/// </summary>
	internal class GridConverter:System.ComponentModel.TypeConverter
	{
		#region ..PropertyDescriptorCollection
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            if (value is Grid)
            {
                PropertyDescriptorCollection collection1 = TypeDescriptor.GetProperties(typeof(Grid), attributes);
                string[] textArray1 = new string[] { "Visible", "Size", "Color", "Snap", "DrawBorder", "GridType" };
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
			object obj2 = propertyValues["Size"];
			object obj3 = propertyValues["Color"];
            //object obj4 = propertyValues["Snap"];
			object obj5 = propertyValues["DrawBorder"];
            object obj7 = propertyValues["GridType"];
			if (obj1 == null)
			{
				obj1 = true;
			}
			if (obj2 == null)
			{
				obj2 = 10;
			}
            //if (obj4 == null)
            //{
            //    obj4 = false;
            //}
			if (obj3 == null)
			{
				obj3 = System.Drawing.Color.LightGray;
			}
			if(obj5 == null)
				obj5 = true;
            //if(obj6 == null)
            //    obj6 = true;
            if (obj7 == null)
                obj7 = GridType.Line;
            return new Grid((bool)obj1, (int)obj2, (Color)obj3, (bool)obj5, (GridType)obj7);
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
			int size = 10;
			Color color = Color.LightGray;
			bool drawBorder = true;
            //bool fillScreen = false;
            GridType gridType = GridType.Line;
			if(textArray1.Length >= 1)
			{
				visible = bool.Parse(textArray1[0]);
			}
			if(textArray1.Length >= 2)
			{
				System.Drawing.SizeConverter sc = new SizeConverter();
				size = (int)sc.ConvertTo(context,culture,textArray1[1],typeof(int));
			}
			if(textArray1.Length >= 3)
			{
				System.Drawing.ColorConverter cc = new System.Drawing.ColorConverter();
				color = (Color)cc.ConvertFrom(context,culture,textArray1[2]);
			}
            //if(textArray1.Length >= 4)
            //{
            //    snap = bool.Parse(textArray1[3]);
            //}
			if(textArray1.Length >= 4)
			{
				drawBorder = bool.Parse(textArray1[3]);
			}
			if(textArray1.Length >= 5)
			{
                if (System.Enum.IsDefined(typeof(GridType), textArray1[4]))
                    gridType = (GridType)(System.Enum.Parse(typeof(GridType), textArray1[4]));
				
			}
            //if (textArray1.Length > 7)
            //{
            //    fillScreen = bool.Parse(textArray1[5]);
            //}
            return new Grid(visible, size, color, drawBorder, gridType);
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
                value = new Grid();
			if (destinationType == typeof(string))
			{
				Grid grid1 = (Grid) value;
				if (culture == null)
				{
					culture = CultureInfo.CurrentCulture;
				}
				
				string text1 = culture.TextInfo.ListSeparator + " ";
				string[] textArray1 = new string[5];
				int num2 = 0;
				textArray1[num2++] = Convert.ToString(grid1.Visible);
                textArray1[num2++] = Convert.ToString(grid1.Size);
				System.Drawing.ColorConverter cc = new ColorConverter();
				textArray1[num2++] = cc.ConvertToString(context,culture,grid1.Color);
                //textArray1[num2++] = Convert.ToString(grid1.Snap);
				textArray1[num2 ++] = Convert.ToString(grid1.DrawBorder);
                //textArray1[num2 ++] = Convert.ToString(grid1.FillScreen);
                textArray1[num2++] = Convert.ToString(grid1.GridType);
				return string.Join(text1, textArray1);
			}
			if ((destinationType == typeof(InstanceDescriptor))&& (value is Grid))
			{

				Grid grid1 = (Grid) value;
				int num3 = 5;
				object[] objArray1 = new object[num3];
				Type[] typeArray1 = new Type[num3];
				objArray1[0] = grid1.Visible;
				typeArray1[0] = typeof(bool);

				objArray1[1] = grid1.Size;
				typeArray1[1] = typeof(int);
				objArray1[2] = grid1.Color;
				typeArray1[2] = typeof(Color);

                //objArray1[3] = grid1.Snap;
                //typeArray1[3] = typeof(bool);

				objArray1[3] = grid1.DrawBorder;
				typeArray1[3] = typeof(bool);

                objArray1[4] = grid1.GridType;
                typeArray1[4] = typeof(GridType);
				MemberInfo info1 = typeof(Grid).GetConstructor(typeArray1);
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
