using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using YP.SVG;
using YP.VectorControl;
using YP.VectorControl.Forms;

namespace YP.SymbolDesigner.Document
{
    public class MatlabDocumentControl:DocumentControl
    {
        #region ..Constructor
        public MatlabDocumentControl():base()
        {
        }

        public MatlabDocumentControl(string filepath)
            : base(filepath)
        {
        }
        #endregion

        #region ..InitializeVectorControl
        protected override void InitializeCanvas()
        {
            base.InitializeCanvas();
            Grid grid = this.canvas.Grid;
            grid.Visible = false;
            this.canvas.Grid = grid;
            this.canvas.ShowConnectablePoint = true;
            this.canvas.TransformBehavior = TransformBehavior.Translate
                | TransformBehavior.Select
                | TransformBehavior.Scale;
            this.canvas.ShowRule = false;
            if (this.canvas != null)
            {
                this.canvas.ElementConnecting += new ElementConnectEventHandler(canvas_ElementConnecting);
                this.canvas.ElementClick += new ElementClickEventHandler(canvas_ElementClick);
                this.canvas.PaintConnectablePoint += new PaintConnectablePointEventHandler(canvas_PaintConnectablePoint);
                this.canvas.ElementDropped += new ElementDroppedEventHandler(canvas_ElementDropped);
            }
        }
        #endregion

        #region ..Dispose
        public override void Dispose()
        {
            base.Dispose();
            if (this.canvas != null)
            {
                this.canvas.ElementDropped -= new ElementDroppedEventHandler(canvas_ElementDropped);
                this.canvas.ElementClick -= new ElementClickEventHandler(canvas_ElementClick);
            }
        }
        #endregion

        #region ..vectorControl_ElementDropped
        void canvas_ElementDropped(object sender, ElementDroppedEventArgs e)
        {
            var baseElement = e.DroppedInstance;
            var mainElm = Helper.MatlabHelper.GetMainElement(e.DroppedInstance as SVGTransformableElement);
            if (e.DroppedInstance is SVG.DocumentStructure.SVGUseElement)
                baseElement = (e.DroppedInstance as SVG.DocumentStructure.SVGUseElement).RefElement;
            //如果包含Label
            if (baseElement != null && baseElement.HasAttribute("label") && mainElm != null)
            {
                var textBlock = this.canvas.Document.CreateSVGTextBlockElement();
                textBlock.SetAttribute("y", "110%");
                textBlock.SetAttribute("height", "10%");
                textBlock.SetAttribute("wrap", "nowrap");
                textBlock.SetAttribute("vertical-align", "top");
                textBlock.SetAttribute("text-align", "center");
                textBlock.SetAttribute("fill", "none");
                textBlock.SetAttribute("stroke", "none");
                textBlock.SetAttribute("font-family", "Microsoft YaHei UI");
                textBlock.SetAttribute("font-size", "8");
                textBlock.InnerText = baseElement.GetAttribute("label");
                mainElm.AppendChild(textBlock);
            }

            
        }
        #endregion

        #region ..vectorControl_ElementClick
        void canvas_ElementClick(object sender, ElementClickEventArgs e)
        {
            if (e.ClickType == MouseClickType.DoubleClick)
            {
                //只有含有标记文本的可以进行编辑
                if (e.Element is SVG.DocumentStructure.SVGUseElement && e.Element.GetElementsByTagName("textBlock").Count == 0)
                    e.Bubble = false;
            }
        }
        #endregion

        #region ..vectorControl_PaintConnectablePoint
        //自己绘制连接点
        void canvas_PaintConnectablePoint(object sender, PaintConnectablePointEventArgs e)
        {
            e.OwnerDraw = true;
            int r = 3;
            PointF point = e.Point;
            if (this.IsAnchorUsed(e.Element, e.ConnectablePointIndex))
                return;
            //if (e.RelativePoint.X == 0)
            //{
            //    e.Graphics.DrawLine(Pens.Black, point.X - r - 1, point.Y - r, point.X-1, point.Y);
            //    e.Graphics.DrawLine(Pens.Black, point.X - r - 1, point.Y + r, point.X - 1, point.Y);
            //}
            //else if(e.RelativePoint.X == 100)
            //{
            //    e.Graphics.DrawLine(Pens.Black, point.X + 1, point.Y - r, point.X + r + 1, point.Y);
            //    e.Graphics.DrawLine(Pens.Black, point.X + 1, point.Y + r, point.X + r + 1, point.Y);
            //}
            //else if (e.RelativePoint.Y == 0)
            //{
            //    e.Graphics.DrawLine(Pens.Black, point.X - r, point.Y - 1, point.X, point.Y - r - 1);
            //    e.Graphics.DrawLine(Pens.Black, point.X + r, point.Y - 1, point.X, point.Y - r - 1);
            //}
            //else if (e.RelativePoint.Y == 100)
            //{
            //    e.Graphics.DrawLine(Pens.Black, point.X - r, point.Y + r + 1, point.X, point.Y +1);
            //    e.Graphics.DrawLine(Pens.Black, point.X + r, point.Y + r + 1, point.X, point.Y +1);
            //}
        }
        #endregion

        #region ..vectorControl_ElementConnecting
        bool canvas_ElementConnecting(object sender, ElementConnectEventArgs e)
        {
            bool result = false;
            //输出
            //StartElement 和Branch都指向Out端子
            if ((e.Type & ConnectionTargetType.StartElement) == ConnectionTargetType.StartElement)
            {
                if (e.AnchorIndex < 0)
                    return false;
                if (e.TargetElement != null && e.TargetElement.RelativeConnectionPoints != null && e.TargetElement.RelativeConnectionPoints.Length > e.AnchorIndex)
                {
                    PointF p = e.TargetElement.RelativeConnectionPoints[e.AnchorIndex];
                    result = result || p.X == 100 || p.Y == 0;
                }
            }
            //输入端子
            if ((e.Type & ConnectionTargetType.EndElement) == ConnectionTargetType.EndElement)
            {
                if (e.AnchorIndex < 0)
                    return false;
                if (e.TargetElement != null && e.TargetElement.RelativeConnectionPoints != null && e.TargetElement.RelativeConnectionPoints.Length > e.AnchorIndex)
                {
                    PointF p = e.TargetElement.RelativeConnectionPoints[e.AnchorIndex];
                    result = result ||  p.X == 0 || p.Y == 100;
                }
            }

            //分支
            if ((e.Type & ConnectionTargetType.Branch) == ConnectionTargetType.Branch)
            {
                result = true;
            }

            return result;
        }
        #endregion

        #region ..IsAnchorUsed
        bool IsAnchorUsed(SVGTransformableElement element, int index)
        {
            var connections = element.GetAllConnections();
            if (connections != null)
            {
                foreach (SVG.BasicShapes.SVGBranchElement cnn in connections)
                    if (cnn.EndElement == element || cnn.OwnerConnection.StartElement == element)
                        return true;
            }
            return false;
        }
        #endregion
    }
}
