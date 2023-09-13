using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace YP.CommonControl.Menu
{
    /// <summary>
    /// define some methods to create the item collection
    /// </summary>
    public class CommandCollectionHelper
    {
        #region ..ToolStripItemCollection
        /// <summary>
        /// 从流创建命令
        /// </summary>
        /// <param name="owner">the parent tool strip you want to add the items to</param>
        /// <param name="stream">定义命令集的xml文档数据流</param>
        /// <param name="clickHandler">处理菜单的选择</param>
        /// <param name="updateHandler">处理菜单弹出时的事件</param>
        /// <param name="images">菜单的Images</param>
        public static void CreateCommandsFromStream(IItemsContainer owner, System.IO.Stream stream, EventHandler clickHandler, System.EventHandler updateHandler, System.Windows.Forms.ImageList images)
        {
            try
            {
                System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(stream);
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                reader.XmlResolver = null;
                doc.XmlResolver = null;
                doc.PreserveWhitespace = false;
                doc.Load(reader);
                reader.Close();
                System.Xml.XmlElement root = doc.DocumentElement;
                foreach (System.Xml.XmlNode child in root.ChildNodes)
                {
                    if (child is System.Xml.XmlElement)
                    {
                        owner.Items.Add(Menu.MenuItemFactory.CreateMenuItem((System.Xml.XmlElement)child, clickHandler, updateHandler, images));
                    }
                }
            }
            catch(System.Exception e1)
            {
                System.Diagnostics.Debug.Assert(true, e1.Message);
            }
        }
        #endregion
    }
}
