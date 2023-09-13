using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using YP.VectorControl;

namespace YP.SymbolDesigner.Document
{
    /// <summary>
    /// 电力文档
    /// </summary>
    public class ElectricDocumentControl:DocumentControl
    {
        #region ..Constructor
        public ElectricDocumentControl():base()
        {
        }

        public ElectricDocumentControl(string filepath):base(filepath)
        {
        }
        #endregion

        #region ..InitializeCanvas
        protected override void InitializeCanvas()
        {
            base.InitializeCanvas();
            this.canvas.TransformBehavior = TransformBehavior.Scale | TransformBehavior.Translate;
            this.canvas.Grid = new Grid(false, 10, Color.LightGray, true, GridType.Dot);
            this.canvas.ShowRule = false;
            this.canvas.CanvasColor = Color.Black;
            this.canvas.Stroke = new Stroke(Color.FromArgb(255, 255, 127));
            this.canvas.TextBlockStyle = new TextBlockStyle(Color.FromArgb(255, 255, 127), SVG.Alignment.Center, SVG.VerticalAlignment.Middle);
        }
        #endregion

        #region ..ElementInserted
        protected override void ElementInsertRemoved(object sender, SVG.SVGElementChangedEventArgs e)
        {
            if (e.Action == SVG.SVGElementChangedAction.Insert)
            {
                if (!(e.Element is SVG.DocumentStructure.SVGUseElement) && e.Element is SVG.SVGTransformableElement)
                    (e.Element as SVG.SVGTransformableElement).CreateDefaultConnectablePoint = false;

                //文本块
                if (e.Element is SVG.Text.SVGTextBlockElement)
                {
                    //use对象的文本块不折行
                    if (e.Element.ParentElement is SVG.DocumentStructure.SVGUseElement)
                        e.Element.SetAttribute("wrap", "nowrap");
                }
            }
        }
        #endregion
    }
}
