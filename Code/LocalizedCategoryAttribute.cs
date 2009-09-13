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
                    return Localization.Translate(value);
        }
       
    }

}
