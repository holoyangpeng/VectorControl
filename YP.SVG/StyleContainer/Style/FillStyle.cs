using System;

using YP.SVG.StyleContainer;

namespace YP.SVG.StyleContainer
{
	/// <summary>
	/// 记录填充类型信息
	/// </summary>
    public struct FillStyle
	{
		#region ..构造及消除
//		public FillStyle()
//		{
//			//
//			// TODO: 在此处添加构造函数逻辑
//			//
//			svgPaint = new Paint.SVGPaint("black",null,"black");
//			fillOpacity = 
//			fillrule = new DataType.SVGString("nonzero");
//		}

		static FillStyle()
		{
//			empty = 
		}

		public FillStyle(FillStyle style)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			_svgPaint = (Paint.SVGPaint)style.svgPaint;
			_fillOpacity = (DataType.SVGNumber)style.fillOpacity;
			_fillrule = (DataType.SVGString)style.fillrule;
			this.hatchcolor = style.hatchcolor;
			this.hatchStyle = (DataType.SVGString)style.hatchStyle;
		}

		public FillStyle(YP.SVG.Paint.SVGPaint paint,YP.SVG.DataType.SVGNumber opacity,YP.SVG.DataType.SVGString rule)
		{
			this._svgPaint = paint;
			this._fillrule = (DataType.SVGString)rule;
			this._fillOpacity = opacity;
			this.hatchcolor = new DataType.RGBColor("black");
			this.hatchStyle = new DataType.SVGString("none");
		}

		public FillStyle(YP.SVG.Paint.SVGPaint paint,YP.SVG.DataType.SVGNumber opacity,YP.SVG.DataType.SVGString rule,DataType.RGBColor hatchcolor,DataType.SVGString hatchstyle)
		{
			this._svgPaint = paint;
			this._fillrule = (DataType.SVGString)rule;
			this._fillOpacity = opacity;
			this.hatchcolor = hatchcolor;
			this.hatchStyle = hatchstyle;
		}
		#endregion

		#region ..私有变量
		SVG.Paint.SVGPaint _svgPaint;
		SVG.DataType.SVGNumber _fillOpacity;
		SVG.DataType.SVGString _fillrule;
		SVG.DataType.RGBColor hatchcolor;
		SVG.DataType.SVGString hatchStyle;
		#endregion

		#region ..静态方法
		static FillStyle empty =new FillStyle(
			Paint.SVGPaint.Empty,
			DataType.SVGNumber.Empty,
			DataType.SVGString.Empty,DataType.RGBColor.Empty,DataType.SVGString.Empty);

		static FillStyle fill = new FillStyle(
			new Paint.SVGPaint("black",null,"black"),
			new DataType.SVGNumber(1),
			new DataType.SVGString("nonzero"));

		public static FillStyle Default
		{
			get
			{
				return fill;
			}
		}


		public static FillStyle Empty
		{
			get
			{
//				FillStyle style = empty;
				return empty;
			}
		}
		#endregion

		#region ..公共属性
		public YP.SVG.Paint.SVGPaint svgPaint
		{
			set
			{
				this._svgPaint = value;// as Paint.SVGPaint;
			}
			get
			{
//				if(this._svgPaint.IsEmpty)
//					this._svgPaint = new Paint.SVGPaint("black",null,"black");
				return this._svgPaint;
			}
		}

		public YP.SVG.Interface.DataType.ISVGNumber fillOpacity
		{
			set
			{
				this._fillOpacity = (DataType.SVGNumber)value;
			}
			get
			{
//				if(this._fillOpacity.IsEmpty)
//					this._fillOpacity = new DataType.SVGNumber(1);
				return this._fillOpacity;
			}
		}

		public YP.SVG.Interface.DataType.ISVGString fillrule
		{
			set
			{
				this._fillrule = (DataType.SVGString)value;
			}
			get
			{
//				if(this._fillrule.IsEmpty)
//					this._fillrule = new DataType.SVGString("nonzero");
				return this._fillrule;
			}
		}

		public DataType.RGBColor HatchColor
		{
			set
			{
				this.hatchcolor = value;
			}
			get
			{
				return this.hatchcolor;
			}
		}

		public DataType.SVGString HatchStyle
		{
			set
			{
				this.hatchStyle = value;
			}
			get
			{
				return this.hatchStyle;
			}
		}
		#endregion

		#region ..公共方法
		/// <summary>
		/// 添加填充信息
		/// </summary>
		/// <param name="svgStyle"></param>
		public FillStyle MutiplyStyle(FillStyle newStyle)
		{
//			if(newStyle == null )
//				return this;
			if(this._fillOpacity.IsEmpty)
				this._fillOpacity = (DataType.SVGNumber)newStyle.fillOpacity;
			else
				this._fillOpacity = TypeMultiply.Multiply((DataType.SVGNumber)this._fillOpacity,(DataType.SVGNumber)newStyle.fillOpacity);
			if(this._svgPaint.IsEmpty)
				this._svgPaint = (Paint.SVGPaint)newStyle.svgPaint;
			else
			{
				this._svgPaint = TypeMultiply.Multiply(this._svgPaint,newStyle.svgPaint);
			}
			if(this._fillrule.IsEmpty)
				this._fillrule = (DataType.SVGString)newStyle.fillrule;
			else
				this._fillrule = TypeMultiply.Multiply(this._fillrule,(DataType.SVGString)newStyle.fillrule);
			if(!newStyle.hatchcolor.IsEmpty)
				this.hatchcolor = newStyle.hatchcolor;
			if(this.hatchStyle.IsEmpty)
				this.hatchStyle = newStyle.hatchStyle;
			else
				this.hatchStyle = TypeMultiply.Multiply(this.hatchStyle,newStyle.hatchStyle);
			return this;
		}
		#endregion
	}
}
