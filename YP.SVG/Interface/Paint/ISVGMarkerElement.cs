using System;

namespace YP.SVG.Interface.Paint
{
	/// <summary>
	/// ISVGMarkerElement 的摘要说明。
	/// </summary>
	public interface ISVGMarkerElement:
		ISVGElement,
		ISVGLangSpace,
		ISVGExternalResourcesRequired,
//		SVGStylable,
		ISVGFitToViewBox
	{
		DataType.ISVGLength RefX{get;}
		DataType.ISVGLength RefY{get;}
		System.Enum MarkerUnits{get;}
		DataType.ISVGLength MarkerWidth{get;}
		DataType.ISVGLength MarkerHeight{get;}
		System.Enum OrientType{get;}
		DataType.ISVGAngle OrientAngle{get;}
		void SetOrientToAuto();
		void SetOrientToAngle(DataType.ISVGAngle angle);
	}
}
