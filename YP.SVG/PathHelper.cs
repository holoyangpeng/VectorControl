using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using YP.SVG.Common;

namespace YP.SVG
{
    /// <summary>
    /// PathHelper ��ժҪ˵����
    /// </summary>
    public class PathHelper
    {
        #region ..GetTheCrossPoints
        /// <summary>
        /// ��ȡֱ����ָ��·���Ľ���
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="path"></param>
        /// <param name="basePoint">�����жϾ���Ļ�׼���꣬�����Ϊ�գ�ȡ����������</param>
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

                            #region ..����
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
        /// �ж�·���Ƿ���
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
        /// �ж����߶�λ�ù�ϵ�����������(�������)��
        /// [���غ�] ��ȫ�غ�(6)��1���˵��غ��ҹ���(5)�������غ�(4)
        /// [���غ�] ���˵��ཻ(3)����������(2)������(1)���޽�(0)����������(-1)
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
            //��֤����p1!=p2��p3!=p4
            if (p1 == p2 || p3 == p4)
                return -1; //����-1����������һ���߶���β�غϣ����ܹ����߶�

            //Ϊ�������㣬��֤���߶ε������ǰ���յ��ں�
            if (PointUpper(p1, p2))
                OperatorHelper<PointF>.Swap(ref p1, ref p2);

            if (PointUpper(p3, p4))
                OperatorHelper<PointF>.Swap(ref p3, ref p4);

            //�ж����߶��Ƿ���ȫ�غ�
            if (PointEqual(p1, p2) && PointEqual(p2, p4))
                return 6;

            //������߶ι��ɵ�����
            PointF v1 = new PointF(p2.X - p1.X, p2.Y - p1.Y), v2 = new PointF(p4.X - p3.X, p4.Y - p3.Y);

            //�������������ƽ��ʱ���Ϊ
            float cross = PointCross(v1, v2);

            //�������غ�
            if (PointEqual(p1, p3))
            {
                result = p1;
                //����غ��ҹ���(ƽ��)����5����ƽ�����ڶ˵㣬����3
                return (FloatEqual(cross, 0) ? 5 : 3);
            }

            //����յ��غ�
            if (PointEqual(p2, p4))
            {
                result = p2;
                //�յ��غ��ҹ���(ƽ��)����5����ƽ�����ڶ˵㣬����3
                return (FloatEqual(cross, 0) ? 5 : 3);
            }

            //������߶���β����
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

            //���������жϣ���β�����ص���������ų���
            //���߶ΰ���������������߶�1�����ϴ������߶ν���
            if (PointUpper(p1, p3))
            {
                OperatorHelper<PointF>.Swap(ref p1, ref p3);
                OperatorHelper<PointF>.Swap(ref p2, ref p4);

                //����ԭ�ȼ���������������
                OperatorHelper<PointF>.Swap(ref v1, ref v2);
                cross = PointCross(v1, v2);
            }

            //�������߶�ƽ�е����
            if (FloatEqual(cross, 0))
            {
                //������v1(p1, p2)��vs(p1,p3)��������ж��Ƿ���
                PointF vs = new PointF(p3.X - p1.X, p3.Y - p1.Y);

                //���Ϊ0����ƽ���߶ι��ߣ������ж��Ƿ����غϲ���
                if (FloatEqual(PointCross(v1, vs), 0))
                {
                    //ǰһ���ߵ��յ���ں�һ���ߵ���㣬���ж������غ�
                    if (PointUpper(p2, p3))
                    {
                        result = p3;
                        return 4; //����ֵ4�����߶β����غ�
                    }
                }

                //�����㲻���ߣ���������ƽ���߶αز����ߡ�
                //�����߻��ߵ����غϵ�ƽ���߾��޽���
                return 0;
            }

            //����Ϊ��ƽ�е�������Ƚ��п����ų�����
            //x���������򣬿�ֱ�ӱȽϡ�y����Ҫ�������߶ε�������Сֵ
            float ymax1 = p1.Y, ymin1 = p2.Y, ymax2 = p3.Y, ymin2 = p4.Y;
            if (ymax1 < ymin1)
                OperatorHelper<float>.Swap(ref ymax1, ref ymin1);

            if (ymax2 < ymin2)
                OperatorHelper<float>.Swap(ref ymax2, ref ymin2);

            //��������߶�Ϊ�Խ��ߵľ��β��ཻ�����޽���
            if (FloatUpper(p1.X, p4.X) || FloatUpper(p3.X, p2.X) || FloatUpper(ymin2, ymax1) || FloatUpper(ymin1, ymax2))
                return 0;

            //������п�������
            PointF vs1 = new PointF(p1.X - p3.X, p1.Y - p3.Y), vs2 = new PointF(p2.X - p3.X, p2.Y - p3.Y);
            PointF vt1 = new PointF(p3.X - p1.X, p3.Y - p1.Y), vt2 = new PointF(p4.X - p1.X, p4.Y - p1.Y);
            float s1v2 = PointCross(vs1, v2), s2v2 = PointCross(vs2, v2), t1v1 = PointCross(vt1, v1), t2v1 = PointCross(vt2, v1);

            //�����������ж���������
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

            //δ�������ϣ����ж��Ƿ��ཻ
            if (s1v2 * s2v2 > 0 || t1v1 * t2v1 > 0)
                return 0;

            //����Ϊ�ཻ��������㷨����ĵ�
            //�����������ʽ������������
            float conA = p1.X * v1.Y - p1.Y * v1.X;
            float conB = p3.X * v2.Y - p3.Y * v2.X;

            //��������ʽD1��D2��ֵ������ϵ������ʽ��ֵ���õ���������
            result.X = (conB * v1.X - conA * v2.X) / cross;
            result.Y = (conB * v1.Y - conA * v2.Y) / cross;
            //��������1
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

        #region ..��ȡ����֮�����б��
        /// <summary>
        /// ��ȡ����֮������߽�
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