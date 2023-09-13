using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Interface
{
	/// <summary>
	/// ISVGFitToViewBox 的摘要说明。
	/// </summary>
	public interface ISVGFitToViewBox
	{
		DataType.ISVGRect ViewBox{get;}
		Interface.CTS.ISVGPreserveAspectRatio PreserveAspectRatio{get;}
	}
}
