using System;

namespace Weavy.WebView.Plugin.Forms.Models
{
    /// <summary>
    /// Badge event args
    /// </summary>
    public class BadgeEventArgs : EventArgs
    {
        public int Conversations { get; set; }
        public int Notifications { get; set; }
    }

    /// <summary>
    /// Sign in/out event args
    /// </summary>
    public class AuthenticationEventArgs : EventArgs
    {
        public string Status { get; set; }
    }

    /// <summary>
    /// Message object for JS bridge
    /// </summary>
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
