using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// 定义ISVGZoomAndPan 的一般行为，这一类对象都具备“ZoomAndPan"行为
	/// </summary>
	public interface ISVGZoomAndPan
	{
		SVGZoomAndPanType ZoomAndPan{get;}
	}
}
