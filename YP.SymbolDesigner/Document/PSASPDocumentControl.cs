using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using YP.VectorControl;

namespace YP.SymbolDesigner.Document
{
    public class PSASPDocumentControl:ElectricDocumentControl
    {
        #region ..Constructor
        public PSASPDocumentControl():base()
        {
        }

        public PSASPDocumentControl(string filepath)
            : base(filepath)
        {
        }
        #endregion

        #region ..InitializeCanvas
        protected override void InitializeCanvas()
        {
            base.InitializeCanvas();
            this.canvas.CanvasColor = Color.White;
            this.canvas.TextBlockStyle = new TextBlockStyle(Color.Black, SVG.Alignment.Center, SVG.VerticalAlignment.Middle);
            this.canvas.Stroke = new Stroke(Color.Black);
            this.canvas.Grid = new Grid(false, 10, Color.Green, true, GridType.Dot);
        }
        #endregion
    }
}
