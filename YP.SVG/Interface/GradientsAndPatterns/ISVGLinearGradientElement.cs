using System;

namespace YP.SVG.Interface.GradientsAndPatterns
{
	/// <summary>
	/// ISVGLinearGradientElement ��ժҪ˵����
	/// </summary>
	public interface ISVGLinearGradientElement:Interface.GradientsAndPatterns.ISVGGradientElement
	{
		DataType.ISVGLength X1{get;}
		DataType.ISVGLength Y1{get;}
		DataType.ISVGLength X2{get;}
		DataType.ISVGLength Y2{get;}
	}
}
