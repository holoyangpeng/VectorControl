using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// ISVGViewSpec 的摘要说明。
	/// </summary>
	public interface ISVGViewSpec:
		ISVGZoomAndPan,
		ISVGFitToViewBox
	{
		Interface.CTS.ISVGTransformList Transform{get;}
		ISVGElement ViewTarget{get;}
		string ViewBoxString{get;}
		string PreserveAspectRatioString{get;}
		string TransformString{get;}
		string ViewTargetString{get;}
	}
}
