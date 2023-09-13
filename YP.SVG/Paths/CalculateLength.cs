using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// 计算长度
	/// </summary>
	public class CalculateLength
	{
		#region ..构造及消除
		public CalculateLength()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..计算t时刻线段长度
		/// <summary>
		/// 计算t时刻线段长度
		/// </summary>
		/// <param name="t">t时刻</param>
		/// <param name="p1">线段起始断电</param>
		/// <param name="p2">线段末端点</param>
		/// <returns></returns>
		public static double CalculateLineLengthAtT(double t,PointF p1,PointF p2)
		{
			return t * Math.Sqrt(Math.Pow(p2.Y - p1.Y,2) + Math.Pow(p1.X - p2.X,2));
		}
		#endregion

		#region ..求Bezier方程根
		/// <summary>
		/// 求指定X的t
		/// </summary>
		/// <param name="p1"></param>
		/// <param name="p2"></param>
		/// <param name="p3"></param>
		/// <param name="p4"></param>
		/// <param name="x"></param>
		/// <returns></returns>
		public static float CalculatePointFForX(PointF p1,PointF p2,PointF p3,PointF p4,float x)
		{
			float middle = 0;
			for(float i = 0;i<= 1;i+=0.001f)
			{
				float temp = Bezier(p1.X,p2.X,p3.X,p4.X,i);
				if(Math.Abs(temp - x) < Math.Pow(10,-4) || temp > x)
				{
					middle = i;
					break;
				}
			}
			return Bezier(p1.Y,p2.Y,p3.Y,p4.Y,middle);
		}

		static float Bezier(float p1,float p2,float p3,float p4,float t)
		{
			return (float)(Math.Pow(1-t,3) * p1 + 3 * (1-t)*(1-t)*t * p2 + 3 * (1-t)*t*t*p3 + Math.Pow(t,3) *p4);
		}
		#endregion

		#region ..计算Beizer曲线在t时刻的长度，其中p1,p2,p3,p4分别是Beizer曲线的锚点和控制点
		/// <summary>
		/// 计算Beizer曲线在t时刻的长度，其中p1,p2,p3,p4分别是Beizer曲线的锚点和控制点
		/// </summary>
		/// <param name="t">时刻t</param>
		/// <param name="p1">第一锚点</param>
		/// <param name="p2">第一控制点</param>
		/// <param name="p3">第二控制点</param>
		/// <param name="p4">第二锚点</param>
		/// <returns></returns>
		public static double CalculateBezierLengthAtT(PointF p1,PointF p2,PointF p3,PointF p4)
		{
			#region ..注释1
//			float px1 = p1.X;
//			float py1 = p1.Y;
//			float px2 = p4.X;
//			float py2 = p4.Y;
//			float pcx1 = p2.X;
//			float pcy1 = p2.Y;
//			float pcx2 = p3.X;
//			float pcy2 = p3.Y;
//			float x0 = px1;
//			float y0 = py1;
//			int Precision = 50;
//			float t1 = 0,t2,t3,rt1,rt2,rt3;
//			float BLen = 0;
//			for(int i = 1;i<Precision;i++)
//			{
//				t1 = (float)i / (float)Precision;
//				t2 = t1 * t1;
//				t3 = t2 * t1;
//				rt1 = 1 - t1;
//				rt2 = rt1 * rt1;
//				rt3 = rt2 * rt1;
//				float x1 = px1 * rt3 + pcx1 * rt2 * t1 + pcx2 * rt1 * t2 + px2 * t3;
//				float y1 = py1 * rt3 + pcy1 * rt2 * t1 + pcy2 * rt1 * t2 + py2 * t3;
//				float len = (float)Math.Sqrt((x1 - x0) * (x1 - x0) + (y1 - y0) * (y1 - y0));
//				BLen += len;
//				x0 = x1;
//				y1 = y1;
//			}
//			return BLen;
			#endregion

			#region ..注释2
//			double TOLERANCE = 0.000001;
//			double a = 0;
//			double b = 1;
//			int n_limit = 1024;
//
//			PointF k1 = new PointF(-p1.X + 3*(p2.X - p3.X) + p4.X ,-p1.Y + 3*(p2.Y - p3.Y) + p4.Y);
//			PointF k2 = new PointF(3*(p1.X + p3.X) - 6*p2.X,3*(p1.Y + p3.Y) - 6*p2.Y);
//			PointF k3 = new PointF(3*(p2.X - p1.X),3 * (p2.Y - p1.Y));
//			PointF k4 = p1;
//
//			double q1 = (9.0*(Math.Pow(k1.X,2) + Math.Pow(k1.Y,2)));
//			double q2 = 12.0*(k1.X*k2.X + k1.Y * k2.Y);
//			double q3 = (3.0*(k1.X * k3.X + k1.Y * k3.Y) + 4.0*(Math.Pow(k2.X,2) + Math.Pow(k2.Y,2)));
//			double q4 = 4.0*(k2.X * k3.X + k2.Y * k3.Y);
//			double q5 = (Math.Pow(k3.X,2) + Math.Pow(k3.Y ,2));
//
//			int n = 1;
//			double multiplier = (b - a)/6.0;
//			double endsum = F1(a,q1,q2,q3,q4,q5) + F1(b,q1,q2,q3,q4,q5);
//			double interval = (b - a)/2.0;
//			double asum = 0;
//			double bsum = F1(a + interval,q1,q2,q3,q4,q5);
//			double est1 = multiplier * (endsum + 2 * asum + 4 * bsum);
//			double est0 = 2 * est1;
//			while(n < n_limit && (Math.Abs(est1) > 0 && Math.Abs((est1 - est0) / est1) > TOLERANCE)) 
//			{
//				n *= 2;
//				multiplier /= 2;
//				interval /= 2;
//				asum += bsum;
//				bsum = 0;
//				est0 = est1;
//				double interval_div_2n = interval / (2.0 * n);
//
//				for (int i = 1; i < 2 * n; i += 2) 
//				{
//					double t1 = a + i * interval_div_2n;
//					bsum += F1(t1,q1,q2,q3,q4,q5);
//				}
//
//				est1 = multiplier*(endsum + 2*asum + 4*bsum);
//			}
//			return est1;
			#endregion

			float len = 0;
			using(System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
			{
				path.AddBezier(p1,p2,p3,p4);
				path.Flatten();
				PointF[] ps = path.PathData.Points;
			
				for(int i = 1;i<ps.Length;i++)
				{
					PointF temp = ps[i-1];
					PointF temp1= ps[i];
					len += (float)Math.Sqrt((temp1.X - temp.X) * (temp1.X - temp.X) +(temp1.Y - temp.Y) * (temp1.Y - temp.Y));
				}
				ps = null;
			}
			return len;
		}
		#endregion
	}
}
