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
                    this.DisplayNameValue = Localization.Translate(base.DisplayName);
                    m_isLocalized = true;
                }
                return base.DisplayName;
                
            }
        }
    }
    
}
