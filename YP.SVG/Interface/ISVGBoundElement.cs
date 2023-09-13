using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// 定义一类对象，这类对象都具备X,Y,Width,Height属性
	/// </summary>
	public interface ISVGBoundElement
	{
		/// <summary>
		/// 表示矩形对象的X属性
		/// </summary>
		SVG.DataType.SVGLength X{get;}

		/// <summary>
		/// 表示矩形对象的Y属性
		/// </summary>
        SVG.DataType.SVGLength Y { get; }
		
		/// <summary>
		/// 表示矩形对象的Width对象
		/// </summary>
        SVG.DataType.SVGLength Width { get; }
		
		/// <summary>
		/// 表示矩形对象的Height属性
		/// </summary>
        SVG.DataType.SVGLength Height { get; }
	}
}
