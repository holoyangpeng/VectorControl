using System;

namespace YP.SVG.Interface.GradientsAndPatterns
{
	/// <summary>
	/// ISVGLinearGradientElement 的摘要说明。
	/// </summary>
	public interface ISVGLinearGradientElement:Interface.GradientsAndPatterns.ISVGGradientElement
	{
		DataType.ISVGLength X1{get;}
		DataType.ISVGLength Y1{get;}
		DataType.ISVGLength X2{get;}
		DataType.ISVGLength Y2{get;}
	}
}
