using System;
using System.Collections.Generic;
using System.Text;

namespace YP.CommonControl.Dock
{
    /// <summary>
    /// define the interface which contains the group
    /// </summary>
    internal interface IGroupContainer
    {
        /// <summary>
        /// gets the child groups
        /// </summary>
        GroupCollection Groups { get;}
    }
}
