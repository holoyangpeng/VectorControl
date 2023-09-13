using System;
using System.Drawing;

namespace YP.SVG.DataType
{
	/// <summary>
	/// 实现SVG中的Color操作
	/// </summary>
	public struct SVGColor:Interface.DataType.ISVGColor 
	{
		#region ..构造及消除
		public SVGColor(string colorstr,YP.SVG.SVGStyleable element,string defaultvalue)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			colorstr = colorstr.Trim();
			if(colorstr.Length == 0)
				colorstr = defaultvalue;
			this.renderstr = colorstr;
			if(string.Compare(colorstr,"currentColor")==0)
			{
				colorstr = element.GetFinalAttributValue("color");
			}

			if(colorstr.Length > 0 &&string.Compare(colorstr,"none") != 0 && !colorstr.StartsWith("url("))
			{
				rgbcolor = new RGBColor(colorstr);
			}
            else if (string.Compare(colorstr,"none") != 0 && !colorstr.StartsWith("url("))
            {
                rgbcolor = new RGBColor(Color.FromArgb(0,0,0,0));
            }
            else
                rgbcolor = null;
			this.ownerStyleElement = element;
			this.defaultValue = defaultvalue;
			this.isEmpty = false;
		}
		#endregion

		#region ..私有变量
		RGBColor rgbcolor ;
		public YP.SVG.SVGStyleable ownerStyleElement ;
		string renderstr;
		string defaultValue;
		bool isEmpty ;
		#endregion

		#region ..静态变量
		static SVGColor color = new SVGColor();
		/// <summary>
		/// 获取空的SVGColor;
		/// </summary>
		public static SVGColor Empty
		{
			get
			{
				color.isEmpty = true;
				return color;
			}
		}
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取表示绘制信息的字符串
		/// </summary>
		public string RenderStr
		{
			get
			{
				return this.renderstr;
			}
		}

		/// <summary>
		/// 获取颜色类型
		/// </summary>
		public ColorType ColorType
		{
			get
			{
				return ColorType.SVG_COLORTYPE_RGBCOLOR;
			}
		}

		/// <summary>
		/// 判断类型是否为空
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.isEmpty;
			}

		}

		/// <summary>
		/// 获取类型的默认值
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		/// <summary>
		/// 获取用RGBColor色彩空间的颜色值
		/// </summary>
		public Interface.DataType.IRgbColor RgbColor
		{
			get
			{
				return this.rgbcolor;
			}
		}

		/// <summary>
		/// 获取ICCColor颜色空间内的颜色值
		/// </summary>
		public Interface.DataType.ISVGIccColor IccColor
		{
			get
			{
				return null;
			}
		}
		#endregion

		#region ..SetColor
		public void SetRGBColor (string rgbColor)
		{
		}

		public void SetRGBColorICCColor ( string rgbColor, string iccColor )
		{
		}

		public void SetColor( ColorType colorType, string rgbColor, string iccColor )
		{
		}
		#endregion

		#region ..获取类型值的文本表达
		/// <summary>
		/// 获取类型值的文本表达
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.renderstr;
		}
		#endregion
	}
}
