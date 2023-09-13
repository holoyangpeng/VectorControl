using System;

namespace YP.SVG.Interface.GradientsAndPatterns
{
	/// <summary>
	/// ISVGPatternElement 的摘要说明。
	/// </summary>
	public interface ISVGPatternElement:
		ISVGElement,
		ISVGURIReference,
		ISVGTests,
		ISVGLangSpace,
		ISVGExternalResourcesRequired,
		ISVGStylable,
		ISVGFitToViewBox 
	{
		/// <summary>
		/// 获取"patternUnits"属性
		/// </summary>
		System.Enum PatternUnits{get;}

		System.Enum PatternContentUnits{get;}

		CTS.ISVGTransformList PatternTransform{get;}

		DataType.ISVGLength X{get;}
		DataType.ISVGLength Y{get;}
		DataType.ISVGLength Width{get;}
		DataType.ISVGLength Height{get;}
	}
}
