using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace YP.SVG.DataType
{
	/// <summary>
	/// SVGRect 的摘要说明。
	/// </summary>
	public struct SVGRect:Interface.DataType.ISVGRect
	{
		#region ..构造及消除
		public SVGRect(string rectstr)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.defaultValue = string.Empty;
			this.rect = RectangleF.Empty;
			string replacedStr = Regex.Replace(rectstr, @"(\s|,)+", ",");
			string[] tokens = replacedStr.Split(new char[]{','});
			if(tokens.Length == 4)
			{
				float x = SVGNumber.ParseNumberStr(tokens[0]);
				float y = SVGNumber.ParseNumberStr(tokens[1]);
				float width = SVGNumber.ParseNumberStr(tokens[2]);
				float height = SVGNumber.ParseNumberStr(tokens[3]);
				this.rect = new RectangleF(x,y,width,height);
			}
			tokens = null;
			replacedStr = null;
//			else
//			{
//				throw new SvgException(SvgExceptionType.SVG_INVALID_VALUE_ERR, "Invalid SvgRect value: " + str);
//			}
		}

		/// <summary>
		/// 利用RectangleF结构构造SVGRect对象
		/// </summary>
		/// <param name="oriRect"></param>
		public SVGRect(RectangleF oriRect)
		{
			this.defaultValue = string.Empty;
			this.rect = oriRect;
		}
		#endregion

		#region ..私有变量
		RectangleF rect;
		string defaultValue;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取或设置指定元素的X属性值
		/// </summary>
		public float X
		{
			get
			{
				return this.rect.X;
			}
			set
			{
				this.rect.X = value;
			}
		}

		/// <summary>
		/// 获取或设置指定元素的Y属性值
		/// </summary>
		public float Y
		{
			set
			{
				this.rect.Y = value;
			}
			get
			{
				return this.rect.Y;
			}
		}

		/// <summary>
		/// 获取或设置指定元素的Width属性值
		/// </summary>
		public float Width
		{
			get
			{
				return this.rect.Width;
			}
			set
			{
				this.rect.Width = value;
			}
		}

		/// <summary>
		/// 获取或设置指定元素的Height属性值
		/// </summary>
		public float Height
		{
			set
			{
				this.rect.Height = value;
			}
			get
			{
				return this.rect.Height;
			}
		}

		/// <summary>
		/// 获取用GDI矩形结构表达的边界
		/// </summary>
		public RectangleF GDIRect
		{
			get
			{
				return this.rect;
			}
		}

		/// <summary>
		/// 获取对象的默认值
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}

		/// <summary>
		/// 判断对象是否为空
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.rect.IsEmpty;
			}
		}
		#endregion

		#region ..获取类型值的文本表达
		/// <summary>
		/// 获取类型值的文本表达
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.X.ToString() + " " + this.Y.ToString() + " " + this.Width .ToString() + " " + this.Height.ToString();
		}
		#endregion
	}
}
