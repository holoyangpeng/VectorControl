using System;

using YP.SVG.Interface;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// 定义Path对象
	/// </summary>
	public interface ISVGPathElement:
		ISVGElement,
		ISVGTests,
		ISVGLangSpace,
		ISVGExternalResourcesRequired,
		ISVGStylable,
		ISVGTransformable
	{
		/// <summary>
		/// 获取对象的Totallength属性值
		/// </summary>
		Interface.DataType.ISVGNumber PathLength{get;}

		/// <summary>
		/// 获取路径数据
		/// </summary>
		Interface.Paths.ISVGPathSegList PathData{get;}


		/// <summary>
		/// 获取路径总长度
		/// </summary>
		/// <returns></returns>
		float GetTotalLength (  );

		/// <summary>
		/// 计算路径上指定距离的点
		/// </summary>
		Interface.CTS.ISVGPoint GetPointAtLength ( float distance);

		/// <summary>
		/// 计算位于指定距离的单位路径单元索引
		/// </summary>
		/// <param name="distance"></param>
		/// <returns></returns>
		int GetPathSegAtLength (  float distance );
		ISVGPathSegClosePath    CreateSVGPathSegClosePath (  );
		ISVGPathSegMovetoAbs    CreateSVGPathSegMovetoAbs (  float x,  float y );
		ISVGPathSegMovetoRel    CreateSVGPathSegMovetoRel (  float x,  float y );
		ISVGPathSegLinetoAbs    CreateSVGPathSegLinetoAbs (  float x,  float y );
		ISVGPathSegLinetoRel    CreateSVGPathSegLinetoRel (  float x,  float y );
		ISVGPathSegCurvetoCubicAbs    CreateSVGPathSegCurvetoCubicAbs (  float x,  float y,  float x1,  float y1,  float x2,  float y2 );
		ISVGPathSegCurvetoCubicRel    CreateSVGPathSegCurvetoCubicRel (  float x,  float y,  float x1,  float y1,  float x2,  float y2 );
		ISVGPathSegCurvetoQuadraticAbs    CreateSVGPathSegCurvetoQuadraticAbs (  float x,  float y,  float x1,  float y1 );
		ISVGPathSegCurvetoQuadraticRel    CreateSVGPathSegCurvetoQuadraticRel (  float x,  float y,  float x1,  float y1 );
		ISVGPathSegArcAbs    CreateSVGPathSegArcAbs (  float x,  float y,  float r1,  float r2,  float angle,  bool largeArcFlag,  bool sweepFlag );
		ISVGPathSegArcRel    CreateSVGPathSegArcRel (  float x,  float y,  float r1,  float r2,  float angle,  bool largeArcFlag,  bool sweepFlag );
		ISVGPathSegLinetoHorizontalAbs    CreateSVGPathSegLinetoHorizontalAbs (  float x );
		ISVGPathSegLinetoHorizontalRel    CreateSVGPathSegLinetoHorizontalRel (  float x );
		ISVGPathSegLinetoVerticalAbs    CreateSVGPathSegLinetoVerticalAbs (  float y );
		ISVGPathSegLinetoVerticalRel    CreateSVGPathSegLinetoVerticalRel (  float y );
		ISVGPathSegCurvetoCubicSmoothAbs    CreateSVGPathSegCurvetoCubicSmoothAbs (  float x,  float y,  float x2,  float y2 );
		ISVGPathSegCurvetoCubicSmoothRel    CreateSVGPathSegCurvetoCubicSmoothRel (  float x,  float y,  float x2,  float y2 );
		ISVGPathSegCurvetoQuadraticSmoothAbs    CreateSVGPathSegCurvetoQuadraticSmoothAbs (  float x,  float y );
		ISVGPathSegCurvetoQuadraticSmoothRel    CreateSVGPathSegCurvetoQuadraticSmoothRel (  float x,  float y );
	}
}
