using System;

namespace YP.SVG.Interface.GradientsAndPatterns
{
	/// <summary>
	/// ISVGRadialGradientElement ��ժҪ˵����
	/// </summary>
	public interface ISVGRadialGradientElement:Interface.GradientsAndPatterns.ISVGGradientElement 
	{
		Interface.DataType.ISVGLength Cx{get;}
		Interface.DataType.ISVGLength Cy{get;}
		Interface.DataType.ISVGLength R{get;}
		Interface.DataType.ISVGLength Fx{get;}
		Interface.DataType.ISVGLength Fy{get;}

	}
}
