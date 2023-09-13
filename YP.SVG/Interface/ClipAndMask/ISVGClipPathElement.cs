using System;

namespace YP.SVG.Interface.ClipAndMask
{
	/// <summary>
	/// ISVGClipPathElement 的摘要说明。
	/// </summary>
	public interface ISVGClipPathElement:
		ISVGElement,
		ISVGTests,
		ISVGLangSpace,
		ISVGExternalResourcesRequired,
		ISVGStylable,
		ISVGTransformable
	{
		/// <summary>
		/// 获取剪切方式
		/// </summary>
		System.Enum ClipPathUnits{get;}
	}
}
