using System;
using System.Collections.Generic;

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
    /// Theming event args
    /// </summary>
    public class ThemingEventArgs : EventArgs
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
        public IEnumerable<string> Colors { get; set; }
    }

    /// <summary>
    /// Link clicked event args
    /// </summary>
    public class LinkEventArgs : EventArgs
    {
        public string Url { get; set; }
        
    }

    /// <summary>
    /// Authentication status enum
    /// </summary>
    public enum AuthenticationStatus
    {
        OK = 1, 
        NOTAUTHENTICATED = 2, 
        ERROR = 3
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

    /// <summary>
    /// User object
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string FullName { get; set; }
    }
}
