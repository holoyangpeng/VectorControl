using System;

namespace YP.SVG.Interface.Link
{
	/// <summary>
	/// ISVGAElement ��ժҪ˵����
	/// </summary>
	public interface ISVGAElement:
		ISVGElement,
		ISVGURIReference,
		ISVGTests,
		ISVGLangSpace,
		ISVGExternalResourcesRequired,
		ISVGStylable,
		ISVGTransformable
	{
		DataType.ISVGString Target{get;}
	}
}
