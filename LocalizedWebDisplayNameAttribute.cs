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
    public class LocalizedWebDisplayNameAttribute: WebDisplayNameAttribute {
        bool m_isLocalized = false;

        /// <summary>
        /// Initializes a new instance of the LocalizedWebDisplayNameAttribute class.
        /// </summary>
        public LocalizedWebDisplayNameAttribute(string description) :base(description)  {
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
