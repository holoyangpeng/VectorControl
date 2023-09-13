using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.SVG.Render
{
    public class SVGGroupRenderer:SVGBaseRenderer
    {
        #region ..Constructor
        public SVGGroupRenderer(DocumentStructure.SVGGElement ownerElement)
            : base(ownerElement)
        {
            
        }
        #endregion

        #region ..绘制元素
        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="g">画布</param>
        /// <param name="sp">类型容器</param>
        public override void Draw(Graphics g, SVG.StyleContainer.StyleOperator sp)
        {
            this.OwnerElement.ApplyCSSStyle();

            DocumentStructure.SVGGElement group = this.OwnerElement as DocumentStructure.SVGGElement;
            Interface.ISVGPathable pathable = group as Interface.ISVGPathable;
            GraphicsPath gp = pathable.GPath;
            if (gp != null)
            {
                if (!BeforeDrawing(g, sp))
                    return;
            }

            if (this.OwnerDocument.DrawElementWithCache(this.OwnerElement))
            {
                this.DrawWithCache(g, sp);
                return;
            }
            sp.BeginStyleContainer(this.OwnerElement);
            System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
            System.Drawing.Text.TextRenderingHint hint = g.TextRenderingHint;
            InterpolationMode mode1 = g.InterpolationMode;
            GraphicsContainer c = g.BeginContainer();
            g.SmoothingMode = mode;
            g.TextRenderingHint = hint;
            g.InterpolationMode = mode1;
            this.OwnerElement.TransformGraphics(g);
            if (sp.ViewVisible 
                && sp.VisualMediaStyle.visiblility != "hidden" 
                && sp.VisualMediaStyle.display != "none")
            {
                this.OwnerElement.Clip(g, sp);
                this.DrawChilds(g, sp);
            }
            g.EndContainer(c);
            sp.EndContainer(this.OwnerElement);

            this.OwnerElement.CurrentTime = this.OwnerDocument.CurrentTime;
        }
        #endregion

        #region ..DrawWithCache
        /// <summary>
        /// draw the content with cache
        /// </summary>
        /// <param name="g"></param>
        /// <param name="sp"></param>
        public virtual void DrawWithCache(Graphics g, SVG.StyleContainer.StyleOperator sp)
        {
            System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
            System.Drawing.Text.TextRenderingHint hint = g.TextRenderingHint;
            GraphicsContainer c = g.BeginContainer();
            g.SmoothingMode = mode;
            g.TextRenderingHint = hint;
            this.OwnerElement.TransformGraphics(g);
            sp.BeginStyleContainer(this.OwnerElement);
            if (this.StyleContainer.ViewVisible && StyleContainer.VisualMediaStyle.visiblility != "hidden" && StyleContainer.VisualMediaStyle.display != "none")
            {
                this.OwnerElement.Clip(g, sp);
                this.DrawChildsWithCache(g, sp);
            }
            sp.EndContainer(this.OwnerElement);
            g.EndContainer(c);
        }
        #endregion

        #region ..绘制子元素
        /// <summary>
        /// 绘制子元素
        /// </summary>
        /// <param name="g">画布</param>
        /// <param name="sp">类型容器</param>
        public virtual void DrawChilds(Graphics g, SVG.StyleContainer.StyleOperator sp)
        {
            try
            {
                DocumentStructure.SVGGElement group = this.OwnerElement as DocumentStructure.SVGGElement;
                SVG.SVGElementCollection childs = group.ChildElements;
                if (group.refedElement is DocumentStructure.SVGSymbolElement)
                    childs = (group.refedElement as DocumentStructure.SVGSymbolElement).ChildElements;

                for (int i = 0; i < childs.Count; i++)
                {
                    SVG.SVGTransformableElement render = childs[i] as SVG.SVGTransformableElement;
                    (render as SVGStyleable).NeedUpdateCSSStyle = (render as SVGStyleable).NeedUpdateCSSStyle || this.OwnerElement.NeedUpdateCSSStyle;
                    render.SVGRenderer.Draw(g, sp);
                    (render as SVGStyleable).NeedUpdateCSSStyle = false;
                }
            }
            catch (System.Exception e1)
            {
                this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e1.Message }, ExceptionLevel.Normal));
            }
            finally
            {
                this.OwnerElement.NeedUpdateCSSStyle = false;
            }
        }
        #endregion

        #region ..DrawChildsWithCache
        /// <summary>
        /// draw the childs with cache
        /// </summary>
        /// <param name="g"></param>
        /// <param name="sp"></param>
        public virtual void DrawChildsWithCache(Graphics g, SVG.StyleContainer.StyleOperator sp)
        {
            DocumentStructure.SVGGElement group = this.OwnerElement as DocumentStructure.SVGGElement;
            SVG.SVGElementCollection childs = group.ChildElements;
            if (group.refedElement is DocumentStructure.SVGSymbolElement)
                childs = (group.refedElement as DocumentStructure.SVGSymbolElement).ChildElements;
            for (int i = 0; i < childs.Count; i++)
            {
                SVGTransformableElement render = childs[i] as SVGTransformableElement;
                //((SVGDom.SVGTransformableElement)render).TotalTransform.Reset();
                //((SVGDom.SVGTransformableElement)render).TotalTransform.Multiply(group.TotalTransform);
                if (group.NeedUpdateCSSStyle)
                    (render as SVGStyleable).CurrentTime = -1;
                (render as SVGStyleable).NeedUpdateCSSStyle = (render as SVGStyleable).NeedUpdateCSSStyle || group.NeedUpdateCSSStyle;
                if(render.SVGRenderer != null)
                    render.SVGRenderer.Draw(g, sp);
                (render as SVGStyleable).NeedUpdateCSSStyle = false;
            }
        }
        #endregion
    }
}
