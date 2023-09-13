using System;

namespace YP.SVG.StyleContainer
{
	/// <summary>
	/// 实现类型的前乘操作
	/// </summary>
    public class TypeMultiply
	{
		#region ..将两个SVGNumber“相乘”，返回一个新的SVGNumber
		/// <summary>
		/// 将两个SVGNumber“相乘”，返回一个新的SVGNumber
		/// </summary>
		/// <param name="orinumber">原始SVGNumber</param>
		/// <param name="multiplynumber">因子SVGNumber</param>
		/// <returns></returns>
		public static DataType.SVGNumber Multiply(DataType.SVGNumber orinumber,DataType.SVGNumber multiplynumber)
		{
			if(multiplynumber.IsEmpty)
			{
				if(orinumber.IsEmpty)
					return orinumber;
				return new DataType.SVGNumber(orinumber.Value);
			}
			if(!multiplynumber.IsEmpty)
				return new DataType.SVGNumber(multiplynumber.Value);
			if(multiplynumber.IsEmpty)
			{
				if(orinumber.IsEmpty)
					return orinumber;
				return new DataType.SVGNumber(orinumber.Value);
			}
			return new DataType.SVGNumber(multiplynumber.Value);
		}
		#endregion

		#region ..将两个SVGLength“相乘”，返回一个新的SVGNumber
		/// <summary>
		/// 将两个SVGNumber“相乘”，返回一个新的SVGNumber
		/// </summary>
		/// <param name="orinumber">原始SVGNumber</param>
		/// <param name="multiplynumber">因子SVGNumber</param>
		/// <returns></returns>
		public static DataType.SVGLength Multiply(YP.SVG.DataType.SVGLength orinumber,DataType.SVGLength multiplynumber)
		{
			if(multiplynumber.IsEmpty)
			{
				if(orinumber.IsEmpty)
					return orinumber;
				return new DataType.SVGLength(orinumber.Value,((YP.SVG.DataType.SVGLength)orinumber).ownerElement,((YP.SVG.DataType.SVGLength)orinumber).direction);
			}
			if(!multiplynumber.IsEmpty)//.BaseValue != null)
			{
				YP.SVG.Interface.DataType.ISVGLength length = multiplynumber;
				return new DataType.SVGLength(length.Value,((DataType.SVGLength)length).ownerElement,((DataType.SVGLength)length).direction);
			}
			if(multiplynumber.IsEmpty)// == null)
			{
				if(orinumber.IsEmpty)
					return orinumber;
				return new DataType.SVGLength(orinumber.Value,((YP.SVG.DataType.SVGLength)orinumber).ownerElement,((YP.SVG.DataType.SVGLength)orinumber).direction);
			}
			YP.SVG.Interface.DataType.ISVGLength length1 = multiplynumber;
			return new DataType.SVGLength(length1.Value,((DataType.SVGLength)length1).ownerElement,((DataType.SVGLength)length1).direction);
		}
		#endregion

		#region ..将两个SVGPaint相乘，返回一个新的SVGPaint
		/// <summary>
		/// 将两个SVGPaint相乘，返回一个新的SVGPaint
		/// </summary>
		/// <param name="oripaint">原始SVGPaint</param>
		/// <param name="multiplypaint">因子SVGPaint</param>
		/// <returns></returns>
		public static Paint.SVGPaint Multiply(YP.SVG.Paint.SVGPaint oripaint,Paint.SVGPaint multiplypaint)
		{
			if(multiplypaint.IsEmpty)
			{
				if(oripaint.IsEmpty)
					return (Paint.SVGPaint)oripaint;
				return new Paint.SVGPaint(((YP.SVG.Paint.SVGPaint)oripaint).RenderStr,((YP.SVG.Paint.SVGPaint)oripaint).ownerStyleElement,oripaint.DefaultValue);
			}
			else
			{
				if(!multiplypaint.IsEmpty)//.BaseValue != null)
				{
					Paint.SVGPaint anim = (Paint.SVGPaint)multiplypaint;
					return new Paint.SVGPaint(anim.RenderStr,anim.ownerStyleElement,anim.DefaultValue);
				}
				if(multiplypaint.IsEmpty)// == null)
				{
					if(oripaint.IsEmpty)
						return oripaint;
					return new Paint.SVGPaint(((YP.SVG.Paint.SVGPaint)oripaint).RenderStr,((YP.SVG.Paint.SVGPaint)oripaint).ownerStyleElement,oripaint.DefaultValue );;
				}
//				if(oripaint.PaintType == multiplypaint.PaintType && oripaint.PaintType == (byte)PaintType.SVG_PAINTTYPE_RGBCOLOR)
//				{
//					DataType.RGBColor c1 = (DataType.RGBColor)oripaint.RgbColor;
//					DataType.RGBColor c2 = (DataType.RGBColor)multiplypaint.RgbColor;
//					int r = c1.R + c2.R;
//					int g = c1.G + c2.G;
//					int b = c1.B + c2.B;
//					if(r > 255)
//						r = r - 255;
//					if( g > 255)
//						g = g - 255;
//					if(b >255)
//						b -= 255;
//					return new Paint.SVGPaint("rgb("+r.ToString()+","+g.ToString()+","+b.ToString()+")",oripaint.ownerStyleElement);
//				}
				return new Paint.SVGPaint(((Paint.SVGPaint)multiplypaint).RenderStr,((Paint.SVGPaint)multiplypaint).ownerStyleElement,((Paint.SVGPaint)multiplypaint).DefaultValue);
			}
		}
		#endregion

		#region ..将两个SVGString相乘，返回一个新的SVGString
		/// <summary>
		/// 将两个SVGString相乘，返回一个新的SVGString
		/// </summary>
		/// <param name="ori">原始SVGString</param>
		/// <param name="mul">因子SVGString</param>
		/// <returns></returns>
		public static DataType.SVGString Multiply(YP.SVG.DataType.SVGString ori,DataType.SVGString mul)
		{
			if(mul.IsEmpty)
			{
				if(ori.IsEmpty)
					return (DataType.SVGString)ori;
				return new DataType.SVGString(ori.Value);
			}
			if(!mul.IsEmpty)//..BaseValue != null)
				return new DataType.SVGString( mul.Value);
			if(mul.IsEmpty)// == null)
			{
				if(ori.IsEmpty)
					return ori;
				return new DataType.SVGString(ori.Value);
			}
			return new DataType.SVGString(mul.Value);
		}
		#endregion
	}
}
