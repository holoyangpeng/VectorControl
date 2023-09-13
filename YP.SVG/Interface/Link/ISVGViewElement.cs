using System;

namespace YP.SVG.Interface.Link
{
	/// <summary>
	/// ISVGViewElement ��ժҪ˵����
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
