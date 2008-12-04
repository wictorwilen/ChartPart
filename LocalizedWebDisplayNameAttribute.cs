using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using ChartPart.Properties;

namespace ChartPart {
    /// <summary>
    /// Locallized version of the WebDisplayNameAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class LocalizedWebDisplayNameAttribute: WebDisplayNameAttribute {
        bool m_isLocalized ;

        /// <summary>
        /// Initializes a new instance of the LocalizedWebDisplayNameAttribute class.
        /// </summary>
        public LocalizedWebDisplayNameAttribute(string displayName)
            : base(displayName) {
        }

        /// <summary>
        /// Overridden DisplayName property, returns the correct localized string
        /// </summary>
        public override string DisplayName {
            get {
                if (!m_isLocalized) {
                    this.DisplayNameValue = Resources.ResourceManager.GetString(base.DisplayName, Resources.Culture);
                    m_isLocalized = true;
                }
                return base.DisplayName;
                
            }
        }
    }
    
}
