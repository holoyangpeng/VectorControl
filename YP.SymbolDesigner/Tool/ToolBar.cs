using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Drawing;
using System.Reflection;
using System.Collections;
using YP.VectorControl;

namespace YP.SymbolDesigner.Tool
{
    public class ToolBar
    {
        #region ..Constructor
        public ToolBar(string configFile)
        {
            if (File.Exists(configFile))
            {
                configDocument = new System.Xml.XmlDocument();
                try
                {
                    configDocument.Load(configFile);
                }
                catch (System.Exception e1)
                {
                    Console.WriteLine("error:" + e1.Message);
                }

                this.ParseConfig(configDocument.DocumentElement);
            }
        }
        #endregion

        #region ..private fields
        ToolStrip toolStrip = new ToolStrip();
        VectorControl.Canvas canvas;
        XmlDocument configDocument;
        //记录xmlelement和对应的control的配对
        Hashtable elementToControls = new Hashtable();
        bool inSync = false;
        #endregion

        #region ..properties
        public ToolStrip ToolStrip
        {
            get
            {
                this.toolStrip.Dock = DockStyle.Top;
                return this.toolStrip;
            }
        }

        /// <summary>
        /// 设置工具条当前控制的VectorControl控件
        /// </summary>
        public VectorControl.Canvas Canvas
        {
            set
            {
                if (this.canvas != value)
                {
                    if (this.canvas != null)
                    {
                        this.canvas.OperatorChanged -= new EventHandler(vectorControl_OperatorChanged);
                        this.canvas.ScaleChanged -= new EventHandler(canvas_ScaleChanged);
                    }
                    this.canvas = value;
                    //同步相关选项
                    if (this.canvas != null)
                    {
                        this.canvas_ScaleChanged(this.canvas, EventArgs.Empty);
                        vectorControl_OperatorChanged(this.canvas, EventArgs.Empty);
                        this.canvas.OperatorChanged += new EventHandler(vectorControl_OperatorChanged);
                        this.canvas.ScaleChanged += new EventHandler(canvas_ScaleChanged);
                        SynSetting();
                    }
                }
            }
            get
            {
                return this.canvas;
            }
        }
        #endregion

