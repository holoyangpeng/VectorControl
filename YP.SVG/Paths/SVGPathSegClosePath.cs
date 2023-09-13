using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Paths
{
	/// <summary>
	/// ��������Z
	/// </summary>
	public class SVGPathSegClosePath:SVGPathSeg,Interface.Paths.ISVGPathSegClosePath
	{
		#region ..���켰����
		public SVGPathSegClosePath()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.pathSegTypeAsLetter = "Z";
			this.pathSegType =(short) PathSegmentType.PATHSEG_CLOSEPATH;
		}
		#endregion

		#region ..��������
		/// <summary>
		/// ���������ص�SVGPathSegMove����
		/// </summary>
//		public YP.SVGDom.Paths.SVGPathSegMove RelativePathSegMove;
		#endregion

		#region ..��ȡ·���յ�
		/// <summary>
		/// ��ȡ·�����յ�
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public override PointF GetLastPoint(Interface.Paths.ISVGPathSegList svgPathList)
		{
			YP.SVG.Paths.SVGPathSeg seg = ((YP.SVG.Paths.SVGPathSegList)svgPathList).GetRelativeStartPathSeg(this);
			if(seg != null)
				return seg.GetLastPoint(svgPathList);
			return PointF.Empty;
		}
		#endregion

		#region ..��·����Ԫ���ӵ�ָ��·��ĩβ
		/// <summary>
		/// ��·����Ԫ���ӵ�ָ��·��ĩβ
		/// </summary>
		/// <param name="path"></param>
		/// <param name="lastPoint"></param>
		public override void AppendGraphicsPath(GraphicsPath path,ref PointF lastPoint,Interface.Paths.ISVGPathSegList svgPathList)
		{
			path.CloseFigure();
		}
		#endregion

		#region ..��ȡ·������
		/// <summary>
		/// ��ȡ·������
		/// </summary>
		/// <param name="svgPathSegList">��Ԫ���ڵ�·���б�</param>
		/// <returns></returns>
		public override float GetPathLength(Interface.Paths.ISVGPathSegList svgPathSegList)
		{
			return 0;
		}
		#endregion

		#region ..��ȡGDI·��
		/// <summary>
		/// ��ȡGDI·��
		/// </summary>
		/// <returns></returns>
		public override GraphicsPath GetGDIPath(YP.SVG.Paths.SVGPathSegList svgPathSegList)
		{
			YP.SVG.Paths.SVGPathSeg seg = (YP.SVG.Paths.SVGPathSeg)svgPathSegList.PreviousSibling(this);
			if(seg == null)
				return null;
			PointF p = this.GetLastPoint(svgPathSegList);
			PointF p1 = seg.GetLastPoint(svgPathSegList);
			GraphicsPath path = new GraphicsPath();
			path.AddLine(p,p1);
			return path;
		}
		#endregion

		#region ..��ȡ��֮��ص�ǰһ�����Ƶ�
		/// <summary>
		/// ��ȡ��֮��ص�ǰһ�����Ƶ�
		/// </summary>
		/// <param name="svgPathSegList">��Ԫ���ڵ��б�</param>
		/// <returns></returns>
		public override PointF GetRelativePreControl(Interface.Paths.ISVGPathSegList svgPathSegList)
		{
			YP.SVG.Paths.SVGPathSeg pre = svgPathSegList.PreviousSibling(this) as YP.SVG.Paths.SVGPathSeg;
			if(pre == null)
				return PointF.Empty;
			
			if(pre is YP.SVG.Paths.SVGPathSegCurve)
			{
				if(!InPoint(this.GetLastPoint(svgPathSegList),pre.GetLastPoint(svgPathSegList)))
					return PointF.Empty;
				return (pre as YP.SVG.Paths.SVGPathSegCurve).GetSecondControl(svgPathSegList);
			}
			return PointF.Empty;
		}
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ·�����ݵ��ı����
		/// </summary>
		public override string PathString
		{
			get
			{
				return "Z";
			}
		}
		#endregion

		#region ..��ȡ��֮��صĺ�һ�����Ƶ�
		public override PointF GetRelativeNextControl(YP.SVG.Interface.Paths.ISVGPathSegList svgPathSegList)
		{
//			YP.SVGDom.Paths.SVGPathSeg pre = svgPathSegList.PreviousSibling(this) as YP.SVGDom.Paths.SVGPathSeg;
//			if(pre != null && InPoint(pre.GetLastPoint(svgPathSegList),this.GetLastPoint(svgPathSegList)))
//			{
				YP.SVG.Paths.SVGPathSegMove move = (svgPathSegList as SVGPathSegList).GetRelativeStartPathSeg(this);
				if(move != null)
					return move.GetRelativeNextControl(svgPathSegList);
//			}
			return this.relativeNextControl;
		}

		#endregion
	}
}
