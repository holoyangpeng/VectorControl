using System;

namespace YP.SVG.Interface.ClipAndMask
{
	/// <summary>
	/// ISVGClipPathElement ��ժҪ˵����
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
		/// ��ȡ���з�ʽ
		/// </summary>
		System.Enum ClipPathUnits{get;}
	}
}
