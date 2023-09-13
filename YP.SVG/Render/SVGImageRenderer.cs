using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace YP.SVG.Render
{
    public class SVGImageRenderer:SVGDirectRenderer
    {
        #region ..Constructor
        public SVGImageRenderer(DocumentStructure.SVGImageElement ownerElement)
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
        public override void Draw(Graphics g, YP.SVG.StyleContainer.StyleOperator sp)
        {
            if (this.OwnerDocument.IsStopRender)
                return;
            DocumentStructure.SVGImageElement img = (DocumentStructure.SVGImageElement)this.OwnerElement;
            GraphicsPath gp = (img as SVG.Interface.ISVGPathable).GPath;
            if (!BeforeDrawing(g, sp))
            {
                this.DrawTextBlock(g, sp, this.OwnerElement);
                return;
            }
            System.Drawing.Drawing2D.SmoothingMode mode = g.SmoothingMode;
            GraphicsContainer c = g.BeginContainer();
            this.OwnerElement.TransformGraphics(g);
            sp.BeginStyleContainer(img);
            string sVisibility = sp.VisualMediaStyle.visiblility.Value;
            string sDisplay = sp.VisualMediaStyle.display.Value;

            if (string.Compare(sVisibility,"hidden") != 0 &&string.Compare(sDisplay,"none") != 0)
            {
                img.Clip(g, sp);
                g.SmoothingMode = mode;
                System.Drawing.Bitmap bmp = img.ImageSource;
                float width = img.Width.Value;
                float height = img.Height.Value;
                if (!this.OwnerDocument.ScaleStroke)
                {
                    g.Transform = sp.coordTransform.Clone();
                    g.MultiplyTransform(img.TotalTransform);
                }
                Rectangle destRect = new Rectangle();
                destRect.X = Convert.ToInt32(img.X.Value);
                destRect.Y = Convert.ToInt32(img.Y.Value);
                destRect.Width = Convert.ToInt32(width);
                destRect.Height = Convert.ToInt32(height);
                if (!sp.BoundView)
                {
                    this.DrawShadow(sp, g, gp);
                    using (ImageAttributes imageAttributes = new ImageAttributes())
                    {
                        //			this.styleContainer = sc;
                        float opacity = sp.ClipStyle.opacity.Value;
                        if (opacity < 1)
                        {
                            //								float opacity = YP.SVGDom.DataType.SVGNumber.ParseNumberStr(sOpacity);
                            ColorMatrix myColorMatrix = new ColorMatrix();
                            myColorMatrix.Matrix00 = 1.00f; // Red
                            myColorMatrix.Matrix11 = 1.00f; // Green
                            myColorMatrix.Matrix22 = 1.00f; // Blue
                            myColorMatrix.Matrix33 = opacity; // alpha
                            myColorMatrix.Matrix44 = 1.00f; // w

                            imageAttributes.SetColorMatrix(myColorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                            myColorMatrix = null;
                        }

                        if (bmp != null)
                        {
                            this.DrawShadow(sp, g, gp);
                            g.DrawImage(bmp, destRect, 0f, 0f, bmp.Width, bmp.Height, GraphicsUnit.Pixel, imageAttributes);
                        }
                        bmp = null;
                    }
                }
                else
                {
                    g.DrawRectangle(sp.outlinePen, destRect);
                }

            }
            sp.EndContainer(img);
            sVisibility = null;
            sDisplay = null;
            g.EndContainer(c);
            this.DrawLabel(g, sp);
            this.DrawTextBlock(g, sp, this.OwnerElement);
            this.DrawConnect(g, sp);
            this.AddToRenderElements(sp);
            img.CurrentTime = this.OwnerDocument.CurrentTime;
        }
		#endregion

        #region ..DrawBackgroundImage
        public override void DrawBackgroundImage(GraphicsPath gp, Graphics g)
        {

        }
        #endregion
    }
}
