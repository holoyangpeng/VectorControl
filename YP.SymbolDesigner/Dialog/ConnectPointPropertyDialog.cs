using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YP.VectorControl;
using YP.SVG;

namespace YP.SymbolDesigner.Dialog
{
    public partial class ConnectPointPropertyDialog : BaseDialog
    {
        #region ..Constructor
        public ConnectPointPropertyDialog()
        {
            InitializeComponent();
            this.vectorControl = new VectorControl.Canvas(new SVG.DataType.SVGLength(100, SVG.LengthType.SVG_LENGTHTYPE_PERCENTAGE),
                new SVG.DataType.SVGLength(100, SVG.LengthType.SVG_LENGTHTYPE_PERCENTAGE));
            Grid grid = this.vectorControl.Grid;
            grid.Visible = false;
            this.vectorControl.Grid = grid;
            this.vectorControl.ShowRule = false;
            this.vectorControl.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(this.vectorControl);
            this.vectorControl.CurrentOperator = Operator.None;
            this.vectorControl.ShowConnectablePoint = true;
        }
        #endregion

        #region ..private fields
        VectorControl.Canvas vectorControl;
        SVGTransformableElement displayMainElm = null;
        SVGTransformableElement propertyMainElm = null;
        #endregion

        #region ..properties
        public SVGTransformableElement Element
        {
            set
            {
                if (value != null)
                {
                    propertyMainElm = Helper.MatlabHelper.GetMainElement(value);
                    var doc = this.vectorControl.Document;
                    SVG.Interface.DataType.ISVGRect rect = value.GetBBox();
                    var elm = doc.ImportNode(value, true) as SVGTransformableElement;
                    //use对象，把symbol加入
                    if (elm is SVG.DocumentStructure.SVGUseElement)
                    {
                        var sym = (elm as SVG.DocumentStructure.SVGUseElement).RefElement;
                        doc.RootElement.AppendChild(doc.ImportNode(sym, true));   
                    }
                    string transform = string.Format("translate({0} {1}) {2} ", -rect.X + 20, -rect.Y + 20, elm.GetAttribute("transform"));
                    elm.SetAttribute("transform", transform);
                    elm = this.vectorControl.Document.RootElement.AppendChild(elm) as SVGTransformableElement;

                    this.checkBox1.Checked = value.GetAttribute(SVG.BasicShapes.SVGConnectionElement.CreateDefaultConnectablePointAttributeString).ToLower() == "false";

                    displayMainElm = Helper.MatlabHelper.GetMainElement(elm);
                    if (displayMainElm != null)
                    {
                        PointF[] ps = displayMainElm.RelativeConnectionPoints;
                        int left = 0, right = 0, top = 0, bottom = 0;
                        foreach (PointF p in ps)
                        {
                            if (p.X == 0)
                                left++;
                            if (p.X == 100)
                                right++;
                            if (p.Y == 0)
                                top++;
                            if (p.Y == 0)
                                bottom++;
                        }

                        this.trackLeft.Value = left;
                        this.trackRight.Value = right;
                        this.trackTop.Value = top;
                        this.trackBottom.Value = bottom;
                    }
                }
            }
        }
        #endregion

        #region ..trackRight_ValueChanged
        private void trackRight_ValueChanged(object sender, EventArgs e)
        {
            string name = (sender as Control).Tag as string;
            var control = this.Controls.Find(name, true);
            if (control != null && control.Length == 1)
                control[0].Text = (sender as TrackBar).Value.ToString();

            this.OnAccept(this.displayMainElm);
        }
        #endregion

        #region ..OnAccept
        protected override void OnAccept()
        {
            base.OnAccept();
            this.OnAccept(this.propertyMainElm);
        }

        void OnAccept(SVGTransformableElement element)
        {
            SVGTransformableElement elm = element;
            if (elm != null)
            {
                if (this.checkBox1.Checked)
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

                    //if (strBuilder.Length > 0)
                    elm.SetAttribute("createDefaultConnectPoint", "false");
                    elm.SetAttribute("connectPoints", strBuilder.ToString());
                }
                else
                    elm.RemoveAttribute(SVG.BasicShapes.SVGConnectionElement.CreateDefaultConnectablePointAttributeString);
            }
        }
        #endregion

        #region ..checkBox1_CheckedChanged
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            this.trackBottom.Enabled = this.trackTop.Enabled
                = this.trackLeft.Enabled = this.trackRight.Enabled = this.checkBox1.Checked;
            this.OnAccept(this.displayMainElm);
        }
        #endregion
    }
}
