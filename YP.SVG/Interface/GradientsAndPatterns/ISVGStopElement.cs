using System;

namespace YP.SVG.Interface.GradientsAndPatterns
{
	/// <summary>
	/// ISVGStopElement ��ժҪ˵����
	/// </summary>
	public interface ISVGStopElement:Interface.ISVGElement,Interface.ISVGStylable
	{
		Interface.DataType.ISVGNumber Offset{get;}
	}
}
