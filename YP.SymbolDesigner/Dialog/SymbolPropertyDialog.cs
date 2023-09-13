using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

using System.Windows.Forms;
using YP.VectorControl;
using YP.SVG.BasicShapes;
using YP.SVG.DocumentStructure;

namespace YP.SymbolDesigner.Dialog
{
    public partial class SymbolPropertyDialog : BaseDialog
    {
        #region ..SymbolPropertyDialog
        public SymbolPropertyDialog()
        {
            InitializeComponent();
            this.vectorControl = new VectorControl.Canvas(new SVG.DataType.SVGLength(100, SVG.LengthType.SVG_LENGTHTYPE_PERCENTAGE),
                new SVG.DataType.SVGLength(100, SVG.LengthType.SVG_LENGTHTYPE_PERCENTAGE));
            Grid grid = this.vectorControl.Grid;
            grid.Visible = false;
            this.vectorControl.Grid = grid;
            this.vectorControl.ZoomWhenMouseWheel = true;
            this.vectorControl.ShowRule = false;
            this.vectorControl.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(this.vectorControl);
            this.vectorControl.CurrentOperator = Operator.None;
            this.vectorControl.ShowConnectablePoint = true;
        }
        #endregion

        #region ..private fields
        VectorControl.Canvas vectorControl;
        SVGUseElement mainDisplayElement = null;
        SVGSymbolElement symbolElement = null;
        #endregion

        #region ..properties
        public SVGSymbolElement SymbolElement
        {
            set
            {
                if (value != null)
                {
                    this.symbolElement = value;
                    this.txtID.Text = value.ID;
                    this.txtTitle.Text = value.GetAttribute("title");
                    this.comboBox1.SelectedIndex = value.GetAttribute(SVGSymbolElement.SymbolAppendModeAttributeString).ToLower() == SVGSymbolElement.SymbolAppendMode_AppendDirectly.ToLower() ? 1 : 0;
                    this.chkConnectionPoint.Checked = value.GetAttribute(SVGConnectionElement.CreateDefaultConnectablePointAttributeString).ToLower() == "false";

                    var doc = this.vectorControl.Document;
                    var symbol = doc.ImportNode(value, true) as SVG.SVGElement;
                    doc.DocumentElement.AppendChild(symbol);

                    var use = doc.CreateSVGUseElement();
                    use.SetAttribute("href", string.Format("#{0}", symbol.ID));
                    use.SetAttribute("fill", "white");
                    use.SetAttribute("stroke", "black");
                    RectangleF bounds = value.GetContentBounds();
                    use.X = -bounds.X + 100;
                    use.Y = -bounds.Y + 100;
                    mainDisplayElement = doc.DocumentElement.AppendChild(use) as SVGUseElement;

                    var str = symbol.GetAttribute(SVGConnectionElement.ConnectablePointAttributeString);
                    SVG.DataType.SVGPointList points = new SVG.DataType.SVGPointList(str);
                    int left = 0, right = 0, top = 0, bottom = 0;
                    foreach (PointF p in points.GetGDIPoints())
                    {
                        if (p.X == 0)
                            left++;
                        if (p.X == 100)
                            right++;
                        if (p.Y == 0)
                            top++;
                        if (p.Y == 100)
                            bottom++;
                    }

                    this.trackLeft.Value = left;
                    this.trackRight.Value = right;
                    this.trackTop.Value = top;
                    this.trackBottom.Value = bottom;

                    this.ShowTip(null, EventArgs.Empty);
                }
            }
        }
        #endregion

        #region ..chkConnectionPoint_CheckedChanged
        private void chkConnectionPoint_CheckedChanged(object sender, EventArgs e)
        {
            this.trackBottom.Enabled = this.trackLeft.Enabled
                = this.trackTop.Enabled = this.trackRight.Enabled = this.chkConnectionPoint.Checked;
            if (this.mainDisplayElement != null)
                this.mainDisplayElement.SetAttribute(SVGConnectionElement.CreateDefaultConnectablePointAttributeString, this.chkConnectionPoint.Checked ? "false" : "true");
            this.ShowTip(sender,e);
        }
        #endregion

        #region ..trackBottom_ValueChanged
        private void trackBottom_ValueChanged(object sender, EventArgs e)
        {
            string name = (sender as Control).Tag as string;
            var control = this.Controls.Find(name, true);
            if (control != null && control.Length == 1)
                control[0].Text = (sender as TrackBar).Value.ToString();

            if (this.mainDisplayElement != null)
                this.mainDisplayElement.SetAttribute(SVGConnectionElement.ConnectablePointAttributeString, this.GetConnectPointString());
        }
        #endregion

        #region ..GetConnectPointString
        string GetConnectPointString()
        {
            int left = this.trackLeft.Value;
            int right = this.trackRight.Value;
            int top = this.trackTop.Value;
            int bottom = this.trackBottom.Value;
            StringBuilder strBuilder = new StringBuilder();
            int[] values = { left, top, right, bottom };
            string[] formats = { "0 {0} ", "{0} 0 ", "100 {0} ", "{0} 100 " };
            for (int j = 0; j < values.Length; j++)
            {
                var value = values[j];
                if (value > 0)
                {
                    int step = (int)(100f / (value + 1));
                    for (int i = 1; i <= value; i++)
                        strBuilder.AppendFormat(formats[j], i * step);
                }
            }
            return strBuilder.ToString();
        }
        #endregion

        #region ..OnAccept
        protected override void OnAccept()
        {
            base.OnAccept();
            if (this.symbolElement != null)
            {
                this.symbolElement.ID = this.txtID.Text;
                this.symbolElement.SetAttribute("title", this.txtTitle.Text);
                if (this.comboBox1.SelectedIndex == 1)
                    this.symbolElement.SetAttribute(SVGSymbolElement.SymbolAppendModeAttributeString, SVGSymbolElement.SymbolAppendMode_AppendDirectly);
                else
                    this.symbolElement.RemoveAttribute(SVGSymbolElement.SymbolAppendModeAttributeString);

                if (this.chkConnectionPoint.Checked)
                {
                    this.symbolElement.SetAttribute(SVGConnectionElement.CreateDefaultConnectablePointAttributeString, "false");
                    this.symbolElement.SetAttribute(SVGConnectionElement.ConnectablePointAttributeString, this.GetConnectPointString());
                }
                else
                    this.symbolElement.RemoveAttribute(SVGConnectionElement.CreateDefaultConnectablePointAttributeString);
            }
        }
        #endregion

        #region ..ShowTip
        void ShowTip(object sender, EventArgs e)
        {
            this.lbWarning.Visible = this.comboBox1.SelectedIndex == 1 && this.chkConnectionPoint.Checked;
        }
        #endregion
    }
}
