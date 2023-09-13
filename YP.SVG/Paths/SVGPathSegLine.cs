using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using YP.SVG.Interface.Paths;

namespace YP.SVG.Paths
{
	/// <summary>
	/// 实现L,l,H,h,V,v
	/// </summary>
	public abstract class SVGPathSegLine:SVGPathSeg
	{
		#region ..构造及消除
		public SVGPathSegLine(float x,float y)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.x = x;
			this.y = y;
		}
		#endregion

		#region ..私有变量
		float x,y;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 定义终点的横坐标
		/// </summary>
		public float X
		{
			set
			{
				this.x = value;
			}
			get
			{
				return this.x;
			}
		}

		/// <summary>
		/// 定义终点的相对纵坐标
		/// </summary>
		public float Y
		{
			set
			{
				this.y = value;
			}
			get
			{
				return this.y;
			}
		}
		#endregion

		#region ..将路径单元附加到指定路径末尾
		/// <summary>
		/// 将路径单元附加到指定路径末尾
		/// </summary>
		/// <param name="path">路径</param>
		/// <param name="lastPoint">路径最末点</param>
		public override void AppendGraphicsPath(GraphicsPath path,ref PointF lastPoint,Interface.Paths.ISVGPathSegList svgPathList)
		{
			PointF p = this.GetLastPoint(svgPathList);
			path.AddLine(lastPoint,p);
			lastPoint = p;
		}
		#endregion

		#region ..转换NormalSVGPathSeg
		/// <summary>
		/// 转换为普通的svgPathSeg
		/// </summary>
		/// <param name="svgPathList">路径单元列表</param>
		/// <returns></returns>
		public override SVGPathSeg ConvertToNormal(Interface.Paths.ISVGPathSegList svgPathSegList)
		{
			PointF endPoint = this.GetLastPoint(svgPathSegList);
			return new SVGPathSegLinetoAbs(endPoint.X,endPoint.Y );
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
			if(this.pathlength == -1)
			{
				Interface.Paths.ISVGPathSeg pre = svgPathSegList.PreviousSibling(this);
				if(pre == null)
					this.pathlength = 0;
				PointF p1 = pre.GetLastPoint(svgPathSegList);
				PointF p2 = this.GetLastPoint(svgPathSegList);
				this.pathlength = (float)Paths.CalculateLength.CalculateLineLengthAtT(1,p1,p2);
			}
			return this.pathlength;
		}
		#endregion

		#region ..获取路径终点
		/// <summary>
		/// 获取路径中终点
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public override PointF GetLastPoint(Interface.Paths.ISVGPathSegList svgPathList)
		{
			return PointF.Empty;
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
			GraphicsPath path = new GraphicsPath();
			path.AddLine(seg.GetLastPoint(svgPathSegList),this.GetLastPoint(svgPathSegList));
			return path;
		}
		#endregion
	}
}
