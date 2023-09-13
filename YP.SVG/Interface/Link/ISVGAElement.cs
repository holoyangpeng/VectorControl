using System;

namespace YP.SVG.Interface.Link
{
	/// <summary>
	/// ISVGAElement 的摘要说明。
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
