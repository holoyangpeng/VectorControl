using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using YP.SVG.DataType;

using System.Text.RegularExpressions;

namespace YP.SVG.Paint
{
	/// <summary>
	/// 实现画刷通用属性和方法
	/// </summary>
	public struct SVGPaint:Interface.Paint.ISVGPaint
	{
		#region ..构造及消除
		static SVGPaint()
		{
			paint = new SVGPaint();
		}

		public SVGPaint(string str,YP.SVG.SVGStyleable element,string defaultvalue)
		{
			this.uri = string.Empty;
			this.defaultvalue = defaultvalue;
			rgbcolor = null;
			this.renderstr = str;
			this.state = 0;
			this.ownerStyleElement = element;
			if(str == null)
			{
				paintType = (ulong)SVG.PaintType.SVG_PAINTTYPE_NONE;
				return;
			}
			paintType = (ulong)SVG.PaintType.SVG_PAINTTYPE_URI_RGBCOLOR;

			if(string.Compare(str,"currentColor")==0)
			{
				paintType = (ulong)SVG.PaintType.SVG_PAINTTYPE_CURRENTCOLOR;
			}
			else if(string.Compare(str ,"none") == 0)
			{
				paintType = (ulong)SVG.PaintType.SVG_PAINTTYPE_NONE;
			}
			else if(str.StartsWith("url("))
			{
				paintType = (ulong)SVG.PaintType.SVG_PAINTTYPE_URI;
				int index = str.IndexOf(")");
				if(index>=0)
				{
					uri = str.Substring(4, str.IndexOf(")")-4);
				}
				// TODO: check for fallbacks
			}
			else
			{
				// TODO: create a regex to validate this
				paintType = (ulong)SVG.PaintType.SVG_PAINTTYPE_RGBCOLOR;
				if(str != null)
				{
					str = str.Trim();
					if(str.Length == 0)
						str = defaultvalue;
					if(string.Compare(str,"currentColor") == 0 && element != null)
						str = element.GetFinalAttributValue("color");

					if(str.Length > 0 && string.Compare(str,"none")!= 0 && !str.StartsWith("url("))
						rgbcolor = new RGBColor(str);
				}	
			}
			this.state = 1;
		}
		#endregion

		#region ..私有变量
		ulong paintType;
		string uri;
		string defaultvalue;
		int state;
		RGBColor rgbcolor ;
		public YP.SVG.SVGStyleable ownerStyleElement ;
		string renderstr;
		#endregion

		#region ..静态变量
		static SVGPaint paint ;

		/// <summary>;
		/// 判断对象是否为空
		/// </summary>
		public static SVGPaint Empty
		{
			get
			{
				return paint;
			}
		}
		#endregion

		#region ..ISVGColor方法
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
				return this.state == 0;
			}

		}

		/// <summary>
		/// 获取类型的默认值
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return this.defaultvalue;
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

		#region ..公共属性
		/// <summary>
		/// 获取绘制类别
		/// </summary>
		public ulong PaintType
		{
			get
			{
				return this.paintType;
			}
		}

		public string Uri
		{
			get
			{
				return this.uri;
			}
		}
		#endregion

		#region ..公共方法
		public void SetUri (string uri)
		{
			this.uri = uri;
		}

		public void SetPaint (PaintType paintType, string uri, string rgbColor, string iccColor )
		{
			throw new NotImplementedException();
		}
		#endregion

		#region ..获取对象的文本表达
		public override string ToString()
		{
			return this.renderstr;
		}

		#endregion
	}
}
