using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace YP.CommonControl.Menu
{
    internal class ShortCutHelper
    {
        #region ..GetShortCutText
        /// <summary>
        /// get the display string for the short cut 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetShortCutText(Keys key)
        {
            char keycode = (char)((int)key & 0x0000FFFF);

            if ((keycode >= '0') && (keycode <= '9'))
            {
                string display = "";
                int modifier = (int)((int)key & 0xFFFF0000);
                if ((modifier & 0x00010000) != 0) display += "Shift+";
                if ((modifier & 0x00020000) != 0) display += "Ctrl+";
                if ((modifier & 0x00040000) != 0) display += "Alt+";
                display += keycode;
                return display;
            }
            else
                return TypeDescriptor.GetConverter(typeof(Keys)).ConvertToString(key);
        }
        #endregion
    }
}
