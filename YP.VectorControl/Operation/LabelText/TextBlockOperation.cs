using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Text;
using PerfectSVG.SVGDom.Text;
using PerfectSVG.SVGDom;

namespace PerfectSVG.VectorControl.Operation.LabelText
{
    internal class TextBlockOperation:Operation
    {
        #region ..Constructor
        public TextBlockOperation(VectorControl mousearea, SVGTextBlockElement textBlockElement)
            : base(mousearea)
		{
            this.textBlockElement = textBlockElement;


        }
        #endregion

        public override void Dispose()
        {
            if (this.textBox != null)
            {
                if (this.mouseArea.Controls.Contains(textBox))
                    this.mouseArea.Controls.Remove(textBox);
            }
            base.Dispose();
        }

        #region ..private fields
        SVGTextBlockElement textBlockElement;
        RichTextBox textBox = null;
        #endregion

        #region ..OnPaint
        protected override void OnPaint(object sender, PaintEventArgs e)
        {
            if (this.textBlockElement != null)
            {
                GraphicsContainer c =  e.Graphics.BeginContainer();
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                using (GraphicsPath path = new GraphicsPath())
                {
                    SVGDom.Render.SVGTextBlockRender render  = this.textBlockElement.SVGRenderer as SVGDom.Render.SVGTextBlockRender;
                    RectangleF rect = render.LabelBounds;
                    rect.Inflate(new Size(2, 2));
                    path.AddRectangle(rect);
                    path.Transform(this.mouseArea.CoordTransform);
                    using (Pen pen = new Pen(mouseArea.SelectedPen.Color, 1))
                    {
                        pen.DashPattern = new float[] { 3, 3, 3 };
                        e.Graphics.DrawPath(pen, path);//rect.X, rect.Y, rect.Width, rect.Height);
                    }
                    if (this.textBox == null)
                    {
                        rect = path.GetBounds();
                        this.textBox = new RichTextBox();
                        this.textBox.Font = new Font(render.TextFont.FontFamily, render.TextFont.Size, render.TextFont.Style);
                        if (render.TextFormat.Alignment == StringAlignment.Center)
                            textBox.SelectionAlignment = HorizontalAlignment.Center;
                        else if (render.TextFormat.Alignment == StringAlignment.Far)
                            textBox.SelectionAlignment = HorizontalAlignment.Right;
                        else if (render.TextFormat.Alignment == StringAlignment.Near)
                            textBox.SelectionAlignment = HorizontalAlignment.Left;
                        textBox.BackColor = Color.White;
                        textBox.ForeColor = Color.Black;
                        textBox.Text = textBlockElement.InnerText;
                        textBox.ScrollBars = RichTextBoxScrollBars.None;
                        this.textBox.BorderStyle = BorderStyle.None;
                        this.mouseArea.Controls.Add(textBox);
                        Rectangle bounds = Rectangle.Round(rect);
                        textBox.SetBounds(bounds.X, bounds.Y, bounds.Width, bounds.Height);
                        textBox.Focus();
                        textBox.LostFocus += new EventHandler(textBox_LostFocus);
                    }
                }
            }
        }

        void textBox_LostFocus(object sender, EventArgs e)
        {
            
        }
        #endregion

        #region ..MouseEvent
        protected override void OnMouseDown(object sender, MouseEventArgs e)
        {
            Console.Write("fasd");
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            Console.Write("fasd");
        }

        protected override void OnMouseUp(object sender, MouseEventArgs e)
        {
            
        }
        #endregion

        #region ..OnAdaptAttribute
        protected override void OnAdaptAttribute(object sender, AdaptAttributeEventArgs e)
        {

        }
        #endregion
    }
}
