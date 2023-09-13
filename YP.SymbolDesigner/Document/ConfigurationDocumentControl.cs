using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using YP.VectorControl;
using YP.SVG.Property;

namespace YP.SymbolDesigner.Document
{
    public enum LightStatus
    {
        Normal,
        Warning
    }

    /// <summary>
    /// 演示组态图元的控制
    /// 1：创建自定义属性，分别绑定立罐和二极管
    /// 2：跟踪文档图元变换事件，当二极管和立罐对应属性发生变化时，更新相应的图元，是的图元显示和值相匹配
    ///     2.1：二极管操作，根据不同的状态，分别设置不同的class
    ///     2.2：立罐操作，根据不同的水位值，计算出矩形的位置然后更新
    /// </summary>
    public class ConfigurationDocumentControl:DocumentControl
    {
        #region ..Constructor
        public ConfigurationDocumentControl():base()
        {
        }

        public ConfigurationDocumentControl(string filepath)
            : base(filepath)
        {
        }
        #endregion

        #region ..CreateVectorControl
        protected override void CreateCanvas(string filePath)
        {
            //如果路径为空，从模板中创建已导入预先定于的资源项
            if (filePath == null && !File.Exists(filePath))
                filePath = "Resources/configurationTemplate.svg";
            base.CreateCanvas(filePath);
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();
            //处理图元属性改变事件
            if(this.canvas != null && this.canvas.Document != null)
            this.canvas.Document.ElementChanged -= new SVG.AttributeChangedEventHandler(Document_ElementChanged);
        }

        #region ..InitializeCanvas
        protected override void InitializeCanvas()
        {
            base.InitializeCanvas();
            this.canvas.ShowSelectionHighlightOutline = false;

            //处理图元属性改变事件
            this.canvas.Document.ElementChanged += new SVG.AttributeChangedEventHandler(Document_ElementChanged);
        }
        #endregion

        protected override void CreateProperties()
        {
            this.canvas.Document.PropertyGenerator = new ConfigurationPropertyGenerator(new SVG.Property.PropertyValueEventHandler(GetValue));
        }

        void GetValue(object sender,PropertyValueEventArgs e)
        {
            var item = sender as CustomPropertyItem;
            string value = string.Empty;
            string attributeName = item.AttributeName;
            //获取选区中图元的共同属性值
            foreach (var element in this.canvas.Selection)
            {
                if (element.HasAttribute(attributeName))
                {
                    if (value.Length == 0)
                        value = element.GetAttribute(attributeName);
                    else if (value != element.GetAttribute(attributeName))
                    {
                        value = null;
                        break;
                    }
                }
            }

            if (value != null)
            {
                switch (item.AttributeName)
                {
                    case "status":
                        LightStatus status = LightStatus.Normal;
                        if (value.Length > 0)
                            value = value.Substring(0, 1).ToUpper() + value.Substring(1).ToLower();
                        if (System.Enum.IsDefined(typeof(LightStatus), value))
                            status = (LightStatus)Enum.Parse(typeof(LightStatus), value, true);
                        e.PropertyValue = status;
                        break;
                    case "value":
                        int iValue = 0;
                        if (int.TryParse(value, out iValue))
                            e.PropertyValue = iValue;
                        break;
                }
            }

        }

        #region ..Document_ElementChanged
        void Document_ElementChanged(object sender, SVG.AttributeChangedEventArgs e)
        {
            //跟踪属性变化，当属性变化时，更新相应图元属性
            //注意，这部分逻辑严格依赖于图元设计
            if (e.ChangedElement != null)
            {
                //二极管
                if (e.ChangedElement.GetAttribute("type").ToLower() == "indicator"
                    && e.AttributeName == "status")
                {
                    //取得对应的指示图元，并且设置class属性
                    var node = GetIndicatorElement(e.ChangedElement);
                    node.SetAttribute("class", e.ChangedElement.GetAttribute(e.AttributeName).ToLower() == "warning" ? "warning" : "normal");
                }
                    //水位
                else if(e.ChangedElement.GetAttribute("type").ToLower() == "container"
                    && e.AttributeName == "value")
                {
                    int value = 100;
                    if(int.TryParse(e.ChangedElement.GetAttribute(e.AttributeName), out value))
                    {
                        float fValue = value / 100.0f;
                        var node = GetIndicatorElement(e.ChangedElement) as SVG.BasicShapes.SVGRectElement;
                        
                        var back = node.PreviousElement as SVG.BasicShapes.SVGRectElement;
                        float y = back.Y.Value;
                        float height = back.Height.Value;
                        float bottom = y +height;
                        height = (int)(height * fValue);
                        float y1 = bottom - height;
                        node.OwnerDocument.DoAction(delegate
                        {
                            node.SetAttribute("height", height.ToString());
                            node.SetAttribute("y", y1.ToString());
                        }, false);
                    }
                }
            }
        }
        #endregion

        #region ..GetMainViewElement
        /// <summary>
        /// 从指定的图元对象中，查找对应于该图元的指示图元
        /// </summary>
        /// <param name="ownerElement">需要进行查找的图元</param>
        /// <returns>如存在指示图元，返回图元，如果不存在，返回自身</returns>
        SVG.SVGElement GetIndicatorElement(SVG.SVGElement ownerElement)
        {
            if (ownerElement == null)
                return null;
            if (ownerElement is SVG.DocumentStructure.SVGGElement)
            {
                var type = ownerElement.GetAttribute("type").ToLower();
                switch (type)
                {
                    case "indicator":
                        {
                            //这一部分逻辑要结合图元定义来进行，当前我们选取第一个具备class属性的图元
                            var node = ownerElement.SelectSingleNode("*[@class]") as SVG.SVGElement;
                            if (node != null)
                                return node;
                        }
                        break;
                    case "container":
                        {
                            //这一部分逻辑要结合图元定义来进行，当前我们选取第一个Fill属性等于url(#linearGradient1)的图元
                            var node = ownerElement.SelectSingleNode("g/*[@fill='url(#linearGradient1)']") as SVG.SVGElement;
                            if (node != null)
                                return node;
                        }
                        break;
                }
            }
            return ownerElement;
        }
        #endregion

        protected override void ElementInsertRemoved(object sender, SVG.SVGElementChangedEventArgs e)
        {
            base.ElementInsertRemoved(sender, e);
            //直线或者折线，直接进入弯管
            //if (e.Action == SVG.SVGElementChangedAction.Insert
            //    && (e.Element is SVG.BasicShapes.SVGPolylineElement 
            //    || e.Element is SVG.BasicShapes.SVGPolygonElement 
            //    || e.Element is SVG.BasicShapes.SVGLineElement))
            //{
            //    e.Element.SetAttribute("stroke", "url(#linearGradient)");
            //    e.Element.SetAttribute("stroke-gradientmode","perpendicular");
            //}
        }
    }
}
