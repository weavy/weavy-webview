using System;
using System.Collections.Generic;
using System.Text;

namespace Weavy.WebView.Plugin.Forms.Models
{
    public class BadgeEventArgs : EventArgs
    {
        public int Number { get; set; }

    }

    public class Message
    {
        /// <summary>
        /// The action to call
        /// </summary>

        public string a { get; set; }

        /// <summary>
        /// Arguments
        /// </summary>

        public object d { get; set; }

        /// <summary>
        /// The callback 
        /// </summary>

        public string c { get; set; }
    }
}
