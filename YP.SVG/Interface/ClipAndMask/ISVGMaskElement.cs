using System;

namespace YP.SVG.Interface.ClipAndMask
{
	/// <summary>
	/// ISVGMaskElement ��ժҪ˵����
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
