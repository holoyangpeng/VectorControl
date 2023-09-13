using System;

namespace YP.SVG.Interface.GradientsAndPatterns
{
	/// <summary>
	/// ISVGGradientElement ��ժҪ˵����
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
