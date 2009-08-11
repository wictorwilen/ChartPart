/*
 * 
 * ChartPart for SharePoint
 * ------------------------------------------
 * Copyright (c) 2008-2009, Wictor Wilén
 * http://www.codeplex.com/ChartPart/
 * http://www.wictorwilen.se/
 * ------------------------------------------
 * Licensed under the Microsoft Public License (Ms-PL) 
 * http://www.opensource.org/licenses/ms-pl.html
 * 
 */
using System;
using System.Web.UI.WebControls.WebParts;


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
                    base.DescriptionValue = Localization.Translate(base.Description);
                    m_isLocalized = true;
                }
                return base.Description;
            }
        }
    }
}
