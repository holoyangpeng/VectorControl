using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using YP.SVG.Interface.Paths;

namespace YP.SVG.Paths
{
	/// <summary>
	/// 实现A ,a
	/// </summary>
	public abstract class SVGPathSegArc:SVGPathSeg
	{
		#region ..构造及消除
		public SVGPathSegArc(float x,float y,float r1,float r2,float angle,bool largeArcFlag,bool sweepFlag)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.x = x;
			this.y = y;
			this.r1 = r1;
			this.r2 = r2;
			this.angle = angle;
			this.largeArcFlag = largeArcFlag;
			this.sweepFlag = sweepFlag;
			
		}
		#endregion

		#region ..私有变量
		float x,y,r1,r2,angle;
		bool largeArcFlag,sweepFlag;
		GraphicsPath gdiPath = new GraphicsPath();
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

		/// <summary>
		/// 定义横半径
		/// </summary>
		public float R1
		{
			get
			{
				return this.r1;
			}
			set
			{
				this.r1 = value;
			}
		}

		/// <summary>
		/// 定义第纵半径
		/// </summary>
		public float R2
		{
			set
			{
				this.r2 = value;
			}
			get
			{
				return this.r2;
			}
		}

		/// <summary>
		/// 定义第二控制点的横坐标
		/// </summary>
		public float Angle
		{
			get
			{
				return this.angle;
			}
			set
			{
				this.angle = value;
			}
		}

		/// <summary>
		/// The value of the large-arc-flag parameter
		/// </summary>
		public bool LargeArcFlag 
		{
			get
			{
				return this.largeArcFlag;
			}
			set
			{
				this.largeArcFlag = value;
			}
		}

