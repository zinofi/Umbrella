using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Umbrella.N2.BaseModels
{
    /// <summary>
    /// Contains a default list of tab names for use inside N2 edit mode. Can be extended by inheriting from this class.
    /// </summary>
    public abstract class ContainerNames
    {
        public const string Metadata = "Metadata";
        public const string Content = "Content";
        public const string Site = "Site";
        public const string Advanced = "Advanced";
        public const string Dynamic = "Dynamic";
        public const string Header = "Header";
        public const string Footer = "Footer";
        public const string Social = "Social";
        public const string Facebook = "Facebook";
    }
}