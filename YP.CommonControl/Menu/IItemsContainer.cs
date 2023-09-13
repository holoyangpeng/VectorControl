using System;
using System.Collections.Generic;
using System.Text;

namespace YP.CommonControl.Menu
{
    /// <summary>
    /// define the interface for the object which has the items collection
    /// </summary>
    public interface IItemsContainer
    {
        /// <summary>
        /// gets the sub items collection
        /// </summary>
        System.Windows.Forms.ToolStripItemCollection Items { get;}
    }
}