        #region ..ParseConfig
        void ParseConfig(System.Xml.XmlElement rootElement)
        {
            if (rootElement == null)
                return;
            ImageList images = null;
            //解析Images
            if (rootElement.HasAttribute("images"))
                images = LoadBitmapStrip(this.GetType(), "YP.SymbolDesigner.Resources.Bitmap1.bmp", new Size(16, 16), new Point(0, 0));
            System.Xml.XmlNodeList nodes = rootElement.GetElementsByTagName("item");
            
            foreach (XmlElement child in nodes)
            {
                string strType = child.GetAttribute("type");
                Type itemType = this.GetControlType(child);
                if (itemType == null)
                    continue;
                ToolStripItem item = Activator.CreateInstance(itemType) as ToolStripItem;//, BindingFlags.Default, null, param,null, null) as ToolStripItem;
                elementToControls[child] = item;
                if (item != null)
                {
                    int index = -1;
                    //如果有有效的ImageIndex，从Imagelist中取出Image
                    if (images != null && int.TryParse(child.GetAttribute("imageIndex"), out index) && index < images.Images.Count)
                        item.Image = images.Images[index];
                    //如果有图片路径，取出
                    else if (child.HasAttribute("image") && File.Exists(child.GetAttribute("image")))
                        item.Image = Image.FromFile(child.GetAttribute("image"), true);
                    item.ToolTipText = child.GetAttribute("title");
                    int width = 100;
                    if (int.TryParse(child.GetAttribute("width"), out width))
                        item.Size = new Size(width, item.Size.Height);

                    if (child.GetAttribute("checkonclick") == "true")
                    {
                        if (item is ToolStripButton)
                            (item as ToolStripButton).CheckOnClick = true;
                    }

                    //绑定事件
                    if (child.HasAttribute("event"))
                    {
                        EventInfo eInfo = itemType.GetEvent(child.GetAttribute("event"));
                        if (eInfo != null)
                            eInfo.AddEventHandler(item, new EventHandler(itemControlEventHandler));
                    }

                    item.Tag = child;
                    //处理DropDownButton类型，根据DropDown节点定义创建对应的ToolStripDropDown
                    XmlNodeList nodelist = child.GetElementsByTagName("dropdown");
                    if (item is ToolStripDropDownItem && nodelist.Count >= 1)
                    {
                        var dropDownElement = nodelist[0] as XmlElement;
                        //根据controlbase，取得对应的Assembly
                        Type dropdownType = this.GetControlType(dropDownElement);
                        if (dropdownType == null)
                            return;
                        //通过Type创建ToolStripItem
                        XmlNodeList parames = dropDownElement.GetElementsByTagName("param");
                        object[] param = null;
                        if (parames.Count > 0)
                        {
                            param = new object[parames.Count];
                            for (int i = 0; i < parames.Count; i++)
                                param[i] = (parames[i] as XmlElement).GetAttribute("value");
                        }
                        Control c = Activator.CreateInstance(dropdownType, param) as Control;// controltype.Assembly.CreateInstance(type1, true, BindingFlags.Default , null, param, null, null) as Control;
                        elementToControls[dropDownElement] = c;
                        if (c != null)
                        {
                            ToolStripDropDown dropDown = new ToolStripDropDown();
                            dropDown.Tag = c;
                            dropDown.Padding = new Padding(2, 2, 2, 2);
                            c.Tag = dropDownElement;
                            if (dropDownElement.HasAttribute("event"))
                            {
                                EventInfo eInfo = dropdownType.GetEvent(dropDownElement.GetAttribute("event"));
                                if (eInfo != null)
                                    eInfo.AddEventHandler(c, new EventHandler(itemControlEventHandler));
                            }
                            else
                            {
                                dropDown.Closed += new ToolStripDropDownClosedEventHandler(dropDown_Closed);
                            }
                            ToolStripControlHost host = new ToolStripControlHost(c);

                            int height = 300;
                            if (int.TryParse(dropDownElement.GetAttribute("height"), out height))
                                c.Height = height;
                            else
                            {
                                c.Height = c.Height < 200 ? 200 : c.Height;
                                c.Height = c.Height > 300 ? 300 : c.Height;
                            }
                            if (int.TryParse(dropDownElement.GetAttribute("width"), out width))
                                c.Size = new Size(width, c.Height);
                            host.AutoSize = dropDownElement.GetAttribute("autosize").ToLower() =="true";
                            dropDown.AutoSize = true;

                            //遍历子节点，进行属性初始化
                            foreach (XmlNode pnode in dropDownElement.ChildNodes)
                            {
                                if (pnode.Name == "property")
                                {
                                    string name = (pnode as XmlElement).GetAttribute("name");
                                    string value = (pnode as XmlElement).GetAttribute("value");

                                    PropertyInfo property= dropdownType.GetProperty(name);
                                    if (property != null)
                                    {
                                        object finalValue = null;
                                        if (property.PropertyType == typeof(bool))
                                            finalValue = bool.Parse(value);
                                        if(finalValue != null)
                                            property.SetValue(c, finalValue, null);
                                    }
                                }
                            }
                            dropDown.Items.Add(host);
                            (item as ToolStripDropDownItem).DropDown = dropDown;
                            (item as ToolStripDropDownItem).DropDownOpened += new EventHandler(ToolBar_DropDownOpened);
                        }
                    }
                    this.toolStrip.Items.Add(item);
                }
            }
        }
        #endregion

        #region ..GetControlType
        /// <summary>
        /// 取得对应配置文件节点所配置的Type
        /// </summary>
        /// <param name="cfgElement"></param>
        /// <returns></returns>
        Type GetControlType(System.Xml.XmlElement cfgElement)
        {
            string strType = cfgElement.GetAttribute("type");
            Type type = typeof(System.Windows.Forms.ToolStripButton);
            string controlbase = cfgElement.GetAttribute("controlbase");
            switch(controlbase)
            {
                case "symboldesigner":
                    type = typeof(ToolBar);
                    break;
                case "vectorcontrol":
                    type = typeof(VectorControl.Canvas);
                    break;
            }
            return type.Assembly.GetType(strType);
        }
        #endregion

