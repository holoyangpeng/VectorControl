using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using YP.SVG.Common;

namespace YP.SVG
{
    /// <summary>
    /// PathHelper 的摘要说明。
    /// </summary>
    public class PathHelper
    {
        #region ..GetTheCrossPoints
        /// <summary>
        /// 获取直线与指定路径的交点
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="path"></param>
        /// <param name="basePoint">用于判断距离的基准坐标，如果不为空，取离距离最近者</param>
        /// <returns></returns>
        public static PointF[] GetTheCrossPoints(PointF start, PointF end, GraphicsPath path, PointF basePoint)
        {
            if (path == null || start == end)
                return null;
            try
            {
                List<PointF> points = new List<PointF>();
                using (System.Drawing.Drawing2D.GraphicsPath path1 = new GraphicsPath())
                {
                    using (Pen pen = new Pen(Color.White, 0.1f))
                    {
                        path1.AddLine(start, end);
                        pen.Alignment = PenAlignment.Center;
                        path1.Widen(pen);
                        using (Region rg = new Region(path))
                        {
                            rg.Intersect(path1);
                            RectangleF[] rects = rg.GetRegionScans(new Matrix());

                            #region ..计算
                            if (rects.Length > 0)
                            {
                                path1.Reset();
                                path1.AddRectangles(rects);
                                RectangleF rect = path1.GetBounds();
                                PointF[] ps = new PointF[] { rect.Location, new PointF(rect.Right, rect.Top), new PointF(rect.Right, rect.Bottom), new PointF(rect.X, rect.Bottom) };

                                float distance = 0;
                                PointF? crossPoint = null;
                                foreach (PointF p in ps)
                                {
                                    float dis = Distance(p, start) + Distance(p, end);
                                    if (!crossPoint.HasValue || dis < distance)
                                    {
                                        crossPoint = p;
                                        distance = dis;
                                    }
                                }

                                int index = Array.IndexOf(ps, crossPoint.Value);
                                int index1 = index;
                                switch (index)
                                {
                                    case 0:
                                        index1 = 2;
                                        break;
                                    case 1:
                                        index1 = 3;
                                        break;
                                    case 2:
                                        index1 = 0;
                                        break;
                                    case 3:
                                        index1 = 1;
                                        break;
                                }
                                if (index >= 0 && index1 >= 0)
                                {
                                    PointF p = ps[index];
                                    PointF p1 = ps[index1];

                                    return new PointF[] { Distance(p, basePoint) < Distance(p1, basePoint) ? p : p1 };
                                }
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (System.Exception e1)
            {
                Console.Write(e1.Message);
            }

            return null;
        }
        #endregion

        #region ..IsClosePath
        /// <summary>
        /// 判断路径是否封闭
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsClosePath(System.Drawing.Drawing2D.GraphicsPath path)
        {
            if (path != null)
            {
                byte[] types = path.PathTypes;
                if (types.Length > 0)
                {
                    System.Drawing.Drawing2D.PathPointType type = (System.Drawing.Drawing2D.PathPointType)types[types.Length - 1];
                    if ((type & System.Drawing.Drawing2D.PathPointType.CloseSubpath) == System.Drawing.Drawing2D.PathPointType.CloseSubpath)
                        return true;
                }
            }
            return false;
        }
        #endregion

        #region ..Float operator
        public static bool FloatEqual(float a, float b)
        {
            return Math.Abs(a - b) < 0.5f;
        }

        public static bool FloatUpper(float a, float b)
        {
            return a - b > 0.5f;
        }

        public static bool PointEqual(PointF p1, PointF p2)
        {
            return FloatEqual(p1.X, p2.X) && FloatEqual(p1.Y, p2.Y);
        }

        public static bool PointUpper(PointF p1, PointF p2)
        {
            return p1.X > p2.X || (FloatEqual(p1.X, p2.X) && p1.Y > p2.Y);
        }

        public static float PointCross(PointF p1, PointF p2)
        {
            return p1.X * p2.Y - p1.Y * p2.X;
        }
        #endregion

        #region ..LineIntersection
        /// <summary>
        /// 判定两线段位置关系，并求出交点(如果存在)。
        /// [有重合] 完全重合(6)，1个端点重合且共线(5)，部分重合(4)
        /// [无重合] 两端点相交(3)，交于线上(2)，正交(1)，无交(0)，参数错误(-1)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static int LineIntersection(PointF p1, PointF p2, PointF p3, PointF p4, out PointF result)
        {
            result = Point.Empty;
            //保证参数p1!=p2，p3!=p4
            if (p1 == p2 || p3 == p4)
                return -1; //返回-1代表至少有一条线段首尾重合，不能构成线段

            //为方便运算，保证各线段的起点在前，终点在后。
            if (PointUpper(p1, p2))
                OperatorHelper<PointF>.Swap(ref p1, ref p2);

            if (PointUpper(p3, p4))
                OperatorHelper<PointF>.Swap(ref p3, ref p4);

            //判定两线段是否完全重合
            if (PointEqual(p1, p2) && PointEqual(p2, p4))
                return 6;

            //求出两线段构成的向量
            PointF v1 = new PointF(p2.X - p1.X, p2.Y - p1.Y), v2 = new PointF(p4.X - p3.X, p4.Y - p3.Y);

            //求两向量外积，平行时外积为
            float cross = PointCross(v1, v2);

            //如果起点重合
            if (PointEqual(p1, p3))
            {
                result = p1;
                //起点重合且共线(平行)返回5；不平行则交于端点，返回3
                return (FloatEqual(cross, 0) ? 5 : 3);
            }

            //如果终点重合
            if (PointEqual(p2, p4))
            {
                result = p2;
                //终点重合且共线(平行)返回5；不平行则交于端点，返回3
                return (FloatEqual(cross, 0) ? 5 : 3);
            }

            //如果两线端首尾相连
            if (PointEqual(p1, p4))
            {
                result = p1;
                return 3;
            }

            if (PointEqual(p2, p3))
            {
                result = p2;
                return 3;
            }

            //经过以上判断，首尾点相重的情况都被排除了
            //将线段按起点坐标排序。若线段1的起点较大，则将两线段交换
            if (PointUpper(p1, p3))
            {
                OperatorHelper<PointF>.Swap(ref p1, ref p3);
                OperatorHelper<PointF>.Swap(ref p2, ref p4);

                //更新原先计算的向量及其外积
                OperatorHelper<PointF>.Swap(ref v1, ref v2);
                cross = PointCross(v1, v2);
            }

            //处理两线段平行的情况
            if (FloatEqual(cross, 0))
            {
                //做向量v1(p1, p2)和vs(p1,p3)的外积，判定是否共线
                PointF vs = new PointF(p3.X - p1.X, p3.Y - p1.Y);

                //外积为0则两平行线段共线，下面判定是否有重合部分
                if (FloatEqual(PointCross(v1, vs), 0))
                {
                    //前一条线的终点大于后一条线的起点，则判定存在重合
                    if (PointUpper(p2, p3))
                    {
                        result = p3;
                        return 4; //返回值4代表线段部分重合
                    }
                }

                //若三点不共线，则这两条平行线段必不共线。
                //不共线或共线但无重合的平行线均无交点
                return 0;
            }

            //以下为不平行的情况，先进行快速排斥试验
            //x坐标已有序，可直接比较。y坐标要先求两线段的最大和最小值
            float ymax1 = p1.Y, ymin1 = p2.Y, ymax2 = p3.Y, ymin2 = p4.Y;
            if (ymax1 < ymin1)
                OperatorHelper<float>.Swap(ref ymax1, ref ymin1);

            if (ymax2 < ymin2)
                OperatorHelper<float>.Swap(ref ymax2, ref ymin2);

            //如果以两线段为对角线的矩形不相交，则无交点
            if (FloatUpper(p1.X, p4.X) || FloatUpper(p3.X, p2.X) || FloatUpper(ymin2, ymax1) || FloatUpper(ymin1, ymax2))
                return 0;

            //下面进行跨立试验
            PointF vs1 = new PointF(p1.X - p3.X, p1.Y - p3.Y), vs2 = new PointF(p2.X - p3.X, p2.Y - p3.Y);
            PointF vt1 = new PointF(p3.X - p1.X, p3.Y - p1.Y), vt2 = new PointF(p4.X - p1.X, p4.Y - p1.Y);
            float s1v2 = PointCross(vs1, v2), s2v2 = PointCross(vs2, v2), t1v1 = PointCross(vt1, v1), t2v1 = PointCross(vt2, v1);

            //根据外积结果判定否交于线上
            if (FloatEqual(s1v2, 0) && PointUpper(p4, p1) && PointUpper(p1, p3))
            {
                result = p1;
                return 2;
            }

            if (FloatEqual(s2v2, 0) && PointUpper(p4, p2) && PointUpper(p2, p3))
            {
                result = p2;
                return 2;
            }

            if (FloatEqual(t1v1, 0) && PointUpper(p2, p3) && PointUpper(p3, p1))
            {
                result = p3;
                return 2;
            }

            if (FloatEqual(t2v1, 0) && PointUpper(p2, p4) && PointUpper(p4, p1))
            {
                result = p4;
                return 2;
            }

            //未交于线上，则判定是否相交
            if (s1v2 * s2v2 > 0 || t1v1 * t2v1 > 0)
                return 0;

            //以下为相交的情况，算法详见文档
            //计算二阶行列式的两个常数项
            float conA = p1.X * v1.Y - p1.Y * v1.X;
            float conB = p3.X * v2.Y - p3.Y * v2.X;

            //计算行列式D1和D2的值，除以系数行列式的值，得到交点坐标
            result.X = (conB * v1.X - conA * v2.X) / cross;
            result.Y = (conB * v1.Y - conA * v2.Y) / cross;
            //正交返回1
            return 1;
        }
        #endregion

        #region ..Distance
        public static float Distance(PointF p1, PointF p2)
        {
            return (float)Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        public static float Distance(PointF[] points)
        {
            if (points == null || points.Length < 2)
                return 0;
            float distance = 0;
            for (int i = 1; i < points.Length; i++)
            {
                distance += Distance(points[i - 1], points[i]);
            }
            return distance;
        }
        #endregion

        #region ..获取两点之间的倾斜角
        /// <summary>
        /// 获取两点之间的切线角
        /// </summary>
        /// <returns></returns>
        public static float GetAngle(PointF startpoint, PointF endpoint)
        {
            double r = Math.Sqrt(Math.Pow(startpoint.X - endpoint.X, 2) + Math.Pow(startpoint.Y - endpoint.Y, 2));
            double startangle = endpoint.Y - startpoint.Y;
            startangle = Math.Asin(startangle / r) / Math.PI * 180;
            if (startpoint.X > endpoint.X)
                startangle = 180 - startangle;

            if (startangle < 0)
                startangle += 360;
            return (float)startangle;
        }
        #endregion

        #region ..GetBounds
        public static DataType.SVGViewport GetViewport(Interface.ISVGPathable pathable)
        {
            if (pathable != null && pathable.GPath != null && pathable.GPath.PointCount > 1)
            {
                RectangleF bounds = RectangleF.Empty;
                float angle = 0;
                if (pathable is SVGTransformableElement)
                {
                    bounds = pathable.GPath.GetBounds();
                    //PointF[] ps = new PointF[] { bounds.Location, new PointF(bounds.Right, bounds.Top), 
                    //    new PointF(bounds.Right, bounds.Bottom), new PointF(bounds.Left, bounds.Bottom),
                    //    new PointF((bounds.Left + bounds.Right) /2, (bounds.Top + bounds.Bottom) / 2)};

                    //(pathable as SVGTransformableElement).TotalTransform.TransformPoints(ps);
                    //float width = Distance(ps[0], ps[1]);
                    //float height = Distance(ps[1], ps[2]);
                    //PointF center = ps[ps.Length - 1];
                    //angle = GetAngle(ps[0], ps[1]);
                    //bounds = new RectangleF(center.X - width / 2, center.Y - height / 2, width, height);
                }
                return new DataType.SVGViewport(new DataType.SVGRect(bounds), angle);
            }
            return new DataType.SVGViewport();
        }
        #endregion
    }
}