using System;

namespace YP.SVG.Interface.GradientsAndPatterns
{
	/// <summary>
	/// ISVGStopElement 的摘要说明。
	/// </summary>
	public interface ISVGStopElement:Interface.ISVGElement,Interface.ISVGStylable
	{
		Interface.DataType.ISVGNumber Offset{get;}
	}
}
