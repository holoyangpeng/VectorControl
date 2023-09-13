using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace YP.CommonControl.Menu
{
    /// <summary>
    /// define the menu item so that it can support theme
    /// also it provide a factory to create the menu item
    /// </summary>
    public class MenuItemFactory
    {
        #region ..MenuItemFactory
        /// <summary>
        /// using the text to create an instance
        /// </summary>
        /// <param name="text"></param>
        public static ToolStripItem CreateMenuItem(string text)
        {
           return CreateMenuItem(text,null,0,Shortcut.None,null);
        }

        /// <summary>
        /// using the text to create an instance
        /// </summary>
        /// <param name="text">the text of them item</param>
        /// <param name="clickHandler">the click handler</param>
        public static ToolStripItem CreateMenuItem(string text,EventHandler clickHandler)
        {
             return CreateMenuItem(text, null, 0, Shortcut.None, clickHandler);
        }

        /// <summary>
        /// using the text to create an instance
        /// </summary>
        /// <param name="text">the text of them item</param>
        /// <param name="clickHandler">the click handler</param>
        public static ToolStripItem CreateMenuItem(string text, EventHandler clickHandler,EventHandler updateHandler)
        {
            return CreateMenuItem(text, null, 0, Shortcut.None, clickHandler, updateHandler);
        }


        /// <summary>
        /// using the param to create a new command
        /// </summary>
        /// <param name="text">the text of the command</param>
        /// <param name="imageList">the image list</param>
        /// <param name="imageIndex">the index of the image</param>
        /// <param name="clickHandler">the click hander</param>
        public static ToolStripItem CreateMenuItem(string text, ImageList imageList, int imageIndex, EventHandler clickHandler)
        {
            return CreateMenuItem(text, imageList, imageIndex, Shortcut.None, clickHandler, null);
        }

        /// <summary>
        /// using the param to create a new command
        /// </summary>
        /// <param name="text">the text of the command</param>
        /// <param name="imageList">the image list</param>
        /// <param name="imageIndex">the index of the image</param>
        /// <param name="shortcut">short cut</param>
        /// <param name="clickHandler">the click hander</param>
        public static ToolStripItem CreateMenuItem(string text, ImageList imageList, int imageIndex, Shortcut shortcut,EventHandler clickHandler)
        {
            return CreateMenuItem(text, imageList, imageIndex, shortcut, clickHandler, null);
        }

        /// <summary>
        /// using the param to create a new command
        /// </summary>
        /// <param name="text">the text of the command</param>
        /// <param name="imageList">the image list</param>
        /// <param name="imageIndex">the index of the image</param>
        /// <param name="shortcut">short cut</param>
        /// <param name="clickHandler">the click hander</param>
        /// <param name="updateHander">the update hander</param>
        public static ToolStripItem CreateMenuItem(string text, ImageList imageList, int imageIndex, Shortcut shortcut, EventHandler clickHandler,EventHandler updateHandler)
        {
            //if text == "-" ,createa a separator
            if (text == "-")
            {
                MenuItemSeparator m = new MenuItemSeparator();
                m.Update += updateHandler;
                return m;
            }
            MenuCommand cmd = new MenuCommand();
            cmd.Text = text;
            if (imageList != null && imageIndex >= 0 && imageIndex < imageList.Images.Count)
                cmd.Image = imageList.Images[imageIndex];
            cmd.Click += clickHandler;
            cmd.Update += updateHandler;
            cmd.ShortcutKeys = (Keys)shortcut;
            return cmd;
        }

        /// <summary>
		/// 根据指定的节点信息创建菜单命令
		/// </summary>
		/// <param name="element"></param>
		public static ToolStripItem CreateMenuItem(System.Xml.XmlElement element,EventHandler clickHandler,EventHandler updateHandler,System.Windows.Forms.ImageList images)
		{
			string name = element.GetAttribute("Label").Trim();
			string key = element.GetAttribute("Key").Trim();
			string shortcut = element.GetAttribute("ShortCut").Trim();
			if(key != string.Empty && key != null)
				name = name+"(&"+key+")";
			Shortcut s = Shortcut.None;
			int imageindex = -1;
			if(shortcut.Length > 0)
			{
				try
				{
					object cut = System.Enum.Parse(typeof(System.Windows.Forms.Shortcut),shortcut,true);
					if(cut is System.Windows.Forms.Shortcut)
						s = (System.Windows.Forms.Shortcut)cut;
				}
				catch
				{
				}
			}
			string image = element.GetAttribute("ImageIndex").Trim();
			if(image.Length > 0)
			{
				try
				{
					imageindex = int.Parse(image,System.Globalization.NumberStyles.Any);
				}
				catch
				{
				}
			}
            ToolStripItem item = CreateMenuItem(name, images, imageindex, s, clickHandler, updateHandler);
            
            //if item is menu command, parse it's child
            MenuCommand cmd = item as MenuCommand;
            if (cmd != null)
            {
                foreach (System.Xml.XmlNode child in element.ChildNodes)
                {
                    if (child is System.Xml.XmlElement)
                    {
                        cmd.Items.Add(CreateMenuItem((System.Xml.XmlElement)child, clickHandler, updateHandler, images));
                    }
                }
            }
			item.Tag = element;
            return item;
        }
        #endregion

        #region ..CreateItems
        /// <summary>
        /// create the items from the stream
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static ToolStripItem[] CreateItems(System.IO.Stream stream,EventHandler clickHandler, EventHandler updateHandler,System.Windows.Forms.ImageList images)
        {
            System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader(stream);
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            reader.XmlResolver = null;
            doc.XmlResolver = null;
            doc.PreserveWhitespace = false;
            doc.Load(reader);
            reader.Close();
            System.Xml.XmlElement root = doc.DocumentElement;
            System.Collections.ArrayList list = new System.Collections.ArrayList();
            for (int i = 0; i < root.ChildNodes.Count; i++)//(System.Xml.XmlNode child in root.ChildNodes)
            {
                System.Xml.XmlNode child = root.ChildNodes[i];
                if (child is System.Xml.XmlElement)
                {
                    list.Add(MenuItemFactory.CreateMenuItem((System.Xml.XmlElement)child, clickHandler, updateHandler, images));
                }
            }

            ToolStripItem[] items = new ToolStripItem[list.Count];
            list.CopyTo(items);
            return items;
        }

        /// <summary>
        /// create the items from the resource file
        /// </summary>
        /// <param name="resourceFileName"></param>
        /// <param name="clickHandler"></param>
        /// <param name="updateHandler"></param>
        /// <param name="images"></param>
        /// <returns></returns>
        public static ToolStripItem[] CreateItems(string resourceFileName, EventHandler clickHandler, EventHandler updateHandler, System.Windows.Forms.ImageList images)
        {
            System.IO.Stream iconStream = Common.ResourceHelper.GetFileStream(resourceFileName);
            try
            {
                if (iconStream != null)
                {
                    return MenuItemFactory.CreateItems(iconStream, clickHandler,updateHandler,images);
                }
            }
            catch
            {
            }
            finally
            {
                if (iconStream != null)
                    iconStream.Close();
            }

            return null;
        }
        #endregion

    }
}
