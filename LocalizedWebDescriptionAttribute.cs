using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using ChartPart.Properties;

namespace ChartPart {
    /// <summary>
    /// A Localized version of the WebDescriptionAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false,Inherited = true)]
    public class LocalizedWebDescriptionAttribute : WebDescriptionAttribute {
        bool m_isLocalized = false;

        /// <summary>
        /// Initializes a new instance of the LocalizedWebDescriptionAttribute class.
        /// </summary>
        public LocalizedWebDescriptionAttribute(string description): base(description) {
        }

        /// <summary>
        /// Overridden Description property, returns the correct localized string
        /// </summary>
        public override string Description {
            get {
                if (!m_isLocalized) {
                    base.DescriptionValue = Resources.ResourceManager.GetString(base.Description, Resources.Culture);
                    m_isLocalized = true;
                }
                return base.Description;
            }
        }
    }
}
