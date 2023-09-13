using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Paths
{
	/// <summary>
	/// 定义命令Z
	/// </summary>
	public class SVGPathSegClosePath:SVGPathSeg,Interface.Paths.ISVGPathSegClosePath
	{
		#region ..构造及消除
		public SVGPathSegClosePath()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.pathSegTypeAsLetter = "Z";
			this.pathSegType =(short) PathSegmentType.PATHSEG_CLOSEPATH;
		}
		#endregion

		#region ..保护变量
		/// <summary>
		/// 定义与此相关的SVGPathSegMove对象
		/// </summary>
//		public YP.SVGDom.Paths.SVGPathSegMove RelativePathSegMove;
		#endregion

		#region ..获取路径终点
		/// <summary>
		/// 获取路径中终点
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

		#region ..将路径单元附加到指定路径末尾
		/// <summary>
		/// 将路径单元附加到指定路径末尾
		/// </summary>
		/// <param name="path"></param>
		/// <param name="lastPoint"></param>
		public override void AppendGraphicsPath(GraphicsPath path,ref PointF lastPoint,Interface.Paths.ISVGPathSegList svgPathList)
		{
			path.CloseFigure();
		}
		#endregion

		#region ..获取路径长度
		/// <summary>
		/// 获取路径长度
		/// </summary>
		/// <param name="svgPathSegList">单元所在的路径列表</param>
		/// <returns></returns>
		public override float GetPathLength(Interface.Paths.ISVGPathSegList svgPathSegList)
		{
			return 0;
		}
		#endregion

		#region ..获取GDI路径
		/// <summary>
		/// 获取GDI路径
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

		#region ..获取与之相关的前一个控制点
		/// <summary>
		/// 获取与之相关的前一个控制点
		/// </summary>
		/// <param name="svgPathSegList">单元所在的列表</param>
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

		#region ..公共属性
		/// <summary>
		/// 获取路径数据的文本表达
		/// </summary>
		public override string PathString
		{
			get
			{
				return "Z";
			}
		}
		#endregion

		#region ..获取与之相关的后一个控制点
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