        #region ..LoadBitmapStrip
        /// <summary>
        /// 从资源中导入ImageList
        /// </summary>
        /// <param name="assemblyType"></param>
        /// <param name="imageName"></param>
        /// <param name="imageSize"></param>
        /// <param name="transparentPixel"></param>
        /// <returns></returns>
        public static ImageList LoadBitmapStrip(Type assemblyType,
            string imageName,
            Size imageSize,
            Point transparentPixel)
        {
            return LoadBitmapStrip(assemblyType, imageName, imageSize, true, transparentPixel);
        }

        /// <summary>
        /// 从资源中导入ImageList
        /// </summary>
        /// <param name="assemblyType"></param>
        /// <param name="imageName"></param>
        /// <param name="imageSize"></param>
        /// <param name="makeTransparent"></param>
        /// <param name="transparentPixel"></param>
        /// <returns></returns>
        protected static ImageList LoadBitmapStrip(Type assemblyType,
            string imageName,
            Size imageSize,
            bool makeTransparent,
            Point transparentPixel)
        {
            // Create storage for bitmap strip
            ImageList images = new ImageList();

            // Define the size of images we supply
            images.ImageSize = imageSize;

            // Get the assembly that contains the bitmap resource
            Assembly myAssembly = Assembly.GetAssembly(assemblyType);

            // Get the resource stream containing the images
            Stream imageStream = myAssembly.GetManifestResourceStream(imageName);

            if (imageStream == null)
                return null;
            // Load the bitmap strip from resource
            Bitmap pics = new Bitmap(imageStream);

            if (makeTransparent)
            {
                Color backColor = pics.GetPixel(transparentPixel.X, transparentPixel.Y);

                // Make backColor transparent for Bitmap
                pics.MakeTransparent(backColor);
            }

            // Load them all !
            images.Images.AddStrip(pics);

            return images;
        }
        #endregion

        #region ..ToolBar_DropDownOpened
        void ToolBar_DropDownOpened(object sender, EventArgs e)
        {
            ToolStripDropDownItem item = sender as ToolStripDropDownItem;
            if (item != null && item.DropDown != null && item.DropDown.Items.Count > 0)
                (item.DropDown.Items[0] as ToolStripControlHost).Control.Focus();
        }
        #endregion

        #region ..itemControlEventHandler
        /// <summary>
        /// 当绑定的控件指定的事件发生后，用规定的指更新vectorcontrol对应的属性
        /// 从配置文档中对应节点取得相关配置，通过反射进行属性、方法等的设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void itemControlEventHandler(object sender, EventArgs e)
        {
            //处于同步状态中时，不处理
            if (this.canvas == null || this.inSync)
                return;
            Control c = sender as Control;
            object tag = this.GetTag(sender);
            //更新属性
            if (tag is XmlElement)
            {
                XmlElement element = tag as XmlElement;
                Type controlType = this.GetControlType(element);
                if (controlType != null)
                {
                    object result = null;
                    string value = element.GetAttribute("value");
                    result = value;
                    if (element.GetAttribute("valuetype") == "property")
                    {
                        PropertyInfo info = controlType.GetProperty(value);
                        if (info != null)
                            result = info.GetValue(sender, null);
                    }

                    {
                        Type vcType = this.canvas.GetType();
                        PropertyInfo info= vcType.GetProperty(element.GetAttribute("behavior"));
                        if (info != null)
                        {
                            //存在target
                            if (element.HasAttribute("target"))
                            {
                                object pValue = info.GetValue(this.canvas, null);
                                if (pValue != null)
                                {
                                    PropertyInfo mInfo = pValue.GetType().GetProperty(element.GetAttribute("target"));
                                    if (mInfo != null)
                                    {
                                        this.SetPropertyValue(pValue, mInfo, result);
                                        //如果对应的behavior是设置Enum类型的属性，则选中当前控件，并重置对应的同一个枚举类型的控件
                                        if (mInfo.PropertyType == typeof(Enum) || mInfo.PropertyType.BaseType == typeof(Enum))
                                        {
                                            var tag1 = GetTag(sender);
                                            if (tag1 is XmlElement)
                                            {
                                                //重置其他同类型控件
                                                string xpath = "*[@behavior='" + element.GetAttribute("behavior") + "' and @target='" + element.GetAttribute("target") + "']";
                                                this.ResetEnumControl((tag1 as XmlElement).ParentNode as XmlElement, xpath);
                                                //选中当前控件
                                                if (sender is ToolStripButton)
                                                    (sender as ToolStripButton).Checked = true;
                                            }
                                        }
                                    }
                                }

                                info.SetValue(this.canvas, pValue, null);
                            }
                            else 
                            {
                                this.SetPropertyValue(this.canvas, info, result);
                            }
                        }
                    }
                }
            }

            //if is dropdown, close it
            while (c != null)
            {
                if (c is ToolStripDropDown)
                {
                    (c as ToolStripDropDown).Close();
                    break;
                }
                else
                    c = c.Parent;
            }
            
        }
        #endregion

