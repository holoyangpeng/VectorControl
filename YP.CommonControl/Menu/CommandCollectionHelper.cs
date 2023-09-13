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
        /// ������������
        /// </summary>
        /// <param name="owner">the parent tool strip you want to add the items to</param>
        /// <param name="stream">���������xml�ĵ�������</param>
        /// <param name="clickHandler">����˵���ѡ��</param>
        /// <param name="updateHandler">����˵�����ʱ���¼�</param>
        /// <param name="images">�˵���Images</param>
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
