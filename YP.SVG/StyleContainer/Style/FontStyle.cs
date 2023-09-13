using System;
using System.Collections;

using YP.SVG.StyleContainer;

namespace YP.SVG.StyleContainer
{
	/// <summary>
	/// 实现字形类型
	/// </summary>
    public struct FontStyle
	{
		#region ..静态变量
		static FontStyle style = new FontStyle(
			new DataType.SVGString(""),
			new DataType.SVGString("Arial"),
			new DataType.SVGLength("12",null,LengthDirection.Viewport),
			new DataType.SVGNumber(0),
			new DataType.SVGString("normal"),
			new DataType.SVGString("normal"),
			new DataType.SVGString("normal"),
		new DataType.SVGString("normal")
			);

		static FontStyle empty = new FontStyle(
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGLength.Empty,
			 DataType.SVGNumber.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty
			);

		/// <summary>
		/// 获取默认对象
		/// </summary>
		public static FontStyle Default
		{
			get
			{
				return style;
			}
		}

		/// <summary>
		/// 获取空对象
		/// </summary>
		public static FontStyle Empty
		{
			get
			{
				return empty;
			}
		}
		#endregion

		#region ..构造及消除
//		public FontStyle()
//		{
//			//
//			// TODO: 在此处添加构造函数逻辑
//			//
//			this.Font = new DataType.SVGString("");
//			this.FontFamily = new DataType.SVGString("Arial");
//			this.FontSizeAdjust = new DataType.SVGNumber(0);
//			this.FontSize = new DataType.SVGLength("12",null,LengthDirection.Viewport);
//			this.FontStretch = new DataType.SVGString("normal");
//			this.FontVariant = new DataType.SVGString("normal");
//			this.FontWeigth = new DataType.SVGString("normal");
//			this.Font_Style = new DataType.SVGString("normal");
//		}

		public FontStyle(FontStyle style)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.Font = style.Font;
			this.FontFamily = style.FontFamily;
			this.FontSizeAdjust = style.FontSizeAdjust;
			this.FontSize = style.FontSize;
			this.FontStretch = style.FontStretch;
			this.FontVariant = style.FontVariant;
			this.FontWeigth = style.FontWeigth;
			this.Font_Style = style.Font_Style;
		}

		FontStyle(
			YP.SVG.DataType.SVGString FontFamily,
			YP.SVG.DataType.SVGString Font,
			YP.SVG.DataType.SVGLength FontSize,
			YP.SVG.DataType.SVGNumber FontSizeAdjust,
			YP.SVG.DataType.SVGString FontStretch,
			YP.SVG.DataType.SVGString FontVariant,
			YP.SVG.DataType.SVGString Font_Style,
			YP.SVG.DataType.SVGString FontWeigth
			)
		{
			this.Font = Font;
			this.Font_Style = Font_Style;
			this.FontFamily = FontFamily;
			this.FontSize = FontSize;
			this.FontSizeAdjust = FontSizeAdjust;
			this.FontStretch = FontStretch;
			this.FontVariant = FontVariant;
			this.FontWeigth = FontWeigth;
		}

		#endregion

		#region ..保护字段
		public YP.SVG.DataType.SVGString FontFamily;
		public YP.SVG.DataType.SVGString Font;
		public YP.SVG.DataType.SVGLength FontSize;
		public YP.SVG.DataType.SVGNumber FontSizeAdjust;
		public YP.SVG.DataType.SVGString FontStretch;
		public YP.SVG.DataType.SVGString FontVariant;
		public YP.SVG.DataType.SVGString Font_Style;
		public YP.SVG.DataType.SVGString FontWeigth;
		#endregion

		#region ..公共方法
		/// <summary>
		/// 添加填充信息
		/// </summary>
		/// <param name="svgStyle"></param>
		public FontStyle MutiplyStyle(FontStyle newStyle)
		{
//			if(newStyle == null )
//				return this;
			if(this.Font.IsEmpty)
				this.Font = (DataType.SVGString)newStyle.Font;
			else
				this.Font = TypeMultiply.Multiply(this.Font,newStyle.Font);
			if(this.FontFamily.IsEmpty)
				this.FontFamily = (DataType.SVGString)newStyle.FontFamily;
			else
				this.FontFamily = TypeMultiply.Multiply(this.FontFamily,newStyle.FontFamily);
			if(this.FontSize.IsEmpty)
				this.FontSize = (YP.SVG.DataType.SVGLength)newStyle.FontSize;
			else
				this.FontSize = TypeMultiply.Multiply((DataType.SVGLength)this.FontSize,newStyle.FontSize);
			if(this.FontSizeAdjust.IsEmpty)
				this.FontSizeAdjust = (DataType.SVGNumber)newStyle.FontSizeAdjust;
			else
				this.FontSizeAdjust = TypeMultiply.Multiply((DataType.SVGNumber)this.FontSizeAdjust,newStyle.FontSizeAdjust);
			if(this.FontStretch.IsEmpty)
				this.FontStretch = (DataType.SVGString)newStyle.FontStretch;
			else
				this.FontStretch = TypeMultiply.Multiply(this.FontStretch,newStyle.FontStretch);
			if(this.FontVariant .IsEmpty)
				this.FontVariant = (DataType.SVGString)newStyle.FontVariant;
			else
				this.FontVariant = TypeMultiply.Multiply(this.FontVariant,newStyle.FontVariant);
			if(this.Font_Style .IsEmpty)
				this.Font_Style = (DataType.SVGString)newStyle.Font_Style;
			else
				this.Font_Style = TypeMultiply.Multiply(this.Font_Style,newStyle.Font_Style);
			if(this.FontWeigth .IsEmpty)
				this.FontWeigth = (DataType.SVGString)newStyle.FontWeigth;
			else
				this.FontWeigth = TypeMultiply.Multiply(this.FontWeigth,newStyle.FontWeigth);
			return this;
		}
		#endregion

		#region ..克隆
//		public FontStyle Clone()
//		{
//			FontStyle fillstyle = new FontStyle();
//			if(this.FontFamily != null)
//				fillstyle.FontFamily = new DataType.SVGString(this.FontFamily.Value);
//			if(this.Font != null)
//				fillstyle.Font = new DataType.SVGString(this.Font.Value);
//			if(this.FontSize != null)
//				fillstyle.FontSize = new DataType.SVGLength(this.FontSize.Value,((YP.SVGDom.DataType.SVGLength)this.FontSize).ownerElement,((YP.SVGDom.DataType.SVGLength)this.FontSize).direction);
//			if(this.FontSizeAdjust != null)
//				fillstyle.FontSizeAdjust = new DataType.SVGNumber(this.FontSizeAdjust.Value);
//			if(this.FontStretch != null)
//				fillstyle.FontStretch = new DataType.SVGString(this.FontStretch.Value);
//			if(this.FontVariant != null)
//				fillstyle.FontVariant = new DataType.SVGString(this.FontVariant.Value);
//			if(this.Font_Style != null)
//				fillstyle.Font_Style = new DataType.SVGString(this.Font_Style.Value);
//			if(this.FontWeigth != null)
//				fillstyle.FontWeigth = new DataType.SVGString(this.FontWeigth.Value);
//			return fillstyle;
//		}
		#endregion
	}
}
