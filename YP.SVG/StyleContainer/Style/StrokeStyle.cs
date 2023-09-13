using System;

using YP.SVG.StyleContainer;
using YP.SVG.Interface;

namespace YP.SVG.StyleContainer
{
	/// <summary>
	/// 记录描边类类型信息
	/// </summary>
    public struct StrokeStyle
	{
		#region ..构造及消除
//		public StrokeStyle()
//		{
//			//
//			// TODO: 在此处添加构造函数逻辑
//			//
//			this.svgStroke = new Paint.SVGPaint("none",null,"none");
//			this.strokeOpacity = new DataType.SVGNumber("1");
//			this.strokewidth = new DataType.SVGNumber("1");
//			this.stroke_dasharray = new DataType.SVGString("none");
//			this.stroke_linecap = new DataType.SVGString("butt");
//			this.stroke_linejoin = new DataType.SVGString("miter");
//			this.stroke_dashoffset = new DataType.SVGLength(0,null,LengthDirection.Viewport);
//			this.stroke_miterlimit = new DataType.SVGNumber(4);
//		}

		public StrokeStyle(StrokeStyle style)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.svgStroke = style.svgStroke;
			this.strokeOpacity = style.strokeOpacity;
			this.strokewidth = style.strokewidth;
			this.stroke_dasharray = style.stroke_dasharray;
			this.stroke_linecap = style.stroke_linecap;
			this.stroke_linejoin = style.stroke_linejoin;
			this.stroke_dashoffset = style.stroke_dashoffset;
			this.stroke_miterlimit = style.stroke_miterlimit;
            this.stroke_hatchStyle = style.stroke_hatchStyle;
            this.stroke_hatchColor = style.stroke_hatchColor;
            this.stroke_gradientMode = style.stroke_gradientMode;
		}

        StrokeStyle(YP.SVG.Paint.SVGPaint stroke,
			YP.SVG.DataType.SVGNumber op,
			YP.SVG.DataType.SVGNumber width,
			YP.SVG.DataType.SVGString dash,
			YP.SVG.DataType.SVGLength offset,
			YP.SVG.DataType.SVGString cap,
			YP.SVG.DataType.SVGString join,
			YP.SVG.DataType.SVGNumber miter):this(stroke, op,width,dash, offset,cap, join, miter,
            new DataType.RGBColor("black"),new DataType.SVGString("none"), new DataType.SVGString("auto"))
        {
        }

		StrokeStyle(YP.SVG.Paint.SVGPaint stroke,
			YP.SVG.DataType.SVGNumber op,
			YP.SVG.DataType.SVGNumber width,
			YP.SVG.DataType.SVGString dash,
			YP.SVG.DataType.SVGLength offset,
			YP.SVG.DataType.SVGString cap,
			YP.SVG.DataType.SVGString join,
			YP.SVG.DataType.SVGNumber miter,
            SVG.DataType.RGBColor stroke_hatchColor,
            SVG.DataType.SVGString stroke_hatchStyle,
            SVG.DataType.SVGString stroke_gradientMode)
		{
			this.svgStroke = stroke;
			this.strokeOpacity = op;
			this.strokewidth = width;
			this.stroke_dasharray = dash;
			this.stroke_dashoffset = offset;
			this.stroke_linecap = cap;
			this.stroke_linejoin = join;
			this.stroke_miterlimit = miter;
            this.stroke_hatchColor = stroke_hatchColor;
            this.stroke_hatchStyle = stroke_hatchStyle;
            this.stroke_gradientMode = stroke_gradientMode;
		}
		#endregion

		#region ..静态
		static StrokeStyle style = new StrokeStyle(new Paint.SVGPaint("none",null,"none"),
			new DataType.SVGNumber(1),
			new DataType.SVGNumber(1),
			new DataType.SVGString("none"),
			new DataType.SVGLength(0,null,LengthDirection.Viewport),
			new DataType.SVGString("butt"),
			new DataType.SVGString("miter"),
			new DataType.SVGNumber(4));
			
		static StrokeStyle empty = new StrokeStyle( Paint.SVGPaint.Empty,
			 DataType.SVGNumber.Empty,
			 DataType.SVGNumber.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGLength.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGString.Empty,
			 DataType.SVGNumber.Empty,
             DataType.RGBColor.Empty,
             DataType.SVGString.Empty,
             DataType.SVGString.Empty);

		/// <summary>
		/// 获取默认对象
		/// </summary>
		public static StrokeStyle Default
		{
			get
			{
				return style;
			}
		}

		/// <summary>
		/// 获取空对象
		/// </summary>
		public static StrokeStyle Empty
		{
			get
			{
				return empty;
			}
		}
		#endregion

		#region ..保护变量
		public SVG.Paint.SVGPaint svgStroke ;
		public SVG.DataType.SVGNumber strokeOpacity;
		public SVG.DataType.SVGNumber strokewidth;
		public SVG.DataType.SVGString stroke_dasharray ;
		public SVG.DataType.SVGLength stroke_dashoffset; 
		public SVG.DataType.SVGString stroke_linecap ;
		public SVG.DataType.SVGString stroke_linejoin ;
		public SVG.DataType.SVGNumber stroke_miterlimit ;
        public SVG.DataType.RGBColor stroke_hatchColor;
        public SVG.DataType.SVGString stroke_hatchStyle;
        public SVG.DataType.SVGString stroke_gradientMode;
		#endregion

