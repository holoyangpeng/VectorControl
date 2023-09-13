using System;

using YP.SVG.StyleContainer;

namespace YP.SVG.StyleContainer
{
	/// <summary>
	/// ColorAndPaintStyle 的摘要说明。
	/// </summary>
    public struct ColorAndPaintStyle
	{
		#region ..构造及消除
//		public ColorAndPaintStyle()
//		{
//			//
//			// TODO: 在此处添加构造函数逻辑
//			//
//			this.color_interpolation = new DataType.SVGString("sRGBlinearRGB");
//			this.color_interpolation_filters = new DataType.SVGString("linearRGB");
//			this.color_profile = new DataType.SVGString("auto");
//			this.color_rendering = new DataType.SVGString("auto");
//			this.image_rendering = new DataType.SVGString("auto");
//			this.marker = new DataType.SVGString("");
//			this.marker_end = new DataType.SVGString("");
//			this.marker_mid = new DataType.SVGString("");
//			this.marker_start = new DataType.SVGString("");
//			this.text_rendering = new DataType.SVGString("auto");
//			this.shape_rendering = new DataType.SVGString("auto");
//		}

		public ColorAndPaintStyle(ColorAndPaintStyle style)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.color_interpolation = style.color_interpolation;
			this.color_interpolation_filters = style.color_interpolation_filters;
			this.color_profile = style.color_profile;
			this.color_rendering = style.color_rendering;
			this.image_rendering = style.image_rendering;
			this.marker = style.marker;
			this.marker_end = style.marker_end;
			this.marker_mid = style.marker_mid;
			this.marker_start = style.marker_start;
			this.text_rendering = style.text_rendering;
			this.shape_rendering = style.shape_rendering;
		}

		ColorAndPaintStyle(
			YP.SVG.DataType.SVGString color_interpolation,
			YP.SVG.DataType.SVGString color_interpolation_filters,
			YP.SVG.DataType.SVGString color_profile,
			YP.SVG.DataType.SVGString color_rendering,
			YP.SVG.DataType.SVGString image_rendering,
			YP.SVG.DataType.SVGString marker,
			YP.SVG.DataType.SVGString marker_end,
			YP.SVG.DataType.SVGString marker_mid,
			YP.SVG.DataType.SVGString marker_start,
			YP.SVG.DataType.SVGString shape_rendering,
			YP.SVG.DataType.SVGString text_rendering)
		{
			this.color_interpolation = color_interpolation;
			this.color_interpolation_filters = color_interpolation_filters;
			this.color_profile = color_profile;
			this.color_rendering = color_rendering;
			this.image_rendering = image_rendering;
			this.shape_rendering = shape_rendering;
			this.text_rendering = text_rendering;
			this.marker = marker;
			this.marker_end = marker_end;
			this.marker_mid = marker_mid;
			this.marker_start = marker_start;
		}

		#endregion

		#region ..静态方法
		static ColorAndPaintStyle style = new ColorAndPaintStyle(
			new DataType.SVGString("sRGBlinearRGB"),
			new DataType.SVGString("linearRGB"),
			new DataType.SVGString("auto"),
			new DataType.SVGString("auto"),
			new DataType.SVGString("auto"),
			new DataType.SVGString(""),
			new DataType.SVGString(""),
			new DataType.SVGString(""),
			new DataType.SVGString("auto"),
			new DataType.SVGString("auto"),
			new DataType.SVGString("auto"));

		static ColorAndPaintStyle empty = new ColorAndPaintStyle(
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty);
		/// <summary>
		/// 获取默认对象
		/// </summary>
		public static ColorAndPaintStyle Default
		{
			get
			{
				return style;
			}
		}

		public static ColorAndPaintStyle Empty
		{
			get
			{
				return empty;
			}
		}
		#endregion

		#region ..保护字段
		public YP.SVG.DataType.SVGString color_interpolation ;
		public YP.SVG.DataType.SVGString color_interpolation_filters ;
		public YP.SVG.DataType.SVGString color_profile ;
		public YP.SVG.DataType.SVGString color_rendering; 
		public YP.SVG.DataType.SVGString image_rendering ;
		public YP.SVG.DataType.SVGString marker ;
		public YP.SVG.DataType.SVGString marker_end ;
		public YP.SVG.DataType.SVGString marker_mid ;
		public YP.SVG.DataType.SVGString marker_start; 
		public YP.SVG.DataType.SVGString text_rendering;
		public YP.SVG.DataType.SVGString shape_rendering;
		#endregion