        #region ..dropDown_Closed
        void dropDown_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            ToolStripDropDown dropdown = sender as ToolStripDropDown;
            if (dropdown != null && dropdown.Tag is Control)
            {
                //手动触发itemcontrol
                itemControlEventHandler(dropdown.Tag, EventArgs.Empty);
            }
        }
        #endregion

        #region ..SetPropertyValue
        /// <summary>
        /// 根据反射设置指定Property项的值
        /// </summary>
        void SetPropertyValue(object element, PropertyInfo property, object result)
        {
            if (result != null && result.GetType() != property.PropertyType)
            {
                //取得属性的类型，查看是否存在Parse方法，如果存在，解析，如果不存在，保留原值
                MethodInfo method = property.PropertyType.GetMethod("Parse", new Type[] { typeof(string) });
                if (method != null)
                    result = method.Invoke(null, new object[] { result.ToString() });
                //处理枚举
                else if (property.PropertyType.BaseType == typeof(System.Enum))
                    result = System.Enum.Parse(property.PropertyType, result.ToString());
            }
            if ((result == null && !(property.PropertyType.IsValueType)) || (result != null &&  property.PropertyType.IsAssignableFrom(result.GetType())))
                property.SetValue(element, result, null);
        }
        #endregion

        #region ..vectorControl_OperatorChanged,操作方式改变
        void vectorControl_OperatorChanged(object sender, EventArgs e)
        {
            string opStr = this.canvas.CurrentOperator.ToString();
            if (configDocument != null && configDocument.DocumentElement != null)
            {
                ResetOperatorButton();

                System.Xml.XmlElement element = configDocument.DocumentElement.SelectSingleNode("*[@behavior='CurrentOperator' and @value = '" + opStr + "']") as XmlElement;
                if (element != null && elementToControls.ContainsKey(element))
                {
                    ToolStripButton btn = elementToControls[element] as ToolStripButton;
                    if (btn != null)
                        btn.Checked = true;
                    else
                    {
                        Controls.ToolStripSplitButtonEx btn1 = elementToControls[element] as Controls.ToolStripSplitButtonEx;
                        if (btn1 != null)
                            btn1.Checked = true;
                    }
                }
            }
        }
        #endregion

        #region ..ResetOperatorButton
        void ResetOperatorButton()
        {
            if(this.configDocument != null)
            ResetEnumControl(this.configDocument.DocumentElement, "*[@behavior='CurrentOperator']");
        }
        #endregion

        #region ..ResetEnumControl
        void ResetEnumControl(XmlElement rootElement, string xpath)
        {
            if (rootElement != null)
            {
                XmlNodeList elements = rootElement.SelectNodes(xpath);
                foreach (XmlElement element in elements)
                {
                    ToolStripButton btn = elementToControls[element] as ToolStripButton;
                    if (btn != null)
                        btn.Checked = false;
                    else
                    {
                        Controls.ToolStripSplitButtonEx btn1 = elementToControls[element] as Controls.ToolStripSplitButtonEx;
                        if (btn1 != null)
                            btn1.Checked = false;
                    }
                }
            }
        }
        #endregion

