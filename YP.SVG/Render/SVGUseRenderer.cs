using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Render
{
    public class SVGUseRenderer:SVGDirectRenderer
    {
        #region ..Constructor
        public SVGUseRenderer(SVG.DocumentStructure.SVGUseElement ownerElement)
            : base(ownerElement)
        {
            
        }
        #endregion

        #region ..DrawShadow
        public override void DrawShadow(YP.SVG.StyleContainer.StyleOperator sp, Graphics g, GraphicsPath shadowPath)
        {
            if (sp.DrawShadow && shadowPath != null)
            {
                using (GraphicsPath path = shadowPath.Clone() as GraphicsPath)
                {
                    path.Transform(this.OwnerElement.TotalTransform);
                    path.Transform(sp.coordTransform);
                    base.DrawShadow(sp, g, path);
                }
            }
        }
        #endregion

        #region ..绘制元素
        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="g">画布</param>
        /// <param name="sp">类型容器</param>
        public override void Draw(Graphics g, StyleContainer.StyleOperator sp)
        {
            if (this.OwnerDocument.IsStopRender)
                return;

            DocumentStructure.SVGUseElement use = this.OwnerElement as DocumentStructure.SVGUseElement;
            GraphicsPath gp = (use as SVG.Interface.ISVGPathable).GPath;
            if (gp != null)
            {
                if (!BeforeDrawing(g, sp))
                {
                    this.DrawTextBlock(g, sp, this.OwnerElement);
                    return;
                }
            }

            DocumentStructure.SVGElementInstance instance = (DocumentStructure.SVGElementInstance)use.InstanceRoot;

            bool old1 = sp.AddRender;
            sp.AddRender = false;
            if (instance != null)
            {
                YP.SVG.DocumentStructure.SVGGElement svgg = instance.BackGroundDrawElement;
                if (g != null)
                {
                    System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
                    System.Drawing.Text.TextRenderingHint hint = g.TextRenderingHint;
                    System.Drawing.Drawing2D.GraphicsContainer c = g.BeginContainer();
                    g.SmoothingMode = mode;
                    g.TextRenderingHint = hint;
                    this.OwnerElement.TransformGraphics(g);
                    sp.BeginStyleContainer(this.OwnerElement);

                    if (sp.ViewVisible && sp.VisualMediaStyle.visiblility != "hidden" && sp.VisualMediaStyle.display != "none")
                    {
                        bool old = sp.drawConnects;
                        bool old2 = sp.DrawShadow;
                        bool old3 = sp.AddConnectableElements;
                        this.DrawShadow(sp, g, gp);

                        sp.DrawShadow = false;
                        sp.drawConnects = false;
                        sp.AddConnectableElements = false;
                        svgg.TotalTransform.Reset();
                        using (Matrix matrix = sp.coordTransform.Clone())
                        {
                            bool old4 = sp.UseCoordTransform;
                            sp.UseCoordTransform = true;
                            try
                            {
                                sp.coordTransform.Multiply(this.OwnerElement.TotalTransform);
                                //use绘制过程中不再做视图检查
                                using (Region rg = sp.ClipRegion.Clone())
                                {
                                    sp.ClipRegion.MakeEmpty();
                                    svgg.SVGRenderer.Draw(g, sp);
                                    //恢复
                                    sp.ClipRegion.Union(rg);

                                }
                            }
                            finally
                            {
                                sp.coordTransform.Reset();
                                sp.coordTransform.Multiply(matrix);
                                sp.UseCoordTransform = old4;
                            }
                        }
                        if (this.OwnerElement.ConnectionChanged)
                        {
                            this.CreateConnectPoint();
                            this.OwnerElement.ConnectionChanged = false;
                        }
                        sp.drawConnects = old;
                        sp.DrawShadow = old2;
                        sp.AddConnectableElements = old3;
                        this.DrawBackgroundImage(gp, g);
                        this.DrawLabel(g, sp);
                        this.DrawTextBlock(g, sp, this.OwnerElement);
                        this.DrawConnect(g, sp);
                    }

                    sp.EndContainer(this.OwnerElement);

                    g.EndContainer(c);
                }
            }
            sp.AddRender = old1;
            this.AddToRenderElements(sp);
            this.OwnerElement.CurrentTime = this.OwnerDocument.CurrentTime;
        }
        #endregion
    }
}