		#region ..公共方法
		/// <summary>
		/// 添加填充信息
		/// </summary>
		/// <param name="svgStyle"></param>
		public ColorAndPaintStyle MutiplyStyle(ColorAndPaintStyle newStyle)
		{
//			if(newStyle == null )
//				return this;
			if(this.color_interpolation.IsEmpty)
				this.color_interpolation = (DataType.SVGString)newStyle.color_interpolation;
			else
				this.color_interpolation = TypeMultiply.Multiply(this.color_interpolation,newStyle.color_interpolation);
			if(this.color_interpolation_filters.IsEmpty)
				this.color_interpolation_filters = (DataType.SVGString)newStyle.color_interpolation_filters ;
			else
				this.color_interpolation_filters = TypeMultiply.Multiply(this.color_interpolation_filters,newStyle.color_interpolation_filters);
			if(this.color_profile .IsEmpty)
				this.color_profile = (DataType.SVGString)newStyle.color_profile;
			else
				this.color_profile = TypeMultiply.Multiply(this.color_profile,newStyle.color_profile);
			if(this.color_rendering .IsEmpty)
				this.color_rendering = (DataType.SVGString)newStyle.color_rendering;
			else
				this.color_rendering = TypeMultiply.Multiply(this.color_rendering,newStyle.color_rendering);
			if(this.image_rendering .IsEmpty)
				this.image_rendering = (DataType.SVGString)newStyle.image_rendering;
			else
				this.image_rendering = TypeMultiply.Multiply(this.image_rendering,newStyle.image_rendering);
			if(this.marker .IsEmpty)
				this.marker = (DataType.SVGString)newStyle.marker;
			else
				this.marker = TypeMultiply.Multiply(this.marker,newStyle.marker);
			if(this.marker_end .IsEmpty)
				this.marker_end = (DataType.SVGString)newStyle.marker_end;
			else
				this.marker_end = TypeMultiply.Multiply(this.marker_end,newStyle.marker_end);
			if(this.marker_mid .IsEmpty)
				this.marker_mid = (DataType.SVGString)newStyle.marker_mid;
			else
				this.marker_mid = TypeMultiply.Multiply(this.marker_mid,newStyle.marker_mid);
			if(this.marker_start .IsEmpty)
				this.marker_start = (DataType.SVGString)newStyle.marker_start;
			else
				this.marker_start = TypeMultiply.Multiply(this.marker_start,newStyle.marker_start);
			if(this.text_rendering .IsEmpty)
				this.text_rendering = (DataType.SVGString)newStyle.text_rendering;
			else
				this.text_rendering = TypeMultiply.Multiply(this.text_rendering,newStyle.text_rendering);
			if(this.shape_rendering.IsEmpty)
				this.shape_rendering = (DataType.SVGString)newStyle.shape_rendering;
			else
				this.shape_rendering = TypeMultiply.Multiply(this.text_rendering,newStyle.text_rendering);
			return this;
		}
		#endregion

		#region ..克隆
//		public ColorAndPaintStyle Clone()
//		{
//			ColorAndPaintStyle fillstyle = new ColorAndPaintStyle();
//			if(this.color_interpolation != null)
//				fillstyle.color_interpolation = new DataType.SVGString(this.color_interpolation.Value);
//			if(this.color_interpolation_filters != null)
//				fillstyle.color_interpolation_filters = new DataType.SVGString(this.color_interpolation_filters.Value);
//			if(this.color_profile != null)
//				fillstyle.color_profile = new DataType.SVGString(this.color_profile.Value);
//			if(this.color_rendering != null)
//				fillstyle.color_rendering = new DataType.SVGString(this.color_rendering.Value);
//			if(this.image_rendering != null)
//				fillstyle.image_rendering = new DataType.SVGString(this.image_rendering.Value);
//			if(this.marker != null)
//				fillstyle.marker = new DataType.SVGString(this.marker.Value);
//			if(this.marker_end != null)
//				fillstyle.marker_end = new DataType.SVGString(this.marker_end.Value);
//			if(this.marker_mid != null)
//				fillstyle.marker_mid = new DataType.SVGString(this.marker_mid.Value);
//			if(this.marker_start != null)
//				fillstyle.marker_start = new DataType.SVGString(this.marker_start.Value);
//			if(this.text_rendering != null)
//				fillstyle.text_rendering = new DataType.SVGString(this.text_rendering.Value);
//			if(this.shape_rendering != null)
//				fillstyle.shape_rendering = new DataType.SVGString(this.shape_rendering.Value);
//			return fillstyle;
//		}
		#endregion
	}
}
