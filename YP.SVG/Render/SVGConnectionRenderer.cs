using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Render
{
    public class SVGConnectionRenderer:SVGDirectRenderer
    {
        #region ..const
        const int ConnectCrossLength = 4;
        #endregion

        #region ..Constructor
        public SVGConnectionRenderer(BasicShapes.SVGConnectionElement ownerElement)
            : base(ownerElement)
        {
            
        }
        #endregion

        #region ..Draw
        public override void Draw(System.Drawing.Graphics g, SVG.StyleContainer.StyleOperator sp)
        {
            if (this.OwnerDocument.IsStopRender)
                return;

            BasicShapes.SVGConnectionElement connection = this.OwnerElement as BasicShapes.SVGConnectionElement;
            connection.ResetTotalTransform();// = (this.OwnerDocument.RootElement as SVGDom.SVGTransformableElement).TotalTransform.Clone();
            sp.BeginStyleContainer(connection);
            GraphicsPath gp = (connection as SVG.Interface.ISVGPathable).GPath;
            if (!BeforeDrawing(g, sp))
            {
                this.DrawTextBlock(g, sp, this.OwnerElement);
                sp.EndContainer(connection);
                return;
            }

            System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
            System.Drawing.Text.TextRenderingHint hint = g.TextRenderingHint;
            InterpolationMode mode1 = g.InterpolationMode;
            GraphicsContainer c = g.BeginContainer();
            try
            {
                g.ResetClip();
                g.SmoothingMode = mode;
                g.TextRenderingHint = hint;
                g.InterpolationMode = mode1;

                if (sp.ViewVisible && sp.VisualMediaStyle.visiblility != "hidden" && sp.VisualMediaStyle.display != "none")
                {
                    this.AdjustConnectPathToAvoidCross(sp, sp.ConnectPath, gp);
                    SVG.Paint.SVGPaint svgstroke = (SVG.Paint.SVGPaint)sp.StrokeStyle.svgStroke;
                    base.DrawBorder(g, gp, sp.StrokeStyle.strokewidth.Value);
                    this.DrawConnectionPath(g, sp, connection, svgstroke, this.OwnerElement.MarkerStart, this.OwnerElement.MarkerEnd);
                    this.DrawLabel(g, sp);
                }
            }
            catch (System.Exception e1)
            {
                this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e1.Message }, ExceptionLevel.Normal));
            }
            finally
            {
                g.EndContainer(c);
                sp.EndContainer(OwnerElement);
                this.AddToRenderElements(sp);
                this.OwnerElement.CurrentTime = this.OwnerDocument.CurrentTime;
            }
        }
        #endregion

        #region ..DrawBorder
        /// <summary>
        /// 重载DrawBorder，Connect类自己处理DrawBorder 的时机
        /// </summary>
        /// <param name="g"></param>
        /// <param name="path"></param>
        public override void DrawBorder(Graphics g, GraphicsPath path, float strokeWidth)
        {

        }
        #endregion

        #region ..DrawConnect
        public override void DrawConnectPoint(System.Drawing.Graphics g, StyleContainer.StyleOperator sp, bool drawConnectPoint)
        {
        }
        #endregion

        #region ..AdjustConnectPathToAvoidCross
        void AdjustConnectPathToAvoidCross(StyleContainer.StyleOperator sp, GraphicsPath previousConnectPath, GraphicsPath resultPath)
        {
            BasicShapes.SVGConnectionElement connection = this.OwnerElement as BasicShapes.SVGConnectionElement;
            if (!sp.AutoBridgeForConnect.HasValue || !sp.AutoBridgeForConnect.Value || !connection.NeedAdjustPath)
                return;
            connection.NeedAdjustPath = false;
            if (previousConnectPath == null)
            {
                this.OwnerDocument.BeginProcess();
                System.Xml.XmlNodeList nodes = this.OwnerDocument.GetElementsByTagName("connect");
                this.OwnerDocument.EndProcess();
                previousConnectPath = new GraphicsPath();
                if (nodes != null)
                {
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        BasicShapes.SVGConnectionElement cnn = nodes[i] as BasicShapes.SVGConnectionElement;
                        if (cnn == this.OwnerElement)
                            break;

                        previousConnectPath.Reset();
                        if (cnn.TotalTransform.IsIdentity)
                            previousConnectPath.AddPath((cnn as SVG.Interface.ISVGPathable).GPath, false);
                    }
                }
            }

            GraphicsPath gp = (connection as Interface.ISVGPathable).GPath;
            int pointcount = 0;
            if (previousConnectPath != null && previousConnectPath.PointCount >= pointcount + 2)
            {
                List<PointF> points = new List<PointF>();
                List<byte> types = new List<byte>();
                List<float> distances = new List<float>();
                List<PointF> tempPoints = new List<PointF>();
                for (int i = 0; i < gp.PointCount - 1; i++)
                {
                    PointF p1 = gp.PathPoints[i];
                    PointF p2 = gp.PathPoints[i + 1];
                    points.Add(p1);
                    types.Add(gp.PathTypes[i]);
                    distances.Clear();
                    tempPoints.Clear();
                    double k = Math.Atan((p2.Y - p1.Y) / (p2.X - p1.X)) - Math.PI / 2;
                    for (int j = 0; j < previousConnectPath.PointCount - 1 - pointcount; j++)
                    {
                        PointF p3 = previousConnectPath.PathPoints[j];
                        PointF p4 = previousConnectPath.PathPoints[j + 1];

                        if (previousConnectPath.PathTypes[j + 1] != (byte)PathPointType.Line)
                            continue;
                        PointF crossPoint = PointF.Empty;
                        int r = PathHelper.LineIntersection(p1, p2, p3, p4, out crossPoint);

                        if (r == 1)
                        {
                            //计算折线点
                            float d = PathHelper.Distance(p1, p2);
                            float d1 = PathHelper.Distance(p3, p4);
                            float d2 = PathHelper.Distance(p1, crossPoint);
                            float d3 = PathHelper.Distance(p3, crossPoint);

                            float x = p1.X + (d2 - ConnectCrossLength) / d * (p2.X - p1.X);
                            float y = p1.Y + (d2 - ConnectCrossLength) / d * (p2.Y - p1.Y);

                            float x1 = (float)(crossPoint.X + ConnectCrossLength * Math.Cos(k));//p3.X + (d3 + ConnectCrossLength) / d1 * (p4.X - p3.X);
                            float y1 = (float)(crossPoint.Y + ConnectCrossLength * Math.Sin(k));

                            float x2 = p1.X + (d2 + ConnectCrossLength) / d * (p2.X - p1.X);
                            float y2 = p1.Y + (d2 + ConnectCrossLength) / d * (p2.Y - p1.Y);
                            float d4 = PathHelper.Distance(p1, new PointF(x, y));
                            float d5 = PathHelper.Distance(p1, new PointF(x2, y2));

                            tempPoints.Add(new PointF(x, y));
                            types.Add((byte)PathPointType.Line);
                            tempPoints.Add(new PointF(x1, y1));
                            types.Add((byte)PathPointType.Line);
                            tempPoints.Add(new PointF(x2, y2));
                            types.Add((byte)PathPointType.Line);

                            distances.Add(d4);
                            distances.Add(d2);
                            distances.Add(d5);
                        }
                    }

                    PointF[] ps = tempPoints.ToArray();
                    Array.Sort(distances.ToArray(), ps);
                    points.AddRange(ps);
                    points.Add(p2);
                    types.Add(gp.PathTypes[i + 1]);
                }

                if (points.Count != resultPath.PointCount)
                {
                    resultPath.Reset();
                    resultPath.AddPath(new GraphicsPath(points.ToArray(), types.ToArray()), false);
                }
            }
        }
        #endregion

        #region ..DrawConnectionPath
        public void DrawConnectionPath(Graphics g, StyleContainer.StyleOperator sp, BasicShapes.SVGBranchElement branch, SVG.Paint.SVGPaint svgStroke, SVG.ClipAndMask.SVGMarkerElement markerStart, SVG.ClipAndMask.SVGMarkerElement markerEnd)
        {
            if (branch == null || branch.ConnectionPath == null || branch.ConnectionPath.PointCount < 2)
                return;
            GraphicsPath path = branch.ConnectionPath;
            this.OwnerElement.TransformPath(g, path, sp);

            this.DrawShadow(sp, g, path);

            if (!svgStroke.IsEmpty)
            {
                //分支
                if (branch.ParentElement is BasicShapes.SVGBranchElement)
                {
                    if (path.PointCount > 0)
                    {
                        PointF p = path.PathPoints[0];
                        g.FillRectangle(Brushes.Black, p.X - 2, p.Y - 2, 4, 4);
                    }
                }
                if (!sp.BoundView)
                    this.StrokePath(g, svgStroke, path, this.OwnerElement, sp);
                else
                    g.DrawPath(sp.outlinePen, path);
            }

            if (!branch.HasChildBranch)
            {
                if (markerStart != null)
                    markerStart.MarkerStart(g, path, sp);
                if (markerEnd != null)
                    markerEnd.MarkerEnd(g, path, sp);
            }

            this.DrawTextBlock(g, sp, branch);

            foreach (SVGElement child in branch.ChildElements)
            {
                if(child is BasicShapes.SVGBranchElement)
                    this.DrawConnectionPath(g, sp, child as BasicShapes.SVGBranchElement, svgStroke, markerStart, markerEnd);
            }

            branch.CurrentTime = this.OwnerDocument.CurrentTime;
        }
        #endregion
    }
}