		/// <summary>
		/// The value of the sweep-flag parameter. 
		/// </summary>
		public bool SweepFlag
		{
			set
			{
				this.sweepFlag = value;
			}
			get
			{
				return this.sweepFlag;
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
				PointF p = PointF.Empty;
				ISVGPathSeg pre = svgPathList.PreviousSibling(this);
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
			using(GraphicsPath gp = this.CreateGDIPath(lastPoint,svgPathList))
			{
				if(gp != null)
					path.AddPath(gp,true);
				lastPoint = this.GetLastPoint(svgPathList);
			}
		}
		#endregion

		#region ..生成GDI路径
		/// <summary>
		/// 生成GDI路径
		/// </summary>
		GraphicsPath CreateGDIPath(PointF lastPoint,Interface.Paths.ISVGPathSegList svgPathList)
		{
			PointF p = this.GetLastPoint(svgPathList);
			GraphicsPath path = null;
			if (lastPoint.Equals(p)) 
			{
				// If the endpoints (x, y) and (x0, y0) are identical, then this
				// is equivalent to omitting the elliptical arc segment entirely.
			}
			else if (this.r1 == 0 || this.r2 == 0) 
			{
				path = new GraphicsPath();
				// Ensure radii are valid
				path.AddLine(lastPoint, p);
			}
			else
			{
				float x = 0,y = 0,width = 0,height = 0,startangle = 0,sweepangle = 0;
				this.CalcuteArc(ref x,ref y,ref width,ref height,ref startangle,ref sweepangle,svgPathList);

				path = new GraphicsPath();
				path.StartFigure();
				path.AddArc(x,y,width,height,startangle,sweepangle);
				
				float cx = x + width/2f;
				float cy = y + height /2f;

				using(Matrix matrix = new Matrix())
				{
					matrix.Translate(-cx,-cy);
					path.Transform(matrix);

					matrix.Reset();
					matrix.Rotate(this.angle );
					path.Transform(matrix);

					matrix.Reset();
					matrix.Translate(cx,cy);
					path.Transform(matrix);
				}
			}

			return path;
		}
		#endregion

		#region ..计算弧线参数
		/// <summary>
		/// 计算弧线参数
		/// </summary>
		/// <param name="x">矩形区域左上角的横坐标，该区域定义将要从中绘制弧线的椭圆</param>
		/// <param name="y">矩形区域左上角的纵坐标，该区域定义将要从中绘制弧线的椭圆</param>
		/// <param name="width">矩形区域宽度，该区域定义将要从中绘制弧线的椭圆</param>
		/// <param name="height">矩形区域高度，该区域定义将要从中绘制弧线的椭圆</param>
		/// <param name="startangle">弧线的其实角度，以度为单位从X轴顺时针开始测量</param>
		/// <param name="sweepangle">stargangle 和弧线末尾之间的角度</param>
		/// <param name="startPoint">弧线起始点</param>
		public void CalcuteArc(ref float x,ref float y,ref float width,ref float height,ref float startangle,ref float sweepangle,Interface.Paths.ISVGPathSegList svgPathList)
		{
			Interface.Paths.ISVGPathSeg prevSeg = svgPathList.PreviousSibling(this);
			if(prevSeg == null)
				throw new SVGException("无效的路径数据",SVGExceptionType.SVG_INVALID_VALUE_ERR,null);

			PointF startPoint = prevSeg.GetLastPoint(svgPathList);
			PointF endPoint = this.GetLastPoint(svgPathList);

			CalcuteArc(ref x,ref y,ref width,ref height,ref startangle,ref sweepangle,startPoint,endPoint,this.R1,this.R2,this.Angle,this.LargeArcFlag,this.SweepFlag);
			
		}
		#endregion

		#region ..计算弧线参数
		/// <summary>
		/// 计算弧线参数
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="startangle"></param>
		/// <param name="sweepangle"></param>
		/// <param name="startPoint"></param>
		/// <param name="endPoint"></param>
		public static void CalcuteArc(ref float x,ref float y,ref float width,ref float height,ref float startangle,ref float sweepangle,PointF startPoint,PointF endPoint,float R1,float R2,float Angle,bool LargeArcFlag,bool SweepFlag)
		{
			float x0 = startPoint.X;
			float y0 = startPoint.Y;

			float xx = endPoint.X;
			float yy = endPoint.Y;

			// Compute the half distance between the current and the final point
			double dx2 = (x0 - xx) / 2.0;
			double dy2 = (y0 - yy) / 2.0;

			// Convert angle from degrees to radians
			double radAngle = Angle * Math.PI / 180;
			double cosAngle = Math.Cos(radAngle);
			double sinAngle = Math.Sin(radAngle);

			//
			// Step 1 : Compute (x1, y1)
			//
			double x1 = (cosAngle * dx2 + sinAngle * dy2);
			double y1 = (-sinAngle * dx2 + cosAngle * dy2);
			// Ensure radii are large enough

			double rx = Math.Abs(R1);
			double ry = Math.Abs(R2);

			double Prx = rx * rx;
			double Pry = ry * ry;
			double Px1 = x1 * x1;
			double Py1 = y1 * y1;

			// check that radii are large enough
			double radiiCheck = Px1/Prx + Py1/Pry;
			if (radiiCheck > 1) 
			{
				rx = Math.Sqrt(radiiCheck) * rx;
				ry = Math.Sqrt(radiiCheck) * ry;
				Prx = rx * rx;
				Pry = ry * ry;
			}

			//
			// Step 2 : Compute (cx1, cy1)
			//
			double sign = (LargeArcFlag == SweepFlag) ? -1 : 1;
			double sq = ((Prx*Pry)-(Prx*Py1)-(Pry*Px1)) / ((Prx*Py1)+(Pry*Px1));
			sq = (sq < 0) ? 0 : sq;
			double coef = (sign * Math.Sqrt(sq));
			double cx1 = coef * ((rx * y1) / ry);
			double cy1 = coef * -((ry * x1) / rx);

			//
			// Step 3 : Compute (cx, cy) from (cx1, cy1)
			//
			double sx2 = (x0 + xx) / 2.0;
			double sy2 = (y0 + yy) / 2.0;
			double cx = sx2 + (cosAngle * cx1 - sinAngle * cy1);
			double cy = sy2 + (sinAngle * cx1 + cosAngle * cy1);

			//
			// Step 4 : Compute the angleStart (angle1) and the angleExtent (dangle)
			//
			double ux = (x1 - cx1); // rx;
			double uy = (y1 - cy1); // ry;
			double vx = (-x1 - cx1); // rx;
			double vy = (-y1 - cy1); // ry;
			double p, n;
			// Compute the angle start
			n = Math.Sqrt((ux * ux) + (uy * uy));
			p = ux; // (1 * ux) + (0 * uy)
			sign = (uy < 0) ? -1d : 1d;
			double angleStart = sign * Math.Acos(p / n);
			angleStart = angleStart * 180 / Math.PI;

			// Compute the angle extent
			n = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
			p = ux * vx + uy * vy;
			sign = (ux * vy - uy * vx < 0) ? -1d : 1d;
			double angleExtent = sign * Math.Acos(p / n);
			angleExtent = angleExtent * 180 / Math.PI;

			if(!SweepFlag && angleExtent > 0) 
			{
				angleExtent -= 360f;
			} 
			else if (SweepFlag && angleExtent < 0) 
			{
				angleExtent += 360f;
			}
			angleExtent %= 360f;
			angleStart %= 360f;

			startangle = (float)angleStart;
			sweepangle = (float)angleExtent;
			x = (float)(cx - rx);
			y = (float)(cy - ry);
			width = 2 * (float)rx;
			height = 2 * (float)ry;
		}
		#endregion

		#region ..转换NormalSVGPathSeg
		/// <summary>
		/// 转换为普通的svgPathSeg
		/// </summary>
		/// <param name="svgPathList">路径单元列表</param>
		/// <returns></returns>
		public override SVGPathSeg ConvertToNormal(Interface.Paths.ISVGPathSegList svgPathList)
		{
			PointF p = this.GetLastPoint(svgPathList);
			return new SVGPathSegArcAbs(p.X,p.Y,this.r1,this.r2,this.angle,this.largeArcFlag,this.sweepFlag);
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
			Interface.Paths.ISVGPathSeg pre = svgPathSegList.PreviousSibling(this);
			if(pre == null)
				return 0;
			GraphicsPath path = this.CreateGDIPath(pre.GetLastPoint(svgPathSegList),svgPathSegList);
			if(path.PointCount == 4)
			{
				PointF[] ps = path.PathPoints;
				return (float)Paths.CalculateLength.CalculateBezierLengthAtT(ps[0],ps[1],ps[2],ps[3]);
			}
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
			return this.CreateGDIPath(seg.GetLastPoint(svgPathSegList),svgPathSegList);
		}
		#endregion
	}
}