		#region ..公共方法
		/// <summary>
		/// 添加填充信息
		/// </summary>
		/// <param name="svgStyle"></param>
		public StrokeStyle MutiplyStyle(StrokeStyle newStyle)
		{
//			if(newStyle == null )
//				return this;
			if(this.strokeOpacity.IsEmpty)
				this.strokeOpacity = (DataType.SVGNumber)newStyle.strokeOpacity;
			else
				this.strokeOpacity = TypeMultiply.Multiply((DataType.SVGNumber)this.strokeOpacity,newStyle.strokeOpacity);
			if(this.svgStroke.IsEmpty)
				this.svgStroke = (Paint.SVGPaint)newStyle.svgStroke;
			else
				this.svgStroke = TypeMultiply.Multiply((Paint.SVGPaint)this.svgStroke,newStyle.svgStroke);
			
			if(this.strokewidth.IsEmpty)
				this.strokewidth = (DataType.SVGNumber)newStyle.strokewidth;
			else
				this.strokewidth = TypeMultiply.Multiply((DataType.SVGNumber)this.strokewidth,newStyle.strokewidth);
			if(this.stroke_miterlimit.IsEmpty)
				this.stroke_miterlimit = (DataType.SVGNumber)newStyle.stroke_miterlimit;
			else
				this.stroke_miterlimit = TypeMultiply.Multiply((DataType.SVGNumber)this.stroke_miterlimit,newStyle.stroke_miterlimit);
			if(this.stroke_linejoin.IsEmpty)
				this.stroke_linejoin = (DataType.SVGString)newStyle.stroke_linejoin;
			else
				this.stroke_linejoin = TypeMultiply.Multiply(this.stroke_linejoin,newStyle.stroke_linejoin);
			if(this.stroke_linecap.IsEmpty)
				this.stroke_linecap = (DataType.SVGString)newStyle.stroke_linecap;
			else
				this.stroke_linecap = TypeMultiply.Multiply(this.stroke_linecap,newStyle.stroke_linecap);
			if(this.stroke_dasharray.IsEmpty)
				this.stroke_dasharray = (DataType.SVGString)newStyle.stroke_dasharray;
			else
				this.stroke_dasharray = TypeMultiply.Multiply(this.stroke_dasharray,newStyle.stroke_dasharray);
			if(this.stroke_dashoffset.IsEmpty)
				this.stroke_dashoffset =(DataType.SVGLength)newStyle.stroke_dashoffset;
			else
				this.stroke_dashoffset = TypeMultiply.Multiply((DataType.SVGLength)this.stroke_dashoffset,newStyle.stroke_dashoffset);
            if (!newStyle.stroke_hatchColor.IsEmpty)
                this.stroke_hatchColor = newStyle.stroke_hatchColor;
            if (this.stroke_hatchStyle.IsEmpty)
                this.stroke_hatchStyle = newStyle.stroke_hatchStyle;
            else
                this.stroke_hatchStyle = TypeMultiply.Multiply(this.stroke_hatchStyle, newStyle.stroke_hatchStyle);

            if (this.stroke_gradientMode.IsEmpty)
                this.stroke_gradientMode = newStyle.stroke_gradientMode;
            else
                this.stroke_gradientMode = TypeMultiply.Multiply(this.stroke_gradientMode, newStyle.stroke_gradientMode);
			return this;
		}
		#endregion

		#region ..克隆
//		public StrokeStyle Clone()
//		{
//			StrokeStyle fillstyle = new StrokeStyle();
//			if(this.svgStroke != null)
//				fillstyle.svgStroke = new  Paint.SVGPaint(((YP.SVGDom.Paint.SVGPaint)this.svgStroke).RenderStr,((YP.SVGDom.Paint.SVGPaint)this.svgStroke).ownerStyleElement,"none");
//			if(this.strokeOpacity != null)
//				fillstyle.strokeOpacity = new DataType.SVGNumber(this.strokeOpacity.Value);
//			if(this.strokewidth != null)
//				fillstyle.strokewidth = new DataType.SVGNumber(this.strokewidth.Value);
//			if(this.stroke_miterlimit != null)
//				fillstyle.stroke_miterlimit = new DataType.SVGNumber(this.stroke_miterlimit.Value);
//			if(this.stroke_dashoffset != null)
//				fillstyle.stroke_dashoffset = new DataType.SVGLength(this.stroke_dashoffset.Value,((YP.SVGDom.DataType.SVGLength)this.stroke_dashoffset).ownerElement,((YP.SVGDom.DataType.SVGLength)this.stroke_dashoffset).direction);
//
//			if(this.stroke_linecap != null)
//				fillstyle.stroke_linecap = new DataType.SVGString(this.stroke_linecap.Value);
//			if(this.stroke_dasharray != null)
//				fillstyle.stroke_dasharray = new DataType.SVGString(this.stroke_dasharray.Value);
//			if(this.stroke_linejoin != null)
//				fillstyle.stroke_linejoin = new DataType.SVGString(this.stroke_linejoin.Value);
//
//			return fillstyle;
//		}
		#endregion
	}
}