        #region ..vectorControl_ScaleChanged,缩放比例
        void canvas_ScaleChanged(object sender, EventArgs e)
        {
            float scale = this.canvas.ScaleRatio;
            bool old = this.inSync;
            this.inSync = true;
            if (configDocument != null && configDocument.DocumentElement != null)
            {
                System.Xml.XmlElement element = configDocument.DocumentElement.SelectSingleNode("*[@behavior='ScaleRatio']") as XmlElement;
                if (element != null && elementToControls.ContainsKey(element))
                {
                    object c = this.elementToControls[element];
                    if (c == null)
                        return;
                    Type type = this.GetControlType(element);
                    if (type != null)
                    {
                        string valueType = element.GetAttribute("valuetype");
                        if (valueType == "property")
                        {
                            PropertyInfo info = type.GetProperty(element.GetAttribute("value"));
                            if (info != null)
                                info.SetValue(c, scale,null);
                        }
                    }
                }
            }
            this.inSync = old;
        }
        #endregion

        #region ..GetTag
        /// <summary>
        /// 获取对象对应的Tag属性
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        object GetTag(object element)
        {
            if (element == null)
                return null;
            PropertyInfo info= element.GetType().GetProperty("Tag");
            if (info != null)
                return info.GetValue(element, null);
            return null;
        }
        #endregion

        #region ..SynSetting
        void SynSetting()
        {
            if (this.Canvas == null)
                return;

            this.inSync = true;
            try
            {
                //Fill
                if (this.configDocument != null)
                {
                    this.SynSubSetting("Fill");
                    this.SynSubSetting("Stroke");
                    this.SynSubSetting("TextStyle");
                    this.SynSubSetting("StartArrow");
                    this.SynSubSetting("EndArrow");
                    this.SynSubSetting("TextBlockStyle");
                    this.SynSubSetting("RadiusOfRectangle");
                    this.SynSubSetting("Star");
                }
            }
            finally
            {
                this.inSync = false;
            }
        }
        #endregion

        #region ..SynSubSetting
        void SynSubSetting(string style)
        {
            XmlNodeList nodes = this.configDocument.DocumentElement.SelectNodes("//*[@behavior='" + style + "']");
            foreach (XmlElement element in nodes)
            {
                //取得VectorControl对应值
                object pValue = this.GetPropertyValueOfVectorConrol(element);
                this.SetControlValue(element, pValue);
            }
        }
        #endregion

        #region ..GetPropertyValueOfVectorConrol
        /// <summary>
        /// 根据配置节点，取得节点所定义的属性值
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        object GetPropertyValueOfVectorConrol(XmlElement element)
        {
            Type type = this.Canvas.GetType();
            if (element.HasAttribute("behavior"))
            {
                string behavior = element.GetAttribute("behavior");
                PropertyInfo vectorControlProperty = type.GetProperty(behavior);
                if (vectorControlProperty != null)
                {
                    object result = vectorControlProperty.GetValue(this.canvas, null);
                    if (result != null && element.HasAttribute("target"))
                    {
                        PropertyInfo pInfo = result.GetType().GetProperty(element.GetAttribute("target"));
                        if (pInfo != null)
                            result = pInfo.GetValue(result, null);
                    }
                    return result;
                }
            }
            else
            {
                XmlElement child = element.SelectSingleNode("*[@behavior]") as XmlElement;
                if (child != null)
                    return GetPropertyValueOfVectorConrol(child);
            }
            return null;
        }
        #endregion

        #region ..SetControlValue
        /// <summary>
        /// 根据指定的配置节点，设置对应组件的对应属性值
        /// </summary>
        /// <param name="cfgElement"></param>
        /// <param name="value"></param>
        void SetControlValue(XmlElement cfgElement, object value)
        {
            if (cfgElement != null && this.elementToControls.ContainsKey(cfgElement))
            {
                object control = this.elementToControls[cfgElement];
                if (control != null)
                {
                    string value1 = cfgElement.GetAttribute("value");
                    string valueType = cfgElement.GetAttribute("valuetype");
                    
                    //如果是静态值
                    if (valueType =="const")
                    {
                        string controlproperty = cfgElement.GetAttribute("controlproperty");
                        PropertyInfo pInfo = control.GetType().GetProperty(controlproperty);
                        if (pInfo != null && pInfo.PropertyType == typeof(Boolean))
                            SetPropertyValue(control, pInfo, value.ToString() == cfgElement.GetAttribute("value"));
                    }
                    else if(valueType == "property")
                    {
                        PropertyInfo pInfo = control.GetType().GetProperty(value1);
                        if (pInfo != null)
                            SetPropertyValue(control, pInfo, value);
                    }
                }
            }
        }
        #endregion
    }
}
