using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls.WebParts;
using ChartPart.Properties;
using System.ComponentModel;

namespace ChartPart {
    /// <summary>
    /// Locallized version of the CategoryAttribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class LocalizedCategoryAttribute: CategoryAttribute {
        /// <summary>
        /// Initializes a new instance of the LocalizedCategoryAttribute class.
        /// </summary>
        public LocalizedCategoryAttribute(string category)
            : base(category) {
        }


        protected override string GetLocalizedString( string value ) {
                    return Resources.ResourceManager.GetString(value, Resources.Culture);
        }
       
    }

}
