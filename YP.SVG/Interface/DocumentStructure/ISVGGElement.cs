using System;

namespace YP.SVG.Interface.DocumentStructure
{
	/// <summary>
	/// ISVGGElement 的摘要说明。
	/// </summary>
	public interface ISVGGElement:
		ISVGElement,
		ISVGTests,
		ISVGLangSpace,
		ISVGExternalResourcesRequired,
		ISVGStylable,
		ISVGTransformable
	{
		
	}
}
