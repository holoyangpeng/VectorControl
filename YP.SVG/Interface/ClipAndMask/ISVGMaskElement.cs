using System;

namespace YP.SVG.Interface.ClipAndMask
{
	/// <summary>
	/// ISVGMaskElement 的摘要说明。
	/// </summary>
	public interface ISVGMaskElement:
		ISVGElement,
		ISVGTests,
		ISVGLangSpace,
		ISVGExternalResourcesRequired,
		ISVGStylable
	{
		System.Enum MaskUnits {get;}
		System.Enum MaskContentUnits {get;}
		DataType.ISVGLength X {get;}
		DataType.ISVGLength Y {get;}
		DataType.ISVGLength Width {get;}
		DataType.ISVGLength Height {get;}	
	}
}
