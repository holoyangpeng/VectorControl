using System;
using System.Drawing.Drawing2D;
using System.Drawing;

using YP.SVG.Interface.Paths;

namespace YP.SVG.Paths
{
	/// <summary>
	/// 实现M 和 m
	/// </summary>
	public abstract class SVGPathSegMove:SVGPathSeg
	{
		#region ..构造及消除
		public SVGPathSegMove(float x,float y)
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
		public PointF relativePreControl = PointF.Empty;
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
		/// 定义终点的纵坐标
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

		#region ..获取路径终点
		/// <summary>
		/// 获取路径中终点
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public override PointF GetLastPoint(Interface.Paths.ISVGPathSegList svgPathList)
		{
			if(!this.Relative)
				return new PointF(this.x,this.y);
			else
			{
				ISVGPathSeg pre = svgPathList.PreviousSibling(this);
				PointF p = PointF.Empty;
				if(pre != null)
				{
					p = pre.GetLastPoint(svgPathList);
				}
				return new PointF(p.X + this.x,p.Y + this.y);
			}
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
			path.StartFigure();
			lastPoint = this.GetLastPoint(svgPathList);
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
			return new SVGPathSegMovetoAbs(endPoint.X,endPoint.Y );
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

		#region ..获取与之相关的前一个控制点
		/// <summary>
		/// 获取与之相关的前一个控制点
		/// </summary>
		/// <param name="svgPathSegList">单元所在的列表</param>
		/// <returns></returns>
		public override PointF GetRelativePreControl(Interface.Paths.ISVGPathSegList svgPathSegList)
		{
			YP.SVG.Paths.SVGPathSegClosePath close = (svgPathSegList as SVGPathSegList).GetRelativeClosePathSeg(this);
			if(close != null)
			{
				YP.SVG.Paths.SVGPathSegCurve curve = svgPathSegList.PreviousSibling(close) as YP.SVG.Paths.SVGPathSegCurve;
				if(curve != null && InPoint(this.GetLastPoint(svgPathSegList),curve.GetLastPoint(svgPathSegList)))
					return curve.GetSecondControl(svgPathSegList);
				return PointF.Empty;
			}
			return this.relativePreControl;
		}
		#endregion

		#region ..设置与之相关的前一个控制点
		/// <summary>
		/// 设置与之相关的前一个控制点
		/// </summary>
		/// <returns></returns>
		public void SetRelativePreControl(PointF point)
		{
			this.relativePreControl = point;
		}
		#endregion

		#region ..获取GDI路径
		/// <summary>
		/// 获取GDI路径
		/// </summary>
		/// <returns></returns>
		public override GraphicsPath GetGDIPath(YP.SVG.Paths.SVGPathSegList svgPathSegList)
		{
			return new GraphicsPath();
		}
		#endregion
	}
}
