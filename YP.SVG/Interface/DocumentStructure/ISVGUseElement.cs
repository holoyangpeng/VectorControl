using System;

namespace YP.SVG.Interface.DocumentStructure
{
	/// <summary>
	/// ISVGUseElement ��ժҪ˵����
	/// </summary>
	public interface ISVGUseElement:
		ISVGElement,
		ISVGURIReference ,
		ISVGTests,
		ISVGLangSpace,
		ISVGExternalResourcesRequired,
		ISVGStylable,
		ISVGTransformable
	{
        SVG.DataType.SVGLength X { get; }
        SVG.DataType.SVGLength Y { get; }
        SVG.DataType.SVGLength Width { get; }
        SVG.DataType.SVGLength Height { get; }
		ISVGElementInstance InstanceRoot{get;}
		ISVGElementInstance AnimatedInstanceRoot{get;}
	}
}
