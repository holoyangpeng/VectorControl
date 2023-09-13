using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace YP.SVG.Render
{
    public class SVGSVGRenderer:SVGGroupRenderer
    {
         #region ..Constructor
        public SVGSVGRenderer(DocumentStructure.SVGSVGElement ownerElement)
            : base(ownerElement)
        {
            
        }
        #endregion

        #region ..private fields
        Cache.SVGSVGElementCache svgCache = new YP.SVG.Cache.SVGSVGElementCache();
        #endregion

        #region ..绘制元素
        /// <summary>
        /// 绘制元素
        /// </summary>
        /// <param name="g">画布</param>
        /// <param name="sp">类型容器</param>
        public override void Draw(Graphics g, YP.SVG.StyleContainer.StyleOperator sp)
        {
            DocumentStructure.SVGSVGElement svgElm = this.OwnerElement as DocumentStructure.SVGSVGElement;
            if (this.OwnerDocument.DrawElementWithCache(svgElm))
            {
                this.DrawWithCache(g, sp);
                return;
            }
            System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
            System.Drawing.Text.TextRenderingHint hint = g.TextRenderingHint;
            GraphicsContainer gc = g.BeginContainer();
            sp.BeginStyleContainer(svgElm);
            svgElm.TransformGraphics(null);
            if (sp.VisualMediaStyle.visiblility != "hidden" && sp.VisualMediaStyle.display != "none")
            {
                g.SmoothingMode = mode;
                g.TextRenderingHint = hint;
                svgElm.Clip(g, sp);
                float x = svgElm.X.Value;
                float y = svgElm.Y.Value;
                float width = svgElm.Width.Value;
                float height = svgElm.Height.Value;

                RectangleF elmRect = new RectangleF(x, y, width, height);

                if (svgElm.ParentNode is YP.SVG.SVGElement)
                {
                    // TODO: should it be moved with x and y?
                }

                #region ..剪切
                // check overflow property
                string overflow = svgElm.GetAttribute("overflow").Trim();
                string clip = svgElm.GetAttribute("clip").Trim();
                if (clip.Length > 0)
                {
                    // only valid value is rect(top, right, bottom, left)
                    if (clip.StartsWith("rect("))
                    {
                        string rect = clip.Substring(5);
                        rect = rect.Substring(0, rect.Length - 1);
                        String[] dimensions = rect.Split(new char[] { ',' });

                        if (dimensions.Length == 4)
                        {
                            RectangleF clipRect = new RectangleF(
                                x + (float)System.Convert.ToInt32(dimensions[3]),
                                y + (float)System.Convert.ToInt32(dimensions[0]),
                                width - System.Convert.ToInt32(dimensions[1]),
                                height - System.Convert.ToInt32(dimensions[2]));
                            g.SetClip(clipRect);
                            this.svgCache.CacheClipRect = clipRect;
                        }
                        else
                        {
                            //throw new YP.SVGDom.SVGException("Invalid clip value",YP.SVGDom.SVGExceptionType.SVG_INVALID_VALUE_ERR,null);
                        }
                    }
                }
                clip = null;
                overflow = null;
                #endregion

                this.DrawChilds(g, sp);
            }
            g.EndContainer(gc);
            sp.EndContainer(svgElm);
            svgElm.CurrentTime = this.OwnerDocument.CurrentTime;
        }
        #endregion

        #region ..DrawWithCache
        /// <summary>
        /// Draw the content with cache 
        /// </summary>
        /// <param name="g"></param>
        /// <param name="sp"></param>
        public override void DrawWithCache(Graphics g, YP.SVG.StyleContainer.StyleOperator sp)
        {
            DocumentStructure.SVGSVGElement svgElm = this.OwnerElement as DocumentStructure.SVGSVGElement;
            System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
            System.Drawing.Text.TextRenderingHint hint = g.TextRenderingHint;
            GraphicsContainer gc = g.BeginContainer();
            svgElm.TransformGraphics(null);
            if (this.StyleContainer.VisualMediaStyle.visiblility != "hidden" && StyleContainer.VisualMediaStyle.display != "none")
            {
                g.SmoothingMode = mode;
                g.TextRenderingHint = hint;
                svgElm.Clip(g, sp);
                if (!this.svgCache.CacheClipRect.IsEmpty)
                    g.SetClip(this.svgCache.CacheClipRect);

                if (this.OwnerDocument.ScaleStroke)
                    g.Transform = this.svgCache.CacheFixTransform.Clone();
                svgElm.TotalTransform.Multiply(this.svgCache.CacheFixTransform);
                this.DrawChildsWithCache(g, sp);
            }
            g.EndContainer(gc);
            svgElm.CurrentTime = this.OwnerDocument.CurrentTime;
        }
        #endregion
    }
}
