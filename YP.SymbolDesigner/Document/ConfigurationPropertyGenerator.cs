using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YP.SVG;
using YP.SVG.Property;

namespace YP.SymbolDesigner.Document
{
    class ConfigurationPropertyGenerator:PropertyGenerator
    {
        private PropertyValueEventHandler getValue;

        public ConfigurationPropertyGenerator(PropertyValueEventHandler getValue)
        {
            this.getValue = getValue;
        }

        public override IProperty[] GeneratePropertiesForElement(SVGElement element)
        {
            List<IProperty> properties = new List<IProperty>();
            properties.Add(base.GeneratePropertiesForElement(element)[0]);
           
            if(element.Name == "g")
            {
                string type = element.GetAttribute("type").ToLower();
                if (type == "indicator")
                 {
                    CustomPropertyItem item = new CustomPropertyItem("状态", LightStatus.Normal.GetType(), "状态", "status");
                    item.GetValue = getValue;
                    item.IsReadOnly = false;
                    item.Description = "指定二极管的状态。\nNormal:状态正常\nWarnning:报警";
                    properties.Add(item);
                }
                else if (type== "container")
                {
                    CustomPropertyItem item = new CustomPropertyItem("液位", typeof(int), "状态", "value");
                    item.GetValue = getValue;
                    item.IsReadOnly = false;
                    item.Description = "指示立灌中的水位百分比";
                    properties.Add(item);
                }
            }

            return properties.ToArray();
        }
    }
}
