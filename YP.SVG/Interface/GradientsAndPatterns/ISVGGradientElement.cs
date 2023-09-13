using System;

namespace YP.SVG.Interface.GradientsAndPatterns
{
	/// <summary>
	/// ISVGGradientElement 的摘要说明。
	/// </summary>
	public interface ISVGGradientElement:
		ISVGElement,
		ISVGURIReference,
		ISVGExternalResourcesRequired,
		ISVGStylable
	{
		System.Enum   GradientUnits{get;}
		CTS.ISVGTransformList GradientTransform{get;}
		System.Enum   SpreadMethod{get;}
	}
}
