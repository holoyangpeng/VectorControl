using System;

using YP.SVG.Interface;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ����Path����
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
		/// ��ȡ�����Totallength����ֵ
		/// </summary>
		Interface.DataType.ISVGNumber PathLength{get;}

		/// <summary>
		/// ��ȡ·������
		/// </summary>
		Interface.Paths.ISVGPathSegList PathData{get;}


		/// <summary>
		/// ��ȡ·���ܳ���
		/// </summary>
		/// <returns></returns>
		float GetTotalLength (  );

		/// <summary>
		/// ����·����ָ������ĵ�
		/// </summary>
		Interface.CTS.ISVGPoint GetPointAtLength ( float distance);

		/// <summary>
		/// ����λ��ָ������ĵ�λ·����Ԫ����
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
