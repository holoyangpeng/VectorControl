using System;

namespace YP.SVG.Interface.Link
{
	/// <summary>
	/// ISVGViewElement 的摘要说明。
	/// </summary>
	public interface ISVGViewElement:
		ISVGElement,
		ISVGExternalResourcesRequired,
		ISVGFitToViewBox,
		ISVGZoomAndPan 
	{
		DataType.ISVGStringList ViewTarget{get;}
	}
}
